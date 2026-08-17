using System.Text.Json;
using FifaPressApp.Models;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The locale service, its three resource files, and the dates it formats
/// itself.
/// </summary>
public class LocaleServiceTests
{
    [Fact]
    public void AllThreeLocalesLoad()
    {
        var locale = LocaleTestData.Loaded();

        Assert.True(locale.IsLoaded);
        Assert.Equal(3, LocaleService.All.Count);
    }

    [Fact]
    public void TheThreeFilesDefineExactlyTheSameKeys()
    {
        // The test that keeps a translation from being quietly forgotten. A key
        // added to English and not to the other two would fall back silently,
        // and an English sentence in a Spanish paragraph is the kind of defect
        // that ships.
        var locale = LocaleTestData.Loaded();

        var english = locale.Keys(AppLocale.En).OrderBy(key => key, StringComparer.Ordinal).ToList();

        foreach (var other in new[] { AppLocale.Es, AppLocale.Pt })
        {
            var theirs = locale.Keys(other).OrderBy(key => key, StringComparer.Ordinal).ToList();

            var missing = english.Except(theirs, StringComparer.Ordinal).ToList();
            var extra = theirs.Except(english, StringComparer.Ordinal).ToList();

            Assert.True(missing.Count == 0, $"{other} is missing: {string.Join(", ", missing)}");
            Assert.True(extra.Count == 0, $"{other} has keys English does not: {string.Join(", ", extra)}");
        }
    }

    [Fact]
    public void NoTranslationIsLeftEmpty()
    {
        var locale = LocaleTestData.Loaded();

        foreach (var which in LocaleService.All)
        {
            foreach (var key in locale.Keys(which))
            {
                Assert.False(
                    string.IsNullOrWhiteSpace(locale[which, key]),
                    $"{which} has an empty value for {key}");
            }
        }
    }

    [Fact]
    public void NoTranslationIsJustTheEnglishStringCopiedAcross()
    {
        // Identifiers, proper names and the product name are supposed to be
        // identical in all three; everything else being identical usually means
        // a placeholder was pasted and never revisited.
        var locale = LocaleTestData.Loaded();
        var allowedToMatch = new[]
        {
            "app.name",

            // The same word in all three languages, which is a fact about the
            // languages rather than a missed translation.
            "phase.final",

            // The name of a FIFA department. 11 §3's rule sends a name that
            // identifies a thing rather than describing it to English, and this
            // is one — translating it would invent a department nobody can find.
            "help.contact.fifaStrong",
        };

        foreach (var key in locale.Keys(AppLocale.En))
        {
            if (allowedToMatch.Contains(key))
            {
                continue;
            }

            var english = locale[AppLocale.En, key];

            Assert.False(
                english == locale[AppLocale.Es, key] && english == locale[AppLocale.Pt, key],
                $"{key} is identical in all three locales — was it translated?");
        }
    }

    [Fact]
    public void EveryFileCarriesTwelveMonthNamesLongAndShort()
    {
        foreach (var which in LocaleService.All)
        {
            using var document = JsonDocument.Parse(LocaleTestData.RawJson(which));

            Assert.Equal(12, document.RootElement.GetProperty("months").GetArrayLength());
            Assert.Equal(12, document.RootElement.GetProperty("monthsShort").GetArrayLength());
        }
    }

    [Theory]
    [InlineData(AppLocale.En, "My Access")]
    [InlineData(AppLocale.Es, "Mi acceso")]
    [InlineData(AppLocale.Pt, "Meu acesso")]
    public void AStringResolvesInItsOwnLocale(AppLocale which, string expected)
    {
        Assert.Equal(expected, LocaleTestData.Loaded()[which, "nav.record"]);
    }

    [Fact]
    public void TheProductNameNeverTranslates()
    {
        // 11 §3's rule: a name that identifies the product stays English; a name
        // that identifies what a person is looking at translates. "FIFA Press
        // App" is the first; "My Access" is the second, and the test above
        // covers it.
        var locale = LocaleTestData.Loaded();

        foreach (var which in LocaleService.All)
        {
            Assert.Equal("FIFA Press App", locale[which, "app.name"]);
        }
    }

    [Fact]
    public void AMissingKeyFallsBackToEnglishRatherThanToNothing()
    {
        var locale = LocaleTestData.Loaded();

        // Nothing is currently missing, so this asserts the mechanism against a
        // key that does not exist anywhere: it returns the key itself, visibly,
        // rather than an empty string nobody would report.
        Assert.Equal("not.a.real.key", locale[AppLocale.Es, "not.a.real.key"]);
        Assert.False(locale.Has(AppLocale.Es, "not.a.real.key"));
    }

    [Fact]
    public void TokensAreSubstituted()
    {
        var locale = LocaleTestData.Loaded();

        Assert.Equal("Group D", locale.Format(AppLocale.En, "phase.group", ("letter", "D")));
        Assert.Equal("Grupo D", locale.Format(AppLocale.Es, "phase.group", ("letter", "D")));
    }

    [Fact]
    public void SettingTheLocaleAnnouncesIt()
    {
        var locale = LocaleTestData.Loaded();
        var announcements = 0;
        locale.OnChanged += () => announcements++;

        locale.Set(AppLocale.Pt);
        Assert.Equal(AppLocale.Pt, locale.Current);
        Assert.Equal(1, announcements);

        // Setting the locale it already holds announces nothing, so nothing
        // re-renders for a no-op.
        locale.Set(AppLocale.Pt);
        Assert.Equal(1, announcements);
    }

    // -------------------------------------------------------------- the dates

    [Theory]
    [InlineData(AppLocale.En, "July 4, 2026")]
    [InlineData(AppLocale.Es, "4 de julio de 2026")]
    [InlineData(AppLocale.Pt, "4 de julho de 2026")]
    public void TheCardDateFormatsPerLocale(AppLocale which, string expected)
    {
        var locale = LocaleTestData.Loaded();

        Assert.Equal(expected, locale.Date(which, new DateTime(2026, 7, 4), "monthDayYear"));
    }

    [Theory]
    [InlineData(AppLocale.En, "3 July 2026, 17:15")]
    [InlineData(AppLocale.Es, "3 de julio de 2026, 17:15")]
    [InlineData(AppLocale.Pt, "3 de julho de 2026, 17:15")]
    public void TheTimestampFormatsPerLocale(AppLocale which, string expected)
    {
        var locale = LocaleTestData.Loaded();

        Assert.Equal(
            expected,
            locale.Date(which, new DateTime(2026, 7, 3, 17, 15, 0), "dayMonthYearTime"));
    }

    [Theory]
    [InlineData(AppLocale.En, "6 July")]
    [InlineData(AppLocale.Es, "6 de julio")]
    [InlineData(AppLocale.Pt, "6 de julho")]
    public void TheShortDateFormatsPerLocale(AppLocale which, string expected)
    {
        var locale = LocaleTestData.Loaded();

        Assert.Equal(expected, locale.Date(which, new DateTime(2026, 7, 6), "dayMonth"));
    }

    [Fact]
    public void PortugueseDatesWorkDespiteTheIcuShardThatDoesNotCarryPortuguese()
    {
        // The finding this whole approach exists for. Blazor WebAssembly picks
        // one ICU shard at boot from the boot culture; an app booting "en" gets
        // icudt_EFIGS.dat, which has English, French, Italian, German and
        // Spanish — and no Portuguese. Anything resolved through CultureInfo
        // would degrade silently on a switch to PT. These month names come from
        // the JSON, so the shard cannot affect them.
        var locale = LocaleTestData.Loaded();

        Assert.Equal("15 de março de 2026", locale.Date(AppLocale.Pt, new DateTime(2026, 3, 15), "dayMonthYear"));
        Assert.Equal("15 de agosto de 2026", locale.Date(AppLocale.Pt, new DateTime(2026, 8, 15), "dayMonthYear"));
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public void TimesStay24HourInEveryLocale(AppLocale which)
    {
        // 11 §6's assumption, held explicitly: 24-hour notation everywhere. A
        // kickoff time read under pressure should never need an AM/PM
        // disambiguation.
        var locale = LocaleTestData.Loaded();

        var evening = locale.Date(which, new DateTime(2026, 7, 3, 20, 30, 0), "dayMonthYearTime");

        Assert.Contains("20:30", evening);
        Assert.DoesNotContain("PM", evening, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("8:30", evening);
    }

    [Fact]
    public void EveryMonthOfTheYearHasAName()
    {
        var locale = LocaleTestData.Loaded();

        foreach (var which in LocaleService.All)
        {
            for (var month = 1; month <= 12; month++)
            {
                var formatted = locale.Date(which, new DateTime(2026, month, 1), "dayMonthYear");

                // A missing month name would fall through to the number.
                Assert.DoesNotContain($" {month} ", formatted);
            }
        }
    }

    [Fact]
    public async Task ALocaleFileThatWillNotLoadDoesNotTakeTheAppDown()
    {
        // Degraded, stated, still running — the app's own argument applied to
        // itself. Nothing is served here, so every lookup falls through to the
        // key rather than throwing.
        var service = new LocaleService(new HttpClient { BaseAddress = new Uri("http://localhost/") });
        await service.InitializeAsync();

        Assert.False(service.IsLoaded);
        Assert.Equal("nav.record", service[AppLocale.Es, "nav.record"]);
        Assert.Equal("1 1 2026", service.Date(AppLocale.Es, new DateTime(2026, 1, 1), "dayMonthYear"));
    }

    [Theory]
    [InlineData("es", AppLocale.Es)]
    [InlineData("pt", AppLocale.Pt)]
    [InlineData("en", AppLocale.En)]
    [InlineData("de", AppLocale.En)]
    [InlineData(null, AppLocale.En)]
    public void ACodeResolvesToItsLocaleOrToEnglish(string? code, AppLocale expected)
    {
        Assert.Equal(expected, LocaleService.FromCode(code));
    }
}
