namespace FifaPressApp.Models;

/// <summary>
/// The tournament phase a fixture belongs to. These are the only values the
/// schedule contains: twelve groups plus six knockout rounds.
/// </summary>
public enum PhaseKind
{
    GroupStage,
    RoundOf32,
    RoundOf16,
    QuarterFinals,
    SemiFinals,
    ThirdPlace,
    Final,
}

/// <summary>
/// One scheduled match.
///
/// <para>
/// <b>The two team-name fields are deliberately nullable, and that is the whole
/// point of this type.</b> The schedule this app reads is a record of a
/// completed tournament: every knockout row already names two real teams, so
/// reading it forward tells you who won every match before it is played. A
/// fixture that has not kicked off yet is therefore handed out with
/// <see cref="HomeLabel"/> and <see cref="AwayLabel"/> set to <c>null</c> — the
/// values are never copied onto the instance in the first place, so there is
/// nothing on a public <c>Fixture</c> for a caller to accidentally read.
/// </para>
///
/// <para>
/// That withholding is applied by the data provider, at one place, on every
/// read path. It is not a display convention, because a display convention can
/// be forgotten by the next person who adds a query.
/// </para>
/// </summary>
public sealed record Fixture
{
    /// <summary>Natural key, 1-104.</summary>
    public required int MatchNumber { get; init; }

    /// <summary>Kickoff on the venue's own clock. This is the field the
    /// resolved/unresolved decision is made against.</summary>
    public required DateTime KickoffLocal { get; init; }

    /// <summary>Kickoff on Eastern time — the schedule's only cross-venue
    /// common clock, retained for that reason.</summary>
    public required DateTime KickoffEastern { get; init; }

    /// <summary>
    /// MOCKED. The schedule carries no zone identifier, and a zone label is
    /// mandatory across three host countries, so the app supplies this from a
    /// city lookup rather than receiving it. Labelled as mocked wherever it is
    /// displayed.
    /// </summary>
    public required string TimeZoneLabel { get; init; }

    public required PhaseKind Phase { get; init; }

    /// <summary>
    /// "A" through "L" for a group-stage fixture, <c>null</c> for a knockout
    /// round. Carried separately from <see cref="Phase"/> so the twelve groups
    /// do not need twelve enum members.
    /// </summary>
    public string? GroupLetter { get; init; }

    /// <summary>Home side. <c>null</c> whenever <see cref="IsResolved"/> is
    /// false — see the type-level note.</summary>
    public string? HomeLabel { get; init; }

    /// <summary>Away side. <c>null</c> whenever <see cref="IsResolved"/> is
    /// false — see the type-level note.</summary>
    public string? AwayLabel { get; init; }

    public required string Venue { get; init; }

    public required string City { get; init; }

    /// <summary>
    /// Whether this fixture's kickoff is at or before the simulated tournament
    /// instant the provider holds. Set by the provider, since the instant is
    /// the provider's to own.
    /// </summary>
    public required bool IsResolved { get; init; }

    /// <summary>
    /// How to name this fixture on screen without leaking an unresolved result.
    /// A resolved fixture reads "Spain v Austria"; an unresolved one reads
    /// "Round of 16" and says plainly that the teams are not decided yet.
    /// </summary>
    public string DisplayLabel => IsResolved && HomeLabel is not null && AwayLabel is not null
        ? $"{HomeLabel} v {AwayLabel}"
        : $"{PhaseLabel} — teams not yet decided";

    /// <summary>Human-readable phase, including the group letter where there
    /// is one.</summary>
    public string PhaseLabel => Phase switch
    {
        PhaseKind.GroupStage => $"Group {GroupLetter}",
        PhaseKind.RoundOf32 => "Round of 32",
        PhaseKind.RoundOf16 => "Round of 16",
        PhaseKind.QuarterFinals => "Quarter-finals",
        PhaseKind.SemiFinals => "Semi-finals",
        PhaseKind.ThirdPlace => "Third Place",
        PhaseKind.Final => "Final",
        _ => "Fixture",
    };
}
