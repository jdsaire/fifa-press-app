# Findings Register

*(Findings recorded under the app's original name, EventEase, including its then source folder
`src/EventEase/` — since renamed to the FIFA Press App / `src/FifaPressApp/` in v7. Every
`src/EventEase/...` path citation below is preserved byte-exact as historical record.)*

The canonical list of every issue found in the EventEase interface, with a stable ID for each.
Every other file in this folder refers back to these IDs.

Each finding cites the file and line where it can be seen. Nothing here is a guess about how
Blazor apps usually behave — if it is listed, it is in the source.

**Audited version:** commit `0653b4e`
**Method legend:** see [`README.md`](README.md)

---

## How severity was decided

The bands were defined before anything was classified, so that severity is applied against a
fixed rule rather than assigned by impression:

| Band | Rule |
|---|---|
| **Critical** | A user in a foreseeable situation either cannot finish a main task, or finishes it believing something happened that did not. Also covers shutting out an entire way of using the app, such as keyboard-only or screen-reader use. |
| **Major** | The task can be finished, but with friction or confusion that happens every single time — or an accessibility barrier that makes access harder without fully blocking it. |
| **Minor** | Cosmetic, inconsistent, or rough around the edges. There is an obvious workaround and the user is unlikely to be misled. |

Where a call was genuinely borderline, it was placed in the **lower** band and the reasoning
stated. Inflating severity would make this register less trustworthy, not more impressive.

---

## Summary

| Band | Count |
|---|---|
| Critical | 4 |
| Major | 12 |
| Minor | 10 |
| **Total** | **26** |

---

## Critical

### UX-C-01 — Every event is shown as an editable form, even when just browsing
**Where:** `src/EventEase/Components/EventCard.razor:3–23` · used at `Pages/EventList.razor:18`, `Pages/EventDetails.razor:16`, `Pages/Registration.razor:16`
**Heuristic:** Match between system and the real world · Recognition rather than recall
**Method:** CODE-VERIFIED

`EventCard` has exactly one appearance: three editable text boxes. It is the only way any event is
ever displayed, so the same editable form is used on the browsing page, the details page, and the
registration confirmation page.

What the user experiences: the home page is a wall of roughly 50 events, each rendered as three
text inputs — around 150 editable boxes — before any of them has a visible name saying what it is.
Nothing on the page reads as a list of events to browse. It reads as a bulk data-entry screen.

Two consequences compound this:

- **It promises something the app cannot do.** An editable box is an invitation to edit. There is
  no save button, no confirmation, and no storage behind it (see UX-C-02).
- **It destroys the page's structure.** The event name is an `<input>`, not a heading, so the
  entire list has no heading structure at all. Someone navigating by headings with a screen reader
  finds only "Upcoming Events" and then nothing — no way to move between events.

**Important for the fix:** two-way data binding on this component is a graded requirement of the
assignment. The remediation must *keep* the binding and keep demonstrating it — it must add a
read-only presentation mode, not remove the editing capability. See
[`remediation-scope.md`](remediation-scope.md).

---

### UX-C-02 — Edits are silently thrown away
**Where:** `Pages/EventList.razor:29` · `Pages/EventDetails.razor:42` · `Pages/Registration.razor:60`
**Heuristic:** Visibility of system status · Error prevention
**Method:** CODE-VERIFIED

`EventList` builds its list once, in a field initialiser. `EventDetails` and `Registration` each
call `MockEventData.GetSampleEvents()` again in `OnParametersSet`, which returns a brand-new list
built from scratch every time.

So if a user changes an event's name on the home page and then opens that event's details, the
details page shows the original name. Navigating back to the home page discards the edit too, since
the component is rebuilt.

What the user experiences: they type a change, it appears to take, they navigate away, and it is
gone. No warning, no prompt, no error. The interface accepted the input and quietly discarded it —
which is the definition of a user believing something happened that did not.

This is inseparable from UX-C-01: if the card were not editable when browsing, there would be
nothing to lose. Fixing UX-C-01 largely resolves this one.

---

### UX-C-03 — The registration form's fields have no usable name for assistive technology
**Where:** `Pages/Registration.razor:31–38`
**WCAG:** 1.3.1 Info and Relationships (A) · 4.1.2 Name, Role, Value (A) · 3.3.2 Labels or Instructions (A)
**Method:** CODE-VERIFIED

```
<label>Name</label>
<InputText class="form-control" @bind-Value="registration.Name" />
```

The `<label>` has no `for` attribute and does not wrap its input. Blazor's `InputText` does not
generate an `id` on its own, so there is nothing for a label to point at. The two elements sit
next to each other visually but are not connected in any way a browser or screen reader
understands.

What the user experiences: a sighted user reads the label above the box and is fine. A screen
reader user hears "edit text, blank" twice, with no indication which is the name and which is the
email. Clicking the label — which normally focuses the field — does nothing.

Registration is the app's single conversion action. Reaching it without being able to tell the two
required fields apart means the primary task is effectively closed to screen reader users, which
is why this sits in the critical band rather than major.

---

### UX-C-04 — "Back to events" leaves the app entirely on the deployed site
**Where:** `Pages/EventDetails.razor:32` · `Pages/Registration.razor:49` · `Pages/NotFound.razor:11`
**Heuristic:** User control and freedom · Consistency and standards
**Method:** REASONED — **flagged for human confirmation**, see [`protocol-results.md`](protocol-results.md)

All three "Back to events" buttons use `href="/"`. Every other link in the app is base-relative:
`href="events/{id}"`, `href="register/{id}"`, `href=""`.

A leading slash means "the root of this domain" and deliberately ignores `<base href>`. On GitHub
Pages the app is served from a subfolder — `.github/workflows/deploy-pages.yml:37–39` rewrites the
base to `/frontend_c4_blazor_eventease/` at publish time — so `/` does not point at the app. It
points at the domain root, one level above it.

What the user expects to happen: they finish registering, click "Back to events", and return to the
event list. What the source indicates will happen instead: they are taken out of EventEase
altogether. Running locally this never appears, because locally the base really is `/`.

This is the clearest example of why a rendered check still matters. The reasoning is
straightforward and the inconsistency in the source is real and visible — every other link in the
app is relative and these three are not — but confirming the exact browser behaviour needs someone
to click the button on the live site. It is recorded as critical because if it behaves as the
source indicates, a main navigation path exits the application; it is marked REASONED, not
CODE-VERIFIED, because that last step was not performed.

---

## Major

### UX-MAJ-01 — The date field has no name at all
**Where:** `Components/EventCard.razor:11`
**WCAG:** 4.1.2 Name, Role, Value (A) · 3.3.2 Labels or Instructions (A) · **Method:** CODE-VERIFIED

The name and location inputs at least carry a `placeholder`. The date input has no placeholder, no
label, no `aria-label`, and no `title`. It is announced as an unnamed date field. A sighted user
can infer it from the date format; a screen reader user gets nothing.

### UX-MAJ-02 — Placeholders are used instead of labels, and vanish exactly when needed
**Where:** `Components/EventCard.razor:5, 17`
**WCAG:** 3.3.2 Labels or Instructions (A) · **Method:** CODE-VERIFIED

`placeholder="Event name"` and `placeholder="Location"` are the only naming these fields have. A
placeholder disappears as soon as the box contains a value — and here the boxes are always
populated from the event data. The result is that the placeholder text is never actually visible in
normal use. The user sees three unlabelled boxes containing text like "Grand Convention Center,
Austin" with nothing saying what that text represents.

### UX-MAJ-03 — Error messages are not connected to the fields they describe
**Where:** `Components/EventCard.razor:6–9, 12–15, 18–21`
**WCAG:** 3.3.1 Error Identification (A) · **Method:** CODE-VERIFIED

Errors render as plain `<div class="text-danger small">`. There is no `aria-describedby` linking
the message to its input, no `role="alert"`, and no live region. Visually the message appears under
the right box; programmatically it is an unrelated piece of text. A screen reader user clearing a
required field is told nothing, and if they navigate back to the field the error is not read out
with it.

### UX-MAJ-04 — The focus outline is switched off on the element that receives focus
**Where:** `wwwroot/css/app.css:5–7` · interacts with `App.razor:8`
**WCAG:** 2.4.7 Focus Visible (AA) · **Method:** CODE-VERIFIED

`App.razor:8` uses `<FocusOnNavigate Selector="h1" />`, which deliberately moves keyboard focus to
the page heading after every navigation — a genuinely good accessibility practice. `app.css` then
sets `h1:focus { outline: none; }`, removing the visible indicator.

The two cancel out. After every navigation, a keyboard user's focus is sitting on the heading with
no visual sign of where it is. This is inherited from the project template, but it is in the app's
own stylesheet and it defeats the app's own deliberate focus management.

### UX-MAJ-05 — Successful registration is not announced, and focus is not moved
**Where:** `Pages/Registration.razor:18–21, 67–71` · **WCAG:** 4.1.3 Status Messages (AA)
**Heuristic:** Visibility of system status · **Method:** CODE-VERIFIED

On success the form is replaced by "You're registered for X!". The message is real and visible,
which is good. But it is a plain `<p>` with no `role="status"` or live region, and focus is left
on the now-removed submit button.

A screen reader user presses Register and hears silence. The form has disappeared from under their
cursor and nothing has told them why. They must go hunting to find out whether it worked.

### UX-MAJ-06 — A registration cannot be undone
**Where:** `Services/SessionTracker.cs:12–14` · `Pages/Registration.razor:18–21`
**Heuristic:** User control and freedom · **Method:** CODE-VERIFIED

`SessionTracker` exposes `IsRegistered` and `Register`. There is no way to un-register. Once the
form is submitted, the registration page for that event shows only a confirmation message for the
rest of the session, and the "Registered" badge is permanent.

A user who registers for the wrong event — easy to do, given every card looks identical (UX-C-01) —
has no route back. Nielsen's "emergency exit" is entirely missing from the one irreversible action
in the app.

### UX-MAJ-07 — No way to skip past the form controls to reach the content
**Where:** `Layout/MainLayout.razor:1–16` (no skip link) · consequence of `Pages/EventList.razor:12–26`
**WCAG:** 2.4.1 Bypass Blocks (A) · **Method:** CODE-VERIFIED

There is no skip link anywhere in the layout. Combined with UX-C-01, a keyboard user landing on the
home page must tab through roughly 250 focusable elements — three inputs and two buttons for each
of about 50 events — to reach the last event. To reach the tenth event's Register button takes
around 50 presses of Tab.

Even after UX-C-01 is fixed, the list will still hold about 100 links, so a skip link remains
worthwhile on its own merits.

### UX-MAJ-08 — Fifty events, with no search, filter, sort, or paging
**Where:** `Pages/EventList.razor:12–26` · `Models/MockEventData.cs:12–36`
**Heuristic:** Flexibility and efficiency of use · **Method:** CODE-VERIFIED

`MockEventData` returns 50 events: 5 named ones and 45 generated "Regional Conference #n" entries.
All 50 render at once, in insertion order, with no way to search by name, filter by city or date,
sort, or page through them.

For an app whose stated purpose is browsing events, finding a specific one means scrolling and
reading. `Microsoft.AspNetCore.Components.Web.Virtualization` is already imported at
`_Imports.razor:7` but never used.

### UX-MAJ-09 — Validation message text fails the minimum contrast requirement
**Where:** `wwwroot/css/app.css:35–37` (and the related outline at `31–33`)
**WCAG:** 1.4.3 Contrast (Minimum) (AA) · **Method:** REASONED (calculated from declared values)

`.validation-message { color: red; }` is pure red, `#FF0000`. Against the white page background
that computes to a contrast ratio of **4.0:1**. The AA minimum for normal-size text is 4.5:1.

This is the class Blazor applies to the registration form's validation errors, so the text that
matters most — the message telling the user what went wrong — is the least readable text on the
page. Worth noting the contrast is close, not catastrophic; but it is below the line, and error
text is the wrong place to be below the line.

For contrast, the EventCard errors use Bootstrap's `text-danger` (`#dc3545`, about 4.53:1), which
passes. Only the `app.css` override fails.

### UX-MAJ-10 — The mobile menu button does not report whether the menu is open
**Where:** `Layout/NavMenu.razor:4–6, 10`
**WCAG:** 4.1.2 Name, Role, Value (A) · **Method:** CODE-VERIFIED

The toggle button has a `title`, which gives it a name, but no `aria-expanded` and no
`aria-controls`. Its open/closed state is carried purely by a CSS class swap
(`NavMenuCssClass`, lines 23 and 10). A screen reader user pressing it is not told whether they
just opened or closed anything.

### UX-MAJ-11 — On the details and registration pages, nothing indicates where you are
**Where:** `Layout/NavMenu.razor:13` · `Pages/EventDetails.razor:12` · `Pages/Registration.razor:11`
**Heuristic:** Visibility of system status · **Method:** CODE-VERIFIED

The only nav item is Home, with `Match="NavLinkMatch.All"`, so it is highlighted only on `/`. On
`/events/3` and `/register/3` no nav item is active, there is no breadcrumb, and the headings are
generic — "Event Details" and "Register for Event" — rather than naming the event.

The page title in the browser tab has the same problem: every event's details page is titled
"Event Details". A user with several tabs open cannot tell them apart, and the back-navigation
trail gives no sense of depth.

### UX-MAJ-12 — The only global link sends users to unrelated vendor documentation
**Where:** `Layout/MainLayout.razor:9`
**Heuristic:** Consistency and standards · Match between system and the real world
**Method:** CODE-VERIFIED

```
<a href="https://learn.microsoft.com/aspnet/core/" target="_blank">About</a>
```

This is leftover project-template content sitting in the app's persistent top bar, on every page.
A user clicking "About" in an event management app reasonably expects to learn about EventEase.
Instead a new tab opens on ASP.NET Core framework documentation.

Three separate problems in one line: the destination is wrong for the product, the new tab is not
announced to the user, and `target="_blank"` is used without `rel="noopener noreferrer"`.

---

## Minor

### UX-MIN-01 — "1 people registered"
`Pages/EventDetails.razor:21` · CODE-VERIFIED. The count is interpolated straight into a fixed
plural string, so a single attendee reads as "1 people registered". Also reads "0 people
registered" on every event initially, which looks like a data failure rather than an empty state.

### UX-MIN-02 — Attendee count appears on one page but not the other
`Pages/EventDetails.razor:21` vs `Pages/EventList.razor:12–26` · CODE-VERIFIED. The list gives no
sense of how busy an event is, so the user must open each one to compare.

### UX-MIN-03 — Browser tab says "Events", the heading says "Upcoming Events"
`Pages/EventList.razor:8` vs `:10` · CODE-VERIFIED. Small inconsistency in naming the same screen.

### UX-MIN-04 — Not-found copy is speculative, and the page still looks like a normal one
`Pages/EventDetails.razor:29` · `Pages/Registration.razor:46` · CODE-VERIFIED. "It may have been
removed" asserts a cause the app cannot know. The `<h1>` still reads "Event Details" and the page
title is unchanged, so the failed state looks much like a successful one.

### UX-MIN-05 — A click handler on a plain `<div>`
`Layout/NavMenu.razor:10` · CODE-VERIFIED. `@onclick` sits on a non-interactive `<div>` with no
role and no keyboard handling. Its purpose — closing the menu after a link is tapped — is
reachable another way for keyboard users, so this is not blocking, which is why it is minor rather
than major.

### UX-MIN-06 — Text lines run the full width of the window
`Layout/MainLayout.razor.css:73–76` · REASONED. `article` gets horizontal padding but no
`max-width`. On a wide monitor a line of text can stretch across the entire screen, which is
uncomfortable to read.

### UX-MIN-07 — A 250px sidebar holding one link
`Layout/MainLayout.razor.css:54–59` · `Layout/NavMenu.razor:12–16` · CODE-VERIFIED. The sidebar
takes a fixed 250px of every desktop screen to present a single "Home" link.

### UX-MIN-08 — No meta description
`wwwroot/index.html:4–15` · CODE-VERIFIED. Nothing describes the app when a link to it is shared or
indexed.

### UX-MIN-09 — No empty state for the event list
`Pages/EventList.razor:12–26` · CODE-VERIFIED. If the list were ever empty the page would show a
heading and nothing else. Not reachable with the current fixed mock data, so recorded as a
robustness gap rather than a live defect.

### UX-MIN-10 — Unused import
`_Imports.razor:7` · CODE-VERIFIED. `Virtualization` is imported but never used. Harmless, but it
points directly at the tool that would address UX-MAJ-08.

---

## What the app does well

An audit that lists only defects misrepresents what was built. These were verified in the same pass
and to the same standard as the findings above.

| # | Strength | Evidence |
|---|---|---|
| S-01 | Focus is deliberately moved to the page heading on navigation — a real accessibility practice, not a default | `App.razor:8` |
| S-02 | Every page sets a browser tab title | `EventList.razor:8`, `EventDetails.razor:10`, `Registration.razor:9`, `NotFound.razor:7` |
| S-03 | The page language is declared | `wwwroot/index.html:2` |
| S-04 | Two structurally different failures are handled separately: an address that matches nothing, and an address that is well-formed but names a nonexistent event | `App.razor:4` + `Pages/NotFound.razor` · `EventDetails.razor:24–30` |
| S-05 | The route constraint `{Id:int}` rejects non-numeric IDs before they reach page code | `EventDetails.razor:4`, `Registration.razor:3` |
| S-06 | Duplicate registration is prevented — the form is replaced once you are registered | `Registration.razor:18–21` |
| S-07 | List rendering is keyed by event ID rather than list position | `EventList.razor:17` |
| S-08 | Sidebar link contrast is approximately 9.8:1, comfortably above AA | `NavMenu.razor.css:50` on `MainLayout.razor.css:12` |
| S-09 | Primary button contrast is approximately 5.2:1, above AA | `app.css:13–17` |
| S-10 | Form validation rules live on the model as data annotations, and invalid submissions never reach the handler | `Models/RegistrationModel.cs:10–14` · `Registration.razor:27–29` |
| S-11 | The deploy rewrites the base path only in the published output, leaving local runs working | `.github/workflows/deploy-pages.yml:32–39` |
| S-12 | Session state survives navigation, so the "Registered" badge follows the user between pages | `Program.cs:16–17` |
