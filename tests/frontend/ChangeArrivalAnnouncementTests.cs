using Bunit;
using FifaPressApp.Components;
using FifaPressApp.Models;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The confirmation moment a just-arrived row gives a sighted user — the row
/// opening and animating in — has no counterpart for assistive technology.
/// This is the narrower half of the original UX-MAJ-05 gap: the error side
/// already carries <c>aria-live</c> elsewhere in the app; the success side did
/// not, until now.
/// </summary>
public class ChangeArrivalAnnouncementTests
{
    private static BunitContext NewContext()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(LocaleTestData.Loaded());
        context.Services.AddSingleton(new DemoAccountStore());
        context.RenderTree.Add<LocaleProvider>();
        return context;
    }

    private static Change Conditional() => new(
        changeId: "ch-test",
        credentialId: "MP-2026-04817",
        writtenUtc: new DateTime(2026, 7, 3, 9, 0, 0, DateTimeKind.Utc),
        effectiveUtc: new DateTime(2026, 7, 6, 14, 0, 0, DateTimeKind.Utc),
        kind: ChangeKind.MatchAccessRevoked,
        track: new Track(TrackId.MemberAssociationQuota, HasNamedContact: false),
        whatChanged: new LocalizedText("Something changed.", "Algo cambió.", "Algo mudou."),
        reason: new LocalizedText("Because of a quota.", "Por un cupo.", "Por causa de uma cota."),
        nextStep: new LocalizedText("Hold your travel.", "Espera para viajar.", "Aguarde para viajar."),
        affectsMatchNumber: 98);

    [Fact]
    public void AJustArrivedRowIsAnnouncedToAssistiveTechnology()
    {
        using var context = NewContext();
        var change = Conditional();

        var row = context.Render<ChangeRow>(parameters => parameters
            .Add(component => component.Change, change)
            .Add(component => component.JustArrived, true));

        var announcement = row.Find("[aria-live='polite']");
        Assert.Contains("Something changed.", announcement.TextContent);
    }
}
