# Learning Mode — Activity 1: Foundation (Event Card + Routing)

## The big picture

Blazor WebAssembly compiles your C# into a `.wasm` binary that runs **inside the
browser**, no server round-trip needed for UI logic. When you open the page, the
browser downloads a small .NET runtime plus your compiled app, boots it, and from
then on your C# code *is* the client-side app — same mental model as a React/Vue
SPA, but you write C# and Razor markup instead of JS/JSX.

Four pieces exist after this Activity: one reusable **component**, one **data
model** plus mock dataset, and three **routed pages** that all lean on that one
component.

## 1. The `EventCard` component — [`../src/EventEase/Components/EventCard.razor`](../src/EventEase/Components/EventCard.razor)

This is the reusable building block. In Blazor, a component is just a `.razor`
file: HTML-like markup on top, C# in an `@code` block underneath, compiled into
a real C# class behind the scenes.

```razor
[Parameter]
public string EventName { get; set; } = string.Empty;

[Parameter]
public EventCallback<string> EventNameChanged { get; set; }
```

- `[Parameter]` marks a property that a **parent** page can pass a value into —
  like a prop in React.
- `EventNameChanged` is the other half of **two-way binding**. Blazor has a
  naming convention: if a component exposes both `X` and `XChanged`, a parent
  can write `@bind-X="someField"` and get binding in *both* directions — parent
  → child (initial value flows in) and child → parent (edits flow back out).

Inside the card, each `<input>` is wired to a handler (`OnNameInput`,
`OnDateInput`, `OnLocationInput`) that updates the local property *and* calls
`EventNameChanged.InvokeAsync(...)`. That `InvokeAsync` call is what pushes the
edited value back up to whatever page is using the card — so editing a card on
the Event List page changes the underlying event object that page holds, not
just a local copy.

*(Validation — rejecting blank/garbage input — is deliberately not here yet.
That's Activity 2's job.)*

## 2. The mock data — [`../src/EventEase/Models/EventModel.cs`](../src/EventEase/Models/EventModel.cs) + [`../src/EventEase/Models/MockEventData.cs`](../src/EventEase/Models/MockEventData.cs)

`EventModel` is a plain C# class — `Id`, `Name`, `Date`, `Location` — the shape
of one event. `MockEventData.GetSampleEvents()` returns a hardcoded
`List<EventModel>` of sample events. No database, no API call — this *is* the
"use mock data" instruction from the guidelines.

## 3. The three routed pages

Blazor routing works via the `@page "/route"` directive at the top of a
`.razor` file — that's what makes a component reachable at a URL, no separate
router-config file needed.

- **[`EventList.razor`](../src/EventEase/Pages/EventList.razor)** — `@page "/"`,
  the home route. Loads the mock events, `@foreach`-loops them into
  `<EventCard>`s, with "View Details" and "Register" links under each one.
- **[`EventDetails.razor`](../src/EventEase/Pages/EventDetails.razor)** —
  `@page "/events/{Id:int}"`. The `{Id:int}` is a route parameter: Blazor
  parses the number out of the URL into a matching `[Parameter] public int Id`
  property. `OnParametersSet()` looks that event up and shows it via the
  *same* `EventCard` — proof it's genuinely reusable, not copy-pasted.
- **[`Registration.razor`](../src/EventEase/Pages/Registration.razor)** —
  `@page "/register/{Id:int}"`. Shows which event you're registering for.
  Intentionally a stub beyond that — the real registrant form is a graded
  Activity 3 deliverable, built there on purpose rather than early.

## What you'd see running it

1. Load the app → cards for each event, each with an editable name box, date
   picker, and location box, pre-filled from mock data.
2. Click **View Details** on any card → URL becomes `/events/3` → same single
   card, plus Register and Back-to-events links.
3. Click **Register** → URL becomes `/register/3` → "You're registering for:"
   plus that same card again.
4. **Back to events** returns to `/` and reloads mock data fresh — nothing is
   persisted, by design.

That's the Activity 1 foundation: one reusable, two-way-bound component, wired
into three routes with working navigation between them.
