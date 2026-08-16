# 12 — Decision Reversals

**Status:** proposed, for gate approval. Fourth and final file of Run 4D, the design addendum
dossier.
**Authority:** `P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md` §2 ("The four decision reversals, stated
for the record") and §1 resolution table.
**Purpose, stated once because it governs the whole file.** Project doctrine — Gate 0 of every
mandate, and the v7 rename precedent before that — requires a reversal be recorded with its reason,
not silently overwritten. This file is where that happens for Run 4D. It does not edit
`03_UI-DECISIONS.md` or `05_SCREENS.md`; both stay exactly as committed. This file sits beside them
and names, for each reversal, the section reversed, the reasoning that section gave, what has
changed since, and what — if anything — survives the reversal unchanged.
**A reader using this file:** should be able to open the cited section in the frozen gate file,
read the original decision and its stated reason, then read this file's account of why that reason
no longer controls, without needing anything else open at the same time.
**Verified against:** live clone, HEAD `147bc4a` (v10 merged, 16 Aug 2026) — all four citations
below were re-read from the live files at authoring time, not carried from memory of earlier gates
in this run.

---

## 1. R1 — Sign In becomes a real (simulated) session

**Reverses:** `05_SCREENS.md` §6.1 and §6.3.

**What §6.1 said, verbatim in substance:** *"This is a form, not authentication. There is no
credential store, no session security, no authorization, and nothing behind it… the screen must
therefore say what it is on the screen itself."* `NavMenu.razor`'s own code comment, still in the
live file at the time of this reversal, calls Sign In *"a door, not a section"* that *"gates
nothing."*

**What §6.3 excluded, and its stated reason for each:**

| Excluded | §6.3's stated reason |
|---|---|
| Published demo credentials on screen | "This app has no credential store at all — there is nothing to publish, and publishing placeholders would imply a store exists" |
| `AuthenticationStateProvider` / `AuthorizeView` / signed-in state | "These constitute a session system. Out of scope for 4B entirely; adopting them would make the interface's claim true in form while remaining false in substance" |
| Sign-out | "No session to end" |
| Redirect-on-success to a protected page | "Nothing is protected. There is no authorization in this app" |

**What changed.** Direct use of the deployed v9 build surfaced that arriving at a personal access
record with no who-are-you moment breaks the mental model — confirmed even though the form was
built exactly to §6.1–§6.5's spec. Separately, R4 (§4 below) means something *is* now protected: the
personal record is gated. Both premises §6.3 built its exclusions on — "there is nothing to
publish" and "nothing is protected" — no longer hold.

**What reverses.** All four exclusions above, per `10_AUTH-AND-ONBOARDING.md` §2.1: a
`DemoAccountStore`-equivalent with two published, working, fake credentials; an in-memory
`AuthenticationStateProvider` explicitly documented as not ASP.NET Identity; a visible sign-out; and
redirect-on-success to the now-gated record.

**What does not reverse, and this is the more important half.** §6.1's own core sentence — *"a
login form that implies an account system it does not have is a lie by interface"* — is **not**
reversed and continues to govern every screen this reversal touches. `10_AUTH-AND-ONBOARDING.md`
§1 restates it as the file's first line for exactly this reason. The injection-pattern blocklist
exclusion in §6.3's table is **also not reversed**: it was never premised on "nothing exists behind
this form," it was premised on the blocklist rejecting real names like *O'Neill*, which remains true
regardless of what does or does not sit behind the form. A reversal that touched every row in a
table without checking each row's own reasoning would be sloppier than the decision it replaces;
this one does not.

---

## 2. R2 — Theme trigger moves to the left panel

**Reverses:** `03_UI-DECISIONS.md` §3.2.

**What §3.2 said, verbatim.** Two placements were considered. The rejected one — inside `.top-row`,
the sidebar's own header — was rejected in these words: *"at desktop width `.top-row` sits at the
top-left of the viewport, not the top-right — the sidebar is a left column. A person told 'the
toggle is top right' who then has to look left because they're on a laptop is exactly the kind of
small inconsistency the concept's own Principle 1 argues against."* The adopted alternative was *"a
new, minimal persistent strip above `main`, present at every breakpoint."*

**What changed.** Nothing about §3.2's diagnosis was wrong — "top right at every breakpoint" and "a
control living in the sidebar's own header" genuinely are incompatible on a sidebar-left layout, and
§3.2 correctly worked out why. What changed is the *instruction* §3.2 was satisfying. That
instruction came from the dossier's own top-right requirement. Resolution 3 in the patch replaces it
with a different requirement entirely — Apple iOS-style placement inside primary navigation chrome —
so §3.2's rejected option is no longer being asked to satisfy the goal it failed against.

**What reverses.** The adopted strip above `main` is discontinued. `MainLayout.razor`'s
`<div class="theme-strip">` wrapper and `MainLayout.razor.css`'s `.theme-strip` rules are removed,
not repurposed. The trigger moves into the relocated nav list `09_DESIGN-ADDENDUM.md` §5.2
specifies — inside `NavMenu.razor`'s own list, the exact structural location (though not the exact
literal instruction) §3.2 evaluated and turned down.

**What does not reverse.** `theme.js`'s storage and application mechanism, `ThemeTrigger.razor`'s
`@code` block, and §3.1's persistence/default-resolution rules are all untouched — this is a
placement reversal, not a mechanism reversal, and `09_DESIGN-ADDENDUM.md` §5.3 states that
boundary explicitly.

---

## 3. R3 — Dark mode retained but re-derived

**Reverses:** a discontinuation decision made after `03_UI-DECISIONS.md` was committed — stated
here because this is, unusually, a reversal of a reversal, and the file recording it should say so
rather than let the double negative pass unremarked.

**What the intervening decision was.** Between v9 shipping and this patch, dark mode was slated for
discontinuation and its trigger for removal — a decision made in response to the FIFA-branding
question, on the reasoning that FIFA's own public site carries no dark mode and a closer visual
alignment with it would drop the feature. That decision is itself what R3 withdraws.

**What §3.2's own justification for dark mode said, and why it survives.** `03_UI-DECISIONS.md`
§3.2's closing paragraph grounds the whole dark/light requirement in Amina's context: *"Amina reads
this app in a stadium concourse, phone in hand, moving between two devices across a tournament… a
control that lives in a different corner depending on which device she's holding is a worse failure
than it looks on a design system audit."* The concourse-context reasoning was never about FIFA's own
branding; it was about outdoor glare and device-switching, and neither of those changed when the
branding question came up.

**What changed.** Resolution 4 settles the branding question as *inspired-by*, not *matching*, and
resolution 3 makes Apple HIG — which does support dark mode — the governing design authority rather
than FIFA's site. With the discontinuation's own premise (closer FIFA alignment) replaced by a
different governing authority that has no such constraint, the reasoning for discontinuing no longer
applies, and the original §3.2 reasoning for having it in the first place was never touched.

**What reverses.** The discontinuation itself: dark mode stays. Its *values* also change —
`09_DESIGN-ADDENDUM.md` §4 re-derives every dark-palette token against a `#000000` anchor in place
of the current `#121212`, per resolution 3's "solid-white/solid-black" instruction — but this is a
palette correction bundled into the same resolution, not a second reversal in its own right, since
no prior gate file specified `#121212` as a considered-and-chosen value the way §3.2 specified the
trigger's placement.

**What does not reverse.** `03_UI-DECISIONS.md` §3.3's AA-floor verification discipline — every
token checked against its correct WCAG 2.2 threshold, in both light and dark — is not reopened as a
practice, only re-run against new values. `09_DESIGN-ADDENDUM.md` §4.2's table is that re-run, and
every re-derived value clears its floor with more headroom than the value it replaces, so no token
that passed before now fails. The net effect, as the patch itself puts it, is *"smaller than first
assumed: a palette re-derivation and a trigger relocation, not a feature removal."*

---

## 4. R4 — Not every route stays reachable without signing in

**Reverses:** `05_SCREENS.md` §6.2 and §6.5.

**What was said, verbatim in substance.** §6.2 requires every part of the app be reachable without
signing in. §6.5 closes the sign-in specification with the same statement generalized: *"Everything
in this app is reachable without signing in. Since there is no authorization, the form gates
nothing — and the screen must not imply otherwise by, for example, presenting itself as a barrier
to My Access."* v9's own verification confirmed this held: `handoff/v9/Completion-Report-v9.md`
records `PASS — no route is guarded` against exactly this requirement.

**What changed.** Resolution 11 introduces a public landing view at `/` for signed-out visitors and,
as its direct consequence, moves the personal access record behind sign-in. The reason given in the
patch: *"an access record is personal by definition; showing one to an unauthenticated visitor
contradicts the concept more than gating it does."* This is not a response to a defect in the
original decision — v9's "no route is guarded" was a correct implementation of what §6.2 asked for.
It is a response to the concept's own logic pointing the other way once a public landing view exists
to hold the front door instead of the record itself.

**What reverses.** The personal record (My Access) requires a session; `Request access`, as a write
to that personal record, requires one too, per `10_AUTH-AND-ONBOARDING.md` §5.2's reasoning — a
write with no holder attached goes nowhere useful, which is a new problem R4 creates and R4's own
file resolves.

**What survives the reversal, stated as the patch itself states it and not weakened in the
restating:** *"Matches and Help stay public. Only the personal record is gated, and the gate must
never imply real security — the simulation notice governs."* `10_AUTH-AND-ONBOARDING.md` §5.3 turns
that into a concrete rule — a gated route reached while signed out shows the full sign-in screen
with its simulation notice, never borrowed vocabulary from real authorization systems like "403" or
"access denied." §6.1's lie-by-interface principle, carried into R1 above, is what makes that rule
necessary here too: a gate that borrows real-security language would be exactly the lie the whole
sign-in redesign exists to avoid telling.

---

## 5. A fifth item, flagged rather than numbered — the `Change` entity's shape

**Not one of the four reversals the patch enumerates**, and this file does not renumber it as R5.
Named here because `11_I18N.md` §9 identified it and asked this file to either record it or state
plainly why it does not qualify — leaving the gap unaddressed would undercut the one file whose job
is to keep the repo honest about exactly this kind of thing.

**What `06_DATA-MODEL.md` specifies, as committed.** The `Change` entity's `whatChanged`, `reason`,
`nextStep`, and `conditionText` fields are typed as plain `string` — single-language, English
content authored once per seeded record, consistent with every other seeded field in the file.

**What changes.** `11_I18N.md` §4.2 establishes that seeded free-text content cannot be translated
at render time the way static UI strings can — each field becomes three parallel strings, one per
locale, authored together at seed time. This is a genuine change to the entity's shape, not a
rendering-layer addition on top of an unchanged model.

**Why this is not treated as a fifth numbered reversal.** R1–R4 each reverse a *decision* — a
choice the frozen dossier considered and made, with a stated reason, about how something should
work. `06_DATA-MODEL.md`'s `string` typing was never a considered-and-rejected-alternative decision
in the way §3.2's trigger placement or §6.3's exclusion list were; it was simply the correct typing
for a single-language app, which this app was until this patch. Extending it to a locale-keyed shape
is closer to `03_UI-DECISIONS.md` §3's palette re-derivation (R3, §3 above) than to a considered
reversal — a value correctly derived under an assumption that has now changed, not a stance that was
argued for and against.

**What this file does instead of numbering it.** Records it here, plainly, as a genuine model change
this run of the dossier surfaces, so that `4E`'s plan does not present the entity change as a
newly-discovered implementation necessity when it was in fact anticipated and named in `4D`. Any
future reader auditing "what did 4D actually change beyond R1–R4" should find it by reading this
section, not by diffing the entity definition against `06_DATA-MODEL.md` and wondering whether the
drift was authorized.

---

## 6. Summary table, for a reader who wants the four-plus-one at a glance

| # | What reverses | Section(s) reversed | What survives unchanged |
|---|---|---|---|
| R1 | Sign In gates the personal record; demo credentials, session, sign-out all real (simulated) | `05_SCREENS.md` §6.1, §6.3 | §6.1's lie-by-interface principle; the injection-blocklist exclusion; every §6.2/6.4 field-validation rule |
| R2 | Theme trigger moves into the nav list | `03_UI-DECISIONS.md` §3.2 | `theme.js`'s mechanism; §3.1's persistence rules |
| R3 | Dark mode's prior discontinuation is itself withdrawn; palette re-derived to `#000000`-anchored | *(intervening decision, not a §3.2 line)* | §3.2's concourse-context justification; §3.3's AA-floor discipline as a practice |
| R4 | Personal record and its write path require sign-in | `05_SCREENS.md` §6.2, §6.5 | Matches and Help stay public; the simulation notice governs the gate |
| *(unnumbered)* | `Change`'s four free-text fields become locale-keyed | `06_DATA-MODEL.md` §2.3 (entity typing) | The append-only log, ordering, and supersession mechanism — all locale-independent |

This table is a navigation aid only. Where it and the prose sections above disagree on any detail,
the prose sections govern — the table exists so a reader can find the right section quickly, not so
it can be cited in place of reading the reasoning it summarizes.
