using FifaPressApp.Models;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The provider's two standing promises: it withholds, and it only ever appends.
/// </summary>
public class MockAccessDataProviderTests
{
    [Fact]
    public async Task NoUnplayedFixtureCarriesATeamName_AcrossTheWholeSchedule()
    {
        // The single highest-value assertion in this project. The schedule is a
        // record of a completed tournament, so a fixture that leaked its sides
        // before kickoff would be telling the reader who won a match nobody has
        // played — and an app that knows the future has no reason to warn anyone
        // about anything, which is the entire concept inverted.
        var provider = TestData.ProviderOverRealSchedule();

        var fixtures = (await provider.GetFixturesAsync()).Value;
        Assert.Equal(TestData.ScheduleRowCount, fixtures.Count);

        Assert.All(fixtures.Where(fixture => !fixture.IsResolved), fixture =>
        {
            Assert.Null(fixture.HomeLabel);
            Assert.Null(fixture.AwayLabel);
            Assert.DoesNotContain(" v ", fixture.DisplayLabel);
            Assert.EndsWith("teams not yet decided", fixture.DisplayLabel);
        });

        // Asserted rather than assumed: the run would otherwise pass vacuously
        // if every fixture happened to be resolved.
        Assert.Contains(fixtures, fixture => !fixture.IsResolved);
        Assert.Contains(fixtures, fixture => fixture.IsResolved);
    }

    [Fact]
    public async Task SingleFixtureRead_WithholdsOnTheSameRule_AcrossTheWholeSchedule()
    {
        // GetFixturesAsync and GetFixtureAsync are two read paths, and a rule
        // held on only one of them is not held.
        var provider = TestData.ProviderOverRealSchedule();

        for (var matchNumber = 1; matchNumber <= TestData.ScheduleRowCount; matchNumber++)
        {
            var fixture = (await provider.GetFixtureAsync(matchNumber)).Value;
            Assert.NotNull(fixture);

            if (!fixture.IsResolved)
            {
                Assert.Null(fixture.HomeLabel);
                Assert.Null(fixture.AwayLabel);
            }
        }
    }

    [Fact]
    public async Task PlayedFixture_NamesItsSides()
    {
        // The rule is withholding, not blanking. A fixture that has kicked off
        // reads normally, and a test suite that only proved the null case would
        // pass against a provider that returned nothing useful at all.
        var provider = TestData.ProviderOverRealSchedule();

        var fixtures = (await provider.GetFixturesAsync()).Value;
        var played = fixtures.Where(fixture => fixture.IsResolved).ToList();

        Assert.NotEmpty(played);
        Assert.All(played, fixture =>
        {
            Assert.NotNull(fixture.HomeLabel);
            Assert.NotNull(fixture.AwayLabel);
            Assert.Contains(" v ", fixture.DisplayLabel);
        });
    }

    [Fact]
    public async Task RequestAppendsExactlyOneChange_AndReturnsIt()
    {
        var provider = TestData.ProviderOverRealSchedule();
        var before = (await provider.GetChangesAsync(MockAccessDataProvider.DemoCredentialId)).Value.Count;

        var written = await provider.RequestMatchAccessAsync(MockAccessDataProvider.DemoCredentialId, 42);

        var after = (await provider.GetChangesAsync(MockAccessDataProvider.DemoCredentialId)).Value;
        Assert.Equal(before + 1, after.Count);
        Assert.Contains(after, change => change.ChangeId == written.ChangeId);
        Assert.Equal(ChangeKind.RequestDecided, written.Kind);
        Assert.Equal(42, written.AffectsMatchNumber);
    }

    [Fact]
    public async Task WithdrawalAppends_AndLeavesTheWithdrawnChangeInPlace()
    {
        // A withdrawal is a write, not an undo. A record that erases what it
        // replaces is not a record.
        var provider = TestData.ProviderOverRealSchedule();
        var request = await provider.RequestMatchAccessAsync(MockAccessDataProvider.DemoCredentialId, 42);
        var before = (await provider.GetChangesAsync(MockAccessDataProvider.DemoCredentialId)).Value.Count;

        var withdrawal = await provider.WithdrawRequestAsync(
            MockAccessDataProvider.DemoCredentialId, request.ChangeId);

        var after = (await provider.GetChangesAsync(MockAccessDataProvider.DemoCredentialId)).Value;
        Assert.Equal(before + 1, after.Count);
        Assert.Equal(ChangeKind.Withdrawal, withdrawal.Kind);
        Assert.Equal(request.ChangeId, withdrawal.SupersedesChangeId);
        Assert.Contains(after, change => change.ChangeId == request.ChangeId);
    }

    [Fact]
    public async Task WithdrawingAChangeThatDoesNotExist_IsRejected()
    {
        var provider = TestData.ProviderOverRealSchedule();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            provider.WithdrawRequestAsync(MockAccessDataProvider.DemoCredentialId, "ch-999"));
    }

    [Fact]
    public async Task ChangesComeBackOrderedByEffectiveDate_NewestFirst()
    {
        // By when each change takes effect, not when it was written: a change
        // landing on Saturday belongs above one written later that lands next
        // month. The seeded record contains exactly that case.
        var provider = TestData.ProviderOverRealSchedule();

        var changes = (await provider.GetChangesAsync(MockAccessDataProvider.DemoCredentialId)).Value;

        Assert.NotEmpty(changes);
        Assert.Equal(
            changes.Select(change => change.EffectiveUtc).OrderByDescending(effective => effective),
            changes.Select(change => change.EffectiveUtc));
    }

    [Fact]
    public async Task ChangesAreScopedToTheCredentialAsked_For()
    {
        var provider = TestData.ProviderOverRealSchedule();

        var mine = (await provider.GetChangesAsync(MockAccessDataProvider.DemoCredentialId)).Value;
        var someoneElse = (await provider.GetChangesAsync("MP-2026-00000")).Value;

        Assert.NotEmpty(mine);
        Assert.Empty(someoneElse);
    }

    [Fact]
    public async Task EveryReadReportsItsOwnFreshness()
    {
        var provider = TestData.ProviderOverRealSchedule();

        var fixtures = await provider.GetFixturesAsync();
        var accreditation = await provider.GetAccreditationAsync(MockAccessDataProvider.DemoCredentialId);

        Assert.True(fixtures.WasServedFromCache);
        Assert.True(accreditation.WasServedFromCache);
        Assert.NotEqual(default, fixtures.LastSyncedUtc);
        Assert.True(fixtures.LastSyncedUtc < provider.AsOfUtc, "the record is synced before 'now', so staleness is visible");
    }
}
