# Scope Lock

**Repo path:** `ux-ui/04-evaluation/00-scope.md`
**Precedent:** `ux-ui/03-ui-prototyping/00_SCOPE.md` (gate process); `ux-ui/00-initial-evaluation/`
(file shape and naming)
**Mandate source:** `P-EVALUATION_FIFA_Run04-Scope.md` — §3 supplies the contract below, §4 the gate
structure, §6 the baseline-comparison requirement
**Audited build:** `main` @ `b37066d` — verified live at this gate, not carried from the mandate

---

## 1. What this mandate can and cannot establish

This mandate grades a finished build against a test protocol written two runs before it. It can
establish whether the shipped app satisfies the three frozen tasks, whether Nielsen's ten and WCAG
2.2 AA hold across both themes and three locales, and — for the first time in this repo — whether
the interface **moved** against a prior measurement rather than merely differing from it. It cannot
establish that any of it works for a real journalist. Every task attempt below is simulated, run by
authored participants against a build read in source rather than driven in a browser. What this
folder produces is a second point of measurement, not evidence of user success.

---

## 2. Live repo state — verified at this gate

The mandate instructed that this be re-derived rather than trusted. It was.

| Claim | Status | Evidence |
|---|---|---|
| `main` @ `b37066d` | **Confirmed** | Full clone; `git log` HEAD is the merge commit for PR #9 |
| PR [#9](https://github.com/jdsaire/fifa-press-app/pull/9) merged | **Confirmed** | `b37066d` = `Merge pull request #9 from jdsaire/deploy/v12-addendum-implementation` |
| `09_DESIGN-ADDENDUM.md` Final | **Confirmed** | Status line: Final, approved and injected at v11, 16 Aug 2026 |
| `10_AUTH-AND-ONBOARDING.md` Final | **Confirmed** | Same status line |
| `11_I18N.md` Final | **Confirmed** | Same status line |
| `12_DECISION-REVERSALS.md` Final | **Confirmed** | Same status line |
| `00-initial-evaluation/` intact, 7 files + README | **Confirmed** | All present; every file cites audited commit `0653b4e` |
| `04_TASKS-AND-SCENARIOS.md` present, §3–§5 = the three tasks | **Confirmed** | §3 read what changed and what it means · §4 find out what is about to change · §5 confirm your current state when you cannot reach the network |
| `ux-ui/04-evaluation/` does not yet exist | **Confirmed** | `ux-ui/` holds `00`–`03` only |
| v12 build health | **Confirmed as reported** | 409 tests passing, 0 failures; release build 0 warnings / 0 errors |

**Two v12 items bear directly on this run.** The root README staleness open since v10 is **resolved**
(commit `6734f8b`). `docs/project-plan.md` and `docs/grading-criteria.md` still describe the pre-v9
registration flow — flagged by v12, not fixed by it, and carried here for Gate 5 to dispose of
rather than decided at this gate.

**Route verification, since the mandate anticipated flowchart drift.** The record moved from `/` to
`/record` in 4E; `/` is now `Landing.razor`. The three task flowcharts name **screens**
("Screen: My Access"), not route literals, so the move does not by itself put any flowchart out of
date. Whether the screens they name still exist under those names is Gate 2's question, not this
one's.

---

## 3. The inherited contract — not reopened

Binding without re-derivation. Source cited for each; nothing here is this mandate's to change.

| # | Inherited term | Source |
|---|---|---|
| 1 | **The three tasks are frozen.** This mandate scores them; it does not rewrite them. A task whose flowchart no longer matches the shipped build is a **finding**, not licence to redraw the flowchart | `03-ui-prototyping/04_TASKS-AND-SCENARIOS.md` §3–§5 |
| 2 | **No task exists for Tomás, and none is invented.** Every task he would actually perform is a bulk task this app does not offer. 4E's two-record demonstration permits an *unscored observation*, not a seventh task | `04_TASKS-AND-SCENARIOS.md` §1; mandate §3, §4 Gate 2 |
| 3 | **W1 and W2 run the tasks — six attempts total.** Only they are marked as end users of a journalist-facing surface; W3, W4, W6 are federation-, FIFA- and venue-side roles with no reason to hold this app's sign-in | `02-ideation/01_WORKSHOP-PROTOCOL.md` §1 |
| 4 | **No new personas.** Standing rule across `01`–`03`; not touched to reach a rounder number | `01-design-research/05_ARTIFACTS.md`; `03/00_SCOPE.md` |
| 5 | **Nielsen's 10 + WCAG 2.2 AA**, the same set and the same AA floor `00` used — not re-selected, not re-argued, so Gate 3 and Gate 4 are comparable to `00` | `00-initial-evaluation/heuristic-evaluation.md`, `accessibility-audit.md` |
| 6 | **Build for Amina; every decision must survive Tomás.** The evaluation lens, not just the build's | `01-design-research/05_ARTIFACTS.md` |
| 7 | **`09`–`12` are Final and frozen.** A fix may correct an *implementation* gap against them; it may not contradict a decision they made | Mandate §7 decision 8 |
| 8 | **Every one of `00`'s 26 findings gets an explicit disposition** — Fixed, Still Open, Regressed, or Not Applicable. None may be silently dropped | Mandate §1, §4 Gate 5 |

---

## 4. The premise stays untested

The interval premise (ID-01) — that a usable window exists between an entitlement change and its
first consequence, and that Amina's is roughly 72 hours — **remains untested and is not tested
here.** No real participants exist to establish it. `04_TASKS-AND-SCENARIOS.md` §2.2 sets 72 hours
as a *published design position*, and this run evaluates whether the interface communicates that
position coherently. It does not and cannot evaluate whether the position is correct.

Stated at this gate specifically so that a clean pass at Gate 2 cannot be read as validation. Six
simulated task attempts succeeding would mean the interface is internally coherent to its own
author. It would not mean the premise under it holds.

---

## 5. W5's navigation objection stays open

W5 — the rights-holding broadcaster's crew coordinator, the seated dissenter, the Tomás archetype —
objects that a per-person notification surface adds work to someone managing 40–120 credentials
against a spreadsheet that already works. Nothing 4E built addresses this. Nothing in this mandate
is positioned to resolve it: the six task attempts run on end-user surfaces, and the bulk-coordinator
question lives outside them.

Gate 8 restates this plainly. A clean evaluation of the end-user surfaces does not close the
bulk-coordinator question, and this folder must not be readable as though it did.

---

## 6. The `src/`-touch boundary

| | This dossier (Gates 0–8) | The CC run that follows |
|---|---|---|
| `src/FifaPressApp/` | **Read only.** Cited by file and line; never modified | **Written**, per `remediation-scope.md` |
| `ux-ui/00-initial-evaluation/` | Read only — frozen historical record | Not touched |
| `ux-ui/03-ui-prototyping/` incl. `09`–`12` | Read only — Final and frozen | Not touched |
| `ux-ui/04-evaluation/` | Authored here, in chat | **Injected verbatim** |
| `tests/`, `docs/`, `learning-mode/` | Read only | Per the remediation table only |

No gate in this dossier produces code. If a gate's instruction would require it, that is a conflict
to report, not to resolve by writing code.

---

## 7. The baseline — what it is, whether it still applies, and what happens after this run

**7.1 — Provenance, confirmed.** All three of the figures below belong to `00-initial-evaluation/`,
locked to **commit `0653b4e`**, audited under the app's pre-rename name, **EventEase** — before the
Access Record concept existed, before any persona shaped a design decision, before theme, auth,
i18n, interop, or disclosure were built. Every file in `00` states this about itself in its own
historical-record disclaimer. Nothing below was re-derived against the current build; "verified"
means the transcription from `00`'s own files was checked against those files, not that the
underlying audit was re-run against `v12`.

**7.2 — Whether `00` still counts as the baseline for this run: yes.** The scope-of-difference
between `00` and the current build is real — different concept, four major feature areas added
since — but it is not disqualifying, for three reasons. First, it is the same codebase lineage: the
same repo, the same component patterns evolved forward, not rewritten from scratch — six of `00`'s
ten weak heuristics traced to one structural decision (`EventCard`-as-editable-form) whose
descendants remain traceable through today's files. Second, the mandate already specifies the
correct instrument for a baseline this different: the **Not Applicable** disposition. A `00` finding
whose surface no longer exists in that form is marked N/A with its replacement named — not forced
into a false "Fixed," not silently dropped. That mechanism is what makes a pre-reskin baseline honest
rather than misleading. Third, `00` is the only prior measurement this repo has; recalculating from
scratch produces no comparison at all, permanently deferring the first real before/after and
forfeiting exactly what this run is scoped to produce (mandate §1, §6: "the repo's second point of
measurement — the first time this project can show movement across an entire arc").

**7.3 — The standing caveat this comparison requires.** Because the span is concept-plus-implementation,
not implementation alone, `remediation-scope.md`'s movement table must carry a note that a shrinking
Critical/Major count means "the rebuilt surfaces got safer," not "the same interface got better." Raw
deltas are not self-explanatory across a reskin; Gate 7 states this framing explicitly rather than
letting the numbers imply more continuity than exists.

**7.4 — Roll-forward: `00` is not cited again as the operative baseline after this run.** Once this
folder's numbers land, **`04-evaluation` becomes the reference point for run 5 and any run after
it** — from here on, comparisons are same-shaped-app to same-shaped-app (post-reskin, full feature
set: auth, three locales, two themes, disclosure), which is a structurally sound trajectory in a way
`00→04` cannot fully be. `00` remains valuable as the portfolio's origin point — the arc from generic
capstone to accredited product — but ceases to be *the* baseline the moment `04` exists. `limitations.md`
(Gate 8) states this rollover explicitly, so a future run does not quietly keep citing `00` after a
better reference is available.

**7.5 — A count that did not survive re-verification, corrected here and carried forward.**
`P-EVALUATION_FIFA_Run04-Scope.md` §6's comparison table cites "Nielsen principles failing: 6 of 10."
The live file does not support that number. `00-initial-evaluation/heuristic-evaluation.md`'s own
at-a-glance table records:

| Assessment | Count | Heuristics |
|---|---|---|
| Fails | **5** | 2, 3, 4, 6, 7 |
| Partial | **4** | 1, 5, 8, 9 |
| Passes | **1** | 10 |

The "six" was carried from that file's closing analysis — *"Six of the ten weak results —
heuristics 2, 3, 4, 5, 6, and 8 — trace back to one decision"* — which counts a **different set**
(includes 5 and 8, both Partial; excludes 7, which Fails) and measures root-cause clustering, not
failure count. **Resolution for this run:** Gate 3 reports **three counts — Passes / Partial /
Fails — not a single "failing" figure**, and §6's movement table carries all three against `00`'s
1 / 4 / 5. Recorded here rather than quietly substituted — the same disposition v12 gave the
unverifiable 299/288 link-integrity baseline. The 6-of-10 figure should not be cited again.

**7.6 — The other two baseline figures, provenance-confirmed, transcription-verified, uncorrected:**

| Baseline | `00`'s figure | Source |
|---|---|---|
| WCAG 2.2 AA | Pass 8 · Fail 12 · Open, needs a human 3 — does not meet AA | `accessibility-audit.md` |
| Findings register | 4 Critical · 12 Major · 10 Minor = 26 | `findings-register.md` |

Both hold as stated. Neither required correction; §7.5's issue was specific to the heuristic count.

---

## 8. What this folder produces

Nine files, numbered for direct repo-injection traceability, in `00`'s flat-file spirit with the
`01`–`03` gate-numbering convention layered on top of the filename itself:

| # | Filename | Gate | Maps to `00` |
|---|---|---|---|
| 0 | `00-scope.md` | Gate 0 — Scope Lock | *(new — no `00` equivalent)* |
| 1 | `01-task-protocol.md` | Gate 1 — Task-Based Usability Protocol | `usability-test-protocol.md` |
| 2 | `02-task-results.md` | Gate 2 — Task Execution | `protocol-results.md` |
| 3 | `03-heuristic-evaluation.md` | Gate 3 — Heuristic Evaluation | `heuristic-evaluation.md` |
| 4 | `04-accessibility-audit.md` | Gate 4 — Accessibility Audit | `accessibility-audit.md` |
| 5 | `05-findings-register.md` | Gate 5 — Findings Register | `findings-register.md` |
| 6 | `06-usability-assessment.md` | Gate 6 — Usability Assessment | `usability-assessment.md` |
| 7 | `07-remediation-scope.md` | Gate 7 — Movement Summary & Remediation Scope | `remediation-scope.md` |
| 8 | `08-limitations.md` | Gate 8 — Limitations | *(new — no `00` equivalent)* |

Plus a folder `README.md` and one new bullet in `ux-ui/README.md`. Six of the nine map onto an
existing `00` file by content; the leading number is this run's own addition, not a departure from
`00`'s naming — `00` predates any folder in this repo needing cross-run injection traceability, so
its files never needed the prefix.

**Findings-ID scheme:** `04-CRIT-NN` / `04-MAJ-NN` / `04-MIN-NN`, distinct from `00`'s
`UX-C-NN` / `UX-MAJ-NN` / `UX-MIN-NN`, with `00`'s severity-band rule text unchanged so a Critical
means the same thing in both registers.

**The `SIMULATED — NOT EVIDENCE` header goes on `02-task-results.md` and `08-limitations.md`** — the
files carrying simulated participant content — matching how `02-ideation/` and `03-ui-prototyping/`
apply it. `00-scope.md` does not carry it: everything above is a verified read of a real repo.

**Web access is off.** Nothing in this mandate needs an external source.

---

## 9. Model, recorded as a deviation from a deviation

`P-EVALUATION_FIFA_Run04-Scope.md` §7.4 specifies **Sonnet 5 High Effort at every stage**, itself an
explicit override of this project's standing rule (Opus for code-intensive runs, Sonnet for dossier
authoring). **This gate was authored on Opus 5 High Effort instead**, at the principal's instruction,
to give the gated pipeline the most solid foundation available at its first file.

This is a second-order deviation and is recorded as one. It does not silently reset §7.4: the model
for Gates 1–8, for the `/cc-deploy-prompts` authoring, and for the CC execution remains **Sonnet 5
High Effort** unless changed by the same explicit instruction. Named rather than smoothed over, in
the register `12_DECISION-REVERSALS.md` established for this project.

---

✅ **GATE 0 COMPLETE** — `00-scope.md`
