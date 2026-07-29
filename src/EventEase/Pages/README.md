# Pages/

Holds every screen you can navigate to directly at its own web address — as opposed to `Components/`, which holds reusable pieces that screens are built out of but that never have their own address.

- **`EventList.razor`** — the home screen (`/`). Loads all 50 mock events and shows one `EventCard` per event, each with a link to that event's details or registration page.
- **`EventDetails.razor`** — a single event's detail screen (`/events/{id}`). Shows that one event, whether you've already registered for it, and how many people in total have.
- **`Registration.razor`** — the signup screen (`/register/{id}`). Shows which event you're registering for, plus a name-and-email form with validation that has to pass before anything is submitted.
- **`NotFound.razor`** — the fallback screen shown automatically whenever a web address doesn't match any of the three above.

Each of these four files starts with a line like `@page "/events/{Id:int}"`, which is what actually makes it reachable at that address — a plain component in `Components/` has no such line and can't be visited directly at all. That one-line distinction is the whole reason `Pages/` and `Components/` are separate folders, even though every file in both is written the same way (`.razor`, markup plus C#).

For how moving between these four screens actually works, see [`learning-mode/01-Building-the-Foundation.md`](../../../learning-mode/01-Building-the-Foundation.md#how-the-app-moves-between-pages-routing) and, for what happens on a bad address, [`learning-mode/02-Fixing-What-Broke.md`](../../../learning-mode/02-Fixing-What-Broke.md#problem-2-a-wrong-web-address-used-to-crash-the-page).
