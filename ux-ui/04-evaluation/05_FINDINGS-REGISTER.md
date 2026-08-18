# Findings Register

**Repo path:** `ux-ui/04-evaluation/05_FINDINGS-REGISTER.md`
**Direct continuation of:** `00-initial-evaluation/findings-register.md` — same severity-band rule
text, unchanged
**New ID scheme:** `04-CRIT-NN` / `04-MAJ-NN` / `04-MIN-NN`, distinct from `00`'s `UX-C-NN` /
`UX-MAJ-NN` / `UX-MIN-NN` so the two registers are never confused when read together
(`00_SCOPE.md` §8)
**Requirement:** every one of `00`'s 26 findings gets an explicit disposition — Fixed, Still Open,
Regressed, or Not Applicable. None may be silently dropped (`00_SCOPE.md` §3 item 8)

---

## How severity was decided

Unchanged from `00`, restated rather than re-argued:

| Band | Rule |
|---|---|
| **Critical** | A user in a foreseeable situation either cannot finish a main task, or finishes it believing something happened that did not. Also covers shutting out an entire way of using the app, such as keyboard-only or screen-reader use. |
| **Major** | The task can be finished, but with friction or confusion that happens every single time — or an accessibility barrier that makes access harder without fully blocking it. |
| **Minor** | Cosmetic, inconsistent, or rough around the edges. There is an obvious workaround and the user is unlikely to be misled. |

---

## Part 1 — Disposition of every `00` finding

### Critical (4 of 4 dispositioned)

| ID | Finding | Disposition | Evidence |
|---|---|---|---|
| **UX-C-01** | Every event shown as an editable form, even when browsing | **Fixed** | `EventCard` defaults `ReadOnly="true"`; both list and request-flow call sites pass `ReadOnly="true" AllowEdit="false"` (`EventList.razor:98-99`). List entries render as structured text with real `<label for=...>` pairing when editable, not bare inputs |
| **UX-C-02** | Edits are silently thrown away | **Not Applicable** — the surface this depended on no longer exists in that form. `00`'s own register named this "inseparable from UX-C-01: fixing UX-C-01 largely resolves this one." With browsing read-only by default and the write path (`/request/{id}`) a dedicated form with no pre-population to lose, there is no longer an editable-then-discarded state to lose data from |
| **UX-C-03** | Registration form fields have no usable name for assistive technology | **Fixed** | `RequestAccessForm.razor:36,54` — labeled inputs, `id`-associated errors, `role="alert" aria-live="polite"`. Confirmed under `04_ACCESSIBILITY-AUDIT.md` §1, WCAG 3.3.1/3.3.2/4.1.2 all re-verified PASS |
| **UX-C-04** | "Back to events" leaves the app entirely via an absolute-slash href | **Fixed** | Full `href=` audit of every `.razor` file in `src/FifaPressApp` (Gate 5, this run) finds zero absolute-slash (`href="/..."`) links anywhere. Every internal link is base-relative (`href="matches"`, `href="record"`, `href="help"`, `href="signin"`) or the empty-string brand link, consistent with `.github/workflows/deploy-pages.yml`'s base-path rewrite. `00` marked this REASONED, not CODE-VERIFIED, for lack of a live click-test; this run is also source-only, so the same caveat applies — but the specific defect (a stray absolute link) is verifiably absent from the source this time, which `00` could not say of its own finding |

### Major (12 of 12 dispositioned)

| ID | Finding | Disposition | Evidence |
|---|---|---|---|
| **UX-MAJ-01** | Date field has no name at all | **Fixed** | `EventCard.razor:18` — `<label for="@DateInputId">Date</label>` |
| **UX-MAJ-02** | Placeholders used instead of labels, vanish when populated | **Fixed** | Same fix as UX-MAJ-01 — explicit `<label for=...>` on all three `EventCard` fields, not placeholder-only |
| **UX-MAJ-03** | Error messages not connected to their fields | **Fixed** | `RequestAccessForm.razor:36,54` — `id`-bound errors with `role="alert" aria-live="polite"` |
| **UX-MAJ-04** | Focus outline switched off on the element that receives focus | **Fixed** | No `outline: none` on `h1:focus` anywhere in `app.css`; `.btn:focus` etc. draw from `var(--color-focus-ring)`, independently contrast-verified in `04_ACCESSIBILITY-AUDIT.md` §1 |
| **UX-MAJ-05** | Successful registration not announced, focus not moved | **Partially Fixed — see `04-MAJ-01` below** | Form-level *errors* now carry `aria-live` (`SignIn.razor:107,127`, `RequestAccessForm.razor:36,54`), closing the error half of this finding. The success half is not closed: a newly-arrived `ChangeRow` on `/record` has no `aria-live` region. Carried forward as its own new finding rather than marked simply Fixed, since the original finding was about a *successful* action's announcement specifically |
| **UX-MAJ-06** | A registration cannot be undone | **Still Open** | `WithdrawRequestAsync` exists in the data layer (per the v12 Completion Report's own open-items table) but is reachable from no screen. Explicitly restated as unresolved at every handoff since v9 — the same shape of gap, now on the app's `/request/{id}` write path instead of `EventEase`'s registration. See `03_HEURISTIC-EVALUATION.md` Heuristic 3 |
| **UX-MAJ-07** | No way to skip past form controls to reach content | **Fixed** | `MainLayout.razor:13` — `<a href="#main-content" class="skip-link">`. Independently, the ~250-focusable-element problem this finding described is resolved a second way by UX-C-01's fix |
| **UX-MAJ-08** | Fifty events, no search/filter/sort/paging | **Fixed** | `EventList.razor:41-71` — live search, group filter, played/not-yet-played status filter, all backed by `FixtureQuery.Apply` (the v10/4B-R extraction); results paginated (`TotalPages`/`PageSize`, `:186-189`) |
| **UX-MAJ-09** | Validation message text fails minimum contrast | **Fixed** | `.validation-message` reads `var(--color-danger)` (`app.css:216-218`), independently recomputed in `04_ACCESSIBILITY-AUDIT.md` §1 at 4.53:1 light / 8.32:1 dark, both clearing the 4.5:1 floor |
| **UX-MAJ-10** | Mobile menu button doesn't report open/closed state | **Fixed** | `NavMenu.razor:8` — `aria-expanded="@(!collapseNavMenu)" aria-controls="nav-menu"` |
| **UX-MAJ-11** | Nothing indicates where you are; duplicate tab titles | **Fixed** | Active nav state via ordinary `NavLink` prefix matching on real paths (`NavMenu.razor:40-56`); breadcrumbs on `/events/{id}` and `/request/{id}` (`aria-label="breadcrumb"`); `PageTitle` computed per-screen from real content (`EventDetails.razor`'s `PageHeading` includes the match number) |
| **UX-MAJ-12** | Only global link sends users to unrelated vendor documentation | **Fixed** | No link in `NavMenu.razor` points at framework or vendor documentation anywhere. The nav is three named destinations (`record`, `matches`, `help`) plus language, theme, and conditional sign-out — `help` is now a genuine in-app destination, not an outbound link (`03_HEURISTIC-EVALUATION.md` Heuristic 10) |

### Minor (10 of 10 dispositioned)

| ID | Finding | Disposition | Evidence |
|---|---|---|---|
| **UX-MIN-01** | "1 people registered" pluralization bug | **Not Applicable** | The attendee-count feature this bug lived in doesn't exist in this concept — the app tracks a holder's own access, not a public attendee count for an event. No successor surface carries an equivalent count to mispluralize |
| **UX-MIN-02** | Attendee count on one page but not the other | **Not Applicable** | Same reasoning as UX-MIN-01 — the feature itself has no successor |
| **UX-MIN-03** | Tab says "Events", heading says "Upcoming Events" | **Fixed** | Tab title and heading both resolve from the same `L[Locale, ...]` resource key on every screen checked (e.g. `/matches`'s `PageTitle` and `<h1>` both read `matches.title`), eliminating the class of bug where the two were authored independently |
| **UX-MIN-04** | Speculative not-found copy; failure looks like success | **Fixed** | `common.noMatchNumber`: "No match has this number." — no claimed cause. `NotFound.razor` renders distinct `notFound.title`/`notFound.body`, not a shared heading with the success path. `MyAccess.razor`'s own empty/error states are deliberately distinguished by design (component header comment, cited in `03_HEURISTIC-EVALUATION.md` Heuristic 9) |
| **UX-MIN-05** | Click handler on a plain `<div>` | **Not Applicable** — no live equivalent found. `NavMenu.razor`'s toggle button is a real `<button>` (`:8`); no bare-`<div>`-with-`@onclick` pattern was found anywhere in the current `Layout/` or `Pages/` tree during this audit's review of those files |
| **UX-MIN-06** | Text lines run full window width, no `max-width` | **Not independently re-verified this run** — recorded as **Still Open (unconfirmed)** rather than guessed at. `MainLayout.razor.css` was read for sidebar width (§7.2 of `04_ACCESSIBILITY-AUDIT.md`) but a `max-width` check on `article`/content-column width specifically was not performed in this pass. Flagged for `06_USABILITY-ASSESSMENT.md` or a future run rather than asserted either way |
| **UX-MIN-07** | 250px sidebar holding one link | **Fixed** | The sidebar now holds three real destinations, session identity, language, theme, and (signed in) sign-out — confirmed in `03_HEURISTIC-EVALUATION.md` Heuristic 8. No unused scaffolding remains |
| **UX-MIN-08** | No meta description | **Fixed** | `wwwroot/index.html:7` — `<meta name="description" content="A media-accreditation companion for journalists covering the 2026 World Cup — see what's changed with your access, before you're turned away." />` |
| **UX-MIN-09** | No empty state for the event list | **Fixed** | `EventList.razor:80-82` — `FilteredFixtures.Count == 0` renders `EmptyStateMessage`, distinct from the loading and loaded states |
| **UX-MIN-10** | Unused `Virtualization` import pointing at the unaddressed fix | **Not Applicable** | No `Virtualization` import found anywhere in the current `_Imports.razor` or `EventList.razor` — the unused import itself is gone, and the problem it pointed at (UX-MAJ-08) is independently Fixed above via search/filter/paging rather than virtualization specifically. A different, valid technique closing the same underlying gap is not a regression |

### Disposition summary

| Disposition | Count |
|---|---|
| Fixed | 18 |
| Not Applicable | 5 |
| Still Open | 2 |
| Partially Fixed (spun into a new finding) | 1 |
| **Total** | **26** |

Zero **Regressed**. Nothing that `00` found working now fails.

---

## Part 2 — New findings, this run

### `04-CRIT-01` — A conditional, undecided change renders as a decided one on two screens

**Where:** `MyAccess.razor:281-291` (`StatusFor`) · `EventDetails.razor:235,283-284` (`status =
FoldStatus(DateTime.MaxValue)`)
**Heuristic:** Visibility of system status · Consistency and standards
**WCAG:** 4.1.3 Status Messages (AA)
**Method:** CODE-VERIFIED, independently confirmed in three locations plus one corroborating record

Both `MyAccess.razor`'s per-match status word and `EventDetails.razor`'s headline status word
derive from a change's `Kind` alone, with no check against `Urgency` or `EffectiveUtc`. A
`Foreseeable` (or, for a holder with a named contact, `Silent`) change that has not taken effect —
seeded `ch-005`, Amina's quarter-final entitlement, conditional on an unplayed Round-of-16 fixture
— renders as an unqualified **"Access withdrawn"** (`status.revoked`) on the My Access headline,
directly above a `ChangeRow` for the identical change correctly marked `ForeseeableBadge`: "Not
decided yet — depends on a match still to be played." Visiting the entitlement's own Match Detail
page (`/events/98`) states it more assertively still: "Access to this match has been withdrawn. See
your record for the reason." No CSS state exists for a conditional match-status word — only
`--granted` and `--revoked` are defined.

**First observed:** `02_TASK-RESULTS.md`, Attempt 1 (W1, Task 1). **Reproduced independently** in
Attempt 2 via `/events/98` (English), Attempt 4 via `/record` (Spanish — same defect, same words,
translated), and via Tomás's unscored observation, where the same headline renders identically for
a `Silent`-classified change that never triggers `ForeseeableBadge` at all — arguably a worse
instance, since nothing on his record ever surfaces the corrective context.

**Why Critical, not Major:** `00`'s own rule for this band is a user who "finishes [a task]
believing something happened that did not." This is the named failure mode of Task 1 itself
(`04_TASKS-AND-SCENARIOS.md` §3: "she believes a foreseeable change has already happened"),
produced not by a misreading but by a sentence the app states as fact, on the first screen every
signed-in holder sees. No test in the 409-test suite exercises the relationship between
`MatchAccessLine`/`StatusFor`'s output and `Change.Urgency`.

**Same underlying gap explains two heuristic scores at once** (`03_HEURISTIC-EVALUATION.md`
Heuristic 1 Partial, Heuristic 4 Fails) and one WCAG failure (`04_ACCESSIBILITY-AUDIT.md` §3). A fix
at this one point is the single highest-value remediation item this dossier identifies.

---

### `04-MAJ-01` — A newly-arrived change is confirmed visually, not to assistive technology

**Where:** `MyAccess.razor` (no `aria-live` region anywhere in the file) · `ChangeRow.razor` (same)
**WCAG:** 4.1.3 Status Messages (AA)
**Method:** CODE-VERIFIED

Submitting a request navigates to `/record`, where the newly-written `ChangeRow` opens
pre-expanded and animates in (`JustArrived`, `ChangeRow.razor`'s own header comment: "the
confirmation moment the request path was missing"). This is a real, deliberate confirmation
mechanism — and it is entirely visual. No `aria-live` region announces the arrival. Contrast with
the same file tree's own form-validation errors, which do use `aria-live="polite"`
(`SignIn.razor:107,127`, `RequestAccessForm.razor:36,54`) — the pattern exists elsewhere in the app
and simply wasn't applied to this specific, successful-write case.

**Disposition of `UX-MAJ-05`, formally:** this finding is the surviving half of `00`'s
UX-MAJ-05 (successful registration not announced). The error-announcement half of that original
finding is Fixed; this is what's left.

**Severity:** Major, not Critical — the task itself still succeeds and a sighted user gets a clear
signal; a screen reader user gets no signal a write occurred at all, which is friction/exclusion
every time, not a wrong belief formed.

---

### `04-MAJ-02` — No demo record exists for the Freelance track

**Where:** `Services/DemoAccountStore.cs:65-82` — exactly two accounts (`Amina`,
`TrackId.MemberAssociationQuota`; `Tomas`, `TrackId.RightsHolder`)
**Method:** CODE-VERIFIED

`Models/Track.cs` defines three `TrackId` values — `MemberAssociationQuota`, `RightsHolder`,
`Freelance` — but only the first two have a seeded demo account. W2, the freelance archetype
(`01_TASK-PROTOCOL.md` §5: "no federation behind her... a refusal or a silent contraction has
nowhere softer to land"), has no record to sign into. `02_TASK-RESULTS.md`'s Attempts 4-6
substitute Amina's record under W2's reading lens rather than exercise genuinely
freelance-track-specific behavior (e.g. a refusal with no institutional escalation path).

**Severity:** Major, not Critical — nothing in the shipped build is broken; a real capability this
mandate's own task set implies (a second, structurally different track represented) is simply
unbuilt. No task fails because of this; the coverage is narrower than the roster's own three-track
concept implies.

---

### `04-MIN-01` — Nav "Sign out" row is the highest-risk ES/PT layout pinch point, unconfirmed

**Where:** `Layout/NavMenu.razor.css:37,40` (fixed `height: 3rem; line-height: 3rem` on nav rows) ·
`wwwroot/i18n/es.json` `nav.signOut`: "Cerrar sesión" (13 chars vs. English "Sign out"'s 8 — a 62%
length increase)
**Method:** REASONED — flagged for human confirmation, per `04_ACCESSIBILITY-AUDIT.md` §4

A fixed height paired with a matching single-line `line-height` does not gracefully absorb a
second line if the label wraps at the narrow (`<641px`) breakpoint. This is the one pinch point
`11_I18N.md` §7.2 asked 4E to verify by render that this dossier's source-only method cannot
settle — narrowed here to the specific row worth checking first, rather than left as an
undifferentiated "check all of §7.2."

**Severity:** Minor — cosmetic if it occurs (a wrapped label, not a lost one), and easily fixed if
confirmed.

---

### `04-MIN-02` — `GateCheckResult` named in the frozen task flowcharts is not a discrete component

**Where:** `04_TASKS-AND-SCENARIOS.md` §5 flowchart, node "GateCheckResult displays the
disagreement" · shipped as an inline conditional block in `EventDetails.razor` (`detail__gate-disagree`)
**Method:** CODE-VERIFIED

Behaviorally equivalent to what the flowchart describes — displays both sides, timestamps them,
routes to Help, never adjudicates — but not a separately named, reusable component the way the
flowchart's capitalized-and-titled node name implies. First noted in `02_TASK-RESULTS.md`, carried
here as a documentation-precision item rather than a functional defect.

**Severity:** Minor — no user-facing consequence; relevant only to a future dossier author tracing
the flowchart against source a second time.

---

## Part 3 — What this run adds to `00`'s strengths list

`00`'s "What the app does well" section (S-01 through S-12) is not re-verified item-by-item here —
most of its subjects (focus-on-navigate, keyed list rendering, data-annotation validation, the
GitHub Pages base-path rewrite) are structural choices this dossier's other gates already confirm
are intact rather than regressed. Two new strengths, specific to this build, are worth naming
because no `00`-era finding anticipated them:

| # | Strength | Evidence |
|---|---|---|
| S-13 | Native `<details>/<summary>` used for all two-layer disclosure, giving correct expand/collapse semantics with no custom ARIA management and no JavaScript dependency | `ChangeRow.razor`, `Help.razor` |
| S-14 | The `lang` document attribute updates live on every language switch, with the reasoning stated in the source itself (a screen reader must not read Spanish prose in an English voice) | `wwwroot/js/locale.js:23-24,55` |

---

## What this file hands to Gate 6 and Gate 7

- **26 of 26 `00` findings dispositioned**, zero silently dropped, zero Regressed.
- **One `04-CRIT`, two `04-MAJ`, two `04-MIN`** newly identified this run.
- `04-CRIT-01` is the load-bearing item for `07_REMEDIATION-SCOPE.md` — named as the single
  highest-value fix across three separate gates now (Heuristics, Accessibility, and here).
- `UX-MAJ-06`'s Withdrawal gap remains **Still Open**, unresolved across five runs (v9 through this
  one) — `07_REMEDIATION-SCOPE.md` must decide explicitly whether building the affordance is in
  scope for a pass bounded to "what 4E already built," rather than let it default to Open again by
  omission.

---

✅ **GATE 5 COMPLETE** — `05_FINDINGS-REGISTER.md`
