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
/// Dates and durations on screen, in three languages.
///
/// <para>
/// The service's own formatting is covered in LocaleServiceTests. What is
/// asserted here is that the components actually go through it — a component
/// still calling <c>ToString("d MMMM yyyy")</c> would produce English on a
/// Spanish screen, and worse, would produce nothing recognisable at all in
/// Portuguese, whose month names the shipped ICU shard does not carry.
/// </para>
/// </summary>
public class LocalizedDateTests
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

    [Theory]
    // ch-005's effective date, which the record renders as "Takes effect …".
    [InlineData(AppLocale.En, "6 July 2026, 14:00")]
    [InlineData(AppLocale.Es, "6 de julio de 2026, 14:00")]
    [InlineData(AppLocale.Pt, "6 de julho de 2026, 14:00")]
    public async Task TheChangeLogsEffectiveDateFormatsPerLocale(AppLocale locale, string expected)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("demo_staff1", "Demo#2026Staff1");

        var markup = RenderThenSwitch<MyAccess>(harness, locale).Markup;

        Assert.Contains(expected, markup);
    }

    [Theory]
    [InlineData(AppLocale.En, "3 Jul, 17:15")]
    [InlineData(AppLocale.Es, "3 jul, 17:15")]
    [InlineData(AppLocale.Pt, "3 jul, 17:15")]
    public async Task TheStalenessTimestampFormatsPerLocale(AppLocale locale, string expected)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("demo_staff1", "Demo#2026Staff1");

        Assert.Contains(
            expected,
            RenderThenSwitch<MyAccess>(harness, locale).Find(".stale-indicator__stamp").TextContent);
    }

    [Theory]
    [InlineData(AppLocale.En, "hours ago")]
    [InlineData(AppLocale.Es, "hace 3 horas")]
    [InlineData(AppLocale.Pt, "há 3 horas")]
    public async Task ThePluralizedDurationIsGrammaticalInEveryLanguage(AppLocale locale, string expected)
    {
        // The one place in the app doing its own pluralization. A naive
        // per-branch translation that appended an "s" produces wrong output in
        // both Spanish and Portuguese, so the one/many split lives in the
        // resources instead.
        var harness = NewHarness();
        using var context = harness.Context;

        await harness.Session.SignInAsync("demo_staff1", "Demo#2026Staff1");

        Assert.Contains(
            expected,
            RenderThenSwitch<MyAccess>(harness, locale).Find(".stale-indicator__label").TextContent);
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public async Task TheSingularCaseNeverReadsAsAPluralWithA1InFrontOfIt(AppLocale locale)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        harness.Locale.Set(locale);

        var synced = new DateTime(2026, 7, 3, 19, 31, 0, DateTimeKind.Utc);
        var now = synced.AddHours(1);

        var indicator = context.Render<StaleIndicator>(parameters => parameters
            .Add(component => component.LastSyncedUtc, synced)
            .Add(component => component.AsOfUtc, now)
            .Add(component => component.SubjectKey, "record.staleSubject"));

        var text = indicator.Find(".stale-indicator__label").TextContent;

        Assert.Contains("1", text);
        Assert.DoesNotContain("1 hours", text);
        Assert.DoesNotContain("1 horas", text);

        await Task.CompletedTask;
    }

    [Theory]
    [InlineData(AppLocale.En, "July 4, 2026")]
    [InlineData(AppLocale.Es, "4 de julio de 2026")]
    [InlineData(AppLocale.Pt, "4 de julho de 2026")]
    public void TheEventCardsDateFormatsPerLocale(AppLocale locale, string expected)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var card = context.Render<EventCard>(parameters => parameters
            .Add(component => component.EventName, "Round of 16 — teams not yet decided")
            .Add(component => component.EventDate, new DateTime(2026, 7, 4, 16, 0, 0))
            .Add(component => component.Location, "AT&T Stadium, Dallas")
            .Add(component => component.ReadOnly, true)
            .Add(component => component.AllowEdit, false));

        harness.Locale.Set(locale);

        Assert.Equal(expected, card.Find("time").TextContent);
    }

    [Fact]
    public void TheMachineReadableDatetimeAttributeIsNeverLocalized()
    {
        // The <time datetime> attribute is ISO-8601 for a machine, in every
        // locale. Localizing it would break the one part of this that is not
        // for a person to read.
        var harness = NewHarness();
        using var context = harness.Context;

        var card = context.Render<EventCard>(parameters => parameters
            .Add(component => component.EventName, "x")
            .Add(component => component.EventDate, new DateTime(2026, 7, 4))
            .Add(component => component.Location, "y")
            .Add(component => component.ReadOnly, true)
            .Add(component => component.AllowEdit, false));

        harness.Locale.Set(AppLocale.Pt);

        Assert.Equal("2026-07-04", card.Find("time").GetAttribute("datetime"));
    }

    [Fact]
    public void TheImportersInvariantParseIsUntouched()
    {
        // 11 §6's own exception: the CSV is an input file in one fixed format,
        // and only display formatting became locale-aware. This asserts the
        // importer still reads the tracked schedule identically whatever the
        // app's active locale is.
        var harness = NewHarness();
        using var context = harness.Context;

        var english = FixtureImporter.Parse(TestData.ScheduleCsv());

        harness.Locale.Set(AppLocale.Pt);
        var portuguese = FixtureImporter.Parse(TestData.ScheduleCsv());

        Assert.Equal(english.Fixtures.Count, portuguese.Fixtures.Count);
        Assert.Equal(
            english.Fixtures.Select(fixture => fixture.KickoffLocal),
            portuguese.Fixtures.Select(fixture => fixture.KickoffLocal));
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public async Task KickoffTimesStay24HourOnScreen(AppLocale locale)
    {
        var harness = NewHarness();
        using var context = harness.Context;

        var page = context.Render<EventDetails>(parameters => parameters.Add(p => p.Id, 22));
        harness.Locale.Set(locale);

        Assert.DoesNotContain(" PM", page.Markup);
        Assert.DoesNotContain(" AM", page.Markup);

        await Task.CompletedTask;
    }
}
