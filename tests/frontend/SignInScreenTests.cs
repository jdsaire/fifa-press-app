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
/// The sign-in experience, rewritten around a session that actually exists.
///
/// <para>
/// It is a component rather than a page as of this run — the form belongs on
/// the record's own route rather than on one of its own — so these tests render
/// the component directly. What they assert is unchanged by that move.
/// </para>
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
    public void TheNoticeStatesAllFourThings()
    {
        using var context = NewContext();

        var page = context.Render<SignInForm>();
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

        var notice = context.Render<SignInForm>().Find(".signin__notice").TextContent;

        Assert.DoesNotContain("sent, stored, or checked", notice);
        Assert.DoesNotContain("fully reachable without signing in", notice);
    }

    [Fact]
    public void TheNoticeIsStillOnTheScreen_NowBelowTheFormRatherThanAboveIt()
    {
        // A REVERSAL, RECORDED. This asserted the opposite until this run: the
        // notice came first, deliberately, per 10 §2.3, and R6 closed on
        // condensing it rather than moving it. It sits last now at the
        // principal's explicit direction.
        //
        // What the original test protected was that the notice is *present and
        // unmissable*, not that it occupies a particular index — so that is
        // what this asserts now: it is still rendered, still a note, and still
        // says all four things (covered by the test above, which passes
        // unchanged because it never depended on position).
        using var context = NewContext();

        var page = context.Render<SignInForm>();
        var markup = page.Markup;

        Assert.Equal("note", page.Find(".signin__notice").GetAttribute("role"));

        Assert.True(markup.IndexOf("signin__form", StringComparison.Ordinal)
                  < markup.IndexOf("signin__notice", StringComparison.Ordinal),
            "the notice should now follow the form");
    }

    [Fact]
    public void TheScreenReadsFieldsThenCredentialsThenTheNotice()
    {
        // The order the principal asked for, in one assertion, so a future
        // reshuffle has to be deliberate rather than incidental.
        using var context = NewContext();

        var markup = context.Render<SignInForm>().Markup;

        var form = markup.IndexOf("signin__form", StringComparison.Ordinal);
        var accounts = markup.IndexOf("signin__accounts", StringComparison.Ordinal);
        var notice = markup.IndexOf("signin__notice", StringComparison.Ordinal);

        Assert.True(form < accounts, "the form should come before the published accounts");
        Assert.True(accounts < notice, "the published accounts should come before the notice");
    }

    [Fact]
    public void BothAccountsArePublishedWithIdentifierAndPasswordAndNoPersonalName()
    {
        // What a person needs in order to sign in, and nothing else. The
        // per-account description this used to assert is retired with the keys
        // that backed it, and the holder's own name is now retired from this
        // screen too — this is a generic test account, and neither published
        // row may say whose record it happens to key.
        using var context = NewContext();

        var page = context.Render<SignInForm>();
        var accounts = page.FindAll(".signin__account");

        Assert.Equal(2, accounts.Count);

        foreach (var account in new[] { DemoAccountStore.Amina, DemoAccountStore.Tomas })
        {
            Assert.Contains(account.Identifier, page.Markup);
            Assert.Contains(account.Password, page.Markup);
        }

        Assert.DoesNotContain("Amina Bello", page.Markup);
        Assert.DoesNotContain("Tomás L.", page.Markup);
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

        var page = context.Render<SignInForm>();

        var identifier = page.Find("#signin-identifier");
        Assert.Equal("username", identifier.GetAttribute("autocomplete"));

        var password = page.Find("#signin-password");
        Assert.Equal("password", password.GetAttribute("type"));
        Assert.Equal("current-password", password.GetAttribute("autocomplete"));

    }

    [Fact]
    public void TheIdentifierFieldCarriesNoInvalidDomEventOverride()
    {
        // The regression guard for the defect that made sign-in unusable in a
        // browser while every test still passed: nothing in this suite had ever
        // fired an input event on this field.
        //
        // `@bind-Value:event="oninput"` was set on <InputText>, which is a
        // component rather than an element. On a component that syntax names an
        // EventCallback *parameter*, and InputText has none called `oninput` —
        // so it fell through AdditionalAttributes onto the underlying <input>
        // as a DOM handler, bound to a delegate typed `string`. Browsers invoke
        // DOM handlers with ChangeEventArgs, so the first keystroke threw
        // ArgumentException and took the whole renderer down with it.
        //
        // Asserted at the cause. An `oninput` handler on this element can only
        // be that splatted delegate — InputText never wires one itself — so its
        // absence is the guarantee, and its presence is the bug.
        using var context = NewContext();

        var page = context.Render<SignInForm>();

        Assert.Throws<Bunit.MissingEventHandlerException>(
            () => page.Find("#signin-identifier").Input("demo_staff1"));
    }

    [Fact]
    public void TheSubmitButtonIsInsideTheLayoutWrapperThatCarriesTheSpacing()
    {
        // The button was rendered directly against the password field's div
        // with no vertical space between them — a second instance of the same
        // defect class as the width bug: `.signin__form`'s flex/gap declaration
        // was scoped to <EditForm>, a component, so it never bound to anything.
        // The fix moved the layout onto a real <div> this file owns; this pins
        // that both the wrapper exists and the button is inside it, so the CSS
        // rule that gives it breathing room actually has something to apply to.
        using var context = NewContext();

        var page = context.Render<SignInForm>();
        var wrapper = page.Find(".signin__form-fields");

        Assert.NotEmpty(wrapper.QuerySelectorAll(".signin__field"));
        Assert.NotEmpty(wrapper.QuerySelectorAll("button[type='submit']"));
    }

    [Fact]
    public void EachPublishedAccountLabelsWhichValueIsTheUsernameAndWhichIsThePassword()
    {
        // "demo_staff1 / Demo#2026Staff1" reads as one opaque token to a first-
        // time reader — nothing in a slash says which half goes in which field.
        // Each value now sits on its own labelled line.
        using var context = NewContext();

        var page = context.Render<SignInForm>();

        foreach (var account in new DemoAccountStore().Published)
        {
            var row = page.FindAll(".signin__account")
                .Single(li => li.TextContent.Contains(account.Identifier));

            var lines = row.QuerySelectorAll(".signin__account-line");
            Assert.Equal(2, lines.Length);

            Assert.Contains("Username", lines[0].QuerySelector("strong")!.TextContent);
            Assert.Equal(account.Identifier, lines[0].QuerySelector("code")!.TextContent.Trim());

            Assert.Contains("Password", lines[1].QuerySelector("strong")!.TextContent);
            Assert.Equal(account.Password, lines[1].QuerySelector("code")!.TextContent.Trim());
        }
    }

    [Fact]
    public void BothFieldsBindOnChange_TheWayShopEasesOwnLoginFormDoes()
    {
        // The contract that has to keep working: what a person types reaches the
        // model. InputText's own change event does it, with no override.
        using var context = NewContext();

        var page = context.Render<SignInForm>();
        page.Find("#signin-identifier").Change("demo_staff1");
        page.Find("#signin-password").Change("Demo#2026Staff1");

        Assert.Equal("demo_staff1", page.Find("#signin-identifier").GetAttribute("value"));
    }

    [Fact]
    public void EachFieldsErrorAnnouncesItselfPolitely()
    {
        // ValidationMessage renders nothing until there is something to say, so
        // the empty submit is what makes the per-field errors exist at all.
        // Submitting an empty form is also the shortest path to proving
        // validation still runs after the rewrite.
        using var context = NewContext();

        var page = context.Render<SignInForm>();
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

        var page = context.Render<SignInForm>();
        page.Find("form").Submit();

        Assert.False(session.IsSignedIn);
    }

    [Fact]
    public void NothingIsDisabledBeforeAnAttemptIsMade()
    {
        using var context = NewContext();

        var page = context.Render<SignInForm>();

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

        var page = context.Render<SignInForm>();

        Assert.Empty(page.FindAll(".signin__result"));
        Assert.DoesNotContain("not implemented", page.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SomeoneAlreadySignedInIsSentToTheirRecord()
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync("demo_staff1", "Demo#2026Staff1");

        using var context = NewContext(session);
        var navigation = context.Services.GetRequiredService<NavigationManager>();

        context.Render<SignInForm>();

        Assert.EndsWith("/record", navigation.Uri);
    }

    [Fact]
    public void TheScreenNeverClaimsToProtectAnything()
    {
        // 10 §5.3: never a 403, never "access denied", never anything borrowing
        // the vocabulary of a real authorization system.
        using var context = NewContext();

        var text = context.Render<SignInForm>().Markup;

        foreach (var forbidden in new[] { "403", "Access denied", "Unauthorized", "Forbidden", "secure" })
        {
            Assert.DoesNotContain(forbidden, text, StringComparison.OrdinalIgnoreCase);
        }
    }
}
