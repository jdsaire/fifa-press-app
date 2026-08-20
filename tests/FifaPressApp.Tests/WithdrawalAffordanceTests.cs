using Bunit;
using FifaPressApp.Pages;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The withdraw affordance — <c>UX-MAJ-06</c>, open since v9, closed here for
/// the first time. A pending request can be withdrawn from My Access itself,
/// gated strictly to a <c>Requested</c> line, with an inline (not modal)
/// confirm step before the write actually happens.
/// </summary>
public class WithdrawalAffordanceTests
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

    [Fact]
    public async Task APendingRequestCanBeWithdrawnFromMyAccess()
    {
        var provider = TestData.ProviderOverRealSchedule();
        await provider.RequestMatchAccessAsync(MockAccessDataProvider.AminaCredentialId, 77);

        using var context = NewContext(await AsAminaAsync(), provider);
        var page = context.Render<MyAccess>();

        page.Find("button.btn-link").Click();
        page.Find(".my-access__withdraw-confirm button.btn-primary").Click();

        // The line for match 77 no longer reads as pending — the withdrawal
        // superseded the request, and StatusFor now resolves NotRequested for
        // whatever remains visible on that match.
        Assert.DoesNotContain("Requested, not yet decided", page.Markup);
    }

    [Fact]
    public async Task WithdrawingRequiresConfirmationFirst()
    {
        var provider = TestData.ProviderOverRealSchedule();
        var written = await provider.RequestMatchAccessAsync(MockAccessDataProvider.AminaCredentialId, 77);

        using var context = NewContext(await AsAminaAsync(), provider);
        var page = context.Render<MyAccess>();

        // Clicking the withdraw button alone only opens the confirm step; it
        // must not itself call WithdrawRequestAsync.
        page.Find("button.btn-link").Click();

        var changesAfterOpeningConfirm = (await provider.GetChangesAsync(
            MockAccessDataProvider.AminaCredentialId)).Value;
        Assert.DoesNotContain(changesAfterOpeningConfirm, c => c.SupersedesChangeId == written.ChangeId);
    }

    [Fact]
    public async Task AGrantedOrRevokedLineOffersNoWithdrawControl()
    {
        // Amina's seeded ch-001 (match 1, Granted) and ch-005 (match 98,
        // conditional — NotRequested once Item 1's fix excludes it) both have
        // nothing pending; neither offers a withdraw control.
        using var context = NewContext(await AsAminaAsync(), TestData.ProviderOverRealSchedule());
        var page = context.Render<MyAccess>();

        Assert.Empty(page.FindAll("button.btn-link"));
        Assert.DoesNotContain("Withdraw request", page.Markup);
    }

    [Fact]
    public async Task TheWithdrawnChangeAppearsAsANewJustArrivedEntry()
    {
        var provider = TestData.ProviderOverRealSchedule();
        await provider.RequestMatchAccessAsync(MockAccessDataProvider.AminaCredentialId, 77);

        using var context = NewContext(await AsAminaAsync(), provider);
        var page = context.Render<MyAccess>();

        page.Find("button.btn-link").Click();
        page.Find(".my-access__withdraw-confirm button.btn-primary").Click();

        // JustArrived opens the row and carries the aria-live announcement
        // Item 2 introduced — one confirmation pattern for the whole app.
        var openDisclosures = page.FindAll("details.change-row__disclosure[open]");
        Assert.NotEmpty(openDisclosures);
        Assert.NotEmpty(page.FindAll("[aria-live='polite']"));
    }

    [Fact]
    public async Task TheOriginalRequestStaysVisibleAfterWithdrawal()
    {
        var provider = TestData.ProviderOverRealSchedule();
        var written = await provider.RequestMatchAccessAsync(MockAccessDataProvider.AminaCredentialId, 77);

        using var context = NewContext(await AsAminaAsync(), provider);
        var page = context.Render<MyAccess>();

        page.Find("button.btn-link").Click();
        page.Find(".my-access__withdraw-confirm button.btn-primary").Click();

        // The CH-3/CH-7 non-deletion convention: the request is superseded,
        // never deleted, so its content stays reachable inside the new row's
        // disclosure as the value the withdrawal replaced.
        var changes = (await provider.GetChangesAsync(MockAccessDataProvider.AminaCredentialId)).Value;
        Assert.Contains(changes, c => c.ChangeId == written.ChangeId);
        Assert.Contains(changes, c => c.SupersedesChangeId == written.ChangeId);
    }
}
