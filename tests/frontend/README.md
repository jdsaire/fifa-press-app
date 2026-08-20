# FifaPressApp.Tests/

The repository's test project. It reads the same code the live app runs — it does not hold a copy of it — so a test passing here means the shipped app behaves that way.

## Why the packages live here and nowhere else

This project is the only place in the repository that adds outside packages:

| Package | What it does |
|---|---|
| `Microsoft.NET.Test.Sdk` | Lets `dotnet test` find and run tests at all |
| `xunit` | The framework the tests are written in — `[Fact]`, `[Theory]`, `Assert` |
| `xunit.runner.visualstudio` | Connects the two above, so the runner can discover xUnit tests |
| `bunit` | Renders a Blazor component in memory, so a test can check what a screen actually puts on the page |

The app project itself gains nothing. That is a deliberate boundary: adding a package to the app to make a test easier would put test-shaped weight into what every visitor downloads. When a test is hard to write, the test gets restructured, not the app.

## What each file protects

**Shared scaffolding**

- **`TestData.cs`** — loads the real schedule file, wraps it so the data provider can read it, and builds fixtures by hand. Those hand-built fixtures carry **no team names**, deliberately, so writing a test that leaks one is awkward rather than easy.
- **`LocaleTestData.cs`** — the equivalent for language: loads the app's real `en.json`/`es.json`/`pt.json` files (not a test-owned copy of them), so a test asserting a translation is asserting against what actually ships.

**The schedule, the record, and the write path**

- **`FixtureImporterTests.cs`** — the three genuinely awkward things about the published schedule: a date format that parses differently depending on machine settings, two team names crammed into one column, and three rows whose kickoff time is written `24:00`, which is not a time a clock can show. Plus a pass over the real file: every row parses, and no row comes out naming a team.
- **`MockAccessDataProviderTests.cs`** — the withholding rule across the whole schedule (a match that has not kicked off never names its teams, on either read path), and the record's append-only behaviour: a withdrawal adds an entry rather than removing one, and the entry it withdraws stays exactly where it was.
- **`TwoRecordsTests.cs`** — the second seeded holder, and the one thing the two records exist to demonstrate: a structurally identical conditional change resolves to two different urgencies depending on the holder's ceiling alone, with no logic added anywhere to produce that. Also proves each record is genuinely isolated — neither can read or write the other's changes.
- **`RequestSubmittingStateTests.cs`** — the regression this run fixed: the write path now returns a task that has not already completed by the time the caller gets it back, which is what lets the request form's Submitting state actually render. Also checks that no read method gained the same delay.
- **`IconTests.cs`** — every icon is decorative (`aria-hidden`, `focusable="false"`) and inherits its colour rather than declaring one, and adding an icon never removes the text it sits beside.
- **`FixtureQuerySearchTests.cs`** — proves the search predicate extracted into `FixtureQuery.cs` behaves identically to the one that used to live inside `EventList.razor`, by holding a literal copy of the original and comparing results across the real schedule.
- **`FixtureQueryGroupTests.cs`** and **`FixtureQueryStatusTests.cs`** — the two match-list filters: their option sets come from the data rather than a hardcoded list, they compose with search and with each other as an AND, and neither one ever surfaces a team name for a fixture that has not been played.

**The simulated session**

- **`DemoSessionTests.cs`** — the two demo accounts and the session they open: a published credential works, a wrong one doesn't, the password is compared byte-for-byte, and the sign-in path genuinely yields so its Submitting state is reachable.
- **`SignInScreenTests.cs`** — everything the sign-in screen renders. The submit path itself is asserted against `DemoSessionTests` instead, at the service layer — bUnit has no JavaScript engine, and the identifier field's binding needs one to drive from a test.
- **`GatingTests.cs`** — the record and the request form refuse a signed-out visitor without ever borrowing the language of real security (no "403," no "access denied"); Matches and Help stay reachable throughout.
- **`SignOutTests.cs`** — the sign-out row appears only once signed in, ends the session with no confirmation dialog, and the sidebar names whichever holder is currently signed in.
- **`LandingTests.cs`** — the front door states what the app is and that it's a demonstration, offers both entry points, and never publishes the demo credentials itself (they belong on the sign-in screen, one click away).

**Three languages**

- **`LocaleServiceTests.cs`** — the language service itself: all three files load, share exactly the same keys as each other, and no value is left untranslated or copy-pasted from English.
- **`LocalizedChangeTests.cs`** — a `Change`'s free-text fields refuse to construct unless all three languages are present, not just English, so a half-translated change can't exist to be discovered later.
- **`LocalizedDateTests.cs`** — every date and duration on screen goes through the language service rather than a raw C# format string, in all three languages.
- **`LocalizedSearchTests.cs`** and **`LanguageSwitchTests.cs`** — searching the match list in Spanish or Portuguese, and switching language mid-session without losing the signed-in session, the theme, or a scrap of app state.

**Look and feel**

- **`ThemePaletteTests.cs`** — the re-derived dark palette clears its contrast floor for every token, computed from the actual hex values rather than quoted from a document.
- **`ThemeTriggerPlacementTests.cs`** — the theme control renders as a nav row rather than the strip it used to live in, with no leftover markup or styling from the old placement.
- **`DisclosureTests.cs`** — a change row's collapsed layer stays fully informative on its own, its expanded layer adds detail without removing anything, and a row you just wrote arrives open and marked as new.
- **`HelpDisclosureTests.cs`** — all eight Help sections start closed, open independently of each other, and the page still renders with no data provider registered at all — proof it stays entirely static.

**The TypeScript interop**

- **`InteropTests.cs`** — the compiled JavaScript in `wwwroot/js/` actually matches what its TypeScript source exports, so the two can't silently drift apart; also confirms the interop toolchain stays outside the app project and outside the deployment workflow entirely.

## Running them

From the repository root:

```
dotnet test tests/frontend
```

For why this folder sits outside `src/` at all, see [`../README.md`](../README.md).
