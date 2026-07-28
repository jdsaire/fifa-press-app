namespace EventEase.Services;

/// <summary>
/// Per-user/per-session state: which events the current user has registered for.
/// In-memory only, scoped to this browser session — not shared across users
/// and not persisted. Distinct from AttendanceTracker, which is per-event.
/// </summary>
public class SessionTracker
{
    private readonly HashSet<int> registeredEventIds = new();

    public bool IsRegistered(int eventId) => registeredEventIds.Contains(eventId);

    public void Register(int eventId) => registeredEventIds.Add(eventId);
}
