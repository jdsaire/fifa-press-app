# Plan — Run 4B-R / v10: Frontend Course Correction + Test Foundation

*The plan as approved before this run started. Reproduced from the approval, not redrafted after
the fact.*

## Context

v9 shipped the Access Record frontend vertical slice (`main` @ `33c6051`). Direct use of the
deployed build surfaced defects and gaps the nine-gate dossier did not anticipate, resolved in
`P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md`. This run — 4B-R, v10 — delivers only the items with
**no design dependency**, so it can proceed before the 4D design addendum exists:

1. **The Submitting state never renders.** `MockAccessDataProvider.RequestMatchAccessAsync` returns
   `Task.FromResult(...)` — an already-completed task — so the continuation in
   `Registration.razor.SubmitAsync` runs synchronously and no render pass ever occurs with
   `submitting == true`. `RequestAccessForm.razor` already implements the state correctly
   (disabled fields, "Sending request…" label); the provider is what makes it unobservable.
   `05_SCREENS.md` §5.2 specifies the state, so this is a defect, not a gap.
2. **No iconography on the match surfaces** — date, venue, phase.
3. **Search is the only way to narrow 104 fixtures.** Group and match-status filters are both
   derivable from data the app already holds.
4. **No committed test project.** v9's verification used a throwaway harness deliberately kept
   outside the repo, so every run since has improvised one.

Intended outcome: one PR against `main`, left unmerged, containing eight commits — the repo's first
committed test project plus four behaviour patches, each covered by tests as it lands.

**Not this run:** visual identity, dark-palette re-derivation, theme-trigger relocation,
authentication, the public landing view, EN/ES/PT, progressive disclosure, FAQ-style Help, and
TypeScript interop. Those are 4D's to specify and 4E's to build.

---

## Preflight results (task 0) — all PASS

| Check | Result |
|---|---|
| GitHub access | `gh` at `~/bin/gh`, authenticated as `jdsaire` (keyring), scopes `gist, read:org, repo, workflow` |
| Scope patch present | `P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md` read in full this session; §5.2 authorizes the dependency reversal, §6 enumerates 4B-R's scope |
| .NET SDK | `10.0.201` |
| HEAD on `main` | `33c6051` — matches `verified_state` exactly. PR #6 merged; no open PRs |
| Baseline build | `dotnet build src/FifaPressApp -c Release` → **0 warnings, 0 errors** |

**Every line-number citation in `verified_state` was re-verified against the live files and all are
accurate.** No drift to report. Additionally confirmed: zero hits for "filter" across
`ux-ui/03-ui-prototyping/`; no solution file anywhere; `.gitignore` is exactly `bin/` + `obj/`;
`index.html` links `FifaPressApp.styles.css`, so a new scoped stylesheet works without wiring.

**Working location.** The existing clone at `Fifa-Press-App/fifa-press-app` (clean tree, on the
merged `deploy/v9-access-record-frontend` branch) was fetched, checked out to `main`, and hard-reset
to `origin/main`, with HEAD verified as `33c6051` before the working branch was created. All
planning reads and toolchain probes ran against a separate throwaway clone in the scratchpad; the
working clone was untouched during planning.

---

## Open decisions, resolved (task 1)

**(a) `{N}` = v10.** `handoff/` holds `v1`–`v9`, each with a plan, a Completion Report and a folder
README, indexed by a bullet in `handoff/README.md`. Destination: `handoff/v10/` with
`CC-PLAN-v10.md`, `Completion-Report-v10.md`, `README.md`.

**(b) Test stack — versions confirmed by building and running, not assumed.** A throwaway probe
project targeting `net10.0` with a `ProjectReference` to the real app compiled clean and ran 3 tests
green, including a bUnit render of `Pages/EventList` with a stubbed `IAccessDataProvider`:

| Package | Version | Verified |
|---|---|---|
| `Microsoft.NET.Test.Sdk` | `18.9.0` | restores, builds, runs |
| `xunit` | `2.9.3` | restores, builds, runs |
| `xunit.runner.visualstudio` | `3.1.5` | discovers and runs |
| `bunit` | `2.9.0` | **resolves and renders on `net10.0`** |

**Component testing is therefore IN SCOPE — the xUnit-only fallback does not apply.** Use
`BunitContext`, not the obsolete `TestContext`, which emits `CS0618` and would break the zero-warning
rule. Also verified in the probe: `MockAccessDataProvider` is constructible in a test via an
`HttpMessageHandler` stub serving the tracked CSV, so the whole-schedule withholding assertion needs
no new mocked data.

**(c) Solution file — NO.** `dotnet test tests/FifaPressApp.Tests` with an explicit project path
works (proven in the probe). The repo root gains no new file and CI stays provably untouched.

**(d) Group filter option set — CONFIRMED as proposed.** "All groups" · each distinct `GroupLetter`
present in the data, sorted · "Knockout rounds" for `GroupLetter is null`. Derived at render time,
never hardcoded. Verified against the real CSV: exactly 12 distinct letters (A–L) across 104
fixtures. Without the knockout option 32 of 104 fixtures would be reachable only under "All".

**(e) Status filter option set — CONFIRMED as proposed.** "All matches" / "Played" / "Not yet
played", derived from `IsResolved` and nothing else. No date comparison in the UI.

**(f) Write latency — `600` ms**, as a named `private static readonly TimeSpan
SimulatedWriteLatency = TimeSpan.FromMilliseconds(600)` on `MockAccessDataProvider`, commented in
the file's existing voice as a simulation device with the same standing as `SimulatedNow` — it
disappears the moment a real provider replaces the mock. Applied on the **write path only**;
`GetFixturesAsync`, `GetFixtureAsync`, `GetAccreditationAsync` and `GetChangesAsync` are untouched,
preserving v9's verified no-spinner first render. `WithdrawRequestAsync` is not touched.

**(g) Link-integrity baseline — 274/275.** Method, in one line: across all git-tracked `.md` files,
every inline `[text](target)` link outside fenced code blocks and inline code spans, excluding
`http(s):`/`mailto:`/`tel:`, resolved as a filesystem path **and**, where the link carries a
`#fragment` into a file, as a GitHub-style heading slug in that file. The single pre-existing
failure is `handoff/v6/Completion-Report-v6.md → v5/` — a historical `handoff/` record this run is
forbidden to alter. It will be named and carried at the same count, not absorbed.

**(h) Archival destination — `handoff/v10/`**, discovered by inspecting the live tree (see (a)).

**Open decision raised, and answered by the principal:** a `learning-mode/` chapter on the test
project is **deferred** to a later run, per the patch's §6 scope and this repo's convention that
chapters are authored after the code they describe has settled. Carried forward as an open item.

---

## Architecture

**New**

- `tests/README.md`, `tests/FifaPressApp.Tests/README.md` — why the folder exists, cross-linked
- `tests/FifaPressApp.Tests/FifaPressApp.Tests.csproj` — **the only place any new package lives**;
  `ProjectReference` to the app; a `<Content Include>`/`Link` so tests read the *tracked* CSV by
  relative path with no second copy in git
- `tests/FifaPressApp.Tests/TestData.cs` — shared helpers: CSV loader, the `HttpMessageHandler`
  stub, a `StubAccessDataProvider`, and fixture builders that **never** carry a team name
- `src/FifaPressApp/Services/FixtureQuery.cs` — pure static helper; holds the search predicate
  (moved verbatim), the two filter predicates, the group-option derivation, and a
  `MatchStatusFilter` enum. Extracted so the query is testable without rendering a component
- `src/FifaPressApp/Components/Icon.razor` + `Icon.razor.css` — one component, a named SVG set
  (`date` · `location` · `phase`), so the markup exists once rather than three times

**Modified**

- `Components/EventCard.razor` — date + location icons, **read-only branch only** (lines 36–44).
  The editing branch and the `[Parameter]`/`EventCallback` contract are untouched
- `Pages/EventList.razor` + `.razor.css` — phase icon, two labelled selects, calls `FixtureQuery`
- `Pages/EventDetails.razor` (`detail__kickoff`) and `Pages/Registration.razor` (`request__meta`) —
  phase icon on the existing meta lines
- `Services/MockAccessDataProvider.cs` — simulated write latency, write path only
- `Components/README.md`, `Services/README.md` — index `Icon.razor` and `FixtureQuery.cs`
- `README.md` (Tech Stack), `docs/how-to-run.md` — the test command
- `handoff/README.md` — v10 row

**Unmodified, verified with `git diff` at the end:** `FifaPressApp.csproj` · `.github/workflows/` ·
`ux-ui/` · `wwwroot/lib/` · `learning-mode/` · `handoff/v1`–`v9` · `Properties/launchSettings.json` ·
`Components/RequestAccessForm.razor`.

---

## Commit sequence

| # | Message | Task |
|---|---|---|
| 1 | `test: establish test project with importer and provider invariant coverage` | 2 |
| — | **GATE 1 — stop for approval** | 3 |
| 2 | `fix(request): make the submitting state observable on the write path` | 4 |
| 3 | `feat(matches): add date, venue and phase iconography to match surfaces` | 5 |
| — | **GATE 2 — stop for approval** | 6 |
| 4 | `refactor(matches): extract fixture search into a pure query helper` | 7 |
| 5 | `feat(matches): add group filter to the match list` | 8 |
| 6 | `feat(matches): add match status filter to the match list` | 9 |
| — | **GATE 3 — stop for approval** | 10 |
| 7 | `docs: document the test project and the new match-list controls` | 11 |
| — | Push branch, open PR against `main`, **leave unmerged**; run verification | 12–13 |
| 8 | `docs: archive v10 frontend course correction plan and completion report` | 14 |

Branch: `deploy/v10-frontend-course-correction`, created by that exact name. Author and committer
`jdsaire` on every commit. No trailers, no attribution of any kind in any message, branch name, PR
title, PR body, file, or test name.

---

## Test inventory (~37 tests planned, one line each)

**Commit 1 — invariants that protect what v9 established**

*`FixtureImporterTests.cs`*
1. `24:00` on the Eastern column rolls to `00:00` the next day — the three real rows (CSV lines 7, 21, 37)
2. `d-MMM-yy` parses identically under a non-English `CurrentCulture` — protects `InvariantCulture`
3. The matchup column splits on `" v "` into two non-empty sides
4. A row with ≠8 columns throws `FormatException` naming the line number
5. A `Group X` row carries the letter; a knockout row carries `null`
6. The tracked CSV parses to exactly 104 fixtures
7. Every fixture out of the importer has `HomeLabel` and `AwayLabel` null

*`MockAccessDataProviderTests.cs`*
8. **No fixture with `IsResolved == false` carries either team name, across the whole schedule** — the highest-value assertion in the project
9. A request appends exactly one change and returns it
10. A withdrawal appends rather than removes; the withdrawn change stays in place
11. The changes list is ordered by effective date, newest first

**Commit 2 — the loading-state regression**

12. `RequestMatchAccessAsync` returns a task that is **not already complete** — fails against v9's synchronous write
13. The change it produces is unchanged in content from what v9 wrote
14. *(bUnit)* `RequestAccessForm` with `Submitting="true"` disables both inputs and the button and shows "Sending request…"

**Commit 3 — iconography**

15. Each of the three icons renders `aria-hidden="true"`, `focusable="false"` and `currentColor`, and no `fill`/`stroke` literal
16. *(bUnit)* `EventCard`'s read-only branch keeps its `<time datetime>` attribute and rendered text with the icon added
17. *(bUnit)* the meta line still renders `PhaseLabel` as text beside the phase icon

**Commit 4 — search preservation (written before any filter exists)**

18–21. `[Theory]` table hitting each of the four matched fields: `DisplayLabel`, `Venue`, `City`, `PhaseLabel`
22. Case-insensitivity, both directions
23. Empty and whitespace-only input return everything
24. A term matching nothing returns empty
25. **Equivalence:** for a table of terms over the whole real schedule, `FixtureQuery.Search` sequence-equals a literal copy of v9's inline predicate held in the test — the direct proof for success criterion 4

**Commit 5 — group filter**

26. The option set equals the distinct groups present in the data, sorted, plus knockout
27. Selecting a letter returns only that group's fixtures
28. "Knockout rounds" returns only fixtures with a null `GroupLetter`
29. Group composes with search as AND
30. No group selection surfaces a team name for an unresolved fixture
31. *(bUnit)* the select is labelled and resets `currentPage` to 1 on change

**Commit 6 — status filter**

32–34. Each option returns the expected partition; the two non-"All" partitions are complementary and sum to 104
35. Status composes with both group and search
36. "Not yet played" carries no team names, and "Played" is the only partition that carries any
37. *(bUnit)* the select is labelled and resets `currentPage` to 1 on change

*(The actual count landed at 82, not ~37 — see the Completion Report for what grew and why.)*

---

## Copy and source-comment decisions

**Empty state**, extended for three composing controls — plainest accurate sentence, naming only the
active controls and inventing no explanation:

- No control active → `No matches to show.` *(unchanged from v9)*
- Any control active → `No matches found. Narrowing this list right now: {active controls}.`
- …and **whenever a search term is present**, v9's existing sentence is appended verbatim:
  `Fixtures that have not been played yet do not list their teams, so searching by team name will not find them.`

**Confederation filter — flagged, not built.** A short comment in the filter block of
`EventList.razor` recording that the tracked CSV has eight columns and no confederation among them,
so the filter would require an invented team→confederation lookup — the same category of mocked data
as `TimeZoneLabel` — and stays unbuilt until new mocked data is authorized. Carried forward in the
Completion Report with its data prerequisite named.

**Icons** are decorative geometry authored in this repo — no icon font, no package, no CDN, no logo
or brand asset. They inherit `currentColor`, so they take `--color-text` inside `EventCard`
(17.40:1 light / 15.29:1 dark) and `--color-stale-text` on the meta lines (5.33:1 / 8.93:1) — every
one already above the 4.5:1 text floor and the 3:1 non-text floor in both palettes, using tokens
whose ratios `app.css` already carries. No new colour is introduced, so no 4D design question is
touched.

---

## Things this prompt does not explicitly authorize, listed so they can be refused

1. `MatchStatusFilter` — a new public enum in the app project, in `Services/FixtureQuery.cs`. The
   status filter needs a vocabulary; a bare `bool?` would read worse. No package, no bundle impact.
2. `tests/FifaPressApp.Tests/TestData.cs` — a shared test-support file (CSV loader, HTTP handler
   stub, stub provider, fixture builders). Not "one test file per unit under test", but the
   alternative is copying the same stub into five files.
3. The tracked CSV is reached via a `<Content Include Link>` in the test `.csproj` rather than a
   runtime directory walk. It is still the tracked file, still by relative path, with no copy in git.
4. bUnit component tests (items 14, 16, 17, 31, 37). Conditionally authorized by task 1(b) and the
   condition is met, but flagged because it is the larger of the two branches that task allowed.
5. **Not proposed, and named so it is a decision:** the root README and `docs/how-to-run.md` still
   describe the pre-v9 app — matches at `/`, a `/register/{id}` route, a "Registered" badge, and a
   walkthrough that follows them. Only the test command is added; the drift is left alone and
   carried forward as an open item in the Completion Report.

---

## Gate boundaries

Three hard stops. At each: the commits (short SHA + message), the boundary's specific evidence, and
build + test results, then wait. **No line of the next boundary's code written before approval.**
Gate 1 additionally reports the resolved package set with exact versions, that component testing is
in scope, and that both `FifaPressApp.csproj` and the workflow are untouched.

---

## Verification (task 13)

Run at the end, PASS/FAIL per check with the evidence in the result:

- `dotnet build src/FifaPressApp -c Release` clean — checked after each commit individually
- `dotnet test tests/FifaPressApp.Tests` green after every commit from commit 1 onward, with counts
- `git diff 33c6051..HEAD --` empty for `src/FifaPressApp/FifaPressApp.csproj`, `.github/workflows/`,
  `ux-ui/`, `src/FifaPressApp/wwwroot/lib/`, `learning-mode/`, `Properties/launchSettings.json`,
  `Components/RequestAccessForm.razor`, and `handoff/v1`–`v9`
- Search behaviour unchanged — cited by name
- The withholding rule holds across the whole schedule — cited by name
- Internal links re-measured with the task-1 method and reported against the 274/275 baseline
- `git log --format='%an|%cn'` shows only `jdsaire`; zero attribution leakage anywhere
- The published bundle is unaffected

---

## Stop conditions honoured mid-run

Stop and report rather than deciding: any patch that turns out to need a colour, a component shape
not already in the repo, or copy that states a position (4D decides those); search behaviour that
cannot be preserved exactly; the withholding rule that cannot be held; any required change to
`FifaPressApp.csproj`, `.github/workflows/`, or `ux-ui/`; or a test that would need data the app
does not have.
