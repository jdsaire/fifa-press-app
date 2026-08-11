# 00 — SCOPE LOCK

**Repo path:** `/Design Research/00_SCOPE.md`
**Study:** Simulated guerrilla research pass — FIFA media accreditation platform, 2030 target
**Evidence base:** `RUN1_EVIDENCE-BASE.md` (secondary research, 2010–2026)
**Status:** Scope frozen at Gate 0. Later gates operate inside this boundary and do not renegotiate it.

---

## 1. What this study can and cannot establish

This study reasons from one sourced evidence base and one set of synthetic responses generated from it. No human participant is involved at any point.

**It can establish:** a defensible design direction for the accreditation platform; a coherent information-architecture argument; a mapped set of failure modes across the accreditation-to-matchday lifecycle, including the backstage steps where the applicant sees nothing; and an auditable chain from sourced pain point to design decision.

**It cannot establish:** anything empirical about real journalists. No percentage in this repo describes a population. No persona is validated. No usability figure is a benchmark. No hypothesis is genuinely tested — hypotheses are stated so the analysis has something to fail against, and simulated data cannot falsify anything, because its shape was chosen by the person who wrote it.

Where a fact was unknown at the end of Run 1, this study does not discover it. It states the requirement a designer would carry into the room where that fact gets settled.

*(198 words)*

---

## 1a. Relationship to `07_LIMITATIONS.md`

These two files are not duplicates and must not be read as such.

- **This file draws the boundary, before the work.** What is in, what is out, which gaps are targeted, what the excluded program would have delivered.
- **`07_LIMITATIONS.md` defends the boundary, after the work.** Method disclosure, rationale for each trade-off, and what a professional reviewer should and should not conclude from the finished repo.

Scope statements belong here. Justification and methodological self-assessment belong there.

---

## 2. Preserved maximalist scope — the program not being run

Documentation only. No remediation plan, no phased proposal, no future-work section. This table exists so a repo reader can see the full-scale study this project deliberately did not run, and judge the omission knowingly.

**Severity:** 1 = design proceeds with minor assumption risk · 2 = a design decision rests on inference · 3 = a decision cannot be validated at all before build.

| Study not run | What it would have established | Consequence of the gap | Sev |
|---|---|---|---|
| Recruitment of real accredited media across six confederations | Actual needs, priorities and workarounds of the user population | Every finding in this repo remains inference; zero external validity | 3 |
| Contextual inquiry at accreditation collection points | Real pickup, queue and passport-validation behaviour | Collection-point flow is designed blind (Run 1 could not source it either) | 3 |
| Matchday diary study across a full tournament | Real rhythm of ticket requests, inter-city travel and filing under deadline | Timing-dependent design — notifications, reminders, cut-offs — is unanchored | 3 |
| Member Association media-officer interviews | How control keys and quotas are actually distributed federation-side | The highest-severity, every-cycle failure is modelled rather than observed | 3 |
| FIFA Event Media Operations workshop | Internal constraints, escalation paths, appeal handling | Backstage columns of the Service Blueprint are inferred, not confirmed | 3 |
| Process tracing with genuinely rejected applicants | What actually happens after a denial, and what recourse exists | Rejection and appeal UX rests entirely on synthetic accounts | 2 |
| Powered quantitative survey (n ≈ 400, stratified) | Population-level prevalence and prioritisation of pain points | No pain point can be ranked by real frequency or severity | 2 |
| Usability testing of the incumbent Media Hub / FIFA Media App | Measured task failure in the system being replaced | No baseline exists to improve against; success metrics stay uncalibrated | 3 |
| Card sort and tree test with media professionals | Validated vocabulary for the accreditation-vs-media-ticket split | The two-token labelling fix — the most-cited confusion — is untested | 2 |
| Comprehension testing in EN / ES / PT | Whether accreditation terminology survives translation | Translation risk is flagged in instruments but never verified | 2 |
| Accessibility audit and testing with disabled journalists | Conformance status and real barriers in credential and workroom flows | Accessibility obligations are acknowledged, not designed for | 3 |
| Legal review and DPIA across EU / Morocco / South American regimes | Lawful biometric handling and data-residency architecture | Privacy architecture is outside this study entirely | 3 |
| Service blueprint workshop with host committees | Real handoff points between host-city portals and the FIFA Hub | Duplicate-system risk for 2030 remains unresolved | 2 |
| Longitudinal comparison across 2027 and 2029 FIFA events | Whether 2026 pains persist, resolve or mutate | Findings are frozen at a single tournament cycle | 1 |

---

## 3. Gap selection

**Selection rule.** An Evidence Gap Register entry is in scope only if closing its *demand-side half* changes a screen, a state or a flow in v1. Several gaps have two halves: a factual half (what FIFA actually does) and a requirement half (what the applicant needs to see, decide or do). A simulated study cannot close a factual half. It is honest about closing only the second.

### Addressed — 3 of 10

| # | Gap (Run 1, D8) | Why it is in scope | What this study closes |
|---|---|---|---|
| **4** | Appeal / rejection route for denied applicants | A rejection is a terminal state in the product with no designed exit today; the flow cannot be built without a position on it | The requirement half: what an applicant needs at the moment of denial — status legibility, reason granularity, next action. Not FIFA's actual appeal policy. |
| **7** | Elimination-driven quota reallocation mechanics | Mid-tournament churn is the one moment the accreditation state changes after approval; it determines whether the product is a form or a live system | The requirement half: how reallocation is experienced and what visibility it demands. Not the FIFA/MA reallocation formula. |
| **5** | Whether the 2026 credential was mobile / biometric | Credential form factor is a v1 architecture decision that cannot be deferred | The requirement half: context of use at the turnstile — device, connectivity, roaming, failure recovery. Not the 2026 technical fact. |

These three map directly onto the three interview scenarios carried into Gate 2: a rejected application, a mid-tournament quota reallocation, and a matchday access denial.

### Out of scope — 7 of 10

| # | Gap | Why excluded |
|---|---|---|
| 1 | On-site accreditation-centre locations and PVC validation steps | A fact about the physical world; no amount of simulated response produces it. |
| 2 | Photo-position allocation method for oversubscribed matches | Requires FIFA-side and picture-editor access; simulating an allocation rule would invent the very thing at issue. |
| 3 | Quota allocation formula and its weightings | Closable only by document analysis of FIFA circulars — retrieval work, and Run 2 does not search. |
| 6 | Biometric data retention policy for media | A legal and DPIA question, not a design-research question; no instrument here can reach it. |
| 8 | Authoritative workroom, Wi-Fi and connectivity assessment | Requires on-site environmental audit; it also sets SLAs rather than product behaviour. |
| 9 | Actual per-MA quota numbers for 2026 | Benchmarking data obtainable only by request to FIFA or AIPS; no design decision in v1 waits on it. |
| 10 | Host-city portal vs FIFA Hub handoff points | Needs a multi-party workshop with host committees; it is an organisational-boundary question above app scope. |

---

✅ GATE 0 COMPLETE — `00_SCOPE.md`
