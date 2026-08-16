# v10 — Frontend Course Correction + Test Foundation

Run 4B-R: four straight patches against v9's Access Record build, all with no design dependency,
plus the repository's first committed test project. Fixes the request-access Submitting state
(specified in `05_SCREENS.md` §5.2 but never observable, because the provider's write path returned
an already-completed task), adds decorative date/venue/phase iconography to the three match
surfaces, and adds group and match-status filters to the match list, composing with the existing
search. Search itself was extracted into a pure query helper and proven behaviourally unchanged.

The highest-value work sits in `tests/`, not in the four patches: 82 tests covering the schedule
importer's real parse hazards, the withholding rule across the whole 104-fixture tracked schedule
(the single most important property this app has), the data provider's append-only change log, and
every item this run added. `07_BUILD-BRIEF.md` §5's standing no-new-dependency rule was reversed for
this one purpose, authorized by `P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md` §5.2 — the new packages
live only in the test project, and `src/FifaPressApp/FifaPressApp.csproj` is byte-identical to what
v9 shipped.

Not this run: visual identity, the dark-palette re-derivation, theme-trigger relocation,
authentication, the public landing view, EN/ES/PT, and TypeScript interop — all reserved for the 4D
design addendum and the 4E build that follows it.

- [`CC-PLAN-v10.md`](CC-PLAN-v10.md) — the plan approved before this run started: preflight findings
  against live HEAD, the test-stack versions confirmed by actually building and running a probe
  project, the group and status filter option sets, the simulated write-latency value, and the
  full commit sequence with its three gates.
- [`Completion-Report-v10.md`](Completion-Report-v10.md) — what actually happened: the commit list
  with SHAs and the PR number, the `success_criteria` checklist reported item by item with evidence,
  the link-integrity sweep, four authorized deviations, five decisions resolved autonomously, and
  open items carried forward — including the confederation filter's data prerequisite and the
  deferred `learning-mode/` chapter.
