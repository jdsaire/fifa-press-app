using System.Text.Json;
using System.Text.Json.Serialization;
using FifaPressApp.Api.Models;

namespace FifaPressApp.Api.Storage;

/// <summary>
/// Every accreditation record this API knows about, and every change written
/// against one, held in memory for the lifetime of the process.
///
/// <para>
/// <b>There is no database, and that is a scope decision rather than a gap.</b>
/// Restarting this process resets the data to the seed below. A real
/// accreditation service is backed by a durable store and this one is not, so
/// nothing here should be read as a persistence design — it is the smallest
/// thing that lets the frontend read a record over HTTP instead of from a
/// mock, which is the entire point of this layer.
/// </para>
///
/// <para>
/// <b>Seeded from a file rather than from C#.</b> The two demo records and
/// their eight changes are the same ones the frontend's mock provider has
/// always held, down to the Spanish and Portuguese prose. They live in
/// <c>Data/seed.json</c> so the before/after comparison is like-for-like and so
/// the data is inspectable without reading code. Loading a seed file into an
/// in-memory list is not persistence: nothing is ever written back to it.
/// </para>
///
/// <para>
/// <b>Locked, because a singleton is shared.</b> ASP.NET Core serves requests
/// concurrently, so a mutable collection behind a singleton needs a lock or it
/// will eventually corrupt itself under two simultaneous writes. One lock over
/// the whole store is correct at this scale and obviously correct to read,
/// which matters more here than throughput on eight records.
/// </para>
/// </summary>
public sealed class AccreditationStore
{
    private static readonly JsonSerializerOptions SeedOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly Lock gate = new();
    private readonly List<AccreditationRecord> accreditations;
    private readonly List<ChangeRecord> changes;

    public AccreditationStore()
        : this(Path.Combine(AppContext.BaseDirectory, "Data", "seed.json"))
    {
    }

    /// <summary>
    /// The seeding constructor tests use, so a test can start from a known file
    /// rather than from whatever the last test left behind.
    /// </summary>
    public AccreditationStore(string seedPath)
    {
        var seed = JsonSerializer.Deserialize<SeedFile>(File.ReadAllText(seedPath), SeedOptions)
            ?? throw new InvalidOperationException($"The seed file at {seedPath} could not be read.");

        accreditations = [.. seed.Accreditations];
        changes = [.. seed.Changes];
    }

    /// <summary>Every record, in seed order.</summary>
    public IReadOnlyList<AccreditationRecord> All()
    {
        lock (gate)
        {
            return [.. accreditations];
        }
    }

    /// <summary>One record, or null. Credential ids are matched exactly.</summary>
    public AccreditationRecord? Find(string credentialId)
    {
        lock (gate)
        {
            return accreditations.FirstOrDefault(a => a.CredentialId == credentialId);
        }
    }

    /// <summary>
    /// Adds a record. Returns false when the credential id is already taken —
    /// the caller turns that into a 409 rather than silently overwriting
    /// somebody's accreditation.
    /// </summary>
    public bool Add(AccreditationRecord record)
    {
        lock (gate)
        {
            if (accreditations.Any(a => a.CredentialId == record.CredentialId))
            {
                return false;
            }

            accreditations.Add(record);
            return true;
        }
    }

    /// <summary>Replaces a record in place. Returns false when it does not exist.</summary>
    public bool Replace(AccreditationRecord record)
    {
        lock (gate)
        {
            var index = accreditations.FindIndex(a => a.CredentialId == record.CredentialId);
            if (index < 0)
            {
                return false;
            }

            accreditations[index] = record;
            return true;
        }
    }

    /// <summary>
    /// Removes a record and every change written against it. Returns false when
    /// it does not exist.
    ///
    /// <para>
    /// Deleting the changes alongside the record is deliberate. Leaving them
    /// behind would produce a change log belonging to nobody, which the append-
    /// only rule does not protect and no reader could interpret. Note the
    /// tension worth naming: DELETE exists because the source document's CRUD
    /// list names it, while this project's own domain says a credential is
    /// withdrawn by writing a change, never by erasing history. Both are true
    /// here — the endpoint exists, and the honest way to end an accreditation
    /// in this app is still a Withdrawal change.
    /// </para>
    /// </summary>
    public bool Remove(string credentialId)
    {
        lock (gate)
        {
            var index = accreditations.FindIndex(a => a.CredentialId == credentialId);
            if (index < 0)
            {
                return false;
            }

            accreditations.RemoveAt(index);
            changes.RemoveAll(c => c.CredentialId == credentialId);
            return true;
        }
    }

    /// <summary>
    /// A holder's changes, ordered by when each one takes effect, newest first
    /// — not by when it was written. Same ordering rule the frontend applies,
    /// applied here so the two cannot disagree about what the record looks like.
    /// </summary>
    public IReadOnlyList<ChangeRecord> ChangesFor(string credentialId)
    {
        lock (gate)
        {
            return
            [
                .. changes
                    .Where(c => c.CredentialId == credentialId)
                    .OrderByDescending(c => c.EffectiveUtc)
            ];
        }
    }

    /// <summary>
    /// Appends a change. There is no matching update or delete anywhere in this
    /// class, on purpose — see <see cref="ChangeRecord"/>.
    /// </summary>
    public void AppendChange(ChangeRecord change)
    {
        lock (gate)
        {
            changes.Add(change);
        }
    }

    /// <summary>Whether a change id is already in use.</summary>
    public bool HasChange(string changeId)
    {
        lock (gate)
        {
            return changes.Any(c => c.ChangeId == changeId);
        }
    }

    private sealed record SeedFile(
        IReadOnlyList<AccreditationRecord> Accreditations,
        IReadOnlyList<ChangeRecord> Changes);
}
