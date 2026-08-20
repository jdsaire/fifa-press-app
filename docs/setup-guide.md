# Setup Guide (Beginner-Friendly)

You don't need any prior experience with GitHub, Blazor, or .NET to get this running. Follow these steps in order.

This guide covers installing the toolchain on your own machine. If you'd rather see the app without installing anything at all — including a no-install GitHub Codespaces option — see [`how-to-run.md`](how-to-run.md) instead.

## 1. Download the code

You don't need to install Git or know any Git commands.
1. Click the green **Code** button at the top of this repository's GitHub page.
2. Choose **Download ZIP**.
3. Find the downloaded ZIP file (usually in your Downloads folder) and unzip/extract it. You'll get a folder named `fifa-press-app-main`.

## 2. Install the tools you need

This project is written in C# and runs on Blazor WebAssembly, part of Microsoft's **.NET** platform.
1. Install the **.NET SDK** (free): https://dotnet.microsoft.com/download — pick the latest version for your operating system and run the installer. Version 10 or later is required.
2. Install **Visual Studio Code** (free — this is the code editor): https://code.visualstudio.com/
3. Open Visual Studio Code, click the Extensions icon in the left sidebar (four squares), search for **"C# Dev Kit"**, and click **Install**.

## 3. Open the project

1. Open Visual Studio Code.
2. Go to **File → Open Folder…** and select the `fifa-press-app-main` folder you unzipped in Step 1.
3. In the file explorer on the left, expand `src/frontend` to see the project files.

## 4. Run the app

1. In Visual Studio Code: **Terminal → New Terminal**.
2. Type and press Enter:
   ```
   dotnet run --project src/frontend
   ```
3. Wait for a line like `Now listening on: http://localhost:5000` (the exact port may differ), then open that URL in your browser.

You should see the Matches page with a list of mock World Cup matches. See the main [README](../README.md#how-to-use-it) for what each page does.

## 5. Optional: the API

This project also has a server — an ASP.NET Core Web API in `src/backend`. **You do not need it.** Everything in step 4 works without it, because the app reads from an in-memory mock unless you tell it otherwise.

If you want to see it anyway, open a second terminal and run:

```
dotnet run --project src/backend
```

Then visit `http://localhost:5226/openapi/v1.json`, which lists everything the API can do. [`backend/05_RUNNING-AND-DEPLOYING.md`](../backend/05_RUNNING-AND-DEPLOYING.md) explains how to point the app at it, and [`backend/07_BEFORE-AND-AFTER.md`](../backend/07_BEFORE-AND-AFTER.md) explains what that changes.
