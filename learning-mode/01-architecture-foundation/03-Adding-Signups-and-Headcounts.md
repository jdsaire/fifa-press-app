# 03 — Adding Signups and Headcounts

## Picking Up Where File 02 Left Off

By the end of file 02, the app did everything it originally set out to do, and did it reliably: browse matches, view one in detail, move between pages, all holding up against bad input and bad web addresses alike. What it still couldn't do was let anyone actually request facility access for a match, or remember that they had. This file adds both.

## The Registration Form, and How It Refuses Bad Input

[`Registration.razor`](../../src/frontend/Pages/Registration.razor) now has a real form asking for a name and an email address. The rules for what counts as valid live in a separate, plain class, [`RegistrationModel.cs`](../../src/frontend/Models/RegistrationModel.cs), attached directly to each property:

```csharp
[Required(ErrorMessage = "Name is required.")]
public string Name { get; set; } = string.Empty;

[Required(ErrorMessage = "Email is required.")]
[EmailAddress(ErrorMessage = "Enter a valid email address.")]
public string Email { get; set; } = string.Empty;
```

Those bracketed lines are **data annotations** ([Glossary.md](../Glossary.md#data-annotation)) — a way of attaching a rule straight onto a property, stated plainly ("this is required," "this must look like an email address"), instead of writing a separate `if` statement to check it by hand every time.

The form itself is built with `EditForm`, Blazor's built-in form component ([Glossary.md](../Glossary.md#editform)), paired with two helpers: `DataAnnotationsValidator`, which reads the rules straight off `RegistrationModel` and checks the form against them, and `ValidationSummary`, which lists out whatever's currently wrong. The important detail is which event handles the submit button: `OnValidSubmit`, not a plain submit handler. That single word means the code behind it only ever runs once every rule has already passed — an invalid form re-displays its error list and goes nowhere near the actual access-request logic.

## Two Different Trackers, On Purpose (Session vs. Attendance)

Requesting facility access for a match raises two different questions, and this app answers them with two separate, independent pieces:

**[`SessionTracker.cs`](../../src/frontend/Services/SessionTracker.cs)** answers *"which matches has this visitor requested access to?"* — a per-person question.

**[`AttendanceTracker.cs`](../../src/frontend/Services/AttendanceTracker.cs)** answers *"how many people, and who, have requested access to this specific match?"* — a per-match question, and one that would make sense to show to anyone looking at that match, not just the person who just submitted a request.

Both are what Blazor calls **services** ([Glossary.md](../Glossary.md#service)): ordinary C# classes that aren't tied to any single page, created once, and shared by whichever pages need them. [`Program.cs`](../../src/frontend/Program.cs) sets both up when the app starts:

```csharp
builder.Services.AddSingleton<SessionTracker>();
builder.Services.AddSingleton<AttendanceTracker>();
```

A page that wants one doesn't build it itself — it just declares "I need one of these" (`@inject SessionTracker Session`) and Blazor hands it the one shared instance automatically. That handoff mechanism has a name, **dependency injection** ([Glossary.md](../Glossary.md#dependency-injection)): instead of a page constructing its own dependencies, something else provides them from the outside, which is exactly what makes "shared" possible — every page asking for `SessionTracker` gets the *same* one, not a fresh copy each time.

## Why Keeping Them Separate Was a Deliberate Choice

It would have been shorter to shove both questions into one shared pile of data. It would also have been a mistake. "Which matches did I request access to" and "how many people requested access to this match" are genuinely different questions with different answers for different audiences, and tangling them together would make each one harder to get right — and harder to change later without accidentally breaking the other.

So a single successful access request does two separate, independent things, both triggered from the same submit handler in `Registration.razor`:

```csharp
private void HandleValidSubmit()
{
    Session.Register(Id);
    Attendance.RegisterAttendee(Id, registration.Name, registration.Email);
}
```

Because both trackers are shared services rather than page-local values, what they remember counts as **state** ([Glossary.md](../Glossary.md#state)) — information that outlives any single page visit, for as long as the browser tab stays open. That's why a "Registered" badge shows up on both [`EventList.razor`](../../src/frontend/Pages/EventList.razor) and [`EventDetails.razor`](../../src/frontend/Pages/EventDetails.razor) after you request access, even after navigating away and back — both pages ask `Session.IsRegistered(...)` fresh, every time. The same page also shows `Attendance.GetAttendeeCount(...)` — a number that has nothing to do with which matches *you* requested access to, because it isn't that tracker's job to know.

## See It for Yourself — and Where to Look Up a Word

Reading about a UI only goes so far. [`docs/how-to-run.md`](../../docs/how-to-run.md) covers the actual ways to get this app open and click through it yourself — browse the list, open a detail page, register (request access), try submitting an empty form, try visiting a web address that doesn't exist.

If any term across these three files didn't fully land, [`Glossary.md`](../Glossary.md) is there to revisit at any point — it isn't meant to be read start to finish, just dipped into whenever a word doesn't ring a bell.
