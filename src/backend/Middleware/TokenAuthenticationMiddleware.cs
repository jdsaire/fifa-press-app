namespace FifaPressApp.Api.Middleware;

/// <summary>
/// Checks that a request carries the expected token, and answers 401 when it
/// does not.
///
/// <para>
/// <b>THIS IS NOT AUTHENTICATION. It is a simulated token check, and every
/// document in this repository says so in the same words.</b> What actually
/// happens below is a string comparison against a value that is written in
/// plain text in <c>appsettings.json</c>, committed to a public repository,
/// printed in <c>backend/02_API-REFERENCE.md</c>, and shipped to the browser
/// inside the frontend's own configuration file. There is no user, no
/// credential store, no issuer, no signature, no expiry, and no revocation.
/// Anyone who can read this repository can pass this check.
/// </para>
///
/// <para>
/// <b>Why it is built this way on purpose.</b> The source material for this
/// layer asks for token-based authentication middleware that returns 401 for an
/// invalid token, and that is exactly what this demonstrates: where in the
/// pipeline the check belongs, what it does to a request that fails it, and how
/// the rest of the application is written as though the check were real. Adding
/// a JWT library, a signing key and an identity store would produce a login
/// system this project has no user accounts for and no server to keep secrets
/// on — a more convincing lie, not a more honest program.
/// </para>
///
/// <para>
/// <b>An interface that implies security it does not have is the one dishonesty
/// this project has refused throughout.</b> The frontend's sign-in screen
/// already publishes its own passwords on screen for the same reason. This is
/// the server-side half of that same commitment.
/// </para>
/// </summary>
public sealed class TokenAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private const string BearerPrefix = "Bearer ";

    /// <summary>
    /// The one value that passes. Read from configuration rather than compiled
    /// in, so it is visible to a reader of <c>appsettings.json</c> rather than
    /// buried — the point is that it is not a secret, so it should not look
    /// like one.
    /// </summary>
    private readonly string expected = configuration["Api:Token"] ?? "demo-token-2026";

    /// <summary>
    /// Paths that answer without a token.
    ///
    /// <para>
    /// The service description and the OpenAPI document are how somebody
    /// discovers what this API is and how to call it. Putting them behind the
    /// token would mean a reader has to already know the answer before they can
    /// find the question, and neither one exposes a record.
    /// </para>
    /// </summary>
    private static readonly string[] Open = ["/", "/openapi"];

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsOpen(context.Request.Path))
        {
            await next(context);
            return;
        }

        if (!IsValid(context.Request))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            // The same one-key shape every other failure in this API uses.
            await context.Response.WriteAsync("""{"error":"Unauthorized."}""");
            return;
        }

        await next(context);
    }

    private static bool IsOpen(PathString path) =>
        path == "/" || Open.Any(open => open != "/" && path.StartsWithSegments(open));

    private bool IsValid(HttpRequest request)
    {
        // The ordinary case: an Authorization header carrying a bearer token.
        var header = request.Headers.Authorization.ToString();
        if (header.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Matches(header[BearerPrefix.Length..]);
        }

        // The WebSocket case. A browser cannot set headers on a WebSocket
        // handshake, so the SignalR client appends the token to the query string
        // instead. Refusing to look there would mean the hub could never
        // authenticate a browser client at all — this is the standard place
        // SignalR puts it, not a loophole invented here.
        if (request.Query.TryGetValue("access_token", out var queryToken))
        {
            return Matches(queryToken.ToString());
        }

        return false;
    }

    /// <summary>
    /// Ordinal, and against the value exactly as it arrived — never trimmed,
    /// never case-folded. Rewriting a token before comparing it is the kind of
    /// leniency that is right for a human-typed identifier and wrong for a
    /// credential.
    /// </summary>
    private bool Matches(string supplied) => string.Equals(supplied, expected, StringComparison.Ordinal);
}
