# 07 — LIMITATIONS & METHODOLOGICAL DISCLOSURE

**Repo path:** `/Design Research/07_LIMITATIONS.md`
**Audience:** a professional reviewer reading this repo cold and deciding whether to take it seriously.

`00_SCOPE.md` drew the boundary before the work and listed the program that was not run. This file states the method actually used, defends every decision inside that boundary, and says exactly what the finished deliverables will and will not carry. It does not restate the scope table.

---

## 1. Method actually used

Two passes.

**Run 1** was secondary research against primary sources: FIFA's own accreditation guidance and advisories for 2026, FIFA's post-tournament figures, the AIPS protest letter of 6 June 2026, IFJ documentation, and comparator systems (Qatar 2022, Russia 2018, Olympic OBS/OIS/Rate Card, NFL credentialing). Every claim in `RUN1_EVIDENCE-BASE.md` is tagged `[VERIFIED]`, `[INFERRED]` or `[REPORTED]`. That file is the only evidence in this repo.

**Run 2** — everything else here — is a simulated study with no human participants. The screener, consent notice, survey and interview guide in `02_INSTRUMENTS.md` were written as if for real fielding and then administered to nobody. The 30 survey rows and 5 interview transcripts were authored by the same person who wrote the hypotheses, generating data consistent with Run 1's documented pain points. That data was then analysed as if it were real, using standard descriptives, two inferential tests, and thematic coding.

No journalist, federation officer or FIFA employee was interviewed, surveyed, observed or contacted at any point.

---

## 2. Scope decisions and why each was made

| Decision | Rationale |
|---|---|
| **No real recruitment** | A resource constraint, stated up front rather than discovered late. Real accredited media at this scale are not reachable by a one-person project without institutional backing, and a small convenience sample of unrepresentative volunteers would have produced a *worse* artifact: real data thin enough to mislead, carrying the authority of being real. Fabricated data honestly labelled is more useful than real data dishonestly generalised. |
| **One research goal** | Three failure moments, one question about each. A study with four goals and no participants is four times as fictional, not four times as informative. |
| **Two hypotheses** | The minimum that gives the analysis something to fail against. One of them did fail (§3). |
| **n = 30 survey** | A floor for analytical mechanics — enough to run stratified descriptives and two tests and demonstrate the pipeline executes. Never presented as powered; the largest stratum holds 8 respondents and the smallest 2. |
| **n = 5 interviews** | Enough accounts to ground two personas, an empathy map and three scenarios in concrete detail. No saturation claim is made anywhere, because none is available at this n and none would be meaningful on authored transcripts. |
| **Two methods only** | Survey plus interviews. Diary studies, contextual inquiry and usability testing all require the real participants this study does not have; simulating them would have compounded fiction rather than adding method. |
| **3 of 10 evidence gaps** | Gaps 4, 5 and 7 — and only their demand-side halves. The factual halves (what FIFA's quota formula actually is, what the 2026 credential actually was) cannot be closed by inference, and pretending otherwise would have been the study's worst failure. |
| **Reduced artifact set** | Two personas, one empathy map, three scenarios, one blueprint. Artifacts are only as good as the accounts under them; five personas on five synthetic transcripts is decoration. |
| **Journey Map omitted** | Its emotional trajectory is carried as a column in the service blueprint. A separate map would restate the same sequence with less operational detail. |
| **Trilingual scope (EN/ES/PT)** | A product decision inherited by the instruments, which is why translation risk is flagged per-item rather than in general. |

---

## 3. The hypothesis that failed, and why it is reported

H2 predicted that confusion between accreditation and match-day tickets persists independently of experience. Test 1 returned the exact disconfirming observation pre-registered in `01_RESEARCH-PLAN.md`: comprehension rose sharply with years of experience (U = 155.5, p = 0.0016, r = 0.58).

It is reported as a failure rather than quietly rewritten. But the honesty cuts both ways, and this is the point on which the whole repo should be judged: **that disconfirmation is as manufactured as support would have been.** The experience values were written into the file at Gate 3 by the person who wrote the hypothesis at Gate 1. A study cannot falsify itself. What the exercise demonstrates is that the analysis pipeline detects a disconfirming pattern when one is present and that the researcher reports it when it appears — a claim about process discipline, not about journalists.

---

## 4. What the deliverables legitimately support

- **Design direction.** The service blueprint's identification of four zero-visibility stages, and the isolation of stage 10 — mid-tournament reallocation — as the one FIFA both causes and fails to communicate. That analysis rests on Run 1's `[VERIFIED]` lifecycle, not on simulated data.
- **Information-architecture reasoning.** The argument that accreditation must be modelled as a live state object rather than a terminal approval artifact follows from the documented existence of post-approval quota change, not from any survey response.
- **Failure-mode awareness.** The scenarios name real, sourced failure classes — visa nullification of valid accreditation, elimination-driven reallocation, two-token access denial.
- **A defensible product rationale.** A reader can trace every design decision back through theme, code and pain point to a citable Run 1 source, and can see where that chain stops.
- **Scope discipline.** The boundary file states what the platform must not claim to solve — visas, vetting, quota politics — which is itself a research output.

## 5. What they do not support

- Any empirical claim about real journalists, real outlets or real federations.
- Any percentage in this repo. Not one describes a population. The 80%, the 8/30, the UMUX-Lite means and both p-values are properties of an authored file.
- Any validated persona. Amina R. and Tomás L. are composites of synthetic accounts, not of interviews.
- Any usability benchmark. The UMUX-Lite figures cannot be compared to published norms, because no one answered the items.
- Any prioritisation by frequency or severity. Which failures matter most to most people is exactly what a representative sample would establish, and none was drawn.
- Any claim of thematic saturation, inter-rater reliability, or generalisability of any kind.

---

## 6. Why this was the correct trade-off

The objective of this project is to demonstrate development capability. The research exists to make the build defensible — to answer *why this product, why these screens, why this scope* with something better than intuition. Measured against that objective, the binding requirement was traceability, not empiricism: a reviewer must be able to follow any design decision back to a source and see the quality of that source labelled honestly.

Simulated data with rigorous provenance meets that requirement. A small convenience sample would not have, and would have carried an unearned authority that made the repo *less* trustworthy, not more. The alternative to fabricated-and-labelled was not real-and-valid; it was unstated assumption dressed as product instinct — which is what this document exists to make impossible.

---

## 7. Closing statement

This repository contains no evidence about journalists. It contains a documented chain of reasoning from published, primary-sourced facts about how FIFA media accreditation actually worked in 2026, through a simulated study that is labelled as simulated on every file and in every table, to a set of design decisions that can be individually traced, individually challenged, and individually discarded. Where the reasoning depends on inference, it says so; where it depends on invention, it says that too. One of its two hypotheses failed, and the failure is reported in full alongside the reason the failure proves nothing. The correct reading of this work is not that its findings are weak but that they are not findings at all — they are a design rationale with its provenance fully exposed, which is a more useful artifact than a small study overclaiming its reach, and a more honest one than a product built on assumptions nobody wrote down.

---

✅ GATE 7 COMPLETE — `07_LIMITATIONS.md`
