# How to Run the FIFA Press App

Three ways to see this app, from easiest to most involved. All three end up showing you the same app.

## Path 1: View It Live — No Setup Required

**https://jdsaire.github.io/fifa-press-app/**

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
   dotnet run --project src/FifaPressApp
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
   dotnet run --project src/FifaPressApp
   ```
5. Open `http://localhost:5126` in any browser.

### What a successful start looks like (Paths 2 and 3)

The terminal prints a few lines ending in something like:

```
Now listening on: http://localhost:5126
Application started. Press Ctrl+C to shut down.
```

That's your cue to open the URL. To stop the app, go back to the terminal and press **Ctrl+C**.

## Running the Tests

The repository's automated test suite lives in [`tests/FifaPressApp.Tests/`](../tests/README.md). With the .NET 10 SDK installed (Path 2 or 3 above), run it from the repository root:

```
dotnet test tests/FifaPressApp.Tests
```

### Troubleshooting

- **"dotnet: command not found"** — the SDK isn't installed, or isn't on your terminal's PATH yet. Close and reopen the terminal after installing, or re-run the `export PATH=...` line from Path 2's step 3.
- **"address already in use" / port 5126 busy** — something else on your machine is already using that port. Close whatever it is, or stop any previous `dotnet run` you left running (Ctrl+C in that terminal).
- **Blank page in the browser** — Blazor WebAssembly caches its files aggressively. Hard-refresh the page (usually Ctrl+Shift+R or Cmd+Shift+R) before assuming something's broken.

## What to Click Once It's Running

However you got here, try this walkthrough — it exercises everything covered in [`learning-mode/`](../learning-mode/):

1. **Start at the landing page** (`/`) — what the app is, that it's a demonstration, and two ways in: sign in, or browse without an account.
2. **Sign in as Amina** — both demo accounts are published on the sign-in screen with their passwords. Notice the field disable and the button say "Signing in…" for a moment; that's a real (simulated) write, not an instant one.
3. **Read her record** (`/record`) — the headline paints instantly, "What changed" lists every entry newest-first, and each one is closed by default. Click one open: the collapsed line already told you *what* changed; opening it gets you *why* and what you can do next.
4. **Sign out, then sign in as Tomás** — the second demo account, published beside the first. His record looks similar on purpose, so the one thing that's actually different — how loudly a conditional change interrupts him versus her — is the thing worth noticing, not six unrelated differences at once.
5. **Switch language** from the sidebar — English, Spanish, Portuguese. Notice the session survives it: you're still signed in, still looking at the same record, just reading it in a different language. Switch the theme too; it's independent of both the language and the session.
6. **Request access to a match** — from `/matches`, open any fixture and request access. You're returned straight to the record rather than to a separate "thanks" screen, and the new entry animates in, already open — that *is* the confirmation.
7. **Sign out and try `/record` directly** — the app asks who you are rather than pretending the page doesn't exist; it says plainly that this is a demonstration, not a security check.
8. **Browse without an account** — Matches and Help stay reachable the whole time you were doing all of the above. Open Help and notice every section starts closed; open a couple, and notice the others stay exactly as they were.
9. **Search a fixture in Spanish or Portuguese** — while the app is in one of those languages, search the match list for the translated round name (e.g. *octavos* for Round of 16 in Spanish). It finds the same fixtures English search would.
10. **Look for a team name on an unplayed fixture** — search for any two teams still to meet later in the schedule. You won't find them paired on a fixture that hasn't kicked off, in any language: the schedule is a record of a completed tournament, and this app is built not to read ahead.
11. **Submit an invalid request form** — leave the name or email blank, or use something that doesn't look like an email, and watch the per-field errors.
12. **Visit a deliberately bad URL** — try an address like `.../events/9999` (a number with no matching match) or something completely made up like `.../nonsense`, and see the two different graceful messages each one produces.
