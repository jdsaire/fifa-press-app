# v14 (Run 4F) — Gate 2: Settings Screen

**Status:** Open for approval. Resolves **R7**. Four calls resolved with the principal before
writing this gate — recorded in §0, not re-opened here.

---

## 0. Resolved before drafting

| Call | Resolution |
|---|---|
| Settings "Role" field | **Dropped.** `TrackId` stays a derived precondition, never a Settings field. `Track.cs`'s "deliberately no way to assign this" doctrine is upheld, not reversed — this is a non-conflict, not a new register entry. |
| Settings visibility | **Public.** Language + Appearance visible signed out; Name appears once signed in. |
| Settings "Name" field | **Read-only** display of the signed-in account's `HolderName`. No edit, no persistence question to dodge. |
| Calendar connectors | **Excluded** from v14 scope entirely. Not specified below. |

Required-fields list from the source annotation is revised accordingly: **Appearance**, **Language**
always; **Name** (read-only) when signed in. Role and Log-out-of-all-devices are both gone — the
former by the resolution above, the latter already closed in Gate 0.

---

## 1. R7 — resolved: `LanguageSwitch` becomes a dropdown

**The conflict**, restated from Gate 0: `LanguageSwitch.razor`'s comment argues three buttons over a
picker because "three is not [a list too long to show]." That reasoning is sound for a persistent
sidebar row and does not, on its own, justify a dropdown.

**What changes the calculus:** the control is leaving the sidebar. In its new context — a Settings
page visited deliberately, sitting beside `<select>`-shaped Appearance options — a labelled dropdown
matches the register of its neighbors, where three loose buttons would be the outlier. This is the
same class of argument Gate 1 used for R9: the original reasoning is not wrong, the *context* it
reasoned about has moved.

> **R7 — CLOSED.** `LanguageSwitch` is retired from `NavMenu` (already actioned in Gate 1's nav
> restructure) and rebuilt as a `<select>` inside Settings. The button-group markup, the
> `aria-pressed` mechanism, and the component itself are all replaced — not relocated as-is.

**What must survive the rebuild**, because these are behavioral guarantees `LanguageSwitchTests`
protects, not incidental markup:

- **No reload, no navigation, no storage write.** Switching is `L.Set(locale)` and nothing else —
  `ASwitchDoesNotNavigate` and the module comment's "THE SESSION SURVIVES THIS" both hold unchanged.
  A `<select @bind="..." @bind:event="onchange">` calling the same `L.Set(locale)` preserves this
  exactly; the mechanism doesn't care what widget calls it.
- **The whole tree re-renders in the new language**, immediately, without leaving Settings.
- **The session survives** — `Session.Current`, `CredentialId`, and the record's holder name are
  untouched by a language switch, exactly as today.

**New markup**, `Components/LanguageSelect.razor` (renamed — `LanguageSwitch` implied a
button-toggle mechanic that no longer applies):

```razor
<div class="settings-field">
    <label for="settings-language">@L[Locale, "nav.language"]</label>
    <select id="settings-language" class="form-select"
            value="@LocaleService.CodeOf(Locale)"
            @onchange="OnChanged">
        @foreach (var option in LocaleService.All)
        {
            <option value="@LocaleService.CodeOf(option)">@L.NameOf(option)</option>
        }
    </select>
</div>
```

`role="group"` and `aria-pressed` are dropped — they were specific to the button-group shape and a
native `<select>` carries its own correct semantics without them.

---

## 2. Appearance — tri-state, new mechanism (flagged, not a reversal, in Gate 0 §"non-conflict")

**Mechanism, verified against `wwwroot/js/theme.js`:** "System" is not a third stored value —
`isTheme()` narrows to `'light' | 'dark'` only, and `getStoredTheme()` returning `null` is what
*means* "no explicit choice, defer to `prefers-color-scheme`." The tri-state control therefore maps
onto the existing module exactly, with one addition:

| UI option | Action | Existing module call |
|---|---|---|
| System | clear the stored choice | `clearStoredTheme()` — **already exported, currently unused by any caller** |
| Light | store `'light'`, apply it | `storeTheme('light')` + `applyTheme('light')` — unchanged |
| Dark | store `'dark'`, apply it | `storeTheme('dark')` + `applyTheme('dark')` — unchanged |

**No `theme.js` changes required.** `clearStoredTheme()` exists and is fully correct for this
purpose; it has simply had no caller until now. This is the cleanest possible outcome of the
tri-state requirement — new UI, zero new JS.

**New markup**, `Components/AppearanceControl.razor` (replaces `ThemeTrigger.razor` — the old name
implied a toggle, and a tri-state selector is not one):

```razor
<div class="settings-field" role="radiogroup" aria-label="@L[Locale, "settings.appearance"]">
    <span class="settings-field__label">@L[Locale, "settings.appearance"]</span>
    <div class="settings-field__options">
        @foreach (var (value, key) in Options)
        {
            <button type="button"
                    class="settings-field__option @(value == current ? "settings-field__option--current" : null)"
                    aria-pressed="@(value == current ? "true" : "false")"
                    disabled="@(!isReady)"
                    @onclick="() => SetAsync(value)">
                @L[Locale, key]
            </button>
        }
    </div>
</div>
```

Three-button-group form is **kept** here, deliberately — this is the one place in the dossier where
the *original* `LanguageSwitch` reasoning ("three is not [too long to show]") applies on its own
terms to a *different* control, since Appearance has exactly three fixed options too. Not a
reversal; the same doctrine, correctly applied to the component it actually fits.

`isReady`/disabled-until-module-loaded behavior carries over unchanged from `ThemeTrigger`
(`ThemeTriggerPlacementTests.TheRowIsStillRendered_DisabledWhenItsModuleNeverArrives`).

---

## 3. Settings — full screen assembly

Route: `/settings`. Public (§0). Structure, adapted from Claude Desktop Settings → General per the
source annotation's reference register, flattened to FIFA's actual field set:

```
Settings
├─ Appearance         (AppearanceControl — always)
├─ Language           (LanguageSelect — always)
├─ Name               (read-only text — signed in only)
└─ Sign Out           (button — signed in only; §4 below)
```

No section labels beyond the page `<h1>Settings</h1>` — four fields don't need internal grouping,
and inventing one would be adding structure the source doesn't ask for.

**Signed-out rendering:** Appearance and Language only. No "sign in to see more" prompt — the two
visible fields are complete and useful on their own; treating the absence of Name/Sign-Out as
something to announce would be manufacturing a gap that doesn't need calling out.

---

## 4. Settings' Sign Out — confirmed against Gate 0

Already specified in Gate 0's R-conflict resolution and unchanged here: plain "Sign Out" button,
calls `Session.SignOut()`, navigates to Home (`/`), no confirmation dialog, no "all devices"
framing. Appears only when signed in. This is the second of the two sign-out locations noted in
Gate 1 §2 — both call the identical method and navigate identically, so they cannot disagree.

---

## 5. i18n — new and retired keys, all three locales

**New**, under `strings`:

| Key | en | es | pt |
|---|---|---|---|
| `settings.title` | Settings | Ajustes | Definições |
| `settings.appearance` | Appearance | Apariencia | Aparência |
| `settings.appearanceSystem` | System | Sistema | Sistema |
| `settings.appearanceLight` | Light | Claro | Claro |
| `settings.appearanceDark` | Dark | Oscuro | Escuro |
| `settings.name` | Name | Nombre | Nome |
| `settings.signOut` | Sign Out | Cerrar sesión | Sair |

Es/pt values `[ASSUMPTION]`, pending Gate 5 review.

**Retired:** none at this gate. `nav.language`, `nav.signOut`, `theme.toLight`, `theme.toDark` stay
— `nav.language` is reused as the `<label>` text for the new dropdown (§1), and `theme.toLight`/
`theme.toDark` are superseded in the UI by the three new `settings.appearance*` keys but are left
in place rather than retired, since retiring them is Gate 3's job together with the other
locale-key cleanup (`useless-text` removal), not scattered across two gates.

---

## 6. Files touched

| File | Action |
|---|---|
| `Pages/Settings.razor` | **new** |
| `Components/LanguageSelect.razor` | **new**, replaces `Components/LanguageSwitch.razor` (deleted) |
| `Components/AppearanceControl.razor` | **new**, replaces `Components/ThemeTrigger.razor` (deleted) |
| `Components/ThemeTrigger.razor.css` | deleted alongside the component |
| `Layout/NavMenu.razor` | already modified in Gate 1; no further change here |
| `wwwroot/i18n/{en,es,pt}.json` | modified — seven new `settings.*` keys |
| `wwwroot/css/app.css` | modified — `.settings-field*` styles |

`wwwroot/js/theme.js` — **not touched.** Confirmed above; `clearStoredTheme()` already covers the
new requirement.

---

## 7. Test impact

**Replaced, not merely adjusted** (component deleted, tests must be rewritten against the new one):

- **`LanguageSwitchTests`** — every assertion targets `.language-switch*` classes and
  `NavMenu`-hosted markup that no longer exists there. The *behaviors* — three-fixed-options
  (`TheSwitchOffersThreeFixedOptionsRatherThanAPicker`, now: exactly one `<select>` with three
  `<option>`s, `Assert.Empty` on button-group markup instead of on `select`), no-navigation
  (`ASwitchDoesNotNavigate`), session-survives (`TheSessionSURVIVESALanguageSwitch`), full-tree
  re-render (`TheNavItselfRerendersIntoTheNewLanguage`) — all restate against `Settings.razor`
  hosting `LanguageSelect`, none are dropped.
- **`ThemeTriggerPlacementTests`** — targets `button.theme-trigger` inside `NavMenu`'s row list; the
  component and its nav placement are both gone. Restate `TheRowIsStillRendered_DisabledWhenItsModuleNeverArrives`
  and `TheRowBecomesUsableOnceItsModuleHasLoaded` against `AppearanceControl` in `Settings.razor`.
  `TheThemeStripIsGoneFromTheMarkupAndTheStylesheet` and the placement-among-destinations assertions
  (`TheTriggerSitsBelowTheDestinations...`) no longer apply to a nav-list component at all — retire
  them rather than force-fitting a placement test onto a page that isn't a list.

**Unaffected, confirm at Gate 5:** `ThemePaletteTests` — asserts CSS token values and contrast
ratios in `app.css`'s theme blocks, none of which this gate touches. Should pass unmodified.

**New tests required**, by name:

- `SettingsScreenTests` — signed-out shows exactly Appearance + Language; signed-in adds Name
  (read-only, matches `HolderName`) and Sign Out; Sign Out ends session and navigates to `/`.
- Fold `AppearanceControl`'s system-clears-storage behavior into whatever replaces
  `ThemeTriggerPlacementTests` — assert `clearStoredTheme()` is invoked on "System" selection specifically,
  since that's the one new code path this gate introduces.

**Frozen-by-precedent, untouched:** `TwoRecordsTests`, `LocalizedChangeTests`, `LocalizedSearchTests`.

---

## Decisions taken

1. Role dropped from Settings; `TrackId` remains structural, never user-facing as an editable field.
2. Settings is public; Name is the only field gated on sign-in.
3. Name is read-only, sourced from `HolderName` — no persistence problem to solve.
4. Calendar connectors out of scope.
5. `LanguageSwitch` → `LanguageSelect`, a native `<select>` (**R7 closed**).
6. `ThemeTrigger` → `AppearanceControl`, tri-state, using the already-exported, previously-unused
   `clearStoredTheme()` — no JS changes needed.
7. Settings' Sign Out matches Gate 0's resolution exactly; no new decision made here.

## Reversals

**R7 — CLOSED.** R6, R8 remain open, both at Gate 3 next.

## Next

Gate 3 — Home, Sign In, and the demo accounts. Resolves **R6** (Sign In notice) and **R8** (demo
account identifier). Per your standing instruction, any further open call inside Gate 3 will surface
as a question here before that document is written.
