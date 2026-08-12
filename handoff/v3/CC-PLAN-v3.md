# Plan: DEPLOY-C4-UX-DeployA-CommitAndRedesign-v1_0 — Task 0/1

*(`EventEase.csproj` was renamed to `FifaPressApp.csproj` in v7. The citations below are preserved byte-exact as this plan's historical record.)*

## Context

Run v2 shipped a documentation-and-accessibility-narrative overhaul with zero application behavior
changes. Separately, a full usability + WCAG 2.2 AA audit was run against that same commit
(`0653b4e`), producing eight files (26 findings, a heuristic evaluation, an accessibility audit, a
54-check protocol, and a 19-commit remediation scope). This run's job was to commit that audit into
the repo verbatim as `ux-ui/evaluation-spec/`, then execute all 19 fixes it specifies — real
component/page/stylesheet changes — while protecting two graded invariants: EventCard's two-way data
binding (rubric criterion 2) and the app's mock-data/no-backend/no-auth scope ceiling.

## Task 0 — preflight (confirmed)

- `gh` authenticated as `jdsaire`, repo `jdsaire/frontend_c4_blazor_eventease` reachable.
- HEAD confirmed at `0653b4ee926eca52780bc6c78ab1b00fd1170ac0` on both `main` and this session's
  branch — no drift from `verified_state`.
- All 13 in-scope source files re-read in full; line counts and cited content matched the audit
  exactly, byte-for-byte on every citation checked.
- `EventEase.csproj`: exactly 2 `PackageReference` entries, unchanged.
- Baseline `dotnet build`: 0 warnings, 0 errors.
- Git identity: `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>` — matches v1/v2.
- `handoff/v1/` and `handoff/v2/` both exist with the `CC-PLAN-v{N}.md` / `Completion-Report-v{N}.md`
  / `README.md` shape → next version is `v3`.
- All 8 evaluation-spec attachments present and read in full.

## Environment note — worktree topology (user-approved deviation)

This session ran in a git worktree on a dedicated working branch, separate from the sibling primary
working copy where `main` was checked out. Per explicit direction during plan review, this run did
not push directly to `main` — it opened a single PR instead, overriding the deploy prompt's own
"NO PULL REQUESTS" hard rule. See the Completion Report's deviations section.

## Design Decision 1 — EventCard editable-mode reachability (as approved)

Added `[Parameter] public bool ReadOnly { get; set; } = true;` to `EventCard`. The three existing
`[Parameter]`/`EventCallback<T>` pairs and their input handlers were left untouched — same
signatures, same `InvokeAsync` calls. A local `isEditing` field (initialized once from `!ReadOnly`)
controls which view renders: read-only by default (heading, `<time>`, plain text, plus an "Edit"
button), or the original editable inputs (reached via the toggle, with "Changes here aren't saved."
shown alongside so the app never implies unsupported persistence). Pages pass `ReadOnly="true"`
explicitly rather than relying silently on the default. A per-instance `instanceId` (added when
labels were introduced) keeps `for`/`id` pairs from colliding across the many cards rendered on the
list page.

## Design Decision 2 — pagination UI shape (as approved)

Google-style numbered pages in `EventList.razor`: a search box filtering by name/location applied
before pagination, `PageSize = 10`, `TotalPages` computed from the filtered count (currently ≤5, so
no ellipsis truncation was needed), numbered `<button>`s (not links, not Prev/Next) with
`aria-current="page"` on the active one. Changing the search term resets to page 1.

## Commit sequence (as executed — see Completion Report for the full list)

20 commits: the evaluation-spec commit, then all 19 remediation items from `remediation-scope.md`,
each as its own commit using that file's proposed message. No item required a two-commit split.

## Verification plan (as approved and executed)

- `dotnet build` after every commit, not batched at the end.
- Two-way binding: Blazor compiles `@bind-EventName="x"` into a matching `EventName`/
  `EventNameChanged` parameter pair at compile time, so the build itself fails if that pairing ever
  breaks — a compile-verified check on every commit touching `EventCard.razor` or a page's
  `<EventCard>` tag, backed by a manual source trace of the three `...Changed.InvokeAsync` calls.
- Link check: every relative markdown link across all tracked `.md` files resolves (as a file or a
  directory), covering the pre-existing 135 plus the new 27.
- Findings closure: every ID in `findings-register.md` cross-checked against the commit that closed
  it, with UX-C-04 recorded as carried-forward open (REASONED evidence only, no browser available).
- `EventEase.csproj` diffed against baseline to confirm zero new dependencies.
- Full diff and commit messages grepped for AI/agent/Claude/Anthropic attribution.

## Handoff version

Confirmed by inspecting `handoff/v1/` and `handoff/v2/` directly: both exist with the established
naming shape, so this run archives as `v3`.
