# Components/

Holds pieces of the app meant to be reused across more than one screen — as opposed to `Pages/`, which holds screens meant to be visited directly at their own web address. This distinction is why the two live in separate folders even though both hold the same kind of file (`.razor`).

- **`AccessCard.razor`** — the headline: what you hold right now. Structurally an `EventCard` variant, with one deliberate difference — no edit toggle, because this data isn't the reader's to change. It keeps accreditation and match access as two visually separate blocks, since the standing credential and access to any one match are genuinely different things.
- **`ChangeRow.razor`** — one entry in the record. Renders what changed, why, what to do next, and when, without guarding any of them: the model refuses to construct without all four, so they're always there. When an entry replaces an earlier one, the replaced value is shown alongside it rather than hidden, which is what makes it read as a *change* instead of an unrelated statement.
- **`ForeseeableBadge.razor`** — marks an entry that hasn't happened yet, so a condition is never mistaken for a decision already taken. Outlined rather than coloured, because giving it a status colour would imply an outcome the app doesn't know.
- **`StaleIndicator.razor`** — how old the information on screen is. Renders *always*, including when the data is seconds old, because an indicator that only appears when something is wrong teaches people to read its absence as "fine" — and absence is what a broken sync looks like too.
- **`ThemeTrigger.razor`** — the light/dark control, living in its own slim strip above the content. It follows the system setting until someone chooses otherwise, and once they do, that choice sticks and can't be quietly overturned by the system flipping later.
- **`RequestAccessForm.razor`** — the request form's fields and validation, extracted so the pattern lives in one place: per-field messages tied to their input for screen readers, and element ids made unique per instance so two forms on a page can't collide.
- **`EventCard.razor`** — the original reusable card, and the pattern the others follow. The match screens reuse it as-is for read-only presentation, opting out of its edit toggle.

A component here has no web address of its own and can't be visited directly; that one-line difference is the whole reason `Pages/` and `Components/` are separate folders.

For how `EventCard` works — its fields, and how editing one updates the underlying data automatically — see [`learning-mode/01-Building-the-Foundation.md`](../../../learning-mode/01-Building-the-Foundation.md#meet-the-building-block-the-eventcard-component).
