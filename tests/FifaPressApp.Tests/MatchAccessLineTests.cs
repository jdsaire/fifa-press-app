using System.Reflection;
using Bunit;
using FifaPressApp.Models;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The pending change's id, threaded through <c>MatchAccessLine</c> — groundwork
/// for the withdraw affordance. Populated only on a <c>Requested</c> line,
/// since only a pending request has anything a withdrawal could target.
///
/// <para>
/// <c>MatchAccessLine</c> has no markup surface of its own yet — that arrives
/// with the withdraw button — so these tests reach the resolved line through
/// reflection on the rendered component rather than through rendered text.
/// </para>
/// </summary>
public class MatchAccessLineTests
{
    private static BunitContext NewContext(SimulatedSessionProvider session, IAccessDataProvider provider)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new ChangeArrivalTracker());
        context.Services.AddSingleton(session);
        context.Services.AddSingleton(provider);
        return context.WithLocale();
    }

    private static async Task<SimulatedSessionProvider> AsAminaAsync()
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync("demo_staff1", "amina-demo-2026");
        return session;
    }

    /// <summary>Every resolved <c>MatchAccessLine</c> on the rendered page, via reflection.</summary>
    private static IReadOnlyList<(int MatchNumber, MatchAccessStatus Status, string? PendingChangeId)> LinesOn(
        IRenderedComponent<MyAccess> page)
    {
        var field = typeof(MyAccess).GetField("matchAccess", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var lines = (System.Collections.IEnumerable)field.GetValue(page.Instance)!;

        var result = new List<(int, MatchAccessStatus, string?)>();
        foreach (var line in lines)
        {
            var type = line.GetType();
            var number = (int)type.GetProperty("MatchNumber")!.GetValue(line)!;
            var status = (MatchAccessStatus)type.GetProperty("Status")!.GetValue(line)!;
            var pendingChangeId = (string?)type.GetProperty("PendingChangeId")!.GetValue(line);
            result.Add((number, status, pendingChangeId));
        }

        return result;
    }

    [Fact]
    public async Task APendingRequestCarriesItsOwnChangeId()
    {
        var provider = TestData.ProviderOverRealSchedule();
        var written = await provider.RequestMatchAccessAsync(
            MockAccessDataProvider.AminaCredentialId, 77);

        using var context = NewContext(await AsAminaAsync(), provider);
        var page = context.Render<MyAccess>();

        var line = LinesOn(page).Single(l => l.MatchNumber == 77);

        Assert.Equal(MatchAccessStatus.Requested, line.Status);
        Assert.Equal(written.ChangeId, line.PendingChangeId);
    }

    [Fact]
    public async Task AGrantedOrRevokedLineCarriesNoChangeId()
    {
        // Amina's seeded ch-001 (match 1, MatchAccessGranted, already effective)
        // resolves to Granted and must carry no pending change id — there is
        // nothing on a decided line for a withdrawal to target.
        using var context = NewContext(await AsAminaAsync(), TestData.ProviderOverRealSchedule());
        var page = context.Render<MyAccess>();

        var line = LinesOn(page).Single(l => l.MatchNumber == 1);

        Assert.Equal(MatchAccessStatus.Granted, line.Status);
        Assert.Null(line.PendingChangeId);
    }
}
