namespace FifaPressApp.Api.Models;

/// <summary>
/// The three accreditation tracks. Mirrors the frontend's <c>TrackId</c>.
/// </summary>
public enum TrackId
{
    MemberAssociationQuota,
    RightsHolder,
    Freelance,
}

/// <summary>
/// Where a standing accreditation sits. Withdrawn is a state the record can
/// reach, and it is reached by writing a change — never by deleting anything.
/// </summary>
public enum AccreditationStatus
{
    Pending,
    Approved,
    Refused,
    Withdrawn,
}

/// <summary>
/// The standing credential: who the holder is and what it permits, as opposed
/// to access to any one match.
///
/// <para>
/// <b>A mirror, not a shared type.</b> The Blazor frontend defines its own
/// <c>Accreditation</c> with these same fields, and the two are deliberately
/// not the same class. There is no project reference between the frontend and
/// this API in either direction. That independence is the whole point of the
/// exercise: the frontend talks to an interface, and swapping the thing behind
/// that interface for this API is a swap rather than a rewrite. Sharing a
/// types assembly would make the two halves one program that happens to be
/// split across a network, which proves nothing.
/// </para>
///
/// <para>
/// <b>Notice what is absent.</b> The frontend derives a change's urgency from
/// the holder's track and never stores it. This API does not transmit urgency
/// either, for the same reason: a value that travels over a network is a value
/// that can arrive disagreeing with the facts it was derived from. The track
/// and its named-contact flag are sent; the conclusion is drawn at the far end.
/// </para>
///
/// <para>
/// MOCKED. In a real deployment the holder, outlet, track and zone list all
/// come from an accreditation system. There is no credential store here.
/// </para>
/// </summary>
public sealed record AccreditationRecord
{
    /// <summary>The holder-facing identifier, and the key every read is made against.</summary>
    public required string CredentialId { get; init; }

    public required string HolderName { get; init; }

    public required string Outlet { get; init; }

    public required TrackId TrackId { get; init; }

    /// <summary>
    /// Whether the holder has a named human contact. Sent rather than the
    /// notification ceiling it implies, so the far end derives the ceiling
    /// instead of trusting a number that could have gone stale in transit.
    /// </summary>
    public required bool HasNamedContact { get; init; }

    public required AccreditationStatus Status { get; init; }

    /// <summary>
    /// Accreditation is never approved on its own — it is approved *until* a
    /// date. Null only while the status is one where no date exists yet.
    /// </summary>
    public DateTime? ValidUntil { get; init; }

    /// <summary>Zones the standing credential permits.</summary>
    public required IReadOnlyList<string> ZoneAccess { get; init; }

    /// <summary>When this record was last synchronised.</summary>
    public required DateTime LastSyncedUtc { get; init; }
}
