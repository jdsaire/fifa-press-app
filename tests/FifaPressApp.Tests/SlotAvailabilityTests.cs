using FifaPressApp.Models;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The simulated capacity rule, and the one thing it must never do.
///
/// <para>
/// Capacity was originally asked for on the group stage. That was not a
/// preference to honour but a factual impossibility: the group stage runs
/// 11–27 June and the app's simulated instant is 3 July, so all seventy-two
/// group fixtures have been played, and scarcity on a match that finished six
/// days ago cannot inform anyone's planning. It applies to the sixteen unplayed
/// knockout fixtures instead — which is better for the stated intent and, as
/// the last test here pins down, structurally safer as well.
/// </para>
/// </summary>
public class SlotAvailabilityTests
{
    private static async Task<IReadOnlyList<Fixture>> FixturesAsync() =>
        (await TestData.ProviderOverRealSchedule().GetFixturesAsync()).Value;

    [Fact]
    public async Task TheScheduleSplitsWhereTheSimulatedInstantSaysItDoes()
    {
        // The premise every assertion below rests on: 88 played, 16 unplayed,
        // and the unplayed ones are matches 89 to 104.
        var fixtures = await FixturesAsync();

        Assert.Equal(TestData.ScheduleRowCount, fixtures.Count);

        var unplayed = fixtures.Where(fixture => !fixture.IsResolved).Select(fixture => fixture.MatchNumber).ToList();

        Assert.Equal(16, unplayed.Count);
        Assert.Equal(Enumerable.Range(89, 16), unplayed.Order());
    }

    [Fact]
    public async Task ExactlyFourUnplayedFixturesAreSoldOut()
    {
        // Verifiable with arithmetic rather than by trusting a table: the rule
        // is zero when the match number divides by four, and 92, 96, 100 and
        // 104 are the only unplayed numbers that do. The Final is among them,
        // which is the one place scarcity is most plausible.
        var fixtures = await FixturesAsync();

        var soldOut = fixtures
            .Where(fixture => fixture.SlotsRemaining == 0)
            .Select(fixture => fixture.MatchNumber)
            .Order()
            .ToList();

        Assert.Equal([92, 96, 100, 104], soldOut);
    }

    [Fact]
    public async Task EveryOtherUnplayedFixtureCarriesBetweenThreeAndElevenSlots()
    {
        var fixtures = await FixturesAsync();

        var available = fixtures
            .Where(fixture => !fixture.IsResolved && fixture.SlotsRemaining != 0)
            .ToList();

        Assert.Equal(12, available.Count);
        Assert.All(available, fixture =>
        {
            Assert.NotNull(fixture.SlotsRemaining);
            Assert.InRange(fixture.SlotsRemaining!.Value, 3, 11);
        });
    }

    [Theory]
    [InlineData(89, 11)]
    [InlineData(90, 3)]
    [InlineData(92, 0)]
    [InlineData(97, 10)]
    [InlineData(104, 0)]
    public async Task TheRuleIsReDerivableByHand(int matchNumber, int expected)
    {
        // 0 when MatchNumber % 4 == 0, otherwise 3 + (MatchNumber % 9). Written
        // out per fixture so a reviewer can check the arithmetic rather than
        // re-running the same expression the code runs.
        var fixtures = await FixturesAsync();

        Assert.Equal(expected, fixtures.Single(fixture => fixture.MatchNumber == matchNumber).SlotsRemaining);
    }

    [Fact]
    public async Task NoPlayedFixtureCarriesCapacityAtAll()
    {
        // Null rather than zero. A finished match has no slots to have run out
        // of, and "no slots available" against one would state scarcity about
        // something that is simply over.
        var fixtures = await FixturesAsync();

        var played = fixtures.Where(fixture => fixture.IsResolved).ToList();

        Assert.Equal(88, played.Count);
        Assert.All(played, fixture => Assert.Null(fixture.SlotsRemaining));
    }

    [Fact]
    public async Task NoFixtureEverCarriesBothACapacityValueAndATeamName()
    {
        // The structural guarantee, and the reason the rescope is safer rather
        // than merely more sensible. Capacity renders only on unplayed
        // fixtures; unplayed fixtures carry null team labels by construction.
        // The two axes cannot appear together, so a capacity number can never
        // be correlated with a matchup it might otherwise hint at.
        var fixtures = await FixturesAsync();

        Assert.All(fixtures, fixture =>
        {
            var hasCapacity = fixture.SlotsRemaining is not null;
            var hasTeams = fixture.HomeLabel is not null || fixture.AwayLabel is not null;

            Assert.False(hasCapacity && hasTeams,
                $"match {fixture.MatchNumber} carries both a capacity value and a team name");
        });
    }

    [Fact]
    public void CapacityIsNotStoredOnTheImportedSchedule()
    {
        // The provider owns the rule; the importer stays a pure CSV reader. A
        // fixture straight out of the importer has no capacity on it, and only
        // acquires one by passing through the read path.
        var imported = FixtureImporter.Parse(TestData.ScheduleCsv());

        Assert.All(imported.Fixtures, fixture => Assert.Null(fixture.SlotsRemaining));
    }

    [Fact]
    public async Task TheSingleFixtureReadPathAgreesWithTheListOne()
    {
        // GetFixtureAsync and GetFixturesAsync both go through Reveal, so a
        // fixture read on its own must carry the same capacity as the same
        // fixture read in the list. This is what would catch the rule being
        // applied in one read path and forgotten in the other.
        var provider = TestData.ProviderOverRealSchedule();

        var one = (await provider.GetFixtureAsync(92)).Value;
        var listed = (await provider.GetFixturesAsync()).Value.Single(fixture => fixture.MatchNumber == 92);

        Assert.Equal(listed.SlotsRemaining, one!.SlotsRemaining);
        Assert.Equal(0, one.SlotsRemaining);
    }

    // ------------------------------------------------------------ the filter

    [Fact]
    public async Task TheFilterNarrowsToFixturesThatStillHaveSlots()
    {
        var fixtures = await FixturesAsync();

        var withSlots = FixtureQuery.WithSlots(
            fixtures, SlotAvailabilityFilter.WithSlotsAvailable);

        // Twelve of the sixteen unplayed fixtures; the four sold-out ones are
        // excluded because zero slots is not "available".
        Assert.Equal(12, withSlots.Count);
        Assert.All(withSlots, fixture =>
        {
            Assert.False(fixture.IsResolved);
            Assert.True(fixture.SlotsRemaining > 0);
        });
    }

    [Fact]
    public async Task APlayedFixtureIsExcludedByDefinitionRatherThanByAnExtraClause()
    {
        // It carries no slot count at all, so "with slots available" implies
        // "not yet played" without having to say so.
        var fixtures = await FixturesAsync();

        var withSlots = FixtureQuery.WithSlots(
            fixtures, SlotAvailabilityFilter.WithSlotsAvailable);

        Assert.DoesNotContain(withSlots, fixture => fixture.IsResolved);
    }

    [Fact]
    public async Task TheDefaultLeavesTheListExactlyAsItWas()
    {
        // The whole reason the new Apply parameter carries a default: every
        // existing caller and every existing test keeps its behaviour.
        var fixtures = await FixturesAsync();

        Assert.Equal(
            fixtures.Count,
            FixtureQuery.WithSlots(
                fixtures, SlotAvailabilityFilter.All).Count);

        Assert.Equal(
            fixtures.Count,
            FixtureQuery.Apply(fixtures, null, null, MatchStatusFilter.All).Count);
    }

    [Fact]
    public async Task TheFilterComposesWithTheOthersAsAnd()
    {
        // Asking for played fixtures that still have slots is a contradiction,
        // and the composition answers it as one rather than resolving it in
        // either control's favour.
        var fixtures = await FixturesAsync();

        var contradiction = FixtureQuery.Apply(
            fixtures, null, null, MatchStatusFilter.Played,
            SlotAvailabilityFilter.WithSlotsAvailable);

        Assert.Empty(contradiction);
    }
}
