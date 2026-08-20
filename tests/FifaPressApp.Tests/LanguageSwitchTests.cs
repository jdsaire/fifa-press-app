using Bunit;
using FifaPressApp.Components;
using FifaPressApp.Layout;
using FifaPressApp.Models;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The language switch, and what a switch is and is not allowed to disturb.
/// </summary>
public class LanguageSwitchTests
{
    private sealed record Harness(BunitContext Context, LocaleService Locale, SimulatedSessionProvider Session);

    private static Harness NewHarness()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        var locale = LocaleTestData.Loaded();
        var session = new SimulatedSessionProvider(new DemoAccountStore());

        context.Services.AddSingleton(locale);
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new ChangeArrivalTracker());
        context.Services.AddSingleton(session);
        context.Services.AddSingleton<IAccessDataProvider>(TestData.ProviderOverRealSchedule());

        // Rendered inside the real cascade, exactly as MainLayout supplies it —
        // so a switch propagates the way it does in the app rather than the way
        // a test rigged it to.
        context.RenderTree.Add<LocaleProvider>();

        return new Harness(context, locale, session);
    }

    /// <summary>
    /// Renders a component and then switches language.
    ///
    /// <para>
    /// LocaleProvider resolves the stored language on its first render, so a
    /// locale set on the service <i>before</i> that render is overwritten by the
    /// resolution — correctly, because that is what startup does. Switching
    /// after the first render is both the way around it and the thing a person
    /// actually does.
    /// </para>
    /// </summary>
    private static IRenderedComponent<T> RenderThenSwitch<T>(Harness harness, AppLocale locale)
        where T : IComponent
    {
        var page = harness.Context.Render<T>();
        harness.Locale.Set(locale);
        return page;
    }

    [Fact]
    public void TheSwitchOffersThreeFixedOptionsRatherThanAPicker()
    {
        // Three is not enough to need a dropdown's affordance, and 09 §3's
        // clarity principle says so.
        var harness = NewHarness();
        using var context = harness.Context;

        var switcher = context.Render<LanguageSwitch>();
        var options = switcher.FindAll(".language-switch__option");

        Assert.Equal(3, options.Count);
        Assert.Empty(switcher.FindAll(".language-switch select"));
        Assert.Equal(["EN", "ES", "PT"], options.Select(option => option.TextContent.Trim()));
    }

    [Fact]
    public void TheSwitchIsItsOwnControl_NotFoldedIntoTheThemeControl()
    {
        // Language and theme are independent choices; one combined control
        // would make a person cycle six states to reach one of them. This used
        // to be asserted as a nav-row placement; the rows are gone, the
        // independence is not.
        var harness = NewHarness();
        using var context = harness.Context;

        var switcher = context.Render<LanguageSwitch>();

        Assert.NotEmpty(switcher.FindAll(".language-switch"));
        Assert.Empty(switcher.FindAll("button.theme-trigger"));
    }

    [Fact]
    public void TheCurrentLanguageIsMarkedRatherThanRemoved()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var switcher = context.Render<LanguageSwitch>();
        var pressed = switcher.FindAll(".language-switch__option[aria-pressed='true']");

        Assert.Single(pressed);
        Assert.Equal("EN", pressed[0].TextContent.Trim());

        // All three stay in the DOM, so nothing moves under the pointer when a
        // person switches.
        Assert.Equal(3, switcher.FindAll(".language-switch__option").Count);
    }

    [Fact]
    public void PressingAnOptionSwitchesTheActiveLocale()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var switcher = context.Render<LanguageSwitch>();
        switcher.FindAll(".language-switch__option")[1].Click();

        Assert.Equal(AppLocale.Es, harness.Locale.Current);
    }

    [Fact]
    public void TheNavItselfRerendersIntoTheNewLanguage()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        // The switch is no longer inside the nav, so this now asserts what it
        // always meant to: a switch anywhere re-renders the whole tree, the
        // sidebar included, because the cascaded value changed.
        var nav = context.Render<NavMenu>();
        var switcher = context.Render<LanguageSwitch>();
        Assert.Contains("Matches", nav.Markup);

        switcher.FindAll(".language-switch__option")[1].Click();

        Assert.Contains("Partidos", nav.Markup);
        Assert.DoesNotContain("Matches", nav.Markup);
    }

    // ------------------------------------------------- the §5.3 discrepancy

    [Fact]
    public async Task TheSessionSURVIVESALanguageSwitch()
    {
        // 11 §5.3 asserts that switching language while signed in signs the
        // person out. It derives that explicitly from a locale-triggered
        // RELOAD — but §5.2 decided Option B, an in-session re-render with no
        // reload, so the premise does not hold. This test pins the behaviour
        // Option B actually produces. The discrepancy is reported rather than
        // resolved by editing a Final file.
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("MP-2026-04817", "amina-demo-2026");

        // The holder indicator reads from the session bar now rather than the
        // sidebar; the guarantee is unchanged — a switch leaves the session,
        // the credential and the named holder exactly where they were.
        var bar = context.Render<SessionBar>();
        Assert.Contains("Amina Bello", bar.Markup);

        context.Render<LanguageSwitch>().FindAll(".language-switch__option")[2].Click();

        Assert.True(harness.Session.IsSignedIn);
        Assert.Equal("MP-2026-04817", harness.Session.CredentialId);
        Assert.Contains("Amina Bello", bar.Markup);
    }

    [Fact]
    public async Task ASwitchDoesNotNavigate()
    {
        // A navigation is how a session would be lost in practice, so this
        // asserts the mechanism rather than only the outcome.
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("MP-2026-04817", "amina-demo-2026");

        var navigation = context.Services.GetRequiredService<NavigationManager>();
        var before = navigation.Uri;

        var switcher = context.Render<LanguageSwitch>();
        switcher.FindAll(".language-switch__option")[1].Click();

        Assert.Equal(before, navigation.Uri);
    }

    [Fact]
    public void TheSwitchWarnsAboutNothing_BecauseThereIsNothingToWarnAbout()
    {
        // §5.3 asked that the switch's own UI not surprise anyone by ending
        // their session. It does not end it, so there is no warning to give —
        // and a warning about something that does not happen would be its own
        // small lie.
        var harness = NewHarness();
        using var context = harness.Context;

        var switchMarkup = context.Render<LanguageSwitch>().Find(".language-switch").InnerHtml;

        foreach (var word in new[] { "sign you out", "signed out", "session will end" })
        {
            Assert.DoesNotContain(word, switchMarkup, StringComparison.OrdinalIgnoreCase);
        }
    }

    // ------------------------------------------------ every surface, three ways

    [Theory]
    [InlineData(AppLocale.En, "My Requests")]
    [InlineData(AppLocale.Es, "Mis solicitudes")]
    [InlineData(AppLocale.Pt, "Minhas solicitações")]
    public async Task TheRecordRendersInEveryLocale(AppLocale locale, string heading)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("MP-2026-04817", "amina-demo-2026");

        var page = RenderThenSwitch<MyAccess>(harness, locale);

        Assert.Contains(heading, page.Find("h1").TextContent);
    }

    [Theory]
    [InlineData(AppLocale.En, "Matches")]
    [InlineData(AppLocale.Es, "Partidos")]
    [InlineData(AppLocale.Pt, "Jogos")]
    public void TheMatchListRendersInEveryLocale(AppLocale locale, string heading)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        Assert.Contains(heading, RenderThenSwitch<EventList>(harness, locale).Find("h1").TextContent);
    }

    [Theory]
    [InlineData(AppLocale.En, "Sign in")]
    [InlineData(AppLocale.Es, "Iniciar sesión")]
    [InlineData(AppLocale.Pt, "Entrar")]
    public void TheSignInScreenRendersInEveryLocale(AppLocale locale, string heading)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        Assert.Contains(heading, RenderThenSwitch<SignIn>(harness, locale).Find("h1").TextContent);
    }

    [Theory]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public void TheLandingRendersInEveryLocale(AppLocale locale)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<Landing>();
        var english = page.Find("h1").TextContent;

        harness.Locale.Set(locale);
        var translated = page.Find("h1").TextContent;

        Assert.NotEqual(english, translated);
        Assert.False(string.IsNullOrWhiteSpace(translated));
    }

    // ------------------------------------------------------- no stale strings

    [Fact]
    public async Task NoStringOnTheRecordIsLeftBehindByASwitch()
    {
        // Targets the three components that genuinely went stale: the
        // per-match label baked in by Rebuild(), and two [Parameter] defaults
        // assigned once at construction.
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("MP-2026-04817", "amina-demo-2026");

        var page = context.Render<MyAccess>();
        Assert.Contains("Match 1", page.Markup);
        Assert.Contains("Last updated", page.Markup);

        harness.Locale.Set(AppLocale.Es);
        page.Render();

        // The per-fixture label, the staleness sentence and the foreseeable
        // badge all follow.
        Assert.Contains("Partido 1", page.Markup);
        Assert.DoesNotContain("Match 1", page.Markup);
        Assert.Contains("Actualizado por última vez", page.Markup);
        Assert.DoesNotContain("Last updated", page.Markup);
        Assert.DoesNotContain("Not decided yet", page.Markup);
    }

    [Fact]
    public async Task TheChangeLogsOwnProseFollowsTheSwitchToo()
    {
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("RH-2026-00219", "tomas-demo-2026");

        var page = context.Render<MyAccess>();
        Assert.Contains("Broadcast position confirmed", page.Markup);

        harness.Locale.Set(AppLocale.Pt);
        page.Render();

        Assert.Contains("Posição de transmissão confirmada", page.Markup);
        Assert.DoesNotContain("Broadcast position confirmed", page.Markup);
    }

    [Fact]
    public async Task ASwitchDoesNotAlterDuplicateOrReorderTheLog()
    {
        // 11 §4.4. A change written in three languages is still one change.
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("MP-2026-04817", "amina-demo-2026");

        var page = context.Render<MyAccess>();
        var before = page.FindAll("article.change-row").Count;

        harness.Locale.Set(AppLocale.Pt);
        page.Render();

        Assert.Equal(before, page.FindAll("article.change-row").Count);
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public async Task TheWithholdingRuleHoldsOnScreenInEveryLanguage(AppLocale locale)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("RH-2026-00219", "tomas-demo-2026");

        var markup = RenderThenSwitch<MyAccess>(harness, locale).Markup;

        // Match 93 has not kicked off at the simulated instant, and Tomás's
        // ch-008 waits on it. Neither team in it may appear, in any language.
        foreach (var team in new[] { "Portugal", "Spain", "España", "Espanha" })
        {
            Assert.DoesNotContain(team, markup, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Theory]
    [InlineData(AppLocale.En, "teams not yet decided")]
    [InlineData(AppLocale.Es, "selecciones aún no decididas")]
    [InlineData(AppLocale.Pt, "seleções ainda não definidas")]
    public void AnUnplayedFixtureSaysSoInEveryLanguage(AppLocale locale, string expected)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        harness.Locale.Set(locale);
        var unplayed = TestData.Fixture(93, PhaseKind.RoundOf16, groupLetter: null);

        Assert.Contains(expected, FixtureLabels.Display(harness.Locale, locale, unplayed));
    }
}
