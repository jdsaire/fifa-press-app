# Properties/

Holds one file:

- **`launchSettings.json`** — configuration used only when running this app from a development machine (via an IDE or `dotnet run`), never when the app is actually published. It defines things like which local web address and port the app starts on — `http://localhost:5126` for this project — and isn't something the app's own code ever reads at runtime.

This is a standard .NET project convention, not something specific to this app's design: any .NET project built with the usual project templates gets a `Properties/launchSettings.json`, generated automatically when the project is first created. It gets its own folder, separate from everything else here, because it's development-machine configuration rather than part of the app itself — nothing in `Pages/`, `Components/`, `Models/`, or `Services/` depends on it or even knows it exists.

For the exact address and port this file sets up, and how to actually start the app on your own machine, see [`docs/how-to-run.md`](../../../docs/how-to-run.md).
