# Models/

Holds the plain C# classes that define the *shape* of the app's data — what a fixture, a credential, or a logged change has on it — separately from anything about how that data gets shown on screen or acted on. None of these files contain any markup or UI logic at all; they're the closest thing in this app to a plain data definition.

## The access record

- **`Fixture.cs`** — one scheduled match: its number, both kickoff times, the phase, the venue and city, and the two team names. The team names are deliberately allowed to be empty, and that's the most important thing in this folder: the published schedule this app reads is a record of a *completed* tournament, so reading it straight through would tell the app who won every match before it was played. A match that hasn't kicked off yet arrives with no teams on it at all.
- **`Accreditation.cs`** — the standing credential: who holds it, which track they're on, what zones it permits, and the date it's approved *until*. Approval is never recorded as open-ended.
- **`Change.cs`** — one movement in the record, and the only way anything about access ever changes. Four things are required to create one — what changed, why, what to do next, and when — and the constructor refuses to build one without them, so a change with a blank reason can't exist rather than rendering as an empty line. There is no update method and no delete method: a correction is a *new* change that points at the one it replaces, and a withdrawal is a new change too.
- **`Track.cs`** — which of the three accreditation tracks a holder is on, and the notification ceiling that follows from it. The ceiling is recomputed every time it's read rather than stored, so it can't drift out of step with the track it came from.

## Retained from the earlier build

- **`EventModel.cs`**, **`MockEventData.cs`**, **`RegistrationModel.cs`** — the shapes the app used before the access record existed. Kept rather than deleted: removing working code is a separate decision from removing the screens that used it. See [Glossary.md](../../../learning-mode/Glossary.md#mock-data).

Keeping these separate from `Pages/` and `Components/` means the *rules* about what a fixture or a change looks like live in exactly one place each, instead of being scattered across every screen that happens to touch that data.

For where this data is used, see [`learning-mode/01-Building-the-Foundation.md`](../../../learning-mode/01-architecture-foundation/01-Building-the-Foundation.md#where-the-event-data-actually-comes-from-the-mock-data) (the earlier mock data) and [`learning-mode/03-Adding-Signups-and-Headcounts.md`](../../../learning-mode/01-architecture-foundation/03-Adding-Signups-and-Headcounts.md#the-registration-form-and-how-it-refuses-bad-input) (the request form).
