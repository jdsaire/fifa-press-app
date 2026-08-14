# 01 — Putting the Data Behind a Door

## What Changed Since the Last Folder

The three files in `01-architecture-foundation/` describe an app that browsed matches and let you sign up for one. This app does something different: it shows a journalist what their tournament access currently permits, what has changed about it, and why.

That meant new data — a credential, a log of changes, the real match schedule — and the first decision about all of it was where it should live. This file is about that decision, because it's the one everything else in this folder sits on top of.

## The Problem With Calling a Class Directly

Say a page needs the list of changes on someone's record. The obvious thing is for the page to ask the class that holds them:

```csharp
@inject MockAccessDataProvider Access
```

That works. It also quietly welds every page to that one class. Right now the data is held in memory, made up, and never leaves the browser. A later version will fetch it from a real service over the network. When that day comes, every page that named `MockAccessDataProvider` has to be found and changed — and "find every place that mentions this class" is exactly the kind of job where one gets missed.

## What an Interface Is

An **interface** is a list of what something can do, with none of the how. It says "anything calling itself a data provider must be able to fetch a credential, fetch the changes on it, fetch fixtures, and record a request" — and stops there. It contains no code that does any of those things.

[`IAccessDataProvider.cs`](../../src/FifaPressApp/Services/IAccessDataProvider.cs) is that list. Six operations, and a description of what each one returns. Nothing else.

Then a separate class *implements* it — promises to actually provide all six. [`MockAccessDataProvider.cs`](../../src/FifaPressApp/Services/MockAccessDataProvider.cs) is this version's implementation, holding everything in memory.

The pages ask for the interface, never the class:

```csharp
@inject IAccessDataProvider Access
```

## What That Buys

One line in [`Program.cs`](../../src/FifaPressApp/Program.cs) is the only place in the entire app that names the concrete class. It's the line that says "when something asks for the interface, hand it this" ([Glossary.md](../Glossary.md#dependency-injection)).

Swapping in a version that talks to a real service means writing a new class that implements the same six operations, and changing that one line. Not one page changes. Not one component. The pages never knew which class they were talking to, so they can't notice when it becomes a different one.

This is the whole reason the interface was written **first**, before any page or component existed. Written the other way round — pages first, interface extracted afterwards — you get pages already shaped around whatever the first class happened to do, and pulling them apart later is a much bigger job than doing it in the right order once.

## The Part That Was Genuinely Hard

There's a second reason to funnel every read through one class, and it's specific to this app.

The match schedule it reads is a real published spreadsheet of the 2026 World Cup — and it's a record of a tournament that has already finished. Every knockout row names two actual teams. There are no "Winner of Group A" placeholders anywhere in it.

Read that file straight through and you know who won every match before it's played. Which sounds like a bonus and is actually a disaster: this app's entire job is warning someone that their access *might* change depending on a result. If it already knows the result, there's nothing to warn about, and the warning it does show is a lie about what it knows.

So `MockAccessDataProvider` holds a simulated "now" — a moment partway through the tournament — and team names get attached to a fixture in exactly one method, `Reveal`, which refuses to attach them to a match that hasn't kicked off by then. Every read goes through it.

The important part is *where* that rule lives. It could have been a rule in the pages: "remember not to show team names for future matches." That kind of rule survives exactly as long as everyone remembers it, and the first person to add a new screen months from now has no way of knowing it exists. Putting it inside the provider means a page can't get a team name for an unplayed match even if it asks — there simply isn't one on what it's handed.

[`Fixture.cs`](../../src/FifaPressApp/Models/Fixture.cs) takes it one step further. The importer that reads the file never puts team names onto a fixture at all; it hands them back separately, in a lookup the provider keeps to itself. So a fixture object carrying a name it shouldn't have isn't something the code can produce by accident — only deliberately, in one method, in one file.

## The Shape of a Read

One more thing the interface carries. Every read comes back wrapped:

```csharp
AccessResponse<T>(T Value, DateTime LastSyncedUtc, bool WasServedFromCache)
```

The data, plus when it was last synchronised, plus where it came from. A page never has to work out how old its information is or remember to track it separately — it arrives attached. File 04 in this folder is about why that matters more than it sounds.

## Next

[File 02](02-Two-Themes-and-a-Pile-of-Hex-Codes.md) leaves the data layer entirely and goes to the stylesheet, which had eleven colours hardcoded into it and needed to grow a second theme.
