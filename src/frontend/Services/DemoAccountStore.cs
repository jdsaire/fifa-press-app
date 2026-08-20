namespace FifaPressApp.Services;

/// <summary>
/// One published demo account.
/// </summary>
/// <param name="Identifier">
/// What the person types into the identifier field.
///
/// <para>
/// It used to be the credential number itself, on the reasoning that this was
/// one fewer invented value to keep in sync and made the connection between the
/// sign-in and the record visible rather than magic. Both halves of that were
/// true and neither survives contact with a person reading the screen:
/// <c>MP-2026-04817</c> is not something anyone types from memory, and the
/// connection it made visible was visible to a reader of this file rather than
/// to a reader of the sign-in form.
/// </para>
///
/// <para>
/// <b>Nothing is keyed by this value.</b> The record is keyed by
/// <see cref="CredentialId"/>, which is unchanged and still carries the
/// credential number — so this rename is confined to what a person types, and
/// there is no second mapping to keep in sync, because there is no mapping: the
/// account carries both values and hands the right one to whoever asks.
/// </para>
/// </param>
/// <param name="Password">
/// Published on the sign-in screen in plain text, because it is not a secret and
/// pretending otherwise would be the interface lying about itself.
/// </param>
/// <param name="CredentialId">The record this account opens.</param>
/// <param name="HolderName">Whose record it is, for the published list.</param>
public sealed record DemoAccount(
    string Identifier,
    string Password,
    string CredentialId,
    string HolderName);

/// <summary>
/// The two demo accounts, and the only credential check this app performs.
///
/// <para>
/// <b>This is not a credential store and must never be described as one.</b>
/// The passwords are compiled into the app, shipped to the browser, and printed
/// on the sign-in screen. There is no hashing here because there is nothing to
/// protect: hashing a password that is published two inches away would be
/// security theatre, and worse, it would make the code look like it was
/// defending something. What this class actually is: a lookup table that decides
/// which of two seeded records to show.
/// </para>
///
/// <para>
/// The identifier is matched case-insensitively and with surrounding whitespace
/// ignored, because a credential number copied off a screen often arrives with a
/// trailing space and turning that person away teaches them nothing. The
/// password is compared byte for byte — never trimmed, never case-folded, never
/// rewritten. Allow-list rewriting is right for an identifier and wrong for a
/// password, and now that a real comparison happens, that rule is testable
/// rather than hypothetical.
/// </para>
/// </summary>
public sealed class DemoAccountStore
{
    /// <summary>
    /// Amina's record: the member-association quota holder with no named
    /// contact, so her ceiling is ImmediateAndForeseeable and a conditional
    /// change interrupts her.
    /// </summary>
    public static readonly DemoAccount Amina = new(
        Identifier: "demo_staff1",
        Password: "Demo#2026Staff1",
        CredentialId: "MP-2026-04817",
        HolderName: "Amina Bello");

    /// <summary>
    /// Tomás's record: the rights-holder with a named contact, so his ceiling is
    /// ImmediateOnly and the same conditional change is written to his record
    /// without interrupting him.
    /// </summary>
    public static readonly DemoAccount Tomas = new(
        Identifier: "demo_staff2",
        Password: "Demo#2026Staff2",
        CredentialId: "RH-2026-00219",
        HolderName: "Tomás L.");

    /// <summary>
    /// Both accounts, in the order the sign-in screen publishes them. Amina
    /// first, because hers is the record every other document in this project
    /// describes.
    /// </summary>
    public IReadOnlyList<DemoAccount> Published { get; } = [Amina, Tomas];

    /// <summary>
    /// The account these details open, or <c>null</c>. One method for both
    /// halves on purpose: the caller cannot find out that the identifier was
    /// right and only the password wrong, which is the shape a real
    /// implementation has to have and the reason to model it correctly here.
    /// </summary>
    public DemoAccount? Match(string? identifier, string? password)
    {
        if (identifier is null || password is null)
        {
            return null;
        }

        var typed = identifier.Trim();

        return Published.FirstOrDefault(account =>
            string.Equals(account.Identifier, typed, StringComparison.OrdinalIgnoreCase)

            // Ordinal, and against the value exactly as it arrived.
            && string.Equals(account.Password, password, StringComparison.Ordinal));
    }
}
