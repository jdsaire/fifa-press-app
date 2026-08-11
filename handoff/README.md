# handoff/

Records of each build run against this repo: the plan approved before the work started, and a completion report afterward. Organized by version, one folder per run, so it's clear which plan and report go together.

- [`v1/`](v1/) — the original build, under this repository's original project name (EventEase): scaffolding through Activity 3, plus the first pass at documentation.
- [`v2/`](v2/) — an accessibility and documentation overhaul on top of v1's already-working app. No application behavior changed.
- [`v3/`](v3/) — commits the usability/accessibility evaluation spec, then executes its 19-item
  remediation scope. Application behavior does change here — component contracts, page markup, and
  stylesheet rules across 13 source files.
- [`v4/`](v4/) — re-skins the documentation layer as the FIFA Press App, dropping the EventEase
  framing. No application behavior changed — every `src/` path, class, and line-number citation
  stayed byte-accurate throughout.
- [`v5/`](v5/) — injects the completed UX Research mandate (eight gate deliverables, two simulated
  datasets, a glossary) as a new `Design Research/` folder at repo root. Documentation and data
  only — no application behavior changed. *(Relocated post-completion to
  [`ux-ui/01-design-research/`](../ux-ui/01-design-research/); see `v5/Completion-Report-v5.md`
  addendum.)*
- [`v6/`](v6/) — injects the completed ideation mandate (eight gate deliverables) as a new
  `ux-ui/02-ideation/` folder. Documentation only — no application behavior changed.

Each version folder has its own README with more detail on what that run covered.
