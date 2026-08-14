# 03 — Records That Only Ever Grow

## The Normal Way, and Why It Wasn't Used

Most apps store the current state of a thing and edit it when it changes. Someone's access is narrowed, so you find their record and change the field. The record always tells you where things stand right now, and that's usually exactly what you want.

It also throws away everything else. After the edit there is no way to answer "what did this say yesterday", "when did it change", or "why". The record is accurate and has no memory.

For this app that trade is backwards. The whole point is answering *what changed and why*, and an app that only stores the present tense can't answer either.

## Append-Only

So [`Change.cs`](../../src/FifaPressApp/Models/Change.cs) is **append-only** ([Glossary.md](../Glossary.md#append-only)): you can add a change, and that is the only thing you can do. Nothing edits one. Nothing deletes one.

That sounds like a restriction you'd have to keep reminding yourself about. It isn't, because the class is built so the restriction is the only option available:

- Every property is get-only. There are no setters, so `change.Reason = "something else"` doesn't compile.
- There is no `Update` method and no `Delete` method, on the class or anywhere else.
- Everything is set once, in the constructor, and never again.

A correction is a **new** change that points back at the one it replaces, through a field called `SupersedesChangeId`. A withdrawal is a new change too. Both leave what came before exactly where it was.

## What That Makes Easy

Reasoning about the record gets noticeably simpler.

Any question about the past is just a matter of reading. "What did this say last Tuesday?" is answered by ignoring everything effective after last Tuesday — no separate history table, no audit log, because the record *is* the history.

[`EventDetails.razor`](../../src/FifaPressApp/Pages/EventDetails.razor) uses precisely this. Its simulated gate check needs two answers: what the app believes, and what a venue's list — running a day behind — would believe. Both come from folding the same changes with two different cut-off dates. There is no second data source. One record, read twice, at two moments.

And there is no state where two places disagree, because there aren't two places. A status is never stored; it's worked out from the changes every time it's needed. It can be out of date, but it cannot contradict the log it came from.

## What It Costs

Reading is more work. "What can this person get into right now" isn't a field to look up — it's a fold over every change affecting that match, skipping any that have been superseded, taking the most recent. That's a handful of lines in a few places rather than one property read.

The record also only grows. Nothing is ever reclaimed. At this size that's irrelevant; at a large enough scale it becomes a real design problem with real solutions, none of which are in this app.

## Failing at Construction

One more decision, and it's the one most visible on screen.

A change carries four things that always matter: what changed, why, what to do next, and when. The first three are text someone has to write. It would be easy to allow them to be empty and let the screen deal with it.

[`Change.cs`](../../src/FifaPressApp/Models/Change.cs) refuses instead. Its constructor throws if any of the three is missing or blank:

```csharp
Require(whatChanged, nameof(whatChanged));
Require(reason, nameof(reason));
Require(nextStep, nameof(nextStep));
```

The effect is that an incomplete change never becomes an object at all. It isn't stored, isn't passed around, and never reaches a screen — so [`ChangeRow.razor`](../../src/FifaPressApp/Components/ChangeRow.razor) renders all four fields with no `if` guarding any of them. There is no "reason missing" case in the display code because there is no way to build one.

There's a fourth check that's less obvious. It also rejects a reason that merely restates what changed:

```csharp
if (Normalize(reason) == Normalize(whatChanged))
```

"Your access was revoked" as the reason for "your access was revoked" passes any is-it-blank test and explains nothing. Catching it needs comparing two fields rather than checking one, which is a small amount of extra code for a rule that would otherwise be a style note nobody enforces.

## Deriving a Value Instead of Storing It

`Change` also has an `Urgency` — whether something is immediate, foreseeable, or quiet enough not to interrupt. It isn't a constructor parameter and has no setter. It's computed once, at construction, from the kind of change, its dates, and the holder's track.

That's deliberate: making it a parameter would let a caller pass in whatever urgency it liked, and the value would then mean nothing beyond "what somebody typed." Derived, it always follows from the facts of the change itself.

`Track` does the same thing one level down. Its notification ceiling is recomputed every time it's read rather than stored, so there's no way for a stored ceiling to drift out of step with the track it came from.

## Next

[File 04](04-Reading-the-Cache-Before-the-Network.md) is about where the data comes from when the page first appears — and what a loading state looks like when the answer is "it's already here."
