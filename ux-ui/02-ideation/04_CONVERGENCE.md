# 04 — ASSESS, PRIORITIZE, CONVERGE

**SIMULATED — NOT EVIDENCE**

**Repo path:** `ux-ui/02-ideation/04_CONVERGENCE.md`
**Pool assessed:** ID-01 to ID-32, from `02_DIVERGENCE-1.md` and `03_DIVERGENCE-2.md`
**Tests applied:** does it serve the HMW · does it survive the `00_SCOPE.md` §2 boundary
**Blocks:** Day 1 prioritisation (20 + 15 min); Day 2 assessment and convergence (20 + 30 min)

Nothing is deleted. Ideas that fail a test are marked and excluded from prioritisation, and remain in the record with the reason.

---

## 1. Assessment pass

**HMW:** *How might we tell a journalist that their access has changed before they discover it by being refused?*

| ID | Idea, one line | Tag | Serves HMW | Survives boundary | Status |
|---|---|---|---|---|---|
| ID-01 | There is a usable window between a change and its consequence | `[SIMULATED]` | — | Y | **Premise, not an idea** — tested, not built (§2.3) |
| ID-02 | Accreditation as a live state object with an append-only change log | `[SOURCED]` | Y | Y | In |
| ID-03 | Every transition emits to the venue access list as well as the holder | `[SOURCED]` | Y | Y | In |
| ID-04 | Message volume scoped inversely to the human support a track already has | `[SOURCED]` | Y | Y | In |
| ID-05 | Mandatory reason field on every negative or changed state | `[SOURCED]` | Y | Y | In |
| ID-06 | Elimination treated as a predictable trigger; warn before recalculation lands | `[SIMULATED]` | Y | Y | In |
| ID-07 | The wait itself has a state, an owner and an expected window | `[SIMULATED]` | **N** | Y | **Excluded** — addresses pre-decision silence (T1), not a post-confirmation change. Correct work, different question. |
| ID-08 | Two-token split stated at the moment of approval | `[ASSUMPTION]` | **N** | Y | **Excluded from this mandate's prioritisation** — it is comprehension, not change notice. Note: `06_DESIGN-BRIEF.md` §5 instructs the build to do it anyway, cheaply. That instruction stands independently of this gate. |
| ID-09 | Current quota state and its changes made legible | `[SOURCED]` | Y | Y | In |
| ID-10 | Last-known state readable offline, legibly timestamped | `[SOURCED]` | Y | Y | In |
| ID-11 | Visa class checked against itinerary shape, incompatibility flagged when known | `[SOURCED]` | Y | Y | In — the permitted edge of the visa boundary |
| ID-12 | The official channel is faster than the group chat | `[SIMULATED]` | Y | Y | **Outcome, not an idea** — a success condition of ID-02 + ID-13, not separately buildable (§2.3) |
| ID-13 | Messages tiered by consequence, derived from change type not user config | `[SOURCED]` | Y | Y | In |
| ID-14 | Mandatory next-step field, including "nothing, and here is who decided" | `[SOURCED]` | Y | Y | In |
| ID-15 | MA key hand-off instrumented for FIFA visibility | `[SOURCED]` | **N** | Y | **Excluded** — pre-approval distribution, not post-confirmation change. Remains a live Run 1 recommendation outside this mandate. |
| ID-16 | Forwardable, timestamped record of a change | `[ASSUMPTION]` | Y | Y | In |
| ID-17 | Delivery timed to her local time and travel state, with imminence override | `[ASSUMPTION]` | Y | Y | In |
| ID-18 | Escalation route reachable from the barrier when app and gate disagree | `[SIMULATED]` | Y | Y | In |
| ID-19 | FIFA issues the control key directly if the MA has not | `[SOURCED]` | Y | **N** | **Excluded** — *replacing the Member Association relationship* |
| ID-20 | FIFA-negotiated pre-cleared press entry status | `[SOURCED]` | Y | **N** | **Excluded** — *visas and entry permits*. Fragment survives in ID-11. |
| ID-21 | Floating reserve reallocated directly to eliminated teams' press | `[SIMULATED]` | Y | **N** | **Excluded** — *quota size and allocation politics*; secondarily *guaranteeing access*. Fragment survives as a Principle 2 reading. |
| ID-22 | Access forecast: entitlements shown under each result before the match | `[SIMULATED]` | Y | Y | In |
| ID-23 | Self-updating e-ink physical credential | `[SIMULATED]` | Y | Y | In — survives both tests; placed on effort honestly (§2.1) |
| ID-24 | Official presence inside the messaging groups journalists already use | `[SIMULATED]` | Y | Y | In, **with hazard** |
| ID-25 | A named human contact for every applicant regardless of track | `[SOURCED]` | Y | Y | In, **as a staffing dependency** |
| ID-26 | Platform holds refundable travel options, releases on change | `[ASSUMPTION]` | **N** | Y | **Excluded** — attacks the cost, not the notice. Also unbounded scope creep into travel. |
| ID-27 | Accreditation identity persists across FIFA events | `[SOURCED]` | **N** | Y | **Excluded** — shortens future silence; does not tell her about a change. Live Run 1 recommendation elsewhere. |
| ID-28 | Appeal as a first-class state with its own clock and terminus | `[SOURCED]` | **N** | Y | **Excluded** — post-refusal recourse, not pre-refusal notice. Gap 4 work, and genuinely needed; not this HMW. |
| ID-29 | Gate reads live state device-to-device, no network in the path | `[SIMULATED]` | Y | Y | In |
| ID-30 | Tournament-wide public change feed | `[SIMULATED]` | Y | Y | In, **with hazard** |
| ID-31 | Her outlet sees her state | `[ASSUMPTION]` | Y | Y | In |
| ID-32 | Pre-tournament rehearsal of a simulated reallocation and refusal | `[SIMULATED]` | Y | Y | In |

**Excluded: 9** (ID-07, 08, 15, 19, 20, 21, 26, 27, 28). **Reclassified: 2** (ID-01, ID-12). **Carried to prioritisation: 21.**

A note on the exclusions that hurt: ID-07, ID-28 and ID-15 are all good, sourced work that the product will probably need. They fail one test — this HMW — and a mandate that quietly widened its own question to keep its best ideas would have no boundary at all.

---

## 2. Prioritisation

### 2.1 Matrix A — effort vs impact on user experience

| | **Low effort** | **High effort** |
|---|---|---|
| **High impact** | **Quick wins** — ID-05, ID-09, ID-13, ID-14, ID-16, ID-17 | **Major projects** — ID-02, ID-03, ID-04, ID-06, ID-10, ID-11, ID-22, ID-29 |
| **Low impact** | **Fill-ins** — ID-18, ID-31 | **Labour intensive** — ID-23, ID-24, ID-25, ID-30, ID-32 |

**Reading it.** The quick-wins quadrant is entirely message *content* — reason, next step, tier, timing, receipt. None of it requires new architecture; all of it requires deciding what a message must contain. The major-projects quadrant is entirely the *substrate* that makes those messages possible. That split is the shape of the whole concept: cheap content sitting on expensive plumbing, and the content is worthless without the plumbing beneath it.

**Labour intensive deserves a second look before it is dismissed.** ID-25 is in that quadrant only because it is staff cost, not because it is a bad answer — it is arguably the correct answer, and it is what makes P04's track work. ID-32 is cheap in absolute terms and sits there only because its impact is speculative.

### 2.2 Matrix B — feasibility vs familiarity–innovation

| | **Familiar** | **Innovative** |
|---|---|---|
| **Feasible** | ID-05, ID-09, ID-13, ID-14, ID-17, ID-31 | ID-02, ID-04, ID-06, ID-10, ID-16, ID-22 |
| **Difficult** | ID-18, ID-25, ID-32 | ID-03, ID-11, ID-23, ID-24, ID-29, ID-30 |

**What the second matrix changes.** Matrix A puts ID-03 and ID-29 in the same quadrant as ID-02. Matrix B separates them: ID-02 is innovative-but-feasible, ID-03 and ID-29 are innovative-and-difficult, because the obstacle is not engineering but the venue-operations organisation that owns the access list. Running only one matrix would have hidden that the hardest thing in the concept is a boundary between two organisations, not a piece of software.

### 2.3 Two ideas that are not ideas

**ID-01** is the premise the mandate stands on, not a candidate for funding. It states that a window exists between a change and its consequence. It is not built; it is either true or the concept is decoration. It is carried to Gate 5 as the first open question and to prototyping as the thing to test before anything else.

**ID-12** — "the official channel is faster than the group chat" — is the success condition of ID-02 and ID-13 working, not a separable thing to build. Prioritising it would mean funding an outcome.

---

## 3. Buy a Feature

**Why this method.** The group already agrees on the problem; what they disagree about is what to spend on. Buy a Feature is the method that forces that disagreement into the open, because it makes participants give something up and prices the biggest items above any one person's budget so nothing important passes without a coalition. Six Hats would have surfaced perspectives the roster already declares by role.

**Setup.** Each participant holds 100 units; 600 total. Prices are set so that the three infrastructural items cannot be bought by any individual.

### 3.1 Round one — independent allocation

| ID | Price | W1 reporter | W2 freelance | W3 MA officer | W4 FIFA ops | W5 broadcaster | W6 gate | Raised | Funded |
|---|---|---|---|---|---|---|---|---|---|
| ID-02 live state + change log | 250 | 40 | 20 | 30 | 60 | — | — | 150 | No |
| ID-03 emit to the gate | 200 | — | — | — | 40 | — | 70 | 110 | No |
| ID-04 volume scoped by track | 90 | — | — | 20 | — | 70 | — | 90 | **Yes** |
| ID-05 reason field | 80 | 20 | 40 | — | — | — | — | 60 | No |
| ID-06 elimination pre-warning | 120 | 40 | — | — | — | — | — | 40 | No |
| ID-09 quota state legible | 60 | — | — | 50 | — | — | — | 50 | No |
| ID-10 offline last-known state | 110 | — | — | — | — | — | 30 | 30 | No |
| ID-13 message tiering | 70 | — | — | — | — | 30 | — | 30 | No |
| ID-14 next-step field | 60 | — | 40 | — | — | — | — | 40 | No |

**One item funded out of nine.** The money fragmented along role lines almost perfectly — each participant funded the thing their own working life is organised around — and the only item that cleared was the dissenter's constraint, because W5 concentrated his entire budget on the one rule that protects him while everyone else spread theirs across the things they personally needed.

**The disagreement, stated plainly.** W5 funded nothing that generates a message. His position across the round was that the concept's value to him is entirely the scoping rule and that every other item is a cost he absorbs. W3, the federation officer, spent half her budget on ID-09 — making quota state legible — and said the reason was that she currently absorbs the anger for a decision she does not make. W6 put 70 units on the gate and nothing on the applicant at all: his failure is not that Amina is uninformed, it is that he is.

### 3.2 Round two — negotiation

The method permits pooling after the first round. The group reconvened around three items.

| ID | Price | Outcome | How the coalition formed |
|---|---|---|---|
| ID-02 | 250 | **Funded** | W4 60, W1 70, W2 30, W3 30, W6 20, **W5 40**. W5 released funds on the stated condition that ID-04 stayed funded — he would pay for the substrate only once the scoping rule was locked. W1 reached 70 by withdrawing her entire ID-06 allocation. |
| ID-05 + ID-14 | 140 | **Funded** | W2 70, W1 30, W3 40. W2 argued the two are one item and refused to split them: a reason without a next step is still a closed door. |
| ID-03 | 200 | **Funded** | W6 80, W4 40, W5 40, W3 40. W5's second contribution, on the grounds that his crews are the ones held at barriers by stale lists. |
| ID-06 | 120 | **Not funded** | W1 gave up her own scenario's warning to fund the infrastructure underneath it. |

### 3.3 What the mechanic rewarded, and the warning in it

*Caption: these figures validate the prioritisation mechanic. They do not validate the winning ideas. Every price, every budget and every participant's preference was set by the author before the round was played, and the coalition that formed is the one the pricing made available.*

**The warning is real regardless.** The exercise pushed money toward the substrate and away from the single moment the HMW was written about. W1 — the proxy for the only persona this concept is being built for — ended the round having funded the plumbing and abandoned the warning she came for. If this had been a real workshop the correct facilitation response would be to say so out loud before anyone left the room, because a prioritisation that defunds the user's own scenario has told you something about the method, not about the scenario.

Accordingly, ID-06 is carried into the shortlist **against the funding result**, with that override stated rather than hidden.

---

## 4. Ranked shortlist to Gate 5

| Rank | ID | Why it survived |
|---|---|---|
| 1 | **ID-02** — live state object + append-only change log | The substrate. Every other item on this list is either a property of it or is worthless without it. Funded by the broadest coalition, and the only item all six contributed to. |
| 2 | **ID-05 + ID-14** — mandatory reason and next-step on every negative or changed state | Funded as one indivisible item and treated as one from here. Directly instantiates Principle 2, and is the cheapest high-impact work in either matrix. |
| 3 | **ID-03** — the same event reaches the venue access list | The only item addressing the second consumer. Sourced to a documented failure (P05, twenty minutes at kickoff minus twenty) and the one Matrix B identifies as organisationally, not technically, hard. |
| 4 | **ID-04** — volume scoped inversely to a track's human support | Funded first and unanimously enough to become a precondition for the rest. It is the concept's veto rule: it is what stops the answer to Amina's problem becoming Tomás's problem. |
| 5 | **ID-06** — elimination as a predictable trigger, warned before recalculation | **Carried against the funding result.** It is the only surviving idea that operates *before* the change lands rather than reporting it after, and the HMW is a question about "before." Dropping it would leave the mandate's central scenario unserved by its own concept. |
| 6 | **ID-13** — messages tiered by consequence, derived from change type | The mechanism that makes ID-04 operable and keeps ID-02 from becoming a firehose. Quick win in both matrices. |
| 7 | **ID-10** — last-known state readable offline, legibly timestamped | The state has to be present where enforcement happens. Carries the surviving fragment of ID-23 without the hardware programme. |
| 8 | **ID-09** — current quota state and its changes made legible | The permitted half of the quota boundary, and the item the federation officer funded above everything else. Makes a change explicable without arguing about how the quota was set. |

**Not carried, and worth naming:** ID-11 (visa incompatibility flag) is sourced, in scope, and the brief already commits to it — it is left out of the concept's core because it is a different trigger on the same substrate, and Gate 5 will name it as an edge the concept must not appear to solve. ID-25 goes forward to Gate 6 as an operations recommendation, not as part of the concept. ID-24 and ID-30 are dropped: both fix the grapevine by publishing her status into a space she does not control, which trades one exposure for another.

---

✅ GATE 4 COMPLETE — `04_CONVERGENCE.md`
