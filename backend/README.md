# backend/

Plain-language documentation for the backend layer: what it does, why it is as
small as it is, and how to run it.

**This folder is documentation. The code lives in [`src/backend/`](../src/backend/).**
That distinction is the single easiest thing to get wrong when you first open
this repository, so it is worth stating twice. `backend/` is deliberately
parallel to [`ux-ui/`](../ux-ui/README.md): `ux-ui/` documents the design of the
frontend, and `backend/` documents the backend. Neither folder contains anything
that compiles.

## Start here

If you want to *see* the difference the backend made, read
[`07_BEFORE-AND-AFTER.md`](07_BEFORE-AND-AFTER.md) first. It is the only one of
these documents written to be followed with the app open in front of you.

| File | What it covers |
|---|---|
| [`01_HOSTING-DECISION.md`](01_HOSTING-DECISION.md) | Where the API is meant to run, why that choice, and what the free tier costs you in behaviour. |
| [`02_API-REFERENCE.md`](02_API-REFERENCE.md) | Every route, verb, request shape and status code, in a table you can use without opening the code. |
| [`03_MIDDLEWARE-PIPELINE.md`](03_MIDDLEWARE-PIPELINE.md) | What middleware is, the three components here, and why their order matters. |
| [`04_REALTIME-SIGNALR.md`](04_REALTIME-SIGNALR.md) | What a connection that stays open is, and why this project wanted one. |
| [`05_RUNNING-AND-DEPLOYING.md`](05_RUNNING-AND-DEPLOYING.md) | Running both halves on your own machine, and deploying the API. |
| [`06_REPO-MAP.md`](06_REPO-MAP.md) | Where everything moved, and which documents still name the old paths on purpose. |
| [`07_BEFORE-AND-AFTER.md`](07_BEFORE-AND-AFTER.md) | What changed between the frontend-only build and this one, and how to see each change yourself. |

## The one thing to know before reading any of them

**The authentication is simulated.** The API checks that requests carry a token,
and that token is a fixed string published in this repository, printed in the
API reference, and shipped to the browser inside the frontend's own
configuration file. There is no user, no credential store, no signature, and no
expiry. Anyone who can read this repository can pass the check.

That is a deliberate scope decision, not an unfinished feature. This layer
exists to demonstrate *where* an authentication check belongs in a request
pipeline and *what* it does to a request that fails — not to secure anything.
Every document here repeats that in the place where it matters, because a
project that implied real security while having none would be lying about
itself, and that is the one thing this repository has refused for fifteen runs.

## Recorded reversals

This project records decisions it reverses, with the reason, rather than
overwriting them quietly. Five apply to this layer.

| # | What was reversed | Why |
|---|---|---|
| **R12** | The frontend directory moved from `src/FifaPressApp/` to `src/frontend/`, superseding the destination the v7 rename established. | v7 was not wrong. A second project simply exists now, and a folder named after the product no longer distinguishes anything from anything. |
| **R13** | The standing rule that the app project takes no new runtime dependency, reversed once for the SignalR client package. | A real-time client cannot exist without a real-time client library. The honest cost: the published bundle is no longer byte-identical to v14's, and this package is the only reason. |
| **R14** | The same rule, for `Microsoft.AspNetCore.Mvc.Testing` in the backend test project only. | It is the difference between asserting the middleware is registered in the right order and actually exercising it. The tests start the real application and send real HTTP through the real pipeline. |
| **R15** | The technical ceiling, for `Microsoft.AspNetCore.OpenApi`. | Registered bare, with no customisation. It is what lets a reader discover the API's shape without reading its source. |
| **R16** | The rule that no `.razor` file changes in this run, for four lines in `MyAccess.razor`'s code block. | Without them a change pushed from the server would arrive and the screen would not repaint — which is this project's own failure case, in miniature. No markup, stylesheet, string or route changed. |

R12 and R13 were authorised in the run's own brief. R14, R15 and R16 were raised
before any code was written and approved individually.
