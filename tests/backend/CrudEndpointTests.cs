using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace FifaPressApp.Api.Tests;

/// <summary>
/// Every CRUD route, in both the case where it works and the case where it does
/// not. The failure cases carry the weight here: a 404 that silently returned
/// an empty 200, or a POST that overwrote an existing record, would each be a
/// bug a caller could not detect.
/// </summary>
public class CrudEndpointTests
{
    private const string Amina = "MP-2026-04817";
    private const string Tomas = "RH-2026-00219";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private static object ValidRecord(string credentialId) => new
    {
        credentialId,
        holderName = "Test Holder",
        outlet = "Test Outlet",
        trackId = "Freelance",
        hasNamedContact = false,
        status = "Approved",
        validUntil = "2026-07-19T23:59:00Z",
        zoneAccess = new[] { "Media tribune" },
    };

    [Fact]
    public async Task The_collection_returns_both_seeded_records()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var records = await client.GetFromJsonAsync<JsonElement>("api/accreditations", Json);

        Assert.Equal(2, records.GetArrayLength());
        Assert.Contains(Amina, records.EnumerateArray().Select(r => r.GetProperty("credentialId").GetString()));
        Assert.Contains(Tomas, records.EnumerateArray().Select(r => r.GetProperty("credentialId").GetString()));
    }

    [Fact]
    public async Task A_single_record_reads_back_the_seeded_holder()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var record = await client.GetFromJsonAsync<JsonElement>($"api/accreditations/{Amina}", Json);

        Assert.Equal("Amina Bello", record.GetProperty("holderName").GetString());

        // The enum travels as its name, not its number. A client parsing "0"
        // would break the moment somebody reordered the enum.
        Assert.Equal("MemberAssociationQuota", record.GetProperty("trackId").GetString());
    }

    [Fact]
    public async Task An_unknown_credential_is_a_404_and_not_an_empty_success()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("api/accreditations/NO-SUCH-ID");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Json);
        Assert.Contains("NO-SUCH-ID", body.GetProperty("error").GetString());
    }

    [Fact]
    public async Task Creating_a_record_returns_201_and_a_location_that_resolves()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("api/accreditations", ValidRecord("FR-2026-00001"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        // The Location header is only useful if it actually points at the thing.
        var followed = await client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, followed.StatusCode);
    }

    [Fact]
    public async Task Creating_a_record_that_already_exists_is_refused_rather_than_overwriting()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("api/accreditations", ValidRecord(Amina));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        // And the original is untouched.
        var original = await client.GetFromJsonAsync<JsonElement>($"api/accreditations/{Amina}", Json);
        Assert.Equal("Amina Bello", original.GetProperty("holderName").GetString());
    }

    [Fact]
    public async Task Updating_a_record_replaces_it()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PutAsJsonAsync($"api/accreditations/{Tomas}", ValidRecord(Tomas));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await client.GetFromJsonAsync<JsonElement>($"api/accreditations/{Tomas}", Json);
        Assert.Equal("Test Holder", updated.GetProperty("holderName").GetString());
    }

    [Fact]
    public async Task Updating_a_record_that_does_not_exist_is_a_404()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PutAsJsonAsync("api/accreditations/GHOST", ValidRecord("GHOST"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task The_route_id_wins_over_the_body_id_on_update()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        // A body naming a different credential must not rename or move the
        // record the URL addressed.
        await client.PutAsJsonAsync($"api/accreditations/{Tomas}", ValidRecord("SOMETHING-ELSE"));

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"api/accreditations/{Tomas}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("api/accreditations/SOMETHING-ELSE")).StatusCode);
    }

    [Fact]
    public async Task Deleting_a_record_removes_it_and_deleting_again_is_a_404()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"api/accreditations/{Tomas}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"api/accreditations/{Tomas}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"api/accreditations/{Tomas}")).StatusCode);
    }

    [Fact]
    public async Task Deleting_a_record_takes_its_change_log_with_it()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        await client.DeleteAsync($"api/accreditations/{Tomas}");

        // Not an empty list: a change log belonging to nobody is not something a
        // reader could interpret, so the record has to be gone as a whole.
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"api/accreditations/{Tomas}/changes")).StatusCode);
    }

    [Fact]
    public async Task A_records_changes_come_back_newest_effective_first()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var changes = await client.GetFromJsonAsync<JsonElement>($"api/accreditations/{Amina}/changes", Json);

        Assert.Equal(5, changes.GetArrayLength());

        // Ordered by when each change takes effect, not when it was written —
        // the same rule the frontend applies, so the two cannot disagree about
        // what the record looks like.
        var effective = changes.EnumerateArray()
            .Select(c => c.GetProperty("effectiveUtc").GetDateTime())
            .ToList();

        Assert.Equal(effective.OrderByDescending(d => d), effective);
    }

    [Fact]
    public async Task Changes_for_an_unknown_credential_are_a_404()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("api/accreditations/GHOST/changes")).StatusCode);
    }

    [Fact]
    public async Task Appending_a_change_adds_it_to_the_record()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync($"api/accreditations/{Amina}/changes", new
        {
            changeId = "ch-900",
            kind = "ZoneAccessWidened",
            effectiveUtc = "2026-07-08T10:00:00Z",
            whatChanged = new { en = "Zone added.", es = "Zona anadida.", pt = "Zona adicionada." },
            reason = new { en = "The quota grew.", es = "El cupo crecio.", pt = "A cota cresceu." },
            nextStep = new { en = "Nothing to do.", es = "Nada que hacer.", pt = "Nada a fazer." },
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var changes = await client.GetFromJsonAsync<JsonElement>($"api/accreditations/{Amina}/changes", Json);
        Assert.Equal(6, changes.GetArrayLength());
        Assert.Contains("ch-900", changes.EnumerateArray().Select(c => c.GetProperty("changeId").GetString()));
    }

    [Fact]
    public async Task There_is_no_route_that_edits_or_deletes_a_change()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateAuthenticatedClient();

        // Append-only is a domain rule, and the routing table is where it is
        // enforced. If either of these ever starts resolving, the record has
        // stopped being append-only and this test is the thing that says so.
        var put = await client.PutAsJsonAsync($"api/accreditations/{Amina}/changes/ch-001", new { });
        var delete = await client.DeleteAsync($"api/accreditations/{Amina}/changes/ch-001");

        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
    }
}
