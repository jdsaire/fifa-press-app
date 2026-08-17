namespace FifaPressApp.Services;

/// <summary>
/// Who is signed in, for the length of one browser tab.
///
/// <para>
/// <b>This is not ASP.NET Identity, and it is not an authentication system.</b>
/// It is the <c>AuthenticationStateProvider</c>-equivalent this app can actually
/// have: <c>Microsoft.AspNetCore.Components.Authorization</c> is not in this
/// project's dependency graph — only <c>Microsoft.AspNetCore.Authorization</c>
/// is — so the framework's own provider and <c>AuthorizeView</c> are unreachable
/// without a new package reference, which this run is not permitted to add. What
/// is built here is the same shape, honestly named.
/// </para>
///
/// <para>
/// There is no token, no cookie, no server, and nothing is verified. The session
/// is a field on a singleton, which means it lasts exactly as long as the tab
/// and dies on refresh. That is deliberate rather than unfinished: a session
/// that survived a refresh would need storage, and storage would make this look
/// more like an account system than it is. The sign-in screen says so on screen.
/// </para>
///
/// <para>
/// What the session genuinely does control is which of two seeded records the
/// app reads, and whether the record is reachable at all. That is a real change
/// in what renders — it is simply not a security boundary, and nothing in this
/// app may describe it as one.
/// </para>
/// </summary>
public sealed class SimulatedSessionProvider
{
    /// <summary>
    /// How long the simulated sign-in takes.
    ///
    /// <para>
    /// The same device, for the same reason, as
    /// <see cref="MockAccessDataProvider.SimulatedWriteLatency"/>: an in-memory
    /// lookup returns without ever yielding, so the caller's continuation runs
    /// before the framework renders and the Submitting state — the disabled
    /// fields, the "Signing in…" label — never reaches the screen at all. The
    /// delay is what makes a specified state observable rather than notional.
    /// </para>
    ///
    /// <para>
    /// Sign-in only. Reads gain nothing: the record still paints on the first
    /// render with no spinner in front of it. Signing in is a write-shaped
    /// action; reading a record is not.
    /// </para>
    /// </summary>
    public static readonly TimeSpan SimulatedSignInLatency = TimeSpan.FromMilliseconds(600);

    private readonly DemoAccountStore accounts;

    public SimulatedSessionProvider(DemoAccountStore accounts) => this.accounts = accounts;

    /// <summary>
    /// Raised whenever the session starts or ends, so the layout's indicator and
    /// the nav's sign-out row re-render without every screen having to know they
    /// exist.
    /// </summary>
    public event Action? OnChanged;

    /// <summary>The account whose record is currently being read, if any.</summary>
    public DemoAccount? Current { get; private set; }

    public bool IsSignedIn => Current is not null;

    /// <summary>
    /// The credential the record screens read. Null when signed out, which is
    /// what makes forgetting to check the session fail loudly rather than
    /// quietly showing somebody else's record.
    /// </summary>
    public string? CredentialId => Current?.CredentialId;

    /// <summary>
    /// Attempts a sign-in. Returns whether it succeeded; the caller shows one
    /// generic failure either way, so there is nothing more to hand back.
    /// </summary>
    public async Task<bool> SignInAsync(string? identifier, string? password)
    {
        // The await genuinely yields, which is the whole point — see the
        // constant's own remarks.
        await Task.Delay(SimulatedSignInLatency);

        var account = accounts.Match(identifier, password);
        if (account is null)
        {
            return false;
        }

        Current = account;
        OnChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Ends the session. No confirmation is asked for anywhere that calls this:
    /// there is nothing to lose, and a dialog guarding a simulated session would
    /// be theatre.
    /// </summary>
    public void SignOut()
    {
        if (Current is null)
        {
            return;
        }

        Current = null;
        signedOutToAnnounce = true;
        OnChanged?.Invoke();
    }

    private bool signedOutToAnnounce;

    /// <summary>
    /// Whether a sign-out has happened that the landing has not yet
    /// acknowledged, cleared by the asking.
    ///
    /// <para>
    /// Signing out has to say plainly that it happened, and the landing is where
    /// the person lands. A query string would put the announcement in the
    /// address bar, where it survives a refresh, gets shared, and outlives the
    /// event it describes — a one-shot flag says it once, to the person it
    /// happened to.
    /// </para>
    /// </summary>
    public bool ConsumeSignOutAnnouncement()
    {
        var pending = signedOutToAnnounce;
        signedOutToAnnounce = false;
        return pending;
    }
}
