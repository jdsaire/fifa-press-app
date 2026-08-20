# A connection that stays open

Almost every request a web app makes is like a phone call that hangs up
immediately. The browser asks a question, the server answers, the line drops. If
the browser wants to know something new, it calls again. In between, the server
cannot reach the browser at all — it does not have the number.

That is fine for most things, and it was not fine for this app.

## The problem this app has always had

The sentence the whole project is built on is that a change to somebody's access
should reach them *before* they discover it by being refused at a gate.

For fourteen runs the app made that argument and could not act on it. The record
became correct the instant a change was written. It became correct **on screen**
when the holder happened to reload the page. If Amina's mixed zone access was
withdrawn while she was walking toward the mixed zone, the app had the
information and no way to volunteer it.

## Why not just ask over and over

The obvious fix is **polling**: have the browser ask "anything new?" every few
seconds. It works. It was rejected here, for two reasons.

First, it narrows the gap without closing it. Poll every thirty seconds and you
have deliberately built a system that can be thirty seconds wrong.

Second — and this matters more — think about who is holding the phone. A
journalist in a stadium concourse, on a foreign SIM, on a battery that has to
last until the final whistle. Polling spends a request every few seconds to
learn "nothing yet". Nearly all of those requests are wasted, and they are paid
for in the two things she has least of.

A connection that stays open costs one handshake and then stays silent until
there is something to say.

## What SignalR gives you

SignalR is the part of ASP.NET Core that manages connections like that. It comes
inside the framework, so using it on the server added no new package at all.

Underneath, it prefers **WebSockets** — a connection that, once opened, stays
open, and lets either end send whenever it likes. Where WebSockets are not
available it quietly falls back to older techniques, and your code does not
change.

The vocabulary is small. A **hub** is the endpoint clients connect to; this API
has one, at `/hubs/changes`. The server can **send** a named message down a
connection, and the client **registers a handler** for that name. That is the
entire model.

On the server:

```csharp
await hub.Clients.All.SendAsync("ChangeRecorded", credentialId, changeId);
```

On the client:

```csharp
connection.On<string, string>("ChangeRecorded", (credentialId, changeId) => { … });
```

The string `"ChangeRecorded"` has to match on both sides. A typo produces a
connection that works perfectly and never fires, which is a memorable afternoon.

## The message deliberately does not contain the change

Look at what gets sent: a credential id and a change id. Not the change itself.

Pushing the whole change down the wire and rendering it directly would have been
easy, and it was deliberately not done. It would give the app two sources for
what the record says — the one it fetched and the one it was told — and those
two can disagree. A pushed change that arrived twice, or out of order, or after
the record had moved on, would put something on screen the record does not
agree with.

So the notification says only *that* something moved. The client then re-reads
the record over HTTP. The record stays the only thing that says *what* it moved
to.

This is a pattern worth keeping. A notification is a prompt to go and look, not
a delivery.

## The one thing in the app that had to change

There was a catch, and it is a nice illustration of how a new capability can
quietly invalidate an old assumption.

The record screen already knew how to reload itself when the session changed,
because a person can sign in while it is already on screen. Nothing else could
make it reload — and there had never been a reason for anything else to, because
until this run the only thing that could change the record was the person
holding the phone.

A pushed change breaks that assumption. The record can now move while somebody
is looking at it, with no action from them at all.

So the record screen gained a second subscription: "the record moved, read it
again", which re-runs the load it already had. Four lines, in the screen's code
block. No markup changed, no styling changed, no new component appeared. And
with no API configured, nothing ever raises that signal, so the screen behaves
exactly as it always did.

Those four lines were the only change to a `.razor` file in the entire run, and
they were worth it for a specific reason: a hub that delivered a change to a
screen that did not repaint would be this project's own failure case in
miniature — the record correct, and the holder still unable to see it. Shipping
the mechanism without them would have meant documenting a feature that did not
work.

## What this is not

**It is not notifications.** Nothing arrives if the app is closed. No push to a
locked phone, no email, no badge. A real accreditation service would need all of
that; this is the piece that has to exist underneath it.

**It does not address one holder at a time.** The message goes to every
connected client, and each one checks whether the credential id concerns it. The
right approach is SignalR *groups* — one per holder, so a message only reaches
the person it is about. It is not done here because there are two demo holders
whose ids are already printed on the sign-in screen, so there is nothing to
leak. In production it would be wrong, and the code says so where a future
reader will find it.
