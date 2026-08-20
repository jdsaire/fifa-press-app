using Microsoft.AspNetCore.SignalR;

namespace FifaPressApp.Api.Realtime;

/// <summary>
/// The one persistent connection this API offers: a channel that stays open so
/// the server can tell a connected client that a change was written, instead of
/// waiting to be asked.
///
/// <para>
/// <b>Why this project wanted one.</b> The Access Record concept is, in one
/// sentence, that a change to somebody's access should reach them before they
/// discover it by being refused at a gate. Every version of this app until now
/// has failed that sentence in the same quiet way: the record was correct, but
/// it only became correct on screen when the holder happened to reload the
/// page. Polling would close the gap by making the client ask repeatedly, which
/// is the same design with a shorter interval. A connection the server can
/// speak down is the concept working as designed rather than approximated.
/// </para>
///
/// <para>
/// <b>Deliberately one hub with one message.</b> There are no groups, no
/// per-holder channels, no acknowledgements and no reconnect protocol beyond
/// what the SignalR client does by default. A production system would want at
/// least per-holder addressing — see the note on
/// <see cref="ChangeNotifier.ChangeRecorded"/> — and this does not have it,
/// because the point here is to demonstrate the mechanism honestly, not to
/// build a notification platform.
/// </para>
///
/// <para>
/// <b>The connection is subject to the same simulated token check as every
/// other route</b>, which for a browser means the token arrives on the query
/// string, because a browser cannot set headers on a WebSocket handshake. It is
/// still not authentication. See <see cref="Middleware.TokenAuthenticationMiddleware"/>.
/// </para>
///
/// <para>
/// The hub class itself is empty on purpose: clients only listen here. Nothing
/// a client could send would be trusted, since writing to the record is what
/// the HTTP endpoints are for, and giving the hub a client-callable write
/// method would create a second way into the log that validation does not
/// guard.
/// </para>
/// </summary>
public sealed class ChangeNotificationHub : Hub;

/// <summary>
/// Sends the one message this hub broadcasts.
///
/// <para>
/// A small class rather than the raw <see cref="IHubContext{THub}"/> so the
/// endpoint that writes a change does not also have to know the client method
/// name — that string appears once, here, instead of at every call site where
/// it could drift.
/// </para>
/// </summary>
public sealed class ChangeNotifier(IHubContext<ChangeNotificationHub> hub)
{
    /// <summary>
    /// The client-side method name. The frontend registers a handler under
    /// exactly this string; a typo on either side produces a connection that
    /// works and never fires, which is the failure mode worth naming once here.
    /// </summary>
    public const string ChangeRecorded = "ChangeRecorded";

    /// <summary>
    /// Tells every connected client that a change was written to a record.
    ///
    /// <para>
    /// <b>Sent to all clients, and it carries the credential id so each one can
    /// decide whether it cares.</b> A real deployment would address the holder
    /// directly rather than broadcasting and filtering at the edge — that is
    /// what SignalR groups are for, and doing it this way means every connected
    /// browser learns that *some* record changed. That is acceptable here
    /// because the only thing sent is a credential id that is already published
    /// on the sign-in screen, and because there are two demo holders rather
    /// than a real population. It would not be acceptable in production, and
    /// this remark exists so nobody promotes it there believing it was designed
    /// for it.
    /// </para>
    ///
    /// <para>
    /// Only the credential id and the change id travel. The client re-reads the
    /// record over HTTP rather than trusting a change pushed down the wire —
    /// one source of truth for what the record says, and the notification is
    /// only ever a prompt to go and look.
    /// </para>
    /// </summary>
    public Task NotifyChangeRecordedAsync(string credentialId, string changeId) =>
        hub.Clients.All.SendAsync(ChangeRecorded, credentialId, changeId);
}
