# 07 — BUILD BRIEF

**Repo path:** `ux-ui/03-ui-prototyping/07_BUILD-BRIEF.md`
**Audience:** whoever drafts the Run 4B deployment prompt. Written so that prompt can be authored from this file without re-deriving anything.
**Repo state assumed:** `main` at HEAD `03663db`, post-rename. Every path below verified against the repo tree, not inferred.

---

## 1. File-level scope

### 1.1 Created

| Path | Purpose | Spec |
|---|---|---|
| `src/FifaPressApp/Models/Fixture.cs` | Fixture entity | `06_DATA-MODEL.md` §2.1 |
| `src/FifaPressApp/Models/Accreditation.cs` | Accreditation entity | §2.2 |
| `src/FifaPressApp/Models/Change.cs` | Log record; construction-time validation of the three required fields | §2.3 |
| `src/FifaPressApp/Models/Track.cs` | Track enum + notification ceiling | §2.4 |
| `src/FifaPressApp/Services/IAccessDataProvider.cs` | **The interface.** Written first | §5.1 |
| `src/FifaPressApp/Services/MockAccessDataProvider.cs` | 4B's in-memory implementation, incl. `AsOfUtc` withholding | §5.2, §1.1 |
| `src/FifaPressApp/Services/FixtureImporter.cs` | CSV parse: `d-MMM-yy`, ` v ` split, city→zone lookup | §1, §6 |
| `src/FifaPressApp/Components/AccessCard.razor` | Headline state | `05_SCREENS.md` §2.1 |
| `src/FifaPressApp/Components/ChangeRow.razor` | One change, four required fields, CH-3 supersede display | §2.1 |
| `src/FifaPressApp/Components/ForeseeableBadge.razor` | Conditional marker | §2.1 |
| `src/FifaPressApp/Components/StaleIndicator.razor` | Age of state; used on every surface reading cache | CH-8 |
| `src/FifaPressApp/Components/ThemeTrigger.razor` | Theme toggle, the new persistent strip | `03_UI-DECISIONS.md` §3.2 |
| `src/FifaPressApp/Pages/MyAccess.razor` | Primary surface, route `/` | `05_SCREENS.md` §2 |
| `src/FifaPressApp/Pages/Help.razor` | Route `/help` | §4 |
| `src/FifaPressApp/Pages/SignIn.razor` | Form only, no auth | §6 |
| `src/FifaPressApp/wwwroot/data/2026_World_Cup_Schedule.csv` | The admitted fixture data | `00_SCOPE.md` §6 |

### 1.2 Modified

| Path | Change |
|---|---|
| `src/FifaPressApp/wwwroot/css/app.css` | Colour literals → tokens; both themes; `#blazor-error-ui` `color-scheme: light only` **removed**; corrected success value | `03_UI-DECISIONS.md` §1, §2 |
| `src/FifaPressApp/Layout/MainLayout.razor` | Add the theme-trigger strip above `main` | §3.2 |
| `src/FifaPressApp/Layout/MainLayout.razor.css` | Strip styling; sidebar gradient → themed tokens | §2.1 |
| `src/FifaPressApp/Layout/NavMenu.razor` | Brand string `EventEase` → `FIFA Press App` (line 3); nav items → My Access / Matches / Help | §4; `02_INFORMATION-ARCHITECTURE.md` §6 |
| `src/FifaPressApp/Layout/NavMenu.razor.css` | Nav colours → tokens; scaffold icon classes replaced or removed | §2.1, §0.3 |
| `src/FifaPressApp/wwwroot/index.html` | `<title>` (line 8) and `<meta name="description">` (line 7) — both currently read `EventEase` | §4 |
| `src/FifaPressApp/Pages/EventList.razor` | Route `/` → `/matches`; becomes the supporting Matches surface | `05_SCREENS.md` §5.1 |
| `src/FifaPressApp/Pages/EventDetails.razor` | Remove request count; rename badge; timezone label; dependency statement; GateCheckResult state | §3 |
| `src/FifaPressApp/Pages/Registration.razor` | Route `/register/{id}` → `/request/{id}`; all "Register" copy → "Request access" | §5.2 |
| `src/FifaPressApp/Components/EventCard.razor` | Reused as MatchCard; read-only presentation unchanged | `03_UI-DECISIONS.md` §6 |
| `src/FifaPressApp/Program.cs` | Register `IAccessDataProvider` → `MockAccessDataProvider`, following the existing singleton pattern | §5 |
| `src/FifaPressApp/README.md`, `Pages/README.md`, `Components/README.md`, `Models/README.md`, `Services/README.md` | Update to match what now exists | Repo standard |

### 1.3 Frozen — not touched by 4B

| Path | Reason |
|---|---|
| `.github/workflows/` | Run v7 owns it |
| `src/FifaPressApp/wwwroot/lib/` | Bootstrap retained as-is; no new CSS framework |
| `src/FifaPressApp/FifaPressApp.csproj` | No new dependency |
| `src/FifaPressApp/Properties/launchSettings.json` | No launch-profile change needed |
| `src/FifaPressApp/Pages/NotFound.razor` | Unaffected |
| `ux-ui/00-initial-evaluation/`, `01-design-research/`, `02-ideation/` | Out-of-scope UX folders |
| `handoff/v1`–`v3` | Historical records: annotate, never alter |

### 1.4 Retained pending decision

`src/FifaPressApp/Models/EventModel.cs`, `MockEventData.cs`, `RegistrationModel.cs`, `Services/SessionTracker.cs`, `AttendanceTracker.cs`.

`AttendanceTracker` backs the per-match request count that Gate 2 §4.5 recommended retiring. **4B removes the count from the UI but does not delete the service** — deleting working code that a later version may want is a separate decision from removing a display element, and this dossier only decided the latter. `SessionTracker` continues to back the request flow.

---

## 2. Build order

Non-negotiable, because two steps depend on it:

1. `IAccessDataProvider` **first** — before any component. Writing components first produces components coupled to a concrete provider, which is the exact refactor §5.3 of the data model exists to prevent.
2. Entities, then `FixtureImporter`, then `MockAccessDataProvider`.
3. `Change` validation before any UI renders a change.
4. Tokens in `app.css` before components, so nothing is authored against literals.
5. Components, then pages, then routes.
6. Docs and READMEs last.
7. `learning-mode/` **after everything else lands** — see §4.

---

## 3. Acceptance criteria

A checklist a build agent can verify. Each is pass/fail, not a matter of judgement.

### 3.1 The withholding rule — highest risk

- [ ] `MockAccessDataProvider` exposes `AsOfUtc`.
- [ ] No method returns `HomeLabel` or `AwayLabel` for a fixture whose kickoff is later than `AsOfUtc`.
- [ ] The withholding is enforced **inside the provider**, not by callers.
- [ ] Elimination is derived only from fixtures at or before `AsOfUtc`.
- [ ] Verifiable check: with `AsOfUtc` set between Match 84 and Match 93, no call returns team names for Match 93 or any later match.

### 3.2 The log

- [ ] `Change` cannot be constructed without `WhatChanged`, `Reason`, and `NextStep`.
- [ ] `Urgency` is derived from `Kind` + `EffectiveUtc` + `Track` — never settable as a user preference.
- [ ] No update or delete path exists on a `Change`.
- [ ] Every write operation returns the resulting `Change`, never `bool`.
- [ ] Changes render ordered by `EffectiveUtc` descending.
- [ ] A superseding change displays the value it replaced.

### 3.3 Staleness and offline

- [ ] Every provider response carries `LastSyncedUtc`.
- [ ] `StaleIndicator` renders on My Access **always**, not only when data is old.
- [ ] My Access headline renders from cache with no network call.
- [ ] No spinner blocks the headline.

### 3.4 Theme and accessibility

- [ ] Both themes defined together as tokens; no colour literal remains in `app.css` outside the token block.
- [ ] `color-scheme: light only` no longer present.
- [ ] Success/valid indicator uses the corrected value, not `#26b050`.
- [ ] Theme trigger appears top-right at both breakpoints, above and below 641px.
- [ ] Focus-ring treatment from `00-initial-evaluation/` still present.
- [ ] Skip link still present and functional.

### 3.5 Rebrand

- [ ] No occurrence of `EventEase` in `NavMenu.razor`, `index.html` `<title>`, or `index.html` `<meta name="description">`.
- [ ] Replacement strings match `03_UI-DECISIONS.md` §4 exactly.

### 3.6 Sign in

- [ ] No credential store, no `AuthenticationStateProvider`, no `AuthorizeView`, no session.
- [ ] Simulation notice visible on screen before interaction.
- [ ] `autocomplete="username"` and `autocomplete="current-password"` present.
- [ ] Password field is `type="password"`; never sanitised, trimmed, or pattern-rewritten.
- [ ] No credential logged, stored, or placed in a query string.
- [ ] Every part of the app remains reachable without signing in.

### 3.7 Routes

- [ ] `/` → My Access. `/matches` → Matches. `/events/{id}` → Match detail. `/request/{id}` → Request access. `/help` → Help. `/signin` → Sign in.
- [ ] No route still resolves to `/register/{id}`.

### 3.8 Build

- [ ] `dotnet build` succeeds with no new warnings.
- [ ] App runs and every route loads.

---

## 4. `learning-mode/` restructure — a specification, not content

**Trigger:** code intervention, not UX process. These files explain frontend technical particularities in the same accessible voice as the existing three. They say **nothing** about the design mandate, the personas, the card sort, or this dossier.

### 4.1 Restructure

Move, preserving git history:

| From | To |
|---|---|
| `learning-mode/01-Building-the-Foundation.md` | `learning-mode/01-architecture-foundation/01-Building-the-Foundation.md` |
| `learning-mode/02-Fixing-What-Broke.md` | `learning-mode/01-architecture-foundation/02-Fixing-What-Broke.md` |
| `learning-mode/03-Adding-Signups-and-Headcounts.md` | `learning-mode/01-architecture-foundation/03-Adding-Signups-and-Headcounts.md` |

`Glossary.md` and `README.md` stay at `learning-mode/` root — the glossary serves all chapters, and the README is the index. Both need their relative links updated.

### 4.2 The new chapter

Next numbered folder: `learning-mode/02-<slug>/`. Written **by the build agent at the end of 4B, after the code lands** — never drafted in advance, because the technical narrative must describe what was actually built rather than what was planned.

Subjects it should cover, as technical topics only:

- Why the data layer sits behind an interface, and what swapping an implementation buys.
- What a CSS custom property is, and why two themes defined together differ from one theme inverted.
- Why a record that only ever appends is easier to reason about than one that updates in place.
- Reading cached data before the network, and why that changes what a loading state looks like.
- Parsing a real CSV: date formats, splitting a text field, and what breaks.

**Not permitted in these files:** persona names, the How Might We, mandate or gate numbering, research findings, anything from `ux-ui/`.

### 4.3 Glossary

Extend `learning-mode/Glossary.md` with the new technical terms only — interface, dependency injection, custom property, append-only, cache. Same plain-language voice. No UX vocabulary.

---

## 5. Anti-scope-creep list

4B is the **frontend vertical slice only**. It must not:

- Add a backend, API, or database.
- Add authentication, authorization, or a session system.
- Add any NuGet or npm dependency.
- Add or replace a CSS framework. Bootstrap stays as-is.
- Touch `.github/workflows/` — run v7 owns it.
- Activate GitHub Pages — that is 4A-D's job.
- Modify anything in `ux-ui/00-initial-evaluation/`, `01-design-research/`, or `02-ideation/`.
- Alter `handoff/v1`–`v3` build records.
- Add a fourth core interaction, a new persona, or a bulk/roster surface.
- Add a notification preference screen — urgency is derived, never configured.
- Add a real notification transport. The mock writes changes; nothing is pushed anywhere.
- Delete `AttendanceTracker` or `SessionTracker` (§1.4).
- Write `learning-mode/` content before the code lands.
- Resolve the venue access list ownership question. `GateCheckResult` stays mocked and labelled.

---

## 6. Known-open items 4B inherits without resolving

| Item | Status |
|---|---|
| Venue access list ownership | Unresolved upstream (`06_HANDOFF.md` rec 4). Display and route only |
| The interval premise (ID-01) | Untested by declared constraint. All threshold values are assumptions |
| W5's navigation objection | Open; no bulk surface in v1 |
| Withdrawal | Specified in the model (`06_DATA-MODEL.md` §3), no UI affordance. Not a defect |
| Request access untested | No Gate 4 task exercises the write path |
| Theme persistence across sessions | 4B's implementation call; must not silently revert an explicit choice mid-session |

---

## 7. Commit discipline

Conventional-commit scoping, one commit per discrete segment. Author and committer `jdsaire`. Zero AI product attribution — the only permitted phrase anywhere is "AI coding assistant." Branch `deploy/v8-<slug>`, PR against `main`, left unmerged for review.

---

✅ GATE 7 COMPLETE — `07_BUILD-BRIEF.md`
