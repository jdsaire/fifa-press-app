using Bunit;
using FifaPressApp.Models;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The group filter: its options come from the data, and it narrows without
/// touching what search matches.
/// </summary>
public class FixtureQueryGroupTests
{
    private static async Task<IReadOnlyList<Fixture>> RealScheduleAsync() =>
        (await TestData.ProviderOverRealSchedule().GetFixturesAsync()).Value;

    [Fact]
    public async Task OptionsAreTheGroupsTheScheduleActuallyContains()
    {
        var all = await RealScheduleAsync();

        var letters = FixtureQuery.GroupLetters(all);

        Assert.Equal(new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" }, letters);
        Assert.Equal(
            all.Select(fixture => fixture.GroupLetter).Where(letter => letter is not null).Distinct().Count(),
            letters.Count);
    }

    [Fact]
    public void OptionsFollowTheDataRatherThanAHardcodedList()
    {
        // A schedule with three groups offers three options, not twelve. This is
        // what distinguishes deriving the list from writing A–L down.
        var fixtures = new[]
        {
            TestData.Fixture(1, groupLetter: "C"),
            TestData.Fixture(2, groupLetter: "A"),
            TestData.Fixture(3, groupLetter: "C"),
            TestData.Fixture(4, PhaseKind.Final, null),
        };

        Assert.Equal(new[] { "A", "C" }, FixtureQuery.GroupLetters(fixtures));
    }

    [Fact]
    public async Task SelectingAGroupReturnsOnlyThatGroup()
    {
        var all = await RealScheduleAsync();

        var groupD = FixtureQuery.InGroup(all, "D");

        Assert.NotEmpty(groupD);
        Assert.All(groupD, fixture =>
        {
            Assert.Equal("D", fixture.GroupLetter);
            Assert.Equal(PhaseKind.GroupStage, fixture.Phase);
        });
    }

    [Fact]
    public async Task KnockoutRoundsReturnsOnlyFixturesWithNoGroupLetter()
    {
        var all = await RealScheduleAsync();

        var knockout = FixtureQuery.InGroup(all, FixtureQuery.KnockoutRounds);

        Assert.NotEmpty(knockout);
        Assert.All(knockout, fixture => Assert.Null(fixture.GroupLetter));
        Assert.DoesNotContain(knockout, fixture => fixture.Phase == PhaseKind.GroupStage);
    }

    [Fact]
    public async Task EveryOptionTogetherAccountsForTheWholeSchedule()
    {
        // The reason "Knockout rounds" exists at all: without it, every fixture
        // it covers would be reachable only under "All groups", and a control
        // that hides a third of the list looks broken.
        var all = await RealScheduleAsync();

        var byGroup = FixtureQuery.GroupLetters(all).Sum(letter => FixtureQuery.InGroup(all, letter).Count);
        var knockout = FixtureQuery.InGroup(all, FixtureQuery.KnockoutRounds).Count;

        Assert.Equal(TestData.ScheduleRowCount, byGroup + knockout);
        Assert.Equal(TestData.ScheduleRowCount, FixtureQuery.InGroup(all, FixtureQuery.AllGroups).Count);
    }

    [Fact]
    public async Task GroupComposesWithSearchAsAnd()
    {
        var all = await RealScheduleAsync();

        var searchOnly = FixtureQuery.Apply(all, "Dallas", FixtureQuery.AllGroups, MatchStatusFilter.All);
        var both = FixtureQuery.Apply(all, "Dallas", FixtureQuery.KnockoutRounds, MatchStatusFilter.All);

        Assert.NotEmpty(both);
        Assert.All(both, fixture =>
        {
            Assert.Null(fixture.GroupLetter);
            Assert.Contains("Dallas", fixture.City, StringComparison.OrdinalIgnoreCase);
        });

        // Narrowing only ever removes. It never introduces a fixture that search
        // alone would not have found.
        Assert.Subset(
            searchOnly.Select(fixture => fixture.MatchNumber).ToHashSet(),
            both.Select(fixture => fixture.MatchNumber).ToHashSet());
    }

    [Fact]
    public async Task FilteringNeverSurfacesATeamNameForAnUnplayedFixture()
    {
        // The filter is a new way to ask for fixtures, and a new way to ask must
        // not become a new way to leak. Every option, over the whole schedule.
        var all = await RealScheduleAsync();

        var selections = new List<string?> { null, FixtureQuery.AllGroups, FixtureQuery.KnockoutRounds };
        selections.AddRange(FixtureQuery.GroupLetters(all));

        foreach (var selection in selections)
        {
            Assert.All(
                FixtureQuery.InGroup(all, selection).Where(fixture => !fixture.IsResolved),
                fixture =>
                {
                    Assert.Null(fixture.HomeLabel);
                    Assert.Null(fixture.AwayLabel);
                });
        }
    }

    [Fact]
    public async Task AnUnknownSelectionNarrowsToNothingRatherThanToEverything()
    {
        // Failing open would be the dangerous direction: a control that silently
        // does nothing looks like a control that found no matches.
        var all = await RealScheduleAsync();

        Assert.Empty(FixtureQuery.InGroup(all, "Z"));
    }

    [Fact]
    public void GroupSelectIsLabelledAndOffersTheDerivedOptions()
    {
        var fixtures = new[]
        {
            TestData.Fixture(1, groupLetter: "A"),
            TestData.Fixture(2, groupLetter: "B"),
            TestData.Fixture(3, PhaseKind.Final, null),
        };

        using var context = new BunitContext();
        context.WithLocale();
        context.Services.AddSingleton<IAccessDataProvider>(new StubAccessDataProvider(fixtures));
        // EventList reads the session to decide whether a request control is
        // offered at all, so the provider has to be registered even where the
        // test is about a filter rather than about who is signed in.
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));

        var page = context.Render<FifaPressApp.Pages.EventList>();

        // Labelled for assistive technology by a real <label for>, not a
        // placeholder that disappears the moment someone uses the control.
        var select = page.Find("select#matches-group");
        var label = page.Find("label[for=matches-group]");
        Assert.Equal("Group", label.TextContent.Trim());

        Assert.Equal(
            new[] { "All groups", "Group A", "Group B", "Knockout rounds" },
            select.QuerySelectorAll("option").Select(option => option.TextContent.Trim()));
    }

    [Fact]
    public void ChangingTheGroupNarrowsTheRenderedList()
    {
        var fixtures = new[]
        {
            TestData.Fixture(1, groupLetter: "A", venue: "Estadio Azteca"),
            TestData.Fixture(2, groupLetter: "B", venue: "BC Place", city: "Vancouver"),
        };

        using var context = new BunitContext();
        context.WithLocale();
        context.Services.AddSingleton<IAccessDataProvider>(new StubAccessDataProvider(fixtures));
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));

        var page = context.Render<FifaPressApp.Pages.EventList>();
        Assert.Contains("BC Place", page.Markup);

        page.Find("select#matches-group").Change("A");

        Assert.Contains("Estadio Azteca", page.Markup);
        Assert.DoesNotContain("BC Place", page.Markup);
    }

    [Fact]
    public void AGroupThatMatchesNothingSaysWhatIsNarrowingTheList()
    {
        var fixtures = new[] { TestData.Fixture(1, groupLetter: "A") };

        using var context = new BunitContext();
        context.WithLocale();
        context.Services.AddSingleton<IAccessDataProvider>(new StubAccessDataProvider(fixtures));
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));

        var page = context.Render<FifaPressApp.Pages.EventList>();
        page.Find("select#matches-group").Change(FixtureQuery.KnockoutRounds);

        var empty = page.Find(".matches__empty").TextContent;
        Assert.Contains("the knockout rounds only", empty);

        // No search term, so the team-name explanation would be answering a
        // question nobody asked.
        Assert.DoesNotContain("do not list their teams", empty);
    }
}
