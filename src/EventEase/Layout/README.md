# Layout/

Holds the parts of the screen that stay the same no matter which page you're on — the surrounding frame the actual page content appears inside of, rather than the content itself.

- **`MainLayout.razor`** — the overall page frame: a sidebar on one side, the current page's content on the other. Every page in this app (`EventList`, `EventDetails`, `Registration`, and the not-found page) gets displayed inside this same frame.
- **`MainLayout.razor.css`** — styling that applies only to `MainLayout.razor`, and nothing else in the app. This file-pairing pattern (a `.razor` file plus a matching `.razor.css` file with the same name) is called **CSS isolation** — see [`learning-mode/Glossary.md`](../../../learning-mode/Glossary.md#css-isolation) — and it means changing this file's styles can't accidentally affect some unrelated part of the app.
- **`NavMenu.razor`** — the sidebar's navigation itself: the "EventEase" title and the collapsible menu, including the "Home" link back to the events list.
- **`NavMenu.razor.css`** — styling private to `NavMenu.razor` alone, following the same CSS-isolation pattern.

This is a separate folder from `Pages/` because layout and page content answer different questions: `Layout/` answers "what surrounds every page," while `Pages/` answers "what's actually different about this one page." Keeping them apart means changing the sidebar once here updates it everywhere, instead of needing the same edit repeated on every individual page.
