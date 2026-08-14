# 03 — UI DESIGN DECISIONS

**Repo path:** `ux-ui/03-ui-prototyping/03_UI-DECISIONS.md`
**Inputs read directly, not re-derived:** `src/FifaPressApp/wwwroot/css/app.css` (130 lines, confirmed), `src/FifaPressApp/Layout/MainLayout.razor.css` (32 lines, confirmed), `src/FifaPressApp/Layout/NavMenu.razor.css` (83 lines, confirmed), `src/FifaPressApp/wwwroot/lib/bootstrap/dist/css/bootstrap.css` (full source), `src/FifaPressApp/Components/EventCard.razor`, `src/FifaPressApp/Pages/EventList.razor` — all retrieved from the repo archive at `main`, matching HEAD `03663db` per Gate 0. Contrast ratios in §2 are computed, not estimated — method stated there.

---

## 0. CSS inheritance audit

### 0.1 `app.css` — what actually exists

Three hardcoded colours the dossier names, confirmed present: `#1b6ec2` (skip-link background, `.btn-primary` background, loading-circle fill), `#0071c1` (link colour), `#258cfb` (`h1:focus` outline and the `.btn:focus`/`.form-control:focus` box-shadow — the `00-initial-evaluation/` remediation).

Four more hardcoded colours the dossier does **not** name, found on read: `#1861ac` (`.btn-primary` border), `#26b050` (`.valid.modified` outline), `#dc3545` (`.invalid` outline and `.validation-message` text — this is Bootstrap's own danger red, hardcoded rather than referenced), `#c02d76` (`code` text), `#b32121` (`.blazor-error-boundary` background).

**A real, pre-existing defect, found and verified, not assumed.** `.valid.modified` uses `#26b050` as a 1px outline on white. Computed contrast: **2.83:1**. The applicable floor for a non-text UI indicator (WCAG 2.2 SC 1.4.11) is 3:1. **This fails today**, before any overhaul touches it. It is not a casualty of this gate's decisions; it is inherited breakage this gate is the first to check. `[SOURCED — verified by computation, §2]`

**A real, structural conflict for dark mode, found and verified.** `#blazor-error-ui` declares `color-scheme: light only`. That is a hard, page-level override — it forces the browser's native form-control and scrollbar rendering to light regardless of the app's own theme, for this one error banner. It must be removed or scoped, not silently carried into a first-class dark mode. `[SOURCED]`

**One rule already anticipates tokens.** `.form-floating > .form-control::placeholder` reads `color: var(--bs-secondary-color)` — a Bootstrap 5 CSS custom property, already in use, already themeable by redefinition rather than replacement. This is the load-bearing fact behind §1's decision.

### 0.2 `MainLayout.razor.css` — the layout is a sidebar, not a top bar

`.page` is a flex column below 641px and a flex row at and above it. `.sidebar` is full-height, sticky, 200px wide, **only at ≥641px**. Below that width there is no persistent sidebar column at all — `.sidebar` returns to normal document flow as a full-width block above `main`.

**This directly conflicts with the dossier's literal instruction** — "trigger sits top right, at every breakpoint" implicitly assumes a page with a top bar spanning the viewport. This app has no such element at desktop width; content begins immediately beside the sidebar with nothing above it. Resolved explicitly in §3.2, not silently reinterpreted.

### 0.3 `NavMenu.razor.css` — confirms the sidebar carries scaffold-era icons

`.bi-house-door-fill-nav-menu`, `.bi-plus-square-fill-nav-menu`, `.bi-list-nested-nav-menu` are the stock Blazor project-template icons (home / add-new / nested-list), inline-SVG data URIs, unrelated to this app's actual content. They were never redrawn when the app's purpose changed. `.top-row` (min-height 3.5rem, `rgba(0,0,0,0.4)` background) is the collapsed-state header bar containing the toggler and brand at <641px, and becomes the top strip of the sidebar column at ≥641px. `.nav-item ::deep a.active` uses `rgba(255,255,255,0.37)` for the active state — a translucency-based indicator, not a hard colour, which travels cleanly to a second theme without redefinition.

### 0.4 Bootstrap

`bootstrap.css` is the full uncompressed distribution (12,056 lines). Confirmed: Bootstrap 5's own build already exposes colours as CSS custom properties (`--bs-*`), which is what `app.css` line 125 already leans on. This means theming does not require replacing Bootstrap's rule set — it requires redefining the custom properties Bootstrap already reads from, at `:root` and again under a theme selector.

---

## 1. Resolution — hybrid, stated once

**Verdict: hybrid.** The sidebar shell, its collapse behaviour, and Bootstrap's component and utility layer are **retained**. The colour system is **overhauled entirely** into tokens layered over Bootstrap's own custom properties. Structure is kept because it already works and a full rebuild would spend this mandate's smallest-defensible-slice budget (dossier preamble) on re-solving a responsive collapse problem that has a working answer. Colour is overhauled entirely, not built over, because "build over" would mean adding a second, competing colour system beside eleven scattered hex values rather than replacing them — the opposite of what a token system is for.

**Bootstrap's fate: partially retained.** Kept as the grid, form-control, and button primitive layer. Its default colour values are overridden at the custom-property layer, not by fighting its selectors with higher-specificity rules. `#dc3545`, hardcoded in `app.css` for `.invalid` and `.validation-message`, is retired as a literal and replaced with `var(--color-danger)`, which is itself Bootstrap's own `--bs-danger` redefined per theme — so the app's danger colour and Bootstrap's danger colour become the same value again, which they already should have been.

**Everything the overhaul removes is re-provided, named here so nothing is silently dropped:**

| Removed literal | Re-provided as | Where |
|---|---|---|
| `#1b6ec2` (skip-link bg, btn-primary bg, loading fill) | `--color-action-primary` | §2 |
| `#0071c1` (link colour) | `--color-link` | §2 |
| `#258cfb` (focus outline/ring) | `--color-focus-ring` | §2 |
| `#1861ac` (btn-primary border) | `--color-action-primary-border` | §2 |
| `#26b050` (valid outline — **replaced with a corrected value**, see §2.1) | `--color-success` | §2 |
| `#dc3545` (invalid outline, validation text) | `--color-danger` (mapped from `--bs-danger`) | §2 |
| `#c02d76` (code text) | `--color-code` | §2 |
| `#b32121` (error boundary bg) | `--color-danger-surface` | §2 |
| `color-scheme: light only` on `#blazor-error-ui` | Removed; banner now themes with the page | §2.4 |
| Sidebar gradient `rgb(5,39,103)`→`#3a0647` | `--color-sidebar-grad-start` / `-end`, themed pair | §2 |

---

## 2. Design tokens — both themes together, verified

Tokens are named CSS custom properties, set at `:root` for light and re-set under `[data-theme="dark"]` for dark, never light-first-then-inverted. Every colour pair below was computed with the WCAG 2.2 relative-luminance formula, not estimated. Text uses the 4.5:1 floor (SC 1.4.3); focus rings, outlines and other non-text UI indicators use the 3:1 floor (SC 1.4.11) — the two are not interchangeable and this table applies the correct one to each row.

### 2.1 Colour — computed and verified

| Token | Light value | Ratio (light) | Dark value | Ratio (dark) | Verdict |
|---|---|---|---|---|---|
| `--color-surface` / `--color-text` | `#ffffff` / `#1a1a1a`* | n/a — see body row | `#121212` / `#e8e8e8` | n/a — see body row | — |
| Body text on surface | `#1a1a1a` on `#ffffff`* | 17.40:1 | `#e8e8e8` on `#121212` | 15.29:1 | **PASS both** |
| `--color-link` | `#0071c1` (kept, inherited) | 5.08:1 | `#6fb3f2` | 8.39:1 | **PASS both** |
| `--color-focus-ring` | `#258cfb` (kept, inherited) | 3.37:1 (3:1 floor) | `#6fb3f2` | 8.39:1 (3:1 floor) | **PASS both** |
| `--color-action-primary` (button bg, text on it) | `#1b6ec2` (kept), white text | 5.18:1 | `#3d8bd4`, near-black text `#0a0a0a` | 5.51:1 | **PASS both** |
| `--color-danger` | `#dc3545` (kept, = `--bs-danger`) | 4.53:1 text / 4.53:1 outline (3:1 floor) | `#ff6b6b` | 6.75:1 text / 6.75:1 outline | **PASS both** |
| `--color-success` | **`#178040` — corrected, was `#26b050` at 2.83:1, failing** | 5.00:1 (3:1 floor) | `#4fd07a` | 9.49:1 (3:1 floor) | **PASS both — light value changed from inherited** |
| `--color-code` | `#c02d76` (kept, inherited) | 5.41:1 | `#e879b0` | 6.95:1 | **PASS both** |
| `--color-sidebar-text` on `--color-sidebar-grad-*` | `#ffffff` on gradient stops `rgb(5,39,103)`→`#3a0647` (kept) | 14.07:1 / 16.19:1 | `#f0f0f0` on darkened stops `#04173d`→`#22032c` | 15.42:1 / 16.49:1 | **PASS both, worst-case stop checked** |
| `--color-nav-item` (idle, on sidebar) | `#d7d7d7` (kept, inherited) | 11.25:1 | `#c9c9c9` | 11.35:1 | **PASS both** |
| `--color-danger-surface` (error boundary bg), text on it | `#b32121` (kept), white text | 6.65:1 | `#7a1414` (darkened), white text | 10.84:1 | **PASS both** |
| `--color-stale-text` (new — CH-8) | `#6b6b6b` | 5.33:1 | `#b3b3b3` | 8.93:1 | **PASS both** |

*Body text colour is a new token; the existing `app.css` never sets one explicitly and relies on browser default black, which the audit treats as an unstated gap rather than a value to preserve.

**One inherited value was corrected, not carried.** `--color-success` in light mode is `#178040`, not the inherited `#26b050`. The old value fails its own floor at 2.83:1; §0.1 names this as pre-existing breakage. Carrying a known failure forward under a new variable name would be laundering it, not fixing it.

**Active-state and hover-state colours are unaffected.** `.nav-item ::deep a.active` (`rgba(255,255,255,0.37)`) and `:hover` (`rgba(255,255,255,0.1)`) are translucency over the sidebar's own background, so they inherit whichever gradient stop is under them in either theme without a separate token.

### 2.2 Typography scale

`app.css` sets one family (`'Helvetica Neue', Helvetica, Arial, sans-serif`) and no scale — every size in the existing app is Bootstrap's default. Tokens introduced: `--font-size-body` (1rem), `--font-size-small` (0.875rem, matches `.nav-item`'s existing 0.9rem rounded to the scale), `--font-size-heading-3` (1.25rem), `--font-size-heading-1` (1.75rem). Family token: `--font-family-base`, value unchanged from the inherited stack — nothing here needed fixing.

### 2.3 Spacing, radius, elevation

Existing spacing is Bootstrap's rem-based scale, used directly (`0.5rem`, `1rem`, `2rem` appear throughout the audited files) — no override needed; tokenised as `--space-1` through `--space-4` mapping to Bootstrap's existing values so the app's own rules can reference a name instead of a raw rem figure. Radius: Bootstrap default (`0.375rem` on `.btn`, `.card`) kept, tokenised as `--radius-default`. Elevation: none exists in the audited files beyond `#blazor-error-ui`'s `box-shadow: 0 -1px 2px rgba(0,0,0,0.2)` — tokenised as `--elevation-banner`, and a second `--elevation-card` (`0 1px 3px rgba(0,0,0,0.12)` light / `0 1px 3px rgba(0,0,0,0.4)` dark) introduced for the new AccessCard component in §6, since cards did not previously need to lift off a coloured background.

### 2.4 The error-boundary dark-mode fix

`color-scheme: light only` is removed from `#blazor-error-ui`. Its background becomes `--color-danger-surface`, verified PASS in both themes above. This is a one-line change with a two-line consequence: without it, a person in dark mode who triggers a Blazor error sees a light-forced native form-control flash inside an otherwise dark page, which reads as a rendering bug rather than an error message. `[SOURCED]`

---

## 3. Dark/light mode — first-class

### 3.1 Default resolution and persistence

Default: system preference (`prefers-color-scheme`), read once on load. Explicit choice, once made via the trigger, is stored client-side and overrides system preference for the remainder of the session at minimum; whether it persists across sessions is a `localStorage`-equivalent decision that belongs to Run 4B's implementation, not this gate — specified here only as a requirement that an explicit choice must not be silently reverted by a later system-preference change mid-session. `[SIMULATED]`

### 3.2 Trigger placement — the real conflict, resolved

The dossier requires the trigger "top right, at every breakpoint." §0.2 established that this app has no top bar at desktop width — `.sidebar` sits flush left and `main` begins immediately beside it with nothing above the content column.

**Two ways to satisfy the instruction, both considered:**

**Rejected: read "top right" as relative to the sidebar's own header.** Place the trigger inside `.top-row`, which already exists at both breakpoints (full-width bar below 641px, sidebar-header strip above it). Cheapest possible change — zero new markup. **Rejected** because at desktop width `.top-row` sits at the *top-left* of the viewport, not the top-right — the sidebar is a left column. A person told "the toggle is top right" who then has to look left because they're on a laptop is exactly the kind of small inconsistency the concept's own Principle 1 argues against: the interface should not require the person to relearn where something lives.

**Adopted: a new, minimal persistent strip above `main`, present at every breakpoint.** Not a second full navbar — a slim bar (`--space-3` tall) containing only the theme trigger, right-aligned, positioned above the `<article>` content at ≥641px and above the collapsed content at <641px, independent of `.top-row`. This is a genuine new element, not a reinterpretation, and is named as such: it is the one place in this gate where a UI decision adds a component the existing layout did not have, rather than restyling one that did.

**Why this earns the cost.** Amina reads this app in a stadium concourse, phone in hand, moving between two devices across a tournament (§ Gate 2 layout note). A control that lives in a different corner depending on which device she's holding is a worse failure than it looks on a design system audit — it is the exact shape of inconsistency that erodes trust in a system whose entire premise is "you can trust what this screen tells you." `[SIMULATED]`

### 3.3 AA floor, verified in both themes

Every colour token in §2.1 is computed against its correct WCAG 2.2 threshold (4.5:1 text, 3:1 non-text UI) in both light and dark. All twelve rows pass; one inherited value (`--color-success` light) required correction to pass, not merely retagging. No token in this gate is asserted compliant without the number behind it.

---

## 4. The "EventEase" rebrand

**Navbar brand:** `FIFA Press App` — matching the repository's own public name (`README.md`, already the app's external identity) rather than inventing a second name for internal chrome. Unchanged across EN/ES/PT: per Gate 2 §4.1 and §5, the app's own product-level names stay in English, matching how "Access Record" was decided.

**Page `<title>` (host page, `wwwroot/index.html`):** `FIFA Press App` — the static boot-time title, locale-agnostic since it renders before Blazor establishes a locale. No per-page `<PageTitle>` system currently exists in the audited files; introducing one is a Gate 6/7 concern if the entity model calls for it, not decided here.

**Meta description:** `A media-accreditation companion for journalists covering the 2026 World Cup — see what's changed with your access, before you're turned away.` English, under 160 characters, written to name the concept's actual promise (Interaction 4.1/4.2) rather than generic app-store copy.

**Scope note.** `wwwroot/index.html` itself was not in this gate's required file list and was not audited for markup structure — these are the decided replacement *values* Run 4B applies; the surrounding HTML is Run 4B's to touch, per the `src/`-touch boundary in `00_SCOPE.md` §5. `[ASSUMPTION]` for the exact string wording; `[SOURCED]` for the constraint that they must replace "EventEase" per Gate 0 §Repo state.

---

## 5. Layout system

**Breakpoint: kept at 641px**, single breakpoint, unchanged from the inherited CSS. Two states, not three: below 641px, a single scrolling column with the sidebar collapsed behind the toggler; at and above it, sidebar-plus-content. This maps adequately onto the two personas the mandate scopes for — Amina on a phone, a coordinator on a laptop — without inventing a tablet-specific third state neither persona was defined to need.

**Density.** My Access (Gate 2 §6) is specified as a single column at every width — it does not gain a second column at desktop, because the headline state and the change history are sequential by nature (CH-2: newest-effective-first) and a two-column split would imply they can be read in either order. Matches and Help may use Bootstrap's existing grid utilities for multi-column card layouts at ≥641px, since fixtures have no inherent reading order between them.

**The phone-in-a-concourse case.** No layout decision in this gate assumes a stable network. §3.2's new trigger strip and the My Access single column are both static, CSS-only layout — nothing in this gate requires JavaScript to compute or animate on load, which matters directly for CH-10 (offline-first headline resolution) even though CH-10 itself is a data concern, not a layout one.

---

## 6. Component inventory — mapped to the three interactions

**Reuse before invention.** `EventCard.razor` is the real precedent, read directly: parameters paired with matching `...Changed` callbacks for two-way binding, a `ReadOnly`-driven default presentation with edit as an explicit one-click toggle, per-field inline validation wired through `aria-describedby`, and instance-unique element IDs so multiple cards on one page (`EventList.razor`) don't collide. This pattern is sound and is extended, not replaced.

| Component | Interaction | Reuse or new | Basis |
|---|---|---|---|
| **AccessCard** | 4.1, 4.3 — the headline "what I hold now" | New, but structurally an `EventCard` variant: read-only by default, no edit toggle (this data is not user-editable), same instance-ID collision-avoidance pattern | `EventCard.razor` precedent |
| **ChangeRow** | 4.1 — one entry in the change list | New. Fields map directly to Gate 2 C07–C10 (what changed, why, next step, timestamp); CH-3's "revised against original" display is this component's job | Gate 2 §4.2, §4.3 CH-3 |
| **ForeseeableBadge** | 4.2 — a change that has not landed yet | New, small — wraps a `ChangeRow` to distinguish CH-7's conditional wording from a landed change | Gate 2 §4.2 C14, CH-7 |
| **StaleIndicator** | 4.3 — CH-8, appears on any surface reading cached data | New, uses `--color-stale-text` (§2.1), carries its own timestamp per CH-8 | Gate 2 §4.4 |
| **MatchCard** | Supports 4.2 (fixture dependency) | Reuse of `EventCard`'s read-only presentation directly — name, date, location map one-to-one onto match name, date, venue; no new fields needed | `EventCard.razor`, unchanged |
| **RequestAccessForm** | Supports 4.1 (initiates a change) | Reuse of `EventCard`'s validated-input pattern (`OnNameInput`-style handlers, inline `role="alert"` errors), renamed from `Registration.razor`'s existing form fields per Gate 2 §4.1's rename | `Registration.razor`, `EventCard.razor` validation pattern |
| **ThemeTrigger** | Not interaction-specific — cross-cutting | New, per §3.2 | This gate |
| **GateCheckResult** | 4.3 — C17 | New. Explicitly a *displayed outcome only* — no assumption about which system supplies it, per `06_HANDOFF.md` rec 4 carried in Gate 2 §4.4 | Gate 2 §4.4 |

**Not reused.** `EventList.razor`'s current page-level layout (a flat list of `EventCard`s) is not reused as My Access's structure, because My Access is one person's single record, not a list of many items of the same kind — the page pattern doesn't transfer even though the card component does.

---

## 7. Heuristic compliance — Nielsen's 10, continuing `00-initial-evaluation/`'s set

Applied against Gate 2's named surfaces: **My Access**, **Matches**, **Match detail**, **Request Access form**, **Sign in**, **Help**. Visibility of system status is weighted heavily per the dossier's own instruction, since it is Principle 1 restated as a heuristic.

| # | Heuristic | My Access | Matches / Match detail | Request Access | Sign in | Help |
|---|---|---|---|---|---|---|
| 1 | **Visibility of system status** | Headline state + StaleIndicator on every read (CH-8) — the heaviest-weighted pass in this gate | MatchCard shows date/venue plainly; no entitlement status leaks here per Gate 2's split | ChangeRow write is confirmed by the resulting entry appearing in My Access, not a separate toast | Standard auth feedback; not concept-specific | N/A — static content |
| 2 | **Match between system and real world** | "Change", "valid until" — Gate 2 §4.1's ontology work exists for this heuristic specifically | Existing match data, unchanged | "Request access", not "Register" — Gate 2's highest-value rename | Standard | Plain-language requirement carries from `01-design-research` |
| 3 | **User control and freedom** | No delete/undo on history — correct, since CH-1 forbids a second write path; freedom here means *reading*, not editing, the record | Standard navigation | Cancel path needed; not yet specified — **flagged for Gate 5** | Standard | Standard |
| 4 | **Consistency and standards** | ThemeTrigger fixed position (§3.2) is the consistency mechanism | MatchCard reuses `EventCard` styling directly | Reuses `EventCard`'s validation pattern | Standard | Standard |
| 5 | **Error prevention** | N/A, read-only surface | N/A | Inline validation per-field, `aria-describedby`, inherited from `EventCard` | Standard | N/A |
| 6 | **Recognition rather than recall** | Full history visible in-place (§4.2); nothing requires remembering a prior state | Fixture dependency (C15) shown inline where relevant, not requiring a separate lookup | Standard | Standard | Staged-by-lifecycle organisation (Gate 1 adopt #8) |
| 7 | **Flexibility and efficiency of use** | Not addressed in this gate — no power-user path specified; **flagged for Gate 4/5** | Standard | Standard | Standard | Standard |
| 8 | **Aesthetic and minimalist design** | Single column, sequential (§5) — deliberately minimal given CH-2's ordering logic | Grid at desktop, list at mobile | Standard form | Standard | Standard |
| 9 | **Help users recognize, diagnose, and recover from errors** | CH-9: a change with no actionable step still states who decided and what remains open | GateCheckResult routes to escalation on failure (Gate 2 §4.4) | Inline field errors, existing pattern | Standard | C20/C21 reachable from point of failure, not just browsed to (Gate 2 §2.3) |
| 10 | **Help and documentation** | Links to Help's staged guidance | Links to Help | Links to Help | Standard | Is the surface |

**Two gaps this table surfaces, not fixed here.** Heuristic 3 (cancel path on Request Access) and heuristic 7 (no flexibility/efficiency path anywhere in this gate) are real and unaddressed — carried to §8 rather than papered over with a table cell that claims a pass that hasn't been earned.

---

## 8. Carried into Gate 4 and beyond

| # | Item | Owner |
|---|---|---|
| 1 | Explicit-choice theme persistence across sessions vs session-only — not decided, deferred to 4B implementation | Run 4B |
| 2 | No cancel/undo path specified for Request Access (heuristic 3 gap) | Gate 5 |
| 3 | No power-user or efficiency path specified anywhere (heuristic 7 gap) | Gate 4/5, or explicitly named as out of v1 scope |
| 4 | `wwwroot/index.html` markup itself not audited — only replacement string values decided | Run 4B, within `src/`-touch boundary |
| 5 | New `--elevation-card` token has no precedent in the inherited CSS — first use is Gate 5/6 | Gate 5 |
| 6 | GateCheckResult's data source remains unsettled per `06_HANDOFF.md` rec 4 | Gates 5–6 |

---

✅ GATE 3 COMPLETE — `03_UI-DECISIONS.md`
