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
}
