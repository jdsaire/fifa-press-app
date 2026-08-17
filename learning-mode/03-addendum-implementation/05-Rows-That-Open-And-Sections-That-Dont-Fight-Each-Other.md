# 05 — Rows That Open, and Sections That Don't Fight Each Other

## Two Places That Showed Everything At Once

Before this run, every entry in the access record showed its full explanation all the time — what changed, why, and what to do about it, all visible whether you wanted that much detail right now or not. The Help page had the same shape at a larger scale: five stages of the process and three other topics, all expanded, all the way down the page, whether or not the section you actually needed was anywhere near the top.

Both got the same fix, and it's a genuinely small piece of HTML: the `<details>` and `<summary>` elements.

## What `<details>` Actually Buys You

```html
<details>
    <summary>Click to see more</summary>
    <p>The hidden content goes here.</p>
</details>
```

That's a working, clickable, keyboard-operable collapsible section — with *zero* JavaScript. The browser itself knows how to show and hide the content, how to respond to a click or an Enter key on the `<summary>`, and how to tell a screen reader "this is collapsed" or "this is expanded." None of that had to be built.

That last part matters more than it might seem. This app's Help page has to keep working with no network connection at all — it's the page every other broken or refused state in the app points to, on the theory that whoever hits a refusal needs somewhere that still works to find out why. A collapsible section built out of a button, some C# state, and a re-render would still *work* offline, technically — but reaching for a browser feature that was already built for exactly this job, instead of rebuilding it, is both less code and one fewer thing that could go wrong.

## The Rule That Decided What Goes Where

For the access record specifically, the collapsed view still had to say everything that matters *before* anyone opens it — what changed, and when it takes effect. The change list's entire purpose is that you find out about something before you're refused at a barrier; a collapsed row that hid the fact something changed at all would quietly undo that. So the split isn't "important stuff visible, everything else hidden" — it's "the fact of the change, always visible; the explanation of it, one click away."

## Marking the Row You Just Wrote

Submitting a request used to send you back to your record with no way to tell which line, among several, was the one you'd just added. Now the new entry does two things the others don't: it opens automatically instead of arriving collapsed like everything around it, and it animates in — a brief slide-and-fade rather than simply appearing. Neither is decorative for its own sake; together they're the confirmation that submitting a request never had before, standing in for a separate "thanks, we got it" screen that this app deliberately doesn't have.

That animation also respects a person's system setting for reduced motion. If someone has told their operating system they'd rather not see things move around, the row still marks itself as new — the border colour and the fact that it's already open both survive — but the sliding motion itself is skipped entirely.
