# Glossary

Not meant to be read start to finish — dip into it whenever a word in the walkthroughs or the `/src` folder READMEs doesn't ring a bell. Each entry says what the term means here, and where to see it in the project.

## Blazor

Microsoft's framework for writing a browser app in C# instead of JavaScript, by compiling that C# into a format the browser can run directly. Covered in [01, "What Even Is Blazor WebAssembly?"](01-Building-the-Foundation.md#what-even-is-blazor-webassembly).

## Bootstrap

A pre-written CSS toolkit — ready-made styling for buttons, cards, layout grids, and so on — that this project uses but didn't write. It lives at `src/EventEase/wwwroot/lib/bootstrap/` and is third-party code, not something built for this app specifically.

## Component

A self-contained, reusable chunk of a Blazor app that bundles its own markup (what shows up) and its own C# (what it does) into one `.razor` file. `EventCard` is this project's example. Covered in [01, "What Even Is Blazor WebAssembly?"](01-Building-the-Foundation.md#what-even-is-blazor-webassembly) and ["Meet the Building Block."](01-Building-the-Foundation.md#meet-the-building-block-the-eventcard-component)

## Component parameter

A piece of information a component receives from whatever is using it, marked with `[Parameter]` in the component's C#. It's how `EventCard` can show a different match every time instead of always showing the same hardcoded one. Covered in [01, "Meet the Building Block."](01-Building-the-Foundation.md#meet-the-building-block-the-eventcard-component)

## `.csproj`

The project file that tells the .NET tools what kind of app this is, which framework version it targets, and which external packages it depends on. This app's is `src/EventEase/EventEase.csproj`. Unlike a single standalone C# script, a real Blazor app can't build or run without one.

## CSS isolation

A Blazor feature that lets a component have its own private stylesheet — a `.razor.css` file with the same name as its component — whose rules only apply to that one component, instead of leaking out and affecting the rest of the app. `MainLayout.razor.css` and `NavMenu.razor.css` are examples.

## Data annotation

A rule attached directly to a class's property, written as a bracketed tag right above it — `[Required]`, `[EmailAddress]` — that Blazor's form tools can read and enforce automatically. `RegistrationModel.cs` uses these for the access-request form's name and email fields. Covered in [03, "The Registration Form, and How It Refuses Bad Input."](03-Adding-Signups-and-Headcounts.md#the-registration-form-and-how-it-refuses-bad-input)

## Dependency injection

The mechanism that lets a page ask for a shared service ("I need one of these") without building it itself, and get back the one instance everyone else is also sharing. It's how every page that needs `SessionTracker` or `AttendanceTracker` ends up talking to the same tracker rather than a fresh, disconnected copy. Covered in [03, "Two Different Trackers, On Purpose."](03-Adding-Signups-and-Headcounts.md#two-different-trackers-on-purpose-session-vs-attendance)

## `EditForm`

Blazor's built-in component for building a form with validation baked in. Paired with `DataAnnotationsValidator` (which checks the form's data against its data annotations) and `ValidationSummary` (which lists what's wrong), it's what `Registration.razor` uses to build the access-request form. Covered in [03, "The Registration Form."](03-Adding-Signups-and-Headcounts.md#the-registration-form-and-how-it-refuses-bad-input)

## Event callback

A component's way of reporting "this changed" back out to whatever page is using it, typed as `EventCallback<T>`. Paired with a matching parameter (`EventName` and `EventNameChanged`, for example), it's the other half of two-way data binding. Covered in [01, "Two-Way Data Binding, Explained Before Any Syntax."](01-Building-the-Foundation.md#two-way-data-binding-explained-before-any-syntax)

## `@key`

A hint you give Blazor inside a loop that produces a list of items, telling it to match each item up by a stable identity (a match's own ID, for example) instead of by its position in the list. It stops Blazor from redrawing items that haven't actually changed just because something else in the list moved. Covered in [02, "The List Got Slow With More Events."](02-Fixing-What-Broke.md#problem-3-the-list-got-slow-with-more-events)

## Mock data

Data that's invented for building and testing an app, standing in for whatever a real system would eventually supply. Every match in this app comes from `MockEventData.cs`, not a database or any outside source. Covered in [01, "Where the Event Data Actually Comes From."](01-Building-the-Foundation.md#where-the-event-data-actually-comes-from-the-mock-data)

## NuGet

.NET's package manager — the system that downloads and manages external code libraries a project depends on. This app's `.csproj` lists two NuGet packages, both from Microsoft, that provide the Blazor WebAssembly framework itself.

## `.razor` file / Razor syntax

The file format Blazor components are written in: HTML-like markup and a block of C# (marked `@code`) living together in one file. Every component and page in this app — `EventCard.razor`, `EventList.razor`, and so on — is one of these. Covered in [01, "What Even Is Blazor WebAssembly?"](01-Building-the-Foundation.md#what-even-is-blazor-webassembly)

## Render / re-render

The process of Blazor figuring out what changed and redrawing only that part of the screen. It happens once when a component first appears, and again ("re-render") every time something it depends on changes afterward. Covered in [02, "The List Got Slow With More Events."](02-Fixing-What-Broke.md#problem-3-the-list-got-slow-with-more-events)

## Route / routing

The system that lets a Blazor app show different "pages" at different web addresses without ever asking a server for a new file — the browser loads one real HTML file, and C# code swaps out which component is currently showing based on the address. Covered in [01, "How the App Moves Between Pages."](01-Building-the-Foundation.md#how-the-app-moves-between-pages-routing)

## Router

The component, set up once in `App.razor`, that watches the browser's current address and decides which page component to show — including falling back to a dedicated "not found" page when nothing matches. Covered in [01, "How the App Moves Between Pages"](01-Building-the-Foundation.md#how-the-app-moves-between-pages-routing) and [02, "A Wrong Web Address Used to Crash the Page."](02-Fixing-What-Broke.md#problem-2-a-wrong-web-address-used-to-crash-the-page)

## SDK

Short for Software Development Kit — the set of tools (compiler, runtime, command-line tools) needed to build and run a .NET/Blazor app. Installing it is the first step in [`docs/how-to-run.md`](../docs/how-to-run.md)'s local-terminal path.

## Service

An ordinary C# class that isn't tied to any single page — created once when the app starts, and shared by whichever pages need it. `SessionTracker` and `AttendanceTracker` are both services. Covered in [03, "Two Different Trackers, On Purpose."](03-Adding-Signups-and-Headcounts.md#two-different-trackers-on-purpose-session-vs-attendance)

## State

Information a service remembers that outlives any single page visit — for as long as the browser tab stays open, but no longer. It's why a "Registered" badge still shows up after navigating away from the access-request page and back. Covered in [03, "Why Keeping Them Separate Was a Deliberate Choice."](03-Adding-Signups-and-Headcounts.md#why-keeping-them-separate-was-a-deliberate-choice)

## Two-way data binding

A built-in bridge between a value and what's shown on screen, working in both directions: change the value, the screen updates; edit the screen, the value updates. `EventCard`'s three fields all work this way. Covered in [01, "Two-Way Data Binding, Explained Before Any Syntax."](01-Building-the-Foundation.md#two-way-data-binding-explained-before-any-syntax)

## WebAssembly (WASM)

A compact, fast format that browsers can run at near-native speed, alongside JavaScript, inside the same safety sandbox. Blazor compiles C# into this format, which is how this entire app ends up running as real C# inside your browser tab. Covered in [01, "What Even Is Blazor WebAssembly?"](01-Building-the-Foundation.md#what-even-is-blazor-webassembly)

## `wwwroot` / static files

The folder holding everything served to the browser exactly as-is, with no C# processing — the starting HTML page, CSS files, images, and third-party code like Bootstrap. "Static" just means these files don't change themselves; they're the same bytes every time they're requested.
