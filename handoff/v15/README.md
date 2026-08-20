# v15 — Backend API and Repository Reorganisation

Run 4C: the run that gave this project a server, and the folder reorganisation that makes
a full-stack repository navigable without a guide.

**Two things, in strict order.** First a path-only move — the Blazor app from
`src/FifaPressApp/` to `src/frontend/`, its tests from `tests/FifaPressApp.Tests/` to
`tests/frontend/` — so a backend could sit beside them without an inspector guessing which
folder held the browser code. Every namespace, assembly name and project filename stayed
byte-identical; `FifaPressApp.csproj` is still `FifaPressApp.csproj`, and `git log --follow`
traces both renames. Then the backend itself: an ASP.NET Core Web API with CRUD endpoints
over the accreditation record and its change log, input validation carrying the same domain
rules the frontend enforces, three middleware components registered error handling →
authentication → logging, and one SignalR hub.

**The point of the run is what did not change.** Since v9 every screen has read its data
through `IAccessDataProvider` and never named the class behind it — a bet placed three runs
before there was anything to swap in. This run collected on it: the app now reads from a
real API without one page, component, stylesheet, route or string being edited. Rendering
the same record screen through both providers in one run produced identical text on every
row. The published output is 329 files before and after, 320 byte-for-byte identical, with
every difference traceable to a commit SHA or a local build path.

**The API is off by default, deliberately.** The frontend ships with no API base URL, runs
on its in-memory mock, and behaves exactly as it did at v14 — so the live site is unaffected
whether or not a server exists, is awake, or is reachable.

**Honest about what it is not.** No database: records live in a list, seeded from a file.
No authentication: the token is a fixed string published in the repository, and all eight
`backend/` documents plus the code say so in the same words. The hosting decision that
patch §5.1 declared blocking is resolved here — Azure App Service free tier — but nothing
is provisioned and no credentials exist in this repository.

Also new: `backend/`, a root-level **documentation** folder deliberately parallel to
`ux-ui/`, and `learning-mode/04-backend-and-integration/`, four chapters written after the
code landed. Tests: 512 → 512 frontend + 33 backend.

- [`CC-PLAN-v15.md`](CC-PLAN-v15.md) — the plan as approved before task 2, including the
  two drifts from the deploy prompt's stated assumptions that preflight surfaced.
- [`Completion-Report-v15.md`](Completion-Report-v15.md) — the 18-commit table with real
  SHAs, a PASS/PARTIAL line against all 20 success criteria, five reversals, ten decisions
  resolved autonomously, the measured publish comparison, the link recount, and an honest
  account of what Gate 4 did and did not prove.

**Audited/executed against:** `main` @ `c77a503`.
