# 06 — DESIGN BRIEF

**Repo path:** `/Design Research/06_DESIGN-BRIEF.md`
**Traces to:** `04_ANALYSIS.md` themes T1–T3; `05_ARTIFACTS.md` blueprint stage 10.
**Confidence:** design direction is **[SOURCED]** to Run 1 pain points; all magnitudes and preferences behind it are **[SIMULATED]**.

---

## 1. Two framings of one problem

### For the builder

Accreditation is currently modelled as a terminal artifact: an application is approved, a record is written, and the system considers the transaction closed. It is not closed. The record continues to change after approval — quota recalculates when a team is eliminated, positions are reassigned, entitlements contract — and none of those changes emit anything. The applicant's first contact with a change is a failed action: a ticket request that won't submit, a barrier that won't open.

The work is to convert accreditation from a stored outcome into a live state object with an append-only change log, where every transition carries three mandatory fields: what changed, why, and what the holder can do next. Transitions must emit to two consumers, not one — the holder and the venue access list, which currently runs on a stale snapshot (P05: an update issued two hours before kickoff had not reached the gate).

Notification scope is a hard requirement, not a setting: tracks with a named human contact receive fewer messages, not more (P04). Open question carried forward: whether the accreditation-versus-ticket confusion is a novice gap or a persistent one — Gate 4 could not resolve it.

*(200 words)*

### For the stakeholder

> ## How might we tell a journalist that their access has changed before they discover it by being refused?

**Why this one.** It traces directly to T2, the only theme where FIFA both causes the failure and controls the fix (blueprint stage 10). It is narrow enough to design against — the unit of work is a state transition and its message — and it does not smuggle a solution: nothing in it names a channel, a screen, or a technology.

---

## 2. Design principles

| # | Principle | This means we will not… |
|---|---|---|
| **1** | **Every state change announces itself.** If the record changes, the holder is told, before the change has consequences. | …ship any transition that a user can only discover by attempting an action and failing. Silent recalculation is a defect, not a background process. |
| **2** | **A refusal is a message, not a wall.** Every terminal or negative state carries a reason and a named next step, even when the next step is "nothing, and here is why." | …ship an outcome screen containing only an outcome. No "your application was not successful" as a complete message. |
| **3** | **Serve the people who have no one to call.** Message volume is inversely scoped to how much human support a track already has. | …broadcast the same notification set to every track. We will not add clicks or alerts to coordinators managing crews in bulk, who will abandon the tool for a spreadsheet if we do. |

---

## 3. Scope boundary — what this app does not do

The most severe failures in Run 1 are legal and diplomatic. The platform must not present itself as solving them.

| Out of scope | Why | What the platform does at that edge instead |
|---|---|---|
| **Visas and entry permits** | Consular decisions belong to host states. The 2026 denials (AIPS, June 2026) happened to correctly accredited journalists. | Records visa class against the itinerary and flags a known incompatibility — single-entry against cross-border fixtures — at the moment the class is known. |
| **Security vetting outcomes** | National authorities decide; FIFA cannot overturn or explain their reasoning. | Shows that vetting is in progress and that FIFA is not the decision-maker, rather than showing nothing for weeks. |
| **Quota size and allocation politics** | Who gets how many places is a governance question between FIFA and Member Associations. | Makes the *current* quota state and its changes legible to the person affected. It does not argue about fairness. |
| **Replacing the Member Association relationship** | Key distribution is federation-controlled and outside FIFA's authority. | Instruments the hand-off so FIFA can see undistributed keys — visibility, not control. |
| **Guaranteeing access** | Accreditation never guaranteed match access, and won't. | Makes the two-token split explicit at approval rather than at the barrier. |

---

## 4. Success metrics

All three are **baseline-free**: no measurement of the 2026 incumbent exists (see `00_SCOPE.md` — usability testing of the incumbent was not run). Each requires a baseline captured at pilot before it can be read as improvement.

| Metric | Definition | Instrumentation source |
|---|---|---|
| **Discovery-by-failure rate** | Share of access changes where the holder's first interaction with the change is a failed action rather than a received message. Target: falling toward zero. | Server-side join of change-event timestamp, notification-delivery timestamp, and the holder's next action. Directly measures Principle 1. |
| **Gate-desync interval** | Median elapsed time between a status change being committed and the venue access list reflecting it. | Diff between accreditation state store and the venue list sync job, sampled per matchday. Measures the P05 failure. |
| **Outcome-message completeness** | Share of terminal and negative states delivered with a populated reason field and a next-step field. Target: 100% — this is a build-quality gate, not a behavioural metric. | Schema validation on the transition log. Measures Principle 2. |

---

## 5. Handoff to the Claude Code phase

**Read, in this order:**
1. `06_DESIGN-BRIEF.md` (this file) — the HMW, the principles, and the boundary.
2. `05_ARTIFACTS.md` §4, the service blueprint — specifically stage 10 and the zero-visibility summary. This is the spec surface.
3. `05_ARTIFACTS.md` §1, both personas — build for Amina, but every notification decision must survive Tomás.
4. `04_ANALYSIS.md` §2.2, themes T1–T3 only.
5. `00_SCOPE.md` — before claiming the build is evidence-based, read what this study can and cannot establish.

**Ignore, and do not cite as justification:**
- Every number in `04_ANALYSIS.md` §1. The p-values, the UMUX-Lite means, the 80% and the 8/30 are properties of an authored file. They validated the pipeline and nothing else.
- `survey_master.csv` and `interviews_master.json` as evidence of anything. They are provenance for the personas and themes, not findings.
- Run 1's `[INFERRED]` and `[REPORTED]` rows as settled fact when a build decision turns on them.

**Carry forward as unresolved:** whether the two-token confusion is a novice gap or persistent (Gate 4, T3). Build the approval-moment disclosure either way — it is cheap and harmless if the confusion is novice-only — but do not let a larger IA restructure rest on it.

---

✅ GATE 6 COMPLETE — `06_DESIGN-BRIEF.md`
