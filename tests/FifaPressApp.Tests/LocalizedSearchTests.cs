using FifaPressApp.Models;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// Search, extended to the reader's language — additively.
///
/// <para>
/// The claim being tested is not "Spanish search works". It is that the
/// extension cannot take anything away: every input that matched before still
/// matches, with the same fixtures, in every locale. That is what makes this a
/// widening of a documented contract rather than a change to it.
/// </para>
/// </summary>
public class LocalizedSearchTests
{
    private static async Task<IReadOnlyList<Fixture>> ScheduleAsync() =>
        (await TestData.ProviderOverRealSchedule().GetFixturesAsync()).Value;

    [Theory]
    [InlineData("Round of 16")]
    [InlineData("Group D")]
    [InlineData("Dallas")]
    [InlineData("AT&T Stadium")]
    [InlineData("quarter")]
    [InlineData("mexico")]
    public async Task EveryInputThatMatchedBeforeMatchesTheSameFixturesInEveryLocale(string term)
    {
        // The load-bearing test. If this passes for every locale, the frozen
        // search tests cannot have been broken by the extension — they drive
        // exactly these shapes of input.
        var all = await ScheduleAsync();
        var locale = LocaleTestData.Loaded();

        var canonical = FixtureQuery.Search(all, term).Select(f => f.MatchNumber).ToList();

        foreach (var which in LocaleService.All)
        {
            var extended = FixtureQuery.Search(all, term, locale, which)
                .Select(f => f.MatchNumber)
                .ToList();

            Assert.All(canonical, number => Assert.Contains(number, extended));
        }
    }

    [Theory]
    [InlineData(AppLocale.Es, "octavos")]
    [InlineData(AppLocale.Es, "cuartos")]
    [InlineData(AppLocale.Pt, "oitavos")]
    [InlineData(AppLocale.Pt, "quartos")]
    public async Task AReaderCanSearchInTheLanguageTheyAreReading(AppLocale which, string term)
    {
        var all = await ScheduleAsync();
        var locale = LocaleTestData.Loaded();

        // Nothing in English matches these.
        Assert.Empty(FixtureQuery.Search(all, term));

        var found = FixtureQuery.Search(all, term, locale, which);
        Assert.NotEmpty(found);
    }

    [Fact]
    public async Task SearchingSpanishForOctavosFindsExactlyTheRoundOf16()
    {
        var all = await ScheduleAsync();
        var locale = LocaleTestData.Loaded();

        var found = FixtureQuery.Search(all, "octavos", locale, AppLocale.Es);

        Assert.All(found, fixture => Assert.Equal(PhaseKind.RoundOf16, fixture.Phase));
        Assert.Equal(
            all.Count(fixture => fixture.Phase == PhaseKind.RoundOf16),
            found.Count);
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public async Task AnEmptySearchStillReturnsTheWholeSchedule(AppLocale which)
    {
        var all = await ScheduleAsync();
        var locale = LocaleTestData.Loaded();

        Assert.Equal(TestData.ScheduleRowCount, FixtureQuery.Search(all, "", locale, which).Count);
        Assert.Equal(TestData.ScheduleRowCount, FixtureQuery.Search(all, "   ", locale, which).Count);
        Assert.Equal(TestData.ScheduleRowCount, FixtureQuery.Search(all, null, locale, which).Count);
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public async Task SearchingATeamNameStillCannotFindAnUnplayedFixture(AppLocale which)
    {
        // The extension reads the fixture's translated label, and an unresolved
        // fixture's label names the round rather than the teams — in every
        // language. A translated search cannot surface a name the English one
        // was withholding.
        var all = await ScheduleAsync();
        var locale = LocaleTestData.Loaded();

        // Portugal and Spain meet in match 93, which has not kicked off.
        foreach (var team in new[] { "Portugal", "Spain" })
        {
            var found = FixtureQuery.Search(all, team, locale, which);
            Assert.DoesNotContain(found, fixture => fixture.MatchNumber == 93);
        }
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public async Task NoSearchInAnyLocaleEverReturnsAFixtureCarryingAWithheldName(AppLocale which)
    {
        var all = await ScheduleAsync();
        var locale = LocaleTestData.Loaded();

        foreach (var term in new[] { "v", "de", "a", "final", "grupo", "group" })
        {
            foreach (var fixture in FixtureQuery.Search(all, term, locale, which))
            {
                if (!fixture.IsResolved)
                {
                    Assert.Null(fixture.HomeLabel);
                    Assert.Null(fixture.AwayLabel);
                }
            }
        }
    }

    [Fact]
    public async Task TheComposedFiltersStillNarrowRatherThanWiden()
    {
        var all = await ScheduleAsync();
        var locale = LocaleTestData.Loaded();

        var searchOnly = FixtureQuery.Search(all, "octavos", locale, AppLocale.Es);
        var withStatus = FixtureQuery.Apply(
            all, "octavos", FixtureQuery.AllGroups, MatchStatusFilter.NotYetPlayed, locale, AppLocale.Es);

        Assert.True(withStatus.Count <= searchOnly.Count);
        Assert.All(withStatus, fixture => Assert.Contains(fixture, searchOnly));
    }
}
