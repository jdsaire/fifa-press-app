# 03 — Speaking Three Languages Without a Reload

## The Constraint That Shaped Everything Here

`Microsoft.Extensions.Localization` — the standard .NET package for the usual `.resx`-file approach to multiple languages — isn't part of this project's dependency graph, and adding a new package wasn't something this run was allowed to do. So the standard textbook answer wasn't available, and every decision in this chapter follows from that one constraint.

## The Alternative: Three JSON Files

`wwwroot/i18n/en.json`, `es.json`, `pt.json` each hold every piece of interface text this app shows, keyed by short names like `"nav.record"` or `"signIn.title"`. A single C# class, `LocaleService`, loads all three at startup — using `System.Text.Json`, which is already part of the .NET runtime and needed no new package — and holds them in memory for the rest of the session.

Loading all three up front, rather than fetching whichever one is currently active, is what makes switching languages instant. There's nothing to wait for: the dictionaries are already sitting in memory, so switching is just pointing at a different one.

## Making a Language Switch Actually Repaint the Screen

Blazor re-renders a component when the *value* it's reading changes — not automatically whenever some object's internal state changes somewhere. `LocaleProvider`, sitting once near the top of the page (in `Layout/MainLayout.razor`), holds the active language and hands it down to every component below it via a Blazor feature called a **cascading value** — a way to make one piece of data available to an entire tree of components without threading it through every parameter list by hand.

The important detail: it cascades the language itself (a simple value like `AppLocale.Es`), not the `LocaleService` object that manages it. Cascading the service would hand every component a reference to something whose *contents* can change without the reference ever pointing anywhere new — and Blazor has no way to notice a change like that. Cascading the value means a language switch is a brand new value flowing down the tree, which is exactly the kind of change Blazor's rendering system is built to detect and react to.

## The Bug This Caught Along the Way

Auditing every component for this turned up three places where a translated string had been computed exactly once, at the moment the component was first built, and simply never touched again after that. A row's label, an indicator's default caption — small things, but each one would have kept showing English forever, even after the whole rest of the app switched to Spanish. The fix in every case was the same: stop storing the resolved word, and instead recompute it fresh every time the component renders. It's a small structural habit — computed values instead of stored ones — but it's the difference between a translation and a translation that quietly stops updating.

## Why Dates Don't Come From .NET's Culture System

The obvious way to format a date differently per language in .NET is `CultureInfo` — hand it a culture name and a date, get back the right words for that language. This app deliberately doesn't do that.

Blazor WebAssembly, running in the browser, downloads a chunk of language data once, when the app first starts — and it only downloads *one* chunk, chosen based on whichever language the app opened in. An app that opens in English downloads the chunk covering English, French, Italian, German, and Spanish — and that chunk simply doesn't include Portuguese month names at all. Switch to Portuguese mid-session, and `CultureInfo` would have nothing correct to hand back; it would degrade silently rather than raise any kind of error you'd notice while testing casually.

So this app writes its own month names and date patterns into the same JSON files that hold everything else, and formats dates from those directly. It's a few more lines of code than trusting the framework, but it can't be undermined by which chunk of data happened to load at boot — because it never depends on that chunk at all.
