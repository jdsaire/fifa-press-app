namespace FifaPressApp.Models;

/// <summary>
/// Where a standing accreditation currently sits. Withdrawn is a state the
/// record can reach, but it is reached by writing a change, never by deleting
/// anything.
/// </summary>
public enum AccreditationStatus
{
    Pending,
    Approved,
    Refused,
    Withdrawn,
}

/// <summary>
/// The standing credential — who the holder is and what it permits, as opposed
/// to access to any one match. The two are named separately on purpose:
/// "accreditation" is the standing thing that is approved until a date, and
/// "match access" is the per-fixture thing that is requested and granted or
/// not. Collapsing them into one word is the confusion this split exists to
/// avoid.
///
/// MOCKED in this version: the holder, outlet, track and zone list would all
/// come from the accreditation system. No credential store exists here.
/// </summary>
public sealed record Accreditation
{
    /// <summary>The holder-facing identifier, and the key every read is made
    /// against.</summary>
    public required string CredentialId { get; init; }

    public required string HolderName { get; init; }

    public required string Outlet { get; init; }

    public required Track Track { get; init; }

    public required AccreditationStatus Status { get; init; }

    /// <summary>
    /// Accreditation is never "approved" on its own — it is approved *until* a
    /// date. Null only while the status is one where no date exists yet.
    /// </summary>
    public DateTime? ValidUntil { get; init; }

    /// <summary>Zones the standing credential permits.</summary>
    public required IReadOnlyList<string> ZoneAccess { get; init; }

    /// <summary>
    /// When this record was last synchronised. Not optional, and not a
    /// convenience: every staleness indicator in the app is driven from this
    /// value, so a record that could omit it would make staleness
    /// unimplementable on the surfaces that need it most.
    /// </summary>
    public required DateTime LastSyncedUtc { get; init; }
}
