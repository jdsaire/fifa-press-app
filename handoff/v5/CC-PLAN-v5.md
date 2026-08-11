# Plan — DEPLOY v5: Design Research Injection into jdsaire/fifa-press-app

## Context

The user authored eight gate deliverables (Gates 0–7) plus two simulated datasets and a glossary in
a prior chat, covering a "Run 2 Guerrilla" UX research study for the FIFA Press App's media-
accreditation workflow. `handoff/v4/Completion-Report-v4.md` explicitly names this injection as its
own "next run" ("Research-mandate injection ... is the next run, explicitly not part of this one").
This run fulfills that named next step: copying the eleven already-approved files verbatim into a
new `Design Research/` folder at repo root, cross-linking them into the existing README doctrine,
and archiving the run under `handoff/v5/` — with zero changes to application code or any other
mandate track.

**Preflight verified (via `gh api`, read-only, this session):**
- `gh` authenticated as `jdsaire` (repo, workflow scopes). ✅
- Live `main` HEAD = `97b724db30dc7aa8da7061bf44b13422ea91ba0e` — **matches** `verified_state` exactly. No drift.
- Root listing matches: `.github/`, `.gitignore`, `README.md`, `docs/`, `handoff/` (v1–v4), `learning-mode/`, `src/`, `ux-ui/evaluation-spec/`.
- All eleven attachment files read and confirmed present locally (00–07, survey_master.csv, interviews_master.json, GLOSSARY.md).
- `FIFA_app_P-Research.txt` and `P-Fifa-Repo-Kickoff.txt` — **confirmed absent** from the live repo (code search returned nothing). Per guardrail: do not invent them; note the gap in the Completion Report; do not cross-link to files that don't exist.
- Root `README.md` **has** a `## Documentation` section (bullet list of docs) — this is where the new folder/glossary links go (task 3).
- Root `README.md` has **no** "pending mandates" / roadmap section naming Design/Prototyping/Evaluation — per guardrail, task 3 stays limited to the documentation-index + glossary-link additions only; no new section is invented.
- `handoff/README.md` parent index is a flat bullet list, one line per version, in prose — v5's row will match that exact style.
- `handoff/v4/` folder shape confirmed: `README.md`, `CC-PLAN-v4.md`, `Completion-Report-v4.md` — v5 will mirror this exactly.

## Scope (unchanged from the deploy prompt)

Touches only: new `Design Research/` folder, root `README.md` (two additive sections), `handoff/`
(new `v5/` folder + one row in the parent index). Nothing under `src/`, `.github/`, `docs/`,
`learning-mode/`, `ux-ui/` is touched. Content of the eleven research files is copied byte-verbatim
— no rewriting.

## File plan

```
Design Research/
├── README.md                    (NEW — authored this run)
├── 00_SCOPE.md                  (verbatim copy)
├── 01_RESEARCH-PLAN.md          (verbatim copy)
├── 02_INSTRUMENTS.md            (verbatim copy)
├── 03_SIMULATION-NOTE.md        (verbatim copy)
├── survey_master.csv            (verbatim copy)
├── interviews_master.json       (verbatim copy)
├── 04_ANALYSIS.md               (verbatim copy)
├── 05_ARTIFACTS.md              (verbatim copy)
├── 06_DESIGN-BRIEF.md           (verbatim copy)
├── 07_LIMITATIONS.md            (verbatim copy)
└── GLOSSARY.md                  (verbatim copy)

handoff/v5/
├── README.md                    (NEW — mirrors handoff/v1–v4 folder-README style)
├── CC-PLAN-v5.md                (this approved plan, renamed)
└── Completion-Report-v5.md      (NEW — outcome, PASS/FAIL table, deviations, open items)
```

**Root `README.md` diff** (two additive edits inside/near the existing `## Documentation` section — exact wording drafted at commit time, matching existing bullet style):
1. One bullet linking `Design Research/README.md`.
2. One direct bullet linking `Design Research/GLOSSARY.md` (not buried a folder deep, per Repo Standard §10).
3. No new "roadmap"/"pending mandates" section — confirmed absent from current README, so per guardrail this run does not create one.

**`handoff/README.md` diff:** one new bullet appended, matching the existing v1–v4 line format exactly, e.g.:
`- [`v5/`](v5/) — injects the completed UX Research mandate (eight gates, simulated survey/interview data, glossary) as a new \`Design Research/\` folder. Documentation only — no application behavior changed.`

## `Design Research/README.md` outline

- What the folder is and why (guerrilla research mandate, Run 2, simulated study — link to `00_SCOPE.md` for the "what this can/cannot establish" framing).
- Table indexing all 8 gate files in order, one line each, noting `survey_master.csv` and `interviews_master.json` as siblings of Gate 3's note (not separate gates).
- `GLOSSARY.md` noted as post-deploy, not a gate deliverable, with a direct link.
- Cross-link to `../ux-ui/evaluation-spec/` clarifying it is a separate, earlier, unrelated mandate track (EventEase-era usability audit).
- Explicit note: `FIFA_app_P-Research.txt` / `P-Fifa-Repo-Kickoff.txt` are not present in this repo, so no link is made to them (documents the guardrail resolution instead of silently omitting it).

## Commit grouping (rationale stated explicitly per hard_rules)

1. **One commit** — all ten frozen research files + `GLOSSARY.md`, added verbatim as a single batch. Rationale: they were authored and approved together as one gated sequence in the source chat; splitting them into 11 commits would fabricate a false incremental-authorship history for content that was actually a single delivered artifact.
2. **One commit** — `Design Research/README.md` (new authored content, logically distinct from the frozen payload).
3. **One commit** — root `README.md` update (documentation index + glossary link).
4. **One commit** — `handoff/v5/` archive (plan + completion report + folder README) + `handoff/README.md` parent index row, message: `"docs: archive v5 plan and completion report"` per the deploy prompt's exact wording.

Branch: `deploy/v5-design-research-injection`, created from `main` at `97b724d`. All commits authored and committed as `jdsaire`, no co-authorship trailers, no AI-assistant mentions beyond the existing repo's neutral "AI coding assistant" phrase (used only where the existing convention already appears — the new files here don't need to restate it).

## Verification steps

1. After writing each of the 11 injected files, diff against the source payload (`Design-Research-Artifacts/*`) to confirm byte-identical content — report per-file zero-drift.
2. Full internal-markdown-link resolution sweep across the whole repo, before and after, reported as N/N (baseline count to be taken right after branch creation, before any file is added).
3. Grep the entire diff (new folder + modified README + handoff/v5) for any AI-assistant-name mentions — must return zero hits.
4. `git log` on the working branch — confirm `jdsaire` as sole author/committer on every commit, no co-authored-by trailers.
5. `git diff 97b724d -- src/ .github/ docs/ learning-mode/ ux-ui/` — must return 0 lines.
6. Open PR against `main` via `gh pr create`, from `deploy/v5-design-research-injection`, and stop — do not merge.
7. Confirm `handoff/v5/` has no AI attribution, and that the Completion Report explicitly re-affirms (not re-discovers) the two carried-forward open items from v4: the Pages/base-href issue, and the three pending UX mandates (Design, Prototyping, Evaluation).

## Open decision already resolved (per guardrails, no action needed)

- `FIFA_app_P-Research.txt` / `P-Fifa-Repo-Kickoff.txt`: absent from repo → not linked, not invented; noted as an open item in the Completion Report.
- Folder name stays `Design Research` (capital D, capital R, one space) exactly as specified — not normalized to kebab-case.
- No "pending mandates" section is created in root README since none currently exists.
