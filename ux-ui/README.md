# ux-ui/

Every UX dossier produced against this app — audits, research, and whatever comes after them.
Organized as one numbered folder per dossier, in the order each was produced, so it's clear which
came first and which builds on which.

- [`00-initial-evaluation/`](00-initial-evaluation/README.md) — a usability and accessibility audit
  of the EventEase-era app as it existed before the FIFA Press App reframing: 26 findings across
  heuristic and WCAG 2.2 AA checks, sequenced into a 19-item remediation scope.
- [`01-design-research/`](01-design-research/README.md) — a simulated guerrilla UX research pass on
  the accreditation-to-matchday journey: eight gate deliverables, two simulated datasets, and a
  methodological disclosure. Not evidence about real journalists — see its own README for what it
  can and cannot establish.
- [`02-ideation/`](02-ideation/README.md) — a simulated eight-gate ideation mandate that turns
  `01-design-research/`'s one open design question into a single buildable concept, Access Record:
  eight gate deliverables and a ranked idea pool of thirty-two, each traceable to a decision. Not a
  validated concept — see its own README for what it can and cannot establish.
- [`03-ui-prototyping/`](03-ui-prototyping/README.md) — a nine-gate specification pass that turns
  `02-ideation/`'s concept into a buildable interface: benchmarking, information architecture, UI
  design decisions, tasks and scenarios, screen specifications, a data model, and a build brief for
  Run 4B. Specification only, no code — see its own README for what it can and cannot establish. Now
  also carries a four-file Run 4D design addendum, finalized, closing out open items before Run 4E
  builds against it.
- [`04-evaluation/`](04-evaluation/README.md) — a task-based usability and accessibility re-audit of
  the app after Run 4E, scored against six simulated task-attempts rather than source alone: 26 of
  `00`'s findings dispositioned, five new findings raised, and a four-item remediation scope
  sequencing the fixes this repo builds against next. See its own README for what it can and cannot
  establish.
- [`05-iteration/`](05-iteration/README.md) — a five-gate design iteration that reads a separate
  reference app (a course-mate's e-commerce build) and realigns this one's navigation, settings,
  front door, sign-in and match list against it: six conflicts between the reference's patterns and
  decisions this codebase argues for in its own comments, each opened as a numbered reversal and
  closed with the reasoning that resolved it, plus the consolidated i18n table, file manifest and
  commit sequence the v14 run builds from. Specification and decisions only — see its own README for
  what it authorizes and what stays provisional.

**From now on, every new UX dossier lands here** as the next numbered folder, rather than as its
own top-level directory at repo root.

Each dossier folder has its own README with more detail on what it covers.
