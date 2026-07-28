namespace EventEase.Services;

/// <summary>
/// Per-event state: who has registered for a given event, across all users.
/// In-memory only, scoped to this browser session. Distinct from
/// SessionTracker, which is per-user (which events *this* user registered for).
/// </summary>
public class AttendanceTracker
{
    private readonly Dictionary<int, List<(string Name, string Email)>> attendeesByEvent = new();

    public void RegisterAttendee(int eventId, string name, string email)
    {
        if (!attendeesByEvent.TryGetValue(eventId, out var attendees))
        {
            attendees = new List<(string Name, string Email)>();
            attendeesByEvent[eventId] = attendees;
        }

        attendees.Add((name, email));
    }

    public int GetAttendeeCount(int eventId) =>
        attendeesByEvent.TryGetValue(eventId, out var attendees) ? attendees.Count : 0;
}
