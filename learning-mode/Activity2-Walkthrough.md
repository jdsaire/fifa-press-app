# Learning Mode — Activity 2: Debug & Optimize

Activity 1 built the foundation; Activity 2 takes the three named bugs the
guidelines call out — bad binding input, routing errors, and slow rendering —
and fixes each one as its own commit.

## Bug 1: invalid input silently accepted — [`../src/EventEase/Components/EventCard.razor`](../src/EventEase/Components/EventCard.razor)

Before this Activity, `EventCard`'s inputs would happily accept an empty name
or an unparseable date with no feedback — the underlying data would just go
bad quietly. Two changes fix that:

```csharp
protected override void OnParametersSet()
{
    nameError = string.IsNullOrWhiteSpace(EventName) ? "Event name is required." : null;
    dateError = EventDate == default ? "A valid date is required." : null;
    locationError = string.IsNullOrWhiteSpace(Location) ? "Location is required." : null;
}
```

`OnParametersSet()` is a **lifecycle method** — Blazor calls it every time a
component receives new parameter values (both on first load and after every
`EventCallback` round-trip). Re-validating here means the error state always
matches whatever's actually in the field, not just what was typed most
recently.

The three `On*Input` handlers do the same check inline as the user types, and
each error is rendered as a small red `<div>` right under its field — an
inline condition, not a crash or a silently-corrupted value.

## Bug 2: bad routes crash instead of degrading gracefully — [`../src/EventEase/Pages/NotFound.razor`](../src/EventEase/Pages/NotFound.razor)

Blazor's `<Router>` (in [`App.razor`](../src/EventEase/App.razor)) already had a
`NotFoundPage` wired to a template default — this Activity made it
app-specific (its own message plus a link back to the event list) and closed
a gap the router *can't* catch on its own: a URL like `/events/9999` is
syntactically valid (`{Id:int}` matches "9999") but no event has that ID. The
router happily renders `EventDetails`, which previously showed almost nothing.
Now both `EventDetails` and `Registration` check for a null lookup result and
show "No event matches this ID" instead of a near-blank page:

```razor
@if (selectedEvent is not null)
{
    <EventCard ... />
}
else
{
    <p class="text-danger">No event matches this ID. It may have been removed.</p>
}
```

Two different failure modes, two different fixes — a bad *shape* of URL goes
to `NotFound`; a well-formed URL pointing at *nothing* gets its own message.

## Bug 3: slow rendering on a big list — [`../src/EventEase/Pages/EventList.razor`](../src/EventEase/Pages/EventList.razor) + [`../src/EventEase/Models/MockEventData.cs`](../src/EventEase/Models/MockEventData.cs)

The mock dataset grew from 5 to 50 events (Activity 2's own guidelines ask you
to "measure performance improvements for larger event datasets" — 5 items
isn't large enough to show a difference). The actual fix is one attribute:

```razor
@foreach (var ev in events)
{
    <div class="event-list-item mb-4" @key="ev.Id">
```

Without `@key`, Blazor's diffing algorithm matches last render's DOM tree to
this render's DOM tree **by position** — item 3 in the old list is compared to
item 3 in the new list, regardless of whether they're actually the same
event. If anything shifts (an insert, a removal, a reorder), every item after
that point gets needlessly re-diffed. `@key="ev.Id"` tells Blazor to match by
the event's own stable identity instead, so a card that hasn't changed is left
alone entirely — the more items in the list, the more that matters.

## What changed, end to end

Same three pages and one component as Activity 1 — nothing new was added,
existing pieces were hardened. That's the difference between "foundation" and
"debug and optimize": no new features, just the identified bugs fixed one at
a time, each independently testable.
