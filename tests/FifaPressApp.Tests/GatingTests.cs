using Bunit;
using FifaPressApp.Models;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// What is gated, what stays public, and what the gate is never allowed to say.
///
/// <para>
/// The reversal under test is narrow: the personal record is gated and
/// everything else stays public. Help in particular must stay readable — it is
/// the terminal route for the no-cache path and the page that explains the
/// boundary, so gating it would be the single worst gating decision available.
/// </para>
/// </summary>
public class GatingTests
{
    private static BunitContext NewContext(SimulatedSessionProvider session)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new ChangeArrivalTracker());
        context.Services.AddSingleton(session);
        context.Services.AddSingleton<IAccessDataProvider>(TestData.ProviderOverRealSchedule());
        return context.WithLocale();
    }

    private static SimulatedSessionProvider SignedOut() => new(new DemoAccountStore());

    private static async Task<SimulatedSessionProvider> SignedInAsync(string identifier, string password)
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync(identifier, password);
        return session;
    }

    private static Task<SimulatedSessionProvider> AsAminaAsync() =>
        SignedInAsync("demo_staff1", "Demo#2026Staff1");

    private static Task<SimulatedSessionProvider> AsTomasAsync() =>
        SignedInAsync("demo_staff2", "Demo#2026Staff2");

    // ------------------------------------------------------------ the record

    [Fact]
    public void TheRecordIsNotReachableSignedOut_AndOffersSignInInPlace()
    {
        // Rewritten rather than adjusted, because there is no navigation left
        // to assert. This used to check that a signed-out visitor was sent to
        // /signin; the record now renders the sign-in experience where the
        // record would have been. The behaviour it protects — a signed-out
        // visitor never sees record content — is unchanged and asserted below
        // and in the test after it.
        using var context = NewContext(SignedOut());
        var navigation = context.Services.GetRequiredService<NavigationManager>();
        var before = navigation.Uri;

        var page = context.Render<MyAccess>();

        // The form is here, in place, and nobody was moved to get to it.
        Assert.NotEmpty(page.FindAll("form.signin__form"));
        Assert.NotEmpty(page.FindAll(".signin__notice"));
        Assert.Equal(before, navigation.Uri);

        // And the retired route is not linked from anywhere on it.
        Assert.Empty(page.FindAll("a[href='signin']"));
    }

    [Fact]
    public void TheSignedOutRecordShowsNoRecordContentAtAll()
    {
        // The other half of what the redirect used to guarantee: rendering the
        // form in place must not mean rendering it *alongside* a record.
        //
        // Asserted against the record's own sections rather than against the
        // holders' names, because the form publishes both demo accounts by
        // design — "Amina Bello" appearing here is the credentials list doing
        // its job, not the record leaking.
        using var context = NewContext(SignedOut());

        var page = context.Render<MyAccess>();

        Assert.Empty(page.FindAll(".access-card"));
        Assert.Empty(page.FindAll(".my-access__changes-heading"));
        Assert.Empty(page.FindAll(".my-access__empty"));
        Assert.Empty(page.FindAll(".my-access__error"));
        Assert.DoesNotContain("What changed", page.Markup);
    }

    [Fact]
    public void TheSignedOutRecordNeverShowsTheEmptyRecordState()
    {
        // "We have no record of you" and "you have not said who you are" are
        // different facts, and a person who confuses them will wait for a
        // notification that is never coming.
        using var context = NewContext(SignedOut());

        var markup = context.Render<MyAccess>().Markup;

        Assert.DoesNotContain("No accreditation record yet", markup);
        Assert.DoesNotContain("Nothing has been issued", markup);
    }

    [Fact]
    public async Task TheRecordRendersTheSignedInHoldersOwnState()
    {
        using var context = NewContext(await AsAminaAsync());

        var page = context.Render<MyAccess>();

        Assert.Contains("Amina Bello", page.Markup);
        Assert.Contains("MP-2026-04817", page.Markup);
        Assert.DoesNotContain("Tomás L.", page.Markup);
    }

    [Fact]
    public async Task SigningInAsTheOtherHolderShowsTheOtherRecord()
    {
        // The demonstration, at the level a person performs it: sign out, sign
        // in as the other, see a different record.
        using var context = NewContext(await AsTomasAsync());

        var page = context.Render<MyAccess>();

        Assert.Contains("Tomás L.", page.Markup);
        Assert.Contains("RH-2026-00219", page.Markup);
        Assert.DoesNotContain("Amina Bello", page.Markup);
    }

    [Fact]
    public async Task EachHoldersRecordShowsOnlyTheirOwnChanges()
    {
        using var amina = NewContext(await AsAminaAsync());
        using var tomas = NewContext(await AsTomasAsync());

        var hers = amina.Render<MyAccess>().Markup;
        var his = tomas.Render<MyAccess>().Markup;

        // Her ch-002 wording and his ch-006 wording are each unique to one
        // record; neither may appear on the other.
        Assert.Contains("Pitch-side photo position", hers);
        Assert.DoesNotContain("Pitch-side photo position", his);

        Assert.Contains("Broadcast position confirmed", his);
        Assert.DoesNotContain("Broadcast position confirmed", hers);
    }

    // ------------------------------------------------------- the write path

    [Fact]
    public void TheRequestFormIsNotReachableSignedOut()
    {
        using var context = NewContext(SignedOut());
        var navigation = context.Services.GetRequiredService<NavigationManager>();

        var page = context.Render<Registration>(parameters => parameters.Add(p => p.Id, 42));

        Assert.NotEmpty(page.FindAll(".request__signed-out"));
        Assert.Empty(page.FindAll("form"));

        // Retargeted with the route: /signin no longer exists, and /record is
        // where the sign-in experience lives now.
        Assert.EndsWith("/record", navigation.Uri);
    }

    [Fact]
    public async Task TheRequestFormIsReachableSignedIn()
    {
        using var context = NewContext(await AsAminaAsync());

        var page = context.Render<Registration>(parameters => parameters.Add(p => p.Id, 42));

        Assert.Empty(page.FindAll(".request__signed-out"));
        Assert.NotEmpty(page.FindAll("form"));
    }

    // ------------------------------------------------------ what stays public

    [Fact]
    public void HelpIsPublic()
    {
        // Non-negotiable. Help is the terminal route for the offline path and
        // the page that states the boundary; gating it would gate the
        // explanation of the gate.
        using var context = NewContext(SignedOut());

        var page = context.Render<Help>();

        Assert.Contains("<h1", page.Markup);

        // This used to assert that the string "signin" was absent. That string
        // now names a route that does not exist, so the check could no longer
        // fail whatever Help did. The obvious swap — look for "record" instead —
        // does not work either: Help's own prose uses the word ("an empty record
        // here means...") and would fail on its copy rather than on its
        // behaviour.
        //
        // So it asserts what it always meant: Help offers no way in and no gate
        // of its own. It is the terminal route for the offline path and the page
        // that explains the boundary, and it must never become something you
        // have to sign in to read.
        Assert.Empty(page.FindAll("a[href='record']"));
        Assert.Empty(page.FindAll("form.signin__form"));
        Assert.Empty(page.FindAll(".signin__notice"));
    }

    [Fact]
    public void TheMatchListIsPublic()
    {
        // Public means it renders, in full, with no session — not that the word
        // "sign in" never appears on it. Since the request control became
        // session-gated, an unplayed fixture offers a signed-out visitor a way
        // in where the request button would be, which is the opposite of
        // gating: the page states the offer rather than hiding the fixture.
        //
        // The old assertion — no "Sign in" anywhere in the markup — would now
        // pass or fail depending on whether the first page happened to hold
        // played fixtures, which is not something this test means to be about.
        using var context = NewContext(SignedOut());

        var page = context.Render<EventList>();

        Assert.NotEmpty(page.FindAll(".matches__item"));
        Assert.Empty(page.FindAll(".my-access__signed-out"));

        // No personal state leaks onto a public surface.
        Assert.DoesNotContain("Amina Bello", page.Markup);
        Assert.DoesNotContain("MP-2026-04817", page.Markup);
    }

    [Fact]
    public void TheMatchListOffersAWayInRatherThanARequestWhenSignedOut()
    {
        // The replacement for what the assertion above used to imply, asserted
        // where it is actually true: on an unplayed fixture, which is the only
        // kind that has a request path to gate.
        using var context = NewContext(SignedOut());

        var page = context.Render<EventList>();
        page.Find("select#matches-status").Change(nameof(MatchStatusFilter.NotYetPlayed));

        Assert.Empty(page.FindAll("a[href^='request/']"));
        Assert.NotEmpty(page.FindAll("a[href='record']"));
    }

    [Fact]
    public async Task TheMatchDetailIsPublicAndFinishesLoadingWhenSignedOut()
    {
        // The regression this exists for: an early return on the signed-out
        // path that skipped the "loaded" flag would freeze a public page on
        // "Loading this match…" forever.
        using var context = NewContext(SignedOut());

        var page = context.Render<EventDetails>(parameters => parameters.Add(p => p.Id, 22));

        Assert.DoesNotContain("Loading this match", page.Markup);
        Assert.NotEmpty(page.FindAll(".detail__kickoff"));
    }

    [Fact]
    public void TheMatchDetailStatesTheRequestAffordanceRatherThanHidingIt()
    {
        using var context = NewContext(SignedOut());

        var page = context.Render<EventDetails>(parameters => parameters.Add(p => p.Id, 22));

        // The section still exists and says what it needs; it is not a hidden
        // or dead control. Its link retargets with the route.
        Assert.NotEmpty(page.FindAll(".detail__signed-out"));
        Assert.NotEmpty(page.FindAll("a[href='record']"));

        // And no personal state leaks into it.
        Assert.Empty(page.FindAll(".detail__status"));
        Assert.Empty(page.FindAll(".detail__gate"));
    }

    [Fact]
    public async Task TheMatchDetailShowsPersonalStateOnceSignedIn()
    {
        using var context = NewContext(await AsAminaAsync());

        var page = context.Render<EventDetails>(parameters => parameters.Add(p => p.Id, 22));

        Assert.Empty(page.FindAll(".detail__signed-out"));
        Assert.NotEmpty(page.FindAll(".detail__status"));
        Assert.NotEmpty(page.FindAll(".detail__gate"));
    }

    // --------------------------------------------- what the gate must not say

    [Theory]
    [InlineData("403")]
    [InlineData("Access denied")]
    [InlineData("Unauthorized")]
    [InlineData("Forbidden")]
    [InlineData("Permission denied")]
    public void NoGatedSurfaceBorrowsTheVocabularyOfRealSecurity(string forbidden)
    {
        using var record = NewContext(SignedOut());
        using var request = NewContext(SignedOut());
        using var detail = NewContext(SignedOut());

        var surfaces = new[]
        {
            record.Render<MyAccess>().Markup,
            request.Render<Registration>(parameters => parameters.Add(p => p.Id, 42)).Markup,
            detail.Render<EventDetails>(parameters => parameters.Add(p => p.Id, 22)).Markup,
        };

        Assert.All(surfaces, markup =>
            Assert.DoesNotContain(forbidden, markup, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void EveryGatedSurfaceSaysWhyRatherThanJustRefusing()
    {
        using var record = NewContext(SignedOut());
        using var request = NewContext(SignedOut());

        // The request writes to a holder's record, and says so. The record's own
        // reason is now the first sentence of the notice inside the form it
        // renders in place — the sentence that used to say it moved there with
        // the branch it belonged to, so this asserts the reason a reader
        // actually meets rather than the wording that used to carry it.
        Assert.Contains("simulated sign-in", record.Render<MyAccess>().Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "writes to a holder's own record",
            request.Render<Registration>(parameters => parameters.Add(p => p.Id, 42)).Markup);
    }

    [Fact]
    public async Task AWriteLandsOnTheSignedInHoldersRecordAndNobodyElses()
    {
        // The seam that matters: the request form writes against whatever
        // credential the session holds.
        var session = await AsTomasAsync();
        var provider = TestData.ProviderOverRealSchedule();

        var written = await provider.RequestMatchAccessAsync(session.CredentialId!, 99);

        Assert.Equal(MockAccessDataProvider.TomasCredentialId, written.CredentialId);

        var his = (await provider.GetChangesAsync(MockAccessDataProvider.TomasCredentialId)).Value;
        var hers = (await provider.GetChangesAsync(MockAccessDataProvider.AminaCredentialId)).Value;

        Assert.Contains(his, change => change.ChangeId == written.ChangeId);
        Assert.DoesNotContain(hers, change => change.ChangeId == written.ChangeId);
    }

    [Fact]
    public async Task TheWithholdingRuleStillHoldsOnAGatedRecord()
    {
        // Gating changed who sees the record. It did not change what the record
        // is allowed to say, and a second holder is a second chance to leak.
        using var context = NewContext(await AsTomasAsync());

        var markup = context.Render<MyAccess>().Markup;
        var teams = FixtureImporter.Parse(TestData.ScheduleCsv()).Matchups.Values
            .SelectMany(matchup => new[] { matchup.Home, matchup.Away })
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase);

        // Only fixtures that have not kicked off are withheld; the record names
        // one of those in ch-008, so no team in it may appear.
        var unplayed = new[] { "Portugal", "Spain" };
        foreach (var team in unplayed.Intersect(teams, StringComparer.OrdinalIgnoreCase))
        {
            Assert.DoesNotContain(team, markup, StringComparison.OrdinalIgnoreCase);
        }
    }
}
