# Plan: DEPLOY-fifa-press-app-rename-v7_0 — Task 0/1 (Preflight + Overall Plan)

## Context

This is Run 4-R in the five-run `03-ui-prototyping` sequence (`P-PROTOTYPE_FIFA_Run4-Scope.md` §4).
It renames `src/EventEase/` → `src/FifaPressApp/` and fixes `.github/workflows/deploy-pages.yml`,
which currently hardcodes a *different* repository's name (`frontend_c4_blazor_eventease`). It must
land before Run 4A (the prototyping dossier) so that dossier's `src/` citations are authored against
correct paths from the start, and before Pages activation (Run 4A-D), which depends on the workflow
fix.

The repo was cloned read-only and re-verified against live HEAD with real `grep`/build tooling
rather than trusting the deploy prompt's prose. Everything checked out except a handful of drift
items and one real internal contradiction in the prompt itself, both resolved below.

## Preflight results (Task 0) — all confirmed

- `gh` authenticated as `jdsaire`, repo access confirmed, `has_pages: false`.
- HEAD on `main` = `accab0c0198123f99dea0d5ec07a2a958ac12434` — matches the deploy prompt exactly.
  No drift.
- No open PRs; branch `deploy/v7-namespace-rename` did not yet exist.
- `src/EventEase/` tree, `deploy-pages.yml` content, and the citation-sweep file paths all matched
  the prompt's claims on live inspection.

## Corrections found during verification (applied without a stop, noted here)

1. **Citation-sweep count is wrong in the prompt's prose.** It states "167 occurrences across 43
   files," but only 31 files are actually enumerated in the prompt's own list, and a real grep
   against HEAD confirms **31 files / 104 occurrences** outside `src/` and outside the workflow
   file. The enumerated list itself is complete and accurate — the "43"/"167" figures are a stray
   arithmetic error in the prompt text, not evidence of missing files.
2. **`src/EventEase/Layout/README.md` was missing from the prompt's "files needing in-content
   updates" list** — it also contains the literal string `EventEase` (a sentence describing what
   `NavMenu.razor` renders). Added to the rename task's scope. Total src in-content-update files:
   15, not 14.
3. **`deploy-pages.yml` had two more stale references than the four the prompt calls out**, both
   inside comments: the base-href step's comment naming the tracked `index.html` path, and the
   404-fallback heredoc's second comment about `location.pathname`. Without these two, the file
   would still contain live hits after the "four corrections," failing the prompt's own zero-hits
   grep and its success criterion. Added to the workflow-fix task.
4. **`wwwroot/index.html` line 14** — `<link href="EventEase.styles.css" rel="stylesheet" />`.
   Blazor auto-names the CSS-isolation bundle after the project/assembly name, so renaming the
   `.csproj` makes the real build output `FifaPressApp.styles.css`. This is a required mechanical
   correction for the build to keep working (not a content change) — the prompt's file list missed
   it. Added to the rename task.
5. **`completion-report-shape.md`, referenced by the archive task, does not exist anywhere in the
   repo** (confirmed by full-tree search). Prior runs (v2–v6) never used a template file — they
   wrote reports in the same shape by precedent. This report follows `handoff/v6/Completion-Report-v6.md`'s
   shape instead.

## Resolved with the principal before execution

**UI-visible "EventEase" text is out of scope for this run.** `Layout/NavMenu.razor`'s navbar brand
(`<a class="navbar-brand" href="">EventEase</a>`), `wwwroot/index.html`'s `<title>EventEase</title>`,
and its meta description stay exactly as-is. These are content, not namespace/path references, and
changing them would violate this run's own invariant (no markup/content changes, byte-equivalent
output). Logged as an explicit, intentional exception to "zero occurrences of `EventEase` under
`src/FifaPressApp/`," carried forward as a future run's job (design tokens/theming per the
Run4-Scope doc) — mirroring v4's precedent of re-skinning docs while deliberately leaving `src/`
content untouched. A fourth occurrence follows from the same decision: `Layout/README.md`'s
description of `NavMenu.razor`'s title is accurate to the unchanged component, so it also stays as
written.

## Citation-sweep treatment table

**Direct/living update** (describes current repo state, or contains functional markdown links into
`src/` that must keep resolving):

| File(s) | Why |
|---|---|
| `docs/how-to-run.md`, `docs/setup-guide.md` | Explicitly living per the prompt |
| `docs/grading-criteria.md` | Present-tense index with 10 live markdown links into `src/EventEase/...` |
| `learning-mode/01-Building-the-Foundation.md`, `02-Fixing-What-Broke.md`, `03-Adding-Signups-and-Headcounts.md` | Present-tense technical walkthroughs, 17 combined live links |
| `learning-mode/Glossary.md` | Present-tense definitions naming current file locations |

**Historical clarifying-clause treatment** (records what was built/found/tested at a point in time;
original wording preserved, a short note added instead):

| File(s) | Why |
|---|---|
| `handoff/README.md`, `handoff/v1/*` (3), `handoff/v2/*` (3), `handoff/v3/*` (2), `handoff/v4/*` (3), `handoff/v5/*` (2) — 14 files | Hard rule, explicit |
| `docs/Original-Build-Flowchart.md` | Already carries a clarifying clause from a prior run; no `src/` links — no further edit needed |
| `ux-ui/00-initial-evaluation/*` (7 files) | Point-in-time audit report; path citations are backtick code, not markdown links |
| `ux-ui/01-design-research/README.md`, `ux-ui/README.md` | Already use "EventEase-era app" framing from a prior run — no further edit needed |

`Plan-C4EventEaseBuild.md` and `c4-eventease-build-completion-report.md` — filenames unchanged,
in-body references only.

## Link-integrity baseline

Ran the same resolution-pass method v6 used (not a bare regex count): **229 raw internal links, 211
live-resolving, 18 documented non-live false positives** — all pre-existing (13 in
`handoff/v6/CC-PLAN-v6.md` and 1 in its README quoting frozen content verbatim, 2 in
`handoff/v2/CC-PLAN-v2.md`, 2 in `handoff/v5/*`), matching exactly what v6's own Completion Report
already recorded.

## Commit sequence (6 commits before archive, Conventional Commits, author `jdsaire` only)

1. `refactor(rename): rename src/EventEase to src/FifaPressApp`
2. `fix(ci): correct deploy-pages workflow to reference fifa-press-app, not frontend_c4_blazor_eventease`
3. `docs(citations): update docs/ references after v7 rename`
4. `docs(handoff): clarify src/EventEase references after v7 rename`
5. `docs(learning-mode): update src/EventEase references after v7 rename`
6. `docs(ux-ui): clarify src/EventEase references after v7 rename`
7. (archive, this commit) `docs: archive v7 plan and completion report`

Build verified clean after commits 1 and 2 (the only `src/`/CI-touching commits), and once more at
the end. PR opened from `deploy/v7-namespace-rename` against `main` after commit 6, left unmerged.

## Verification plan

- `dotnet build` (and `publish`) succeeds after commit 1, with no new warnings.
- `grep -r "EventEase" src/FifaPressApp/` returns only the 4 documented UI-content lines.
- `grep -E "EventEase|frontend_c4_blazor_eventease" .github/workflows/deploy-pages.yml` returns zero
  hits after commit 2.
- Full link-resolution pass at the end: report as N/N against the 229/211 baseline above.
- `git log` on the branch: author/committer `jdsaire` only, zero AI/agent mentions in messages,
  files, or branch name.
- `gh pr create` against `main`, left unmerged, per push policy.
