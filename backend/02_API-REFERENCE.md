# API reference

Everything the API does, in one table you can use without opening the code.

**Base URL when running locally:** `http://localhost:5226/`
**Authentication:** simulated. See [the note below](#the-token).

---

## Routes

| Verb | Route | What it does | Returns |
|---|---|---|---|
| `GET` | `/` | Names the service and links its OpenAPI document. Open, no token. | `200` |
| `GET` | `/openapi/v1.json` | Machine-readable description of every route. Open, no token. | `200` |
| `GET` | `/api/accreditations` | Every accreditation record. | `200` · `401` |
| `GET` | `/api/accreditations/{credentialId}` | One record. | `200` · `401` · `404` |
| `POST` | `/api/accreditations` | Creates a record. | `201` + `Location` · `400` · `401` · `409` |
| `PUT` | `/api/accreditations/{credentialId}` | Replaces a record. The id in the URL wins over any id in the body. | `200` · `400` · `401` · `404` |
| `DELETE` | `/api/accreditations/{credentialId}` | Removes a record **and its change log**. | `204` · `401` · `404` |
| `GET` | `/api/accreditations/{credentialId}/changes` | The record's change log, newest **effective** date first. | `200` · `401` · `404` |
| `POST` | `/api/accreditations/{credentialId}/changes` | Appends a change, and broadcasts it over SignalR. | `201` · `400` · `401` · `404` · `409` |

In development only, one more route exists so the error handler can be
demonstrated: `GET /api/diagnostics/throw` fails on purpose. It is registered
behind an environment check and is absent from a deployed instance.

### There is no way to edit or delete a change

Deliberately. The change log is append-only, and that is a rule about what an
access record *is*, not a feature nobody got round to.

A correction is a **new** change carrying `supersedesChangeId`, pointing at the
one it replaces. A withdrawal is a **new** change of kind `Withdrawal`. Both
leave the original in place and readable. A record whose history can be
rewritten cannot be trusted to say what happened, and this whole project is
about a record a person can trust.

---

## Shapes

### Accreditation record

```json
{
  "credentialId": "MP-2026-04817",
  "holderName": "Amina Bello",
  "outlet": "The National Daily",
  "trackId": "MemberAssociationQuota",
  "hasNamedContact": false,
  "status": "Approved",
  "validUntil": "2026-07-19T23:59:00Z",
  "zoneAccess": ["Media tribune", "Mixed zone", "Press conference room"],
  "lastSyncedUtc": "2026-07-03T17:15:00Z"
}
```

`trackId` is one of `MemberAssociationQuota`, `RightsHolder`, `Freelance`.
`status` is one of `Pending`, `Approved`, `Refused`, `Withdrawn`.
Enums travel as names, never numbers, so reordering one cannot silently change
what a stored value means.

`lastSyncedUtc` is **set by the server** and ignored if you send it. It is the
server's statement about when it last reconciled with the accreditation system,
and letting a client assert its own data was fresh would break the staleness
indicator the frontend shows.

`POST` and `PUT` accept every field above except `lastSyncedUtc`. On `PUT` the
`credentialId` in the body is ignored — a `PUT` names its target in the URL, and
letting the body rename the record would turn an update into a move.

### Change

```json
{
  "changeId": "ch-001",
  "credentialId": "MP-2026-04817",
  "writtenUtc": "2026-06-05T10:00:00Z",
  "effectiveUtc": "2026-06-11T00:00:00Z",
  "kind": "MatchAccessGranted",
  "whatChanged": { "en": "…", "es": "…", "pt": "…" },
  "reason":      { "en": "…", "es": "…", "pt": "…" },
  "nextStep":    { "en": "…", "es": "…", "pt": "…" },
  "nextStepIsActionable": true,
  "decidedBy": null,
  "supersedesChangeId": null,
  "affectsMatchNumber": 1,
  "dependsOnMatchNumber": null,
  "conditionText": null
}
```

`kind` is one of `MatchAccessGranted`, `MatchAccessRevoked`,
`ZoneAccessNarrowed`, `ZoneAccessWidened`, `ValidityShortened`,
`RequestDecided`, `AdministrativeCorrection`, `Withdrawal`.

**`writtenUtc` and `effectiveUtc` are different things and the difference
matters.** One is when the change was recorded; the other is when it starts to
mattering to the holder. The log is ordered by `effectiveUtc`, so a change
landing on Saturday sits above one written later that lands next month.

**`affectsMatchNumber` and `dependsOnMatchNumber` are also different.** The
first names the match the change *is about*; the second names a match the change
*waits on*. A revocation of Tuesday's access because of Saturday's result sets
both, to two different numbers.

**There is no `urgency` field, and that is on purpose.** How loudly a change
arrives is derived from the change's own facts and the holder's track, at the
moment it is read. Sending it over a network would create a value that could
arrive disagreeing with the facts it came from.

---

## Validation

Rejected requests return `400` with this shape, always:

```json
{
  "error": "Validation failed.",
  "details": {
    "HolderName": ["HolderName is required and cannot be empty."],
    "TrackId": ["'Wizard' is not a recognised TrackId. Expected one of: …"]
  }
}
```

**Every problem is reported at once, not the first one found.** A validator that
stopped early would make you fix one field, resubmit, and discover the next —
which is the same "find out by being refused" pattern this project argues
against, pointed at a developer instead of a journalist.

### The rules

**On a record:** holder name, outlet, track and status are required and cannot
be blank; the track and status must be recognised values; at least one zone is
required and no zone name may be blank; an `Approved` record must say what date
it is valid until, because every validity line the frontend prints reads
"approved until <date>".

**On a change:** a change id and a recognised kind are required.
`whatChanged`, `reason` and `nextStep` are required **in all three languages** —
a half-translated change is refused at write time rather than rendering as a
blank line that only appears once somebody switches language. A `reason` that
merely restates `whatChanged` is refused, in each language separately, because
"your access was revoked" explains nothing the outcome did not already say. A
change whose next step is not actionable must name who decided. A change that
depends on an unplayed fixture must state the condition, or it reads as a
decision already taken.

---

## Errors

Every failure answers with a single `error` key carrying a sentence, so you can
write one parser rather than one per failure mode.

| Status | Body | When |
|---|---|---|
| `400` | `{"error":"Validation failed.","details":{…}}` | The request was understood and refused. |
| `401` | `{"error":"Unauthorized."}` | No token, or the wrong one. |
| `404` | `{"error":"No accreditation record with credential id '…'."}` | No such record. |
| `409` | `{"error":"An accreditation record with credential id '…' already exists."}` | `POST` to an id already in use. |
| `500` | `{"error":"Internal server error."}` | A bug in this server. Never a stack trace, and never the exception message — the operator gets those in the log. |

---

## The token

Send it as a header:

```bash
curl -H "Authorization: Bearer demo-token-2026" http://localhost:5226/api/accreditations
```

Or, where a header is impossible — a browser opening a WebSocket — on the query
string as `?access_token=…`.

**The token is `demo-token-2026`, and printing it here is not a mistake.**

This is not authentication. It is a fixed string compared with `==`, written in
plain text in the API's `appsettings.json`, committed to a public repository,
and shipped to the browser inside the frontend's own configuration. There is no
user, no credential store, no issuer, no signature, no expiry, no revocation.
Anyone reading this file can pass the check.

It exists to demonstrate where such a check belongs in a request pipeline and
what it does to a request that fails it. Treating it as a security boundary
would be a mistake, and describing it as one would be a lie. See
[`03_MIDDLEWARE-PIPELINE.md`](03_MIDDLEWARE-PIPELINE.md).

---

## What the API does not serve

**The match schedule.** Fixtures come from a CSV tracked in the frontend, parsed
in the browser, and the rule that withholds team names from a fixture nobody has
played yet lives there with it. The API was scoped to accreditation records and
their changes, and no fixture endpoints were invented to make the boundary look
tidier. If you are reading the app's screens and wondering which half served
what: the record and its change log came from here; everything about matches did
not.
