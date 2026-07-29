# How to Run EventEase

Three ways to see this app, from easiest to most involved. All three end up showing you the same app.

## Path 1: View It Live — No Setup Required

**https://jdsaire.github.io/frontend_c4_blazor_eventease/**

This is the app itself, hosted for free by GitHub Pages — a way to publish a built app as a public website, automatically updated every time the `main` branch changes. Just open the link in any browser. Nothing to install, nothing to run.

## Path 2: GitHub Codespaces (run it yourself, no local install)

Codespaces gives you a full copy of Visual Studio Code running in your browser, connected to a temporary cloud machine with this repo already checked out — useful if you don't have VS Code installed locally.

1. On the repo's GitHub page, click the green **Code** button, then the **Codespaces** tab, then **Create codespace on main**.
2. Wait for the container to finish building (a minute or two the first time).
3. Once VS Code opens in the browser, open a terminal (**Terminal → New Terminal**) and check what's already installed:
   ```
   dotnet --list-sdks
   ```
   If a `10.0.x` entry appears, skip to step 4. If it doesn't, install it directly in this terminal (this doesn't change anything in the repo — it only affects this temporary Codespace):
   ```
   curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 10.0
   export PATH="$HOME/.dotnet:$PATH"
   ```
4. Run the app:
   ```
   dotnet run --project src/EventEase
   ```
5. A notification should appear offering to open the forwarded port. If it doesn't, open the **Ports** panel (next to the terminal) and look for port `5126`, then click its globe icon to open it in a browser tab. You may need to set that port's visibility to "Public" if you want to share the link with someone else.

Free-tier Codespaces usage is metered by the hour — check your GitHub plan if you're using this regularly.

## Path 3: Local Terminal

For a fuller beginner walkthrough of this same path, including installing an editor, see [`setup-guide.md`](setup-guide.md). The short version:

1. Install the **.NET 10 SDK** (free): https://dotnet.microsoft.com/download — pick your operating system.
2. Confirm it installed:
   ```
   dotnet --version
   ```
   This should print something starting with `10.`.
3. Download this repo (green **Code** button → **Download ZIP**, then unzip — or `git clone` if you have Git installed) and open a terminal inside the unzipped/cloned folder.
4. Run:
   ```
   dotnet run --project src/EventEase
   ```
5. Open `http://localhost:5126` in any browser.

### What a successful start looks like (Paths 2 and 3)

The terminal prints a few lines ending in something like:

```
Now listening on: http://localhost:5126
Application started. Press Ctrl+C to shut down.
```

That's your cue to open the URL. To stop the app, go back to the terminal and press **Ctrl+C**.

### Troubleshooting

- **"dotnet: command not found"** — the SDK isn't installed, or isn't on your terminal's PATH yet. Close and reopen the terminal after installing, or re-run the `export PATH=...` line from Path 2's step 3.
- **"address already in use" / port 5126 busy** — something else on your machine is already using that port. Close whatever it is, or stop any previous `dotnet run` you left running (Ctrl+C in that terminal).
- **Blank page in the browser** — Blazor WebAssembly caches its files aggressively. Hard-refresh the page (usually Ctrl+Shift+R or Cmd+Shift+R) before assuming something's broken.

## What to Click Once It's Running

However you got here, try this walkthrough — it exercises everything covered in [`learning-mode/`](../learning-mode/):

1. **View the event list** — the home page, showing every mock event as a card.
2. **Open a detail page** — click "View Details" on any event.
3. **Register** — click "Register," fill in a name and a real-looking email, submit. Notice the "Registered" badge appear back on the list and detail pages.
4. **Submit an invalid form** — go to another event's registration page and try submitting with the name or email left blank, or an email that doesn't look like one.
5. **Visit a deliberately bad URL** — try an address like `.../events/9999` (a number with no matching event) or something completely made up like `.../nonsense`, and see the two different graceful messages each one produces.
