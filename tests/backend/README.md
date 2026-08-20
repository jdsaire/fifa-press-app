# tests/backend/

Automated tests for the API in [`../../src/backend/`](../../src/backend/).

## Why this folder exists

The frontend has had a committed test suite since v10, and the reasoning that
justified it applies here too: a verification harness that lives outside the
repository proves something once and then disappears, and the next person has to
improvise their own.

There is a second reason specific to this layer. Most of what the API does is
invisible from the outside — a status code, a header, the order components run
in. A reader cannot check any of it by looking at a screen. These tests are how
that behaviour gets stated in a form that fails if it stops being true.

## What is covered

| File | Covers |
|---|---|
| `CrudEndpointTests.cs` | Every route, in both the case where it works and the case where it does not: the 404s, the 409 on a duplicate, the `Location` header actually resolving, deletion taking the change log with it, and the absence of any route that edits or deletes a change. |
| `ValidationTests.cs` | Every rule the validator enforces, including that all problems are reported at once, that a half-translated change is refused, and that a valid change is still accepted. |
| `MiddlewareTests.cs` | The 401 path, the ordinal token comparison, the query-string token, the open routes, the error handler's response shape, and that no stack trace reaches the caller. |

## Running them

```bash
dotnet test tests/backend
```

33 tests. Nothing needs to be running first — each test starts the real API
in-process.

## How they work, and the one package that made it possible

Each test creates an `ApiFactory`, which boots the actual application in memory
and hands back an `HttpClient` wired to it. Requests go through the real
pipeline: a 401 in these tests is a 401 the token middleware produced, and a 500
body is what the error handler actually wrote.

That comes from `Microsoft.AspNetCore.Mvc.Testing`, a package outside this run's
stated technical ceiling and added deliberately, recorded as reversal R14. The
alternative was to test the handlers as plain classes and assert that
`Program.cs` registers three components in a particular order — which would have
proved that a file contains three lines in a particular order, and nothing about
what happens to a request.

**Each factory gets its own store.** The store is a singleton holding mutable
state, so a test that creates a record would otherwise be visible to every test
that ran afterwards. xUnit guarantees no ordering, so those failures would be
intermittent and would look like flakiness rather than shared state.

The seed file is linked from `src/backend` rather than copied here. A second
copy could drift from the one the API actually reads, at which point these tests
would be proving something about data nobody serves.
