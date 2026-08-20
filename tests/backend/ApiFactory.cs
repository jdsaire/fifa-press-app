using FifaPressApp.Api.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FifaPressApp.Api.Tests;

/// <summary>
/// Starts the real API in-process for one test, with its own store.
///
/// <para>
/// <b>A fresh store per factory, and that matters more than it looks.</b> The
/// store is a singleton holding mutable state, so a test that creates a record
/// would otherwise be visible to every test that ran after it — and xUnit gives
/// no ordering guarantee, so the failures would be intermittent and would look
/// like flakiness rather than like shared state. Each factory replaces the
/// registration with a store seeded from the file, so every test starts from
/// the same two records.
/// </para>
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// The simulated token, matching the one in the API's own appsettings.json.
    /// Not a secret; see the authentication middleware.
    /// </summary>
    public const string ValidToken = "demo-token-2026";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Not Production, so the diagnostics route that proves the error
        // handler exists. It is absent from a deployed instance by design.
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AccreditationStore>();
            services.AddSingleton(_ => new AccreditationStore(
                Path.Combine(AppContext.BaseDirectory, "Data", "seed.json")));
        });
    }

    /// <summary>A client that carries the valid token on every request.</summary>
    public HttpClient CreateAuthenticatedClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", ValidToken);
        return client;
    }
}
