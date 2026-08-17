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
        SignedInAsync("MP-2026-04817", "amina-demo-2026");

    private static Task<SimulatedSessionProvider> AsTomasAsync() =>
        SignedInAsync("RH-2026-00219", "tomas-demo-2026");

    // ------------------------------------------------------------ the record

    [Fact]
    public void TheRecordIsNotReachableSignedOut_AndOffersSignInInstead()
    {
        using var context = NewContext(SignedOut());
        var navigation = context.Services.GetRequiredService<NavigationManager>();

        var page = context.Render<MyAccess>();

        Assert.NotEmpty(page.FindAll(".my-access__signed-out"));
        Assert.NotEmpty(page.FindAll("a[href='signin']"));
        Assert.EndsWith("/signin", navigation.Uri);
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
        Assert.EndsWith("/signin", navigation.Uri);
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
        Assert.DoesNotContain("signin", page.Markup);
    }

    [Fact]
    public void TheMatchListIsPublic()
    {
        using var context = NewContext(SignedOut());

        var page = context.Render<EventList>();

        Assert.DoesNotContain("Sign in", page.Markup);
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
        // or dead control.
        Assert.NotEmpty(page.FindAll(".detail__signed-out"));
        Assert.NotEmpty(page.FindAll("a[href='signin']"));

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

        // The record is personal; the request writes to one. Both reasons are
        // on screen, because a boundary stated without its reason is
        // indistinguishable from an arbitrary one.
        Assert.Contains("belongs to a holder", record.Render<MyAccess>().Markup);
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
