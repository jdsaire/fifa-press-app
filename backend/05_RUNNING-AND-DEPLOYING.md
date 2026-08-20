# Running and deploying

## What you need

The **.NET 10 SDK**, and nothing else. No database to install, no Docker, no
Node. [`docs/setup-guide.md`](../docs/setup-guide.md) walks through installing
the SDK from scratch if you have never used .NET.

---

## Running the frontend on its own

This is the default, and it needs no API at all.

```bash
dotnet run --project src/frontend
```

Open the URL it prints. Sign in with `demo_staff1` / `Demo#2026Staff1` (or
`demo_staff2` / `Demo#2026Staff2` for the second holder). The app reads from its
in-memory mock and makes no network call.

**Nothing about this changed when the backend was added.** If you only want to
see the app, you are done.

---

## Running the API

In its own terminal:

```bash
dotnet run --project src/backend
```

It listens on `http://localhost:5226`. Two things to try immediately:

```bash
curl http://localhost:5226/openapi/v1.json
curl -H "Authorization: Bearer demo-token-2026" http://localhost:5226/api/accreditations
```

The first needs no token; the second does. Without it you get `401`.

---

## Running both, with the app reading from the API

Three steps.

**1.** Start the API, as above, and leave it running.

**2.** Create `src/frontend/wwwroot/appsettings.Development.json`:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5226/",
    "HubPath": "hubs/changes",
    "Token": "demo-token-2026"
  }
}
```

This file is **not** in the repository, and that is deliberate. The committed
`appsettings.json` ships with an empty `BaseUrl` so the app's default behaviour
is unchanged for anyone who clones it. Creating this one is how you opt in;
deleting it is how you opt out.

**3.** Start the frontend in another terminal and sign in.

The screens look identical, because they are — see
[`07_BEFORE-AND-AFTER.md`](07_BEFORE-AND-AFTER.md) for how to convince yourself
the data really is coming from the API.

### Watching a change arrive

With both halves running and the record screen open, in a third terminal:

```bash
curl -X POST -H "Authorization: Bearer demo-token-2026" -H "Content-Type: application/json" \
  -d '{"changeId":"ch-demo","kind":"MatchAccessRevoked","effectiveUtc":"2026-07-09T12:00:00Z","whatChanged":{"en":"Mixed zone access withdrawn.","es":"Se retira el acceso a la zona mixta.","pt":"O acesso a zona mista foi retirado."},"reason":{"en":"The host city reduced the allocation.","es":"La ciudad sede redujo la asignacion.","pt":"A cidade-sede reduziu a alocacao."},"nextStep":{"en":"Contact the venue media office.","es":"Contacta la oficina de prensa.","pt":"Contacte o gabinete de imprensa."},"affectsMatchNumber":98}' \
  http://localhost:5226/api/accreditations/MP-2026-04817/changes
```

The row appears on the record screen without you touching the browser.

**Note that the API's data resets when you restart it.** Storage is in memory,
seeded from a file. Restarting gives you the two original records back — which
is convenient when you have been experimenting.

---

## Running the tests

```bash
dotnet test tests/frontend
dotnet test tests/backend
```

512 and 33. Neither needs a server running; the backend suite starts the API
in-process for each test.

---

## Deploying the API to Azure App Service

**These are instructions for the principal, not something this run performed.**
No Azure resource has been created and no credentials exist in this repository.
Until somebody follows these steps, no API is deployed — and the live site is
unaffected either way, because it ships with no API configured.

The reasoning behind choosing App Service free tier is in
[`01_HOSTING-DECISION.md`](01_HOSTING-DECISION.md).

### 1. Sign in and pick a name

```bash
az login
```

Choose a globally unique app name. `fifa-press-api` is used below; substitute
your own if it is taken.

### 2. Create the free-tier resources

```bash
az group create --name fifa-press-rg --location eastus

az appservice plan create \
  --name fifa-press-plan \
  --resource-group fifa-press-rg \
  --sku F1 --is-linux

az webapp create \
  --name fifa-press-api \
  --resource-group fifa-press-rg \
  --plan fifa-press-plan \
  --runtime "DOTNETCORE:10.0"
```

`F1` is the free tier. Confirm the portal shows the plan as **Free F1** before
going further — creating a paid plan by accident is the one expensive mistake
available here.

### 3. Turn on WebSockets

**Do not skip this.** It is off by default, and SignalR will silently fall back
to slower transports without it.

```bash
az webapp config set \
  --name fifa-press-api --resource-group fifa-press-rg \
  --web-sockets-enabled true
```

### 4. Let the deployed frontend call it

The API only accepts browser requests from origins it knows. The GitHub Pages
origin is already in `src/backend/appsettings.json`; if you deploy the frontend
anywhere else, add that origin there and redeploy.

### 5. Publish

```bash
dotnet publish src/backend -c Release -o ./publish-api
cd publish-api && zip -r ../api.zip . && cd ..

az webapp deploy \
  --name fifa-press-api --resource-group fifa-press-rg \
  --src-path api.zip --type zip
```

### 6. Check it

```bash
curl https://fifa-press-api.azurewebsites.net/
curl -H "Authorization: Bearer demo-token-2026" \
  https://fifa-press-api.azurewebsites.net/api/accreditations
```

The **first request after a quiet period will be slow** — several seconds. That
is the free tier starting the process back up, not a fault. See
[`01_HOSTING-DECISION.md`](01_HOSTING-DECISION.md).

### 7. Point the deployed frontend at it

Edit `src/frontend/wwwroot/appsettings.json`, set `BaseUrl` to
`https://fifa-press-api.azurewebsites.net/`, and merge to `main`. The Pages
workflow republishes the site.

**Think about whether you want this.** Once configured, the live site depends on
the API being awake, and a visitor arriving after an idle period waits through a
cold start. Leaving `BaseUrl` empty keeps the deployed demonstration instant and
self-contained, and the API can still be shown by running both halves locally.
Either choice is defensible; the default is the one that cannot break.

### Tearing it down

```bash
az group delete --name fifa-press-rg --yes
```

Removes everything created above.

---

## A reminder about the token

`demo-token-2026` is printed throughout this repository on purpose. It is a
fixed string, not a secret, and the check it satisfies is simulated. Deploying
this API to a public URL does not put anything behind a lock. Do not put
anything in it you would not publish. See
[`03_MIDDLEWARE-PIPELINE.md`](03_MIDDLEWARE-PIPELINE.md).
