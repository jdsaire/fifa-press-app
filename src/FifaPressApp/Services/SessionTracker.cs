namespace FifaPressApp.Services;

/// <summary>
/// Per-user/per-session state: which events the current user has registered for.
/// In-memory only, scoped to this browser session — not shared across users
/// and not persisted. Distinct from AttendanceTracker, which is per-event.
/// </summary>
public class SessionTracker
{
    // Keyed by event, storing who registered so a cancellation can remove the
    // matching AttendanceTracker entry rather than just clearing a flag.
    private readonly Dictionary<int, (string Name, string Email)> registrations = new();

    public bool IsRegistered(int eventId) => registrations.ContainsKey(eventId);

    public void Register(int eventId, string name, string email) => registrations[eventId] = (name, email);

    public void Unregister(int eventId) => registrations.Remove(eventId);

    public (string Name, string Email)? GetRegistration(int eventId) =>
        registrations.TryGetValue(eventId, out var registration) ? registration : null;
}
