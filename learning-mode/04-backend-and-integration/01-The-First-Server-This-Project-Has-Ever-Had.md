# The first server this project has ever had

Everything in this app, until now, ran inside a browser tab.

That is worth sitting with for a second, because it is unusual. When you visited
the live site, GitHub handed your browser a pile of files — some HTML, some CSS,
and a compiled bundle of C# — and then GitHub was finished. It did no work on
your behalf. Every list that got filtered, every record that got read, every
change that got written happened on your own machine, in your own browser. If
you had turned off your network after the page loaded, almost all of it would
have kept working.

The data behaved the same way. There was a class called
`MockAccessDataProvider` holding two accreditation records and eight changes as
ordinary C# objects, compiled into the bundle and shipped to you along with
everything else. When a screen asked for a record, it got one instantly, because
"fetching" meant reading a variable that was already in memory.

This run added a second program that runs somewhere else.

## What a server actually is here

A web API is a program that sits at an address, waits for requests, and answers
them. That is the whole idea. The interesting part is what "a request" and "an
answer" look like.

A request is a **method** and a **path**, plus optionally some data. `GET
/api/accreditations/MP-2026-04817` means "give me the accreditation record whose
id is MP-2026-04817". `POST /api/accreditations/MP-2026-04817/changes` with a
body attached means "add this change to that record's log".

An answer is a **status code** and usually some data. `200` means it worked.
`404` means there is no such thing. `400` means you asked wrongly. `401` means
you are not allowed to ask. The data comes back as JSON — text in a shape both
programs agree on.

None of that is specific to .NET. It is how nearly every web API works, and once
you have seen the pattern once you have seen it everywhere.

## The smallest thing that could be a server

Here is roughly what the API's starting point looks like, with the comments
stripped out:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AccreditationStore>();

var app = builder.Build();

app.MapGet("/api/accreditations", (AccreditationStore store) => Results.Ok(store.All()));

app.Run();
```

Five meaningful lines. `CreateBuilder` sets up the machinery. `AddSingleton`
says "there is one record store, share it with anything that asks".
`MapGet` connects a path to a function. `Run` starts listening.

The `(AccreditationStore store)` parameter is worth pausing on, because it looks
like magic the first time. Nobody passes that argument in. The framework sees
that the function wants an `AccreditationStore`, remembers that one was
registered, and supplies it. This is called **dependency injection**, and the
frontend has been doing exactly the same thing since it was built — every page
that writes `@inject IAccessDataProvider Access` is asking the same question of
the same machinery.

## Where the data lives, which is nowhere

There is no database.

The store holds its records in a `List<>` in memory, seeded at startup from a
JSON file. Restart the program and every change anyone made is gone, and the
two original records are back.

For a real accreditation system this would be indefensible. Here it is the right
answer, for a reason worth being explicit about: adding a database would have
meant a connection string, a schema, migrations, a hosting account for the
database itself, and a great deal of code whose purpose is storage rather than
demonstration. None of it would have made the API easier to understand, and all
of it would have made it harder to run — you could no longer clone this
repository and have the whole thing working with one command.

So the store is a list, and every document says so plainly rather than leaving
you to discover it.

## Two programs that share nothing

The frontend defines a class called `Accreditation`. The API defines one called
`AccreditationRecord`. They have the same fields. They are not the same class,
neither project references the other, and that is deliberate.

The tempting alternative is a third project holding the shared types, referenced
by both. It would remove the duplication. It would also mean that changing a
field in one place changes both programs at once, and that neither can be
deployed or understood without the other. Two programs that share a types
assembly are, in a real sense, one program that happens to be split across a
network.

Keeping them separate costs a mapping step — code that turns one shape into the
other. What it buys is that the API could be rewritten in another language
tomorrow and the frontend would neither know nor care, which is precisely the
property the next chapter is about.

## The thing that made this possible

None of this would have been a small job if the frontend had been written
differently.

Since v9 every screen has asked for data through an interface called
`IAccessDataProvider`, and no screen has ever named the class behind it. That
was a deliberate bet, made several runs before there was anything to swap in:
write the screens against a contract rather than a thing, and one day the thing
can change without the screens noticing.

This run is when that bet paid out. Adding a server did not require editing a
single page, component, or stylesheet. Chapter 4 is about what that actually
looked like.
