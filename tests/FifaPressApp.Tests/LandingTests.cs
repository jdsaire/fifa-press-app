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
    public void TheLandingOffersBothEntryPoints()
    {
        using var context = NewContext();

        var page = context.Render<Landing>();

        Assert.Equal(2, page.FindAll(".landing__entry").Count);
        Assert.NotEmpty(page.FindAll("a[href='signin']"));
        Assert.NotEmpty(page.FindAll("a[href='matches']"));
        Assert.NotEmpty(page.FindAll("a[href='help']"));
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
    public void TheLandingSaysDemoAccountsExistAndWhereTheyAre()
    {
        using var context = NewContext();

        var entry = context.Render<Landing>().Find(".landing__entry");

        Assert.Contains("demo account", entry.TextContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("sign-in", entry.TextContent, StringComparison.OrdinalIgnoreCase);
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
    public void TheNavsFirstRowPointsAtTheRecordWithoutAMatchOverride()
    {
        using var context = NewContext();

        var nav = context.Render<NavMenu>();
        var first = nav.FindAll("nav.flex-column > .nav-item")[0].QuerySelector("a")!;

        Assert.Equal("record", first.GetAttribute("href"));

        // The override existed only to work around an empty href prefix-matching
        // every address. A real path does not need it, and leaving it would make
        // the row active only on an exact match. Comments are stripped first —
        // the file explains the override it no longer uses.
        var source = Regex.Replace(
            File.ReadAllText(Path.Combine(SourceRoot(), "Layout", "NavMenu.razor")),
            @"@\*.*?\*@",
            "",
            RegexOptions.Singleline);

        Assert.DoesNotContain("NavLinkMatch.All", source);
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
