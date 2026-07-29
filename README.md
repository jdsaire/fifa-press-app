# EventEase — Blazor WebAssembly App

A client-side event management app: browse events, view details, and register — built with Blazor WebAssembly.

## What is this project?

A Blazor WebAssembly application built as the Course 4 capstone for the Coursera Microsoft Front-End Developer Professional Certificate. It lets a user browse a mock list of events, drill into an individual event's details, and register for one through a validated form — with session-scoped state tracking which events the current user has registered for, and a separate per-event attendance count. All data is mock/in-memory for the duration of the browser session; nothing is persisted.

New to Blazor or .NET? [`docs/setup-guide.md`](docs/setup-guide.md) walks through installing the SDK and running this project from scratch — no prior experience assumed.

## How to Use It

The fastest way to see it: **https://jdsaire.github.io/frontend_c4_blazor_eventease/** — no installation needed.

To run it yourself instead — GitHub Codespaces or a local terminal — see [`docs/how-to-run.md`](docs/how-to-run.md).

However you get there, you'll see:

| Page | Route | What it does |
|---|---|---|
| **Events** | `/` | Lists all mock events as editable cards (name, date, location), each with **View Details** and **Register** links. |
| **Event Details** | `/events/{id}` | Shows a single event, whether you're already registered, and how many people have registered for it. |
| **Register** | `/register/{id}` | A validated form (name, email) — submitting marks you registered for that event and updates the attendee count. |

A "Registered" badge follows you back to the Events and Event Details pages after you sign up, and stays until you close the tab (state is in-memory only).

## Tech Stack

- **Language:** C#
- **Framework:** Blazor WebAssembly (client-side, `.NET 10`)
- **Editor used for development:** Visual Studio Code
- **AI coding assistant used for development (per assignment requirements):** an AI coding assistant

## Documentation

- [`docs/setup-guide.md`](docs/setup-guide.md) — beginner walkthrough: install .NET, run the app.
- [`docs/how-to-run.md`](docs/how-to-run.md) — every way to see the app running: the live URL, GitHub Codespaces, or a local terminal.
- [`docs/project-plan.md`](docs/project-plan.md) — requirements, objectives, design outline, AI coding assistant summary.
- [`docs/grading-criteria.md`](docs/grading-criteria.md) — how each of the 6 grading criteria is satisfied in the code.
- [`docs/EventEase-Flowchart.md`](docs/EventEase-Flowchart.md) — preliminary design flowchart, drafted before implementation.
- [`learning-mode/`](learning-mode/) — a plain-language walkthrough of how the app works and why, one file per build stage.
- [Glossary of Blazor/front-end terms](learning-mode/Glossary.md) — for readers new to front-end development.

## Course Attribution

Built as the Course 4 capstone project for the Coursera **Microsoft Front-End Developer** Professional Certificate. Per the assignment's own instructions, an AI coding assistant was used across all three graded Activities — generating the foundation, debugging and optimizing it, then expanding it with advanced features.
