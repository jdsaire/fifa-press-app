# v14 (Run 4F) — Gate 0: Reconciliation and Reversal Register

**Status:** Open for approval. One gate; stop here.

---

## Sync confirmation

`jdsaire/fifa-press-app` and `jdsaire/frontend_c6_ecommerce` were both re-fetched fresh
(`codeload.github.com/.../tar.gz/refs/heads/main`, discarding the prior local clones first) before
writing anything below. The GitHub REST API was rate-limited at fetch time, so HEAD SHAs could not
be confirmed by commit hash; codeload serves the live default-branch tarball directly (no
robots.txt restriction, no caching layer to go stale), so content-freshness was confirmed instead
by structural markers: `handoff/` contains `v1`–`v13` with no `v14` present, and
`ux-ui/04-evaluation/` exists — both consistent with the v13-merged state already established
this session. Treat this as **content-verified, SHA-unconfirmed**; if you need the exact commit
hash before proceeding, say so and I'll retry the API call.

---

## Routing correction, restated

The annotations describe a layout state that predates `main`. Restated accurately:

- **Landing (`/`) and Sign In (`/signin`) are already separate screens.** The "Home replaces My
  Access, whose content migrates to a dedicated Sign In space" instruction describes a migration
  that already happened, in an earlier run. There is no create-two-screens-from-one work here —
  the actual work is *reshaping* three screens that already exist in their own right.
- **Sign-in and sign-out already exist as paths**, just not as a persistent top-bar affordance. Sign-out is a
  `nav-signout` button in the sidebar (`Layout/NavMenu.razor`); sign-in is a CTA on Landing. The
  accurate finding, and the one this run specifies against, is: **neither is visible from every
  screen** — both live only in the collapsible sidebar, which is exactly the gap ShopEase's
  persistent `top-row` closes.

This confirms the annotations' underlying intent (surface auth status persistently, give sign-in
one obvious door) without accepting their premise (that no such path exists at all).

---

## Conflicts — annotation instructions against documented codebase decisions

Four instructions in the dossier prompt would overturn a decision the codebase documents in its
own comments. Each gets a reversal number now; the resolution argument is specified in the gate
named, not here — Gate 0's job is to name the conflict and open the register, not settle it twice.

### R6 — The Sign In notice is not "useless text"

**Annotation says:** four `signin__notice` paragraphs are "useful for the designers or developers,
but not for the end users" and should be eliminated.

**Codebase says**, `Pages/SignIn.razor` header comment:
> "Visible before any interaction, and first on the page. The four things it states are in the
> order 10 §2.3 sets: what this is, that the credentials below are real and work, how long the
> session lasts, and what it is not."

This is not incidental copy — it is sourced from a governing document (10 §2.3), deliberately
ordered, and framed as the fix for a specific prior defect (v9's notice going quietly false). The
annotation's own attached rationale ("This is a simulated sign-in... nothing that could tell you
apart from anyone else") is drawn near-verbatim from this same notice, which suggests the objection
is to *length and prominence*, not to the disclosure existing at all.
**Resolved at Gate 3.** Landing's disclosure is separately confirmed as also non-removable in
Gate 3's own text, for the same reason — consistency across both entry points is part of the
resolution, not incidental.

### R7 — `LanguageSwitch` as a dropdown

**Annotation says:** language switching moves into Settings as a dropdown ("not pressing buttons
anymore").

**Codebase says**, `Components/LanguageSwitch.razor` header comment:
> "Three fixed options, rendered as three buttons rather than a dropdown: a picker's affordance
> exists to manage a list too long to show, and three is not that."

Direct contradiction, and the codebase's stated reasoning (three doesn't need a picker) is sound
*for the control's current context* — a persistent sidebar row. **Resolved at Gate 2.** The
strongest available argument for the reversal is contextual, not a rebuttal of the original logic:
the control is moving from a persistent, always-visible sidebar row to a settings page the user
visits deliberately, where a labelled dropdown matches the register of the other Settings fields
(Name, Role, Appearance) better than three loose buttons would. Confirmed at Gate 2, not assumed
here.

### R8 — Demo account identifier renaming

**Annotation says:** rename `demo_shopper1`-style ShopEase identifiers into FIFA equivalents like
`demo_staff1`.

**Codebase says**, `Services/DemoAccountStore.cs`, on why `Identifier` equals `CredentialId`:
> "one fewer invented value to keep in sync, and it makes the connection between the sign-in and
> the record visible rather than magic."

A rename to `demo_staff1` breaks this identity outright — `MyAccess`/`/record` keys its data by
`CredentialId`, and a `demo_staff1` identifier would need a second, separately-maintained mapping
back to `MP-2026-04817` to keep working, exactly the "invented value to keep in sync" the current
design avoids. **Resolved at Gate 3**, with three options on the table (rename and accept the
break; keep the credential number and drop only the persona prose; show a memorable label
alongside the credential) and a recommendation to be made there, not here.

### R9 — Auth status in the top bar vs. the sidebar session indicator

**Annotation implies:** a ShopEase-style persistent top bar replaces the current sidebar-only auth
surfacing.

**Codebase says**, `Layout/NavMenu.razor` header comment, on why the `nav-session` block sits where
it does:
> "Above the rows, and read-only... on every screen except the record itself, nothing else on the
> page says [which holder is signed in]."

Adding a top bar makes this comment's premise false — something else *would* say it, on every
screen. Left unresolved, this produces either silent duplication (two indicators disagreeing on
prominence) or a silent deletion the comment never sanctioned. **Resolved at Gate 1.** Duplication
is ruled out by the dossier's own hard constraints; the choice is between retiring the sidebar
block in favor of the top bar (with the reversal recorded here) or finding a division of labor
between the two that the comment's original reasoning still supports. Gate 1 decides which.

---

## Register (open)

| # | Conflict | Resolved at | Status |
|---|---|---|---|
| R6 | Sign In notice framed as removable "useless text" | Gate 3 | Open |
| R7 | LanguageSwitch: three buttons → dropdown | Gate 2 | Open |
| R8 | Demo account identifier rename (`demo_staff1`) | Gate 3 | Open |
| R9 | Sidebar session indicator vs. persistent top bar | Gate 1 | Open |

Each closes with the resolving gate's approval, not before. None are pre-decided by this document.

---

## One non-conflict worth flagging now, so Gate 2 isn't a surprise

`Components/ThemeTrigger.razor` is a **strict binary toggle** — `current == "dark" ? "light" :
"dark"` — with "system" used only as an *unstored fallback* when no explicit choice has ever been
made, never as a value a person can select and have stored. There is no comment asserting "must
stay binary," so this is not a reversal — but the tri-state Appearance control Gate 2 specifies is
genuinely new mechanism, not a relocation of the existing toggle. Flagging it here so Gate 2
budgets for it correctly.

---

## Frozen paths — confirmed untouched by this dossier

`ux-ui/00-initial-evaluation/`, `ux-ui/01-design-research/`, `ux-ui/02-ideation/`,
`ux-ui/03-ui-prototyping/` (including files `09`–`12`). Nothing specified in Gates 1–5 touches
these paths — every file this run reaches lives under `src/`, `tests/`, or
`wwwroot/i18n/`. Confirmed, not just asserted: none of the conflicts above, nor anything in the
source prompt, names a file under `ux-ui/`.

---

## Next

Gate 1 — Navigation, top bar, and iconography. Resolves **R9**, specifies the top bar's FIFA
equivalent of `CartSummary` (or its deliberate absence), and adds the four nav icons.

Stopping here for approval.
