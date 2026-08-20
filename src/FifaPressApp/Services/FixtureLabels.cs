using FifaPressApp.Models;

namespace FifaPressApp.Services;

/// <summary>
/// How a fixture is named on screen, in the language the screen is in.
///
/// <para>
/// <b>Why this is a presentation helper and not a property on
/// <see cref="Fixture"/>.</b> <c>Fixture.DisplayLabel</c> and
/// <c>Fixture.PhaseLabel</c> are English on the model and must stay that way:
/// the frozen withholding test asserts <c>EndsWith("teams not yet decided")</c>
/// against the model, and the search tests drive it with <c>"Round of 16"</c>
/// and <c>"Group D"</c>. Those are contracts, and localizing the model would
/// break them.
/// </para>
///
/// <para>
/// More importantly it would be wrong on its own terms. The model's label is
/// canonical data — it is what the fixture <i>is</i>, and it is what the search
/// index matches against. What a person reads is a rendering of that, and
/// rendering is where language belongs.
/// </para>
///
/// <para>
/// <b>The withholding rule is not re-implemented here.</b> This helper reads
/// <c>IsResolved</c> and the two nullable name fields exactly as they arrive
/// from the provider. A fixture that has not kicked off arrives with no names on
/// it and there is nothing here to leak.
/// </para>
/// </summary>
public static class FixtureLabels
{
    /// <summary>
    /// The fixture's name: the two teams once it has been played, and the round
    /// plus a plain statement that the teams are not decided before that.
    /// </summary>
    public static string Display(LocaleService locale, AppLocale which, Fixture fixture) =>
        fixture is { IsResolved: true, HomeLabel: not null, AwayLabel: not null }
            ? locale.Format(which, "fixture.versus",
                ("home", Team(locale, which, fixture.HomeLabel)),
                ("away", Team(locale, which, fixture.AwayLabel)))
            : locale.Format(which, "fixture.undecided", ("phase", Phase(locale, which, fixture)));

    /// <summary>
    /// A country's name in the language the screen is in.
    ///
    /// <para>
    /// <b>Reached only from the resolved branch above, and that is structural
    /// rather than careful.</b> A fixture that has not kicked off carries no
    /// labels at all, so it takes the <c>fixture.undecided</c> path and never
    /// arrives here — the withholding rule is not re-implemented in this
    /// method, it simply cannot be reached in violation of it.
    /// </para>
    ///
    /// <para>
    /// Keyed on the canonical English name, which is what the model carries and
    /// what search matches against. English values are identity mappings held
    /// explicitly in the locale file rather than special-cased here, so this
    /// method behaves the same in all three languages instead of having a
    /// branch that only one language takes.
    /// </para>
    ///
    /// <para>
    /// A name with no entry renders as the canonical English it arrived as —
    /// never an empty string, never a raw resource key. A country added to the
    /// schedule later therefore degrades to English rather than breaking a
    /// card.
    /// </para>
    /// </summary>
    private static string Team(LocaleService locale, AppLocale which, string canonical) =>
        locale.Has(which, $"team.{canonical}") ? locale[which, $"team.{canonical}"] : canonical;

    /// <summary>The round, including the group letter where there is one.</summary>
    public static string Phase(LocaleService locale, AppLocale which, Fixture fixture) =>
        fixture.Phase switch
        {
            PhaseKind.GroupStage => locale.Format(which, "phase.group", ("letter", fixture.GroupLetter ?? string.Empty)),
            PhaseKind.RoundOf32 => locale[which, "phase.roundOf32"],
            PhaseKind.RoundOf16 => locale[which, "phase.roundOf16"],
            PhaseKind.QuarterFinals => locale[which, "phase.quarterFinals"],
            PhaseKind.SemiFinals => locale[which, "phase.semiFinals"],
            PhaseKind.ThirdPlace => locale[which, "phase.thirdPlace"],
            PhaseKind.Final => locale[which, "phase.final"],
            _ => locale[which, "phase.unknown"],
        };
}
