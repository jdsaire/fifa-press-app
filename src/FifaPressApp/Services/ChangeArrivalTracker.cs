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
    /// Records that a change has just been written, so the record screen can
    /// mark its arrival rather than letting it appear indistinguishable from the
    /// rows that were already there.
    /// </summary>
    public void Announce(string changeId) => justWritten = changeId;

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
