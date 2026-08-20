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

var builder = WebApplication.CreateBuilder(args);

// The OpenAPI document, registered bare. It is how a reader discovers the
// routes without reading the code, and it is the whole of this API's tooling.
builder.Services.AddOpenApi();

var app = builder.Build();

// The generated document, served at /openapi/v1.json.
app.MapOpenApi();

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
