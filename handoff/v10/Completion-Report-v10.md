# Completion Report: v10 Frontend Course Correction + Test Foundation

Run 4B-R, per `P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md` §6. Executed against `main` @ `33c6051` on
branch `deploy/v10-frontend-course-correction`.

## Commits

| # | SHA | Message |
|---|---|---|
| 1 | `36cecf1` | `test: establish test project with importer and provider invariant coverage` |
| 2 | `e084a7d` | `fix(request): make the submitting state observable on the write path` |
| 3 | `07c1cdb` | `feat(matches): add date, venue and phase iconography to match surfaces` |
| 4 | `c4e7b87` | `refactor(matches): extract fixture search into a pure query helper` |
| 5 | `ce48c8b` | `feat(matches): add group filter to the match list` |
| 6 | `6d11fc7` | `feat(matches): add match status filter to the match list` |
| 7 | `e681091` | `docs: document the test project and the new match-list controls` |
| 8 | `aff7419` | `docs: archive v10 frontend course correction plan and completion report` |
| 9 | *(this commit)* | `docs: correct the v10 archive's link-integrity count` |

**Pull request:** [#7](https://github.com/jdsaire/fifa-press-app/pull/7), opened against `main`,
left unmerged per push policy.

## Outcome

Every invariant the plan named to protect held for the entire run. Search behaviour is proven
byte-for-byte equivalent to what shipped in v9 — the extraction commit changed nothing a reader
could observe, verified by comparing a literal copy of the original inline predicate against the
extracted `FixtureQuery.Search` across 14 terms over the whole 104-fixture schedule. The withholding
rule — no fixture whose kickoff has not passed ever carries a team name — was proven at the
importer, at the provider, under both new filters in every combination, and under search, all
against the real tracked schedule rather than hand-built data standing in for it. The app `.csproj`
and every frozen path (`.github/workflows/`, `ux-ui/`, `wwwroot/lib/`, `learning-mode/`,
`Properties/launchSettings.json`, `Components/RequestAccessForm.razor`, `handoff/v1`–`v9`) are
byte-identical to `33c6051`, confirmed by an empty `git diff` on each path individually. The
regression test for the loading-state defect was verified to actually regress — stashing the fix
and re-running the suite reproduced two failures, which is the standard the plan set for calling it
proven rather than merely asserted.

## PASS/FAIL against `success_criteria`

| # | Criterion | Result | Evidence |
|---|---|---|---|
| 1 | Submitting state renders, proven by a test that fails against v9's synchronous write | **PASS** | `RequestSubmittingStateTests.WritePathReturnsATaskThatHasNotAlreadyCompleted`. Verified failing against the pre-fix provider by stashing the change and re-running: 2 failures, `Assert.False() Failure` |
| 2 | No read path gained latency; §5.2's state satisfied without disturbing the no-spinner render | **PASS** | `ReadPathsGainedNoDelay` asserts all four read methods still return already-completed tasks. `600ms` `SimulatedWriteLatency` applied on the write path only |
| 3 | Date, location, phase icons on all three match surfaces, decorative, theme-inheriting, `time` elements intact, WCAG 2.2 AA met in both themes | **PASS** | `IconTests` (5 tests). Contrast recomputed, not quoted: `--color-text` 17.40:1 light / 15.29:1 dark; `--color-stale-text` 5.33:1 / 8.93:1 — all above the 4.5:1 text / 3:1 non-text floors |
| 4 | Search byte-for-byte equivalent to v9's, proven by tests written before the filters | **PASS** | `FixtureQuerySearchTests` (10 tests, committed in `c4e7b87` before either filter existed), `ExtractedSearchAgreesWithThePredicateAsItShipped` compares 14 terms across the real schedule |
| 5 | Group and status filters compose with search and each other, derive from existing data, reset pagination, labelled for AT | **PASS** | `FixtureQueryGroupTests`, `FixtureQueryStatusTests` (22 tests). `ChangingAControlReturnsToTheFirstPage` proves the page reset by paging to 3 of 3, filtering to 5 fixtures, and asserting the pager disappears |
| 6 | No confederation filter; absence commented in source, carried in this report with data prerequisite | **PASS** | Comment block in `EventList.razor` above the filter row. Prerequisite: a team→confederation lookup covering all 48 teams, not present in the 8-column tracked CSV |
| 7 | Test project under `tests/`, runs green, covers importer hazards, whole-schedule withholding, append-only log, query helper, and every item this run added | **PASS** | `tests/FifaPressApp.Tests/`, 82 tests, 0 failures |
| 8 | `FifaPressApp.csproj` unmodified; every new package in the test project only; bundle unaffected | **PASS** | `git diff 33c6051..HEAD -- src/FifaPressApp/FifaPressApp.csproj` → 0 changed files. Packages live only in `tests/FifaPressApp.Tests/FifaPressApp.Tests.csproj` |
| 9 | `.github/workflows/`, `ux-ui/`, `wwwroot/lib/`, `learning-mode/`, `handoff/v1`–`v9` unmodified | **PASS** | Each path individually diffed against `33c6051..HEAD`: 0 changed files |
| 10 | Build clean and tests green after every commit, verified individually | **PASS** | Rechecked out each of the 7 SHAs and rebuilt: `0 Warning(s)`, `0 Error(s)` at every one; test count climbed 21 → 27 → 34 → 60 → 71 → 82 with 0 failures throughout |
| 11 | Every gate hit and approved before its successor began | **PASS** | Three gates reported and approved before task 4, task 7, and task 11 respectively |
| 12 | Every new folder has a README; root README and how-to-run carry the test command; links report N/N with historical failures named and unchanged | **PASS** — see link count below | `tests/README.md`, `tests/FifaPressApp.Tests/README.md` present. Test command in both `README.md` and `docs/how-to-run.md` |
| 13 | All commits on `deploy/v10-frontend-course-correction`; PR opened against `main`, unmerged; solely `jdsaire`; zero AI attribution anywhere | **PASS** | `git log --format='%an|%cn' 33c6051..HEAD` → single line, `Juan Diego S.|Juan Diego S.` for all 8 commits. Full diff content, the branch name, and the PR title and body were each swept against a list of AI product, agent, and vendor name patterns — zero hits |
| 14 | Zero subagents used; no PAT requested, printed, or referenced | **PASS** | Single-agent run throughout. `gh` used exclusively for all GitHub interaction; no token printed |
| 15 | Plan and Completion Report archived with folder README; `handoff/README.md` updated; §5 reversal recorded citing the patch; no AI attribution in either file | **PASS** | This folder. `handoff/README.md` v10 row added (in commit `e681091` — see deviation below) |

## Link-integrity sweep

Method (stated once, per the plan): every inline `[text](target)` link in every git-tracked
`.md` file, outside fenced code blocks and inline code spans, excluding `http(s):`/`mailto:`/
`tel:`, resolved as a filesystem path and, where it carries a `#fragment`, as a GitHub-style
heading slug in the target file.

| Point | Result |
|---|---|
| Baseline (task 1, before any change) | 274/275 |
| After the docs commit (`e681091`), before this archive commit | 280/282 (`handoff/README.md → v10/` fails only because the folder did not exist yet) |
| **After the archive commit (`aff7419`), the last documentation commit of this run** | **283/284** |

The one persistent failure at every measurement point is `handoff/v6/Completion-Report-v6.md → v5/`
— inside a historical `handoff/` record this run is forbidden to alter. Named at the baseline and
unchanged in count throughout. The link total grew from 275 to 284 because this run added eight new
markdown files (two `tests/` READMEs and the three files in this archive folder, plus revisions to
`handoff/README.md` and the two `src/` folder READMEs) each carrying their own cross-links.

## Authorized deviations from the plan

1. **The `07_BUILD-BRIEF.md` §5 dependency reversal**, authorized by
   `P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md` §5.2. Four packages —
   `Microsoft.NET.Test.Sdk 18.9.0`, `xunit 2.9.3`, `xunit.runner.visualstudio 3.1.5`,
   `bunit 2.9.0` — were added, exclusively to `tests/FifaPressApp.Tests/FifaPressApp.Tests.csproj`.
   `src/FifaPressApp/FifaPressApp.csproj` is untouched and the published bundle gains nothing.

2. **Test count landed at 82, not the ~37 the plan sketched.** The plan's inventory was a floor,
   not a ceiling: several tests were added beyond it where a single assertion would have left a
   gap a careful reader would notice — e.g. `Eastern2500_IsRejected_BecauseOnly2400HasAMeaning`
   (proving the `24:00` roll doesn't silently accept other invalid hours),
   `PlayedFixture_NamesItsSides` (the converse of the withholding assertion, so the suite can't
   pass against a provider returning nothing useful), and `WithNoControlActiveTheEmptyStateStaysAsItWas`.
   None of these change what the plan committed to; they cover the same commits more thoroughly.

3. **`handoff/README.md`'s v10 row landed in the docs commit (`e681091`, task 11) rather than
   in this archive commit (task 14) as the prompt's task list literally specifies.** The plan's
   architecture section listed `handoff/README.md` as a modified file without pinning it to a
   specific commit, and it was bundled with the other README updates in task 11's single docs
   commit rather than split across two. The content is correct and complete either way; this is a
   sequencing note, not a content gap.

4. **A ninth commit corrects this report's own link-integrity numbers**, in place of amending the
   archive commit. The number first written for the post-archive measurement was estimated rather
   than re-run against the actual committed tree, and came out wrong (`281/282` written versus
   `283/284` measured). Caught by re-running the same tool used at every other measurement point
   immediately after the archive commit landed. Fixed as a new commit rather than an amend, per
   standing git-safety practice; flagged here as the genuine cause to split this run's task 14 into
   two commits.

5. **Component testing was in scope, not the xUnit-only fallback** — confirmed at plan time by
   building and running bUnit `2.9.0` against `net10.0` before any implementation began. This is
   not a deviation from the plan (the plan's task 1(b) anticipated exactly this branch), but is
   restated here because the prompt's guardrails discuss the fallback at length and a reader
   checking this report against them should see plainly that the fallback did not apply.

## Decisions resolved autonomously

1. **The `MatchStatusFilter` enum's member names** (`All`, `Played`, `NotYetPlayed`) — not
   specified anywhere; chosen to read naturally in both the C# call sites and the `nameof()`-based
   `<option>` values in the markup.
2. **The order of composition inside `FixtureQuery.Apply`** — Search → Group → Status. Not
   specified; chosen because it matches the left-to-right reading order of the controls on screen,
   and because Search is the one behaviour this run is forbidden to alter, so applying it first and
   unconditionally makes that guarantee visible in the code rather than only in the tests.
3. **`GroupLetters` and `InGroup` compare group letters with `OrdinalIgnoreCase`**, matching the
   comparison style `FixtureQuery.Search` already uses, rather than introducing a second string-
   comparison convention into the same file.
4. **The empty-state sentence joins active controls with `", "` and a trailing period**, rather
   than an oxford-comma "and" construction — chosen for the same reason the plan gave for
   preferring the plainest accurate sentence: a list of clauses reads more predictably than a
   sentence that has to handle one, two, or three items grammatically.
5. **`RequestSubmittingStateTests.cs` was kept as its own file** rather than folded into
   `MockAccessDataProviderTests.cs`, even though several of its tests exercise the same provider —
   because it is testing a behavioural regression tied to one specific defect and gate, and a
   reader tracing "why does this test exist" benefits more from a file boundary that matches the
   defect than from one that matches the class under test.

## Open items carried forward

Restating v9's list unresolved by this run, plus what is new here:

| Item | Status |
|---|---|
| Venue access list ownership | Unresolved upstream, from v9 |
| The interval premise (ID-01) | Untested by declared constraint, from v9 |
| W5's navigation objection | Open, from v9 |
| Withdrawal has no UI affordance | Unchanged from v9 — `WithdrawRequestAsync` was explicitly out of scope for this run |
| `EventCard`'s edit path unexercised | Unchanged from v9 |
| Route rendering unverified in a browser | Unchanged from v9. This run's icon and filter rendering is verified through bUnit DOM assertions, not a live browser — no browser tooling was available in this session |
| **Confederation filter — unbuilt** | New. Needs a team→confederation lookup covering all 48 teams; the tracked 8-column CSV has no such column. Flag-don't-build per the patch and this run's hard rules |
| **`learning-mode/` chapter on the test project — deferred** | New. Raised at plan time; the principal confirmed deferral to a later run, per the patch's §6 scope and the repo's convention that chapters follow settled code |
| **Root README and `docs/how-to-run.md` describe the pre-v9 app** | New (carried, not introduced, by this run). Matches at `/`, a `/register/{id}` route, and a "Registered" badge are all stale — none have existed since v9. Only the test command was added this run, per an explicit decision to keep this run's docs commit scoped to what task 11 authorized rather than widen it into an unrelated correction |

**Next: Run 4D** — the design addendum dossier. It resolves R1–R4, the Apple HIG provenance
statement, and the two-record login decision, none of which this run touched or needed to.
