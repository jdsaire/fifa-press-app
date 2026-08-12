# Usability Assessment

*(Assessed under the app's original name, EventEase — since renamed to the FIFA Press App /
`src/FifaPressApp/` in v7. Preserved as historical record.)*

An assessment of how usable EventEase is, based on what can be established from the interface
itself.

**Audited version:** commit `0653b4e`
**Finding IDs** refer to [`findings-register.md`](findings-register.md).

---

## What this file does and does not claim

Usability is normally measured by watching real people attempt real tasks: how many finish, how
long it takes, how many mistakes they make, how they rate the experience afterwards. None of that
happened here. No users were recruited and no sessions were run.

What this assessment does instead is examine the interface for the conditions that produce good or
bad usability — how many steps a task takes, whether mistakes can be undone, whether the interface
names its own parts, whether every user can reach the controls at all. These are *proxies*: things
that reliably predict usability outcomes without measuring them directly.

That distinction is worth keeping in view. A proxy tells you where the problems probably are. It
does not tell you how severely real users would struggle. Where a claim rests on inference rather
than observation, it says so.

Two related measures — user satisfaction and user loyalty — were considered for this audit and
deliberately excluded. Both are attitudinal and longitudinal: satisfaction is what people report
after using something, loyalty is whether they come back over time. Neither can be honestly
assessed from source code, and a session-scoped app with mock data offers nothing to measure return
behaviour against. Including them would have meant inventing proxies weak enough to be misleading.
This assessment covers usability only, and covers it properly.

---

## The five components of usability

Nielsen's standard decomposition, assessed one at a time.

| Component | Assessment |
|---|---|
| Learnability — how easily a first-time user accomplishes a task | Weak |
| Efficiency — how quickly an experienced user works | Weak |
| Memorability — how easily a returning user picks it back up | Adequate |
| Errors — how many, how severe, how recoverable | Weak |
| Accessibility — whether all users can use it at all | Failing |

---

### Learnability — **weak**

A first-time user arrives at a page whose purpose is not self-evident. Roughly 50 events, each
rendered as three editable boxes, none of which is labelled once populated (**UX-C-01**,
**UX-MAJ-02**). The correct mental model — "this is a list of events I can browse and sign up
for" — is contradicted by the strongest visual signal on the page, which says "this is a form you
fill in."

Working out what the interface is takes deduction. That is the definition of a learnability
problem.

Once past the list, the flow improves markedly. The details page and registration form are
conventional, well-labelled visually, and behave as expected. The concepts are not hard; the
entry point misrepresents them.

**What supports this assessment:** the affordance mismatch is structural and visible in the source
(`EventCard.razor:3–23`), and it appears on the first screen a user sees. **What is inferred:**
that real users would actually be confused. Plausible, but not observed.

### Efficiency — **weak**

For a returning user who knows what they want, the app offers no shortcuts. Finding a known event
among 50 means scrolling and reading — no search, no filter, no sort, no paging (**UX-MAJ-08**).
Task time grows linearly with list position, which is exactly the pattern accelerators exist to
break.

Keyboard efficiency is worse. Every card contributes five focusable elements, so around 250 stops
lie between the top of the page and the last event, with no skip link (**UX-MAJ-07**). Reaching
the tenth event's Register button takes roughly 50 keystrokes.

**What supports this assessment:** element counts are arithmetic from the source. **What is
inferred:** the practical cost to a user, which was not timed.

### Memorability — **adequate**

The strongest of the five, and worth crediting. The app is small — three screens and one action —
with a consistent layout and a persistent sidebar. There is little to forget.

State also survives navigation: both trackers are registered as singletons (`Program.cs:16–17`), so
the "Registered" badge follows the user across pages within a session. A user who steps away and
returns mid-session finds the app as they left it.

The limit is that the app cannot show *where* you are once you leave the home route
(**UX-MAJ-11**). A user returning to an open tab titled "Event Details" has no way to tell which
event it is.

### Errors — **weak**

Error *prevention* is genuinely well done and should not be lost in the summary. The route
constraint rejects malformed IDs before they reach page code. `OnValidSubmit` guarantees an invalid
form never reaches the handler. Duplicate registration is structurally impossible. Validation rules
sit on the model rather than being scattered through the UI.

Error *recovery* is where it falls down, in two specific ways:

The interface accepts input it silently discards (**UX-C-02**). A user edits an event name, the
edit appears to take, and it is gone on navigation with no warning. This is the most serious class
of usability error — not a mistake the user makes, but one the system makes and conceals.

And the app's one irreversible action has no exit (**UX-MAJ-06**). Registering for the wrong event
is easy given that every card looks alike, and there is no way to undo it.

Error messages themselves are well written and specific — that is a real strength. The problems are
in how they are delivered, not what they say (**UX-MAJ-03**, **UX-MAJ-09**).

### Accessibility — **failing**

Assessed in full in [`accessibility-audit.md`](accessibility-audit.md); summarised here because it
is inseparable from usability. Twelve of fifteen assessable accessibility checks fail.

The decisive one: a screen reader user cannot reliably tell the registration form's two required
fields apart, because neither has an accessible name (**UX-C-03**). That is the app's single
conversion action.

A component is not "mostly usable" if an entire category of user cannot complete the primary task.
This is the weakest of the five components and the one that most affects the overall picture.

---

## Task-level friction

The three primary journeys, counted in steps and marked for where friction appears.

| Journey | Steps | Friction |
|---|---|---|
| Browse the list | 1 — load `/` | Purpose misread on arrival (**UX-C-01**); no way to find a specific event (**UX-MAJ-08**) |
| View one event | 2 — load, click View Details | Clean. The best-working journey in the app |
| Register | 3–4 — reach the form, enter name, enter email, submit | Fields unnamed for assistive technology (**UX-C-03**); success unannounced (**UX-MAJ-05**); no undo (**UX-MAJ-06**) |
| Return to the list | 1 — click Back to events | Source indicates this exits the app on the deployed site (**UX-C-04**) |

Step counts are low, which is good — the app is not over-engineered. The friction is not in the
number of actions but in what the interface communicates while they are performed.

---

## Overall

**Usability is currently limited by a small number of structural decisions rather than by broad
poor design.**

That distinction matters for what happens next. The routing is sound, error prevention is
thoughtful, state management works correctly across navigation, and the error copy is well written.
These are not the marks of a carelessly built app.

What holds it back is concentrated. One component having only an editable presentation accounts for
the learnability failure, most of the efficiency cost, the silent-data-loss error, and a large part
of the accessibility failure. Missing form labels account for most of the rest.

The practical implication: a relatively contained set of changes should move usability
substantially, because the same fixes resolve problems across several components at once. That is
set out, sequenced, in [`remediation-scope.md`](remediation-scope.md).

**What this assessment cannot tell you:** how severely real users would actually struggle, whether
the affordance confusion resolves within seconds or persists, and whether the keyboard cost is
merely annoying or genuinely prohibitive. Those need people. The nine open checks in
[`protocol-results.md`](protocol-results.md) are where that work starts.
