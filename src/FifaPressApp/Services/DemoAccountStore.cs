namespace FifaPressApp.Services;

/// <summary>
/// One published demo account.
/// </summary>
/// <param name="Identifier">
/// What the person types into the identifier field. The credential number, which
/// is also what the record is keyed by — one fewer invented value to keep in
/// sync, and it makes the connection between the sign-in and the record visible
/// rather than magic.
/// </param>
/// <param name="Password">
/// Published on the sign-in screen in plain text, because it is not a secret and
/// pretending otherwise would be the interface lying about itself.
/// </param>
/// <param name="CredentialId">The record this account opens.</param>
/// <param name="HolderName">Whose record it is, for the published list.</param>
/// <param name="DescriptionKey">
/// The resource key for one line saying what makes this record worth looking at
/// next to the other. This is the part that makes two accounts a demonstration
/// rather than two accounts: a person should be told what the difference is
/// before they go looking for it.
///
/// A key rather than the sentence itself, because the sentence is interface
/// copy on the sign-in screen and has to render in whichever of the three
/// languages the reader is in.
/// </param>
public sealed record DemoAccount(
    string Identifier,
    string Password,
    string CredentialId,
    string HolderName,
    string DescriptionKey);

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
        Identifier: "MP-2026-04817",
        Password: "amina-demo-2026",
        CredentialId: "MP-2026-04817",
        HolderName: "Amina Bello",
        DescriptionKey: "signIn.accountAmina");

    /// <summary>
    /// Tomás's record: the rights-holder with a named contact, so his ceiling is
    /// ImmediateOnly and the same conditional change is written to his record
    /// without interrupting him.
    /// </summary>
    public static readonly DemoAccount Tomas = new(
        Identifier: "RH-2026-00219",
        Password: "tomas-demo-2026",
        CredentialId: "RH-2026-00219",
        HolderName: "Tomás L.",
        DescriptionKey: "signIn.accountTomas");

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
