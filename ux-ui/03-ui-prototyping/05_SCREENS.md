# 05 — SCREEN SPECIFICATIONS

**Repo path:** `ux-ui/03-ui-prototyping/05_SCREENS.md`
**Inputs:** `04_TASKS-AND-SCENARIOS.md` §6 screen list; `03_UI-DECISIONS.md` §6 components; `02_INFORMATION-ARCHITECTURE.md` CH-1…CH-10.
**Login reference read directly:** `jdsaire/frontend_c6_ecommerce` at `main` — `src/ShopEase/Pages/Login.razor`, `Services/InputValidationService.cs`, `Services/SafeTextAttribute.cs`. Retrieved as a repo archive; cited in §6.
**This gate specifies the interface in words. No code.**

---

## 1. Screen register

| Screen | New / Modified / Retired | Existing file | Tasks served |
|---|---|---|---|
| **My Access** | New | — | 1, 2, 3 |
| **Match detail** | Modified | `Pages/EventDetails.razor` | 2, 3 |
| **Help** | New | — | 1, 3 |
| **Matches** | Modified | `Pages/EventList.razor` | Supporting |
| **Request access** | Modified | `Pages/Registration.razor` | Supporting |
| **Sign in** | New | — | Supporting |
| **Not found** | Unchanged | `Pages/NotFound.razor` | — |

**Nothing is retired.** The per-match request count (C05) recommended for retirement at Gate 2 §4.5 is removed as *content* from Match detail, but no page is deleted. `GateCheckResult` from Gate 4 is specified as a **state of Match detail**, not a screen of its own — it has no route, no entry point a person navigates to, and exists only as an outcome displayed in place.

---

## 2. My Access — the primary surface

**Purpose.** Answer "what do I hold, what changed, and what can I do" without a tap and without a network.
**Route.** `/` — the default landing, displacing the match list (Gate 2 §6).
**Entry points.** App launch; notification tap; nav item; post-submission return from Request access.
**Interactions.** 4.1 primary, 4.2 and 4.3 co-resident.

### 2.1 Content inventory

| Order | Element | Component | Rule |
|---|---|---|---|
| 1 | Headline — what I hold now | AccessCard | Resolves from cache before any network call (CH-10) |
| 2 | Staleness | StaleIndicator | Always present, not only when old (CH-8) |
| 3 | Foreseeable changes | ForeseeableBadge + ChangeRow | Above landed changes only when effective-date ordering puts them there (CH-2) |
| 4 | Change list, newest-effective-first | ChangeRow ×n | Each carries what changed, why, next step, timestamp — all four or it is malformed (CH-9) |
| 5 | Identity and track | Plain text, secondary | Stable data; never competes with the headline |
| 6 | Link to Help | Text link | Always reachable, per Gate 2 §4.4 |

**Accreditation vs match access are visually distinct blocks** within the headline, per the two-token naming decision (Gate 2 §4.1). Standing accreditation shows "valid until"; match access is listed per fixture.

### 2.2 State matrix

| State | What shows | What must never happen |
|---|---|---|
| **Loading** | Cached state renders immediately with its existing StaleIndicator; a subtle refresh affordance indicates a fetch in flight | A spinner blocking the headline. A person at a barrier must never wait on a network to see what they already had |
| **Empty — no record** | Plain statement that no accreditation record exists yet, with the application stage named and a Help link | An empty change list styled to look like "nothing has changed." Absence of a record and absence of changes are different facts |
| **Empty — record, no changes** | Headline state plus explicit "no changes since [timestamp]" | Blank space below the headline. Silence must be stated, per CH-6 |
| **Populated** | Headline, staleness, changes newest-effective-first | A change without all four fields rendering |
| **Stale** | Identical layout; StaleIndicator carries the age and uses `--color-stale-text` | A stale headline rendered identically to a fresh one. **This is the failure the whole concept exists to prevent** |
| **Error — fetch failed, cache present** | Cached state, staleness, and a plain statement that the last refresh failed and when it was attempted | Silently showing cached data as if the refresh succeeded |
| **Error — fetch failed, no cache** | Statement that no state is available offline, routed to Help escalation | An indefinite spinner. A dead end with no route out |
| **Contradictory — cached state disagrees with a just-received change** | The newer change wins the headline; the superseded value stays visible per CH-3 | Both displayed as equally current with no ordering |

**Why the stale row is the deliverable.** Every other row degrades gracefully. This one, done wrong, actively misleads: a person reads a confident headline, walks to a barrier, and is refused — which is precisely the discovery-by-failure the mandate exists to remove, now with the app's endorsement attached.

---

## 3. Match detail — modified

**Purpose.** Show one fixture, the access the holder has for it, and — when relevant — that a pending entitlement depends on this match's outcome.
**Route.** `/events/{id}`, unchanged.
**Existing file.** `Pages/EventDetails.razor`.
**Tasks.** 2 (the fixture a foreseeable change depends on), 3 (per-match cached status at the barrier).

### 3.1 Changes from current behaviour

| Change | Reason |
|---|---|
| Per-match request count **removed** | Gate 2 §4.5 — tells the holder nothing about her own entitlement and implies a competition she cannot act on |
| "Registered" badge **renamed** to access status | Gate 2 §4.1 rename; "registered" describes an account, not an entitlement |
| Kickoff time gains an explicit timezone label | Gate 1 adopt #10 — mandatory across three host countries |
| New: dependency statement when this fixture gates a pending change | Interaction 4.2 |
| New: GateCheckResult state | Interaction 4.3 |

### 3.2 State matrix

| State | What shows |
|---|---|
| **Loading** | Fixture data from cache if present; access status may resolve after |
| **Empty** | Fixture exists, no access requested — request affordance shown |
| **Populated** | Fixture, access status, timezone-stamped kickoff |
| **Stale** | StaleIndicator on the *access status specifically*, not the fixture — kickoff times do not go stale the way entitlements do |
| **Error** | Fixture shown, access status unavailable, stated plainly rather than defaulted to "no access" |
| **Dependency pending** | Statement that an entitlement depends on this result, worded as a condition, both outcomes named (CH-7) |
| **GateCheckResult — agreement** | Access status confirmed as presented |
| **GateCheckResult — disagreement** | Both the record's state and the barrier's outcome displayed side by side, with escalation routing. **The screen does not adjudicate** |

**The disagreement state carries a hard constraint.** Venue access list ownership is unresolved (`06_HANDOFF.md` rec 4). This screen displays a disagreement and routes it to a human; it never resolves one, and no copy on it may imply the app knows which side is correct.

---

## 4. Help — new

**Purpose.** State what the service does not do, publish what will not interrupt, and route escalation.
**Route.** `/help`.
**Entry points.** Nav item; **direct link from any refusal, failed gate check, or no-cache error state** — per Gate 2 §2.3, the person who needs the boundary stated is the person who just hit it.
**Tasks.** 1 and 3, as the terminal route.

### 4.1 Content inventory

| Section | Content | Basis |
|---|---|---|
| Staged guidance | Organised by lifecycle stage — before applying, applied, decided, in tournament, matchday | Gate 1 adopt #8 |
| What this service does not do | Visas, vetting outcomes, quota decisions, guaranteed access, appeals | `05_CONCEPT.md` §5 (C20) |
| What will not notify you | The Silent class, in plain language | CH-6; Gate 4 §2.3 |
| Escalation route | Who to contact and for what | C21 |

**The escalation section must not resemble an appeal channel.** A screen that looks like one is a boundary violation regardless of label (`05_CONCEPT.md` §5). It routes to an existing human relationship; it does not accept a submission.

### 4.2 State matrix

Static content: **Populated** only. No loading, empty, stale, or error state — this content ships with the app and must be readable offline, since Task 3's no-cache path terminates here.

---

## 5. Supporting surfaces — justified, per Gate 4 §6

### 5.1 Matches — modified

**Why it exists with no task attached.** It is the only route to Match detail, which two tasks require. A fixture list is navigation infrastructure, not a destination the concept is about.
**Route.** `/matches` — demoted from `/` (Gate 2 §6).
**Existing file.** `Pages/EventList.razor`.
**Changes.** Route change; MatchCard reuses `EventCard`'s read-only presentation unchanged (Gate 3 §6); may use Bootstrap grid at ≥641px since fixtures have no inherent reading order.
**States.** Loading (cached list if present) · Empty (no fixtures — a data-load failure, stated as such) · Populated · Stale (list-level indicator) · Error (statement plus retry, never a blank list).

### 5.2 Request access — modified

**Why it exists with no task attached.** It is the app's existing write path and its only write surface. Gate 4 §6 flags plainly that no task tests it, and that this dossier will therefore not have predicted a break in it.
**Route.** `/request/{id}` — renamed from `/register/{id}` (Gate 2 §4.1).
**Existing file.** `Pages/Registration.razor`.
**Changes.** Route and all copy renamed from "Register" to "Request access"; reuses `EventCard`'s validated-input pattern — per-field inline errors with `aria-describedby`, instance-unique IDs (Gate 3 §6).
**Cancel path — unresolved, and named here.** Gate 3 §7 flagged the heuristic 3 gap; Gate 4 §7 established that withdrawal is a *data* question, since CH-1 permits no second write path — a withdrawal is either a change written to the record or it does not exist. **Gate 6 decides this.** This spec therefore describes no cancel affordance, and that absence is a deferred decision rather than a design position.
**States.** Empty (blank form) · Populated (in progress) · Submitting · Success (returns to My Access, where the resulting change appears — no separate confirmation screen, per CH-1) · Error (submission failed, input preserved, plainly stated) · Stale (N/A — a form does not go stale).

### 5.3 Sign in — new

Specified in full in §6.

---

## 6. Sign in — form specification

### 6.1 The boundary, stated first

**This is a form, not authentication.** There is no credential store, no session security, no authorization, and nothing behind it. Run 4C is where an endpoint could exist. A login form that implies an account system it does not have is a lie by interface, and the screen must therefore say what it is *on the screen itself*, not only in a repo file.

### 6.2 What is adopted from `jdsaire/frontend_c6_ecommerce`

Read directly at `src/ShopEase/Pages/Login.razor` and its supporting services.

| Adopted | Detail |
|---|---|
| **On-screen honesty about simulation** | The reference states in its own visible intro copy that the login is simulated and not a real account system, and that the session is held in memory and will not survive a refresh. This is the single most important thing to carry — it solves the lie-by-interface problem in copy rather than in documentation |
| **`autocomplete` attributes** | `username` on the identifier field, `current-password` on the password field |
| **`type="password"`** | On the password field |
| **Idiomatic Blazor form validation** | `EditForm` with `EditContext`, `DataAnnotationsValidator`, per-field `ValidationMessage` |
| **`aria-live="polite"` on field errors, `role="alert"` on form-level error** | Accessible error announcement, matching the reference's treatment |
| **Single generic failure message** | The reference returns one message for both wrong identifier and wrong password, rather than revealing which was wrong |
| **Allow-list validation on the identifier, never on the password** | The reference's code comment is explicit: allow-list rewriting is appropriate for a username but never for a password, which must compare byte-exact and must never be silently rewritten |
| **Client-side honesty as a documented stance** | The reference's validation service states in its own remarks that it runs entirely in the browser, is not a security boundary, and can be bypassed with developer tools |

### 6.3 What is NOT adopted

| Not adopted | Why |
|---|---|
| **Published demo credentials on screen** | The reference lists working demo accounts openly, correct for a teaching shop. This app has no credential store at all — there is nothing to publish, and publishing placeholders would imply a store exists |
| **`AuthenticationStateProvider` / `AuthorizeView` / signed-in state** | These constitute a session system. Out of scope for 4B entirely; adopting them would make the interface's claim true in form while remaining false in substance |
| **Sign-out** | No session to end |
| **Redirect-on-success to a protected page** | Nothing is protected. There is no authorization in this app |
| **The injection-pattern blocklist** | The reference's list rejects apostrophes and the substring " or ", which would reject legitimate names such as *O'Neill*. An allow-list that rejects real input is a bug, not stricter defence — the reference's own service says exactly this in its remarks. This app's identifier field must accept real journalist names |

### 6.4 Fields and validation

| Field | Type | `autocomplete` | `inputmode` | Client validation |
|---|---|---|---|---|
| Identifier (email or credential number) | `text` | `username` | `email` when the value contains `@`, otherwise default | Required; length bounds; permissive allow-list covering letters, digits, spaces, hyphen, apostrophe, period, underscore, `@` |
| Password | `password` | `current-password` | — | Required; length bound only. **No pattern rewriting, no sanitisation, no trimming** |

**Security attributes.** No credential is logged, echoed to console, written to storage, or placed in a query string. No secret exists in client state because no secret exists at all. Submission is inert: it validates, and on success states plainly that authentication is not implemented in this version.

### 6.5 State matrix

| State | What shows |
|---|---|
| **Empty** | Blank form with the simulation notice visible before any interaction |
| **Populated** | Entered values; password masked |
| **Validating / invalid** | Per-field messages via `ValidationMessage`, `aria-live="polite"` |
| **Submitted — valid input** | Plain statement that sign-in is not implemented in this version and what the person can still do without it |
| **Error** | Form-level message with `role="alert"` |
| **Stale** | N/A |

**Everything in this app is reachable without signing in.** Since there is no authorization, the form gates nothing — and the screen must not imply otherwise by, for example, presenting itself as a barrier to My Access.

---

## 7. Consistency check against Gate 4

| Gate 4 required screen | Specified | Where |
|---|---|---|
| My Access | Yes | §2 |
| Match detail | Yes | §3 |
| Help — boundary and escalation | Yes | §4 |
| GateCheckResult | Yes — as a **state of Match detail**, not a screen | §3.2 |

| Screen here with no Gate 4 task | Justified as supporting |
|---|---|
| Matches | §5.1 — sole route to Match detail |
| Request access | §5.2 — the app's only write path; untested by design, flagged |
| Sign in | §5.3, §6 — the door; gates nothing |

No screen appears in this gate without either a task or a stated supporting justification.

---

## 8. Carried into Gate 6

| # | Item | Owner |
|---|---|---|
| 1 | Withdrawal-as-a-change — decides whether Request access needs a cancel path | Gate 6 entity model |
| 2 | GateCheckResult has no data source; the disagreement state must not assume one | Gate 6 |
| 3 | Cache-versus-fetch precedence in My Access loading is a data-layer behaviour, specified here only as a display rule | Gate 6 service abstraction |
| 4 | W5's navigation objection remains unresolved — My Access as the front door is specified here for Amina, and no bulk surface exists | Deferred to v2, per Gate 4 §7 |

---

✅ GATE 5 COMPLETE — `05_SCREENS.md`
