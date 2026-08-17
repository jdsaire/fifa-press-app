# 01 — A Dark Theme Anchored to Black

## What Changed, in One Sentence

The dark theme's surface and text moved from a soft dark grey (`#121212`/`#e8e8e8`) to solid black and solid white (`#000000`/`#ffffff`), and every colour that depends on that surface — links, buttons, warnings, the sidebar's own text — was recomputed against the new anchor rather than assumed to still work.

## Why "Assumed to Still Work" Wasn't Good Enough

A colour that reads clearly against `#121212` doesn't automatically read clearly against `#000000`. Contrast — how easy text is to read against its background — depends on *both* colours, and moving the background is exactly the kind of change that can quietly break a value that used to be fine. [`02-Two-Themes-and-a-Pile-of-Hex-Codes.md`](../02-access-record-frontend/02-Two-Themes-and-a-Pile-of-Hex-Codes.md) already covered why this app defines two complete palettes instead of inverting one; this run is that same discipline applied a second time, to a palette that already existed.

So every dependent token got recomputed by hand from the actual formula the accessibility standard (WCAG 2.2) specifies, rather than assumed to carry over. Some of the values landed with more headroom above their floor than before — black is a more extreme anchor than dark grey, so a colour tuned against it usually has more contrast to spend.

## The Bug That Made This Worth Writing Down

`app.css` defines the dark palette **twice**: once inside a rule that says "use dark if the operating system prefers it," and once inside a rule that says "use dark because a person explicitly chose it." Two rules, so that a person's own choice always wins over whatever their operating system happens to be set to at that moment.

Two rules holding the same values is also two places that can quietly disagree. If a future edit updates one block and forgets the other, the app would show a different dark theme depending on *how* someone ended up in dark mode — system preference versus an explicit click — which is precisely the kind of small inconsistency that erodes trust in an app whose entire premise is "you can trust what this screen tells you." A test now asserts, directly against the file, that both blocks hold identical values for every colour token they define — not just the ones anyone happened to think to check by eye.

## What Didn't Move

The sidebar's gradient background — the one strip of colour that gives this app a visual identity distinct from generic dark-mode chrome — stayed exactly as it was. It's a small, fixed part of the screen rather than the whole canvas, and the two text colours that sit on top of it were already checked against the gradient's own darkest points, not against the general "surface" colour that just changed. Re-anchoring the surface to black had nothing to do with them.
