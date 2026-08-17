# 04 — Why Two Files Got Their Own Language

This app is written in C#. All of it, except two small files. This chapter is about those two files: what they do, why they exist at all inside a C# app, and why they're written in TypeScript rather than in C# or in plain JavaScript. If you've never touched TypeScript before, this is written for you.

## First, What Those Two Files Actually Are

`src/FifaPressApp/wwwroot/js/theme.js` and `locale.js`. Between them, under 150 lines. Here's everything `theme.js` does, in full:

- reads one value out of the browser's local storage (`"light"` or `"dark"`, if anything was ever saved)
- checks whether the operating system is set to a dark appearance
- sets or removes one HTML attribute (`data-theme`) on the page
- saves a value back to local storage

`locale.js` does the same four things, for the app's active language instead of its theme. That's the entire scope. Neither file holds a colour, a translated sentence, or any decision about what the app does — those all live in C# and in the `.json` language files covered in the previous chapter. These two files are plumbing, not logic.

## 1. Why Does an App With No JavaScript in It Need These Two Files?

Blazor WebAssembly runs your C# code *inside the browser*, using a technology called **WebAssembly** — see [Glossary.md](../Glossary.md#webassembly-wasm) — which lets compiled code run at near-native speed without ever leaving the page. That's genuinely remarkable, and it's why this whole app can be C# instead of JavaScript.

But WebAssembly code doesn't get to reach out and touch the browser directly. The browser's own features — reading and writing local storage, checking a system appearance setting, changing an attribute on a live HTML element — are all things the *browser's JavaScript engine* exposes, because JavaScript is the language browsers have always spoken natively. C# running as WebAssembly has to ask the browser to do those things on its behalf, through a bridge Blazor provides called **JavaScript interop** — literally, C# calling out to a small piece of JavaScript, and getting an answer back.

So the two files aren't "parts of the app that happen to be in JavaScript." They're the *only* two places this app needs to reach past WebAssembly's boundary and touch something that only the browser's own JavaScript engine can touch directly. Everything else — every page, every button, every decision about what to show — never needs to leave C# at all, and doesn't.

## 2. So Why TypeScript, and Not Just Plain JavaScript?

Once you've accepted that *some* JavaScript is unavoidable, the next question is what to write it in. Plain JavaScript would have worked — the browser only ever runs plain JavaScript in the end, regardless of what you started with.

TypeScript is a language that looks almost exactly like JavaScript, with one addition: you can say what *kind* of value something is supposed to be, and a tool checks that you're telling the truth *before* the code ever runs. Take this real example from `theme.ts`:

```typescript
export type Theme = 'light' | 'dark';

export function applyTheme(theme: string | null): void {
    // ...
}
```

That first line says: a `Theme` is only ever exactly the text `'light'` or exactly the text `'dark'` — nothing else is allowed to claim that type. Anywhere else in this same file that a value is declared as a `Theme`, TypeScript's checker will refuse to compile the code if you ever try to hand it `'purple'`, or a number, or a value that came from somewhere the checker can't yet prove is safe. It catches that mistake sitting still in your editor, before you've ever run the app — not three clicks into testing, when the theme silently fails to apply and you're left wondering why.

This matters more than it might sound like for a six-line function, because of *where* this code lives: right at the edge where C# hands control over to JavaScript. That edge is exactly the place nothing checks anything by default — C#'s own strict typing has already been left behind by the time execution reaches this file, and plain JavaScript checks nothing at all, ever. TypeScript's checking is what closes that gap on the JavaScript side, so the one seam in the whole app where type safety could have quietly disappeared doesn't.

Here's the part that makes this cost nothing to ship: **TypeScript disappears completely once compiled.** The command that turns `theme.ts` into `theme.js` (`tsc`, TypeScript's own compiler) strips every type annotation out and leaves behind exactly the JavaScript the browser was always going to run anyway — you can see it yourself; `theme.js` in this repo is a completely ordinary, ungarnished JavaScript file. Nobody visiting this app downloads TypeScript, needs a browser that understands TypeScript, or pays any performance cost for it having existed. The safety is entirely a *build-time* thing.

## 3. TypeScript vs. Plain JavaScript, and vs. C#

**Against plain JavaScript** — the comparison that actually applies here — the benefit is exactly what the example above shows: mistakes get caught while you're writing the code, instead of while a user is running it. A renamed function, a value passed in the wrong shape, a `null` that was never handled — TypeScript's checker flags all of these as compile errors. Plain JavaScript would accept every one of them silently and only reveal the problem later, at runtime, in whatever way happens to break first.

**Against C#**, this isn't really a competition — it's not "TypeScript is better than C#," it's that they're solving two different problems in two different places. C# is the strongly-typed, compiled language for essentially this entire application: every page, every rule, every piece of business logic. TypeScript is used for exactly the sliver of code that has to be literal, native JavaScript to run in the browser at all — code C# structurally cannot reach past its own WebAssembly boundary to write. TypeScript's whole job here is bringing the same category of safety C# already has everywhere else into that one small area where C# can't go.

## What This Cost, in Practice

A `package.json` and a `tsconfig.json` inside `src/interop/` — outside the actual app project entirely, so `FifaPressApp.csproj` has no idea any of this exists. The compiled `.js` files are committed straight into the repository alongside their `.ts` sources, which means nobody building, running, or deploying this app ever needs Node.js or TypeScript installed — those tools are only needed by whoever next *edits* one of these two files. See [`src/interop/README.md`](../../src/interop/README.md) for exactly how that's wired up, and how a test suite guards against the compiled `.js` ever silently drifting out of sync with the `.ts` it's supposed to match.
