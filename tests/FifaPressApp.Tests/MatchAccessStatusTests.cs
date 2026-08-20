using Bunit;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The per-match status word on My Access, and the one thing it must never do:
/// state a conditional change as though it were decided.
///
/// <para>
/// <c>StatusFor</c> used to fold in every change affecting a match regardless of
/// whether it had taken effect, so a still-conditional <c>ch-005</c>/<c>ch-008</c>
/// rendered identically to an already-decided revocation. The fix bounds the
/// fold by <c>Access.AsOfUtc</c>, the same "now" the rest of the app already
/// treats as authoritative.
/// </para>
/// </summary>
public class MatchAccessStatusTests
{
    private static BunitContext NewContext()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context.WithLocale();
    }

    private static async Task<SimulatedSessionProvider> SignedInAsync(string identifier, string password)
    {
        var session = new SimulatedSessionProvider(new DemoAccountStore());
        await session.SignInAsync(identifier, password);
        return session;
    }

    private static Task<SimulatedSessionProvider> AsAminaAsync() =>
        SignedInAsync("demo_staff1", "amina-demo-2026");

    private static Task<SimulatedSessionProvider> AsTomasAsync() =>
        SignedInAsync("demo_staff2", "tomas-demo-2026");

    private static BunitContext Configured(SimulatedSessionProvider session)
    {
        var context = NewContext();
        context.Services.AddSingleton(new DemoAccountStore());
        context.Services.AddSingleton(new ChangeArrivalTracker());
        context.Services.AddSingleton(session);
        context.Services.AddSingleton<IAccessDataProvider>(TestData.ProviderOverRealSchedule());
        return context;
    }

    [Fact]
    public async Task AForeseeableChangeDoesNotRenderAsADecidedStatus()
    {
        // Amina's ch-005: MatchAccessRevoked, affects match 98, effective 6 July
        // — three days after SimulatedNow (3 July). Before the fix this rendered
        // "Access withdrawn"; it must not, while it is still conditional.
        using var context = Configured(await AsAminaAsync());

        var markup = context.Render<MyAccess>().Markup;

        Assert.DoesNotContain("Access withdrawn", markup);
    }

    [Fact]
    public async Task ASilentChangeDoesNotRenderAsADecidedStatusEither()
    {
        // Tomás's ch-008: same shape, same match, but Silent rather than
        // Foreseeable for him — the defect reproduced regardless of which
        // urgency band produced it, since StatusFor never looked at either one.
        using var context = Configured(await AsTomasAsync());

        var markup = context.Render<MyAccess>().Markup;

        Assert.DoesNotContain("Access withdrawn", markup);
    }

    [Fact]
    public async Task AnAlreadyEffectiveChangeStillRendersItsDecidedStatus()
    {
        // Regression guard. Amina's ch-001 (MatchAccessGranted, match 1,
        // effective 11 June — well before SimulatedNow) must still resolve to
        // "Access granted": the fix bounds the fold by AsOfUtc, it does not
        // suppress every status word.
        using var context = Configured(await AsAminaAsync());

        var markup = context.Render<MyAccess>().Markup;

        Assert.Contains("Access granted", markup);
    }
}
