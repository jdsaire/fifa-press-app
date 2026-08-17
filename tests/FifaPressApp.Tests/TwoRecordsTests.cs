using FifaPressApp.Models;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The second seeded record, and the contrast it exists to make visible.
///
/// <para>
/// The claim under test is narrow and worth stating exactly: the same change,
/// on two records, resolves to two different urgencies, and it does so with no
/// new logic in <c>Track</c> or <c>Change</c>. A test that only checked Tomás's
/// record renders would not be testing anything this record was added for.
/// </para>
/// </summary>
public class TwoRecordsTests
{
    private static MockAccessDataProvider NewProvider() => TestData.ProviderOverRealSchedule();

    [Fact]
    public async Task BothRecordsResolve()
    {
        var provider = NewProvider();

        var amina = (await provider.GetAccreditationAsync(MockAccessDataProvider.AminaCredentialId)).Value;
        var tomas = (await provider.GetAccreditationAsync(MockAccessDataProvider.TomasCredentialId)).Value;

        Assert.Equal("Amina Bello", amina!.HolderName);
        Assert.Equal("Tomás L.", tomas!.HolderName);
    }

    [Fact]
    public async Task AnUnknownCredentialStillResolvesToNothing()
    {
        // Two records, not "any credential works". The record screen's empty
        // state depends on this staying true.
        var provider = NewProvider();

        Assert.Null((await provider.GetAccreditationAsync("MP-2026-00000")).Value);
        Assert.Empty((await provider.GetChangesAsync("MP-2026-00000")).Value);
    }

    [Fact]
    public async Task NeitherRecordCanSeeTheOthersChanges()
    {
        var provider = NewProvider();

        var aminas = (await provider.GetChangesAsync(MockAccessDataProvider.AminaCredentialId)).Value;
        var tomass = (await provider.GetChangesAsync(MockAccessDataProvider.TomasCredentialId)).Value;

        Assert.All(aminas, change =>
            Assert.Equal(MockAccessDataProvider.AminaCredentialId, change.CredentialId));
        Assert.All(tomass, change =>
            Assert.Equal(MockAccessDataProvider.TomasCredentialId, change.CredentialId));

        Assert.Empty(aminas.Select(c => c.ChangeId).Intersect(tomass.Select(c => c.ChangeId)));
    }

    [Fact]
    public async Task TheOneVariableThatDiffersIsTheNamedContact()
    {
        // 10 §3.2's design: everything else stays as close to Amina's record as
        // the persona permits, so what a person observes when switching accounts
        // is attributable to the ceiling and not to six differences at once.
        var provider = NewProvider();

        var amina = (await provider.GetAccreditationAsync(MockAccessDataProvider.AminaCredentialId)).Value!;
        var tomas = (await provider.GetAccreditationAsync(MockAccessDataProvider.TomasCredentialId)).Value!;

        Assert.Equal(amina.Status, tomas.Status);
        Assert.Equal(amina.ValidUntil, tomas.ValidUntil);

        Assert.False(amina.Track.HasNamedContact);
        Assert.True(tomas.Track.HasNamedContact);

        Assert.Equal(NotificationCeiling.ImmediateAndForeseeable, amina.Track.NotificationCeiling);
        Assert.Equal(NotificationCeiling.ImmediateOnly, tomas.Track.NotificationCeiling);
    }

    [Fact]
    public async Task TheSameConditionalChangeIsSilentForTomasAndForeseeableForAmina()
    {
        // The demonstration, asserted at the level a person sees it.
        var provider = NewProvider();

        var hers = (await provider.GetChangesAsync(MockAccessDataProvider.AminaCredentialId)).Value
            .Single(change => change.ChangeId == "ch-005");
        var his = (await provider.GetChangesAsync(MockAccessDataProvider.TomasCredentialId)).Value
            .Single(change => change.ChangeId == "ch-008");

        // Structurally the same change.
        Assert.Equal(hers.Kind, his.Kind);
        Assert.Equal(hers.AffectsMatchNumber, his.AffectsMatchNumber);
        Assert.Equal(hers.DependsOnMatchNumber, his.DependsOnMatchNumber);

        // Resolving differently, and only because of the ceiling.
        Assert.Equal(Urgency.Foreseeable, hers.Urgency);
        Assert.Equal(Urgency.Silent, his.Urgency);
    }

    [Fact]
    public void TheContrastComesFromTheCeilingAlone()
    {
        // The same construction, differing only in the track, proves the seeded
        // data is not what produces the contrast. If this ever fails, the
        // demonstration has been faked somewhere upstream.
        static Change Build(Track track) => new(
            changeId: "ch-test",
            credentialId: "MP-2026-04817",
            writtenUtc: new DateTime(2026, 7, 3, 9, 45, 0, DateTimeKind.Utc),
            effectiveUtc: new DateTime(2026, 7, 6, 14, 0, 0, DateTimeKind.Utc),
            kind: ChangeKind.MatchAccessRevoked,
            track: track,
            whatChanged: "Something is conditional on an unplayed fixture.",
            reason: "The allocation contracts once that fixture settles it.",
            nextStep: "Wait for the fixture.",
            affectsMatchNumber: 98,
            dependsOnMatchNumber: 93,
            conditionText: "If it goes one way the access is withdrawn; if not, it stands.");

        Assert.Equal(
            Urgency.Foreseeable,
            Build(new Track(TrackId.MemberAssociationQuota, HasNamedContact: false)).Urgency);

        Assert.Equal(
            Urgency.Silent,
            Build(new Track(TrackId.RightsHolder, HasNamedContact: true)).Urgency);
    }

    [Fact]
    public async Task TheCeilingSuppressesTheForeseeableAndNotTheImmediate()
    {
        // Without this, "ImmediateOnly" could be read as "he is told nothing",
        // which is not what the ceiling means and not what his record does.
        var provider = NewProvider();

        var his = (await provider.GetChangesAsync(MockAccessDataProvider.TomasCredentialId)).Value;

        Assert.Equal(Urgency.Immediate, his.Single(change => change.ChangeId == "ch-007").Urgency);
        Assert.Equal(Urgency.Silent, his.Single(change => change.ChangeId == "ch-008").Urgency);
    }

    [Fact]
    public async Task TheWithholdingRuleHoldsAcrossTomassRecord()
    {
        // Every team name in the schedule, checked against every string his
        // record renders. The fixture ch-008 waits on has not been played, so
        // naming either team in its condition text would be the exact leak the
        // whole app exists to prevent.
        var provider = NewProvider();
        var teams = await LoadTeamNamesAsync();

        var his = (await provider.GetChangesAsync(MockAccessDataProvider.TomasCredentialId)).Value;

        foreach (var change in his)
        {
            var text = string.Join(
                " ",
                change.WhatChanged,
                change.Reason,
                change.NextStep,
                change.ConditionText,
                change.DecidedBy);

            foreach (var team in teams)
            {
                Assert.DoesNotContain(team, text, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Fact]
    public async Task TomassConditionalChangeNamesItsFixtureByRoundVenueAndDate()
    {
        var provider = NewProvider();

        var his = (await provider.GetChangesAsync(MockAccessDataProvider.TomasCredentialId)).Value
            .Single(change => change.ChangeId == "ch-008");

        // Saying which fixture without saying who is in it is the whole
        // technique — a person can act on "Dallas, 6 July" without being told a
        // result that does not exist yet.
        Assert.Contains("Round of 16", his.WhatChanged);
        Assert.Contains("Dallas", his.WhatChanged);
        Assert.Contains("6 July", his.WhatChanged);
        Assert.NotNull(his.ConditionText);
    }

    [Fact]
    public async Task TomassRecordIsHisOwnCredentialAndNotACrewRoster()
    {
        // 10 §3.3 prohibits this explicitly, and it is the most likely place for
        // scope to leak: he coordinates 40-120 credentials in reality and this
        // app shows exactly one.
        var provider = NewProvider();

        var record = (await provider.GetAccreditationAsync(MockAccessDataProvider.TomasCredentialId)).Value!;
        var his = (await provider.GetChangesAsync(MockAccessDataProvider.TomasCredentialId)).Value;

        Assert.Equal(MockAccessDataProvider.TomasCredentialId, record.CredentialId);
        Assert.All(his, change => Assert.Equal(record.CredentialId, change.CredentialId));

        // Nothing on the record talks about other people's credentials.
        foreach (var change in his)
        {
            var text = $"{change.WhatChanged} {change.Reason} {change.NextStep}";
            Assert.DoesNotContain("crew", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("your team's credentials", text, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task TheChangeCounterContinuesAcrossBothRecords()
    {
        // Eight seeded changes means the next written one is ch-009, with no new
        // counter logic. A collision here would put two changes under one id.
        var provider = NewProvider();

        var written = await provider.RequestMatchAccessAsync(
            MockAccessDataProvider.TomasCredentialId, 99);

        Assert.Equal("ch-009", written.ChangeId);
    }

    [Fact]
    public async Task AWriteToTomassRecordCarriesHisTrackAndNotHers()
    {
        // The subtle one. A write authored against the wrong track would give
        // his change her urgency, and nothing on screen would show it.
        var provider = NewProvider();

        var written = await provider.RequestMatchAccessAsync(
            MockAccessDataProvider.TomasCredentialId, 99);

        Assert.Equal(MockAccessDataProvider.TomasCredentialId, written.CredentialId);

        // A request decided within the immediate window interrupts either
        // holder, so the observable proof is that the change landed on his
        // record and reads back from it.
        var his = (await provider.GetChangesAsync(MockAccessDataProvider.TomasCredentialId)).Value;
        Assert.Contains(his, change => change.ChangeId == written.ChangeId);

        var hers = (await provider.GetChangesAsync(MockAccessDataProvider.AminaCredentialId)).Value;
        Assert.DoesNotContain(hers, change => change.ChangeId == written.ChangeId);
    }

    [Fact]
    public async Task AWriteAgainstACredentialThatDoesNotExistIsRefused()
    {
        var provider = NewProvider();

        await Assert.ThrowsAsync<ArgumentException>(
            () => provider.RequestMatchAccessAsync("MP-2026-00000", 99));
    }

    private static Task<IReadOnlyList<string>> LoadTeamNamesAsync()
    {
        var parsed = FixtureImporter.Parse(TestData.ScheduleCsv());

        IReadOnlyList<string> names = parsed.Matchups.Values
            .SelectMany(matchup => new[] { matchup.Home, matchup.Away })
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Task.FromResult(names);
    }
}
