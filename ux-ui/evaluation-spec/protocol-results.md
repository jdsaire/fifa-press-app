# Protocol Results

The outcome of running every check in [`usability-test-protocol.md`](usability-test-protocol.md)
against commit `0653b4e`.

**How to read this:** PASS and FAIL are settled. OPEN means the check could not be closed without
running the app in a browser, which was not possible in this audit. OPEN is not a hidden failure —
it is a check waiting on a person.

---

## Totals

| Result | Count | Share |
|---|---|---|
| PASS | 24 | 44% |
| FAIL | 21 | 39% |
| OPEN | 9 | 17% |
| **Total** | **54** | |

| Method used | Count |
|---|---|
| CODE-VERIFIED | 40 |
| REASONED | 5 |
| REQUIRES-HUMAN-CHECK (all OPEN) | 9 |

---

## A. Task completion

| ID | Result | Method | Evidence |
|---|---|---|---|
| A-01 | PASS | CODE-VERIFIED | `EventList.razor:3, 12–26` renders all 50 events from `MockEventData.cs:12–36` |
| A-02 | PASS | CODE-VERIFIED | `EventList.razor:23` links to `events/{ev.Id}`; route at `EventDetails.razor:4` |
| A-03 | PASS | CODE-VERIFIED | `EventList.razor:24` and `EventDetails.razor:22` both link to `register/{id}` |
| A-04 | PASS | CODE-VERIFIED | `Registration.razor:67–71` updates both trackers on valid submit |
| A-05 | PASS | CODE-VERIFIED | `OnValidSubmit` + `DataAnnotationsValidator` (`Registration.razor:27–29`) gate the handler |
| A-06 | **FAIL** | REASONED | `href="/"` at `EventDetails.razor:32`, `Registration.razor:49`, `NotFound.razor:11` bypasses the base path — **UX-C-04** |
| A-07 | **FAIL** | CODE-VERIFIED | `SessionTracker.cs:12–14` offers no un-register — **UX-MAJ-06** |
| A-08 | **FAIL** | CODE-VERIFIED | No search, filter, sort or paging in `EventList.razor` — **UX-MAJ-08** |

## B. Visibility of system status

| ID | Result | Method | Evidence |
|---|---|---|---|
| B-01 | PASS | CODE-VERIFIED | `Registration.razor:20` shows a confirmation on success |
| B-02 | **FAIL** | CODE-VERIFIED | That confirmation is a plain `<p>` with no live region — **UX-MAJ-05** |
| B-03 | PASS | CODE-VERIFIED | Badge shown at `EventList.razor:19–22` and `EventDetails.razor:17–20` |
| B-04 | **FAIL** | CODE-VERIFIED | No active nav state or breadcrumb off the home route — **UX-MAJ-11** |
| B-05 | **FAIL** | CODE-VERIFIED | `EventDetails.razor:10` titles every event "Event Details" — **UX-MAJ-11** |
| B-06 | PASS | CODE-VERIFIED | Loading indicator at `index.html:19–23`, styled `app.css:70–102` |
| B-07 | **FAIL** | CODE-VERIFIED | `EventDetails.razor:21` produces "1 people registered" — **UX-MIN-01** |

## C. Error handling and recovery

| ID | Result | Method | Evidence |
|---|---|---|---|
| C-01 | PASS | CODE-VERIFIED | `App.razor:4` wires `NotFound.razor`, which offers a route back at line 11 |
| C-02 | PASS | CODE-VERIFIED | `{Id:int}` constraint at `EventDetails.razor:4`, `Registration.razor:3` |
| C-03 | PASS | CODE-VERIFIED | Distinct handling at `EventDetails.razor:24–30`, `Registration.razor:44–47` |
| C-04 | **FAIL** | CODE-VERIFIED | Heading and tab title unchanged on the not-found branch — **UX-MIN-04** |
| C-05 | PASS | CODE-VERIFIED | Specific messages at `RegistrationModel.cs:10–14`, `EventCard.razor:60–62` |
| C-06 | **FAIL** | CODE-VERIFIED | No `aria-describedby` on any error (`EventCard.razor:6–21`) — **UX-MAJ-03** |
| C-07 | **FAIL** | CODE-VERIFIED | "It may have been removed" asserts an unknowable cause — **UX-MIN-04** |
| C-08 | **FAIL** | CODE-VERIFIED | Edits discarded silently on navigation — **UX-C-02** |

## D. Consistency and expectations

| ID | Result | Method | Evidence |
|---|---|---|---|
| D-01 | **FAIL** | CODE-VERIFIED | `EventCard.razor:3–23` is editable on every page, with no save path — **UX-C-01** |
| D-02 | **FAIL** | REASONED | Root-absolute and base-relative links mixed — **UX-C-04** |
| D-03 | **FAIL** | CODE-VERIFIED | `MainLayout.razor:9` opens a new tab unannounced, no `rel="noopener"` — **UX-MAJ-12** |
| D-04 | **FAIL** | CODE-VERIFIED | "Events" vs "Upcoming Events" (`EventList.razor:8` vs `:10`) — **UX-MIN-03** |
| D-05 | **FAIL** | CODE-VERIFIED | Attendee count on details only — **UX-MIN-02** |

## E. Accessibility

| ID | Result | Method | Evidence |
|---|---|---|---|
| E-01 | **FAIL** | CODE-VERIFIED | Date input has no name at all (`EventCard.razor:11`) — **UX-MAJ-01**; registration fields unnamed — **UX-C-03** |
| E-02 | **FAIL** | CODE-VERIFIED | `Registration.razor:32, 36` labels have no `for` and do not wrap — **UX-C-03** |
| E-03 | **FAIL** | CODE-VERIFIED | `app.css:5–7` removes the outline from the focus target — **UX-MAJ-04** |
| E-04 | **FAIL** | CODE-VERIFIED | No skip link in `MainLayout.razor` — **UX-MAJ-07** |
| E-05 | **FAIL** | REASONED | `.validation-message` pure red computes to 4.0:1, below the 4.5:1 minimum — **UX-MAJ-09** |
| E-06 | **FAIL** | CODE-VERIFIED | Event names are inputs, not headings, so the list has no structure — **UX-C-01** |
| E-07 | **FAIL** | CODE-VERIFIED | No `role="status"` on the success message — **UX-MAJ-05** |
| E-08 | **FAIL** | CODE-VERIFIED | No `aria-expanded` on the nav toggle (`NavMenu.razor:4`) — **UX-MAJ-10** |
| E-09 | PASS | CODE-VERIFIED | `index.html:2` declares `lang="en"` |
| E-10 | PASS | CODE-VERIFIED | All primary actions are native buttons and links; the one `<div>` handler at `NavMenu.razor:10` is redundant, not required — **UX-MIN-05** |
| E-11 | PASS | REASONED | DOM order matches visual order throughout; no positional CSS reorders content |
| E-12 | PASS | CODE-VERIFIED | `App.razor:8` moves focus to the heading on navigation — the intent is right even though the indicator is suppressed (E-03) |
| E-13 | **OPEN** | REQUIRES-HUMAN-CHECK | Calculated from declared values only; not verified against rendered pixels |
| E-14 | **OPEN** | REQUIRES-HUMAN-CHECK | No screen reader available in this environment |
| E-15 | **OPEN** | REQUIRES-HUMAN-CHECK | Tab route inferred from source; not walked in a browser |

## F. Responsive behaviour

| ID | Result | Method | Evidence |
|---|---|---|---|
| F-01 | PASS | CODE-VERIFIED | `index.html:6` |
| F-02 | PASS | CODE-VERIFIED | `MainLayout.razor.css:39–47, 49–77` restructure at 641px |
| F-03 | PASS | CODE-VERIFIED | `NavMenu.razor.css:68–76` hides the toggle and expands the menu above 641px |
| F-04 | **FAIL** | REASONED | No `max-width` on `article` — **UX-MIN-06** |
| F-05 | **OPEN** | REQUIRES-HUMAN-CHECK | Not measured in a browser |
| F-06 | **OPEN** | REQUIRES-HUMAN-CHECK | Not measured on a device |
| F-07 | **OPEN** | REQUIRES-HUMAN-CHECK | No resize sweep possible |

## G. Performance as the user feels it

| ID | Result | Method | Evidence |
|---|---|---|---|
| G-01 | PASS | CODE-VERIFIED | `EventList.razor:17` keys by `ev.Id` |
| G-02 | **FAIL** | CODE-VERIFIED | All 50 events render eagerly; `Virtualize` imported at `_Imports.razor:7` but unused — **UX-MAJ-08** |
| G-03 | **OPEN** | REQUIRES-HUMAN-CHECK | Not measured |
| G-04 | **OPEN** | REQUIRES-HUMAN-CHECK | Not measured |

---

## The nine open checks

These are the audit's honest boundary. They are grouped here so they can be handed to a person as a
single list.

| ID | What still needs a human |
|---|---|
| E-13 | Confirm contrast against rendered pixels rather than declared colour values |
| E-14 | Complete a registration end to end using a screen reader |
| E-15 | Tab through the live app and confirm the keyboard route is workable |
| F-05 | Confirm content reflows at 320px with no horizontal scrolling |
| F-06 | Confirm tap targets are comfortable on a real touch device |
| F-07 | Resize sweep for overlap and clipping at intermediate widths |
| G-03 | Confirm the 50-item list stays responsive while scrolling and typing |
| G-04 | Measure initial load on the live deployment |
| A-06 | Confirm the "Back to events" behaviour described in **UX-C-04** — the highest-value single click in this list |

**A note on A-06.** It is recorded as FAIL rather than OPEN because the source evidence is
unambiguous: three links use a root-absolute path while every other link in the app is
base-relative, and the deploy demonstrably serves the app from a subfolder. The conclusion follows
from what is in the files. But the method is REASONED, not CODE-VERIFIED, and one click on the live
site would settle it beyond argument. It is worth doing that click before the fix, not after.
