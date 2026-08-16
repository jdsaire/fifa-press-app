using FifaPressApp.Models;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// Search, which this run moved and must not have changed.
///
/// <para>
/// These tests are written before any filter exists, on purpose. Their job is to
/// pin down what search already did, so that the filters added after them can be
/// shown to compose with it rather than to have quietly rewritten it.
/// </para>
/// </summary>
public class FixtureQuerySearchTests
{
    /// <summary>
    /// The predicate exactly as it read inside <c>EventList.razor</c> before it
    /// moved — kept here as the thing the extracted version is measured against.
    /// If these two ever disagree, the extraction changed behaviour.
    /// </summary>
    private static List<Fixture> PredicateAsItShipped(IEnumerable<Fixture> fixtures, string searchTerm) =>
        string.IsNullOrWhiteSpace(searchTerm)
            ? fixtures.ToList()
            : fixtures.Where(fixture =>
                fixture.DisplayLabel.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                fixture.Venue.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                fixture.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                fixture.PhaseLabel.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

    private static async Task<IReadOnlyList<Fixture>> RealScheduleAsync() =>
        (await TestData.ProviderOverRealSchedule().GetFixturesAsync()).Value;

    [Theory]
    // Every one of the four matched fields, one term each.
    [InlineData("Mexico")]           // DisplayLabel and City both
    [InlineData("Azteca")]           // Venue
    [InlineData("Vancouver")]        // City
    [InlineData("Round of 16")]      // PhaseLabel
    [InlineData("Group D")]          // PhaseLabel, group form
    [InlineData("teams not yet")]    // DisplayLabel, withheld form
    // Case-insensitivity, both directions.
    [InlineData("AZTECA")]
    [InlineData("round of 16")]
    [InlineData("vAnCoUvEr")]
    // Nothing, whitespace, and a term that matches nothing.
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Wembley")]
    // Partial and punctuated terms, which Contains handles and a smarter
    // matcher would not.
    [InlineData("Estadio")]
    [InlineData("-fin")]
    public async Task ExtractedSearchAgreesWithThePredicateAsItShipped(string term)
    {
        // The direct proof that the extraction preserved behaviour: the same
        // terms over the whole real schedule, compared match number by match
        // number against a literal copy of the version that shipped.
        var all = await RealScheduleAsync();

        Assert.Equal(
            PredicateAsItShipped(all, term).Select(fixture => fixture.MatchNumber),
            FixtureQuery.Search(all, term).Select(fixture => fixture.MatchNumber));
    }

    [Fact]
    public void SearchMatchesOnDisplayLabel()
    {
        var played = TestData.Resolved(TestData.Fixture(1), "Mexico", "South Africa");
        var other = TestData.Fixture(2, venue: "BC Place", city: "Vancouver");

        var result = FixtureQuery.Search([played, other], "South Africa");

        Assert.Equal(1, Assert.Single(result).MatchNumber);
    }

    [Fact]
    public void SearchMatchesOnVenue()
    {
        var azteca = TestData.Fixture(1, venue: "Estadio Azteca");
        var bcPlace = TestData.Fixture(2, venue: "BC Place", city: "Vancouver");

        var result = FixtureQuery.Search([azteca, bcPlace], "BC Place");

        Assert.Equal(2, Assert.Single(result).MatchNumber);
    }

    [Fact]
    public void SearchMatchesOnCity()
    {
        var mexicoCity = TestData.Fixture(1, city: "Mexico City");
        var vancouver = TestData.Fixture(2, venue: "BC Place", city: "Vancouver");

        var result = FixtureQuery.Search([mexicoCity, vancouver], "Vancouver");

        Assert.Equal(2, Assert.Single(result).MatchNumber);
    }

    [Fact]
    public void SearchMatchesOnPhaseLabel()
    {
        var group = TestData.Fixture(1, PhaseKind.GroupStage, "A");
        var knockout = TestData.Fixture(2, PhaseKind.QuarterFinals, null);

        var result = FixtureQuery.Search([group, knockout], "Quarter-finals");

        Assert.Equal(2, Assert.Single(result).MatchNumber);
    }

    [Fact]
    public void SearchIsCaseInsensitive()
    {
        var fixture = TestData.Fixture(1, venue: "Estadio Azteca");

        Assert.Single(FixtureQuery.Search([fixture], "ESTADIO"));
        Assert.Single(FixtureQuery.Search([fixture], "estadio"));
        Assert.Single(FixtureQuery.Search([fixture], "eStAdIo"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData(null)]
    public async Task EmptyOrWhitespaceSearchReturnsTheWholeSchedule(string? term)
    {
        var all = await RealScheduleAsync();

        Assert.Equal(TestData.ScheduleRowCount, FixtureQuery.Search(all, term).Count);
    }

    [Fact]
    public async Task ATermMatchingNothingReturnsNothing()
    {
        var all = await RealScheduleAsync();

        Assert.Empty(FixtureQuery.Search(all, "Wembley Stadium"));
    }

    [Fact]
    public async Task SearchingForATeamNameFindsOnlyMatchesAlreadyPlayed()
    {
        // The consequence of the withholding rule as a reader actually meets it:
        // a team name typed into the box can only ever find fixtures that have
        // kicked off, because no other fixture carries one. This is why the
        // empty state says so.
        var all = await RealScheduleAsync();

        var result = FixtureQuery.Search(all, "Mexico");

        Assert.NotEmpty(result);
        Assert.All(
            result.Where(fixture => !fixture.IsResolved),
            fixture => Assert.Contains("Mexico", fixture.Venue + fixture.City + fixture.PhaseLabel,
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void SearchDoesNotReorderWhatItIsGiven()
    {
        // Pagination slices this result, so a stable order is load-bearing even
        // though nothing about the query says "sort".
        var fixtures = new[]
        {
            TestData.Fixture(3, venue: "Estadio Azteca"),
            TestData.Fixture(1, venue: "Estadio Azteca"),
            TestData.Fixture(2, venue: "Estadio Azteca"),
        };

        Assert.Equal(
            new[] { 3, 1, 2 },
            FixtureQuery.Search(fixtures, "Azteca").Select(fixture => fixture.MatchNumber));
    }
}
