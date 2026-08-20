# 04 — Reading the Cache Before the Network

## The Usual Shape of Loading

Most screens that need data follow the same pattern. Show a spinner, ask the network, wait, then replace the spinner with the answer. In Blazor that falls out of the framework almost automatically: an `async` method that awaits something, and markup that shows a placeholder until it finishes.

It's a reasonable default when the alternative is a blank screen. It gets much less reasonable when the person using the app is standing at a barrier with a queue behind them and no usable signal.

## A Cache Is Just Data You Already Have

A **cache** is a local copy of something kept so you don't have to fetch it again ([Glossary.md](../Glossary.md#cache)). Nothing more exotic than that.

**Cache-first** means: look at the copy you already have, show it immediately, and treat asking the network as a separate thing that may or may not happen. Not "try the network, fall back to the copy if it fails" — the copy goes first, always, and the network is a top-up.

The difference is subtle in code and large on screen. Network-first with a fallback still shows a spinner while it tries, so someone with no signal waits for a timeout before seeing information that was on their device the whole time.

## How the Provider Does It

The reads in [`MockAccessDataProvider.cs`](../../src/frontend/Services/MockAccessDataProvider.cs) come back from local state, and — this is the part that matters — they come back *without yielding*:

```csharp
public Task<AccessResponse<Accreditation?>> GetAccreditationAsync(string credentialId)
{
    var record = credentialId == accreditation.CredentialId ? accreditation : null;
    return Task.FromResult(Cached<Accreditation?>(record));
}
```

`Task.FromResult` produces a task that is already finished. Awaiting it doesn't pause anything.

That's what removes the spinner, and it's worth being precise about why. When a Blazor component's `OnInitializedAsync` hits an `await` that hasn't completed, the component renders once *without* the data — which is the moment a spinner appears — and again later when it arrives. If nothing actually pauses, that first data-less render never happens. The component's first appearance already has everything.

So there is no spinner over the headline on [`MyAccess.razor`](../../src/frontend/Pages/MyAccess.razor) — not because one was hidden, but because there is no in-between state for it to occupy.

## What Still Has to Be Fetched

The match schedule is a genuine file fetch. It can't be instant.

`MyAccess` handles that by not depending on it. The record, the changes, and the status all render from local state straight away. The schedule loads afterwards, and all it does is upgrade "Match 98" into the round, venue and date. If it never arrives, the match number stays — which is still a true statement about someone's access, just a less convenient one.

This is what the split makes possible: the data the screen exists for is never behind the data that's merely nice to have.

## Freshness Travels With the Data

Cache-first creates a problem it has to solve. If the screen always shows something immediately, the person can't tell whether it's current.

That's why every read returns an `AccessResponse` carrying `LastSyncedUtc` alongside the value. The page doesn't work out how old its data is or track it separately — it arrives attached.

[`StaleIndicator.razor`](../../src/frontend/Components/StaleIndicator.razor) turns that into a sentence, and it renders **always**, including when the data is seconds old:

> Last updated 3 hours ago. Your access may have changed since.

Always, rather than only when something is old, for a specific reason. An indicator that appears only when there's a problem trains people to read its absence as "fine" — and absence is also what a component that failed to render looks like, and what a screen you forgot to put it on looks like. An indicator that's always there has no ambiguous state.

Past a threshold the same component adds weight and a left rule. The layout doesn't change and the headline doesn't move; what changes is that the line is now hard to skim past. That distinction is the entire point: a headline that's hours old must not look identical to one that's current, because someone reads it, believes it, walks to a barrier, and gets turned away — having been actively encouraged by the screen.

## What a Loading State Becomes

Once reads are cache-first, "loading" stops meaning "nothing to show yet." Everything is already on screen. What's in flight is a refresh, which is a much smaller claim, and the honest way to show it is a quiet indication that something is being checked — not a spinner sitting where the answer should be.

## Next

[File 05](05-Parsing-a-Real-CSV.md) is about the schedule file itself, which turned out to be less well-behaved than it looks.
