# Components/

Holds pieces of the app meant to be reused across more than one screen — as opposed to `Pages/`, which holds screens meant to be visited directly at their own web address. This distinction is why the two live in separate folders even though both hold the same kind of file (`.razor`).

Right now this folder holds exactly one file:

- **`EventCard.razor`** — the card that displays a single event's name, date, and location as editable fields. It's the one piece of UI in this entire app built to be handed different data and reused, rather than written once per screen. Three different pages — the events list, the event details page, and the registration page — all use this same component instead of three separate, near-identical copies of the same markup.

A single-file folder might look unnecessary, but the separation matters as the app grows: anything that should behave consistently everywhere it appears — a card, a button, a badge — belongs here, distinct from the screens that use it. If a second reusable piece is ever added, it goes here too, not into `Pages/`.

For how `EventCard` actually works — its fields, and how editing one updates the underlying data automatically — see [`learning-mode/01-Building-the-Foundation.md`](../../../learning-mode/01-Building-the-Foundation.md#meet-the-building-block-the-eventcard-component).
