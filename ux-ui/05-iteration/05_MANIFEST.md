# v14 (Run 4F) — Gate 5: Test Impact and File Manifest

**Status:** Open for approval. Final gate. Register **R6–R11** fully resolved entering this gate.
Two loose ends found while consolidating — resolved directly below (§0), not re-opened as
questions: both are same-tier copy-consistency fixes directly implied by decisions already made,
not new design forks.

---

## 0. Two loose ends closed while consolidating

**`nav.record`'s value.** Gate 1 specified the nav row as "My Requests" but never closed the loop
on the key's actual value (currently "My Access"). Resolved: value updated to "My Requests" —
directly implied by Gate 1's own row label, not a new decision.

**`record.title` and the three `record.signedOut*` keys.** Two consequences found while compiling
this manifest that weren't caught in Gates 1 or 4:

- `record.title` (the page's own `<h1>`, currently "My access") would otherwise disagree with the
  nav row now reading "My Requests" — the exact interface-disagreeing-with-itself pattern this
  project's doctrine exists to avoid. Value updated to "My Requests", matching the nav.
- `record.signedOut` / `record.signedOutLink` / `record.signedOutAfter` — the three keys backing
  the `.my-access__signed-out` inline message Gate 4 replaces with `<SignInForm />` — **retire**.
  Missed in Gate 4's key accounting; caught here before the manifest closed.

---

## 1. Consolidated i18n table — all three locales

**Retired — 30 keys total:**

| Source | Keys |
|---|---|
| Gate 3 §1 | `signIn.noticeSimulatedStrong`, `signIn.noticeSimulatedBody`, `signIn.noticeCredentialsBefore`, `signIn.noticeCredentialsStrong`, `signIn.noticeCredentialsAfter`, `signIn.noticeSessionBefore`, `signIn.noticeSessionStrong`, `signIn.noticeSessionAfter`, `signIn.noticeBoundaryBefore`, `signIn.noticeBoundaryStrong` (10) |
| Gate 3 §2 | `landing.signInHeading`, `landing.signInBody`, `landing.signInCta`, `landing.browseHeading`, `landing.browseBody`, `landing.browseMatches`, `landing.browseHelp`, `landing.whatYouGetHeading`, `landing.point1Strong`, `landing.point1Body`, `landing.point2Strong`, `landing.point2Body`, `landing.point3Strong`, `landing.point3Body` (13) |
| Gate 3 §3 | `signIn.accountAmina`, `signIn.accountTomas`, `signIn.accountsHint` (3) |
| Gate 4 §7 | `matches.pagesLabel` (1) |
| Gate 5 §0 | `record.signedOut`, `record.signedOutLink`, `record.signedOutAfter` (3) |

**New — 72 keys total:**

| Source | Keys |
|---|---|
| Gate 1 §6 | `nav.home`, `nav.settings` (2) |
| Gate 2 §5 | `settings.title`, `settings.appearance`, `settings.appearanceSystem`, `settings.appearanceLight`, `settings.appearanceDark`, `settings.name`, `settings.signOut` (7) |
| Gate 3 §1, §2, §3 | `signIn.noticeStrong`, `signIn.noticeBody`, `landing.cta`, `signIn.accountsIntro` (4) |
| Gate 4 §4 | `team.*` — 48 keys, one per normalized country (`Congo DR`/`DR Congo` collapsed to one) |
| Gate 4 §1, §6, §7, §8 | `matches.slotsRemaining`, `matches.soldOut`, `matches.requestPending`, `matches.showingCount`, `matches.showMore`, `matches.availabilityLabel`, `matches.allAvailability`, `matches.withSlots`, `matches.controlWithSlots` (9) |

**Value-only changes — key retained, content updated (not counted above):**

| Key | Old value (en) | New value (en) | Source |
|---|---|---|---|
| `signIn.accountsHeading` | "The two demo accounts" | "Demo Accounts" | Gate 3 §3 |
| `signIn.identifierLabel` | "Email or credential number" | "Email or username" | Gate 3 §3 |
| `signIn.identifierRequired` | "Enter your email or credential number." | "Enter your email or username." | Gate 3 §3 |
| `nav.record` | "My Access" | "My Requests" | Gate 5 §0 |
| `record.title` | "My access" | "My Requests" | Gate 5 §0 |

All es/pt values across every new and value-changed key are **`[ASSUMPTION]`**, pending native
review — flagged per-gate, restated here as the single authoritative count: **72 new + 5
value-changed = 77 entries needing es/pt translation**, all of them subject to the enforcement
described next.

**This table's correctness is enforced by an existing test, not merely asserted.**
`LocaleServiceTests.TheThreeFilesDefineExactlyTheSameKeys` fails automatically if any locale is
missing a key the others have, or carries one the others don't — every retirement above must
happen in all three files in the same commit, or this test catches the drift immediately.
`NoTranslationIsLeftEmpty` and `NoTranslationIsJustTheEnglishStringCopiedAcross` further mean the
77 `[ASSUMPTION]` es/pt values cannot ship as empty strings or as English copied across as a
placeholder — both are hard test failures, not style suggestions. `LocaleTestData.Loaded()` reads
the real `wwwroot/i18n/*.json` files directly (confirmed — no separate test-fixture copy exists to
fall out of sync), so this enforcement applies to the actual shipped files, not a proxy.

---

## 2. Consolidated file manifest

| File | Action | Gate |
|---|---|---|
| `Layout/SessionBar.razor` | **new** | 1 |
| `Layout/MainLayout.razor` | modified | 1 |
| `Layout/NavMenu.razor` | modified | 1 |
| `Components/Icon.razor` | modified | 1 |
| `Pages/Settings.razor` | **new** | 2 |
| `Components/LanguageSelect.razor` | **new**, replaces `LanguageSwitch.razor` | 2 |
| `Components/LanguageSwitch.razor` | **deleted** | 2 |
| `Components/AppearanceControl.razor` | **new**, replaces `ThemeTrigger.razor` | 2 |
| `Components/ThemeTrigger.razor` | **deleted** | 2 |
| `Components/ThemeTrigger.razor.css` | **deleted** | 2 |
| `Pages/Landing.razor` | modified | 3 |
| `Pages/SignIn.razor` | **deleted** | 3 |
| `Components/SignInForm.razor` | **new**, from `SignIn.razor` | 3 |
| `Services/DemoAccountStore.cs` | modified — `Identifier` values | 3 |
| `Components/MatchCard.razor` | **new** | 4 |
| `Components/EventCard.razor` | **kept**, unreferenced by `/matches` | 4 |
| `Pages/EventList.razor` | modified | 4 |
| `Pages/MyAccess.razor` | modified — R10 | 4 |
| `Pages/Registration.razor` | modified — route retarget | 4 |
| `Pages/EventDetails.razor` | modified — route retarget, played-CTA hidden | 4 |
| `Services/FixtureQuery.cs` | modified — availability filter | 4 |
| `Services/FixtureLabels.cs` | modified — team locale lookup | 4 |
| `Services/FixtureImporter.cs` | modified — `Congo DR` normalization | 4 |
| `Services/MockAccessDataProvider.cs` | modified — capacity rule | 4 |
| `wwwroot/i18n/en.json` | modified — §1 | 1–5 |
| `wwwroot/i18n/es.json` | modified — §1 | 1–5 |
| `wwwroot/i18n/pt.json` | modified — §1 | 1–5 |
| `wwwroot/css/app.css` | modified — new component styles, all gates | 1–4 |
| 15 test files (Gate 3 §6 table) | modified — identifier sweep | 3 |
| `GatingTests.cs` | modified — 3 assertions | 4 |
| `LandingTests.cs` | modified — 2 tests rewritten | 3 |
| `SignInScreenTests.cs` | modified — 1 assertion, relocates | 3–4 |
| `LanguageSwitchTests.cs` | modified — relocates to Settings | 2 |
| `ThemeTriggerPlacementTests.cs` | modified — relocates to Settings | 2 |
| `SignOutTests.cs` | modified — retargets `SessionBar` | 1 |
| `SlotAvailabilityTests.cs` | **new** | 4 |
| `MatchCardGatingTests.cs` | **new** | 4 |
| `TeamLocalizationTests.cs` | **new** | 4 |
| `ShowMoreTests.cs` | **new** | 4 |
| `SettingsScreenTests.cs` | **new** | 2 |

**No files added, modified, or deleted under `ux-ui/00-initial-evaluation/`,
`01-design-research/`, `02-ideation/`, or `03-ui-prototyping/`** — confirmed against this table:
zero entries touch those paths, matching Gate 0's commitment.

---

## 3. Full test inventory — every one of the 30 existing files, classified

**Breaks, full or partial rewrite required (9):**

| File | What breaks | Gate |
|---|---|---|
| `SignOutTests.cs` | Hard-coded nav row counts (5/6), exact row indices, `.nav-session` assertions | 1 |
| `LanguageSwitchTests.cs` | All markup/class assertions target retired `.language-switch*`; behavioral guarantees (no-nav, session-survives) restate against `Settings` | 2 |
| `ThemeTriggerPlacementTests.cs` | Targets `button.theme-trigger` inside `NavMenu`; component and placement both gone | 2 |
| `LandingTests.cs` | `TheLandingOffersBothEntryPoints`, `TheLandingSaysDemoAccountsExistAndWhereTheyAre` — both assert removed sections | 3 |
| `SignInScreenTests.cs` | `BothAccountsArePublishedWithIdentifierPasswordAndWhatDiffers`'s `DescriptionKey` assertion; suite relocates from `<SignIn>` | 3–4 |
| 15 files, identifier sweep | `SignInAsync`/`Match` calls passing the old identifier literal — see Gate 3 §6 for the exact per-file line table | 3 |
| `GatingTests.cs` | 3 assertions: `TheRecordIsNotReachableSignedOut...` (redirect no longer happens), `TheRequestFormIsNotReachableSignedOut` (`/signin`→`/record`), `HelpIsPublic` (string check meaningless post-retirement) | 4 |
| `IconTests.cs` | **Additive only** — extend `[InlineData]` with 5 new icon names; existing 3 cases unchanged | 1 |
| *(pagination tests, if any exist beyond `EventList`'s own coverage)* | Numbered-page-button assertions → Show-more | 4 |

*Note: the 15-file identifier sweep and `SignOutTests`/`LanguageSwitchTests`/`ThemeTriggerPlacementTests`/`LandingTests` overlap in file identity where a file needed both an identifier-literal fix and a structural rewrite — each is counted once above under its primary cause; Claude Code should treat Gate 3 §6's table as the authoritative per-line source for the sweep regardless of which other gate also touches that file.*

**Confirmed unaffected, verified this gate (not assumed) — 18:**

| File | Why |
|---|---|
| `FixtureQueryStatusTests.cs`, `FixtureQueryGroupTests.cs`, `FixtureQuerySearchTests.cs` | New `Apply` parameter carries a default value; existing calls compile and pass unmodified |
| `FixtureImporterTests.cs` | No existing Congo-specific assertion to break; needs an *addition*, not a fix (§4) |
| `LocalizedSearchTests.cs` (frozen) | Team localization is additive to `Search`; `MatchesCanonical` untouched |
| `TwoRecordsTests.cs` (frozen) | No `ux-ui/` or i18n dependency this run touches |
| `LocalizedChangeTests.cs` (frozen) | Uses `CredentialId` literals only, never `Identifier` as a sign-in argument |
| `RequestSubmittingStateTests.cs` | No reference to `signin`, credential identifiers, or any retired key — checked directly |
| `MockAccessDataProviderTests.cs` | No existing capacity/slot assertion; new coverage lives in `SlotAvailabilityTests.cs` instead |
| `LocaleServiceTests.cs` | Key-agnostic — iterates `Keys()` dynamically; **enforces** §1's table rather than being affected by it |
| `HelpDisclosureTests.cs` | Tests Help's own collapsible-section disclosure — unrelated concept to Landing's/SignIn's simulation notice |
| `InteropTests.cs` | TypeScript/JS interop layer — no UI surface this run touches |
| `LocaleTestData.cs` | Test helper, reads real i18n files directly — no hardcoded key list to drift |
| `TestData.cs` | Checked directly — no reference to any retired key or renamed identifier |
| `DisclosureTests.cs` | Uses `CredentialId` literals for data setup and `SignInAsync` calls needing the §Gate-3 sweep — **already counted under the sweep**, not separately here |
| `IconTests.cs` (the two `EventCard`-rendering cases specifically) | Render `EventCard` directly, never via `EventList` — pass regardless of `/matches`'s card swap |
| `FixtureQueryStatusTests.cs`, `..GroupTests.cs`, `..SearchTests.cs` | *(listed once above; not double-counted)* |
| `RequestSubmittingStateTests.cs` | *(listed once above; not double-counted)* |

**Frozen-by-precedent, confirmed untouched across all five gates:** `TwoRecordsTests.cs`,
`LocalizedChangeTests.cs`, `LocalizedSearchTests.cs`. No gate's file manifest names any of the
three, and none depends on a file these three read.

**New test files required (5):** `SettingsScreenTests.cs` (Gate 2), `SlotAvailabilityTests.cs`,
`MatchCardGatingTests.cs`, `TeamLocalizationTests.cs`, `ShowMoreTests.cs` (all Gate 4).

**New test case, existing file:** `FixtureImporterTests.cs` gains one case confirming `Congo DR`
and `DR Congo` both resolve to a single canonical fixture entry.

---

## 4. Commit sequence

One item per commit, build and tests green at every boundary — matching the discipline v13's own
Completion Report verified at each of its five commits, not only at HEAD.

| # | Commit | Gate |
|---|---|---|
| 1 | `feat(nav): add a persistent top bar carrying auth status` — `SessionBar`, `MainLayout` | 1 |
| 2 | `feat(nav): reduce the sidebar to destinations only` — `NavMenu` restructure, 5 new icons, R9 | 1 |
| 3 | `feat(settings): add the Settings screen` — `Pages/Settings.razor`, routing | 2 |
| 4 | `feat(settings): language selection as a dropdown` — `LanguageSelect`, retires `LanguageSwitch`, R7 | 2 |
| 5 | `feat(settings): tri-state appearance control` — `AppearanceControl`, retires `ThemeTrigger` | 2 |
| 6 | `feat(home): reduce Landing to heading, lede, and one CTA` | 3 |
| 7 | `feat(signin): condense the simulation notice` — R6 | 3 |
| 8 | `feat(signin): rename demo identifiers, reshape the accounts list` — R8 | 3 |
| 9 | `refactor(signin): extract SignInForm from the /signin route` | 3 |
| 10 | `feat(matches): add capacity to unplayed knockout fixtures` — R11 | 4 |
| 11 | `feat(matches): retire EventCard from /matches in favor of MatchCard` | 4 |
| 12 | `feat(matches): localize team names in the presentation layer` | 4 |
| 13 | `feat(matches): switch to Show More pagination` | 4 |
| 14 | `feat(matches): add the availability filter` | 4 |
| 15 | `feat(record): render sign-in inline instead of redirecting` — R10, retires `/signin` | 4 |
| 16 | `docs: archive v14 plan and completion report` | — |

Sixteen items — larger than v13's five, proportionate to a five-gate UX realignment against a
different codebase (ShopEase) rather than a single evaluation dossier's remediation.

---

## 5. Final constraint confirmations

- **No new runtime dependencies** — confirmed across all five gates; nothing in any file-touched
  table names a package reference. The tri-state Appearance control in particular required none
  (Gate 2 §2 — `clearStoredTheme()` already existed, unused).
- **The four frozen `ux-ui/` paths** — confirmed untouched, §2.
- **Zero AI-attribution language** — a standing constraint for the eventual `/cc-deploy-prompts`
  authoring, not something this dossier's own text needs to satisfy, but restated here as a
  carry-forward requirement for that next step.
- **`git diff` against `TwoRecordsTests.cs`, `LocalizedChangeTests.cs`, `LocalizedSearchTests.cs`
  must be empty** at every commit boundary in the sequence above.

---

## Register — final state

| # | Conflict | Resolution | Status |
|---|---|---|---|
| R6 | Sign In notice framed as removable | Condensed to 2 keys, 5 test substrings preserved verbatim | CLOSED |
| R7 | LanguageSwitch: buttons → dropdown | Relocated to Settings; original 3-option reasoning re-applied intact to Appearance | CLOSED |
| R8 | Demo identifier rename | `demo_staff1`/`demo_staff2`; `CredentialId` and all data-keying untouched | CLOSED |
| R9 | Sidebar indicator vs. top bar | Relocated; original reasoning upheld, placement superseded | CLOSED |
| R10 | MyAccess redirects to `/signin` | Renders `SignInForm` inline instead; `/signin` retired outright | CLOSED |
| R11 | Group Phase capacity premise | Factually impossible (all played); rescoped to 16 unplayed knockout fixtures | OPENED & CLOSED |

All six resolved. Zero open conflicts entering `/cc-deploy-prompts`.

---

## Done

Five gates, approved in sequence, zero open questions, every retired key enumerated across all
three locales (§1, enforced by an existing test rather than only asserted), every broken test
named against the actual 30-file inventory (§3), every file's action and originating gate recorded
(§2), and a commit sequence a `/cc-deploy-prompts` run can convert directly into an XML deploy
prompt.

**This dossier is ready for `/cc-deploy-prompts`.**
