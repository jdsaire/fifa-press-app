# 00 — SCOPE LOCK

**SIMULATED — NOT EVIDENCE**

**Repo path:** `ux-ui/02-ideation/00_SCOPE.md`
**Mandate:** Minimal guerrilla ideation — FIFA media accreditation platform, 2030 target
**Inherited from:** `ux-ui/01-design-research/` (Run 2, eight gates), itself built on `RUN1_EVIDENCE-BASE.md`
**Status:** Scope frozen at Gate 0. Later gates work inside this boundary and do not renegotiate it.

---

## 1. What this mandate can and cannot establish

This mandate simulates a design-thinking workshop that never took place. No person attended, no idea was voted on by anyone, and no participant quoted here exists.

**It can establish:** a concept that answers an already-chosen design question; a traceable chain from a research theme to an idea to a decision; a documented record of which ideas were rejected and on what grounds; a stated boundary between what the concept does and what it refuses to attempt; and a brief that gives prototyping something specific to build against.

**It cannot establish:** that this concept is the right one. It can only show that it is a defensible one. No idea here has been preferred by a real journalist. No vote count reflects anyone's judgement but the author's. No prioritisation is validated, and the fact that one idea outscored another says nothing except that the scoring mechanic ran.

Where the workshop would have surfaced something nobody anticipated, this mandate cannot. Simulated participants only disagree in ways the author thought to write down.

*(191 words)*

---

## 2. The inherited contract

This is what the mandate is held to. Every gate is checkable against this table, and nothing in it is reopened.

| Constraint | Content | Source |
|---|---|---|
| **How Might We** (single, fixed) | *How might we tell a journalist that their access has changed before they discover it by being refused?* | `01-design-research/06_DESIGN-BRIEF.md` §1 |
| **Principle 1** | Every state change announces itself. → We will not ship a transition discoverable only by attempting an action and failing. | `06_DESIGN-BRIEF.md` §2 |
| **Principle 2** | A refusal is a message, not a wall. → We will not ship an outcome screen containing only an outcome. | `06_DESIGN-BRIEF.md` §2 |
| **Principle 3** | Serve the people who have no one to call. → We will not broadcast one notification set to every track. | `06_DESIGN-BRIEF.md` §2 |
| **Boundary — visas and entry permits** | Out. Consular decisions belong to host states. Platform records visa class against itinerary and flags incompatibility; it does not obtain, appeal or predict a visa. | `06_DESIGN-BRIEF.md` §3 |
| **Boundary — security vetting outcomes** | Out. National authorities decide. Platform shows vetting is in progress and that FIFA is not the decision-maker. | `06_DESIGN-BRIEF.md` §3 |
| **Boundary — quota size and allocation politics** | Out. Governance question between FIFA and Member Associations. Platform makes the current quota state legible; it does not argue fairness. | `06_DESIGN-BRIEF.md` §3 |
| **Boundary — replacing the MA relationship** | Out. Key distribution is federation-controlled. Platform instruments the hand-off: visibility, not control. | `06_DESIGN-BRIEF.md` §3 |
| **Boundary — guaranteeing access** | Out. Accreditation never guaranteed match access. Platform makes the two-token split explicit at approval rather than at the barrier. | `06_DESIGN-BRIEF.md` §3 |
| **Persona — build for** | Amina R., quota-dependent national reporter. MA quota → control key → Media Hub. No named FIFA contact. Abandons if the app is a place she checks and learns nothing. | `01-design-research/05_ARTIFACTS.md` §1.1 |
| **Persona — must survive** | Tomás L., rights-holder crew coordinator, 40–120 people. Has a named contact and uses it. Abandons if the tool adds per-person clicks or floods him with individual-scoped notifications. | `05_ARTIFACTS.md` §1.2 |
| **Target failure point** | Blueprint stage 10 — mid-tournament reallocation. The one zero-visibility stage FIFA both causes and fails to communicate. | `05_ARTIFACTS.md` §4 |
| **Carried-forward unknown** | Whether two-token confusion is a novice gap or a persistent one. Quant (Test 1) and qual (P02) disagree; Gate 4 declined to resolve it. No concept's core may rest on the answer. | `01-design-research/04_ANALYSIS.md` §2.2 T3, §3 |
| **Evidence discipline** | Every idea reaching the concept carries `[SOURCED]`, `[SIMULATED]` or `[ASSUMPTION]`. Numbers in `04_ANALYSIS.md` §1 are not citable as justification. | `06_DESIGN-BRIEF.md` §5 |

---

## 3. Preserved maximalist scope — the design programme not being run

Documentation only. No remediation plan, no phased proposal, no future-work section. This exists so a reader can see what a properly resourced ideation phase would have contained and judge the omission knowingly.

**Severity:** 1 = concept proceeds with minor assumption risk · 2 = a concept decision rests on inference · 3 = a decision cannot be validated at all before prototyping.

| Activity not run | What it would have established | Consequence of the gap | Sev |
|---|---|---|---|
| Live design-thinking workshop with 12–24 real participants across tracks | Ideas nobody in this repo would have thought of; genuine friction between stakeholder and end-user framings | Every idea here originates from one author reading one prior mandate; the divergence ceiling is that author's imagination | 3 |
| Real end-users in the room (accredited journalists, freelancers, MA officers) | Whether the HMW is the question they would have chosen, and whether "tell me sooner" is really the fix they want | The problem framing is inherited and never stress-tested by anyone who lives it | 3 |
| FIFA-side stakeholders present (Event Media Ops, Media Partnerships) | Which ideas are organisationally impossible before effort is spent scoring them | Feasibility placements in Gate 4 are guesses about an organisation nobody in this project has spoken to | 3 |
| Real prioritisation with budget holders (Buy a Feature with actual constraint) | A ranking with consequences attached, where trade-offs cost the voter something | The shortlist reflects an authored score, not a contested decision | 2 |
| Concept testing of the chosen concept with journalists | Whether the concept is comprehensible, wanted, and better than the group chat it competes with | The concept ships to prototyping untested against the only people who would use it | 3 |
| Technical feasibility review with engineering | Whether live state propagation to venue access lists is buildable at tournament scale | The concept's central mechanic — push on change, to two consumers — is assumed feasible | 2 |
| Notification-copy testing in EN / ES / PT | Whether the messages the concept depends on survive translation and land under deadline pressure | Message design is specified in English and assumed portable; Run 2 already flagged *cuota/cupo* as a live translation trap | 2 |
| Accessibility co-design with disabled journalists | Whether a notification-led concept works for people it was not designed around | Accessibility is acknowledged in Run 1's constraint register and designed for nowhere | 3 |
| Assumption-mapping and experiment design on the riskiest assumptions | Which assumptions to test first and how cheaply | Ideas are recorded as assumptions in Gate 2 but no test is designed for any of them | 2 |
| Competitive benchmarking of comparable notification-led services | Established patterns for state-change messaging in high-stakes, low-attention contexts | Interaction patterns are reasoned from first principles rather than from prior art | 1 |
| Second workshop after a cooling-off day, per the course protocol | Convergence performed by a rested, separate group rather than the same author minutes later | Divergence and convergence share one perspective; the check that convergence is meant to provide is structurally absent | 2 |

---

## 4. Handoff boundary

`03-ui-prototyping` inherits the concept, its three core interactions as behaviour, its boundary edges and its open questions — and must decide for itself every screen, flow, component, label, information architecture and visual choice, none of which this mandate is permitted to produce.

---

✅ GATE 0 COMPLETE — `00_SCOPE.md`
