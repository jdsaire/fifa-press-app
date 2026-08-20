using Bunit;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The Settings screen: which fields it shows, to whom, and what the one
/// action on it does.
///
/// <para>
/// The field set is the assertion that matters. Two calls were resolved before
/// this screen was specified — no Role field, because a track is a derived
/// precondition rather than a preference, and no "log out of all devices",
/// because there are no devices — and a test that only checked the fields that
/// exist would pass just as happily if either came back.
/// </para>
/// </summary>
public class SettingsScreenTests
{
    private static BunitContext NewContext(SimulatedSessionProvider session)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(session);
        return context.WithLocale();
    }

    private static SimulatedSessionProvider SignedOut() => new(new DemoAccountStore());

    private static async Task<SimulatedSessionProvider> AsAminaAsync()
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync("MP-2026-04817", "amina-demo-2026");
        return session;
    }

    [Fact]
    public void SignedOutTheScreenOffersAppearanceAndLanguageAndNothingElse()
    {
        // Public on purpose: these are choices about how the app is read, not
        // about what it will show you.
        using var context = NewContext(SignedOut());

        var page = context.Render<Settings>();

        Assert.Equal(2, page.FindAll(".settings-field").Count);
        Assert.Empty(page.FindAll(".settings__signout"));
        Assert.Empty(page.FindAll(".settings-field__value"));
    }

    [Fact]
    public void SignedOutThereIsNoPromptToSignInForMore()
    {
        // The two visible fields are complete on their own. Announcing the
        // absence of the other two would manufacture a gap that does not need
        // calling out.
        using var context = NewContext(SignedOut());

        var markup = context.Render<Settings>().Markup;

        Assert.DoesNotContain("Sign in", markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SignedInTheScreenAddsTheHoldersNameAndAWayOut()
    {
        using var context = NewContext(await AsAminaAsync());

        var page = context.Render<Settings>();

        Assert.Equal(4, page.FindAll(".settings-field").Count);
        Assert.Equal("Amina Bello", page.Find(".settings-field__value").TextContent.Trim());
        Assert.NotEmpty(page.FindAll(".settings__signout"));
    }

    [Fact]
    public async Task TheNameIsReadOnlyRatherThanAnInputThatForgetsWhatYouTyped()
    {
        // There is no account system behind it to write a new name to, so an
        // editable field would be the interface lying about what it is.
        using var context = NewContext(await AsAminaAsync());

        var page = context.Render<Settings>();
        var name = page.Find(".settings-field__value");

        Assert.Empty(name.QuerySelectorAll("input, select, textarea, button"));
        Assert.Equal("P", name.TagName);
    }

    [Fact]
    public async Task TheOtherHoldersNameAppearsWhenTheOtherHolderIsSignedIn()
    {
        // The field reads the session rather than a constant, which is the
        // whole reason two demo records exist.
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync("RH-2026-00219", "tomas-demo-2026");
        using var context = NewContext(session);

        Assert.Equal("Tomás L.", context.Render<Settings>().Find(".settings-field__value").TextContent.Trim());
    }

    [Fact]
    public async Task SigningOutEndsTheSessionAndReturnsToHome()
    {
        var session = await AsAminaAsync();
        using var context = NewContext(session);
        var navigation = context.Services.GetRequiredService<NavigationManager>();

        var page = context.Render<Settings>();
        page.Find(".settings__signout").Click();

        Assert.False(session.IsSignedIn);
        Assert.Equal(navigation.BaseUri, navigation.Uri);

        // And the screen it left behind is the signed-out one, not a stale copy
        // of the signed-in fields.
        Assert.Equal(2, page.FindAll(".settings-field").Count);
    }

    [Fact]
    public async Task SigningOutAsksForNoConfirmation()
    {
        var session = await AsAminaAsync();
        using var context = NewContext(session);

        var page = context.Render<Settings>();
        page.Find(".settings__signout").Click();

        Assert.Empty(page.FindAll("dialog, [role='dialog'], [role='alertdialog']"));
        Assert.False(session.IsSignedIn);
    }

    [Fact]
    public void ThereIsNoRoleFieldAndNoAllDevicesFraming()
    {
        // Track.cs documents that there is deliberately no way to assign a
        // track. A Settings row that let a person pick one would invent an
        // entitlement system this app does not have.
        using var context = NewContext(SignedOut());

        // Read as text rather than as markup: `role` is an ARIA attribute on
        // half the controls in the app, and a raw-markup check would be
        // asserting against the accessibility layer instead of the field set.
        var text = context.Render<Settings>().Find("h1").ParentElement!.TextContent;

        foreach (var absent in new[] { "Role", "Track", "all devices", "everywhere" })
        {
            Assert.DoesNotContain(absent, text, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void TheScreenIsReachableAtItsOwnRoute()
    {
        var route = typeof(Settings)
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Components.RouteAttribute), inherit: false)
            .Cast<Microsoft.AspNetCore.Components.RouteAttribute>()
            .Single();

        Assert.Equal("/settings", route.Template);
    }
}
