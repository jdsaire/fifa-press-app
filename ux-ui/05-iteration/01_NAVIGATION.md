# v14 (Run 4F) — Gate 1: Navigation, Top Bar, and Iconography

**Status:** Open for approval. Resolves **R9**. One decision flagged for explicit sign-off (§4).

---

## 1. R9 — resolved: the session indicator moves to the top bar

**The conflict.** `Layout/NavMenu.razor` documents why the `nav-session` block sits in the sidebar:

> "Above the rows, and read-only. With two demo records whose whole point is comparison, 'which one
> am I looking at' is a question the interface has to answer without being asked — and on every
> screen except the record itself, nothing else on the page says."

**The answer.** The comment's *premise* is what v14 changes, not its reasoning. Its reasoning — the
holder must be identifiable from every screen without asking — is correct and is exactly what a
persistent top bar delivers **better**, because the sidebar is collapsible at the mobile breakpoint
and the top bar is not. Today, a phone user with a collapsed nav has no holder indicator at all;
the comment's own goal is unmet in precisely the case it matters most.

**Resolution:** retire `nav-session` from the sidebar; the holder indicator lives in the new top
bar. This is a **relocation that better serves the original intent**, not a rejection of it — but
it does falsify the comment as written, so it is recorded as a reversal.

> **R9 — CLOSED.** The session indicator moves from the sidebar (`nav-session`) to a persistent top
> bar. Original reasoning upheld; original placement superseded because it fails at the collapsed
> breakpoint. `NavMenu.razor`'s comment is deleted along with the block it describes — it does not
> survive as a stale comment about a component that has moved.

**Duplication is ruled out:** exactly one holder indicator exists after this change.

---

## 2. The top bar — `Layout/SessionBar.razor` (new)

Modelled on ShopEase `Layout/AuthStatus.razor` `[VERIFIED]`, adapted for FIFA's two-record purpose.

**Placement.** Inside `<main>`, immediately above `<article class="content">`, as
`<div class="top-row px-4">` — matching ShopEase's `MainLayout.razor` structure `[VERIFIED]`.
`MainLayout.razor`'s existing comment ("main holds only the content column. The theme control used
to sit in a strip above it") is now false and must be rewritten, not left in place.

**Signed out:** a single `Sign in` link → `/record` (not `/signin` — per the Gate 3 boundary
decision, `/record` is the conditional sign-in/dashboard surface).

**Signed in:** the holder's name and credential ID, plus a `Sign out` button.

Both the name **and** the credential ID are carried, departing from ShopEase's name-only
`AuthStatus`. Justification: the credential ID is the record key and the thing that differs
between the two demo records — `DemoAccountStore.cs` documents that `Identifier` *is*
`CredentialId` specifically to make that connection visible. Dropping it would hide the value the
two-record demonstration exists to expose.

**The indicator is not a link.** ShopEase's `CartSummary` is a plain `<div>`, not an anchor
`[VERIFIED]`; FIFA's indicator follows suit. The record gets its door from the nav (§4), not from
a clickable status block. This also preserves the read-only property the retired `nav-session`
block had, and which `SignOutTests` asserts today.

**Sign-out appears in two places** after Gate 2 (top bar and Settings), and this is intentional.
The distinction that makes it acceptable: **a duplicated status indicator can disagree with itself;
a duplicated action cannot.** Two indicators showing different holders is a defect; two buttons
ending the same session is a convenience. Both call `Session.SignOut()` and navigate to Home,
identically.

---

## 3. No `CartSummary` analogue — decided, not hedged

**FIFA ships no pending-requests counter in the top bar.** ShopEase's cart badge has no viable
FIFA equivalent at acceptable cost, for a reason that is architectural rather than aesthetic.

A pending-requests count is not a stored field. It is derived: `GetChangesAsync(credentialId)` →
fold by `AsOfUtc` → per-match `StatusFor()` → count `MatchAccessStatus.Requested`. That derivation
lives in `Pages/MyAccess.razor` (`StatusFor`, ~lines 380–400) and is roughly 130 lines of
fold-and-resolve logic. Putting a counter in `MainLayout` means either:

- **duplicating the derivation** — creating a second computation of record state that can disagree
  with the record itself. `ChangeArrivalTracker`'s own comment names this as the thing to avoid:
  *"It is not a second copy of the log, it cannot disagree with the record"*; or
- **extracting the fold into a shared service** — correct, but a refactor of the record's core
  logic, which is squarely outside "only specify what was requested" and would put the
  frozen-by-precedent `TwoRecordsTests` in scope.

Neither is worth a badge. The top bar carries auth status only.

*Deferred, not dismissed:* a counter becomes cheap once the status fold is extracted into a shared
service. That extraction is a reasonable standalone future run; it is not this one. Recorded here
so the omission reads as a decision rather than an oversight.

---

## 4. Nav restructure — one item needs your explicit sign-off

**Current** (`nav.flex-column > .nav-item`, verified): Record · Matches · Help · LanguageSwitch ·
ThemeTrigger, plus a conditional sign-out row = **5 rows signed out, 6 signed in**.

**Target:**

| Row | Route | Icon | Visibility |
|---|---|---|---|
| Home | `/` | house | always |
| Matches | `/matches` | football | always |
| **My Requests** | `/record` | clipboard | **signed in only** |
| Help | `/help` | question mark | always |
| Settings | `/settings` | gear | always |

Language and theme leave the nav entirely (into Settings, Gate 2). Sign-out leaves the nav (into
the top bar, §2). The nav becomes **destinations only** — which is what `NavMenu.razor`'s existing
comment already says it should be:

> "Three destinations, then the controls. Sign in is not among the destinations — it is a door, not
> a section."

v14 completes that doctrine rather than reversing it: after this gate the nav contains destinations
and nothing else, and the "controls in the tail" exception disappears because the controls have a
home of their own.

> ### ⚠ Decision required — the fifth row
>
> The annotations specify **four** nav items (Home, Matches, Help, Settings). I am proposing a
> **fifth, conditional** row — "My Requests" → `/record` — because the dashboard otherwise has no
> persistent door: the top-bar indicator is deliberately not a link (§2), and `/record` is where
> every submitted request lands.
>
> A signed-out visitor still sees exactly the four items the annotations describe. The fifth
> appears only once there is a record to look at.
>
> **Alternatives if you'd rather hold at four:** (a) make the top-bar indicator a link to `/record`
> — costs the read-only property and diverges from ShopEase's non-anchor `CartSummary`; (b) reach
> the dashboard only via post-submit redirect and Home's CTA — fragile, and leaves signed-in users
> with no standing way back.
>
> **Recommendation: approve the fifth conditional row.** Confirm or override before Gate 2.

---

## 5. Icons — `Components/Icon.razor`

Five glyphs added to the existing `switch`-arm form. No new dependency; `Icon.razor`'s standing
rules are unchanged and unbroken:

- **Decorative, always** — each sits beside a nav label that already says the word, so
  `aria-hidden="true"` / `focusable="false"` continue to be correct. No conflict with the file's
  "EVERY ICON HERE IS DECORATIVE, AND THAT IS A RULE" clause.
- **`currentColor` only** — no hex, no `rgb()`, per `IconTests` assertions.
- 16×16 viewBox, `stroke-width="1.5"`, round caps/joins.

```razor
// A house: a roof over a body.
"home" => @<g>
    <path d="M2.25 7.25 8 2.5l5.75 4.75" />
    <path d="M3.75 6.5v7h8.5v-7" />
</g>,

// A football: a ball with a centre panel and its seams.
"matches" => @<g>
    <circle cx="8" cy="8" r="5.75" />
    <path d="M8 4.6 5.4 6.5l1 3.05h3.2l1-3.05Z" />
    <path d="M8 2.25v2.35M3.15 6.2l2.25 .3M4.55 11.6l1.85-2.05M11.45 11.6 9.6 9.55M12.85 6.2l-2.25 .3" />
</g>,

// A clipboard: a board with a clip and two written lines.
"record" => @<g>
    <path d="M5.75 3.25h-1.5a1 1 0 0 0-1 1v8.5a1 1 0 0 0 1 1h7.5a1 1 0 0 0 1-1v-8.5a1 1 0 0 0-1-1h-1.5" />
    <rect x="5.75" y="2" width="4.5" height="2.5" rx="0.75" />
    <path d="M6 8h4M6 10.5h2.5" />
</g>,

// A question mark inside a circle.
"help" => @<g>
    <circle cx="8" cy="8" r="5.75" />
    <path d="M6.4 6.4a1.65 1.65 0 1 1 2.2 1.6c-.4.15-.6.5-.6.9v.3" />
    <path d="M8 11.55h.01" />
</g>,

// A gear: a hub, and six teeth around it.
"settings" => @<g>
    <circle cx="8" cy="8" r="2.15" />
    <path d="M8 1.9v1.7M8 12.4v1.7M13.15 5.05l-1.5.85M4.35 10.1l-1.5.85M13.15 10.95l-1.5-.85M4.35 5.9l-1.5-.85" />
</g>,
```

`[ASSUMPTION]` on **path geometry only**. The structure, attributes, and accessibility properties
are `[VERIFIED]` against the existing file's conventions; the exact coordinates have not been
visually rendered and may need tuning at implementation. Claude Code should render each at 16px in
both themes and adjust, without changing the attribute contract. The gear's six-tooth form and the
football's five-seam form are the most likely to need it.

Update the `Name` XML doc comment, which currently reads "`date`, `location` or `phase`."

---

## 6. i18n — new keys

Two new keys, required in **all three** of `wwwroot/i18n/{en,es,pt}.json` under `strings`:

| Key | en | es | pt |
|---|---|---|---|
| `nav.home` | Home | Inicio | Início |
| `nav.settings` | Settings | Ajustes | Definições |

**Retained and reused:** `nav.matches`, `nav.help`, `nav.signIn`, `nav.signOut`, `nav.signedInAs`,
`nav.menu`, `nav.skipToContent`, `app.name` — all verified present.

**`nav.record` is retained**, not retired — it relabels the conditional "My Requests" row. Confirm
at Gate 3 whether its *value* should change from the current record-framing to a requests-framing
in all three locales; the key itself stays.

Spanish/Portuguese values above are `[ASSUMPTION]` pending Gate 5 review against the existing
translation register in those files.

---

## 7. Files touched

| File | Action |
|---|---|
| `Layout/SessionBar.razor` | **new** — top bar auth status |
| `Layout/MainLayout.razor` | modified — insert `top-row`; rewrite the now-false comment |
| `Layout/NavMenu.razor` | modified — remove `nav-session`, `nav-signout`, `LanguageSwitch`, `ThemeTrigger` rows; add Home/Settings/My Requests rows; add icons |
| `Components/Icon.razor` | modified — five glyphs, updated doc comment |
| `wwwroot/i18n/{en,es,pt}.json` | modified — `nav.home`, `nav.settings` |
| `wwwroot/css/app.css` | modified — `.top-row`, `.session-bar*` styles |

---

## 8. Test impact

**Breaks outright — requires rewrite, not adjustment:**

- **`SignOutTests`** — asserts `nav.flex-column > .nav-item` counts of exactly **5** (signed out)
  and **6** (signed in), asserts `rows[3]` is `.language-switch`, `rows[4]` is `button.theme-trigger`,
  `rows[5]` is `button.nav-signout`, and asserts `.nav-session` exists in the sidebar, is read-only
  (`Assert.Empty(indicator.QuerySelectorAll("a, button, select, input"))`), and sits above the rows.
  Every one of those assertions is false after this gate. The *behaviours* they protect — the
  indicator names holder and credential, is read-only, follows the session without a render lag,
  and is absent when signed out — must all survive, re-pointed at `SessionBar`.

**Affected, confirm at Gate 5:** `LanguageSwitchTests`, `ThemeTriggerPlacementTests` (both
primarily Gate 2's problem — the components move there), `LandingTests` (references nav).

**`IconTests`** does not break: its `[Theory]` `[InlineData]` list is additive and the new glyphs
satisfy the same assertions. Extend it with the five new names rather than rewriting it.

**Frozen-by-precedent, untouched:** `TwoRecordsTests`, `LocalizedChangeTests`, `LocalizedSearchTests`.

---

## Decisions taken

1. `nav-session` retires from the sidebar into a persistent top bar (**R9 closed**).
2. Top bar carries holder name + credential ID, departing from ShopEase's name-only pattern.
3. The indicator is not a link, matching ShopEase's non-anchor `CartSummary`.
4. **No pending-requests counter** — the derivation can't be relocated without a second source of
   truth or an out-of-scope refactor.
5. Nav becomes destinations-only; language, theme, and sign-out all leave it.
6. Five icon glyphs, geometry provisional pending visual check.

## Reversals

**R9 — CLOSED.** R6, R7, R8 remain open at their assigned gates.

## Awaiting

The §4 fifth-row decision. Everything else in this gate stands independent of it.

## Next

Gate 2 — the Settings screen. Resolves **R7** (LanguageSwitch → dropdown), specifies the tri-state
Appearance control, and settles whether Settings is public or gated.
