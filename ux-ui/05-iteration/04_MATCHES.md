# v14 (Run 4F) — Gate 4: Matches and the Conditional Record Route

**Status:** Open for approval. Resolves **R10**. Opens and closes **R11** (capacity scope
correction). Two calls were resolved with the principal before drafting — §0.

---

## 0. Resolved before drafting

| Call | Resolution |
|---|---|
| Capacity/sold-out scope | **Unplayed knockout fixtures only (matches 89–104).** Corrects the source resolution, which named the Group Phase — see **R11**. |
| CTAs on already-played fixtures | **Hidden.** A played fixture offers no request path at all. |

---

## R11 — the Group Phase capacity premise was factually impossible

**Opened and closed within this gate**, since the correction was confirmed before drafting.

`MockAccessDataProvider.SimulatedNow` is `2026-07-03 20:31 UTC` (line 75). The group stage runs
**11–27 June 2026**. Every one of the 72 group-stage fixtures is therefore already played at the
app's own "now," and `IsResolved` is true for all of them.

The source resolution asked for sold-out group matches *"so the user can anticipate and plan
accordingly."* Scarcity on a fixture that finished six days ago cannot inform planning — there is
nothing left to plan. The premise was not a preference to honor but a factual impossibility.

**Verified split at `SimulatedNow`:** 88 played (all 72 group + all 16 Round of 32), **16 unplayed**
(Round of 16 ×8, Quarter-finals ×4, Semi-finals ×2, Third Place, Final) = matches **89–104**.

> **R11 — CLOSED.** Capacity applies to the 16 unplayed knockout fixtures. This is strictly better
> for the stated intent *and* for the withholding rule: those 16 are exactly the fixtures carrying
> null team labels, so a capacity value can never sit beside a team name it might correlate with.

---

## 1. Capacity — `[SIMULATED]`, rule-based, deterministic

**Rule** (re-derivable by inspection, no lookup table):

```
Capacity applies only where IsResolved == false.
  Slots remaining = 0                      when MatchNumber % 4 == 0
                  = 3 + (MatchNumber % 9)  otherwise
```

Yielding, across the 16 unplayed fixtures: **sold out** at matches **92, 96, 100, 104** (four of
sixteen — including the Final, which is plausible as the scarcest); every other unplayed fixture
carries between 3 and 11 slots. A reviewer can verify any single value with arithmetic.

**Provenance:** `[SIMULATED]`. No real accreditation capacity exists for these fixtures and none is
implied. The rule must be commented as simulated at its definition site, in the same register
`Fixture.TimeZoneLabel` already uses for its MOCKED note.

**Placement:** a computed property on the provider's fixture read path, *not* a stored field on the
CSV import — capacity is a simulation device the provider owns, exactly as `IsResolved` and
`SimulatedNow` are. This keeps `FixtureImporter` a pure CSV reader.

**Withholding is structurally safe here, not merely checked:** capacity renders only on unplayed
fixtures, and unplayed fixtures carry `HomeLabel`/`AwayLabel` as `null` by construction. There is no
code path on which a capacity number and a team name appear together. The two axes cannot interact.

**Display**, ShopEase `product-card__stock` parity `[VERIFIED]`:
`matches.slotsRemaining` = "{count} slots available" · `matches.soldOut` = "No slots available".

---

## 2. CTA gating — three states, not two

`Pages/EventList.razor` currently renders `View details` **and** `Request access` unconditionally on
every card. Replaced by a three-way branch, adopting ShopEase `ProductCard`'s exact pattern
(auth-unaware card, calling page computes a plain bool and passes it down) `[VERIFIED]`:

| Fixture state | Signed out | Signed in |
|---|---|---|
| **Played** (88) | *no CTA* | *no CTA* |
| **Unplayed, slots available** | `Sign in to request access` → `/record` | `View details` + `Request access` |
| **Unplayed, sold out** | `Sign in to request access` → `/record` | `View details` only, plus the sold-out line |

A played fixture is not requestable in any session state — the request path is hidden, not disabled,
because a disabled control invites the question "why," and there is no answer worth a tooltip: the
match is over.

Sign-in link target is `/record`, not `/signin` — per Gate 3's boundary decision and §5 below.

**`MatchCard` receives plain parameters only**: `CanRequest` (bool), `SlotsRemaining` (int?),
`IsPlayed` (bool). It performs no session lookup and injects no `SimulatedSessionProvider`, matching
ShopEase's stated rule that the gate stays entirely in the calling layer.

---

## 3. `EventCard` → `MatchCard`

`Components/EventCard.razor` is retired from `/matches` and replaced by `Components/MatchCard.razor`,
purpose-built for a fixture: no `EventName`/`Location` parameter names, no inline Edit toggle, no
*"Changes here aren't saved."* This closes the spine complaint at its most literal point — a
World Cup fixture is currently passed into a parameter called `EventName`.

**Where the two-way binding demonstration survives:** `EventCard.razor` is **kept in the repository**,
unreferenced by `/matches` but still rendered by its own tests. It is the app's only demonstration of
the `X`/`XChanged` + `@bind` + `EventCallback` pairing, which is capstone-relevant and must not be
deleted to tidy a page. Its continued existence is the deliberate outcome, not an oversight — record
it as such in the commit message so a future reader doesn't "clean up" an unreferenced component.

`IconTests.CardKeepsItsDateAndLocationTextAlongsideTheIcons` and
`CardEditingBranchIsUntouchedByTheIcons` render `EventCard` directly and therefore **pass
unmodified** — they never went through `EventList`.

---

## 4. Country-name localization

**Root cause, verified:** `FixtureLabels.Display` interpolates `fixture.HomeLabel`/`AwayLabel` — raw
CSV strings — into the `fixture.versus` template (`"{home} v {away}"`). The model's labels are
canonical English by contract (`Fixture`'s own note; the frozen withholding test asserts
`EndsWith("teams not yet decided")`; search tests drive `"Round of 16"` and `"Group D"`).

**The fix is presentation-only.** `FixtureLabels.Display` resolves each label through a locale lookup
before interpolating. The model is not touched.

**Lookup:** a new `team.*` key namespace in the three i18n files, keyed on the canonical English name
(`team.Germany` → "Alemania" / "Alemanha"). 48 keys — see normalization below. English values are
identity mappings, kept explicit rather than special-cased, so `L[which, "team.Germany"]` behaves
identically in all three locales.

**Missing-key degradation:** `LocaleService.Has(which, key)` is checked; on miss, the canonical
English string renders unchanged. Never an empty name, never a raw key. This also means a country
added to the CSV later degrades gracefully instead of breaking the card.

**`Congo DR` / `DR Congo` — normalize at import.** The CSV carries both spellings for one country.
Normalizing in `FixtureImporter` (a single canonical `Congo DR`) is correct because the alternative —
two lookup keys for one nation — would propagate the source defect into all three locale files and
into search. One canonical key, 48 total rather than 49.

**Withholding stays intact:** the lookup is reached only inside the `IsResolved: true, HomeLabel: not
null, AwayLabel: not null` branch of `Display`. An unplayed fixture takes the `fixture.undecided`
path and never reaches a team lookup.

**Search widens, nothing narrows.** `FixtureQuery.Search`'s locale-aware overload already matches
against `FixtureLabels.Display(...)` output — so localizing that output means a Spanish reader
typing *"Alemania"* finds Germany, while *"Germany"* still matches via `MatchesCanonical`, which is
untouched. This is precisely the additive pattern that method's own comment documents as the reason
every frozen search test passes untouched. **`LocalizedSearchTests` stays green without modification.**

---

## 5. R10 — resolved: `/record` renders, no longer redirects

**The conflict**, opened at Gate 3: `MyAccess.OnInitializedAsync` currently *redirects* signed-out
visitors away, documented as *"this moves the person to the screen that explains what signing in here
does and does not mean."*

**Resolution:** the redirect is deleted. `/record` renders `<SignInForm />` (Gate 3's route-less
extraction) inline when signed out, and the record when signed in. The comment's goal — the person
reaches the screen that explains what signing in means — is now met *without navigating*, because
that explanation has come to them. Same destination, one fewer redirect.

> **R10 — CLOSED.** `NavigateTo("signin", replace: true)` removed from `MyAccess.OnInitializedAsync`;
> the existing `.my-access__signed-out` branch is replaced by `<SignInForm />`.

**`/signin` is retired outright**, not kept as a redirect. Two live routes rendering the same form is
the duplication the source prompt explicitly forbids, and no external consumer can hold a stale
bookmark to a portfolio demo. Three call sites retarget to `record`:

| File | Line | Change |
|---|---|---|
| `Pages/Registration.razor` | ~41 | `href="signin"` → `href="record"` |
| `Pages/Registration.razor` | ~107 | `NavigateTo("signin", replace: true)` → `NavigateTo("record", replace: true)` |
| `Pages/EventDetails.razor` | ~73 | `href="signin"` → `href="record"` |

`Pages/MyAccess.razor` line ~51's own `href="signin"` disappears with the branch that contained it.

**The requests dashboard already exists.** Verified: `Registration.razor` line ~154 already calls
`NavigateTo("record")` on successful submit, and `MyAccess` already renders per-match status with the
v13 withdraw affordance. The "sign in → request → it appears in the dashboard" flow is **built**. No
new screen; this gate changes only the signed-out branch.

---

## 6. Cancel-request affordance

Carried from Gate 2 §"in its place." **Decision: it is the same action, surfaced once — not twice.**

The existing withdraw affordance (`WithdrawRequestAsync`, `/record`, gated to `Requested` status,
closed in v13) is already the cancel path, and it lives on the screen that lists what you have
pending. Adding a second entry point on `/matches` would require the fixture card to know the
session's per-match request status — precisely the auth/state awareness ShopEase's `ProductCard`
comment forbids, and which §2 keeps out of `MatchCard` by design.

**What ships instead:** on `/matches`, a signed-in user whose request for a fixture is pending sees
that stated on the card, with the action living where the record does. This is one line of status,
not a control. Concretely: `MatchCard` gains no new awareness — `EventList` computes the pending set
once and passes a bool, identical in shape to `CanRequest`.

`matches.requestPending` = "Request pending — manage it in My Requests" (linking to `/record`).

---

## 7. Pagination

Numbered page buttons are **fully retired**. Replaced by ShopEase's `Showing N of M` +
`Show more` `[VERIFIED]`, `aria-live="polite"` on the count.

**Page size: 12** (ShopEase uses 6 for 12 products; 104 fixtures need a larger increment, and 12 is
one row-pair at the existing `col-md-6` breakpoint). Each press reveals 12 more — the same increment
every time, never a show-all jump. Nine presses reach the full 104, which is the deliberate friction:
the intent is to make filtering more attractive than paging.

`_visibleCount` resets to 12 whenever any filter or the search term changes — otherwise a narrowed
list inherits an expanded count and the control disappears without explanation.

**Retired key:** `matches.pagesLabel`. **New keys:** `matches.showingCount` = "Showing {shown} of
{total} matches" · `matches.showMore` = "Show more".

---

## 8. Availability filter

A fourth control, composing with the existing three via `FixtureQuery.Apply` (AND semantics,
matching how `Search` → `InGroup` → `WithStatus` already nest).

```csharp
public enum SlotAvailabilityFilter { All, WithSlotsAvailable }
```

`Apply` gains a fifth parameter on both overloads. `WithSlots` filters to fixtures where
`IsResolved == false && SlotsRemaining > 0` — a played fixture has no slots concept and is excluded
from this filter's positive case by definition.

New keys: `matches.availabilityLabel` = "Availability" · `matches.allAvailability` = "Any" ·
`matches.withSlots` = "With slots available". Plus `matches.controlWithSlots` = "matches with slots
available only", for the existing `matches.emptyNarrowed` control-list message — which enumerates
active filters and must name this one too, or the empty state will under-report why a list is empty.

---

## 9. Sign-in defect — verification step, no fix specified

Per the standing resolution: source evidence contradicts a blanket "credentials don't work."
`DemoSessionTests` alone exercises the exact published pairs across ~13 assertions, including
whitespace-padded input, and all 421 tests pass at HEAD.

**Required before any fix is written:** render the app and attempt sign-in with both published pairs
exactly as displayed. Report reproduce / no-reproduce.

**Note this gate changes the identifiers** — Gate 3 renamed them to `demo_staff1`/`demo_staff2`. If
the original defect was a copy-paste or input-mode artifact of the `MP-2026-04817` format, the rename
may incidentally resolve it. **Verify against the post-rename build**, and if it no longer
reproduces, say so plainly rather than recording a fix that was never made.

If it *does* reproduce, candidates in likelihood order remain: display/typed character mismatch;
`IdentifierInputMode`'s `@`-detection (untested against non-email identifiers — note `demo_staff1`
contains no `@`, same as before); or live-deploy drift from `main`. Specify no fix until reproduction
identifies which.

---

## 10. Files touched

| File | Action |
|---|---|
| `Components/MatchCard.razor` | **new** — fixture card, ShopEase `ProductCard` contract |
| `Components/EventCard.razor` | **kept, unreferenced by `/matches`** — binding demo preserved |
| `Pages/EventList.razor` | modified — `MatchCard`, three-way CTA, Show-more, availability filter, pending line |
| `Pages/MyAccess.razor` | modified — redirect deleted, `<SignInForm />` inline (**R10**) |
| `Pages/Registration.razor` | modified — 2 × `signin` → `record` |
| `Pages/EventDetails.razor` | modified — 1 × `signin` → `record`; played-fixture CTA hidden |
| `Services/FixtureQuery.cs` | modified — `SlotAvailabilityFilter`, `WithSlots`, `Apply` ×2 |
| `Services/FixtureLabels.cs` | modified — team-name locale lookup in `Display` |
| `Services/FixtureImporter.cs` | modified — `DR Congo` → `Congo DR` normalization |
| `Services/MockAccessDataProvider.cs` | modified — `[SIMULATED]` capacity rule |
| `wwwroot/i18n/{en,es,pt}.json` | modified — 48 `team.*` keys + 8 `matches.*` new, 1 retired |
| `wwwroot/css/app.css` | modified — `.match-card*`, `.matches__show-more` |

---

## 11. Test impact

**Breaks, rewrite required:**

- **`GatingTests.TheRecordIsNotReachableSignedOut_AndOffersSignInInstead`** — asserts
  `navigation.Uri` ends `/signin` and `a[href='signin']` exists. After R10 there is **no navigation
  at all**; rewrite to assert `SignInForm` renders inline and no redirect occurs. The behavior it
  protects (signed-out visitors don't see record content) must still be asserted.
- **`GatingTests.TheRequestFormIsNotReachableSignedOut`** — asserts `EndsWith("/signin")`; retarget
  to `/record`.
- **`GatingTests.HelpIsPublic`** — asserts `Assert.DoesNotContain("signin", page.Markup)`. Still
  passes (Help gains no sign-in link), but the string is now meaningless; update to `"record"` so it
  keeps testing what it means to test.
- **`EventList` pagination tests** — any asserting numbered page buttons. Rewrite for Show-more.
  Confirm the exact set at Gate 5.

**Unaffected — confirmed, not assumed:**

- **`LocalizedSearchTests`** (frozen) — team localization only widens matches; `MatchesCanonical` is
  untouched. §4.
- **`IconTests`** `EventCard`-rendering tests — render the component directly, never via `EventList`. §3.
- **`FixtureQueryStatusTests` / `FixtureQueryGroupTests` / `FixtureQuerySearchTests`** — existing
  `Apply` overloads keep their behavior; the new parameter defaults to `All`. **Add the parameter
  with a default value** so these compile and pass unmodified.
- **`TwoRecordsTests`, `LocalizedChangeTests`** (frozen) — untouched.

**New tests required:**

- `SlotAvailabilityTests` — the capacity rule yields 0 at 92/96/100/104 and 3–11 elsewhere; capacity
  is absent on all 88 played fixtures; **no fixture ever carries both a capacity value and a team name**.
- `MatchCardGatingTests` — three-way CTA matrix (§2); `MatchCard` injects no session provider.
- `TeamLocalizationTests` — `Display` renders "Alemania"/"Alemanha" in es/pt; unknown key falls back
  to English; an unplayed fixture reaches no team lookup.
- `ShowMoreTests` — 12 per press, count line has `aria-live="polite"`, `_visibleCount` resets on
  filter change.
- Extend `GatingTests` for `/record`'s new conditional rendering.

---

## Decisions taken

1. Capacity scoped to the 16 unplayed knockout fixtures (**R11**); group-stage premise was impossible.
2. Deterministic rule: 0 slots when `MatchNumber % 4 == 0`, else `3 + (MatchNumber % 9)`, unplayed only.
3. Played fixtures offer no request CTA in any session state.
4. `MatchCard` replaces `EventCard` on `/matches`; `EventCard` kept for its binding demonstration.
5. Team names localized in presentation only, 48 keys, `DR Congo` normalized at import.
6. `/record` renders inline when signed out (**R10**); `/signin` retired; 3 call sites retargeted.
7. Cancel-request stays a single affordance on `/record`; `/matches` shows status, not a control.
8. Show-more at 12, numbered pages retired, count resets on filter change.
9. Availability filter as a fifth `Apply` parameter with a default, preserving existing tests.

## Reversals

**R10 — CLOSED.** **R11 — OPENED AND CLOSED.** All register entries R6–R11 now resolved.

## Next

Gate 5 — test impact and file manifest: the consolidated cross-gate file list, the full three-locale
i18n table (26 retired from Gate 3 + additions here), every broken test named, and the commit
sequence. Any open call surfaces as a question before that document is written.
