using Microsoft.AspNetCore.SignalR.Client;

namespace FifaPressApp.Services;

/// <summary>
/// Holds the open connection to the API's change hub, and raises the record's
/// arrival signal when the server says something was written.
///
/// <para>
/// <b>Why a connection that stays open, rather than asking repeatedly.</b> This
/// app exists to argue that a change to somebody's access should reach them
/// before they discover it by being refused. Polling would narrow that gap
/// without closing it, and would spend a request every few seconds to learn
/// "nothing yet" — which on a phone in a stadium concourse is the wrong thing
/// to spend a battery and a data allowance on. A connection the server can
/// speak down costs one handshake and then stays quiet until there is something
/// to say.
/// </para>
///
/// <para>
/// <b>The message is a prompt, not the data.</b> The server sends a credential
/// id and a change id and nothing else; this class then tells the record screen
/// to re-read over HTTP. The alternative — pushing the change itself and
/// trusting it — would give the app two sources for what the record says, and
/// the whole concept depends on there being one.
/// </para>
///
/// <para>
/// <b>Started only when an API is configured</b>, and failure to connect is
/// survivable by design: the app works exactly as it always has without this
/// class, so a hub that never comes up costs the live update and nothing else.
/// </para>
/// </summary>
public sealed class ChangeNotificationClient(ChangeArrivalTracker arrivals) : IAsyncDisposable
{
    private HubConnection? connection;

    /// <summary>Whether the hub connection is currently open.</summary>
    public bool IsConnected => connection?.State == HubConnectionState.Connected;

    /// <summary>
    /// Opens the connection and starts listening.
    ///
    /// <para>
    /// The token goes on the query string because that is where SignalR puts
    /// it: a browser cannot set an Authorization header on a WebSocket
    /// handshake. It is the same simulated token every other call uses, and it
    /// is not a secret — see the API's own authentication middleware.
    /// </para>
    /// </summary>
    public async Task StartAsync(string hubUrl, string token)
    {
        connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options => options.AccessTokenProvider = () => Task.FromResult<string?>(token))

            // Free hosting tiers stop an idle process, so a dropped connection
            // is an ordinary event here rather than an exceptional one.
            .WithAutomaticReconnect()
            .Build();

        connection.On<string, string>("ChangeRecorded", (credentialId, changeId) =>
        {
            // Announce, then raise. The record screen consumes the id to mark
            // the new row as having just arrived, exactly as it already does for
            // a change the person wrote themselves — the entrance treatment was
            // built for that case and means the same thing here.
            arrivals.Announce(changeId);
            arrivals.RaiseChanged();
        });

        await connection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (connection is not null)
        {
            await connection.DisposeAsync();
        }
    }
}
