using Bunit;
using FifaPressApp.Components;
using FifaPressApp.Models;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// Iconography, and the rule it is held to: decoration that adds a picture to
/// text without ever taking a word away from it.
/// </summary>
public class IconTests
{
    [Theory]
    [InlineData("date")]
    [InlineData("location")]
    [InlineData("phase")]
    [InlineData("home")]
    [InlineData("matches")]
    [InlineData("record")]
    [InlineData("help")]
    [InlineData("settings")]
    public void EveryIconIsDecorativeAndInheritsItsColour(string name)
    {
        using var context = new BunitContext();
        context.WithLocale();

        var icon = context.Render<Icon>(parameters => parameters.Add(component => component.Name, name));

        var svg = icon.Find("svg");
        Assert.Equal("true", svg.GetAttribute("aria-hidden"));
        Assert.Equal("false", svg.GetAttribute("focusable"));

        // Colour is inherited, never declared: a literal here would be a colour
        // decision, which belongs to a later run, and would need a second
        // definition to survive the dark theme.
        Assert.Equal("currentColor", svg.GetAttribute("stroke"));
        Assert.DoesNotContain("#", icon.Markup);
        Assert.DoesNotContain("rgb", icon.Markup);
    }

    [Fact]
    public void AnUnknownIconNameDrawsNothingRatherThanFailing()
    {
        using var context = new BunitContext();
        context.WithLocale();

        var icon = context.Render<Icon>(parameters => parameters.Add(component => component.Name, "trophy"));

        Assert.Empty(icon.Markup.Trim());
    }

    [Fact]
    public void CardKeepsItsDateAndLocationTextAlongsideTheIcons()
    {
        using var context = new BunitContext();
        context.WithLocale();

        var card = context.Render<EventCard>(parameters => parameters
            .Add(component => component.EventName, "Round of 16 — teams not yet decided")
            .Add(component => component.EventDate, new DateTime(2026, 7, 4, 16, 0, 0))
            .Add(component => component.Location, "AT&T Stadium, Dallas")
            .Add(component => component.ReadOnly, true)
            .Add(component => component.AllowEdit, false));

        // The machine-readable attribute and the human-readable text both
        // survive the icon that now sits beside them.
        var time = card.Find("time");
        Assert.Equal("2026-07-04", time.GetAttribute("datetime"));
        Assert.Equal("July 4, 2026", time.TextContent);

        // Read as text rather than as markup: what matters is the words a
        // person sees, not how the ampersand in the venue name is encoded.
        Assert.Equal("AT&T Stadium, Dallas", card.Find("p.mb-2").TextContent.Trim());

        Assert.Equal(2, card.FindAll("svg.icon").Count);
    }

    [Fact]
    public void CardEditingBranchIsUntouchedByTheIcons()
    {
        // The icons land in the read-only branch only. The editing branch and
        // its two-way binding are not this run's to reconsider.
        using var context = new BunitContext();
        context.WithLocale();

        var card = context.Render<EventCard>(parameters => parameters
            .Add(component => component.EventName, "Something editable")
            .Add(component => component.EventDate, new DateTime(2026, 7, 4))
            .Add(component => component.Location, "Somewhere")
            .Add(component => component.ReadOnly, false));

        Assert.Empty(card.FindAll("svg.icon"));
        Assert.Equal(3, card.FindAll("input").Count);
    }

    [Fact]
    public void MatchListMetaLineKeepsItsPhaseTextBesideThePhaseIcon()
    {
        var fixture = TestData.Fixture(1, PhaseKind.GroupStage, "A");

        using var context = new BunitContext();
        context.WithLocale();
        // EventList reads the session to decide whether a request control is
        // offered at all, so the provider has to be registered even where the
        // test is about an icon.
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));
        context.Services.AddSingleton<FifaPressApp.Services.IAccessDataProvider>(
            new StubAccessDataProvider([fixture]));

        var page = context.Render<FifaPressApp.Pages.EventList>();

        Assert.Contains("Group A", page.Markup);
        Assert.NotEmpty(page.FindAll("svg.icon"));

        // The <time> element in the meta line keeps its datetime attribute.
        Assert.All(page.FindAll("time"), time => Assert.True(time.HasAttribute("datetime")));
    }
}
