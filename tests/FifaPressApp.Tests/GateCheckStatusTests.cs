using Bunit;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The Match Detail headline's own copy of the same defect
/// <see cref="MatchAccessStatusTests"/> covers on My Access — one level more
/// assertive in wording ("Access to this match has been withdrawn"), reached by
/// a second, equally natural navigation path.
/// </summary>
public class GateCheckStatusTests
{
    private static BunitContext NewContext(SimulatedSessionProvider session)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new ChangeArrivalTracker());
        context.Services.AddSingleton(session);
        context.Services.AddSingleton<IAccessDataProvider>(TestData.ProviderOverRealSchedule());
        return context.WithLocale();
    }

    private static async Task<SimulatedSessionProvider> AsAminaAsync()
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync("demo_staff1", "amina-demo-2026");
        return session;
    }

    [Fact]
    public async Task TheHeadlineStatusExcludesNotYetEffectiveChanges()
    {
        // /events/98 is Amina's own quarter-final entitlement, conditional on
        // ch-005 (effective 6 July, three days after SimulatedNow). The
        // headline used FoldStatus(DateTime.MaxValue), folding it in as though
        // already decided; it must now bound by Access.AsOfUtc instead.
        using var context = NewContext(await AsAminaAsync());

        var page = context.Render<EventDetails>(parameters => parameters.Add(p => p.Id, 98));

        Assert.DoesNotContain(
            "Access to this match has been withdrawn",
            page.Markup);
    }
}
