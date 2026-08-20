using Bunit;
using FifaPressApp.Models;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The sign-in screen, rewritten around a session that actually exists.
///
/// <para>
/// <b>Why the submit path is asserted at the service seam rather than by
/// clicking the button.</b> The identifier field carries
/// <c>@bind-Value:event="oninput"</c>, which on a component splats a
/// string-typed <c>oninput</c> handler whose conversion Blazor performs in
/// JavaScript. bUnit has no JavaScript, so it cannot drive that binding, and a
/// test that pretended to would be testing its own stub. What the person
/// actually depends on — that the published credentials establish a session and
/// nothing else does — is asserted directly against
/// <see cref="SimulatedSessionProvider"/> in DemoSessionTests. What is asserted
/// here is everything the screen renders.
/// </para>
/// </summary>
public class SignInScreenTests
{
    private static BunitContext NewContext(SimulatedSessionProvider? session = null)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(session ?? new SimulatedSessionProvider(new DemoAccountStore()));
        return context.WithLocale();
    }

    [Fact]
    public void TheNoticeComesFirstAndStatesAllFourThings()
    {
        using var context = NewContext();

        var page = context.Render<SignIn>();
        var notice = page.Find(".signin__notice").TextContent;

        Assert.Contains("simulated sign-in", notice, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("no account system", notice, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("they work", notice, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Refreshing the page signs you out", notice, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not a security boundary", notice, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TheNoticeNoLongerMakesTheTwoClaimsThatBecameFalse()
    {
        // v9 said nothing is sent, stored or checked, and that every part of the
        // app is reachable without signing in. A session and a gated record made
        // both false, and a notice that has quietly become false is worse than
        // none at all.
        using var context = NewContext();

        var notice = context.Render<SignIn>().Find(".signin__notice").TextContent;

        Assert.DoesNotContain("sent, stored, or checked", notice);
        Assert.DoesNotContain("fully reachable without signing in", notice);
    }

    [Fact]
    public void TheNoticeIsBeforeTheFormInTheMarkup()
    {
        using var context = NewContext();

        var markup = context.Render<SignIn>().Markup;

        Assert.True(markup.IndexOf("signin__notice", StringComparison.Ordinal)
                  < markup.IndexOf("signin__form", StringComparison.Ordinal));
    }

    [Fact]
    public void BothAccountsArePublishedWithHolderIdentifierAndPassword()
    {
        // What a person needs in order to sign in, and nothing else. The
        // per-account description this used to assert is retired with the keys
        // that backed it: what the two records demonstrate is shown on the
        // screen where they are actually compared, not explained in advance to
        // somebody who has seen neither.
        using var context = NewContext();

        var page = context.Render<SignIn>();
        var accounts = page.FindAll(".signin__account");

        Assert.Equal(2, accounts.Count);

        foreach (var account in new[] { DemoAccountStore.Amina, DemoAccountStore.Tomas })
        {
            var markup = page.Markup;
            Assert.Contains(account.Identifier, markup);
            Assert.Contains(account.Password, markup);
            Assert.Contains(account.HolderName, markup);
        }
    }

    [Fact]
    public void ThePublishedIdentifierIsTypeableAndIsNotTheCredentialNumber()
    {
        // The rename's whole point. The credential number is still what the
        // record is keyed by — it is simply no longer what a person is asked to
        // type from memory.
        foreach (var account in new DemoAccountStore().Published)
        {
            Assert.StartsWith("demo_staff", account.Identifier);
            Assert.NotEqual(account.CredentialId, account.Identifier);

            // And the allow-list the form validates against accepts it: an
            // underscore is in the permitted set, and there is no "@" in it to
            // trip the email input-mode nudge either way.
            Assert.Matches(@"^[\p{L}\p{N} .\-'_@]+$", account.Identifier);
            Assert.DoesNotContain("@", account.Identifier);
        }
    }

    [Fact]
    public void TheCredentialNumbersStillKeyTheRecordsTheyAlwaysDid()
    {
        // The half of the rename that must NOT have moved. Every stored change,
        // every seeded accreditation, and MockAccessDataProvider's own two
        // constants are keyed by these values.
        Assert.Equal("MP-2026-04817", DemoAccountStore.Amina.CredentialId);
        Assert.Equal("RH-2026-00219", DemoAccountStore.Tomas.CredentialId);

        Assert.Equal(MockAccessDataProvider.AminaCredentialId, DemoAccountStore.Amina.CredentialId);
        Assert.Equal(MockAccessDataProvider.TomasCredentialId, DemoAccountStore.Tomas.CredentialId);
    }

    [Fact]
    public void ThePublishedCredentialsAreTheOnesThatActuallyWork()
    {
        // The failure this catches is a screen that publishes one thing and a
        // store that accepts another — a demo whose printed credentials do not
        // work is worse than one with none.
        var store = new DemoAccountStore();

        foreach (var account in store.Published)
        {
            Assert.NotNull(store.Match(account.Identifier, account.Password));
        }
    }

    [Fact]
    public void TheFormKeepsEverythingV9BuiltThatWasNotSupposedToChange()
    {
        using var context = NewContext();

        var page = context.Render<SignIn>();

        var identifier = page.Find("#signin-identifier");
        Assert.Equal("username", identifier.GetAttribute("autocomplete"));

        var password = page.Find("#signin-password");
        Assert.Equal("password", password.GetAttribute("type"));
        Assert.Equal("current-password", password.GetAttribute("autocomplete"));

    }

    [Fact]
    public void EachFieldsErrorAnnouncesItselfPolitely()
    {
        // ValidationMessage renders nothing until there is something to say, so
        // the empty submit is what makes the per-field errors exist at all.
        // Submitting an empty form is also the shortest path to proving
        // validation still runs after the rewrite.
        using var context = NewContext();

        var page = context.Render<SignIn>();
        page.Find("form").Submit();

        var announced = page.FindAll(".signin__field [aria-live='polite']");
        Assert.Equal(2, announced.Count);
        Assert.All(announced, message => Assert.False(string.IsNullOrWhiteSpace(message.TextContent)));
    }

    [Fact]
    public void AnInvalidFormNeverReachesTheSession()
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        using var context = NewContext(session);

        var page = context.Render<SignIn>();
        page.Find("form").Submit();

        Assert.False(session.IsSignedIn);
    }

    [Fact]
    public void NothingIsDisabledBeforeAnAttemptIsMade()
    {
        using var context = NewContext();

        var page = context.Render<SignIn>();

        Assert.False(page.Find("#signin-identifier").HasAttribute("disabled"));
        Assert.False(page.Find("#signin-password").HasAttribute("disabled"));
        Assert.False(page.Find("button[type='submit']").HasAttribute("disabled"));
        Assert.Equal("Sign in", page.Find("button[type='submit']").TextContent.Trim());
    }

    [Fact]
    public void ThereIsNoInertNotImplementedResultPanelAnyMore()
    {
        // v9's submission validated and then said sign-in was not implemented.
        // It is implemented now, and the confirmation is being on the record.
        using var context = NewContext();

        var page = context.Render<SignIn>();

        Assert.Empty(page.FindAll(".signin__result"));
        Assert.DoesNotContain("not implemented", page.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SomeoneAlreadySignedInIsSentToTheirRecord()
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync("demo_staff1", "amina-demo-2026");

        using var context = NewContext(session);
        var navigation = context.Services.GetRequiredService<NavigationManager>();

        context.Render<SignIn>();

        Assert.EndsWith("/record", navigation.Uri);
    }

    [Fact]
    public void TheScreenNeverClaimsToProtectAnything()
    {
        // 10 §5.3: never a 403, never "access denied", never anything borrowing
        // the vocabulary of a real authorization system.
        using var context = NewContext();

        var text = context.Render<SignIn>().Markup;

        foreach (var forbidden in new[] { "403", "Access denied", "Unauthorized", "Forbidden", "secure" })
        {
            Assert.DoesNotContain(forbidden, text, StringComparison.OrdinalIgnoreCase);
        }
    }
}
