# The middleware pipeline

## What middleware is, if you have not met the idea

When a request arrives at a web server, it does not go straight to the code that
answers it. It passes through a series of small components first, and the
answer passes back out through the same series in reverse.

The useful mental picture is not a queue but a set of **nested envelopes**. The
first component you register is the outermost envelope: everything registered
after it, including the code that actually answers, sits inside. A request works
its way inward through each layer, the innermost code produces a response, and
that response travels back out through every layer it came in through.

That nesting is the whole reason order matters. A component can only see, guard,
or fix things that are *inside* it.

Each component gets two things: the request, and a handle to "whatever comes
next". It can do work before calling next, after, both, or refuse to call next
at all — which is how a request gets stopped before it reaches anything.

## The three components here

This API has three, and they are the three the project's source material calls
for.

### 1. Error handling — outermost

Wraps everything in a `try`/`catch`. If anything inside throws and nobody caught
it, this returns:

```json
{"error":"Internal server error."}
```

with status `500`.

**Why it exists.** Without it, an unhandled exception behaves differently
depending on where the server is running. In development it returns a full stack
trace — file paths, method names, the shape of your code. In production it
returns an empty `500` with no body at all. The first leaks; the second tells
the caller nothing it can parse. One deliberate shape, identical everywhere, is
more useful than either.

**What it deliberately does not do.** It does not translate different exception
types into different status codes, and it does not put the exception's message
in the response. An unhandled exception means this server has a bug that the
caller can do nothing about, and dressing that up as a specific, actionable
failure would be a guess presented as a diagnosis. Anything a caller *can* act
on — a bad field, a missing record — is caught much earlier and answered with a
`400` or a `404` long before it reaches here.

The detail is not thrown away. It is logged in full. The operator gets the
exception; the caller gets a sentence.

### 2. Authentication — in the middle

Checks that the request carries the expected token, and returns `401` with
`{"error":"Unauthorized."}` if it does not.

**This is a simulated check, not authentication.** What actually happens is a
string comparison against `demo-token-2026`, a value written in plain text in
the API's `appsettings.json`, committed to a public repository, printed in
[`02_API-REFERENCE.md`](02_API-REFERENCE.md), and shipped to the browser inside
the frontend's own configuration file. There is no user, no credential store, no
issuer, no signature, no expiry, no revocation. Anyone who can read this
repository can pass it.

**Why build it that way on purpose.** The point of this layer is to demonstrate
*where* an authentication check belongs in a pipeline, *what* it does to a
request that fails, and how the rest of the application is written as though the
check were real. Adding a JWT library, a signing key, and an identity store
would produce a login system for a project that has no user accounts and no
server to keep secrets on. That would be a more convincing lie, not a more
honest program. The frontend's sign-in screen already publishes its own
passwords on screen for exactly the same reason; this is the server-side half of
that same commitment.

Two details worth knowing. The comparison is **ordinal** — the token is never
trimmed or case-folded, because leniency that is right for a human-typed
identifier is wrong for a credential. And the token may arrive either as an
`Authorization: Bearer` header or as an `access_token` query parameter, because
a browser cannot set headers on a WebSocket handshake and that is where SignalR
puts it.

The service description at `/` and the OpenAPI document are open. Putting them
behind the token would mean a reader has to already know the answer before they
can find the question, and neither exposes a record.

### 3. Logging — innermost

Records one line per request: the HTTP method, the request path, and the
response status code.

```
GET /api/accreditations/MP-2026-04817 -> 200
GET /api/accreditations/GHOST -> 404
```

**Three fields, and no more.** Request bodies here carry accreditation records,
and a logger that quietly wrote them to disk would turn an audit aid into a
second, unmanaged copy of personal data. Query strings are excluded from the
logged path for the same reason: the SignalR client puts its token there, and a
log that captured it would publish the one value the check depends on.

## Why this order

**Error handling first, authentication second, logging last.** That order comes
from the project's source material, and it is not to be "improved".

**Error handling is outermost because it can only catch what is inside it.**
Registered anywhere else, a throw from a layer above would escape it entirely
and the caller would get a stack trace or a bare `500`. Being outermost is not a
preference here; it is the only position from which the component does its job
at all.

**Authentication comes next so nothing further in runs for a request that has
not passed it.** No endpoint executes, no store is read, no record is touched.
Work done before rejecting is work done for nobody, and on a shared free-tier
instance it is work done on somebody else's behalf.

You can observe this ordering rather than take it on trust: ask for a record
that does not exist, without a token. You get `401`, not `404`. If the token
check ran after routing, the server would tell you the record was missing before
telling you that you were not allowed to ask.

**Logging is innermost so it sees the real answer.** It observes the status code
the endpoint actually produced, rather than one an outer layer might still
change on the way out.

## The cost of that order, stated plainly

This is the part most write-ups leave out.

Because logging is innermost, **two cases never reach it**: a request refused by
the token check, and a request that throws. The first is stopped one layer out;
the second propagates straight past the logging component instead of returning
through it. Either way, the single-choke-point property a request logger usually
has is not true here.

That is a real trade-off of the specified order. The response was **not** to
reorder the pipeline — the order is the thing being demonstrated, and quietly
rearranging it to make one component tidier would defeat the exercise. Instead
each component logs the case it handles, in the identical shape:

```
GET /api/accreditations -> 200                          ← logging middleware
DELETE /api/accreditations/MP-2026-04817 -> 401 (no valid token)   ← auth middleware
GET /api/diagnostics/throw -> 500 (unhandled exception)  ← error handler
```

Nothing goes unrecorded, and the log still greps as one stream — but it is
produced by three components rather than one, and you should know that before
you rely on it.

If this were a production system, the resolution would be to move logging
outermost and accept that it sees the status code slightly later, or to use the
framework's own diagnostics rather than a hand-written component. Both are
correct and both are outside what this exercise is demonstrating. The honest
thing is to build what was specified and write down what it costs.

## CORS

One more thing runs ahead of all three, and it is not one of them.

A browser will not hand a response from one origin to a page served from
another unless the response says it may. The deployed frontend is on
`jdsaire.github.io`; the API is not. Without a CORS policy every request the
live site made would reach the server, succeed, and be thrown away by the
browser before the page could read it.

It sits ahead of the three components for two reasons. A browser's preflight
`OPTIONS` request carries no token by design, so it has to be answered before
the token check would reject it. And a rejected request still needs CORS headers
on it — otherwise the browser reports a network error instead of showing the
page the `401` the server actually sent, and you spend an afternoon debugging
the wrong thing.

Origins are listed explicitly rather than opened with a wildcard. That is not
caution for its own sake: SignalR needs credentials to be allowed, and the CORS
specification forbids combining credentials with a wildcard origin.
