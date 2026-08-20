
namespace FifaPressApp.Api.Middleware;

/// <summary>
/// Catches anything the rest of the pipeline throws and answers with one
/// consistent JSON error instead of a stack trace.
///
/// <para>
/// <b>Why this exists.</b> Without it, an unhandled exception in Development
/// returns a full stack trace — file paths, method names, the shape of the
/// code — and in Production returns an empty 500 with no body at all. The first
/// leaks; the second tells a client nothing it can parse. One deliberate shape,
/// the same in both environments, is more useful than either.
/// </para>
///
/// <para>
/// <b>What it deliberately does not do.</b> It does not translate exception
/// types into different status codes, and it does not put the exception message
/// in the response. An unhandled exception means this server has a bug the
/// caller cannot do anything about, and dressing that up as a specific,
/// actionable failure would be a guess presented as a diagnosis. Anything the
/// caller *can* act on — a bad field, a missing record — is caught earlier and
/// answered with a 400 or a 404 long before it reaches here.
/// </para>
///
/// <para>
/// The detail is not discarded, it is logged. The operator gets the full
/// exception; the caller gets a sentence.
/// </para>
/// </summary>
public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    /// <summary>
    /// The one body every unhandled failure produces. Matches the shape the
    /// validation responses use — an <c>error</c> key carrying a sentence — so a
    /// client writes one parser rather than one per failure mode.
    /// </summary>
    private const string Body = """{"error":"Internal server error."}""";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception for {Method} {Path}.",
                context.Request.Method, context.Request.Path);

            // If the response has already started, the status line and some of
            // the body are on the wire and cannot be recalled. Overwriting them
            // would corrupt the response rather than improve it, so the only
            // honest thing left is to log and let the connection fail.
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(Body);
        }
    }
}
