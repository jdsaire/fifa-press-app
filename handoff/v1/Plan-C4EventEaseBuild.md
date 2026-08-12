# Plan: DEPLOY-C4-EventEaseBuild-v1_0 — Task 0/1 (Preflight + Overall Plan)

## Context

This is the Course 4 (Blazor for Front-End Development) capstone build: the EventEase App — this repository's project name at the time, since adapted into the FIFA Press App — executed per `DEPLOY-C4-EventEaseBuild-v1_0.xml`. The deploy prompt is itself already a fully detailed, pre-approved task sequence (18 tasks, three graded Activities from `c4-capstone-guidelines.json`, each its own commit-gated checkpoint). The task-1 job was not to redesign that sequence — it was to verify the preflight conditions it depends on (GitHub access, repo state, git identity, .NET/Blazor tooling) and surface any place where live reality diverges from the document's assumptions, per its own `verified_state` instruction to never assume greenfield.

This plan is the task-1 deliverable the XML requires before any code is written: repo create-vs-adopt decision, confirmed tree, full commit sequence, git identity, and verification steps. Approving this plan authorized task 2 (scaffold) through the first Activity 1 commit — it did **not** pre-approve past Gate 1 (task 6), Gate 2 (task 10), or Gate 3 (task 14). Those remained separate, explicit approval points per the XML's own hard rule ("repeating checkpoint, separate from and in addition to the single task-1 overall-plan approval").

*(`src/EventEase/` was renamed to `src/FifaPressApp/` in v7. Every `src/EventEase` path/file citation below is preserved byte-exact as this plan's historical record.)*

## Preflight findings (task 0)

All required conditions confirmed live:

- **GitHub access:** `gh` authenticated as `jdsaire` (keyring, scopes `repo`/`workflow`/`read:org`/`gist`). Working.
- **Attachments:** all three present and read in full this session — `c4-capstone-guidelines.json`, `C4_Syllabus.txt`, `EventEase-Flowchart.md`.
- **.NET SDK:** `10.0.201` installed. `dotnet new blazorwasm` template confirmed available and functional (`dotnet new blazorwasm --help` succeeds), targets `net10.0`, `--auth None` by default — matches the "no authentication system" hard rule with zero extra flags needed.

## Task-1 finding: repo state — ADOPT, not create fresh

`gh repo view jdsaire/frontend_c4_blazor_eventease` showed the repo **already existed**:
- `createdAt`: 2026-07-28, `isEmpty: true`, `defaultBranchRef.name: ""` (no commits, no branch yet).
- Description already set: "EventEase App with Blazor".

This was the "repo already exists" branch of task 1, but simpler than the `frontend_c2_cs_library` precedent (which had one pre-existing content commit to preserve) — here there was **no history at all** to adopt. Task 2 `git init`'d a local clone and pushed straight to `main`; no `gh repo create` was needed, no reset/force-push risk, nothing to preserve or overwrite.

## Git identity (hard rule: read existing local gitconfig, never invent)

No global `~/.gitconfig` exists on this machine. However, the sibling repo `frontend_c2_cs_library` (local clone at `/Users/jd-mac/Downloads/KEY Multi-page Deploy/frontend_c2_cs_library`) has a **local, per-repo** git identity used consistently across every one of its commits:

```
user.name  = Juan Diego S.
user.email = 88201583+jdsaire@users.noreply.github.com
```

Verified via `git log --format='%an <%ae>'` across that repo's full history — one identity, every commit, matching the GitHub account `jdsaire` (numeric ID 88201583 in the noreply address confirms it's that account's real GitHub-issued noreply email). This was the confirmed jdsaire identity; it was set **locally** (not `--global`) in the new clone, matching how the sibling repo itself is configured — no invented identity, no stop condition triggered.

## Local clone location (confirmed with project owner)

The local working copy was placed alongside the course materials already present, at:

```
/Users/jd-mac/Downloads/Coursera Microsoft Frontend/C4 – Blazor EaseEvent/frontend_c4_blazor_eventease/
```

(Not the `KEY Multi-page Deploy/` convention used by the C1/C2 sibling clones — a deliberate choice to keep this one co-located with its own course-materials folder instead.)

## Template output vs. architecture doc — anticipated adaptations

`dotnet new blazorwasm --help` confirmed the template exists and targets `net10.0` with no auth. The XML's `<architecture>` tree was a reasonable approximation but the actual dotnet 10 SDK template differed in specifics only certain once generated at task 2:

- Sample/placeholder pages were named `Home.razor` / `Counter.razor` / `Weather.razor` (not `Index.razor` — a naming change from older SDKs). All three were deleted per task 2's "remove template placeholder content" instruction.
- Layout/nav (`MainLayout.razor`, `NavMenu.razor`) lived in a `Layout/` folder, separate from the `Components/` folder used for `EventCard`.
- `Program.cs` used top-level statements (no explicit `Program` class), since nothing in the guidelines required one.

## Confirmed file tree (target, as approved)

```
frontend_c4_blazor_eventease/
├── README.md
├── src/
│   └── EventEase/
│       ├── EventEase.csproj
│       ├── Program.cs
│       ├── App.razor
│       ├── _Imports.razor
│       ├── wwwroot/
│       ├── Layout/                      (MainLayout.razor, NavMenu.razor — SDK default)
│       ├── Pages/
│       │   ├── EventList.razor          (home route)
│       │   ├── EventDetails.razor
│       │   ├── Registration.razor
│       │   └── NotFound.razor
│       ├── Components/
│       │   └── EventCard.razor
│       ├── Models/
│       │   ├── EventModel.cs
│       │   ├── MockEventData.cs
│       │   └── RegistrationModel.cs
│       └── Services/
│           ├── SessionTracker.cs
│           └── AttendanceTracker.cs
├── docs/
│   ├── project-plan.md
│   ├── setup-guide.md
│   ├── grading-criteria.md
│   └── EventEase-Flowchart.md            (copied verbatim, unedited)
├── learning-mode/
│   ├── Activity1-Walkthrough.md
│   ├── Activity2-Walkthrough.md
│   └── Activity3-Walkthrough.md
└── handoff/
    ├── Plan-C4EventEaseBuild.md
    └── c4-eventease-build-completion-report.md
```

No `LICENSE`. A minimal `.gitignore` (`bin/`, `obj/`) was added beyond the original plan's assumption of none — see the completion report's approved-deviations section.

## Full commit sequence (as planned — 12 commits; see completion report for what actually shipped)

| # | Task | Commit message | Gate after? |
|---|---|---|---|
| 1 | 2 (scaffold) | `chore: scaffold Blazor WebAssembly project` | |
| 2 | 3 (activity1-step3) | `feat(activity1): scaffold EventCard component with name, date, location fields` | |
| 3 | 4 (activity1-step4) | `feat(activity1): wire two-way data binding with mock event data` | |
| 4 | 5 (activity1-step5) | `feat(activity1): set up routing between event list, details, and registration pages` | **Gate 1 STOP** (task 6) |
| 5 | 7 (activity2-step3a) | `fix(activity2): add input validation to EventCard binding` | |
| 6 | 8 (activity2-step3b) | `fix(activity2): handle invalid routes gracefully with a NotFound page` | |
| 7 | 9 (activity2-step3c) | `perf(activity2): optimize event list rendering for larger datasets` | **Gate 2 STOP** (task 10) |
| 8 | 11 (activity3-step3a) | `feat(activity3): add Registration Form with validation` | |
| 9 | 12 (activity3-step3b) | `feat(activity3): add SessionTracker state service` | |
| 10 | 13 (activity3-step3c) | `feat(activity3): add AttendanceTracker state service` | **Gate 3 STOP** (task 14) |
| 11 | 15 (docs) | `docs: add README, project docs, and preliminary flowchart reference` | |
| 12 | 17 (archive) | `docs: archive build plan and completion report` | |

Every commit: author + committer `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>`, no trailers, no AI/agent mention anywhere, pushed directly to `main`, no PRs.

## Verification steps (task 16)

1. `dotnet build` inside `src/EventEase` — zero errors/warnings.
2. `dotnet run --project src/EventEase` — confirms the app serves.
3. Re-walk all three gates' manual checks in one pass.
4. Cross-check `docs/grading-criteria.md` line references against final code.
5. Report PASS/FAIL against all 6 rubric criteria in `c4-capstone-guidelines.json`.
6. Return final `git log --oneline`, all authored as `jdsaire`.
7. Grep the full tree for unwanted AI/agent attribution.

## What happened after this plan was approved

Approval authorized task 2 through the Activity-1 commit sequence, ending at Gate 1. Three gate approvals (tasks 6, 10, 14) followed, each explicit, before the docs and archive tasks closed out the build. Mid-build, the project owner introduced a new standing instruction — a "Learning Mode" walkthrough per Activity, saved under `learning-mode/` — which added two documentation commits beyond this plan's original 12. See the completion report for the full accounting.
