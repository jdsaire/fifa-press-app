# v14 (Run 4F) — Gate 3: Home, Sign In, and the Demo Accounts

**Status:** Open for approval. Resolves **R6** and **R8**. Opens **R10** (identified this gate,
resolved at Gate 4 — not decided here). Both R6 and R8's open calls were resolved with the
principal before drafting; recapped in §0, not re-opened.

---

## 0. Resolved before drafting

| Call | Resolution |
|---|---|
| R8 — demo account naming | **Full rename.** `Identifier` becomes `demo_staff1`/`demo_staff2`. `CredentialId` is untouched — it remains `MP-2026-04817`/`RH-2026-00219` and continues to key all stored data. Confirmed after surfacing the true cost: ~30 call sites across 15 test files pass the identifier as `SignInAsync`'s first argument and need a mechanical sweep (§6). Accepted knowingly. |

---

## 1. R6 — resolved: the notice is condensed, not deleted

**The conflict**, restated from Gate 0: the four `signin__notice` paragraphs are sourced from a
governing document (10 §2.3) and deliberately ordered — not arbitrary copy, so wholesale deletion
was never on the table. The tension was length and prominence, not existence.

**Resolution: one condensed paragraph, ShopEase-register, that still states all four facts.**
Critically, `SignInScreenTests.TheNoticeComesFirstAndStatesAllFourThings` asserts five *exact*
substrings against the notice text (case-insensitive `Contains`). The condensed copy is engineered
to contain all five verbatim, so this test **passes unmodified** — a rare case where compression
and full backward compatibility aren't in tension:

```
This is a simulated sign-in. There is no account system behind it — the credentials
below are fake, but they work. Refreshing the page signs you out, since the session
lives only in this tab. It is not a security boundary and it protects nothing.
```

Required substrings present: *"simulated sign-in"* · *"no account system"* · *"they work"* ·
*"Refreshing the page signs you out"* · *"not a security boundary"*. All five intact, verbatim.

**Structure:** one `<p class="signin__notice" role="note">`, matching Landing's disclosure
(`Strong`/`Body` two-key pattern, the app's own established convention for this exact kind of
notice) rather than inventing a third format:

- `signIn.noticeStrong` = "This is a simulated sign-in."
- `signIn.noticeBody` = the remainder, above.

`TheNoticeNoLongerMakesTheTwoClaimsThatBecameFalse` also passes unmodified — the condensed copy
never approaches the two retired v9 phrases. `TheNoticeIsBeforeTheFormInTheMarkup` is a structural
assertion, unaffected by content, and carries forward once the notice relocates with the form (§4).

> **R6 — CLOSED.** Ten `signIn.notice*` keys retire; two replace them. Content preserved in
> substance and, where tested, verbatim.

---

## 2. Home (`Pages/Landing.razor`) — reduced to the ShopEase register

**Kept, unconditionally** (per Gate 0's non-removable findings): the `justSignedOut` `role="status"`
announcement, the signed-in redirect to `/record`, and the disclosure (`landing__disclosure`,
`Strong`/`Body`, unchanged content) — `LandingTests.TheLandingSaysItIsADemonstrationAndNotAFifaProduct`
passes unmodified.

**Removed**, because Landing sheds the sign-in role entirely (Gate 3 boundary, established when the
original prompt was authored and reaffirmed by Gate 4's already-resolved MyAccess conditionality):

- The two-entry section (`landing__entry` × 2: Sign In heading/body/CTA, Browse heading/body/links)
- The "what a signed-in record shows" three-point list

**Replacing both:** a single primary CTA, matching ShopEase `Home.razor`'s eleven-line register
exactly — heading, lede, one `btn btn-primary` link. Destination is `/matches` (browse-first,
exactly parallel to ShopEase's CTA into `/products`), copy is the annotation's own suggested text:

```razor
<a class="btn btn-primary landing__cta" href="matches">@L[Locale, "landing.cta"]</a>
```

New key: `landing.cta` = "Book match facility" (es/pt `[ASSUMPTION]`, Gate 5 review).

`landing.title` and `landing.lede` are **retained, values unchanged for now** —
`TheLandingSaysWhatTheAppIs` asserts "2026 World Cup" and "accreditation" appear in the markup;
both terms are already in the current values, so this passes unmodified as long as final copy
polish (a Gate 5/implementation-time task, not this dossier's) preserves them.

**Retired**, 13 keys, all three locales: `landing.signInHeading`, `landing.signInBody`,
`landing.signInCta`, `landing.browseHeading`, `landing.browseBody`, `landing.browseMatches`,
`landing.browseHelp`, `landing.whatYouGetHeading`, `landing.point1Strong`, `landing.point1Body`,
`landing.point2Strong`, `landing.point2Body`, `landing.point3Strong`, `landing.point3Body`.

---

## 3. Demo accounts — R8 resolved, ShopEase shape adopted

**Identifier rename**, `Services/DemoAccountStore.cs`:

| | Old `Identifier` | New `Identifier` | `CredentialId` (unchanged) |
|---|---|---|---|
| Amina | `MP-2026-04817` | `demo_staff1` | `MP-2026-04817` |
| Tomás | `RH-2026-00219` | `demo_staff2` | `RH-2026-00219` |

Passwords (`amina-demo-2026`, `tomas-demo-2026`) are **unchanged** — not part of the ask. No change
to `Match()`, `PermissiveIdentifierAttribute`'s regex (`demo_staff1` satisfies
`^[\p{L}\p{N} .\-'_@]+$` — letters, digits, underscore all allowed), or length bounds (3–120).
`IdentifierInputMode`'s `@`-detection is unaffected either way — neither the old nor new identifier
contains `@`.

**Confirmed zero-risk to data-keying:** `MockAccessDataProvider.AminaCredentialId`/`TomasCredentialId`
are independent constants (`"MP-2026-04817"`/`"RH-2026-00219"`), never derived from
`DemoAccountStore.Identifier`. `Session.CredentialId` reads `Current.CredentialId`, never
`Current.Identifier`. The rename touches exactly one file's data (`DemoAccountStore.cs`) plus the
test sweep in §6 — the record-keying architecture is provably untouched.

> **R8 — CLOSED.** `Identifier` renamed; `CredentialId` — and everything it keys — is unchanged.

**Container shape**, inherited from ShopEase `Login.razor`'s `auth-demo-accounts` block
`[VERIFIED]`, adapted to keep `HolderName` (closed as intentional at 4D-I, not touched here):

```razor
<div class="signin__accounts" aria-label="@L[Locale, "signIn.accountsHeading"]">
    <h2>@L[Locale, "signIn.accountsHeading"]</h2>
    <p>@L[Locale, "signIn.accountsIntro"]</p>
    <ul class="signin__account-list">
        @foreach (var account in Accounts.Published)
        {
            <li class="signin__account" @key="account.Identifier">
                <strong>@account.HolderName</strong> —
                <code>@account.Identifier</code> / <code>@account.Password</code>
            </li>
        }
    </ul>
</div>
```

**Dropped:** the `<dl>` identifier/password label pairing (folded into the inline `<code>`/`<code>`
pair, ShopEase's exact shape), and the per-account `<p>` biography (`accountAmina`/`accountTomas`
descriptions) and the cross-account comparison hint (`accountsHint`) — all three retired per the
already-established Gate 3 boundary (drop persona bios, keep the name).

`signIn.accountsHeading`'s **value** simplifies toward ShopEase's plain "Demo Accounts" (currently
"The two demo accounts") — value change, key retained. New key `signIn.accountsIntro` (short,
ShopEase-parity: *"These are published, fake demo credentials — not real accounts, and nothing to
keep secret. Sign in with either:"*) replaces the structural slot `accountsHint` occupied, with
materially different, much shorter content.

**Retired**, 3 keys, all three locales: `signIn.accountAmina`, `signIn.accountTomas`,
`signIn.accountsHint`.

`signIn.identifierLabel` ("Email or credential number") and `signIn.identifierRequired` ("Enter your
email or credential number") **need value updates** — "credential number" no longer describes what
`demo_staff1` looks like. Key retained; suggested values: `signIn.identifierLabel` = "Email or
username", `signIn.identifierRequired` = "Enter your email or username." `[ASSUMPTION]`, Gate 5
review, all three locales.

---

## 4. `SignInForm` extraction — Gate 3 stops here; embedding is Gate 4's

`Pages/SignIn.razor` is extracted into a **route-less** `Components/SignInForm.razor`: the
`@page "/signin"` directive is removed, the file moves from `Pages/` to `Components/`, and its
content is the trimmed notice (§1) + form + demo accounts (§3) — everything else in the file
(`EditForm`/`EditContext`, autocomplete attributes, `PermissiveIdentifierAttribute`, the generic
failure message) carries forward untouched, per the file's own "CARRIED FORWARD FROM v9,
UNCHANGED" doctrine.

**This gate does not embed `SignInForm` into `/record`, and does not retire the standalone
`/signin` route's callers.** That is explicitly Gate 4's scope per the source prompt (*"`/record`
becomes a single conditional route... Signed out: renders the Sign In experience"*), and one
finding makes clear it is more than a relocation:

> **R10 — opened, resolves at Gate 4.** `Pages/MyAccess.razor`'s `OnInitializedAsync` currently
> **redirects** rather than renders:
> ```
> if (Session.CredentialId is not string credentialId)
> {
>     Navigation.NavigateTo("signin", replace: true);
>     return;
> }
> ```
> documented as: *"Signed out, there is no record to read and nothing below applies... this moves
> the person to the screen that explains what signing in here does."* Gate 4's conditional-MyAccess
> decision requires this redirect become an inline render of `SignInForm` instead — a behavior
> change, not a move, and it directly falsifies this comment's premise. `GatingTests
> .TheRecordIsNotReachableSignedOut_AndOffersSignInInstead` currently asserts the *redirect*
> happens (`navigation.Uri` ends with `/signin`) and needs a full rewrite at Gate 4, not an
> adjustment — the new behavior has no navigation to assert at all.

**Also deferred to Gate 4, flagged now so it isn't a surprise:** `Registration.razor` and
`EventDetails.razor` each hold one `href="signin"` and Registration also holds one
`NavigateTo("signin", replace: true)` — all three need retargeting once Gate 4 decides `/record`
absorbs `/signin`'s content. Not touched in this gate.

---

## 5. i18n — consolidated key changes, this gate only

**Retired** (16 keys total): 10 `signIn.notice*` (§1) + 13 `landing.*` (§2) − wait, recount: 10 +
13 + 3 (`signIn.accountAmina`/`accountTomas`/`accountsHint`, §3) = **26 keys retired**, all three
locales.

**New** (4 keys): `signIn.noticeStrong`, `signIn.noticeBody`, `landing.cta`, `signIn.accountsIntro`.

**Value-only changes** (keys retained, content updated — flag for Gate 5, not counted as
retire/add): `signIn.accountsHeading`, `signIn.identifierLabel`, `signIn.identifierRequired`.

A key removed from `en.json` without the matching removal in `es.json`/`pt.json` is a defect per
standing doctrine — Gate 5's file manifest carries the full three-locale table.

---

## 6. Test impact

**Breaks outright, full rewrite:**

- **`LandingTests`** — `TheLandingOffersBothEntryPoints` (asserts exactly 2 `.landing__entry`
  divs and three specific hrefs, none of which exist post-reduction) and
  `TheLandingSaysDemoAccountsExistAndWhereTheyAre` (asserts `.landing__entry` text, a div that no
  longer exists). Both rewritten or retired outright — Landing no longer advertises demo-account
  location itself; the top bar's persistent "Sign in" link (Gate 1) is now that pointer.
- **`SignInScreenTests.BothAccountsArePublishedWithIdentifierPasswordAndWhatDiffers`** — the
  `DescriptionKey` assertion (line ~95) targets retired bio keys; rewrite to check `HolderName` +
  `Identifier` + `Password` only, dropping the "what differs" check.
- **The identifier-literal sweep** — every `SignInAsync`/`Match` call passing `"MP-2026-04817"` or
  `"RH-2026-00219"` as the identifier argument (not as `credentialId:`) needs its literal updated to
  `"demo_staff1"`/`"demo_staff2"`. Confirmed by file, with the non-identifier occurrences in the same
  file called out so the sweep doesn't touch them by mistake:

  | File | Identifier-argument lines to change | Other same-file literals — leave unchanged |
  |---|---|---|
  | `SignInScreenTests.cs` | 1 call | — |
  | `DemoSessionTests.cs` | ~13 calls/assertions | — |
  | `LocalizedDateTests.cs` | 3 calls | — |
  | `GatingTests.cs` | 2 calls (helper methods) | 2 markup assertions (`Assert.Contains("MP-2026-04817", page.Markup)`) check the displayed `CredentialId` in the top bar — **unchanged** |
  | `LandingTests.cs` | 1 call | — |
  | `SignOutTests.cs` | 2 calls | 1 assertion checks the top-bar indicator's displayed `CredentialId` — **unchanged** |
  | `MatchAccessLineTests.cs` | 1 call | — |
  | `LanguageSwitchTests.cs` | 6 calls | 1 assertion checks `Session.CredentialId` — **unchanged** |
  | `MatchAccessStatusTests.cs` | 2 calls | — |
  | `GateCheckStatusTests.cs` | 1 call | — |
  | `WithdrawalAffordanceTests.cs` | 1 call | — |
  | `DisclosureTests.cs` | 3 calls | 2 occurrences are `credentialId:` record fields — **unchanged** |

  **Confirmed unaffected — `CredentialId`-only literals, no sweep needed:** `TwoRecordsTests.cs`,
  `LocalizedChangeTests.cs`, `ChangeArrivalAnnouncementTests.cs` — each uses `"MP-2026-04817"` solely
  as `credentialId:` in a data-record constructor, never as a sign-in argument.

**Relocates at Gate 4, not rewritten here:** `SignInScreenTests` as a suite renders `<SignIn>`
directly (a page that no longer exists as a route after §4); re-targeting at wherever Gate 4 embeds
`SignInForm` is Gate 4's job. `GatingTests.TheRecordIsNotReachableSignedOut_AndOffersSignInInstead`
— see **R10** above.

**Unaffected, confirm at Gate 5:** `LandingTests.TheLandingSaysWhatTheAppIs`,
`TheLandingSaysItIsADemonstrationAndNotAFifaProduct`, `TheLandingIsNotAMarketingPage`;
`SignInScreenTests.TheNoticeComesFirstAndStatesAllFourThings`,
`TheNoticeNoLongerMakesTheTwoClaimsThatBecameFalse`, `TheNoticeIsBeforeTheFormInTheMarkup`,
`ThePublishedCredentialsAreTheOnesThatActuallyWork` (reads `account.Identifier` live — passes
automatically against the new values), the autocomplete/disabled/validation-announcement tests.

**Frozen-by-precedent, untouched:** `TwoRecordsTests`, `LocalizedChangeTests`, `LocalizedSearchTests`.

---

## 7. Files touched

| File | Action |
|---|---|
| `Pages/Landing.razor` | modified — reduced to heading/lede/CTA/disclosure/announcement |
| `Pages/SignIn.razor` | **deleted** — content moves to `Components/SignInForm.razor` |
| `Components/SignInForm.razor` | **new** — route-less; notice condensed, accounts reshaped, identifiers renamed |
| `Services/DemoAccountStore.cs` | modified — `Identifier` values only |
| `wwwroot/i18n/{en,es,pt}.json` | modified — 26 keys retired, 4 added, 3 values updated |
| 15 test files (§6 table) | modified — identifier-literal sweep |
| `LandingTests.cs`, `SignInScreenTests.cs` | modified — assertions rewritten per §6 |

---

## Decisions taken

1. Notice condensed to two keys, engineered to preserve five test-asserted substrings verbatim.
2. Landing reduced to ShopEase's heading/lede/CTA register; 13 keys retired.
3. Demo identifier renamed (`demo_staff1`/`demo_staff2`); `CredentialId` and all data-keying
   provably untouched.
4. Demo accounts container adopts ShopEase's shape; bios and comparison hint dropped, `HolderName`
   kept.
5. `SignIn.razor` → route-less `SignInForm.razor`; embedding and route retirement deferred to
   Gate 4 by design, not oversight.

## Reversals

**R6 — CLOSED.** **R8 — CLOSED.** **R10 — OPENED**, resolves at Gate 4.

## Next

Gate 4 — Matches. Resolves **R10**, plus country-name localization, sign-in gating on the fixture
card, the `EventCard`→`MatchCard` retirement, the rule-based capacity/sold-out state, pagination,
and the full `/record` conditional-route assembly. Per your standing instruction, any open call
inside Gate 4 surfaces as a question here before that document is written.
