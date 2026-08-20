# Iteration & Realignment Dossier — Run 05

*(The first dossier in this repository whose subject is a second interface rather than this one.
`00`–`04` measured, researched, specified and re-evaluated the FIFA Press App on its own terms; this
one reads a separate reference app — ShopEase, a course-mate's e-commerce build — and asks which of
its patterns this app should adopt, which it should refuse, and what each adoption costs in
decisions this codebase has already written down.)*

A five-gate design iteration, approved gate by gate, that reshapes navigation, settings, the front
door, sign-in, and the match list against the ShopEase register — and records, as a reversal
register, every place where doing so overturns a decision the codebase argues for in its own
comments.

The dossier's governing constraint is that a documented decision is never overturned silently. Six
conflicts were found; each was opened as a numbered register entry, argued at the gate that owns it,
and closed with the reasoning that resolved it. **No entry was closed by preference alone**, and one
(**R11**) turned out to rest on a premise that was factually impossible against the app's own
simulated clock.

---

## The register

| # | Conflict | Resolution | Closed at |
|---|---|---|---|
| R6 | The sign-in notice framed as removable "useless text" | Condensed to two keys; all five test-asserted substrings preserved verbatim | Gate 3 |
| R7 | `LanguageSwitch`: three buttons → a dropdown | Rebuilt as a `<select>` inside Settings; the original three-buttons reasoning re-applied intact to Appearance | Gate 2 |
| R8 | Demo account identifier rename | `demo_staff1`/`demo_staff2`; `CredentialId` and everything it keys provably untouched | Gate 3 |
| R9 | Sidebar session indicator vs. a persistent top bar | Relocated to the top bar; original reasoning upheld, original placement superseded because it fails at the collapsed breakpoint | Gate 1 |
| R10 | `/record` redirects signed-out visitors to `/signin` | Renders `SignInForm` inline instead; `/signin` retired outright | Gate 4 |
| R11 | Group-phase capacity premise | Factually impossible — every group fixture is already played at `SimulatedNow`; rescoped to the 16 unplayed knockout fixtures | Gate 4 |

All six resolved. Zero open conflicts.

---

## The files

Read in gate order.

| # | File | What it covers |
|---|---|---|
| 0 | [`00_RECONCILIATION.md`](00_RECONCILIATION.md) | Sync confirmation, the routing correction the annotations got wrong, and the four conflicts that open the register (R6–R9) |
| 1 | [`01_NAVIGATION.md`](01_NAVIGATION.md) | The persistent top bar, the sidebar reduced to destinations only, the deliberate absence of a cart-badge analogue, and five new icon glyphs. Closes **R9** |
| 2 | [`02_SETTINGS.md`](02_SETTINGS.md) | The Settings screen: a language dropdown, a tri-state Appearance control built on an already-exported, previously-unused JS function, and what was deliberately left out. Closes **R7** |
| 3 | [`03_HOME-AND-SIGNIN.md`](03_HOME-AND-SIGNIN.md) | Landing reduced to heading/lede/one CTA, the simulation notice condensed without losing a tested word, and the demo identifier rename with its ~30-call-site cost accepted knowingly. Closes **R6** and **R8**, opens **R10** |
| 4 | [`04_MATCHES.md`](04_MATCHES.md) | Rule-based capacity, three-way CTA gating, `EventCard`→`MatchCard`, team-name localization, Show-more pagination, an availability filter, and the conditional `/record`. Closes **R10**; opens and closes **R11** |
| 5 | [`05_MANIFEST.md`](05_MANIFEST.md) | The consolidated three-locale i18n table (30 retired, 72 new, 5 value-changed), the full file manifest, all 30 existing test files classified, and the commit sequence |

---

## What this dossier authorizes

The code changes in this repository's v14 run, and nothing beyond them. Every design decision the
run implements is traceable to a gate above; where a gate marks a value provisional — icon path
geometry, and every Spanish and Portuguese translation — it stays provisional in the implementation
too, tagged rather than quietly promoted to reviewed.

**Untouched by this dossier and by the run it authorizes:** `00-initial-evaluation/`,
`01-design-research/`, `02-ideation/`, `03-ui-prototyping/`. Nothing here revisits a finding those
folders closed.

**Reference build:** `main` @ `e996d8a`.
