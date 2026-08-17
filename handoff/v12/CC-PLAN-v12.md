# Plan — DEPLOY-FifaPressApp-4E-AddendumImplementation-v12_0

> **STATUS, 16 Aug 2026:** this plan was approved and executed through boundaries 1-3 (14 commits,
> 224 tests green). The working clone was then lost when the session scratchpad was wiped, and the
> branch had never been pushed, so that work no longer exists. The `main` branch is unaffected at `ac5555c`.
> Before rebuilding, read `~/.claude/recovery/v12-addendum-implementation-RECOVERY.md` — it carries
> the commit list, the platform findings, and the principal's decisions, so none of it needs
> re-deriving. Push the branch after every commit this time.


## Context

Run 4D produced a four-file design addendum (`09`–`12`), finalized at v11. This run builds it. It is
an implementation run, not a design run: every decision it needs was already made in those files,
and where this prompt and the addendum disagree, the addendum governs.

The work lands as **five gated boundaries** — visual identity, auth and landing, EN/ES/PT,
TypeScript interop, disclosure patterns — each ending in a STOP for approval. Two invariants hold
throughout: the **withholding rule** (an unplayed fixture carries no team names) and the **honesty
rules** (nothing on screen may imply an account system or security that does not exist). This run
puts both at new risk — a second seeded holder, three languages, and new rendering paths are each a
fresh way to leak a withheld name or overstate what the app is.

---

## Task 0 — preflight, all confirmed

| Check | Result |
|---|---|
| `gh` auth | ✅ `jdsaire`, keyring, scopes include `repo`/`workflow` |
| HEAD on `main` | ✅ `ac5555c` — matches `verified_state` exactly, no drift |
| Four addendum files | ✅ all present, all carry `Final — approved and injected at v11, 16 Aug 2026` |
| .NET SDK | ✅ `10.0.201` |
| Baseline build | ✅ `dotnet build src/FifaPressApp -c Release` — 0 warnings, 0 errors |
| Baseline tests | ✅ `dotnet test tests/FifaPressApp.Tests` — **82 passed, 0 failed** |
| TypeScript toolchain | ⚠️ **Not installed** — no node/npm/brew/nvm anywhere. **Installable without sudo**: nodejs.org reachable, arm64, `~/bin` writable. Node LTS tarball extracts to a user-local path; `tsc` via a local `package.json`. No system-level change, no sudo. |

`dotnet test` at the repo root fails with MSB1003 (no solution file); the working invocation is
`dotnet test tests/FifaPressApp.Tests`. Noted so the Completion Report cites the command that works.

### `verified_state` citation re-check

Every citation verified against the live files. **No drift.** Confirmed specifically: both dark
blocks in `app.css` carry identical values (the second, `:root[data-theme="dark"]`, has *no* ratio
comments — updating only one is the defect the prompt warns of); `MainLayout.razor` holds the
`theme-strip` wrapper; `NavMenu.razor`'s comment says Sign In "is a door, not a section, and it
gates nothing"; `MockAccessDataProvider.DemoCredentialId` carries the "There is one holder" comment;
the live route table matches `10` §5.2 exactly (`/`, `/matches`, `/events/{id}`, `/request/{id}`,
`/help`, `/signin`).

---

## The `11_I18N.md` §5.3 discrepancy — confirmed, as instructed

I have read §5.2 and §5.3 in the live file. **§5.2 decides Option B — in-session re-render, no
reload.** §5.3 still reads "regardless of which option 4E picks" and asserts that "switching language
while signed in signs the person out," deriving that explicitly from a reload ("is not restored by a
locale-triggered *reload* either"). With Option B there is no reload, so the premise does not hold.

**I will build the Option B behaviour: switching language preserves the signed-in session.** I will
not implement a sign-out on language change, and I will not edit `11_I18N.md` — it is Final and
frozen to this run, exactly as the four files were at v11. The discrepancy gets its own heading in
the Completion Report so a later addendum can correct the document.

---

## Decisions (task 1 a–h)

### (a) Localization approach — JSON per locale. Forced by verification, not preferred.

**`.resx` + `IStringLocalizer` is impossible without breaking a hard rule.** I checked the resolved
dependency graph (`obj/project.assets.json`, 32 libraries): **`Microsoft.Extensions.Localization` is
not present**. `AddLocalization()`/`IStringLocalizer` would require a new package reference on
`FifaPressApp.csproj` — forbidden by `hard_rules`, `success_criteria` #13 and a `stop_condition`.
Satellite assemblies would additionally want `BlazorWebAssemblyLoadAllGlobalizationData`, another
csproj change.

**Chosen:** `wwwroot/i18n/en.json`, `es.json`, `pt.json`, fetched once at startup via the existing
`HttpClient` and `System.Text.Json` (already in the shared framework — verified in `_framework/`).
Zero new packages, zero csproj change. Serves Option B directly: the dictionaries live in a
singleton, so a locale switch is a dictionary swap plus a re-render, with no I/O and no reload.

### The Portuguese/ICU finding — and why the JSON also owns date formatting

Read verbatim out of the shipped runtime (`dotnet.3x7dwio0xe.js`), the ICU shard heuristic is:

```
culture "en"                                      -> icudt_EFIGS.dat
culture in [fr,fr-FR,it,it-IT,de,de-DE,es,es-ES]  -> icudt_EFIGS.dat
culture in [zh,ko,ja]                             -> icudt_CJK.dat
everything else (including pt, pt-BR, pt-PT)      -> icudt_no_CJK.dat
```

The shard is chosen **once, at boot**. An app booting `en` downloads `icudt_EFIGS.dat`, which
carries English/French/Italian/German/Spanish and **not Portuguese** — so an in-session switch to PT
has no Portuguese ICU data and `CultureInfo`-based date formatting silently degrades. This is a real,
verified blocker for `11` §6 under Option B, and the standard fix is a forbidden csproj change.

**Resolved (principal's decision, all three locales retained):** date formatting is owned by the same
per-locale JSON — month names and the three patterns the app actually uses (`d MMMM yyyy`,
`d MMM, HH:mm`, `d MMMM`). `CultureInfo` stays out of the display path entirely, so the shard
behaviour cannot affect output. This is deterministic, unit-testable in xUnit with no browser, and
immune to future runtime changes.

**This is a deviation from §6's literal mechanism** ("resolve against the active `CultureInfo`") while
delivering §6's actual required outcome (*"3 de julio de 2026"* / *"3 de julho de 2026"*). Recorded as
an authorized deviation in the Completion Report. `FixtureImporter.ParseDate`'s `InvariantCulture`
**parse** is untouched, per §6's own exception.

### (b) Cascading locale, and the cached-string bug

`LocaleService` (singleton) holds the active locale and the loaded dictionaries and raises
`OnChanged`. A root `LocaleProvider` component subscribes and supplies a `CascadingValue<AppLocale>`;
a locale change re-renders the whole tree. Resolution reads through the service at render time.

I audited every component for the specific bug §5.2 names. **The codebase is already largely immune**
— it consistently uses expression-bodied computed properties, which recompute every render:
`AccessCard.StatusSentence`, `StaleIndicator.AgeSentence`, `EventDetails.PageHeading`/`StatusSentence`,
`Registration.PageHeading`, `ThemeTrigger.ButtonLabel`, `MyAccess.StatusWord`/`TrackName` are all safe
by construction. **Exactly three genuinely go stale:**

1. **`MyAccess.matchAccess`** — a `private List<MatchAccessLine>` whose `Label` is baked by
   `LabelFor()` inside `Rebuild()`, called from `OnInitializedAsync`. This is precisely the bug.
   *Fix:* the record carries `MatchNumber` + `Status` only; the label is computed in the render method.
2. **`ForeseeableBadge.Label`** — `[Parameter] public string Label { get; set; } = "…"`, a default
   literal assigned once at construction and never re-evaluated. *Fix:* default `null`, resolve in
   the render method when not supplied.
3. **`StaleIndicator.Subject`** — same shape, and `MyAccess` passes the English literal
   `"your access"`. *Fix:* same treatment, caller passes a key.

### (c) `Change` goes locale-keyed (R5) — without breaking a frozen test

New `LocalizedText(string En, string Es, string Pt)` with a `For(locale)` resolver. `Change` takes
`LocalizedText` for `whatChanged`, `reason`, `nextStep`, `conditionText`.

**Constraint discovered:** `RequestSubmittingStateTests.cs:55` asserts
`Assert.Contains("recorded as requested", written.WhatChanged)` — a **string** assertion. Changing
`WhatChanged`'s type outright would fail to compile and force a test change, which `hard_rules` allow
only under protest. So `Change` keeps `string` accessors that resolve at access time (`WhatChanged =>
WhatChangedText.For(current)`), backed by the locale-keyed values. `11` §4.3 explicitly leaves the
shape to 4E ("or three parallel string properties per field — 4E's implementation choice") and asks
for resolution "at render time", which this satisfies. **The frozen test compiles and passes
unmodified.**

The constructor's existing validation — non-empty `whatChanged`/`reason`/`nextStep`, `reason` must not
restate `whatChanged`, `decidedBy` required when the next step is not actionable, `conditionText`
required when a change depends on a fixture — is applied **per locale**, so a half-translated change
cannot be constructed.

`RequestMatchAccessAsync`/`WithdrawRequestAsync` author their changes in all three languages at write
time from resource templates, so a later locale switch still shows the right language. The provider
therefore takes the locale resources in its constructor (a `Program.cs` registration change, both
singletons — no lifetime mismatch).

### (d) Tomás's record and change list

Per `10` §3.2, every field derived from `05_ARTIFACTS.md` §1.2, no real broadcaster named:

`RH-2026-00219` · Tomás L. · a rights-holding broadcaster (generic) · `RightsHolder`,
`HasNamedContact: true` → `ImmediateOnly` · Approved · valid until 19 Jul 2026 · zones: Media
tribune, Mixed zone, Press conference room, Broadcast position, Camera platform.

Three seeded changes, `ch-006`–`ch-008`, so `NextChangeId()`'s existing counter continues correctly at
`ch-009` with no new logic. The third is **the demonstration** (`10` §3.3):

> **`ch-008`** — `MatchAccessRevoked`, `affectsMatchNumber: 98`, `dependsOnMatchNumber: 93`, with
> `conditionText`. Structurally identical to Amina's `ch-005`. `Classify` returns **Foreseeable**
> (it depends on an unplayed fixture); Tomás's `ImmediateOnly` ceiling then makes it **Silent**, while
> the same shape on Amina's record stays **Foreseeable** and interrupts.

That contrast is produced entirely by the existing `Track.NotificationCeiling` and
`Change.DeriveUrgency` — **no new logic in `Track` or `Change`**, satisfying `success_criteria` #4.

Full EN/ES/PT text for all three changes is drafted and will be presented at **gate 2** before it is
seeded. **Withholding check:** `ch-008` names its dependency as "the Round of 16 fixture in Dallas on
6 July" — round, venue, date, never teams — in all three languages.

### (e) TypeScript scope — and no workflow change

Authored in TypeScript: **the new locale interop** (persisting the choice to `localStorage`, mirroring
`theme.js`'s pattern, and setting `document.documentElement.lang`) **and `theme.js`, converted**
(your decision; `verified_state` names it "the natural first candidate"). Behaviour and mechanism are
preserved per `09` §5.3 and proven by test.

Sources in `src/interop/` with a dev-only `package.json`/`tsconfig.json`, **outside** the app project.
Compiled plain JavaScript is emitted to `src/FifaPressApp/wwwroot/js/` and **committed to the repo**.

**Consequence: no `.github/workflows/` change is required, so none is made.** CI publishes the
committed JavaScript exactly as it publishes `theme.js` today; local `dotnet run` needs no Node. The
§5.3 workflow lift is therefore **not used**, and the Completion Report will say so explicitly, as the
prompt requires.

Everywhere this is described: *Blazor WebAssembly with a small, type-checked JavaScript interop layer
authored in TypeScript.* Never "built in TypeScript."

### (f) The route `10` §8 left open — a named route

**`/record` for the record; `/` reserved for the landing view.** `Landing.razor` takes `@page "/"`;
`MyAccess.razor` moves to `@page "/record"`.

Reasoning, judged against the live router: `/`-with-branching would make one component decide which
*page* it is, tangling the landing's content into the record's error and empty states and against
`09` §3's clarity principle. The named route also fixes the nav's first row honestly — `href=""` with
`Match="NavLinkMatch.All"` exists only because an empty href prefix-matches everything; `href="record"`
uses default prefix matching and gets a correct active state for free. `FocusOnNavigate Selector="h1"`
keeps working since each route owns its own `h1`. The CI SPA shim already encodes arbitrary deep
paths, so `/record` survives a direct hit on GitHub Pages.

### (g) String inventory, reconciled against `11` §2's ~140

| Category | `11` §2 estimate | Counted | Note |
|---|---|---|---|
| A — static UI | ~55 | **~123** | §2.2 enumerated a subset of files; counting every block-level text unit across all `.razor`, Help alone carries 31 and `EventDetails` 22 |
| B — computed/interpolated | (unstated) | ~15 | `StatusSentence` ×5, `AgeSentence` ×7 + wrappers, `ChangeRow`'s effect/recorded lines, `LabelFor` |
| C — enum-derived | ~24 | ~25 | `TrackName` 3, `StatusWord` 4, `PhaseLabel` 7 + "Group", status words 4, `ZoneByCity` 4 distinct |
| D — seeded free-text | 18 fields | ~32 fields | Amina 18 + Tomás ~14, each × 3 locales |
| **New copy (sign-in, landing, nav rows)** | included above | ~25 | `10` §2.3 and §6.2 |
| **Total distinct keys** | **~140** | **~220** | |

The divergence is Category A and is explained: §2.2's ~55 scoped a named subset of files, not the whole
app. §2.6 anticipated exactly this ("re-count against its own final copy rather than reconcile against
this estimate").

### (h) Link-integrity baseline and archive

**Method (one line):** inline `[text](target)` links outside fenced code, excluding external URLs and
bare `#fragment` links, resolved as relative filesystem paths from the containing file.

**Baseline: 299 internal links, 288 resolve, 11 known-inert.** All 11 sit inside frozen `handoff/`
archives and are not live defects: literal placeholder tokens (`(target)`, `(path)`) used by earlier
reports to *describe* link-counting method, and historical `v5/` paths predating the v5 relocation.
Re-measured the same way after the last documentation commit; reported as N/N against this baseline.

**Archive:** `handoff/v12/` — confirmed `v1`–`v11` exist. Convention from `v6`/`v9`/`v10`/`v11`:
`CC-PLAN-v12.md`, `Completion-Report-v12.md`, `README.md`, plus a v12 row in `handoff/README.md`.

---

## Conflicts between the spec and verified constraints

1. **Search vs. translated phase labels — resolved by your decision (extend search).**
   `Fixture.DisplayLabel`/`PhaseLabel` **must stay English on the model**: the frozen withholding test
   asserts `Assert.EndsWith("teams not yet decided", fixture.DisplayLabel)` and the search tests use
   `[InlineData("Round of 16")]`, `[InlineData("Group D")]`. Localization happens in the rendering
   layer only. Per your decision, `FixtureQuery.Search` is **additively** extended to match the active
   locale's label as well as the canonical English one — every input that matched before still
   matches, so all frozen search tests pass unmodified. Recorded as an authorized deviation, since the
   method's own comment calls its field list "a contract."

2. **`09` §4.2's stated contrast ratios are imprecise.** I recomputed every re-derived token against
   the WCAG 2.2 relative-luminance formula. **Every token passes its floor comfortably**, but the
   documented figures are off (they are labelled `[SIMULATED]`, hand-computed):

   | Token | Value | Doc says | Actual | Floor |
   |---|---|---|---|---|
   | `--color-text` | `#ffffff` | 21.00 | **21.00** ✅ | 4.5 |
   | `--color-stale-text` | `#c4c4c4` | 10.94 | **12.04** | 4.5 |
   | `--color-link` / `--color-focus-ring` | `#7ab8f5` | 9.87 | **10.00** | 4.5 / 3 |
   | `--color-danger` | `#ff7a7a` | 7.21 | **8.32** | 4.5 |
   | `--color-success` | `#57d382` | 9.98 | **11.05** | 3 |
   | `--color-code` | `#ec84b8` | 7.44 | **8.56** | 4.5 |
   | `--color-action-primary` | `#4a94dc` vs `#000000` | 5.72 | **6.56** | 4.5 |
   | `--color-danger-surface` | `#6e0f0f` vs `#ffffff` | 12.63 | **12.08** | 4.5 |

   I use the addendum's **hex values verbatim** (the spec governs) and write the **correctly computed**
   ratios in the CSS comments, per task 2's explicit instruction that comments be "accurate to the new
   values." The divergence is reported at gate 1 and in the Completion Report; `11_I18N.md`-style, the
   Final file is not edited.

   §4.3 re-verification: gradient retained; darkest stop is `#22032c`, so the **worst case** is against
   `#04173d`. `--color-sidebar-text` `#f0f0f0` = **15.42:1** (matches doc exactly);
   `--color-nav-item` `#c9c9c9` = **10.61:1** (doc says 11.35). Both pass; both stay unchanged.

---

## Commit sequence — 5 boundaries, 21 commits

Build clean and `dotnet test tests/FifaPressApp.Tests` green after **every** commit, verified
individually. Tests for an item land in that item's own commit.

**Boundary 1 — visual identity** → *gate 1*
1. `feat(theme): re-derive the dark palette against a #000000 anchor` — both dark blocks, accurate ratio comments, light palette and gradient untouched
2. `feat(theme): relocate the theme trigger into the nav list` — delete `theme-strip` wrapper + CSS block; render as a nav row, never `.active`; `theme.js` and the `@code` block untouched

**Boundary 2 — auth and landing** → *gate 2*
3. `feat(auth): add the demo account store and in-memory session provider` — documented as **not** ASP.NET Identity
4. `feat(auth): seed the second demo record and its change list` — Tomás; ceiling contrast; withholding holds
5. `feat(auth): rewrite the sign-in screen around a working simulated session` — v9's `EditForm`/`PermissiveIdentifier`/generic failure retained per §2.2; new notice + published credentials; observable submitting state with its own latency constant
6. `feat(auth): add sign-out and the signed-in indicator to the nav list` — and correct the falsified "gates nothing" comment
7. `feat(auth): add the public landing view and move the record to /record`
8. `feat(auth): gate the record and the request write path` — sign-in prompt, never a hidden control; Matches and Help stay public

**Boundary 3 — EN/ES/PT** → *gate 3*
9. `feat(i18n): add the locale service and per-locale resources`
10. `feat(i18n): make Change's free-text fields locale-keyed` — R5
11. `feat(i18n): translate the static UI surfaces` — product-name rule from §3 honoured
12. `feat(i18n): add the language switch as a nav row` — **session survives the switch**
13. `feat(i18n): format dates and pluralized durations per locale` — importer's invariant parse untouched
14. `feat(i18n): extend match search to the active locale's labels` — additive

**Boundary 4 — TypeScript interop** → *gate 4*
15. `build(interop): add the TypeScript sources and local compile step`
16. `build(interop): author the theme and locale interop in TypeScript`

**Boundary 5 — disclosure patterns** → *gate 5*
17. `feat(help): give change rows a two-layer disclosure` — collapsed layer stays fully informative; entrance treatment for a newly-arrived change
18. `feat(help): make Help sections independently collapsible` — all collapsed by default, fully static, **no URL-fragment auto-expand**

**Documentation and archive**
19. `docs: update the root README, how-to-run, and folder READMEs`
20. `docs: add the learning-mode chapter for this run` — written after the code, per repo convention
21. `docs: archive v12 addendum implementation plan and completion report`

## Test inventory per boundary

- **B1** — palette present in *both* dark blocks; trigger renders inside the nav list; no theme strip in markup or CSS.
- **B2** — both records resolve; **a change Silent for Tomás is Foreseeable for Amina**; gated routes redirect signed out; Matches and Help public; withholding holds for Tomás's record; submitting state genuinely yields.
- **B3** — every surface renders in all three locales; **no stale string after a switch** (targeting the three audited offenders); **session survives a switch**; dates format per locale; ES/PT pluralization correct; **withholding holds in all three languages**; search still matches every previously-matching input.
- **B4** — compiled output behaves identically to what it replaces; `tsc` type-checks clean.
- **B5** — collapsed rows still state that something changed; expansion reveals reason and next step; Help readable fully collapsed; no section depends on a fetch.

## Proposed beyond what the prompt explicitly authorizes

1. **Localizing `Change.DecidedBy`.** R5 names four fields; `DecidedBy` is a fifth, and it is
   user-visible (`ChangeRow` renders "Decided by …"). `success_criteria` #7 requires every user-visible
   string to render in all three locales, so leaving it English would fail #7. Same `LocalizedText`
   treatment, same `string` accessor, so the frozen `Assert.Contains("simulated", written.DecidedBy)`
   still passes.
2. **Installing Node locally** (user-writable path, no sudo, nothing system-wide) to obtain `tsc`.
3. **Committing the compiled interop JavaScript** as a tracked artifact, which is what avoids a
   workflow change entirely.
4. **Writing correct contrast ratios** into the CSS comments where they diverge from `09` §4.2's
   stated figures — required by task 2's "accurate to the new values", reported as a divergence.

## Verification (task 14)

- `dotnet build src/FifaPressApp -c Release` clean, after each commit individually.
- `dotnet test tests/FifaPressApp.Tests` green; final count vs. the **82** baseline.
- `git diff ac5555c..HEAD` for `src/FifaPressApp/FifaPressApp.csproj`, `ux-ui/`, `wwwroot/lib/` — all empty.
- `.github/workflows/deploy-pages.yml` — **unchanged**, shown as an empty diff.
- Withholding: the whole-schedule test passes **unmodified**, for both records, in all three locales.
- Search: every frozen search test passes unmodified.
- No read path gained latency (`GetAccreditationAsync`/`GetChangesAsync` still return completed tasks).
- Zero AI product names anywhere in diff, messages, branch, PR; zero "built in TypeScript" phrasing.
- Links N/N against the 288/299 baseline.
- All commits on `deploy/v12-addendum-implementation`, authored `jdsaire`, PR opened against `main` and **left unmerged**.
