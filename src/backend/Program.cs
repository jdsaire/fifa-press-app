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

// The generated document, served at /openapi/v1.json.
app.MapOpenApi();

app.MapAccreditationEndpoints();

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
