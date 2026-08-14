# Services/

Holds the app's shared, ongoing memory — plain C# classes that aren't tied to any single screen, created once when the app starts, and shared by whichever pages need them. This is what lets information survive moving from one page to another, which nothing in `Pages/` or `Components/` can do on its own.

## The data layer

- **`IAccessDataProvider.cs`** — the contract every page and component talks to, and the first file in this whole feature that was written. Nothing in the app names a concrete provider, so replacing the in-memory one with something that talks to a real service is a change to one line in `Program.cs` rather than a change to every caller. It also carries four rules so its callers don't have to: every read reports its own freshness, reads come from local state before the network, writes return the change they produced rather than a success flag, and no method hands out the teams for a match that hasn't been played.
- **`MockAccessDataProvider.cs`** — the only implementation this version ships, holding everything in memory. It owns a simulated "now", and that's what makes the last rule above enforceable: team names get attached to a match in exactly one method, which refuses to attach them to a match that hasn't kicked off. Every read path goes through it, so a caller can't opt out by forgetting.
- **`FixtureImporter.cs`** — reads the published schedule CSV into fixtures. Kept separate from the provider on purpose: it knows about a file format and nothing about entitlements. Three things in the file are genuinely awkward — a date format that parses differently depending on machine settings, two team names crammed into one column, and three rows whose kickoff time is written `24:00`, which is not a time a clock can show.

## Retained from the earlier build

- **`SessionTracker.cs`** — answers "what has this visitor done in this session?"
- **`AttendanceTracker.cs`** — answers "how many people signed up for this?"

Both are kept rather than deleted. The count they backed is no longer displayed anywhere, but removing a display element and deleting the working code behind it are two different decisions, and only the first one has been taken. Neither is used to record access any more: access moves only by writing a change through the provider above, and having a second way to change it would defeat the point of keeping a record at all.

For why keeping the two trackers separate mattered enough to be a deliberate choice, see [`learning-mode/03-Adding-Signups-and-Headcounts.md`](../../../learning-mode/03-Adding-Signups-and-Headcounts.md#why-keeping-them-separate-was-a-deliberate-choice).
