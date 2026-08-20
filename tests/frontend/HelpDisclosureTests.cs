using Bunit;
using FifaPressApp.Components;
using FifaPressApp.Models;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// Help, as eight independently collapsible sections.
///
/// <para>
/// Two constraints from the frozen spec survive this rendering change, and both
/// are asserted here rather than assumed: the page is entirely static, so no
/// section's state may depend on a fetch and the whole thing must render from
/// cache; and the contact section must not resemble an appeal channel, so
/// collapsing it changed its visibility and nothing else.
/// </para>
/// </summary>
public class HelpDisclosureTests
{
    private sealed record Harness(BunitContext Context, LocaleService Locale);

    private static Harness NewHarness()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        var locale = LocaleTestData.Loaded();
        context.Services.AddSingleton(locale);
        context.RenderTree.Add<LocaleProvider>();

        return new Harness(context, locale);
    }

    [Fact]
    public void EverySectionIsClosedOnArrival()
    {
        // Somebody arriving from a specific refusal should not have to scroll
        // past four sections they did not come for.
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Help>();

        Assert.NotEmpty(page.FindAll("details.help__section"));
        Assert.Empty(page.FindAll("details.help__section[open]"));
    }

    [Fact]
    public void TheFiveStagesArePeersOfTheOtherThreeCategories()
    {
        // Eight sections at one level, not four with five headings nested in
        // one of them. That is what makes the page read as a list of answerable
        // questions rather than a single document.
        var harness = NewHarness();
        using var context = harness.Context;

        Assert.Equal(8, context.Render<Help>().FindAll("details.help__section").Count);
    }

    [Fact]
    public void EverySectionsHeadingIsItsOwnControl()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Help>();
        var summaries = page.FindAll("details.help__section > summary");

        Assert.Equal(8, summaries.Count);
        Assert.All(summaries, summary => Assert.NotEmpty(summary.QuerySelectorAll("h2")));
    }

    [Fact]
    public void EachSectionOpensIndependentlyOfTheOthers()
    {
        // An accordion that closed its neighbours would take a decision away
        // from a person who wants two answers at once.
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Help>();

        // Nothing here couples the sections: they are separate <details>
        // elements with no shared name attribute, which is the one thing that
        // would make the browser treat them as mutually exclusive.
        Assert.All(
            page.FindAll("details.help__section"),
            section => Assert.False(section.HasAttribute("name")));
    }

    [Fact]
    public void ThePageIsEntirelyStatic()
    {
        // 4.2's state matrix is not reopened: the offline path terminates here,
        // so nothing on this page may need a network. It injects the locale
        // service and nothing else — no data provider, no session, no fetch.
        var harness = NewHarness();
        using var context = harness.Context;

        // Rendering with no IAccessDataProvider registered at all is the proof:
        // if the page touched one, this would throw.
        var page = context.Render<Help>();

        Assert.NotEmpty(page.FindAll("h1"));
        Assert.Empty(page.FindAll("button"));
    }

    [Fact]
    public void TheDisclosureNeedsNoJavaScript()
    {
        // <details> is browser behaviour. A section opens with no interop, no
        // state and nothing to load, which is what keeps this page readable
        // when everything else has failed.
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Help>();

        Assert.Empty(page.FindAll("[onclick]"));
        Assert.All(
            page.FindAll("details.help__section"),
            section => Assert.Equal("DETAILS", section.TagName));
    }

    [Fact]
    public void NoSectionAutoExpandsFromAUrlFragment()
    {
        // 09 §7.3 names this as a natural extension and explicitly does not
        // authorize it. Raised in the Completion Report rather than assumed.
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Help>();

        Assert.Empty(page.FindAll("details.help__section[open]"));
        Assert.All(
            page.FindAll("details.help__section"),
            section => Assert.False(section.HasAttribute("id")));
    }

    [Fact]
    public void TheContentInventoryIsUnchanged()
    {
        // 09 §7.4: no change to what Help covers. The five stages and the four
        // categories are all still here, with their own words.
        var harness = NewHarness();
        using var context = harness.Context;

        var text = context.Render<Help>().Markup;

        foreach (var heading in new[]
                 {
                     "Before you apply",
                     "While your application is being assessed",
                     "Once a decision is taken",
                     "During the tournament",
                     "On a matchday",
                     "What this service does not do",
                     "What will not reach you as a notification",
                     "Who to contact",
                 })
        {
            Assert.Contains(heading, text);
        }
    }

    [Fact]
    public void TheContactSectionStillSaysItIsNotAnAppealChannel()
    {
        // Collapsing it changed its visibility, not its framing. The copy
        // constraint from 05_CONCEPT §5 is unaffected by a rendering change,
        // and this is what would notice if it quietly were not.
        var harness = NewHarness();
        using var context = harness.Context;

        var text = context.Render<Help>().Markup;

        Assert.Contains("no appeal channel", text);
        Assert.Contains("does not accept submissions", text);

        // And it still names people who already hold the file rather than
        // offering a route back into this app.
        Assert.Contains("accreditation centre", text);
        Assert.DoesNotContain("submit an appeal", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TheBoundaryIsStillStatedInFull()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var text = context.Render<Help>().Markup;

        foreach (var required in new[] { "Visas", "Vetting", "Quota", "Guaranteed access", "Appeals" })
        {
            Assert.Contains(required, text);
        }
    }

    // ------------------------------------------------------- three languages

    [Theory]
    [InlineData(AppLocale.En, "Help")]
    [InlineData(AppLocale.Es, "Ayuda")]
    [InlineData(AppLocale.Pt, "Ajuda")]
    public void HelpRendersInEveryLocale(AppLocale locale, string heading)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Help>();
        harness.Locale.Set(locale);

        Assert.Equal(heading, page.Find("h1").TextContent);
    }

    [Theory]
    [InlineData(AppLocale.Es, "Qué no hace este servicio")]
    [InlineData(AppLocale.Pt, "O que este serviço não faz")]
    public void EverySectionHeadingTranslates(AppLocale locale, string expected)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Help>();
        harness.Locale.Set(locale);

        Assert.Contains(expected, page.Markup);
    }

    [Theory]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public void NoEnglishHeadingSurvivesATranslatedRender(AppLocale locale)
    {
        // Help was deliberately left English through boundary 3 because it was
        // going to be rewritten here. This is what confirms the deferral was
        // discharged rather than forgotten.
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Help>();
        harness.Locale.Set(locale);

        foreach (var english in new[]
                 {
                     "Before you apply",
                     "On a matchday",
                     "What this service does not do",
                     "Who to contact",
                 })
        {
            Assert.DoesNotContain(english, page.Markup);
        }
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public void TheSectionsStayClosedInEveryLanguage(AppLocale locale)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Help>();
        harness.Locale.Set(locale);

        Assert.Equal(8, page.FindAll("details.help__section").Count);
        Assert.Empty(page.FindAll("details.help__section[open]"));
    }
}
