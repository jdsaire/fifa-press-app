# Swapping a mock for the real thing without the screens noticing

This is the chapter the other three were building toward, and its punchline is
that almost nothing happened.

## The bet made several runs ago

Back in v9, when the access record was first built, the data layer was written a
particular way. Every page and component asks for its data through an
**interface** — a contract that says what you can ask for, without saying who
answers:

```csharp
public interface IAccessDataProvider
{
    Task<AccessResponse<Accreditation?>> GetAccreditationAsync(string credentialId);
    Task<AccessResponse<IReadOnlyList<Change>>> GetChangesAsync(string credentialId);
    Task<Change> RequestMatchAccessAsync(string credentialId, int matchNumber);
    // …and a few more
}
```

No screen has ever named `MockAccessDataProvider`. They ask for
`IAccessDataProvider` and use whatever they are handed.

That was a bet. Writing against a contract instead of a concrete class costs
something — an extra file, an extra layer of indirection, and a reader who has
to follow one more hop to find the code that actually runs. The payoff is
supposed to arrive later, when the thing behind the contract changes.

This run is when it arrived.

## What "swapping the provider" actually looked like

A new class, `ApiAccessDataProvider`, implements the same interface. Where the
mock read a list in memory, this one makes HTTP calls.

Then one place in `Program.cs` chooses between them:

```csharp
var apiBaseUrl = builder.Configuration["Api:BaseUrl"];

if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    builder.Services.AddSingleton<IAccessDataProvider>(_ => new MockAccessDataProvider(…));
}
else
{
    builder.Services.AddSingleton<IAccessDataProvider>(_ => new ApiAccessDataProvider(…));
}
```

That is the swap. **Not one page, component, stylesheet, route, or string
changed.** The record screen does not know it is talking to a server. It cannot
find out, and it has no reason to want to.

If you want to see this rather than believe it: run the app, look at the record
screen, then point it at a running API and look again. The rows are identical —
same wording, same order, same count. That was measured during the build, not
assumed: the same screen was rendered through both providers in one run and the
text of every row compared.

## The default stays the mock, on purpose

Notice which branch runs when nothing is configured. With no API base URL, the
app registers the mock and makes no network call at all.

This is not laziness about finishing the integration. It is a deliberate
property: the deployed site must keep working whether or not an API exists, is
awake, or is reachable. The API is meant to run on a free hosting tier that
stops the process when nobody has used it for twenty minutes, so "unreachable"
is a normal state rather than an emergency. Configuring a URL is what opts in,
and nothing else does.

## Where the abstraction did not fit cleanly

Interfaces are often described as if they make substitution free. They do not,
quite, and the interesting parts of this run were the places where the seam
showed.

### One method could not wait

Most of the interface is asynchronous — every method returns a `Task`, because
talking to a server takes time. One method is not:

```csharp
MatchAccessStatus GetMatchAccessStatus(string credentialId, int matchNumber);
```

It is called during rendering, from a component that cannot pause and wait. With
the mock that was harmless, because the answer was already in memory. With a
server, there is nowhere to fetch from without an `await` that is not available.

The answer was to keep a local copy of the changes as they are read, and fold
the status out of that copy. Which turns out not to be a compromise at all: the
interface has always said that reads resolve from local state first and the
network second, precisely so a screen can paint immediately instead of showing a
spinner. The API-backed provider follows the same rule the mock did, with a
network behind it rather than a seeded list.

### Some data never came from the API

The API serves accreditation records and their change logs. It does not serve
the match schedule — that is a CSV parsed in the browser, along with the rule
that hides team names for fixtures nobody has played yet.

So `ApiAccessDataProvider` holds a `MockAccessDataProvider` inside it and passes
every fixture question straight through to it.

That looks like a fudge and it is worth defending. Inventing fixture endpoints
would have meant duplicating the CSV, the parser, and the withholding rule on
the server so that the architecture diagram looked tidier — more code, two
copies of a rule that must never disagree, and no new capability. The honest
option was to serve what the API was scoped to serve and write down the boundary
where a reader will find it. Someone reading the app's screens should know that
the record came from the API and everything about matches did not.

### One value is deliberately not sent

Each change has an **urgency** — whether it interrupts the holder or is written
quietly to the record. It is derived from the change's own facts and the
holder's track, and it is never stored.

It would have been easy for the API to compute it and put it in the JSON. It
does not. A value that travels over a network is a value that can arrive
disagreeing with the facts it was derived from — a change re-classified on the
server, cached somewhere, and rendered next to the very data that contradicts
it. So the API sends the facts and the track, and the frontend draws the same
conclusion it always drew.

This is a small decision that generalises: **send what is true, not what you
concluded.** Conclusions can go stale in transit; facts cannot.

## What the whole thing cost

One package — the SignalR client — and it is the only runtime dependency this
app has gained since it was built. Two new service files. One line of
configuration. Four lines in the record screen so a pushed change repaints it.

And in exchange, the same app that has always run entirely in a browser can now
read from a real service, and a change written by somebody else reaches the
person it concerns without them touching anything.

The interface was written three runs before there was anything to put behind it.
That is usually the way: the cost of an abstraction is paid immediately and
visibly, and the benefit arrives later, quietly, as work you did not have to do.
