# 00 — SCOPE LOCK

**Repo path:** `ux-ui/03-ui-prototyping/00_SCOPE.md`
**Precedent:** `01-design-research/00_SCOPE.md`, `02-ideation/00_SCOPE.md`
**Source note:** authored with `02-ideation/05_CONCEPT.md` and `01-design-research/05_ARTIFACTS.md` supplied directly. `02-ideation/06_HANDOFF.md` §7 was not available as a live source and is not quoted or paraphrased here; nothing below depends on it, since the contract table is built from `05_CONCEPT.md` and `05_ARTIFACTS.md` directly.

---

## 1. What this mandate can and cannot establish

This mandate turns the Access Record concept into a buildable specification: information architecture, UI decisions, a bounded task set, screen specs, and a data model — none of it code. It can establish *what* Run 4B should build and *why*, traceably, from prior evidence and prior simulated work. It cannot establish that the concept is correct. No real journalist has seen a card sort, a screen, or a task. The premise itself — that a usable interval exists between an entitlement change and its first consequence — is untested and stays untested here (see §4). What this dossier produces is a defensible design position, not a validated one.

*(97 words)*

---

## 2. Inherited contract

| Element | Content | Source file |
|---|---|---|
| **Concept** | Access Record — accreditation as a live state object with an append-only entry log; every change is written as an entry (what changed, why, what's possible now) before it takes effect, read identically by holder and venue access list | `02-ideation/05_CONCEPT.md` §1 |
| **Three interactions** | 4.1 a change lands · 4.2 a change becomes foreseeable · 4.3 the state is enforced — the complete inventory; no fourth may be added | `02-ideation/05_CONCEPT.md` §4 |
| **Principle 1 + must-never-do** | Every state change announces itself → never let a change be committed by a route that bypasses the log | `02-ideation/05_CONCEPT.md` §3 |
| **Principle 2 + must-never-do** | A refusal is a message, not a wall → never write an entry with an outcome alone; a reason satisfied by a status label is not a reason | `02-ideation/05_CONCEPT.md` §3 |
| **Principle 3 + must-never-do** | Serve the people who have no one to call → never broadcast one entry set to every track; never add per-person interaction to a bulk coordinator | `02-ideation/05_CONCEPT.md` §3 |
| **Scope boundary** | Visas, security-vetting outcomes, quota allocation politics, the MA relationship, guaranteed access — OUT. A screen resembling an appeal channel is a violation regardless of label | `02-ideation/05_CONCEPT.md` §5 |
| **Persona — Amina R.** | Quota-dependent reporter, mid-size national outlet; abandons the tool if status is stale or absent twice running | `01-design-research/05_ARTIFACTS.md` §1.1 |
| **Persona — Tomás L.** | Rights-holder crew coordinator, 40–120 people; abandons the tool if it adds per-person clicks or floods him with individual-scoped notifications | `01-design-research/05_ARTIFACTS.md` §1.2 |
| **Carried-forward unknown — the interval** | Whether a usable window exists between an entitlement change and its consequence (ID-01) is unresolved; nothing may depend on it being real | `02-ideation/05_CONCEPT.md` §6 Q1 |
| **Carried-forward unknown — two-token confusion** | Whether accreditation-vs-ticket confusion is novice or persistent stays unresolved; no concept element rests on resolving it | `02-ideation/05_CONCEPT.md` §6 Q4 |

---

## 3. Preserved maximalist scope — the full programme NOT run

| Activity not run | What it would have established | Consequence of the gap | Severity |
|---|---|---|---|
| Real usability testing of the interval premise | Whether a change-to-consequence window actually exists for Amina's track | Every downstream screen assumes a window that may be zero or too short to act on | 3 |
| Moderated card sort with real journalists and coordinators | Whether the ontology and taxonomy chosen in Gate 2 match real mental models, not W1–W6's | IA may optimize for a simulated roster's disagreements rather than real ones | 2 |
| Live accessibility audit with assistive-technology users | Whether the WCAG 2.2 AA floor is sufficient in practice, not only on paper | Contrast and structure may pass review yet still fail real usage | 2 |
| Full competitive teardown with account-holder access | Behaviour of tracking, alert and ETA systems past their public-facing screens | Gate 1's adopt/avoid table is built from partial visibility into the closest analogues | 2 |
| Field study of accreditation-centre pickup and matchday gate flow | Ground truth for Interaction 4.3's enforcement moment | The "state is enforced" interaction is specified from Run 1 inference, not observation | 2 |
| Load and scale testing against Run 1's media population | Whether the architecture holds under real volume and push-scoping load | Data model and service abstraction are specified for correctness, not throughput | 1 |

Documentation only; no remediation plan.

---

## 4. The premise-untested declaration

Handoff recommendation 1 — test the premise before building the concept — cannot be executed. There are no real users available to establish whether a usable interval exists between an entitlement change and its first consequence (ID-01, `02-ideation/05_CONCEPT.md` §6 Q1). This dossier proceeds on that unvalidated premise, by declared constraint, not by oversight.

The cost is specific: every screen state, every notification-timing decision, and every task flow in Gates 4–5 is specified as if the window exists and is long enough to act on. If it does not, the Access Record is a better-explained failure rather than a prevented one, and no artifact in this mandate can detect that from where it sits. This is stated once here, properly, and once more in Gate 8. It is not restated as a hedge in every gate between.

---

## 5. The `src/`-touch boundary

All paths use `src/FifaPressApp/`, current at repo HEAD `03663db`.

| Status | Scope |
|---|---|
| **Frozen — not touched by 4B** | `.github/workflows/deploy-pages.yml` (owned by run 4-R); any backend, API, database or auth code (Run 4C's scope) |
| **Modified — existing files 4B may change** | `wwwroot/css/app.css`, `Layout/MainLayout.razor.css`, `Layout/NavMenu.razor.css` per Gate 3's CSS-inheritance audit; navbar brand, page `<title>` and meta description strings (the EventEase rebrand, decided in Gate 3); existing components where reused per Gate 3's component inventory |
| **Created — new files 4B writes** | Models for fixture, accreditation and entry (log record) per Gate 6; a state/log service behind an interface per Gate 6; new pages per Gate 5, including the login form; `learning-mode/` restructure and new chapter, written by the build agent after code lands (Gate 7) |

Exact file-level detail is Gate 7's job; this is the boundary the later gates are held to.

---

## 6. The CSV reversal

`P-Fifa-Repo-Kickoff.txt` rejected wiring `2026_World_Cup_Schedule.csv` into the app, and both prior mandates repeated the prohibition. What was rejected was the CSV as a *substitute for design work* — injecting real match data so a registration app would look like a World Cup product without solving anything for a journalist.

What changed is not the judgment about shortcuts. It is that the concept now contains an interaction that structurally requires a schedule. Interaction 4.2 — a change becomes foreseeable — depends on knowing which fixture a person's entitlement depends on and when it resolves. That is exactly a schedule: date, matchup, phase, venue, city. The CSV enters at Gate 6 as the *dependency source* for a designed interaction, not as decoration standing in for design. A reader who finds the earlier prohibition without this paragraph would reasonably conclude the discipline slipped; it did not — the object the data serves changed.

---

## 7. The Mermaid exception

Every prior mandate banned diagram markup entirely; this one lifts that ban once, narrowly, for fenced Mermaid code blocks in Gate 4 only, because a task flow with boolean decision points cannot be expressed as a table without losing its branching — everywhere else in this dossier the no-diagrams rule still holds.

---

✅ GATE 0 COMPLETE — `00_SCOPE.md`
