# Design Research

A simulated guerrilla UX research pass on the FIFA media-accreditation platform: what a journalist
needs to be told, and how fast, at the moment their access falls apart after they thought it was
already settled — a rejection, a mid-tournament reallocation, or a denial at the stadium gate.

This is not an audit of the app in this repo, and it is not empirical evidence about real
journalists. It is a documented chain of reasoning — from a sourced evidence base, through an
eight-gate research process with no human participants, to a set of design decisions each traceable
back to its source and labelled by how much weight it can actually carry. Start with
[`00_SCOPE.md`](00_SCOPE.md) for exactly what this study can and cannot establish, and
[`07_LIMITATIONS.md`](07_LIMITATIONS.md) for the full methodological defence of that boundary.

---

## The eight gates, in order

| Gate | File | What it delivers |
|---|---|---|
| 0 | [`00_SCOPE.md`](00_SCOPE.md) | Scope lock — what the study can and cannot establish, the preserved maximalist scope, and which evidence gaps this study addresses. |
| 1 | [`01_RESEARCH-PLAN.md`](01_RESEARCH-PLAN.md) | Research plan — the SMART goal, two hypotheses, traceability matrix, and sample design (n=30 survey, n=5 interviews). |
| 2 | [`02_INSTRUMENTS.md`](02_INSTRUMENTS.md) | Instruments — screener, consent notice, 12-item survey, and semi-structured interview guide. |
| 3 | [`03_SIMULATION-NOTE.md`](03_SIMULATION-NOTE.md) | Simulation methodology note — how the two datasets below were constructed and their declared limitations. |
| 3 | [`survey_master.csv`](survey_master.csv) | Simulated survey dataset — 30 synthetic respondents. Sibling to Gate 3's note, not a separate gate. |
| 3 | [`interviews_master.json`](interviews_master.json) | Simulated interview dataset — 5 synthetic participants. Sibling to Gate 3's note, not a separate gate. |
| 4 | [`04_ANALYSIS.md`](04_ANALYSIS.md) | Analysis — quantitative descriptives and two inferential tests, plus qualitative coding and themes. |
| 5 | [`05_ARTIFACTS.md`](05_ARTIFACTS.md) | Artifacts — two personas, one empathy map, three scenarios, and a service blueprint of the accreditation-to-matchday lifecycle. |
| 6 | [`06_DESIGN-BRIEF.md`](06_DESIGN-BRIEF.md) | Design brief — the How Might We question, design principles, scope boundary, and success metrics. |
| 7 | [`07_LIMITATIONS.md`](07_LIMITATIONS.md) | Methodological disclosure — defends every scope decision and states plainly what the deliverables do and do not support. |

## Glossary

[`GLOSSARY.md`](GLOSSARY.md) is not a gate deliverable — it was added after the eight-gate process
completed, to define every research, software, and football-administration term a reader outside
those worlds might not know.

## Relationship to `ux-ui/00-initial-evaluation/`

[`../00-initial-evaluation/`](../00-initial-evaluation/) is a separate, earlier, unrelated mandate
track: a usability and accessibility audit of the EventEase-era app as it existed in this repo
before the FIFA Press App reframing. That folder asks whether the app that was built is usable.
This folder asks what a research process — run without access to real users — can responsibly say
about what the accreditation experience should tell a journalist. Neither folder's findings extend
into the other. The two now sit side by side under [`ux-ui/`](../README.md), which indexes every
UX dossier for this app.

## A note on two files this folder does not link

The source study references `FIFA_app_P-Research.txt` and `P-Fifa-Repo-Kickoff.txt` for the
project's original framing. Neither file exists in this repository at the time of this folder's
addition, so no link to them is made here. This is a documented gap, not an oversight — see this
run's Completion Report in [`handoff/v5/`](../../handoff/v5/) for the resolution.
