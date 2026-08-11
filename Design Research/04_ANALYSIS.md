# 04 — ANALYSIS

**SIMULATED — NOT EVIDENCE**

**Repo path:** `/Design Research/04_ANALYSIS.md`
**Inputs:** `survey_master.csv` (n = 30), `interviews_master.json` (n = 5)
**Headline:** H1 is consistent with the data. **H2 is not** — the data ran against it, and that result is reported below rather than reframed.

---

## 1. Quantitative

### 1.1 Descriptives by archetype

**SIMULATED — NOT EVIDENCE**

| Archetype | n | Mean yrs exp | Q11 (capabilities) | Q12 (ease) | UMUX-Lite mean |
|---|---|---|---|---|---|
| Digital creator | 2 | 1.5 | 1.50 | 1.50 | **1.50** |
| Non-rights broadcaster | 3 | 3.3 | 1.67 | 1.33 | **1.50** |
| Freelance / independent | 6 | 5.2 | 2.67 | 2.83 | **2.75** |
| Host-city local press | 3 | 3.7 | 3.00 | 2.67 | **2.83** |
| Wire / photo agency | 4 | 8.3 | 3.00 | 2.75 | **2.88** |
| Mid-size national outlet | 8 | 4.8 | 3.25 | 2.75 | **3.00** |
| Large rights-holding broadcaster | 4 | 10.3 | 4.00 | 3.50 | **3.75** |
| **All** | **30** | 5.6 | — | — | **2.77** |

*Caption: UMUX-Lite on a 7-point scale. These figures describe a file that was authored, not a population that was measured. The rank order reproduces the weighting decisions made in Gate 3; it is not a usability benchmark and must not be compared against published UMUX-Lite norms.*

### 1.2 Key item distributions

**SIMULATED — NOT EVIDENCE**

| Item | Result |
|---|---|
| Q1 — two-token comprehension correct | **8 / 30** (22 answered "No"/"Not sure" incorrectly or uncertainly) |
| Q3 — asked what their accreditation allowed, 3+ times | 20 / 30 |
| Q5 — could see application status "Never" or "Rarely" | 17 / 30 |
| Q6 — chose reason-with-delay over speed-without-reason | 24 / 30 |
| Q7 — wanted quota context or more (options b/c/d) | 25 / 30 |
| Q8 — access changed after confirmation | 20 / 30 ("Yes"), 4 "Not sure" |
| Q9 — learned of the change at the gate, or never officially | 15 / 30 |
| Q10 — wanted notice within minutes or same day | 20 / 30 |

### 1.3 Test 1 — H2: does two-token comprehension rise with experience?

Mann-Whitney U, years of experience compared between respondents answering Q1 correctly and all others. Non-parametric because experience is skewed and the correct-answer group is small (n = 8).

| | n | Median yrs | Mean yrs |
|---|---|---|---|
| Q1 correct | 8 | 9.5 | 9.50 |
| Q1 incorrect / unsure | 22 | 3.5 | 4.14 |

**U = 155.5, p = 0.0016, z = 3.17, r = 0.58 (large).**

**H2 is not supported.** H2 predicted that misunderstanding persists *independently* of experience. The stated disconfirming observation in `01_RESEARCH-PLAN.md` — "correct prediction rises with years of experience" — is exactly what appeared. On this data the two-token problem looks like a novice problem, which would reclassify it as onboarding rather than information architecture.

*Caption: pipeline validated, finding not established. This result is a property of how Gate 3 assigned experience values, not a discovery about journalists. It is reported because reversing a hypothesis after seeing data that contradicts it would be the more serious methodological failure — but the reversal itself carries no evidential weight either.*

### 1.4 Test 2 — H1: is reason preferred over speed?

Exact binomial test of the Q6 forced choice against chance (0.50).

**24 / 30 (80.0%) chose reason-with-delay. p = 0.0014. 95% CI [0.61, 0.92]. Cohen's h = 0.64 (medium).**

**H1 is consistent with the data.** Six respondents chose speed without reason — the disconfirming voices are present, concentrated among low-experience and structurally excluded respondents (R11, R23, R28), for whom a fast "no" ends the uncertainty sooner.

*Caption: pipeline validated, finding not established. The 80% figure is the rate Gate 3 wrote in. It describes the file, not the profession.*

### 1.5 What n = 30 can and cannot support

At n = 30 with seven strata, the largest cell holds 8 respondents and the smallest holds 2. That is enough to execute descriptives and two inferential tests correctly and to show the pipeline runs end to end — it is not enough to compare archetypes against one another, to estimate any prevalence with a usable margin of error, or to detect anything but a large effect. Both tests above returned p < 0.01, which reflects effect sizes deliberately written into the data, not the sensitivity of the design. On real data at this n, only an effect of roughly r ≥ 0.5 would be detectable at all; anything subtler would be invisible. No number in this section should be quoted outside this repo.

---

## 2. Qualitative

### 2.1 Codebook

Seeded from the Run 1 pain-point inventory (D5) unless marked **[EMERGENT]**.

| Code | Definition | Example |
|---|---|---|
| SILENT-WAIT | No visible status between submission and decision; the applicant fills the gap by guessing or chasing | P01, P03 |
| NO-REASON | An outcome delivered without a cause the applicant can act on or learn from | P03 |
| NO-EXIT | A refusal with no appeal route, no contact, no defined next step | P03 |
| TWO-TOKEN-BLIND | Believes the accreditation itself grants match-level access | P02 |
| QUOTA-CHURN | Access changes after confirmation because the applicant's team was eliminated | P01 |
| POSITION-NOT-ENTRY | For agency staff the contested resource is placement, not admission | P05 |
| TRACK-PRIVILEGE | Experience quality differs sharply by accreditation track, not by need | P04 |
| OFFLINE-FRAGILITY | The digital layer fails exactly where it is needed — at the venue, on bad connectivity | P02 |
| **DISCOVERY-BY-FAILURE** **[EMERGENT]** | Learns their status changed only by attempting an action and having it fail | P01, P05 |
| **GRAPEVINE** **[EMERGENT]** | Operationally necessary information arrives from peers rather than from the organiser | P01, P03 |
| **GATE-DESYNC** **[EMERGENT]** | Ground staff work from stale records while the applicant holds a newer digital state | P05 |
| **NOISE-RESISTANCE** **[EMERGENT]** | More messaging is experienced as burden, not help, by those already well served | P04, P01 |

### 2.2 Themes

**T1 — The failure is the silence, not the answer.**
Across the accreditation lifecycle, participants object less to unfavourable outcomes than to unexplained ones. A refusal with a cause is treated as information; a refusal without one is treated as a closed door with nobody behind it.
*Supported by:* P01, P03, P05. *Codes:* SILENT-WAIT, NO-REASON, NO-EXIT, GRAPEVINE.
*Disconfirming case:* **P04** — a broadcaster coordinator whose track already provides a named contact, and who states that additional messaging would be noise. Silence is only a failure where nobody is reachable.
*Design implication:* every terminal or changed state needs a reason field and a named next action — but scoped to tracks without a human contact, not broadcast to everyone.

**T2 — Status changes travel slower than the consequences they cause.**
Participants learn that their access has changed by trying to use it and failing, or by being told at a barrier. The change is real before it is communicated, and the gap is where the cost lands — a wasted flight, twenty minutes at a gate before kickoff.
*Supported by:* P01, P05. *Codes:* DISCOVERY-BY-FAILURE, QUOTA-CHURN, GATE-DESYNC.
*Disconfirming case:* **P05 himself, partially** — he reports that position reassignment is expected in agency work and not taken personally; his complaint is that the update didn't reach the gate staff, not that it happened. The problem is propagation, not change.
*Design implication:* accreditation must be a live state that pushes on change, and the same state must reach venue-side systems, not just the applicant's phone.

**T3 — What the badge means is learned at the barrier.**
The accreditation-versus-match-ticket split is not understood from documentation. It is understood the first time someone is stopped.
*Supported by:* P02, P05. *Codes:* TWO-TOKEN-BLIND, POSITION-NOT-ENTRY, OFFLINE-FRAGILITY.
*Disconfirming case:* **Test 1 (§1.3)** — the survey data says experienced respondents do get it right, which points to onboarding rather than to a structural labelling failure. P02 is the counter-case to the counter-case: nine years' experience, read the guidance closely, still turned back. One case cannot resolve this.
*Design implication:* state the two-token split at the moment of approval, on the confirmation itself. Treat the open question — novice gap or persistent gap — as unresolved and cheap to answer later.

---

## 3. Triangulation

| Theme | Survey item(s) | Direction | Run 1 link | Confidence |
|---|---|---|---|---|
| T1 — Silence, not the answer | Q5 (17/30 never/rarely), Q6 (24/30), Q7 (25/30) | Supports | D5 #5 (late confirmation), #11 (no appeal route) — both [VERIFIED] | **[SOURCED]** for the underlying pain; **[SIMULATED]** for every magnitude |
| T2 — Changes travel slower than consequences | Q8 (20/30), Q9 (15/30 gate or never), Q10 (20/30 want minutes/same day) | Supports | D5 #17 (elimination-driven churn) — [INFERRED] in Run 1, not verified | **[SIMULATED]** — the mechanism is inferred in Run 1 and modelled here |
| T3 — Badge meaning learned at the barrier | Q1 (8/30 correct), Q3 (20/30 asked 3+ times) | Supports | D5 #2 (two-token confusion) — [VERIFIED] | **[SOURCED]** for the confusion existing |
| T3 — as a *structural* rather than novice failure | Test 1 (p = 0.0016, r = 0.58) | **Contradicts** | — | **[ASSUMPTION]** — unresolved; quant and qual disagree |
| T1 — scoping to unserved tracks | P04 only | Qualifies | D4 (rights-holder track handled separately) — [VERIFIED] | **[SOURCED]** for the track split; **[ASSUMPTION]** for the noise objection |

---

## 4. Limitations of this analysis

1. Both p-values are properties of data that was authored with these effects deliberately inserted; statistical significance here means the pipeline runs, nothing more.
2. H2's disconfirmation is as manufactured as H1's support would have been — neither outcome tells us anything about real comprehension.
3. Three of the twelve codes rest on a single participant each (GATE-DESYNC, TRACK-PRIVILEGE, POSITION-NOT-ENTRY); at n = 5 no code frequency is meaningful and none is reported.
4. The coder, the interview author, and the hypothesis author are the same person, so the codebook cannot be independent of the data it codes — there is no inter-rater reliability to report and none is claimed.
5. The T3 contradiction between Test 1 and P02 is left open rather than resolved, because resolving it on synthetic data would mean picking whichever answer suited the design.

---

✅ GATE 4 COMPLETE — `04_ANALYSIS.md`
