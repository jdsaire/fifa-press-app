using Bunit;
using FifaPressApp.Models;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// How much of the match list is on screen, and what changes it.
///
/// <para>
/// Numbered pages are retired. They asked a reader to hold a position in a list
/// they had no reason to have a position in — a hundred and four fixtures with
/// no inherent reading order — and page four of a list that now has one page is
/// a blank screen that looks like a bug. Show-more only ever adds, so there is
/// no position to lose.
/// </para>
/// </summary>
public class ShowMoreTests
{
    private static BunitContext NewContext(int fixtureCount)
    {
        var fixtures = Enumerable.Range(1, fixtureCount)
            .Select(number => TestData.Fixture(number, PhaseKind.GroupStage, "A"))
            .ToArray();

        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));
        context.Services.AddSingleton<IAccessDataProvider>(new StubAccessDataProvider(fixtures));
        return context.WithLocale();
    }

    [Fact]
    public void TwelveFixturesAreOnScreenBeforeAnythingIsPressed()
    {
        using var context = NewContext(40);

        var page = context.Render<EventList>();

        Assert.Equal(12, page.FindAll(".matches__item").Count);
    }

    [Fact]
    public void EachPressRevealsTwelveMoreAndNeverAShowAllJump()
    {
        using var context = NewContext(40);

        var page = context.Render<EventList>();

        page.Find(".matches__show-more").Click();
        Assert.Equal(24, page.FindAll(".matches__item").Count);

        page.Find(".matches__show-more").Click();
        Assert.Equal(36, page.FindAll(".matches__item").Count);

        // The last press reveals what is left rather than overshooting.
        page.Find(".matches__show-more").Click();
        Assert.Equal(40, page.FindAll(".matches__item").Count);
    }

    [Fact]
    public void TheControlDisappearsOnceTheListIsComplete()
    {
        // A control that does nothing is worse than no control: it invites a
        // press and answers with silence.
        using var context = NewContext(20);

        var page = context.Render<EventList>();
        page.Find(".matches__show-more").Click();

        Assert.Equal(20, page.FindAll(".matches__item").Count);
        Assert.Empty(page.FindAll(".matches__show-more"));
    }

    [Fact]
    public void AListShorterThanOnePressNeverOffersTheControlAtAll()
    {
        using var context = NewContext(5);

        var page = context.Render<EventList>();

        Assert.Equal(5, page.FindAll(".matches__item").Count);
        Assert.Empty(page.FindAll(".matches__show-more"));
    }

    [Fact]
    public void TheCountSaysHowMuchOfHowManyAndIsAnnouncedPolitely()
    {
        // Pressing the control moves nothing already on screen and adds rows
        // below the fold. Without a polite live region a screen-reader user
        // gets no notification that anything happened at all.
        using var context = NewContext(40);

        var page = context.Render<EventList>();
        var count = page.Find(".matches__count");

        Assert.Equal("polite", count.GetAttribute("aria-live"));
        Assert.Equal("Showing 12 of 40 matches", count.TextContent.Trim());

        page.Find(".matches__show-more").Click();
        Assert.Equal("Showing 24 of 40 matches", page.Find(".matches__count").TextContent.Trim());
    }

    [Fact]
    public void TheCountIsAbsentWhenThereIsNothingToCount()
    {
        using var context = NewContext(0);

        var page = context.Render<EventList>();

        Assert.Empty(page.FindAll(".matches__count"));
    }

    [Fact]
    public void SearchingResetsHowMuchIsShown()
    {
        // Otherwise a narrowed list inherits an expanded count, shows itself
        // whole, and the control vanishes with no explanation of why.
        using var context = NewContext(40);

        var page = context.Render<EventList>();
        page.Find(".matches__show-more").Click();
        Assert.Equal(24, page.FindAll(".matches__item").Count);

        page.Find("input[type=search]").Input("Azteca");

        Assert.Equal(12, page.FindAll(".matches__item").Count);
        Assert.Equal("Showing 12 of 40 matches", page.Find(".matches__count").TextContent.Trim());
    }

    [Fact]
    public void ChangingAFilterResetsHowMuchIsShown()
    {
        using var context = NewContext(40);

        var page = context.Render<EventList>();
        page.Find(".matches__show-more").Click();
        page.Find(".matches__show-more").Click();
        Assert.Equal(36, page.FindAll(".matches__item").Count);

        page.Find("select#matches-status").Change(nameof(MatchStatusFilter.NotYetPlayed));

        Assert.Equal(12, page.FindAll(".matches__item").Count);
    }

    [Fact]
    public void NoNumberedPageControlSurvivesAnywhere()
    {
        using var context = NewContext(40);

        var page = context.Render<EventList>();

        Assert.Empty(page.FindAll("[aria-current='page']"));
        Assert.Empty(page.FindAll("nav[aria-label*='page']"));
        Assert.DoesNotContain("pagesLabel", page.Markup);
    }
}
