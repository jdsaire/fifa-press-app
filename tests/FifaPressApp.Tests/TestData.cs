using System.Net;
using FifaPressApp.Models;
using FifaPressApp.Services;

namespace FifaPressApp.Tests;

/// <summary>
/// Shared test scaffolding, kept in one place so five test files do not each
/// carry their own copy of the same stub.
///
/// <para>
/// <b>Nothing here invents data.</b> The schedule comes from the tracked CSV the
/// app itself reads, and the fixtures built by hand below carry no team names at
/// all — not even placeholder ones. A test that needed a team name the provider
/// withholds would be testing the wrong thing, so the helpers make writing that
/// test awkward on purpose.
/// </para>
/// </summary>
internal static class TestData
{
    /// <summary>How many data rows the tracked schedule has.</summary>
    public const int ScheduleRowCount = 104;

    /// <summary>
    /// The real schedule, copied beside the test assembly by the project file.
    /// Read from disk rather than embedded so it stays the same bytes the app
    /// serves.
    /// </summary>
    public static string ScheduleCsv() =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data", "2026_World_Cup_Schedule.csv"));

    /// <summary>
    /// A provider reading the real schedule. The provider fetches its CSV over
    /// <see cref="HttpClient"/>, so the file is handed to it through a stub
    /// handler rather than by changing how the provider loads.
    /// </summary>
    public static MockAccessDataProvider ProviderOverRealSchedule() =>
        new(new HttpClient(new StaticContentHandler(ScheduleCsv())) { BaseAddress = new Uri("http://localhost/") });

    /// <summary>
    /// One fixture, built by hand for the query tests. Team names are not a
    /// parameter: an unresolved fixture has none, and a resolved one gets them
    /// through <see cref="Resolved"/> below, which is the only door.
    /// </summary>
    public static Fixture Fixture(
        int matchNumber,
        PhaseKind phase = PhaseKind.GroupStage,
        string? groupLetter = "A",
        string venue = "Estadio Azteca",
        string city = "Mexico City",
        DateTime? kickoffLocal = null) =>
        new()
        {
            MatchNumber = matchNumber,
            KickoffLocal = kickoffLocal ?? new DateTime(2026, 6, 11, 19, 0, 0),
            KickoffEastern = kickoffLocal ?? new DateTime(2026, 6, 11, 20, 0, 0),
            TimeZoneLabel = "Central Time (CST, UTC-6)",
            Phase = phase,
            GroupLetter = groupLetter,
            HomeLabel = null,
            AwayLabel = null,
            Venue = venue,
            City = city,
            IsResolved = false,
        };

    /// <summary>
    /// The same fixture, played. Named teams are only ever attached here, which
    /// mirrors what the provider does and keeps the rule visible in the tests
    /// as well as in the code.
    /// </summary>
    public static Fixture Resolved(Fixture fixture, string home, string away) =>
        fixture with { IsResolved = true, HomeLabel = home, AwayLabel = away };

    /// <summary>Serves one string to every request, whatever the address.</summary>
    private sealed class StaticContentHandler(string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content),
            });
    }
}

/// <summary>
/// An <see cref="IAccessDataProvider"/> that hands back exactly the fixtures a
/// test gives it, for the component tests that need a page rendered without the
/// real provider's schedule load in the way.
/// </summary>
internal sealed class StubAccessDataProvider(IReadOnlyList<Fixture> fixtures) : IAccessDataProvider
{
    public DateTime AsOfUtc { get; init; } = new(2026, 7, 3, 20, 31, 0, DateTimeKind.Utc);

    public Task<AccessResponse<Accreditation?>> GetAccreditationAsync(string credentialId) =>
        throw new NotSupportedException("No test needs an accreditation from this stub.");

    public Task<AccessResponse<IReadOnlyList<Change>>> GetChangesAsync(string credentialId) =>
        Task.FromResult(new AccessResponse<IReadOnlyList<Change>>([], AsOfUtc, true));

    public Task<AccessResponse<IReadOnlyList<Fixture>>> GetFixturesAsync() =>
        Task.FromResult(new AccessResponse<IReadOnlyList<Fixture>>(fixtures, AsOfUtc, true));

    public Task<AccessResponse<Fixture?>> GetFixtureAsync(int matchNumber) =>
        Task.FromResult(new AccessResponse<Fixture?>(
            fixtures.FirstOrDefault(fixture => fixture.MatchNumber == matchNumber), AsOfUtc, true));

    public Task<Change> RequestMatchAccessAsync(string credentialId, int matchNumber) =>
        throw new NotSupportedException("The write path is tested against the real provider.");

    public Task<Change> WithdrawRequestAsync(string credentialId, string changeId) =>
        throw new NotSupportedException("The write path is tested against the real provider.");

    /// <summary>
    /// This stub serves no changes, so nothing has ever been requested against
    /// it. Tests that need a real pending state use the real provider.
    /// </summary>
    public MatchAccessStatus GetMatchAccessStatus(string credentialId, int matchNumber) =>
        MatchAccessStatus.NotRequested;
}
