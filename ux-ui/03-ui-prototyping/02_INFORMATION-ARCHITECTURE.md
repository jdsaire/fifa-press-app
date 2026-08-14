# 02 — CARD SORT & INFORMATION ARCHITECTURE

**SIMULATED — NOT EVIDENCE**

**Repo path:** `ux-ui/03-ui-prototyping/02_INFORMATION-ARCHITECTURE.md`
**Participants:** W1–W6, synthetic, carried from `02-ideation/01_WORKSHOP-PROTOCOL.md` §1. Third and final reuse.
**Inputs:** `02-ideation/05_CONCEPT.md`; `02-ideation/06_HANDOFF.md` §6–§7; `01-design-research/05_ARTIFACTS.md`; existing app surfaces; `01_BENCHMARKING.md` adopt table.

---

## 0. Roster and standing, before any finding

### 0.1 The roster, as seated

Carried verbatim from `01-design-research` archetypes via `02-ideation/01_WORKSHOP-PROTOCOL.md` §1. The tracks are `[SOURCED]` to Run 1 D4; the people are `[SIMULATED]`.

| ID | Seat | What they push for in a sort |
|---|---|---|
| **W1** | Football reporter, mid-size national daily — the Amina archetype | Her own entitlement, which a federation she does not work for can change mid-tournament |
| **W2** | Freelance reporter, no fixed outlet | Explanation and recourse. The weakest appeal path in the evidence base (D5 #11); the person Principle 3 exists for |
| **W3** | Media officer, Member Association | The distribution layer she administers but did not set; absorbs anger for decisions taken above her |
| **W4** | FIFA Event Media Operations, accreditation | What the system emits and must answer for — he owns the state changes |
| **W5** | Crew coordinator, rights-holding broadcaster — the Tomás archetype | **The dissenter.** 40–120 credentials, a named contact, and a spreadsheet that already works |
| **W6** | Venue accreditation supervisor, host city | The barrier, worked from a list that is stale by the time it matters |

### 0.2 The constraint this gate operates under

`06_HANDOFF.md` §7 is explicit that prototyping inherits **no information architecture** — it is this mandate's to make. Equally explicit is the instruction in §7 not to cite any W1–W6 statement as a user need. Both are honoured here: the sort below is a *method for surfacing disagreement the author already knew existed*, and every decision it produces is defended on stated grounds rather than on a participant having said it. Where a decision rests on a participant alone, it is tagged `[SIMULATED]` and is weaker for it.

---

## 1. Card inventory

Twenty-two cards. Every card traces to the concept, the three interactions, or a surface that already exists in the app. No invented content types.

| # | Card | Traceability |
|---|---|---|
| C01 | Match list | Existing app surface (`/`) `[SOURCED]` |
| C02 | Match detail | Existing app surface (`/events/{id}`) `[SOURCED]` |
| C03 | Access request form | Existing app surface (`/register/{id}`) `[SOURCED]` |
| C04 | Request-submitted confirmation | Existing app surface (badge) `[SOURCED]` |
| C05 | Per-match request count | Existing app surface `[SOURCED]` |
| C06 | Sign-in form | `P-PROTOTYPE_FIFA_Run4-Scope.md` §5 `[SOURCED]` |
| C07 | What changed | `05_CONCEPT.md` §1 `[SOURCED]` |
| C08 | Why it changed (reason) | §3 Principle 2; `06_HANDOFF.md` §6 rec 3 `[SOURCED]` |
| C09 | What you can do now (next step) | §3 Principle 2; rec 3 `[SOURCED]` |
| C10 | Entry timestamp | §4.3, ID-10 `[SOURCED]` |
| C11 | Urgency, derived from change type | ID-13; rec 5 `[SOURCED]` |
| C12 | The full history of changes | ID-02 `[SOURCED]` |
| C13 | Most recent state, held on device, readable offline | ID-10 `[SOURCED]` |
| C14 | A change that has not happened yet | ID-06, Interaction 4.2 `[SOURCED]` |
| C15 | Which fixture my access depends on | Interaction 4.2; CSV admitted at Gate 6 `[SOURCED]` |
| C16 | What I currently hold | §4 `[SOURCED]` |
| C17 | Gate check result at the venue | Interaction 4.3 `[SOURCED]` |
| C18 | My credential identity | Run 1 D2 `[SOURCED]` |
| C19 | My track | ID-04; Run 1 D4 `[SOURCED]` |
| C20 | What this service does not do | §5; `06_HANDOFF.md` §5 `[SOURCED]` |
| C21 | Escalation route when the two sides disagree | §4.3 `[SOURCED]` |
| C22 | Stale-state indicator | §4.3, ID-10 `[SOURCED]` |

---

## 2. Open sort

Six sorts, unmoderated grouping, participant-chosen labels. `[SIMULATED]`

### 2.1 Where participants agreed

Five of six placed **C07, C08, C09, C10** together as an indivisible unit. W4's framing is the one that carries weight, because he is the person who must answer for what the system emits: these are not four things that belong near each other, they are four fields of one thing, and a sort that separates them has produced a malformed entry. This restates `06_HANDOFF.md` §6 rec 3 — reason and next step are schema, not copy — which makes it the one grouping in this sort that was already a build instruction before anyone sorted anything. `[SOURCED]`

Five of six also separated **C06** from everything else. It is a door, not a room.

### 2.2 Where they split

| Contested card | The split | Reading |
|---|---|---|
| **C12** (history) | W1, W2, W6 → its own destination. W4, W5 → a property of the record, not a place | The deepest disagreement in the sort. Resolved in §4.2 |
| **C01, C02** (matches) | W1, W3, W6 → top-level; a fixture list is what an app about a tournament *is*. W2, W4, W5 → subordinate; a fixture matters only where an entitlement hangs on it | The real IA fight. Resolved in §4.2 |
| **C03, C04** (request + confirmation) | W1, W3 → with the match. W4 → with the record, since a request changes entitlement state. W6 → with neither; "a thing you do", not a thing you read | Resolved in §4.2 |
| **C16 vs C18/C19** | W3 grouped all three as "my status". W1 split them: identity and track are stable, what she holds changes weekly | Adopted W1's split |
| **C17, C21, C22** | W6 → one group, "the barrier". W1 → C22 belongs with every change everywhere, not with the barrier | Both correct; see §4.4 |
| **C20, C21** | W2 → these two are the *first* thing a person reads, not help-desk residue. Every other participant filed them last | Partly adopted; see §4.4 |

### 2.3 The two dissents that changed the outcome

**W5 refused the frame.** He sorted once, then said the exercise assumes a person looking at their own record, and that he manages between forty and a hundred and twenty of them. His grouping was by *people and crews* — a roster with state hanging off each row. He was explicit that a per-person record view is not something he would open, and that if the product's primary surface is one person's log, he reads it as not built for him and keeps the spreadsheet.

This is a **navigation** objection, and it is not the same claim Principle 3 already answers. Principle 3 scopes push volume inversely to a track's human support; W5 is talking about what the app's front door *is*. `06_HANDOFF.md` §6 rec 5 requires track scoping to be a precondition rather than a setting — that governs notification, and it does not decide navigation. Recorded as an open item for Gates 4 and 5, not silently absorbed. `[SIMULATED]`

**W4's counter,** from the side that owns the state changes: a bulk roster surface carries a different permission model — one person reading many people's entitlements — and building it inside a v1 aimed at the unserved track would consume the whole build. Noted; no bulk surface is specified in this dossier, and the objection stands unresolved rather than answered.

**W2 attacked the filing, not the frame.** She put C20 and C21 first on the grounds that a person with no federation behind her needs to know what the service will not do and where to go when it refuses, *before* she needs anything else. She is characteristically the participant who challenges scope this way — the protocol's own twist table anticipates her arguing that the platform's boundaries are the main event. Her placement is not adopted wholesale, because a person who already holds accreditation opens the app to read state, not to read disclaimers. What is adopted is that C20 and C21 must be reachable from the point of refusal rather than buried in a help section — see §4.4.

---

## 3. Closed validation

Labels from the open sort, tested back against the group. `[SIMULATED]`

| Proposed label | Survived? | Outcome |
|---|---|---|
| **My Access** | Yes, 6/6 | Adopted as the primary section |
| **Entries** | **No** | W1 and W2 read it as ledger jargon; W6 asked entries into *what*. Fails |
| **Updates** | **No** | W4's objection carried: an update is something the sender did; the object here is the record's own history. Also implies optionality |
| **History** | Partial, 4/6 | Survives as a *sub-label inside* the record, not as a destination |
| **Matches** | Yes, 5/6 | Survives, demoted; see §4.2 |
| **Register** | **No, 0/6** | Unanimous rejection. Beside a sign-in form it reads as account creation. Highest-value naming finding in this gate |
| **Request access** | Yes, 6/6 | Adopted, replacing "Register" |
| **My credential** | Partial | Kept for identity only (C18, C19); not extended to C16 |
| **What I hold now** | Yes, 5/6 | Adopted as the record's headline state |

---

## 4. Information architecture

### 4.1 Ontology — what each thing means

| Decision | Resolution | Rationale | Tag |
|---|---|---|---|
| **The object** | **Access Record** | Inherited, not reopened | `[SOURCED]` |
| **The unit inside it** | **Change** — "what changed", not "entry" or "update" | "Entry" failed validation and breaks in translation (§5); "update" misattributes agency to the sender. The record accumulates *changes* | `[SIMULATED]` |
| **Register → Request access** | Renamed everywhere | "Register" implies account creation, and a sign-in form arrives in Gate 5. Nothing in this app registers a person; it requests access to a match | `[SIMULATED]` |
| **Accreditation vs match access** | Two named things: **Accreditation** (standing; who you are; valid until) and **Match access** (per fixture; requested, granted or not) | `06_HANDOFF.md` §6 rec 6 requires the two-token disclosure to be carried at approval regardless of whether the confusion is novice-only. Naming them distinctly is the cheapest way to carry it. It does **not** resolve the unknown, and nothing depends on it being resolved | `[SOURCED]` for carrying it; `[ASSUMPTION]` for these specific words |
| **"Valid until"** | Accreditation is never "approved" alone; it is approved *until* a date, and certain identity changes invalidate it independently of any quota movement | Gate 1 adopt #11 | `[SOURCED]` |
| **Stale** | A displayed state that is knowingly not current, always labelled with its own timestamp | §4.3 — a stale state that announces itself is recoverable | `[SOURCED]` |

**On not naming the mechanism.** "Change" names what happened to the record. It does not name a notification, an alert, or a message — consistent with the concept's own reasoning about why it is Access Record and not AccessAlerts (§1).

### 4.2 Taxonomy — how things are arranged

Three top-level sections plus a door.

| Section | Contains | Note |
|---|---|---|
| **My Access** | C16 (headline: what I hold now) · C12 as history *within* the record · C07–C11 as the fields of each change · C14 foreseeable changes · C18, C19 identity and track, secondary · C22 staleness, always present | The primary surface. History is a region of this section, not a sibling destination |
| **Matches** | C01, C02, C15, C03, C04 | Demoted from the existing app's primary position. A fixture appears here as something access can depend on |
| **Help** | C20, C21, staged guidance | Organised by lifecycle stage, per Gate 1 adopt #8 |
| **Sign in** | C06 | A door. Not a section |
| *Unplaced* | C05, C17 | See §4.4–§4.5 |

**Why history is not a destination.** A separate History tab invites the question "is there anything new?", which is the poll model the concept exists to remove (Gate 1, DHL §2.2). The most recent change *is* the headline; older ones sit beneath it. Making history a place to visit reinstates checking as the user's job.

**Why C03/C04 land in Matches despite W4's objection.** W4 is right that a request changes entitlement state — and so it does, by writing a change into the record. The *act* of requesting is bound to a fixture and belongs beside it; the *consequence* appears in My Access like every other change. Splitting act from consequence this way is what CH-1 requires anyway.

### 4.3 Choreography — behaviour over time

The parameter Access Record turns on.

| # | Rule | Basis |
|---|---|---|
| CH-1 | A change is written before it takes effect on any path. If it cannot be written, the change does not take effect. There is no second write path | §4.1 + `06_HANDOFF.md` §6 rec 2 `[SOURCED]` |
| CH-2 | Newest change is the headline. Ordering is by when a change *takes effect*, not when it was written — a foreseeable change resolving Saturday sits above one written later that resolves next month | ID-06 + Gate 1 adopt #3 `[SIMULATED]` |
| CH-3 | A superseding change displays the value it replaced alongside the new one. A change with nothing visible to compare against is not legible as a change | Gate 1 adopt #6 `[SOURCED]` |
| CH-4 | Interrupt vs wait is decided by change type and track, never by recipient configuration. There is no notification preference screen — scoping is a precondition, not a setting | ID-13 + rec 5 `[SOURCED]` |
| CH-5 | Below a materiality threshold a change is written to the record but does not interrupt. **The threshold values are undetermined**; Gate 4 must set them as `[ASSUMPTION]` | Gate 1 adopt #1 `[ASSUMPTION]` |
| CH-6 | What does *not* interrupt is published in Help. An unexplained silence is indistinguishable from a broken system | Gate 1 adopt #2 `[SOURCED]` |
| CH-7 | A foreseeable change is worded as a condition, never as a commitment, and is superseded by the landed change rather than deleted | §4.2 `[SOURCED]` |
| CH-8 | Staleness is a property of every surface, not a state of one. Any surface reading cached data carries its own timestamp | §4.3 `[SOURCED]` |
| CH-9 | A change with no actionable next step still says so, names who decided, and states what remains open. A change that cannot populate reason and next step is malformed and fails at build time | Principle 2 + rec 3 + Gate 1 adopt #4 `[SOURCED]` |
| CH-10 | Every surface must resolve its headline with no signal. The offline case is a first-class state, not a degraded one | ID-10; protocol §4 constraint round `[SOURCED]` |

### 4.4 The barrier, resolved three ways

W6 and W1 were both right, and W2 was right about reachability.

- **C17** (gate check result) is a moment inside Matches — it belongs to a specific fixture.
- **C22** (staleness) is CH-8, a property everywhere.
- **C21** (escalation) and **C20** (what this does not do) live in Help *and* must be reachable directly from a failed gate check and from any refusal. W2's placement was wrong as a default landing and right as a routing rule: the person who needs the boundary stated is the person who just hit it.

**Unsettled dependency.** `06_HANDOFF.md` §6 rec 4 states that venue access list ownership is unresolved and that prototyping must not design as though it were settled. C17 is therefore specified as a *displayed outcome* with no assumption about which system owns the list or how the app learns the result. Gates 5 and 6 inherit that constraint. `[SOURCED]`

### 4.5 Unplaced card, and why

**C05, per-match request count.** Inherited from the existing app, where it showed how many people had requested access to a match. It has no role in the concept: it tells Amina nothing about her own entitlement, and in a quota context a visible count implies a competition she cannot act on — drifting toward the quota-politics boundary (§5). **Recommended for retirement in Gate 5**, flagged here rather than silently dropped, since it is working code today.

---

## 5. Trilingual check — EN/ES/PT

Only labels that genuinely break are listed. Everything else translates without incident and is not enumerated.

| Label | Break | Resolution |
|---|---|---|
| **Entry** | Severe. ES/PT *entrada* means a stadium ticket in exactly this domain. A log entry and an admission token would share one word in a product whose purpose is distinguishing them | Already rejected in §4.1 on validation grounds; translation independently forbids it. Use *cambio* / *mudança* |
| **Record** | ES/PT *registro* also carries "to register" — the very verb removed in §4.1. Reintroducing it as the object's name restores the confusion in translation only | **Access Record** stays in English as a product name; the ES/PT surface uses *mi acceso* / *meu acesso* for the section and avoids *registro* as a noun |
| **Stale** | No single-word ES/PT equivalent; literal translations read as "spoiled" | Rendered as an explicit timestamp phrase rather than an adjective — *actualizado por última vez…* / *atualizado pela última vez…*. Consistent with CH-8, which wants a timestamp regardless |
| **Request access** | No break. ES *solicitar acceso*, PT *solicitar acesso* | Listed only to confirm the gate's main ontology change survives translation |

---

## 6. Navigation model

**Persistent top-level:** My Access · Matches · Help — with the theme trigger top right at every breakpoint (Gate 3 owns its behaviour). Sign in sits outside the primary set.

**Default landing is My Access**, not the match list. The current app lands on fixtures; the concept's whole claim is that the record changes underneath you. Landing on fixtures would restate the app the repo already has.

**On mobile,** My Access is a single scrolling column: current state, then staleness, then changes newest-effective-first. Amina reads it between filing deadlines, on roaming data, in a concourse — the headline must resolve without a tap and without a network (CH-10).

### Rejected alternative — match-first navigation

Keep the existing structure: fixtures at the root, access shown per match. **Rejected.**

It was the cheaper path and it preserves working code, which is why W1, W3 and W6 sorted toward it. But it distributes a person's entitlement across as many screens as there are fixtures, so no surface can answer "what changed about my access" at all — the person assembles the answer by visiting matches and noticing something is different. That is discovery-by-failure with extra steps, and it is the failure this mandate exists to remove. The demotion of Matches is the cost of the concept, taken deliberately.

---

## 7. Carried into later gates

| # | Item | Owner |
|---|---|---|
| 1 | W5's navigation objection — a per-person primary surface may make the tool unusable for a bulk coordinator. Notification scoping does not answer it | Gate 4 tasks, Gate 5 screens |
| 2 | CH-5's materiality threshold has no values | Gate 4 |
| 3 | C05 retirement recommendation | Gate 5 |
| 4 | Venue access list ownership is unsettled; C17 must not assume a source | Gates 5 and 6 |
| 5 | The "Register" → "Request access" rename touches existing routes and component copy | Gate 3 rebrand decision, Gate 7 file scope |

---

✅ GATE 2 COMPLETE — `02_INFORMATION-ARCHITECTURE.md`
