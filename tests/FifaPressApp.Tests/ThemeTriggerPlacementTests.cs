using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Bunit;
using FifaPressApp.Components;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The appearance control: where it lives, what it does, and what went away
/// when it moved.
///
/// <para>
/// This file used to be about placement — the theme trigger's move out of a
/// strip above the content column and into the sidebar's nav list. Both of
/// those homes are now gone, and asserting a placement among nav rows against a
/// control that is no longer in a list would be force-fitting an old test onto
/// a new shape. What survives is everything that was ever about behaviour: the
/// control renders disabled rather than absent while its module loads, becomes
/// usable once it arrives, carries no active state because it is not a
/// destination, and pairs an icon-free label with what pressing it does.
/// </para>
///
/// <para>
/// One assertion is genuinely new, because one code path is: "System" is the
/// first caller `clearStoredTheme()` has ever had.
/// </para>
/// </summary>
public class ThemeTriggerPlacementTests
{
    private static string SourceRoot([CallerFilePath] string thisFile = "")
        => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "..", "src", "FifaPressApp"));

    private static string Read(params string[] parts)
        => File.ReadAllText(Path.Combine(SourceRoot(), Path.Combine(parts)));

    private static BunitContext NewContext()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new SimulatedSessionProvider(new DemoAccountStore()));
        return context.WithLocale();
    }

    [Fact]
    public void TheControlOffersThreeStatesRatherThanATwoWayToggle()
    {
        // The mechanism this gate actually adds. Its predecessor was a strict
        // binary toggle, and "system" was a state the app could be in but never
        // one a person could choose.
        using var context = NewContext();

        var options = context.Render<AppearanceControl>().FindAll(".appearance-option");

        Assert.Equal(3, options.Count);
        Assert.Equal(["System", "Light", "Dark"], options.Select(option => option.TextContent.Trim()));
    }

    [Fact]
    public void TheControlIsNotANavLinkAndCarriesNoActiveState()
    {
        using var context = NewContext();

        var options = context.Render<AppearanceControl>().FindAll(".appearance-option");

        foreach (var option in options)
        {
            Assert.False(option.HasAttribute("href"));
            Assert.DoesNotContain("active", option.GetAttribute("class") ?? string.Empty);
        }

        // And no rule anywhere grants one.
        Assert.DoesNotContain(".appearance-option.active", Read("wwwroot", "css", "app.css"));
    }

    [Fact]
    public void TheCurrentStateIsMarkedRatherThanRemoved()
    {
        // The same reasoning the language control's aria-pressed carried: the
        // field keeps its shape, and a screen reader is told which state is
        // active instead of inferring it from what is missing.
        using var context = NewContext();

        var control = context.Render<AppearanceControl>();

        Assert.Single(control.FindAll(".appearance-option[aria-pressed='true']"));
        Assert.Equal(3, control.FindAll(".appearance-option").Count);
    }

    [Fact]
    public void TheFieldIsStillRendered_DisabledWhenItsModuleNeverArrives()
    {
        // Rendering disabled buttons rather than no buttons is what stops the
        // screen changing height the moment the app finishes starting — and if
        // the module never loads at all, the app still renders in the system
        // theme and only the ability to override it is lost. That degrades
        // quietly; it does not remove a field from the settings screen.
        using var context = NewContext();
        context.JSInterop
            .SetupModule("./js/theme.js")
            .Setup<string?>("getStoredTheme")
            .SetException(new JSException("module unavailable"));

        var control = context.Render<AppearanceControl>();

        Assert.Equal(3, control.FindAll(".appearance-option").Count);
        Assert.All(control.FindAll(".appearance-option"), option => Assert.True(option.HasAttribute("disabled")));
    }

    [Fact]
    public void TheFieldBecomesUsableOnceItsModuleHasLoaded()
    {
        using var context = NewContext();

        var control = context.Render<AppearanceControl>();

        Assert.All(control.FindAll(".appearance-option"), option => Assert.False(option.HasAttribute("disabled")));
    }

    [Fact]
    public void ChoosingSystemClearsTheStoredChoiceRatherThanStoringAThirdValue()
    {
        // The one new code path this gate introduces, and the first caller
        // clearStoredTheme() has ever had. "System" is not a third stored
        // value: a null stored value is what *means* "defer to the system", so
        // choosing it clears rather than writes.
        using var context = NewContext();
        var module = context.JSInterop.SetupModule("./js/theme.js");
        module.Setup<string?>("getStoredTheme").SetResult("dark");
        module.SetupVoid("applyTheme", _ => true).SetVoidResult();
        module.SetupVoid("storeTheme", _ => true).SetVoidResult();
        var cleared = module.SetupVoid("clearStoredTheme");

        var control = context.Render<AppearanceControl>();
        control.FindAll(".appearance-option")[0].Click();

        cleared.SetVoidResult();
        Assert.Single(context.JSInterop.Invocations["clearStoredTheme"]);

        // Nothing was stored in its place.
        Assert.Empty(context.JSInterop.Invocations["storeTheme"]);

        // And the override comes off the document, so the stylesheet's own
        // media query decides again without a reload.
        Assert.Contains(context.JSInterop.Invocations["applyTheme"],
            invocation => invocation.Arguments.Count == 1 && invocation.Arguments[0] is null);
    }

    [Fact]
    public void ChoosingLightOrDarkAppliesAndStoresIt()
    {
        using var context = NewContext();
        var module = context.JSInterop.SetupModule("./js/theme.js");
        module.Setup<string?>("getStoredTheme").SetResult(null);
        module.SetupVoid("applyTheme", _ => true).SetVoidResult();
        module.SetupVoid("storeTheme", _ => true).SetVoidResult();

        var control = context.Render<AppearanceControl>();
        control.FindAll(".appearance-option")[2].Click();

        // Applied and stored together: storing is what makes the choice outlast
        // the session, applying is what makes it outlast a system flip during
        // one.
        Assert.Contains(context.JSInterop.Invocations["storeTheme"],
            invocation => (string?)invocation.Arguments[0] == "dark");
        Assert.Contains(context.JSInterop.Invocations["applyTheme"],
            invocation => (string?)invocation.Arguments[0] == "dark");
    }

    [Fact]
    public void AStoredChoiceIsWhatTheFieldReportsOnArrival()
    {
        using var context = NewContext();
        var module = context.JSInterop.SetupModule("./js/theme.js");
        module.Setup<string?>("getStoredTheme").SetResult("light");
        module.SetupVoid("applyTheme", _ => true).SetVoidResult();

        var control = context.Render<AppearanceControl>();

        Assert.Equal("Light", control.Find(".appearance-option[aria-pressed='true']").TextContent.Trim());
    }

    [Fact]
    public void TheThemeStripIsGoneFromTheMarkupAndTheStylesheet()
    {
        Assert.DoesNotContain("<div class=\"theme-strip\">", Read("Layout", "MainLayout.razor"));
        Assert.DoesNotContain(".theme-strip {", Read("Layout", "MainLayout.razor.css"));

        // Nothing else in the app refers to it either — and nor, now, to the
        // binary trigger the tri-state control replaced.
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
            Assert.False(
                Regex.IsMatch(text, @"<ThemeTrigger\s*/>|theme-trigger"),
                $"{Path.GetFileName(file)} still refers to the deleted theme trigger");
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

    [Fact]
    public void TheControlLivesOnTheSettingsScreen()
    {
        // Where it went, asserted where it can actually be observed: rendered
        // by the screen, rather than as a position in a list it is no longer in.
        using var context = NewContext();

        Assert.NotEmpty(context.Render<Settings>().FindAll(".appearance-option"));
    }
}
