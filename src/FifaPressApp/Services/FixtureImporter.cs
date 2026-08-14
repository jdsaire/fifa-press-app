using System.Globalization;
using FifaPressApp.Models;

namespace FifaPressApp.Services;

/// <summary>
/// The two team names on one schedule row, kept apart from the fixture itself.
/// </summary>
public readonly record struct Matchup(string Home, string Away);

/// <summary>
/// What a parse of the schedule produces: the fixtures, and — held separately —
/// the team names each row named.
///
/// <para>
/// <b>The separation is deliberate and is the first half of the withholding
/// rule.</b> The fixtures in <see cref="Fixtures"/> come out of the importer
/// with no team names on them at all. The names live in
/// <see cref="Matchups"/>, keyed by match number, and only the provider decides
/// which of them a caller is allowed to see. A <c>Fixture</c> carrying a team
/// name it should not have is therefore not something this code can produce by
/// accident — it can only be produced deliberately, in one method, in one file.
/// </para>
/// </summary>
public sealed record FixtureImportResult(
    IReadOnlyList<Fixture> Fixtures,
    IReadOnlyDictionary<int, Matchup> Matchups);

/// <summary>
/// Turns the published schedule CSV into fixtures.
///
/// <para>
/// Deliberately a separate concern from the data provider: this class knows how
/// to read a file format and nothing about entitlements, tournament instants or
/// withholding. The provider consumes what this produces; it does not parse.
/// </para>
///
/// <para>
/// <b>Three things in this file are real-world parsing, not ceremony.</b> The
/// date column is <c>d-MMM-yy</c> with single-digit days, which needs an
/// explicit format and a fixed culture or it silently parses differently on a
/// machine set to another locale. The matchup column is one string holding two
/// teams joined by <c>" v "</c>, so it has to be split rather than read. And
/// the Eastern-time column contains <c>24:00</c> on three rows, which is not a
/// valid clock time at all — it means midnight ending that day, and has to roll
/// to <c>00:00</c> on the next one.
/// </para>
/// </summary>
public static class FixtureImporter
{
    private const string DateFormat = "d-MMM-yy";
    private const string MatchupSeparator = " v ";

    /// <summary>
    /// MOCKED. The schedule carries no zone identifier for any row, so the app
    /// supplies one from the host city. Each label below is consistent with the
    /// offset between the file's own two clock columns, which is the only check
    /// the data itself makes possible.
    /// </summary>
    private static readonly Dictionary<string, string> ZoneByCity = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Atlanta"] = "Eastern Time (EDT, UTC-4)",
        ["Boston"] = "Eastern Time (EDT, UTC-4)",
        ["Miami"] = "Eastern Time (EDT, UTC-4)",
        ["New York/New Jersey"] = "Eastern Time (EDT, UTC-4)",
        ["Philadelphia"] = "Eastern Time (EDT, UTC-4)",
        ["Toronto"] = "Eastern Time (EDT, UTC-4)",
        ["Dallas"] = "Central Time (CDT, UTC-5)",
        ["Houston"] = "Central Time (CDT, UTC-5)",
        ["Kansas City"] = "Central Time (CDT, UTC-5)",
        ["Guadalajara"] = "Central Time (CST, UTC-6)",
        ["Mexico City"] = "Central Time (CST, UTC-6)",
        ["Monterrey"] = "Central Time (CST, UTC-6)",
        ["Los Angeles"] = "Pacific Time (PDT, UTC-7)",
        ["San Francisco Bay Area"] = "Pacific Time (PDT, UTC-7)",
        ["Seattle"] = "Pacific Time (PDT, UTC-7)",
        ["Vancouver"] = "Pacific Time (PDT, UTC-7)",
    };

    /// <summary>
    /// Parses the whole file. Throws <see cref="FormatException"/> naming the
    /// offending line if any row does not parse — a schedule that silently
    /// dropped rows would give every screen above it a quietly wrong answer,
    /// which is worse than failing here.
    /// </summary>
    public static FixtureImportResult Parse(string csv)
    {
        ArgumentNullException.ThrowIfNull(csv);

        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimEnd('\r'))
            .Where(line => line.Length > 0)
            .ToArray();

        if (lines.Length < 2)
        {
            throw new FormatException("The schedule file has no data rows.");
        }

        var fixtures = new List<Fixture>(lines.Length - 1);
        var matchups = new Dictionary<int, Matchup>(lines.Length - 1);

        // Row 1 is the header; every row after it is a fixture.
        for (var i = 1; i < lines.Length; i++)
        {
            var lineNumber = i + 1;
            var fields = lines[i].Split(',');

            if (fields.Length != 8)
            {
                throw new FormatException(
                    $"Line {lineNumber}: expected 8 columns, found {fields.Length}.");
            }

            var matchNumber = ParseInt(fields[0], lineNumber, "match number");
            var date = ParseDate(fields[1], lineNumber);
            var kickoffEastern = ParseClock(date, fields[2], lineNumber, "Eastern");
            var kickoffLocal = ParseClock(date, fields[3], lineNumber, "local");
            var (home, away) = ParseMatchup(fields[4], lineNumber);
            var (phase, groupLetter) = ParsePhase(fields[5], lineNumber);
            var venue = fields[6].Trim();
            var city = fields[7].Trim();

            fixtures.Add(new Fixture
            {
                MatchNumber = matchNumber,
                KickoffLocal = kickoffLocal,
                KickoffEastern = kickoffEastern,
                TimeZoneLabel = ZoneByCity.TryGetValue(city, out var zone) ? zone : "Local time",
                Phase = phase,
                GroupLetter = groupLetter,

                // Left null on purpose. Only the provider fills these in, and
                // only for a fixture that has already kicked off.
                HomeLabel = null,
                AwayLabel = null,

                Venue = venue,
                City = city,
                IsResolved = false,
            });

            matchups[matchNumber] = new Matchup(home, away);
        }

        return new FixtureImportResult(fixtures, matchups);
    }

    private static int ParseInt(string value, int lineNumber, string what)
    {
        if (!int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            throw new FormatException($"Line {lineNumber}: '{value}' is not a valid {what}.");
        }

        return parsed;
    }

    private static DateTime ParseDate(string value, int lineNumber)
    {
        // InvariantCulture is not decoration here: "11-Jun-26" parses against a
        // machine set to English and fails, or lands on a different date,
        // against one set to something else.
        if (!DateTime.TryParseExact(
                value.Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            throw new FormatException(
                $"Line {lineNumber}: '{value}' is not a date in {DateFormat} form.");
        }

        return parsed;
    }

    private static DateTime ParseClock(DateTime date, string value, int lineNumber, string which)
    {
        var text = value.Trim();
        var parts = text.Split(':');

        if (parts.Length != 2 ||
            !int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var hour) ||
            !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var minute))
        {
            throw new FormatException(
                $"Line {lineNumber}: '{value}' is not a valid {which} kickoff time.");
        }

        // Three rows in this file record a 22:00 Eastern kickoff on the west
        // coast as "24:00" — midnight closing that date, not an hour that
        // exists on a clock. Rolling it to 00:00 the next day is what the
        // schedule means; parsing it strictly would just reject the row.
        if (hour == 24 && minute == 0)
        {
            return date.AddDays(1);
        }

        if (hour is < 0 or > 23 || minute is < 0 or > 59)
        {
            throw new FormatException(
                $"Line {lineNumber}: '{value}' is not a valid {which} kickoff time.");
        }

        return date.AddHours(hour).AddMinutes(minute);
    }

    private static (string Home, string Away) ParseMatchup(string value, int lineNumber)
    {
        var text = value.Trim();
        var separator = text.IndexOf(MatchupSeparator, StringComparison.Ordinal);

        if (separator < 0)
        {
            throw new FormatException(
                $"Line {lineNumber}: '{value}' does not name two teams separated by '{MatchupSeparator.Trim()}'.");
        }

        var home = text[..separator].Trim();
        var away = text[(separator + MatchupSeparator.Length)..].Trim();

        if (home.Length == 0 || away.Length == 0)
        {
            throw new FormatException($"Line {lineNumber}: '{value}' names an empty side.");
        }

        return (home, away);
    }

    private static (PhaseKind Phase, string? GroupLetter) ParsePhase(string value, int lineNumber)
    {
        var text = value.Trim();

        if (text.StartsWith("Group ", StringComparison.OrdinalIgnoreCase))
        {
            var letter = text["Group ".Length..].Trim();
            if (letter.Length == 0)
            {
                throw new FormatException($"Line {lineNumber}: '{value}' names no group letter.");
            }

            return (PhaseKind.GroupStage, letter.ToUpperInvariant());
        }

        return text.ToLowerInvariant() switch
        {
            "round of 32" => (PhaseKind.RoundOf32, null),
            "round of 16" => (PhaseKind.RoundOf16, null),
            "quarter-finals" => (PhaseKind.QuarterFinals, null),
            "semi-finals" => (PhaseKind.SemiFinals, null),
            "third place" => (PhaseKind.ThirdPlace, null),
            "final" => (PhaseKind.Final, null),
            _ => throw new FormatException($"Line {lineNumber}: '{value}' is not a known phase."),
        };
    }
}
