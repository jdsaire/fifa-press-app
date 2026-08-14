# Pages/

Holds every screen you can navigate to directly at its own web address — as opposed to `Components/`, which holds reusable pieces that screens are built out of but that never have their own address.

- **`MyAccess.razor`** — the home screen (`/`). What you hold, what changed about it, and what you can do — answered without a tap and without a network. The headline comes from data already on the device, so there's no spinner in front of it, and it always shows how old that data is.
- **`EventList.razor`** — the match list (`/matches`). Used to be the home screen; it's now a supporting surface, because a fixture list can't answer what changed about your own access. Deliberately shows no access status: that belongs to the person, not scattered across a hundred and four match screens.
- **`EventDetails.razor`** — one match (`/events/{id}`). The fixture, your access to it, a timezone-stamped kickoff, and — when one applies — a statement that some entitlement depends on this result. It can also run a simulated gate check, which displays a disagreement between the record and the venue's list without deciding which side is right.
- **`Registration.razor`** — the request screen (`/request/{id}`). Renamed throughout from "Register", which read as account creation; nothing here creates a person, it asks for access to one match. On success it returns you to the record, where the resulting change is now the newest entry.
- **`SignIn.razor`** — the sign-in form (`/signin`). A form, not authentication: no account system, no session, nothing behind it, and the page says so on screen before you touch it. Every part of the app is reachable without it.
- **`Help.razor`** — help (`/help`). What the service does *not* do, what deliberately won't reach you as a notification, and who to contact. Entirely static, because the offline path through this app ends here and it has to be readable with no signal.
- **`NotFound.razor`** — shown automatically whenever an address doesn't match any of the above.

Each of these files starts with a line like `@page "/events/{Id:int}"`, which is what actually makes it reachable at that address — a plain component in `Components/` has no such line and can't be visited directly at all. That one-line distinction is the whole reason `Pages/` and `Components/` are separate folders, even though every file in both is written the same way (`.razor`, markup plus C#).

For how moving between these screens actually works, see [`learning-mode/01-Building-the-Foundation.md`](../../../learning-mode/01-Building-the-Foundation.md#how-the-app-moves-between-pages-routing) and, for what happens on a bad address, [`learning-mode/02-Fixing-What-Broke.md`](../../../learning-mode/02-Fixing-What-Broke.md#problem-2-a-wrong-web-address-used-to-crash-the-page).
