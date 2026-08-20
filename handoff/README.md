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
- [`v8/`](v8/) — injects the completed UI-prototyping mandate (nine gate deliverables) as a new
  `ux-ui/03-ui-prototyping/` folder, then enables GitHub Pages (source: GitHub Actions).
  Documentation and a settings change only — no application behavior changed.
- [`v9/`](v9/) — builds the Access Record concept as a frontend vertical slice from v8's dossier:
  four entities, a data-provider interface with one in-memory implementation, six pages, six
  components, and a token-based dual-theme stylesheet. The first run since v3 in which application
  behavior changes substantially — the app stops being an event-registration demo. Frontend only:
  no API, no database, no authentication.
- [`v10/`](v10/) — the repository's first committed test project (`tests/FifaPressApp.Tests/`,
  xUnit + bUnit), plus four straight patches with no design dependency: the request-access
  Submitting state made observable, decorative date/venue/phase iconography on the match surfaces,
  and group and match-status filters on the match list. Search behaviour is unchanged, proven by
  tests written before either filter existed. Frontend only, and no `.csproj` reversal reaches the
  published bundle: every new package lives in the test project alone.
- [`v11/`](v11/) — injects the four Run 4D design-addendum files (`09_DESIGN-ADDENDUM.md` through
  `12_DECISION-REVERSALS.md`) into `ux-ui/03-ui-prototyping/`, byte-identical to how they were
  authored, then applies targeted patches — once three principal-gated open items were resolved —
  that flip all four from proposed to Final. Documentation only: no `src/` file and no frozen gate
  file changed.
- [`v12/`](v12/) — builds the four Run 4D addendum files into working code, across five gated
  boundaries: a dark theme re-anchored to solid black; a simulated, honestly-documented sign-in with
  two seeded holders demonstrating the same conditional change resolving two different ways; the
  app in English, Spanish and Portuguese with the session surviving a language switch; a small,
  strictly-typed TypeScript interop layer whose compiled output is committed so nothing about
  building or deploying the app needs Node; and two-layer disclosure on the change log and Help.
  Frontend and documentation only — the app project's `.csproj`, `ux-ui/`, `wwwroot/lib/`, and every
  workflow file are byte-identical to `ac5555c` throughout. Tests: 82 → 409.
- [`v13/`](v13/) — injects the completed `04-evaluation` dossier (nine gate deliverables, a
  task-based re-audit) into `ux-ui/04-evaluation/`, then executes its four-item remediation scope: a
  status-logic fix, an accessibility fix, and a two-part Withdrawal affordance — open since v9, the
  first time it closes. Frontend, locale files, and documentation only. Tests: 409 → 421.
- [`v14/`](v14/) — injects the completed `05-iteration` dossier (six gate deliverables) into
  `ux-ui/05-iteration/`, then executes its sixteen-item commit sequence across four gated
  workstreams: a persistent top bar, a Settings screen, a reduced Landing and condensed sign-in
  notice with renamed demo identifiers, and a rebuilt match list with capacity, localization,
  Show-more pagination, and a conditional `/record`. The Gate 4 stop's own live test then surfaced a
  sign-in rejection, which three further approved addenda investigate and resolve: generic,
  non-personal demo credentials; a real sign-in crash root-caused to one line and fixed, plus a
  regression it had been masking; a ShopEase-parity layout pass; and a final round of surgical
  fixes. Frontend, locale files, and documentation only. Tests: 421 → 512.
- [`v15/`](v15/) — the first run to add a server. Moves the Blazor app to `src/frontend/`
  and its tests to `tests/frontend/` as a path-only change — no namespace, assembly or
  project filename touched — then builds an ASP.NET Core Web API at `src/backend/`: CRUD
  over the accreditation record and its change log, validation carrying the frontend's own
  domain rules, three middleware components in the documented order, and one SignalR hub.
  The frontend gains an API-backed provider behind the `IAccessDataProvider` interface it
  has used since v9, **off by default**, so the deployed site is unchanged. Adds `backend/`
  (documentation, parallel to `ux-ui/`) and `learning-mode/04-backend-and-integration/`.
  The published output is 320 of 329 files byte-identical, every difference a commit SHA or
  a build path. Tests: 512 → 512 frontend + 33 backend.

Each version folder has its own README with more detail on what that run covered.
