> **Archived as approved and executed.** This file is the plan exactly as it was approved at each
> stage — the original 17-commit sequence against the six gate documents, followed by three
> addenda approved in the same session as new information arrived (a reported sign-in rejection, a
> request to remove personal names from a generic test account, and a set of surgical UI fixes
> found while investigating that rejection). Nothing here is edited after the fact; each addendum
> is exactly what was proposed and approved before its own commits landed. See
> `Completion-Report-v14.md` for the outcome, the full 28-commit list with SHAs, and every deviation
> and decision this run made.

---

# Run 4F (v14) — ShopEase Realignment: Execution Plan

## Context

Six gate documents (`00_RECONCILIATION.md` … `05_MANIFEST.md`), produced and individually approved
in a prior session, are the complete authorization for this run. They close reversal register
**R6–R11** and specify a four-workstream UX realignment of `jdsaire/fifa-press-app` against the
ShopEase reference: a persistent top bar, a destinations-only sidebar, a new Settings screen,
a reduced Landing and condensed sign-in notice, renamed demo identifiers, and a rebuilt `/matches`
surface with capacity, localized team names, Show-more pagination, an availability filter, and a
conditional `/record` that renders sign-in inline instead of redirecting.

This plan is the output of tasks 0–1 (preflight + plan). Nothing is written to the repository until
it is approved.

---

## 1. Preflight results (task 0)

| Check | Result |
|---|---|
| GitHub access | `~/bin/gh` authenticated as **jdsaire** (keyring), scopes `repo`, `workflow`, `read:org`, `gist`. **PASS** |
| Six gate attachments | All six present and readable at `/Users/jd-mac/Downloads/05-iteration/`. **PASS** |
| Live HEAD on `main` | **`e996d8a8ee72d7b66cfd3f94d39647cd1bb974e3`** — "Merge pull request #10 from jdsaire/deploy/v13-evaluation-remediation". PR #10 merged; v13 remediation present. **PASS** |
| Baseline test count | **421**, derived by static count of every `[Fact]` + `[InlineData]` across all 30 test files (exact match to the figure every gate assumed). The live `dotnet test` run is the first executed action after approval; **if it does not report 421 passing / 0 failing, I stop and report before any commit.** |
| `MockAccessDataProvider.SimulatedNow` | **`new(2026, 7, 3, 20, 31, 0, DateTimeKind.Utc)`** at line 75 — unchanged, exactly as R11 assumed. Matches 1–88 played, **89–104 unplayed**. **PASS** |
| `ux-ui/05-iteration/` | Does not exist. Unclaimed. **PASS** |
| `handoff/v14/` | Does not exist (`v1`–`v13` present). Unclaimed. **PASS** |
| Distinct team spellings in the CSV | **49**, collapsing to **48** after `DR Congo` → `Congo DR`. Matches Gate 4 §4 exactly. **PASS** |
| Toolchain | .NET **10.0.201**; no new dependency required or introduced. **PASS** |

Local clone: `/Users/jd-mac/Downloads/Fifa-Press-App/fifa-press-app`, currently on the stale
`deploy/v10-frontend-course-correction`. First action is `git fetch origin`, then branch from
`origin/main` — no existing local branch is touched or deleted.

---

## 2. Citation drift and findings (reported, not silently reconciled)

Every file each gate names exists at the path it assumed. Line citations verified where given:
`Registration.razor` 41 / 107, `EventDetails.razor` 73, `MyAccess.razor` 51, `SimulatedNow` line 75
— **all exact**. The findings below are consequences the gates did not enumerate; none blocks the
run, each is carried into the Completion Report.

**D1 — `.top-row` is an occupied class name.** `NavMenu.razor` already renders
`<div class="top-row ps-3 navbar navbar-dark">` (the mobile header), styled in the *scoped*
`NavMenu.razor.css`. Gate 1 puts a new `top-row` in `MainLayout` styled from *global* `app.css`,
where the rule would also hit the sidebar's navbar. Resolution: keep Gate 1's class on the element,
scope the global rules as `main > .top-row` / `.session-bar*` so nothing leaks into the sidebar.

**D2 — `ThemeTriggerPlacementTests.MainHoldsOnlyTheContentColumnAgain` breaks at task 3**, not task 8.
It regex-asserts that `<main>` contains exactly one element. Inserting the top row falsifies it.
Its assertion lands in task 3's commit; the rest of the file is rewritten at task 8 as Gate 2 specifies.

**D3 — `LanguageSwitchTests` and `ThemeTriggerPlacementTests` break at task 4**, two commits before
their specified rewrite. Both render `NavMenu` for their placement and mechanism tests
(`TheSwitchOffersThreeFixedOptions…`, `TheNavItselfRerendersIntoTheNewLanguage`,
`TheTriggerRendersAsARowInsideTheNavList`, `TheRowIsStillRendered_Disabled…`, and others).
Resolution: task 4 retargets the *behavioral* assertions to render `LanguageSwitch` / `ThemeTrigger`
directly (both components still exist at that point) and retires only the nav-placement assertions
Gate 2 §7 already authorizes retiring; tasks 7 and 8 then restate them against `Settings.razor`
hosting `LanguageSelect` / `AppearanceControl`, exactly as Gate 2 specifies. Nothing is deleted
whose behavior isn't restated.

**D4 — `LandingTests.TheNavsFirstRowPointsAtTheRecordWithoutAMatchOverride` breaks at task 4.**
Gate 5 credits `LandingTests` with two rewritten tests; this is a third. After the restructure the
first row is Home (`/`) and My Requests is conditional. Rewritten in task 4's commit to assert the
new row order and to keep its `NavLinkMatch.All` guarantee.

**D5 — `IconTests.MatchListMetaLineKeepsItsPhaseTextBesideThePhaseIcon` renders `EventList`**, not
`EventCard`. Gate 4 §3 verified only the two `EventCard`-rendering cases. When `EventList` starts
injecting `SimulatedSessionProvider` (task 16) this test's context fails DI. Fixed mechanically in
task 16's commit by registering the provider; assertions unchanged.

**D6 — two further `GatingTests` beyond the three Gate 4 §11 names.**
`TheMatchListIsPublic` asserts `DoesNotContain("Sign in", markup)` on `EventList` — falsified by the
signed-out "Sign in to request access" CTA (task 16). `TheMatchDetailStatesTheRequestAffordanceRatherThanHidingIt`
asserts `a[href='signin']` on `EventDetails` — falsified by the route retarget (task 20). Both are
rewritten in the commit that breaks them.

**D7 — `SignOutTests` splits across two commits.** Task 3 adds the `SessionBar` restatements while
`nav-session` still exists and still passes; task 4 removes the now-false row-count / row-index /
`.nav-session` assertions when it removes the block itself. `TheSignOutRowIsShapedLikeTheRowsAboveIt`
(a CSS-shape test for a nav row that ceases to exist) retires at task 4; its subject is gone, not
relocated. `TheNavNoLongerClaimsThatSignInGatesNothing` reads `NavMenu.razor`'s source for the phrase
"gates the record now" — the rewritten comment keeps that clause, so the test survives unmodified.

**D8 — `LocaleServiceTests.NoTranslationIsJustTheEnglishStringCopiedAcross` will fail on the
`team.*` keys.** Country names identical in English, Spanish and Portuguese —
**Argentina, Portugal, Senegal** (exact set confirmed when the 48 values are written) — trip a test
whose `allowedToMatch` list is hard-coded. The test's own comment sanctions exactly this case
(`phase.final`: "the same word in all three languages, which is a fact about the languages rather
than a missed translation"). Resolution: extend `allowedToMatch` with those specific `team.*` keys
and a comment, inside task 17's commit. Gate 5 lists this file as unaffected; that is the deviation.

**D9 — `Fixture.cs` gains one property.** Gate 4 §1 puts capacity on "the provider's fixture read
path, not a stored CSV field". Implementation: `SlotsRemaining` as an `int?` init-only member on
`Fixture`, set **only** by `MockAccessDataProvider.Reveal` from a private `[SIMULATED]`-commented
rule — precisely the pattern `IsResolved` already uses ("Set by the provider, since the instant is
the provider's to own"). `FixtureImporter` stays a pure CSV reader. `Fixture.cs` is not in Gate 5's
manifest; recorded as an addition.

**D10 — identifier-sweep counts.** 15 test files contain the old literals. Twelve need
identifier-argument changes (Gate 3 §6's table); three — `TwoRecordsTests`, `LocalizedChangeTests`,
`ChangeArrivalAnnouncementTests` — are `credentialId:`-only and are not touched. Two files carry
slightly more occurrences than the gate's tilde-estimates: `DemoSessionTests` **15** (gate: ~13),
`LanguageSwitchTests` **8** (gate: 6 + 1). Each occurrence is classified by call site at
implementation; `CredentialId` literals stay.

**D11 — commit count.** The XML's success criterion 2 says 18 commits; the actual sequence is
**17** (Gate 5's 15 feature commits + 1 dossier injection + 1 archival). 17 is what lands.

---

## 3. Branch, identity, push policy

- Branch: **`deploy/v14-shopease-realignment`**, cut from `origin/main` @ `e996d8a`. Never auto-named.
- PR: opened against `main` right after the first push, title **"Run 4F (v14): ShopEase Realignment"**,
  left **open and unmerged** for the entire run.
- Every commit pushed the moment it lands — no batching.
- Author and committer: **`Juan Diego S. <88201583+jdsaire@users.noreply.github.com>`** (this clone's
  gitconfig, identical to all 13 prior runs, GitHub account `jdsaire`) — *your decision this session,
  overriding the XML's literal-string reading*. No co-author trailer, no generated-with line, zero
  AI/agent/vendor attribution anywhere.
- No subagents. `gh` CLI only; no PAT ever requested, printed, or referenced.

---

## 4. Decisions taken this session (carried into the Completion Report)

1. **Sign-in defect verification** — at the workstream-4 gate I run the app locally and hand you the
   URL plus both credential pairs exactly as displayed; **you** attempt both sign-ins and report
   reproduce / no-reproduce, which I record verbatim. No speculative fix under any outcome.
   (This machine has no Node, no Playwright, and only Safari — a browser-driven check is not
   available to me.)
2. **Played fixtures keep `View details`** and lose only the request CTA — task 20's gloss over
   Gate 4 §2's table cell, preserving `/matches` as the only route into the 88 played detail pages.
3. **`DemoAccount.DescriptionKey` is removed**, along with both constructions — its markup consumer
   and its test assertion are retired by the same gate that retires the keys it names.
4. Git identity as above.

---

## 5. Dossier injection (task 2)

| Source attachment | Lands at |
|---|---|
| `00_RECONCILIATION.md` | `ux-ui/05-iteration/00_RECONCILIATION.md` |
| `01_NAVIGATION.md` | `ux-ui/05-iteration/01_NAVIGATION.md` |
| `02_SETTINGS.md` | `ux-ui/05-iteration/02_SETTINGS.md` |
| `03_HOME-AND-SIGNIN.md` | `ux-ui/05-iteration/03_HOME-AND-SIGNIN.md` |
| `04_MATCHES.md` | `ux-ui/05-iteration/04_MATCHES.md` |
| `05_MANIFEST.md` | `ux-ui/05-iteration/05_MANIFEST.md` |

Byte-identity confirmed by `diff` against the attachments, not by eye. Plus a new
`ux-ui/05-iteration/README.md` in the register style of the four existing dossier READMEs, and a new
`05-iteration/` bullet in `ux-ui/README.md` matching the existing one-paragraph bullets.

---

## 6. The 17-commit sequence

**Workstream 1 — Navigation (Gate 1, closes R9)**

| # | Commit | Touches |
|---|---|---|
| 1 | `docs(ux): add run 05 iteration dossier` | 6 gate files, `05-iteration/README.md`, `ux-ui/README.md` → **push, open PR** |
| 2 | `feat(nav): add a persistent top bar carrying auth status` | new `Layout/SessionBar.razor`; `MainLayout.razor` (+ top row, false comment rewritten); `app.css` (`main > .top-row`, `.session-bar*`); `SignOutTests` (SessionBar restatements); `ThemeTriggerPlacementTests.MainHoldsOnlyTheContentColumnAgain` (D2) |
| 3 | `feat(nav): reduce the sidebar to destinations only` | `NavMenu.razor` + `.razor.css` (drop `nav-session`, `nav-signout`, `LanguageSwitch`, `ThemeTrigger`; add Home / My Requests (conditional) / Settings rows + icons); `Icon.razor` (5 glyphs + doc comment); `nav.home`/`nav.settings` ×3 locales; `IconTests` (+5 `[InlineData]`); `SignOutTests`, `LanguageSwitchTests`, `ThemeTriggerPlacementTests`, `LandingTests` (D3, D4, D7) |

→ **GATE 1 STOP** (task 5): build + full suite, expected still 421, summarize both commits, wait.

**Workstream 2 — Settings (Gate 2, closes R7)**

| # | Commit | Touches |
|---|---|---|
| 4 | `feat(settings): add the Settings screen` | new `Pages/Settings.razor` at `/settings`, public; 7 `settings.*` keys ×3; new `SettingsScreenTests.cs`; `app.css` `.settings-field*` |
| 5 | `feat(settings): language selection as a dropdown` | new `Components/LanguageSelect.razor`; delete `LanguageSwitch.razor` + `.razor.css`; `LanguageSwitchTests` restated against `Settings` |
| 6 | `feat(settings): tri-state appearance control` | new `Components/AppearanceControl.razor` (System → existing unused `clearStoredTheme()`; **`theme.js` untouched**); delete `ThemeTrigger.razor` + `.razor.css`; `ThemeTriggerPlacementTests` restated against `Settings`; `ThemePaletteTests` untouched |

→ **GATE 2 STOP** (task 9).

**Workstream 3 — Home & Sign-in (Gate 3, closes R6/R8, opens R10)**

| # | Commit | Touches |
|---|---|---|
| 7 | `feat(home): reduce Landing to heading, lede, and one CTA` | `Landing.razor` (keep `justSignedOut`, signed-in redirect, disclosure); retire 13 `landing.*`, add `landing.cta` ×3; `LandingTests` two tests rewritten |
| 8 | `feat(signin): condense the simulation notice` | `SignIn.razor` notice → `signIn.noticeStrong`/`noticeBody`, verbatim copy from Gate 3 §1; retire 10 `signIn.notice*` ×3; confirm the two notice tests pass **unmodified** |
| 9 | `feat(signin): rename demo identifiers, reshape the accounts list` | `DemoAccountStore.cs` (`demo_staff1`/`demo_staff2`; `DescriptionKey` removed); accounts container → ShopEase shape; retire 3 keys, add `accountsIntro`, update 3 values; **12-file identifier sweep**; `SignInScreenTests` assertion rewritten |
| 10 | `refactor(signin): extract SignInForm from the /signin route` | `Pages/SignIn.razor` → route-less `Components/SignInForm.razor` (+ `.razor.css`); retarget `LanguageSwitchTests.TheSignInScreenRendersInEveryLocale` and `SignInScreenTests` to the component |

→ **GATE 3 STOP** (task 14), noting explicitly that `/signin` is retired only in workstream 4.

**Workstream 4 — Matches & Record (Gate 4, closes R10, opens/closes R11)**

| # | Commit | Touches |
|---|---|---|
| 11 | `feat(matches): add capacity to unplayed knockout fixtures` | `MockAccessDataProvider` `[SIMULATED]` rule (0 when `n % 4 == 0`, else `3 + (n % 9)`, unplayed only) + `Fixture.SlotsRemaining` (D9); `matches.slotsRemaining`/`soldOut` ×3; new `SlotAvailabilityTests.cs` (sold out exactly at 92/96/100/104; 3–11 elsewhere; absent on all 88 played; never beside a team name) |
| 12 | `feat(matches): retire EventCard from /matches in favor of MatchCard` | new `Components/MatchCard.razor` (plain `CanRequest`/`SlotsRemaining`/`IsPlayed`, no session injection); `EventList.razor` three-way gating (played → View details, no request CTA); **`EventCard.razor` kept**, unreferenced, per §3 — said so in the commit message; new `MatchCardGatingTests.cs`; `IconTests` DI fix (D5); `GatingTests.TheMatchListIsPublic` (D6) |
| 13 | `feat(matches): localize team names in the presentation layer` | `FixtureLabels.Display` locale lookup inside the resolved branch only; 48 `team.*` ×3; `FixtureImporter` `DR Congo` → `Congo DR`; new `TeamLocalizationTests.cs`; +1 `FixtureImporterTests` case; `LocaleServiceTests.allowedToMatch` extension (D8); frozen `LocalizedSearchTests` must pass untouched — if it breaks I stop |
| 14 | `feat(matches): switch to Show More pagination` | numbered pages retired; `Showing {shown} of {total}` (`aria-live="polite"`) + `Show more`, page size 12, `_visibleCount` resets on any filter/search change; retire `matches.pagesLabel`, add 2 keys ×3; new `ShowMoreTests.cs` |
| 15 | `feat(matches): add the availability filter` | `SlotAvailabilityFilter` enum + `WithSlots` in `FixtureQuery`; both `Apply` overloads gain a **defaulted** fifth parameter (the three FixtureQuery test files must compile and pass unmodified); control on `EventList`, AND-composed; 4 keys ×3 incl. `controlWithSlots` for the `emptyNarrowed` enumeration |
| 16 | `feat(record): render sign-in inline instead of redirecting` | `MyAccess`: redirect + `.my-access__signed-out` branch deleted, `<SignInForm />` inline; retire 3 `record.signedOut*`; `/signin` retired; retarget `Registration.razor` ×2 and `EventDetails.razor` ×1 to `record`; pending-request status line on `/matches` (`EventList` computes a bool, `MatchCard` gains no session awareness) + `matches.requestPending` ×3; value updates `nav.record`/`record.title` → "My Requests"; `GatingTests` ×4 rewritten (D6) |

→ **GATE 4 STOP** (task 21): expected delta +5 test files + 1 `FixtureImporterTests` case against 421; actual delta reported either way. **The live sign-in verification happens here, with you.**

**Verification + archive**

| # | Commit | Touches |
|---|---|---|
| 17 | `docs: archive v14 plan and completion report` | `handoff/v14/` (renamed plan + Completion Report in v13's section set), `handoff/v14/README.md`, `handoff/README.md` bullet |

---

## 7. Per-commit discipline

- `dotnet build src/FifaPressApp -c Release` → **zero errors, zero warnings**, and
  `dotnet test tests/FifaPressApp.Tests -c Release` → **zero failures**, immediately after every
  commit. A commit that would break either gets fixed inside itself before moving on.
- A feature and the tests it breaks land in the **same** commit. Never a feature split from its tests,
  never two unrelated items batched.
- `git diff main` stays empty at every commit boundary for `ux-ui/00-initial-evaluation/`,
  `01-design-research/`, `02-ideation/`, `03-ui-prototyping/`, and `TwoRecordsTests.cs`,
  `LocalizedChangeTests.cs`, `LocalizedSearchTests.cs`.
- No new package reference in either project.

---

## 8. Final verification (task 22)

| Check | Method |
|---|---|
| Build + full suite | `dotnet build -c Release`, `dotnet test -c Release` — zero warnings, zero failures |
| Frozen paths | `git diff main --stat` against each of the 4 `ux-ui/` folders and the 3 test files → empty |
| i18n key parity | `LocaleServiceTests.TheThreeFilesDefineExactlyTheSameKeys` (263 → **305** keys: −30, +72) |
| No empty / no English-copied translations | `NoTranslationIsLeftEmpty`, `NoTranslationIsJustTheEnglishStringCopiedAcross` |
| Internal markdown links | Count before/after across the repo, report N/N |
| Authorship | `git log` shows one identity on every commit; grep the whole branch diff + all commit messages for AI/agent/vendor terms → expect zero hits |

PASS/FAIL reported per check. The es/pt values for all 77 new and value-changed entries stay
**`[ASSUMPTION]`**-tagged pending native review — stated plainly in the Completion Report, never as
reviewed.

---

## 9. Stop conditions honored

I stop and report, without proceeding, if: the live test count is not 421/0 at start; any of the four
workstream gates is not explicitly and unambiguously approved for that specific gate; a commit would
break build or tests irrecoverably within itself; a frozen path would be touched; `LocalizedSearchTests`
breaks at task 13; or the sign-in investigation would require code before you confirm reproduction.

---
---

# Addendum — Post-Gate-4 fix: generic sign-in identity, no personal names

**Status at this point:** all four workstreams above landed (16 commits on `deploy/v14-shopease-realignment`,
PR #11). Task 21's Gate-4 stop asked you to test sign-in live; you reported the credentials were
**rejected**, and separately that you want the two published demo names never displayed anywhere. This
addendum is a new, explicitly out-of-band change on top of the six gate documents — nothing in
`ux-ui/05-iteration/` asked for this, and you are directing it directly. It lands as additional commits
on the same open PR. Final verification and archival (deploy tasks 22–23) stay paused until this is
in and confirmed.

## Context

Two separate things are being fixed:

1. **A reported sign-in rejection** at `/record` using `demo_staff1` / `amina-demo-2026` — the exact
   pair the screen displays. I cannot independently reproduce this: all 493 passing tests exercise
   `DemoAccountStore.Match` and `SimulatedSessionProvider.SignInAsync` with these exact literal values
   against the real production code, and the identifier's allow-list regex and the password's
   byte-for-byte comparison both accept them cleanly. There is no service worker or PWA cache in this
   app (confirmed — no `service-worker.js` in `wwwroot/`) that could be serving a stale WASM bundle
   against fresh-looking markup. **I am not claiming to have found and fixed this rejection** — per this
   run's own discipline, no speculative fix. What I *am* doing is replacing the credentials entirely per
   your explicit instruction below, which makes the specific pair that was rejected moot; if the new pair
   is rejected too after a hard refresh / fresh tab, that will isolate the cause to something environment-
   or session-side rather than to `DemoAccountStore` itself, and I'll investigate from there.

2. **Personal names must never render**, per your decision: keep both backend demo records (needed by
   the frozen `TwoRecordsTests.cs`, which reads `HolderName` directly from the provider and must stay
   untouched — `git diff main` empty for it at every commit boundary, same as every other workstream),
   but stop displaying `HolderName` anywhere in the UI. Two sign-in identities stay reachable — your
   answer confirmed the two-record comparison (the app's throughline since v9: two holders, one
   conditional change, two different resolutions) stays live — but nothing about either identity may look
   personalized.

## Decisions confirmed with you this turn

1. **Two generic accounts, not one.** `demo_staff1` and `demo_staff2` (identifiers unchanged from the
   current build) stay as two separate sign-in identities, so "sign in as one, then the other" keeps
   working. New passwords, following the pattern you gave: `Demo#2026Staff1` / `Demo#2026Staff2`.
2. **Backend data stays frozen-compliant.** `Accreditation.HolderName` and `DemoAccountStore`'s
   `HolderName` fields keep their literal values (`"Amina Bello"`, `"Tomás L."`) — `TwoRecordsTests.cs`
   is not touched and keeps passing unmodified. Every UI surface that currently renders `HolderName`
   is changed to render a generic label instead.
3. **The sign-in rejection you hit could not be reproduced from the code** — see Context §1. Proceeding
   with the credential replacement regardless, since you're changing the values anyway; will investigate
   further if the *new* pair also fails after a clean reload.
4. **Generic label: "Demo Staff"**, localized (`record.genericHolderName`, new key, all three locales),
   used everywhere a holder name currently shows.

## What changes

**`src/FifaPressApp/Services/DemoAccountStore.cs`** — passwords only:
- `Amina.Password`: `"amina-demo-2026"` → `"Demo#2026Staff1"`
- `Tomas.Password`: `"tomas-demo-2026"` → `"Demo#2026Staff2"`
- `Identifier` (`demo_staff1`/`demo_staff2`), `CredentialId`, and `HolderName` **unchanged**.

**Four UI render sites**, each swapping `@holder.HolderName` / `@record.HolderName` /
`@account.HolderName` for `@L[Locale, "record.genericHolderName"]`, or removing the name entirely where
showing it twice (once per account) would be confusing rather than informative:
- `Layout/SessionBar.razor:35` — signed-in indicator. Generic label; `CredentialId` still shown beside
  it, so the bar keeps telling you *which* record you're looking at without naming who it is.
- `Pages/Settings.razor:52` — the read-only "Name" field. Generic label.
- `Pages/MyAccess.razor:192` — the record's own identity section (`record.name` row). Generic label.
- `Components/SignInForm.razor:90` — the published accounts list. **Drops the name entirely** rather
  than showing "Demo Staff" twice side by side, which would be actively confusing on a list whose whole
  job is telling two rows apart. `account.Identifier` (`demo_staff1`/`demo_staff2`) already does that
  job — it's what the field *is*. Markup becomes `<code>@account.Identifier</code> / <code>@account.Password</code>`
  with no `<strong>` prefix.

**i18n** — one new key, all three locales, no key retired:

| Key | en | es | pt |
|---|---|---|---|
| `record.genericHolderName` | Demo Staff | Personal de prueba | Equipa de teste |

**The 39-occurrence password sweep** — mechanical, same discipline as the earlier identifier sweep:
every occurrence of `"amina-demo-2026"` / `"tomas-demo-2026"` across 13 test files (`DemoSessionTests`
13, `LanguageSwitchTests` 7, `DisclosureTests` 3, `LocalizedDateTests` 3, `SettingsScreenTests` 2,
`GatingTests` 2, `MatchAccessStatusTests` 2, `SignOutTests` 2, `MatchAccessLineTests` 1,
`SignInScreenTests` 1, `LandingTests` 1, `GateCheckStatusTests` 1, `WithdrawalAffordanceTests` 1) is a
literal password argument to `SignInAsync`/`Match` — unlike the identifier sweep, passwords have no
second, do-not-touch use anywhere (never a stored-data key), so every occurrence gets swapped.

**Test rewrites — every markup assertion for `"Amina Bello"`/`"Tomás L."`** (backend-level assertions
that read `.HolderName` off a `DemoAccount`/`Accreditation` object directly, like
`DemoSessionTests.cs:112`, are untouched — they test data that hasn't changed):
- `GatingTests.cs` (`TheRecordRendersTheSignedInHoldersOwnState`,
  `SigningInAsTheOtherHolderShowsTheOtherRecord`) — assert the credential ID present, **both** names
  absent (never either, not just "not the other one").
- `LanguageSwitchTests.cs` (`TheSessionSURVIVESALanguageSwitch`) — swap the two
  `Assert.Contains("Amina Bello", bar.Markup)` for a locale-invariant check (`"MP-2026-04817"` in the
  bar), since a name assertion would be checking something no longer there rather than the guarantee
  the test is for (session survives a switch).
- `SettingsScreenTests.cs` (`SignedInTheScreenAddsTheHoldersNameAndAWayOut`,
  `TheOtherHoldersNameAppearsWhenTheOtherHolderIsSignedIn`) — assert `.settings-field__value` reads
  "Demo Staff" for **both** signed-in identities; the second test's name changes accordingly (it no
  longer demonstrates a *different* name — it demonstrates the *same* generic label under a different
  credential, which is now the point).
- `SignOutTests.cs` (`TheSessionBarNamesTheHolderAndTheirCredential`,
  `TheSessionBarFollowsTheSessionWithoutARenderLag`) — same swap: generic label + credential ID.
- `SignInScreenTests.cs` (`BothAccountsArePublishedWithHolderIdentifierAndPassword`) — drops the
  `Assert.Contains(account.HolderName, markup)` line (nothing to assert — the name isn't rendered) and
  gains a new explicit guarantee: `Assert.DoesNotContain("Amina Bello", markup)` /
  `Assert.DoesNotContain("Tomás L.", markup)`, which is the actual behavior now under test.

**New test, once the above lands:** `TheGenericLabelReplacesTheHolderNameEverywhereItUsedToShow` (or
similar, exact name decided at implementation) — one assertion sweeping `SessionBar`, `Settings`, and
`MyAccess` in both signed-in states, confirming neither persona name appears in any of the three, so
this guarantee has a single owning test rather than living only as a side effect of other tests.

## Frozen-path discipline, unchanged

`git diff main` stays empty for `TwoRecordsTests.cs` and the other two frozen test files, and for all
four frozen `ux-ui/` folders, at every commit boundary — exactly as every workstream above. Confirmed
after each commit the same way.

## Commits

Two, following this run's one-item-per-commit discipline (a feature and the tests it breaks land
together):

1. `fix(signin): replace demo passwords with a generic pair` — `DemoAccountStore.cs` password values,
   the 39-occurrence test sweep, and `DemoSessionTests`' `[InlineData]` rows that carry the old
   passwords as literal theory data.
2. `feat(record): stop rendering the demo holders' names` — the four UI sites, the new
   `record.genericHolderName` key in all three locales, and every test rewrite listed above.

Build and the full suite stay green after each. Both push immediately, same branch, same open PR.

## Verification

- `dotnet build src/FifaPressApp -c Release` → 0 warnings, 0 errors after each commit.
- `dotnet test tests/FifaPressApp.Tests -c Release` → 0 failures after each commit; report the new total
  (493 + 1 new test − 0 retired ≈ 494).
- `grep -rn "Amina Bello\|Tomás L\." src/FifaPressApp` → the only hits left should be inside
  `DemoAccountStore.cs`'s and `MockAccessDataProvider.cs`'s data/doc-comments, never in a `.razor` render
  path.
- Rebuild and restart the local server (`dotnet run --project src/FifaPressApp -c Release --urls
  http://localhost:5199`), and ask you to retry sign-in at `/record` with `demo_staff1` /
  `Demo#2026Staff1` in a **fresh tab** (ruling out any stale-session artifact from the earlier attempt) —
  report reproduce/no-reproduce on the new pair before resuming toward final verification and archival.

---
---

# Addendum 2 — Sign-in crash, ShopEase layout parity, and five UI fixes

**Status:** 18 commits on `deploy/v14-shopease-realignment` (PR #11). Final verification and archival
(deploy tasks 22–23) stay **halted** at your instruction until this lands. Seven surgical commits on
the same open PR.

## Context

You reported an unhandled error the moment you type in the identifier field, and that you still
cannot get in. You supplied the console trace, which identifies the cause exactly — it is a real
defect, not an environment artifact, and it has been in the code since v9. Two further defects were
found while tracing it. Items 2–6 are your directed UI changes on top.

## The root cause — confirmed, not guessed

```
System.ArgumentException: Arg_ObjObjEx, Microsoft.AspNetCore.Components.ChangeEventArgs, System.String
   at System.Delegate.DynamicInvoke(Object[] args)
   at Microsoft.AspNetCore.Components.EventCallbackWorkItem.InvokeAsync[Object](...)
```

`SignInForm.razor:114` carries `@bind-Value:event="oninput"` on `<InputText>`. **`InputText` is a
component, not an element.** `@bind-Value:event` only names a DOM event when applied to an element;
on a component it must name an `EventCallback` *parameter*, and `InputText` has none called
`oninput`. So `oninput` falls through `AdditionalAttributes` and is splatted onto the underlying
`<input>` as a DOM handler — bound to the delegate Blazor generated for the setter, which is typed
`string`. The browser fires it with `ChangeEventArgs`, the invoke fails the type check, and the
renderer tears down. Every subsequent interaction re-throws, which is why reloading doesn't help.

**It is the only occurrence in the app** (`grep -rn "bind-Value:event\|bind:event" src/` → one hit).
ShopEase's equivalent is `<InputText id="login-username" @bind-Value="_model.Username" />` with no
`:event` override — which is precisely why sign-in works there and not here. The fix is ShopEase
parity: delete the override.

**Why no test caught it:** no test in the suite ever fires an input event on `#signin-identifier`,
and no test ever completes a sign-in *through the form* — every "signed in" test calls
`Session.SignInAsync(...)` directly, bypassing the UI entirely. The whole type → submit → record path
is untested. That gap is closed in commits 1 and 2.

## Two further defects found while tracing it

**A. The record never loads after signing in on `/record`.** `MyAccess.OnInitializedAsync` runs once
and returns early when signed out. It does **not** subscribe to `Session.OnChanged`. After a
successful inline sign-in, `SignInForm` calls `NavigateTo("record")` — the same route — so Blazor
reuses the component instance and `OnInitializedAsync` never re-runs. `record` stays `null`, and the
page falls through to the empty state: *"No accreditation record yet."* This is a regression from
commit `cc0d2dc` (R10, redirect → inline render) — removing the redirect removed the only thing that
used to re-initialize the page.

**B. `.signin__form { max-width: 28rem }` has never applied.** It sits in scoped CSS
(`SignInForm.razor.css`) but targets `<EditForm>`, a **component**. Blazor only stamps scope
attributes onto elements written in the `.razor` file, never onto a child component's rendered root,
so the compiled `.signin__form[b-xxxxx]` matches nothing. Bootstrap's `.form-control` is
`width: 100%`, so the inputs fill the full content column — exactly the "extend almost the whole page
with no justification" you describe. Fixed by moving the constraint onto a container element the
component actually owns, per ShopEase's `.auth-page`.

## ⚠️ One flagged reversal

Moving the simulation notice to the bottom (your item 2) **overturns a documented decision**. Gate 3
§1 and the file's own header state it is *"Visible before any interaction, and first on the page,"*
sourced from governing document 10 §2.3, and **R6 closed on exactly that** — condensed, never
removed, order preserved. Your instruction is explicit and it is your call; I am implementing it and
recording it in the Completion Report as an authorized deviation against R6, with this note. All
five asserted substrings and `role="note"` survive — only position changes.

## The seven commits

| # | Commit | What |
|---|---|---|
| 1 | `fix(signin): bind the identifier field without the invalid event override` | Delete `@bind-Value:event="oninput"`. New tests: typing into the identifier field raises no exception, and a wrong credential shows the generic failure. |
| 2 | `fix(record): load the record when a session begins on the page` | `MyAccess` gains `@implements IDisposable` + `Session.OnChanged` subscription; the load body extracts to `LoadRecordAsync()`, called from `OnInitializedAsync` **and** on session change. New test: the full end-to-end path — type both fields, submit, assert the record renders (not the empty state). |
| 3 | `feat(signin): reorder the sign-in screen and constrain its width` | Wrap content in `<section class="signin">` (a real element, so scoped CSS binds). Order → **h1 → form → demo accounts → notice**. `.signin { max-width: 32rem; margin: 0 auto; }` per ShopEase `.auth-page`; fields per `.auth-form__field` (flex column, `gap: .3rem`); accounts block gets ShopEase's card treatment using **FIFA tokens**, not ShopEase's hardcoded `#f7f9fb`, so both themes hold. Retires the dead `.signin__form` rule. Rewrites `TheNoticeIsBeforeTheFormInTheMarkup`; renames `TheNoticeComesFirstAndStatesAllFourThings` (content-only, still passes). |
| 4 | `feat(nav): make the sign-in affordance a button` | `SessionBar`'s sign-in link → `class="btn btn-outline-primary btn-sm"`, matching MatchCard's "View details" exactly. Drops the underline rule, adds margin. Keeps `.session-bar__signin` so its existing test holds. |
| 5 | `feat(nav): let the sidebar collapse at desktop widths` | `MainLayout` holds the state and renders a `«`/`»` toggle at the head of the top row (always reachable in both states, ≥641px only — the mobile hamburger already covers narrow). Sidebar hides completely; content takes full width. `aria-expanded` + `aria-controls="nav-menu"`, new `nav.hideMenu`/`nav.showMenu` keys ×3 locales. **Updates `MainHoldsTheSessionRowAndTheContentColumnAndNothingElse`**, which asserts `<main>`'s exact element list. |
| 6 | `feat(nav): enlarge the sidebar labels and their icons` | `.nav-item` `0.875rem → 1rem` (parity with body text); `.nav-item ::deep svg.icon` `16px → 20px` via CSS (overrides the SVG's own width/height attributes — no change to `Icon.razor`'s contract). |
| 7 | `feat(settings): add icons to the appearance options` | Four new `Icon.razor` glyphs — `system` (laptop), `phone`, `sun`, `moon` — in the existing decorative/`currentColor` form. `AppearanceControl` renders one per option; System shows the laptop ≥641px and the phone below, swapped by media query (both rendered, one hidden). Extends `IconTests`' `[InlineData]` by four. |

## Discipline, unchanged

Build 0 warnings / 0 errors and the full suite green after **every** commit; each pushes immediately
to the same open PR. `git diff main` stays empty for the four frozen `ux-ui/` folders and
`TwoRecordsTests.cs` / `LocalizedChangeTests.cs` / `LocalizedSearchTests.cs` at every boundary. No new
dependency. Locale key parity held at every commit that touches i18n.

## Verification

- After commit 1: a test that fires `Input()` on `#signin-identifier` — this is the regression guard;
  it fails against the current code and passes after.
- After commit 2: the end-to-end test signs in through the form and asserts the credential number
  renders — proving the record actually loads.
- `grep -rn "bind-Value:event\|bind:event" src/` → zero hits.
- Rebuild, restart `http://localhost:5199`, and hand back to you to confirm in a fresh tab: typing
  raises no error, `demo_staff1` / `Demo#2026Staff1` reaches the record, the screen order and widths
  match ShopEase, the sidebar collapses, labels are larger, and the appearance options carry icons.
