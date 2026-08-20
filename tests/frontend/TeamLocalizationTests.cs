using FifaPressApp.Models;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// Country names in the language the screen is in — and the two rules that
/// constrain how they get there.
///
/// <para>
/// The fix is presentation-only. <c>Fixture.HomeLabel</c> and
/// <c>AwayLabel</c> stay canonical English on the model, because that is what
/// the withholding test and the frozen search tests are written against, and
/// because a label is what the fixture <i>is</i> while a rendering is what a
/// person reads. Nothing below asserts against the model; everything asserts
/// against <c>FixtureLabels.Display</c>.
/// </para>
/// </summary>
public class TeamLocalizationTests
{
    private static Fixture Played(string home, string away) =>
        TestData.Resolved(TestData.Fixture(1, PhaseKind.GroupStage, "A"), home, away);

    [Theory]
    [InlineData(AppLocale.En, "Germany v Spain")]
    [InlineData(AppLocale.Es, "Alemania - España")]
    [InlineData(AppLocale.Pt, "Alemanha - Espanha")]
    public void APlayedFixtureNamesItsTeamsInTheReadersLanguage(AppLocale which, string expected)
    {
        // Note the separator changes with the locale too — that is the
        // fixture.versus template doing its existing job, untouched by this
        // change. Only the two names now pass through a lookup.
        var locale = LocaleTestData.Loaded();

        Assert.Equal(expected, FixtureLabels.Display(locale, which, Played("Germany", "Spain")));
    }

    [Fact]
    public void TheModelItselfIsUntouchedByAnyOfThis()
    {
        // The contract the frozen tests depend on. Localizing the model would
        // break DisplayLabel's "teams not yet decided" assertion and the search
        // index in the same stroke.
        var fixture = Played("Germany", "Spain");

        Assert.Equal("Germany", fixture.HomeLabel);
        Assert.Equal("Spain", fixture.AwayLabel);
        Assert.Equal("Germany v Spain", fixture.DisplayLabel);
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public void ACountryWithNoEntryRendersAsTheEnglishItArrivedAs(AppLocale which)
    {
        // Never an empty name, never a raw resource key. A country added to the
        // schedule later degrades to English instead of breaking a card.
        var locale = LocaleTestData.Loaded();

        var rendered = FixtureLabels.Display(locale, which, Played("Atlantis", "Wakanda"));

        Assert.Contains("Atlantis", rendered);
        Assert.Contains("Wakanda", rendered);
        Assert.DoesNotContain("team.", rendered);
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public void AnUnplayedFixtureNeverReachesATeamLookupAtAll(AppLocale which)
    {
        // Structural rather than careful: an unplayed fixture carries null
        // labels, so it takes the undecided path and there is nothing to look
        // up. The withholding rule is not re-implemented in the presentation
        // layer — it cannot be violated there.
        var locale = LocaleTestData.Loaded();
        var unplayed = TestData.Fixture(89, PhaseKind.RoundOf16, groupLetter: null);

        var rendered = FixtureLabels.Display(locale, which, unplayed);

        Assert.DoesNotContain("Germany", rendered);
        Assert.DoesNotContain("Alemania", rendered);
        Assert.DoesNotContain("Alemanha", rendered);
        Assert.Contains(locale[which, "phase.roundOf16"], rendered);
    }

    [Fact]
    public void EveryCountryInTheTrackedScheduleHasAnEntryInAllThreeFiles()
    {
        // The test that would catch a schedule change outrunning the locale
        // files. It reads the real CSV rather than a list written down here,
        // so adding a country to the data is what makes it fail.
        var locale = LocaleTestData.Loaded();
        var imported = FixtureImporter.Parse(TestData.ScheduleCsv());

        var countries = imported.Matchups.Values
            .SelectMany(matchup => new[] { matchup.Home, matchup.Away })
            .Distinct()
            .ToList();

        Assert.Equal(48, countries.Count);

        foreach (var which in LocaleService.All)
        {
            foreach (var country in countries)
            {
                Assert.True(locale.Has(which, $"team.{country}"),
                    $"{which} has no entry for team.{country}");
            }
        }
    }

    [Fact]
    public void EnglishIsAnExplicitIdentityMappingRatherThanASpecialCase()
    {
        // Held in the file, not branched around in code, so the lookup behaves
        // identically in all three languages instead of having a path only one
        // of them takes.
        var locale = LocaleTestData.Loaded();

        Assert.Equal("Germany", locale[AppLocale.En, "team.Germany"]);
        Assert.Equal("Congo DR", locale[AppLocale.En, "team.Congo DR"]);
    }

    [Theory]
    [InlineData(AppLocale.Es, "RD Congo")]
    [InlineData(AppLocale.Pt, "RD Congo")]
    public void TheNormalizedCongoSpellingIsTheOneThatGetsLookedUp(AppLocale which, string expected)
    {
        // Both CSV spellings collapse to one canonical name at import, so there
        // is one key to translate and one spelling to search — see
        // FixtureImporterTests for the collapse itself.
        var locale = LocaleTestData.Loaded();

        Assert.Contains(expected, FixtureLabels.Display(locale, which, Played("Congo DR", "Senegal")));
    }
}
