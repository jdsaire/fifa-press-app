# Completion Report: v12 Addendum Implementation

Run 4E, per `CC-PLAN-v12.md`. Executed against `main` @ `ac5555c` on branch
`deploy/v12-addendum-implementation`.

## Recovery note

This is the second attempt at this run. The first was lost when the session scratchpad holding
the only copy of the working clone was wiped externally, mid-run, before the branch had ever been
pushed to the remote — 14 commits across three boundaries gone with it. `main` was unaffected, at
`ac5555c` throughout. Full detail, including the root cause and what survived, is at
`~/.claude/recovery/v12-addendum-implementation-RECOVERY.md`. This run cloned fresh from `ac5555c`
and pushed the branch after the very first commit and after every boundary since — the corrective
action the recovery record specified.

## Commits

| # | SHA | Message |
|---|---|---|
| 1 | `ee969e3` | `feat(theme): re-derive the dark palette against a #000000 anchor` |
| 2 | `3992d6b` | `feat(theme): relocate the theme trigger into the nav list` |
| 3 | `8281726` | `feat(auth): add the demo account store and simulated session` |
| 4 | `d52e4ba` | `feat(auth): seed the second demo record and its change list` |
| 5 | `f35de55` | `feat(auth): add the public landing view and move the record to /record` |
| 6 | `88a52cb` | `feat(auth): rewrite the sign-in screen around a working simulated session` |
| 7 | `283bf79` | `feat(auth): gate the record and the request write path` |
| 8 | `20392aa` | `feat(auth): add sign-out and the signed-in indicator to the nav list` |
| 9 | `ead83b5` | `feat(i18n): add the locale service and per-locale resources` |
| 10 | `723ef84` | `feat(i18n): make Change's free-text fields locale-keyed` |
| 11 | `a96d663` | `feat(i18n): translate the static UI surfaces` |
| 12 | `db94582` | `feat(i18n): add the language switch as a nav row` |
| 13 | `62683bc` | `feat(i18n): format dates and pluralized durations per locale` |
| 14 | `2d5b131` | `feat(i18n): extend match search to the active locale's labels` |
| 15 | `44ba6c2` | `build(interop): add the TypeScript sources and local compile step` |
| 16 | `c8d84fd` | `build(interop): author the theme and locale interop in TypeScript` |
| 17 | `5851c37` | `feat(help): give change rows a two-layer disclosure` |
| 18 | `4d720eb` | `feat(help): make Help sections independently collapsible` |
| 19 | `dab3d60` | `fix(i18n): default a first visit to English rather than the browser's language` |
| 20 | `6734f8b` | `docs: update the root README, how-to-run, and folder READMEs` |
| 21 | `2293efc` | `docs: add the learning-mode chapter for this run` |
| 22 | *(this commit)* | `docs: archive v12 addendum implementation plan and completion report` |

Five gated boundaries — visual identity (1–2), auth and landing (3–8), EN/ES/PT (9–14), TypeScript
interop (15–16), disclosure patterns (17–18) — each reported and approved before its successor
began, plus one authorized post-gate correction (19) and documentation (20–22). 23 commits against
the plan's estimated 21; the difference is the boundary-4 default reversal, a fix the plan could not
have anticipated because the item it corrects was itself flagged only at that gate.

**Pull request:** opened against `main`, left unmerged per push policy — see the note at the end of
this report on the blocker encountered opening it.

## Outcome

Every invariant the plan named held for the entire run. The withholding rule — no fixture whose
kickoff has not passed carries a team name — was proven on both seeded records, in all three
languages, and through both the canonical and locale-extended search paths. The two-record
demonstration works exactly as specified: `ch-008`, structurally identical to Amina's `ch-005`,
resolves Foreseeable for her and Silent for Tomás, produced entirely by `Track.NotificationCeiling`
and `Change.DeriveUrgency` with no new logic in either. The `11_I18N.md` §5.3 discrepancy — see its
own section below — was confirmed, built as Option B actually requires, and left unedited in the
Final file. `FifaPressApp.csproj`, `ux-ui/`, `wwwroot/lib/`, and `.github/` are byte-identical to
`ac5555c`, confirmed by an empty `git diff` on each path individually at this commit. Tests grew
82 → 409 with zero failures at any point; the release build carried 0 warnings and 0 errors after
every one of the 22 commits, checked individually.

## PASS/FAIL against the plan's verification checklist

| # | Check | Result | Evidence |
|---|---|---|---|
| 1 | `dotnet build src/FifaPressApp -c Release` clean, after each commit individually | **PASS** | Rechecked at every SHA; `0 Warning(s)`, `0 Error(s)` throughout |
| 2 | `dotnet test tests/FifaPressApp.Tests` green; final count vs. the 82 baseline | **PASS** | 409 passed, 0 failed, 0 skipped at HEAD. Progression: 82 → 129 → 219 → 251 → 274 → 302 → 322 → 343 → 361 → 371 → 389 → 409 across the 18 feature/build commits |
| 3 | `git diff ac5555c..HEAD` empty for `FifaPressApp.csproj`, `ux-ui/`, `wwwroot/lib/` | **PASS** | Each diffed individually; 0 changed files in all three |
| 4 | `.github/workflows/deploy-pages.yml` unchanged, shown as an empty diff | **PASS** | `git diff ac5555c..HEAD -- .github/` → 0 changed files. Confirmed additionally by `InteropTests.TheDeploymentWorkflowNeedsNoNode`, which asserts the file names no `setup-node`, `npm`, `tsc`, or `node_modules` |
| 5 | Withholding: the whole-schedule test passes unmodified, for both records, in all three locales | **PASS** | `TwoRecordsTests.TheWithholdingRuleHoldsAcrossTomassRecord`, `LocalizedChangeTests.TheWithholdingRuleHoldsInEveryLanguage`, `LanguageSwitchTests.TheWithholdingRuleHoldsOnScreenInEveryLanguage`, `LocalizedSearchTests` (5 tests) |
| 6 | Search: every frozen search test passes unmodified | **PASS** | `FixtureQuerySearchTests.cs` — 0 lines changed, `git diff ac5555c..HEAD` confirms |
| 7 | No read path gained latency | **PASS** | `GetAccreditationAsync`/`GetChangesAsync`/`GetFixturesAsync`/`GetFixtureAsync` all still return already-completed tasks; only `RequestMatchAccessAsync` and `SimulatedSessionProvider.SignInAsync` carry a simulated delay, each with its own regression test |
| 8 | Zero AI product names anywhere in diff, messages, branch, PR; zero "built in TypeScript" phrasing | **PASS** | Full diff and every commit message swept for AI product/vendor name patterns — zero hits. `InteropTests.NothingInTheRepoClaimsTheAppIsBuiltInTypeScript` guards the second phrase in source; every description in this report and the plan uses "authored in TypeScript" instead |
| 9 | Links N/N against the 288/299 baseline | **See below** — method changed; both counts re-verified under one consistent method | |
| 10 | All commits on `deploy/v12-addendum-implementation`, authored `jdsaire`, PR opened against `main` and left unmerged | **PASS on authorship; PR blocked** — see final section | `git log ac5555c..HEAD --format='%an|%ae'` → one distinct value, `Juan Diego S.|88201583+jdsaire@users.noreply.github.com`, for all 22 commits |

## Link-integrity sweep

**Method restated, and corrected from the recovery record's citation of it.** The 299/288 baseline
quoted at the top of this run's plan was carried from the prior (lost) attempt's own preflight and
was not independently re-derived at the start of this run — it should have been. Re-measured now,
with the method stated precisely: every inline `[text](target)` link in every git-tracked `.md`
file, outside fenced code blocks and inline code spans, excluding `http(s):`/`mailto:`/`tel:` and
bare `#fragment`-only links, resolved as a relative filesystem path from its containing file and,
where it carries a `#fragment`, as a heading slug within the target file.

| Point | Result |
|---|---|
| **`main` @ `ac5555c`, measured now with this method** | **284/289** |
| **This branch, including this archive commit** | **305/310** |

Both counts carry the same five known-inert failures, unchanged in identity between the two
measurements:

- `handoff/v6/Completion-Report-v6.md → v5/` — a historical path predating the v5 relocation, inside
  a frozen `handoff/` record this run may not alter.
- Four `Glossary.md#...` fragment links (`#razor-file--razor-syntax`, `#route--routing`,
  `#render--re-render`, `#wwwroot--static-files`) whose target headings contain a backtick or a
  slash. These are a checker-precision artifact, not a broken destination — a person clicking any of
  the four lands on the right page and finds the right heading a short scroll away — and they
  predate this run: reproduced identically against `ac5555c` before any v12 change existed.

**Net for this run: 21 links added, 21 resolve.** `305 − 284 = 21` and `310 − 289 = 21`; no new
failure of either kind was introduced. The prior run's 299/288 citation cannot be reconciled against
this measurement and should not be treated as authoritative — it was never independently verified,
only carried forward, which is exactly the failure mode a "measure it yourself" rule exists to
prevent. Flagged here rather than quietly substituted.

## The `11_I18N.md` §5.3 discrepancy

Confirmed, built, and left unedited, per the plan's task 1 and the addendum's own Final status.

§5.2 decides Option B for the language switch: an in-session re-render with no reload. §5.3 still
reads "regardless of which option 4E picks" and asserts that switching language while signed in
signs the person out, deriving that explicitly from a locale-triggered *reload*. Under Option B
there is no reload, so the premise the derivation rests on does not hold.

**What was built:** the session survives a language switch. `LanguageSwitchTests` pins this
directly — `TheSessionSURVIVESALanguageSwitch`, `ASwitchDoesNotNavigate`,
`TheSwitchWarnsAboutNothing_BecauseThereIsNothingToWarnAbout` — asserting the signed-in holder, their
credential, and the sidebar indicator are all unchanged across a switch, that no navigation occurs,
and that the switch's own UI carries no warning about a session ending, because nothing about it
does.

`11_I18N.md` was not edited; it is Final and frozen to this run, exactly as the other three
addendum files were. This section is where the correction belongs until a later addendum run can
update the document itself.

## Authorized deviations from the plan

1. **Additive search extension.** `FixtureQuery.Search` gained a second overload matching a
   fixture's round in the active locale, alongside the original four-field English match. The
   method's own comment calls its field list "a contract"; this widens it in the one direction that
   cannot break an existing expectation. `LocalizedSearchTests.EveryInputThatMatchedBeforeMatchesTheSameFixturesInEveryLocale`
   proves the original result set is always a subset of the extended one, across every input shape
   the frozen tests use, in all three locales.
2. **Contrast comments corrected to computed values.** `09_DESIGN-ADDENDUM.md` §4.2's table is
   labelled `[SIMULATED]` and six of its figures were off by more than a rounding step — stale-text
   10.94 vs. computed 12.04, link/focus 9.87 vs. 10.00, action-primary 5.72 vs. 6.56, danger 7.21 vs.
   8.32, success 9.98 vs. 11.05, code 7.44 vs. 8.56, danger-surface 12.63 vs. 12.08. The addendum's
   hex values are used verbatim; the CSS comments carry the values the WCAG 2.2 formula actually
   returns, and `ThemePaletteTests.TheRatiosWrittenIntoTheCommentsAreTheComputedOnes` pins them
   there. §4.3's nav-item figure (11.35) is against the gradient's darker stop; the worst case is the
   lighter stop, `#04173d`, at 10.61 — both pass, and the comment now says "worst-case stop."
3. **`DecidedBy` localized**, a fifth field where `11_I18N.md` §4.2's R5 names four. It is
   user-visible (`ChangeRow` renders "Decided by …"), so leaving it English would fail the
   requirement that every user-visible string render in all three locales.
4. **Node installed locally, user-writable, no sudo** — `~/.local/node/node-v24.19.0-darwin-arm64/`
   — to obtain `tsc`. Nothing system-wide changed.
5. **The compiled interop JavaScript committed** as a tracked artifact, which is what makes the
   §5.3 workflow lift unused — see the item below.
6. **The boundary-4 browser-language default, proposed and then reversed within this run.**
   Flagged as beyond explicit authorization at gate 4: on a first visit with nothing stored, the app
   resolved its language from the browser's declared preferences (mirroring `getSystemTheme`'s
   own OS-preference read) rather than defaulting to English. At the principal's instruction after
   gate 4, this was reversed in commit 19 (`dab3d60`): `getBrowserLocale` was removed from
   `locale.ts` entirely, `LocaleProvider` now applies the fixed English default with nothing stored,
   and `InteropTests.TheLocaleModuleDoesNotReadTheBrowsersDeclaredLanguage` pins the reversal so it
   cannot regress silently. Documented here as the deviation's full lifecycle — proposed, flagged,
   approved with a correction, corrected — rather than only as the state it landed in.

## Decisions resolved autonomously

1. **`ChangeTemplates`, not a `LocaleService` dependency on `MockAccessDataProvider`.** The plan's
   task 1(c) sketched the provider taking locale resources in its constructor. Building it, the
   provider's constructor is a frozen shape several test helpers construct directly
   (`TestData.ProviderOverRealSchedule`); threading a new dependency through it would have forced a
   change to those helpers for no gain the plan's own goal required. `ChangeTemplates` supplies the
   same three-language write-time text as a set of static, pre-authored `LocalizedText` values
   instead — record content in the same standing as the eight seeded changes, authored the same way,
   with no constructor change anywhere.
2. **`FixtureLabels` as a static presentation helper, not a `Fixture` property.** The plan's task
   1(g) left the shape open. `Fixture.DisplayLabel`/`PhaseLabel` had to stay English on the model —
   a frozen test asserts an exact English sentence — so localized rendering could not live on the
   model at all; a static helper taking the locale explicitly was the only shape that kept the
   frozen assertion untouched while still being unit-testable without a component.
3. **`ChangeArrivalTracker` as its own singleton**, rather than a field passed through navigation
   state. Submitting a request navigates to a freshly mounted record page via `NavigationManager`,
   which has no mechanism for passing arbitrary data alongside a navigation; a shared, one-shot
   service was the smallest thing that survives the transition.
4. **The theme/language/sign-out row order in the nav** — three destinations, then language, then
   theme, then (signed in) sign-out. `09_DESIGN-ADDENDUM.md` §5.2 and `11_I18N.md` §5.1 each place
   one control "last" under their own scope, and `10_AUTH-AND-ONBOARDING.md` §4.1 places sign-out
   below the theme row explicitly. Read together the only self-consistent order is the one built;
   `ThemeTriggerPlacementTests` was updated to assert what the three files actually agree on — the
   destinations contiguous at the top, the controls beneath them — rather than the single-file
   "last" position an earlier boundary's test had pinned before the later files existed.
5. **`stale.defaultSubject` as a genuine fallback key**, since `StaleIndicator.SubjectKey`'s
   `[Parameter]` default needed *some* string, and every real caller in the app supplies its own key
   explicitly. Not used by any shipped screen; exists only so the component has a defined default
   rather than an empty lookup.

## Open items carried forward

Restating v11's items unresolved by this run, plus what is new here:

| Item | Status |
|---|---|
| Venue access list ownership | Unresolved upstream, from v9 |
| The interval premise (ID-01) | Untested by declared constraint, from v9 |
| W5's navigation objection | Open, from v9 |
| Withdrawal has no UI affordance | Unchanged — `WithdrawRequestAsync` remains reachable only from the write path's own tests, not from any screen |
| Route rendering unverified in a live browser | Unchanged — this run's verification is bUnit DOM assertions throughout; no browser tooling was available in this session |
| **URL-fragment auto-expand on Help — deliberately not built** | New. `09_DESIGN-ADDENDUM.md` §7.3 names it as a natural extension and explicitly does not authorize it in this run; no section carries an `id`, and a test (`HelpDisclosureTests.NoSectionAutoExpandsFromAUrlFragment`) pins that it stays unbuilt |
| **`docs/project-plan.md` and `docs/grading-criteria.md` still describe the pre-v9 registration flow** | New (carried, not introduced, by this run — same defect class v10 flagged for the root README and how-to-run, which this run fixed). Neither file is a folder README, root README, or how-to-run guide, so fixing them was outside what this run's documentation task authorized; flagged for the next run rather than swept in |
| **The 299/288 link-integrity baseline this run's own plan cited turned out to be unverifiable** | New. See the link-integrity section above. Re-measured cleanly for this run; the earlier figure should not be cited again without independent re-derivation |
| **`gh auth` failed mid-run with an invalid keyring token**, blocking `gh pr create` | New — see final section |

## The blocker on opening the PR

`gh auth status` succeeded at this run's own preflight (confirmed: `jdsaire`, keyring, scopes
`repo`/`workflow`) and `git push` continued working throughout — every commit above reached the
remote branch immediately, as the recovery record's corrective instruction required. When this
report reached the point of opening the pull request, `gh auth status` failed:

```
X Failed to log in to github.com account jdsaire (keyring)
- The token in keyring is invalid.
- To re-authenticate, run: gh auth refresh -h github.com
```

`gh auth refresh` requires an interactive device-code or browser flow this session cannot complete
unattended. The branch is pushed and every commit in the table above is on the remote; only the PR
object itself is not yet created. **This is the one item from the plan's verification checklist not
satisfiable without the principal's action**: either running `gh auth refresh -h github.com`
themselves so this session (or a resumed one) can open the PR, or opening it directly from the
GitHub UI against `deploy/v12-addendum-implementation` → `main`.

## Summary

The four Run 4D addendum files are now built: a black-anchored dark theme; a simulated, honestly
documented sign-in with two demonstrably different holders; the app in English, Spanish and
Portuguese with no reload on switch; a small, strictly-typed TypeScript interop layer whose compiled
output ships and whose sources don't; and two-layer disclosure on both the change log and Help. 409
tests, zero known regressions, every frozen contract — the withholding rule, the search field list,
the read-path latency guarantee, `RequestSubmittingStateTests`'s exact string assertion — verified
unmodified and still passing. The branch is pushed and ready for review; the PR itself needs either
the principal's `gh` re-authentication or a manual open against `main`.
