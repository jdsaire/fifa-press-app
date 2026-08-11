# 05 — ARTIFACTS

**SIMULATED — NOT EVIDENCE**

**Repo path:** `/Design Research/05_ARTIFACTS.md`
**Grounding:** `04_ANALYSIS.md` themes T1–T3; `interviews_master.json` P01–P05; Run 1 D2, D4, D5.
**Set justification:** two personas, one empathy map, three scenarios, one blueprint. This is the minimum set that covers the two archetypes carrying the most severe Run 1 pain, the one journey the app most changes, and the backstage dependencies where the service actually fails. It is not expanded.

---

## 1. Personas

### 1.1 Amina R. — quota-dependent national reporter
**Confidence: [SIMULATED]** — composite of P01, P02 and survey rows R01–R08; the archetype and its pains are [SOURCED] to Run 1 D4/D5, the individual is not.

| Field | Content |
|---|---|
| **Role** | Football reporter, mid-size national daily. Files 1–2 pieces per matchday, covers her national team plus regional interest matches. |
| **Track** | Member Association quota → control key → Media Hub. No direct FIFA relationship. |
| **Goals** | Be in the building for every match her team plays; get mixed-zone quotes nobody else has; keep filing after her team goes out, because her editor still wants tournament coverage. |
| **Context of use** | Applies from her newsroom months ahead. During the tournament she works from press centres, hotel lobbies, airports and coaches between host cities. Checks status between filing deadlines, not at leisure. |
| **Constraints** | Her access is set by a federation she does not work for and cannot influence. Her budget is booked before her access is confirmed. Her plans change when the team loses. She has no named contact at FIFA. |
| **Technical environment** | Personal phone as primary device, laptop for filing. Roaming data across three countries, unreliable in venues. Uses the app because she has to, not because she likes it. |
| **Quote** | *"Tell me when something about my status changes. Don't make me discover it by trying to do something and failing."* (P01) |
| **Abandons when** | The app becomes a place she checks and learns nothing. If status is stale or absent twice in a row, she reverts to WhatsApp groups and phoning her federation contact — and does not come back. |

### 1.2 Tomás L. — rights-holder crew coordinator
**Confidence: [SIMULATED]** — composite of P04 and survey rows R15–R18; the track structure is [SOURCED] to Run 1 D4/D6 item 4.

| Field | Content |
|---|---|
| **Role** | Accreditation and logistics coordinator for a rights-holding broadcaster. Manages credentials for 40–120 people: camera, sound, producers, on-air talent, technicians. |
| **Track** | FIFA Media Partnerships — parallel to and siloed from the Media Hub. Has a named contact and uses them. |
| **Goals** | Every crew member holds the right zone access on the right day; no one is stopped at a barrier; reassignments propagate before they matter; equipment clears customs in three jurisdictions. |
| **Context of use** | Operations room or IBC, dual monitors, working a spreadsheet that is the real source of truth. Reassigns people daily as the schedule and story priorities shift. |
| **Constraints** | Contractual obligations to deliver coverage; his failures are broadcast failures. Manages people across time zones who each have individual visa and customs status. Cannot see into vetting or consular decisions any more than Amina can. |
| **Technical environment** | Desktop-first. Would use an API or bulk upload if offered; currently reconciles by hand. Phone only as an escalation device. |
| **Quote** | *"Fix it for the outlets that don't have someone like me doing this full-time. We're fine."* (P04) |
| **Abandons when** | The tool adds per-person clicks without adding bulk capability, or floods him with notifications scoped for individuals. He keeps the spreadsheet and treats the platform as a form to satisfy. |

---

## 2. Empathy map — Amina R. only
**Confidence: [SIMULATED]** — derived from P01 and P02 verbatims. Built for Amina and not Tomás because she is the persona whose experience the product actually changes; Tomás already has a human contact doing what the product proposes to do.

| Quadrant | Content |
|---|---|
| **Says** | "Has anyone heard anything?" · "I emailed them twice." · "Is my code coming or should I chase the federation?" · "Wait — I can't request that match?" |
| **Thinks** | *Did I do something wrong, or is it just numbers?* · *If I book the flight now and the access doesn't come, that's my money.* · *Someone knows the answer to this and it isn't me.* |
| **Does** | Refreshes a status page that hasn't changed. Asks in journalist group chats before asking any official channel. Books travel on the assumption of approval. Discovers changes by attempting an action and failing. |
| **Feels** | Uncertainty during the wait; embarrassment at having to ask peers what should have been told to her; acute frustration at the barrier; resignation that this is how it works. |
| **Pains** | No visible status between submission and decision. No reason attached to outcomes. No named person to ask. Access that changes without notice. Costs already sunk when the change lands. |
| **Gains** | Knowing where she stands, even when the answer is bad. Enough warning to re-plan rather than react. A reason she can act on or learn from. One place that is more current than the group chat. |

---

## 3. Scenarios — Amina R.
**Confidence: [SIMULATED]** for narrative; failure classes are **[SOURCED]** to Run 1 as marked.

### 3.1 Typical — the process works
Amina's federation issues her control key six weeks before the deadline. She creates her Media Hub account, submits, and waits five weeks with no visible status; she chases twice and is told the federation is also waiting. Approval arrives in early March. She books travel, collects her card at the accreditation centre, and for her team's group matches she requests one media ticket per matchday through the app. It works. She files. The parts that functioned were the parts with a defined transaction; the five-week silence was the only thing she would change.

### 3.2 Critical — access changes after confirmation
*Failure class: elimination-driven quota reallocation — Run 1 D5 #17 [INFERRED].*
Her team loses in the round of 16. Two days later her media ticket request for the quarter-final silently fails. No message preceded it. She learns from another journalist that her federation's quota contracted when the team went out. She has already paid for the flight to the next host city. She spends two days working contacts and eventually covers the match through a wire pool arrangement. The access change was correct policy; the absence of notice is what cost her the two days and the fare.

### 3.3 Tragic — accredited and still unable to enter
*Failure class: valid accreditation nullified by host-state immigration — Run 1 D5 #1, AIPS protest 6 June 2026 [VERIFIED].*
Amina is approved. Her visa arrives late and single-entry; nobody flags what that means for a tournament spread across three countries. She covers two matches, then follows her team across a border for the next fixture. She cannot re-enter. Her accreditation remains valid and entirely useless — the platform shows her approved, her match ticket confirmed, and a venue she cannot physically reach. She files the rest of the tournament from a country her team is no longer playing in, and her outlet questions the cost of sending her. **No product decision in this repo prevents this outcome.** The failure is consular. What the platform could have done is warn her, at the moment her visa class was known, that single-entry and cross-border fixtures are incompatible.

---

## 4. Service blueprint — accreditation to matchday
**Confidence: [SOURCED]** for stages, actors and systems (Run 1 D2, D6). **[SIMULATED]** for the emotional trajectory. **[ASSUMPTION]** where marked in-row.

**Why there is no separate Journey Map:** the emotional trajectory is carried as a column in this blueprint, so a standalone journey map would restate the same sequence with less operational detail. The omission is a decision, not an oversight.

**ZV** = FIFA currently has zero visibility into this step.

| Stage | User actions | Emotional trajectory | Frontstage | Backstage | Support processes | Evidence | Failure point | Wait state |
|---|---|---|---|---|---|---|---|---|
| **1. Quota allocation** | None — she is not a party to it | Unaware | Nothing shown to her | FIFA sets per-MA quota by participation and qualification | Quota Management System | None visible to applicant | Allocation logic opaque; perceived inequity | Indefinite; she doesn't know it's happening |
| **2. Control key distribution** | Waits; chases federation | Anxiety, dependence | Email or message from federation, if any | MA media department distributes keys at its own pace **ZV** | MA internal process, no FIFA oversight | The key itself | Slow or uneven distribution; freelancers may get none | Days to months, unbounded |
| **3. Media Hub account** | Registers, awaits approval | Mild friction | Account status | FIFA account approval queue | Media Hub | Approved account | Approval lag; account non-transferable | Days |
| **4. Application** | Completes form with key | Relief at having acted | Submission confirmation | Application enters review | Media Hub form | Submission receipt | Conflicting published deadlines | — |
| **5. Security vetting** | Nothing. Cannot see or influence | Uncertainty → suspicion | **Nothing shown** | National authorities screen applicants **ZV** | Non-FIFA government systems | None | Opaque; no appeal route; no reason on refusal | Weeks — the longest silence in the service |
| **6. Confirmation / refusal** | Receives outcome | Relief or a closed door | Approval or refusal message | FIFA issues decision | Media Hub notification | Approval letter | Refusal carries no reason and no exit (T1) | — |
| **7. Visa / entry permit** | Applies separately, alone | Stress; cost exposure | Consular process, outside the platform | Host-state consulates decide **ZV** | Consular systems | Visa | Denial or single-entry after approval; incompatible with cross-border fixtures | Weeks, compressed by late confirmation |
| **8. Card collection** | Attends collection point | Procedural | Accreditation centre desk | Card production and passport validation | Accreditation centre | Physical card + bib | Locations and validation steps undisclosed **[ASSUMPTION]** | Queue |
| **9. Media ticket request** | Requests one per matchday | Routine, then contested | App request screen | Allocation against remaining quota | Media Hub / FIFA Media App | Match ticket | One-per-day cap; oversubscription | Hours |
| **10. Mid-tournament reallocation** | Attempts a request; it fails | Confusion → anger | **Nothing — no notice precedes the change** | Quota recalculated on elimination **ZV** | QMS ↔ MA | None | Discovery-by-failure (T2); learned via peers | Change is live before she knows |
| **11. Matchday access** | Presents card at gate | Confidence → possible humiliation | Turnstile, security staff | Venue access lists, biometrics | Venue systems | Scan result | Two-token confusion (T3); gate list stale vs app state (P05) | 0–20 min at the barrier |

**Zero-visibility summary:** stages 2, 5, 7 and 10. FIFA cannot see federation key distribution, national security vetting, consular decisions, or the moment an applicant discovers her own reallocation. Three of the four are outside FIFA's authority; **stage 10 is the one FIFA both causes and fails to communicate** — which is why it is the highest-value target in this repo.

---

✅ GATE 5 COMPLETE — `05_ARTIFACTS.md`
