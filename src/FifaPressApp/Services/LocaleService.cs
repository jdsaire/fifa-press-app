using System.Text.Json;
using System.Text.Json.Serialization;

namespace FifaPressApp.Services;

/// <summary>
/// The three languages this app is built in. English first because it is the
/// authoring language and the fallback, not because it is more important.
/// </summary>
public enum AppLocale
{
    En,
    Es,
    Pt,
}

/// <summary>
/// One locale's strings, month names and date patterns, as loaded from its JSON
/// file.
/// </summary>
public sealed class LocaleResources
{
    [JsonPropertyName("code")]
    public string Code { get; init; } = "en";

    /// <summary>The language's own name for itself, for the language switch.</summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = "English";

    [JsonPropertyName("months")]
    public string[] Months { get; init; } = [];

    [JsonPropertyName("monthsShort")]
    public string[] MonthsShort { get; init; } = [];

    /// <summary>
    /// Date patterns as token templates rather than .NET format strings. See
    /// <see cref="LocaleService"/>'s remarks for why this app formats dates
    /// itself instead of handing them to <c>CultureInfo</c>.
    /// </summary>
    [JsonPropertyName("dateFormats")]
    public Dictionary<string, string> DateFormats { get; init; } = [];

    [JsonPropertyName("strings")]
    public Dictionary<string, string> Strings { get; init; } = [];
}

/// <summary>
/// The active locale, the three loaded dictionaries, and every localized string
/// and date the app renders.
///
/// <para>
/// <b>Why JSON files and not <c>.resx</c>.</b> Not a preference — a constraint.
/// <c>Microsoft.Extensions.Localization</c> is not in this project's resolved
/// dependency graph, so <c>IStringLocalizer</c> and <c>AddLocalization()</c>
/// would need a new package reference, which this run may not add. Satellite
/// assemblies would additionally want a csproj property. Three JSON files
/// fetched with the <c>HttpClient</c> that already exists, deserialized with
/// <c>System.Text.Json</c> from the shared framework, need neither.
/// </para>
///
/// <para>
/// <b>Why this class formats dates itself instead of using
/// <c>CultureInfo</c>.</b> Blazor WebAssembly picks exactly one ICU shard at
/// boot, from the boot culture. An app booting <c>en</c> downloads
/// <c>icudt_EFIGS.dat</c> — English, French, Italian, German, Spanish — which
/// carries <b>no Portuguese</b>. So an in-session switch to PT has no
/// Portuguese ICU data, and <c>CultureInfo</c>-based formatting degrades
/// silently rather than failing. The standard fix is a csproj property, which
/// is forbidden here. Month names and date patterns therefore live in the same
/// per-locale JSON as everything else, which is deterministic, unit-testable
/// without a browser, and immune to what the runtime happens to ship.
/// </para>
///
/// <para>
/// This is a deviation from <c>11_I18N.md</c> §6's literal mechanism — "resolve
/// against the active <c>CultureInfo</c>" — while delivering exactly the output
/// §6 requires. <c>FixtureImporter.ParseDate</c>'s invariant <i>parse</i> of the
/// source CSV is untouched, per §6's own exception: that reads an input file
/// with one fixed format, and only display formatting becomes locale-aware.
/// </para>
/// </summary>
public sealed class LocaleService
{
    /// <summary>
    /// The authoring language, and the fallback for any key a translation is
    /// missing. A missing string falls back visibly rather than rendering empty:
    /// an English word in a Spanish sentence is a bug a person can report, and a
    /// blank space is one they cannot.
    /// </summary>
    public const AppLocale Fallback = AppLocale.En;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient http;
    private readonly Dictionary<AppLocale, LocaleResources> loaded = [];

    public LocaleService(HttpClient http) => this.http = http;

    /// <summary>Raised when the active locale changes.</summary>
    public event Action? OnChanged;

    public AppLocale Current { get; private set; } = AppLocale.En;

    /// <summary>
    /// Whether the resources are in hand. Until they are, every lookup returns
    /// its key — which is ugly on screen and is meant to be, because the app
    /// should not be rendering text before its text has arrived.
    /// </summary>
    public bool IsLoaded => loaded.Count == 3;

    /// <summary>All three locales, in the order the language switch lists them.</summary>
    public static IReadOnlyList<AppLocale> All { get; } = [AppLocale.En, AppLocale.Es, AppLocale.Pt];

    /// <summary>
    /// Loads all three locales once, at startup.
    ///
    /// <para>
    /// All three, not just the active one, and that is what makes Option B's
    /// in-session switch a dictionary lookup rather than a fetch: changing
    /// language does no I/O, cannot fail halfway, and cannot leave half a screen
    /// in one language while the rest waits on a network.
    /// </para>
    /// </summary>
    public async Task InitializeAsync()
    {
        foreach (var locale in All)
        {
            var resources = await http.GetFromJsonSafeAsync(Path(locale), JsonOptions);
            if (resources is not null)
            {
                loaded[locale] = resources;
            }
        }
    }

    /// <summary>
    /// Switches locale. Raising the event is what re-renders the tree; nothing
    /// is fetched, stored, or reloaded here.
    /// </summary>
    public void Set(AppLocale locale)
    {
        if (locale == Current)
        {
            return;
        }

        Current = locale;
        OnChanged?.Invoke();
    }

    /// <summary>
    /// A string in a named locale.
    ///
    /// <para>
    /// The locale is a parameter rather than read from <see cref="Current"/> on
    /// purpose. Every caller passes the cascading <see cref="AppLocale"/> it
    /// received, which means a component physically cannot render a string
    /// without participating in the render pass that a locale change triggers.
    /// An ambient read would compile just as well and go stale silently.
    /// </para>
    /// </summary>
    public string this[AppLocale locale, string key] => Resolve(locale, key);

    /// <summary>The same lookup with tokens substituted, e.g. <c>{count}</c>.</summary>
    public string Format(AppLocale locale, string key, params (string Token, object Value)[] values)
    {
        var text = Resolve(locale, key);

        foreach (var (token, value) in values)
        {
            text = text.Replace($"{{{token}}}", value?.ToString() ?? string.Empty, StringComparison.Ordinal);
        }

        return text;
    }

    /// <summary>Whether a key exists in a locale, without falling back.</summary>
    public bool Has(AppLocale locale, string key) =>
        loaded.TryGetValue(locale, out var resources) && resources.Strings.ContainsKey(key);

    /// <summary>Every key this locale defines. Used by the tests that hold the three files in step.</summary>
    public IReadOnlyCollection<string> Keys(AppLocale locale) =>
        loaded.TryGetValue(locale, out var resources) ? resources.Strings.Keys : [];

    /// <summary>The language's own name for itself.</summary>
    public string NameOf(AppLocale locale) =>
        loaded.TryGetValue(locale, out var resources) ? resources.Name : locale.ToString();

    /// <summary>The two-letter code, for <c>lang</c> attributes and storage.</summary>
    public static string CodeOf(AppLocale locale) => locale switch
    {
        AppLocale.Es => "es",
        AppLocale.Pt => "pt",
        _ => "en",
    };

    /// <summary>The locale a stored or declared code names, falling back to English.</summary>
    public static AppLocale FromCode(string? code) => code?.ToLowerInvariant() switch
    {
        "es" => AppLocale.Es,
        "pt" => AppLocale.Pt,
        _ => AppLocale.En,
    };

    // ------------------------------------------------------------------ dates

    /// <summary>
    /// A date, formatted by this app rather than by <c>CultureInfo</c> — see the
    /// class remarks for why. <paramref name="format"/> names a pattern the
    /// locale file defines, e.g. <c>dayMonthYear</c>.
    /// </summary>
    public string Date(AppLocale locale, DateTime value, string format)
    {
        var resources = ResourcesFor(locale);
        var pattern = resources.DateFormats.TryGetValue(format, out var found)
            ? found
            : ResourcesFor(Fallback).DateFormats.GetValueOrDefault(format, "{day} {month} {year}");

        var month = Nth(resources.Months, value.Month - 1) ?? value.Month.ToString();
        var shortMonth = Nth(resources.MonthsShort, value.Month - 1) ?? month;

        return pattern
            .Replace("{day}", value.Day.ToString(), StringComparison.Ordinal)
            .Replace("{month}", month, StringComparison.Ordinal)
            .Replace("{shortMonth}", shortMonth, StringComparison.Ordinal)
            .Replace("{year}", value.Year.ToString(), StringComparison.Ordinal)

            // 24-hour, in every locale. Standard in Spanish- and
            // Portuguese-language sports journalism as much as in English, and
            // a kickoff time read under pressure should never need an AM/PM
            // disambiguation.
            .Replace("{time}", $"{value.Hour:D2}:{value.Minute:D2}", StringComparison.Ordinal);
    }

    // -------------------------------------------------------------- internals

    private static string Path(AppLocale locale) => $"i18n/{CodeOf(locale)}.json";

    private static string? Nth(string[] values, int index) =>
        index >= 0 && index < values.Length ? values[index] : null;

    private LocaleResources ResourcesFor(AppLocale locale) =>
        loaded.TryGetValue(locale, out var resources)
            ? resources
            : loaded.GetValueOrDefault(Fallback) ?? new LocaleResources();

    private string Resolve(AppLocale locale, string key)
    {
        if (loaded.TryGetValue(locale, out var resources)
            && resources.Strings.TryGetValue(key, out var text))
        {
            return text;
        }

        if (locale != Fallback
            && loaded.TryGetValue(Fallback, out var english)
            && english.Strings.TryGetValue(key, out var fallback))
        {
            return fallback;
        }

        // Neither the locale nor English has it. Returning the key puts the
        // missing string on screen where somebody will notice, which is the
        // point — a silently empty span is a defect nobody reports.
        return key;
    }
}

internal static class LocaleHttpExtensions
{
    /// <summary>
    /// Fetches and deserializes one locale file, returning null rather than
    /// throwing.
    ///
    /// <para>
    /// A locale file that will not load must not take the app down with it. The
    /// service falls back to English for every key, which is a degraded screen
    /// rather than no screen — and the app's whole argument is that a degraded
    /// state stated plainly beats a confident failure.
    /// </para>
    /// </summary>
    public static async Task<LocaleResources?> GetFromJsonSafeAsync(
        this HttpClient http, string path, JsonSerializerOptions options)
    {
        try
        {
            var json = await http.GetStringAsync(path);
            return JsonSerializer.Deserialize<LocaleResources>(json, options);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
