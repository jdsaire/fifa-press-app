# 02 — Fixing What Broke

## Picking Up Where File 01 Left Off

File 01 left `EventCard`, the mock data, and the three-page routing all genuinely working — but "working" and "working correctly under every condition" aren't the same thing. Testing that foundation turned up three specific problems. This file walks through each one: what it looked like from a user's point of view, and exactly what fixed it.

## Problem 1: Bad Input Used to Slip Through Silently

Type nothing into a match's name box, or type something into the date box that isn't actually a date, and `EventCard` used to just accept it without comment. A blank match name isn't obviously wrong at a glance — it could sit there unnoticed.

The fix lives in [`EventCard.razor`](../../src/frontend/Components/EventCard.razor), in a method named `OnParametersSet`. Blazor calls this method automatically every time the component receives a fresh value for one of its parameters — both the moment it first appears, and again any time whatever page is using it changes something. `EventCard` uses that moment to check all three fields at once: is the name blank? Is the date actually a valid date? Is the location blank? The same three checks also run immediately as you type, inside the handlers that already existed for two-way binding. Whichever check fails, a small red message appears right under that field — "Event name is required," "A valid date is required," and so on — instead of the bad value just sitting there, unflagged.

## Problem 2: A Wrong Web Address Used to Crash the Page

Type a web address that doesn't correspond to anything real — a typo, an old bookmark, a guess — and the app used to fail ungracefully instead of showing anything useful. This turned out to be two separate problems wearing one disguise.

The first is a plainly wrong address, one the router doesn't recognize the shape of at all. [`App.razor`](../../src/frontend/App.razor) now tells the router about a dedicated fallback page — [`NotFound.razor`](../../src/frontend/Pages/NotFound.razor) — to show automatically whenever nothing else matches, with a plain "Page Not Found" message and a link back to the match list.

The second is sneakier: an address that *looks* completely valid — `/events/9999`, say — but points to a match ID that doesn't exist among the mock data. The router happily shows the Event Details page, because the address shape genuinely matches `/events/{Id:int}`; there's just no match behind it. That case can't be caught by the router itself, so [`EventDetails.razor`](../../src/frontend/Pages/EventDetails.razor) and [`Registration.razor`](../../src/frontend/Pages/Registration.razor) each check, after looking the ID up, whether they actually found a match — and if not, show "No event matches this ID. It may have been removed." instead of a page that looks broken for no visible reason.

## Problem 3: The List Got Slow With More Events

With only five matches, everything felt instant. Testing against a much bigger list — the mock data now has 50 matches, up from five, specifically so this problem would show up — revealed the match list wasn't redrawing itself as efficiently as it should.

Every time something on screen needs to change, Blazor has to work out exactly what's different and redraw only that part — this redrawing is called **rendering**, or **re-rendering** when it happens again after the first time ([Glossary.md](../Glossary.md#render--re-render)). Left to its own defaults, Blazor compares a list's new version to its old version position by position: "is the third card still showing the same thing it showed last time?" The trouble is that if anything shifts — a match added, removed, or reordered — every card after that point *looks* different by position alone, even when the actual match underneath it hasn't changed at all, and Blazor ends up redrawing far more than it needs to.

The fix is one small addition in [`EventList.razor`](../../src/frontend/Pages/EventList.razor): `@key="ev.Id"` on the loop that produces each card ([Glossary.md](../Glossary.md#key)). This tells Blazor to match cards up by the match's own ID instead of by position in the list, so a card whose match genuinely hasn't changed gets left alone, no matter what moved around it.

## What's Solid Now — and What's Coming Next

The foundation from file 01 is now hardened: bad input gets flagged instead of ignored, a wrong address shows a real message instead of breaking, and the list holds up at a much larger size. None of that touched what the app can actually *do*, though — there's still no real way to request facility access for a match, and nothing remembers that you did.

File `03-Adding-Signups-and-Headcounts.md` is where that gets built.
