# Heuristic Evaluation

EventEase assessed against Jakob Nielsen's ten usability heuristics — the most widely used
checklist in interface design, published in 1994 and still standard.

A *heuristic* here just means a rule of thumb: a general principle good interfaces tend to follow.
The point of checking against them is to catch problems by reasoning about the design, without
needing to recruit users first.

**Audited version:** commit `0653b4e`
**Finding IDs** refer to [`findings-register.md`](findings-register.md).

---

## At a glance

| # | Heuristic | Assessment |
|---|---|---|
| 1 | Visibility of system status | Partial |
| 2 | Match between system and the real world | Fails |
| 3 | User control and freedom | Fails |
| 4 | Consistency and standards | Fails |
| 5 | Error prevention | Partial |
| 6 | Recognition rather than recall | Fails |
| 7 | Flexibility and efficiency of use | Fails |
| 8 | Aesthetic and minimalist design | Partial |
| 9 | Help users recognise and recover from errors | Partial |
| 10 | Help and documentation | Passes |

---

## 1. Visibility of system status
*The system should always keep users informed about what is going on, through appropriate feedback
within reasonable time.*

**Partial.**

What works: registering produces a visible confirmation naming the event
(`Registration.razor:20`). The "Registered" badge follows the user to the list and details pages
(`EventList.razor:19–22`, `EventDetails.razor:17–20`) because both trackers are singletons
(`Program.cs:16–17`). The app also shows a loading indicator while the WebAssembly runtime starts
(`index.html:19–23`) — easy to skip, and it is here.

What does not: none of that status reaches assistive technology. The confirmation is a plain
paragraph with no live region (**UX-MAJ-05**), so a screen reader user submits the form and hears
nothing at all while the form disappears.

And away from the home route, the app cannot answer "where am I?". No nav item is active, there is
no breadcrumb, and every event's details page is titled "Event Details" in the browser tab
(**UX-MAJ-11**). With several tabs open they are indistinguishable.

The attendee count also misreports: "1 people registered" (**UX-MIN-01**).

## 2. Match between system and the real world
*Speak the users' language. Follow real-world conventions.*

**Fails.**

The core problem is **UX-C-01**. In the real world, browsing events and editing an event are
different activities done by different people. EventEase collapses them: every event, everywhere,
appears as three editable text boxes. The home page of an event-browsing app looks like a
spreadsheet import screen.

"About" in the top bar (**UX-MAJ-12**) is the second mismatch. In an event management product it
should describe EventEase. It opens ASP.NET Core framework documentation instead — the language of
the tool used to build the app, surfacing in the app's own interface.

## 3. User control and freedom
*Users need a clearly marked "emergency exit" to leave an unwanted state.*

**Fails.**

Registration is the only irreversible action in the app, and it cannot be undone
(**UX-MAJ-06**). `SessionTracker` offers `Register` but no counterpart. Once submitted, the
registration page shows a confirmation for the rest of the session and the badge is permanent.

This compounds badly with **UX-C-01**: when all 50 events look identical, registering for the wrong
one is easy — and once done, there is no exit.

"Back to events" is present on every page, which is the right instinct, but the source indicates
those links leave the application entirely on the deployed site (**UX-C-04**).

## 4. Consistency and standards
*Users should not have to wonder whether different words, situations, or actions mean the same thing.*

**Fails.**

- Internal links are inconsistent: `events/{id}` and `register/{id}` are base-relative, but all
  three "Back to events" links are root-absolute (**UX-C-04**).
- The same screen is called "Events" in the tab and "Upcoming Events" in the heading
  (**UX-MIN-03**).
- Attendee counts appear on the details page but not the list (**UX-MIN-02**).
- A link that opens a new tab gives no sign that it will (**UX-MAJ-12**).
- Most consequentially, an editable-looking control is the standard signal for "you can change
  this". Here it is not true anywhere (**UX-C-01**, **UX-C-02**).

## 5. Error prevention
*Even better than good error messages is a careful design which prevents a problem occurring.*

**Partial.**

Genuinely good work here. The route constraint `{Id:int}` stops a malformed ID from ever reaching
page code (`EventDetails.razor:4`). `OnValidSubmit` means an invalid form cannot reach the handler
(`Registration.razor:27`). Duplicate registration is structurally impossible because the form is
replaced once you are registered (`Registration.razor:18–21`). Validation rules live on the model
as data annotations rather than scattered hand-written checks (`RegistrationModel.cs:10–14`).

Against that: the largest preventable error in the app is not prevented at all. Offering an edit
box for data that cannot be saved invites a mistake the design could simply have removed
(**UX-C-01**, **UX-C-02**).

## 6. Recognition rather than recall
*Minimise memory load. Make objects, actions, and options visible.*

**Fails.**

Because field names are carried only by placeholders, and placeholders vanish once a field has a
value (**UX-MAJ-02**), the user sees three unlabelled boxes containing text. Which one is the
location? You work it out from the content. The date field has no name at all (**UX-MAJ-01**).

To compare two events, the user must open one, remember it, go back, and open the other — the
attendee count exists only on the details page (**UX-MIN-02**), and there is no way to see two
events side by side.

## 7. Flexibility and efficiency of use
*Accelerators may speed up interaction for the expert user.*

**Fails.**

Fifty events, presented as one flat list in fixed order, with no search, no filter, no sort, and no
paging (**UX-MAJ-08**). There is no fast path to a known event — only scrolling.

For keyboard users the position is worse than slow. With every card rendering three inputs and two
links, reaching the last event means passing roughly 250 focusable elements, and there is no skip
link (**UX-MAJ-07**).

## 8. Aesthetic and minimalist design
*Interfaces should not contain information which is irrelevant or rarely needed.*

**Partial.**

The pages themselves are restrained — no clutter, no competing calls to action, a clear single
heading per page. Colour use is limited and mostly principled.

But the app carries visible scaffolding it does not use: a 250px sidebar holding one link
(**UX-MIN-07**), and a top bar whose only content links to framework documentation
(**UX-MAJ-12**). And the editable-fields decision (**UX-C-01**) adds substantial visual noise to
what should be a simple browsing list — three form controls per event where three lines of text
would do.

## 9. Help users recognise, diagnose, and recover from errors
*Error messages should be in plain language, precisely indicate the problem, and constructively
suggest a solution.*

**Partial.**

The messages themselves are well written. "Enter a valid email address." says what is wrong and
what to do. "No event matches this ID." is clear. Two structurally different failures — an address
matching nothing, and an address naming a nonexistent event — are handled separately rather than
collapsed into one generic page, which shows real care (`EventDetails.razor:24–30`).

The problems are in delivery, not wording:

- Errors are not programmatically tied to their fields (**UX-MAJ-03**), so assistive technology
  cannot connect a message to the input it describes.
- Validation message text falls below the minimum contrast requirement (**UX-MAJ-09**) — the text
  most important to read is the hardest to read.
- The not-found state still shows the ordinary heading and tab title, so failure looks much like
  success (**UX-MIN-04**).
- "It may have been removed" claims a cause the app has no way of knowing (**UX-MIN-04**).

## 10. Help and documentation
*It may be necessary to provide documentation. Any such information should be easy to search and
focused on the user's task.*

**Passes.**

Unusually strong, and worth saying plainly. The repository carries a plain-language walkthrough in
`learning-mode/`, a glossary of front-end terms, a setup guide, a how-to-run guide covering three
different ways to run the app, and a README for every folder.

The one gap is that none of it is reachable *from inside the app* — the only in-app link points off
to vendor documentation (**UX-MAJ-12**). Given this is a coursework project whose documentation
audience is a reviewer reading the repository rather than an end user inside the app, that is a
reasonable place to land, and it is not counted against this heuristic.

---

## What this pattern suggests

The failures cluster. Six of the ten weak results — heuristics 2, 3, 4, 5, 6, and 8 — trace back to
one decision: `EventCard` having only an editable form as its presentation.

That is encouraging rather than alarming. It means the interface is not broadly poorly designed. It
means one structural choice, made once and reused everywhere, propagated into six different kinds
of usability problem. Giving the component a read-only presentation — while keeping the two-way
binding the assignment requires — resolves or reduces the majority of them at a single point.

The accessibility failures are more distributed and need individual attention; those are set out in
[`accessibility-audit.md`](accessibility-audit.md).
