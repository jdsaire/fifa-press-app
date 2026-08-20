namespace FifaPressApp.Services;

/// <summary>
/// Which change, if any, has just arrived in the record because this person
/// wrote it.
///
/// <para>
/// <b>Why a service and not a field on a page.</b> Submitting a request
/// navigates from <c>/request/{id}</c> to <c>/record</c>, which mounts the
/// record screen fresh — so the screen that needs to know a row is new is not
/// the screen that knows it. Something has to survive that navigation, and a
/// singleton is the smallest thing that does.
/// </para>
///
/// <para>
/// <b>Asked once and cleared by the asking</b>, the same shape as the
/// sign-out announcement. An entrance treatment that replayed on every refresh
/// would stop meaning "this just happened" and start meaning "this row is
/// decorated", which is the opposite of a confirmation.
/// </para>
///
/// <para>
/// It holds an id and nothing else. It is not a second copy of the log, it
/// cannot disagree with the record, and losing it costs the animation and
/// nothing more.
/// </para>
/// </summary>
public sealed class ChangeArrivalTracker
{
    private string? justWritten;

    /// <summary>
    /// Raised when a change has arrived from somewhere other than this browser
    /// — which in practice means the API pushed one down the hub.
    ///
    /// <para>
    /// <b>Why this event exists at all.</b> The record screen reloads when the
    /// session changes, because a person can sign in while it is already on
    /// screen. Nothing else could ever make it reload, because until now nothing
    /// else could change the record while somebody was looking at it. A pushed
    /// change can, and a screen that received one and did not repaint would be
    /// the exact failure this project is about: the record is correct and the
    /// holder still cannot see it.
    /// </para>
    ///
    /// <para>
    /// Nothing raises this in the app's default configuration. With no API
    /// configured there is no hub, no push, and no subscriber ever hears
    /// anything — the screen behaves precisely as it did before this event was
    /// added.
    /// </para>
    /// </summary>
    public event Action? OnChanged;

    /// <summary>
    /// Records that a change has just been written, so the record screen can
    /// mark its arrival rather than letting it appear indistinguishable from the
    /// rows that were already there.
    /// </summary>
    public void Announce(string changeId) => justWritten = changeId;

    /// <summary>
    /// Tells whoever is listening that the record has moved and should be read
    /// again. Separate from <see cref="Announce"/> because the two happen
    /// together for a pushed change and separately for a written one: a person
    /// who submits a request navigates to a freshly mounted screen, which reads
    /// the record on its own without needing to be told.
    /// </summary>
    public void RaiseChanged() => OnChanged?.Invoke();

    /// <summary>
    /// The change that just arrived, or null. Clears as it answers.
    /// </summary>
    public string? Consume()
    {
        var pending = justWritten;
        justWritten = null;
        return pending;
    }
}
