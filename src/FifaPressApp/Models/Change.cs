namespace FifaPressApp.Models;

/// <summary>
/// What kind of state movement a change records.
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
/// Where access to one particular fixture currently stands. Never stored:
/// always folded from the changes that name that fixture, so the status and
/// the log can never disagree.
/// </summary>
public enum MatchAccessStatus
{
    NotRequested,
    Requested,
    Granted,
    Revoked,
}

/// <summary>
/// How loudly a change arrives. Derived, never configured — see
/// <see cref="Change.Urgency"/>.
/// </summary>
public enum Urgency
{
    /// <summary>Written to the record and interrupts.</summary>
    Immediate,

    /// <summary>Written and interrupts once, when it becomes foreseeable.</summary>
    Foreseeable,

    /// <summary>Written to the record only. Never interrupts.</summary>
    Silent,
}

/// <summary>
/// One movement in the access record: what changed, why, and what the holder
/// can do about it.
///
/// <para>
/// <b>Append-only, enforced by the shape of this type.</b> Every property is
/// get-only and there is no method on this class that alters anything. There is
/// no <c>Update</c>, no <c>Delete</c>, no setter, and nothing anywhere else in
/// the codebase that edits a change after it exists. A correction is a *new*
/// change carrying <see cref="SupersedesChangeId"/>; a withdrawal is a *new*
/// change of kind <see cref="ChangeKind.Withdrawal"/>. Both leave the record
/// they replace intact and readable.
/// </para>
///
/// <para>
/// <b>Three fields are required at construction, and this constructor throws
/// without them.</b> A change that cannot say what changed, why, and what
/// happens next is malformed — it must fail to exist rather than render as a
/// blank line on screen, because a blank "why" is exactly the silence this
/// whole record exists to replace.
/// </para>
///
/// MOCKED in this version: every change instance is authored by the mock
/// provider. A real system emits these from the accreditation system at the
/// moment state actually moves.
/// </summary>
public sealed class Change
{
    /// <summary>
    /// The window inside which a consequence is treated as immediate rather
    /// than foreseeable. Seventy-two hours is the smallest span in which
    /// re-planning intercity travel is plausibly cheaper than absorbing the
    /// loss. It is an inference from the holder's constraints, not a measured
    /// figure, and it is stated here as one number so it can be changed in one
    /// place.
    /// </summary>
    public static readonly TimeSpan ImmediateWindow = TimeSpan.FromHours(72);

    public Change(
        string changeId,
        string credentialId,
        DateTime writtenUtc,
        DateTime effectiveUtc,
        ChangeKind kind,
        Track track,
        string whatChanged,
        string reason,
        string nextStep,
        bool nextStepIsActionable = true,
        string? decidedBy = null,
        string? supersedesChangeId = null,
        int? affectsMatchNumber = null,
        int? dependsOnMatchNumber = null,
        string? conditionText = null)
    {
        // The three required fields, checked before anything else. Throwing
        // here is the point: an invalid change never becomes an object, so no
        // screen ever has to decide how to render a missing reason.
        Require(whatChanged, nameof(whatChanged));
        Require(reason, nameof(reason));
        Require(nextStep, nameof(nextStep));

        // A reason that just restates the outcome is not a reason. "Your
        // access was revoked" explains nothing that "what changed" did not
        // already say, and accepting it would let the record look complete
        // while telling the holder nothing.
        if (Normalize(reason) == Normalize(whatChanged))
        {
            throw new ArgumentException(
                "Reason must explain why the change happened, not restate what changed.",
                nameof(reason));
        }

        // A change with nothing the holder can act on still owes them a name.
        if (!nextStepIsActionable && string.IsNullOrWhiteSpace(decidedBy))
        {
            throw new ArgumentException(
                "DecidedBy is required when the next step is not actionable.",
                nameof(decidedBy));
        }

        // A change that hangs on an unplayed fixture has to say what the
        // condition is, or it reads as a decision already taken.
        if (dependsOnMatchNumber is not null && string.IsNullOrWhiteSpace(conditionText))
        {
            throw new ArgumentException(
                "ConditionText is required when a change depends on a fixture.",
                nameof(conditionText));
        }

        if (string.IsNullOrWhiteSpace(changeId))
        {
            throw new ArgumentException("ChangeId is required.", nameof(changeId));
        }

        if (string.IsNullOrWhiteSpace(credentialId))
        {
            throw new ArgumentException("CredentialId is required.", nameof(credentialId));
        }

        ChangeId = changeId;
        CredentialId = credentialId;
        WrittenUtc = writtenUtc;
        EffectiveUtc = effectiveUtc;
        Kind = kind;
        WhatChanged = whatChanged;
        Reason = reason;
        NextStep = nextStep;
        NextStepIsActionable = nextStepIsActionable;
        DecidedBy = decidedBy;
        SupersedesChangeId = supersedesChangeId;
        AffectsMatchNumber = affectsMatchNumber;
        DependsOnMatchNumber = dependsOnMatchNumber;
        ConditionText = conditionText;

        // Computed once, here, from the change's own facts and the holder's
        // track. There is no parameter for it and no setter, so no caller can
        // hand in an urgency of their choosing.
        Urgency = DeriveUrgency(kind, writtenUtc, effectiveUtc, track, dependsOnMatchNumber);
    }

    public string ChangeId { get; }

    public string CredentialId { get; }

    /// <summary>When this change was recorded.</summary>
    public DateTime WrittenUtc { get; }

    /// <summary>
    /// When this change starts to matter. This — not <see cref="WrittenUtc"/> —
    /// is the ordering key for the record, so a change resolving on Saturday
    /// sits above one written later that resolves next month.
    /// </summary>
    public DateTime EffectiveUtc { get; }

    public ChangeKind Kind { get; }

    /// <summary>
    /// Derived at construction from kind, effective date and track. Never a
    /// user preference: interrupt-versus-wait is decided by what happened and
    /// who it happened to, not by a setting the holder was expected to find.
    /// </summary>
    public Urgency Urgency { get; }

    public string WhatChanged { get; }

    public string Reason { get; }

    public string NextStep { get; }

    /// <summary>
    /// Whether <see cref="NextStep"/> is something the holder can act on. When
    /// it is not, <see cref="DecidedBy"/> is required — a dead end still has to
    /// name who decided and what remains open.
    /// </summary>
    public bool NextStepIsActionable { get; }

    public string? DecidedBy { get; }

    /// <summary>
    /// The change this one replaces. The replaced change is not deleted; both
    /// stay in the record so the holder can see the old value beside the new
    /// one, which is what makes a change legible *as* a change.
    /// </summary>
    public string? SupersedesChangeId { get; }

    /// <summary>
    /// The fixture whose access this change moves, when it moves one.
    ///
    /// <para>
    /// Distinct from <see cref="DependsOnMatchNumber"/>, and the distinction
    /// matters: this names the match the change *is about*, while that one
    /// names a match the change *waits on*. A revocation of Tuesday's access
    /// because of Saturday's result sets both, to two different numbers.
    /// Without this field there is no way to list access per fixture, or to
    /// show a cached per-match status at a barrier, since the log would record
    /// that access changed without recording what it changed for.
    /// </para>
    /// </summary>
    public int? AffectsMatchNumber { get; }

    /// <summary>The fixture a foreseeable change hangs on.</summary>
    public int? DependsOnMatchNumber { get; }

    /// <summary>
    /// The condition, worded as a condition and never as a commitment. Names
    /// both outcomes.
    /// </summary>
    public string? ConditionText { get; }

    /// <summary>
    /// Whether this change reduces what the holder can do. Reducing changes are
    /// the ones worth interrupting for; expansions and corrections are not.
    /// </summary>
    public static bool IsReducing(ChangeKind kind) => kind switch
    {
        ChangeKind.MatchAccessRevoked => true,
        ChangeKind.ZoneAccessNarrowed => true,
        ChangeKind.ValidityShortened => true,
        ChangeKind.Withdrawal => true,
        _ => false,
    };

    private static Urgency DeriveUrgency(
        ChangeKind kind,
        DateTime writtenUtc,
        DateTime effectiveUtc,
        Track track,
        int? dependsOnMatchNumber)
    {
        var classified = Classify(kind, writtenUtc, effectiveUtc, dependsOnMatchNumber);

        // The track's ceiling is the last word. A holder with a named contact
        // has a human who can answer a conditional question, so a foreseeable
        // change is still written to the record but does not interrupt — which
        // is what Silent means.
        if (classified == Urgency.Foreseeable &&
            track.NotificationCeiling == NotificationCeiling.ImmediateOnly)
        {
            return Urgency.Silent;
        }

        return classified;
    }

    private static Urgency Classify(
        ChangeKind kind,
        DateTime writtenUtc,
        DateTime effectiveUtc,
        int? dependsOnMatchNumber)
    {
        var reducesOrDecides = IsReducing(kind) || kind == ChangeKind.RequestDecided;

        if (!reducesOrDecides)
        {
            // Nothing the holder can do less of, and no deadline attached.
            return Urgency.Silent;
        }

        // Conditional on a fixture that has not been played: foreseeable by
        // definition, whatever the date arithmetic says.
        if (dependsOnMatchNumber is not null)
        {
            return Urgency.Foreseeable;
        }

        return effectiveUtc - writtenUtc <= ImmediateWindow
            ? Urgency.Immediate
            : Urgency.Foreseeable;
    }

    private static void Require(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"A change cannot be created without {parameterName}.", parameterName);
        }
    }

    private static string Normalize(string value) =>
        new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
}
