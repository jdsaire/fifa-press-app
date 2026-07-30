# Remediation Scope

The fixes this audit calls for, sequenced into commit-sized units.

This is the hand-off artefact: the next development run executes this list. Each entry names the
findings it closes, the files it touches, and a proposed commit message matching the conventional
style already used in this repository's history.

**Derived from:** [`findings-register.md`](findings-register.md) at commit `0653b4e`

---

## Two constraints that bound every item below

**1. The scope ceiling.** This project is a Blazor WebAssembly app with mock in-memory data, no
authentication, no backend, and no database. Every fix listed here stays inside that. Anything
requiring a breach is in the [out of scope](#out-of-scope) section at the end — recorded, not
smuggled into the actionable list.

**2. Two-way data binding must survive.** Criterion 2 of the assignment rubric explicitly requires
the Event Card component to demonstrate fields with two-way data binding. **UX-C-01** asks for the
card to stop being editable everywhere — this must be implemented as an *added read-only
presentation mode*, with the binding retained and still demonstrated. Removing the binding would
close a usability finding at the cost of a graded requirement. This is the single most important
instruction in this document.

---

## Sequence

Ordered by severity first, then by file locality so related edits land together. Dependencies are
noted where one commit assumes another.

### Batch 1 — Critical

| # | Change | Closes | Files | Proposed commit |
|---|---|---|---|---|
| 1 | Add a read-only display mode to `EventCard`, defaulting to read-only. Render name as a heading, date as a `<time>` element, location as text. Keep all three `[Parameter]` / `EventCallback` pairs intact and keep an editable mode available so two-way binding remains demonstrated. | UX-C-01, UX-C-02 (partial), UX-MAJ-02 (partial) | `Components/EventCard.razor` | `feat(ux): add read-only display mode to EventCard` |
| 2 | Point the list, details and registration pages at the read-only mode. Registration keeps showing the event being registered for, as confirmation context rather than an editable form. | UX-C-01, UX-C-02 | `Pages/EventList.razor`, `Pages/EventDetails.razor`, `Pages/Registration.razor` | `fix(ux): show events as content rather than editable forms` |
| 3 | Associate every label with its control in the registration form: explicit `id` on each `InputText`, matching `for` on each `<label>`. | UX-C-03 | `Pages/Registration.razor` | `fix(a11y): associate registration form labels with their inputs` |
| 4 | Replace the three root-absolute `href="/"` back-links with base-relative `href=""`, matching every other link in the app. | UX-C-04 | `Pages/EventDetails.razor`, `Pages/Registration.razor`, `Pages/NotFound.razor` | `fix(routing): use base-relative links for back navigation` |

**Depends on:** items 1 and 2 are a pair — 2 is meaningless without 1, and 1 alone changes nothing
visible. They are kept separate so the component change and the page changes are individually
reviewable, but they should land together.

**Verify before item 4:** the live-site behaviour described in **UX-C-04** is REASONED, not
observed. One click on the deployed "Back to events" button confirms it. Worth doing before the
fix, so the completion report can state the behaviour rather than the inference.

### Batch 2 — Major, accessibility

| # | Change | Closes | Files | Proposed commit |
|---|---|---|---|---|
| 5 | Add visible labels to the editable mode of `EventCard`, each tied to its input by `for`/`id`, including the date field. | UX-MAJ-01, UX-MAJ-02 | `Components/EventCard.razor` | `fix(a11y): label every EventCard input` |
| 6 | Tie inline error messages to their inputs with `aria-describedby`, and mark them as alerts so they are announced. | UX-MAJ-03 | `Components/EventCard.razor` | `fix(a11y): announce EventCard validation errors` |
| 7 | Replace `outline: none` on the focus target with a visible focus indicator. | UX-MAJ-04 | `wwwroot/css/app.css` | `fix(a11y): restore a visible focus indicator` |
| 8 | Mark the registration confirmation as a status message and move focus to it on success. | UX-MAJ-05 | `Pages/Registration.razor` | `feat(a11y): announce successful registration` |
| 9 | Add a skip link to the layout, targeting the main content region. | UX-MAJ-07 | `Layout/MainLayout.razor`, `wwwroot/css/app.css` | `feat(a11y): add skip-to-content link` |
| 10 | Raise validation message and invalid-outline colour to meet the 4.5:1 minimum. | UX-MAJ-09 | `wwwroot/css/app.css` | `fix(a11y): raise validation message contrast to AA` |
| 11 | Add `aria-expanded` and `aria-controls` to the navigation toggle, driven by the existing collapse state. | UX-MAJ-10 | `Layout/NavMenu.razor` | `fix(a11y): expose navigation toggle state` |

**Depends on:** item 5 assumes item 1 has introduced the editable/read-only split.

### Batch 3 — Major, interaction

| # | Change | Closes | Files | Proposed commit |
|---|---|---|---|---|
| 12 | Add an un-register path to `SessionTracker` and surface a cancel action on the registration page. Keep `AttendanceTracker` consistent so the count reflects the cancellation. | UX-MAJ-06 | `Services/SessionTracker.cs`, `Services/AttendanceTracker.cs`, `Pages/Registration.razor` | `feat(ux): allow cancelling a registration` |
| 13 | Paginate the event list at 10 events per page, with page-number navigation (5 pages for the current 50-event dataset) — a numbered-results pattern rather than infinite scroll or a single long list. Add a search box filtering by name and location, applied before pagination so search results are also paginated. | UX-MAJ-08 | `Pages/EventList.razor` | `feat(ux): paginate the event list and add search` |
| 14 | Name the event in the page heading and browser title on the details and registration pages; add a breadcrumb trail back to the list. | UX-MAJ-11 | `Pages/EventDetails.razor`, `Pages/Registration.razor` | `feat(ux): identify the current event in headings and titles` |
| 15 | Replace the template "About" link with an app-relevant destination, or remove it. If any external link remains, signpost the new tab and add `rel="noopener noreferrer"`. | UX-MAJ-12 | `Layout/MainLayout.razor` | `fix(ux): replace template About link with app navigation` |

**Note on item 13, revised.** The original finding (**UX-MAJ-08**) was scoped narrowly, as a
*findability* problem — no way to locate a known event. Re-reading the rubric confirms the 50-event
count is not a grading requirement; it is `MockEventData.cs`'s own implementation choice, made to
give Activity 2's rendering-optimization work "a large-enough list to matter" (see the comment at
`Models/MockEventData.cs:23`). The deeper problem findability sat on top of is a *page-length*
problem: the full unbounded roster renders as one long scroll regardless of dataset size.

Pagination is the more complete fix and should be the primary mechanic, with search layered on top
rather than standing alone. It bounds what renders on screen — 10 cards, not 50 — which directly
reduces the ~250-element keyboard tab chain behind **UX-MAJ-07** as a side effect, something search
alone would not do. `Microsoft.AspNetCore.Components.Web.Virtualization`, already imported at
`_Imports.razor:7` and currently unused, remains an optional complementary technique if the
dataset grows further, but pagination alone is sufficient at the current scale and is the pattern
this item specifies.

### Batch 4 — Minor

| # | Change | Closes | Files | Proposed commit |
|---|---|---|---|---|
| 16 | Correct attendee-count pluralisation and show the count on the list as well as the details page. | UX-MIN-01, UX-MIN-02 | `Pages/EventDetails.razor`, `Pages/EventList.razor` | `fix(ux): correct attendee count wording and show it consistently` |
| 17 | Align tab title with heading; change the not-found heading, title and copy so a failed lookup is distinguishable and does not assert an unknowable cause; add an empty-state message. | UX-MIN-03, UX-MIN-04, UX-MIN-09 | `Pages/EventList.razor`, `Pages/EventDetails.razor`, `Pages/Registration.razor` | `fix(ux): clarify page titles and not-found messaging` |
| 18 | Constrain content width for readable line length; remove the redundant click handler from the non-interactive `<div>`; reconsider sidebar width now it holds one link. | UX-MIN-05, UX-MIN-06, UX-MIN-07 | `Layout/MainLayout.razor.css`, `Layout/NavMenu.razor` | `fix(ux): constrain content width and tidy layout scaffolding` |
| 19 | Add a meta description; remove the unused import if virtualization was not adopted in item 13. | UX-MIN-08, UX-MIN-10 | `wwwroot/index.html`, `_Imports.razor` | `chore: add meta description and remove unused import` |

---

## Summary

| Batch | Commits | Findings closed |
|---|---|---|
| 1 — Critical | 4 | 4 critical, 1 major partially |
| 2 — Major, accessibility | 7 | 7 major |
| 3 — Major, interaction | 4 | 4 major |
| 4 — Minor | 4 | 10 minor |
| **Total** | **19** | **26** |

Nineteen commits, of which the first four carry most of the weight. Items 1 and 2 alone resolve or
substantially reduce problems recorded against six of Nielsen's ten heuristics, because the same
structural decision was producing all of them.

**This is an implementation run, not a documentation pass.** Batches 1 and 2 change component
contracts, page markup and stylesheet rules across nine files. It should be planned and gated
accordingly.

---

## Verification the next run should perform

- `dotnet build` and `dotnet run --project src/EventEase` succeed with zero errors and zero
  warnings, as in previous runs.
- Two-way data binding on `EventCard` still works and is still demonstrated — confirm against
  rubric criterion 2 specifically, since item 1 touches exactly that surface.
- Every finding ID in this document is either closed or explicitly carried forward with a reason.
- No new external dependency or package added.
- Re-run the link check across all tracked markdown; the previous run verified 135 links clean and
  this run adds a folder of new files.

---

## Out of scope

Recorded because the audit surfaced them, excluded because acting on them would breach the
project's ceiling. These are not deferred work items — they are outside what this project is
permitted to be.

| Item | Why it is excluded |
|---|---|
| Persisting event edits | Requires storage. The app is explicitly mock and in-memory |
| Email confirmation of registration | Requires a backend and a mail service |
| Real attendee data instead of a session counter | Requires shared storage across users |
| User accounts, saved preferences, registration history | Requires authentication, explicitly excluded |
| Measuring satisfaction or loyalty | Requires real users over time; deliberately excluded from this audit's scope |
| Server-side rendering to improve first load | Changes the rendering model away from WebAssembly |

---

## What still needs a person

Nine checks in [`protocol-results.md`](protocol-results.md) could not be closed without running the
app in a browser. Three of them — screen-reader completion of the registration journey, the
keyboard tab route, and rendered contrast — bear directly on fixes in Batch 2, and are the natural
way to confirm those fixes actually worked rather than merely landed.
