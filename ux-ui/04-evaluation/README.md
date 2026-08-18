# Evaluation & Remediation Scope — Run 04

*(This is the second point of measurement this repository has produced, run against the app after
`00-initial-evaluation/`'s pre-reskin baseline. From this folder onward, `00`'s numbers stop being
the operative comparison — see `08_LIMITATIONS.md` §5.)*

A task-based usability and accessibility re-audit of the app as it stood after v12
(`main` @ `b37066d`), followed by a remediation scope sequencing the fixes it found into
commit-sized units.

Unlike `00-initial-evaluation/`, this run has something to score against: **six simulated
task-attempts, run against the shipped build rather than derived from it in the abstract.** It is
still not real usability testing — no person sat down with this app — and the dossier says so at
every gate that matters, most fully in `08_LIMITATIONS.md`.

---

## What was found

| Severity | `00-initial-evaluation/` | This run | Movement |
|---|---|---|---|
| Critical | 4 | **1** (`04-CRIT-01`, new) | ↓↓↓ |
| Major | 12 | **2** (new) | ↓↓↓↓↓ |
| Minor | 10 | **2** (new) | ↓↓↓↓↓ |
| Nielsen's 10 | 5 Fails · 4 Partial · 1 Pass | **2 Fails · 1 Partial · 7 Pass** | ↑↑↑↑↑↑ |
| WCAG 2.2 AA | 8 Pass · 12 Fail · 3 Open | **18 Pass · 1 Fail · 3 Open — meets AA on every settleable criterion** | ↑↑↑↑↑↑↑↑↑↑ |
| Task success | N/A — no tasks existed | **6 of 6 (100%)** | New this run |

All 26 of `00`'s findings received an explicit disposition (18 Fixed, 5 Not Applicable, 2 Still
Open, 1 Partially Fixed) — none silently dropped. One critical finding, `04-CRIT-01`, is responsible
for most of what's left: the single highest-value remediation item this dossier identifies.

---

## The files

Read in gate order.

| # | File | What it covers |
|---|---|---|
| 0 | [`00_SCOPE.md`](00_SCOPE.md) | Scope lock: live repo state, the inherited contract from `00`, the untested interval premise, and the baseline roll-forward this run commits to |
| 1 | [`01_TASK-PROTOCOL.md`](01_TASK-PROTOCOL.md) | The task-based usability protocol — three frozen tasks, six roster attempts, locale/theme coverage requirements |
| 2 | [`02_TASK-RESULTS.md`](02_TASK-RESULTS.md) | Six task-attempt walkthroughs against the real build, plus Tomás's unscored observation. Surfaces `04-CRIT-01` |
| 3 | [`03_HEURISTIC-EVALUATION.md`](03_HEURISTIC-EVALUATION.md) | Nielsen's ten heuristics, re-run: 7 Passes, up from 1 |
| 4 | [`04_ACCESSIBILITY-AUDIT.md`](04_ACCESSIBILITY-AUDIT.md) | WCAG 2.2 AA, re-run: 18 Pass, up from 8 |
| 5 | [`05_FINDINGS-REGISTER.md`](05_FINDINGS-REGISTER.md) | Every `00` finding dispositioned; five new findings raised, `04-CRIT-01` through `04-MIN-02` |
| 6 | [`06_USABILITY-ASSESSMENT.md`](06_USABILITY-ASSESSMENT.md) | The five usability components, scored against real task evidence for the first time |
| 7 | [`07_REMEDIATION-SCOPE.md`](07_REMEDIATION-SCOPE.md) | The movement summary and the sequenced fix specification (Items 1–3b) this run's code changes are built from verbatim |
| 8 | [`08_LIMITATIONS.md`](08_LIMITATIONS.md) | What this whole evaluation cannot claim — consolidated into one closing statement |

---

## How this run differs from `00`

`00` had no tasks and no participants — a checklist run against source code, in the abstract. This
run scores **six task-attempts** (two frozen tasks × three roster members short one) against the
shipped build, screen by screen, citing real routes, real components, and real copy. That is a real
methodological step up, and every claim below leans on it wherever task evidence exists — where it
still rests on inference from source alone, that is marked too.

Two findings were deliberately left out of the remediation sequence rather than resolved
(`04-MAJ-02`, no Freelance-track demo record; `04-MIN-01`/`UX-MIN-06`, needing a render this dossier
cannot produce) — flagged, not silently closed. See `07_REMEDIATION-SCOPE.md` §5.

**Audited build:** `main` @ `b37066d`.
