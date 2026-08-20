namespace FifaPressApp.Models;

/// <summary>
/// The three accreditation tracks. No new archetypes: these are the only three
/// the concept scopes for.
/// </summary>
public enum TrackId
{
    MemberAssociationQuota,
    RightsHolder,
    Freelance,
}

/// <summary>
/// How far down the urgency scale a track is allowed to interrupt. Derived from
/// whether the holder has a named human contact, never chosen by the holder —
/// there is no notification preference screen anywhere in this app, and this
/// enum existing as a *derived* value rather than a stored setting is what
/// keeps it that way.
/// </summary>
public enum NotificationCeiling
{
    /// <summary>
    /// Only an immediate change interrupts. A holder with a named contact
    /// already has a human who can answer a conditional question, so a
    /// foreseeable change is written to the record without interrupting.
    /// </summary>
    ImmediateOnly,

    /// <summary>
    /// Both immediate and foreseeable changes interrupt. This is the ceiling
    /// for a holder with nobody to ask — the record is the only channel they
    /// have, so it has to carry more.
    /// </summary>
    ImmediateAndForeseeable,
}

/// <summary>
/// A holder's track, and the notification ceiling that follows from it.
///
/// MOCKED: in a real deployment the track and its named-contact flag are
/// carried on the accreditation record issued by the accreditation system.
/// </summary>
public sealed record Track(TrackId TrackId, bool HasNamedContact)
{
    /// <summary>
    /// A precondition, not a setting. There is deliberately no way to assign
    /// this — it is recomputed from <see cref="HasNamedContact"/> every time it
    /// is read, so no code path can store a ceiling that disagrees with the
    /// track it came from.
    /// </summary>
    public NotificationCeiling NotificationCeiling => HasNamedContact
        ? NotificationCeiling.ImmediateOnly
        : NotificationCeiling.ImmediateAndForeseeable;
}
