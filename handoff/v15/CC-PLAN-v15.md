# CC-PLAN-v15 — Run 4C: Backend API and Repository Reorganisation

The plan as approved before task 2, archived unchanged from what was presented and
signed off. Where execution departed from it, the departures are recorded in
[`Completion-Report-v15.md`](Completion-Report-v15.md) rather than edited in here.

---


**Prompt:** `DEPLOY-FifaPressApp-4C-BackendApi-v15_0.xml`
**Branch:** `deploy/v15-backend-api` → PR against `main`, **left unmerged**
**Base:** `main` @ `c77a503` — confirmed via `git ls-remote`, exact match to `verified_state`

---

## Context

`main` today is a Blazor WebAssembly frontend with a mock data provider, living at
`src/FifaPressApp/` with its tests at `tests/FifaPressApp.Tests/`. Scope §7 frames 4B as the
"before" — the same UI reading from a mock — and 4C as the "after": the same UI, unchanged,
reading from a real API. Patch §5.1 adds SignalR and declares the hosting question blocking; the
deploy prompt resolves it (Azure App Service free tier, recorded in `backend/01_HOSTING-DECISION.md`).

Two things happen here, in strict order. First a **path-only reorganisation**, so a backend can sit
beside the frontend without an inspector guessing which files are which. Then the **backend itself**
— CRUD, validation, three middleware, one SignalR hub — plus its plain-language documentation.

The binding constraint is the **frontend invariant**: the deployed app must look and behave
identically when this run ends. Nothing here is a redesign, a namespace rename, or a database.

---

## Preflight — results

| Check | Result |
|---|---|
| `gh` CLI | `~/bin/gh`, authenticated as **jdsaire** (keyring), scopes include `repo`, `workflow` |
| `main` HEAD | **`c77a503`** — "Run 4F (v14): ShopEase Realignment", PR #11. Exact match |
| Governing inputs | All three present and readable: `P-PROTOTYPE_FIFA_Run4-Scope.md` (§7), `…-PATCH_v1.md` (§5.1, §8), `C4 Backend – Project…md` (tech stack only) |
| Target branch | `deploy/v15-backend-api` does not exist remotely. No open PRs |
| Frozen test files | `TwoRecordsTests.cs`, `LocalizedChangeTests.cs`, `LocalizedSearchTests.cs` — **zero** path dependency. Confirmed |
| Hardcoded-path list | Re-derived by fresh grep. The prompt's eight files are **exactly complete and accurate** |
| Link baseline | **333 internal links, 331 resolving** (methodology below) |
| Build + 512 tests | **Not yet run** — requires writes, blocked in plan mode. Runs as the first action on approval; a non-green baseline is a STOP |

### Two drifts from the prompt's assumptions

1. **A second pre-existing broken link.** The prompt names one (`handoff/v6/Completion-Report-v6.md:61 → v5/`).
   There are two: `learning-mode/02-access-record-frontend/02-Two-Themes-and-a-Pile-of-Hex-Codes.md:61`
   points at `ThemeTrigger.razor`, which v14 replaced with `AppearanceControl.razor`.
2. **`learning-mode/` is a sweep gap.** The citation-sweep guardrail names `.github/`, the csprojs,
   `tsconfig.json`, test files, root README, `docs/`, and folder READMEs — but not `learning-mode/`,
   which holds **37 clickable links** into `src/FifaPressApp/`. Unswept, the move introduces 36 new
   broken links and fails criterion 14. It is neither a frozen dossier nor a historical handoff
   record, and this run adds chapter `04-` to it. **Decision: sweep it** (D2).

Link-count methodology: fenced blocks and inline code spans stripped before matching; directory-style
targets resolve if any tracked file sits under the prefix. Same rules as v13/v14, but my counter is
broader than the one that produced v14's `264/265`, so **333/331 is this run's own baseline** and both
before and after figures come from the identical script.

---

## Citation sweep — the two lists

**Operational, MUST update** (a stale path breaks a build, a test, CI, a toolchain config, or a
reader's run instructions):

| File | What changes |
|---|---|
| `.github/workflows/deploy-pages.yml` | publish path + the comment naming the tracked `index.html` (2 hits) |
| `tests/frontend/FifaPressApp.Tests.csproj` | `ProjectReference` + CSV `Content Include` |
| `src/interop/tsconfig.json` | `outDir` → `../frontend/wwwroot/js` |
| `TestPaths.cs:18`, `LandingTests.cs:26`, `ThemeTriggerPlacementTests.cs:36`, `SignOutTests.cs:234`, `DisclosureTests.cs:348` | `Path.Combine(…, "..", "..", "src", "FifaPressApp")` → `"frontend"` |
| `LocaleTestData.cs:24` | same walk, `…/wwwroot/i18n` |
| `ThemePaletteTests.cs:25` | `Path.Combine(RepoRoot(), "src", "FifaPressApp", "wwwroot", "css", "app.css")` |
| `InteropTests.cs:38,69,76,225` | four path builds; **`:275`** asserts the literal `"outDir": "../FifaPressApp/wwwroot/js"` — updated to the new value, **not** weakened or deleted |
| `docs/how-to-run.md`, `docs/setup-guide.md`, `docs/grading-criteria.md` | 26 link-form citations + `dotnet run --project` / `dotnet test` commands |
| `README.md`, `tests/README.md`, `src/interop/README.md`, `src/frontend/README.md`, `src/frontend/wwwroot/README.md` | paths + the new where-things-live map |
| `learning-mode/` (10 files) | 37 link-form citations — **D2** |

Both moved test projects keep depth `tests/<dir>/`, so the `".." , ".."` walk length is **unchanged**;
only the `FifaPressApp` segment moves. Verified against the directory tree, not assumed.

**Frozen or historical, MUST NOT touch:** all of `ux-ui/` (including `04-evaluation/` and
`05-iteration/`, which also carry citations) and all of `handoff/v1`–`v14`. `07_BUILD-BRIEF.md` alone
holds 33 citations and stays byte-identical. `backend/06_REPO-MAP.md` carries the translation instead.

---

## Frontend files this run touches, and why each is permitted

| File | Edit | Authority |
|---|---|---|
| every file under `src/FifaPressApp/` | **path only** (`git mv`) | R12 |
| `src/frontend/FifaPressApp.csproj` | one package: `Microsoft.AspNetCore.SignalR.Client` | R13 |
| `src/frontend/Program.cs` | provider selection + SignalR client registration | hard rule 2(b), task 17 |
| `src/frontend/Services/ApiAccessDataProvider.cs` | **new** | task 17 |
| `src/frontend/Services/ChangeNotificationClient.cs` | **new** | task 17 |
| `src/frontend/Services/ChangeArrivalTracker.cs` | add `event Action? OnChanged` | R16 |
| `src/frontend/Pages/MyAccess.razor` | **`@code` only** — subscribe / unsubscribe / reuse existing `LoadRecordAsync()` | R16 |
| `src/frontend/wwwroot/appsettings.json` | **new**, empty `BaseUrl` placeholder | task 17 |

**No `.razor` markup, no stylesheet, no string, no route, no component, no page is changed.**
`MyAccess.razor`'s markup block is byte-identical; only its `@code` block gains a subscription that
nothing raises in mock mode.

---

## Reversals and deviations

| # | What | Authority |
|---|---|---|
| **R12** | Frontend dir moves `src/FifaPressApp` → `src/frontend`, superseding v7's destination. v7 is not wrong; a second project now exists and a folder named for the product no longer distinguishes anything | prompt |
| **R13** | One runtime package in the app project: `Microsoft.AspNetCore.SignalR.Client`. The frontend bundle is **no longer byte-identical to v14's**, for this reason alone | patch §5.1 |
| **R14** | `Microsoft.AspNetCore.Mvc.Testing` in `tests/backend` only — real in-process HTTP so middleware **order** and the 401 path are exercised, not just read | principal, at plan approval |
| **R15** | `Microsoft.AspNetCore.OpenApi` in `src/backend`, bare `AddOpenApi()`/`MapOpenApi()`, zero customisation | principal, at plan approval |
| **R16** | `MyAccess.razor` `@code` subscription so a pushed change repaints the record | principal, at plan approval |
| **D1** | Git identity `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>` — same account, uniform with all 14 prior runs | principal |
| **D2** | `learning-mode/` included in the sweep (37 links) | autonomous, forced by criterion 14 |
| **D3** | The stale `ThemeTrigger.razor` link retargeted to `AppearanceControl.razor`; prose untouched. Final count expected **333/332** | principal |
| **D4** | CORS lists the Pages origin **and** the local dev origins — Gate 4 runs on localhost, and `AllowCredentials` (required by SignalR) forbids `AllowAnyOrigin` | autonomous |
| **D5** | Logging registered **last** per the source document, so a 401 short-circuited by auth never reaches it. The ceiling wins; the tension is documented, not "improved" | guardrail |

---

## Backend design (fixed by the tech ceiling)

**Project:** `src/backend/FifaPressApp.Api.csproj`, assembly + root namespace `FifaPressApp.Api`,
`net10.0`. Tests: `tests/backend/FifaPressApp.Api.Tests.csproj`. Independent of the frontend project —
**no `ProjectReference` between them**; that independence is what makes the provider swap demonstrable.

**Models** — mirrored from the frontend, never imported. `AccreditationRecord` (CredentialId,
HolderName, Outlet, TrackId, HasNamedContact, Status, ValidUntil, ZoneAccess[], LastSyncedUtc);
`ChangeRecord` (ChangeId, CredentialId, WrittenUtc, EffectiveUtc, Kind, WhatChanged/Reason/NextStep
each `{en,es,pt}`, NextStepIsActionable, DecidedBy?, SupersedesChangeId?, AffectsMatchNumber?,
DependsOnMatchNumber?, ConditionText?). **Urgency is not transmitted** — it stays derived on the
frontend from the holder's track, preserving the "derived, never stored" invariant. Store seeded to
the same two demo records (`MP-2026-04817` Amina, `RH-2026-00219` Tomás), static singleton, in-memory.

**Endpoints**

| Verb | Route | Codes |
|---|---|---|
| GET | `/api/accreditations` | 200, 401 |
| GET | `/api/accreditations/{credentialId}` | 200, 401, 404 |
| POST | `/api/accreditations` | 201 + Location, 400, 401 |
| PUT | `/api/accreditations/{credentialId}` | 200, 400, 401, 404 |
| DELETE | `/api/accreditations/{credentialId}` | 204, 401, 404 |
| GET | `/api/accreditations/{credentialId}/changes` | 200, 401, 404 |
| POST | `/api/accreditations/{credentialId}/changes` | 201, 400, 401, 404 — **broadcasts over SignalR** |

Changes carry no PUT or DELETE: the record is append-only by domain rule, documented as a deliberate
constraint rather than a missing feature.

**Validation** — handwritten, no library. Rejects empty required fields, unknown enum values, an
empty zone list, and a `ValidUntil` in the past on an approved record. One consistent shape:
`{"error":"Validation failed.","details":{"holderName":["…"]}}`.

**Middleware**, registered **error → authentication → logging**, with a comment recording why:

1. `ErrorHandlingMiddleware` — try/catch → `500 {"error":"Internal server error."}`, never a stack trace.
2. `TokenAuthenticationMiddleware` — **simulated**. A fixed string compared with `==`. Reads
   `Authorization: Bearer …` or `?access_token=` (browsers cannot set headers on a WebSocket, so
   SignalR uses the query string). `401 {"error":"Unauthorized."}`. The OpenAPI document is exempt.
   Every comment and every document says plainly this is not authentication.
3. `RequestLoggingMiddleware` — method, path, response status code.

**Realtime** — one hub at `/hubs/changes`, one broadcast method, client method `ChangeRecorded`.
**CORS** — one named policy: `https://jdsaire.github.io` + local dev origins, `AllowCredentials`.

**Frontend swap** — `ApiAccessDataProvider` implements the existing `IAccessDataProvider`: accreditation,
changes and writes go over HTTP; **fixture reads delegate to an inner `MockAccessDataProvider`**,
because the API serves accreditation records only and the CSV plus its withholding rule are
frontend-owned. That gap is stated plainly in `02_API-REFERENCE.md` rather than papered over by
inventing fixture endpoints. Selection is config-driven: `wwwroot/appsettings.json` ships with
`Api:BaseUrl` **empty**, so the default is unchanged and an unreachable or sleeping API changes nothing.

---

## Commit sequence — 18 commits, one item each

Build + full suite verified and recorded at **every** boundary. PR opened immediately after commit 1
and updated after every commit thereafter; **never merged**.

| # | Message | Task |
|---|---|---|
| 1 | `refactor(layout): move the Blazor app to src/frontend` | 2 |
| 2 | `refactor(layout): move the test project to tests/frontend` | 3 |
| 3 | `ci(pages): publish the frontend from its new path` | 4 |
| — | **GATE 1 — frontend invariant. STOP.** | 5 |
| 4 | `feat(api): add the backend Web API project` | 6 |
| 5 | `feat(api): add the accreditation model and in-memory store` | 7 |
| 6 | `feat(api): add CRUD endpoints for accreditation records` | 8 |
| 7 | `feat(api): validate incoming request data` | 9 |
| — | **GATE 2 — API surface. STOP.** | 10 |
| 8 | `feat(api): add error-handling middleware` | 11 |
| 9 | `feat(api): add token authentication middleware` | 12 |
| 10 | `feat(api): add request and response logging middleware` | 13 |
| 11 | `feat(api): configure the middleware pipeline and CORS` | 14 |
| — | **GATE 3 — middleware demonstrated. STOP.** | 15 |
| 12 | `feat(api): add the SignalR hub for change notifications` | 16 |
| 13 | `feat(frontend): add an API-backed provider behind the existing interface` | 17 |
| 14 | `test(api): cover the endpoints, validation and middleware` | 18 |
| — | **GATE 4 — end-to-end. STOP.** | 19 |
| 15 | `docs(backend): add the backend documentation folder` | 20 |
| 16 | `docs(learning): add the backend and integration chapter` | 21 |
| 17 | `docs: update the repository map for the full-stack layout` | 22 |
| 18 | `docs: archive v15 plan and completion report` | 23 |

Commit 1 carries the `git mv` **and** every operational reference needed for the build to work
(tests csproj, `tsconfig.json`, all eight test files) — the suite must be green inside that one commit.
Documentation waits for commit 17.

---

## Documentation inventory

`backend/` — **documentation**, deliberately parallel to `ux-ui/`; `src/backend/` is the **code**.
The root README states that distinction in its own explicit line, because it is the single most
likely thing for an inspector to confuse.

- `backend/README.md` — why the folder exists; records R12–R16.
- `01_HOSTING-DECISION.md` — **the addendum.** Azure App Service free tier, hard no-cost constraint,
  Pages frontend calling the hosted API directly. States that this unblocks 4C, which patch §5.1
  declared blocked. Cross-references `ux-ui/03-ui-prototyping/07_BUILD-BRIEF.md` **by relative link**
  and explains in one paragraph that the brief is a frozen 4B document listing a backend under its own
  anti-scope-creep section (§5), that the freeze is why this decision lives here, and that **nothing
  in the brief has been edited**. Records cold starts after idle and that no paid Azure SignalR
  resource is used.
- `02_API-REFERENCE.md` — every route, verb, request/response shape and status code, as a table.
- `03_MIDDLEWARE-PIPELINE.md` — what middleware is, then each component and why the order matters,
  including D5's honest consequence.
- `04_REALTIME-SIGNALR.md` — persistent connections, tied back to the Access Record concept.
- `05_RUNNING-AND-DEPLOYING.md` — both halves locally; Azure steps written as instructions **for the
  principal**, since this run has no credentials and provisions nothing.
- `06_REPO-MAP.md` — before/after path map, naming which documents still cite old paths and why they
  were deliberately left alone.

Every one of these states plainly that the authentication is simulated.

`learning-mode/04-backend-and-integration/` — README plus chapters written **after** the code lands:
the first server this project has ever had; what middleware is and why order matters; a connection
that stays open; swapping a mock for the real thing without the screens noticing. New terms added to
`learning-mode/Glossary.md`.

`handoff/v15/` — `CC-PLAN-v15.md` (this file, as approved), `Completion-Report-v15.md`, `README.md`,
plus a new row in `handoff/README.md`. Convention confirmed against the live repo.

---

## Verification

**At every commit boundary:** `dotnet build -c Release`; `dotnet test tests/frontend` (≥512, never
below); from commit 14 also `dotnet test tests/backend`; and `git diff main --stat` empty for the four
frozen `ux-ui/` paths and the three frozen test files.

**Gate 1** — clean Release build; full suite; local `dotnet publish src/frontend -c Release` compared
file-by-file against a publish taken from `main` **before** the move; `git log --follow` traced across
the rename for one representative file in each moved project; frozen-path diffs empty.
→ **localhost URL:** `dotnet run --project src/frontend`.

**Gate 2** — every route, verb, request shape and status code reported; confirmation that no package
and no concept outside the ceiling was added.
→ **localhost URLs:** the API root and `/openapi/v1.json`.

**Gate 3** — live transcripts: valid token succeeding, invalid token returning 401, a deliberately
triggered exception returning the consistent JSON error rather than a stack trace, plus the log lines
actually produced.
→ **localhost URL:** the running API.

**Gate 4** — API and frontend running together, signed in, record rendering **from the API**; then a
change POSTed to the API reaching the running frontend over SignalR with no page refresh. Reported
honestly, including anything that did not work.
→ **localhost URLs:** frontend and API, both live.

**Final** — link count re-run with the identical script; expected **333/332** (D3 fixes one; the v6
defect carried forward unchanged and named). `git log` shows one author and committer across all 18
commits. Zero AI attribution and no vendor name from the C4 source document anywhere.

---

## Stop conditions I will honour mid-run

Baseline not green before any change · a frozen file needing to change for anything to work · a test's
intent not preservable across the move · a frontend behaviour change I cannot trace to a path edit
(revert, do not patch forward) · anything outside the tech ceiling appearing necessary, including any
further package · the Gate 4 SignalR demonstration not working · any instruction requiring invented
domain data or implied real security. Zero subagents. No token ever requested, printed or referenced.
