# Learning Mode

A plain-language walkthrough of how the FIFA Press App was built and why, written for a reader with some general programming background but no prior experience with Blazor, front-end frameworks, or web development specifically.

## What's here

The walkthroughs are grouped into numbered folders, one per stretch of work. Read a folder's chapters in order; the folders themselves also run in order.

### [`01-architecture-foundation/`](01-architecture-foundation/) — building the app

| File | Covers |
|---|---|
| [`01-Building-the-Foundation.md`](01-architecture-foundation/01-Building-the-Foundation.md) | What Blazor WebAssembly is, the reusable `EventCard` component, the mock match data, two-way data binding, and routing between the three pages. |
| [`02-Fixing-What-Broke.md`](01-architecture-foundation/02-Fixing-What-Broke.md) | Three real problems testing turned up — unvalidated input, a crash on a bad web address, a list that got slow — and exactly what fixed each one. |
| [`03-Adding-Signups-and-Headcounts.md`](01-architecture-foundation/03-Adding-Signups-and-Headcounts.md) | The registration form and its validation, and the two separate trackers behind "who requested access to what" versus "how many requests each match has." |

### [`02-access-record-frontend/`](02-access-record-frontend/) — rebuilding it around the access record

| File | Covers |
|---|---|
| [`01-Putting-the-Data-Behind-a-Door.md`](02-access-record-frontend/01-Putting-the-Data-Behind-a-Door.md) | Why the data layer sits behind an interface, what swapping the thing behind it buys, and how one class ended up responsible for not telling the app the future. |
| [`02-Two-Themes-and-a-Pile-of-Hex-Codes.md`](02-access-record-frontend/02-Two-Themes-and-a-Pile-of-Hex-Codes.md) | What a CSS custom property actually is, and why defining two themes together is a different job from inverting one. |
| [`03-Records-That-Only-Ever-Grow.md`](02-access-record-frontend/03-Records-That-Only-Ever-Grow.md) | Why a record that only appends is easier to reason about than one that edits in place, and what it costs. |
| [`04-Reading-the-Cache-Before-the-Network.md`](02-access-record-frontend/04-Reading-the-Cache-Before-the-Network.md) | Looking at stored data before asking the network, and how that changes what a loading state even looks like. |
| [`05-Parsing-a-Real-CSV.md`](02-access-record-frontend/05-Parsing-a-Real-CSV.md) | A real spreadsheet, a date format that breaks across machines, two names in one column, and a clock that says 24:00. |

### Reference

| File | Covers |
|---|---|
| [`Glossary.md`](Glossary.md) | Every term used across the chapters above, defined in plain language, with a note on where it appears in the project. |

## How to read this

Start at `01-architecture-foundation/` and read its three files in order — each picks up where the last left off. Then `02-access-record-frontend/`, whose five chapters are shorter and can be read in any order once you have the first folder behind you.

`Glossary.md` isn't part of that sequence; it's a reference to dip into whenever a word doesn't ring a bell, not something to read start to finish.

Reading everything straight through takes roughly 30–40 minutes.

## Want to see the app itself?

These files describe what the code does — they don't replace actually clicking through it. [`docs/how-to-run.md`](../docs/how-to-run.md) covers every way to get the app open, from a live URL that needs no setup at all to running it yourself.
