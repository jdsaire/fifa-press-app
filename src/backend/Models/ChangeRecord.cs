namespace FifaPressApp.Api.Models;

/// <summary>
/// What kind of state movement a change records. Mirrors the frontend's
/// <c>ChangeKind</c>.
/// </summary>
public enum ChangeKind
{
    MatchAccessGranted,
    MatchAccessRevoked,
    ZoneAccessNarrowed,
    ZoneAccessWidened,
    ValidityShortened,
    RequestDecided,
    AdministrativeCorrection,
    Withdrawal,
}

/// <summary>
/// One piece of authored content, in all three languages.
///
/// <para>
/// Authored three times, not translated once. A real accreditation system
/// authors the three languages at the source, and so does this one — there is
/// no translation call anywhere in this API and no language is derived from
/// another at request time.
/// </para>
/// </summary>
public sealed record LocalizedText(string En, string Es, string Pt);

/// <summary>
/// One movement in the access record: what changed, why, and what the holder
/// can do about it.
///
/// <para>
/// <b>Append-only, and the routing table enforces it.</b> There is no PUT and
/// no DELETE for a change anywhere in this API. That is a domain rule, not an
/// unfinished feature: a correction is a *new* change carrying
/// <see cref="SupersedesChangeId"/>, and a withdrawal is a *new* change of kind
/// <see cref="ChangeKind.Withdrawal"/>. Both leave the record they replace
/// intact and readable, because a record that can be edited after the fact
/// cannot be trusted to say what actually happened.
/// </para>
///
/// <para>
/// MOCKED. A real system emits these from the accreditation system at the
/// moment state actually moves.
/// </para>
/// </summary>
public sealed record ChangeRecord
{
    public required string ChangeId { get; init; }

    public required string CredentialId { get; init; }

    /// <summary>When this change was recorded.</summary>
    public required DateTime WrittenUtc { get; init; }

    /// <summary>
    /// When this change starts to matter. This — not <see cref="WrittenUtc"/> —
    /// is the ordering key, so a change resolving on Saturday sits above one
    /// written later that resolves next month.
    /// </summary>
    public required DateTime EffectiveUtc { get; init; }

    public required ChangeKind Kind { get; init; }

    public required LocalizedText WhatChanged { get; init; }

    public required LocalizedText Reason { get; init; }

    public required LocalizedText NextStep { get; init; }

    /// <summary>
    /// Whether the next step is something the holder can act on. When it is
    /// not, <see cref="DecidedBy"/> is required — a dead end still has to name
    /// who decided.
    /// </summary>
    public bool NextStepIsActionable { get; init; } = true;

    public LocalizedText? DecidedBy { get; init; }

    /// <summary>
    /// The change this one replaces. The replaced change is not deleted; both
    /// stay in the record.
    /// </summary>
    public string? SupersedesChangeId { get; init; }

    /// <summary>The fixture whose access this change moves, when it moves one.</summary>
    public int? AffectsMatchNumber { get; init; }

    /// <summary>The fixture a foreseeable change hangs on.</summary>
    public int? DependsOnMatchNumber { get; init; }

    /// <summary>
    /// The condition, worded as a condition and never as a commitment. Required
    /// whenever <see cref="DependsOnMatchNumber"/> is set, or the change reads
    /// as a decision already taken.
    /// </summary>
    public LocalizedText? ConditionText { get; init; }
}
