# Task Execution

**Repo path:** `ux-ui/04-evaluation/02_TASK-RESULTS.md`
**Adapts:** `00-initial-evaluation/protocol-results.md`
**Runs against:** `01_TASK-PROTOCOL.md` — every pass/fail call below is scored against that file's
§3 table, not re-derived here
**Audited build:** `main` @ `b37066d`, read in source — no browser session available in this chat,
per `00_SCOPE.md` §1

**SIMULATED — NOT EVIDENCE.** Every attempt below is an authored walkthrough against real source,
not a recording of a real person using the app.

---

## Method

Each attempt is a screen-by-screen walk, citing the real route, the real component, and real copy
pulled from `wwwroot/i18n/{locale}.json` — not paraphrased. Two seed facts ground every attempt and
are stated once here rather than repeated six times:

- **Simulated "now":** `MockAccessDataProvider.SimulatedNow` = **3 July 2026, 20:31 UTC**
  (`Services/MockAccessDataProvider.cs:75`).
- **Amina's record last synced:** `SeededLastSyncedUtc` = **3 July 2026, 17:15 UTC**
  (`:82`) — an age of **3h16m** against `SimulatedNow`, which is past `StaleIndicator`'s one-hour
  `StaleAfter` default, so every attempt that reaches My Access sees the indicator in its **old**
  state, not its quiet one. This is by design (`StaleIndicator.razor`'s own header comment: "the
  staleness this app is built to show is actually visible rather than always reading zero") and
  every attempt below confirms it renders that way.

---

## Attempt 1 — W1 · Task 1 · English · Light

**Route:** sign in at `/signin` with Amina's demo credential (`MP-2026-04817` /
`amina-demo-2026`, `DemoAccountStore.cs:65`) → redirected to `/record` (`MyAccess.razor:29`,
`SignIn.razor` on success) → **`MyAccess.razor`**.

**Walkthrough.** The headline renders instantly from cache — no spinner, per the component's own
loading-rule comment (`MyAccess.razor:18-21`). `StaleIndicator` renders immediately below the
`AccessCard`, always-on: **"Last updated 3 hours ago. Your access may have changed since."**
(`stale.lastUpdatedStale`, formatted with `stale.hours` and `record.staleSubject`). Under **"What
changed"** (`record.changesHeading`), `ch-005` is the only visible change (nothing supersedes it,
nothing is superseded by it) and renders collapsed via `<ChangeRow>`, with `ForeseeableBadge`
showing because `Change.Urgency == Urgency.Foreseeable` (`ChangeRow.razor:32-33`). Opening the
disclosure shows the condition, both outcomes, the reason, and the next step verbatim from
`ch-005`'s seeded `LocalizedText` — "Hold your Dallas travel until after that fixture, or book
something you can change. You will not need to re-apply either way." (`nextStep.En`,
`MockAccessDataProvider.cs:649-652`).

**Success criterion:** states what changed, why, and remaining options, without leaving My Access.
**Met** — everything the criterion needs is on this one screen without navigation.

**PASS**, with one finding surfaced in the process:

### `04`-scoped finding — the headline contradicts the change entry directly beneath it

**Before opening `ch-005`'s disclosure, the "MatchAccess" block at the top of the same screen —
part of the same render, above the fold** — already states Amina's quarter-final status in a single
word: **"Access withdrawn"** (`status.revoked`, `wwwroot/i18n/en.json`), rendered by
`MyAccess.razor`'s `StatusWord(MatchAccessStatus.Revoked)` (`MyAccess.razor:206-207,315-321`).

This is not a display bug in the visible text — it is a genuine gap in `StatusFor`
(`MyAccess.razor:281-291`), which selects the latest change affecting a match number and maps its
`Kind` straight to a status word, with **no check against `EffectiveUtc` and no check against
`Urgency`**. `ch-005` is `Kind: MatchAccessRevoked`, so it resolves to `Revoked` → "Access
withdrawn" — identically to how an already-effective revocation would render — even though the
change is `Urgency.Foreseeable`, has not taken effect (`EffectiveUtc` is 6 July, three days after
`SimulatedNow`), and is explicitly conditional on a match that has not been played.

Two inches below that same headline, `ChangeRow` for the very same change correctly marks it
`ForeseeableBadge` and reads "Not decided yet — depends on a match still to be played"
(`foreseeable.label`). **The screen asserts both things about the same fact at once.** No CSS
variant exists to soften the headline word either — `MyAccess.razor.css` defines
`--granted` and `--revoked` only, no conditional state (confirmed: no third class in that file).

This lands on exactly the failure mode `04_TASKS-AND-SCENARIOS.md` §3 names for this task — *"she
believes a foreseeable change has already happened"* — sourced not from a misreading, but from a
sentence the app itself states as fact. No test in the 409-test suite exercises this: a search of
`tests/FifaPressApp.Tests/` for any assertion tying `MatchAccessLine`/`StatusFor` to `Urgency`
returns nothing.

**Task 1 still passes** — the participant can find the reason and the real, conditional status by
opening the row, and the success criterion doesn't require the headline to be correct, only that
the reason be findable. This is recorded as friction on a passing attempt, per Gate 2's own
instruction, and handed to `05_FINDINGS-REGISTER.md` as a **04-CRIT** candidate: the severity band's
own rule (`00_SCOPE.md` §3 item 5, `00`'s definition) is *"a user finishes believing something
happened that did not"* — which is precisely what an unqualified reading of the headline alone
produces.

---

## Attempt 2 — W1 · Task 2 · English · Light

**Route:** `/record`, signed in as Amina (session carried from Attempt 1) → `ForeseeableBadge` on
`ch-005`'s row prompts a look at the dependency → **`/events/93`** (Match detail for the Round-of-16
fixture, per the task's optional branch, `04_TASKS-AND-SCENARIOS.md` §4 flowchart node I).

**Walkthrough.** `/events/93` loads the fixture (`EventDetails.razor:20`). Withholding holds:
`fixture.IsResolved` is false (kickoff 6 July, after `SimulatedNow`), so `detail.unplayed` renders
— **"This match has not been played, so the teams in it are not shown."** — and `EventCard`'s title
comes from `FixtureLabels.Display`, which for an unresolved fixture is round + venue + city, not the
CSV's actual "Portugal v Spain" matchup (confirmed against `2026_World_Cup_Schedule.csv` match 93:
Round of 16, AT&T Stadium, Dallas, 6 Jul). The withholding rule holds exactly as the v12 Completion
Report's `TwoRecordsTests`/`LocalizedChangeTests` claim.

Because `dependentChange = changes.FirstOrDefault(c => c.DependsOnMatchNumber == 93)` finds `ch-005`
(`EventDetails.razor:237`), the **"An entitlement depends on this match"** section
(`detail.dependencyHeading`) renders with its own `<ForeseeableBadge>` and the condition text —
correct, consistent with My Access, no contradiction on this particular page.

**Note, not a second finding:** match 93's own `detail__status` line (Amina holds no direct entry
for match 93 itself) correctly reads **"Not requested"** — `StatusFor`/`FoldStatus` for match 93
finds no change with `AffectsMatchNumber == 93`, so this page is unaffected by Attempt 1's bug. The
bug is specific to a match a *change is written against* (`AffectsMatchNumber`), not one a change
merely *depends on* (`DependsOnMatchNumber`).

**Curiosity check, surfacing the same finding a second, independent way.** A participant reading
"an entitlement depends on this match" might reasonably ask "what does my access to *that*
entitlement currently say?" and navigate to **`/events/98`** — the quarter-final itself — rather than
stop at the dependency. On that page, `status = FoldStatus(DateTime.MaxValue)` is used for the
headline (`EventDetails.razor:235`), which — unlike `venueStatus = FoldStatus(VenueListAsOf)` two
lines below it — imposes no upper bound on `EffectiveUtc` at all, so it folds in `ch-005` as though
already resolved. The page states, unconditionally: **"Access to this match has been withdrawn. See
your record for the reason."** (`detail.statusRevoked`). This is the same defect as Attempt 1's, one
level more assertive in wording, reached by a second, equally natural navigation path.

**Success criterion:** identifies the dependency fixture, states both outcomes without treating
either as settled. **Met**, on the path the flowchart actually names (`/events/93`). **PASS.**

---

## Attempt 3 — W1 · Task 3 · English · Light

**Route:** `/record`, signed in as Amina, simulating "no usable signal" — since the provider has no
live network dependency for reads (v12 Completion Report §PASS/FAIL check 7: reads return
already-completed tasks), this is read as-is from cache, which is the scenario itself.

**Walkthrough.** Headline renders with no network call, as designed. `StaleIndicator` states the
same 3h16m age as Attempt 1 — **the deliverable this task actually tests**
(`StaleIndicator.razor`'s own header comment: "the stale row is the deliverable"). Success requires
both halves — producing the state, and correctly reading its age — and both are on screen without
interaction.

No barrier disagreement is exercised in this attempt: Amina holds no change whose `EffectiveUtc`
falls inside the 24-hour `VenueListLag` window relative to `SimulatedNow` (`ch-004`, her most recent
already-effective change, landed 1 July — two days outside that window), so a `RunGateCheck` on any
of her matches would resolve `Agreement`, not the disagreement branch. This is stated rather than
staged: the task's success criterion doesn't require a disagreement to occur, only that she can
produce and correctly age her state, which she can.

**Success criterion:** produces current state offline, correctly reads its age. **Met. PASS.**

---

## Attempt 4 — W2 · Task 1 · Spanish · Light *(locale coverage requirement)*

**Route:** language switched to Spanish via `<LanguageSwitch />` in the nav (`NavMenu.razor:75`) —
confirmed session-preserving per `LanguageSwitchTests.TheSessionSURVIVESALanguageSwitch`
(`tests/FifaPressApp.Tests/LanguageSwitchTests.cs:140`) — then signed in as Tomás's *counterpart
scenario is not applicable here; W2 signs in with Amina's own credential is wrong per roster: W2 is
the freelance archetype, not a seeded account.* **Correction, stated rather than smoothed over:**
only two demo accounts exist — Amina and Tomás (`DemoAccountStore.cs:65-82`) — and W2 (freelance, no
seeded record) has no account to sign into. This attempt runs against **Amina's seeded data under
W2's lens** — reading the same record a freelance-track holder without a member association behind
her would face, per `01_TASK-PROTOCOL.md` §5's stated roster constraint — since no second
MemberAssociationQuota/Freelance record was seeded in v12. **Flagged here as a gap this dossier
cannot close**: a genuine W2 attempt would need a freelance-track demo record that does not exist in
the shipped build. Carried to `05_FINDINGS-REGISTER.md` as scope evidence, not silently substituted.

**Walkthrough, continued against Amina's record, Spanish locale.** `MyAccess.razor` at `/record`:
title and headline resolve to `L[Locale, "record.title"]` in Spanish. `StaleIndicator` reads
**"Actualizado por última vez hace 3 horas. Tu acceso puede haber cambiado desde entonces."**
(`stale.lastUpdatedStale` + `stale.hours` + `record.staleSubject`, `es.json`) — grammatically
correct Spanish pluralization, confirming `11_I18N.md` §4's locale-keyed field claim holds for this
string. `ch-005`'s `ChangeRow` opens to Spanish reason and next-step text verbatim from the seed
data's `Es:` fields (`MockAccessDataProvider.cs:637-660`) — e.g. next step: **"Espera a después de
ese partido para cerrar tu viaje a Dallas, o reserva algo que puedas cambiar."**

**The Attempt-1 finding reproduces identically in Spanish**, since it is a logic gap, not a copy
gap: the headline status word resolves to **"Acceso retirado"** (`status.revoked`, es.json) — the
same unconditional phrasing problem, now confirmed present in the second of three shipped locales.
Not treated as a new finding — the same `04-` candidate, additional evidence for its scope.

**Success criterion:** met via the same disclosure path as Attempt 1, in Spanish. **PASS**, same
finding carried forward, plus the roster-gap note above.

---

## Attempt 5 — W2 · Task 2 · English · Light

**Route:** same substitution as Attempt 4 (Amina's record, W2's reading lens), English, `/record` →
`/events/93`.

**Walkthrough.** Identical screen sequence to Attempt 2. Read under W2's constraint — no
institutional layer absorbing the shock of a contracting quota — the same information is present:
the dependency fixture, both outcomes, withholding intact. Nothing about the *shipped screens*
differs for a freelance-track reading; what would differ is data this build does not seed (e.g. a
refusal with no institutional next step, per §2's "the weakest appeal path" framing) — out of reach
for the same reason Attempt 4 flagged.

**Success criterion:** met. **PASS.**

---

## Attempt 6 — W2 · Task 3 · English · Dark theme *(theme coverage requirement)*

**Route:** theme switched via `<ThemeTrigger />` (`NavMenu.razor:78`), `/record`, same substitution
as Attempts 4–5.

**Walkthrough.** `MyAccess.razor` renders unchanged in structure under dark theme — theming is a CSS
concern layered beneath the component tree, confirmed by `ThemePaletteTests` existing as a distinct
suite from any `MyAccess`-specific test (`tests/FifaPressApp.Tests/ThemePaletteTests.cs`), i.e. no
component conditionally changes behavior by theme. `StaleIndicator` shows the same 3h16m age; its
`--old` CSS state is a color/weight change, not a structural one, so what a participant reads is
identical to Attempt 3's English/light content. **This attempt confirms coverage, not new content**
— the mandate's own reasoning for pairing Task 3 with theme rather than locale (nothing about
offline/stale-state rendering interacts with theme) holds up against the actual component.

**Whether the dark palette's contrast is adequate is explicitly out of scope here** — that is Gate
4's question, not Gate 2's; this attempt confirms only that the *task* is completable in dark
theme, not that it is comfortable to read.

**Success criterion:** met. **PASS.**

---

## Attempt summary

| # | Roster | Task | Locale | Theme | Result | Notable |
|---|---|---|---|---|---|---|
| 1 | W1 | 1 | EN | Light | **PASS** | Surfaces `04-`-candidate finding (headline/detail contradiction) |
| 2 | W1 | 2 | EN | Light | **PASS** | Finding reproduces a second way via `/events/98` |
| 3 | W1 | 3 | EN | Light | **PASS** | No disagreement branch available in seeded data (stated, not staged) |
| 4 | W2* | 1 | ES | Light | **PASS** | Finding reproduces in Spanish; roster-gap note (no freelance demo record) |
| 5 | W2* | 2 | EN | Light | **PASS** | — |
| 6 | W2* | 3 | EN | Dark | **PASS** | Confirms theme coverage; contrast judgment deferred to Gate 4 |

*W2 attempts read Amina's seeded record under W2's stated constraint, per the roster-gap note in
Attempt 4 — no second demo account exists in the shipped build. Six of six task-attempts pass their
Gate 1 success criteria. One finding, first observed in Attempt 1, reproduces in Attempts 2 and 4
and is the single most consequential thing this run's task-based testing surfaced — carried to
`05_FINDINGS-REGISTER.md` next.

---

## Tomás — unscored observation, not a seventh attempt

Per `01_TASK-PROTOCOL.md` §6. Signed in as Tomás (`RH-2026-00219` / `tomas-demo-2026`,
`DemoAccountStore.cs:77`), `/record`. The seeded `ch-008` — structurally identical to Amina's
`ch-005`: same `Kind` (`MatchAccessRevoked`), same affected match (98), same dependency (93) — is
classified `Foreseeable` by `Change.Classify` exactly as `ch-005` is (`Change.cs:305-323`), then
downgraded to `Silent` by `DeriveUrgency` because `Tomas.Track.NotificationCeiling ==
NotificationCeiling.ImmediateOnly` (`Track.cs:52-54`, driven by his `HasNamedContact = true`
in the seed). No `ForeseeableBadge` renders for `ch-008` on his `/record` — it's written to the
record, present in `visibleChanges`, but does not interrupt.

**Same underlying issue, same headline.** Tomás's `MatchAccess` block also shows match 98 as
**"Access withdrawn"** — `StatusFor` has no urgency awareness regardless of whose ceiling produced
it, so the Attempt-1 finding is not Amina-specific; it reproduces identically for the record where
the change is *Silent*, which arguably makes it worse for him — nothing on his own record's headline
distinguishes a silent, still-conditional entry from a decided one, and the one place that would
have told him (`ForeseeableBadge`) never fires for a Silent change by design.

One paragraph, as scoped. Not counted toward the six-attempt total. Carried to
`05_FINDINGS-REGISTER.md` as corroborating evidence for the same finding, not a second one.

---

## What this file hands to Gate 3 onward

- **Six of six task-attempts pass** their Gate 1 criteria.
- **One `04-`-scoped finding**, reproduced across three independent paths (My Access headline for
  Amina, Match detail for the entitlement itself, My Access headline for Amina in Spanish) plus
  Tomás's corroborating read — `MyAccess.StatusFor` and `EventDetails`'s `status` field both
  compute a match's access word from `Kind` alone, with no `Urgency` or `EffectiveUtc` check, so a
  still-conditional `Foreseeable`/`Silent` change renders as decided wherever a per-match status
  word appears, contradicting the same screen's own `ForeseeableBadge`/`ChangeRow` content for the
  identical change.
- **One scope gap, not a build defect**: no freelance-track (`TrackId.Freelance`) demo record exists
  to run a genuine W2 attempt against; both W2 attempts above substitute Amina's record under W2's
  reading lens, per `01_TASK-PROTOCOL.md`'s own roster note.
- **One naming-precision note for future dossier authors**: `04_TASKS-AND-SCENARIOS.md`'s
  flowcharts name "`GateCheckResult`" as though it were a discrete component; the shipped
  implementation is an inline conditional block inside `EventDetails.razor`
  (`detail__gate-disagree`), not a separately named component. Behaviorally equivalent, so not
  scored as a finding — recorded so Gate 5 doesn't need to rediscover it while tracing the
  flowchart against the source a second time.

---

✅ **GATE 2 COMPLETE** — `02_TASK-RESULTS.md`
