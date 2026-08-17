using FifaPressApp.Models;

namespace FifaPressApp.Services;

/// <summary>
/// Whether the list is narrowed to matches that have been played, to matches
/// that have not, or to neither.
/// </summary>
public enum MatchStatusFilter
{
    /// <summary>No narrowing by status.</summary>
    All,

    /// <summary>Fixtures whose kickoff has passed.</summary>
    Played,

    /// <summary>Fixtures still to come.</summary>
    NotYetPlayed,
}

/// <summary>
/// How the match list narrows a schedule down.
///
/// <para>
/// Pure functions over fixtures, deliberately holding no state and touching no
/// component. That is the whole reason this file exists: the match list's query
/// used to live inside <c>EventList.razor</c>, where testing it meant rendering
/// a page, and the correctness of the one thing a reader most needs to trust
/// should not depend on a component-rendering package resolving.
/// </para>
///
/// <para>
/// <b>Nothing here knows about the withholding rule, and nothing here needs
/// to.</b> These functions filter whatever fixtures they are handed. The
/// provider has already decided which of them may carry a team name, so a query
/// cannot leak one by asking a different question — there is nothing on an
/// unresolved fixture to leak.
/// </para>
/// </summary>
public static class FixtureQuery
{
    /// <summary>
    /// The group selection meaning "do not narrow by group". The empty string,
    /// so it is what a <c>select</c> hands back for an option with no value.
    /// </summary>
    public const string AllGroups = "";

    /// <summary>
    /// The group selection meaning "the rounds that have no group letter".
    /// Cannot collide with a real selection: group letters are single
    /// characters, A through L.
    /// </summary>
    public const string KnockoutRounds = "knockout";

    /// <summary>
    /// The group letters actually present in a schedule, in order, derived
    /// rather than assumed.
    ///
    /// <para>
    /// Not a hardcoded A–L list, deliberately. A hardcoded list is a claim about
    /// data this function has been handed and can simply read, and it goes wrong
    /// silently the first time the schedule it describes is not the schedule it
    /// gets.
    /// </para>
    /// </summary>
    public static List<string> GroupLetters(IEnumerable<Fixture> fixtures) =>
        fixtures
            .Select(fixture => fixture.GroupLetter)
            .Where(letter => letter is not null)
            .Select(letter => letter!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>
    /// Narrows to one group, or to the knockout rounds, or not at all.
    /// </summary>
    public static List<Fixture> InGroup(IEnumerable<Fixture> fixtures, string? groupSelection) =>
        groupSelection switch
        {
            null or AllGroups => fixtures.ToList(),

            KnockoutRounds => fixtures.Where(fixture => fixture.GroupLetter is null).ToList(),

            _ => fixtures.Where(fixture =>
                string.Equals(fixture.GroupLetter, groupSelection, StringComparison.OrdinalIgnoreCase)).ToList(),
        };

    /// <summary>
    /// The free-text search, moved here unchanged from the match list.
    ///
    /// <para>
    /// <b>Four fields, case-insensitive, in this order — and that is a
    /// contract, not an implementation detail.</b> This search works, people
    /// rely on how it behaves, and the filters added around it compose with it
    /// rather than altering it. Changing which fields it reads, or how it
    /// compares them, is a behavioural change to something nobody asked to have
    /// changed.
    /// </para>
    ///
    /// <para>
    /// This overload is the original one and stays exactly as it was. The
    /// locale-aware overload below <i>adds</i> to it and never subtracts.
    /// </para>
    /// </summary>
    public static List<Fixture> Search(IEnumerable<Fixture> fixtures, string? searchTerm) =>
        string.IsNullOrWhiteSpace(searchTerm)
            ? fixtures.ToList()
            : fixtures.Where(fixture => MatchesCanonical(fixture, searchTerm)).ToList();

    /// <summary>
    /// The same search, extended to also match the fixture's label in the
    /// language the person is reading.
    ///
    /// <para>
    /// <b>Additive, and that word is doing real work.</b> Every input that
    /// matched before still matches: the canonical English fields are checked
    /// first and unchanged, and the locale's rendering of the round is checked
    /// <i>as well</i>. A person reading Spanish who types <i>octavos</i> finds
    /// the Round of 16; a person who types <i>Round of 16</i> in any language
    /// still finds it too. Nothing that used to be findable stops being
    /// findable, which is why every frozen search test passes untouched.
    /// </para>
    ///
    /// <para>
    /// Recorded as an authorized deviation: the method's own remarks above call
    /// its field list a contract, and this widens it. It widens it in the only
    /// direction that cannot break an existing expectation — a search that
    /// matched N fixtures before matches those N and possibly more, never fewer.
    /// </para>
    ///
    /// <para>
    /// The withholding rule is untouched here, as it was before: an unresolved
    /// fixture carries no team names in any language, so searching a translated
    /// label cannot surface one. What a translated label can reveal is the round
    /// — which the app already prints on the card.
    /// </para>
    /// </summary>
    public static List<Fixture> Search(
        IEnumerable<Fixture> fixtures,
        string? searchTerm,
        LocaleService locale,
        AppLocale which) =>
        string.IsNullOrWhiteSpace(searchTerm)
            ? fixtures.ToList()
            : fixtures.Where(fixture =>
                MatchesCanonical(fixture, searchTerm) ||
                FixtureLabels.Display(locale, which, fixture)
                    .Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                FixtureLabels.Phase(locale, which, fixture)
                    .Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

    /// <summary>
    /// The original four fields, unchanged, in the original order. Extracted so
    /// both overloads read from one definition and cannot drift apart.
    /// </summary>
    private static bool MatchesCanonical(Fixture fixture, string searchTerm) =>
        fixture.DisplayLabel.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        fixture.Venue.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        fixture.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        fixture.PhaseLabel.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Narrows to played or not-yet-played fixtures.
    ///
    /// <para>
    /// <b>Read from <see cref="Fixture.IsResolved"/> and from nothing else.</b>
    /// The provider sets that flag against the tournament instant it owns, and
    /// re-deriving the same answer here by comparing dates would put a second
    /// definition of "played" in the app — one that could disagree with the
    /// first, silently, on exactly the fixtures where it matters.
    /// </para>
    /// </summary>
    public static List<Fixture> WithStatus(IEnumerable<Fixture> fixtures, MatchStatusFilter status) =>
        status switch
        {
            MatchStatusFilter.Played => fixtures.Where(fixture => fixture.IsResolved).ToList(),
            MatchStatusFilter.NotYetPlayed => fixtures.Where(fixture => !fixture.IsResolved).ToList(),
            _ => fixtures.ToList(),
        };

    /// <summary>
    /// Every active control applied together.
    ///
    /// <para>
    /// The controls compose as AND: each one narrows what the one before it
    /// left. Search is applied first and unchanged, so adding a filter can only
    /// ever remove fixtures from a search result — it can never add one search
    /// would not have found, and it cannot change which fixtures search matches.
    /// </para>
    /// </summary>
    public static List<Fixture> Apply(
        IEnumerable<Fixture> fixtures,
        string? searchTerm,
        string? groupSelection,
        MatchStatusFilter status) =>
        WithStatus(InGroup(Search(fixtures, searchTerm), groupSelection), status);

    /// <summary>
    /// The same composition, searching in the reader's language as well as the
    /// canonical one.
    /// </summary>
    public static List<Fixture> Apply(
        IEnumerable<Fixture> fixtures,
        string? searchTerm,
        string? groupSelection,
        MatchStatusFilter status,
        LocaleService locale,
        AppLocale which) =>
        WithStatus(InGroup(Search(fixtures, searchTerm, locale, which), groupSelection), status);
}
