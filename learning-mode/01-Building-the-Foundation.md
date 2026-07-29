# 01 — Building the Foundation

## What Even Is Blazor WebAssembly?

You've programmed before — maybe in C#, maybe in something else entirely — and that's really all this needs. This app does something that can feel backwards at first: instead of the page's behavior living in JavaScript, the language browsers have always run natively, it lives in C# — and your browser actually runs that C#, for real, not a translation of it into JavaScript.

This is possible because of a technology called **WebAssembly** (often shortened to "WASM" — see [Glossary.md](Glossary.md#webassembly-wasm)). Browsers have always had one language they could run natively: JavaScript. WebAssembly adds a second one — not a specific language, but a compact, fast format that other languages can be compiled *into*, so the browser can run them at near-native speed inside the same safety sandbox it already uses for JavaScript. **Blazor** ([Glossary.md](Glossary.md#blazor)) is Microsoft's framework for compiling C# into that format. When a browser opens this app, it isn't just downloading HTML and CSS — it's downloading a small version of the .NET runtime itself, plus the compiled C# code, and running both directly on your machine, inside the browser tab.

That's the "WebAssembly" half of "Blazor WebAssembly." The other big idea is how the app's screen gets built in the first place. A plain web page is usually one file with everything in it. Blazor instead breaks the screen into **components** ([Glossary.md](Glossary.md#component)) — self-contained, reusable chunks that each bundle their own markup (what shows up) and their own C# (what it does), living together in a single `.razor` file ([Glossary.md](Glossary.md#razor-file--razor-syntax)). A page gets built by assembling components, the way a plain web page is normally assembled out of `<div>`s — except each one of these can hold its own logic and its own little pocket of memory.

This app has exactly one component built this way so far: `EventCard`, at [`../src/EventEase/Components/EventCard.razor`](../src/EventEase/Components/EventCard.razor). It's worth understanding well, because everything else in this file builds on it.

## Meet the Building Block: the EventCard Component

Open `EventCard.razor` and you'll find two halves in one file: HTML-like markup on top, and a block of C# underneath marked `@code`. That's the whole idea of a `.razor` file — the "what it looks like" and the "how it behaves" live side by side instead of in separate files.

The C# half declares three **component parameters** ([Glossary.md](Glossary.md#component-parameter)) — `EventName`, `EventDate`, `Location` — each marked with `[Parameter]`. A parameter is how a component receives information from whatever page is using it, the same way a method receives its arguments. Without parameters, `EventCard` would only ever be able to show one hardcoded event, which is useless.

Here's the part that makes it genuinely reusable rather than just flexible: this app never writes three separate blocks of markup for three events. It writes `EventCard` once, and three different pages — the events list, the event details page, and the registration page — all hand it a different event and get a correctly filled-in card back. One piece of code, reused everywhere it's needed, instead of near-identical copies that would all need fixing separately the moment something about how a card looks needed to change.

## Where the Event Data Actually Comes From (the Mock Data)

None of the events in this app are real, and none of them come from anywhere outside the app itself. `EventModel`, at [`../src/EventEase/Models/EventModel.cs`](../src/EventEase/Models/EventModel.cs), is a plain C# class with four properties: an ID number, a name, a date, and a location — nothing more than a shape for "one event's worth of data."

`MockEventData`, at [`../src/EventEase/Models/MockEventData.cs`](../src/EventEase/Models/MockEventData.cs), is where that shape gets filled in with made-up sample values — this is what "**mock data**" means ([Glossary.md](Glossary.md#mock-data)): data invented for building and testing, standing in for whatever a real system would eventually supply. There's no database here and no outside service being called. Every time the app starts, it calls one method and gets back the same list of pretend events, held only in the browser's memory for as long as the tab stays open.

## Two-Way Data Binding, Explained Before Any Syntax

Before looking at how this works in code, it's worth understanding the problem it solves.

Normally, connecting "data" to "what's on screen" is a one-way street unless you build the other lane yourself. If a value changes somewhere in your program, the screen doesn't magically update — you have to write code that notices the change and redraws the relevant part. And it works the same in reverse: if a user types into a text box, that keystroke doesn't automatically change anything else in your program unless you write code that reads the box and updates something with it.

**Two-way data binding** ([Glossary.md](Glossary.md#two-way-data-binding)) is Blazor's built-in bridge for both directions at once: change the underlying value, the screen reflects it; edit the screen, the underlying value updates. You still have to set it up — it isn't magic — but Blazor gives you a standard, short way to do it instead of wiring that plumbing by hand every time.

The way `EventCard` sets this up is a naming pattern worth recognizing, because you'll see it everywhere in Blazor code: alongside the `EventName` parameter, there's a second one called `EventNameChanged`, typed as an **event callback** ([Glossary.md](Glossary.md#event-callback)) — a way for a component to report "this changed" back out to whoever is using it. `EventDate`/`EventDateChanged` and `Location`/`LocationChanged` follow the identical pattern. Whenever someone types into one of the card's boxes, the component updates its own copy of the value and immediately calls the matching `...Changed` callback to pass the new value back out.

On the page side, at [`EventList.razor:13`](../src/EventEase/Pages/EventList.razor), you'll see the shorthand this pattern unlocks:

```razor
<EventCard @bind-EventName="ev.Name" @bind-EventDate="ev.Date" @bind-Location="ev.Location" />
```

`@bind-X` is Blazor recognizing the `X` / `XChanged` pair automatically and wiring both directions for you in one line. Type a new name into that card, and the page's own copy of that event's data changes right along with it — no extra code needed on the page's side at all.

## How the App Moves Between Pages (Routing)

A traditional website has a different file for every page, and clicking a link asks the server for a whole new one. This app only ever sends the browser one real HTML file. From then on, moving between "pages" is just Blazor swapping out which component is currently showing, entirely inside the browser, without asking a server for anything new. That's **routing** ([Glossary.md](Glossary.md#route--routing)), and it's why moving around the app feels instant.

Three components in this app are marked as full pages, each with a `@page` directive at the very top of the file naming its web address:

- [`EventList.razor`](../src/EventEase/Pages/EventList.razor) — `@page "/"`, the address you land on first. Loops over every mock event and shows one `EventCard` per event.
- [`EventDetails.razor`](../src/EventEase/Pages/EventDetails.razor) — `@page "/events/{Id:int}"`. The `{Id:int}` part is a **route parameter**: whatever number appears in that spot of the actual address (`/events/3`, `/events/7`) gets captured and handed to the page as an `Id` value it can use to look up the right event.
- [`Registration.razor`](../src/EventEase/Pages/Registration.razor) — `@page "/register/{Id:int}"`, the same pattern, for registering for a specific event.

A component called a **router** ([Glossary.md](Glossary.md#router)) — set up once, in [`App.razor`](../src/EventEase/App.razor) — is what watches the current address and decides which of these three pages to show. Moving between them is done with `NavLink`, a clickable link that updates the address and lets the router take it from there — you'll find one under every event card ("View Details," "Register") and on the details/registration pages themselves ("Back to events").

## What Still Doesn't Work Yet — and Where That Gets Fixed

What exists at this point is a real, working foundation: one reusable component, genuinely two-way bound to made-up data, reachable across three pages that link to each other correctly. But foundations are exactly that — built first, then tested, and testing this one turned up three real problems: certain kinds of bad input could sneak past `EventCard` unnoticed, visiting a web address that didn't exist crashed the page instead of showing a friendly message, and the events list would start to feel sluggish if it had to show a lot more than a handful of events.

File `02-Fixing-What-Broke.md` picks up exactly there.
