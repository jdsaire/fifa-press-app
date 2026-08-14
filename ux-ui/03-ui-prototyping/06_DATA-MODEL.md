# 06 — DATA AND CONTEXT MODEL

**Repo path:** `ux-ui/03-ui-prototyping/06_DATA-MODEL.md`
**Inputs:** `2026_World_Cup_Schedule.csv` read directly — 104 rows, columns verified below; `05_SCREENS.md` state matrices; `02_INFORMATION-ARCHITECTURE.md` CH-1…CH-10; `00_SCOPE.md` §6 CSV reversal.
**This gate specifies structure as design. Run 4B writes the C#.**

---

## 1. The CSV, as it actually is

Read from the file, not assumed. Columns: `Match`, `Date`, `Time (ET)`, `Time (Local)`, `Matchup`, `Group / Phase`, `Venue`, `City`. 104 rows. Dates run `11-Jun-26` to `19-Jul-26` in `d-MMM-yy`. 16 venues across 16 cities.

`Group / Phase` values, verified: `Group A`–`Group L` (12 groups × 6 matches = 72), `Round of 32` (16), `Round of 16` (8), `Quarter-finals` (4), `Semi-finals` (2), `Third Place` (1), `Final` (1).

### 1.1 The hazard this gate exists to contain

**Every knockout row names two real teams. There are no placeholders** — not one `Winner Group A`, not one `W73`. Verified across all 104 rows.

This means the file is a record of a **completed** tournament, and results are encoded implicitly: a team appearing in the Round of 16 *is* the outcome of its Round of 32 match. Traced directly — Spain appears at Group H → Round of 32 (84) → Round of 16 (93) → Quarter-finals (98) → Semi-finals (101) → Final (104). Reading the file forward tells you who won every match before it is played.

**The consequence for Interaction 4.2 is severe.** That interaction requires a fixture whose outcome is *not yet known* — a foreseeable change is conditional precisely because the condition is unresolved. A naive wiring of this CSV produces an app that can see the future: it would know Amina's team is eliminated before the match, which is not a better product, it is a broken premise. It would also silently invert the mandate — a system that knows outcomes in advance has no need to warn anyone about anything.

**Containment, specified as a hard rule:** the data layer holds a **simulated tournament instant** (`AsOfUtc`), and **no read may consult any row whose kickoff is later than `AsOfUtc`.** Rounds after that instant are known only as *scheduled fixtures*; their `Matchup` values are never read. This is not a display convention — it is a data-access constraint, because a display convention can be forgotten by the next person to add a query.

There are no result or score columns. Elimination is therefore never *read*; it is *derived*, and only from rows at or before `AsOfUtc`.

---

## 2. Entity model

Field-level, framed as design. Types are indicative.

### 2.1 `Fixture`

| Field | Type | Source | Notes |
|---|---|---|---|
| `MatchNumber` | int | CSV `Match` | Natural key, 1–104 |
| `KickoffLocal` | DateTime | CSV `Date` + `Time (Local)` | Parsed from `d-MMM-yy` + `HH:mm` |
| `KickoffEastern` | DateTime | CSV `Date` + `Time (ET)` | Retained; the file's only cross-venue common clock |
| `TimeZoneLabel` | string | **Mocked** — derived from `City` | **The CSV carries no zone identifier.** Gate 1 adopt #10 makes a zone label mandatory across three host countries, so this is a lookup the app supplies, not data it receives. Labelled mocked wherever displayed |
| `Phase` | enum | CSV `Group / Phase` | The 18 verified values, mapped to `GroupStage` + group letter, or a knockout round |
| `HomeLabel` / `AwayLabel` | string | CSV `Matchup`, split on ` v ` | **Only readable when `KickoffLocal <= AsOfUtc`.** See §1.1 |
| `Venue` | string | CSV `Venue` | |
| `City` | string | CSV `City` | |
| `IsResolved` | bool | Derived | `KickoffLocal <= AsOfUtc` |

### 2.2 `Accreditation`

| Field | Type | Notes |
|---|---|---|
| `CredentialId` | string | The holder-facing identifier; also the read key (Gate 1 adopt #7) |
| `HolderName` | string | Mocked |
| `Outlet` | string | Mocked |
| `Track` | `Track` | See §2.4 |
| `Status` | enum | `Pending`, `Approved`, `Refused`, `Withdrawn` |
| `ValidUntil` | DateTime? | Gate 2 §4.1 — approval is "approved *until*", never bare |
| `ZoneAccess` | string[] | Zones the standing credential permits |
| `LastSyncedUtc` | DateTime | **Drives every StaleIndicator (CH-8).** Not optional |

### 2.3 `Change` — the log record

Named `Change`, not `Entry`, per Gate 2 §4.1 and the ES/PT collision in §5 of that gate.

| Field | Type | Notes |
|---|---|---|
| `ChangeId` | string | |
| `CredentialId` | string | Owner |
| `WrittenUtc` | DateTime | When recorded |
| `EffectiveUtc` | DateTime | **When it starts to matter.** Ordering key for CH-2 — not `WrittenUtc` |
| `Kind` | enum | `MatchAccessGranted`, `MatchAccessRevoked`, `ZoneAccessNarrowed`, `ZoneAccessWidened`, `ValidityShortened`, `RequestDecided`, `AdministrativeCorrection`, `Withdrawal` |
| `Urgency` | enum | `Immediate`, `Foreseeable`, `Silent` — **derived from `Kind` + `EffectiveUtc` + `Track`, never stored as a user preference** (CH-4) |
| `WhatChanged` | string | Required |
| `Reason` | string | **Required. A value that restates the outcome is invalid** (Principle 2) |
| `NextStep` | string | Required; may state that nothing is actionable, but may not be empty (CH-9) |
| `DecidedBy` | string | Required when `NextStep` is non-actionable (CH-9) |
| `SupersedesChangeId` | string? | Enables CH-3's revised-against-original display |
| `DependsOnMatchNumber` | int? | Set for `Foreseeable`; the fixture the condition hangs on |
| `ConditionText` | string? | Required when `DependsOnMatchNumber` is set; worded as a condition, never a commitment (CH-7) |

**Schema-level validation, per `06_HANDOFF.md` rec 3:** a `Change` missing `WhatChanged`, `Reason`, or `NextStep` is **malformed and must fail at construction**, not render as a blank field. This is the mandate's only 100%-target success metric and it is checkable at build time.

**Append-only.** No update, no delete. CH-1 permits no second write path; a correction is a new `Change` that supersedes, and a withdrawal is a `Change` of kind `Withdrawal` — which resolves the open question below.

### 2.4 `Track`

| Field | Type | Notes |
|---|---|---|
| `TrackId` | enum | `MemberAssociationQuota`, `RightsHolder`, `Freelance` — the three in Run 1 D4; no new archetypes |
| `HasNamedContact` | bool | The scoping input for Principle 3 |
| `NotificationCeiling` | enum | Derived from `HasNamedContact`; a **precondition, not a setting** (`06_HANDOFF.md` rec 5) |

---

## 3. Resolved: withdrawal, carried from Gates 3–5

Gate 3 §7 flagged the missing cancel path; Gate 4 §7 established it was a data question; Gate 5 §5.2 deferred it here.

**Resolution: withdrawal is a `Change` of kind `Withdrawal`, not a deletion and not a form action that unwrites anything.** A person withdrawing a request produces a new log record like every other state movement, with a reason and a next step. This satisfies CH-1 without inventing a second write path, and it means Request access **may** carry a cancel affordance in a later version — but it is a write, not an undo, and Gate 5's spec correctly describes no undo. `[SOURCED]` from CH-1.

---

## 4. Fixture → entitlement dependency

What makes Interaction 4.2 buildable, and why the CSV is admissible (`00_SCOPE.md` §6).

### 4.1 The mapping

A `Change` with `Urgency = Foreseeable` carries `DependsOnMatchNumber`. The dependency is evaluated against `AsOfUtc`:

| Condition | Behaviour |
|---|---|
| Fixture kickoff **after** `AsOfUtc` | Unresolved. The `Change` stays conditional; `ConditionText` names both outcomes; `Matchup` for that row is **not read** |
| Fixture kickoff **at or before** `AsOfUtc` | Resolved. A landed `Change` is written that **supersedes** the conditional one via `SupersedesChangeId`. The conditional record is **not deleted** (CH-7) |

### 4.2 Deriving elimination without reading the future

Since there are no result columns, and forward rows leak outcomes, elimination is derived from one rule only:

> A team is treated as eliminated when the next round's fixtures have kicked off (are at or before `AsOfUtc`) and that team does not appear in any of them.

This consults only resolved rows. It is a **mocked inference**, not a result feed — a real system would receive match outcomes from a results service, not deduce them from a schedule's shape. Labelled as mocked wherever it drives a displayed change.

### 4.3 Worked example, using real rows

Amina's track depends on her national team. Take `AsOfUtc` set between Match 84 (Spain v Austria, Round of 32, 2 Jul) and Match 93 (Round of 16).

- Match 84 is resolved. Its `Matchup` is readable.
- Round of 16 fixtures are **after** `AsOfUtc`. Their `Matchup` values are **not read** — the app does not know Spain advances, even though the file says so.
- A `Foreseeable` change exists: `DependsOnMatchNumber = 93`, `ConditionText` naming both outcomes for her quota.
- Once `AsOfUtc` passes Match 93's kickoff, the derived rule applies and a landed change supersedes the conditional one.

This reproduces `05_ARTIFACTS.md` §3.2 — the critical scenario — **with warning instead of silence**, which is the whole point of the mandate.

---

## 5. Service abstraction — written knowing Run 4C exists

The data-access layer sits behind an interface from the start, so 4B's in-memory provider and 4C's HTTP provider are a swap rather than a rewrite. This is what makes the before/after comparison clean instead of a refactor.

### 5.1 The contract

| Operation | Returns | Notes |
|---|---|---|
| `GetAccreditation(credentialId)` | `Accreditation` | Includes `LastSyncedUtc` |
| `GetChanges(credentialId)` | `Change[]` | Ordered by `EffectiveUtc` descending (CH-2) |
| `GetFixtures()` | `Fixture[]` | `Matchup` withheld on unresolved rows |
| `GetFixture(matchNumber)` | `Fixture` | Same withholding rule |
| `RequestMatchAccess(credentialId, matchNumber)` | `Change` | The write path. Returns the resulting `Change`, never a bare success flag — CH-1 means the change *is* the outcome |
| `WithdrawRequest(credentialId, changeId)` | `Change` | Per §3 |

### 5.2 Rules the interface must carry, not the caller

- **Every read returns its own freshness.** A response carries `LastSyncedUtc`; the caller never guesses. This is what makes CH-8 implementable rather than aspirational.
- **Cache-first, network-second.** Reads resolve from local state before any fetch, per Gate 5 §2.2's loading rule and CH-10. The interface exposes cached-versus-fetched as a property of the response, not as two different methods.
- **Writes return the resulting `Change`.** A write that returned `bool` would permit a state movement with no log record — exactly the second write path CH-1 forbids.
- **No method exposes a fixture's `Matchup` for an unresolved row.** The withholding lives in the provider, so a future caller cannot bypass it by accident.

### 5.3 What 4C would change

Only the provider implementation. The interface, the entities, the ordering rules, and the withholding rule are all provider-agnostic. 4C additionally replaces `AsOfUtc` — a simulation device — with real time, at which point the withholding rule stops being a containment measure and becomes simply true.

---

## 6. Mocked versus real — every value labelled

A demo that quietly implies a live FIFA integration is a lie by interface.

| Data | Status in 4B | What a real system would do |
|---|---|---|
| Fixture schedule | **Real data**, from the published CSV | Same, from a fixtures service |
| `TimeZoneLabel` | **Mocked** — city→zone lookup the app supplies | Carried on the fixture record |
| Match outcomes / elimination | **Mocked inference** (§4.2) | Received from a results feed |
| `AsOfUtc` | **Simulation device** | Real clock |
| Accreditation record, holder, outlet | **Mocked** | FIFA Media Hub |
| All `Change` records | **Mocked** | Emitted by the accreditation system at the moment state moves |
| Track and `HasNamedContact` | **Mocked** | Accreditation system |
| Quota figures | **Absent, deliberately** | Quota Management System — and out of scope regardless (`05_CONCEPT.md` §5) |
| Venue access list / gate result | **Mocked, and ownership unresolved** | Unsettled — `06_HANDOFF.md` rec 4 |
| Credentials at Sign in | **None exist** | Out of scope entirely (Gate 5 §6.1) |

**Surface requirement.** Wherever mocked data drives something a reader could mistake for a live integration — a change record, a gate result, an elimination-derived warning — the interface says so on screen, in the same spirit as the login form's simulation notice (Gate 5 §6.2).

---

## 7. Carried into Gate 7

| # | Item | Owner |
|---|---|---|
| 1 | `AsOfUtc` withholding rule is the highest-risk thing to implement wrongly; needs an explicit acceptance-criteria line | Gate 7 |
| 2 | `Change` construction-time validation of the three required fields is build-checkable | Gate 7 acceptance criteria |
| 3 | CSV parsing of `d-MMM-yy` and the ` v ` split are the two fragile parse points | Gate 7 |
| 4 | Withdrawal is now specified but has no screen affordance — Gate 5 stands unchanged; a later version may add one | v2 |
| 5 | Venue access list ownership still unresolved; gate result stays mocked and labelled | Blocked, `06_HANDOFF.md` rec 4 |

---

✅ GATE 6 COMPLETE — `06_DATA-MODEL.md`
