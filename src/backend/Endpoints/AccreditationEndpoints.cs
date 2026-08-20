using FifaPressApp.Api.Models;
using FifaPressApp.Api.Realtime;
using FifaPressApp.Api.Storage;
using FifaPressApp.Api.Validation;

namespace FifaPressApp.Api.Endpoints;

/// <summary>
/// What a caller sends to create or replace an accreditation record.
///
/// <para>
/// A separate type from <see cref="AccreditationRecord"/> deliberately. The
/// stored record carries <c>LastSyncedUtc</c>, which is the server's statement
/// about when it last reconciled with the accreditation system — not something
/// a client gets to assert. Accepting the stored type directly would let any
/// caller declare its own data fresh, which is exactly the lie the staleness
/// indicator on the frontend exists to prevent.
/// </para>
///
/// <para>
/// Every property is nullable so that "absent" and "empty" stay distinguishable
/// and validation can report a missing field as missing rather than as blank.
/// </para>
/// </summary>
public sealed record AccreditationInput
{
    public string? CredentialId { get; init; }
    public string? HolderName { get; init; }
    public string? Outlet { get; init; }
    public string? TrackId { get; init; }
    public bool? HasNamedContact { get; init; }
    public string? Status { get; init; }
    public DateTime? ValidUntil { get; init; }
    public IReadOnlyList<string>? ZoneAccess { get; init; }
}

/// <summary>What a caller sends to append a change to a record.</summary>
public sealed record ChangeInput
{
    public string? ChangeId { get; init; }
    public DateTime? WrittenUtc { get; init; }
    public DateTime? EffectiveUtc { get; init; }
    public string? Kind { get; init; }
    public LocalizedText? WhatChanged { get; init; }
    public LocalizedText? Reason { get; init; }
    public LocalizedText? NextStep { get; init; }
    public bool? NextStepIsActionable { get; init; }
    public LocalizedText? DecidedBy { get; init; }
    public string? SupersedesChangeId { get; init; }
    public int? AffectsMatchNumber { get; init; }
    public int? DependsOnMatchNumber { get; init; }
    public LocalizedText? ConditionText { get; init; }
}

/// <summary>
/// The API's routes.
///
/// <para>
/// <b>Two resources, and they are shaped differently on purpose.</b> An
/// accreditation record has the full CRUD set the source document names:
/// list, read, create, replace, remove. Its change log has only list and
/// append. There is no route that edits or deletes a change, because a record
/// whose history can be rewritten cannot be trusted to say what happened — that
/// is the concept the whole project is built on, and it would be odd to build a
/// server that undoes it.
/// </para>
/// </summary>
public static class AccreditationEndpoints
{
    public static void MapAccreditationEndpoints(this WebApplication app)
    {
        var records = app.MapGroup("/api/accreditations");

        // ------------------------------------------------------------ read

        records.MapGet("/", (AccreditationStore store) => Results.Ok(store.All()));

        records.MapGet("/{credentialId}", (string credentialId, AccreditationStore store) =>
        {
            var record = store.Find(credentialId);

            // 404 rather than an empty 200. A caller that asked for one
            // specific credential and got back "nothing" needs to be able to
            // tell "no such record" from "a record with no data in it".
            return record is null
                ? NotFound(credentialId)
                : Results.Ok(record);
        });

        records.MapGet("/{credentialId}/changes", (string credentialId, AccreditationStore store) =>
            store.Find(credentialId) is null
                ? NotFound(credentialId)
                : Results.Ok(store.ChangesFor(credentialId)));

        // ----------------------------------------------------------- write

        records.MapPost("/", (AccreditationInput input, AccreditationStore store) =>
        {
            var validation = InputValidator.Validate(input, input.CredentialId ?? string.Empty);
            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var record = ToRecord(input, input.CredentialId!);

            // 409 rather than a silent overwrite: POST creates, and a POST to an
            // id that already exists is a mistake worth telling the caller
            // about rather than quietly replacing somebody's accreditation.
            return store.Add(record)
                ? Results.Created($"/api/accreditations/{record.CredentialId}", record)
                : Results.Conflict(new
                {
                    error = $"An accreditation record with credential id '{record.CredentialId}' already exists.",
                });
        });

        records.MapPut("/{credentialId}", (string credentialId, AccreditationInput input, AccreditationStore store) =>
        {
            var validation = InputValidator.Validate(input, credentialId);
            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            // The route's id wins over the body's. A PUT names its target in the
            // URL, and letting the body rename the record would turn an update
            // into a move that no caller asked for.
            var record = ToRecord(input, credentialId);

            return store.Replace(record)
                ? Results.Ok(record)
                : NotFound(credentialId);
        });

        records.MapDelete("/{credentialId}", (string credentialId, AccreditationStore store) =>
            store.Remove(credentialId)
                ? Results.NoContent()
                : NotFound(credentialId));

        records.MapPost("/{credentialId}/changes", async (
            string credentialId,
            ChangeInput input,
            AccreditationStore store,
            ChangeNotifier notifier) =>
        {
            if (store.Find(credentialId) is null)
            {
                return NotFound(credentialId);
            }

            var validation = InputValidator.Validate(input);
            if (!validation.IsValid)
            {
                return ValidationFailed(validation);
            }

            var change = ToChange(input, credentialId);

            if (store.HasChange(change.ChangeId))
            {
                return Results.Conflict(new { error = $"A change with id '{change.ChangeId}' already exists." });
            }

            store.AppendChange(change);

            // Written first, announced second. If the broadcast were to fail,
            // the change is still in the record and the next read will find it
            // — whereas announcing before writing could tell a client to go and
            // look at something that is not there yet.
            await notifier.NotifyChangeRecordedAsync(credentialId, change.ChangeId);

            return Results.Created(
                $"/api/accreditations/{credentialId}/changes",
                change);
        });
    }

    /// <summary>
    /// One shape for every rejected request: a headline a human reads and a
    /// field-keyed map a client can act on. Consistent with the error shape the
    /// error-handling middleware returns, so a caller writes one parser rather
    /// than one per failure mode.
    /// </summary>
    private static IResult ValidationFailed(ValidationResult validation) =>
        Results.BadRequest(new
        {
            error = "Validation failed.",
            details = validation.Details,
        });

    /// <summary>
    /// One shape for every "no such record" answer, so a caller can parse the
    /// failure the same way wherever it came from.
    /// </summary>
    private static IResult NotFound(string credentialId) =>
        Results.NotFound(new { error = $"No accreditation record with credential id '{credentialId}'." });

    private static AccreditationRecord ToRecord(AccreditationInput input, string credentialId) => new()
    {
        CredentialId = credentialId,
        HolderName = input.HolderName ?? string.Empty,
        Outlet = input.Outlet ?? string.Empty,
        TrackId = Enum.Parse<TrackId>(input.TrackId!, ignoreCase: true),
        HasNamedContact = input.HasNamedContact ?? false,
        Status = Enum.Parse<AccreditationStatus>(input.Status!, ignoreCase: true),
        ValidUntil = input.ValidUntil,
        ZoneAccess = input.ZoneAccess ?? [],

        // The server's own statement about freshness, stamped here rather than
        // accepted from the caller. See AccreditationInput.
        LastSyncedUtc = DateTime.UtcNow,
    };

    private static ChangeRecord ToChange(ChangeInput input, string credentialId) => new()
    {
        ChangeId = input.ChangeId ?? string.Empty,
        CredentialId = credentialId,
        WrittenUtc = input.WrittenUtc ?? DateTime.UtcNow,
        EffectiveUtc = input.EffectiveUtc ?? DateTime.UtcNow,
        Kind = Enum.Parse<ChangeKind>(input.Kind!, ignoreCase: true),
        WhatChanged = input.WhatChanged!,
        Reason = input.Reason!,
        NextStep = input.NextStep!,
        NextStepIsActionable = input.NextStepIsActionable ?? true,
        DecidedBy = input.DecidedBy,
        SupersedesChangeId = input.SupersedesChangeId,
        AffectsMatchNumber = input.AffectsMatchNumber,
        DependsOnMatchNumber = input.DependsOnMatchNumber,
        ConditionText = input.ConditionText,
    };
}
