# Learning Mode

A plain-language walkthrough of how this app was built and why, written for a reader with some general programming background but no prior experience with Blazor, front-end frameworks, or web development specifically.

## What's here

| File | Covers |
|---|---|
| [`01-Building-the-Foundation.md`](01-Building-the-Foundation.md) | What Blazor WebAssembly is, the reusable `EventCard` component, the mock event data, two-way data binding, and routing between the three pages. |
| [`02-Fixing-What-Broke.md`](02-Fixing-What-Broke.md) | Three real problems testing turned up — unvalidated input, a crash on a bad web address, a list that got slow — and exactly what fixed each one. |
| [`03-Adding-Signups-and-Headcounts.md`](03-Adding-Signups-and-Headcounts.md) | The registration form and its validation, and the two separate trackers behind "who signed up for what" versus "how many people are coming." |
| [`Glossary.md`](Glossary.md) | Every term used across the three files above, defined in plain language, with a note on where it appears in the project. |

## How to read this

The three numbered files build on each other in order — 01, then 02, then 03 — each one picking up exactly where the last left off. `Glossary.md` isn't part of that sequence; it's a reference to dip into whenever a word in one of the three files doesn't ring a bell, not something to read start to finish.

Reading all three walkthroughs straight through takes roughly 15–20 minutes.

## Want to see the app itself?

These files describe what the code does — they don't replace actually clicking through it. [`docs/how-to-run.md`](../docs/how-to-run.md) covers every way to get the app open, from a live URL that needs no setup at all to running it yourself.
