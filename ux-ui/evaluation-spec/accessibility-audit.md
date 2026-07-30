# Accessibility Audit

EventEase assessed against **WCAG 2.2 Level AA** — the Web Content Accessibility Guidelines
published by the W3C, and the level referenced by most accessibility legislation worldwide.

AA is the standard threshold. AAA exists but is not expected of general-purpose applications, and
holding a course project to it would be misleading rather than rigorous.

**Audited version:** commit `0653b4e`
**Finding IDs** refer to [`findings-register.md`](findings-register.md).

---

## Result

| Result | Count |
|---|---|
| Pass | 8 |
| Fail | 12 |
| Open — needs a human | 3 |

**The app does not currently meet WCAG 2.2 AA.**

Twelve failures is a meaningful number, but the shape matters more than the count: most trace back
to two root causes rather than twelve independent oversights. Form controls have no accessible
names, and content structure is carried by form fields instead of headings.

---

## Failures

### 1.3.1 Info and Relationships (Level A) — FAIL

*Structure and relationships conveyed visually must also be available programmatically.*

Two separate breaches.

**Labels are not connected to their inputs.** At `Registration.razor:31–38`:

```
<label>Name</label>
<InputText class="form-control" @bind-Value="registration.Name" />
```

The `<label>` has no `for` attribute and does not wrap the input, and Blazor's `InputText` does not
emit an `id` by itself. Visually they sit together; programmatically they are unrelated. (**UX-C-03**)

**The event list has no structure.** Each event's name is an `<input>` (`EventCard.razor:5`), not a
heading. So a page listing 50 events contains exactly one heading — "Upcoming Events" — and 50
unstructured groups of form fields. A screen reader user navigating by heading, which is the normal
way to move through a page, finds nothing beneath the top level. (**UX-C-01**)

---

### 1.4.3 Contrast (Minimum) (Level AA) — FAIL

*Text must have a contrast ratio of at least 4.5:1 against its background (3:1 for large text).*

`app.css:35–37` sets `.validation-message { color: red; }`. Pure red `#FF0000` on white computes
to **4.0:1** — below the 4.5:1 requirement.

This is the class Blazor applies to validation errors in the registration form, so the text a user
most needs to read when something has gone wrong is the least legible text on the page. The same
pure red is used for the invalid-field outline at `app.css:31–33`. (**UX-MAJ-09**)

The margin is small — 4.0 against 4.5 — and worth stating plainly rather than dramatising. But it
is below the line, and error text is the wrong place to sit below the line.

Elsewhere contrast is good: sidebar links compute to roughly 9.8:1, primary buttons to roughly
5.2:1, and the EventCard error text uses Bootstrap's `text-danger` at roughly 4.53:1, which passes.

---

### 2.4.1 Bypass Blocks (Level A) — FAIL

*A mechanism must exist to skip repeated blocks of content.*

There is no skip link anywhere in `MainLayout.razor`. Every page begins with the sidebar navigation
and top bar before reaching content.

The consequence is severe on the home page specifically. With each of ~50 cards contributing three
inputs and two links, a keyboard user faces roughly 250 focusable stops with no way past them.
(**UX-MAJ-07**)

---

### 2.4.7 Focus Visible (Level AA) — FAIL

*Any keyboard-operable interface must have a visible focus indicator.*

`app.css:5–7` sets `h1:focus { outline: none; }` with no replacement.

This interacts badly with something the app does deliberately well. `App.razor:8` uses
`<FocusOnNavigate Selector="h1" />` to move keyboard focus to the page heading after every
navigation — genuinely good practice. The stylesheet then makes that focus invisible. After every
page change, a keyboard user's focus is somewhere they cannot see. (**UX-MAJ-04**)

---

### 3.3.2 Labels or Instructions (Level A) — FAIL

*Labels or instructions must be provided when content requires user input.*

Three inputs in `EventCard.razor` and two in `Registration.razor` lack usable labels.

The date input at `EventCard.razor:11` is the clearest case: no label, no placeholder, no
`aria-label`, no `title`. Nothing names it at all. (**UX-MAJ-01**)

The name and location inputs carry only a `placeholder` (`EventCard.razor:5, 17`). A placeholder is
not a label — it disappears as soon as the field has a value, and here the fields are always
populated from event data, so the placeholder text is effectively never visible in normal use.
(**UX-MAJ-02**)

---

### 3.3.1 Error Identification (Level A) — FAIL

*If an input error is detected, the item in error must be identified and described in text.*

The description exists and is well written. The identification does not.

Errors render as plain `<div class="text-danger small">` at `EventCard.razor:6–9, 12–15, 18–21`
with no `aria-describedby` tying the message to its input and no `role="alert"`. A screen reader
user who empties a required field is told nothing, and returning to that field does not surface the
error. (**UX-MAJ-03**)

---

### 4.1.2 Name, Role, Value (Level A) — FAIL

*For all interface components, the name and role must be programmatically determinable, and states
must be communicated to assistive technology.*

Two breaches.

**Unnamed form controls** — the same evidence as 3.3.2 and 1.3.1. Five inputs across the app have
no accessible name. (**UX-C-03**, **UX-MAJ-01**, **UX-MAJ-02**)

**Unreported state.** The mobile navigation toggle at `NavMenu.razor:4` has a `title`, which gives
it a name, but no `aria-expanded` and no `aria-controls`. Its open/closed state lives entirely in a
CSS class swap (`NavMenu.razor:10, 23`). A screen reader user pressing it is not told what
happened. (**UX-MAJ-10**)

---

### 4.1.3 Status Messages (Level AA) — FAIL

*Status messages must be programmatically determinable through role or properties, so they can be
announced without receiving focus.*

Registering successfully replaces the form with "You're registered for X!"
(`Registration.razor:20`) — a plain `<p>` with no `role="status"` and no live region. Focus is left
on the submit button, which no longer exists.

A screen reader user completes the app's single conversion action and receives no confirmation of
any kind. (**UX-MAJ-05**)

---

## Passes

| Criterion | Level | Evidence |
|---|---|---|
| **1.3.2** Meaningful Sequence | A | DOM order matches visual order throughout; no CSS repositioning reorders content |
| **2.1.1** Keyboard | A | All primary actions are native `<button>` and `<a>` elements. The one `@onclick` on a `<div>` (`NavMenu.razor:10`) is a redundant convenience — the links inside remain keyboard-operable, so nothing is keyboard-inaccessible. Recorded as **UX-MIN-05** for tidiness, not as a barrier |
| **2.1.2** No Keyboard Trap | A | No modals, overlays, or focus-capturing components anywhere in the app |
| **2.4.2** Page Titled | A | `<PageTitle>` on all four pages. Titles are not *distinct* per event (**UX-MAJ-11**), but the criterion requires a title, which is met |
| **2.4.3** Focus Order | A | Order follows the document. Focus is explicitly managed on navigation (`App.razor:8`) — the indicator being hidden is a 2.4.7 failure, not a focus-order one |
| **3.1.1** Language of Page | A | `lang="en"` at `index.html:2` |
| **3.2.1** On Focus | A | No context change is triggered by focus alone |
| **3.2.2** On Input | A | Input updates values but never navigates or submits automatically |

---

## Open — cannot be settled without a browser

These three need someone to run the app. They are recorded as OPEN, not as passes.

| Criterion | Why it is open |
|---|---|
| **1.4.3** Contrast, as rendered | The failure above was calculated from declared colour values. Confirming against actual painted pixels — including any Bootstrap cascade this audit did not model — needs a real render |
| **1.4.10** Reflow | Confirming content reflows at 320px with no horizontal scrolling requires resizing a real viewport |
| **2.5.8** Target Size (Minimum) | Confirming interactive targets meet 24×24 CSS pixels requires measuring rendered elements |

---

## Where to start

The failures are not evenly weighted. In order of impact:

1. **Give every form control an accessible name.** This single change addresses 1.3.1, 3.3.2 and
   4.1.2 at once, and it is what currently makes the registration journey unusable with a screen
   reader. Highest value per unit of effort in the entire remediation.
2. **Give the event list real heading structure.** Falls out naturally from giving `EventCard` a
   read-only presentation, and restores the ability to navigate the page by heading.
3. **Restore the focus indicator.** A one-line change closing a AA failure.
4. **Associate errors with their fields and announce status changes.** Closes 3.3.1 and 4.1.3.
5. **Add a skip link.** Closes 2.4.1 and remains worthwhile even after the list shrinks.
6. **Raise validation message contrast.** A colour value change closing a AA failure.

Sequenced with commit messages in [`remediation-scope.md`](remediation-scope.md).

---

## A note on what this audit could not do

No screen reader was available. No browser rendered the app. Everything above was established by
reading source and calculating from declared values.

That method is reliable for what it covers — whether a label exists is not a matter of opinion, and
a contrast ratio computed from a hex value is arithmetic. But it cannot confirm how the app behaves
when actually operated. A screen reader pass and a keyboard walkthrough remain necessary, and are
listed among the open checks in [`protocol-results.md`](protocol-results.md).
