using FifaPressApp.Models;

namespace FifaPressApp.Services;

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
    /// </summary>
    public static List<Fixture> Search(IEnumerable<Fixture> fixtures, string? searchTerm) =>
        string.IsNullOrWhiteSpace(searchTerm)
            ? fixtures.ToList()
            : fixtures.Where(fixture =>
                fixture.DisplayLabel.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                fixture.Venue.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                fixture.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                fixture.PhaseLabel.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

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
        string? groupSelection) =>
        InGroup(Search(fixtures, searchTerm), groupSelection);
}
