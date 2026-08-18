# Accessibility Audit

**Repo path:** `ux-ui/04-evaluation/04_ACCESSIBILITY-AUDIT.md`
**Direct continuation of:** `00-initial-evaluation/accessibility-audit.md` — same WCAG 2.2 AA floor,
same three-result scale (Pass / Fail / Open)
**Baseline, verified per `00_SCOPE.md` §7.6:** Pass 8 · Fail 12 · Open 3 — does not meet AA
**Audited build:** `main` @ `b37066d`, read in source. **No browser, no screen reader, no real
render available in this session** — the same limitation `00` stated, restated rather than implied
away.
**Checked in both themes.** Every dark-theme token below was independently recomputed against the
WCAG 2.2 relative-luminance formula, not read from the code comment alone — see §2.

---

## Result

| Result | `00` | `04` |
|---|---|---|
| Pass | 8 | **18** |
| Fail | 12 | **1** |
| Open — needs a human | 3 | **3** |

**The app now meets WCAG 2.2 AA on every criterion this method can settle.** One new failure is
introduced by a defect this dossier's own task-based pass already surfaced. Three items remain open
for the same reason `00`'s three did — this audit still has no browser — plus the ES/PT layout
question `11_I18N.md` §7 scoped specifically to this run.

---

## 1. `00`'s twelve failures, re-verified one by one

### 1.3.1 Info and Relationships (Level A) — was FAIL, now **PASS**

Both breaches `00` found are gone. `<label for="@NameInputId">`/`<input id="@NameInputId">` pairing
is explicit in `EventCard.razor:10-11,18-19,26-27` — every label carries a real `for` attribute
matched to a real `id`, unlike the unconnected pair `00` found. The event list is no longer 50
unstructured form-field groups: `EventCard` defaults to read-only presentation
(`ReadOnly="true" AllowEdit="false"` on every list card, `EventList.razor:98-99`), and the match name
renders as plain structured text rather than an `<input>`. A screen reader navigating by heading now
finds real content beneath the page-level heading.

### 1.4.3 Contrast (Minimum) (Level AA) — was FAIL, now **PASS**, independently re-verified

`.validation-message` (`app.css:216-218`) now reads its colour from `var(--color-danger)`, not a
hardcoded `#FF0000`. Light-theme `--color-danger` is `#dc3545` (`app.css:41`) — independently
computed here at **4.53:1** against white, clearing the 4.5:1 floor by the same narrow margin `00`
found the old value missing it by, but on the correct side of the line this time. Every other light
token checked (`--color-focus-ring` 3.37:1 against a 3:1 floor, `--color-action-primary` 5.18:1,
`--color-stale-text` 5.33:1) independently recomputes to match its own code comment exactly.

**Dark theme, independently recomputed against `#000000`, matching every code-comment value
exactly:** text 21.00:1, stale-text 12.04:1, link/focus 10.00:1, action-primary 6.56:1, danger
8.32:1, success 11.05:1, code 8.56:1, danger-surface 12.08:1. **These are the code's corrected
values, not `09_DESIGN-ADDENDUM.md` §4.2's originally-stated ones** (which read 10.94, 9.87, 5.72,
7.21, 9.98, 7.44, 12.63 respectively) — the v12 Completion Report's authorized deviation #2
corrected six of the addendum's own figures to match the WCAG formula's actual output, and every
one of those corrections is confirmed here by independent computation, not merely re-read from the
comment. The nav-item/sidebar-text pair, checked against the gradient's genuinely lighter stop
(`#04173d`, luminance-verified higher than `#22032c`) rather than the darker one, independently
recomputes to **10.61:1** and **15.42:1** — matching the Completion Report's "worst-case stop"
correction exactly, not the addendum's original 11.35 figure computed against the wrong stop.

### 2.4.1 Bypass Blocks (Level A) — was FAIL, now **PASS**

`MainLayout.razor:13` — `<a href="#main-content" class="skip-link">`. Present, where `00` found
nothing. The ~250-focusable-stop problem is independently resolved by the same read-only `EventCard`
default that fixed 1.3.1 above — each list card now carries two links and zero inputs.

### 2.4.7 Focus Visible (Level AA) — was FAIL, now **PASS**

No `outline: none` on `h1:focus` anywhere in `app.css`. Focus rings are token-driven:
`.btn:focus, ... { outline... var(--color-focus-ring) }` (`app.css:196`, values independently
verified above), applied consistently rather than suppressed.

### 3.3.2 Labels or Instructions (Level A) — was FAIL, now **PASS**

Resolved by the same `1.3.1` fix — every `EventCard` field carries an explicit `<label for=...>`.
The date field specifically, `00`'s clearest case (no label, no placeholder, no `aria-label`), now
has `<label for="@DateInputId">Date</label>` (`EventCard.razor:18`).

### 3.3.1 Error Identification (Level A) — was FAIL, now **PASS**

`RequestAccessForm.razor:36,54` — each field error carries `role="alert" aria-live="polite"` and an
`id` (`@NameErrorId`/`@EmailErrorId`) available for `aria-describedby` association, unlike `00`'s
plain `<div class="text-danger small">` with neither.

### 4.1.2 Name, Role, Value (Level A) — was FAIL, now **PASS**

Both breaches resolved. Unnamed controls: closed by `3.3.2` above. The mobile nav toggle now
carries `aria-expanded="@(!collapseNavMenu)" aria-controls="nav-menu"` (`NavMenu.razor:8`) — state
is programmatically exposed, not left to a bare CSS class swap as `00` found.

### 4.1.3 Status Messages (Level AA) — was FAIL, **PARTIAL improvement, one gap remains — see §3**

Form-level errors now use `aria-live="polite"` consistently (`SignIn.razor:107,127`,
`RequestAccessForm.razor:36,54`) — the confirmation gap `00` found on the registration path's
*error* side is closed. The *success* side is not: a written change arriving on `/record` opens and
animates its `ChangeRow` with no `aria-live` region announcing it (confirmed: no `aria-live`
anywhere in `MyAccess.razor` or `ChangeRow.razor`). This is `00`'s original UX-MAJ-05 in narrower
form — recorded as a genuine gap in §3, not folded into the "PASS" list.

---

## 2. `00`'s eight passes — reconfirmed, one criterion newly relevant

All eight hold: DOM order still matches visual order; all primary actions remain native
`<button>`/`<a>`; no modal or focus trap exists anywhere in the current component tree; page titles
are now not just present but *distinct* per screen (`EventDetails.razor`'s `PageHeading` includes
the match number — closing the one gap `00` noted without penalizing); focus order is unchanged;
`lang="en"` is still the document default; neither on-focus nor on-input triggers an unannounced
context change anywhere in the six task-attempts Gate 2 walked.

**One criterion `00` could not meaningfully test now applies with real stakes: 3.1.1 Language of
Page, across three locales rather than one.** `wwwroot/js/locale.js:55` —
`document.documentElement.setAttribute('lang', isLocaleCode(code) ? code : 'en')` — updates the
`lang` attribute on every switch, with the module's own comment stating exactly why: "a page of
Spanish prose still marked `lang="en"` is read out by an English voice." **PASS**, confirmed for
all three locales, and confirmed as a deliberate engineering decision rather than an incidental
default.

---

## 3. New failure, surfaced by this dossier's own task-based pass

### 4.1.3 Status Messages (Level AA) — **FAIL**, narrower than `00`'s original but real

Distinct from the write-confirmation gap in §1 above. `02_TASK-RESULTS.md`'s Attempt 1 finding is,
at its WCAG root, a status-messages failure: `MyAccess.razor`'s `StatusFor` and
`EventDetails.razor`'s `status` field both compute a match's access word from `Change.Kind` alone,
with no check against `Urgency` or `EffectiveUtc`. The resulting text — "Access withdrawn" — is
programmatically identical in markup and ARIA terms whether the underlying state is decided or
merely conditional; nothing distinguishes the two states to assistive technology any more than it
does visually. WCAG 4.1.3 requires a status be *programmatically determinable*; here the two
different statuses (decided vs. conditional) are not even determinable from the rendered word
itself, sighted or not. Scored as its own line because it is a distinct, code-verified defect from
the missing `aria-live` region in §1 — that gap is about *whether* a real status change is announced;
this one is about whether the status *stated* is the right one to announce in the first place.

---

## 4. `11_I18N.md` §7's layout re-verification — Open, narrowed by source-level evidence

`09_DESIGN-ADDENDUM.md` §8 and `11_I18N.md` §7 both flag ES/PT string growth as a layout risk this
dossier cannot resolve from source — §7.4 states plainly it is "a rendered-check task for 4E, not a
value this dossier can compute." That remains true here; **this stays Open, not Pass.** What this
audit can add beyond `00`'s equivalent open items is which pinch point actually carries risk, now
that real translated strings exist to measure:

| Pinch point (`11_I18N.md` §7.2) | Length delta, now measurable | Container constraint | Assessment |
|---|---|---|---|
| Nav "Sign out" row | EN 8 chars → ES **"Cerrar sesión," 13 chars** (+62%) | `nav-item ::deep a { height: 3rem; line-height: 3rem; }` — **fixed height, single-line line-height** (`NavMenu.razor.css:37,40`) | **Highest risk of the set.** A fixed height paired with a matching line-height does not gracefully absorb a wrapped second line; if "Cerrar sesión" wraps at the narrow breakpoint, the second line has nowhere defined to go. REASONED risk, not confirmed — genuinely needs the render §7.4 calls for |
| `ForeseeableBadge` label | EN 55 chars → ES **65 chars** (+18%), already the longest string in the app before translation | `.foreseeable-badge { display: inline-flex; ... }` — **no fixed width, no `white-space: nowrap`** (`ForeseeableBadge.razor.css`) | **Lower risk than `09`'s own flag anticipated.** The container can wrap freely with no clipping; growth here degrades to an extra line, not lost content |
| `AccessCard` status sentence | Not independently measured (sentence is fully dynamic, no static resource string to compare) | No fixed width or `overflow` rule found in `AccessCard.razor.css` | Low risk by the same reasoning `09` §7.2 already gave it — free-wrapping `<p>`, no fixed width |
| `ChangeRow` field labels | N/A — labels sit on their own line above content by design | Not applicable | No risk, per `09`'s own reasoning, confirmed unchanged |

**Recorded as Open, with the nav sign-out row named as the one item worth prioritizing if a render
becomes available** — a meaningfully more useful Open than `00`'s equivalent items could be, since
`00` had no second language to compare string lengths against at all.

---

## 5. The other two items carried Open, unchanged in kind from `00`

| Criterion | Why it stays Open |
|---|---|
| **2.5.8** Target Size (Minimum) | No `min-height`/`min-width` rule found on `.btn`/`.btn-sm` in `app.css` beyond Bootstrap's own defaults — confirming 24×24px compliance needs a rendered measurement, exactly as `00` stated |
| **1.4.10** Reflow | Viewport meta tag present (`index.html:6`) and the one-breakpoint responsive rule exists (`MainLayout.razor.css:26-32`), but confirming no horizontal scroll at 320px across all three locales — now a genuinely harder question than `00` faced, given the longer ES/PT strings — needs a real viewport |

Both are restated rather than silently dropped, per `00_SCOPE.md` §3 item 8's instruction that
nothing gets silently dropped between runs.

---

## Where to start

Mirroring `00`'s own closing structure, in order of impact:

1. **Fix `StatusFor`/`status` to check `Urgency` before rendering a per-match word.** Closes the
   §3 failure and, per `03_HEURISTIC-EVALUATION.md`'s own observation, would very likely move
   Heuristic 1 and Heuristic 4 at the same time — the single highest-value fix available to this
   remediation pass, the same shape of finding `00`'s own "where to start" led with.
2. **Add an `aria-live` region to a newly-arrived `ChangeRow`.** Closes the narrower §1 gap; small,
   isolated, and matches the pattern already used for form errors elsewhere in the same file tree.
3. **Render-check the nav sign-out row in Spanish at the narrow breakpoint**, specifically — not
   the whole nav, just the one row §4 identifies as carrying a fixed-height/single-line-height
   collision risk.
4. **Render-check target size and 320px reflow**, same open items `00` could not close either,
   now with three languages' worth of string lengths to check instead of one.

Sequencing into commit-sized units is `07_REMEDIATION-SCOPE.md`'s job, not this file's — this list
orders by impact, not by what's authorized to fix.

---

✅ **GATE 4 COMPLETE** — `04_ACCESSIBILITY-AUDIT.md`
