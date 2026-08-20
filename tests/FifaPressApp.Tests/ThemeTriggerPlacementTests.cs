using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Bunit;
using FifaPressApp.Components;
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
    public void TheTriggerIsNotANavLinkAndCarriesNoActiveState()
    {
        using var context = NewContext();

        var trigger = context.Render<ThemeTrigger>().Find("button.theme-trigger");

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

        var trigger = context.Render<ThemeTrigger>().Find("button.theme-trigger");

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

        var trigger = context.Render<ThemeTrigger>();

        Assert.NotEmpty(trigger.FindAll("button.theme-trigger"));
        Assert.True(trigger.Find("button.theme-trigger").HasAttribute("disabled"));
    }

    [Fact]
    public void TheRowBecomesUsableOnceItsModuleHasLoaded()
    {
        using var context = NewContext();

        var trigger = context.Render<ThemeTrigger>();

        Assert.False(trigger.Find("button.theme-trigger").HasAttribute("disabled"));
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
    public void MainHoldsTheSessionRowAndTheContentColumnAndNothingElse()
    {
        // This test used to assert that main held the content column alone,
        // after the theme strip was deleted from it. The strip is still gone —
        // the assertion above and its stylesheet check both still hold — and
        // what sits there now is a different element for a different reason:
        // the session row reports state rather than changing it, and it exists
        // precisely because it survives the breakpoint the sidebar does not.
        var layout = Read("Layout", "MainLayout.razor");
        var main = Regex.Match(layout, @"<main>(.*?)</main>", RegexOptions.Singleline);
        Assert.True(main.Success, "<main> was not found in MainLayout.razor");

        var body = Regex.Replace(main.Groups[1].Value, @"@\*.*?\*@", "", RegexOptions.Singleline);
        var elements = Regex.Matches(body, @"<(\w+)").Select(match => match.Groups[1].Value).ToList();

        Assert.Equal(["div", "SessionBar", "article"], elements);
        Assert.Contains("class=\"top-row", body);
    }

}
