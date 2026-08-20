# `src/interop` — the TypeScript sources for the app's JavaScript interop

This folder holds the TypeScript that compiles to the two modules in
`src/frontend/wwwroot/js/`. It sits **outside** the app project on purpose:
`FifaPressApp.csproj` does not reference it, does not build it, and does not know
it exists.

## The one thing to understand before changing anything here

**The compiled JavaScript is committed, and that is a deliberate choice rather
than an oversight.**

- CI publishes the committed `wwwroot/js/*.js` exactly as it published
  `theme.js` before any of this existed. `.github/workflows/deploy-pages.yml` is
  **unchanged** by this folder's existence.
- A local `dotnet run` needs no Node, no `npm install`, and no build step.
- Someone cloning this repo to read it, or to run it, never touches this folder.

The alternative — compiling in CI — would have added a Node setup step to the
deployment workflow and made the app's build depend on a toolchain it does not
otherwise need, to produce a file that could simply be checked in. The cost of
committing it is that the `.js` and the `.ts` can drift apart if somebody edits
the output directly. That is what the tests in `InteropTests.cs` are for: they
assert that every function the TypeScript exports is present in the compiled
JavaScript, and that the storage keys match.

## How this app is described

**Blazor WebAssembly with a small, type-checked JavaScript interop layer
authored in TypeScript.** Not "built in TypeScript" — the app is C# and Razor,
and this folder is two files.

## Why type-check this seam in particular

An interop boundary is the one place in a Blazor app where the compiler stops
helping. `IJSObjectReference.InvokeAsync<T>` will send whatever it is given and
believe whatever comes back; nothing on either side checks the other. Both
modules here take `string` parameters that .NET could fill with anything, so the
runtime guards stay runtime guards — what TypeScript adds is that the *intent* is
now written down as a type, and the compiler holds the module's own internals to
it.

`tsconfig.json` turns on `strict` plus `noUncheckedIndexedAccess`,
`exactOptionalPropertyTypes` and the unused-symbol checks. Those flags are the
reason to do this at all rather than a default that came with the template.

## Working on it

Node lives wherever you put it; this was developed against Node 24. From this
folder:

```
npm install        # once, and only if you are changing the TypeScript
npm run check      # type-check without emitting
npm run build      # emit to ../FifaPressApp/wwwroot/js/
```

Then **commit the emitted `.js` alongside the `.ts`**. A change to one without
the other is the only way this arrangement goes wrong.

## What is here

| File | Compiles to | What it does |
|---|---|---|
| `src/theme.ts` | `wwwroot/js/theme.js` | Reads and writes the stored theme, and sets the `data-theme` attribute the stylesheet watches. Converted from the original JavaScript with its mechanism unchanged. |
| `src/locale.ts` | `wwwroot/js/locale.js` | Reads and writes the stored language, and sets `document.documentElement.lang`. Written in TypeScript from the start. |

Neither module holds a colour, a translated string, or any application logic.
CSS owns the palette; the per-locale JSON owns the text; these two files own one
browser attribute and one storage key each.
