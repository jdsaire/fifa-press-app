# src/FifaPressApp/

This is the project root — the folder that makes this a real, buildable Blazor app rather than a loose pile of code files. Everything else under `src/FifaPressApp/` (`Components/`, `Layout/`, `Models/`, `Pages/`, `Properties/`, `Services/`, `wwwroot/`) sits inside this one.

Four loose files live directly here, not tucked into a subfolder, because each one applies to the app as a whole rather than to one particular piece of it:

- **`FifaPressApp.csproj`** — the project file. It tells the .NET tools what kind of app this is (a Blazor WebAssembly app), which version of .NET it targets, and which external packages ([Glossary.md](../../learning-mode/Glossary.md#nuget)) it depends on. Nothing here builds or runs without this file.
- **`Program.cs`** — the very first C# code that runs, the moment the app starts. Its main job is registering the app's shared services so every page can reach the same ones — including the data provider, which is registered against an interface rather than a class so the implementation behind it can be swapped without touching a single page.
- **`App.razor`** — sets up the router: the piece that watches the browser's current address and decides which page to show, including the fallback for an address that doesn't match anything.
- **`_Imports.razor`** — a shared list of C# `using` statements applied automatically to every `.razor` file in the project, so each individual file doesn't have to repeat the same imports.

## What this app is

A media-accreditation companion: it shows a journalist what their tournament access currently permits, what has changed about it, and why — so a change is something they read rather than something they discover by being turned away at a barrier. `Pages/README.md` walks through the screens, and `Services/README.md` explains the data layer behind them.

The match schedule in `wwwroot/data/` is real published data. Everything else — the credential, the holder, and every logged change — is simulated, and the app says so on screen wherever a reader might otherwise take it for a live connection.

For the concepts behind these files — what "Blazor WebAssembly" actually means, what a `.csproj` is for — see [`learning-mode/01-Building-the-Foundation.md`](../../learning-mode/01-architecture-foundation/01-Building-the-Foundation.md) and [`learning-mode/Glossary.md`](../../learning-mode/Glossary.md#csproj).
