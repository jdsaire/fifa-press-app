# Plan: DEPLOY-FifaPressApp-DocLayerReskin-v4_0 — Task 0/1 (Preflight + Overall Plan)

## Context

`jdsaire/fifa-press-app` presented itself as "EventEase," a generic Coursera Course-4 Blazor
capstone. This run re-skins the **documentation layer only** (`README.md`, `docs/`,
`learning-mode/`, `handoff/`) to present the same, untouched app as the **FIFA Press App** — an
original adaptation for journalists covering the 2026 World Cup — per `FIFA_app_P-Research.txt`'s
media-accreditation scenario and `P-Fifa-Repo-Kickoff.txt`'s explicit scope ("not a course
capstone, but my original adaptation and evolution"). `src/`, `.github/workflows/`, and `ux-ui/`
were not touched. This is the precondition for a later run that injects the Research mandate
(explicitly out of scope here). Work landed on `deploy/v4-fifa-press-app-reskin`, ending in an open
PR against `main`, left unmerged for the repository owner to review.

*(`src/` was first touched three runs later, in v7, which renamed `src/EventEase/` to
`src/FifaPressApp/`. Every `src/EventEase/...` / `EventEase.csproj` citation below — including the
FROZEN FACT table — is preserved byte-exact as this plan's historical record of what existed at v4
time.)*

## Preflight results (Task 0 + Task 1 research)

- **GitHub access**: confirmed — `gh` authenticated as `jdsaire`, repo `jdsaire/fifa-press-app`
  reachable.
- **Attachments**: both `FIFA_app_P-Research.txt` and `P-Fifa-Repo-Kickoff.txt` read in full.
- **HEAD drift check**: fresh clone HEAD = `fc49574e2e06989dab87b177a41e915616bbf134`, 52 commits —
  matches `verified_state` exactly, no drift.
- **EventEase mention-count audit**: re-counted live across all 21 in-scope files — every count
  matched `verified_state` exactly (e.g. `grading-criteria.md` 10, `Plan-C4EventEaseBuild.md` 10,
  `learning-mode/01-...` 8, etc.).
- **Git identity**: this machine has no global `user.name`/`user.email` set. The repo's own commit
  history uses exactly one identity: `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>`.
  Set **locally inside the clone** (not globally) before committing.

## Correction to verified_state: the Pages/repo-rename finding was bigger than assumed

`verified_state` theorized a same-repo rename (`frontend_c4_blazor_eventease` → `fifa-press-app`)
with a stale CI base-href. Live verification showed something different and more significant:

- `jdsaire/fifa-press-app` (this repo) has **`has_pages: false`** — Pages has never been enabled on
  it. `https://jdsaire.github.io/fifa-press-app/` returns **HTTP 404**.
- `jdsaire/frontend_c4_blazor_eventease` is a **separate, still-existing, independent repo**
  (created 2026-07-28, `fork: false`, different HEAD commit) — not a rename, not a redirect. Its
  Pages site is live and working (`status: "built"`, HTTP 200) at
  `https://jdsaire.github.io/frontend_c4_blazor_eventease/`.
- In short: the live site anyone can currently see belongs to a different, sibling repo. This
  repo's own Pages was simply never turned on.

Per hard_rules ("verify the live state... report it accurately... fixing it is out of scope"), and
per the project owner's explicit direction during plan review: this run still corrected every
stale `frontend_c4_blazor_eventease` / old-URL mention to `fifa-press-app` /
`https://jdsaire.github.io/fifa-press-app/` in the four in-scope folders, **with no inline caveat**
in the docs themselves. The non-functional-today state is carried forward as an open item below,
framed precisely as "Pages was never enabled on this repo" rather than the milder base-href-drift
issue the prompt anticipated. `.github/workflows/` and Pages settings were not touched.

## File-by-file treatment table

**FROZEN FACT (never edited, cited byte-exact)** — every `src/EventEase/...` path, the `EventEase`
namespace, class names (`EventCard`, `SessionTracker`, `AttendanceTracker`, `MockEventData`, etc.),
every line-number citation, and every literal on-screen UI string (button/badge/link text such as
"Register", "Registered", "View Details", "Back to events", "Page Not Found", "No event matches
this ID.") — confirmed against the actual `.razor` source before use — wherever they appear across
all files below, including inside rewritten prose.

**REWRITTEN NARRATIVE (identity/scenario reframed)**:
| File | What changed |
|---|---|
| `README.md` | Title, purpose paragraph, live-URL → `https://jdsaire.github.io/fifa-press-app/`, doc index, Course Attribution (per NO CAPSTONE rule). Tech Stack's `AI coding assistant` line kept verbatim. |
| `docs/README.md` | Folder description + table descriptions, incl. renamed flowchart row |
| `docs/project-plan.md` | Intro line, requirements/objectives prose reframed to FIFA vocabulary, AI Summary section's scenario wording — technical substance and all class/method names stayed exact; flowchart link updated |
| `docs/grading-criteria.md` | §1 repo link only — every criterion description, path, and line number untouched |
| `docs/how-to-run.md` | Title, live URL, walkthrough-step scenario wording — with literal button/badge text ("Register"/"Registered") kept exact; `dotnet run --project src/EventEase` frozen |
| `docs/setup-guide.md` | Downloaded-folder name → `fifa-press-app-main`; `src/EventEase` path frozen |
| `docs/EventEase-Flowchart.md` → renamed | See CLARIFIED HISTORICAL row below |
| `learning-mode/README.md`, `01-03*.md`, `Glossary.md` | Surrounding scenario prose reframed (events→matches, registration→facility-access request); literal UI quotes and all `src/EventEase/...` citations kept exact |
| `handoff/README.md`, `handoff/v1/README.md` | Freely rewritable per hard_rules — reframed within honest historical wording |

**CLARIFIED HISTORICAL (one contextual clause at first narrative mention only; substance/commit
lists/PASS-FAIL/line numbers untouched)**:
- `handoff/v1/Plan-C4EventEaseBuild.md`, `handoff/v1/c4-eventease-build-completion-report.md`,
  `handoff/v2/CC-PLAN-v2.md` — each got exactly one added clause at its first narrative "EventEase"
  mention.
- `handoff/v2/Completion-Report-v2.md`, `handoff/v3/CC-PLAN-v3.md`, `handoff/v3/Completion-Report-v3.md`,
  `handoff/v2/README.md`, `handoff/v3/README.md` — left untouched: their only "EventEase" occurrences
  are frozen path/filename citations (`src/EventEase/`, `EventEase.csproj`), not narrative brand
  mentions, so no clarifying clause was needed (under-editing preferred per the plan's own guardrail).
- The renamed flowchart's title line and footer attribution only (its diagrams and "Confirmed
  decisions" notes contain no EventEase mentions and were untouched either way).

## Flowchart rename

`docs/EventEase-Flowchart.md` → **`docs/Original-Build-Flowchart.md`**. Inbound links updated in
the same commit: `README.md`, `docs/README.md`, `docs/project-plan.md`.

## Commit sequence

1. `docs(readme): reframe root README as the FIFA Press App`
2. `docs(docs): reframe docs/ narrative for the FIFA Press App; correct stale repo references`
   (includes the flowchart rename + its 3 inbound-link fixes)
3. `docs(learning-mode): reframe walkthroughs for the FIFA Press App context`
4. `docs(handoff): reframe version index READMEs; clarify historical build records without
   rewriting them`
5. `docs: archive v4 plan and completion report`

Author/committer on every commit: `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>` (set
locally in the clone). No AI attribution anywhere, no Co-authored-by trailer.

## Link-integrity target

Baseline internal markdown link count across `README.md` + `docs/` + `learning-mode/` +
`handoff/`: **72**. Target after this run: **72/72** — reported again at Task 6 (see Completion
Report for the one approved deviation from this target).

## Archive (Task 7)

`handoff/v4/`, following the `CC-PLAN-vN.md` / `Completion-Report-vN.md` convention v2 and v3
already established:
- `handoff/v4/CC-PLAN-v4.md` — this plan, renamed from its Plan Mode filename
- `handoff/v4/Completion-Report-v4.md` — commit list w/ SHAs, PASS/FAIL vs. success_criteria,
  deviations, open items — explicitly including the corrected (more severe) Pages finding above
- `handoff/v4/README.md` — indexes both, plus a new row added to `handoff/README.md`

Note: `completion-report-shape.md` referenced in the deploy prompt does not exist anywhere in this
repo or its history. The v2/v3 Completion Reports share one consistent de facto shape (`#
Completion Report: v{N} {title}` → `## Outcome` → `## Results` → `## Approved deviations from the
plan` → `## Open items carried forward`) — this run followed that established shape instead.

## Verification (Task 6, before the archive commit)

1. `dotnet build` sanity check — expect identical result to HEAD (no src/ touched).
2. `git diff` of `src/`, `ux-ui/`, `.github/` against pre-run HEAD — expect byte-identical, PASS/FAIL.
3. Recount internal markdown links across the four folders — expect 72/72.
4. `git log` on the branch — confirm sole author/committer `jdsaire`, zero AI attribution in any
   commit message, file, or the branch name.

## Guardrails carried forward

- Ambiguous treatment defaults to CLARIFIED HISTORICAL, never a full rewrite.
- No src/, `.github/workflows/`, or `ux-ui/` edits under any framing.
- No subagents (per this prompt's explicit hard rule) — all work done directly in this session.
- No PAT ever printed/requested; `gh` only.
- `2026_World_Cup_Schedule.csv` not touched or wired in anywhere (rejected shortcut, stays rejected).
