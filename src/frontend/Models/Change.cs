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
        LocalizedText whatChanged,
        LocalizedText reason,
        LocalizedText nextStep,
        bool nextStepIsActionable = true,
        LocalizedText? decidedBy = null,
        string? supersedesChangeId = null,
        int? affectsMatchNumber = null,
        int? dependsOnMatchNumber = null,
        LocalizedText? conditionText = null)
    {
        // The three required fields, checked before anything else, IN EVERY
        // LANGUAGE. Throwing here is the point: an invalid change never becomes
        // an object, so no screen ever has to decide how to render a missing
        // reason — and applying the checks per locale is what stops a
        // half-translated change existing at all, which would otherwise be a
        // blank line that only appears once somebody switches language.
        RequireAll(whatChanged, nameof(whatChanged));
        RequireAll(reason, nameof(reason));
        RequireAll(nextStep, nameof(nextStep));

        // A reason that just restates the outcome is not a reason. "Your
        // access was revoked" explains nothing that "what changed" did not
        // already say, and accepting it would let the record look complete
        // while telling the holder nothing. Checked per locale, so a Spanish
        // reason cannot restate the Spanish outcome either.
        foreach (var locale in Locales)
        {
            if (Normalize(reason.For(locale)) == Normalize(whatChanged.For(locale)))
            {
                throw new ArgumentException(
                    "Reason must explain why the change happened, not restate what changed.",
                    nameof(reason));
            }
        }

        // A change with nothing the holder can act on still owes them a name.
        if (!nextStepIsActionable && (decidedBy is null || HasAnyBlank(decidedBy)))
        {
            throw new ArgumentException(
                "DecidedBy is required when the next step is not actionable.",
                nameof(decidedBy));
        }

        // A change that hangs on an unplayed fixture has to say what the
        // condition is, or it reads as a decision already taken.
        if (dependsOnMatchNumber is not null && (conditionText is null || HasAnyBlank(conditionText)))
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
        WhatChangedText = whatChanged;
        ReasonText = reason;
        NextStepText = nextStep;
        NextStepIsActionable = nextStepIsActionable;
        DecidedByText = decidedBy;
        SupersedesChangeId = supersedesChangeId;
        AffectsMatchNumber = affectsMatchNumber;
        DependsOnMatchNumber = dependsOnMatchNumber;
        ConditionTextLocalized = conditionText;

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

    /// <summary>What changed, in all three languages.</summary>
    public LocalizedText WhatChangedText { get; }

    /// <summary>Why it changed, in all three languages.</summary>
    public LocalizedText ReasonText { get; }

    /// <summary>What the holder can do about it, in all three languages.</summary>
    public LocalizedText NextStepText { get; }

    /// <summary>
    /// The English canonical form. <b>Not for rendering.</b>
    ///
    /// <para>
    /// Screens resolve the language they are in from the cascading
    /// <see cref="AppLocale"/> and call <c>WhatChangedText.For(locale)</c>. This
    /// accessor exists so that code with no locale in hand — tests, logs, the
    /// withholding checks — has one stable string to reason about, and so that
    /// the assertions written against this record before it was localized still
    /// compile and still mean what they meant.
    /// </para>
    ///
    /// <para>
    /// It deliberately does <i>not</i> read an ambient "current locale". An
    /// earlier attempt at exactly that leaked across parallel test classes and
    /// broke unrelated tests — and the deeper problem is that an ambient read
    /// lets a component render localized text without participating in the
    /// render pass a locale change triggers, which is a stale string nobody
    /// notices until they switch language.
    /// </para>
    /// </summary>
    public string WhatChanged => WhatChangedText.En;

    /// <inheritdoc cref="WhatChanged" />
    public string Reason => ReasonText.En;

    /// <inheritdoc cref="WhatChanged" />
    public string NextStep => NextStepText.En;

    /// <summary>
    /// Whether <see cref="NextStep"/> is something the holder can act on. When
    /// it is not, <see cref="DecidedBy"/> is required — a dead end still has to
    /// name who decided and what remains open.
    /// </summary>
    public bool NextStepIsActionable { get; }

    /// <summary>
    /// Who decided, in all three languages.
    ///
    /// <para>
    /// <c>11_I18N.md</c> §4.2's R5 names four fields; this is a fifth, and it is
    /// localized because it is user-visible — <c>ChangeRow</c> renders "Decided
    /// by …" — and leaving it English would fail the requirement that every
    /// user-visible string render in all three locales.
    /// </para>
    /// </summary>
    public LocalizedText? DecidedByText { get; }

    /// <inheritdoc cref="WhatChanged" />
    public string? DecidedBy => DecidedByText?.En;

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
    /// both outcomes, in all three languages.
    /// </summary>
    public LocalizedText? ConditionTextLocalized { get; }

    /// <inheritdoc cref="WhatChanged" />
    public string? ConditionText => ConditionTextLocalized?.En;

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

    private static readonly AppLocale[] Locales = [AppLocale.En, AppLocale.Es, AppLocale.Pt];

    private static bool HasAnyBlank(LocalizedText text) =>
        text.All.Any(entry => string.IsNullOrWhiteSpace(entry.Text));

    private static void RequireAll(LocalizedText value, string parameterName)
    {
        foreach (var (locale, text) in value.All)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(
                    $"A change cannot be created without {parameterName} in {locale}.", parameterName);
            }
        }
    }

    private static string Normalize(string value) =>
        new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
}
