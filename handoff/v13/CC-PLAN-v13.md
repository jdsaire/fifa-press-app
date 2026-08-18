# Plan — DEPLOY-FifaPressApp-EvaluationRemediation-v13_0

## Context

This deploy does two things, in order: (1) commits the already-approved `04-evaluation` dossier (9
files) verbatim into `ux-ui/04-evaluation/` — not a re-audit, findings and severity were already
settled at chat time; (2) executes the 4-item remediation scope that dossier's own Gate 7
(`07_REMEDIATION-SCOPE.md`) specified — a status-logic fix, an accessibility fix, and a two-part
Withdrawal affordance, the first time that gap closes in five runs. Not a redesign: every fix is
bounded to what Gate 7 already specifies.

---

## Task 0 — preflight, all confirmed

| Check | Result |
|---|---|
| `gh` auth | ✅ `jdsaire`, keyring |
| HEAD on `main` | ✅ `b37066d` — matches `verified_state` exactly, no drift |
| All 9 dossier attachments | ✅ present and readable |
| Baseline build | ✅ `dotnet build src/FifaPressApp -c Release` — 0 warnings, 0 errors |
| Baseline tests | ✅ `dotnet test tests/FifaPressApp.Tests` — **409 passed, 0 failed** |
| The one stray, out-of-scope remote branch named in this deploy's own preflight | ✅ confirmed present, unmerged, untouched |
| `ux-ui/04-evaluation/` | ✅ confirmed does not exist yet |

## Task 1 — source-file verification against `07_REMEDIATION-SCOPE.md` §4

All five files the dossier cites (`MyAccess.razor`, `EventDetails.razor`, `ChangeRow.razor`,
`ChangeArrivalTracker.cs`, `DemoAccountStore.cs`) and the three `wwwroot/i18n/*.json` files were
read in full, plus supporting files (`IAccessDataProvider.cs`, `MockAccessDataProvider.cs`,
`Change.cs`, `NavMenu.razor`, `Registration.razor`, `SignIn.razor`, `RequestAccessForm.razor`,
`ChangeTemplates.cs`).

**Logic-level match confirmed exactly** for every fix the dossier describes. One citation-only
discrepancy, not a stop condition: `MyAccess.razor`'s `StatusFor` sits at lines 309–323 in the live
tree rather than the cited 281–291 (drift from unrelated `v9`–`v12` changes) — the defect and the
fix are identical regardless of the line offset.

`WithdrawRequestAsync` already exists in the data layer, already tested, already appends a
superseding `Withdrawal` change. `ChangeTemplates.WithdrawalWhatChanged/Reason/NextStep` already
exist in all three languages. No `record.withdraw*` locale keys exist yet. The `aria-live="polite"`
pattern is established elsewhere (`SignIn.razor`, `RequestAccessForm.razor`). Test infrastructure is
xUnit + bUnit with an established `Harness`-style pattern (`DisclosureTests.cs`, `GatingTests.cs`).

**No discrepancy rose to a stop condition.**

### Commit sequence approved

1. `docs(ux): add run 04 evaluation dossier` — 9 dossier files byte-identical, renamed
   `00-scope.md` → `00_SCOPE.md`; new `ux-ui/04-evaluation/README.md`; one new row in
   `ux-ui/README.md`.
2. `fix(record): exclude not-yet-effective changes from the per-match status word` — Item 1:
   `EventDetails.razor:235` `FoldStatus(DateTime.MaxValue)` → `FoldStatus(Access.AsOfUtc)`;
   `MyAccess.razor`'s `StatusFor` gains an `EffectiveUtc <= Access.AsOfUtc` filter. Four new tests.
3. `feat(record): announce a newly arrived change to assistive technology` — Item 2: an
   `aria-live="polite"` region on `ChangeRow.razor`, scoped to `JustArrived`. One new test.
4. `feat(record): thread the pending change's id through MatchAccessLine` — Item 3a: extends
   `MatchAccessLine` with `PendingChangeId`; refactors `StatusFor` to resolve the underlying change
   once. Two new tests. Lands strictly after commit 2, reusing its corrected lookup.
5. `feat(record): add a withdraw affordance to pending match requests` — Item 3b: a withdraw
   button + inline confirm gated to `Status == Requested`; four new locale strings in all three
   `wwwroot/i18n/*.json` files, verbatim as pre-authored. Five new tests.

Verification after every commit: `dotnet build src/FifaPressApp -c Release` clean, `dotnet test
tests/FifaPressApp.Tests` green with the expected growing count.

## Task 7 — verification checklist

The full `07_REMEDIATION-SCOPE.md` §6 checklist, run after commit 5: build, test count and growth,
frozen-path diffs, frozen-test immutability, the twelve named new tests, an attribution sweep, a
link-integrity recount, and the Withdrawal-gating proof.

## Task 8 — archive

`handoff/v13/`: this plan, a Completion Report, a folder README, one new row in `handoff/README.md`.
PR opened against `main` from `deploy/v13-evaluation-remediation`, left unmerged.

---

*(This plan was approved via Plan Mode before any commit in this run landed. See
`Completion-Report-v13.md` for what was actually executed against it.)*
