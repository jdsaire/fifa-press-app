# 04 — Backend and integration

The run that gave this app a server.

Everything before this ran inside a browser tab: the screens, the data, the
logic, all shipped to your machine and executed there. This stretch of work
added a second program — an ASP.NET Core Web API — and connected the two,
without changing anything you can see.

These four chapters are written for a reader who has followed the earlier
folders but has never built a backend. They assume you know what a class and a
method are, and nothing about HTTP, servers, or web APIs.

| File | Covers |
|---|---|
| [`01-The-First-Server-This-Project-Has-Ever-Had.md`](01-The-First-Server-This-Project-Has-Ever-Had.md) | What a web API actually is, the five lines that make one, why there is no database, and why the two programs deliberately share no code. |
| [`02-What-Middleware-Is-And-Why-Order-Matters.md`](02-What-Middleware-Is-And-Why-Order-Matters.md) | Middleware as nested envelopes rather than a queue, the three components here, why their order is what it is — and the real cost that order carries. |
| [`03-A-Connection-That-Stays-Open.md`](03-A-Connection-That-Stays-Open.md) | Why a normal request cannot tell you anything you did not ask for, why polling was rejected, and what a persistent connection changed about the concept this app is built on. |
| [`04-Swapping-A-Mock-For-The-Real-Thing.md`](04-Swapping-A-Mock-For-The-Real-Thing.md) | The interface written three runs early, what the swap actually cost, and the three places the abstraction did not fit cleanly. |

Read them in order the first time; chapter 4 refers back to all three.

Written after the code landed, describing what was actually built — including
the parts that did not go to plan.

## A standing note

The API's authentication is **simulated**: a comparison against a fixed string
that is published in this repository. It demonstrates where such a check belongs
in a request pipeline and what it does to a request that fails. It secures
nothing, and every chapter that touches it says so.

For the reference material rather than the narrative — every route, every status
code, how to run both halves — see [`backend/`](../../backend/README.md).
