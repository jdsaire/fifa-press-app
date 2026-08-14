# 01 — COMPETITIVE BENCHMARKING

**Repo path:** `ux-ui/03-ui-prototyping/01_BENCHMARKING.md`
**Question this gate answers:** which patterns serve a state-change record — not which sites look good.

---

## 0. Standing limit — read this before the findings

The most relevant screens in the tracking, airline and travel-authorization properties sit behind a booking, a shipment number, or a submitted application. **No accounts were created, no applications submitted, no credentials entered, no shipment or booking numbers used.** What is publicly reachable is tagged `[OBSERVED]`; what is described in the operator's own published documentation is `[REPORTED]`; the rest is out of reach and named as such here rather than deferred to Gate 8.

This matters most for the properties that are most relevant. The authenticated status view — the screen a person actually stares at when their state has changed — was not seen on any of the three comparators. What was reachable was the operator's own account of how those systems behave, which is often more precise about thresholds and triggers than an interface walkthrough would be, and less reliable about what the experience feels like.

The Play Store listings were not consulted. They yield marketing copy and vendor-supplied screenshots, nothing inspectable, and the football properties' own public sites answered the visual-language question directly. Their absence costs this gate nothing.

| Source | What it can honestly answer | Tag achieved |
|---|---|---|
| `realmadrid.com` | Football visual language, fixture presentation, state qualifiers on scheduled events | `[OBSERVED]` |
| `mancity.com` | Fixture card structure, multi-team fixture streams | `[OBSERVED]` public listing surface |
| `maersk.com` | Identifier-keyed tracking without an account; what unauthenticated access withholds | `[OBSERVED]` public + `[REPORTED]` support docs |
| `dhl.com` | Push-versus-poll status architecture; notification preference management | `[REPORTED]` developer and support docs |
| `delta.com` / `pro.delta.com` | Disruption notification thresholds, suppression rules, channel fallback | `[REPORTED]` published policy |
| `israel-entry.piba.gov.il` (via PIBA and US Embassy publications) | Application-to-decision authorization; authorization-versus-entry separation | `[REPORTED]` |

---

## 1. Football properties — visual language only

### 1.1 Real Madrid

**Navigation model.** `[OBSERVED]` Top-level split by sport, then by team, then by function (news, tickets, club, shop). Commercial and membership destinations sit in the header alongside editorial. The fixture list is a distinct block on the home page rather than a page one navigates to.

**State and status communication.** `[OBSERVED]` This is the genuinely useful finding. Fixture entries carry explicit qualifiers where a detail is unresolved — a date shown with a parenthetical noting time to be confirmed, a training session marked closed-door, a broadcast row listing the carrying channels or noting a press conference will not occur. The scheduled event is not presented as fully determined when it is not. The interface has a vocabulary for partial knowledge and uses it inline, next to the item, rather than in a footnote.

**Notification surfacing.** `[OBSERVED]` A subscribe affordance sits on the upcoming-events block. Nothing about its behaviour after subscription is inspectable.

**Theme handling.** `[OBSERVED]` Light, high-contrast, white-dominant, with a declared light theme colour. No dark mode was found on the public site. `[INFERRED]` For a club whose identity is literally white, dark mode is an identity problem, not only a technical one — worth noting because this repo has committed to first-class dark mode and cannot borrow a football-brand precedent for it.

**Typography and colour.** `[OBSERVED]` Restrained palette: white ground, dark text, gold and navy as accents. Sponsor and competition logos supply nearly all the colour variance. Type is used at a strong size hierarchy for headlines with a much quieter body treatment.

**Responsive behaviour.** `[OBSERVED]` Distinct desktop and mobile image renditions are requested by the same markup, indicating breakpoint-aware asset delivery rather than a single fluid image.

### 1.2 Manchester City

**Navigation and fixtures.** `[OBSERVED]` The public listing surface shows fixture cards structured identically across competitions and squads: competition label, date, both clubs, kickoff time with an explicit timezone abbreviation, and a repeated all-fixtures affordance per block. Men's, women's, academy and EDS fixtures interleave in one stream, each labelled by which team it belongs to.

**Why the timezone label matters here.** `[OBSERVED]` Every time is stamped with its zone. `[INFERRED]` For a single-city club this is a courtesy to an international audience; for this repo it is closer to a requirement, since Amina works across three host countries and a kickoff time without a zone is a defect rather than a nicety.

**Theme, type, colour.** `[OBSERVED]` Sky blue as the single identity colour against a neutral ground; heavy reliance on photography for visual weight; advertising slots interleaved into the editorial stream.

### 1.3 What the football properties cannot answer

Both are broadcast surfaces. Content flows outward to an audience that has no state of its own — no application, no entitlement, nothing that can change while they are not looking. Neither club site contains the pattern this project turns on. They are used here for football visual language and fixture presentation, and for nothing else.

---

## 2. Pattern-class comparators — the systems that tell you something changed

### 2.1 Maersk — identifier-keyed status without an account

`[REPORTED]` Maersk's support documentation states plainly that a shipment or container can be tracked without logging in, and that the unauthenticated view is deliberately reduced: current location, previous movements, and expected schedule only.

`[OBSERVED]` The public support surface is organised by lifecycle stage — pre-booking, booking, post-booking, arrival — rather than by feature. Support entry points (live chat, raise a case, find a local office) sit persistently at the foot of the support content.

**Why this is the most transferable finding in the gate.** The unit of access is the *shipment identifier*, not the account. Anyone holding the number sees the state. That is a direct structural analogue to a credential number, and it bears on a real decision in Gate 5: whether Amina's Access Record requires authentication to read, or whether the credential identifier is itself the key. Maersk's answer — identifier gets you the state, account gets you everything else — is a defensible middle position, and notably it is the *history* (previous movements) that survives into the unauthenticated view, not just the current state.

**The staged-lifecycle support taxonomy** is also directly relevant: accreditation has the same shape (before application, applied, decided, in-tournament, matchday), and organising help by stage rather than by feature is a pattern this app could adopt cheaply.

### 2.2 DHL — push versus poll, stated as an architecture

`[REPORTED]` DHL publishes both a request-response tracking interface and a push variant that proactively sends status updates. The published rationale for the push version is explicit: consumers no longer need to actively request the latest update in order to have it. The push endpoint documentation also describes a retry mechanism so that notifications are not lost when the receiving system is unavailable.

`[REPORTED]` On the consumer side, DHL's on-demand delivery service centres on managing delivery and notification preferences, with actions such as changing the delivery date, redirecting to another address, authorising release without signature, or postponing delivery — and its documentation describes a *revised* delivery date displayed against the original.

**Three findings that transfer.**

1. **Push exists because polling fails the user.** DHL's own framing of why the push API exists is the argument for Principle 1, stated by an operator at scale. Amina refreshing a status page that has not changed (`01-design-research/05_ARTIFACTS.md` §2) is the poll model failing exactly as described.
2. **Retry-on-failure is the correct failure posture.** The concept already holds that a failed push leaves the entry intact and the failure mode is a late message rather than a missing fact (`05_CONCEPT.md` §4.1). DHL implements precisely that at the transport layer.
3. **"Revised against original" is a display pattern, not just a data point.** Showing the changed value alongside what it replaced is how a change becomes legible rather than merely current. Gate 5's state matrix should specify this for any entry that supersedes a prior entitlement.

**Where DHL diverges and must not be copied.** Its notification model is heavily *preference-driven* — the user tunes which events reach them. The Access Record explicitly rejects this: urgency derives from the change type, not from recipient configuration (ID-13, `05_CONCEPT.md` §2.1). DHL's preference centre is the wrong pattern for this product and is listed as avoid below.

### 2.3 Delta — the closest analogue in the set

`[REPORTED]` Delta's published policy for its automated notification system is unusually specific, and nearly every clause maps onto a decision this dossier must make.

**Notification thresholds are defined numerically.** Schedule-change notification triggers on departure fifteen minutes or more earlier, departure thirty minutes or more later, arrival thirty minutes or more later, or an equipment product change. Delay notification triggers at thirty minutes or more, or departures fifteen minutes or more early.

**Non-notification is specified as explicitly as notification.** The policy enumerates conditions under which no notification is sent — minor time changes below the thresholds, equivalent equipment substitutions, a bare flight-number change, and certain special-service records. Responsibility in those cases transfers to the agency.

**The system waits for a resolution before speaking.** During an irregular-operations event, the notifier waits for the record to be rebooked with new flights before contacting the customer.

**It speaks anyway when there is no resolution.** For records that cannot be rebooked, a notification is still sent stating that rebooking is being attempted and where to go for more information, with a subsequent alert if a rebooking is later added within a defined window.

**Timing respects the human.** Calls are placed within a defined daytime window in the customer's own timezone based on the next departing city; for disruptions outside that window, contact is made a fixed interval before the original departure time, explicitly reasoned as giving the customer enough time to reach the airport.

**Channel fallback is a defined chain.** Email always sends; SMS goes to a valid mobile; a phone call is attempted only where no mobile is detected. Preferences are consulted first, then the booking record.

**Why this is the gate's most valuable source.** Four things transfer almost directly:

- **A materiality threshold is mandatory.** Delta does not notify on every change; it notifies on changes that cross a stated bar. Gate 4 and Gate 5 need the equivalent for entitlements, or Principle 3's inverse scoping degrades into noise for Amina.
- **"We are working on it" is a legitimate entry.** The un-rebooked notification is a message with no outcome — it states the situation, the effort in progress, and where to go. This is direct precedent for Principle 2's hardest case, where the honest next step is "nothing you can do yet."
- **Notification timing is computed from the consequence, not from the change.** Delta anchors contact to how long the person needs to act, not to when the disruption occurred. That is Interaction 4.2 in operation.
- **The suppression list is itself a deliverable.** Publishing what does *not* trigger a message is what makes silence interpretable. Without it, a quiet system is indistinguishable from a broken one.

**The trap.** Delta's model discharges responsibility to a travel agent for the suppressed cases — someone whose job is to catch what the automated system deliberately dropped. Amina has no travel agent. Principle 3 exists precisely because that intermediary does not exist for her track, so this repo may adopt the threshold discipline but must not adopt the handoff that makes it safe for Delta.

### 2.4 ETA-IL — application-to-decision, and the two-token model in the wild

`[REPORTED]` The Israeli travel-authorization scheme is structurally the nearest public analogue to accreditation itself: an applicant submits personal, passport and travel details ahead of travel; a decision is returned by email, typically within a window measured from minutes to roughly seventy-two hours; approval is valid for a fixed period or until the passport expires, whichever comes first; and the authorization is bound electronically to the passport rather than issued as a physical document.

`[REPORTED]` Re-application is required on specified identity changes — a new passport, or a change of name, gender or country of citizenship — rather than on a fixed renewal schedule.

**The finding that matters most in this entire gate.** `[REPORTED]` The US Embassy's published guidance states that the authorization only allows a visitor to *reach* the border crossing; it does not grant permission to enter. On arrival, a border officer determines whether the visitor is authorized to enter.

That is the two-token model, in production, in a governmental system, described in one sentence by the receiving state. Run 1 documented the same split in FIFA's own advisory: accreditation grants media-centre entry, while a separate per-match media ticket is required for photo positions, mixed zone and press conferences (`RUN1_EVIDENCE-BASE.md` D1 item 2, D5 pain point 2).

**What this does and does not license.** It does **not** resolve the two-token unknown — whether the confusion is a novice gap or persistent — and per the inherited contract nothing here depends on resolving it. What it establishes is narrower and still useful: the pattern of *separating the token that lets you travel from the token that lets you in* is not a FIFA idiosyncrasy, it is how travel authorization commonly works, and at least one operator states the boundary explicitly in plain language at the point of guidance. Gate 2's ontology work can therefore draw on existing public phrasing conventions rather than inventing vocabulary from nothing.

**Validity as a first-class attribute.** Both the validity window and the identity-change re-application rule are worth carrying into Gate 6's entity model: an accreditation is not simply approved or refused, it is approved *until*, and certain changes to the holder invalidate it independently of any quota movement.

---

## 3. Adopt / avoid / not applicable

| # | Pattern | Source | Verdict | Reasoning |
|---|---|---|---|---|
| 1 | Notify on defined materiality thresholds, not on every change | Delta `[REPORTED]` | **Adopt** | Without a bar, Principle 3's scoping produces noise; with one, silence becomes interpretable |
| 2 | Publish what does *not* trigger a notification | Delta `[REPORTED]` | **Adopt** | Makes silence readable rather than ambiguous; Gate 5 owes a suppression list |
| 3 | Time the message from the consequence, not the change | Delta `[REPORTED]` | **Adopt** | This is Interaction 4.2's operating rule, already proven at scale |
| 4 | A no-outcome message is still a message | Delta `[REPORTED]` | **Adopt** | Direct precedent for Principle 2's hardest case |
| 5 | Push rather than poll; retry on delivery failure | DHL `[REPORTED]` | **Adopt** | Matches Interaction 4.1's stated failure posture exactly |
| 6 | Display revised value against the original | DHL `[REPORTED]` | **Adopt** | A change is legible only when what it replaced is visible |
| 7 | Identifier-keyed read access, reduced scope without an account | Maersk `[OBSERVED]`/`[REPORTED]` | **Adopt, with a Gate 5 decision** | Credential number as read key is plausible; the reduced-scope split needs deciding, not inheriting |
| 8 | Support and help organised by lifecycle stage | Maersk `[OBSERVED]` | **Adopt** | Accreditation has the same staged shape |
| 9 | Explicit state qualifiers inline on scheduled items | Real Madrid `[OBSERVED]` | **Adopt** | A vocabulary for partial knowledge, placed next to the item — directly reusable for stale and provisional states |
| 10 | Timezone stamped on every time | Man City `[OBSERVED]` | **Adopt** | Optional for one city; mandatory across three host countries |
| 11 | Validity window and identity-change invalidation as first-class fields | ETA-IL `[REPORTED]` | **Adopt** | Approval is "approved until", not "approved" — Gate 6 entity model |
| 12 | State the authorization-versus-entry boundary in plain language at the point of guidance | ETA-IL `[REPORTED]` | **Adopt as a phrasing precedent only** | Informs Gate 2 ontology; does not resolve the two-token unknown and must not be presented as resolving it |
| 13 | User-tunable notification preference centre | DHL `[REPORTED]` | **Avoid** | Contradicts ID-13 — urgency derives from change type, not recipient configuration |
| 14 | Discharging suppressed cases to a human intermediary | Delta `[REPORTED]` | **Avoid** | Amina has no agent; Principle 3 exists because that intermediary is absent |
| 15 | Preference-driven delivery rescheduling and redirection | DHL `[REPORTED]` | **Avoid** | Implies the recipient can negotiate the change; the concept explicitly cannot arbitrate a reallocation (`05_CONCEPT.md` §5) |
| 16 | Commercial and membership destinations in primary navigation | Real Madrid `[OBSERVED]` | **Not applicable** | Broadcast-surface concern; no commercial layer in this product |
| 17 | Advertising interleaved into content streams | Man City `[OBSERVED]` | **Not applicable** | No advertising in this product |
| 18 | Multi-squad interleaved fixture streams | Man City `[OBSERVED]` | **Not applicable** | This app's fixtures matter only where an entitlement depends on them |
| 19 | Light-only, identity-driven theming | Real Madrid `[OBSERVED]` | **Not applicable** | Dark mode is first-class here by decision; no football precedent available to borrow |

---

## 4. What this gate could not establish

- No authenticated status view was seen in any comparator. The screen a person reads when their state has changed remains uninspected across all three pattern-class properties.
- No notification was received from any system. Every claim about notification content, tone or layout is from published documentation, never from a delivered message.
- Delta's thresholds are transferable as *discipline*, not as *values*. Nothing here establishes what the equivalent materiality bar is for an entitlement change, and Gate 4 must set it as an `[ASSUMPTION]`.
- Whether any comparator distinguishes a stale cached state from a current one — the question Interaction 4.3 turns on — was not answerable from public surfaces.

---

✅ GATE 1 COMPLETE — `01_BENCHMARKING.md`
