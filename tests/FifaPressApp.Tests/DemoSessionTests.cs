using System.Diagnostics;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The demo account store and the simulated session.
///
/// <para>
/// These tests hold the line the class's own remarks draw: the session changes
/// what renders and nothing else. What is asserted here is the behaviour a
/// person can observe — a published credential works, a wrong one does not, the
/// password is compared exactly, and the session ends when asked.
/// </para>
/// </summary>
public class DemoSessionTests
{
    private static SimulatedSessionProvider NewSession() => new(new DemoAccountStore());

    [Fact]
    public void BothPublishedAccountsAreListed_AndEachSaysWhatMakesItDifferent()
    {
        var store = new DemoAccountStore();

        Assert.Equal(2, store.Published.Count);
        Assert.All(store.Published, account =>
        {
            Assert.False(string.IsNullOrWhiteSpace(account.Identifier));
            Assert.False(string.IsNullOrWhiteSpace(account.Password));
            Assert.False(string.IsNullOrWhiteSpace(account.DescriptionKey));
        });

        // Two accounts that opened the same record would demonstrate nothing.
        Assert.Equal(2, store.Published.Select(account => account.CredentialId).Distinct().Count());
    }

    [Theory]
    [InlineData("MP-2026-04817", "amina-demo-2026", "MP-2026-04817")]
    [InlineData("RH-2026-00219", "tomas-demo-2026", "RH-2026-00219")]
    public void APublishedCredentialOpensItsOwnRecord(string identifier, string password, string credentialId)
    {
        var account = new DemoAccountStore().Match(identifier, password);

        Assert.NotNull(account);
        Assert.Equal(credentialId, account.CredentialId);
    }

    [Theory]
    [InlineData("mp-2026-04817", "amina-demo-2026")]   // case
    [InlineData("  MP-2026-04817  ", "amina-demo-2026")] // pasted with whitespace
    public void TheIdentifierIsForgivingAboutCaseAndSurroundingSpace(string identifier, string password)
    {
        // A credential number copied off a screen arrives with a trailing space
        // more often than not. Turning that person away teaches them nothing.
        Assert.NotNull(new DemoAccountStore().Match(identifier, password));
    }

    [Theory]
    [InlineData("Amina-Demo-2026")]      // case-folded
    [InlineData(" amina-demo-2026")]     // leading space
    [InlineData("amina-demo-2026 ")]     // trailing space
    [InlineData("amina-demo-202")]       // truncated
    public void ThePasswordIsComparedByteForByte(string password)
    {
        // Never trimmed, never case-folded, never rewritten. The identifier gets
        // an allow-list; a password has to compare exactly or the comparison is
        // not a comparison.
        Assert.Null(new DemoAccountStore().Match("MP-2026-04817", password));
    }

    [Fact]
    public void OneAccountsPasswordDoesNotOpenTheOthersRecord()
    {
        Assert.Null(new DemoAccountStore().Match("RH-2026-00219", "amina-demo-2026"));
    }

    [Theory]
    [InlineData("nobody@example.com", "whatever")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void AnythingElseMatchesNothing(string? identifier, string? password)
    {
        Assert.Null(new DemoAccountStore().Match(identifier, password));
    }

    [Fact]
    public async Task ASessionStartsSignedOut_AndHoldsNoCredential()
    {
        var session = NewSession();

        Assert.False(session.IsSignedIn);
        Assert.Null(session.CredentialId);
        Assert.Null(session.Current);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task SigningInWithAPublishedCredentialEstablishesTheSession()
    {
        var session = NewSession();

        Assert.True(await session.SignInAsync("RH-2026-00219", "tomas-demo-2026"));
        Assert.True(session.IsSignedIn);
        Assert.Equal("RH-2026-00219", session.CredentialId);
        Assert.Equal("Tomás L.", session.Current!.HolderName);
    }

    [Fact]
    public async Task AFailedSignInLeavesTheSessionExactlyAsItWas()
    {
        var session = NewSession();
        await session.SignInAsync("MP-2026-04817", "amina-demo-2026");

        Assert.False(await session.SignInAsync("MP-2026-04817", "wrong"));

        // Still signed in as Amina: a failed attempt is not a sign-out.
        Assert.Equal("MP-2026-04817", session.CredentialId);
    }

    [Fact]
    public async Task SigningOutEndsTheSession()
    {
        var session = NewSession();
        await session.SignInAsync("MP-2026-04817", "amina-demo-2026");

        session.SignOut();

        Assert.False(session.IsSignedIn);
        Assert.Null(session.CredentialId);
    }

    [Fact]
    public async Task TheSessionAnnouncesEveryChange_SoTheChromeCanFollowIt()
    {
        var session = NewSession();
        var announcements = 0;
        session.OnChanged += () => announcements++;

        await session.SignInAsync("MP-2026-04817", "amina-demo-2026");
        session.SignOut();

        Assert.Equal(2, announcements);

        // A sign-out with nothing to end announces nothing, so an indicator
        // cannot flicker on a no-op.
        session.SignOut();
        Assert.Equal(2, announcements);
    }

    [Fact]
    public async Task AFailedSignInAnnouncesNothing()
    {
        var session = NewSession();
        var announcements = 0;
        session.OnChanged += () => announcements++;

        await session.SignInAsync("MP-2026-04817", "wrong");

        Assert.Equal(0, announcements);
    }

    [Fact]
    public void TheSignInPathGenuinelyYields_SoItsSubmittingStateCanBeSeen()
    {
        // The regression test 10 §7 asks for. An in-memory lookup that returned
        // an already-completed task would let the caller's continuation run
        // before the framework ever rendered, and the disabled fields and
        // "Signing in…" label would never reach the screen.
        var session = NewSession();

        var pending = session.SignInAsync("MP-2026-04817", "amina-demo-2026");

        Assert.False(pending.IsCompleted);
    }

    [Fact]
    public async Task TheSimulatedLatencyIsActuallyWaited()
    {
        var session = NewSession();
        var clock = Stopwatch.StartNew();

        await session.SignInAsync("MP-2026-04817", "amina-demo-2026");
        clock.Stop();

        // Deliberately loose: this asserts the delay is real, not that it is
        // precise. A timing test that fails on a loaded machine is a test that
        // gets deleted.
        Assert.True(
            clock.Elapsed >= SimulatedSessionProvider.SimulatedSignInLatency - TimeSpan.FromMilliseconds(50),
            $"sign-in returned after {clock.ElapsedMilliseconds}ms");
    }
}
