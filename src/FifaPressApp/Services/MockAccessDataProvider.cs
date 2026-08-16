using FifaPressApp.Models;

namespace FifaPressApp.Services;

/// <summary>
/// The in-memory implementation of <see cref="IAccessDataProvider"/>, and the
/// only one this version ships.
///
/// <para>
/// <b>This class owns the withholding rule, and that is its most important
/// job.</b> The schedule it reads is a completed tournament: every knockout row
/// already names two real teams, so a plain read of the file tells the app who
/// won every match before it is played. An app that knows the future has no
/// reason to warn anyone about anything, which would quietly invert the point
/// of the whole thing. So this class holds a simulated tournament instant, and
/// team names are attached to a fixture in exactly one method —
/// <see cref="Reveal"/> — which refuses to attach them to a fixture that has
/// not kicked off. Every read path goes through it. A caller cannot opt out,
/// because a caller is never handed anything else.
/// </para>
///
/// <para>
/// Everything here is mocked except the schedule itself, and the interface says
/// so on screen wherever a reader could mistake it for a live integration.
/// </para>
/// </summary>
public sealed class MockAccessDataProvider : IAccessDataProvider
{
    /// <summary>
    /// The credential this version reads. There is one holder, because this is
    /// a single-record surface.
    /// </summary>
    public const string DemoCredentialId = "MP-2026-04817";

    /// <summary>
    /// The simulated tournament instant: one minute after the last Round-of-32
    /// kickoff, and before the first Round-of-16 one. Every group and
    /// Round-of-32 fixture has therefore been played; nothing beyond it has.
    ///
    /// <para>
    /// The last Round-of-32 kickoff is match <b>87</b>, at 20:30 on 3 July —
    /// not match 88, which is the highest-numbered one but kicks off earlier
    /// that afternoon. Match numbers in this schedule run in broadcast order,
    /// not clock order, and picking the instant off the number rather than the
    /// time leaves two fixtures of that round unplayed, which in turn makes
    /// every elimination in the round underivable.
    /// </para>
    ///
    /// <para>
    /// A simulation device. Replacing it with the real clock is the single
    /// change that turns the withholding rule from a containment measure into
    /// a plain statement of fact.
    /// </para>
    /// </summary>
    private static readonly DateTime SimulatedNow = new(2026, 7, 3, 20, 31, 0, DateTimeKind.Utc);

    /// <summary>
    /// Deliberately behind <see cref="SimulatedNow"/>. The record was last
    /// synchronised a few hours ago, not this second, so the staleness this app
    /// is built to show is actually visible rather than always reading zero.
    /// </summary>
    private static readonly DateTime SeededLastSyncedUtc = new(2026, 7, 3, 17, 15, 0, DateTimeKind.Utc);

    /// <summary>
    /// How long the simulated write takes.
    ///
    /// <para>
    /// <b>On the write path only, and that asymmetry is the point.</b> Reads
    /// resolve from local state and must stay instant — the headline paints on
    /// the first render with no spinner in front of it, and adding delay to a
    /// read would take that away. A write is the one thing here that genuinely
    /// goes somewhere, so it is the one thing worth showing progress for.
    /// </para>
    ///
    /// <para>
    /// Without this the request form's Submitting state cannot be seen at all:
    /// an already-completed task lets the caller's continuation run before the
    /// framework ever renders, so the disabled fields and the "Sending request…"
    /// label the form already implements never reach the screen.
    /// </para>
    ///
    /// <para>
    /// A simulation device, with the same standing as <see cref="SimulatedNow"/>
    /// above. A provider talking to a real service deletes this and gets its
    /// latency from the network, which is where latency is supposed to come
    /// from.
    /// </para>
    /// </summary>
    private static readonly TimeSpan SimulatedWriteLatency = TimeSpan.FromMilliseconds(600);

    /// <summary>
    /// The order rounds are played in. Used only to ask "what comes after the
    /// deepest round this team reached", which is how elimination is inferred
    /// without reading a result. Third place is not on the path — losing a
    /// semi-final does not put a team out of the third-place match.
    /// </summary>
    private static readonly PhaseKind[] ProgressionPath =
    [
        PhaseKind.GroupStage,
        PhaseKind.RoundOf32,
        PhaseKind.RoundOf16,
        PhaseKind.QuarterFinals,
        PhaseKind.SemiFinals,
        PhaseKind.Final,
    ];

    private readonly HttpClient http;
    private readonly List<Change> changes;
    private readonly Accreditation accreditation;

    private Task<FixtureImportResult>? scheduleLoad;
    private FixtureImportResult? schedule;

    public MockAccessDataProvider(HttpClient http)
    {
        this.http = http;
        accreditation = SeedAccreditation();
        changes = SeedChanges(accreditation.Track);
    }

    /// <inheritdoc />
    public DateTime AsOfUtc => SimulatedNow;

    /// <summary>
    /// Whether the schedule failed to load. The fixture surfaces state this
    /// plainly rather than showing an empty list that looks like a tournament
    /// with no matches in it.
    /// </summary>
    public bool ScheduleUnavailable { get; private set; }

    // ---------------------------------------------------------------- reads

    /// <inheritdoc />
    public Task<AccessResponse<Accreditation?>> GetAccreditationAsync(string credentialId)
    {
        // Returns an already-completed task from local state. That is what
        // "cache first" buys: the caller has its answer before it ever yields,
        // so the headline paints on the first render with no spinner in front
        // of it and no network in the way.
        var record = credentialId == accreditation.CredentialId ? accreditation : null;
        return Task.FromResult(Cached<Accreditation?>(record));
    }

    /// <inheritdoc />
    public Task<AccessResponse<IReadOnlyList<Change>>> GetChangesAsync(string credentialId)
    {
        IReadOnlyList<Change> ordered = changes
            .Where(change => change.CredentialId == credentialId)

            // By when each change takes effect, not when it was written. A
            // change landing on Saturday belongs above one written later that
            // lands next month.
            .OrderByDescending(change => change.EffectiveUtc)
            .ToList();

        return Task.FromResult(Cached(ordered));
    }

    /// <inheritdoc />
    public async Task<AccessResponse<IReadOnlyList<Fixture>>> GetFixturesAsync()
    {
        var loaded = await LoadScheduleAsync();

        IReadOnlyList<Fixture> visible = loaded is null
            ? []
            : loaded.Fixtures.Select(fixture => Reveal(fixture, loaded)).ToList();

        return Cached(visible);
    }

    /// <inheritdoc />
    public async Task<AccessResponse<Fixture?>> GetFixtureAsync(int matchNumber)
    {
        var loaded = await LoadScheduleAsync();
        var match = loaded?.Fixtures.FirstOrDefault(fixture => fixture.MatchNumber == matchNumber);

        return Cached(match is null || loaded is null ? null : Reveal(match, loaded));
    }

    // --------------------------------------------------------------- writes

    /// <inheritdoc />
    public async Task<Change> RequestMatchAccessAsync(string credentialId, int matchNumber)
    {
        // The write takes a moment, and the caller's await genuinely yields
        // here. That is what makes the form's Submitting state reachable: a
        // render pass now happens between the button press and the navigation,
        // which is where the disabled fields and the "Sending request…" label
        // get their chance to appear.
        await Task.Delay(SimulatedWriteLatency);

        // The change is the outcome. There is no separate "it worked" to hand
        // back, and handing back a bare success flag would let access move with
        // nothing written down — the one thing this record exists to stop.
        var written = new Change(
            changeId: NextChangeId(),
            credentialId: credentialId,
            writtenUtc: SimulatedNow,
            effectiveUtc: SimulatedNow,
            kind: ChangeKind.RequestDecided,
            track: accreditation.Track,
            whatChanged: $"Access to match {matchNumber} is now recorded as requested.",
            reason: "You submitted a request from the match page. It is written to your record "
                  + "before any decision is taken, so a request in progress is never invisible.",
            nextStep: "Nothing to do now. When a decision is taken it appears here as its own "
                    + "change, with the reason attached.",
            nextStepIsActionable: false,
            decidedBy: "FIFA Event Media Operations (simulated — no request is actually sent)",
            affectsMatchNumber: matchNumber);

        changes.Add(written);
        return written;
    }

    /// <inheritdoc />
    public Task<Change> WithdrawRequestAsync(string credentialId, string changeId)
    {
        var target = changes.FirstOrDefault(
            change => change.ChangeId == changeId && change.CredentialId == credentialId)
            ?? throw new ArgumentException($"No change {changeId} exists on this record.", nameof(changeId));

        // A withdrawal appends; it does not unwrite. The change being withdrawn
        // stays exactly where it was, and this new one points at it.
        var written = new Change(
            changeId: NextChangeId(),
            credentialId: credentialId,
            writtenUtc: SimulatedNow,
            effectiveUtc: SimulatedNow,
            kind: ChangeKind.Withdrawal,
            track: accreditation.Track,
            whatChanged: target.AffectsMatchNumber is int match
                ? $"Your request for access to match {match} is withdrawn."
                : "Your request is withdrawn.",
            reason: "You withdrew this request yourself. The original request stays in the record "
                  + "below, because a record that erases what it replaces is not a record.",
            nextStep: "You can request access to this match again at any time before kickoff.",
            supersedesChangeId: target.ChangeId,
            affectsMatchNumber: target.AffectsMatchNumber);

        changes.Add(written);
        return Task.FromResult(written);
    }

    // ------------------------------------------------------------- derived

    /// <summary>
    /// Where access to one fixture currently stands, folded from the changes
    /// that name it. Never stored, so it cannot drift from the log it came
    /// from.
    /// </summary>
    public MatchAccessStatus GetMatchAccessStatus(string credentialId, int matchNumber)
    {
        var superseded = changes
            .Where(change => change.SupersedesChangeId is not null)
            .Select(change => change.SupersedesChangeId!)
            .ToHashSet(StringComparer.Ordinal);

        var latest = changes
            .Where(change => change.CredentialId == credentialId
                          && change.AffectsMatchNumber == matchNumber
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

    /// <summary>
    /// Whether a team is out, inferred without reading a single result.
    ///
    /// <para>
    /// The schedule has no result columns, and the rows that would give the
    /// answer away are exactly the rows this class refuses to read. So
    /// elimination is derived from one rule instead: a team is out once every
    /// fixture of the round after its own has kicked off and the team is in
    /// none of them. That consults resolved rows only. If the next round has
    /// not started, the honest answer is "not known yet", and this returns
    /// false rather than guessing.
    /// </para>
    ///
    /// <para>
    /// MOCKED INFERENCE, not a result feed. A real system is told who lost.
    /// </para>
    /// </summary>
    public bool IsTeamEliminated(string team)
    {
        if (schedule is null || string.IsNullOrWhiteSpace(team))
        {
            return false;
        }

        var resolved = schedule.Fixtures.Where(IsResolved).ToList();

        // The deepest round this team is known to have reached — known, because
        // only kicked-off fixtures are consulted.
        var deepest = -1;
        foreach (var fixture in resolved)
        {
            if (!schedule.Matchups.TryGetValue(fixture.MatchNumber, out var matchup))
            {
                continue;
            }

            if (!Names(matchup, team))
            {
                continue;
            }

            var index = Array.IndexOf(ProgressionPath, fixture.Phase);
            if (index > deepest)
            {
                deepest = index;
            }
        }

        if (deepest < 0 || deepest == ProgressionPath.Length - 1)
        {
            return false;
        }

        var nextRound = ProgressionPath[deepest + 1];
        var nextRoundFixtures = schedule.Fixtures.Where(fixture => fixture.Phase == nextRound).ToList();

        // If any fixture of the next round is still to be played, the team's
        // fate is genuinely unknown and saying otherwise would be reading ahead.
        if (nextRoundFixtures.Count == 0 || !nextRoundFixtures.All(IsResolved))
        {
            return false;
        }

        return !nextRoundFixtures.Any(fixture =>
            schedule.Matchups.TryGetValue(fixture.MatchNumber, out var matchup) && Names(matchup, team));
    }

    // ------------------------------------------------------------ internals

    /// <summary>
    /// The one place a team name is ever attached to a fixture. A fixture whose
    /// kickoff is after the simulated instant is returned exactly as it came
    /// out of the importer: no names on it, and none available to put there.
    /// </summary>
    private Fixture Reveal(Fixture fixture, FixtureImportResult loaded)
    {
        if (!IsResolved(fixture))
        {
            return fixture;
        }

        if (!loaded.Matchups.TryGetValue(fixture.MatchNumber, out var matchup))
        {
            return fixture with { IsResolved = true };
        }

        return fixture with
        {
            IsResolved = true,
            HomeLabel = matchup.Home,
            AwayLabel = matchup.Away,
        };
    }

    private static bool Names(Matchup matchup, string team) =>
        string.Equals(matchup.Home, team, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(matchup.Away, team, StringComparison.OrdinalIgnoreCase);

    private bool IsResolved(Fixture fixture) => fixture.KickoffLocal <= SimulatedNow;

    private static AccessResponse<T> Cached<T>(T value) =>
        new(value, SeededLastSyncedUtc, WasServedFromCache: true);

    private string NextChangeId() => $"ch-{changes.Count + 1:D3}";

    private async Task<FixtureImportResult?> LoadScheduleAsync()
    {
        if (schedule is not null)
        {
            return schedule;
        }

        try
        {
            scheduleLoad ??= FetchScheduleAsync();
            schedule = await scheduleLoad;
            ScheduleUnavailable = false;
            return schedule;
        }
        catch (Exception)
        {
            // A schedule that will not load is a stated condition, not a crash:
            // the fixture surfaces say so, and the access record — which does
            // not depend on the schedule — carries on working.
            scheduleLoad = null;
            ScheduleUnavailable = true;
            return null;
        }
    }

    private async Task<FixtureImportResult> FetchScheduleAsync()
    {
        var csv = await http.GetStringAsync("data/2026_World_Cup_Schedule.csv");
        return FixtureImporter.Parse(csv);
    }

    // ----------------------------------------------------------- mock data

    private static Accreditation SeedAccreditation() => new()
    {
        CredentialId = DemoCredentialId,
        HolderName = "Amina Bello",
        Outlet = "The National Daily",
        Track = new Track(TrackId.MemberAssociationQuota, HasNamedContact: false),
        Status = AccreditationStatus.Approved,
        ValidUntil = new DateTime(2026, 7, 19, 23, 59, 0, DateTimeKind.Utc),
        ZoneAccess = ["Media tribune", "Mixed zone", "Press conference room"],
        LastSyncedUtc = SeededLastSyncedUtc,
    };

    private static List<Change> SeedChanges(Track track) =>
    [
        new Change(
            changeId: "ch-001",
            credentialId: DemoCredentialId,
            writtenUtc: new DateTime(2026, 6, 5, 10, 0, 0, DateTimeKind.Utc),
            effectiveUtc: new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc),
            kind: ChangeKind.MatchAccessGranted,
            track: track,
            whatChanged: "Match access granted for your group-stage allocation.",
            reason: "Your member association confirmed its opening quota split, and your name was "
                  + "on it.",
            nextStep: "Collect your credential at the accreditation centre before your first match.",
            affectsMatchNumber: 1),

        new Change(
            changeId: "ch-002",
            credentialId: DemoCredentialId,
            writtenUtc: new DateTime(2026, 6, 28, 8, 30, 0, DateTimeKind.Utc),
            effectiveUtc: new DateTime(2026, 6, 29, 12, 0, 0, DateTimeKind.Utc),
            kind: ChangeKind.ZoneAccessNarrowed,
            track: track,
            whatChanged: "Pitch-side photo position removed from your zone access.",
            reason: "The host city reduced the photo pool for knockout fixtures after the venue "
                  + "operator revised the perimeter plan.",
            nextStep: "Apply for a pool position through your member association if you need one "
                    + "for a specific match."),

        new Change(
            changeId: "ch-003",
            credentialId: DemoCredentialId,
            writtenUtc: new DateTime(2026, 6, 30, 16, 15, 0, DateTimeKind.Utc),
            effectiveUtc: new DateTime(2026, 6, 30, 16, 15, 0, DateTimeKind.Utc),
            kind: ChangeKind.AdministrativeCorrection,
            track: track,
            whatChanged: "Mixed zone access restored to your credential.",
            reason: "Mixed zone was withdrawn together with the photo position two days ago. That "
                  + "was a clerical error in the perimeter revision; only the photo position was "
                  + "meant to move.",
            nextStep: "Nothing to do. Your credential already carries mixed zone access again.",
            nextStepIsActionable: false,
            decidedBy: "Host city accreditation office",
            supersedesChangeId: "ch-002"),

        new Change(
            changeId: "ch-004",
            credentialId: DemoCredentialId,
            writtenUtc: new DateTime(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc),
            effectiveUtc: new DateTime(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc),
            kind: ChangeKind.ZoneAccessWidened,
            track: track,
            whatChanged: "Press conference room added for the remainder of the tournament.",
            reason: "Your member association's knockout allocation includes conference access, "
                  + "which your original credential did not carry.",
            nextStep: "Nothing to do. The access is already on your credential."),

        new Change(
            changeId: "ch-005",
            credentialId: DemoCredentialId,
            writtenUtc: new DateTime(2026, 7, 3, 11, 20, 0, DateTimeKind.Utc),
            effectiveUtc: new DateTime(2026, 7, 6, 14, 0, 0, DateTimeKind.Utc),
            kind: ChangeKind.MatchAccessRevoked,
            track: track,
            whatChanged: "Your quarter-final allocation depends on the Round of 16 fixture in "
                       + "Dallas on 6 July.",
            reason: "Your member association's quarter-final quota is set by how far its team "
                  + "goes. The quota contracts as soon as that fixture removes it from the "
                  + "tournament.",
            nextStep: "Hold your Dallas travel until after that fixture, or book something you "
                    + "can change. You will not need to re-apply either way.",
            affectsMatchNumber: 98,
            dependsOnMatchNumber: 93,
            conditionText: "If the association's team is eliminated in that fixture, your "
                         + "quarter-final access is withdrawn and your accreditation continues "
                         + "unchanged for every other match. If it is not, your quarter-final "
                         + "access stands as it is now."),
    ];
}
