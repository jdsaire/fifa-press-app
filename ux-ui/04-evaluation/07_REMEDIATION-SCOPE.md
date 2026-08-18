# Movement Summary & Remediation Scope

**Repo path:** `ux-ui/04-evaluation/07_REMEDIATION-SCOPE.md`
**Direct continuation of:** `00-initial-evaluation/remediation-scope.md` — same two-constraint
discipline, this run's own constraints in place of `00`'s
**This table is what the CC prompt (step 2, `/cc-deploy-prompts`) is authored from.** Nothing in
that prompt should assert a fix this file does not already specify.

---

## 1. Movement summary — the headline, before the mechanism

| Metric | `00-initial-evaluation/` | `04-evaluation` | Movement |
|---|---|---|---|
| Critical findings | 4 | **1** (`04-CRIT-01`, new) | ↓↓↓ |
| Major findings | 12 | **2** (`04-MAJ-01`, `04-MAJ-02`, new) | ↓↓↓↓↓ |
| Minor findings | 10 | **2** (`04-MIN-01`, `04-MIN-02`, new) | ↓↓↓↓↓ |
| Nielsen principles | 5 Fails · 4 Partial · 1 Pass | **2 Fails · 1 Partial · 7 Pass** | ↑↑↑↑↑↑ |
| WCAG 2.2 AA | 8 Pass · 12 Fail · 3 Open — does not meet AA | **18 Pass · 1 Fail · 3 Open — meets AA on every settleable criterion** | ↑↑↑↑↑↑↑↑↑↑ |
| Task success rate | N/A — no tasks existed | **6 of 6 (100%)** | New this run |

**Does "one structural fix addressed most of `00`'s findings at once" continue, or does this run get
a different chapter?** Both, in different directions. The *positive* movement is distributed, not
concentrated the way `00`'s single `EventCard` decision was — seven heuristics moved to Passes via a
set of independent investments (disclosure, `StaleIndicator`, search/filter, read-only-by-default
cards, in-app Help), not one lucky cascade. But the *residual* problem clusters exactly the way
`00`'s did: `04-CRIT-01` alone is responsible for the one Nielsen Fail that isn't Withdrawal, the one
WCAG failure, the usability ceiling on two of five components, and friction in two of three tasks.
`00`'s story was "one bad decision, six symptoms." This run's story is "many good decisions, plus one
remaining bad one with almost as wide a reach." Both stories point the same way: fix the one thing.

---

## 2. Constraints, restated and one formally amended

**Constraint 1 — every fix stays inside what 4E already built, with one named exception.** This is a
remediation pass, not a sixth design mandate. A finding that requires a new design decision the
addendum never made gets flagged for a future run, not resolved here by inventing one — **except
Withdrawal**, which is now in scope by explicit, recorded authorization rather than by this file
quietly deciding it belongs. See §3.

**Constraint 2 — `09`–`12` are Final and frozen, unchanged.** A fix may correct an *implementation*
gap against them; it may not contradict a decision they made. This is why `04-MAJ-02` (no
Freelance-track demo record) stays out of scope below despite being the same *shape* of gap as
Withdrawal was — `10_AUTH-AND-ONBOARDING.md` specifically decided on two demo records, not three, and
adding a third without the same kind of explicit authorization Withdrawal received would be this file
making that call unilaterally rather than recording one that was actually made.

---

## 3. The Withdrawal decision, recorded formally

Per this project's own standing practice (`12_DECISION-REVERSALS.md`'s register: named, reasoned,
not smoothed over) — this is not a reversal of a prior decision, but the same discipline applied to
a decision made fresh, mid-dossier, outside the normal per-gate approval flow.

**What was decided, and by whom:** at the principal's explicit instruction, given between Gate 6 and
this gate, `UX-MAJ-06` (a registration/request cannot be undone — Still Open across five prior runs,
per `05_FINDINGS-REGISTER.md`) is brought into scope for this remediation pass. The shape of the fix
was proposed by this chat and confirmed by the principal before this file was authored: a minimal
affordance on `MyAccess.razor`, gated to `MatchAccessStatus.Requested` lines only, with an inline
(not modal) confirm step, calling the data layer's existing, already-tested `WithdrawRequestAsync`.

**Why this needed the same weight as a `12_DECISION-REVERSALS.md`-style entry rather than a quiet
inclusion:** building any UI for this is a genuine design decision — where it lives, what it says,
whether it confirms — and `Registration.razor`'s own header comment states plainly that this absence
was deliberate ("no surface for it has been specified. Adding one here would be inventing scope
rather than completing it"). Overriding a deliberate prior absence needs the same explicit,
attributable authorization any other decision reversal in this project gets.

---

## 4. Sequenced remediation, commit-sized units

Ordered so each item builds on the one before it where a dependency exists. This table is the
direct input to the CC prompt; nothing below asserts a design decision not already settled in this
file or an earlier gate.

### Item 1 — Fix the per-match status word to respect `Urgency` and `EffectiveUtc`

**Closes:** `04-CRIT-01` · the WCAG 4.1.3 failure (`04_ACCESSIBILITY-AUDIT.md` §3) · contributes to
moving Heuristic 1 and Heuristic 4 (`03_HEURISTIC-EVALUATION.md`)

**The fix, precisely — not a new decision, a correction to match logic the codebase already uses
correctly elsewhere:** `EventDetails.razor:235`'s `status = FoldStatus(DateTime.MaxValue)` folds in
every change regardless of whether it has taken effect, unlike `venueStatus =
FoldStatus(VenueListAsOf)` two lines below it, which already correctly bounds by time. Change the
headline computation to `status = FoldStatus(Access.AsOfUtc)` — the same "now" bound the rest of the
app already treats as authoritative. In `MyAccess.razor`'s `StatusFor(matchNumber)`
(`:281-291`), add an `EffectiveUtc <= Access.AsOfUtc` filter to the `.Where` clause before selecting
`latest`, mirroring the same correction.

**What this does *not* change:** `ChangeRow`/`ForeseeableBadge` are already correct and untouched.
The seeded data (`ch-005`, `ch-008`) is untouched. No `09`–`12` decision is contradicted — this
makes two pieces of the app's own logic consistent with each other and with a third piece
(`venueStatus`) that already does it right.

**Tests to add** (naming convention matching the existing suite's descriptive-sentence style):
- `MatchAccessStatusTests.AForeseeableChangeDoesNotRenderAsADecidedStatus` — Amina, `ch-005`
- `MatchAccessStatusTests.ASilentChangeDoesNotRenderAsADecidedStatusEither` — Tomás, `ch-008`,
  since the same defect reproduced for a `Silent`-classified change per `05_FINDINGS-REGISTER.md`
- `MatchAccessStatusTests.AnAlreadyEffectiveChangeStillRendersItsDecidedStatus` — regression guard;
  the fix must not suppress a status for a change that genuinely has taken effect
- `GateCheckStatusTests.TheHeadlineStatusExcludesNotYetEffectiveChanges` — `EventDetails.razor`
  equivalent

**Files touched:** `Pages/MyAccess.razor`, `Pages/EventDetails.razor`, plus new test file(s)

**Proposed commit message:** `fix(record): exclude not-yet-effective changes from the per-match status word`

---

### Item 2 — Announce a newly-arrived change to assistive technology

**Closes:** `04-MAJ-01`

**The fix:** add an `aria-live="polite"` (or `role="status"`) region that announces the arriving
change's `WhatChangedText` when `JustArrived` is true — matching the pattern already used for form
errors elsewhere in the same file tree (`SignIn.razor:107,127`, `RequestAccessForm.razor:36,54`),
applied to a success case for the first time. Can live on `ChangeRow.razor` itself (scoped to the
one row that just arrived) or as a small standalone announcement region on `MyAccess.razor` — either
satisfies the finding; implementation is free to choose based on what's cleaner against the existing
`<details>` structure.

**Tests to add:**
- `ChangeArrivalAnnouncementTests.AJustArrivedRowIsAnnouncedToAssistiveTechnology`

**Files touched:** `Components/ChangeRow.razor` (or `Pages/MyAccess.razor`), plus test file(s)

**Proposed commit message:** `feat(record): announce a newly arrived change to assistive technology`

---

### Item 3a — Thread the pending change's id through `MatchAccessLine`

**Closes:** groundwork for Item 3b · `UX-MAJ-06`

**Depends on Item 1** — reuses the same effective-date-filtered "latest" lookup Item 1 corrects, so
this should land after it rather than duplicate the fix against the old, uncorrected logic.

**The fix:** `MatchAccessLine` currently carries `(int MatchNumber, MatchAccessStatus Status)`.
Extend it to `(int MatchNumber, MatchAccessStatus Status, string? PendingChangeId)`, populated only
when `Status == MatchAccessStatus.Requested`, from the same underlying change `StatusFor` already
resolves internally (refactor `StatusFor` to expose the resolved change, not just its `Kind`, so
this doesn't duplicate the LINQ lookup a second time).

**Tests to add:**
- `MatchAccessLineTests.APendingRequestCarriesItsOwnChangeId`
- `MatchAccessLineTests.AGrantedOrRevokedLineCarriesNoChangeId`

**Files touched:** `Pages/MyAccess.razor`, plus test file(s)

**Proposed commit message:** `feat(record): thread the pending change's id through MatchAccessLine`

---

### Item 3b — Add a withdraw affordance to pending requests, gated and confirmed

**Closes:** `UX-MAJ-06`, open since v9 — the first run in five to close it

**Depends on Item 3a.**

**The fix, per the shape confirmed with the principal before this file was authored:**
- A button rendered only on a `MatchAccess` line where `Status == Requested` (using `3a`'s
  `PendingChangeId`), placed beside the existing status word.
- Clicking shows an inline confirm — not a modal, matching this app's own established register
  (`NavMenu.razor`'s sign-out has no confirmation dialog "because there is nothing to lose"; a
  withdrawal is a real recorded write, so it gets a lightweight confirm, but the app's own pattern
  argues against a heavier modal for anything this reversible-in-spirit).
- Confirming calls `Access.WithdrawRequestAsync(credentialId, PendingChangeId)` — already
  implemented, already tested at the data layer, no change needed there.
- On success, the returned `Change` is appended to the page's local `allChanges`, `arrivedChangeId`
  is set to its `ChangeId` directly (no navigation occurs, so the existing `ChangeArrivalTracker`
  round-trip isn't needed — set the field and call `Rebuild()` + `StateHasChanged()` inline), and
  the row opens via the same `JustArrived` mechanism `/request/{id}` already uses. This should also
  fire whatever announcement mechanism Item 2 introduces — one confirmation pattern for the whole
  app, not two.

**New locale strings needed — drafted here, `[ASSUMPTION]`-tagged as voice-consistent but not yet
reviewed against a addendum, per this project's provenance discipline:**

| Key | EN | ES | PT |
|---|---|---|---|
| `record.withdrawButton` | Withdraw request | Retirar solicitud | Retirar pedido |
| `record.withdrawConfirmPrompt` | Withdraw your request for this match? It stays visible in your record either way. | ¿Retirar tu solicitud para este partido? De todas formas seguirá visible en tu registro. | Retirar o seu pedido para este jogo? De qualquer forma, continuará visível no seu registo. |
| `record.withdrawConfirmYes` | Yes, withdraw | Sí, retirar | Sim, retirar |
| `record.withdrawConfirmCancel` | Cancel | Cancelar | Cancelar |

`ChangeTemplates.WithdrawalWhatChanged/Reason/NextStep` already exist, fully authored in all three
languages (`Services/ChangeTemplates.cs:57-79`), and need no change — the resulting `ChangeRow`
after a withdrawal already reads correctly without any new template work.

**Tests to add:**
- `WithdrawalAffordanceTests.APendingRequestCanBeWithdrawnFromMyAccess`
- `WithdrawalAffordanceTests.WithdrawingRequiresConfirmationFirst`
- `WithdrawalAffordanceTests.AGrantedOrRevokedLineOffersNoWithdrawControl`
- `WithdrawalAffordanceTests.TheWithdrawnChangeAppearsAsANewJustArrivedEntry`
- `WithdrawalAffordanceTests.TheOriginalRequestStaysVisibleAfterWithdrawal` — the CH-3/CH-7
  non-deletion convention, extended to this new write path

**Files touched:** `Pages/MyAccess.razor`, `wwwroot/i18n/en.json`, `wwwroot/i18n/es.json`,
`wwwroot/i18n/pt.json`, plus test file(s)

**Proposed commit message:** `feat(record): add a withdraw affordance to pending match requests`

---

## 5. Explicitly not sequenced — flagged, not resolved

| Finding | Why it stays out |
|---|---|
| **`04-MAJ-02`** — no Freelance-track demo record | Same *shape* of decision as Withdrawal — adding a third demo account is a real design choice, not an implementation-gap fix — but it has not received the same explicit authorization Withdrawal did. `10_AUTH-AND-ONBOARDING.md` specifically decided on two records; a third contradicts, not corrects, that decision per Constraint 2. Flagged for a future run, or for the principal to authorize the same way Withdrawal was authorized here |
| **`04-MIN-01`** — nav "Sign out" row ES layout risk | Gate 4 classified this REASONED, not CODE-VERIFIED — the structural fragility (fixed height + matching line-height) is confirmable from source, but whether "Cerrar sesión" actually wraps at the narrow breakpoint needs a real render this dossier cannot produce. Fixing speculatively, without confirming the defect exists, would be acting on inference the project's own method explicitly declines to do elsewhere. Recommended as the first thing to render-check if a browser session becomes available, not actioned here |
| **`04-MIN-02`** — `GateCheckResult` naming mismatch | No code defect — behaviorally equivalent to the flowchart's description. Informational only, for a future dossier author; no remediation action |
| **`UX-MIN-06`** — no `max-width` on text columns, from `00` | `05_FINDINGS-REGISTER.md` marked this "Still Open (unconfirmed)" rather than guessed at — not independently re-verified this run. Not fixed here for the same reason `04-MIN-01` isn't: acting on an unconfirmed claim would contradict this dossier's own evidence discipline. Carried to a future run's verification pass |

---

## 6. Verification checklist for the CC run

Mirroring the PASS/FAIL convention every prior build-adjacent run in this project has used:

| # | Check |
|---|---|
| 1 | `dotnet build src/FifaPressApp -c Release` clean after each commit individually |
| 2 | `dotnet test tests/FifaPressApp.Tests` green; count grows from the current baseline with zero failures at any point |
| 3 | `git diff` empty, individually, for `ux-ui/00-initial-evaluation/`, `ux-ui/01-design-research/`, `ux-ui/02-ideation/`, `ux-ui/03-ui-prototyping/` (all of `09`–`12` included) |
| 4 | The withholding rule's frozen tests (`TwoRecordsTests`, `LocalizedChangeTests`, `LanguageSwitchTests`, `LocalizedSearchTests`) pass unmodified |
| 5 | Every new test named in §4 above exists and passes |
| 6 | No commit message or diff contains AI-attribution language, per this project's standing rule |
| 7 | Link-integrity sweep re-measured with the method §7.5-corrected in `00_SCOPE.md`, not carried from a prior run's citation |
| 8 | Withdrawal is reachable **only** from a `Requested`-status line — a `Granted`/`Revoked`/`NotRequested` line offers no control |

---

## 7. What this file hands to Gate 8 and the CC prompt

- **Three commit-sized fixes, sequenced** (Item 1 → Item 2 → Item 3a → Item 3b), closing
  `04-CRIT-01`, `04-MAJ-01`, and `UX-MAJ-06` — the last of these closed for the first time in five
  runs.
- **Two findings deliberately withheld from this pass**, each with a stated reason distinct from
  "ran out of time": `04-MAJ-02` needs the same kind of explicit authorization Withdrawal received
  and hasn't gotten it; `04-MIN-01` and `UX-MIN-06` need a render this dossier cannot produce and
  won't fix speculatively.
- **One formally recorded, attributed decision** (§3) that a future reader can trace exactly the way
  `12_DECISION-REVERSALS.md` lets them trace every other project-level decision.
- Gate 8 (`08_LIMITATIONS.md`) is next — stating plainly what this whole evaluation cannot claim,
  including that this remediation, once merged, immediately begins drifting from whatever lands on
  `main` after it.

---

✅ **GATE 7 COMPLETE** — `07_REMEDIATION-SCOPE.md`
