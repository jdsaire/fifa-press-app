namespace FifaPressApp.Api.Middleware;

/// <summary>
/// Records the HTTP method, the request path, and the response status code for
/// every request that reaches it.
///
/// <para>
/// <b>Three fields, and no more.</b> The source material for this layer names
/// exactly these, and there is a good reason not to grow the list on instinct:
/// request bodies here carry accreditation records, and a logger that quietly
/// wrote them to disk would turn an audit aid into a second, unmanaged copy of
/// personal data. Query strings are excluded from the logged path for the same
/// reason — the SignalR client puts its token there, and a log that captured it
/// would publish the one value the check depends on. The path is logged; what
/// follows the <c>?</c> is not.
/// </para>
///
/// <para>
/// <b>Registered last, and that has a consequence worth stating.</b> Middleware
/// wraps what comes after it, so registering this one last makes it the
/// innermost layer: it sees the status code an endpoint actually produced,
/// which is what makes the log useful. The cost is that a request rejected by
/// the token check above never reaches it, so a 401 short-circuited there is
/// not logged here. That is a real gap, it follows directly from the pipeline
/// order this project was asked to implement, and the honest response is to
/// name it rather than to quietly reorder the pipeline.
/// </para>
///
/// <para>
/// <b>Two cases therefore never reach this middleware, and each is logged by
/// whichever layer does handle it,</b> in the same "METHOD path -&gt; status"
/// shape so the log reads uniformly: a request refused by the token check is
/// logged by that middleware, and a request that throws is logged by the error
/// handler, because the exception propagates straight past this one rather than
/// returning through it. See <c>backend/03_MIDDLEWARE-PIPELINE.md</c>.
/// </para>
/// </summary>
public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        // Read after the call, not before: the status code does not exist until
        // whatever comes next has decided what it is.
        logger.LogInformation("{Method} {Path} -> {StatusCode}",
            context.Request.Method,
            context.Request.Path.Value,
            context.Response.StatusCode);
    }
}
