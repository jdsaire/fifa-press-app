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
        await session.SignInAsync("demo_staff1", "amina-demo-2026");
        return session;
    }

    [Fact]
    public void TheNavCarriesNoSessionControlsAtAllAnyMore()
    {
        // The indicator and the sign-out row both left: the first because a
        // collapsed sidebar answered "which record am I looking at" nowhere,
        // the second because it follows the indicator. What is left is four
        // destinations, and a signed-out visitor sees exactly those.
        using var context = NewContext(new SimulatedSessionProvider(new DemoAccountStore()));

        var nav = context.Render<NavMenu>();

        Assert.Empty(nav.FindAll(".nav-signout"));
        Assert.Empty(nav.FindAll(".nav-session"));
        Assert.Empty(nav.FindAll(".language-switch"));
        Assert.Empty(nav.FindAll("button.theme-trigger"));

        Assert.Equal(4, nav.FindAll("nav.flex-column > .nav-item").Count);
    }

    [Fact]
    public async Task SignedInThereIsAFifthRowAndItIsADestination()
    {
        // The row a session adds is not a control in a tail — the tail is gone.
        // It is the requests dashboard, sitting among the destinations because
        // that is what it is.
        using var context = NewContext(await AsAminaAsync());

        var nav = context.Render<NavMenu>();
        var rows = nav.FindAll("nav.flex-column > .nav-item");

        Assert.Equal(5, rows.Count);

        Assert.Equal(
            ["", "matches", "record", "help", "settings"],
            rows.Select(row => row.QuerySelector("a")!.GetAttribute("href")));
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

        await bar.InvokeAsync(() => session.SignInAsync("demo_staff2", "tomas-demo-2026"));

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
    public async Task SignOutFromTheBarAsksForNoConfirmation()
    {
        // There is nothing to lose, and a modal guarding a simulated session
        // would be theatre. Carried over from the retired nav row, which is
        // where this guarantee used to be asserted.
        var session = await AsAminaAsync();
        using var context = NewContext(session);

        var bar = context.Render<SessionBar>();
        bar.Find(".session-bar__signout").Click();

        Assert.Empty(bar.FindAll("dialog, [role='dialog'], [role='alertdialog']"));
        Assert.False(session.IsSignedIn);
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
