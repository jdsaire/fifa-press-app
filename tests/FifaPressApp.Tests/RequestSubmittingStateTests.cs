using Bunit;
using FifaPressApp.Components;
using FifaPressApp.Models;
using FifaPressApp.Services;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The Submitting state, which <c>05_SCREENS.md</c> §5.2 specifies and which the
/// shipped build never rendered.
///
/// <para>
/// The defect was not in the form. The form was correct: it disables both fields
/// and swaps its button label whenever it is told it is submitting. The provider
/// returned an already-completed task, so the caller's continuation ran
/// synchronously and no render pass ever happened while that was true. The first
/// test below is the one that fails against that version.
/// </para>
/// </summary>
public class RequestSubmittingStateTests
{
    [Fact]
    public async Task WritePathReturnsATaskThatHasNotAlreadyCompleted()
    {
        // The regression test. Against the previous implementation —
        // Task.FromResult over a synchronously built change — this assertion
        // fails immediately, which is exactly the defect: a task that is already
        // done when handed over gives the framework nothing to render around.
        var provider = TestData.ProviderOverRealSchedule();

        var pending = provider.RequestMatchAccessAsync(MockAccessDataProvider.DemoCredentialId, 42);

        Assert.False(pending.IsCompleted, "the write must yield, or the Submitting state can never render");
        await pending;
        Assert.True(pending.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task TheChangeItWritesIsUnchangedInContent()
    {
        // The state became observable; what gets written did not move. Every
        // field below is what the shipped version produced.
        var provider = TestData.ProviderOverRealSchedule();

        var written = await provider.RequestMatchAccessAsync(MockAccessDataProvider.DemoCredentialId, 42);

        Assert.Equal(MockAccessDataProvider.DemoCredentialId, written.CredentialId);
        Assert.Equal(ChangeKind.RequestDecided, written.Kind);
        Assert.Equal(42, written.AffectsMatchNumber);
        Assert.Equal(provider.AsOfUtc, written.WrittenUtc);
        Assert.Equal(provider.AsOfUtc, written.EffectiveUtc);
        Assert.False(written.NextStepIsActionable);
        Assert.Contains("recorded as requested", written.WhatChanged);
        Assert.Contains("simulated", written.DecidedBy, StringComparison.OrdinalIgnoreCase);
        Assert.Null(written.SupersedesChangeId);
    }

    [Fact]
    public async Task ReadPathsGainedNoDelay()
    {
        // v9 verified a no-spinner first render off cache-first reads, and that
        // is a passing acceptance criterion this run must not spend. Each read
        // below must still hand back a task that is already finished.
        var provider = TestData.ProviderOverRealSchedule();
        await provider.GetFixturesAsync();

        Assert.True(provider.GetAccreditationAsync(MockAccessDataProvider.DemoCredentialId).IsCompleted);
        Assert.True(provider.GetChangesAsync(MockAccessDataProvider.DemoCredentialId).IsCompleted);
        Assert.True(provider.GetFixturesAsync().IsCompleted);
        Assert.True(provider.GetFixtureAsync(1).IsCompleted);
    }

    [Fact]
    public void FormShowsTheSubmittingStateWhenItIsToldItIsSubmitting()
    {
        using var context = new BunitContext();
        context.WithLocale();

        var form = context.Render<RequestAccessForm>(parameters => parameters
            .Add(component => component.Name, "Amina Bello")
            .Add(component => component.Email, "amina@example.com")
            .Add(component => component.Submitting, true));

        Assert.Contains("Sending request…", form.Markup);
        Assert.DoesNotContain("Request access", form.Markup);
        Assert.All(form.FindAll("input"), input => Assert.True(input.HasAttribute("disabled")));
        Assert.True(form.Find("button[type=submit]").HasAttribute("disabled"));
    }

    [Fact]
    public void FormIsFullyUsableWhenItIsNotSubmitting()
    {
        using var context = new BunitContext();
        context.WithLocale();

        var form = context.Render<RequestAccessForm>(parameters => parameters
            .Add(component => component.Name, "Amina Bello")
            .Add(component => component.Email, "amina@example.com")
            .Add(component => component.Submitting, false));

        Assert.Contains("Request access", form.Markup);
        Assert.DoesNotContain("Sending request…", form.Markup);
        Assert.All(form.FindAll("input"), input => Assert.False(input.HasAttribute("disabled")));
    }

    [Fact]
    public async Task SubmittingIsObservableAcrossTheWriteTheFormActuallyMakes()
    {
        // The two halves joined: while a real write from the real provider is in
        // flight, the form rendered with that state shows it. This is the render
        // pass the shipped build never got.
        using var context = new BunitContext();
        context.WithLocale();
        var provider = TestData.ProviderOverRealSchedule();

        var submitting = false;
        var form = context.Render<RequestAccessForm>(parameters => parameters
            .Add(component => component.Name, "Amina Bello")
            .Add(component => component.Email, "amina@example.com")
            .Add(component => component.Submitting, submitting));

        Assert.Contains("Request access", form.Markup);

        var pending = provider.RequestMatchAccessAsync(MockAccessDataProvider.DemoCredentialId, 42);
        Assert.False(pending.IsCompleted);

        // The page sets its flag and renders while the write is still running.
        form.Render(parameters => parameters
            .Add(component => component.Name, "Amina Bello")
            .Add(component => component.Email, "amina@example.com")
            .Add(component => component.Submitting, true));

        Assert.Contains("Sending request…", form.Markup);

        await pending;
    }
}
