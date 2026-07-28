# Grading Criteria Breakdown

30 points across six criteria (per `c4-capstone-guidelines.json`). Exactly where each is satisfied:

## 1. GitHub Repository (5 pts)

This repository — [`jdsaire/frontend_c4_blazor_eventease`](https://github.com/jdsaire/frontend_c4_blazor_eventease), public, on `main`.

## 2. Event Card Component with Fields + Two-Way Data Binding (5 pts)

[`../src/EventEase/Components/EventCard.razor`](../src/EventEase/Components/EventCard.razor):
- Three fields as bindable `[Parameter]`s: `EventName` (lines 24–25), `EventDate` (30–31), `Location` (36–37).
- Two-way binding via the `@bind-X` / `XChanged` convention: `EventNameChanged` (27–28), `EventDateChanged` (33–34), `LocationChanged` (39–40).
- Each field's `<input>` (lines 3, 9, 15) is wired to a handler that updates the local value and invokes the matching `*Changed` callback: `OnNameInput` (53–59), `OnDateInput` (61–73), `OnLocationInput` (75–81).
- Used across three different pages — [`EventList.razor:13`](../src/EventEase/Pages/EventList.razor), [`EventDetails.razor:13`](../src/EventEase/Pages/EventDetails.razor), [`Registration.razor:14`](../src/EventEase/Pages/Registration.razor) — confirming it's genuinely reusable, not duplicated.

## 3. Routing Implementation + Debugging (5 pts)

- Routes: [`EventList.razor:1`](../src/EventEase/Pages/EventList.razor) (`@page "/"`), [`EventDetails.razor:1`](../src/EventEase/Pages/EventDetails.razor) (`@page "/events/{Id:int}"`), [`Registration.razor:1`](../src/EventEase/Pages/Registration.razor) (`@page "/register/{Id:int}"`).
- Navigation links between them: [`EventList.razor:18-19`](../src/EventEase/Pages/EventList.razor), [`EventDetails.razor:19,26`](../src/EventEase/Pages/EventDetails.razor), [`Registration.razor:44`](../src/EventEase/Pages/Registration.razor).
- Debugged: unmatched/malformed routes fall through to a custom [`NotFound.razor`](../src/EventEase/Pages/NotFound.razor), wired via `NotFoundPage="typeof(Pages.NotFound)"` in [`App.razor:1`](../src/EventEase/App.razor). A syntactically valid but nonexistent event ID (which the router can't catch on its own) is handled separately with a graceful message: [`EventDetails.razor:21-24`](../src/EventEase/Pages/EventDetails.razor), [`Registration.razor:39-42`](../src/EventEase/Pages/Registration.razor).

## 4. Performance Optimization: Input Validation + Routing Errors (5 pts)

- Input validation: [`EventCard.razor`](../src/EventEase/Components/EventCard.razor) validates on load and on every keystroke — `OnParametersSet` (46–51), inline error state and display (4–7, 10–13, 16–19, 42–44), per-field checks in `OnNameInput`/`OnDateInput`/`OnLocationInput` (53–81).
- Routing errors: same as Criterion 3 above — `NotFound.razor` + the nonexistent-ID checks.
- Rendering optimization: `@key="ev.Id"` on the event list's `@foreach` — [`EventList.razor:12`](../src/EventEase/Pages/EventList.razor) — so Blazor diffs by identity instead of position. Mock dataset expanded to 50 events to give this a dataset large enough to matter: [`MockEventData.cs:10-34`](../src/EventEase/Models/MockEventData.cs).

## 5. Advanced Features: Registration Form, Session State, Attendance Tracker (5 pts)

- **Registration Form with validation:** [`Registration.razor:22-36`](../src/EventEase/Pages/Registration.razor) (`EditForm` + `DataAnnotationsValidator` + `ValidationSummary`), field rules in [`RegistrationModel.cs:7-12`](../src/EventEase/Models/RegistrationModel.cs) (`[Required]`, `[EmailAddress]`); submission only reaches `HandleValidSubmit` (58–62) when valid.
- **Session state management:** [`SessionTracker.cs`](../src/EventEase/Services/SessionTracker.cs) — `IsRegistered`/`Register` (lines 12, 14), registered as an app-lifetime singleton in [`Program.cs:11`](../src/EventEase/Program.cs); read back on [`EventList.razor:14`](../src/EventEase/Pages/EventList.razor) and [`EventDetails.razor:14`](../src/EventEase/Pages/EventDetails.razor) to show a "Registered" badge after navigation.
- **Attendance Tracker:** [`AttendanceTracker.cs`](../src/EventEase/Services/AttendanceTracker.cs) — `RegisterAttendee`/`GetAttendeeCount` (lines 12–21, 23–24), a distinct service/store from `SessionTracker` (per the project's own guardrail against merging them), registered in [`Program.cs:12`](../src/EventEase/Program.cs); count displayed on [`EventDetails.razor:18`](../src/EventEase/Pages/EventDetails.razor).

## 6. Copilot Assistance Summary (5 pts)

[`project-plan.md`](project-plan.md#copilot-assistance-summary) — a per-Activity summary of how Copilot assisted at each step of development.
