# Task-Based Usability Protocol

**Repo path:** `ux-ui/04-evaluation/01_TASK-PROTOCOL.md`
**Adapts:** `00-initial-evaluation/usability-test-protocol.md` — same discipline (a protocol is
written *before* results are looked at), reshaped from a free-form journey checklist into a
task-grounded one, per this mandate's whole premise (`00_SCOPE.md` §1)
**Tasks scored against:** `03-ui-prototyping/04_TASKS-AND-SCENARIOS.md` §3–§5 — **cited here, not
restated.** This file may not drift from that frozen source by becoming a second copy of it
**Bound by:** `00_SCOPE.md` §3 — the inherited contract; §4 — the premise stays untested; §7 — the
`00` baseline and its roll-forward disposition

---

## 1. How this protocol differs from `00`'s

`00` audited six free-standing journeys (browse, open, register, return, error, shell) with no
participant and no task narrative — a checklist run against source code. This mandate scores **six
task-attempts**: two frozen tasks × three roster members short one (W1 and W2 only, per
`00_SCOPE.md` §3 item 3), run against the shipped build rather than derived from it in the abstract.

The verification-method legend below is inherited unchanged, because the underlying constraint is
identical in both audits: no browser is available in this session, only source.

| Method | Meaning |
|---|---|
| **CODE-VERIFIED** | The source settles it outright — a route exists, a component renders a given state, a string is present |
| **REASONED** | The source determines the outcome but reaching it takes an inference — following a conditional branch, confirming a locale key resolves |
| **REQUIRES-HUMAN-CHECK** | Cannot be settled without a browser. Recorded as **OPEN**, never as passing |

An OPEN result is not a failure. This mandate inherits `00`'s honesty rule on this point exactly:
waiting on a person is not the same as failing a person.

---

## 2. The three tasks — cited, not restated

| Task | Interaction | Prompt source | Success criterion source | Failure modes source |
|---|---|---|---|---|
| **Task 1** — read what changed and what it means | 4.1 — a change lands | `04_TASKS-AND-SCENARIOS.md` §3 | §3, "Success" row | §3, "Failure modes worth catching" row |
| **Task 2** — find out what is about to change | 4.2 — a change becomes foreseeable | §4 | §4, "Success" row | §4, "Failure modes worth catching" row |
| **Task 3** — confirm your current state when you cannot reach the network | 4.3 — the state is enforced (holder-side half only) | §5 | §5, "Success" row | §5, "Failure modes worth catching" row |

**Task 3's scope note carries forward unedited.** §5 states plainly that this task tests one half
of Interaction 4.3 — the barrier-side half is untestable while venue access list ownership stays
unresolved (`00_SCOPE.md` §2, carried from v9). Scoring Task 3 as covering 4.3 end-to-end would be
overclaiming; this protocol does not do that.

No task is authored, reworded, or extended here. A prompt that has drifted from its shipped route
(§6 below) is scored as a finding against the build, not silently corrected in this file.

---

## 3. Pass/fail criteria, brought into one place

Restated from each task's own Success and Failure-modes rows — not re-derived — so Gate 2 has a
single table to score against instead of three separate files.

| Task | Passes when | Fails when |
|---|---|---|
| **Task 1** | States what changed, why, and remaining options — without facilitator help, without leaving My Access | Reads the outcome but can't find the reason; can't identify the most recent change; believes a foreseeable change has already happened |
| **Task 2** | Identifies the dependency fixture and states both outcomes without treating either as settled | Reads a conditional entry as decided; can't find the dependency; assumes the app promises the favourable outcome |
| **Task 3** | Produces current state offline **and** correctly reads how old that state is — both halves required | Can't distinguish cached from live data; the staleness indicator is present but goes unregistered; over-trusts hours-old state |

---

## 4. Coverage requirements — new to this run

`00`'s journeys had no locale and no theme to vary; this build has three locales and two themes, and
the mandate requires both be exercised by at least one attempt rather than asserted from the source
alone (`P-EVALUATION_FIFA_Run04-Scope.md` §4, Gate 1).

| Requirement | Where it lands | Why here |
|---|---|---|
| **At least one attempt in a non-English locale** | **W1 · Task 1 · Spanish (`es`)** | Task 1 already asks the participant to read a change's reason. Reading it in a second language exercises the exact seeded, locale-keyed content `11_I18N.md` §4 specifies — Amina's `ch-005` reason and next-step text are among the fields `ChangeTemplates` supplies as pre-authored `LocalizedText`, per the v12 Completion Report's decisions-resolved-autonomously §1 |
| **At least one attempt in dark theme** | **W2 · Task 3 · Dark theme** | Task 3 is offline/stale-state rendering — nothing about that interacts with locale, so pairing it with theme instead of a second locale attempt avoids needing a seventh combination this protocol has no task to hang it on |

Portuguese (`pt`) is not separately exercised by a task attempt in this run. That is a gap this
protocol states rather than hides: the coverage requirement is "at least one non-English locale,"
which Spanish satisfies, not "all three locales exercised by a task." Recorded here so Gate 5 does
not need to rediscover it as an omission.

---

## 5. Roster assignment — W1 and W2, six attempts

Per `00_SCOPE.md` §3 item 3: only W1 and W2 are marked as end users of a journalist-facing surface;
the roster is not expanded to reach a rounder number.

| Attempt | Roster | Task | Locale | Theme |
|---|---|---|---|---|
| 1 | **W1** (Amina archetype — mid-size national daily; access set by a federation she doesn't work for; elimination-driven quota contraction) | Task 1 | English | Light |
| 2 | **W1** | Task 2 | English | Light |
| 3 | **W1** | Task 3 | English | Light |
| 4 | **W2** (freelance archetype — no fixed outlet; a refusal is terminal and unexplained; the weakest appeal path) | Task 1 | **Spanish** | Light |
| 5 | **W2** | Task 2 | English | Light |
| 6 | **W2** | Task 3 | English | **Dark** |

Each roster member's relevant constraint, restated in one line per `P-EVALUATION_FIFA_Run04-Scope.md`
Gate 1's own requirement — so this file is legible without opening `02-ideation/01_WORKSHOP-PROTOCOL.md`:

- **W1** — quota-dependent, mid-size outlet, primary design target. Her constraint is that a
  federation she has no direct relationship with sets terms that change under her.
- **W2** — freelance, no federation behind her. Her constraint is that a refusal or a silent
  contraction has nowhere softer to land; there is no institutional layer absorbing the shock the
  way there might be for a staffer.

Six attempts, as scoped. No attempt is assigned to W3–W6 — W3/W4/W6 are non-journalist roles with no
sign-in reason, and W5's exclusion from tasking is itself part of the inherited contract
(`00_SCOPE.md` §3 item 2), not an oversight to correct here.

---

## 6. Tomás — an observation, not a seventh attempt

Carried from `00_SCOPE.md` §3 item 2 and `04_TASKS-AND-SCENARIOS.md` §1: no task exists for Tomás
and none is invented here. What 4E's two-record demonstration newly permits is a **scoped,
unscored** qualitative note, placed at Gate 2 rather than protocolized here as a graded check:

- Sign in as `Tomas` (the second demo record).
- Confirm that the seeded change interrupting Amina's `ch-005` resolves **Silent** for him via the
  same `Change.DeriveUrgency` / `Track.NotificationCeiling` logic — cite the specific change ID
  (`ch-008`, per the Completion Report's two-record demonstration).
- One paragraph. Evidence for `05_FINDINGS-REGISTER.md`, not a graded task-attempt, and not counted
  toward the six-attempt total anywhere in this dossier.

---

## 7. Route and screen verification against the frozen flowcharts

`00_SCOPE.md` §2 already checked that the record's move from `/` to `/record` doesn't by itself
invalidate a flowchart, since the flowcharts name screens rather than routes. This protocol makes
that check explicit per-task, since it's what Gate 2 will need to confirm before scoring any attempt:

| Task flowchart names | Current route | Verified present |
|---|---|---|
| Screen: My Access | `/record` (`MyAccess.razor`) | To confirm at Gate 2 |
| Screen: Help — what this does not do and escalation | `/help` (`Help.razor`) | To confirm at Gate 2 |
| Screen: Match detail | `/events/{Id:int}` (`EventDetails.razor`) | To confirm at Gate 2 |

This table is a pointer for Gate 2, not a result — `00_SCOPE.md` §2 already established the route
move alone isn't a finding; whether the *component-level* behaviour each flowchart node expects
(`StaleIndicator`, `ForeseeableBadge`, `ChangeRow`, `GateCheckResult`) actually exists on those
screens is what Gate 2 checks, attempt by attempt.

---

## 8. Scope boundary — inherited, not re-argued

Every check in this protocol sits inside what `09`–`12` actually authorized and 4E actually built.
Nothing here asks the app to do something the addenda excluded — venue-side arbitration, real
authentication, a backend. Those stay out by design, restated at `00_SCOPE.md` §3 item 7 (`09`–`12`
frozen) and §6 (`src/` read-only in this dossier), not oversights of this protocol.

---

✅ **GATE 1 COMPLETE** — `01_TASK-PROTOCOL.md`
