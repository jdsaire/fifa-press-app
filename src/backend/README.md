# src/backend/

The ASP.NET Core Web API. **This is the code.** The plain-language
documentation for it is in [`backend/`](../../backend/README.md) at the
repository root — a different folder, with a deliberately similar name, and the
easiest thing in this repository to confuse.

## Why this folder exists

For fourteen runs this app ran entirely in a browser. Every screen read its data
from an in-memory mock compiled into the bundle and shipped to the visitor.
That was enough to demonstrate the design, and it left one claim untested: the
data layer was written behind an interface specifically so a real service could
replace the mock without the screens changing.

This folder is that real service, and the frontend reading from it is the test
of that claim.

## What is in here

| Folder | What it holds |
|---|---|
| `Endpoints/` | The routes, and the request shapes they accept. |
| `Middleware/` | Error handling, the simulated token check, request logging. |
| `Models/` | The accreditation record and the change, mirrored from the frontend. |
| `Realtime/` | The SignalR hub, and the one message it broadcasts. |
| `Storage/` | The in-memory store. |
| `Validation/` | What is refused, and why. |
| `Data/seed.json` | The two demo records and their eight changes. |
| `Program.cs` | The pipeline, in order, with the reasoning written down. |

## Running it

```bash
dotnet run --project src/backend
```

It listens on `http://localhost:5226` and describes itself at
`/openapi/v1.json`. [`backend/05_RUNNING-AND-DEPLOYING.md`](../../backend/05_RUNNING-AND-DEPLOYING.md)
covers running it alongside the frontend.

## Three things that surprise people

**There is no database.** Records live in a list in memory, seeded from
`Data/seed.json` at startup. Restart the process and every change is gone.
That is a scope decision, not an unfinished feature — a database would have
added a connection string, a schema, migrations and a hosting account, none of
which would make this API easier to understand or to run.

**The authentication is simulated.** The token is a fixed string, published in
this repository and printed in the documentation. There is no user, no
credential store, no signature. It demonstrates where such a check belongs in a
request pipeline; it secures nothing, and every file that touches it says so.

**It shares no code with the frontend.** There is no project reference in either
direction, and the models are mirrored rather than shared. Two programs that
share a types assembly are really one program split across a network — and the
point here was to show that the frontend can talk to a genuinely separate
service.
