using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Bunit;
using FifaPressApp.Layout;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// Sign-out, and knowing which record you are looking at.
/// </summary>
public class SignOutTests
{
    private static BunitContext NewContext(SimulatedSessionProvider session)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(session);
        return context.WithLocale();
    }

    private static async Task<SimulatedSessionProvider> AsAminaAsync()
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync("MP-2026-04817", "amina-demo-2026");
        return session;
    }

    [Fact]
    public void SignedOutThereIsNoSignOutRowAndNoIndicator()
    {
        using var context = NewContext(new SimulatedSessionProvider(new DemoAccountStore()));

        var nav = context.Render<NavMenu>();

        Assert.Empty(nav.FindAll(".nav-signout"));
        Assert.Empty(nav.FindAll(".nav-session"));
        Assert.Equal(5, nav.FindAll("nav.flex-column > .nav-item").Count);
    }

    [Fact]
    public async Task SignedInThereIsASixthRowBelowTheThemeRow()
    {
        using var context = NewContext(await AsAminaAsync());

        var nav = context.Render<NavMenu>();
        var rows = nav.FindAll("nav.flex-column > .nav-item");

        Assert.Equal(6, rows.Count);

        // Destinations contiguous at the top, the three controls grouped at the
        // bottom in the order 11 §5.1 and 10 §4.1 set between them: language,
        // theme, sign-out last.
        Assert.NotEmpty(rows[3].QuerySelectorAll(".language-switch"));
        Assert.NotEmpty(rows[4].QuerySelectorAll("button.theme-trigger"));
        Assert.NotEmpty(rows[5].QuerySelectorAll("button.nav-signout"));
    }

    [Fact]
    public async Task TheIndicatorNamesTheHolderAndTheirCredential()
    {
        using var context = NewContext(await AsAminaAsync());

        var indicator = context.Render<NavMenu>().Find(".nav-session");

        Assert.Contains("Amina Bello", indicator.TextContent);
        Assert.Contains("MP-2026-04817", indicator.TextContent);
    }

    [Fact]
    public async Task TheIndicatorIsReadOnlyAndSitsAboveTheRows()
    {
        // It is not a menu and has no affordance beyond being read.
        using var context = NewContext(await AsAminaAsync());

        var nav = context.Render<NavMenu>();
        var indicator = nav.Find(".nav-session");

        Assert.Empty(indicator.QuerySelectorAll("a, button, select, input"));

        Assert.True(nav.Markup.IndexOf("nav-session", StringComparison.Ordinal)
                  < nav.Markup.IndexOf("nav flex-column", StringComparison.Ordinal));
    }

    [Fact]
    public async Task TheIndicatorFollowsTheSessionWithoutARenderLag()
    {
        // The session changes from the sign-in screen, which is not this
        // component's child and cannot tell it directly. Without the
        // subscription the sidebar would be a render behind.
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        using var context = NewContext(session);

        var nav = context.Render<NavMenu>();
        Assert.Empty(nav.FindAll(".nav-session"));

        await nav.InvokeAsync(() => session.SignInAsync("RH-2026-00219", "tomas-demo-2026"));

        Assert.Contains("Tomás L.", nav.Find(".nav-session").TextContent);
    }

    [Fact]
    public async Task PressingSignOutEndsTheSessionAndReturnsToTheLanding()
    {
        var session = await AsAminaAsync();
        using var context = NewContext(session);
        var navigation = context.Services.GetRequiredService<NavigationManager>();

        var nav = context.Render<NavMenu>();
        nav.Find("button.nav-signout").Click();

        Assert.False(session.IsSignedIn);
        Assert.Equal(navigation.BaseUri, navigation.Uri);
    }

    [Fact]
    public async Task SignOutAsksForNoConfirmation()
    {
        // There is nothing to lose, and a modal guarding a simulated session
        // would be theatre.
        var session = await AsAminaAsync();
        using var context = NewContext(session);

        var nav = context.Render<NavMenu>();
        nav.Find("button.nav-signout").Click();

        Assert.Empty(nav.FindAll("dialog, [role='dialog'], [role='alertdialog']"));
        Assert.False(session.IsSignedIn);
    }

    [Fact]
    public async Task TheLandingStatesThatTheSignOutHappened()
    {
        var session = await AsAminaAsync();
        session.SignOut();

        using var context = NewContext(session);
        var page = context.Render<Landing>();

        var announcement = page.Find(".landing__signed-out");
        Assert.Contains("signed out", announcement.TextContent, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("status", announcement.GetAttribute("role"));
    }

    [Fact]
    public async Task TheAnnouncementIsMadeOnceAndNotRepeatedOnARefresh()
    {
        var session = await AsAminaAsync();
        session.SignOut();

        using var first = NewContext(session);
        Assert.NotEmpty(first.Render<Landing>().FindAll(".landing__signed-out"));

        using var second = NewContext(session);
        Assert.Empty(second.Render<Landing>().FindAll(".landing__signed-out"));
    }

    [Fact]
    public void AVisitorWhoNeverSignedInIsNotToldTheySignedOut()
    {
        using var context = NewContext(new SimulatedSessionProvider(new DemoAccountStore()));

        Assert.Empty(context.Render<Landing>().FindAll(".landing__signed-out"));
    }

    [Fact]
    public void TheNavNoLongerClaimsThatSignInGatesNothing()
    {
        // It gated nothing when that comment was written. It gates the record
        // now, and a comment that has quietly become false is the same defect
        // as a notice that has.
        var source = File.ReadAllText(Path.Combine(SourceRoot(), "Layout", "NavMenu.razor"));

        Assert.DoesNotContain("it gates nothing", source);
        Assert.Contains("gates the record now", source);
    }

    [Fact]
    public async Task TheSignOutRowIsShapedLikeTheRowsAboveIt()
    {
        using var context = NewContext(await AsAminaAsync());
        var nav = context.Render<NavMenu>();

        Assert.NotEmpty(nav.FindAll("button.nav-signout"));

        var navCss = File.ReadAllText(Path.Combine(SourceRoot(), "Layout", "NavMenu.razor.css"));

        static string? Property(string css, string selector, string property)
        {
            var block = Regex.Match(css, $@"{Regex.Escape(selector)}\s*\{{(.*?)\}}", RegexOptions.Singleline);
            return block.Success
                ? Regex.Match(block.Groups[1].Value, $@"(?<![\w-]){Regex.Escape(property)}:\s*([^;]+);") is { Success: true } m
                    ? m.Groups[1].Value.Trim()
                    : null
                : null;
        }

        foreach (var property in new[] { "height", "line-height", "border-radius", "padding-left", "color" })
        {
            Assert.Equal(
                Property(navCss, ".nav-item ::deep a", property),
                Property(navCss, ".nav-signout", property));
        }
    }

    // ------------------------------------------------------- the session bar

    [Fact]
    public async Task TheSessionBarNamesTheHolderAndTheirCredential()
    {
        // Both, not just the name: the credential is the record key and the
        // thing that differs between the two demo records, so dropping it would
        // hide the value the two-record demonstration exists to expose.
        using var context = NewContext(await AsAminaAsync());

        var bar = context.Render<SessionBar>().Find(".session-bar");

        Assert.Contains("Amina Bello", bar.TextContent);
        Assert.Contains("MP-2026-04817", bar.TextContent);
    }

    [Fact]
    public async Task TheSessionBarsIndicatorIsReadOnly()
    {
        // It is a statement of fact, not a door. The door is the nav row below
        // it, and the sign-out button beside the indicator is an action rather
        // than part of what the indicator says.
        using var context = NewContext(await AsAminaAsync());

        var indicator = context.Render<SessionBar>().Find(".session-bar__holder");

        Assert.Empty(indicator.QuerySelectorAll("a, button, select, input"));
    }

    [Fact]
    public void SignedOutTheSessionBarSaysNothingAboutAnyHolderAndOffersAWayIn()
    {
        using var context = NewContext(new SimulatedSessionProvider(new DemoAccountStore()));

        var bar = context.Render<SessionBar>();

        Assert.Empty(bar.FindAll(".session-bar__holder"));
        Assert.Empty(bar.FindAll(".session-bar__signout"));

        // One way in, pointing at the conditional record surface rather than at
        // a sign-in route of its own.
        var signIn = bar.Find(".session-bar__signin");
        Assert.Equal("record", signIn.GetAttribute("href"));
    }

    [Fact]
    public async Task TheSessionBarFollowsTheSessionWithoutARenderLag()
    {
        // The session changes from a form this component does not own. Without
        // the subscription the bar would be a render behind on every screen at
        // once, which is worse than the sidebar ever was.
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        using var context = NewContext(session);

        var bar = context.Render<SessionBar>();
        Assert.Empty(bar.FindAll(".session-bar__holder"));

        await bar.InvokeAsync(() => session.SignInAsync("RH-2026-00219", "tomas-demo-2026"));

        Assert.Contains("Tomás L.", bar.Find(".session-bar__holder").TextContent);
    }

    [Fact]
    public async Task PressingSignOutInTheBarEndsTheSessionAndReturnsToTheLanding()
    {
        // The second of the two sign-out locations. Both call the same method
        // and navigate identically, which is what makes two of them a
        // convenience rather than a disagreement.
        var session = await AsAminaAsync();
        using var context = NewContext(session);
        var navigation = context.Services.GetRequiredService<NavigationManager>();

        context.Render<SessionBar>().Find(".session-bar__signout").Click();

        Assert.False(session.IsSignedIn);
        Assert.Equal(navigation.BaseUri, navigation.Uri);
    }

    [Fact]
    public void TheBarSitsAboveTheContentColumnRatherThanInsideIt()
    {
        // The whole reason the indicator moved: a row above the content column
        // is present at every breakpoint, and the sidebar is not.
        var layout = File.ReadAllText(Path.Combine(SourceRoot(), "Layout", "MainLayout.razor"));

        Assert.True(layout.IndexOf("<SessionBar />", StringComparison.Ordinal)
                  < layout.IndexOf("<article class=\"content", StringComparison.Ordinal));
    }

    private static string SourceRoot([CallerFilePath] string thisFile = "")
        => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "..", "src", "FifaPressApp"));
}
