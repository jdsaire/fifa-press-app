# Heuristic Evaluation

**Repo path:** `ux-ui/04-evaluation/03_HEURISTIC-EVALUATION.md`
**Direct continuation of:** `00-initial-evaluation/heuristic-evaluation.md` — same ten heuristics,
same three-tier scale (Fails / Partial / Passes), reported next to `00`'s own per-principle call so
movement is visible principle by principle, not only in aggregate (`00_SCOPE.md` §3 item 5)
**Baseline counts, corrected per `00_SCOPE.md` §7.5:** `00` — **5 Fails · 4 Partial · 1 Passes**
(heuristics 2, 3, 4, 6, 7 Fail; 1, 5, 8, 9 Partial; 10 Passes)
**Audited build:** `main` @ `b37066d`, read in source
**Checked in both themes**, per the mandate's requirement. Finding: **no heuristic-level score
differs by theme.** Every component's structure, semantics, and behavior are identical under light
and dark — confirmed by `ThemePaletteTests` existing as a suite wholly separate from any
component-behavior test, and independently by Gate 2 Attempt 6, which found dark theme changed
nothing but color and weight. Where a claim below is theme-independent by nature (a label existing,
a route resolving), it is not re-verified twice. **Contrast itself — whether a color is legible
enough — is explicitly Gate 4's question, not this one's**, and no heuristic score below is
inflated or deflated by anticipating that result.

---

## At a glance

| # | Heuristic | `00` | `04` | Movement |
|---|---|---|---|---|
| 1 | Visibility of system status | Partial | **Partial** | Same band, different shape — see below |
| 2 | Match between system and the real world | Fails | **Passes** | ↑ |
| 3 | User control and freedom | Fails | **Fails** | Same band, unresolved |
| 4 | Consistency and standards | Fails | **Fails** | Same band, different (single) cause |
| 5 | Error prevention | Partial | **Passes** | ↑ |
| 6 | Recognition rather than recall | Fails | **Passes** | ↑ |
| 7 | Flexibility and efficiency of use | Fails | **Passes** | ↑ |
| 8 | Aesthetic and minimalist design | Partial | **Passes** | ↑ |
| 9 | Help users recognise and recover from errors | Partial | **Passes** | ↑ |
| 10 | Help and documentation | Passes | **Passes** | Same band, one prior gap now closed |

**Totals: Fails 5→2 · Partial 4→1 · Passes 1→7.**

---

## 1. Visibility of system status

**`00`:** Partial — a working confirmation and a persistent "Registered" badge, undermined by no
live region reaching assistive technology, no way to answer "where am I", and a miscounted
attendee total.

**`04`: Partial — genuinely different reasons, not the same gap carried forward.**

What's fixed: `StaleIndicator` renders unconditionally on every screen holding cached state, always
— not only when something is wrong (`StaleIndicator.razor`, header comment: "the stale row is the
deliverable"). Active nav state now works correctly (`NavMenu.razor:40-56`, ordinary `NavLink`
prefix matching on real paths). Both `/events/{id}` and `/request/{id}` carry a breadcrumb
(`aria-label="breadcrumb"`, `EventDetails.razor:27`, `Registration.razor:26`). `PageTitle` is
computed per-screen from real content (`EventDetails.razor`'s `PageHeading` includes the match
number and label) rather than a static string — the exact defect `00` flagged (UX-MAJ-11) is gone.

What isn't: **the Gate 2 finding.** `MyAccess.razor`'s per-match status word and
`EventDetails.razor`'s headline status word both derive from `Change.Kind` alone, with no check
against `Urgency` or `EffectiveUtc` — so a still-conditional change reads as decided
(`02_TASK-RESULTS.md`, Attempt 1). This is a system-status failure in the most literal sense: the
system is not accurately informing the user what is going on. **A second, narrower gap**: a
successful write's confirmation — the new `ChangeRow` opening and animating in — has no `aria-live`
region announcing it, unlike the form-validation errors elsewhere in the app, which do
(`SignIn.razor:107,127`, `RequestAccessForm.razor:36,54` both use `aria-live="polite"`). A sighted
user sees the row arrive; nothing is announced to a screen reader user, which is a narrower version
of the exact gap `00` flagged as UX-MAJ-05.

---

## 2. Match between system and the real world

**`00`:** Fails — every event rendered as three editable text boxes; an "About" link opening
framework documentation instead of describing the app.

**`04`: Passes.**

`EventCard` now defaults to read-only (`ReadOnly = true`, `Components/EventCard.razor:85`) and both
call sites that matter — the match list and the request-flow header — pass `ReadOnly="true"
AllowEdit="false"` explicitly (`EventList.razor:98-99`, confirmed identically for the fixture header
on `Registration.razor`). Browsing a match no longer looks like editing a spreadsheet row. No link
anywhere in `NavMenu.razor` points at framework or vendor documentation — the entire nav is three
named destinations plus language, theme, and (signed in) sign-out. "Register" was renamed to
"Request access" specifically because "the real-world action here is asking for access to one
match, not creating an account" (`Registration.razor`, header comment) — a direct, named repair of
the same class of mismatch `00` flagged in the "About" link. The withholding rule itself —
`detail.unplayed`: "This match has not been played, so the teams in it are not shown" — is a
real-world-conventions win in its own right: it matches how actual accreditation systems withhold
information rather than inventing a UI convention with no real referent.

---

## 3. User control and freedom

**`00`:** Fails — registration is irreversible, with no counterpart action and a permanent badge.

**`04`: Fails — the same shape of gap, on the app's one write action, carried forward unresolved
across four runs.**

`WithdrawRequestAsync` exists in the data layer and is exercised by its own tests, but is reachable
from **no screen in the app** — an open item explicitly restated at every handoff since v9 and
carried again in the v12 Completion Report's own open-items table. `Registration.razor`'s header
comment states the absence is deliberate — "no surface for it has been specified. Adding one here
would be inventing scope rather than completing it" — which is an honest design statement, not a
gap the build failed to notice, but the heuristic doesn't grade intent, it grades the interface: a
person who requests the wrong match's access today has no marked exit from that state, exactly as a
person who registered for the wrong event in `00` didn't.

**Genuinely better elsewhere:** sign-out is a single, unconfirmed, reversible action by design
("there is nothing to lose, and a modal guarding a simulated session would be theatre",
`NavMenu.razor`); language and theme are both fully reversible, independent controls. The gap is
narrow — one write path — but it is the app's only consequential write path, which is exactly why
the heuristic still fails rather than moving to Partial.

---

## 4. Consistency and standards

**`00`:** Fails — inconsistent link bases, mismatched tab-title/heading naming, an editable control
that never saves.

**`04`: Fails — a different, single, more severe cause.**

Every scattered issue `00` named is gone: link handling is uniform (all internal `NavLink`/`href`
targets are relative, app-root paths), tab titles match headings because both are computed from the
same `PageHeading`/resource key, and the editable-control mismatch is resolved by Heuristic 2's
fix above. Presentation is genuinely more consistent than `00` found it — `StaleIndicator` is one
component reused identically on `MyAccess` and `EventDetails`, differing only by a `SubjectKey`
parameter (`record.staleSubject` vs. `detail.staleSubject`); disclosure uses native
`<details>/<summary>` uniformly for both `ChangeRow` and every `Help` section, rather than a custom
widget on one screen and a different pattern on another.

**But the Gate 2 finding is, at its core, a consistency violation** — arguably the purest form one:
the same fact (`ch-005`'s status), on the same screen, rendered two different, contradictory ways at
once. `ForeseeableBadge` says "Not decided yet"; two inches above it, the headline says "Access
withdrawn." `00`'s own heuristic-4 rule is exactly this: *"users should not have to wonder whether
different words... mean the same thing."* Here they don't have to wonder — the app tells them two
different things about the identical fact in the same render. One cause, not five scattered ones,
but severe enough on its own to hold this heuristic at Fails rather than let the surrounding
consistency wins carry it to Partial.

**Worth stating once, since `00` made the same observation about its own findings:** this
heuristic's failure and Heuristic 1's partial status trace to the *same* underlying gap
(`StatusFor`/`status` ignoring `Urgency`). A single fix at that one point would very likely move
both scores — the same clustering pattern `00` found in `EventCard`, on a much smaller surface.

---

## 5. Error prevention

**`00`:** Partial — strong route constraints and form validation undermined by the one preventable
error the design didn't remove (an unsaveable editable field).

**`04`: Passes.**

The strengths `00` credited are intact and unchanged: `{Id:int}` route constraints
(`EventDetails.razor:20`, `Registration.razor:20`), `OnValidSubmit`-gated forms, data-annotation
validation. The one thing `00` faulted — an editable-looking control inviting a mistake the design
could have removed — is gone per Heuristic 2 above. One new, well-reasoned decision worth noting:
`SignIn.razor`'s header comment explicitly declines to adopt a reference implementation's identifier
blocklist, because it "rejects apostrophes and the substring ' or ', which turns away O'Neill,
D'Angelo and Ba'ath" — a case of *not* adding a check that would itself have introduced an error
class. Genuine error-prevention reasoning, not just a checklist item ticked.

---

## 6. Recognition rather than recall

**`00`:** Fails — unlabelled fields distinguishable only by content, an attendee count visible on
only one of two screens that needed it.

**`04`: Passes.**

`EventCard` carries real `<label for=...>` elements for every field, `for`/`id` paired, even in its
(now non-default) editable mode (`EventCard.razor:10,18,26`) — nothing is left to infer from
placeholder text alone. `StaleIndicator`'s always-on rendering means a person never has to recall
when they last checked — the age is stated every time. `ChangeRow` keeps a superseded value visible
beside its replacement specifically so a reader doesn't have to remember the old value to recognise
that something changed (`ChangeRow.razor`, header comment: "a correction that hides what it
corrected leaves the reader unable to tell whether anything actually moved"). The persistent
"signed in as" block in the nav (`NavMenu.razor:20-27`) exists for exactly the recognition problem
two comparable demo records create — "which record am I looking at" is answered on every screen
without being asked, per that block's own header comment. `/matches`' search and filters
(`EventList.razor:41-71`) remove the need to scroll-and-remember across fifty items.

---

## 7. Flexibility and efficiency of use

**`00`:** Fails — fifty events as one flat list, no search, no filter, no sort, no paging, and a
roughly 250-element keyboard path with no skip link.

**`04`: Passes.**

A real-time search box, a phase/group filter, and a played/not-yet-played status filter all narrow
`FilteredFixtures` before it ever renders (`EventList.razor:41-71,183-184`), backed by
`FixtureQuery.Apply` — the extraction the v10 (4B-R) run delivered specifically for this. Results
are paginated (`TotalPages`/`PageSize`, `EventList.razor:186-189`). The keyboard-path problem is
independently resolved by Heuristic 2's read-only-by-default `EventCard`: a list card now carries
two links and no inputs, not three inputs and two links, cutting the focusable-element count per
card roughly in half before pagination is even counted. A skip link is present
(`MainLayout.razor:13`, `href="#main-content"`) — the exact gap `00` named (UX-MAJ-07) is closed.

---

## 8. Aesthetic and minimalist design

**`00`:** Partial — restrained pages undercut by unused sidebar scaffolding, a documentation link
with no relevant content, and visual noise from the editable-everywhere decision.

**`04`: Passes.**

The sidebar `00` found holding one link now holds three real destinations plus session identity,
language, theme, and sign-out — no unused scaffolding remains, and nothing points at irrelevant
content. The two-layer disclosure pattern (`ChangeRow`, `Help`) is a minimalism decision made
explicit in the code itself: "the collapsed layer is still fully informative... what the second
layer holds is detail a person standing at a barrier does not need before they need it"
(`ChangeRow.razor`, header comment). That is close to a textbook statement of this heuristic's own
principle, applied deliberately rather than incidentally. The editable-everywhere noise `00` flagged
is gone per Heuristic 2.

---

## 9. Help users recognise, diagnose, and recover from errors

**`00`:** Partial — well-written messages undercut by missing field associations, low-contrast
validation text, and a not-found state that looked too much like success.

**`04`: Passes** (contrast itself deferred to Gate 4, per this file's opening note).

Field-level errors now carry both `role="alert"` and `aria-live="polite"`, with `id`-based
association to their inputs (`RequestAccessForm.razor:36,54`) — the exact association gap `00`
flagged (UX-MAJ-03) is closed. `MyAccess.razor` distinguishes its failure states precisely rather
than collapsing them: "no record of you" (`record.emptyHeading`) and "nothing has changed" are
worded and styled distinctly on purpose, per the component's own comment — "a person who confuses
them will wait for a notification that is never coming." No message claims a cause the app cannot
know: `common.noMatchNumber` reads simply "No match has this number," and `record.errorBody2`
gives a constructive, actionable instruction ("take your credential to the accreditation desk")
rather than a guess. `00`'s not-found-looks-like-success complaint (UX-MIN-04) doesn't recur:
`NotFound.razor` renders a distinct `notFound.title`/`notFound.body` with no shared heading path to
a success state.

---

## 10. Help and documentation

**`00`:** Passes — strong repository documentation, with the one in-app link pointing at framework
documentation instead, excused because the audience was a reviewer, not an end user.

**`04`: Passes — the same excused gap is now actually closed for a real end user, not just no
longer counted against the score.**

`/help` is a first-class nav destination (`NavMenu.razor:53-57`), not a repository file — eight
independently collapsible sections covering what the service doesn't do, what won't reach a holder
as a notification, and who to contact (`Help.razor`, per root README's own screen table). The escalation/contact
section is fully static and reachable with no network, by design, since it is the terminus of the
offline path through the app (`Help.razor`, header comment) — directly serving Task 3's
worst-case scenario. `00` had already declined to penalise the gap this closes; the closure is
recorded here as a genuine improvement rather than a debt that was merely forgiven.

---

## What this pattern suggests

`00` found six of ten weak results tracing to one decision (`EventCard`-as-editable-form). `04`'s
pattern is more distributed on the *positive* side — no single structural choice explains seven
heuristics moving to Passes; it is a set of independent investments (disclosure, `StaleIndicator`,
search/filter, read-only-by-default cards, in-app Help) each closing a different `00` gap. That is
still encouraging, if less tidy a story: it says the build responded to the *range* of `00`'s
findings rather than one lucky structural fix cascading.

**The two remaining Fails, though, do cluster the way `00`'s did.** Heuristic 1's partial status and
Heuristic 4's Fails trace to the same single gap — `StatusFor`/`status` ignoring `Urgency` — and a
fix at that one point would plausibly move both. Heuristic 3 is a different, standalone gap
(Withdrawal has no UI), open since v9 and not obviously in scope for a remediation pass bounded to
"what 4E already built" (`00_SCOPE.md` §6, carried into `07_REMEDIATION-SCOPE.md`'s own
constraint) — flagged here rather than resolved, since resolving it is Gate 7's call, not this
gate's.

---

✅ **GATE 3 COMPLETE** — `03_HEURISTIC-EVALUATION.md`
