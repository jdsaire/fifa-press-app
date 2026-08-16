using System.Globalization;
using FifaPressApp.Models;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The importer's three real parse hazards, plus its guards, plus a pass over
/// the tracked file itself.
///
/// <para>
/// These exist to protect what v9 already established, before anything in this
/// run changes. Each hazard below is a genuine property of the published
/// schedule, not an invented edge case: the file really does carry single-digit
/// dates, two teams in one column, and three rows whose clock reads 24:00.
/// </para>
/// </summary>
public class FixtureImporterTests
{
    private const string Header = "Match,Date,Time (ET),Time (Local),Matchup,Group / Phase,Venue,City";

    private static string Row(string fields) => $"{Header}\n{fields}\n";

    [Fact]
    public void Eastern2400_RollsToMidnightTheFollowingDay()
    {
        // The real shape of line 7 of the tracked file: a 21:00 local kickoff in
        // Vancouver recorded as 24:00 Eastern. Midnight closing 13 June, not an
        // hour any clock shows.
        var result = FixtureImporter.Parse(
            Row("6,13-Jun-26,24:00,21:00,Australia v Türkiye,Group D,BC Place,Vancouver"));

        var fixture = Assert.Single(result.Fixtures);
        Assert.Equal(new DateTime(2026, 6, 14, 0, 0, 0), fixture.KickoffEastern);
        Assert.Equal(new DateTime(2026, 6, 13, 21, 0, 0), fixture.KickoffLocal);
    }

    [Fact]
    public void Eastern2500_IsRejected_BecauseOnly2400HasAMeaning()
    {
        // 24:00 is rolled because the schedule means midnight by it. Nothing
        // beyond it is a time, and rolling those too would turn a corrupt row
        // into a plausible one.
        Assert.Throws<FormatException>(() => FixtureImporter.Parse(
            Row("6,13-Jun-26,25:00,21:00,Australia v Türkiye,Group D,BC Place,Vancouver")));
    }

    [Theory]
    [InlineData("de-DE")]
    [InlineData("es-PE")]
    [InlineData("tr-TR")]
    public void DateColumn_ParsesTheSameUnderAnyMachineCulture(string culture)
    {
        // The reason ParseDate names InvariantCulture explicitly. Without it,
        // "11-Jun-26" lands on a different date, or fails outright, depending on
        // what the machine running the build happens to be set to.
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            var result = FixtureImporter.Parse(
                Row("1,11-Jun-26,15:00,13:00,Mexico v South Africa,Group A,Estadio Azteca,Mexico City"));

            Assert.Equal(new DateTime(2026, 6, 11, 13, 0, 0), Assert.Single(result.Fixtures).KickoffLocal);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Fact]
    public void MatchupColumn_SplitsOnSpaceVSpace()
    {
        var result = FixtureImporter.Parse(
            Row("1,11-Jun-26,15:00,13:00,Mexico v South Africa,Group A,Estadio Azteca,Mexico City"));

        var matchup = result.Matchups[1];
        Assert.Equal("Mexico", matchup.Home);
        Assert.Equal("South Africa", matchup.Away);
    }

    [Fact]
    public void MatchupColumn_WithNoSeparator_IsRejected()
    {
        Assert.Throws<FormatException>(() => FixtureImporter.Parse(
            Row("1,11-Jun-26,15:00,13:00,Mexico versus South Africa,Group A,Estadio Azteca,Mexico City")));
    }

    [Fact]
    public void RowWithWrongColumnCount_IsRejectedByLineNumber()
    {
        // A schedule that silently dropped rows would give every screen above it
        // a quietly wrong answer, so the importer fails loudly and says where.
        var error = Assert.Throws<FormatException>(() => FixtureImporter.Parse(
            Row("1,11-Jun-26,15:00,13:00,Mexico v South Africa,Group A,Estadio Azteca")));

        Assert.Contains("Line 2", error.Message);
        Assert.Contains("expected 8 columns, found 7", error.Message);
    }

    [Fact]
    public void GroupRow_CarriesItsLetter_AndKnockoutRowCarriesNone()
    {
        // The knockout row's sides are deliberately placeholders rather than the
        // pairing the real file records. A knockout pairing IS the result of the
        // round before it, and writing one into a test file would put in this
        // repository exactly what the app refuses to hand out.
        var result = FixtureImporter.Parse(
            $"{Header}\n"
            + "1,11-Jun-26,15:00,13:00,Mexico v South Africa,Group A,Estadio Azteca,Mexico City\n"
            + "89,04-Jul-26,16:00,16:00,Placeholder One v Placeholder Two,Round of 16,AT&T Stadium,Dallas\n");

        var group = result.Fixtures[0];
        Assert.Equal(PhaseKind.GroupStage, group.Phase);
        Assert.Equal("A", group.GroupLetter);
        Assert.Equal("Group A", group.PhaseLabel);

        var knockout = result.Fixtures[1];
        Assert.Equal(PhaseKind.RoundOf16, knockout.Phase);
        Assert.Null(knockout.GroupLetter);
        Assert.Equal("Round of 16", knockout.PhaseLabel);
    }

    [Fact]
    public void TrackedSchedule_ParsesEveryRow()
    {
        var result = FixtureImporter.Parse(TestData.ScheduleCsv());

        Assert.Equal(TestData.ScheduleRowCount, result.Fixtures.Count);
        Assert.Equal(TestData.ScheduleRowCount, result.Matchups.Count);
        Assert.Equal(
            Enumerable.Range(1, TestData.ScheduleRowCount),
            result.Fixtures.Select(fixture => fixture.MatchNumber).Order());
    }

    [Fact]
    public void TrackedSchedule_YieldsNoFixtureCarryingATeamName()
    {
        // The first half of the withholding rule: the importer never puts a name
        // on a fixture, for any row, played or not. Only the provider does, and
        // only for a fixture that has kicked off.
        var result = FixtureImporter.Parse(TestData.ScheduleCsv());

        Assert.All(result.Fixtures, fixture =>
        {
            Assert.Null(fixture.HomeLabel);
            Assert.Null(fixture.AwayLabel);
            Assert.False(fixture.IsResolved);
        });
    }

    [Fact]
    public void TrackedSchedule_CarriesTwelveGroupsAndSixKnockoutRounds()
    {
        var result = FixtureImporter.Parse(TestData.ScheduleCsv());

        Assert.Equal(
            new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" },
            result.Fixtures.Where(fixture => fixture.GroupLetter is not null)
                .Select(fixture => fixture.GroupLetter!)
                .Distinct()
                .Order());

        Assert.Equal(
            new[]
            {
                PhaseKind.RoundOf32, PhaseKind.RoundOf16, PhaseKind.QuarterFinals,
                PhaseKind.SemiFinals, PhaseKind.ThirdPlace, PhaseKind.Final,
            },
            result.Fixtures.Where(fixture => fixture.GroupLetter is null)
                .Select(fixture => fixture.Phase)
                .Distinct()
                .Order());
    }
}
