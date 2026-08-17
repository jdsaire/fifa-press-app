# v12 — Addendum Implementation

Run 4E: builds the four Run 4D design-addendum files (`09_DESIGN-ADDENDUM.md` through
`12_DECISION-REVERSALS.md`, finalized at v11) into working code, in five gated boundaries — visual
identity, auth and landing, EN/ES/PT, TypeScript interop, disclosure patterns. The addendum files
themselves were not touched; every decision they made was already final, and this run is where those
decisions became a dark theme, a simulated two-holder session, three languages with no reload on
switch, a small TypeScript interop layer, and progressive disclosure on the change log and Help.

This is the second attempt at the run. The first was lost mid-boundary-4 when the session scratchpad
holding its only working clone was wiped externally, before the branch had ever reached the remote —
`main` was unaffected throughout, at `ac5555c`. The recovery record this run started from is at
`~/.claude/recovery/v12-addendum-implementation-RECOVERY.md`, outside this repository; its corrective
instruction — push the branch after every boundary, not just at the end — is why this run's 22
commits reached the remote continuously rather than sitting unpushed for three boundaries at a time.

- [`CC-PLAN-v12.md`](CC-PLAN-v12.md) — the plan approved before this run's first commit landed: every
  decision task 1 required (localization approach, the second demo record, TypeScript scope, the
  open route, the string inventory, the link baseline), the `11_I18N.md` §5.3 discrepancy stated and
  resolved in advance, the full 21-commit sequence across five boundaries, and the verification plan.
- [`Completion-Report-v12.md`](Completion-Report-v12.md) — what actually happened: the 22-commit
  list with SHAs, the verification checklist reported item by item, a corrected link-integrity
  measurement (the plan's own 299/288 citation turned out to be unverifiable and is superseded here
  by a figure independently re-derived against `main` with the method stated precisely), six
  authorized deviations including the full lifecycle of one that was proposed, flagged, and then
  reversed within this same run, five decisions resolved autonomously, open items carried forward,
  and the one item this run could not complete unattended — opening the pull request itself, blocked
  by an expired `gh` credential mid-run.
