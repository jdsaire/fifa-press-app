# tests/

Automated tests for both halves of this project — code that checks other code, run by a test runner rather than by a person clicking through the app.

## Why this folder is outside `src/`

Everything under `src/` is the app itself: the files that get compiled and published to the live site. Test code is not part of the app — nobody visiting the site should be downloading it — so it lives in its own top-level folder instead. The publishing workflow names `src/frontend` by explicit path, so nothing here can reach what gets deployed.

## Two different things called "testing" in this repo

They are easy to confuse and mean genuinely different things:

- **Frontend tests — this folder.** Automated checks, written in C#, run by a command. Engineering evidence: does the code still do what it did yesterday?
- **UX usability testing — [`../ux-ui/`](../ux-ui/README.md).** Studies of how people understand and use the interface. Design evidence: does the thing the code does make sense to a person?

A reader who treats them as the same thing will misread both.

## Running them

From the repository root:

```
dotnet test tests/frontend
dotnet test tests/backend
```

512 and 33. Neither needs anything running first.

## What is in here

- [`frontend/`](frontend/) — tests for the Blazor app in [`../src/frontend/`](../src/frontend/): the schedule importer, the data provider, the match-list query, and the components those touch. Moved here from `FifaPressApp.Tests/` when a second test project appeared; the project file, its assembly and its namespace are unchanged.
- [`backend/`](backend/) — tests for the API in [`../src/backend/`](../src/backend/): every route and its failure cases, every validation rule, and the three middleware components exercised through the real request pipeline.

One folder per thing being tested, named after it. Before v15 there was one test
project and naming it after the product distinguished it from nothing; now there
are two, and the folders say which is which.
