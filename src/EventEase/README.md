# src/EventEase/

This is the project root — the folder that makes this a real, buildable Blazor app rather than a loose pile of code files. Everything else under `src/EventEase/` (`Components/`, `Layout/`, `Models/`, `Pages/`, `Properties/`, `Services/`, `wwwroot/`) sits inside this one.

Four loose files live directly here, not tucked into a subfolder, because each one applies to the app as a whole rather than to one particular piece of it:

- **`EventEase.csproj`** — the project file. It tells the .NET tools what kind of app this is (a Blazor WebAssembly app), which version of .NET it targets, and which external packages ([Glossary.md](../../learning-mode/Glossary.md#nuget)) it depends on. Nothing here builds or runs without this file.
- **`Program.cs`** — the very first C# code that runs, the moment the app starts. Its main job is registering the app's two shared services, `SessionTracker` and `AttendanceTracker` (see `Services/`), so every page can reach the same ones.
- **`App.razor`** — sets up the router: the piece that watches the browser's current address and decides which page to show, including the fallback for an address that doesn't match anything.
- **`_Imports.razor`** — a shared list of C# `using` statements applied automatically to every `.razor` file in the project, so each individual file doesn't have to repeat the same imports.

For the concepts behind these files — what "Blazor WebAssembly" actually means, what a `.csproj` is for — see [`learning-mode/01-Building-the-Foundation.md`](../../learning-mode/01-Building-the-Foundation.md) and [`learning-mode/Glossary.md`](../../learning-mode/Glossary.md#csproj).
