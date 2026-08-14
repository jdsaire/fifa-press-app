using FifaPressApp.Models;

namespace FifaPressApp.Services;

/// <summary>
/// A single read result, together with how fresh it is and where it came from.
///
/// <para>
/// Every read returns one of these rather than a bare value, so the caller
/// never has to guess how old what it is holding is. That is what makes
/// "this screen tells you how stale it is" implementable rather than
/// aspirational: the freshness travels with the data instead of being
/// remembered separately by whichever page happened to ask.
/// </para>
/// </summary>
/// <param name="Value">The data itself.</param>
/// <param name="LastSyncedUtc">When this data was last synchronised.</param>
/// <param name="WasServedFromCache">
/// Whether this came from local state rather than a fetch. Cache-versus-fetched
/// is a property of the response, deliberately, rather than two different
/// methods a caller has to choose between.
/// </param>
public sealed record AccessResponse<T>(T Value, DateTime LastSyncedUtc, bool WasServedFromCache);

/// <summary>
/// The data-access contract for the access record.
///
/// <para>
/// <b>Written before anything that consumes it.</b> Every page and component in
/// this app talks to this interface, never to a concrete provider, so replacing
/// the in-memory implementation with one that talks to a real service is a swap
/// rather than a rewrite. The entities, the ordering rules and the withholding
/// rule below are all implementation-agnostic; only the class behind this
/// interface would change.
/// </para>
///
/// <para>
/// <b>Four rules this interface carries so its callers do not have to:</b>
/// </para>
/// <list type="number">
/// <item>Every read returns its own freshness, via <see cref="AccessResponse{T}"/>.</item>
/// <item>Reads resolve from local state first and the network second.</item>
/// <item>Writes return the resulting <see cref="Change"/>, never a success flag —
/// a write that returned <c>bool</c> would permit a state movement with no log
/// record, which is the one thing this record exists to prevent.</item>
/// <item>No method hands out team names for a fixture that has not kicked off.
/// The withholding lives here, in the implementation, so a future caller cannot
/// bypass it by forgetting about it.</item>
/// </list>
/// </summary>
public interface IAccessDataProvider
{
    /// <summary>
    /// The simulated tournament instant this provider reads the schedule
    /// against. A fixture at or before this moment is resolved and may name its
    /// teams; anything after it is a scheduled fixture and may not.
    ///
    /// <para>
    /// A simulation device, and only that. A version of this app talking to a
    /// real service replaces it with the actual clock, at which point the
    /// withholding rule stops being a containment measure and is simply true.
    /// </para>
    /// </summary>
    DateTime AsOfUtc { get; }

    /// <summary>The standing credential for a holder.</summary>
    Task<AccessResponse<Accreditation?>> GetAccreditationAsync(string credentialId);

    /// <summary>
    /// The holder's changes, ordered by when each one takes effect, newest
    /// first — not by when it was written.
    /// </summary>
    Task<AccessResponse<IReadOnlyList<Change>>> GetChangesAsync(string credentialId);

    /// <summary>All fixtures, with team names withheld on unresolved rows.</summary>
    Task<AccessResponse<IReadOnlyList<Fixture>>> GetFixturesAsync();

    /// <summary>One fixture, under the same withholding rule.</summary>
    Task<AccessResponse<Fixture?>> GetFixtureAsync(int matchNumber);

    /// <summary>
    /// The write path. Returns the change it produced, because the change *is*
    /// the outcome — there is nothing else for a caller to be told.
    /// </summary>
    Task<Change> RequestMatchAccessAsync(string credentialId, int matchNumber);

    /// <summary>
    /// Withdrawing is a write, not an undo: it appends a new change rather than
    /// unwriting the one it follows.
    /// </summary>
    Task<Change> WithdrawRequestAsync(string credentialId, string changeId);
}
