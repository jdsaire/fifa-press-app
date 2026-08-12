# Models/

Holds the plain C# classes that define the *shape* of the app's data — what properties an event or a registration has — separately from anything about how that data gets shown on screen or acted on. None of these files contain any markup or UI logic at all; they're the closest thing in this app to a plain data definition.

- **`EventModel.cs`** — the shape of one event: an ID, a name, a date, and a location. Every event card, detail page, and registration page is ultimately displaying one of these.
- **`MockEventData.cs`** — where the app's made-up sample events actually live: a method that returns a list of 50 `EventModel` instances, invented for building and testing rather than pulled from a database or an outside service. See [Glossary.md](../../../learning-mode/Glossary.md#mock-data).
- **`RegistrationModel.cs`** — the shape of one signup attempt: a name and an email address, each tagged with a rule (required, must look like an email) that the registration form checks against automatically.

Keeping these separate from `Pages/` and `Components/` means the *rules* about what an event or a registration looks like live in exactly one place each, instead of being scattered across every screen that happens to touch that data.

For where this data is used, see [`learning-mode/01-Building-the-Foundation.md`](../../../learning-mode/01-Building-the-Foundation.md#where-the-event-data-actually-comes-from-the-mock-data) (events) and [`learning-mode/03-Adding-Signups-and-Headcounts.md`](../../../learning-mode/03-Adding-Signups-and-Headcounts.md#the-registration-form-and-how-it-refuses-bad-input) (registration).
