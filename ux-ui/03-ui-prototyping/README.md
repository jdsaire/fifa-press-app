# UI Prototyping

A nine-gate specification pass that turns `02-ideation/`'s concept — **Access Record** — into a
buildable interface: competitive benchmarking, an information architecture built from a simulated
card sort, UI design decisions verified against the app's own inherited CSS, three bounded tasks
and scenarios, full screen specifications, a data and context model, and a build brief a build
agent can execute without re-deriving anything. No code is written here. Start with
[`00_SCOPE.md`](00_SCOPE.md) for what this mandate can and cannot establish, and
[`08_LIMITATIONS.md`](08_LIMITATIONS.md) for the full methodological defence of that boundary —
this folder produces a design position, not a validated one, and that file states plainly what was
simulated and what was not.

---

## The nine gates, in order

| Gate | File | What it delivers |
|---|---|---|
| 0 | [`00_SCOPE.md`](00_SCOPE.md) | Scope lock — the inherited contract, the preserved maximalist scope, the premise-untested declaration, and the `src/`-touch boundary Run 4B is held to. |
| 1 | [`01_BENCHMARKING.md`](01_BENCHMARKING.md) | Competitive benchmarking — football, tracking, and travel-authorization comparators, read for state-change patterns rather than visual polish, closing on an adopt/avoid table. |
| 2 | [`02_INFORMATION-ARCHITECTURE.md`](02_INFORMATION-ARCHITECTURE.md) | Card sort and information architecture — a simulated six-participant sort, the ontology and taxonomy it produces, and ten choreography rules governing how the record behaves over time. |
| 3 | [`03_UI-DECISIONS.md`](03_UI-DECISIONS.md) | UI design decisions — a CSS-inheritance audit of the live app, a hybrid retain/overhaul resolution, WCAG 2.2 AA-verified design tokens for both themes, and the component inventory. |
| 4 | [`04_TASKS-AND-SCENARIOS.md`](04_TASKS-AND-SCENARIOS.md) | Tasks and scenarios — three tasks, one per core interaction, each with a Mermaid task flow and named failure modes. |
| 5 | [`05_SCREENS.md`](05_SCREENS.md) | Screen specifications — every screen's content inventory and state matrix, including the sign-in form's boundary as a simulation, not authentication. |
| 6 | [`06_DATA-MODEL.md`](06_DATA-MODEL.md) | Data and context model — the entity model, the fixture-CSV hazard and its containment rule, and the service abstraction Run 4B builds against. |
| 7 | [`07_BUILD-BRIEF.md`](07_BUILD-BRIEF.md) | Build brief — file-level scope, build order, pass/fail acceptance criteria, and the anti-scope-creep list for Run 4B. |
| 8 | [`08_LIMITATIONS.md`](08_LIMITATIONS.md) | Methodological disclosure — every scope decision defended, and what this mandate does and does not legitimately support. |

## Relationship to its sibling folders

[`../00-initial-evaluation/`](../00-initial-evaluation/) audited the app that already existed in
this repo before the FIFA Press App reframing. [`../01-design-research/`](../01-design-research/)
produced the evidence base and the one design question later mandates build on.
[`../02-ideation/`](../02-ideation/) turned that question into the Access Record concept. This
folder does none of those: it takes an already-approved concept and specifies the interface that
would express it — screens, states, data, and an executable build brief — without writing a line
of application code. Run 4B is where this specification becomes the app.
