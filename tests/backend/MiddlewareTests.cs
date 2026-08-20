using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace FifaPressApp.Api.Tests;

/// <summary>
/// The three middleware components, exercised through the real pipeline rather
/// than called directly.
///
/// <para>
/// <b>This is what the extra test package bought.</b> Asserting that
/// <c>Program.cs</c> registers three components in a particular order would
/// prove that a file contains three lines in a particular order. These tests
/// send real requests and read what actually came back, so the ordering is
/// demonstrated by its consequences.
/// </para>
/// </summary>
public class MiddlewareTests
{
    private const string Amina = "MP-2026-04817";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    // ------------------------------------------- authentication (simulated)

    [Fact]
    public async Task A_request_with_no_token_is_refused()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/accreditations");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        Assert.Equal("Unauthorized.", body.GetProperty("error").GetString());
    }

    [Fact]
    public async Task A_request_with_the_wrong_token_is_refused()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", "not-the-token");

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("api/accreditations")).StatusCode);
    }

    [Fact]
    public async Task The_token_comparison_is_ordinal()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", ApiFactory.ValidToken.ToUpperInvariant());

        // Case-folding a token before comparing it is the kind of leniency that
        // is right for a human-typed identifier and wrong for a credential.
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("api/accreditations")).StatusCode);
    }

    [Fact]
    public async Task Every_write_route_is_behind_the_token_too()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // A read guarded and a write open would be worse than no check at all.
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("api/accreditations", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PutAsJsonAsync($"api/accreditations/{Amina}", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.DeleteAsync($"api/accreditations/{Amina}")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync($"api/accreditations/{Amina}/changes", new { })).StatusCode);
    }

    [Fact]
    public async Task A_token_on_the_query_string_is_accepted()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // The WebSocket case: a browser cannot set headers on a hub handshake,
        // so SignalR puts the token here instead.
        var response = await client.GetAsync($"api/accreditations?access_token={ApiFactory.ValidToken}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task The_service_description_and_the_openapi_document_answer_without_a_token()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Putting these behind the token would mean a reader has to already
        // know the answer before they can find the question. Neither exposes a
        // record.
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/openapi/v1.json")).StatusCode);
    }

    // --------------------------------------------------------- error handling

    [Fact]
    public async Task An_unhandled_exception_returns_the_consistent_json_error()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("api/diagnostics/throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        Assert.Equal("Internal server error.", body.GetProperty("error").GetString());
    }

    [Fact]
    public async Task An_unhandled_exception_never_leaks_the_stack_trace()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var raw = await (await client.GetAsync("api/diagnostics/throw")).Content.ReadAsStringAsync();

        // The exception type, the message and the source path all stay
        // server-side. The operator gets them in the log; the caller does not.
        Assert.DoesNotContain("InvalidOperationException", raw);
        Assert.DoesNotContain("Deliberate failure", raw);
        Assert.DoesNotContain("Program.cs", raw);
    }

    [Fact]
    public async Task The_error_shape_is_the_same_one_every_other_failure_uses()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        // 500, 404 and 401 all answer with a single "error" key carrying a
        // sentence, so a caller writes one parser rather than three.
        foreach (var route in new[] { "api/diagnostics/throw", "api/accreditations/GHOST" })
        {
            var body = await (await client.GetAsync(route)).Content.ReadFromJsonAsync<JsonElement>(Json);
            Assert.True(body.TryGetProperty("error", out var error));
            Assert.False(string.IsNullOrWhiteSpace(error.GetString()));
        }

        using var anonymous = factory.CreateClient();
        var refused = await (await anonymous.GetAsync("api/accreditations")).Content.ReadFromJsonAsync<JsonElement>(Json);
        Assert.True(refused.TryGetProperty("error", out _));
    }

    // ------------------------------------------------------ pipeline ordering

    [Fact]
    public async Task Authentication_runs_before_any_endpoint_does()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // If the token check ran after routing, this would 404 (no such record)
        // instead of 401. That it refuses first is the ordering, observed
        // rather than asserted about a source file.
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("api/accreditations/GHOST")).StatusCode);
    }

    [Fact]
    public async Task Authentication_refuses_a_throwing_route_before_it_can_throw()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // The diagnostics route throws unconditionally, so if anything reached
        // it this would be a 500. A 401 means the token check resolved the
        // request first — authentication sits outside the endpoint, and no work
        // is done for a caller who has not passed it.
        var response = await client.GetAsync("api/diagnostics/throw");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }
}
