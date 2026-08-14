# CC-PLAN-v9 — Access Record Frontend Vertical Slice

## Context

`jdsaire/fifa-press-app` currently ships an EventEase-derived event-registration demo: three pages, one reusable card, two in-memory trackers, no domain of its own. A nine-gate UX dossier landed in v8 at `ux-ui/03-ui-prototyping/` specifying a different product — the **Access Record**, a media-accreditation companion that tells a journalist what changed about their tournament access, why, and what they can do, before they are turned away at a barrier.

This run builds that dossier as a frontend vertical slice in Blazor WebAssembly: four entities, one service interface with a mock implementation, six new/modified pages, five new components, a token-based dual-theme CSS overhaul, the EventEase→FIFA Press App rebrand, and a restructured `learning-mode/` series written after the code lands. It is **not** a backend — no API, no database, no auth. That is Run 4C.

The dossier is the spec, not a suggestion. Every entity field (`06_DATA-MODEL.md` §2), every state-matrix row (`05_SCREENS.md`), and every acceptance checkbox (`07_BUILD-BRIEF.md` §3) is a requirement.

---

## Preflight findings (task 0 — complete)

| Check | Result |
|---|---|
| `gh` CLI | v2.96.0 at `~/bin/gh`, authenticated as **jdsaire** (keyring), scopes `gist, read:org, repo, workflow` |
| `fifa-press-app` HEAD | `3e3f001b1eb3ca4a61931ac5679ecfde6210ea62` — **matches `verified_state` exactly**; no drift, no path re-verification needed |
| `frontend_c6_ecommerce` | Reachable at `main`, public; all three cited files read live (`Login.razor` 4647 B, `InputValidationService.cs` 4347 B, `SafeTextAttribute.cs` 1309 B) |
| `2026_World_Cup_Schedule.csv` | Present, 104 rows + header, 8 columns, verified |
| .NET SDK | 10.0.201; project targets `net10.0`, packages 10.0.5 |
| Governing files | All five read in full, in order (07 → 06 → 05 → 03 → 02) |
| Existing source | `EventCard.razor`, `EventList/EventDetails/Registration.razor`, `app.css` (130 lines), `MainLayout`, `NavMenu`, `index.html` read directly |

**Confirmed against the repo, not assumed:** `NavMenu.razor:3` reads `EventEase`; `index.html:7` meta description and `index.html:8` `<title>` both read `EventEase`; `app.css:44` carries the failing `#26b050`; `app.css:56` carries `color-scheme: light only`.

### CSV parse hazards — two beyond what the brief flagged

Verified by scanning all 104 rows:

1. **`24:00` appears in `Time (ET)` on matches 6, 20, 36.** `DateTime.ParseExact` with `HH:mm` rejects this. It means midnight ending that date → must roll to next day `00:00`. **`Time (Local)` never contains `24:00`** — so `KickoffLocal`, the field the withholding rule keys on, parses cleanly. The hazard is confined to `KickoffEastern`.
2. All 104 `Matchup` values split cleanly on ` v `; all rows have exactly 8 fields; no embedded commas. Non-ASCII present (`Türkiye`, `Curaçao`) → read as UTF-8. Source-data inconsistency noted but **not corrected**: `Congo DR` (23, 48, 72) vs `DR Congo` (80).

### Link-integrity baseline

**231/237** internal markdown links resolve across 83 files. Six pre-existing breaks, all inside historical `handoff/` records that this run must not alter (`v2` ×2, `v5` ×2, `v6` ×1, `v8` ×1). Target after this run: **≥231/237**.

The `learning-mode/` move in task 11 puts **43 links at risk**: 17 in `Glossary.md`, 3 in `learning-mode/README.md`, 5 in `src/` READMEs (`../../../learning-mode/0N-...` → needs the new folder segment), and 18 `../`-relative links *inside* the three moving chapters (→ `../../`). References in `07_BUILD-BRIEF.md` and `handoff/v7/CC-PLAN-v7.md` are inline code, not links — they neither count nor need altering.

> **Sequencing trap:** task 9 rewrites the `src/` READMEs while the old learning-mode paths are still correct; task 11 then moves the files. The 5 `src/` README links must therefore be fixed **in task 11's restructure commit**, not task 9's.

---

## Decisions taken (approved before planning)

| Decision | Call | Basis |
|---|---|---|
| **`AsOfUtc`** | `2026-07-03 13:01` — just after Match 88, the last Round-of-32 fixture | Satisfies §3.1 (84 < AsOf < 93). All of R32 resolved, no R16 resolved. Gives §4.2's elimination rule a complete round of resolved rows |
| **Route swap** | EventList `/` → `/matches` moves into **task 6's** commit alongside MyAccess at `/` | Prevents an `AmbiguousMatchException` at `/` for the duration of one commit. Authorized deviation, recorded in the Completion Report |
| **Theme persistence** | Explicit choice → `localStorage`, key `fifa-press-app.theme`, survives sessions | `03_UI-DECISIONS.md` §3.1 / `07_BUILD-BRIEF.md` §6 delegate this to the build. Honours "never silently reverted mid-session" |

### Dossier gaps resolved autonomously (smallest defensible call)

1. **`ConditionText` must not leak the fixture it depends on.** The Foreseeable change carries `DependsOnMatchNumber = 93`, but naming either participant of Match 93 would reveal an unresolved matchup through mock copy — a leak around the provider. **Call:** `ConditionText` identifies the fixture by *phase, venue, city and date only* (all readable for unresolved rows) and states both outcomes in terms of the holder's quota, never team names.
2. **Unresolved fixtures' labels.** `HomeLabel`/`AwayLabel` are `string?`, **null** when unresolved; the provider projects a redacted copy at a single exit point, so no public `Fixture` instance ever carries a withheld value.
3. **`KickoffLocal` vs `AsOfUtc` comparison.** `06_DATA-MODEL.md` §2.1 defines `IsResolved = KickoffLocal <= AsOfUtc` — comparing a local wall clock to a UTC instant. The CSV carries no UTC offsets and `TimeZoneLabel` is itself mocked, so a true conversion is not derivable. **Call:** implement the dossier's literal formula; document the semantic caveat.
4. **`24:00` handling** → parse as next-day `00:00` for `KickoffEastern`.
5. **CSV parse failure** → fail loudly at startup with the offending row number; never silently drop a fixture.
6. **`NotFound.razor`** is frozen per §1.3, so its "Back to events" copy is left as-is and flagged as an inherited inconsistency, not fixed.

---

## Architecture

**Interface-first**, so Run 4C's HTTP provider is a swap, not a rewrite (`06_DATA-MODEL.md` §5.3).

```
Models/          Fixture · Accreditation · Change · Track          (§2.1–2.4)
Services/        IAccessDataProvider          ← written first, before any component
                 FixtureImporter              ← CSV → Fixture[]; a separate concern
                 MockAccessDataProvider       ← owns AsOfUtc + withholding + elimination
Components/      AccessCard · ChangeRow · ForeseeableBadge · StaleIndicator · ThemeTrigger
                 EventCard (reused as MatchCard, read-only presentation unchanged)
Pages/           MyAccess (/) · Help (/help) · SignIn (/signin)
                 EventList (/matches) · EventDetails (/events/{id}) · Registration (/request/{id})
```

**The three invariants this build is judged on:**

- **Withholding lives inside the provider.** No method returns `HomeLabel`/`AwayLabel` for `KickoffLocal > AsOfUtc`. Enforced at one private projection point every read path passes through — never by a caller remembering to check.
- **`Change` fails at construction** without `WhatChanged`, `Reason`, or `NextStep`. A real throw, not a comment.
- **Append-only.** No update, no delete path on `Change` anywhere. Every write returns the resulting `Change`, never `bool`.

**Reuse, not reinvention.** `EventCard.razor`'s established pattern — `X`/`XChanged` parameter pairing, `ReadOnly`-driven default presentation, per-field inline validation via `aria-describedby`, `Guid`-based instance-unique element IDs — is extended by `AccessCard` and `RequestAccessForm`, per `03_UI-DECISIONS.md` §6.

---

## Commit sequence — 11 commits on `deploy/v9-access-record-frontend`

Follows `07_BUILD-BRIEF.md` §2's build order. Author and committer **jdsaire** on every commit; no trailers, no AI reference anywhere in message, branch name, file, or PR.

| # | Commit | Contents |
|---|---|---|
| 1 | `feat(models): add entity model and IAccessDataProvider per dossier §2, §5.1` | `Fixture.cs`, `Accreditation.cs`, `Change.cs` (construction-time validation), `Track.cs`, `IAccessDataProvider.cs` |
| 2 | `feat(services): add FixtureImporter and MockAccessDataProvider with AsOfUtc withholding` | CSV at `wwwroot/data/`, `FixtureImporter.cs`, `MockAccessDataProvider.cs`, `Program.cs` registration. **Withholding verification runs here, not at the end** |
| 3 | `style(tokens): overhaul app.css to token-based dual-theme system per dossier §1–2` | `app.css`, `MainLayout.razor.css`, `NavMenu.razor.css` |
| 4 | `feat(components): add AccessCard, ChangeRow, ForeseeableBadge, StaleIndicator, ThemeTrigger` | Five components + `MainLayout.razor` strip + `RequestAccessForm` extraction |
| 5 | `feat(pages): add MyAccess and Help per dossier §2, §4` | `MyAccess.razor` (`/`), `Help.razor` (`/help`), **EventList route → `/matches`**, NavMenu nav items |
| 6 | `feat(pages): modify EventList and EventDetails per dossier §3, §5.1` | MatchCard reuse, EventDetails §3.1 changes + §3.2 states incl. `GateCheckResult` |
| 7 | `feat(pages): modify Registration and add SignIn per dossier §5.2, §6` | `/request/{id}` rename + copy, `SignIn.razor` |
| 8 | `docs: apply v9 rebrand strings and update src/ READMEs per dossier §4` | `NavMenu.razor:3`, `index.html:7–8`, five `src/` READMEs |
| 9 | `docs(learning-mode): restructure into 01-architecture-foundation` | Three `git mv`s + all 43 at-risk links |
| 10 | `docs(learning-mode): add 02-access-record-frontend chapter documenting the Access Record build` | New chapter + `Glossary.md` extension |
| 11 | `docs: archive v9 plan and completion report` | `handoff/v9/` + `handoff/README.md` row |

`dotnet build` is verified **individually after each of commits 1–8**, not only at the end.

---

## Entity definitions — field-for-field against `06_DATA-MODEL.md` §2

**`Fixture`** (§2.1) — `MatchNumber` int · `KickoffLocal` DateTime · `KickoffEastern` DateTime · `TimeZoneLabel` string *(mocked, city→zone lookup)* · `Phase` enum *(GroupStage + group letter, or knockout round; 18 verified values)* · `HomeLabel`/`AwayLabel` **`string?`, null unless resolved** · `Venue` string · `City` string · `IsResolved` bool *(derived)*.

**`Accreditation`** (§2.2) — `CredentialId` · `HolderName` · `Outlet` · `Track` · `Status` enum `{Pending, Approved, Refused, Withdrawn}` · `ValidUntil` DateTime? · `ZoneAccess` string[] · `LastSyncedUtc` DateTime *(not optional — drives every StaleIndicator)*.

**`Change`** (§2.3) — `ChangeId` · `CredentialId` · `WrittenUtc` · `EffectiveUtc` *(the ordering key, not `WrittenUtc`)* · `Kind` enum *(8 values)* · `Urgency` enum `{Immediate, Foreseeable, Silent}` **derived from `Kind` + `EffectiveUtc` + `Track`, never settable** · `WhatChanged` **required** · `Reason` **required** · `NextStep` **required** · `DecidedBy` *(required when `NextStep` is non-actionable)* · `SupersedesChangeId` string? · `DependsOnMatchNumber` int? · `ConditionText` string? *(required when `DependsOnMatchNumber` is set)*.

Immutable type; the constructor throws `ArgumentException` on any missing required field. No setters, no `Update`, no `Delete`, no `With`-style mutation of the three required fields.

**`Track`** (§2.4) — `TrackId` enum `{MemberAssociationQuota, RightsHolder, Freelance}` · `HasNamedContact` bool · `NotificationCeiling` enum *(derived from `HasNamedContact`; a precondition, not a setting)*.

**`IAccessDataProvider`** (§5.1) — six operations, each carrying its own freshness:

```csharp
Task<AccessResponse<Accreditation>> GetAccreditationAsync(string credentialId);
Task<AccessResponse<IReadOnlyList<Change>>> GetChangesAsync(string credentialId);  // EffectiveUtc desc
Task<AccessResponse<IReadOnlyList<Fixture>>> GetFixturesAsync();                   // labels withheld
Task<AccessResponse<Fixture?>>              GetFixtureAsync(int matchNumber);      // same rule
Task<Change> RequestMatchAccessAsync(string credentialId, int matchNumber);        // never bool
Task<Change> WithdrawRequestAsync(string credentialId, string changeId);           // never bool
```

`AccessResponse<T>` carries `Value`, `LastSyncedUtc`, and `WasServedFromCache` — cache-versus-fetched as a *property of the response*, not two methods (§5.2). `DateTime AsOfUtc { get; }` is exposed on the provider.

---

## Withholding verification (`07_BUILD-BRIEF.md` §3.1) — run at commit 2

Run **before** any component exists, so a failure is cheap. A throwaway console harness in the scratchpad `<Compile Include>`-links the real `Models/` and `Services/` files — **nothing is added to the repo and no NuGet package is introduced.** Assertions:

1. `AsOfUtc` is exposed and equals `2026-07-03 13:01`.
2. `GetFixturesAsync()` returns 104 fixtures; **every** fixture with `KickoffLocal > AsOfUtc` (matches 89–104) has `HomeLabel == null && AwayLabel == null`.
3. `GetFixtureAsync(93)` returns null labels; `GetFixtureAsync(88)` returns `"Australia"` / `"Egypt"`.
4. No public member on any returned `Fixture` exposes the raw `Matchup` string for an unresolved row (reflection sweep over the returned instances).
5. Elimination inference consults only rows with `KickoffLocal <= AsOfUtc`.
6. `new Change(...)` throws when `WhatChanged`, `Reason`, or `NextStep` is null/whitespace — three separate assertions.

Failure here is a stop condition, not a patch-and-continue.

---

## Highest-stakes screen requirements

**My Access `/` — `05_SCREENS.md` §2.2, all eight state rows implemented:** Loading (cached renders immediately, **no blocking spinner**) · Empty–no record (application stage named + Help link) · Empty–record-no-changes (explicit "no changes since [timestamp]", never blank space) · Populated · **Stale (identical layout, `--color-stale-text`, age carried — a stale headline must never render identically to a fresh one; this is the single highest-stakes row in the run)** · Error–cache-present (states the refresh failed and when) · Error–no-cache (routed to Help, never an indefinite spinner) · Contradictory (newer change wins headline, superseded value stays visible per CH-3).

**Match detail** — request count removed; "Registered" → access-status label; timezone label; dependency statement (both outcomes, worded as a condition per CH-7); `GateCheckResult` **as a state of this page, not a route**. The disagreement state shows both sides with a timestamp and an escalation route and **never implies which side is correct**.

**Sign In `/signin`** — adopts exactly §6.2 (on-screen simulation notice visible before interaction, `autocomplete="username"`/`current-password"`, `type="password"`, `EditForm`/`EditContext`/`DataAnnotationsValidator`, `aria-live="polite"` field errors, `role="alert"` form errors, single generic failure message, allow-list on the identifier only). Excludes exactly §6.3 — **no demo credentials, no `AuthenticationStateProvider`, no `AuthorizeView`, no sign-out, no redirect-on-success**, and **not** the reference's blocklist, which rejects `'` and `" or "` and would refuse *O'Neill*. Every route stays reachable without signing in.

**Mocked data is labelled on screen** (`06_DATA-MODEL.md` §6) wherever it could be mistaken for a live integration: `Change` records, `GateCheckResult`, elimination-derived warnings, `TimeZoneLabel`.

**Tokens** — every colour literal replaced; both themes defined together at `:root` and `[data-theme="dark"]`, never light-first-then-inverted; `--color-success: #178040` (the corrected value, **not** the failing `#26b050`); `color-scheme: light only` removed from `#blazor-error-ui`; `--elevation-card` added for `AccessCard`. Skip link and focus-ring treatment preserved.

**ThemeTrigger** — a new slim strip above `main`, right-aligned, at every breakpoint, independent of `.top-row`. Placing it inside the sidebar header was **explicitly rejected** in §3.2 and is not revisited.

---

## Anti-scope-creep (`07_BUILD-BRIEF.md` §5 — binding)

No backend, API, or database. No auth, authorization, or session. **No new NuGet or npm dependency.** Bootstrap stays exactly as-is. `.github/workflows/` untouched. No `ux-ui/` folder modified. `handoff/v1`–`v3` unaltered. No fourth interaction, no bulk/roster surface, no notification-preference screen, no cancel affordance on Request access (its absence is the dossier's deferred decision). **`AttendanceTracker.cs` and `SessionTracker.cs` are not deleted** — only the per-match count display is removed.

---

## learning-mode/ — task 11, last content commits, written only after verification passes

Three `git mv`s into `01-architecture-foundation/` preserving history; `Glossary.md` and `README.md` stay at root with links updated. New chapter `02-access-record-frontend/` covers §4.2's exact subject list — interface-backed data layer, CSS custom properties and why two themes together ≠ one inverted, append-only records, cache-before-network and what that does to a loading state, parsing a real CSV and what breaks. **Describes what was actually built, not what was planned.** Contains nothing from `ux-ui/`: no persona names, no How Might We, no gate or mandate numbering, no research findings. `Glossary.md` gains interface, dependency injection, custom property, append-only, cache — plain-language voice, no UX vocabulary.

---

## Verification

**Per-commit:** `dotnet build` clean, no new warnings, after each of commits 1–8 individually.

**Task 10 — full `07_BUILD-BRIEF.md` §3 checklist, all eight subsections reported item-by-item as literal PASS/FAIL** (§3.1 withholding · §3.2 log · §3.3 staleness · §3.4 theme/a11y · §3.5 rebrand · §3.6 sign in · §3.7 routes · §3.8 build). Not an aggregate pass.

**App run:** `dotnet run`, then load every §3.7 route — `/`, `/matches`, `/events/{id}`, `/request/{id}`, `/help`, `/signin` — and confirm `/register/{id}` no longer resolves anywhere.

**Link sweep:** real resolution (the script already used for the baseline), run **twice** — at task 10 and again after task 11's moves — reported as N/N against the 231/237 baseline.

**Attribution:** `git log --format='%an|%ae|%cn|%ce'` shows only jdsaire; grep the full diff and every new file for tooling vendor and product names, and for the generic assistant/agent terms — zero hits. The one permitted phrase repo-wide is the pre-existing "AI coding assistant" in prior docs, preserved verbatim if a sentence containing it is touched.

**Then:** open the PR against `main` from `deploy/v9-access-record-frontend` and **STOP — do not merge.** Archive plan + Completion Report at `handoff/v9/` with a folder README, add the v9 row to `handoff/README.md`.

---

## Open items carried forward (restated in the Completion Report)

`07_BUILD-BRIEF.md` §6's inherited-unresolved list, carried without resolution: venue access list ownership · the interval premise ID-01 (all threshold values are assumptions) · W5's navigation objection (no bulk surface in v1) · withdrawal specified but with no UI affordance (not a defect) · Request access untested by any Gate 4 task. Plus this run's own: `NotFound.razor` copy inconsistency. **Next run: 4C** — the HTTP provider swap, where `AsOfUtc` is replaced by real time.
