// The FIFA Press App API — the first server this project has ever had.
//
// WHAT THIS IS. A deliberately small ASP.NET Core Web API that serves the
// accreditation record the Blazor frontend already knows how to display. The
// frontend has read from an in-memory mock since it was built; this exists so
// the same screens, unchanged, can read from a real service instead.
//
// WHAT THIS IS NOT. There is no database, no ORM, and no persistence beyond
// the lifetime of this process. There is no real authentication — the token
// check added later in this file is a fixed string comparison and every
// document in backend/ says so plainly. That is not an oversight to be fixed
// later; it is the honest scope of a foundational demonstration, and claiming
// otherwise would be the one dishonesty this project has refused throughout.

using System.Text.Json.Serialization;
using FifaPressApp.Api.Endpoints;
using FifaPressApp.Api.Middleware;
using FifaPressApp.Api.Storage;

var builder = WebApplication.CreateBuilder(args);

// The record store, as a singleton: one in-memory collection for the life of
// the process, which is what "in-memory storage" means here. See
// AccreditationStore for why there is no database and why that is a scope
// decision rather than an omission.
builder.Services.AddSingleton<AccreditationStore>();

// Enums travel as their names, not their numbers. "MemberAssociationQuota" is
// readable in a response body and survives someone reordering the enum; a bare
// 0 is neither. The frontend parses these names directly.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// The OpenAPI document, registered bare. It is how a reader discovers the
// routes without reading the code, and it is the whole of this API's tooling.
builder.Services.AddOpenApi();

var app = builder.Build();

// FIRST IN THE PIPELINE, and it has to be. Middleware wraps whatever is
// registered after it, so error handling registered first is the outermost
// layer and therefore the only one positioned to catch a throw from anything
// else — including the middleware registered below it.
app.UseMiddleware<ErrorHandlingMiddleware>();

// SECOND. Inside the error handler, so a throw from the token check is still
// caught and still answers with the consistent JSON error; outside everything
// else, so no endpoint runs for a request that failed the check.
app.UseMiddleware<TokenAuthenticationMiddleware>();

// LAST. Innermost, so it observes the status code the endpoint actually
// returned rather than one that later middleware might still change.
app.UseMiddleware<RequestLoggingMiddleware>();

// The generated document, served at /openapi/v1.json.
app.MapOpenApi();

app.MapAccreditationEndpoints();

// A route that exists only to prove the error handler works, and only outside
// Production. Gate 3 of this run has to demonstrate that an unhandled
// exception returns the consistent JSON error rather than a stack trace, and
// the only honest way to show that is to cause one. It is registered behind an
// environment check so it cannot exist on a deployed instance.
if (!app.Environment.IsProduction())
{
    app.MapGet("/api/diagnostics/throw", IResult () =>
        throw new InvalidOperationException("Deliberate failure, to demonstrate the error handler."));
}

app.MapGet("/", () => Results.Ok(new
{
    name = "FIFA Press App API",
    description = "Accreditation records and their change log, for the FIFA Press App frontend.",
    documentation = "/openapi/v1.json",
}));

app.Run();

/// <summary>
/// Named so the test project can reach this assembly's entry point. A Web API
/// built from top-level statements has an implicit <c>Program</c> class that is
/// internal by default, and a test host needs to name it.
/// </summary>
public partial class Program;
