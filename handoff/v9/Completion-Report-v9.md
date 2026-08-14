# Completion Report: v9 Access Record Frontend Vertical Slice

**Commits (in order, on `deploy/v9-access-record-frontend`, after v8's `3e3f001`):**

- `21d793b` — "feat(models): add entity model and IAccessDataProvider per dossier §2, §5.1"
- `d332665` — "feat(services): add FixtureImporter and MockAccessDataProvider with AsOfUtc withholding"
- `e0f4000` — "style(tokens): overhaul app.css to token-based dual-theme system per dossier §1–2"
- `e26c922` — "feat(components): add AccessCard, ChangeRow, ForeseeableBadge, StaleIndicator, ThemeTrigger"
- `da2aac5` — "feat(pages): add MyAccess and Help per dossier §2, §4"
- `239ab63` — "feat(pages): modify EventList and EventDetails per dossier §3, §5.1"
- `ade67a9` — "feat(pages): modify Registration and add SignIn per dossier §5.2, §6"
- `606e74a` — "docs: apply v9 rebrand strings and update src/ READMEs per dossier §4"
- `80f1790` — "docs(learning-mode): restructure into 01-architecture-foundation"
- `50104dd` — "docs(learning-mode): add 02-access-record-frontend chapter documenting the Access Record build"
- (this commit) — "docs: archive v9 plan and completion report"

PR: [#6](https://github.com/jdsaire/fifa-press-app/pull/6), opened against `main`, **left unmerged for review**.

## Outcome

The Access Record now exists as a working frontend vertical slice. Four entities, one service interface with a single in-memory implementation, six pages, six components, and a token-based dual-theme stylesheet replace what was an event-registration demo. No API, no database, no authentication — those remain Run 4C's.

The highest-risk item was the withholding rule, and it was implemented and verified before any component was written, as the build order requires. The published schedule is a record of a completed tournament — every knockout row names two real teams, with no placeholders anywhere in 104 rows — so a naive read gives the app knowledge of results before they happen, which would silently invert the concept's premise. Containment is structural rather than conventional: `FixtureImporter` never attaches team names to a `Fixture` at all, returning them in a separate lookup; `MockAccessDataProvider` keeps that lookup private and attaches names in exactly one method, which refuses to attach them to a fixture whose kickoff is after the simulated instant. A caller cannot bypass the rule by forgetting it, because a caller is never handed anything else.

`Change` is append-only and enforces it by shape: every property is get-only, there is no update or delete method anywhere in the codebase, and the constructor throws without `WhatChanged`, `Reason` or `NextStep`. It additionally rejects a `Reason` that merely restates `WhatChanged`, which is the one rule in the dossier that would otherwise have been a style note nobody enforces. Both write operations return the resulting `Change`; neither returns `bool`.

`app.css` carries zero colour literals outside its token block, with both themes defined together as complete palettes and every computed ratio annotated inline. The inherited success green — 2.83:1 against white, below the 3:1 floor for a non-text indicator, and failing before this run touched it — is corrected to `#178040` rather than carried forward under a variable name. The forced light colour-scheme on `#blazor-error-ui` is removed, not scoped.

Verification was run as a throwaway harness outside the repo that compiles the real source files via `Compile Include`, so no test project and no package dependency entered the repository. It reports **32 passed, 0 failed**.

## Results — `07_BUILD-BRIEF.md` §3, item by item

### §3.1 The withholding rule

| Criterion | Result |
|---|---|
| `MockAccessDataProvider` exposes `AsOfUtc` | **PASS** — `2026-07-03 20:31 UTC` |
| No method returns `HomeLabel`/`AwayLabel` for a fixture whose kickoff is later than `AsOfUtc` | **PASS** — all 16 unresolved fixtures return null on both; all 88 resolved fixtures return both |
| The withholding is enforced inside the provider, not by callers | **PASS** — single `Reveal` method on every read path; the importer never produces a labelled fixture, so no public `Fixture` ever carries a withheld value |
| Elimination is derived only from fixtures at or before `AsOfUtc` | **PASS** — Curaçao derives as eliminated (whole Round of 32 played, absent from it); Spain and England both derive as *not* eliminated, because the Round of 16 is unplayed and the honest answer is "not known" |
| With `AsOfUtc` between Match 84 and Match 93, no call returns team names for Match 93 or later | **PASS** — `GetFixturesAsync` leaks nothing at or beyond 93; `GetFixtureAsync(93)` returns null labels while still returning venue, city and phase; `GetFixtureAsync(84)` returns "Spain"/"Austria" and `(88)` returns "Australia"/"Egypt" |

A reflection sweep additionally confirms no public property on any unresolved `Fixture` equals either withheld name or contains the raw matchup string.

### §3.2 The log

| Criterion | Result |
|---|---|
| `Change` cannot be constructed without `WhatChanged`, `Reason`, `NextStep` | **PASS** — three separate assertions, each throws |
| `Urgency` derived from `Kind` + `EffectiveUtc` + `Track`, never settable | **PASS** — no setter, and not a constructor parameter |
| No update or delete path exists on a `Change` | **PASS** — every property get-only; no `Update`/`Delete`/`Remove`/`Edit` method on the type or elsewhere |
| Every write returns the resulting `Change`, never `bool` | **PASS** — both interface methods return `Task<Change>` |
| Changes render ordered by `EffectiveUtc` descending | **PASS** — verified on the provider's output and applied in `MyAccess.razor` |
| A superseding change displays the value it replaced | **PASS** — `ChangeRow` renders the superseded entry inline; the seeded record contains such a pair |

### §3.3 Staleness and offline

| Criterion | Result |
|---|---|
| Every provider response carries `LastSyncedUtc` | **PASS** — carried on `AccessResponse<T>`, verified on all three read paths |
| `StaleIndicator` renders on My Access always, not only when old | **PASS** — rendered unconditionally; past the threshold it gains weight and a rule rather than appearing for the first time |
| My Access headline renders from cache with no network call | **PASS** — the accreditation and changes reads return already-completed tasks, asserted via `Task.IsCompleted` |
| No spinner blocks the headline | **PASS** — because nothing yields, the data-less first render never occurs; the schedule fetch happens afterwards and only enriches labels already on screen |

### §3.4 Theme and accessibility

| Criterion | Result |
|---|---|
| Both themes defined together as tokens; no colour literal outside the token block | **PASS** — scripted scan of the non-token portion returns zero hex, `rgb(`, `rgba(` or named-colour hits. One exception, stated: the `.blazor-error-boundary` warning icon is an embedded base64 data URI, an image rather than a colour declaration, and is left intact |
| `color-scheme: light only` no longer present | **PASS** — zero occurrences in the file, including in comments |
| Success indicator uses the corrected value | **PASS** — `#178040` light / `#4fd07a` dark; the failing value appears nowhere in the file |
| Theme trigger appears top-right at both breakpoints | **PASS** — `.theme-strip` is defined outside any media query, so it applies at every width; right-aligned via `justify-content: flex-end` in the base rule |
| Focus-ring treatment still present | **PASS** — `h1:focus` outline and the `.btn`/`.form-control:focus` ring both retained, now reading `--color-focus-ring` |
| Skip link still present and functional | **PASS** — markup in `MainLayout.razor`, styles in `app.css`, target `#main-content` unchanged |

### §3.5 Rebrand

| Criterion | Result |
|---|---|
| No occurrence of `EventEase` in `NavMenu.razor`, `index.html` `<title>` or `<meta name="description">` | **PASS** — zero occurrences anywhere under `src/` |
| Replacement strings match `03_UI-DECISIONS.md` §4 exactly | **PASS** — brand and title `FIFA Press App`; meta description verbatim from §4 |

### §3.6 Sign in

| Criterion | Result |
|---|---|
| No credential store, no `AuthenticationStateProvider`, no `AuthorizeView`, no session | **PASS** — zero occurrences of any of them |
| Simulation notice visible on screen before interaction | **PASS** — rendered above the form, first content on the page |
| `autocomplete="username"` and `autocomplete="current-password"` present | **PASS** |
| Password field is `type="password"`; never sanitised, trimmed or pattern-rewritten | **PASS** — the only code touching the value is the two-way binding and its `[Required]`/`[StringLength]` attributes |
| No credential logged, stored or placed in a query string | **PASS** — no logging call, no storage call, no navigation carrying either value |
| Every part of the app reachable without signing in | **PASS** — no route is guarded; the notice links to all three sections explicitly |

The §6.3 exclusions hold: no demo credentials, no sign-out, no redirect-on-success, and the reference's injection blocklist is deliberately **not** adopted — the identifier allow-list permits apostrophes, so *O'Neill* is accepted.

### §3.7 Routes

| Criterion | Result |
|---|---|
| `/` → My Access, `/matches` → Matches, `/events/{id}` → Match detail, `/request/{id}` → Request access, `/help` → Help, `/signin` → Sign in | **PASS** — all six `@page` directives present and distinct; no duplicates |
| No route still resolves to `/register/{id}` | **PASS** — no `@page "/register/…"` directive exists anywhere, and no markup links to that path |

### §3.8 Build

| Criterion | Result |
|---|---|
| `dotnet build` succeeds with no new warnings | **PASS** — 0 warnings, 0 errors, verified individually after **each** of the nine `src/`-touching commits, not only at the end |
| App runs and every route loads | **PASS with a stated limit** — `dotnet publish` succeeds and the app serves; `index.html`, `css/app.css`, `js/theme.js`, `FifaPressApp.styles.css` and `data/2026_World_Cup_Schedule.csv` all return 200, and the isolated CSS for all five new components is present in the generated bundle. **In-browser rendering of each route was not automated**: no browser or Node runtime is installed on the build machine, and a headless-browser or component-test dependency would have violated the no-new-dependency rule. Because this is a single-page app, an HTTP 200 on a route path proves only that the shell was served, not that the route matched — so route correctness is evidenced by the `@page` inventory above rather than by status codes. DI resolution was verified separately under stricter scope validation than Blazor WebAssembly applies by default. |

## Link-integrity sweep

Real resolution, not a regex count, run twice as required.

| Point | Result |
|---|---|
| Pre-run baseline | 231/237 |
| After task 10, before the `learning-mode/` moves | 231/237 |
| **After the `learning-mode/` restructure** | **269/275** |

Net +38 resolving links and **zero new breakages**. The same six failures persist at every stage, all inside historical `handoff/` records this run is forbidden to alter (`v2` ×2, `v5` ×2, `v6` ×1, `v8` ×1).

The restructure put 43 links at risk and all were repaired: 18 depth-sensitive `../` links inside the moved chapters, 18 bare `Glossary.md` links from those chapters, 19 in `Glossary.md`, 3 in `learning-mode/README.md`, and 7 across the five `src/` READMEs. A separate check confirms **56/56 heading anchors resolve**.

One sequencing hazard was identified at plan time and handled: task 9 rewrote the `src/` READMEs while the old learning-mode paths were still correct, so those five links were repaired in the restructure commit rather than the docs commit.

## Authorized deviations from the plan

1. **`AsOfUtc` is `2026-07-03 20:31`, not `13:01`.** The approved plan named "just after Match 88" on the stated intent that all of Round 32 be resolved and no Round of 16 be. Verification caught that these are not the same instant: match numbers run in broadcast order, not clock order, and matches 86 (18:00) and 87 (20:30) kick off *later* on 3 July than match 88 (13:00) does. At 13:01 two Round-of-32 fixtures were still unplayed, which made every elimination in that round underivable — the failing check that surfaced it. The instant moved to one minute after match 87, the genuine last Round-of-32 kickoff. This delivers the approved intent exactly and still satisfies §3.1 literally, since 84 < AsOf < 93.

2. **The route swap moved into commit 5.** Approved in advance. `EventList`'s `@page "/"` → `/matches` landed in the same commit as My Access rather than the next one, so no commit leaves two components claiming `/` and throwing at runtime.

3. **`Change` gained one field beyond `06_DATA-MODEL.md` §2.3: `AffectsMatchNumber`.** The dossier gives `Change` no way to say which match a match-scoped change is *about* — `DependsOnMatchNumber` names a fixture a change *waits on*, which is a different thing, and requiring `ConditionText` alongside it makes it unusable for a granted or revoked entitlement. Without the new field, §2.1's "match access is listed per fixture" and §3.2's per-match cached status are both unimplementable. A revocation of Tuesday's access caused by Saturday's result legitimately sets both, to two different numbers.

4. **`GetAccreditation` returns `Accreditation?`, not `Accreditation`.** §5.1 specifies a non-nullable return, but §2.2's "Empty — no record" state requires distinguishing "no accreditation exists" from "an accreditation exists with nothing in it". Nullable is the smaller change.

5. **`EventCard` gained an `AllowEdit` parameter, defaulting to `true`.** §1.2 requires its read-only presentation be reused unchanged, and it is — the default preserves existing behaviour exactly. The match surfaces pass `false`, because a fixture is not the reader's to edit and an edit box over a fixture with deliberately withheld teams would invite typing in the very names the data layer refuses to hand out.

6. **`RequestAccessForm` uses a plain `<form>`, not `EditForm`.** §6's component table specifies reuse of `EventCard`'s validated-input pattern — handler-based, with inline `role="alert"` errors — which is what was built. An `EditForm` with `DataAnnotationsValidator` over a model carrying no annotations would have validated nothing while looking like validation. `SignIn` does use `EditForm`/`EditContext`/`DataAnnotationsValidator`, as §6.2 requires, because its model does carry annotations.

## Decisions resolved autonomously

1. **Theme persistence: across sessions.** Explicit choice is written to `localStorage` under `fifa-press-app.theme` and survives a reload. `03_UI-DECISIONS.md` §3.1 and §6 both assign this to the build. The mid-session requirement is met structurally: the stylesheet's `prefers-color-scheme` block is guarded with `:not([data-theme="light"])`, so a system flip cannot overturn an explicit choice. Storage failures (private browsing, blocked storage) are caught and degrade to session-only rather than failing to render.

2. **`ConditionText` names no team from the fixture it depends on.** Naming either side of match 93 in authored copy would leak an unresolved matchup around the provider — the rule enforced in code, defeated in prose. The conditional change therefore identifies its fixture by phase, venue, city and date, all of which are legitimately readable for an unplayed row, and states both outcomes in terms of the holder's quota. Asserted in verification.

3. **`NotificationCeiling` has two values.** §2.4 specifies the field and its derivation input but not its value set. `ImmediateOnly` (has a named contact) and `ImmediateAndForeseeable` (does not) map directly onto the three urgency classes, and the ceiling participates in urgency derivation so `Track` is a real input rather than a decorative one.

4. **`Change` gained a `NextStepIsActionable` construction flag.** §2.3 requires `DecidedBy` "when `NextStep` is non-actionable" but gives the type no way to know which case it is in. The flag makes the conditional requirement enforceable at construction instead of aspirational.

5. **`Phase` is an enum plus a separate `GroupLetter`.** §2.1's note requires mapping to "GroupStage + group letter, or a knockout round", which needs two values; twelve enum members for twelve groups would have been the alternative.

6. **`IsResolved` compares `KickoffLocal` to `AsOfUtc` literally, as §2.1 defines it.** This compares a local wall clock against a UTC instant. The CSV carries no UTC offsets and `TimeZoneLabel` is itself mocked, so a true conversion is not derivable from the data. The dossier's formula is implemented as written; the semantic caveat is recorded here.

7. **`24:00` parses as next-day `00:00`.** Three rows (matches 6, 20, 36) record their Eastern kickoff this way, which no strict time parser accepts. It appears only in the Eastern column — `KickoffLocal`, the field the withholding rule keys on, parses cleanly. That is luck rather than design and is worth knowing.

8. **CSV parse failures throw, naming the offending line.** Silently dropping rows would leave every screen above confidently wrong.

9. **`SessionTracker` is no longer written to by the request flow.** §1.4 says it "continues to back the request flow", but §5.1 makes `RequestMatchAccessAsync` the write path and CH-1 forbids a second one. Writing access state to both would be exactly the second write path the concept exists to prevent. Both files are retained, as §1.4 requires; only the per-match count display was removed.

10. **`learning-mode/01-architecture-foundation/` has no README of its own.** Checked at plan time as instructed: `learning-mode/README.md` is the index and now covers both folders, so a per-folder README would duplicate it.

11. **Simulated gate check is triggered by an explicit, labelled button.** `GateCheckResult` is specified as a state of Match detail with no route, which leaves no way to reach it. The venue list is simulated as the same change log folded at a cut-off 24 hours behind the record — which reproduces the actual failure (a gate list stale by the time it matters) rather than inventing a second data source. The screen displays both sides with both timestamps and routes to a human; no copy on it implies which side is correct.

12. **The mock provider's error branches are implemented but not exercised.** `MyAccess` implements all eight §2.2 state rows, including both fetch-failure states. The in-memory provider cannot fail, so those two branches are reachable in code but not demonstrated by the running app. Stated rather than claimed as tested.

13. **`NotFound.razor` left untouched.** Frozen per §1.3. Its "Back to events" copy now points at My Access and reads slightly stale — flagged below rather than silently fixed.

## Open items carried forward

Restating `07_BUILD-BRIEF.md` §6's inherited-unresolved list, none of which this run resolves:

| Item | Status |
|---|---|
| Venue access list ownership | Unresolved upstream. `GateCheckResult` displays and routes only; it never adjudicates, and says so on screen |
| The interval premise (ID-01) | Untested by declared constraint. Every threshold value, including the 72-hour immediate window, remains an assumption |
| W5's navigation objection | Open. No bulk or roster surface exists in v1 |
| Withdrawal | Specified in the model and implemented on the provider (`WithdrawRequestAsync`), with no UI affordance. Not a defect |
| Request access untested | No task exercises the write path; a break in it would not have been predicted |
| Theme persistence across sessions | **Resolved this run** — see "Decisions resolved autonomously" #1 |

New to this run:

- **`NotFound.razor` copy.** Its "Back to events" link now leads to My Access. The file is frozen by §1.3, so the wording was left as-is.
- **The `EventCard` edit path is no longer exercised.** Every current caller passes `AllowEdit="false"`. The component and its two-way binding are unchanged and still work; nothing in the app currently demonstrates them.
- **Route rendering unverified in a browser.** See §3.8 above for what was and was not checked, and why.

**Next: Run 4C** — the backend. It replaces `MockAccessDataProvider` with an implementation that talks to a real service, at which point `AsOfUtc` gives way to the real clock and the withholding rule stops being a containment measure and becomes simply true. The interface, the entities, the ordering rules and the withholding contract are all provider-agnostic and should not need to change.
