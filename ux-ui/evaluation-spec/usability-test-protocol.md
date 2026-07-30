# Usability Test Protocol

The list of things EventEase has to get right, written as checks that can be run against the app.
Results of running them are in [`protocol-results.md`](protocol-results.md).

A protocol is written *before* the results, on purpose. Deciding what counts as passing after you
have already looked at the answers is how audits end up confirming whatever the author already
believed.

**Audited version:** commit `0653b4e`

---

## How each check is verified

Every check states how it can be settled. This matters because the audit environment had no way to
run the app in a browser.

| Method | Meaning |
|---|---|
| **CODE-VERIFIED** | The source settles it outright. Whether a label exists, whether a route is defined, whether an attribute is present — reading the file gives a definite answer. |
| **REASONED** | The source determines the outcome, but reaching it takes a calculation or an inference: working out a contrast ratio from colour values, or following how a link will resolve. |
| **REQUIRES-HUMAN-CHECK** | Cannot be settled without running the app and looking at it. Recorded as **OPEN**. Never recorded as passing. |

An OPEN result is not a failure and not a pass. It is an honest statement that this check is
waiting on a person.

---

## The journeys under test

| ID | Journey |
|---|---|
| **J1** | Browse the event list |
| **J2** | Open one event's details |
| **J3** | Register for an event — both the rejected-input path and the successful path |
| **J4** | Return and continuity — does the app remember, and can you get back? |
| **J5** | Error paths — an address matching nothing, and an address naming an event that does not exist |
| **J6** | The shell around everything — navigation, mobile menu, loading |

---

## A. Task completion

| ID | Check | Passes when | Journey | Method |
|---|---|---|---|---|
| A-01 | The event list is reachable and lists events | The `/` route renders every event in the data source | J1 | CODE-VERIFIED |
| A-02 | A specific event's details can be opened from the list | Each list entry links to `/events/{id}` for its own event | J1→J2 | CODE-VERIFIED |
| A-03 | Registration can be reached from both the list and the details page | Both pages link to `/register/{id}` | J1→J3, J2→J3 | CODE-VERIFIED |
| A-04 | A valid registration is accepted and recorded | Submitting valid details updates both session and attendance state | J3 | CODE-VERIFIED |
| A-05 | An invalid registration is refused | Submission with a missing or malformed field does not reach the handler | J3 | CODE-VERIFIED |
| A-06 | The user can return to the list from anywhere | Every page offers a working route back to `/` | J4 | REASONED |
| A-07 | Registering for the wrong event can be undone | Some path exists to cancel a registration | J4 | CODE-VERIFIED |
| A-08 | A specific event can be located without reading the whole list | Search, filter, sort, or paging exists | J1 | CODE-VERIFIED |

## B. Visibility of system status

| ID | Check | Passes when | Journey | Method |
|---|---|---|---|---|
| B-01 | A successful registration is confirmed on screen | A visible confirmation replaces or follows the form | J3 | CODE-VERIFIED |
| B-02 | That confirmation is also announced to assistive technology | It is a live region or carries `role="status"` | J3 | CODE-VERIFIED |
| B-03 | Registration status is visible wherever the event appears | A "Registered" indicator shows on list and details | J4 | CODE-VERIFIED |
| B-04 | The current location in the app is identifiable | Active nav state, breadcrumb, or a heading naming the event | J6 | CODE-VERIFIED |
| B-05 | Page titles distinguish one event from another | The tab title includes the event name | J2 | CODE-VERIFIED |
| B-06 | Loading is communicated while the app starts | A loading indicator is present before the app is ready | J6 | CODE-VERIFIED |
| B-07 | The attendee count is accurate and grammatical | Count agrees in number with its noun | J2 | CODE-VERIFIED |

## C. Error handling and recovery

| ID | Check | Passes when | Journey | Method |
|---|---|---|---|---|
| C-01 | An address matching no route shows a helpful page | A custom not-found page renders with a route back | J5 | CODE-VERIFIED |
| C-02 | A non-numeric event ID does not reach page code | The route constraint rejects it | J5 | CODE-VERIFIED |
| C-03 | A well-formed but nonexistent event ID is handled distinctly | A specific message explains the event was not found | J5 | CODE-VERIFIED |
| C-04 | Error states look different from success states | Heading, title, or layout distinguishes them | J5 | CODE-VERIFIED |
| C-05 | Form errors say what is wrong and how to fix it | Each rule has a specific, actionable message | J3 | CODE-VERIFIED |
| C-06 | Error messages are tied to the field they describe | `aria-describedby` or equivalent association exists | J3 | CODE-VERIFIED |
| C-07 | Error text does not assert causes the app cannot know | Copy stays within what the system can actually determine | J5 | CODE-VERIFIED |
| C-08 | Work is not lost without warning | Any accepted input is either kept or the user is told it will not be | J1→J2 | CODE-VERIFIED |

## D. Consistency and expectations

| ID | Check | Passes when | Journey | Method |
|---|---|---|---|---|
| D-01 | An element that looks editable is editable and saveable | Editable controls appear only where editing is supported | J1, J2 | CODE-VERIFIED |
| D-02 | Links behave consistently | All internal links resolve the same way relative to the app root | J1–J6 | REASONED |
| D-03 | Links leaving the app are identifiable as such | New-tab or external destinations are signposted | J6 | CODE-VERIFIED |
| D-04 | Naming is consistent between tab title and heading | The same screen is called the same thing in both | J1 | CODE-VERIFIED |
| D-05 | The same information is presented the same way across pages | Shared data appears consistently wherever shown | J1, J2 | CODE-VERIFIED |

## E. Accessibility

Full detail in [`accessibility-audit.md`](accessibility-audit.md).

| ID | Check | Passes when | WCAG | Method |
|---|---|---|---|---|
| E-01 | Every form control has an accessible name | Label, `aria-label`, or `aria-labelledby` on each | 4.1.2 / 3.3.2 | CODE-VERIFIED |
| E-02 | Labels are programmatically tied to their control | `for`/`id` pairing or wrapping | 1.3.1 | CODE-VERIFIED |
| E-03 | A visible focus indicator is present on every focusable element | No indicator is suppressed without a replacement | 2.4.7 | CODE-VERIFIED |
| E-04 | Repeated blocks can be bypassed | A skip link or landmark route to main content exists | 2.4.1 | CODE-VERIFIED |
| E-05 | Text meets minimum contrast | 4.5:1 for normal text, 3:1 for large | 1.4.3 | REASONED |
| E-06 | Content has a meaningful heading structure | Headings describe the content beneath them | 1.3.1 | CODE-VERIFIED |
| E-07 | Status messages are announced without stealing focus | Live regions used for status changes | 4.1.3 | CODE-VERIFIED |
| E-08 | Controls that expand or collapse report their state | `aria-expanded` reflects the state | 4.1.2 | CODE-VERIFIED |
| E-09 | The page language is declared | `lang` on the root element | 3.1.1 | CODE-VERIFIED |
| E-10 | Everything operable by mouse is operable by keyboard | No interaction depends on pointer input alone | 2.1.1 | CODE-VERIFIED |
| E-11 | Focus order follows a sensible reading order | DOM order matches visual order | 2.4.3 | REASONED |
| E-12 | Focus is handled sensibly when content is replaced | Focus moves somewhere meaningful, not nowhere | 2.4.3 | CODE-VERIFIED |
| E-13 | Contrast holds in the app's rendered state | Verified against what is actually painted | 1.4.3 | REQUIRES-HUMAN-CHECK |
| E-14 | A screen reader can complete registration end to end | Verified with an actual screen reader | 1.3.1 / 4.1.2 | REQUIRES-HUMAN-CHECK |
| E-15 | The keyboard tab route is workable in practice | Verified by tabbing through the live app | 2.4.3 / 2.4.1 | REQUIRES-HUMAN-CHECK |

## F. Responsive behaviour

| ID | Check | Passes when | Method |
|---|---|---|---|
| F-01 | A viewport meta tag is present | Declared in the page head | CODE-VERIFIED |
| F-02 | The layout adapts between narrow and wide screens | Breakpoint rules restructure the layout | CODE-VERIFIED |
| F-03 | Navigation collapses on narrow screens and expands on wide | Toggle appears below the breakpoint, hidden above | CODE-VERIFIED |
| F-04 | Line length stays readable on wide screens | Content width is bounded | REASONED |
| F-05 | Content reflows without horizontal scrolling at 320px | Verified at that width in a browser | REQUIRES-HUMAN-CHECK |
| F-06 | Tap targets are large enough on touch devices | Verified on a real device | REQUIRES-HUMAN-CHECK |
| F-07 | Nothing overlaps or clips at intermediate widths | Verified across a resize sweep | REQUIRES-HUMAN-CHECK |

## G. Performance as the user feels it

| ID | Check | Passes when | Method |
|---|---|---|---|
| G-01 | List rendering is identity-keyed, not position-keyed | Keys are stable across list changes | CODE-VERIFIED |
| G-02 | Large lists do not render every item eagerly | Virtualization or paging limits what is built | CODE-VERIFIED |
| G-03 | The list stays responsive while scrolling and typing | Verified in a browser | REQUIRES-HUMAN-CHECK |
| G-04 | Initial load time is acceptable | Measured on the live deployment | REQUIRES-HUMAN-CHECK |

---

## Scope boundary

Every check above sits inside what this project is allowed to be: a Blazor WebAssembly app with
mock in-memory data, no authentication, no backend, and no database. Nothing here asks the app to
do something the assignment excludes.

Some things a fuller product audit would cover are therefore deliberately absent — password
handling, data retention, email confirmation, real attendance figures. Those are out of scope by
design, not oversights. They are listed explicitly in
[`remediation-scope.md`](remediation-scope.md).
