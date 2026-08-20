# Glossary

Not meant to be read start to finish — dip into it whenever a word in the walkthroughs or the `/src` folder READMEs doesn't ring a bell. Each entry says what the term means here, and where to see it in the project.

## API (web API)

A program that sits at an address, waits for requests, and answers them. A request is a method and a path — `GET /api/accreditations/MP-2026-04817` means "give me that record" — and an answer is a status code plus, usually, some JSON. This project gained one in v15, at `src/backend/`. Covered in [04-backend-and-integration/01, "The First Server This Project Has Ever Had."](04-backend-and-integration/01-The-First-Server-This-Project-Has-Ever-Had.md)

## Append-only

A way of storing something where records can be added but never edited or deleted. A correction isn't a change to the original — it's a new record that points back at the one it replaces, leaving that one exactly where it was. It makes the past readable by default, since the record *is* its own history, at the cost of having to work out the current state by reading through rather than looking it up. `Change.cs` is built this way: every property is get-all-you-like and set-never, and there is no update or delete method anywhere. Covered in [02-access-record-frontend/03, "Records That Only Ever Grow."](02-access-record-frontend/03-Records-That-Only-Ever-Grow.md)

## Blazor

Microsoft's framework for writing a browser app in C# instead of JavaScript, by compiling that C# into a format the browser can run directly. Covered in [01, "What Even Is Blazor WebAssembly?"](01-architecture-foundation/01-Building-the-Foundation.md#what-even-is-blazor-webassembly).

## Bootstrap

A pre-written CSS toolkit — ready-made styling for buttons, cards, layout grids, and so on — that this project uses but didn't write. It lives at `src/frontend/wwwroot/lib/bootstrap/` and is third-party code, not something built for this app specifically.

## Cache

A local copy of data kept so it doesn't have to be fetched again. "Cache-first" means showing that copy immediately and treating the network as a top-up afterwards — as opposed to asking the network first and falling back to the copy, which still makes someone wait. It's why this app's headline appears with no spinner in front of it, and why every read also reports how old it is. Covered in [02-access-record-frontend/04, "Reading the Cache Before the Network."](02-access-record-frontend/04-Reading-the-Cache-Before-the-Network.md)

## Cascading value

A way for a Blazor component to make a piece of data available to every component nested inside it, without passing it down as an explicit parameter to each one individually. `LocaleProvider.razor` cascades the app's active language this way, once, near the top of every page, so any component anywhere below it can read the current language without every parent in between having to pass it along. Covered in [03-addendum-implementation/03, "Speaking Three Languages Without a Reload."](03-addendum-implementation/03-Speaking-Three-Languages-Without-A-Reload.md)

## Component

A self-contained, reusable chunk of a Blazor app that bundles its own markup (what shows up) and its own C# (what it does) into one `.razor` file. `EventCard` is this project's example. Covered in [01, "What Even Is Blazor WebAssembly?"](01-architecture-foundation/01-Building-the-Foundation.md#what-even-is-blazor-webassembly) and ["Meet the Building Block."](01-architecture-foundation/01-Building-the-Foundation.md#meet-the-building-block-the-eventcard-component)

## Component parameter

A piece of information a component receives from whatever is using it, marked with `[Parameter]` in the component's C#. It's how `EventCard` can show a different match every time instead of always showing the same hardcoded one. Covered in [01, "Meet the Building Block."](01-architecture-foundation/01-Building-the-Foundation.md#meet-the-building-block-the-eventcard-component)

## CORS (cross-origin resource sharing)

The rule that stops a page loaded from one address reading a response from another, unless that response explicitly permits it. It matters here because the deployed app and the API live at different addresses, so without a policy naming the app's address every request would reach the server, succeed, and be discarded by the browser before the page could read it. Covered in [04-backend-and-integration/02, "What Middleware Is and Why Order Matters."](04-backend-and-integration/02-What-Middleware-Is-And-Why-Order-Matters.md)

## `.csproj`

The project file that tells the .NET tools what kind of app this is, which framework version it targets, and which external packages it depends on. This app's is `src/frontend/FifaPressApp.csproj`. Unlike a single standalone C# script, a real Blazor app can't build or run without one.

## CSS isolation

A Blazor feature that lets a component have its own private stylesheet — a `.razor.css` file with the same name as its component — whose rules only apply to that one component, instead of leaking out and affecting the rest of the app. `MainLayout.razor.css` and `NavMenu.razor.css` are examples.

## Custom property

A named value defined once in a stylesheet and referred to everywhere else, written with two leading dashes and read back with `var()` — `--color-link: #0071c1`, then `color: var(--color-link)`. The value can be redefined later in a different context, and every rule reading it picks up the new one automatically, without any of those rules changing. It's what makes two themes possible without writing the whole stylesheet twice. Covered in [02-access-record-frontend/02, "Two Themes and a Pile of Hex Codes."](02-access-record-frontend/02-Two-Themes-and-a-Pile-of-Hex-Codes.md)

## Data annotation

A rule attached directly to a class's property, written as a bracketed tag right above it — `[Required]`, `[EmailAddress]` — that Blazor's form tools can read and enforce automatically. `RegistrationModel.cs` uses these for the access-request form's name and email fields. Covered in [03, "The Registration Form, and How It Refuses Bad Input."](01-architecture-foundation/03-Adding-Signups-and-Headcounts.md#the-registration-form-and-how-it-refuses-bad-input)

## Dependency injection

The mechanism that lets a page ask for a shared service ("I need one of these") without building it itself, and get back the one instance everyone else is also sharing. It's how every page that needs `SessionTracker` or `AttendanceTracker` ends up talking to the same tracker rather than a fresh, disconnected copy. Covered in [03, "Two Different Trackers, On Purpose."](01-architecture-foundation/03-Adding-Signups-and-Headcounts.md#two-different-trackers-on-purpose-session-vs-attendance)

What a page asks for can also be an [interface](#interface) rather than a class, in which case `Program.cs` is the one place that decides which class actually gets handed over. Covered in [02-access-record-frontend/01, "Putting the Data Behind a Door."](02-access-record-frontend/01-Putting-the-Data-Behind-a-Door.md)

## `EditForm`

Blazor's built-in component for building a form with validation baked in. Paired with `DataAnnotationsValidator` (which checks the form's data against its data annotations) and `ValidationSummary` (which lists what's wrong), it's what `Registration.razor` uses to build the access-request form. Covered in [03, "The Registration Form."](01-architecture-foundation/03-Adding-Signups-and-Headcounts.md#the-registration-form-and-how-it-refuses-bad-input)

## Event callback

A component's way of reporting "this changed" back out to whatever page is using it, typed as `EventCallback<T>`. Paired with a matching parameter (`EventName` and `EventNameChanged`, for example), it's the other half of two-way data binding. Covered in [01, "Two-Way Data Binding, Explained Before Any Syntax."](01-architecture-foundation/01-Building-the-Foundation.md#two-way-data-binding-explained-before-any-syntax)

## HTTP status code

The three-digit number an API returns alongside its answer, saying what kind of answer it is. `200` worked, `201` created something, `204` worked and there is nothing to send back, `400` you asked wrongly, `401` you are not allowed to ask, `404` there is no such thing, `409` that already exists, `500` the server has a bug. Every route in this project's API and the codes it can return are listed in [`backend/02_API-REFERENCE.md`](../backend/02_API-REFERENCE.md).

## ICU

International Components for Unicode — the data a program needs to correctly format dates, numbers, and text for a given language and region. Blazor WebAssembly downloads one bundle of this data when the app starts, chosen based on whichever language it opened in, and that bundle doesn't necessarily cover every language the app might later switch to. It's why this app writes its own month names and date patterns into its language files instead of relying on .NET's built-in culture formatting. Covered in [03-addendum-implementation/03, "Speaking Three Languages Without a Reload."](03-addendum-implementation/03-Speaking-Three-Languages-Without-A-Reload.md)

## Interface

A list of what something can do, with none of the how — the operations it must provide, and no code that provides any of them. A separate class then implements it, promising to supply all of them for real. Pages in this app ask for `IAccessDataProvider`, never for the class behind it, so replacing that class with one that talks to a real service is a change to a single line in `Program.cs` rather than a change to every page. Covered in [02-access-record-frontend/01, "Putting the Data Behind a Door."](02-access-record-frontend/01-Putting-the-Data-Behind-a-Door.md)

## Interop (JavaScript interop)

The bridge that lets C# code running as WebAssembly call out to JavaScript, and get an answer back — used for the small number of things only the browser's own JavaScript engine can do directly, like reading local storage or checking a system setting. This app's entire interop surface is two small files, `theme.js` and `locale.js`. Covered in [03-addendum-implementation/04, "Why Two Files Got Their Own Language."](03-addendum-implementation/04-Why-Two-Files-Got-Their-Own-Language.md)

## `@key`

A hint you give Blazor inside a loop that produces a list of items, telling it to match each item up by a stable identity (a match's own ID, for example) instead of by its position in the list. It stops Blazor from redrawing items that haven't actually changed just because something else in the list moved. Covered in [02, "The List Got Slow With More Events."](01-architecture-foundation/02-Fixing-What-Broke.md#problem-3-the-list-got-slow-with-more-events)

## Middleware

A small component that a request passes through on its way to the code that answers it, and that the answer passes back out through. They nest rather than queue: the first one registered is the outermost, and everything after it sits inside — which is why a component can only guard or fix what is registered after it. This API has three: error handling, then authentication, then logging. Covered in [04-backend-and-integration/02, "What Middleware Is and Why Order Matters."](04-backend-and-integration/02-What-Middleware-Is-And-Why-Order-Matters.md)

## Mock data

Data that's invented for building and testing an app, standing in for whatever a real system would eventually supply. Every match in this app comes from `MockEventData.cs`, not a database or any outside source. Covered in [01, "Where the Event Data Actually Comes From."](01-architecture-foundation/01-Building-the-Foundation.md#where-the-event-data-actually-comes-from-the-mock-data)

## NuGet

.NET's package manager — the system that downloads and manages external code libraries a project depends on. This app's `.csproj` lists two NuGet packages, both from Microsoft, that provide the Blazor WebAssembly framework itself.

## Pipeline (request pipeline)

The arrangement of middleware a request travels through, in the order they were registered. The order is a set of trade-offs rather than a convention to get right — in this project, putting logging last means it sees the true status code but never sees a request that was rejected earlier. Covered in [04-backend-and-integration/02, "What Middleware Is and Why Order Matters."](04-backend-and-integration/02-What-Middleware-Is-And-Why-Order-Matters.md)

## `.razor` file / Razor syntax

The file format Blazor components are written in: HTML-like markup and a block of C# (marked `@code`) living together in one file. Every component and page in this app — `EventCard.razor`, `EventList.razor`, and so on — is one of these. Covered in [01, "What Even Is Blazor WebAssembly?"](01-architecture-foundation/01-Building-the-Foundation.md#what-even-is-blazor-webassembly)

## Render / re-render

The process of Blazor figuring out what changed and redrawing only that part of the screen. It happens once when a component first appears, and again ("re-render") every time something it depends on changes afterward. Covered in [02, "The List Got Slow With More Events."](01-architecture-foundation/02-Fixing-What-Broke.md#problem-3-the-list-got-slow-with-more-events)

## Route / routing

The system that lets a Blazor app show different "pages" at different web addresses without ever asking a server for a new file — the browser loads one real HTML file, and C# code swaps out which component is currently showing based on the address. Covered in [01, "How the App Moves Between Pages."](01-architecture-foundation/01-Building-the-Foundation.md#how-the-app-moves-between-pages-routing)

## Router

The component, set up once in `App.razor`, that watches the browser's current address and decides which page component to show — including falling back to a dedicated "not found" page when nothing matches. Covered in [01, "How the App Moves Between Pages"](01-architecture-foundation/01-Building-the-Foundation.md#how-the-app-moves-between-pages-routing) and [02, "A Wrong Web Address Used to Crash the Page."](01-architecture-foundation/02-Fixing-What-Broke.md#problem-2-a-wrong-web-address-used-to-crash-the-page)

## SDK

Short for Software Development Kit — the set of tools (compiler, runtime, command-line tools) needed to build and run a .NET/Blazor app. Installing it is the first step in [`docs/how-to-run.md`](../docs/how-to-run.md)'s local-terminal path.

## Service

An ordinary C# class that isn't tied to any single page — created once when the app starts, and shared by whichever pages need it. `SessionTracker` and `AttendanceTracker` are both services. Covered in [03, "Two Different Trackers, On Purpose."](01-architecture-foundation/03-Adding-Signups-and-Headcounts.md#two-different-trackers-on-purpose-session-vs-attendance)

## SignalR

The part of ASP.NET Core that manages a connection which stays open, so the server can send a message to a browser without being asked first. It ships inside the framework, so the server side of it added no package. This project uses one hub and one message, to tell an open record screen that a change was written. Covered in [04-backend-and-integration/03, "A Connection That Stays Open."](04-backend-and-integration/03-A-Connection-That-Stays-Open.md)

## Simulated authentication

A check that has the shape of authentication without any of its substance. This API compares an incoming token against a fixed string that is published in the repository, printed in the documentation, and shipped to the browser — there is no user, no credential store, no signature, no expiry. It exists to show where such a check belongs in a pipeline, and it secures nothing. Named this way everywhere in the project on purpose: an interface implying security it does not have is the one dishonesty this repository has refused. See [`backend/03_MIDDLEWARE-PIPELINE.md`](../backend/03_MIDDLEWARE-PIPELINE.md).

## Singleton

A service registered so that exactly one instance of it exists for the whole browser tab, shared by every page that asks for it, for as long as the tab stays open. `SessionTracker`, `LocaleService`, and `SimulatedSessionProvider` are all registered this way — it's what lets "which language is active" or "who is signed in" survive moving from one page to another, the same way [state](#state) does. Covered in [03, "Two Different Trackers, On Purpose."](01-architecture-foundation/03-Adding-Signups-and-Headcounts.md#two-different-trackers-on-purpose-session-vs-attendance)

## State

Information a service remembers that outlives any single page visit — for as long as the browser tab stays open, but no longer. It's why a "Registered" badge still shows up after navigating away from the access-request page and back. Covered in [03, "Why Keeping Them Separate Was a Deliberate Choice."](01-architecture-foundation/03-Adding-Signups-and-Headcounts.md#why-keeping-them-separate-was-a-deliberate-choice)

## Two-way data binding

A built-in bridge between a value and what's shown on screen, working in both directions: change the value, the screen updates; edit the screen, the value updates. `EventCard`'s three fields all work this way. Covered in [01, "Two-Way Data Binding, Explained Before Any Syntax."](01-architecture-foundation/01-Building-the-Foundation.md#two-way-data-binding-explained-before-any-syntax)

## TypeScript

A language that adds optional type-checking on top of JavaScript — you can say what kind of value something is supposed to be, and a compiler checks that the code is telling the truth before it ever runs. It compiles down to plain JavaScript, so none of the checking survives into what actually ships; the safety is entirely a build-time thing. This app's two interop files are written this way. Covered in [03-addendum-implementation/04, "Why Two Files Got Their Own Language."](03-addendum-implementation/04-Why-Two-Files-Got-Their-Own-Language.md)

## WebAssembly (WASM)

A compact, fast format that browsers can run at near-native speed, alongside JavaScript, inside the same safety sandbox. Blazor compiles C# into this format, which is how this entire app ends up running as real C# inside your browser tab. Covered in [01, "What Even Is Blazor WebAssembly?"](01-architecture-foundation/01-Building-the-Foundation.md#what-even-is-blazor-webassembly)

## WebSocket

A connection that, once opened between a browser and a server, stays open and lets either end send a message whenever it likes — as opposed to an ordinary request, which is a question, an answer, and a hang-up. It is what SignalR prefers to use underneath. Covered in [04-backend-and-integration/03, "A Connection That Stays Open."](04-backend-and-integration/03-A-Connection-That-Stays-Open.md)

## `wwwroot` / static files

The folder holding everything served to the browser exactly as-is, with no C# processing — the starting HTML page, CSS files, images, and third-party code like Bootstrap. "Static" just means these files don't change themselves; they're the same bytes every time they're requested.
