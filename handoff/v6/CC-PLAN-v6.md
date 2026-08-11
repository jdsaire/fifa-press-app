# Plan — DEPLOY v6: Ideation Injection into jdsaire/fifa-press-app

## Context

Eight gate-final ideation deliverables (Gates 0–7) were authored and approved outside this repo,
turning `01-design-research/`'s one open design question — *how might we tell a journalist that
their access has changed before they discover it by being refused?* — into a single buildable
concept, **Access Record**. This run's job is placement only: copy the eight files verbatim into a
new `ux-ui/02-ideation/` folder, write that folder's own README to the same standard as
`01-design-research/README.md`, add one index bullet to `ux-ui/README.md`, and archive the run
under `handoff/v6/` — matching v5's precedent for injecting an already-approved mandate. No
application code, no other mandate folder, and no root README line is touched.

**Preflight verified this session (read-only):**
- `gh` authenticated as `jdsaire` (repo, workflow scopes). ✅
- Live `main` HEAD = `19e3b21df56eaa58bdc2b9d0f7c9d67824d6dbae` — **matches** `verified_state` exactly, no drift. Last commit: "docs(ux-ui): consolidate UX dossiers under ux-ui/".
- No `deploy/v6-*` branch exists yet. `handoff/` contains v1–v5. `remotes/origin` also has stale `deploy/v4-...` and `deploy/v5-...` branches (already merged, left over — not this run's concern).
- All eight attachments read and confirmed present at `Ideation-Artifacts/` with exact filenames/casing matching `verified_state`: `00_SCOPE.md`, `01_WORKSHOP-PROTOCOL.md`, `02_DIVERGENCE-1.md`, `03_DIVERGENCE-2.md`, `04_CONVERGENCE.md`, `05_CONCEPT.md`, `06_HANDOFF.md`, `07_LIMITATIONS.md`. Each ends with its `✅ GATE n COMPLETE` footer, intact.
- Root `README.md` line 43 confirmed **byte-identical** to what `verified_state` quotes: "every UX dossier for this app: research, evaluation, and whatever mandate tracks land here next." → generically covers `02-ideation/` already. **No root README edit.**
- `ux-ui/01-design-research/README.md` read in full (style precedent) — framing paragraph → `---` → gate table (Gate | File | What it delivers) → relationship section to sibling folder. `ux-ui/README.md`'s live top framing differs slightly in wording from what `verified_state` paraphrases, but carries the same meaning ("From now on, every new UX dossier lands here as the next numbered folder") — no wording change needed, only the new bullet.
- Baseline internal (non-`http`) markdown link count, repo-wide: **199** — confirmed by live regex sweep, matches `verified_state` exactly.
- `completion-report-shape.md`, named in the deploy prompt, **does not exist in this repo** (v4 already hit this and noted it). It does exist as a standing skill reference at `cc-deploy-prompts/references/completion-report-shape.md` outside the repo — read in full this session and used directly as the shape for `Completion-Report-v6.md` (6 sections: commit list, outcome, results table, authorized deviations, decisions resolved autonomously, open items carried forward).
- **Flagged discrepancy, resolved as noted below, not blocking:** `success_criteria` #6 says "exactly four commits (frozen batch, folder README, index bullet) plus the fifth archive commit — five total." The task list (`<tasks>` 2, 3, 5) defines exactly **four** commit-producing actions (batch, folder README, index bullet, archive) — three named items plus the archive is **four total**, not five. Treating this as a drafting slip in the prompt's own success-criteria wording; will report the actual total (4) in the Completion Report with this discrepancy noted explicitly, per the "decisions resolved autonomously" section — not stopping for it, since it changes nothing about which files get written or what gets committed, only the arithmetic in one success-criteria sentence.

## Scope

Touches only: new `ux-ui/02-ideation/` folder (8 frozen files + 1 new README), one new bullet in
`ux-ui/README.md`, and `handoff/v6/` + one new row in `handoff/README.md`. Nothing under `src/`,
`.github/`, `docs/`, `learning-mode/`, `ux-ui/00-initial-evaluation/`, `ux-ui/01-design-research/`,
or root `README.md` is touched. Content of the eight ideation files is copied byte-verbatim — no
rewriting, no rewording, no correcting.

## (a) File plan

```
ux-ui/02-ideation/
├── README.md                    (NEW — authored this run)
├── 00_SCOPE.md                  (verbatim copy from Ideation-Artifacts/)
├── 01_WORKSHOP-PROTOCOL.md      (verbatim copy)
├── 02_DIVERGENCE-1.md           (verbatim copy)
├── 03_DIVERGENCE-2.md           (verbatim copy)
├── 04_CONVERGENCE.md            (verbatim copy)
├── 05_CONCEPT.md                (verbatim copy)
├── 06_HANDOFF.md                (verbatim copy)
└── 07_LIMITATIONS.md            (verbatim copy)
```

## (b) `ux-ui/02-ideation/README.md` — full drafted text

```markdown
# Ideation

A simulated eight-gate ideation mandate that takes the one design question
`01-design-research/` left unanswered — *how might we tell a journalist that their access has
changed before they discover it by being refused?* — and turns it into a single buildable concept:
**Access Record**. No workshop took place; six participants were authored, not recruited, and every
vote, coalition and objection recorded here is one person's simulation of a documented method.
Start with [`00_SCOPE.md`](00_SCOPE.md) for the inherited contract this mandate was held to, and
[`07_LIMITATIONS.md`](07_LIMITATIONS.md) for the full defence of every scope decision made along
the way.

---

## The eight gates, in order

| Gate | File | What it delivers |
|---|---|---|
| 0 | [`00_SCOPE.md`](00_SCOPE.md) | Scope lock — the inherited contract, the preserved maximalist scope, and the boundary this mandate does not reopen. |
| 1 | [`01_WORKSHOP-PROTOCOL.md`](01_WORKSHOP-PROTOCOL.md) | Workshop protocol — the simulated six-participant roster, the minute-scripted two-day agenda, and what a simulation can and cannot produce. |
| 2 | [`02_DIVERGENCE-1.md`](02_DIVERGENCE-1.md) | First ideation — a Value Proposition Canvas for the Amina segment, yielding 18 risk-sorted assumptions (ID-01–ID-18). |
| 3 | [`03_DIVERGENCE-2.md`](03_DIVERGENCE-2.md) | Second ideation — unconstrained divergence with feasibility suspended, yielding 14 more ideas (ID-19–ID-32) and a second-degree reading of the three least realistic. |
| 4 | [`04_CONVERGENCE.md`](04_CONVERGENCE.md) | Assessment and convergence — two prioritization matrices and a Buy a Feature round against the full 32-idea pool, closing on a ranked eight-item shortlist. |
| 5 | [`05_CONCEPT.md`](05_CONCEPT.md) | The concept — **Access Record**: what it absorbs, what it deliberately leaves out, three core interactions as behaviour, and the boundary edges where it will disappoint. |
| 6 | [`06_HANDOFF.md`](06_HANDOFF.md) | Mandate handoff — the whole mandate in one document for a reader who did not attend, ordered recommendations, and what `03-ui-prototyping` inherits and does not. |
| 7 | [`07_LIMITATIONS.md`](07_LIMITATIONS.md) | Methodological disclosure — every scope decision defended, and what this mandate does and does not legitimately support. |

## Relationship to its sibling folders

[`../00-initial-evaluation/`](../00-initial-evaluation/) audited the app that already existed in
this repo before the FIFA Press App reframing — a usability and accessibility pass on working
software. [`../01-design-research/`](../01-design-research/) produced the evidence base this folder
builds on and the one design question it hands off unanswered. This folder does neither: it does
not audit an app and it does not gather evidence. It takes an already-chosen question and an
already-gathered evidence base and turns them into one defensible, unvalidated concept — concept,
not evidence, and not interface. `03-ui-prototyping`, not yet begun, is where that concept becomes
screens.
```

## (c) New bullet for `ux-ui/README.md`

Inserted as the third bullet, immediately after the existing `01-design-research/` bullet, before
the closing "From now on..." line:

```markdown
- [`02-ideation/`](02-ideation/README.md) — a simulated eight-gate ideation mandate that turns
  `01-design-research/`'s one open design question into a single buildable concept, Access Record:
  eight gate deliverables and a ranked idea pool of thirty-two, each traceable to a decision. Not a
  validated concept — see its own README for what it can and cannot establish.
```

The two existing bullets and the folder's top framing paragraph / closing line are not touched.

## (d) Root README

Confirmed unchanged and generically covering `02-ideation/` already (see preflight above). **No
edit to root `README.md` in this run.**

## (e) Commit sequence — 4 commits total (see flagged discrepancy above)

On branch `deploy/v6-ideation-injection`, created from `main` at
`19e3b21df56eaa58bdc2b9d0f7c9d67824d6dbae`:

1. `docs(ux-ui): inject 02-ideation mandate — eight gate deliverables` — adds all 8 verbatim files under `ux-ui/02-ideation/`.
2. `docs(ux-ui): add README for 02-ideation` — adds `ux-ui/02-ideation/README.md`.
3. `docs(ux-ui): index 02-ideation in ux-ui/README.md` — adds the one bullet.
4. `docs: archive v6 plan and completion report` — adds `handoff/v6/` (CC-PLAN-v6.md, Completion-Report-v6.md, README.md) + new row in `handoff/README.md`.

PR opened from `deploy/v6-ideation-injection` against `main` after commit 3 and verification
(task 4), left unmerged; commit 4 (archive) is pushed to the same branch afterward, updating the
same PR — matching v5's precedent where the archive commit landed on the already-open PR.

Author and committer on every commit: `Juan Diego S. <88201583+jdsaire@users.noreply.github.com>`
(this repo's existing identity — matches v4/v5). No AI attribution anywhere.

## (f) Link-integrity check plan

Baseline (confirmed live, this session): **199** internal (non-`http`) markdown links across 55
`.md` files. Expected new links from this run's own content, counted from the drafted text above:

- `ux-ui/02-ideation/README.md`: 12 (2 in the framing paragraph, 8 in the gate table, 2 in the
  relationship section).
- `ux-ui/README.md` bullet: 1.
- `handoff/v6/README.md` (indexing the plan + report): 2.
- `handoff/README.md` new row: 1.

Expected new: **~16**, expected total **~215/215** — but per v4/v5 precedent, the authoritative
number comes from a full-repo link-*resolution* pass at task 4 (verify), not the regex count, since
prior runs found regex-only counts catch false positives (code-span example links, forward
references to files not yet written at check-time). Any such false positives will be named
explicitly and excluded with reasons, exactly as `handoff/v5/Completion-Report-v5.md`'s
"Link-integrity sweep detail" section did. Reported as `(199 + new)/(199 + new)` with the actual
resolved numbers, not the estimate above.

## Verification steps (task 4, before the archive commit)

1. Byte-diff each of the 8 injected files against its `Ideation-Artifacts/` source — expect
   identical; confirm every `SIMULATED — NOT EVIDENCE` header, every `[SOURCED]`/`[SIMULATED]`/
   `[ASSUMPTION]` tag, every ID-01–ID-32 reference, and every `✅ GATE n COMPLETE` footer survived
   untouched.
2. `git diff` of `src/`, `.github/`, `docs/`, `learning-mode/`, `ux-ui/00-initial-evaluation/`,
   `ux-ui/01-design-research/`, and root `README.md` against pre-run HEAD — expect byte-identical.
3. Full-repo internal-link resolution pass — report `(199 + new)/(199 + new)`.
4. `git log` on the branch — confirm sole author/committer `jdsaire`, zero AI attribution in any
   commit message, branch name, or file content (including both authored files).
5. Open PR from `deploy/v6-ideation-injection` against `main`; do not merge; report PR URL.

## Archive (task 5)

`handoff/v6/`, following the `CC-PLAN-vN.md` / `Completion-Report-vN.md` convention v2–v5 already
established:
- `handoff/v6/CC-PLAN-v6.md` — this plan, renamed from its Plan Mode filename, as actually approved.
- `handoff/v6/Completion-Report-v6.md` — following the actual `completion-report-shape.md` read
  this session: commit list w/ SHAs, one outcome paragraph, PASS/FAIL table against this prompt's
  11 `success_criteria` (with the 4-vs-5 commit-count discrepancy noted under "decisions resolved
  autonomously"), authorized deviations (empty section if none), decisions resolved autonomously,
  and open items carried forward — explicitly including the glossary gap (`ux-ui/01-design-research/GLOSSARY.md`
  does not cover `02-ideation/`'s new terms — logged as carried forward, not actioned, per hard_rules)
  and the PR URL.
- `handoff/v6/README.md` — indexes both, matching `handoff/v5/README.md`'s shape.
- New row appended to `handoff/README.md`, matching the existing five rows' one-line style.

Commit: `docs: archive v6 plan and completion report`. Verify neither new file contains any AI or
agent attribution.

## Guardrail confirmations

- No attachment filename/casing mismatch found — all 8 match `verified_state` exactly.
- `01-design-research/README.md`'s shape has not changed materially — style precedent applies as
  written.
- Root README `ux-ui/` line still generically covers new dossiers — no edit, no unilateral decision
  needed.
- No credential printed or referenced at any point; all GitHub access via `gh`.
