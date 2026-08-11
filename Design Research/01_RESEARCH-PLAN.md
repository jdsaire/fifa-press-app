# 01 — RESEARCH PLAN

**Repo path:** `/Design Research/01_RESEARCH-PLAN.md`
**Scope authority:** `00_SCOPE.md` (gaps 4, 7, 5 — demand side only)
**Methods:** two. Simulated survey (n = 30) and simulated interviews (n = 5). Nothing else.

---

## 1. Research goal (SMART)

> **Find out what a journalist needs to be told, and how fast, at the exact moment their access falls apart after they thought it was already settled: when their application gets rejected, when their confirmed spot is taken away mid-tournament, or when they're turned away at the stadium gate.**

**Justification.** Each moment is the same broken promise — access seemed secured, then wasn't — and each maps to one of the three gaps this study is allowed to touch: rejection (Gap 4), mid-tournament reallocation (Gap 7), and gate-level denial tied to the credential itself (Gap 5).
The goal is achievable on simulated data because the answer we need is a description of what to show and how soon — not a measured fact about how often each moment actually happens. It is met when we can say, in plain words, what each of the three moments should tell the person and how quickly; it fails if any of the three stays vague.

---

## 2. Hypotheses

Stated so the Gate 4 analysis has something to fail against. Per `00_SCOPE.md`, simulated data cannot genuinely falsify either; the disconfirming observations below are the shape of evidence that *would* overturn them if this study ever ran with real participants.

### H1 — Visibility outranks outcome

At each of the three failure moments, the dominant grievance is the absence of state and reason — not knowing where the application stands or why it moved — rather than the unfavourable outcome itself. An applicant would prefer a refusal carrying a reason and a next action over an ambiguous silence of equal or shorter duration.

**Disconfirming observation:** survey respondents rank outcome speed or outcome reversal above explanation; interview participants treat a stated reason as irrelevant, cosmetic, or as an insult added to the refusal.

### H2 — The two-token split is a mental-model failure, not an information-supply failure

Misunderstanding of accreditation versus per-match media ticket persists independently of exposure to FIFA's published guidance and of years of tournament experience. Experienced applicants who have read the rules still mispredict what their credential permits at the turnstile.

**Disconfirming observation:** correct prediction of credential permissions rises with years of experience or with self-reported guidance-reading — i.e. veterans get it right and only newcomers err. That result reclassifies the problem as onboarding, not information architecture, and the v1 labelling work loses its justification.

---

## 3. Traceability matrix

| Goal component | Gap (Run 1 D8) | Hypothesis | Method | Design decision unblocked |
|---|---|---|---|---|
| Rejection state | 4 — appeal / rejection route | H1 | Interviews (primary), survey items | Whether rejection is a terminal screen or a state with a defined exit; reason granularity shown to the applicant |
| Mid-tournament reallocation state | 7 — elimination-driven quota churn | H1 | Interviews (primary), survey items | Whether accreditation is a form-and-forget artifact or a live status object with post-approval notifications |
| Matchday access state | 5 — credential form factor | H2 | Survey (primary), interviews | Credential form factor and the labelling of the accreditation-vs-ticket split at the point of entry |
| All three | — | H1, H2 | Both | Notification trigger set for v1: which state changes push, which pull, which stay silent |

---

## 4. Sample

### Survey — n = 30 synthetic respondents

Stratified across the seven Run 1 archetypes (D4), weighted toward the archetypes carrying the highest-severity Run 1 pain rather than toward real-world population share.

| Archetype (Run 1 D4) | Quota | Rationale for the weight |
|---|---|---|
| Mid-size national outlet | 8 | Quota-dependent; carries pains 3, 4, 17 — the Gate 5 primary persona |
| Freelance / independent | 6 | Weakest appeal path; carries pains 4, 11 — central to gap 4 |
| Large rights-holding broadcaster | 4 | Operationally complex; the second Gate 5 persona |
| Wire / photo agency | 4 | FIFA-direct track; the matchday-position contrast case |
| Non-rights broadcaster | 3 | Structurally excluded category; a source of disconfirming voices |
| Host-city local press | 3 | Separate track; tests whether findings generalise across tracks |
| Digital creator | 2 | No defined category; included to keep the emerging case visible |
| **Total** | **30** | |

Secondary stratification by confederation, coarse: UEFA 9 · CONMEBOL 5 · CAF 5 · AFC 5 · CONCACAF 5 · OFC 1.

**n = 30 is a floor set for analytical mechanics, not for statistical power.** It is the smallest dataset on which descriptives per archetype and a two-test inferential pipeline can be executed and shown to run correctly. It supports no claim about prevalence, and no cell of the stratification is large enough to compare archetypes against one another.

### Interviews — n = 5 synthetic participants

| ID | Archetype | Moment carried |
|---|---|---|
| P01 | Mid-size national outlet | Mid-tournament quota reallocation after elimination |
| P02 | Mid-size national outlet | Matchday access denial (two-token) |
| P03 | Freelance / independent | Rejected application, no visible appeal route |
| P04 | Large rights-holding broadcaster (coordinator side) | Crew-level reallocation and credential logistics |
| P05 | Wire / photo agency | Matchday position and credential at the turnstile |

**n = 5 is set for artifact grounding, not saturation.** It is the number of accounts needed to populate two personas, one empathy map and three scenarios with concrete detail. No claim of thematic saturation is made or available at this n, and Gate 4 will not assert one.

---

## 5. Why qualitative depth outranks statistical representativeness here

The decisions this study unblocks are decisions about what must exist on a screen — whether a rejection has a reason field, whether a reallocation generates a notification, whether the credential's permissions are legible at the turnstile. Those are settled by one clear account of a moment going wrong, because a state that is needed once is needed in the design; frequency changes its priority in a backlog, not its presence in the specification. A representative sample would tell us how many applicants hit each failure and in what order to build the fixes — genuinely useful, and genuinely out of reach at n = 30 on synthetic data. Depth is what this study can honestly produce, and it happens to be what the design decisions in scope actually require.

---

✅ GATE 1 COMPLETE — `01_RESEARCH-PLAN.md`
