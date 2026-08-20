using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace FifaPressApp.Api.Tests;

/// <summary>
/// What the API refuses, and the shape it refuses in.
///
/// <para>
/// The consistent envelope matters as much as the individual rules: a caller
/// should write one parser for a rejected request, not one per endpoint.
/// </para>
/// </summary>
public class ValidationTests
{
    private const string Amina = "MP-2026-04817";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task An_empty_record_is_rejected_with_every_problem_at_once()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("api/accreditations", new
        {
            credentialId = "",
            holderName = "   ",
            outlet = "",
            trackId = "Freelance",
            status = "Pending",
            zoneAccess = Array.Empty<string>(),
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        Assert.Equal("Validation failed.", body.GetProperty("error").GetString());

        var details = body.GetProperty("details");

        // All four at once, not the first one found. A validator that stopped
        // early would make the caller fix a field, resubmit, and discover the
        // next — which is the same "find out by being refused" pattern this
        // project argues against, pointed at a developer instead of a holder.
        Assert.True(details.TryGetProperty("CredentialId", out _));
        Assert.True(details.TryGetProperty("HolderName", out _));
        Assert.True(details.TryGetProperty("Outlet", out _));
        Assert.True(details.TryGetProperty("ZoneAccess", out _));
    }

    [Fact]
    public async Task An_unrecognised_track_is_a_400_and_never_a_500()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("api/accreditations", new
        {
            credentialId = "FR-2026-00002",
            holderName = "Test",
            outlet = "Test",
            trackId = "Wizard",
            status = "Pending",
            zoneAccess = new[] { "Media tribune" },
        });

        // The caller's typo must be reported as the caller's mistake. Parsing
        // the enum optimistically would throw, and the error handler would turn
        // that into "we broke" for what is plainly a bad request.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        var message = body.GetProperty("details").GetProperty("TrackId")[0].GetString();

        // And it says what would have been acceptable.
        Assert.Contains("MemberAssociationQuota", message);
    }

    [Fact]
    public async Task An_approved_record_must_say_what_date_it_is_valid_until()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("api/accreditations", new
        {
            credentialId = "FR-2026-00003",
            holderName = "Test",
            outlet = "Test",
            trackId = "Freelance",
            status = "Approved",
            zoneAccess = new[] { "Media tribune" },
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        Assert.True(body.GetProperty("details").TryGetProperty("ValidUntil", out _));
    }

    [Fact]
    public async Task A_half_translated_change_cannot_be_written()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync($"api/accreditations/{Amina}/changes", new
        {
            changeId = "ch-901",
            kind = "ZoneAccessWidened",
            whatChanged = new { en = "Something changed.", es = "Algo cambio.", pt = "" },
            reason = new { en = "A reason.", es = "Una razon.", pt = "Uma razao." },
            nextStep = new { en = "A step.", es = "Un paso.", pt = "Um passo." },
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        var message = body.GetProperty("details").GetProperty("WhatChanged")[0].GetString();

        // Rejected at write time rather than rendering as a blank line that
        // only appears once somebody switches language.
        Assert.Contains("Portuguese", message);
    }

    [Fact]
    public async Task A_reason_that_only_restates_the_outcome_is_not_a_reason()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync($"api/accreditations/{Amina}/changes", new
        {
            changeId = "ch-902",
            kind = "MatchAccessRevoked",
            whatChanged = new { en = "Access was revoked.", es = "Se revoco el acceso.", pt = "O acesso foi revogado." },
            reason = new { en = "Access was revoked!", es = "Otra cosa.", pt = "Outra coisa." },
            nextStep = new { en = "A step.", es = "Un paso.", pt = "Um passo." },
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        Assert.True(body.GetProperty("details").TryGetProperty("Reason", out _));
    }

    [Fact]
    public async Task A_change_that_waits_on_a_fixture_must_state_the_condition()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync($"api/accreditations/{Amina}/changes", new
        {
            changeId = "ch-903",
            kind = "MatchAccessRevoked",
            dependsOnMatchNumber = 93,
            whatChanged = new { en = "Access depends on a fixture.", es = "Depende de un partido.", pt = "Depende de um jogo." },
            reason = new { en = "The quota contracts.", es = "El cupo se reduce.", pt = "A cota reduz-se." },
            nextStep = new { en = "Hold your travel.", es = "Espera.", pt = "Aguarde." },
        });

        // Without the condition spelled out, a conditional change reads as a
        // decision already taken.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        Assert.True(body.GetProperty("details").TryGetProperty("ConditionText", out _));
    }

    [Fact]
    public async Task A_dead_end_must_name_who_decided()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync($"api/accreditations/{Amina}/changes", new
        {
            changeId = "ch-904",
            kind = "AdministrativeCorrection",
            nextStepIsActionable = false,
            whatChanged = new { en = "A correction.", es = "Una correccion.", pt = "Uma correcao." },
            reason = new { en = "A clerical error.", es = "Un error administrativo.", pt = "Um erro administrativo." },
            nextStep = new { en = "Nothing to do.", es = "Nada que hacer.", pt = "Nada a fazer." },
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        Assert.True(body.GetProperty("details").TryGetProperty("DecidedBy", out _));
    }

    [Fact]
    public async Task A_valid_change_is_still_accepted()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        // The counterweight to every test above: validation that rejected
        // everything would pass all of them and be useless.
        var response = await client.PostAsJsonAsync($"api/accreditations/{Amina}/changes", new
        {
            changeId = "ch-905",
            kind = "ZoneAccessWidened",
            whatChanged = new { en = "Zone added.", es = "Zona anadida.", pt = "Zona adicionada." },
            reason = new { en = "The quota grew.", es = "El cupo crecio.", pt = "A cota cresceu." },
            nextStep = new { en = "Nothing to do.", es = "Nada que hacer.", pt = "Nada a fazer." },
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
