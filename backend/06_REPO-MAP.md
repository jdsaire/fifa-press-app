# Where everything is, and where it used to be

## Why anything moved

Until this run the repository held one application, and its folder was named
after the product: `src/FifaPressApp/`. That was correct when there was one
thing in `src/` — the v7 run renamed it there deliberately, and that decision
was right.

A second application changes the arithmetic. With a frontend and a backend both
under `src/`, a folder named after the product no longer distinguishes anything
from anything: both are the FIFA Press App. Someone opening this repository for
the first time would have to guess which of the two folders held the browser
code.

So the folders are now named for what they are.

## The map

| Before (up to v14) | Now (v15) | What it is |
|---|---|---|
| `src/FifaPressApp/` | **`src/frontend/`** | The Blazor WebAssembly app. Moved with `git mv`, so history follows it. |
| — | **`src/backend/`** | The ASP.NET Core Web API. New. |
| `src/interop/` | `src/interop/` | The TypeScript interop toolchain. Unmoved; only its output path changed. |
| `tests/FifaPressApp.Tests/` | **`tests/frontend/`** | The bUnit and xUnit suite for the app. Moved. |
| — | **`tests/backend/`** | Tests for the API. New. |
| — | **`backend/`** | This folder. Documentation, not code. |

### The two that get confused

**`backend/` is documentation. `src/backend/` is code.** Nothing in `backend/`
compiles or runs; it is prose, in the same spirit as `ux-ui/`, which documents
the design of the frontend without containing any of it.

### What did not move

**Names.** Not one namespace, assembly name, or project filename changed. The
project file is still `FifaPressApp.csproj` — it just lives in
`src/frontend/FifaPressApp.csproj` now. The root namespace is still
`FifaPressApp`. Every `@using` is untouched.

That is what makes this a path change rather than a refactor, and it is why the
move was safe to do in a single commit with the whole test suite passing inside
it. A directory rename that also renamed identities would have been a different
and much riskier operation, and the v7 run already did that once, on its own.

`git log --follow` traces files across both renames.

## What was updated, and what was deliberately left alone

Roughly 185 citations of the old path exist across the repository. Most were not
touched, and that needs explaining, because a stale path in a document normally
looks like an oversight.

### Updated — anything a stale path would break

The rule was: update a path where a wrong one breaks something that *executes*
or that a reader would *follow*.

- `.github/workflows/deploy-pages.yml` — the publish path, and the comment
  naming the tracked `index.html`.
- `tests/frontend/FifaPressApp.Tests.csproj` — the project reference and the
  schedule CSV it copies.
- `src/interop/tsconfig.json` — where compiled JavaScript is written.
- Eight test files that resolve the app's source folder at runtime, including
  one that asserts the interop output path as a literal string. That assertion
  was updated to the new value rather than weakened or deleted: it was checking
  something real, and it still checks the same real thing.
- `docs/how-to-run.md`, `docs/setup-guide.md`, `docs/grading-criteria.md` — the
  commands a reader would type, and the source links.
- The root `README.md`, `tests/README.md`, `src/interop/README.md`, and the
  moved projects' own READMEs.
- `learning-mode/` — thirty-seven links into the app's source. These are
  narrative documents rather than instructions, but they are actively
  maintained, this run adds a chapter to the same series, and leaving them would
  have broken every one of those links.

### Left alone — the historical record

**`ux-ui/` and `handoff/` still say `src/FifaPressApp/`, and that is correct.**

These are frozen documents. The `ux-ui/` dossiers record design mandates as they
were completed; the `handoff/` folders record what each run actually did, at the
time it did it. `handoff/v7/Completion-Report-v7.md` says the app was renamed to
`src/FifaPressApp/` because that is what happened in v7. Editing it so that it
described v15's layout instead would make it wrong about its own run.

A record that gets quietly rewritten to agree with later decisions stops being a
record. So they were not swept, and this document is the translation table
instead: **wherever a frozen dossier or a historical handoff report says
`src/FifaPressApp/`, read `src/frontend/`.**

`ux-ui/03-ui-prototyping/07_BUILD-BRIEF.md` alone carries thirty-three such
citations and is byte-identical to how it was written.

## One document that says something else worth knowing

That same build brief lists "add a backend, API, or database" under its
anti-scope-creep section. It was governing a frontend-only run, and it was right
about that run. It has not been edited. The decision that supersedes it for
*this* run is recorded in
[`01_HOSTING-DECISION.md`](01_HOSTING-DECISION.md), which explains why it lives
there rather than inside the frozen file.

## And the standing note

Wherever you land in the new layout: the API's token check is a **simulated**
string comparison, not authentication. `src/backend/` contains no credential
store, no signing key, and nothing that secures anything. See
[`03_MIDDLEWARE-PIPELINE.md`](03_MIDDLEWARE-PIPELINE.md).
