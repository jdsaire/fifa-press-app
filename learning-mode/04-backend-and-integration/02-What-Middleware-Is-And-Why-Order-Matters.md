# What middleware is, and why order matters

When a request arrives at the API, it does not go straight to the code that
answers it. It passes through a few small components first, and the answer
travels back out through the same components in reverse.

Those components are called **middleware**, and the arrangement of them is
called the **pipeline**.

## The picture that makes it click

The word "pipeline" is slightly misleading, because it suggests a queue —
one thing after another, in a line. The arrangement is actually **nested**, like
envelopes inside envelopes.

The first component you register is the outermost envelope. Everything
registered after it sits inside, including the code that actually answers the
request. A request works its way inward through each layer; the innermost code
produces an answer; the answer travels back out through every layer it came
through.

That single fact explains everything else in this chapter. **A component can
only see, guard, or fix what is inside it.**

In code, a piece of middleware looks like this:

```csharp
public async Task InvokeAsync(HttpContext context)
{
    // anything here happens on the way IN

    await next(context);   // everything inside me runs here

    // anything here happens on the way OUT
}
```

`next` is "whatever comes after me". A component can do work before calling it,
after calling it, both — or refuse to call it at all, which is how a request
gets stopped before it reaches anything.

## The three in this API

**Error handling** wraps everything in a `try`/`catch`. If anything inside
throws and nobody caught it, this returns `{"error":"Internal server error."}`
and a `500`.

**Authentication** checks the request carries the expected token, and returns
`401` if not. (It is a simulated check — a comparison against a fixed string
published in this repository. Chapter 4 of the `backend/` folder is blunt about
this, and so is the code. It is not security.)

**Logging** writes one line per request: the method, the path, and the status
code that came back.

They are registered in that order: error handling, then authentication, then
logging.

## Why that order

**Error handling has to be outermost**, because it can only catch what is inside
it. Put it anywhere else and a throw from a layer above escapes it entirely —
and the caller gets a stack trace, or an empty `500` with no explanation. Being
first is not a preference here; it is the only position from which it does its
job at all.

**Authentication comes next** so that nothing further in runs for a request that
has not passed it. No endpoint executes, no data is read. Work done before
rejecting is work done for nobody.

You can watch this ordering rather than trust it. Ask the API for a record that
does not exist, without a token:

```bash
curl -i http://localhost:5226/api/accreditations/NO-SUCH-THING
```

You get `401`, not `404`. The server refused you before it looked. If the token
check ran *after* routing, it would have told you the record was missing before
telling you that you were not allowed to ask — which is both less useful and, in
a real system, a way of leaking which records exist.

**Logging is innermost** so that it sees the real answer. It reads the status
code after calling `next`, which means it reads the code the endpoint actually
produced rather than one an outer layer might still change.

## The part that is usually left out

Here is the honest complication, and it is a good illustration of why ordering
is a trade-off rather than a solved problem.

Because logging is innermost, **two kinds of request never reach it**. One is a
request refused by the token check — that gets stopped one layer further out.
The other is a request that throws, because an exception propagates straight
past the logging component instead of returning through it.

So the property you would most want from a request logger — that it sees
everything, one place, no exceptions — is not true here.

The fix was **not** to reorder the pipeline. This order is the one the project
set out to implement, and quietly rearranging it to make one component tidier
would have meant documenting something other than what was built. Instead, each
component logs the case it handles, in the same shape:

```
GET /api/accreditations -> 200                                   (logging)
DELETE /api/accreditations/MP-2026-04817 -> 401 (no valid token) (auth)
GET /api/diagnostics/throw -> 500 (unhandled exception)          (error handler)
```

Nothing goes unrecorded, and the log still reads as one stream. But it is
produced by three components rather than one, and anyone relying on it should
know that.

If this were a production system the answer would probably be to move logging
outermost and accept that it learns the status code slightly later. That is a
real design decision with real reasons on both sides — which is the point worth
taking from this chapter. Middleware order is not a detail to get "right" by
convention. It is a set of trade-offs you make on purpose and then write down.

## One more thing, running ahead of all three

There is a fourth component, and it is not really one of the three: **CORS**.

A browser will not hand a response from one address to a page loaded from a
different address unless the response explicitly says it may. The deployed app
lives on `jdsaire.github.io`; the API does not. Without this, every request the
live site made would reach the server, succeed, and be discarded by the browser
before the page could read it — which produces one of the more confusing
afternoons available in web development, because the server logs show everything
working perfectly.

It runs ahead of the other three for two reasons. Browsers send a preliminary
`OPTIONS` request to ask permission before the real one, and that preliminary
request carries no token by design — so it has to be answered before the token
check would reject it. And a rejected request still needs the CORS headers on
it, or the browser reports a generic network error instead of showing the page
the `401` the server actually sent.
