# Grading Criteria Breakdown

30 points across six criteria (per `c4-capstone-guidelines.json`). Exactly where each is satisfied:

## 1. GitHub Repository (5 pts)

This repository — [`jdsaire/fifa-press-app`](https://github.com/jdsaire/fifa-press-app), public, on `main`.

## 2. Event Card Component with Fields + Two-Way Data Binding (5 pts)

[`../src/EventEase/Components/EventCard.razor`](../src/EventEase/Components/EventCard.razor):
- Three fields as bindable `[Parameter]`s: `EventName` (lines 51–52), `EventDate` (57–58), `Location` (63–64).
- Two-way binding via the `@bind-X` / `XChanged` convention: `EventNameChanged` (54–55), `EventDateChanged` (60–61), `LocationChanged` (66–67).
- Each field's `<input>` (lines 9, 17, 25) is wired to a handler that updates the local value and invokes the matching `*Changed` callback: `OnNameInput` (113–119), `OnDateInput` (121–133), `OnLocationInput` (135–141).
- Used across three different pages — [`EventList.razor:28`](../src/EventEase/Pages/EventList.razor), [`EventDetails.razor:20`](../src/EventEase/Pages/EventDetails.razor), [`Registration.razor:20`](../src/EventEase/Pages/Registration.razor) — confirming it's genuinely reusable, not duplicated.

## 3. Routing Implementation + Debugging (5 pts)

- Routes: [`EventList.razor:1`](../src/EventEase/Pages/EventList.razor) (`@page "/"`), [`EventDetails.razor:1`](../src/EventEase/Pages/EventDetails.razor) (`@page "/events/{Id:int}"`), [`Registration.razor:1`](../src/EventEase/Pages/Registration.razor) (`@page "/register/{Id:int}"`).
- Navigation links between them: [`EventList.razor:34-35`](../src/EventEase/Pages/EventList.razor), [`EventDetails.razor:26,36`](../src/EventEase/Pages/EventDetails.razor), [`Registration.razor:54`](../src/EventEase/Pages/Registration.razor).
- Debugged: unmatched/malformed routes fall through to a custom [`NotFound.razor`](../src/EventEase/Pages/NotFound.razor), wired via `NotFoundPage="typeof(Pages.NotFound)"` in [`App.razor:4`](../src/EventEase/App.razor). A syntactically valid but nonexistent event ID (which the router can't catch on its own) is handled separately with a graceful message: [`EventDetails.razor:30-33`](../src/EventEase/Pages/EventDetails.razor), [`Registration.razor:49-51`](../src/EventEase/Pages/Registration.razor).

## 4. Performance Optimization: Input Validation + Routing Errors (5 pts)

- Input validation: [`EventCard.razor`](../src/EventEase/Components/EventCard.razor) validates on load and on every keystroke — `OnParametersSet` (97–108), inline error state and display (10–14, 18–22, 26–30), per-field checks in `OnNameInput`/`OnDateInput`/`OnLocationInput` (113–141).
- Routing errors: same as Criterion 3 above — `NotFound.razor` + the nonexistent-ID checks.
- Rendering optimization: `@key="ev.Id"` on the event list's `@foreach` — [`EventList.razor:27`](../src/EventEase/Pages/EventList.razor) — so Blazor diffs by identity instead of position. Mock dataset expanded to 50 events to give this a dataset large enough to matter: [`MockEventData.cs:23-33`](../src/EventEase/Models/MockEventData.cs).

## 5. Advanced Features: Registration Form, Session State, Attendance Tracker (5 pts)

- **Registration Form with validation:** [`Registration.razor:32-46`](../src/EventEase/Pages/Registration.razor) (`EditForm` + `DataAnnotationsValidator` + `ValidationSummary`), field rules in [`RegistrationModel.cs:10-15`](../src/EventEase/Models/RegistrationModel.cs) (`[Required]`, `[EmailAddress]`); submission only reaches `HandleValidSubmit` (76–81) when valid.
- **Session state management:** [`SessionTracker.cs`](../src/EventEase/Services/SessionTracker.cs) — `IsRegistered`/`Register` (lines 14, 16), registered as an app-lifetime singleton in [`Program.cs:16`](../src/EventEase/Program.cs); read back on [`EventList.razor:30`](../src/EventEase/Pages/EventList.razor) and [`EventDetails.razor:21`](../src/EventEase/Pages/EventDetails.razor) to show a "Registered" badge after navigation.
- **Attendance Tracker:** [`AttendanceTracker.cs`](../src/EventEase/Services/AttendanceTracker.cs) — `RegisterAttendee`/`GetAttendeeCount` (lines 12–21, 23–24), a distinct service/store from `SessionTracker` (per the project's own guardrail against merging them), registered in [`Program.cs:17`](../src/EventEase/Program.cs); count displayed on [`EventDetails.razor:25`](../src/EventEase/Pages/EventDetails.razor).

## 6. AI Coding Assistant Summary (5 pts)

[`project-plan.md`](project-plan.md#ai-coding-assistant-summary) — a per-Activity summary of how the AI coding assistant assisted at each step of development.
