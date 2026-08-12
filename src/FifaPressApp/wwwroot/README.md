# wwwroot/

Short for "web root." Everything in here is a **static file** — see [Glossary.md](../../../learning-mode/Glossary.md#wwwroot--static-files) — meaning it's sent to the browser exactly as it is, with no C# processing along the way, unlike everything in `Pages/`, `Components/`, or `Layout/`.

- **`index.html`** — the one real HTML page the browser ever loads for this entire app. It's mostly empty on purpose: an empty `<div id="app">` that Blazor fills in once it starts up, plus the `<script>` tag that loads the Blazor runtime itself.
- **`css/app.css`** — styling that applies across the whole app, as opposed to the per-component styling in `Layout/`'s `.razor.css` files.
- **`favicon.png`**, **`icon-192.png`** — the small icons used for the browser tab and for installing the app to a home screen.
- **`lib/`** — **third-party code, not written for this project.** It holds Bootstrap ([Glossary.md](../../../learning-mode/Glossary.md#bootstrap)), a pre-built CSS toolkit this app uses for its buttons, cards, and layout styling. Nothing in `lib/` was authored as part of building FifaPressApp, and it isn't documented or commented on any further than this paragraph.

This folder is separate from everything else in `src/FifaPressApp/` because its contents aren't compiled or interpreted by .NET at all — they're just files, copied into the finished app as-is, the same way they'd be handled on any plain website.
