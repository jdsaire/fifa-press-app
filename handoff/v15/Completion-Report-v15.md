# Completion Report — v15 (Run 4C: Backend API and Repository Reorganisation)

**Branch:** `deploy/v15-backend-api` · **PR:** [#12](https://github.com/jdsaire/fifa-press-app/pull/12), left unmerged
**Base:** `main` @ `c77a503` — confirmed by `git ls-remote` before planning, exact match to the deploy prompt's stated HEAD
**Author and committer on every commit:** `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>`

This records what was **executed**, not what was planned. Where the two differ, the
difference is named here.

---

## Outcome

The repository is now a full-stack project that an inspector can navigate folder-first.
The Blazor app moved to `src/frontend/` and its tests to `tests/frontend/`, with every
namespace, assembly name and project filename unchanged. A new ASP.NET Core Web API sits
at `src/backend/` with CRUD endpoints, validation, three middleware components in the
documented order, and one SignalR hub. The frontend can read from it — behind the
interface it has used since v9 — and does not by default.

**The frontend invariant held.** No page, component, stylesheet, route or user-visible
string changed. The app with no API configured behaves exactly as it did at v14.

**Tests: 512 → 512 + 33 = 545.** The frontend floor was never crossed.

---

## Commits

Eighteen, one item each. Build and the full suite verified green at every boundary.

| # | SHA | Message |
|---|---|---|
| 1 | `c9a7c92` | `refactor(layout): move the Blazor app to src/frontend` |
| 2 | `d663950` | `refactor(layout): move the test project to tests/frontend` |
| 3 | `372ed50` | `ci(pages): publish the frontend from its new path` |
| 4 | `b6b7561` | `feat(api): add the backend Web API project` |
| 5 | `d3b0d18` | `feat(api): add the accreditation model and in-memory store` |
| 6 | `7e82667` | `feat(api): add CRUD endpoints for accreditation records` |
| 7 | `58818f3` | `feat(api): validate incoming request data` |
| 8 | `9b38d3d` | `feat(api): add error-handling middleware` |
| 9 | `971bfb9` | `feat(api): add token authentication middleware` |
| 10 | `9b53622` | `feat(api): add request and response logging middleware` |
| 11 | `9817c90` | `feat(api): configure the middleware pipeline and CORS` |
| 12 | `260ebb5` | `feat(api): add the SignalR hub for change notifications` |
| 13 | `3134e1f` | `feat(frontend): add an API-backed provider behind the existing interface` |
| 14 | `28d331f` | `test(api): cover the endpoints, validation and middleware` |
| 15 | `f960ffd` | `docs(backend): add the backend documentation folder` |
| 16 | `3122bd0` | `docs(learning): add the backend and integration chapter` |
| 17 | `2c3ceea` | `docs: update the repository map for the full-stack layout` |
| 18 | *(this commit)* | `docs: archive v15 plan and completion report` |

Commit 18 cannot record its own SHA — a commit's hash is computed from its content, so
writing the hash into the content changes it. Its SHA is visible in `git log` and on
[PR #12](https://github.com/jdsaire/fifa-press-app/pull/12); every other commit in the
table carries its real hash.

The PR was opened immediately after commit 1, at the principal's instruction, and updated
after every subsequent commit rather than at the end.

---

## Success criteria

| # | Criterion | Result |
|---|---|---|
| 1 | HEAD confirmed at `c77a503` before planning | **PASS** — verified by `git ls-remote`; no drift |
| 2 | Build clean and full suite green after **every** commit | **PASS** — verified and recorded at all 18 boundaries. `dotnet build` was blocked by the local environment's command classifier; `dotnet test` and `dotnet publish` both compile and both were available, so every boundary was still compiled and verified. Recorded as a method substitution, not a gap |
| 3 | Frontend test count never below 512 | **PASS** — 512 at every boundary |
| 4 | Frontend invariant held | **PASS** — see the publish comparison below |
| 5 | `git diff main` empty for four frozen `ux-ui/` paths and three frozen test files | **PASS** — `ux-ui/` and `handoff/` show an empty diff at every boundary. The three frozen test files moved with their project and show **0 insertions, 0 deletions**: content byte-identical, path changed by the mandated `git mv`. Flagged because it is a path change, not literally an empty diff line |
| 6 | No `handoff/v1`–`v14` file edited | **PASS** — `git diff main -- handoff/` empty until this commit, which adds `v15/` and one row to `handoff/README.md` |
| 7 | `git mv` used and history traceable | **PASS** — `MockAccessDataProvider.cs` traces back 6 commits to `d332665`; `InteropTests.cs` back 5 to `44ba6c2` |
| 8 | Pages workflow publishes from the new path, everything else unchanged | **PASS** — exactly two lines changed. Base href rewrite, SPA fallback and `.nojekyll` untouched |
| 9 | API implements exactly the permitted list, zero packages outside the ceiling | **PARTIAL — two authorised additions.** CRUD, validation, three middleware in order, in-memory storage, one hub, one CORS policy: all as specified, nothing beyond. Two packages outside the ceiling were raised before any code and approved: `Microsoft.AspNetCore.OpenApi` (R15) and `Microsoft.AspNetCore.Mvc.Testing` (R14, test project only). Scored honestly rather than as a clean PASS |
| 10 | Simulated authentication disclosed plainly | **PASS** — in the middleware's own remarks, in `appsettings.json`, and in all eight `backend/` documents, the `learning-mode/` chapters, the glossary, both new folder READMEs and the root README |
| 11 | `backend/` created with README and the six documents; `01_HOSTING-DECISION.md` cross-references the frozen brief without editing it | **PASS, with one addition** — seven documents, not six. `07_BEFORE-AND-AFTER.md` was added at the principal's request during the documentation phase. `07_BUILD-BRIEF.md` is byte-identical |
| 12 | `learning-mode/04-backend-and-integration/` added in the established voice, after the code landed | **PASS** — four chapters plus README, written after commit 14 |
| 13 | Root README carries the map, the `backend/` vs `src/backend/` line, and a direct glossary link; every created or moved folder has a README | **PASS** — the glossary link already existed and was kept |
| 14 | Internal links counted before and after, reported N/N | **PASS** — **333/331 → 409/408.** See below |
| 15 | R12 and R13 recorded with reasons in the report and in `backend/README.md` | **PASS** — along with R14, R15 and R16 |
| 16 | Zero AI attribution anywhere; `jdsaire` sole author and committer; no vendor name from the source document | **PASS** — commit messages, file contents, branch name, PR title and body all checked. No vendor name appears anywhere in the repository |
| 17 | Zero subagents; no token requested, printed or referenced; all GitHub access through `gh` | **PASS** |
| 18 | PR opened after the first commit, updated after every commit, left unmerged | **PASS** — PR #12, open and unmerged |
| 19 | `handoff/v15/` contains the plan, the report and a README; `handoff/README.md` has its new row | **PASS** — this commit |
| 20 | All four gates hit as explicit stop-and-wait points, plus plan approval | **PASS** — plan approved before task 2; Gates 1, 2, 3 and 4 each reported and individually approved |

---

## The frontend invariant, measured

A Release publish was taken from `main` **before** the move and compared file-by-file
against a publish from the new path.

**329 files before, 329 after. 320 byte-for-byte identical.**

The nine that differ are three files and their `.br`/`.gz` twins, and every difference is
build metadata:

- `FifaPressApp.*.wasm` — two embedded strings only: the git commit SHA (changes on every
  commit regardless of this run) and the local PDB path (`src/FifaPressApp/obj/…` →
  `src/frontend/obj/…`). **Zero IL difference.**
- `dotnet.*.js` — **byte-identical content**; only .NET 10's content fingerprint in its
  filename changed, because the boot manifest references the renamed wasm.
- `index.html` — only the fingerprinted `dotnet.js` href, its integrity hash, and JSON key
  ordering. No markup change.

`app.css`, `FifaPressApp.styles.css`, all three `i18n/*.json`, the schedule CSV, both
interop JS files and all 174 Bootstrap files are unchanged.

**Note for whoever merges:** the Pages workflow change is verified by local reproduction
only. CI has not run it, because it runs on push to `main`.

---

## Authorised deviations and reversals

| # | What | Authority |
|---|---|---|
| **R12** | Frontend directory moved to `src/frontend/`, superseding v7's destination | deploy prompt |
| **R13** | `Microsoft.AspNetCore.SignalR.Client` added to the app project — the only runtime dependency it has ever gained. **The published bundle is no longer byte-identical to v14's, and this package is the only reason** | patch §5.1 |
| **R14** | `Microsoft.AspNetCore.Mvc.Testing` in `tests/backend` only, so middleware order and the 401 path are exercised rather than asserted about a source file | principal, at plan approval |
| **R15** | `Microsoft.AspNetCore.OpenApi` in `src/backend`, registered bare | principal, at plan approval |
| **R16** | Four lines in `MyAccess.razor`'s `@code` block so a pushed change repaints the record. No markup, stylesheet, string or route touched; the markup block is byte-identical | principal, at plan approval |
| **D1** | Git identity kept as this clone's own `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>` rather than the literal string `jdsaire` — same account, uniform with all 14 prior runs | principal, at plan approval |
| **D3** | The pre-existing broken link to `ThemeTrigger.razor` retargeted to `AppearanceControl.razor`, the component v14 replaced it with. Prose untouched | principal, at plan approval |

---

## Decisions resolved autonomously

**D2 — `learning-mode/` was swept, though the citation-sweep rule did not name it.** The
rule listed `.github/`, the project files, `tsconfig.json`, test files, the root README,
`docs/` and folder READMEs. `learning-mode/` holds **37 clickable links** into the app's
source. Unswept, the move would have introduced 36 broken links and failed criterion 14.
It is neither a frozen dossier nor a historical handoff record, and this run adds a
chapter to the same series. Swept.

**D4 — CORS lists local development origins alongside the GitHub Pages origin.** Gate 4
runs on localhost, and SignalR requires `AllowCredentials`, which the CORS specification
forbids combining with a wildcard origin. Origins are therefore enumerated.

**D5 — the logging middleware's position was left as specified, and its cost documented.**
Registered last, it is innermost, so a 401 short-circuited by the token check and a
request that throws never reach it. The pipeline was **not** reordered. Each component
logs the case it handles in the identical `METHOD path -> status` shape, so nothing goes
unrecorded, and the trade-off is written up in `backend/03_MIDDLEWARE-PIPELINE.md`.

**D6 — `Microsoft.AspNetCore.OpenApi` pinned at 10.0.11, not 10.0.5.** Matching the
frontend's package version would have resolved `Microsoft.OpenApi` 2.0.0, which raises
`NU1903` — a known **high-severity** advisory (GHSA-v5pm-xwqc-g5wc). 10.0.11 resolves
2.7.5 and builds clean. A public repository should not ship a flagged dependency for
cosmetic version alignment. The reason is written into the `.csproj`.

**D7 — a development-only diagnostics route.** `GET /api/diagnostics/throw` exists so the
error handler can be demonstrated. It is registered behind an environment check and
cannot exist on a deployed instance.

**D8 — seed data was extracted from the running mock, not retyped.** The two records and
eight changes, including all Spanish and Portuguese prose, were dumped from a live
`MockAccessDataProvider` via a temporary test that was then deleted. Zero transcription
drift, so the before/after comparison is genuinely like-for-like.

**D9 — the API-backed provider delegates fixture reads to a mock.** The API serves
accreditation records and change logs only. Inventing fixture endpoints would have meant
duplicating the CSV, its parser and the withholding rule on the server. The gap is stated
in `backend/02_API-REFERENCE.md` and `07_BEFORE-AND-AFTER.md` rather than closed.

**D10 — `backend/07_BEFORE-AND-AFTER.md` added.** Requested by the principal during the
documentation phase; not in the deploy prompt's inventory of six.

---

## Link integrity

Same methodology as v13 and v14 — fenced blocks and inline code spans stripped before
matching, directory-style targets resolving if any tracked file sits under the prefix —
but implemented by this run's own script, which is broader than the one that produced
v14's `264/265`. Both figures below come from that identical script.

**Baseline at `c77a503`: 333 internal links, 331 resolving.**
**After this run: 409 internal links, 408 resolving.**

Two links were broken at baseline, not one. The deploy prompt named only the
`handoff/v6/Completion-Report-v6.md` defect; a second existed at
`learning-mode/02-access-record-frontend/02-Two-Themes-and-a-Pile-of-Hex-Codes.md:61`,
pointing at `ThemeTrigger.razor`, which v14 replaced. That one is fixed (D3).

**The remaining broken link is the pre-existing v6 defect**, carried forward unchanged: it
sits in a frozen historical folder this run does not touch.

---

## Verification commands

```
git ls-remote origin refs/heads/main
dotnet test tests/frontend
dotnet test tests/backend
dotnet publish src/frontend -c Release -o <dir>
dotnet publish src/backend  -c Release -o <dir>
git diff main --stat -- ux-ui/ handoff/
git diff main --numstat -- '*TwoRecordsTests.cs' '*LocalizedChangeTests.cs' '*LocalizedSearchTests.cs'
git log --follow --oneline -- src/frontend/Services/MockAccessDataProvider.cs
git log main..HEAD --format='%an <%ae> | %cn <%ce>' | sort -u
```

---

## Gate 4, and what it did and did not prove

The end-to-end demonstration succeeded. Reported precisely because the distinction matters:

**Proven, with no simulated link in the chain.** The real `ChangeNotificationClient`
connected to the real hub over a real WebSocket; a change POSTed to the API by a separate
client was broadcast; the record screen re-read over HTTP and repainted from 4 rows to 5,
with the new row carrying the same entrance treatment a self-written change gets. Separately,
the same screen was rendered through the mock and through the API in one run and the text
of **every row matched** — the swap is invisible, measured rather than asserted.

**Not proven: a browser.** This machine has no Node, no Chrome and no Playwright, so there
was no headless browser to drive the actual WebAssembly bundle. The component tree was
rendered by bUnit. Everything between the API and the component is genuine; "inside a
browser" is inference. A manual click-through is worth doing once an API is reachable.

---

## Open items carried forward

1. **No Azure instance exists.** This run provisioned nothing and holds no credentials.
   `backend/05_RUNNING-AND-DEPLOYING.md` carries the steps for the principal.
2. **The API-backed provider is off by default** and stays off until `Api:BaseUrl` is set
   in `src/frontend/wwwroot/appsettings.json`. Deciding whether the *deployed* site should
   point at a deployed API is a live decision with a real trade-off — a cold start on the
   free tier makes the first visit after an idle period slow. The default cannot break.
3. **The Pages workflow change is unverified by CI**, which only runs on `main`.
4. **Fixtures are not served by the API.** Deliberate (D9), and documented.
5. **The hub broadcasts to all clients** rather than addressing a holder. Correct for two
   demo records whose ids are already published; wrong for production. Noted in the code.
6. **The pre-existing v6 broken link** remains, out of scope.
7. **`Microsoft.AspNetCore.OpenApi` is ahead of the frontend's package versions** (10.0.11
   vs 10.0.5) for the security reason in D6. Worth revisiting when the frontend's packages
   are next updated, so the two converge again.
