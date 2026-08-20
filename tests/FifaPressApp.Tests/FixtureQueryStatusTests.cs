using Bunit;
using FifaPressApp.Models;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The match-status filter, which reads played-versus-not from
/// <see cref="Fixture.IsResolved"/> and from nothing else.
/// </summary>
public class FixtureQueryStatusTests
{
    private static async Task<IReadOnlyList<Fixture>> RealScheduleAsync() =>
        (await TestData.ProviderOverRealSchedule().GetFixturesAsync()).Value;

    [Fact]
    public async Task AllMatchesReturnsTheWholeSchedule()
    {
        var all = await RealScheduleAsync();

        Assert.Equal(TestData.ScheduleRowCount, FixtureQuery.WithStatus(all, MatchStatusFilter.All).Count);
    }

    [Fact]
    public async Task PlayedAndNotYetPlayedPartitionTheScheduleExactly()
    {
        // Complementary and exhaustive: every fixture lands in one of the two
        // and no fixture lands in both. A control that silently dropped rows
        // would be worse than no control.
        var all = await RealScheduleAsync();

        var played = FixtureQuery.WithStatus(all, MatchStatusFilter.Played);
        var upcoming = FixtureQuery.WithStatus(all, MatchStatusFilter.NotYetPlayed);

        Assert.NotEmpty(played);
        Assert.NotEmpty(upcoming);
        Assert.Equal(TestData.ScheduleRowCount, played.Count + upcoming.Count);
        Assert.Empty(played.Select(f => f.MatchNumber).Intersect(upcoming.Select(f => f.MatchNumber)));

        Assert.All(played, fixture => Assert.True(fixture.IsResolved));
        Assert.All(upcoming, fixture => Assert.False(fixture.IsResolved));
    }

    [Fact]
    public async Task PlayedIsTheOnlyPartitionWhoseFixturesCarryTeamNames()
    {
        // The status filter is the control that comes closest to the withholding
        // rule, because "not yet played" is precisely the set the app refuses to
        // name. Both directions asserted.
        var all = await RealScheduleAsync();

        Assert.All(FixtureQuery.WithStatus(all, MatchStatusFilter.NotYetPlayed), fixture =>
        {
            Assert.Null(fixture.HomeLabel);
            Assert.Null(fixture.AwayLabel);
            Assert.EndsWith("teams not yet decided", fixture.DisplayLabel);
        });

        Assert.All(FixtureQuery.WithStatus(all, MatchStatusFilter.Played), fixture =>
        {
            Assert.NotNull(fixture.HomeLabel);
            Assert.NotNull(fixture.AwayLabel);
        });
    }

    [Fact]
    public void StatusReadsIsResolvedRatherThanComparingDates()
    {
        // A fixture dated in the past but marked unresolved must come back as
        // not-yet-played. Only the provider decides what has been played, and
        // this proves the filter did not quietly grow a second opinion.
        var pastButUnresolved = TestData.Fixture(1, kickoffLocal: new DateTime(2020, 1, 1));
        var futureButResolved = TestData.Resolved(
            TestData.Fixture(2, kickoffLocal: new DateTime(2030, 1, 1)), "Mexico", "South Africa");

        Assert.Equal(
            new[] { 1 },
            FixtureQuery.WithStatus([pastButUnresolved, futureButResolved], MatchStatusFilter.NotYetPlayed)
                .Select(fixture => fixture.MatchNumber));

        Assert.Equal(
            new[] { 2 },
            FixtureQuery.WithStatus([pastButUnresolved, futureButResolved], MatchStatusFilter.Played)
                .Select(fixture => fixture.MatchNumber));
    }

    [Fact]
    public async Task StatusComposesWithBothGroupAndSearch()
    {
        var all = await RealScheduleAsync();

        var everything = FixtureQuery.Apply(all, null, FixtureQuery.AllGroups, MatchStatusFilter.All);
        var narrowed = FixtureQuery.Apply(all, "Group D", "D", MatchStatusFilter.Played);

        Assert.Equal(TestData.ScheduleRowCount, everything.Count);
        Assert.NotEmpty(narrowed);
        Assert.All(narrowed, fixture =>
        {
            Assert.Equal("D", fixture.GroupLetter);
            Assert.True(fixture.IsResolved);
        });

        // Each control only ever removes what the one before it left.
        Assert.Subset(
            FixtureQuery.Apply(all, "Group D", "D", MatchStatusFilter.All).Select(f => f.MatchNumber).ToHashSet(),
            narrowed.Select(f => f.MatchNumber).ToHashSet());
    }

    [Fact]
    public async Task AllThreeControlsTogetherCanNarrowToNothingWithoutFailing()
    {
        var all = await RealScheduleAsync();

        // A group-stage search term, the knockout rounds, and a status: a
        // combination with no possible answer, which must be empty rather than
        // wrong.
        Assert.Empty(FixtureQuery.Apply(all, "Group A", FixtureQuery.KnockoutRounds, MatchStatusFilter.Played));
    }

    [Fact]
    public void StatusSelectIsLabelledAndOffersTheThreeOptions()
    {
        var fixtures = new[] { TestData.Fixture(1) };

        using var context = new BunitContext();
        context.WithLocale();
        context.Services.AddSingleton<IAccessDataProvider>(new StubAccessDataProvider(fixtures));
        // EventList reads the session to decide whether a request control is
        // offered at all, so the provider has to be registered even where the
        // test is about a filter rather than about who is signed in.
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));

        var page = context.Render<FifaPressApp.Pages.EventList>();

        var label = page.Find("label[for=matches-status]");
        Assert.Equal("Match status", label.TextContent.Trim());

        Assert.Equal(
            new[] { "All matches", "Played", "Not yet played" },
            page.Find("select#matches-status").QuerySelectorAll("option")
                .Select(option => option.TextContent.Trim()));
    }

    [Fact]
    public void ChangingTheStatusNarrowsTheRenderedList()
    {
        var upcoming = TestData.Fixture(1, venue: "Estadio Azteca");
        var played = TestData.Resolved(
            TestData.Fixture(2, venue: "BC Place", city: "Vancouver"), "Mexico", "South Africa");

        using var context = new BunitContext();
        context.WithLocale();
        context.Services.AddSingleton<IAccessDataProvider>(new StubAccessDataProvider([upcoming, played]));
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));

        var page = context.Render<FifaPressApp.Pages.EventList>();
        Assert.Contains("Estadio Azteca", page.Markup);
        Assert.Contains("BC Place", page.Markup);

        page.Find("select#matches-status").Change(nameof(MatchStatusFilter.NotYetPlayed));

        Assert.Contains("Estadio Azteca", page.Markup);
        Assert.DoesNotContain("BC Place", page.Markup);
    }

    [Fact]
    public void ChangingAControlResetsHowMuchOfTheListIsShown()
    {
        // Page 4 of a list that now has one page is a blank screen that reads as
        // a bug. Search already reset the page; every control does now.
        var fixtures = Enumerable.Range(1, 25)
            .Select(number => number <= 20
                ? TestData.Fixture(number, groupLetter: "A")
                : TestData.Fixture(number, groupLetter: "B", venue: "BC Place", city: "Vancouver"))
            .ToArray();

        using var context = new BunitContext();
        context.WithLocale();
        context.Services.AddSingleton<IAccessDataProvider>(new StubAccessDataProvider(fixtures));
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));

        var page = context.Render<FifaPressApp.Pages.EventList>();

        // Twenty-five fixtures at twelve a press: expand twice, to twenty-four
        // on screen and one still held back.
        page.Find(".matches__show-more").Click();
        Assert.Equal(24, page.FindAll(".matches__item").Count);

        page.Find("select#matches-group").Change("B");

        // Five fixtures left, so the control is gone entirely — and they are
        // all on screen, which is only true because the count went back to
        // twelve rather than inheriting the expansion.
        Assert.Empty(page.FindAll(".matches__show-more"));
        Assert.Equal(5, page.FindAll(".matches__item").Count);
    }

    [Fact]
    public void TheEmptyStateNamesEveryControlThatIsNarrowingTheList()
    {
        var fixtures = new[] { TestData.Fixture(1, groupLetter: "A", venue: "Estadio Azteca") };

        using var context = new BunitContext();
        context.WithLocale();
        context.Services.AddSingleton<IAccessDataProvider>(new StubAccessDataProvider(fixtures));
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));

        var page = context.Render<FifaPressApp.Pages.EventList>();
        page.Find("input[type=search]").Input("Azteca");
        page.Find("select#matches-status").Change(nameof(MatchStatusFilter.Played));

        var empty = page.Find(".matches__empty").TextContent;
        Assert.Contains("a search for \"Azteca\"", empty);
        Assert.Contains("matches already played only", empty);

        // A search term is active, so the reason a team name finds nothing is
        // worth stating.
        Assert.Contains("do not list their teams", empty);
    }

    [Fact]
    public void WithNoControlActiveTheEmptyStateStaysAsItWas()
    {
        using var context = new BunitContext();
        context.WithLocale();
        context.Services.AddSingleton<IAccessDataProvider>(new StubAccessDataProvider([]));
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));

        var page = context.Render<FifaPressApp.Pages.EventList>();

        // An empty schedule is a load failure, and the page says so instead —
        // the "no matches to show" wording is reserved for a list that is
        // genuinely empty with nothing narrowing it.
        Assert.Contains("could not be loaded", page.Markup);
    }
}
