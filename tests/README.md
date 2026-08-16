# tests/

Automated tests for the app in [`../src/FifaPressApp/`](../src/FifaPressApp/) — code that checks other code, run by a test runner rather than by a person clicking through the app.

## Why this folder is outside `src/`

Everything under `src/` is the app itself: the files that get compiled and published to the live site. Test code is not part of the app — nobody visiting the site should be downloading it — so it lives in its own top-level folder instead. The publishing workflow names `src/FifaPressApp` by explicit path, so nothing here can reach what gets deployed.

## Two different things called "testing" in this repo

They are easy to confuse and mean genuinely different things:

- **Frontend tests — this folder.** Automated checks, written in C#, run by a command. Engineering evidence: does the code still do what it did yesterday?
- **UX usability testing — [`../ux-ui/`](../ux-ui/README.md).** Studies of how people understand and use the interface. Design evidence: does the thing the code does make sense to a person?

A reader who treats them as the same thing will misread both.

## Running them

From the repository root:

```
dotnet test tests/FifaPressApp.Tests
```

## What is in here

- [`FifaPressApp.Tests/`](FifaPressApp.Tests/) — the one test project, covering the schedule importer, the data provider, the match-list query, and the components those touch.
