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

- **`TestData.cs`** — shared scaffolding: it loads the real schedule file, wraps it so the data provider can read it, and builds fixtures by hand. Those hand-built fixtures carry **no team names**, deliberately, so writing a test that leaks one is awkward rather than easy.
- **`FixtureImporterTests.cs`** — the three genuinely awkward things about the published schedule: a date format that parses differently depending on machine settings, two team names crammed into one column, and three rows whose kickoff time is written `24:00`, which is not a time a clock can show. Plus a pass over the real file: every row parses, and no row comes out naming a team.
- **`MockAccessDataProviderTests.cs`** — the withholding rule across the whole schedule (a match that has not kicked off never names its teams, on either read path), and the record's append-only behaviour: a withdrawal adds an entry rather than removing one, and the entry it withdraws stays exactly where it was.
- **`RequestSubmittingStateTests.cs`** — the regression this run fixed: the write path now returns a task that has not already completed by the time the caller gets it back, which is what lets the request form's Submitting state actually render. Also checks that no read method gained the same delay.
- **`IconTests.cs`** — every icon is decorative (`aria-hidden`, `focusable="false"`) and inherits its colour rather than declaring one, and adding an icon never removes the text it sits beside.
- **`FixtureQuerySearchTests.cs`** — proves the search predicate extracted into `FixtureQuery.cs` behaves identically to the one that used to live inside `EventList.razor`, by holding a literal copy of the original and comparing results across the real schedule.
- **`FixtureQueryGroupTests.cs`** and **`FixtureQueryStatusTests.cs`** — the two match-list filters: their option sets come from the data rather than a hardcoded list, they compose with search and with each other as an AND, and neither one ever surfaces a team name for a fixture that has not been played.

## Running them

From the repository root:

```
dotnet test tests/FifaPressApp.Tests
```

For why this folder sits outside `src/` at all, see [`../README.md`](../README.md).
