# 05 — CONCEPT

**SIMULATED — NOT EVIDENCE**

**Repo path:** `ux-ui/02-ideation/05_CONCEPT.md`
**Inputs:** the ranked shortlist in `04_CONVERGENCE.md` §4
**Block:** Day 2, convergent ideation — facilitator, W1, W4 (60 min) plus principle check (20 min)
**Boundary:** this gate produces concept, not interface. No screens appear below, and none should be inferred.

---

## 1. The concept

### Access Record

Today a journalist's accreditation is a decision that was made once. The Access Record makes it a record that keeps a history — one that keeps changing after approval, and says so each time it does.

Every change to what someone is allowed to do is written down as an entry: what changed, why it changed, and what they can do now. The entry is sent before the change starts to matter, not discovered afterwards when something stops working. The same entry goes to the people checking credentials at the venue, so the person at the barrier and the person being checked are reading the same thing.

How much someone hears depends on how much help they already have. A reporter with nobody to call gets told everything. A coordinator with a direct line and forty crew members gets told almost nothing.

*(148 words)*

**On the name.** *Access Record* names the object, not the remedy. It says the thing is a record of access that accumulates entries — which is the actual structural change — without asserting that notifying, alerting or messaging is the answer. A name like "AccessAlerts" would have decided the mechanism in the noun and closed a question this mandate has no right to close.

---

## 2. Composition

### 2.1 Absorbed

| ID | How it lives in the concept |
|---|---|
| **ID-02** | The concept *is* this. Accreditation as a live state object with an append-only entry log. Everything else is a property of it or a consumer of it. |
| **ID-05 + ID-14** | Every entry carries a reason and a next step. Not optional fields, not populated where convenient — an entry that cannot state both is a malformed entry. |
| **ID-03** | The entry has two consumers. The holder and the venue access list read from the same log, not from two systems that periodically agree. |
| **ID-04** | Entry *volume* is scoped by track. The log is complete for everyone; what is pushed out of it is not. This is the concept's veto rule. |
| **ID-13** | The urgency of an entry is derived from what changed, not configured by the person receiving it. She does not tune her own alerts; the change type decides whether it interrupts. |
| **ID-06** | Entries can be written for changes that are *about to* become possible, not only for changes that have happened. This is what makes the concept answer "before." |
| **ID-10** | The most recent entry is held on the device and readable without a network, carrying its own timestamp so a stale entry announces its staleness rather than impersonating a current one. |

### 2.2 Deliberately left out

| ID | Why it is not in the concept |
|---|---|
| **ID-09** — a quota-state surface | The part that matters to Amina — what changed in *her* entitlement and why — is already an entry. A standing surface showing quota state as a thing to look at invites the next question, which is why the quota is that size, and that is the boundary row the concept must not walk toward. Absorbed as entry content; rejected as a destination. |
| **ID-11** — visa incompatibility flag | Sourced, in scope, and already committed to by `06_DESIGN-BRIEF.md`. It is a different trigger riding the same substrate, and folding it into the concept's definition would make the concept look like it addresses entry rights. It is built on the Access Record; it is not part of what the Access Record *is*. |
| **ID-25** — a named human for every applicant | Not rejected on merit — it is probably the better answer, and it is the reason the rights-holder track works. It is an operations budget, not a product decision, and carries to `06_HANDOFF.md` as a recommendation rather than a component. |

---

## 3. Concept against the inherited principles

| Principle | How the concept satisfies it | What the concept must never do |
|---|---|---|
| **1. Every state change announces itself** | A change and its entry are the same operation. There is no path where entitlements move without an entry being written, because writing the entry is how the move is recorded. Predictable changes can be announced before they resolve. | Never let a change be committed by any route that bypasses the log. The moment there is a second way to alter entitlements — an admin override, a bulk import, a manual correction — Principle 1 is dead and the concept is a reporting layer on an unreliable source. |
| **2. A refusal is a message, not a wall** | Reason and next step are structural requirements of an entry, not editorial additions. Where the honest next step is "nothing you can do," the entry says that and names who decided. Following the surviving fragment of ID-21: an entry states what remains possible, not only what was taken. | Never write an entry that states an outcome alone. Never let "reason" degrade into a code, a status label, or a sentence that restates the outcome in other words. If the reason field can be satisfied by "quota", it is not a reason field. |
| **3. Serve the people who have no one to call** | Push volume is inversely scoped to a track's human support. The unserved track receives every entry that affects them; the well-supported track receives near-nothing and reads the log when it wants to. | Never broadcast one entry set to every track. Never add per-person interaction to a coordinator managing crews in bulk — W5 funded the substrate only on this condition, and Tomás abandons the tool and keeps the spreadsheet if it is broken. |

---

## 4. Three core interactions, as behaviour

No screens. Each is described as trigger, system behaviour, what the person receives, and what happens when it fails.

### 4.1 A change lands

**Trigger.** Something recalculates entitlements after approval — a team is eliminated and the Member Association's quota contracts, a position is reassigned, an allocation is revised.

**System behaviour.** The change is written as an entry before it takes effect on any request path. The entry carries what changed, why, what remains available, and what she can do now. Urgency is derived from the change type. The entry is pushed to the holder if her track's scoping calls for it, and to the venue access list regardless.

**What she receives.** A statement that her access has changed, arriving before the first thing she tries to do stops working — including, where anything remains open to her, what that is.

**When it fails.** If the push does not reach her, the entry still exists and is still the newest thing in her record; the system's failure mode is a late message, not a missing fact. If the entry cannot be written, the change must not take effect — this is the one place the concept insists on blocking, because a change that takes effect unlogged is precisely the failure the whole thing exists to remove.

### 4.2 A change becomes foreseeable

**Trigger.** A fixture whose result will move entitlements is approaching — the knockout match her team is playing.

**System behaviour.** The dependency is identified in advance and an entry is written about a change that has not happened yet, stating the condition rather than asserting an outcome.

**What she receives.** Notice that her access depends on a result, what it becomes under each outcome, and by when she would know — early enough that a booking decision is still a decision.

**When it fails.** If the dependency is not detected, the interaction degrades to 4.1: she learns after rather than before. That is the current state of the world, so failure here costs the concept its advantage but does not make anything worse. If the forecast is wrong, the concept has told her something false about her own access, which is worse than silence — so a foreseeable-change entry must be conditional in its wording and must never read as a commitment.

### 4.3 The state is enforced

**Trigger.** She presents her credential at a venue.

**System behaviour.** The access list has been reading the same entry log as she has. Her device holds the most recent entry it has seen, timestamped, and can present it without a network.

**What she receives.** Either an uneventful entry — which is the goal, and looks like nothing happening — or, when the two disagree, a visible timestamp on both sides and a route to escalate that does not require her to phone an editor who phones somebody else.

**When it fails.** If her device holds a stale entry, the timestamp says so; a stale-but-labelled state is recoverable, a stale state impersonating a current one is not. If the access list is behind, the disagreement is between two timestamps rather than between a person and an official, which is the difference between a discrepancy and an argument. Neither mechanism guarantees she gets in — see §5.

---

## 5. Boundary restatement — where she will expect more than this does

Three edges where a person will reasonably assume the Access Record solves something it does not.

| Edge | What she will expect | What the concept actually does |
|---|---|---|
| **The change itself** | That being told earlier means the reallocation can be contested, delayed or reversed. It cannot. The quota moved for reasons the platform is forbidden to arbitrate. | States the change, its reason, and what remains open. The concept improves the timing and the legibility of a loss; it does not prevent the loss. If it ever reads as an appeal channel, it has misrepresented itself. |
| **Entry to the country** | That a valid, current, well-explained record means she can get to the stadium. The most severe failure in the evidence base happened to correctly accredited journalists at a border. | Carries the visa incompatibility flag (ID-11) as a separate trigger, names the deciding authority, and stops. The record will show her approved and current while she is unable to enter, and the concept must not obscure that contradiction — the tragic scenario in `05_ARTIFACTS.md` §3.3 remains unprevented. |
| **The silence before a decision** | That a record which explains every change also explains the weeks of nothing before her first answer, and the security vetting she cannot see into. | Nothing. ID-07 was excluded at Gate 4 as a different question, and vetting is a boundary row. The concept begins at approval. A reader who expects it to fix the five-week wait has been misled about its scope, and Gate 6 must say so plainly. |

---

## 6. Open questions carried into `03-ui-prototyping`

| # | Question | Why it is unresolved here |
|---|---|---|
| 1 | **Does the window exist?** ID-01: is there usable time between a change and its consequence? | The concept's premise. Nothing in Run 1 or Run 2 establishes the interval; Run 1 D5 #17 is `[INFERRED]`. If the interval is zero, the Access Record is a better-explained failure, not a prevented one. Test this before anything else. |
| 2 | **Is ID-03 organisationally possible?** | Matrix B identified it as difficult for reasons that are not technical. Venue access lists are owned by host-city operations. Prototyping cannot resolve an inter-organisational boundary, but it must not design as though it were resolved. |
| 3 | **Is the scoping rule right, or merely safe?** ID-04 | Derived from one simulated participant's objection (P04/W5). Inverse scoping might under-serve a well-supported track's individuals, who are not their coordinator. |
| 4 | **The two-token unknown, still unresolved.** | Carried from `04_ANALYSIS.md` T3 and untouched by this mandate, per instruction. No part of the Access Record rests on it. |
| 5 | **What makes a reason sufficient?** | Principle 2 forbids a reason that restates the outcome, but the granularity that satisfies Amina without exposing MA politics is undetermined. `02_INSTRUMENTS.md` Q7 offered four levels and never asked anyone real. |
| 6 | **Is the forwardable record wanted?** ID-16 | Dropped from the shortlist and tagged `[ASSUMPTION]` throughout. It has no evidence behind it and is recorded here so its absence is a decision rather than an oversight. |

---

✅ GATE 5 COMPLETE — `05_CONCEPT.md`
