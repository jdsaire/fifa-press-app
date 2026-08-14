# Plan — DEPLOY v8: Dossier Injection + Pages Activation into jdsaire/fifa-press-app

## Context

Nine dossier files (Gates 0–8 of the "Prototyping" mandate) were authored and approved in a
separate chat session, turning the ideation mandate's Access Record concept into a buildable UX
specification: scope, benchmarking, IA, UI decisions, tasks/scenarios, screens, data model, build
brief, and limitations. This run's job is placement only — land the nine files verbatim into a new
`ux-ui/03-ui-prototyping/` folder (following the exact precedent set by v5's and v6's dossier
injections), then separately activate GitHub Pages, which v7's namespace rename made safe to do.
No dossier content is rewritten, and nothing under `src/FifaPressApp/` is touched — this run only
lands the specification and turns on the inspection surface; Run 4B builds the frontend from it.

**Preflight verified this session (read-only, via `gh api` and a scratch clone):**
- `gh` authenticated as `jdsaire` (repo, workflow, read:org, gist scopes). ✅
- Live `main` HEAD = `03663db36430642ac966499dd8e45c57fb470b4c` — **matches** `03663db` from
  `verified_state` exactly, no drift.
- `.github/workflows/deploy-pages.yml` read directly from the live repo — content **matches**
  `verified_state`'s description exactly: publishes `src/FifaPressApp` via `dotnet publish`,
  rewrites `<base href>` to `/fifa-press-app/` only in the CI publish output (tracked
  `wwwroot/index.html` untouched), adds a `404.html` SPA fallback keyed to `/fifa-press-app`, adds
  `.nojekyll`, triggers on push to `main` and `workflow_dispatch`. **No edit needed or made.**
- `ux-ui/03-ui-prototyping/` does **not** exist yet (404) — matches assumption; this run creates it.
- `ux-ui/README.md` **exists** — read in full. Lists `00-initial-evaluation/`, `01-design-research/`,
  `02-ideation/` as bullets, ending "From now on, every new UX dossier lands here as the next
  numbered folder." This run adds one bullet for `03-ui-prototyping/`, matching that stated pattern.
- `handoff/` contains `v1`–`v7` plus its own `README.md` (indexes each version in one bullet). Next
  version is **v8** — consistent with the deploy prompt's own branch name
  (`deploy/v8-dossier-injection-pages`). `handoff/v7/` is the closest-shape precedent for the
  archive folder: `CC-PLAN-v7.md`, `Completion-Report-v7.md`, `README.md` indexing both.
- No `deploy/v8-*` branch exists yet (existing branches: `v4`–`v7`, one stale `claude/...` branch,
  `main`). No open PRs — PRs #1–#4 (v4–v7) are all merged. **No naming collision.**
- GitHub Pages (`gh api repos/jdsaire/fifa-press-app/pages`) returns 404 — **not yet enabled**,
  matching `verified_state`. This run's Task 4 enables it via `gh api`, source GitHub Actions.
- Baseline internal-markdown-link count: a raw regex sweep of the current tree gives **232**; v7's
  own completion report recorded **229** raw / **211** live-resolving (18 documented pre-existing
  false positives from quoted example links inside `handoff/v2`, `v5`, `v6` plan/report files) at
  the same HEAD. The small delta is almost certainly the regex also catching image syntax
  (`![alt](path)`) that a plain link-count shouldn't include. **Task 1/5 in the actual run must
  replicate v6/v7's methodology exactly** — a real resolution pass (raw count vs. live-resolving
  count, with every non-resolving link individually attributed to a known cause), not a bare regex
  count — so the reported N/N is trustworthy and comparable to prior runs' own numbers.
- All nine attachment files read in full this session, confirmed present, each ending with its own
  `✅ GATE n COMPLETE` footer intact. Line/byte counts: `00_SCOPE.md` 85L/8332B ·
  `01_BENCHMARKING.md` 159L/19429B · `02_INFORMATION-ARCHITECTURE.md` 219L/19176B ·
  `03_UI-DECISIONS.md` 201L/24124B · `04_TASKS-AND-SCENARIOS.md` 206L/16801B · `05_SCREENS.md`
  231L/16319B · `06_DATA-MODEL.md` 197L/13281B · `07_BUILD-BRIEF.md` 222L/12399B ·
  `08_LIMITATIONS.md` 89L/9230B. These are committed byte-identical to the local
  `Prototyping-Artifacts/` copies — no regeneration.
- Commit author identity: no global `git config user.name`/`user.email` set in this environment.
  v7's own commits used `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>` (GitHub's
  no-reply address for user id `88201583`, confirmed via `gh api user`). The actual run must set
  this as local (repo-scoped) git config in the working clone — never global — before committing.

## Scope

Touches only:
- New `ux-ui/03-ui-prototyping/` folder: the nine frozen dossier files + one new `README.md`.
- One new bullet in `ux-ui/README.md`.
- GitHub Pages repo setting (source: GitHub Actions) — no file, no commit of its own.
- New `handoff/v8/`: renamed plan file, new `Completion-Report-v8.md`, new folder `README.md`.
- One new row in `handoff/README.md`.

Nothing under `src/FifaPressApp/`, `.github/workflows/`, `docs/`, `learning-mode/`,
`ux-ui/00-initial-evaluation/`, `ux-ui/01-design-research/`, `ux-ui/02-ideation/`, or root
`README.md` is touched.

## Branch and commit sequence

Branch: `deploy/v8-dossier-injection-pages`, cut from `main` at `03663db`.

Eleven commits total, in this order:

1. `docs(ux): add 00_SCOPE.md — Run 4A scope lock`
2. `docs(ux): add 01_BENCHMARKING.md — competitive benchmarking`
3. `docs(ux): add 02_INFORMATION-ARCHITECTURE.md — card sort and IA`
4. `docs(ux): add 03_UI-DECISIONS.md — UI design decisions`
5. `docs(ux): add 04_TASKS-AND-SCENARIOS.md — tasks and scenarios`
6. `docs(ux): add 05_SCREENS.md — screen specifications`
7. `docs(ux): add 06_DATA-MODEL.md — data and context model`
8. `docs(ux): add 07_BUILD-BRIEF.md — build brief for Run 4B`
9. `docs(ux): add 08_LIMITATIONS.md — Run 4A limitations`
10. `docs(ux): add 03-ui-prototyping folder README` (new folder README + `ux-ui/README.md` bullet)
11. `docs: archive v8 plan and completion report` (`handoff/v8/` + `handoff/README.md` row)

Between commit 9 and commit 10, GitHub Pages activation happens via `gh api` — a settings change,
not a commit, per the deploy prompt's explicit separation rule.

Each of commits 1–9 is verified byte-identical against the local `Prototyping-Artifacts/` source
**before** that commit is made, not after. Author and committer on every commit: `Juan Diego S.
<88201583+jdsaire@users.noreply.github.com>`, set as local git config in the working clone. No
`Co-authored-by`, no AI/agent mention anywhere.

## File plan

```
ux-ui/03-ui-prototyping/
├── README.md                        (NEW — authored this run, links all 9 in gate order,
│                                      points to 08_LIMITATIONS.md rather than restating it)
├── 00_SCOPE.md                      (verbatim copy)
├── 01_BENCHMARKING.md               (verbatim copy)
├── 02_INFORMATION-ARCHITECTURE.md   (verbatim copy)
├── 03_UI-DECISIONS.md               (verbatim copy)
├── 04_TASKS-AND-SCENARIOS.md        (verbatim copy)
├── 05_SCREENS.md                    (verbatim copy)
├── 06_DATA-MODEL.md                 (verbatim copy)
├── 07_BUILD-BRIEF.md                (verbatim copy)
└── 08_LIMITATIONS.md                (verbatim copy)

ux-ui/README.md                       (MODIFIED — one new bullet, matching the existing three)

handoff/v8/
├── README.md                        (NEW — indexes the two files below)
├── CC-PLAN-v8.md                    (this plan, renamed to match the run's nature)
└── Completion-Report-v8.md          (NEW — commit list w/ SHAs, PR #, outcome, PASS/FAIL table
                                       against success_criteria, authorized deviations or "None",
                                       autonomous decisions, open items carried forward — including
                                       every item from 08_LIMITATIONS.md's carried-forward table and
                                       07_BUILD-BRIEF.md §6, restated as inherited)

handoff/README.md                     (MODIFIED — one new row for v8, matching existing rows' style)
```

## Verification (during and after the run)

1. **Byte-identity**: `diff` each committed dossier file against the local
   `Prototyping-Artifacts/` source — not a re-read/eyeball check.
2. **Link integrity**: full-repo internal-link resolution pass, before and after, using v6/v7's
   raw-vs-live methodology (individually attribute every non-resolving link). Report N/N.
3. **Author check**: `git log` on `deploy/v8-dossier-injection-pages` shows only `Juan Diego S.`
   as author and committer on every commit; zero AI/agent attribution anywhere in commit messages,
   branch name, or authored files.
4. **Workflow untouched**: `diff` `.github/workflows/deploy-pages.yml` against its pre-run content
   — must be empty.
5. **Pages activation**: confirm via `gh api repos/jdsaire/fifa-press-app/pages` that source is
   GitHub Actions; report that the live URL resolves only once this PR merges and the workflow
   subsequently runs against `main` (this run does not merge).
6. **PR**: opened against `main` via `gh pr create`, left unmerged, title/body summarizing the
   eleven commits.
7. Stop-and-report conditions from the deploy prompt remain in force during execution: any drift
   from what preflight found, any dossier content that looks wrong, any folder that unexpectedly
   already has content, or any Pages-activation failure — report rather than improvise.
