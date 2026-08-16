# FIFA Press App — Blazor WebAssembly App

A client-side media-accreditation app for journalists covering the 2026 World Cup: browse matches, view details, and request facility access — built with Blazor WebAssembly.

## What is this project?

A Blazor WebAssembly application reimagining a press-accreditation workflow for World Cup match coverage: the kind of tool a press office might use to let journalists, cameramen, and producers browse which matches they can request access to. It lets a user browse a mock list of matches, drill into an individual match's details, and request facility access for one through a validated form — with session-scoped state tracking which matches the current user has requested access to, and a separate per-match request count. All data is mock/in-memory for the duration of the browser session; nothing is persisted.

New to Blazor or .NET? [`docs/setup-guide.md`](docs/setup-guide.md) walks through installing the SDK and running this project from scratch — no prior experience assumed.

## How to Use It

The fastest way to see it: **https://jdsaire.github.io/fifa-press-app/** — no installation needed.

To run it yourself instead — GitHub Codespaces or a local terminal — see [`docs/how-to-run.md`](docs/how-to-run.md).

However you get there, you'll see:

| Page | Route | What it does |
|---|---|---|
| **Matches** | `/` | Lists all mock World Cup matches as editable cards (name, date, location), each with **View Details** and **Register** links — "Register" is this app's built-in wording for submitting a facility-access request. |
| **Match Details** | `/events/{id}` | Shows a single match, whether you've already requested access, and how many people have requested access to it. |
| **Register** | `/register/{id}` | A validated form (name, email) — submitting marks you as having requested access to that match and updates the request count. |

A "Registered" badge follows you back to the Matches and Match Details pages after you submit, and stays until you close the tab (state is in-memory only).

## Tech Stack

- **Language:** C#
- **Framework:** Blazor WebAssembly (client-side, `.NET 10`)
- **Editor used for development:** Visual Studio Code
- **AI coding assistant used for development (per assignment requirements):** an AI coding assistant
- **Tests:** xUnit + bUnit, in [`tests/FifaPressApp.Tests/`](tests/README.md) — run with `dotnet test tests/FifaPressApp.Tests`

## Documentation

- [`docs/setup-guide.md`](docs/setup-guide.md) — beginner walkthrough: install .NET, run the app.
- [`docs/how-to-run.md`](docs/how-to-run.md) — every way to see the app running: the live URL, GitHub Codespaces, or a local terminal.
- [`docs/project-plan.md`](docs/project-plan.md) — requirements, objectives, design outline, AI coding assistant summary.
- [`docs/grading-criteria.md`](docs/grading-criteria.md) — how each of the 6 grading criteria is satisfied in the code.
- [`docs/Original-Build-Flowchart.md`](docs/Original-Build-Flowchart.md) — preliminary design flowchart from the original build, drafted before implementation.
- [`learning-mode/`](learning-mode/) — a plain-language walkthrough of how the app works and why, one file per build stage.
- [Glossary of Blazor/front-end terms](learning-mode/Glossary.md) — for readers new to front-end development.
- [`ux-ui/`](ux-ui/README.md) — every UX dossier for this app: research, evaluation, and whatever mandate tracks land here next.
- [Glossary of UX research and accreditation terms](ux-ui/01-design-research/GLOSSARY.md) — for readers new to UX research, football accreditation, or both.

## Course Attribution

This project originally followed the structure and grading rubric of the Coursera **Microsoft Front-End Developer** Professional Certificate's Course 4 capstone as a best-practices foundation — the same Blazor WebAssembly architecture, component patterns, and AI-assisted workflow are still visible throughout `src/`. It has since become Juan Diego Saire's own original adaptation and evolution: the FIFA Press App, reframing that foundation as a media-accreditation tool for journalists covering the 2026 World Cup. Per the original assignment's own instructions, an AI coding assistant was used across all three graded Activities — generating the foundation, debugging and optimizing it, then expanding it with advanced features.
