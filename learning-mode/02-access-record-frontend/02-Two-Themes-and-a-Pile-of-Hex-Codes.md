# 02 — Two Themes and a Pile of Hex Codes

## What the Stylesheet Looked Like

[`app.css`](../../src/FifaPressApp/wwwroot/css/app.css) was 130 lines and had eleven colours written directly into it as hex codes — `#1b6ec2` for buttons and the skip link, `#0071c1` for links, `#dc3545` for validation errors, and so on. Each one appeared wherever it was needed, spelled out in full.

That's fine until you want a second theme. Then every one of those eleven values needs a partner, and there's nowhere to put it: a rule that says `background: #1b6ec2` says it always, in every context, with no room for "except in dark mode."

## What a Custom Property Is

A **CSS custom property** is a named value you define once and refer to everywhere else ([Glossary.md](../Glossary.md#custom-property)). You define it with two leading dashes, and read it back with `var()`:

```css
:root {
    --color-action-primary: #1b6ec2;
}

.btn-primary {
    background-color: var(--color-action-primary);
}
```

On its own that's just a name for a number, which doesn't obviously buy anything. What buys something is that the *definition* can be replaced later, in a different context, and every `var()` that reads it picks up the new value automatically. The rules don't change. The value underneath them does.

`:root` means the top of the document, so anything defined there is visible to every rule in the file.

## Why This Is Called a Token

Once the values have names, the names start carrying meaning that the numbers never did. `--color-action-primary` says what the colour is *for*. `#1b6ec2` says what it *is*.

That difference shows up the moment two things share a value by coincidence. The old stylesheet used `#1b6ec2` for the primary button and for the skip link. Are those the same colour because they're both "the main interactive colour", or did they just happen to land on the same blue? With hex codes you can't tell, and changing one means hunting through the file deciding case by case. With names, the file answers the question itself.

## Two Themes Together, Not One Inverted

Here's the part that took the most care.

The tempting way to build a dark theme is to take the light one and flip it: dark background, light text, and lighten each colour a bit. It's fast and it produces the washed-out grey look that retro-fitted dark modes tend to have, usually with one link somewhere that's now unreadable.

The reason is that lightness isn't the only thing that changes. A blue that reads clearly on white doesn't read clearly on near-black — not because it's too dark, but because the *contrast* between it and its background has collapsed, and contrast depends on both values, not one. Flipping the background changes what every foreground colour needs to be, and no single adjustment applied uniformly gets them all right.

So `app.css` defines two complete sets. Neither is derived from the other. Every pair was checked against the contrast threshold that applies to it — a stricter one for text, a looser one for things like focus rings and outlines that aren't text — and the computed ratio is written next to each line in the file.

One inherited value didn't survive that check. The green used for a valid form field measured 2.83 against white, under the 3.0 floor for that kind of indicator. It was already failing before any of this started; tokenising it just meant somebody finally computed it. It's now a darker green that passes, rather than the original value carried forward under a nicer name.

## How the Theme Actually Gets Chosen

Three blocks, in this order:

```css
:root { /* light */ }

@media (prefers-color-scheme: dark) {
    :root:not([data-theme="light"]) { /* dark */ }
}

:root[data-theme="dark"] { /* dark */ }
```

The first is light, the starting point. The second says "if the operating system is set to dark, use dark" — that's `prefers-color-scheme`, a setting the browser already knows and passes through, which is why the app appears in the right theme before any of its own code runs.

The third is an explicit choice. [`ThemeTrigger.razor`](../../src/FifaPressApp/Components/ThemeTrigger.razor) sets a `data-theme` attribute on the page and remembers it, and because that block comes last it wins.

The `:not([data-theme="light"])` on the middle block is small and load-bearing. Without it, someone who deliberately chose light would get flipped to dark the moment their laptop switched at sunset — the system preference would quietly overrule a decision they'd actually made. With it, the system preference stands down as soon as there's a real choice to respect.

The stylesheet decides which palette applies. [`theme.js`](../../src/FifaPressApp/wwwroot/js/theme.js) only sets or clears that one attribute and remembers what was picked; there's no colour value anywhere in it.

## Next

[File 03](03-Records-That-Only-Ever-Grow.md) goes back to the data, and to a class that deliberately has no way to change anything it holds.
