# Plan — DEPLOY-FifaPressApp-4D-I-AddendumInjectionAndResolution-v11_0

## Context

Run 4D produced four proposed design-addendum files (`09_DESIGN-ADDENDUM.md` through
`12_DECISION-REVERSALS.md`) that close out open design questions before Run 4E builds the frontend
against them. This run (4D-I) injects those four files into the repo byte-identical to what was
attached, then — once the principal resolves the three items the dossier itself left open —
applies targeted patches that turn "proposed" into "Final." The two steps land as separate commits
so a reviewer can see "the file as authored" and "exactly what closing the open items changed" as
two distinct diffs.

**STOP 1 cleared.** The three resolution questions were asked in Plan Mode and answered:
- **Q1 (language switch, `11_I18N.md` §5.2):** Update live, no reload (**Option B**).
- **Q2 (Amina naming, `10_AUTH-AND-ONBOARDING.md` §3.2):** Close as intentional.
- **Q3 (Change-entity model note, `12_DECISION-REVERSALS.md` §5):** Number it **R5**.

**Preflight (task 0) confirmed:** `gh` authenticated as `jdsaire`; `jdsaire/fifa-press-app` reachable;
`main` HEAD = `147bc4a` (matches `verified_state`); all four attachments present, status lines match
`verified_state` exactly (the 1-line delta between my read-tool line numbering and the prompt's
stated lengths is a trailing-newline counting artifact, consistent across all four files — not
content drift).

This is STOP 2 — the complete plan, per task 2(a)–(f). Execution (tasks 3–10) begins only after this
plan is approved.

---

## (a) Exact patch text for the three resolutions

### Q1 — `11_I18N.md` §5.2, replace the closing recommendation paragraph

The **Option A** and **Option B** description paragraphs (the two bulleted mechanism/cost blocks)
stay untouched, as the record of what was weighed. Only the final paragraph is replaced:

Old:
> **Recommended: Option A**, for the same reason `10_AUTH-AND-ONBOARDING.md` §8 left session
> persistence unresolved rather than building it — this is a demonstration, and a working reload
> delivers the correct, verifiable result with a fraction of the risk surface. The visible flash is a
> real cost but a bounded, honest one; a partially-migrated Option B that misses one cached string is
> a worse outcome that looks fine until someone finds the one stale label. **This is 4E's
> implementation call to confirm, not this dossier's to mandate irreversibly** — flagged as a strong
> recommendation with the reasoning attached, not a locked decision, per `guardrails`-style practice
> elsewhere in this project's prompts.

New:
> **Decided: Option B.** In-session re-render, no reload, is the mechanism. Its cost — every
> component that displays Category A/B/C/D content must actually re-render on locale change, via a
> `[CascadingParameter]`-style locale value with no component allowed to cache a localized string
> outside its render method — is accepted as the price of this decision, not weighed against it: a
> demonstration whose sign-in flow (`10_AUTH-AND-ONBOARDING.md` §2) already commits to a working,
> observable state machine rather than the cheaper inert option should not reintroduce a jarring
> transition at the one other piece of interactive state the app exposes. Option A's visible reload
> flash — a worse moment than the instant, flash-free theme toggle already sets as the app's own bar
> — is why it was not chosen.

§5.1 and §5.3 are not touched, per the question's own scope.

### Q2 — `10_AUTH-AND-ONBOARDING.md` §3.2, replace the naming-discrepancy paragraph

Old:
> **Naming discrepancy, flagged not resolved.** `05_ARTIFACTS.md` calls the persona **Amina R.**; the
> seeded record calls her **Amina Bello**. Both predate this file, neither is wrong, and the surname
> initial in the research artifact is a persona convention rather than a contradiction. Left as-is —
> renaming a seeded record to match a persona document would edit shipped data to satisfy a cosmetic
> consistency nobody has asked for. Noted so a future reader does not treat it as a defect.

New:
> **Naming difference — settled, intentional, not an open item.** `05_ARTIFACTS.md` calls the persona
> **Amina R.**; the seeded record calls her **Amina Bello**. Both predate this file. The persona
> document's surname initial and the seeded record's full surname are two conventions for the same
> person, not a contradiction — no change to either is being requested. A future reader should not
> treat this as a defect or reopen it.

### Q3 — three files, four patch points, **plus one scope note requiring your sign-off**

The question names three patch points exactly:

1. **`12_DECISION-REVERSALS.md` §5 heading** —
   Old: `## 5. A fifth item, flagged rather than numbered — the \`Change\` entity's shape`
   New: `## 5. R5 — The \`Change\` entity's shape changes to locale-keyed fields`

2. **The paragraph beginning "Not one of the four reversals the patch enumerates"** —
   Old:
   > **Not one of the four reversals the patch enumerates**, and this file does not renumber it as
   > R5. Named here because `11_I18N.md` §9 identified it and asked this file to either record it or
   > state plainly why it does not qualify — leaving the gap unaddressed would undercut the one file
   > whose job is to keep the repo honest about exactly this kind of thing.

   New:
   > **Formally the fifth reversal, numbered R5.** `11_I18N.md` §9 identified this change and asked
   > this file to either record it or state plainly why it does not qualify; on the reasoning below,
   > it qualifies — a frozen gate file's specification (`06_DATA-MODEL.md`'s `string` typing) is
   > superseded by this patch, with a stated reason, which is the same shape R1–R4 each take.

3. **Summary table `#` column, the `Change`-entity row** —
   Old: `| *(unnumbered)* |`
   New: `| R5 |`

4. **`11_I18N.md` §9's closing paragraph**, so the two files don't disagree —
   Old (last three sentences):
   > What *is* worth naming in `12` for completeness: §4.3's `Change` entity change (four `string`
   > fields becoming locale-keyed) is a modification to a frozen `06_DATA-MODEL.md` section, even
   > though it is not one of the four numbered reversals (R1–R4) the patch enumerates.
   > `12_DECISION-REVERSALS.md` should record it alongside R1–R4, or note explicitly why it does not
   > qualify as a fifth reversal, so the file that exists specifically to keep the repo honest does
   > not have a documented gap of its own.

   New:
   > What *is* worth naming in `12` for completeness: §4.3's `Change` entity change (four `string`
   > fields becoming locale-keyed) is a modification to a frozen `06_DATA-MODEL.md` section.
   > `12_DECISION-REVERSALS.md` §5 records it as R5, the fifth of the reversals it enumerates, so the
   > file that exists specifically to keep the repo honest does not have a documented gap of its own.

**The scope note:** §5 of `12_DECISION-REVERSALS.md` contains two more paragraphs the question's
text does not name — "**Why this is not treated as a fifth numbered reversal**" and "**What this
file does instead of numbering it**." Both are written in present tense arguing the *opposite* of
what the new heading now says. Patching only the four points above would leave the file
self-contradicting one paragraph below its own new "R5" heading — exactly the kind of sloppiness the
`role` section says this run values precision against. Rather than improvise silently, I'm proposing
a minimal fix to these two lead-in phrases (substance otherwise preserved) and flagging it here per
the guardrail on unanticipated patches. If you'd rather I patch *only* the four named points and
leave the contradiction for the record, tell me and I'll do that instead.

Proposed additional patch — bolded lead-in only, reasoning paragraph otherwise kept:
- `**Why this is not treated as a fifth numbered reversal.**` → `**Why this is numbered despite being
  a different shape of change than R1–R4.**`, with one sentence appended at the end: *"It is numbered
  anyway: R1–R4 are numbered because each changes what a frozen gate file specifies, and this does
  too, regardless of which of the two sub-shapes above the underlying change happens to be."*
- `**What this file does instead of numbering it.**` → `**What this section records.**`, with the
  paragraph's first clause changed from "Records it here, plainly, as a genuine model change..." to
  "This is R5, stated here plainly as a genuine model change..." and its closing clause changed from
  "should find it by reading this section" to "should find it here as R5, alongside R1–R4."

---

## (b) Status-line replacement — all four files, identical structure

Per the fixed rule, landing date is **16 Aug 2026** (today), same across all four since they land in
one commit:

- `09_DESIGN-ADDENDUM.md`: `**Status:** Final — approved and injected at v11, 16 Aug 2026. First file of Run 4D, the design addendum dossier.`
- `10_AUTH-AND-ONBOARDING.md`: `**Status:** Final — approved and injected at v11, 16 Aug 2026. Second file of Run 4D, the design addendum dossier.`
- `11_I18N.md`: `**Status:** Final — approved and injected at v11, 16 Aug 2026. Third file of Run 4D, the design addendum dossier.`
- `12_DECISION-REVERSALS.md`: `**Status:** Final — approved and injected at v11, 16 Aug 2026. Fourth and final file of Run 4D, the design addendum dossier.`

---

## (c) README additions

**New table section in `ux-ui/03-ui-prototyping/README.md`** (appended after the existing nine-gate
table, matching its exact row style, visually distinct per `verified_state`):

```markdown
---

## The design addendum, Run 4D

Not a tenth gate — an addendum layered on top of the nine gates above, closing out three items the
prototyping pass left open before Run 4E builds against it. Injected proposed, then finalized once
three principal-gated resolutions closed; see each file's own status line.

| # | File | What it delivers |
|---|---|---|
| 1 | [`09_DESIGN-ADDENDUM.md`](09_DESIGN-ADDENDUM.md) | Design authority and provenance limit, a black-anchored dark palette re-derivation, the theme trigger's relocation to the nav list, and progressive-disclosure patterns for the change list and Help. |
| 2 | [`10_AUTH-AND-ONBOARDING.md`](10_AUTH-AND-ONBOARDING.md) | Sign-in becomes a real (simulated) session with two demo records, the public landing view, and what stays reachable without signing in versus what is gated. |
| 3 | [`11_I18N.md`](11_I18N.md) | The EN/ES/PT string inventory, the seeded-content localization approach, and the language-switch mechanism. |
| 4 | [`12_DECISION-REVERSALS.md`](12_DECISION-REVERSALS.md) | The formal record of every decision this addendum reverses against the frozen gate files above, with the reasoning for each. |
```

**Clause added to `ux-ui/README.md`'s existing `03-ui-prototyping/` bullet** (appended to the end of
the existing sentence, nothing else in the bullet rewritten):

> ...Specification only, no code — see its own README for what it can and cannot establish. Now also
> carries a four-file Run 4D design addendum, finalized, closing out open items before Run 4E builds
> against it.

---

## (d) Commit sequence — 4 commits, not 5–6, and why

1. **Inject verbatim** — the four attachments, byte-identical, into `ux-ui/03-ui-prototyping/`.
   `docs(ux-ui): inject 4D design addendum — 09 through 12, as approved for gate review`
2. **Index** — the new table in `03-ui-prototyping/README.md` + the new clause in `ux-ui/README.md`.
   `docs(ux-ui): index the 4D design addendum in 03-ui-prototyping and ux-ui READMEs`
   *(→ gate-1 STOP here, per task 5 — a checkpoint before the patch commit, separate from this
   plan's own approval)*
3. **Surgical patch** — (a) + (b) above, nothing else.
   `docs(ux-ui): resolve 4D open items and finalize 09 through 12 (Q1–Q3)`
4. **Archive** — plan-as-approved, Completion Report, archive-folder README, `handoff/README.md` row.
   `docs: archive v11 design addendum injection and resolution plan and completion report`

**Why not 5–6:** task 7 (docs-and-links) produces a commit only if a real change is needed beyond
what commit 2 already added. I checked: the four dossier files contain **zero** inline markdown
links (`grep` confirms — they cite other files via backtick file names, not `[text](target)` links),
and the live repo's individual gate files (`00_SCOPE.md`...`08_LIMITATIONS.md`) don't cross-link back
to their own README entry, so there's no established convention for the new files to follow either.
Task 7 therefore folds into task 9's report with no commit of its own, exactly as its own instructions
permit. Push (task 8) and archive-push happen around commits 1–4; neither is a fifth content commit.

---

## (e) Link-integrity baseline and method

The four dossier files carry **zero** markdown-style inline links (confirmed by `grep`), so this
run's own link surface is entirely the additions it makes: the 4 new links in commit 2's addendum
table, plus whatever links land in commit 4's archive folder (links to the plan file, the completion
report, and the new `handoff/README.md` row). **Method:** count every `[text](target)` outside code
fences introduced or modified by this run's own commits, resolve each as a relative filesystem path
against the live repo tree post-commit; report N/N once all four commits have landed.

---

## (f) Archive destination

`handoff/v11/`, confirmed against the live repo (`v1`–`v10` exist, `v11` is next; naming convention
confirmed from `v6`/`v9`/`v10`: `CC-PLAN-v11.md`, `Completion-Report-v11.md`, `README.md`). Plus one
new row appended to `handoff/README.md`'s existing list (style matched to the `v9`/`v10` entries
already there).

---

## Execution notes carried into tasks 3–10

- Task 3 diffs against the local attachments before staging — must be byte-identical.
- Task 5 is an in-run checkpoint (short SHAs + messages, byte-diff confirmation, plain restatement
  of the three resolutions) that I'll pause for before committing the patch — separate from this
  plan's own approval gate.
- Task 6 diffs each patched file against its task-3 (injected) version before staging — every
  changed line must trace to (a), (b), or the scope-note addendum if you approve it.
- Task 8 PR body is written from the actual task-6 diff, per-file before/after, after the patch
  lands — not drafted here in advance.
- Completion Report (task 10) states plainly whether the addendum is ready for Run 4E: yes, with the
  five items the dossier itself deliberately left open named as carried-forward (not blocking) items
  — the record's route choice and onboarding scope (`10` §8), the pluralization API (`11` §2.3), the
  CSS shadow-fallback contingency (`09` §6), and Tomás's exact zone-label wording (`10` §3.2).
- All commits authored/committed as `jdsaire`, branch `deploy/v11-design-addendum-injection`, PR
  against `main` left unmerged, zero AI attribution anywhere, no subagents, no PAT ever printed.

## Verification

- `git diff` of commit 1 against the original four local attachment files — must be empty.
- `git diff` of commit 3 against commit 1's versions — every hunk must map to (a) or (b) above (or
  the approved scope-note addendum).
- `git diff 147bc4a..HEAD` scoped to `src/` and to `00_SCOPE.md`...`08_LIMITATIONS.md` — must be
  empty.
- Re-run the link count from (e) after commit 4 lands; report N/N.
- `git log` on the branch — author/committer both `jdsaire`, no trailers, no AI mentions; same check
  on the PR title/body and branch name.
- `gh pr view` on the opened PR — confirm it targets `main`, is not merged, and its body contains a
  per-file before/after section for all four dossier files.
