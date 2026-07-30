# Evaluation Spec

A usability and accessibility audit of the EventEase app.

The rest of this repository explains **how the app is built** — the tech stack, the Blazor
concepts, the reasoning behind each Activity. This folder asks a different question: **is the app
actually good to use?**

A working app and a usable app are not the same thing. Every button here works and no navigation is
broken, but an interface can be fully functional and still confuse the person in front of it. That
gap is what this audit measures.

---

## What was found

| Severity | Count | What it means |
|---|---|---|
| **Critical** | 4 | A user cannot finish a main task, or finishes it believing something happened that did not |
| **Major** | 12 | The task can be finished, but with friction that happens every time |
| **Minor** | 10 | Cosmetic or inconsistent; an obvious workaround exists |
| **Total** | **26** | |

Alongside these, **twelve things the app does genuinely well** are recorded in the findings
register. An audit that lists only defects misrepresents what was built.

The most useful thing in this audit is not the total. It is that six of the ten usability
principles the app was measured against fail for **the same underlying reason** — one component
having only an editable presentation, reused on every page. One structural fix addresses most of
them at once.

---

## The files

Read in this order if you are new to the audit.

| File | What it covers |
|---|---|
| [`usability-test-protocol.md`](usability-test-protocol.md) | The checks the app must pass — written before any results were looked at |
| [`protocol-results.md`](protocol-results.md) | How it did: 24 pass, 21 fail, 9 open |
| [`findings-register.md`](findings-register.md) | Every issue found, with a stable ID and the exact file and line where it lives. The canonical list |
| [`heuristic-evaluation.md`](heuristic-evaluation.md) | Assessment against Nielsen's ten usability heuristics |
| [`accessibility-audit.md`](accessibility-audit.md) | Assessment against WCAG 2.2 Level AA |
| [`usability-assessment.md`](usability-assessment.md) | Usability broken into its five components — learnability, efficiency, memorability, errors, accessibility |
| [`remediation-scope.md`](remediation-scope.md) | What to fix, sequenced into 19 commits. The hand-off to the next development run |

---

## How the audit was done

Every file the app is built from was read in full — all 19 hand-written source files, roughly 530
lines, excluding the third-party Bootstrap library. Nothing was sampled and nothing was assumed
from how Blazor apps usually behave.

**Every finding cites the file and line where it can be seen.** A claim that could not be pointed
at a specific place in the source was left out rather than stated. This is the rule the audit holds
itself to, and it is why the total is 26 rather than a rounder, larger number.

### Verification method — the legend used throughout

Each check states how it was settled:

| Method | Meaning |
|---|---|
| **CODE-VERIFIED** | The source settles it outright. Whether a label exists, whether an attribute is present — reading the file gives a definite answer |
| **REASONED** | The source determines the outcome, but reaching it takes a calculation or an inference — computing a contrast ratio from colour values, or following how a link resolves |
| **REQUIRES-HUMAN-CHECK** | Cannot be settled without running the app and looking at it. Recorded as **OPEN** |

### What this audit could not do

No browser rendered the app during this audit, and no screen reader was used. That is a real
boundary and it is stated rather than worked around.

It means nine checks are **OPEN** — waiting on a person, not quietly counted as passing. They cover
things only a rendered app can answer: how content reflows on a phone, whether tap targets are
comfortable, whether a screen reader can complete the registration journey end to end.

The method is reliable for what it does cover. Whether a form field has a label is not a matter of
opinion, and a contrast ratio calculated from a hex value is arithmetic. But it cannot tell you how
the app feels to operate. Both halves of that are true, and both are stated where they apply.

The nine open checks are listed together at the end of
[`protocol-results.md`](protocol-results.md), ready to hand to whoever performs the visual pass.

---

## What happens next

The audit produces a fix list, not just a verdict. [`remediation-scope.md`](remediation-scope.md)
sequences 26 findings into 19 commits, ordered by severity, each naming the files it touches and
the commit message it should carry.

One constraint governs that work and is worth stating here too: the fixes must keep the Event
Card's two-way data binding intact, because it is a graded requirement of the assignment. The
recommendation is to *add* a read-only display mode, not to remove the editing capability.

**Audited version:** commit `0653b4e`
