# Services/

Holds the app's shared, ongoing memory — plain C# classes that aren't tied to any single screen, created once when the app starts, and shared by whichever pages need them. This is what lets information survive moving from one page to another, which nothing in `Pages/` or `Components/` can do on its own.

- **`SessionTracker.cs`** — answers "which events has this visitor signed up for?" A per-person question: it doesn't know or care how many other people registered for the same event, only what this one visitor has done.
- **`AttendanceTracker.cs`** — answers "how many people, and who, have signed up for this specific event?" A per-event question, independent of any one visitor.

These two are kept as separate services on purpose, not merged into one, because the two questions they answer are genuinely different — cramming them together would make each harder to get right and harder to change safely later. Both are registered once, when the app starts, in `Program.cs`, and both are asked for by name wherever a page needs them (`@inject SessionTracker Session`, for example) rather than being built fresh each time.

For why keeping them separate mattered enough to be a deliberate choice, see [`learning-mode/03-Adding-Signups-and-Headcounts.md`](../../../learning-mode/03-Adding-Signups-and-Headcounts.md#why-keeping-them-separate-was-a-deliberate-choice).
