# A connection that stays open

## The ordinary way, and why it was not enough

Almost every request a web app makes works like a phone call that ends
immediately. The browser asks a question, the server answers, the line drops.
If the browser wants to know something new, it calls again. The server has no
way to reach the browser in between — it does not have the number.

That is fine for most things. You ask for a page, you get a page. It is not fine
for this app, and the reason is the sentence the whole project is built on:

> A change to somebody's access should reach them before they discover it by
> being refused at a gate.

For fourteen runs this app made that argument and could not act on it. The
record was correct the moment a change was written. It became correct **on
screen** when the holder happened to reload the page. If a journalist's mixed
zone access was withdrawn while they were walking toward the mixed zone, the app
had the information and no way to volunteer it.

## Why not just ask repeatedly

The obvious fix is polling: have the browser ask "anything new?" every few
seconds. It works, and it was rejected here for two reasons.

The first is that it narrows the gap without closing it. Poll every thirty
seconds and you have built a system that can be up to thirty seconds wrong, on
purpose. That is the same design with a smaller number in it.

The second is about who this app is for. Amina is a journalist on a phone in a
stadium concourse, on a foreign SIM, on a battery that has to last until the
final whistle. Polling spends a request every few seconds to learn "nothing
yet". Almost every one of those requests is wasted, and they are paid for in the
two resources she has least of.

A connection that stays open costs one handshake and then stays quiet until
there is something to say.

## What SignalR is

SignalR is the part of ASP.NET Core that manages such connections. It ships
inside the framework this API already runs on, so using it added no package on
the server side.

Underneath it prefers **WebSockets** — a connection that, once opened, stays
open and lets either end send whenever it likes. Where WebSockets are
unavailable, SignalR falls back to older techniques automatically, and code
written against it does not change.

The vocabulary is small:

- A **hub** is the server-side endpoint clients connect to. This API has one,
  at `/hubs/changes`.
- A **connection** is one client's open line to that hub.
- The server can **send** a named message down a connection. The client
  **registers a handler** for that name, and it runs when the message arrives.

That is the whole model: named messages, in either direction, over a line that
stays up.

## What this app does with it

One hub. One message. It is called `ChangeRecorded`, and it carries two strings:
which credential the change belongs to, and the change's id.

When somebody writes a change to the API — `POST /api/accreditations/{id}/changes`
— the endpoint stores it and then broadcasts that message. Any browser with the
record screen open receives it and re-reads the record over HTTP, and the new
row appears without the person touching anything.

### The message is a prompt, not the data

Notice what the message does **not** contain: the change itself.

It would have been easy to push the whole change down the wire and render it
directly. It was deliberately not done, because that would give the app two
sources for what the record says — the one it fetched and the one it was told —
and they can disagree. A pushed change that arrived out of order, or twice, or
after the record had moved on, would put something on screen that the record
does not agree with.

So the notification says only *that* something moved. The record itself remains
the only thing that says *what* it moved to.

### Written first, announced second

The endpoint stores the change before it broadcasts. If the broadcast fails, the
change is still in the record and the next read finds it. Announcing first would
risk telling a client to go and look at something that is not there yet.

### The connection is optional, always

The hub client only starts if an API base URL is configured, and if it fails to
connect the app carries on without it. That is not defensive coding for its own
sake — the app worked for fourteen runs with no server at all, so a hub that
never comes up costs the live update and nothing else. On a free hosting tier
that stops the process when idle, a dropped connection is an ordinary event
rather than an exceptional one, and the client is configured to reconnect
automatically.

## The part that needed a change in the app

There was one thing this could not be done without touching.

The record screen already reloaded itself when the session changed, because a
person can sign in while it is already on screen. Nothing else could ever make
it reload, for a simple reason: until this run, nothing else could change the
record while somebody was looking at it. The only writer was the person holding
the phone.

A pushed change breaks that assumption. So the record screen now also listens
for "the record moved, read it again", and re-runs the load it already had. Four
lines, in the screen's code block — no markup, no styling, no new component. And
with no API configured nothing ever raises that signal, so the screen behaves
exactly as it did before.

It is worth being clear about why those four lines were worth an exception to a
rule that otherwise held across the whole run. A hub that delivered a change to
a screen that did not repaint would be this project's own failure case, in
miniature: the record correct, and the holder still unable to see it. Shipping
the mechanism without them would have meant documenting a feature that did not
work.

## What this is not

**It is not a notification system.** There is no push to a closed browser, no
email, no SMS, no badge on a home screen. If the app is not open, nothing
arrives. A real accreditation service would need all of that; this demonstrates
the piece that has to exist underneath it.

**It does not address individual holders.** The broadcast goes to every
connected client, each of which checks whether the credential id concerns it.
The right way is SignalR *groups*, one per holder, so a message only reaches the
person it is about. It is not done here because there are two demo holders whose
credential ids are already printed on the sign-in screen, so there is nothing to
leak — but it would be wrong in production, and it is written down here so
nobody promotes this code believing it was designed for that.

**The authentication on the connection is simulated**, like everywhere else. The
hub is behind the same fixed-string token check as every other route. See
[`03_MIDDLEWARE-PIPELINE.md`](03_MIDDLEWARE-PIPELINE.md).
