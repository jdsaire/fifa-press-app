# Usability Assessment

**Repo path:** `ux-ui/04-evaluation/06_USABILITY-ASSESSMENT.md`
**Direct continuation of:** `00-initial-evaluation/usability-assessment.md` — same five components,
same doctrine on what a source-only assessment can and cannot claim
**Assessed against:** `02_TASK-RESULTS.md`'s actual task evidence — six task-attempts plus Tomás's
observation — not impression, and not structural inference alone. Where a claim rests on Gate 2's
walkthroughs rather than that evidence, it says so
**Findings cited:** `05_FINDINGS-REGISTER.md`

---

## What this file does and does not claim, updated from `00`

`00` had no task evidence at all — no tasks existed yet — so its whole assessment was proxy-based:
element counts, structural inference, "this predicts a problem" rather than "this is what happened
when the task ran." This run has something `00` didn't: **six simulated task-attempts that actually
ran against the shipped build, screen by screen.** That is a real methodological step up from pure
structural reading, and the assessment below leans on it wherever Gate 2 produced direct evidence.

It is still not real usability testing. No person sat down with this app. The task-attempts are
authored walkthroughs, not recordings — `02_TASK-RESULTS.md`'s own header states this in the same
`SIMULATED — NOT EVIDENCE` terms every prior mandate uses. Where a claim below rests on a real
task-attempt outcome, it is marked as such; where it still rests on inference from source alone
(because no task exercised that path), that is marked too.

**User satisfaction and user loyalty remain excluded**, for the same reason `00` gave and not
reopened here: both are attitudinal or longitudinal, and neither can be honestly assessed from
source or from six authored walkthroughs. Including them would mean inventing proxies weak enough
to mislead.

---

## Scale, extended from `00`'s own vocabulary

`00` used three labels across its five scores — Weak, Adequate, Failing — and never needed a fourth,
because nothing scored well enough to need one. This run does. The scale is stated once, in the
order `00`'s own usage implies:

**Failing < Weak < Adequate < Strong**

---

## The five components of usability

| Component | `00` | `04` | Movement |
|---|---|---|---|
| Learnability | Weak | **Adequate** | ↑, capped by one finding — see below |
| Efficiency | Weak | **Strong** | ↑↑ |
| Memorability | Adequate | **Strong** | ↑ |
| Errors | Weak | **Adequate** | ↑ |
| Accessibility | Failing | **Strong** | ↑↑↑, capped by the same finding as Learnability |

---

### Learnability — **Adequate** (up from Weak)

**Task evidence, not inference:** all six of Gate 2's attempts succeed with no facilitator help,
using only what's on screen — `StaleIndicator` states its own meaning inline, `ForeseeableBadge`'s
wording is self-explanatory without a legend, and the two-layer disclosure pattern
(`<details>/<summary>`) needs no instruction to operate; native semantics mean a first-time user's
existing browser knowledge already covers it. `00`'s specific problem — the first screen's strongest
visual signal contradicting what the app actually is — has no equivalent here: `MyAccess.razor`'s
structure (headline → staleness → what changed → identity) matches what a first-time user would
guess it does.

**What caps this below Strong:** `04-CRIT-01` (`05_FINDINGS-REGISTER.md`) sits on exactly the
learnability-critical spot — the first sentence a new user reads about their own entitlement can be
wrong in the same direction `00`'s own affordance mismatch was: the strongest, earliest signal
misrepresenting the actual state. `00`'s version was structural (the whole page misrepresented its
purpose); this version is narrower (one status word misrepresents one fact) but sits in the same
functional position — the first thing learned, learned wrong. That narrowness is exactly why this
scores Adequate rather than Weak, and exactly why it doesn't reach Strong.

### Efficiency — **Strong** (up from Weak)

**Task evidence:** Task 1 completes in one screen-load plus one disclosure-open — no navigation.
Task 3 completes in one screen-load with zero interaction — the scenario's entire point.
Task 2 completes in one screen-load, optionally one navigation to the dependency fixture (Attempt
2) — and the flowchart's own "no" branch (read from the record alone) is equally valid and equally
fast. No attempt across all six required more than one navigation beyond the entry screen.

**Structural confirmation:** `00`'s two named efficiency costs are both independently closed —
search/filter/pagination now exist (`05_FINDINGS-REGISTER.md`, UX-MAJ-08 Fixed) and the ~250
keyboard-focusable-element problem is resolved by read-only-by-default list cards (UX-MAJ-07 Fixed).
Nothing in six task-attempts surfaced a new efficiency cost to weigh against these gains.

### Memorability — **Strong** (up from Adequate)

**Task evidence:** Attempt 4 (Gate 2) confirms a language switch mid-session preserves the signed-in
holder, their credential, and the sidebar identity block unchanged — `LanguageSwitchTests` pins this
directly. A returning user picking the app back up after switching language or theme finds
themselves exactly where they left off, which `00`'s app had no equivalent mechanism to test (no
session, no locale, one theme).

**Structural confirmation:** the persistent "signed in as [holder, credential]" block
(`NavMenu.razor:20-27`) resolves `00`'s specific gap — "the app cannot show *where* you are once you
leave the home route" — more completely than `00`'s own app needed, since this build has two
comparable demo records whose confusion risk is exactly what that block was built to prevent. Active
nav state and breadcrumbs add location cues `00` found entirely absent (UX-MAJ-11 Fixed).

### Errors — **Adequate** (up from Weak)

**Prevention, unchanged and still strong:** route constraints, `OnValidSubmit`-gated forms, and
data-annotation validation are all intact — `00` credited these and nothing found here disturbs
that credit.

**Recovery, genuinely mixed — task evidence and register evidence together:** sign-out and the
language/theme controls are all trivially reversible, unlike anything in `00`'s app. But
`05_FINDINGS-REGISTER.md`'s `UX-MAJ-06` disposition is **Still Open** — the app's one write action
(`/request/{id}`) still has no undo, the same shape of gap `00` found in registration, five runs
running. And a genuinely new error was found, in the same *category* `00`'s worst finding occupied:
`00` called silent data loss "the most serious class of usability error — not a mistake the user
makes, but one the system makes and conceals." `04-CRIT-01` is that same category, differently
shaped: not data silently lost, but a status silently misstated. Worth naming as a real echo, not a
coincidence to smooth over — a system stating something untrue to the person relying on it is the
same failure mode whether the untruth is "your edit saved" or "your access was withdrawn."

**What moves this to Adequate rather than holding at Weak:** error *messaging* — as opposed to
error *state* — is measurably better: `aria-live` on validation errors (closing `00`'s UX-MAJ-03),
distinct and non-speculative not-found/empty/error copy (closing UX-MIN-04), and no unfounded
causal claims anywhere checked. The delivery half of `00`'s errors problem is resolved; the
existence half (one irreversible action, one status inaccuracy) persists in a different shape.

### Accessibility — **Strong** (up from Failing)

Assessed in full in `04_ACCESSIBILITY-AUDIT.md`; summarised here because, per `00`'s own framing,
it is inseparable from usability. **18 of 22 assessable checks now pass**, against `00`'s 8 of 23.
The decisive `00` failure — a screen reader user unable to tell the registration form's fields
apart — has no equivalent here: every field checked carries a proper label, and error identification
now meets WCAG 3.3.1 throughout.

**What caps this below a clean Strong-with-no-caveat:** the one remaining accessibility failure
(`04_ACCESSIBILITY-AUDIT.md` §3, WCAG 4.1.3) is `04-CRIT-01` again — the same finding driving
Learnability's cap above. A screen reader user gets exactly as misled by "Access withdrawn" as a
sighted user does; the defect has no accessibility-specific severity beyond what every user already
experiences. Recorded as a single caveat rather than a separate deduction, since it is not a new
accessibility-only problem.

---

## Task-level friction, from Gate 2's actual attempts

Unlike `00`'s table — built from inferred journeys, since no task existed yet — this one is built
directly from the six attempts that actually ran.

| Task | Steps (from Gate 2) | Friction |
|---|---|---|
| **Task 1** — read what changed | 1 screen-load + 1 disclosure-open, no navigation (Attempts 1, 4) | `04-CRIT-01` surfaces on arrival, before the disclosure is even opened — the headline is read before the correct, qualified detail is |
| **Task 2** — find out what's about to change | 1 screen-load, optional 1 navigation to the dependency fixture (Attempts 2, 5) | Clean on the flowchart's own named path (`/events/93`). `04-CRIT-01` reproduces a second way only if a participant instead visits the entitlement's own page (`/events/98`) — a natural but not-required extension of the task |
| **Task 3** — confirm state offline | 1 screen-load, zero interaction required (Attempts 3, 6) | **The cleanest of the three.** No friction surfaced in either attempt, in either theme |

**Roster-level friction, not task-level:** `04-MAJ-02` — no Freelance-track demo record exists, so
every W2 attempt substitutes Amina's record under W2's reading lens rather than exercising
genuinely different data. This doesn't register as friction *within* a task, since the substituted
attempts still pass — it registers as a gap in what this run's evidence can speak to at all for the
freelance archetype specifically.

Step counts are as low as `00`'s were, and for the same reason `00` called good: the app is not
over-engineered. Where `00` found friction concentrated in *what the interface communicated during*
low step-counts, this run finds the same shape of concentration — one specific untruth, surfacing at
the least convenient possible moment in two of three tasks.

---

## Overall

**Usability has moved substantially, and — like `00`'s own closing observation — the movement and
the one remaining problem are both concentrated rather than distributed.**

Four of five components improved by at least one full band; two improved by two bands. Every
structural strength `00` credited (routing, validation architecture, state persistence) is intact or
extended. The specific mechanisms `00` asked for — real labels, search and filtering, a skip link,
an in-app help destination, live regions on form errors — are all present and independently verified
across three prior gates.

**What holds it back is, again, one thing.** `04-CRIT-01` is directly responsible for capping
Learnability below Strong, for the one open WCAG failure capping Accessibility, for one of two
Fails still standing in `03_HEURISTIC-EVALUATION.md`, and for the friction row in two of three
tasks above. `00` found six heuristic failures tracing to one component; this run finds one
component's usability ceiling — across two of five factors, plus a chunk of the Errors picture —
tracing to one function (`StatusFor`/`status`, `05_FINDINGS-REGISTER.md` `04-CRIT-01`). The same
practical implication `00` drew still holds, now on a much smaller surface: a contained fix moves
several numbers at once.

**What this assessment still cannot tell you:** whether `04-CRIT-01`'s misleading headline would
actually derail a real first-time reader, or whether the qualifying `ForeseeableBadge` two inches
below is enough that most people would self-correct before acting on it. That needs a person. The
open checks in `04_ACCESSIBILITY-AUDIT.md` §4-§5 and the untested interval premise (`00_SCOPE.md`
§4) are where that work would start, if a future run picks it up.

---

✅ **GATE 6 COMPLETE** — `06_USABILITY-ASSESSMENT.md`
