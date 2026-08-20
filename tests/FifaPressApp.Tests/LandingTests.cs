using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Bunit;
using FifaPressApp.Layout;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The public landing view, and the route move that made room for it.
/// </summary>
public class LandingTests
{
    private static BunitContext NewContext(SimulatedSessionProvider? session = null)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(session ?? new SimulatedSessionProvider(new DemoAccountStore()));
        return context.WithLocale();
    }

    private static string SourceRoot([CallerFilePath] string thisFile = "")
        => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "..", "src", "FifaPressApp"));

    [Fact]
    public void TheLandingOwnsTheRootRouteAndTheRecordHasItsOwn()
    {
        var landing = File.ReadAllText(Path.Combine(SourceRoot(), "Pages", "Landing.razor"));
        var record = File.ReadAllText(Path.Combine(SourceRoot(), "Pages", "MyAccess.razor"));

        Assert.Contains("@page \"/\"", landing);
        Assert.Contains("@page \"/record\"", record);

        // Exactly one component claims "/". Two would be a runtime ambiguity
        // that no compiler catches.
        var claimants = Directory
            .EnumerateFiles(Path.Combine(SourceRoot(), "Pages"), "*.razor")
            .Count(file => Regex.IsMatch(File.ReadAllText(file), @"@page\s+""/""\s*$", RegexOptions.Multiline));

        Assert.Equal(1, claimants);
    }

    [Fact]
    public void TheLandingSaysWhatTheAppIs()
    {
        using var context = NewContext();

        var page = context.Render<Landing>();

        Assert.Contains("2026 World Cup", page.Markup);
        Assert.Contains("accreditation", page.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TheLandingSaysItIsADemonstrationAndNotAFifaProduct()
    {
        // 10 §6.2's second item, and 09 §1's disclosure carried onto the front
        // door where it does the most work.
        using var context = NewContext();

        var notice = context.Render<Landing>().Find(".landing__disclosure");

        Assert.Contains("not a FIFA product", notice.TextContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not affiliated", notice.TextContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("simulated", notice.TextContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TheLandingOffersExactlyOneWayOn()
    {
        // It used to offer two weighted entry points, one of them into the
        // sign-in form. That stopped being the front door's job when sign-in
        // became a persistent affordance in the session row above every screen,
        // and a front door with two doors on it is the duplication this run
        // exists to remove. There is no replacement assertion for the removed
        // section, because there is no replacement section — the assertion is
        // that there is one link and it goes to the matches.
        using var context = NewContext();

        var page = context.Render<Landing>();

        Assert.Empty(page.FindAll(".landing__entry"));

        var links = page.FindAll("a");
        Assert.Single(links);
        Assert.Equal("matches", links[0].GetAttribute("href"));
        Assert.Contains("landing__cta", links[0].GetAttribute("class"));
    }

    [Fact]
    public void TheLandingDoesNotPublishTheDemoCredentials()
    {
        // 10 §6.3 resolves this deliberately: the credentials belong beside the
        // form they are typed into, and publishing them twice would create two
        // places to keep in sync.
        using var context = NewContext();

        var markup = context.Render<Landing>().Markup;

        Assert.DoesNotContain(DemoAccountStore.Amina.Password, markup);
        Assert.DoesNotContain(DemoAccountStore.Tomas.Password, markup);
        Assert.DoesNotContain(DemoAccountStore.Amina.Identifier, markup);
        Assert.DoesNotContain(DemoAccountStore.Tomas.Identifier, markup);
    }

    [Fact]
    public void TheLandingNoLongerAdvertisesWhereTheDemoAccountsAre()
    {
        // The retired counterpart of this test asserted that the landing said
        // demo accounts exist and named where to find them. That pointer moved
        // rather than disappearing: the session row's "Sign in" link is on every
        // screen, and the accounts are published beside the form it leads to.
        // Landing no longer carries a second copy of the directions, and the
        // rule below it — never publish the credentials here — is unchanged and
        // asserted separately.
        using var context = NewContext();

        var page = context.Render<Landing>();

        Assert.Empty(page.FindAll(".landing__entry"));
        Assert.DoesNotContain("demo account", page.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TheLandingIsNotAMarketingPage()
    {
        // 10 §6.4. This screen has no content of its own to defer to, which
        // makes it the easiest one in the app to over-design.
        using var context = NewContext();

        var page = context.Render<Landing>();

        Assert.Empty(page.FindAll("img"));
        Assert.Empty(page.FindAll("blockquote"));

        // No invented numbers. Years and the tournament's own name are the only
        // digits this page has any business carrying.
        var numbers = Regex
            .Matches(page.Markup.Replace("2026", ""), @"\b\d[\d,.]*\s*(%|\+|k\b|million|users|journalists)")
            .Count;
        Assert.Equal(0, numbers);
    }

    [Fact]
    public async Task ASignedInVisitorIsSentToTheirRecordRatherThanTheFrontDoor()
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync("MP-2026-04817", "amina-demo-2026");

        using var context = NewContext(session);
        var navigation = context.Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();

        context.Render<Landing>();

        Assert.EndsWith("/record", navigation.Uri);
    }

    [Fact]
    public void ASignedOutVisitorStaysOnTheLanding()
    {
        using var context = NewContext();
        var navigation = context.Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        var before = navigation.Uri;

        context.Render<Landing>();

        Assert.Equal(before, navigation.Uri);
    }

    [Fact]
    public void TheNavsFirstRowIsHomeAndItIsTheOnlyRowThatOverridesMatching()
    {
        using var context = NewContext();

        var nav = context.Render<NavMenu>();
        var rows = nav.FindAll("nav.flex-column > .nav-item");

        // Home owns the root now, and the record has a row of its own that
        // appears once there is a record to look at.
        Assert.Equal("", rows[0].QuerySelector("a")!.GetAttribute("href"));

        // The override is correct here rather than a workaround. It used to sit
        // on a row that pointed at the root while meaning the record, which is
        // why it was removed; this row points at the root and means it, and
        // without the override an empty href prefix-matches every address and
        // leaves Home active on every screen. Exactly one row carries it.
        var source = Regex.Replace(
            File.ReadAllText(Path.Combine(SourceRoot(), "Layout", "NavMenu.razor")),
            @"@\*.*?\*@",
            "",
            RegexOptions.Singleline);

        Assert.Single(Regex.Matches(source, "NavLinkMatch.All"));
        Assert.DoesNotContain("href=\"record\" Match", source);
    }

    [Fact]
    public void NoLinkInTheAppStillPointsAtTheRootExpectingTheRecord()
    {
        // "/" is the landing now. Any link that meant "the record" and still
        // says href="" would quietly send a person to the front door.
        foreach (var file in Directory.EnumerateFiles(Path.Combine(SourceRoot(), "Pages"), "*.razor"))
        {
            var text = File.ReadAllText(file);
            foreach (Match match in Regex.Matches(text, @"<a href=""""[^>]*>(?<label>[^<]*)</a>"))
            {
                Assert.DoesNotContain("record", match.Groups["label"].Value, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("access", match.Groups["label"].Value, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Fact]
    public void TheDeepRouteSurvivesADirectHitOnStaticHosting()
    {
        // /record is reached by typing it, not only by navigating to it, and
        // GitHub Pages has no server-side routing. The shim that makes deep
        // links work is generated into the publish output by CI rather than
        // tracked in wwwroot, so this asserts against the workflow — the shim
        // encodes an arbitrary path, so /record needs nothing added to it, and
        // this test is what would notice if that stopped being true.
        var repoRoot = Path.GetFullPath(Path.Combine(SourceRoot(), "..", ".."));
        var workflow = File.ReadAllText(
            Path.Combine(repoRoot, ".github", "workflows", "deploy-pages.yml"));

        Assert.Contains("404.html", workflow);
        Assert.Contains("location.pathname.slice(basePath.length)", workflow);
        Assert.Contains("history.replaceState", workflow);
    }
}
