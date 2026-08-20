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
using FifaPressApp.Api.Realtime;
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

// The persistent-connection layer. SignalR ships inside the ASP.NET Core shared
// framework, so this adds no package: it is the same runtime the rest of the
// API already runs on.
builder.Services.AddSignalR();
builder.Services.AddSingleton<ChangeNotifier>();

// The browsers allowed to call this API.
//
// A browser refuses to hand a response from one origin to a page served from
// another unless the response says it may. The deployed frontend is on
// jdsaire.github.io and this API is not, so without this policy every request
// the live site makes would be blocked before it was read — the request would
// reach the server and succeed, and the browser would throw the answer away.
//
// Origins are listed rather than opened with AllowAnyOrigin, and they have to
// be: SignalR needs AllowCredentials, and the CORS specification forbids
// combining credentials with a wildcard origin. The localhost entries are the
// ports the two halves use when run side by side during development.
builder.Services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));

// The OpenAPI document, registered bare. It is how a reader discovers the
// routes without reading the code, and it is the whole of this API's tooling.
builder.Services.AddOpenApi();

var app = builder.Build();

// ---------------------------------------------------------------- pipeline
//
// THE ORDER BELOW IS ERROR HANDLING, THEN AUTHENTICATION, THEN LOGGING, and it
// is not to be "improved".
//
// Middleware nests rather than queues. The first thing registered is the
// outermost wrapper: every later component, and the endpoint itself, runs
// inside it, and the response travels back out through all of them in reverse.
// So "first" means "sees everything", and "last" means "closest to the work".
//
// WHY EACH ONE SITS WHERE IT DOES.
//   Error handling is outermost because it can only catch what is inside it.
//   Registered anywhere else, a throw from the layer above would escape it and
//   the caller would get a stack trace or a bare 500.
//
//   Authentication comes next so that nothing further in — no endpoint, no
//   route handler, no store read — executes for a request that has not passed
//   the check. Work done before rejecting is work done for nobody.
//
//   Logging is innermost so it observes the status code the endpoint actually
//   produced, rather than one an outer layer might still change.
//
// THE COST OF THAT ORDER, STATED PLAINLY. Because logging is innermost, the two
// cases resolved above it never reach it: a 401 from the token check, and an
// exception caught by the error handler. Both are logged by the component that
// handles them, in the same shape, so nothing goes unrecorded — but the
// single-choke-point property a logger usually has is not true here. That is a
// genuine trade-off of the specified order, not a defect in the code, and it is
// written up in backend/03_MIDDLEWARE-PIPELINE.md rather than papered over.

// CORS runs ahead of all three. A browser's preflight OPTIONS request carries
// no token by design, so it has to be answered before the token check would
// reject it; and a rejected request still needs CORS headers on it, or the
// browser shows the page a network error instead of the 401 the server sent.
app.UseCors(FrontendCorsPolicy);

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

// The hub's own address. Clients connect here and then listen; see
// ChangeNotificationHub for why it has no client-callable methods.
app.MapHub<ChangeNotificationHub>("/hubs/changes");

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
public partial class Program
{
    /// <summary>The one CORS policy this API defines.</summary>
    private const string FrontendCorsPolicy = "frontend";
}
