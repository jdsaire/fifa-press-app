namespace FifaPressApp.Models;

/// <summary>
/// The three languages this app is built in. English first because it is the
/// authoring language and the fallback, not because it is more important.
///
/// <para>
/// It lives in Models rather than Services because <see cref="Change"/> depends
/// on it: seeded record content is authored in three languages, so the language
/// a piece of content is in is part of the domain here, not a presentation
/// detail layered on afterwards.
/// </para>
/// </summary>
public enum AppLocale
{
    En,
    Es,
    Pt,
}

/// <summary>
/// One piece of authored content, in all three languages.
///
/// <para>
/// <b>Authored three times, not translated once.</b> A change's narrative prose
/// — "Your member association confirmed its opening quota split, and your name
/// was on it" — is not a template with a slot in it. It is a sentence somebody
/// wrote, for one record, and the Spanish version is a sentence somebody wrote
/// too. Running it through a translation call at render time would imply a
/// pipeline this project has no reason to build and a real accreditation system
/// would not have either: a real one authors the three languages at the source,
/// the same way the seed data does here.
/// </para>
///
/// <para>
/// <b>The recurring cost, stated rather than hidden:</b> every future change
/// seeded into this app is written three times. That is acceptable at this
/// scale — eight records — and would not be the right architecture for a real
/// multi-holder dataset. It is fine precisely because this is a demonstration.
/// </para>
/// </summary>
public sealed record LocalizedText(string En, string Es, string Pt)
{
    /// <summary>
    /// The text in one language. Never falls back: a half-authored change fails
    /// to be constructed at all rather than rendering an English sentence in the
    /// middle of a Spanish record — see <see cref="Change"/>'s constructor,
    /// which validates every locale.
    /// </summary>
    public string For(AppLocale locale) => locale switch
    {
        AppLocale.Es => Es,
        AppLocale.Pt => Pt,
        _ => En,
    };

    /// <summary>Every language this holds, for validation that has to check all of them.</summary>
    public IEnumerable<(AppLocale Locale, string Text)> All =>
    [
        (AppLocale.En, En),
        (AppLocale.Es, Es),
        (AppLocale.Pt, Pt),
    ];
}
