# v7 — Namespace Rename + Workflow Correction

Renames the project's source folder and namespace from `EventEase` to `FifaPressApp`, and corrects
`.github/workflows/deploy-pages.yml`, which had hardcoded a different repository's name
(`frontend_c4_blazor_eventease`) in six places. Sweeps every citation of the old path across
`docs/`, `handoff/v1-v5`, `learning-mode/`, and `ux-ui/`. Mechanical rename only: no application
behavior, CSS, or content changed, and GitHub Pages is not yet activated.

- [`CC-PLAN-v7.md`](CC-PLAN-v7.md) — the plan approved before this run started: verification
  findings against live HEAD, the citation-sweep treatment table, the link-integrity baseline, and
  the commit sequence.
- [`Completion-Report-v7.md`](Completion-Report-v7.md) — what actually happened: commit list,
  PASS/FAIL results, the authorized UI-text exception, decisions resolved autonomously, and open
  items carried forward — including that GitHub Pages activation is next.
