# Plan: P-CC-C4-EventEaseAccessibility-v2_0 (+ three-precisions patch) — Task 0/1

## Context

Run 1 built the EventEase Blazor WASM capstone (14 commits, HEAD `f702868`) — "EventEase" was this repository's project name at the time, since adapted into the FIFA Press App. The principal (project owner) then tried to actually use what got built and hit two real walls: (1) the documentation's AI-attribution wording named a specific tool inaccurately, and needed to read as generic "AI coding assistant" language throughout, with no disclosure of what changed or why; (2) the three Learning Mode walkthroughs written in v1 were, despite the name, written for a reader with front-end framework experience (comparing Blazor to React/JSX) — the principal has never used any front-end framework, so they couldn't actually learn from what "Learning Mode" produced. Separately, the principal has no local IDE and the earlier "clone it and run it" answer to the visual-verification gap didn't work for them, and they want the app viewable from a plain URL.

This run (v2) is a documentation-and-accessibility overhaul on the already-working app: fix the AI-attribution wording repo-wide, fully regenerate the three walkthroughs for a non-technical-but-programming-literate reader plus a Glossary, add per-folder READMEs and inspector comments across `/src` and the main folders, restructure `handoff/` into versioned subfolders, and — per the patch — actually publish the app to GitHub Pages so it's viewable with zero setup. **Zero application behavior changes.** The task-0/1 job was to verify every live-state assumption the core prompt and patch made (HEAD, Copilot hit list, runtime facts, Pages state), and produce the full commit-by-commit plan before writing anything.

*(`src/EventEase/` was renamed to `src/FifaPressApp/` in v7. Every `src/EventEase`/`EventEase.csproj` citation below is preserved byte-exact as this plan's historical record.)*

## Preflight (task 0) — all confirmed

- `gh` authenticated as `jdsaire`, working.
- `P-blazor-run2.txt` present and read in full — the principal's own instruction file; the core prompt's `resolved_decisions` block already reconciles every gap in it, so it governs scope but not literal wording.
- Local clone present and clean at `.../C4 – Blazor EaseEvent/frontend_c4_blazor_eventease`, `git status` clean, up to date with `origin/main`.
- Local git identity confirmed (same as run 1): `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>`.

## Task-1 finding 1: HEAD matches, no divergence

Local and remote both at `f702868e82bbc0d35623774777326875c6f21d65` — "docs: archive build plan and completion report." Matched `verified_state` exactly.

## Task-1 finding 2: confirmed Copilot hit list (11 lines, 3 files) + exact replacement wording

Fresh case-insensitive grep across all tracked files (excluding `wwwroot/lib/`) found exactly 11 lines — matched `verified_state`'s named locations, confirmed complete, nothing extra:

| # | File:Line | Old | New |
|---|---|---|---|
| 1 | `README.md:34` | `**AI coding assistant used for development (per assignment requirements):** Microsoft Copilot` | `**AI coding assistant used for development (per assignment requirements):** an AI coding assistant` |
| 2 | `README.md:39` | `...design outline, Copilot assistance summary.` | `...design outline, AI coding assistant summary.` |
| 3 | `README.md:46` | `...Microsoft Copilot was used as the AI coding assistant across all three graded Activities...` | `...an AI coding assistant was used across all three graded Activities...` |
| 4 | `docs/grading-criteria.md:35` | `## 6. Copilot Assistance Summary (5 pts)` | `## 6. AI Coding Assistant Summary (5 pts)` |
| 5 | `docs/grading-criteria.md:37` | `[project-plan.md](project-plan.md#copilot-assistance-summary) — ...how Copilot assisted...` | `[project-plan.md](project-plan.md#ai-coding-assistant-summary) — ...how the AI coding assistant assisted...` |
| 6 | `docs/project-plan.md:20` | `...rendering models, Copilot-assisted development).` | `...rendering models, AI-assisted development).` |
| 7 | `docs/project-plan.md:38` | `## Copilot Assistance Summary` | `## AI Coding Assistant Summary` |
| 8 | `docs/project-plan.md:40` | `...Microsoft Copilot was the AI coding assistant used throughout all three Activities:` | `...an AI coding assistant was used throughout all three Activities:` |
| 9 | `docs/project-plan.md:42` | `**Activity 1 (Foundation):** Copilot suggested...` | `**Activity 1 (Foundation):** The AI coding assistant suggested...` |
| 10 | `docs/project-plan.md:43` | `**Activity 2 (Debug & Optimize):** Copilot helped identify...` | `**Activity 2 (Debug & Optimize):** The AI coding assistant helped identify...` |
| 11 | `docs/project-plan.md:44` | `**Activity 3 (Expansion):** Copilot suggested building...` | `**Activity 3 (Expansion):** The AI coding assistant suggested building...` |

New anchor slug for the renamed heading: `## AI Coding Assistant Summary` → `#ai-coding-assistant-summary`. Row 5's link updated accordingly.

## Task-1 finding 3: runtime facts reconfirmed

`EventEase.csproj`: `net10.0`, `Microsoft.AspNetCore.Components.WebAssembly` 10.0.5, `.WebAssembly.DevServer` 10.0.5 (PrivateAssets=all). `launchSettings.json`: HTTP profile `http://localhost:5126`; HTTPS profile `https://localhost:7123` (also binds 5126). Local SDK: `10.0.201`.

## Task-1 finding 4 — live-state divergence: GitHub Pages was already enabled, in the wrong mode

`gh api repos/jdsaire/frontend_c4_blazor_eventease/pages` showed Pages was **already live** — `build_type: "legacy"`, source `{branch: "main", path: "/"}`, `cname: null`. Not what `verified_state`/the patch assumed. Confirmed by fetching the live URL (HTTP 200): Jekyll was auto-rendering root `README.md` — not the Blazor app. Not a stop condition (no CNAME, app itself untouched), but it changed the concrete action from "create" to "reconfigure": switch the Pages source to `build_type: workflow` via `gh api` before the first workflow run.

## Task-1 finding 5: current best practice for the Pages workflow (researched, not assumed)

- **Base href fix scope:** CI-published output only (`publish/wwwroot/index.html`), never the tracked `src/EventEase/wwwroot/index.html` — otherwise local `dotnet run`/Codespaces would break.
- **`.nojekyll`:** required so `_framework/` isn't dropped.
- **SPA deep-link 404s:** GitHub Pages is a static host; a direct request to `/events/3` 404s without a `404.html` redirect + decode-script pair, generated only into the ephemeral publish output.
- **No `.gitattributes`/CRLF concern:** doesn't apply — this workflow uses `upload-pages-artifact` + `deploy-pages`, no git commit of build output.
- **SDK pin:** `actions/setup-dotnet@v4` with `dotnet-version: '10.0.x'`, floating patch version.
- **Standard permissions/environment block.**

## Full ordered commit sequence (as planned)

| # | Phase | Commit message |
|---|---|---|
| 1 | terminology | `docs: standardize AI coding assistant terminology across documentation` |
| — | gate-register | *(GATE — no commit)* |
| 2 | walkthrough-1 | `docs: rewrite Activity 1 walkthrough for a non-technical reader` |
| 3 | walkthrough-2 | `docs: rewrite Activity 2 walkthrough for a non-technical reader` |
| 4 | walkthrough-3 | `docs: rewrite Activity 3 walkthrough for a non-technical reader` |
| 5 | glossary | `docs: add glossary for learning-mode walkthroughs` |
| 6 | learning-mode-readme | `docs: add learning-mode folder README` |
| 7 | src-readmes | `docs: add per-folder READMEs across the src project` |
| 8 | src-comments | `docs: add inspector-facing comments across key source files` |
| 9 | pages-publish | `ci: publish Blazor WebAssembly build to GitHub Pages` |
| 10 | how-to-run | `docs: add how-to-run guide covering Codespaces and local terminal` |
| 11 | main-readmes-and-handoff | `docs: add docs README and restructure handoff into versioned folders` |
| 12 | readme-refresh | `docs: refresh root README to reflect v2 accessibility and Pages changes` |
| — | verify | *(no commit unless drift found)* |
| 13 | archive | `docs: archive run 2 plan and completion report` |

## Approval scope

This plan approval authorized tasks up through commit 1 (terminology) and the register-gate presentation — nothing in commits 2–13 was to be written before the register gate was separately approved. See `Completion-Report-v2.md` for what actually shipped, including one approved deviation from this sequence.
