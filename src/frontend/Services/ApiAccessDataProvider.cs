using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FifaPressApp.Models;

namespace FifaPressApp.Services;

/// <summary>
/// The implementation of <see cref="IAccessDataProvider"/> that reads the
/// access record from the backend API instead of from memory.
///
/// <para>
/// <b>This class is the entire point of the backend run, and what matters most
/// about it is what it did NOT require.</b> Not one page, component, stylesheet
/// or route changed to make the app read from a real service. Every screen asks
/// for <see cref="IAccessDataProvider"/> and never names a concrete provider,
/// so the swap happens on one line of <c>Program.cs</c>. That was the promise
/// the interface was written to keep, several runs before anything existed to
/// keep it against.
/// </para>
///
/// <para>
/// <b>It is not registered by default.</b> The mock stays in place unless an API
/// base URL is present in configuration. A deployed site with nothing
/// configured — or one whose API is asleep on a free hosting tier — behaves
/// exactly as it did before this class existed.
/// </para>
///
/// <para>
/// <b>The honest gap: fixtures do not come from the API.</b> The backend serves
/// accreditation records and their change log, and nothing else. The match
/// schedule is a tracked CSV that the frontend parses, and the rule that
/// withholds team names from a fixture nobody has played yet lives in
/// <see cref="MockAccessDataProvider"/> alongside it. So this class delegates
/// every fixture read to an inner provider rather than inventing endpoints for
/// data the API was never scoped to hold. Two consequences worth stating: the
/// withholding rule is still enforced, and a reader should not conclude from
/// "the app talks to an API" that everything on screen came from one.
/// </para>
/// </summary>
public sealed class ApiAccessDataProvider : IAccessDataProvider
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient http;
    private readonly IAccessDataProvider fixtures;

    /// <summary>
    /// What the API last said, kept so the synchronous parts of the interface
    /// can still answer.
    ///
    /// <para>
    /// <see cref="GetMatchAccessStatus"/> has no <c>await</c> available to it —
    /// it is called during a render pass, from a component that cannot yield —
    /// so the changes it folds have to already be in hand. This is the same
    /// cache-first rule the interface has always carried, with a network behind
    /// it now instead of a seeded list.
    /// </para>
    /// </summary>
    private readonly Dictionary<string, Accreditation> records = [];
    private readonly Dictionary<string, List<Change>> changes = [];

    public ApiAccessDataProvider(HttpClient http, IAccessDataProvider fixtures)
    {
        this.http = http;
        this.fixtures = fixtures;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Delegated. The simulated tournament instant is what makes the withholding
    /// rule work, and it belongs with the schedule reader rather than with the
    /// service that serves records.
    /// </remarks>
    public DateTime AsOfUtc => fixtures.AsOfUtc;

    /// <summary>
    /// Whether the last read failed. The surfaces that show a record already
    /// know how to say "this did not load" rather than rendering an empty
    /// record as though it were an accurate one.
    /// </summary>
    public bool LastReadFailed { get; private set; }

    // ---------------------------------------------------------------- reads

    /// <inheritdoc />
    public async Task<AccessResponse<Accreditation?>> GetAccreditationAsync(string credentialId)
    {
        var dto = await GetAsync<AccreditationDto>($"api/accreditations/{Uri.EscapeDataString(credentialId)}");

        if (dto is null)
        {
            // Fall back to whatever was last read successfully. A record that
            // was true a minute ago, clearly labelled with when it was last
            // synchronised, is more use to somebody standing at a gate than an
            // empty screen — and the staleness indicator the app already renders
            // is what keeps that honest.
            return new AccessResponse<Accreditation?>(
                records.GetValueOrDefault(credentialId),
                records.GetValueOrDefault(credentialId)?.LastSyncedUtc ?? DateTime.MinValue,
                WasServedFromCache: true);
        }

        var record = ToAccreditation(dto);
        records[credentialId] = record;

        return new AccessResponse<Accreditation?>(record, record.LastSyncedUtc, WasServedFromCache: false);
    }

    /// <inheritdoc />
    public async Task<AccessResponse<IReadOnlyList<Change>>> GetChangesAsync(string credentialId)
    {
        // The track has to be in hand before a change can be constructed at all:
        // urgency is derived from it, and the constructor derives it rather than
        // accepting one. This is deliberate — the API never sends an urgency, so
        // there is no value on the wire that could arrive disagreeing with the
        // track it was supposed to have come from.
        var track = await TrackForAsync(credentialId);

        var dtos = await GetAsync<List<ChangeDto>>(
            $"api/accreditations/{Uri.EscapeDataString(credentialId)}/changes");

        if (dtos is null || track is null)
        {
            IReadOnlyList<Change> cached = changes.GetValueOrDefault(credentialId) ?? [];
            return new AccessResponse<IReadOnlyList<Change>>(
                cached, records.GetValueOrDefault(credentialId)?.LastSyncedUtc ?? DateTime.MinValue,
                WasServedFromCache: true);
        }

        var built = dtos.Select(dto => ToChange(dto, track)).ToList();
        changes[credentialId] = built;

        IReadOnlyList<Change> ordered = built
            .OrderByDescending(change => change.EffectiveUtc)
            .ToList();

        return new AccessResponse<IReadOnlyList<Change>>(
            ordered,
            records.GetValueOrDefault(credentialId)?.LastSyncedUtc ?? DateTime.UtcNow,
            WasServedFromCache: false);
    }

    /// <inheritdoc />
    public Task<AccessResponse<IReadOnlyList<Fixture>>> GetFixturesAsync() => fixtures.GetFixturesAsync();

    /// <inheritdoc />
    public Task<AccessResponse<Fixture?>> GetFixtureAsync(int matchNumber) => fixtures.GetFixtureAsync(matchNumber);

    // --------------------------------------------------------------- writes

    /// <inheritdoc />
    public Task<Change> RequestMatchAccessAsync(string credentialId, int matchNumber) =>
        AppendAsync(credentialId, new ChangeDto
        {
            ChangeId = NextChangeId(credentialId),
            Kind = nameof(ChangeKind.RequestDecided),
            WhatChanged = Wire(ChangeTemplates.RequestWhatChanged(matchNumber)),
            Reason = Wire(ChangeTemplates.RequestReason),
            NextStep = Wire(ChangeTemplates.RequestNextStep),
            NextStepIsActionable = false,
            DecidedBy = Wire(ChangeTemplates.RequestDecidedBy),
            AffectsMatchNumber = matchNumber,
            EffectiveUtc = AsOfUtc,
            WrittenUtc = AsOfUtc,
        });

    /// <inheritdoc />
    public async Task<Change> WithdrawRequestAsync(string credentialId, string changeId)
    {
        var target = changes.GetValueOrDefault(credentialId)?
            .FirstOrDefault(change => change.ChangeId == changeId)
            ?? throw new ArgumentException($"No change {changeId} exists on this record.", nameof(changeId));

        // A withdrawal appends; it does not unwrite. Same rule the mock follows,
        // and the API has no route that could do otherwise even if this class
        // asked it to.
        return await AppendAsync(credentialId, new ChangeDto
        {
            ChangeId = NextChangeId(credentialId),
            Kind = nameof(ChangeKind.Withdrawal),
            WhatChanged = Wire(ChangeTemplates.WithdrawalWhatChanged(target.AffectsMatchNumber)),
            Reason = Wire(ChangeTemplates.WithdrawalReason),
            NextStep = Wire(ChangeTemplates.WithdrawalNextStep),
            SupersedesChangeId = target.ChangeId,
            AffectsMatchNumber = target.AffectsMatchNumber,
            EffectiveUtc = AsOfUtc,
            WrittenUtc = AsOfUtc,
        });
    }

    // -------------------------------------------------------------- derived

    /// <inheritdoc />
    /// <remarks>
    /// The identical fold the mock performs, over the cached changes. Kept
    /// byte-for-byte the same logic on purpose: a status that differed between
    /// the two providers would mean the screens behaved differently depending on
    /// where the data came from, which is exactly what swapping behind an
    /// interface is supposed to rule out.
    /// </remarks>
    public MatchAccessStatus GetMatchAccessStatus(string credentialId, int matchNumber)
    {
        var all = changes.GetValueOrDefault(credentialId) ?? [];

        var superseded = all
            .Where(change => change.SupersedesChangeId is not null)
            .Select(change => change.SupersedesChangeId!)
            .ToHashSet(StringComparer.Ordinal);

        var latest = all
            .Where(change => change.AffectsMatchNumber == matchNumber
                          && !superseded.Contains(change.ChangeId))
            .OrderByDescending(change => change.EffectiveUtc)
            .FirstOrDefault();

        return latest?.Kind switch
        {
            ChangeKind.MatchAccessGranted => MatchAccessStatus.Granted,
            ChangeKind.MatchAccessRevoked => MatchAccessStatus.Revoked,
            ChangeKind.RequestDecided => MatchAccessStatus.Requested,
            ChangeKind.Withdrawal => MatchAccessStatus.NotRequested,
            _ => MatchAccessStatus.NotRequested,
        };
    }

    // ------------------------------------------------------------- plumbing

    private async Task<T?> GetAsync<T>(string route)
        where T : class
    {
        try
        {
            var response = await http.GetAsync(route);
            LastReadFailed = !response.IsSuccessStatusCode;

            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<T>(Json)
                : null;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            // An unreachable or sleeping API is an expected state, not a crash.
            // Free hosting tiers stop the process when idle, so the first
            // request after a quiet period can simply time out.
            LastReadFailed = true;
            return null;
        }
    }

    private async Task<Change> AppendAsync(string credentialId, ChangeDto dto)
    {
        var response = await http.PostAsJsonAsync(
            $"api/accreditations/{Uri.EscapeDataString(credentialId)}/changes", dto, Json);

        response.EnsureSuccessStatusCode();

        var written = await response.Content.ReadFromJsonAsync<ChangeDto>(Json)
            ?? throw new InvalidOperationException("The API accepted the change but returned nothing.");

        var track = await TrackForAsync(credentialId)
            ?? throw new InvalidOperationException($"No record exists for credential {credentialId}.");

        var change = ToChange(written, track);

        // Kept locally too, so the synchronous status fold sees it without
        // waiting for the next read.
        if (!changes.TryGetValue(credentialId, out var list))
        {
            list = [];
            changes[credentialId] = list;
        }

        list.Add(change);
        return change;
    }

    private async Task<Track?> TrackForAsync(string credentialId)
    {
        if (records.TryGetValue(credentialId, out var known))
        {
            return known.Track;
        }

        var dto = await GetAsync<AccreditationDto>($"api/accreditations/{Uri.EscapeDataString(credentialId)}");
        if (dto is null)
        {
            return null;
        }

        var record = ToAccreditation(dto);
        records[credentialId] = record;
        return record.Track;
    }

    private string NextChangeId(string credentialId) =>
        $"ch-{credentialId}-{(changes.GetValueOrDefault(credentialId)?.Count ?? 0) + 1:D3}";

    private static Accreditation ToAccreditation(AccreditationDto dto) => new()
    {
        CredentialId = dto.CredentialId,
        HolderName = dto.HolderName,
        Outlet = dto.Outlet,
        Track = new Track(Enum.Parse<TrackId>(dto.TrackId, ignoreCase: true), dto.HasNamedContact),
        Status = Enum.Parse<AccreditationStatus>(dto.Status, ignoreCase: true),
        ValidUntil = dto.ValidUntil,
        ZoneAccess = dto.ZoneAccess,
        LastSyncedUtc = dto.LastSyncedUtc,
    };

    private static Change ToChange(ChangeDto dto, Track track) => new(
        changeId: dto.ChangeId,
        credentialId: dto.CredentialId ?? string.Empty,
        writtenUtc: dto.WrittenUtc ?? DateTime.UtcNow,
        effectiveUtc: dto.EffectiveUtc ?? DateTime.UtcNow,
        kind: Enum.Parse<ChangeKind>(dto.Kind, ignoreCase: true),
        track: track,
        whatChanged: Local(dto.WhatChanged),
        reason: Local(dto.Reason),
        nextStep: Local(dto.NextStep),
        nextStepIsActionable: dto.NextStepIsActionable,
        decidedBy: dto.DecidedBy is null ? null : Local(dto.DecidedBy),
        supersedesChangeId: dto.SupersedesChangeId,
        affectsMatchNumber: dto.AffectsMatchNumber,
        dependsOnMatchNumber: dto.DependsOnMatchNumber,
        conditionText: dto.ConditionText is null ? null : Local(dto.ConditionText));

    private static LocalizedText Local(LocalizedTextDto? dto) =>
        new(dto?.En ?? string.Empty, dto?.Es ?? string.Empty, dto?.Pt ?? string.Empty);

    private static LocalizedTextDto Wire(LocalizedText text) => new(text.En, text.Es, text.Pt);

    // The wire shapes, kept separate from the domain types on purpose: the
    // domain's Change validates itself in its constructor and cannot be
    // deserialized into directly, and a DTO that mirrors the API's JSON is
    // easier to read than attributes scattered across a model that has other
    // jobs.

    private sealed record LocalizedTextDto(string En, string Es, string Pt);

    private sealed record AccreditationDto
    {
        public string CredentialId { get; init; } = string.Empty;
        public string HolderName { get; init; } = string.Empty;
        public string Outlet { get; init; } = string.Empty;
        public string TrackId { get; init; } = nameof(Models.TrackId.Freelance);
        public bool HasNamedContact { get; init; }
        public string Status { get; init; } = nameof(AccreditationStatus.Pending);
        public DateTime? ValidUntil { get; init; }
        public IReadOnlyList<string> ZoneAccess { get; init; } = [];
        public DateTime LastSyncedUtc { get; init; }
    }

    private sealed record ChangeDto
    {
        public string ChangeId { get; init; } = string.Empty;
        public string? CredentialId { get; init; }
        public DateTime? WrittenUtc { get; init; }
        public DateTime? EffectiveUtc { get; init; }
        public string Kind { get; init; } = string.Empty;
        public LocalizedTextDto? WhatChanged { get; init; }
        public LocalizedTextDto? Reason { get; init; }
        public LocalizedTextDto? NextStep { get; init; }
        public bool NextStepIsActionable { get; init; } = true;
        public LocalizedTextDto? DecidedBy { get; init; }
        public string? SupersedesChangeId { get; init; }
        public int? AffectsMatchNumber { get; init; }
        public int? DependsOnMatchNumber { get; init; }
        public LocalizedTextDto? ConditionText { get; init; }
    }
}
