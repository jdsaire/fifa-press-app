# Project Plan

The FIFA Press App: a media-accreditation tool letting journalists browse World Cup matches and request facility access for one. Originally built as the Course 4 capstone project (three graded Activities: Foundation, Debug & Optimize, Expansion), later adapted into this app's own scenario without touching its underlying architecture.

## Requirements & Objectives

**Functional requirements**
- Browse matches with name, date, and location, via a reusable `EventCard` component.
- Navigate seamlessly between a match list, a match's details, and its access-request page.
- Reject invalid or empty match data with an inline message instead of silently rendering a broken state.
- Handle unmatched routes (and valid-but-nonexistent match IDs) gracefully instead of crashing or showing a blank page.
- Render responsively with a larger mock dataset.
- Request facility access for a match through a form with field-level validation (name, email) that blocks submission until valid.
- Track, per user session, which matches the current user has requested access to, reflected in the UI after navigation.
- Track, per match, how many people have requested access — independent of the per-user session state.

**Non-functional requirements**
- Rendering model: Blazor WebAssembly only — no Server or Hybrid constructs.
- Mock/in-memory data only — no database, no external API, no authentication.
- Originally scoped within the Module 1–5 boundary of the Course 4 syllabus (project setup, components/lifecycle, binding/events/routing/state/forms, rendering models, AI-assisted development); the FIFA Press App adaptation keeps that same technical scope.

**Objectives**
- Deliver a working Blazor WASM app that satisfies all three graded Activities (see [`grading-criteria.md`](grading-criteria.md)).
- Build it as three incremental, approved gates — one per Activity — mirroring how the original assignment itself was structured.

## Design Outline

See [`Original-Build-Flowchart.md`](Original-Build-Flowchart.md) for the preliminary flowchart from the original build, drafted before implementation, one diagram per Activity.

At a high level, the app:
1. Boots as a Blazor WebAssembly app in the browser (`Program.cs`), registering `SessionTracker` and `AttendanceTracker` as app-lifetime singleton services.
2. Routes `/` to `EventList`, `/events/{id}` to `EventDetails`, and `/register/{id}` to `Registration`, all sharing the reusable `EventCard` component.
3. Loads mock match data (`MockEventData`) fresh on every page visit — no persistence.
4. Lets a user request facility access through a validated form; a successful submission updates both state services independently and the UI reflects that state on every page afterward.

Activity 1 established the foundation (component, mock data, binding, routing); Activity 2 debugged and optimized it (input validation, route error handling, list rendering); Activity 3 expanded it with the access-request form and the two state-tracking services.

## AI Coding Assistant Summary

Per the assignment's own instructions, an AI coding assistant was used throughout all three Activities:

- **Activity 1 (Foundation):** The AI coding assistant suggested the initial `EventCard` markup/parameter structure, then the `@bind-X` / `XChanged` `EventCallback` pattern used to make its three fields genuinely two-way bindable, and the `@page` route + route-parameter (`{Id:int}`) pattern used to wire `EventList`/`EventDetails`/`Registration` together.
- **Activity 2 (Debug & Optimize):** The AI coding assistant helped identify the three named bugs (unvalidated input, ungraceful routing failures, unoptimized list rendering) and proposed the fixes — validating in `OnParametersSet` plus each input handler, distinguishing a router-level `NotFoundPage` from an app-level "no match matches this ID" check, and applying `@key` to the match list's `@foreach` loop.
- **Activity 3 (Expansion):** The AI coding assistant suggested building the access-request form with `EditForm` + `DataAnnotationsValidator` instead of manual validation, and recommended keeping `SessionTracker` (per-user) and `AttendanceTracker` (per-match) as two independent services rather than one shared store, to keep each concern separately testable.
