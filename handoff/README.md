# handoff/

Records of each build run against this repo: the plan approved before the work started, and a completion report afterward. Organized by version, one folder per run, so it's clear which plan and report go together.

- [`v1/`](v1/) — the original build, under this repository's original project name (EventEase, source folder `src/EventEase/` — renamed to `src/FifaPressApp/` in v7): scaffolding through Activity 3, plus the first pass at documentation.
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
- [`v7/`](v7/) — renames the source folder and namespace from `EventEase` to `FifaPressApp`, and
  corrects `.github/workflows/deploy-pages.yml`'s six hardcoded references to a different
  repository's name. Mechanical rename only — no application behavior, CSS, or content changed.

Each version folder has its own README with more detail on what that run covered.
