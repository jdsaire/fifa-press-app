# 02 — FIRST IDEATION (DIVERGENCE 1)

**SIMULATED — NOT EVIDENCE**

**Repo path:** `ux-ui/02-ideation/02_DIVERGENCE-1.md`
**Method:** Value Proposition Canvas, filled right to left, two teams of three (W1/W3/W5 and W2/W4/W6)
**Segment:** Amina R. only — `01-design-research/05_ARTIFACTS.md` §1.1
**Bound to:** the HMW and scope boundary in `00_SCOPE.md` §2
**Block:** Day 1, 60 minutes

---

## 1. Customer segment — Amina R.

Filled first, as the method requires. One segment only: mixing Amina and Tomás into one profile would produce a canvas that describes nobody.

### 1.1 Jobs — her concrete goals

| # | Job | Source |
|---|---|---|
| J1 | Be in the building for every match her national team plays | `05_ARTIFACTS.md` §1.1 |
| J2 | Get quotes in the mixed zone that nobody else has | `05_ARTIFACTS.md` §1.1 |
| J3 | Keep filing after her team is eliminated, because her editor still wants tournament coverage | `05_ARTIFACTS.md` §1.1 |
| J4 | Commit travel and money on a decision she can defend to her editor | `05_ARTIFACTS.md` §1.1 (budget booked before access confirmed) |
| J5 | Know what her credential permits before she is standing in front of someone enforcing it | Run 2 theme T3 |
| J6 | Re-plan fast when the tournament changes shape around her | Run 1 D5 #17; Run 2 theme T2 |

### 1.2 Pains

| # | Pain | Source |
|---|---|---|
| P-a | No visible status between submission and decision; she fills the gap by guessing or chasing | Run 2 code SILENT-WAIT; Run 1 D5 #5 |
| P-b | Outcomes arrive with no reason she can act on or learn from | Run 2 theme T1; Run 1 D5 #11 |
| P-c | Her access changes after confirmation and nothing precedes the change | Run 2 theme T2; Run 1 D5 #17 |
| P-d | She learns her status changed by attempting an action and failing | Run 2 code DISCOVERY-BY-FAILURE |
| P-e | Operationally necessary information reaches her from other journalists before any official channel | Run 2 code GRAPEVINE |
| P-f | What the badge actually permits is learned at the barrier | Run 2 theme T3; Run 1 D5 #2 |
| P-g | The gate is working from a list that disagrees with what she was sent | Run 2 code GATE-DESYNC; P05 |
| P-h | The digital layer fails exactly where it is needed — in venues, on roaming data | Run 2 code OFFLINE-FRAGILITY |
| P-i | Costs are already sunk when the change lands | `05_ARTIFACTS.md` §2 empathy map, §3.2 |
| P-j | Her key arrives from a federation at a pace she cannot influence | Run 1 D5 #4 |
| P-k | She has no named person to ask, at any point | `05_ARTIFACTS.md` §1.1 |
| P-l | Her visa class can make her accreditation useless without anyone flagging it | Run 1 D5 #1; `05_ARTIFACTS.md` §3.3 |
| P-m | Checking the app and learning nothing is worse than not checking | **[ASSUMPTION]** — extrapolated from her stated abandonment condition |

### 1.3 Gains

| # | Gain | Source |
|---|---|---|
| G1 | Knowing where she stands, even when the answer is bad | `05_ARTIFACTS.md` §2 |
| G2 | Enough warning to re-plan rather than react | `05_ARTIFACTS.md` §2 |
| G3 | A reason she can act on or learn from | `05_ARTIFACTS.md` §2; Run 2 T1 |
| G4 | One place that is more current than the group chat | `05_ARTIFACTS.md` §2 |
| G5 | Something she can show an editor to justify a cost or a change of plan | **[ASSUMPTION]** — implied by J4, not stated in any prior file |
| G6 | Confidence at the barrier instead of exposure | `05_ARTIFACTS.md` §2 ("confidence → possible humiliation") |

---

## 2. Value proposition side

### 2.1 Products and services

| # | Service | Description at service level, not interface |
|---|---|---|
| S1 | **Live accreditation state** | The credential is a state object that changes after approval, not a document issued once. |
| S2 | **Change log** | Append-only record of every transition, each carrying what changed, why, and what she can do next. |
| S3 | **Outbound messaging** | State changes are pushed to the holder before the change has consequences, scoped by track. |
| S4 | **Gate-side propagation** | The same state reaches venue access lists, as a second consumer of the same event. |
| S5 | **Anticipatory signalling** | Known-upcoming state risks (knockout elimination, single-entry visa against a cross-border fixture) are surfaced before they resolve. |
| S6 | **Standing status surface** | Something legible at any moment, including during the long silences and without signal. |

### 2.2 Pain relievers → pains answered

| Reliever | Answers |
|---|---|
| Every transition emits before it takes effect | P-c, P-d |
| Mandatory reason field on every negative or changed state | P-b, P-e |
| Mandatory next-step field, even when the next step is "nothing, and here is why" | P-b, P-k |
| The wait itself is given a state and an owner ("in vetting; FIFA is not deciding this") | P-a, P-k |
| One event stream, two consumers — holder and gate | P-g |
| Two-token meaning stated at the moment of approval | P-f |
| Last-known state readable offline | P-h |
| Visa class checked against itinerary shape and flagged when known | P-l |
| Change notice timed to arrive before money is committed where the trigger is predictable | P-i |
| MA key hand-off instrumented so undistributed keys are visible to FIFA | P-j *(partial — visibility only)* |

### 2.3 Gain creators → gains answered

| Creator | Answers |
|---|---|
| Status is always answerable, including "still nothing, here is why and until when" | G1 |
| Predictable triggers generate warnings ahead of the event, not reports after it | G2 |
| Reason granularity sufficient to act on: category and consequence, not the internal decision record | G3 |
| The official channel carries the change before the grapevine does | G4 |
| Every change produces a forwardable receipt with a timestamp | G5 |
| She arrives at the barrier already knowing what her credential permits today | G6 |

### 2.4 Pains with no reliever

Named rather than quietly dropped.

| Pain | Why no reliever | Status |
|---|---|---|
| P-l, the visa denial itself | Consular decision; boundary row "visas and entry permits" | **Partially relieved** — the incompatibility is flagged, the denial is untouched |
| P-j, the federation's actual pace | Boundary row "replacing the MA relationship" | **Partially relieved** — visibility, not control. Her key does not arrive faster |
| P-b at the vetting stage | National authorities give no reason to FIFA either; boundary row "security vetting outcomes" | **Unrelieved** — the platform can only say who is deciding and that it is not FIFA |
| Quota size itself | Boundary row "quota size and allocation politics" | **Unrelieved by design** |
| P-i, the sunk cost | Money is spent before any system can know the outcome | **Unrelieved** — warning shortens exposure, it does not remove it |

---

## 3. Fit assessment

**Essential pains addressed.** P-c and P-d — the change arriving before its own announcement — are the canvas's centre of gravity and are directly answerable. P-b, P-a and P-e follow from the same mechanic: if every transition carries reason and next step, silence stops being a state the applicant has to interpret. P-g is addressed by treating the gate as a consumer rather than an afterthought. P-f is addressed cheaply at the approval moment.

**Essential gains addressed.** G1 through G4 all fall out of the same underlying change: accreditation stops being a document and becomes a state that emits. G6 is a downstream effect of G1.

**Where the gaps are.** Every unrelieved pain in §2.4 sits on the far side of a scope-boundary row. This is the expected result and not a defect of the canvas — Run 1's most severe pains are legal and diplomatic, and the platform was explicitly forbidden from presenting itself as their solution.

**Are the gaps in scope to close?** No. Four of five are boundary rows and closing them would require authority FIFA does not hold. The fifth, sunk cost, is a timing problem no notification fully solves. The honest position is that this canvas relieves the pains caused by *not being told*, and leaves untouched the pains caused by the decisions themselves. That distinction is the concept's actual scope, and Gate 5 must not blur it.

**One thing the canvas exposed that the brief did not.** G5 — a forwardable record she can show an editor — appears nowhere in Run 1 or Run 2. It is tagged `[ASSUMPTION]` and carried forward as such. It is the only gain in this canvas with no evidence behind it.

---

## 4. Solution ideas as risk-sorted assumptions

**Sorting convention:** highest risk first. **R3** = if this assumption is wrong, the concept does not work. **R2** = if wrong, a component fails and can be replaced. **R1** = if wrong, a detail changes.

IDs are permanent and persist through every later gate.

| ID | Assumption | Risk | Serves | Tag |
|---|---|---|---|---|
| **ID-01** | We assume that being told earlier materially changes what Amina can do — that there is a window between the change and its consequence in which action is still possible. If the change is always simultaneous with its consequence, the entire concept is decoration. | **R3** | P-c, P-d, G2 | `[SIMULATED]` — no evidence establishes the window's size |
| **ID-02** | We assume that accreditation can be re-modelled as a live state object with an append-only change log, rather than a record written once at approval. | **R3** | P-c, S1, S2 | `[SOURCED]` — `06_DESIGN-BRIEF.md` §1 |
| **ID-03** | We assume that a state change can be made to reach the venue access list as reliably as it reaches the holder's phone, and that this is an organisational problem rather than an impossible one. | **R3** | P-g, S4 | `[SOURCED]` — P05 |
| **ID-04** | We assume that message volume can be scoped by track without the well-served tracks being under-informed and the unserved tracks being spammed — that Principle 3 is implementable and not just a stated intention. | **R3** | P-k, W5's veto | `[SOURCED]` — `04_ANALYSIS.md` T1 disconfirming case |
| **ID-05** | We assume that a reason can be attached to every negative state at a granularity that is useful to Amina without exposing FIFA's internal decision record or MA politics. | **R3** | P-b, G3 | `[SOURCED]` — Q7 item design, `02_INSTRUMENTS.md` |
| **ID-06** | We assume that elimination is a predictable trigger — that the system knows a knockout result will change quota entitlements and can warn before the recalculation lands, not after. | **R2** | P-c, P-i, G2 | `[SIMULATED]` — mechanism is `[INFERRED]` in Run 1 D5 #17 |
| **ID-07** | We assume that a "nothing has happened yet" state, with an owner and an expected window, is experienced as information rather than as an empty screen. | **R2** | P-a, P-m, G1 | `[SIMULATED]` — P01, P03 |
| **ID-08** | We assume that stating the two-token split at the moment of approval is enough to land it, where the same information in a PDF was not. | **R2** | P-f, J5 | `[ASSUMPTION]` — the novice-vs-persistent question is unresolved |
| **ID-09** | We assume that showing the current quota state and its changes is possible without being drawn into arguing about how the quota was set. | **R2** | P-b, boundary row | `[SOURCED]` — `06_DESIGN-BRIEF.md` §3 |
| **ID-10** | We assume that last-known state held on the device is more useful at a barrier than a live lookup that fails, and that a stale-but-labelled state is better than no state. | **R2** | P-h, G6 | `[SOURCED]` — P02 |
| **ID-11** | We assume that visa class and itinerary shape are both knowable to the platform early enough for an incompatibility flag to be actionable. | **R2** | P-l, boundary edge | `[SOURCED]` — `06_DESIGN-BRIEF.md` §3 |
| **ID-12** | We assume that the official channel can be made faster than the journalist group chat, and that being first is what displaces the grapevine. | **R2** | P-e, G4 | `[SIMULATED]` — P01, P03 |
| **ID-13** | We assume that messages can be tiered by consequence — interrupt now, tell her today, tell her before her next match — and that the tiering can be derived from the change type rather than configured by her. | **R2** | P-c, P-m | `[SOURCED]` — Q10 response spread |
| **ID-14** | We assume that a next-step field can be populated for every terminal state, including the ones where the honest next step is "nothing you can do, and here is who decided." | **R2** | P-b, P-k, G3 | `[SOURCED]` — `06_DESIGN-BRIEF.md` §2, Principle 2 |
| **ID-15** | We assume that instrumenting the MA key hand-off gives FIFA useful visibility even though it gives FIFA no control, and that visibility alone changes something. | **R2** | P-j | `[SOURCED]` — Run 1 recommendation 1 |
| **ID-16** | We assume Amina wants a forwardable, timestamped record of a change to justify a cost or a re-plan to her editor. | **R1** | G5, J4 | `[ASSUMPTION]` — no supporting evidence in Run 1 or Run 2 |
| **ID-17** | We assume that delivery timing should respect her local time and travel state, except where the consequence is imminent enough to override it. | **R1** | P-m, P-h | `[ASSUMPTION]` |
| **ID-18** | We assume that when the platform's state and the gate's list disagree, an escalation route reachable from the barrier is better than sending her to find her editor. | **R1** | P-g, G6 | `[SIMULATED]` — P05's twenty minutes |

**18 ideas.** ID-01 is the assumption the whole mandate rests on and the one with the least behind it: nothing in Run 1 or Run 2 establishes how much warning is enough, or whether any amount of warning would have saved Amina the flight.

---

✅ GATE 2 COMPLETE — `02_DIVERGENCE-1.md`
