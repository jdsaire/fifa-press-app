# Learning Mode — Activity 3: Expansion (Registration, Session, Attendance)

Activity 1 built the foundation, Activity 2 hardened it. Activity 3 adds the
three "advanced features" the guidelines name: a real Registration Form, a
per-user Session Tracker, and a per-event Attendance Tracker — three
independently graded pieces, three commits.

## 1. The Registration Form — [`../src/EventEase/Pages/Registration.razor`](../src/EventEase/Pages/Registration.razor) + [`../src/EventEase/Models/RegistrationModel.cs`](../src/EventEase/Models/RegistrationModel.cs)

Until now, `Registration.razor` just showed which event you were about to
register for — a placeholder. This Activity adds the actual form, built the
idiomatic Blazor way with `EditForm` instead of hand-rolled `<input>`
elements:

```razor
<EditForm Model="registration" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />
    <InputText @bind-Value="registration.Name" />
    <InputText @bind-Value="registration.Email" />
    <button type="submit">Register</button>
</EditForm>
```

`RegistrationModel` carries `[Required]` and `[EmailAddress]` attributes from
`System.ComponentModel.DataAnnotations`. `DataAnnotationsValidator` reads
those attributes at runtime and checks the bound model against them.
Crucially, `OnValidSubmit` — as opposed to a plain `OnSubmit` — **only fires
when validation passes**: an invalid form never reaches `HandleValidSubmit` at
all, it just re-renders with `ValidationSummary` showing what's wrong. That's
"blocks submission until valid" with zero manual if-checks.

## 2. SessionTracker — [`../src/EventEase/Services/SessionTracker.cs`](../src/EventEase/Services/SessionTracker.cs)

A Blazor **service** is just a plain C# class registered with dependency
injection so any component can ask for it instead of constructing its own
copy:

```csharp
// Program.cs
builder.Services.AddSingleton<SessionTracker>();
```

`AddSingleton` means one instance for the entire lifetime of the app — which,
in Blazor WebAssembly, means one instance per browser tab (there's no
multi-user server process to share across, unlike Blazor Server). That's
exactly the "in-memory, per-session" scope the project calls for.

Any page grabs it with `@inject SessionTracker Session` and calls
`Session.IsRegistered(eventId)` or `Session.Register(eventId)`. Because it's
a singleton, the *same* underlying `HashSet<int>` backs every page — register
on the Registration page, and the "Registered" badge on `EventList` and
`EventDetails` picks it up immediately, including after navigating away and
back. That's what "state survives navigation" actually means in a
component-based app: the data lives above the page, in a service, not inside
any one page's fields.

## 3. AttendanceTracker — [`../src/EventEase/Services/AttendanceTracker.cs`](../src/EventEase/Services/AttendanceTracker.cs)

Same DI pattern, deliberately **separate** service and separate storage:

```csharp
private readonly Dictionary<int, List<(string Name, string Email)>> attendeesByEvent = new();
```

`SessionTracker` answers "which events did *I* register for" (keyed by
nothing — it's implicitly "this browser tab's user"). `AttendanceTracker`
answers "who registered for *this event*" (keyed by event ID, and could in
principle be viewed by anyone browsing that event). Same submit button
updates both:

```csharp
private void HandleValidSubmit()
{
    Session.Register(Id);
    Attendance.RegisterAttendee(Id, registration.Name, registration.Email);
}
```

Two calls, two independent stores. It would have been shorter to just shove
everything into one dictionary — but then "which events do I follow" and "who
attended this event" would be tangled together, and a future feature needing
only one of those concerns would drag in the other for no reason. Keeping
them separate is a direct, deliberate design decision, not an oversight.

`EventDetails` reads `Attendance.GetAttendeeCount(selectedEvent.Id)` to show
"N people registered" per event — proof the second service works
independently of the first.

## What changed, end to end

Registration went from a stub to a real, validated form. Two new services
(`SessionTracker`, `AttendanceTracker`) sit above the pages and outlive any
single page visit, which is what turns "a page that shows a form" into "an
app that remembers what you did." Nothing here talks to a database — both
trackers reset the moment the browser tab closes, exactly matching the
mock-data-only constraint that's held since Activity 1.
