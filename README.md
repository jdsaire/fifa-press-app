# FIFA Press App — Blazor WebAssembly frontend and ASP.NET Core API

A media-accreditation companion for journalists covering the 2026 World Cup, in English, Spanish and Portuguese: sign in as one of two demo holders and see what your access currently permits, what has changed about it, and why — before you discover it by being refused. A Blazor WebAssembly frontend, and an ASP.NET Core Web API it can read from.

## What is this project?

A portfolio demonstration, not a FIFA product — it is not affiliated with, endorsed by, or connected to FIFA, and the public landing view says so before anything else. It reimagines what a journalist's accreditation record could show: not a login-gated status page, but an append-only log of every change to that record, each one carrying a reason and a next step, with the ones that depend on a result nobody has played yet worded as conditions rather than decisions already taken. Two demo accounts, published on the sign-in screen, open two different holders' records — the difference between them is the point, not an accident of the data.

All data is simulated for the duration of the browser session; nothing is persisted server-side, and the app says so wherever a reader might otherwise mistake it for a live integration. The match schedule is the one real thing in it.

New to Blazor or .NET? [`docs/setup-guide.md`](docs/setup-guide.md) walks through installing the SDK and running this project from scratch — no prior experience assumed.

## How to Use It

The fastest way to see it: **https://jdsaire.github.io/fifa-press-app/** — no installation needed.

To run it yourself instead — GitHub Codespaces or a local terminal — see [`docs/how-to-run.md`](docs/how-to-run.md).

However you get there, you'll see:

| Page | Route | What it does |
|---|---|---|
| **Landing** | `/` | The public front door: what this app is, that it's a demonstration, and two equally weighted ways in — sign in, or browse without an account. |
| **Sign in** | `/signin` | A simulated session — genuinely functional, genuinely not a security boundary — with both demo accounts published on screen, passwords included. |
| **My Access** | `/record` | The signed-in holder's record: what they hold, every change to it (each one a row you open for the reason and next step), and how old the data on screen is. Gated — an access record is personal by definition. |
| **Matches** | `/matches` | Every scheduled fixture, searchable and filterable. Public: no sign-in needed. |
| **Match details** | `/events/{id}` | One fixture, kickoff time, and — signed in — your access to it and a simulated gate check. |
| **Request access** | `/request/{id}` | A validated form; submitting writes a new, animated-in entry to your record rather than showing a separate confirmation screen. Gated, since it writes to a personal record. |
| **Help** | `/help` | What this service doesn't do, what won't reach you as a notification, and who to contact — eight independently collapsible sections, all closed on arrival, entirely static so it still reads with no network at all. Public. |

A fixture that hasn't been played yet never names its teams — on any of these screens, in any of the three languages — because the schedule this app reads is a record of a *completed* tournament, and the app is built not to read ahead. Switch language or theme from the sidebar; both are independent of the other and of the session, so changing either never signs you out.

## Where things live

| Path | What it is |
|---|---|
| [`src/frontend/`](src/frontend/) | **The Blazor WebAssembly app** — every screen you see. This is what the live URL serves. |
| [`src/backend/`](src/backend/) | **The ASP.NET Core Web API** — accreditation records and their change log, over HTTP. |
| [`src/interop/`](src/interop/README.md) | The small TypeScript toolchain that compiles two browser helper files. |
| [`tests/frontend/`](tests/frontend/) | Tests for the app. 512 of them. |
| [`tests/backend/`](tests/backend/) | Tests for the API. 33 of them. |
| [`backend/`](backend/README.md) | **Documentation** for the backend layer. |
| [`ux-ui/`](ux-ui/README.md) | Every UX dossier: research, ideation, prototyping, evaluation, iteration. |
| [`learning-mode/`](learning-mode/) | Plain-language walkthroughs of how it all works, for readers new to this stack. |
| [`docs/`](docs/README.md) | How to run it, how to set it up, how it maps to its original grading criteria. |
| [`handoff/`](handoff/README.md) | One folder per build run: the plan approved beforehand, and the report afterwards. |

**[`backend/`](backend/README.md) is documentation; [`src/backend/`](src/backend/) is the code.** Two folders, similar names, entirely different contents — `backend/` is prose that never compiles, deliberately parallel to `ux-ui/`, which documents the frontend's design without containing any of it. This is the single easiest thing to get wrong here, which is why it has its own line.

**The API is optional and off by default.** The app ships with no API configured and runs on an in-memory mock, exactly as it did before the backend existed — so the live site works whether or not a server is running anywhere. [`backend/07_BEFORE-AND-AFTER.md`](backend/07_BEFORE-AND-AFTER.md) shows how to run both halves together and what changes when you do (very little, on purpose).

**The authentication is simulated**, in both halves. The sign-in publishes its own passwords on screen; the API's token is a fixed string printed in this repository. Neither secures anything, and both say so wherever a reader could mistake them for real.

## Tech Stack

- **Language:** C#, with a small TypeScript interop layer (two files: theme and locale storage) — see [`src/interop/README.md`](src/interop/README.md) for why and how narrowly.
- **Frameworks:** Blazor WebAssembly on the client, ASP.NET Core Web API on the server, with SignalR between them (`.NET 10` throughout)
- **Editor used for development:** Visual Studio Code
- **AI coding assistant used for development (per assignment requirements):** an AI coding assistant
- **Tests:** xUnit + bUnit, in [`tests/`](tests/README.md) — `dotnet test tests/frontend` (512) and `dotnet test tests/backend` (33)

## Documentation

- [`docs/setup-guide.md`](docs/setup-guide.md) — beginner walkthrough: install .NET, run the app.
- [`docs/how-to-run.md`](docs/how-to-run.md) — every way to see the app running: the live URL, GitHub Codespaces, or a local terminal.
- [`docs/project-plan.md`](docs/project-plan.md) — requirements, objectives, design outline, AI coding assistant summary.
- [`docs/grading-criteria.md`](docs/grading-criteria.md) — how each of the 6 grading criteria is satisfied in the code.
- [`docs/Original-Build-Flowchart.md`](docs/Original-Build-Flowchart.md) — preliminary design flowchart from the original build, drafted before implementation.
- [`learning-mode/`](learning-mode/) — a plain-language walkthrough of how the app works and why, one file per build stage.
- [Glossary of Blazor/front-end terms](learning-mode/Glossary.md) — for readers new to front-end development.
- [`backend/`](backend/README.md) — the backend layer in plain language: the API reference, the middleware pipeline, the real-time connection, and how to run and deploy both halves.
- [`backend/07_BEFORE-AND-AFTER.md`](backend/07_BEFORE-AND-AFTER.md) — what the backend changed and what it deliberately did not, with a comparison table and steps to check each claim yourself.
- [`ux-ui/`](ux-ui/README.md) — every UX dossier for this app: research, evaluation, and whatever mandate tracks land here next.
- [Glossary of UX research and accreditation terms](ux-ui/01-design-research/GLOSSARY.md) — for readers new to UX research, football accreditation, or both.

## Course Attribution

This project originally followed the structure and grading rubric of the Coursera **Microsoft Front-End Developer** Professional Certificate's Course 4 capstone as a best-practices foundation — the same Blazor WebAssembly architecture, component patterns, and AI-assisted workflow are still visible throughout `src/`. It has since become Juan Diego Saire's own original adaptation and evolution: the FIFA Press App, reframing that foundation as a media-accreditation tool for journalists covering the 2026 World Cup. Per the original assignment's own instructions, an AI coding assistant was used across all three graded Activities — generating the foundation, debugging and optimizing it, then expanding it with advanced features.
