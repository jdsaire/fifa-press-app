# v14 — ShopEase Realignment

Injects the completed `05-iteration` dossier (six gate deliverables — a design iteration that reads
a course-mate's e-commerce build, ShopEase, and realigns this app's navigation, settings, front
door, sign-in, and match list against it) into `ux-ui/05-iteration/`, then executes the sixteen-item
commit sequence that dossier's own `05_MANIFEST.md` specified across four gated workstreams: a
persistent top bar (closing **R9**), a Settings screen with a dropdown language control and a
tri-state appearance control (closing **R7**), a reduced Landing and condensed sign-in notice with
renamed demo identifiers (closing **R6** and **R8**), and a rebuilt `/matches` surface with
rule-based capacity, localized team names, Show-more pagination, an availability filter, and a
`/record` route that renders sign-in inline instead of redirecting (closing **R10**; opening and
closing **R11**, a corrected capacity scope).

**Not only that.** The Gate 4 stop's own live sign-in check — required before any fix, per this
run's hard rules — reproduced a rejection. Three further rounds of live testing and fixing follow
in the same session, each its own approved addendum: demo credentials replaced entirely and every
on-screen personal name removed; a real, previously-undetected sign-in crash root-caused to one
line (`@bind-Value:event="oninput"` on a Blazor component, where that syntax has no meaning) and
fixed, alongside a regression this run's own redirect-removal had introduced; a ShopEase-parity
layout pass, a collapsible sidebar, larger navigation text, and appearance-option icons; and a final
round of surgical fixes — including a second and third instance of the same CSS-scoping defect
class the crash investigation first uncovered.

Application behavior changes across navigation, settings, the front door, sign-in, and the match
list, plus all three locale files; test count grows from 421 to 512, all named exactly as each
document or addendum specified.

- [`CC-PLAN-v14.md`](CC-PLAN-v14.md) — all four plan documents this run produced, in the order they
  were approved: the original preflight and 17-commit sequence against the six gate documents, and
  the three addenda that followed the Gate 4 stop's own live test.
- [`Completion-Report-v14.md`](Completion-Report-v14.md) — the full 28-commit list with real SHAs,
  a PASS/FAIL table against every success criterion the original deploy named, every deviation and
  autonomous decision across all three addenda, the sign-in defect's actual root cause, and the
  link-integrity recount.

**Audited/executed against:** `main` @ `e996d8a`.
