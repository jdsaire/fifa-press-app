# Before and after: what the backend actually changed

Fourteen runs built a frontend. This one gave it a server. This document is
about the difference — what is genuinely new, what only looks new, and how to
see each thing with your own eyes rather than taking this file's word for it.

It is written to be read with the app open. Everything below can be checked in
about fifteen minutes on your own machine, and
[`05_RUNNING-AND-DEPLOYING.md`](05_RUNNING-AND-DEPLOYING.md) covers the setup in
more detail if any step here is too terse.

---

## 1. The honest headline

**The app looks exactly the same.** That is not a disappointing result — it is
the result the whole exercise was designed to produce.

Since v9 every screen in this app has asked for its data through an interface
called `IAccessDataProvider` and has never named the thing behind it. The
promise of writing it that way was that a real service could be substituted
later without the screens noticing. This run is the first time that promise was
tested, and the way you know it held is that nothing moved.

So the interesting comparison is not "what does the app look like now." It is
**where the data on screen comes from, what happens when it changes, and what a
reviewer can now inspect that did not exist before.**

---

## 2. The comparison table

| | Up to v14 — frontend only | v15 — full stack |
|---|---|---|
| **Projects in the repository** | One app, one test project | Two apps (`src/frontend`, `src/backend`), two test projects, one interop toolchain |
| **Where the record comes from** | `MockAccessDataProvider`, an in-memory list compiled into the browser bundle | An HTTP API — `GET /api/accreditations/{id}` — when one is configured; the mock otherwise |
| **Server** | None. GitHub Pages serves static files and nothing runs | ASP.NET Core Web API, `net10.0` |
| **HTTP surface** | None to inspect | 8 routes, 5 verbs, documented at `/openapi/v1.json` |
| **Writing a change** | A method call inside the browser | `POST /api/accreditations/{id}/changes`, validated server-side |
| **Validation** | In the `Change` constructor, client-side only | Same domain rules, enforced again at the server before anything is stored |
| **Request pipeline** | Not a concept that applied | Three middleware components in a documented order |
| **Authentication** | None at all | A **simulated** token check returning 401 — still not security, but now visible where a real one would sit |
| **Error handling** | Browser-side exceptions | Unhandled exceptions return one consistent JSON shape, never a stack trace |
| **Logging** | Browser console | Server-side, one line per request: method, path, status |
| **How a change reaches the holder** | Only by reloading the page | Pushed over a SignalR WebSocket; the record repaints with no refresh |
| **Cross-origin concerns** | None — one origin | A CORS policy, because the deployed site and the API are on different origins |
| **Automated tests** | 512 | 512 frontend **+ 33 backend** = 545 |
| **What a reviewer can poke at** | The rendered UI | The rendered UI, plus a live API they can `curl` |
| **Runtime dependencies in the app project** | Two (Blazor WebAssembly) | Three — the SignalR client is the only addition, ever |

### What did *not* change

Worth its own list, because it is the load-bearing half of the claim.

- No page, component, stylesheet, route, or user-visible string.
- No namespace, assembly name, or `.csproj` filename. `FifaPressApp.csproj` is
  still `FifaPressApp.csproj`; only its folder moved.
- No behaviour when no API is configured — which is the shipped default.
- The three languages, both themes, the demo sign-in, the match list, the
  withholding rule that hides team names for unplayed fixtures: all untouched.

---

## 3. See it for yourself

### 3.1 The app, unchanged, with no server at all

```bash
dotnet run --project src/frontend
```

Open the URL it prints. Sign in with `demo_staff1` / `Demo#2026Staff1`. This is
v14's app, running v15's code, with no backend involved — no API is configured
by default, so the mock provider is registered and not one network call is made.

**What to look for:** everything works. That is the point. If you have seen the
deployed site before, compare them; you should not be able to tell which is
which.

### 3.2 The server exists now

In a second terminal:

```bash
dotnet run --project src/backend
```

Then, without any special tooling:

```bash
curl http://localhost:5226/openapi/v1.json
```

**What to look for:** a machine-readable description of eight routes. Before
this run there was no such thing to ask for.

### 3.3 The record is really being served over HTTP

```bash
curl -H "Authorization: Bearer demo-token-2026" \
  http://localhost:5226/api/accreditations/MP-2026-04817
```

**What to look for:** Amina's accreditation as JSON — the same holder, outlet,
track and zone list the app has shown since v9, now arriving over a wire. Drop
the `-H` and you get `401` instead.

### 3.4 The same screens, reading from the API

Create `src/frontend/wwwroot/appsettings.Development.json`:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5226/",
    "HubPath": "hubs/changes",
    "Token": "demo-token-2026"
  }
}
```

Restart the frontend and sign in again.

**What to look for:** no visible difference. Same rows, same wording, same
order. The record you are looking at came from the API this time. Delete that
file and restart to go back to the mock — the screen will look identical again.

This is the comparison the run exists to make, and its whole content is that
there is nothing to see.

### 3.5 A change arriving without you doing anything

Leave the record screen open. In a terminal:

```bash
curl -X POST -H "Authorization: Bearer demo-token-2026" -H "Content-Type: application/json" \
  -d '{"changeId":"ch-demo","kind":"MatchAccessRevoked","effectiveUtc":"2026-07-09T12:00:00Z","whatChanged":{"en":"Mixed zone access withdrawn for the Dallas fixture.","es":"Se retira el acceso a la zona mixta.","pt":"O acesso a zona mista foi retirado."},"reason":{"en":"The host city reduced the mixed-zone allocation.","es":"La ciudad sede redujo la asignacion.","pt":"A cidade-sede reduziu a alocacao."},"nextStep":{"en":"Contact the venue media office on arrival.","es":"Contacta la oficina de prensa al llegar.","pt":"Contacte o gabinete de imprensa a chegada."},"affectsMatchNumber":98}' \
  http://localhost:5226/api/accreditations/MP-2026-04817/changes
```

**What to look for:** the new row appearing on the record screen **without you
touching the browser**, marked with the same entrance treatment a change you
wrote yourself gets.

This is the only item in this document that is a genuinely new *capability*
rather than a relocation of an existing one, and it is the one worth dwelling
on. This whole project argues that a change to somebody's access should reach
them before they discover it by being refused at a gate. Until this run the app
made that argument and could not act on it: the record was correct, and it
became correct on screen only when the holder happened to reload. Now the server
can speak first.

### 3.6 The middleware, from the outside

```bash
curl -i http://localhost:5226/api/accreditations                       # 401, no token
curl -i "http://localhost:5226/api/diagnostics/throw?access_token=demo-token-2026"
```

**What to look for:** the first is `{"error":"Unauthorized."}`. The second is
`{"error":"Internal server error."}` — a deliberate crash, returning a clean
sentence rather than a stack trace. Watch the API's terminal while you do it:
one log line per request, method, path and status.

The `diagnostics/throw` route exists only to make that demonstrable and is
registered behind an environment check, so it does not exist on a deployed
instance.

### 3.7 The tests

```bash
dotnet test tests/frontend
dotnet test tests/backend
```

**What to look for:** 512 and 33. The frontend number is unchanged from v14 —
the reorganisation and the new provider did not cost a single existing test.

---

## 4. What a reviewer should *not* conclude

Three things this build might look like from a distance, and is not.

**"The app is now backed by a real service."** It is backed by one *if you
configure one*. The shipped default is still the mock, and that is deliberate:
the deployed site must keep working whether or not an API exists, is awake, or
is reachable.

**"The data all comes from the API now."** It does not. The API serves
accreditation records and their change log. The match schedule is still a CSV
parsed in the browser, and the rule that withholds team names for fixtures
nobody has played stays on the frontend with it. That gap is real and is stated
here rather than hidden behind endpoints invented to close it.

**"There is authentication."** There is a token check. It is a string
comparison against a value published in this repository. See
[`03_MIDDLEWARE-PIPELINE.md`](03_MIDDLEWARE-PIPELINE.md), which says so at
greater length.

---

## 5. Why the backend is this small

A reviewer used to production systems will notice what is missing: no database,
no ORM, no real authentication, no repository pattern, no containers, no
versioning. All of that was ruled out before a line was written.

The reason is that this layer is a demonstration of *foundational* backend
understanding, and the fastest way to make such a demonstration untrustworthy is
to fill it with architecture the author cannot account for. A small API that is
completely explainable — where every file has a reason and every shortcut is
labelled — says more than a large one assembled from patterns.

Where the simple choice and the correct-in-production choice disagree, the
documents say so. `DELETE /api/accreditations/{id}` is the clearest example:
the CRUD set calls for it, while this project's own domain says a credential is
withdrawn by *writing a change*, never by erasing history. Both are true, the
endpoint exists, and the tension is written down instead of being resolved
silently in one direction.
