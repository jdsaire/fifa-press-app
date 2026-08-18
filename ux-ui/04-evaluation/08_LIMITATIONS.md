# Limitations

**Repo path:** `ux-ui/04-evaluation/08_LIMITATIONS.md`
**No `00` equivalent** — `00_SCOPE.md` §8 flagged this as new to this dossier. `00` scattered its
caveats gate by gate; this run consolidates them into one place, on purpose, so nothing this
evaluation found gets read as more conclusive than it is.

**SIMULATED — NOT EVIDENCE.** This file is a statement of what the preceding eight gates cannot
claim, not an additional finding.

---

## 1. Method-level limits — apply to every prior gate, restated once rather than nine times

**No browser, no screen reader, no real render, at any point in this dossier.** Every
`CODE-VERIFIED` tag across `01`–`07` means "the source settles it," not "it was observed happening."
Every `REASONED` tag means an inference from source, not a measurement. This is the same limitation
`00` stated about itself; it did not go away because this run had more source to read.

**Reading source and reasoning about behavior is not the same as running the app.** Nothing in this
dossier can speak to Blazor WebAssembly hydration timing, real JavaScript-interop latency, actual
font metrics, or browser-specific focus-order quirks — the class of defect that only exists once
code is loaded into an actual runtime. The contrast ratios in `04_ACCESSIBILITY-AUDIT.md` were
independently recomputed against the WCAG formula and matched the shipped CSS exactly, which is
real, meaningful verification — but it is verification of the numbers in the file, not of what a
human eye sees under real screen conditions.

**Six task-attempts are simulated, authored walkthroughs, not recordings of real people.**
`02_TASK-RESULTS.md`'s own header says this. A 6-of-6 pass rate means the interface is internally
coherent against its own frozen task descriptions — it does not mean six real journalists would
succeed, would find the interface intuitive, or would trust what it told them.

---

## 2. Roster and coverage limits

- **Two of six roster members were ever exercised at all.** W1 (Amina) ran all three tasks
  genuinely. W2 (freelance) had no seeded demo record to run against at all —
  `05_FINDINGS-REGISTER.md`'s `04-MAJ-02` — so every W2 attempt substitutes Amina's record read
  under W2's *stated* constraint, not W2's actual data. Genuine freelance-track behavior — a
  refusal with no institutional escalation path, the "weakest appeal path" `01_TASK-PROTOCOL.md`
  §5 describes — remains completely untested by this dossier.
- **W3, W4, W6 were never touched by this evaluation, in any capacity.** Not out of oversight —
  they have no reason to hold this app's sign-in — but this run says literally nothing about
  whether the product serves their needs, because it was never asked to.
- **W5's navigation objection stays open**, restated from `00_SCOPE.md` §5: nothing in six
  end-user task-attempts is positioned to resolve a bulk-coordinator's complaint about a
  per-person surface. A clean pass on the tasks that exist does not imply anything about the task
  that doesn't.
- **Portuguese was never independently exercised by a task-attempt.** `01_TASK-PROTOCOL.md` §4's
  coverage requirement — "at least one non-English locale" — was satisfied by Spanish. The
  `11_I18N.md` §7 layout risk this dossier flagged (`04-MIN-01`) is itself specific to Spanish's
  string-length growth; Portuguese's own growth pattern was never separately checked against any
  container.
- **Six task-attempts, total, across the entire dossier.** Even accepting every attempt as valid
  simulated evidence on its own terms, six is not a sample size that supports any claim about
  consistency or reliability — one clean pass and one that happened to hit friction would look
  identical to a genuinely 50/50 outcome at this scale.

---

## 3. The premise this dossier still cannot touch

`00_SCOPE.md` §4 stated this before a single task ran, and nothing found since changes it: **the
interval premise (ID-01) — that a usable window exists between an entitlement change and its first
consequence, and that Amina's is roughly 72 hours — remains completely untested.** No real
participant exists in this project to establish it, and none will, by design (`00_SCOPE.md`'s own
purpose statement: all study data is simulated, and real-data thresholds don't apply here).

This matters specifically for how the 6-of-6 task success rate should be read. A clean pass means
the interface communicates its own published design position — the 72-hour window — coherently to
someone who already knows to look for it. It says nothing about whether 72 hours is the right
number, whether a real journalist would notice a change within that window at all, or whether the
premise the entire "elimination-driven churn" domain model rests on holds up outside this project's
own reasoning about itself.

**Task 3 tests one half of one interaction, by its own admission.** `04_TASKS-AND-SCENARIOS.md` §5
states plainly that Task 3 covers the holder-side half of Interaction 4.3 only — the barrier-side
half stays untestable while venue access list ownership is unresolved, an open item carried since
v9. This dossier did not close that gap; it inherited it, stated it once more (`01_TASK-PROTOCOL.md`
§2), and moved on, exactly as instructed.

---

## 4. What `07_REMEDIATION-SCOPE.md` itself cannot yet claim

Everything in Gate 7 is a specification, not code. Item 1 through Item 3b describe fixes precisely
enough for a CC run to build from, but **none of it has been built, compiled, or tested yet.**
Whether the fix to `StatusFor`/`status` is as clean as it looks on paper, whether the `Withdrawal`
affordance's inline confirm actually reads well against the app's real layout, whether the drafted
`[ASSUMPTION]`-tagged locale strings hold up against a native reviewer — all of that is unverified
until a real build happens and the tests in §4 of that file actually run and pass.

Two findings were deliberately left out of the remediation sequence rather than resolved:
`04-MAJ-02` needs the same kind of explicit authorization `UX-MAJ-06` (Withdrawal) received and
has not gotten it; `04-MIN-01` and `UX-MIN-06` need a render this dossier cannot produce and were
not fixed speculatively. Both stay genuinely open, not quietly closed by omission.

---

## 5. Baseline rollover, executed as `00_SCOPE.md` §7.4 committed to

Stated here explicitly, as promised at Gate 0: **`00-initial-evaluation/` is not cited again as the
operative baseline after this run.** From `04-evaluation` onward, any future evaluation of this app
compares against **this** folder's numbers — 2 Fails/1 Partial/7 Pass, 18/1/3, 1/2/2 — not `00`'s
pre-reskin figures. `00` remains valuable as the portfolio's origin point, the arc from a generic
capstone to an accredited product, but it stops being the thing a run-5 comparison table cites.

---

## 6. What this dossier is, honestly, inside its own portfolio context

This entire evaluation exists inside a project whose primary purpose is a portfolio deliverable for
a specific job application with a hard 31 August 2026 deadline (`00_SCOPE.md`'s own framing, carried
from this project's standing purpose). That context did not make any individual finding in this
dossier less true — the contrast ratios are real numbers, the `04-CRIT-01` defect is a real gap in
the shipped source — but it did bound this evaluation's scope and depth to what is achievable inside
that timeframe, not to what a production accreditation system serving real journalists at a real
World Cup would need before launch. Specifically out of reach by design, not oversight:

- **Real usability testing** with real journalists, replacing every one of this dossier's simulated
  walkthroughs with an actual recorded session.
- **A render-based accessibility pass** — an actual screen reader, actual viewport testing at 320px
  across all three locales, actual target-size measurement — closing the three items this dossier
  could only leave Open.
- **4C, the SignalR backend**, structurally blocked on a hosting decision outside this workflow —
  meaning every "real-time" claim anywhere in this project's documentation describes a simulated
  read, not a live system, and will continue to until that run happens.
- **Legal, compliance, or security review** of an actual credential-issuance system — never in scope
  for any run in this project, and not implied by anything in this dossier.

None of this is a hedge against the findings above. The findings hold. This section exists so that
a reader — including a hiring reviewer this project is ultimately built for — sees exactly where
the evaluation's honest edge is, rather than discovering it later as an unstated gap.

---

✅ **GATE 8 COMPLETE** — `08_LIMITATIONS.md`
✅ **`ux-ui/04-evaluation/` dossier complete** — nine files, `00_SCOPE.md` through `08_LIMITATIONS.md`
