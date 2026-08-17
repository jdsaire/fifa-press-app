using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Bunit;
using FifaPressApp.Layout;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// Where the theme control lives, and what went away when it moved.
///
/// The move is only half the change: the strip it used to sit in had to be
/// deleted rather than left behind empty, in both the markup and the stylesheet.
/// A test that only checked the new placement would pass with a dead element
/// still shipping.
/// </summary>
public class ThemeTriggerPlacementTests
{
    private static string SourceRoot([CallerFilePath] string thisFile = "")
        => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "..", "src", "FifaPressApp"));

    private static string Read(params string[] parts)
        => File.ReadAllText(Path.Combine(SourceRoot(), Path.Combine(parts)));

    /// <summary>
    /// NavMenu reads the session now — it renders the signed-in indicator and
    /// the sign-out row. These tests are about the theme row, so they use a
    /// signed-out session: four rows, the theme trigger last.
    /// </summary>
    private static BunitContext NewContext()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));
        return context.WithLocale();
    }

    [Fact]
    public void TheTriggerRendersAsARowInsideTheNavList()
    {
        using var context = NewContext();

        var nav = context.Render<NavMenu>();

        // Inside the list, not merely somewhere in the component. Five rows
        // signed out: three destinations, then language, then theme.
        var rows = nav.FindAll("nav.flex-column > .nav-item");
        Assert.Equal(5, rows.Count);
        Assert.NotEmpty(rows[4].QuerySelectorAll("button.theme-trigger"));
    }

    [Fact]
    public void TheTriggerSitsBelowTheDestinations_LastOfThemWhenSignedOut()
    {
        using var context = NewContext();

        var nav = context.Render<NavMenu>();
        var rows = nav.FindAll("nav.flex-column > .nav-item");

        // Help is third and last of the destinations; everything below it is a
        // control. 09 §5.2 put the trigger visually last; 11 §5.1 then put the
        // language switch above it and 10 §4.1 put sign-out below it, so
        // "last" no longer describes the theme row at all. What survives — and
        // what these three files actually agree on — is the arrangement: three
        // destinations contiguous at the top, controls beneath them, none of
        // the controls a NavLink.
        Assert.Contains("Help", rows[2].TextContent);
        Assert.Empty(rows[3].QuerySelectorAll("a"));
        Assert.Empty(rows[4].QuerySelectorAll("a"));
    }

    [Fact]
    public void TheTriggerIsNotANavLinkAndCarriesNoActiveState()
    {
        using var context = NewContext();

        var nav = context.Render<NavMenu>();
        var trigger = nav.Find("button.theme-trigger");

        Assert.False(trigger.HasAttribute("href"));
        Assert.DoesNotContain("active", trigger.GetAttribute("class"));

        // And no rule anywhere grants it one.
        Assert.DoesNotContain(".theme-trigger.active", Read("Components", "ThemeTrigger.razor.css"));
        Assert.DoesNotContain(".theme-trigger.active", Read("Layout", "NavMenu.razor.css"));
    }

    [Fact]
    public void TheTriggerStillRendersAnIconAndItsLabelTogether()
    {
        // The move is a placement decision. What the control renders — an icon
        // paired with a label that says what pressing it does — is unchanged,
        // and the label is what a screen reader gets; the icon is hidden.
        using var context = NewContext();

        var nav = context.Render<NavMenu>();
        var trigger = nav.Find("button.theme-trigger");

        Assert.Equal("true", trigger.QuerySelector(".theme-trigger__icon")!.GetAttribute("aria-hidden"));
        Assert.Contains("theme", trigger.QuerySelector(".theme-trigger__label")!.TextContent);
    }

    [Fact]
    public void TheRowIsStillRendered_DisabledWhenItsModuleNeverArrives()
    {
        // Rendering a disabled row rather than no row is what stops the list
        // changing height the moment the app finishes starting — and if the
        // module never loads at all, the app still renders in the system theme
        // and only the ability to override it is lost. That degrades quietly;
        // it does not remove a row from the navigation.
        using var context = NewContext();
        context.JSInterop
            .SetupModule("./js/theme.js")
            .Setup<string?>("getStoredTheme")
            .SetException(new JSException("module unavailable"));

        var nav = context.Render<NavMenu>();

        Assert.Equal(5, nav.FindAll("nav.flex-column > .nav-item").Count);
        Assert.True(nav.Find("button.theme-trigger").HasAttribute("disabled"));
    }

    [Fact]
    public void TheRowBecomesUsableOnceItsModuleHasLoaded()
    {
        using var context = NewContext();

        var nav = context.Render<NavMenu>();

        Assert.False(nav.Find("button.theme-trigger").HasAttribute("disabled"));
    }

    [Fact]
    public void TheThemeStripIsGoneFromTheMarkupAndTheStylesheet()
    {
        Assert.DoesNotContain("<div class=\"theme-strip\">", Read("Layout", "MainLayout.razor"));
        Assert.DoesNotContain(".theme-strip {", Read("Layout", "MainLayout.razor.css"));

        // Nothing else in the app refers to it either.
        foreach (var file in Directory.EnumerateFiles(SourceRoot(), "*.razor*", SearchOption.AllDirectories))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                || file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            {
                continue;
            }

            var text = File.ReadAllText(file);
            Assert.False(
                Regex.IsMatch(text, @"class=""theme-strip""|^\.theme-strip", RegexOptions.Multiline),
                $"{Path.GetFileName(file)} still refers to the deleted theme strip");
        }
    }

    [Fact]
    public void MainHoldsOnlyTheContentColumnAgain()
    {
        var layout = Read("Layout", "MainLayout.razor");
        var main = Regex.Match(layout, @"<main>(.*?)</main>", RegexOptions.Singleline);
        Assert.True(main.Success, "<main> was not found in MainLayout.razor");

        // Comments aside, the only element left inside main is the article.
        var elements = Regex.Matches(Regex.Replace(main.Groups[1].Value, @"@\*.*?\*@", "", RegexOptions.Singleline), @"<(\w+)");
        Assert.Single(elements);
        Assert.Equal("article", elements[0].Groups[1].Value);
    }

    [Fact]
    public void TheTriggerRowMatchesTheHeightAndRadiusOfTheDestinationRows()
    {
        // The fourth row has to be the same shape as the three above it, and the
        // two stylesheets that decide that are separate files. This is what
        // catches one of them drifting.
        var navCss = Read("Layout", "NavMenu.razor.css");
        var triggerCss = Read("Components", "ThemeTrigger.razor.css");

        static string? Property(string css, string selector, string property)
        {
            var block = Regex.Match(css, $@"{Regex.Escape(selector)}\s*\{{(.*?)\}}", RegexOptions.Singleline);
            if (!block.Success)
            {
                return null;
            }

            var match = Regex.Match(block.Groups[1].Value, $@"(?<![\w-]){Regex.Escape(property)}:\s*([^;]+);");
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        foreach (var property in new[] { "height", "line-height", "border-radius", "padding-left", "color" })
        {
            Assert.Equal(
                Property(navCss, ".nav-item ::deep a", property),
                Property(triggerCss, ".theme-trigger", property));
        }
    }
}
