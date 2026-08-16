# 10 — Auth and Onboarding

**Status:** proposed, for gate approval. Second file of Run 4D, the design addendum dossier.
**Authority:** `P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md` §4 (login, onboarding, the public landing
view; the two-record decision), §2 (R1, R4 — the reversals this file grounds).
**Relationship to the frozen gate files.** This file does not edit `05_SCREENS.md`. It supersedes
§6.1's "gates nothing" boundary, §6.3's exclusion list, and §6.5's closing "everything is reachable
without signing in" statement — each named below at the point it is superseded. The formal
reversal record is `12_DECISION-REVERSALS.md` (R1, R4); this file specifies the replacement design.
**Depends on:** `09_DESIGN-ADDENDUM.md` — the black-anchored dark palette (§4) and the four-row
nav list (§5.2) are the visual target every screen below is specified against.
**Verified against:** live clone, HEAD `147bc4a` (v10 merged, 16 Aug 2026) — `Pages/SignIn.razor`,
`Services/MockAccessDataProvider.cs`, `Models/Track.cs`, `Models/Change.cs`,
`Models/Accreditation.cs`, `Layout/NavMenu.razor`.

---

## 1. The boundary, restated before anything is reversed

**This remains a simulation, and the reversals below do not change that.** There is no credential
store, no password hashing, no server, no session token, no authorization. `05_SCREENS.md` §6.1's
core sentence — *"a login form that implies an account system it does not have is a lie by
interface"* — is **not** reversed and governs every screen in this file. The patch states the same
constraint in its own words: the gate must never imply real security, since none exists.

What changes is *what the simulation simulates*. v9 built a form that validated input and then said
sign-in was not implemented. That was exactly to spec, and direct use confirmed it reads as broken:
arriving at a personal access record with no who-are-you moment breaks the mental model. So the
form becomes a working simulated session — openly labelled as simulated, with fake published
credentials, in-memory only. The honesty moves from *"this does nothing"* to *"this does something,
and here is precisely how little that something is."*

**The load-bearing distinction, stated once and applied throughout:** a *simulated session* is a
demonstration of a UI state machine. It is not a security boundary, it protects nothing, and any
person with developer tools can reach any screen without it. Every screen below says so on screen,
not only here.

---

## 2. R1 — Sign In becomes a real (simulated) session

### 2.1 What `05_SCREENS.md` §6.3 excluded, and what is now adopted

| §6.3 excluded | Now | Why the exclusion no longer holds |
|---|---|---|
| **Published demo credentials on screen** | **Adopted** | §6.3's reason was *"this app has no credential store at all — there is nothing to publish."* That premise is what changed: §3 below establishes a `DemoAccountStore` with two records. There is now something to publish, and publishing it is what keeps the screen honest — the ShopEase reference's own pattern |
| **`AuthenticationStateProvider` / `AuthorizeView` / signed-in state** | **Adopted** | §6.3 excluded these as out of scope for 4B, on the ground that adopting them *"would make the interface's claim true in form while remaining false in substance."* With a demo store behind them, the claim is now true in form **and** in substance at the level the app actually operates: a simulated session genuinely exists and genuinely changes what renders |
| **Sign-out** | **Adopted** | §6.3: *"no session to end."* There is now a session to end — and per §3.3 it is the mechanism that makes the two-record demonstration usable |
| **Redirect-on-success to a protected page** | **Adopted, narrowly** | §6.3: *"nothing is protected."* R4 changes that: the personal record is now gated (§5). Redirect goes to the record and nowhere else |
| **The injection-pattern blocklist** | **Still not adopted** | Unchanged and not reopened. The reference's blocklist rejects apostrophes and the substring `" or "`, which turns away *O'Neill*, *D'Angelo*, *Ba'ath*. The permissive allow-list v9 shipped (`^[\p{L}\p{N} .\-'_@]+$`) is correct and stays exactly as it is |

### 2.2 What v9 already built that survives untouched

`Pages/SignIn.razor` is not rewritten. These carry forward verbatim, and 4E should treat them as
existing assets rather than re-derive them: the `EditForm`/`EditContext`/`DataAnnotationsValidator`
structure; `autocomplete="username"` and `autocomplete="current-password"`; `type="password"`; the
`IdentifierInputMode` switch to `email` once the value contains `@`; `ValidationMessage` with
`aria-live="polite"` per field and `role="alert"` on the form-level error; the `PermissiveIdentifier`
allow-list and its reasoning comment; the single generic failure message; and the rule that no
credential is logged, echoed, stored, or placed in a query string.

**One rule from §6.4 needs restating because a working store makes it newly testable:** the password
is compared byte-exact — never trimmed, never rewritten, never pattern-checked. Allow-list rewriting
is right for an identifier and wrong for a password. With a real comparison now happening, this
stops being a hypothetical and becomes something `4E`'s tests should assert.

### 2.3 What changes on the screen

**The notice, rewritten, and still first on the page before any interaction.** v9's notice says
nothing is sent, stored, or checked, and that the app is fully reachable without signing in. Both
halves are now false. The replacement states, in this order: that the sign-in is simulated and holds
no real account system; that the credentials below are fake, published, and work; that the session
lives in memory and does not survive a page refresh; and that it is a demonstration of what the app
shows a signed-in holder, not a security boundary.

**Demo credentials, published on screen, adjacent to the form** — the ShopEase copy pattern adapted
to this domain. Both records from §3 are listed with their identifier and password in plain text,
each labelled with whose record it opens and, in one short line, what makes that record different to
look at. That last part is the point: a person who signs in as Amina and then as Tomás should be
told, before they do it, that the difference is the thing worth seeing.

**Submission outcome replaces the inert "not implemented" result.** On a match: the session is
established and the person is redirected to their record (§5.2). On no match: the existing single
generic failure message, unchanged — one message for both wrong identifier and wrong password. With
published credentials this is arguably ceremony, and it is kept anyway, because the pattern is
correct and a demo that models the wrong pattern teaches the wrong pattern.

**State matrix**, replacing §6.5's:

| State | What shows |
|---|---|
| **Empty** | Blank form, simulation notice and published credentials both visible before any interaction |
| **Populated** | Entered values; password masked |
| **Validating / invalid** | Per-field `ValidationMessage`, `aria-live="polite"` — unchanged from v9 |
| **Submitting** | Fields and button disabled; label states the attempt is in progress. **New**, and it must be genuinely observable — see §7 |
| **Submitted — match** | No result panel; the person is on their record. The session banner (§4.2) is the confirmation |
| **Submitted — no match** | Form-level generic failure, `role="alert"`, input preserved |
| **Stale** | N/A — a form does not go stale |

---

## 3. The two demo records

### 3.1 Why two, and why these two

Tomás L. has constrained every design decision in this project — he is `05_ARTIFACTS.md` §1.2, he
was seated at the ideation workshop as W5 the dissenter, and his objection is why urgency is derived
rather than configured. He has never been visible in the product. A second seeded record makes the
third design principle demonstrable instead of documented: the same foreseeable change interrupts
Amina and stays silent for Tomás, and you can watch it happen by signing out and back in.

**This needs no new logic.** `Track.NotificationCeiling` already derives `ImmediateOnly` from
`HasNamedContact: true` and `ImmediateAndForeseeable` from `false`, and `Change.DeriveUrgency`
already takes the track's ceiling as the last word. The mechanism is built, tested as of v10, and
currently demonstrable only in the abstract because one record exists. `[VERIFIED]` against
`Models/Track.cs` and `Models/Change.cs` at HEAD `147bc4a`.

### 3.2 The records

**Record 1 — Amina.** The existing seeded record, unchanged in every field:
`MP-2026-04817` · Amina Bello · The National Daily · `MemberAssociationQuota`,
`HasNamedContact: false` → `ImmediateAndForeseeable` · Approved · valid until 19 Jul 2026 · zones
Media tribune, Mixed zone, Press conference room.

**Naming discrepancy, flagged not resolved.** `05_ARTIFACTS.md` calls the persona **Amina R.**; the
seeded record calls her **Amina Bello**. Both predate this file, neither is wrong, and the surname
initial in the research artifact is a persona convention rather than a contradiction. Left as-is —
renaming a seeded record to match a persona document would edit shipped data to satisfy a cosmetic
consistency nobody has asked for. Noted so a future reader does not treat it as a defect.

**Record 2 — Tomás.** New, and every field is derived from `05_ARTIFACTS.md` §1.2 rather than
invented:

| Field | Value | Derived from |
|---|---|---|
| `CredentialId` | `RH-2026-00219` — the `RH` prefix distinguishes the rights-holder track from Amina's `MP` at a glance | `[ASSUMPTION]` — format follows the existing `MP-2026-#####` shape |
| `HolderName` | Tomás L. | `05_ARTIFACTS.md` §1.2 verbatim |
| `Outlet` | A rights-holding broadcaster — named generically, not invented as a real broadcaster | §1.2 "rights-holding broadcaster"; naming a real one would be a trademark problem this project has no reason to take on |
| `Track` | `RightsHolder`, `HasNamedContact: true` → `ImmediateOnly` | §1.2: *"Has a named contact and uses them"* — `[SOURCED]` to the persona |
| `Status` | Approved | Same as Amina; the demonstration is about notification ceiling, not status |
| `ValidUntil` | Same tournament window as Amina | Keeps the single variable isolated |
| `ZoneAccess` | Broader than Amina's, including a broadcast-position zone | §1.2's crew-coordinator role; `[ASSUMPTION]` for the exact zone labels |

**The one variable that matters is `HasNamedContact`.** Everything else stays as close to Amina's
record as the persona permits, precisely so the difference a person observes when switching accounts
is attributable to the ceiling and not to six other differences at once.

### 3.3 What the demonstration must actually show

Tomás's seeded change list must include **at least one foreseeable change that is classified Silent
for him and would be interrupting for Amina** — otherwise two accounts exist and demonstrate
nothing. The change content itself is `4E`'s to seed against `06_DATA-MODEL.md`'s existing
`ChangeKind` vocabulary; this file specifies only that the contrast must be present and visible
without the person having to reason about it.

**What must not happen:** Tomás's record must not become a bulk or roster surface. `05_CONCEPT.md`'s
scope boundary and W5's own navigation objection both stand — he coordinates 40–120 credentials in
reality, and this app shows **his own** credential only. A second record is a second individual
holder, not the beginning of a crew-management view. This is the most likely place for scope to
leak, and it is prohibited here explicitly.

### 3.4 Where the store lives

A `DemoAccountStore`-equivalent holding both identifier/password pairs and mapping each to a
`CredentialId`, following the ShopEase reference's structure. `MockAccessDataProvider`'s
`DemoCredentialId` constant — currently a single `public const string` documented as *"there is one
holder, because this is a single-record surface"* — is superseded: the provider seeds two records
and resolves reads by the credential the session holds. That comment becomes wrong the moment this
lands and must be updated with the code, not left to contradict it.

`AuthenticationStateProvider` is in-memory and documented in its own remarks as **not** ASP.NET
Identity, per the reference. Session does not survive a refresh, and the notice says so.

---

## 4. Sign-out, and the signed-in indicator

### 4.1 Sign-out

Visible whenever a session exists, per the reference. It ends the session, returns to the public
landing view (§6), and states plainly that it did so. No confirmation dialog — there is nothing to
lose, and a modal guarding a fake session would be theatre.

**Placement:** in the nav list, which `09_DESIGN-ADDENDUM.md` §5.2 established now holds four rows
(My Access, Matches, Help, theme). Sign-out becomes a fifth row, positioned **below** the theme row
at the list's end, so the destination rows stay contiguous at the top and the two
"state-not-destination" controls group together at the bottom. §9 of `09` asked this file to resolve
exactly that placement; this is the resolution.

### 4.2 Knowing who you are signed in as

A persistent, low-weight indicator naming the current holder — because with two demo records whose
whole purpose is comparison, *"which one am I looking at"* is a question the interface must answer
without being asked. Placed in the sidebar above the nav rows, using the existing sidebar text
tokens (`--color-sidebar-text`, `--color-nav-item`), stating the holder's name and credential
number. It is not a menu and has no affordance beyond being read.

**On the record screen itself**, the existing accreditation header already names the holder — no
duplication is being introduced there. The sidebar indicator exists for every *other* screen, where
nothing currently says whose session is active.

---

## 5. R4 — what is gated and what stays public

### 5.1 The reversal

`05_SCREENS.md` §6.2 and §6.5 require every part of the app to be reachable without signing in, and
v9 verified this (`PASS — no route is guarded`). Reversed: **the personal access record is gated;
everything else stays public.**

**Reason:** an access record is personal by definition. Showing one to an unauthenticated visitor
contradicts the concept more than gating it does — the app's entire premise is that this is *your*
state, and a state anybody can read without identifying themselves is not that.

### 5.2 The map

| Surface | Route | Signed out | Signed in |
|---|---|---|---|
| Public landing | `/` | **The landing view (§6)** | Redirects to the record |
| Access record | `/` (or `/record`, 4E's call — see §8) | Not reachable; sign-in offered | **The record** |
| Matches | `/matches` | **Public** | Public |
| Match detail | `/events/{id}` | **Public** | Public |
| Request access | `/request/{id}` | Sign-in required — it writes to a personal record | Available |
| Help | `/help` | **Public** | Public |
| Sign in | `/signin` | The form | Redirects to the record |

**Matches and Help stay public — non-negotiable, per the patch.** Help in particular: it is the
terminal route for Task 3's no-cache path and must be readable offline by someone who has hit a
barrier. Gating the page that explains the boundary would be the single worst gating decision
available.

**Request access is gated**, and this is the one row not stated verbatim in the patch, so the
reasoning is given: it is a write to a personal record. A request submitted with no holder attached
either goes nowhere or goes to an arbitrary record, and both are worse than asking the person to
identify themselves first. A signed-out visitor on a match detail page sees the request affordance
with a sign-in prompt rather than a hidden or dead control — the boundary is stated, not concealed.

### 5.3 What the gate must never do

It must never imply real security. A gated route reached while signed out shows the sign-in screen
with its full simulation notice — never a "403", never "access denied", never anything borrowing the
vocabulary of a real authorization system. The distinction is stated on screen: *this is
demonstrating what a holder sees, not protecting anything.*

---

## 6. The public landing view

### 6.1 What it is for

`/` signed out is the first thing a reviewer, recruiter, or inspector sees. It has two audiences at
once — someone evaluating the work, and the notional journalist the app is built for — and it should
serve the first without pretending not to be a demonstration.

### 6.2 What it states

1. **What this is**, in two or three sentences: a media-accreditation companion for journalists
   covering the 2026 World Cup, showing what has changed about their access and why, before they
   discover it by being refused. The concept's own promise, in the concept's own language.
2. **That it is a demonstration** — a portfolio project with simulated data, not a FIFA product and
   not affiliated with FIFA. This carries `09_DESIGN-ADDENDUM.md` §1's inspired-by disclosure onto
   the app's own front door, where it does the most work.
3. **Two entry points, equally weighted**: sign in with a demo account, and browse the public
   surfaces (Matches, Help) without one.
4. **A short statement of what a signed-in record shows** — the change log, the reason attached to
   each change, the staleness indicator — so the value is legible before anyone signs in.

### 6.3 Where the demo credentials live — resolved

The patch leaves this open: does the landing carry the demo-credential disclosure, or defer it to
sign-in? **Resolved: defer to the sign-in screen**, with the landing saying only that demo accounts
exist and are published on the sign-in page.

**Reason:** credentials on the landing page are noise for the visitor who wants to know what the
project *is*, and the sign-in screen is where they are needed, one click later and directly beside
the form they are typed into. Splitting them across both surfaces would also create two places to
keep in sync. The landing's job is orientation; the sign-in screen's job is access.

**Rejected alternative:** publishing both accounts prominently on the landing to reduce friction for
a reviewer in a hurry. Rejected because it inverts the page's priority — a reviewer who lands on a
credential dump learns what to type before learning what they are looking at.

### 6.4 What it is not

Not a marketing page. No feature grid, no testimonials, no invented statistics, no logos. It states
what the app does, that it is simulated, and how to proceed. `09_DESIGN-ADDENDUM.md` §3's deference
principle applies with unusual force here: this is the one screen with no content of its own to
defer to, which makes it the easiest one to over-design.

---

## 7. Loading and submitting states — a note carried from v10

The sign-in submission has the same defect shape v10 fixed on the request path: an in-memory account
lookup returns instantly, so a `Submitting` state specified in §2.3 would never render. `4E` should
apply the same solution v10 applied — a named, commented simulated-latency constant on the demo
authentication path only, in the same voice as `MockAccessDataProvider.SimulatedWriteLatency`, with
its own regression test asserting the returned task is not already complete.

**Reads still gain no latency.** The rule v10 established holds: the record paints on first render
with no spinner in front of it. Signing in is a write-shaped action; reading a record is not.

---

## 8. Open items this file deliberately does not resolve

- **The record's route when signed in.** Whether the record stays at `/` (with `/` branching on
  session state) or moves to a named route with `/` reserved for the landing. Both work; the choice
  interacts with the SPA fallback and `NavLink`'s `Match="NavLinkMatch.All"` behaviour on the nav's
  first row, which is an implementation concern `4E` is better positioned to judge against the
  running router than this dossier is against source inspection.
- **Whether onboarding exists beyond the landing and sign-in.** No first-run tour, no coach marks,
  nothing proposed here. If real use of the corrected build shows people arriving at a record
  without understanding what a "change" is, that is `04-evaluation`'s finding to make and a later
  run's to answer — not something to build pre-emptively.
- **Session persistence across refresh.** Deliberately *not* persisted, per the reference and per
  §2.3's notice copy. Named here so a future run does not "fix" it as an oversight: a session that
  survives a refresh would need storage, and storage would make the simulation look more like an
  account system than it is.

---

## 9. Carried into `11_I18N.md` and `12_DECISION-REVERSALS.md`

- **`11_I18N.md`** inherits three new copy surfaces, all of which are among the most prose-heavy in
  the app and therefore the most likely to break layout in ES/PT: the sign-in simulation notice
  (§2.3), the published-credentials block with its per-account descriptions (§2.3), and the public
  landing view's four content items (§6.2). The nav list now carries five rows including two
  translated non-destination labels (theme, sign-out), against the `3rem` row height
  `09_DESIGN-ADDENDUM.md` §8 already flagged.
- **`12_DECISION-REVERSALS.md`** takes R1 from §2.1's table — four exclusions adopted, one held —
  and R4 from §5.1, each citing `05_SCREENS.md` §6.1, §6.2, §6.3 and §6.5 as the sections
  superseded, and each recording that §6.1's lie-by-interface principle survives the reversal
  unchanged.
