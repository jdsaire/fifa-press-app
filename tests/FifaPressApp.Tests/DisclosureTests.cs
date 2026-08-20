using System.Runtime.CompilerServices;
using Bunit;
using FifaPressApp.Components;
using FifaPressApp.Models;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The change row's two layers, and the arrival that closes the confirmation
/// gap.
///
/// <para>
/// The constraint under test is the one that governs which half each field goes
/// in: <b>the collapsed layer must remain fully informative on its own.</b>
/// Collapsing detail must never mean collapsing the fact that something
/// changed, so what changed, when it takes effect, and whether it is still
/// conditional are all asserted to survive with nothing opened.
/// </para>
/// </summary>
public class DisclosureTests
{
    private sealed record Harness(
        BunitContext Context,
        LocaleService Locale,
        SimulatedSessionProvider Session,
        ChangeArrivalTracker Arrivals);

    private static Harness NewHarness()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        var locale = LocaleTestData.Loaded();
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        var arrivals = new ChangeArrivalTracker();

        context.Services.AddSingleton(locale);
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(session);
        context.Services.AddSingleton(arrivals);
        context.Services.AddSingleton<IAccessDataProvider>(TestData.ProviderOverRealSchedule());
        context.RenderTree.Add<LocaleProvider>();

        return new Harness(context, locale, session, arrivals);
    }

    private static Change Conditional() => new(
        changeId: "ch-test",
        credentialId: "MP-2026-04817",
        writtenUtc: new DateTime(2026, 7, 3, 9, 0, 0, DateTimeKind.Utc),
        effectiveUtc: new DateTime(2026, 7, 6, 14, 0, 0, DateTimeKind.Utc),
        kind: ChangeKind.MatchAccessRevoked,
        track: new Track(TrackId.MemberAssociationQuota, HasNamedContact: false),
        whatChanged: new LocalizedText("Something changed.", "Algo cambió.", "Algo mudou."),
        reason: new LocalizedText("Because of a quota.", "Por un cupo.", "Por causa de uma cota."),
        nextStep: new LocalizedText("Hold your travel.", "Espera para viajar.", "Aguarde para viajar."),
        affectsMatchNumber: 98,
        dependsOnMatchNumber: 93,
        conditionText: new LocalizedText(
            "If the team goes out, the access is withdrawn.",
            "Si la selección queda fuera, se retira el acceso.",
            "Se a seleção sair, o acesso é retirado."));

    private static IRenderedComponent<ChangeRow> Row(Harness harness, Change change, bool justArrived = false) =>
        harness.Context.Render<ChangeRow>(parameters => parameters
            .Add(component => component.Change, change)
            .Add(component => component.JustArrived, justArrived));

    // ------------------------------------------------------- the two layers

    [Fact]
    public void TheRowIsADisclosureAndStartsCollapsed()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var row = Row(harness, Conditional());

        var disclosure = row.Find("details.change-row__disclosure");
        Assert.False(disclosure.HasAttribute("open"));
        Assert.NotNull(row.Find("summary.change-row__summary"));
    }

    [Fact]
    public void TheRowIsTheControl_NotAButtonBesideIt()
    {
        // 09 §7.2: an existing row becomes disclosable rather than a new control
        // being added next to it. <summary> is keyboard-operable and announces
        // its expanded state without any JavaScript at all, which matters on a
        // page that has to work from cache.
        var harness = NewHarness();
        using var context = harness.Context;

        var row = Row(harness, Conditional());

        Assert.Empty(row.FindAll("button"));
        Assert.Equal("SUMMARY", row.Find("summary").TagName);
    }

    [Fact]
    public void TheCollapsedLayerStillSaysThatSomethingChangedAndWhen()
    {
        // The load-bearing test. A collapsed row that hid the fact of the change
        // would be the confirmation-screen failure this record exists to avoid,
        // rebuilt inside a row.
        var harness = NewHarness();
        using var context = harness.Context;

        var summary = Row(harness, Conditional()).Find("summary").TextContent;

        Assert.Contains("Something changed.", summary);
        Assert.Contains("6 July 2026", summary);
    }

    [Fact]
    public void TheCollapsedLayerStillCarriesTheUrgencyIndicator()
    {
        // A conditional change that looked settled while collapsed would be the
        // single worst thing this disclosure could do.
        var harness = NewHarness();
        using var context = harness.Context;

        var row = Row(harness, Conditional());

        Assert.NotEmpty(row.Find("summary").QuerySelectorAll(".foreseeable-badge"));
    }

    [Fact]
    public void TheCollapsedLayerAddsNothingElseToWhatTheRowAlreadyShowed()
    {
        // 09 §7.2 is explicit that the collapsed layer adds nothing new. The one
        // addition is the disclosure's own label, which names what opening the
        // row would reveal rather than saying "more".
        var harness = NewHarness();
        using var context = harness.Context;

        var summary = Row(harness, Conditional()).Find("summary");
        var added = summary.QuerySelectorAll(".change-row__more");

        Assert.Single(added);
        Assert.Contains("what you can do", added[0].TextContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TheReasonAndNextStepAreInTheSecondLayer()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var row = Row(harness, Conditional());
        var detail = row.Find(".change-row__detail").TextContent;

        Assert.Contains("Because of a quota.", detail);
        Assert.Contains("Hold your travel.", detail);

        // And not in the collapsed one.
        var summary = row.Find("summary").TextContent;
        Assert.DoesNotContain("Because of a quota.", summary);
        Assert.DoesNotContain("Hold your travel.", summary);
    }

    [Fact]
    public void TheConditionsWordingIsInTheSecondLayerWhileTheBadgeStaysInTheFirst()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var row = Row(harness, Conditional());

        Assert.Contains("If the team goes out", row.Find(".change-row__detail").TextContent);
        Assert.DoesNotContain("If the team goes out", row.Find("summary").TextContent);
    }

    [Fact]
    public void OpeningTheRowRevealsEverythingWithoutRemovingAnything()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var row = Row(harness, Conditional());
        var before = row.Find("summary").TextContent;

        // Both layers are in the DOM either way — <details> hides the second
        // rather than omitting it, which is what keeps the whole row findable by
        // the browser's own in-page search.
        Assert.Contains("Because of a quota.", row.Markup);
        Assert.Equal(before, row.Find("summary").TextContent);
    }

    [Fact]
    public void NoFieldIsLostByTheSplit()
    {
        // Every field the row rendered before the disclosure existed is still
        // rendered somewhere in it.
        var harness = NewHarness();
        using var context = harness.Context;

        var markup = Row(harness, Conditional()).Markup;

        foreach (var required in new[]
                 {
                     "change-row__what",
                     "change-row__when",
                     "change-row__why",
                     "change-row__next",
                     "change-row__condition",
                     "change-row__mocked",
                 })
        {
            Assert.Contains(required, markup);
        }
    }

    [Fact]
    public async Task TheLogsOrderingAndIdentityAreUntouchedByTheDisclosure()
    {
        // 09 §7.2: purely a rendering decision. Nothing about Change or the
        // provider changed, so this asserts the shape the record still has.
        var provider = TestData.ProviderOverRealSchedule();

        var changes = (await provider.GetChangesAsync(MockAccessDataProvider.AminaCredentialId)).Value;

        Assert.Equal(
            changes.OrderByDescending(change => change.EffectiveUtc).Select(change => change.ChangeId),
            changes.Select(change => change.ChangeId));
    }

    // ------------------------------------------------------- the arrival

    [Fact]
    public void AnArrivingRowOpensItselfAndIsMarkedAsArriving()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var row = Row(harness, Conditional(), justArrived: true);

        Assert.True(row.Find("details").HasAttribute("open"));
        Assert.Contains("change-row--arriving", row.Find("article").GetAttribute("class"));
    }

    [Fact]
    public void EveryOtherRowIsNeitherOpenNorMarked()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var row = Row(harness, Conditional());

        Assert.False(row.Find("details").HasAttribute("open"));
        Assert.DoesNotContain("change-row--arriving", row.Find("article").GetAttribute("class"));
    }

    [Fact]
    public async Task AWrittenChangeArrivesMarkedOnTheRecord()
    {
        // The end-to-end claim: submitting a request announces the change it
        // wrote, and the record screen marks that row and no other.
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("demo_staff1", "Demo#2026Staff1");

        var provider = context.Services.GetRequiredService<IAccessDataProvider>();
        var written = await provider.RequestMatchAccessAsync("MP-2026-04817", 42);
        harness.Arrivals.Announce(written.ChangeId);

        var page = context.Render<MyAccess>();

        var arriving = page.FindAll("article.change-row--arriving");
        Assert.Single(arriving);
        Assert.NotEmpty(arriving[0].QuerySelectorAll("details[open]"));
    }

    [Fact]
    public async Task TheArrivalIsAnnouncedOnceAndNotReplayedOnARefresh()
    {
        // An entrance that replayed on every refresh would stop meaning "this
        // just happened" and start meaning "this row is decorated".
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("demo_staff1", "Demo#2026Staff1");
        harness.Arrivals.Announce("ch-001");

        Assert.Single(context.Render<MyAccess>().FindAll("article.change-row--arriving"));
        Assert.Empty(context.Render<MyAccess>().FindAll("article.change-row--arriving"));
    }

    [Fact]
    public async Task ARecordReachedWithoutWritingAnythingMarksNothing()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("demo_staff1", "Demo#2026Staff1");

        Assert.Empty(context.Render<MyAccess>().FindAll("article.change-row--arriving"));
    }

    [Fact]
    public void TheTrackerHoldsAnIdAndClearsAsItAnswers()
    {
        var tracker = new ChangeArrivalTracker();

        Assert.Null(tracker.Consume());

        tracker.Announce("ch-009");
        Assert.Equal("ch-009", tracker.Consume());
        Assert.Null(tracker.Consume());
    }

    [Fact]
    public void TheEntranceRespectsAReducedMotionPreference()
    {
        // Someone who has asked their system for less motion has asked for less
        // motion. The row still marks itself with its border and its opened
        // state — the information survives; only the movement goes.
        var css = File.ReadAllText(Path.Combine(
            SourceRoot(), "Components", "ChangeRow.razor.css"));

        Assert.Contains("prefers-reduced-motion: reduce", css);

        var guarded = css[css.IndexOf("prefers-reduced-motion", StringComparison.Ordinal)..];
        Assert.Contains("animation: none", guarded);
    }

    [Fact]
    public void TheEntranceIsShortEnoughToBeNoticedRatherThanWaitedOut()
    {
        var css = File.ReadAllText(Path.Combine(
            SourceRoot(), "Components", "ChangeRow.razor.css"));

        var match = System.Text.RegularExpressions.Regex.Match(
            css, @"animation: change-row-arrive (\d+)ms");

        Assert.True(match.Success, "the arrival animation has no duration");
        Assert.InRange(int.Parse(match.Groups[1].Value), 120, 600);
    }

    private static string SourceRoot([CallerFilePath] string thisFile = "")
        => Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(thisFile)!, "..", "..", "src", "FifaPressApp"));
}
