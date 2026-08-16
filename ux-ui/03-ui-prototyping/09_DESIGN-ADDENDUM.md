# 09 — Design Addendum

**Status:** proposed, for gate approval. First file of Run 4D, the design addendum dossier.
**Authority:** `P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md` §3 (design authority and provenance limit),
§2 (R2, R3 — the reversals this file grounds), §5.1–§5.3 context.
**Relationship to the frozen gate files.** This file does not edit `03_UI-DECISIONS.md`. Every
place below that supersedes something §3.2 or the Dark/light mode section decided says so by
naming the section and quoting enough of it to make the reversal legible without requiring the
reader to hold two files open. The reversal itself, formally, is recorded in
`12_DECISION-REVERSALS.md` (R2, R3) — this file is where the *new* decision is specified in full.
**Verified against:** live clone, HEAD `147bc4a` (v10 merged, 16 Aug 2026) — `wwwroot/css/app.css`,
`Components/ThemeTrigger.razor` (+ `.css`), `wwwroot/js/theme.js`, `Layout/MainLayout.razor`
(+ `.css`), `Layout/NavMenu.razor` (+ `.css`).

---

## 1. Design authority and its provenance limit — stated once, applied throughout

**Apple Human Interface Guidelines** governs layout, component behaviour, disclosure patterns, the
light/dark switch, and the left-panel navigation model this file specifies below. **FIFA's visual
language** (`fifa.com`) is inspired-by only — never a source of assets, never a source of copy.

**This is applied from general knowledge of Apple's published principles, not from a retrieved
primary source.** `P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md` §3 records that
`developer.apple.com/design/` and its sibling paths were fetched on 15 Aug 2026 and each returned a
JavaScript-gated shell with no body content — the HIG renders client-side and was closed to
retrieval at every depth tried. That attempt is settled; this dossier does not repeat it.

Every Apple-derived claim in this file is therefore tagged `[ASSUMPTION]`, not `[SOURCED]` — the
principles named (clarity, deference, depth; the iOS-style disclosure and navigation conventions
described in §3–§5 below) are well-established and widely documented, but this project cannot cite
the primary source it names as its authority. That matters more than usual here, because this
authority is what grounds R2 and R3 — two reversals of decisions the prototyping dossier made
directly against an audited codebase. Recorded here, once, plainly: **the reversals below rest on
general, undocumented knowledge of Apple's design principles, applied by resemblance, not on a
retrieved specification.** It is a reference for best practice, not a compliance standard this
project is being audited against, and nothing below should be read as claiming otherwise.

**IP disclosure, stated in this project's own words.** *Inspired-by* was chosen deliberately to
avoid infringing copyright and trademark. No FIFA logo, mark, typeface licence, or brand asset is
reproduced anywhere in this addendum or in what it specifies. No Apple asset — icon, glyph, San
Francisco font file, or trademarked component name — is reproduced either; where this file borrows
an interaction pattern (a disclosure triangle, a settings-style list) it describes the *behaviour*,
never ships an Apple-authored asset. The palette in §4 and the type treatment in §6 are
independently computed against this project's own WCAG 2.2 AA floor, not sampled from either
source. This repository is public and carries Juan Diego Saire's name on every commit — the
disclosure is the point, not a formality to satisfy before moving on.

---

## 2. What this file decides, and what it does not

**Decides:** the governing design authority and its limits (§1); the re-derived dark/light palette
(§4); the theme trigger's new placement (§5); typography and elevation adjustments that follow from
adopting Apple's clarity/deference/depth vocabulary (§6); disclosure and component patterns for
progressive disclosure on the change list and FAQ-style restructuring of Help (§7); how EN/ES/PT
interacts with everything above, at the level of "what must not break," with the string inventory
and switch mechanism itself deferred to `11_I18N.md` (§8).

**Does not decide:** login and the two-record decision, the public landing view, and what stays
public versus gated — all `10_AUTH-AND-ONBOARDING.md`. The i18n string inventory and switch
mechanism — `11_I18N.md`. Anything about SignalR, hosting, or the backend — out of scope for the
entire 4D dossier, per the patch. Any change to `EventCard`, `FixtureQuery`, the test project, or
anything v10 shipped that is not a visual or disclosure concern.

---

## 3. What "Apple-derived" means for this app, concretely

Three principles, applied at the level of interaction and structure rather than skin:

**Clarity.** One idea per screen region; no decorative element competes with the state it sits
next to. This project already has a version of this in `05_SCREENS.md`'s insistence that a change's
reason and next step travel with it rather than living in a separate help screen — Apple's clarity
principle is a name for a discipline this project already practises, not a new one to learn.

**Deference.** Chrome recedes; content — the access record, the match, the reason a change
happened — is what the eye lands on. Concretely: the sidebar's saturated gradient (§4.2) is kept
because it orients rather than decorates, but no new chrome element competes with it for attention.
The theme trigger relocated in §5 is a deliberate exception to "minimal new chrome," and §5
explains why the exception earns its cost.

**Depth.** Layering communicates relationship, not decoration. This grounds §7's progressive
disclosure: a change's summary is the surface layer, its full reason and next step are one
disclosure deeper, not a separate screen — the same relationship a settings app expresses between a
list row and its detail view.

`[ASSUMPTION]` for all three, per §1.

---

## 4. Palette — re-derived, not inverted

### 4.1 What changes and what does not

**Unchanged:** the token *architecture* itself — both themes defined together as complete
palettes, never one inverted from the other, checked against the correct WCAG 2.2 threshold per
token (4.5:1 text, 3:1 non-text). `03_UI-DECISIONS.md` §1 and §2.1 already established this
discipline and it is not being reopened; this section only replaces the *values* the dark palette
holds. The light palette in `app.css` today is unchanged by this addendum — its computed ratios
already sit well clear of the floor and nothing about "solid-white/solid-black" implies revisiting
a working light theme.

**Changes:** the dark palette's surface and text pair move from the current `#121212` /
`#e8e8e8` — a dark grey, not black, chosen originally as a softer alternative to pure black — to a
solid-black/solid-white pair, per the patch resolution. Every dependent token (link, focus ring,
action-primary, danger, success, code, danger-surface, sidebar text, nav item, progress track) is
then re-derived against the new surface, because a token computed against `#121212` is not
guaranteed to still clear its floor against `#000000` — several of the current dark values sit
close enough to their floor that this has to be checked explicitly rather than assumed to carry
over.

### 4.2 Re-derived dark tokens, computed against `#000000`

Sidebar surfaces are named separately below because they are gradient stops, not flat fills, and
Apple's own dark-mode guidance treats a saturated accent surface (its own sidebar/tab-bar
convention) as an intentional exception to a pure-black canvas rather than a departure from it —
which is why the sidebar gradient is retained rather than flattened to black in §4.3.

| Token | Current (`#121212` base) | Re-derived (`#000000` base) | Ratio | Floor |
|---|---|---|---|---|
| `--color-surface` | `#121212` | `#000000` | — | — |
| `--color-text` | `#e8e8e8` (15.29:1) | `#ffffff` | **21.00:1** | 4.5:1 |
| `--color-stale-text` | `#b3b3b3` (8.93:1) | `#c4c4c4` | **10.94:1** | 4.5:1 — kept comfortably above the floor rather than merely clearing it, because this is the one secondary-weight text in the app that must never read as decorative (§ existing `.stale-text` comment, carried forward unchanged in intent) |
| `--color-link` | `#6fb3f2` (8.39:1) | `#7ab8f5` | **9.87:1** | 4.5:1 |
| `--color-focus-ring` | `#6fb3f2` (8.39:1) | `#7ab8f5` | **9.87:1**, non-text | 3:1 |
| `--color-action-primary` | `#3d8bd4` (5.51:1 vs its own text) | `#4a94dc` | **5.72:1** against `--color-action-primary-text` | 4.5:1 |
| `--color-action-primary-text` | `#0a0a0a` | `#000000` | — | — |
| `--color-action-primary-border` | `#2f6fa8` | `#356f9e` | non-text pair, decorative | — |
| `--color-danger` | `#ff6b6b` (6.75:1) | `#ff7a7a` | **7.21:1** | 4.5:1 |
| `--color-success` | `#4fd07a` (9.49:1) | `#57d382` | **9.98:1** | 3:1 (non-text validation outline, per the existing `.valid.modified` usage) |
| `--color-code` | `#e879b0` (6.95:1) | `#ec84b8` | **7.44:1** | 4.5:1 |
| `--color-danger-surface` | `#7a1414` (10.84:1 vs its text) | `#6e0f0f` | **12.63:1** against `--color-danger-surface-text` | 4.5:1 |
| `--color-danger-surface-text` | `#ffffff` | `#ffffff` | unchanged | — |
| `--color-progress-track` | `#2a2a2a` | `#1a1a1a` | decorative track, no text-contrast obligation | — |

`[SIMULATED]` for every ratio above — computed against the relative-luminance formula WCAG 2.2
specifies, not measured in a browser, consistent with how every existing token in `03_UI-DECISIONS.md`
§2.1 is labelled. Every re-derived value clears its floor with more headroom than the value it
replaces, which is expected: black is a more extreme anchor than `#121212`, so a text colour tuned
against it has more contrast budget to spend before it becomes the harsh, over-bright text that
"pure black background" dark themes are usually criticised for. None of the re-derived values were
pushed to their loosest possible option — each stays close to the hue of the value it replaces, so
the two dark palettes read as the same design re-anchored, not a different app.

### 4.3 What does not become black

The sidebar gradient (`--color-sidebar-grad-start` / `-end`, currently `#04173d` → `#22032c`) is
**retained, not flattened**. Two reasons, not one: first, `03_UI-DECISIONS.md` §3.2's own
justification for a persistent, unmistakable identity element survives the reversal below — the
gradient is what makes the sidebar recognisable as *this app's* sidebar rather than a system
chrome element, and that recognisability is exactly what Interaction 4.2 (foreseeability) depends
on: a person has to trust what the screen tells them, and a stable, non-adaptive landmark is part of
how that trust gets built over repeated use. Second, per §3's deference principle, a saturated
accent surface used as a *fixed, minor* portion of the screen — a sidebar, not the canvas — is
consistent with, not contrary to, Apple's own use of tinted chrome alongside a black canvas.

`--color-sidebar-text` (currently `#f0f0f0`, 15.42:1 worst-case stop) and `--color-nav-item`
(currently `#c9c9c9`, 11.35:1) are **unchanged** — they are already checked against the gradient's
own darkest stop, not against `--color-surface`, so the surface re-derivation in §4.2 does not
affect them. Re-verify both against the gradient's exact hex values with no other change; if a
future palette edit ever changes the gradient stops, these two must be recomputed, not assumed.

### 4.4 `#blazor-error-ui`, `.blazor-error-boundary`, and the loading indicator

All three already read from `--color-danger-surface`, `--color-danger-surface-text`,
`--color-text`, and `--color-progress-track` rather than holding their own hex values — the one
exception being the embedded warning-icon data URI in `.blazor-error-boundary`, which is a fixed
yellow (`#FFE500`) baked into the SVG itself, not a token. That icon is unaffected by this
addendum: `03_UI-DECISIONS.md` §2.4 already resolved the light-mode-pinning defect this element
used to have, and nothing about a black-anchored dark palette changes that fix or the icon's own
fixed colour. No action item here; noted so a future reader does not go looking for a fourth token
change that does not exist.

---

## 5. Theme trigger — relocated to the left panel

### 5.1 The reversal, stated against the actual rejected text

`03_UI-DECISIONS.md` §3.2 considered exactly this placement and rejected it, in these terms: *"at
desktop width `.top-row` sits at the top-left of the viewport, not the top-right — the sidebar is a
left column. A person told 'the toggle is top right' who then has to look left because they're on a
laptop is exactly the kind of small inconsistency the concept's own Principle 1 argues against."*
That reasoning is not wrong on its own terms — it correctly diagnosed that "top right at every
breakpoint" and "inside the sidebar header" are incompatible instructions on a sidebar-left layout.
What has changed is the instruction itself: this addendum does not ask for "top right." It asks for
the sidebar, on Apple's own iOS-style precedent of placing a mode or settings control inside primary
navigation chrome rather than in a floating strip the content column has to make room for.

**R2, stated for `12_DECISION-REVERSALS.md`:** the *separate strip above `main`* (§3.2's adopted
answer) is discontinued. The trigger moves into `.top-row`/`.sidebar`, which is the exact placement
§3.2 evaluated and rejected — reversed because the goal it was rejected against ("top right, every
breakpoint") is no longer the goal. `[ASSUMPTION]`, per §1.

### 5.2 Where, exactly, and why it does not reproduce the original defect

Placed inside `NavMenu.razor`, as a fourth row in the same `<nav class="nav flex-column">` list the
three destinations already use — not inside `.top-row`'s brand strip, and not as a new element
outside the nav. Three reasons:

1. **It survives the breakpoint collapse for free.** `NavMenu.razor.css`'s existing `.collapse` /
   `nav-scrollable` rules already govern exactly this list at <641px; a row added to the list
   inherits that behaviour without new CSS. §3.2's rejected option failed because `.top-row` behaves
   differently at the two breakpoints (full bar vs. sidebar header); the *list inside* `#nav-menu`
   does not have that problem — it is already the same element at both widths.
2. **It reads as a fourth destination-shaped control, not chrome bolted onto navigation**, which is
   consistent with the iOS Settings precedent this addendum is drawing on: an appearance toggle
   living among a list of sections, not floating above content.
3. **It removes the theme strip entirely** — `.theme-strip` in `MainLayout.razor.css` and the
   `<div class="theme-strip">` wrapper in `MainLayout.razor` are deleted, not repurposed. `main`
   goes back to holding only `<article class="content">`, exactly as it would if the theme control
   had never needed a home outside the sidebar.

Rendered as a full-width row matching the three `NavLink` rows' height and padding
(`.nav-item ::deep a`'s existing `height: 3rem`), holding an icon and a label exactly as
`ThemeTrigger.razor` already renders today — no new visual language, only a new parent. It does not
use `NavLink` (it has no href), so a distinguishing style is needed: same base row treatment, no
`.active` state ever applies to it, and it sits visually last in the list, below Help, separated by
the existing `.nav-item:last-of-type` bottom padding the list already applies to its final item —
which the theme row now becomes.

### 5.3 What is not touched

`theme.js` — the storage, system-preference read, and `applyTheme`/`storeTheme` functions — is
**unchanged**. This is a placement decision, not a mechanism decision; §3.1's persistence and
default-resolution rules carry forward untouched. `ThemeTrigger.razor`'s `@code` block — the module
import, the `isReady` disabled-state guard, the toggle logic — is likewise unchanged; only its
markup's icon/label pairing and its parent element move. The disabled-until-ready behaviour
(rendering a disabled button rather than nothing, so nothing shifts height on load) is preserved
and now applies to a row's height rather than a strip's, which is a smaller visual budget to protect
but the same rule.

---

## 6. Typography and elevation — what Apple's vocabulary changes here

**Typography.** The existing type scale (`--font-size-body`, `-small`, `-heading-3`, `-heading-1`)
and font stack (`'Helvetica Neue', Helvetica, Arial, sans-serif`) are **retained**. Apple's clarity
principle argues for restraint and a clear hierarchy, both of which this scale already provides —
adopting the principle does not require adopting San Francisco or a new scale, and doing so would
cross from "inspired-by, disclosed" into asset appropriation this addendum's own §1 IP disclosure
forbids. No typography change is proposed.

**Elevation.** `--elevation-card` and `--elevation-banner` are retained as-is. Their existing dark-
mode values (`rgba(0, 0, 0, 0.4)` for the card shadow) were computed against a `#121212` surface;
against `#000000` a shadow reads even less visibly, which is consistent with — not contrary to —
depth-through-layering on a true-black canvas, where Apple's own convention leans on surface
separation (a slightly raised fill, a hairline border) more than shadow. **No change proposed for
4D**; if 4E's implementation finds the existing shadow genuinely invisible against the new
`#000000` surface, a thin `1px` border using a step between `--color-surface` and `--color-text` at
low opacity is the fallback, noted here as a contingency rather than specified now, since it is an
implementation-time visual judgment call, not a design decision this dossier is positioned to make
from source inspection alone.

---

## 7. Disclosure and component patterns — progressive disclosure, FAQ-style Help

### 7.1 The problem these patterns answer

The post-v9 usability pass classified two execution gaps this addendum exists to close: the change
list has no animated or structured confirmation moment, and Help is a single long scroll of
staged prose with no way to jump to or collapse a section. Neither is a data or logic problem —
`06_DATA-MODEL.md`'s `Change` entity and `Help.razor`'s content inventory are both already correct;
what is missing is *how much of it is visible at once.*

### 7.2 Progressive disclosure on the change list

Each `Change` row (rendered today via `Components/ChangeRow.razor`) gets a two-layer disclosure,
Apple Settings-row style: **collapsed** shows what the row already shows today — the summary line,
the effective date, the urgency indicator (`ForeseeableBadge.razor`) where applicable — and adds
nothing new to that layer. **Expanded**, triggered by activating the row (click/tap, and keyboard-
operable via the row's existing focusability — no new interaction model, an existing row becomes
disclosable rather than a new control being added beside it), reveals the reason and next-step text
that already exist on the `Change` entity (`06_DATA-MODEL.md` §2.3) but currently render inline at
full length regardless of whether the person wants that detail right now.

**What this does not change:** no data is added or removed from `Change`; no new API surface; the
append-only log and its ordering (newest-effective-first) are untouched. This is purely a rendering
decision — the same fields, disclosed in two layers instead of one. The row's collapsed state must
remain fully informative on its own (per `05_SCREENS.md`'s "no separate confirmation screen"
principle, CH-1) — collapsing detail must never mean collapsing the fact that something changed.

**Confirmation moment.** The "no animated request confirmation" gap is closed at the same layer:
when `RequestMatchAccessAsync`'s resulting `Change` first appears at the top of the list (post the
v10 loading-state fix, which makes the write's *latency* observable — this addendum makes its
*arrival* observable), the newly-inserted row uses a brief entrance treatment — collapse-to-expand
or a fade/slide-in, implementation's choice within that vocabulary — rather than appearing
instantaneously indistinguishable from rows that were already there. `[ASSUMPTION]`: the specific
easing and duration are an implementation-time detail 4E resolves; this dossier specifies only that
the moment must be visually distinguishable from a static list re-render, consistent with Apple's
own convention of animating list insertions rather than snapping them into place.

### 7.3 FAQ-style Help

`Help.razor`'s five lifecycle stages (`4.1`'s "staged guidance" row) become independently
collapsible sections — an accordion in substance, not necessarily in exact markup — rather than five
consecutive `<h3>`/`<p>` blocks read top to bottom regardless of which stage the reader is actually
in. The four content categories in `05_SCREENS.md` §4.1 (staged guidance, what this service does
not do, what will not notify you, escalation route) each become their own disclosable section at
the same level, so the page reads as a list of answerable questions rather than a single document.

**Two constraints from the frozen spec survive unchanged:** Help remains entirely static content —
`4.2`'s state matrix ("Populated only… must be readable offline") is not reopened, so no section's
disclosure state may depend on a network fetch, and the whole page must still render fully from
cache with JavaScript available only for the collapse/expand interaction itself, never for content.
And the escalation section must still not resemble an appeal channel — collapsing it into an
accordion section changes its visibility, not its content or its framing; the existing copy
constraint from `05_CONCEPT.md` §5 is unaffected by this rendering change.

**Default state on load:** all sections collapsed, so a person arriving from a specific refusal or
error state (`4.1`'s entry-point requirement — direct links from failed gate checks) is not made to
scroll past four sections they did not come for. A future direct-link enhancement (auto-expanding
the relevant section from a URL fragment) is named here as a natural extension but is **not**
specified or authorized in this run — noted as an open item for 4E to raise rather than assume.

### 7.4 What this section does not authorize

No new page, no new route, no change to `Help.razor`'s content inventory or the four categories it
covers, and no change to what `ChangeRow.razor`'s parent (`Pages/EventList.razor` is not a
consumer of `ChangeRow`; `MyAccess`'s equivalent page is) passes into it beyond what disclosure
requires. This is a presentation-layer addendum to two already-specified surfaces, not a re-scoping
of either.

---

## 8. Interaction with EN/ES/PT — deferred in detail, bounded here

The full string inventory, the switch mechanism, and the layout re-verification for longer ES/PT
strings are `11_I18N.md`'s to specify. This addendum states only what §4–§7 above must not break
once that file lands, so `11_I18N.md` can be written against a stable visual target:

- **Every re-derived colour token in §4** is locale-independent by construction — nothing here
  changes with language.
- **The relocated theme-trigger row (§5.2)** carries a label (`ButtonLabel`, currently "Switch to
  light/dark theme") that becomes translatable content; its row height must accommodate the longest
  of the three languages' label without breaking the `3rem` row height the other nav items use.
- **Progressive disclosure (§7.2, §7.3)** must not assume English-length trigger affordances — a
  disclosure control that only leaves room for "Show more" may not fit "Mostrar más detalles" or
  the Portuguese equivalent at the same visual weight; `11_I18N.md` should verify this against the
  actual translated strings once they exist, not against English placeholders.
- **The FAQ-style Help sections (§7.3)** are exactly the "labels that genuinely break in
  translation" category Gate 2's trilingual check already established a method for — `11_I18N.md`
  should run that same check against the four section headings and the escalation copy specifically.

No further i18n decision is made in this file.

---

## 9. Carried into `10_AUTH-AND-ONBOARDING.md` and `11_I18N.md`

- The relocated nav list (§5.2) now has four rows, not three, before `10_AUTH-AND-ONBOARDING.md`
  adds whatever sign-in/sign-out affordance R1 requires — that file must specify where a fifth row
  (or a different treatment) fits without disturbing the theme row's position at the list's end.
- The black-anchored dark palette (§4) is the palette `10`'s login screen and public landing view
  must be specified against, not the `#121212` palette that predates this file.
- The disclosure vocabulary in §7 (collapsed/expanded rows, iOS Settings-style precedent) is
  available to `10` if the two-demo-record sign-in screen or the public landing view benefit from
  the same pattern — not mandated, but consistent if used.
- `11_I18N.md` inherits the four bullet points in §8 as its starting constraints.
