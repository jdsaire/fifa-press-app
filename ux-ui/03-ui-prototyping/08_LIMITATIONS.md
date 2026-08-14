# 08 — LIMITATIONS

**SIMULATED — NOT EVIDENCE**

**Repo path:** `ux-ui/03-ui-prototyping/08_LIMITATIONS.md`
**Read this before citing anything in this folder as evidence of anything.**

---

## 1. The method actually used

A simulated card sort and a specification pass. One person, one chat session, no participants.

Six synthetic participants, W1–W6, carried from `02-ideation`. They were authored, not recruited. Their groupings, their disagreements, their label rejections and their objections were all written by the same person who then decided what the information architecture should be. Where they disagree, they disagree because the author staged a disagreement he already knew existed.

This mandate rests on two prior mandates that were themselves simulated. `01-design-research` ran a simulated research pass; `02-ideation` ran a simulated workshop. The evidence base underneath all three is real — FIFA-published sources, the AIPS protest, tournament documentation — but every layer built on top of it is authored. Errors in the research mandate propagate here undetected, because nothing in this mandate is positioned to catch them.

---

## 2. What must be stated plainly

- **No user testing informed any UI decision in this folder.** Not one.
- **The card sort had no participants.** Section 2 of `02_INFORMATION-ARCHITECTURE.md` describes an exercise that did not happen.
- **The premise is untested.** The concept assumes a usable interval exists between an entitlement change and its first consequence. Handoff recommendation 1 required testing this before building. It could not be executed — there are no real journalists to observe. If that interval is zero, the Access Record is a better-explained failure rather than a prevented one, and every screen state and threshold in this dossier is specified around a window that may not exist.
- **The prototype demonstrates a design position. It does not validate one.**
- **Every threshold value in `04_TASKS-AND-SCENARIOS.md` §2 is an assumption**, including the 72-hour boundary. It is inferred from a persona's travel constraints, not measured. `04-evaluation` cannot validate it either, because that mandate tests the build, not the interval.

This is the third and final reuse of the W1–W6 roster. A cast reused across research, ideation and prototyping cannot surprise the person who wrote it, and by the third pass its disagreements are load-bearing structure rather than findings.

---

## 3. Scope decisions and their rationale

| Decision | Rationale |
|---|---|
| **Inherited concept not reopened** | Access Record, its three interactions, its three principles and its boundary rows arrived fixed from `02-ideation`. Reopening them would have made this mandate a second ideation rather than a prototyping pass, and nothing new had been learned that would justify it |
| **No new personas** | Amina and Tomás carry the archetypes with the most severe documented pain. A third persona would have been invented to fit a screen, which is backwards |
| **Two to three tasks only** | Three, one per core interaction. Both floor and ceiling: fewer would ship an interaction untested, more would degrade measurement quality in `04-evaluation` through participant fatigue |
| **Specification, not code** | Run 4B writes the code. A dossier that shipped implementations would leave the build run nothing to do and would couple design decisions to one framework's idioms |
| **No bulk surface** | Tomás gets no task and no screen. Every task he would perform is a bulk task; v1 has none. He remains the constraint every decision must survive, not a user this version serves |
| **Benchmark set partly inspectable** | The most relevant screens in the tracking, airline and travel-authorization comparators sit behind bookings, shipment numbers and submitted applications. No accounts were created and none were submitted, so the authenticated status view — the screen a person reads when their state has changed — was never seen in any comparator |

---

## 4. Specific weaknesses in this dossier

**The roster was reconstructed before it was read.** `02-ideation/01_WORKSHOP-PROTOCOL.md` was unavailable when Gate 2 was first written; two participant roles were assigned incorrectly and corrected only when the file arrived. The corrected version stands, but the sequence is worth knowing: attributions in a simulated exercise are only as stable as the files describing it.

**One benchmark finding does more work than it should.** Delta's published notification policy supplied the materiality-threshold discipline, the suppression-list idea, and the no-outcome-message precedent. Three of the adopted patterns trace to one source, read as documentation rather than as an interface.

**No task exercises the only write path.** Request access is untested by design, because all three core interactions concern a change arriving rather than being made. If the request flow is broken, this dossier did not predict it.

**Interaction 4.3 is half-covered.** The holder-side half is testable. The barrier-side half is not, because venue access list ownership is unresolved upstream. Any claim that this dossier specifies enforcement end-to-end would be an overclaim.

---

## 5. What this legitimately supports

A traceable decision chain from evidence to interface. Every UI decision in `03_UI-DECISIONS.md` cites a rejected alternative and a source. Every screen in `05_SCREENS.md` traces to a task or carries a stated supporting justification. Every entity field in `06_DATA-MODEL.md` traces to a choreography rule or an inherited principle. A reader can disagree with any of it on stated grounds, which is the property a simulation can honestly deliver.

An explicit scope boundary, including the parts that hurt: visas unprevented, quota politics untouched, no appeal channel, no bulk capability, the tragic scenario still tragic.

A build brief a competent agent can execute without re-deriving anything, with acceptance criteria that are pass/fail rather than matters of taste.

Two defects found by inspection rather than assumption, both pre-existing: an inherited validation colour failing its contrast floor at 2.83:1, and a hardcoded `color-scheme: light only` that would have broken dark mode. Neither required a user to discover.

One hazard found by reading the data rather than its column list: the fixture CSV records a completed tournament with real team names in every knockout row, so a naive wiring would let the app see results before matches are played — dissolving the premise of the very interaction the data was admitted to serve. The containment rule in `06_DATA-MODEL.md` §1.1 exists because the file was opened.

---

## 6. Why this was the correct trade-off

The objective of this repository is to demonstrate development capability. The UX track exists to make the build defensible — to show that the interface was reasoned toward rather than assembled, and that the reasoning is inspectable. It is not a research portfolio and does not claim to be one.

Given that objective and a one-person, no-budget constraint, the alternative to a documented simulation was not a real study. It was no design record at all: screens chosen by taste, defended after the fact. A simulation that labels every synthetic input, tags every decision by provenance, and states what it cannot establish is more useful to a reviewer than an undocumented build, and more honest than a study of five convenience participants presented as evidence.

The discipline that makes this defensible is the labelling. `SIMULATED — NOT EVIDENCE` appears on every file containing authored participant output. Every insight carries a provenance tag. Every mocked value is named as mocked in `06_DATA-MODEL.md` §6, because a demo that quietly implies a live FIFA integration is a lie by interface — and so is a design process that quietly implies users were consulted.

---

## 7. Closing

This dossier contains no findings. It contains decisions, each traceable to a source, each stated with its rejected alternative, and each made without a single user having seen a screen. The distinction matters more than any individual choice recorded here: a design process that cannot say which of its inputs were observed and which were authored is not a rigorous process with gaps, it is an argument wearing the costume of a study. What this mandate can defend is the reasoning — why the record became the front door, why history is not a destination, why silence is published rather than assumed, why the data layer refuses to read a fixture that has not happened. What it cannot defend is that any of it is right, because nothing here was tested against a person who needed it. That question belongs to `04-evaluation`, and this folder has deliberately left it ungraded.

---

✅ GATE 8 COMPLETE — `08_LIMITATIONS.md`

**RUN 4A COMPLETE.** Nine files: `00_SCOPE.md` · `01_BENCHMARKING.md` · `02_INFORMATION-ARCHITECTURE.md` · `03_UI-DECISIONS.md` · `04_TASKS-AND-SCENARIOS.md` · `05_SCREENS.md` · `06_DATA-MODEL.md` · `07_BUILD-BRIEF.md` · `08_LIMITATIONS.md`. All destined for `ux-ui/03-ui-prototyping/`; Run 4A-D commits them.
