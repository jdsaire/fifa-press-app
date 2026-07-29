# Project Plan

Built for the **EventEase App** capstone project (Course 4's three graded Activities: Foundation, Debug & Optimize, Expansion).

## Requirements & Objectives

**Functional requirements**
- Browse events with name, date, and location, via a reusable `EventCard` component.
- Navigate seamlessly between an events list, an event's details, and its registration page.
- Reject invalid or empty event data with an inline message instead of silently rendering a broken state.
- Handle unmatched routes (and valid-but-nonexistent event IDs) gracefully instead of crashing or showing a blank page.
- Render responsively with a larger mock dataset.
- Register for an event through a form with field-level validation (name, email) that blocks submission until valid.
- Track, per user session, which events the current user has registered for, reflected in the UI after navigation.
- Track, per event, how many people have registered — independent of the per-user session state.

**Non-functional requirements**
- Rendering model: Blazor WebAssembly only — no Server or Hybrid constructs.
- Mock/in-memory data only — no database, no external API, no authentication.
- Stay within the Module 1–5 scope of the Course 4 syllabus (project setup, components/lifecycle, binding/events/routing/state/forms, rendering models, AI-assisted development).

**Objectives**
- Deliver a working Blazor WASM app that satisfies all three graded Activities (see [`grading-criteria.md`](grading-criteria.md)).
- Build it as three incremental, approved gates — one per Activity — mirroring how the assignment itself is structured.

## Design Outline

See [`EventEase-Flowchart.md`](EventEase-Flowchart.md) for the preliminary flowchart, drafted before implementation, one diagram per Activity.

At a high level, the app:
1. Boots as a Blazor WebAssembly app in the browser (`Program.cs`), registering `SessionTracker` and `AttendanceTracker` as app-lifetime singleton services.
2. Routes `/` to `EventList`, `/events/{id}` to `EventDetails`, and `/register/{id}` to `Registration`, all sharing the reusable `EventCard` component.
3. Loads mock event data (`MockEventData`) fresh on every page visit — no persistence.
4. Lets a user register through a validated form; a successful submission updates both state services independently and the UI reflects that state on every page afterward.

Activity 1 established the foundation (component, mock data, binding, routing); Activity 2 debugged and optimized it (input validation, route error handling, list rendering); Activity 3 expanded it with the Registration Form and the two state-tracking services.

## AI Coding Assistant Summary

Per the assignment's own instructions, an AI coding assistant was used throughout all three Activities:

- **Activity 1 (Foundation):** The AI coding assistant suggested the initial `EventCard` markup/parameter structure, then the `@bind-X` / `XChanged` `EventCallback` pattern used to make its three fields genuinely two-way bindable, and the `@page` route + route-parameter (`{Id:int}`) pattern used to wire `EventList`/`EventDetails`/`Registration` together.
- **Activity 2 (Debug & Optimize):** The AI coding assistant helped identify the three named bugs (unvalidated input, ungraceful routing failures, unoptimized list rendering) and proposed the fixes — validating in `OnParametersSet` plus each input handler, distinguishing a router-level `NotFoundPage` from an app-level "no event matches this ID" check, and applying `@key` to the event list's `@foreach` loop.
- **Activity 3 (Expansion):** The AI coding assistant suggested building the Registration Form with `EditForm` + `DataAnnotationsValidator` instead of manual validation, and recommended keeping `SessionTracker` (per-user) and `AttendanceTracker` (per-event) as two independent services rather than one shared store, to keep each concern separately testable.
