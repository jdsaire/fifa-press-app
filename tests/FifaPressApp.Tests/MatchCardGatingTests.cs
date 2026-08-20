using Bunit;
using FifaPressApp.Components;
using FifaPressApp.Models;
using FifaPressApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// What a fixture card offers, in each of the states a fixture can be in — and
/// the architectural rule underneath it: the card never decides.
/// </summary>
public class MatchCardGatingTests
{
    private static BunitContext NewContext()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context.WithLocale();
    }

    private static IRenderedComponent<MatchCard> Card(
        BunitContext context, bool isPlayed, int? slots, bool canRequest) =>
        context.Render<MatchCard>(parameters => parameters
            .Add(card => card.MatchNumber, 92)
            .Add(card => card.Name, "Round of 16 — teams not yet decided")
            .Add(card => card.Kickoff, new DateTime(2026, 7, 4, 16, 0, 0))
            .Add(card => card.TimeZoneLabel, "Central Time (CST, UTC-6)")
            .Add(card => card.Venue, "AT&T Stadium")
            .Add(card => card.City, "Dallas")
            .Add(card => card.Phase, "Round of 16")
            .Add(card => card.IsPlayed, isPlayed)
            .Add(card => card.SlotsRemaining, slots)
            .Add(card => card.CanRequest, canRequest));

    // ------------------------------------------------- the three-way matrix

    [Fact]
    public void APlayedFixtureOffersNoRequestPathInAnySessionState()
    {
        // The match is over. A disabled control would invite the question
        // "why", and there is no answer worth a tooltip.
        using var context = NewContext();

        foreach (var signedIn in new[] { true, false })
        {
            var card = Card(context, isPlayed: true, slots: null, canRequest: signedIn);

            Assert.Empty(card.FindAll("a[href^='request/']"));
            Assert.Empty(card.FindAll("a[href='record']"));

            // Details stay reachable: this card is the only route into a
            // fixture's own page, and eighty-eight of the hundred and four are
            // played ones.
            Assert.NotEmpty(card.FindAll("a[href='events/92']"));
        }
    }

    [Fact]
    public void AnUnplayedFixtureSignedOutOffersAWayInRatherThanARequest()
    {
        using var context = NewContext();

        var card = Card(context, isPlayed: false, slots: 7, canRequest: false);

        // Stated as an offer, not hidden and not a dead control.
        var link = card.Find("a[href='record']");
        Assert.Contains("Sign in", link.TextContent, StringComparison.OrdinalIgnoreCase);

        // And it points at the record, which is where the sign-in experience
        // now lives — never at a route of its own.
        Assert.Empty(card.FindAll("a[href='signin']"));
        Assert.Empty(card.FindAll("a[href^='request/']"));
    }

    [Fact]
    public void AnUnplayedFixtureWithSlotsSignedInOffersDetailsAndARequest()
    {
        using var context = NewContext();

        var card = Card(context, isPlayed: false, slots: 7, canRequest: true);

        Assert.NotEmpty(card.FindAll("a[href='events/92']"));
        Assert.NotEmpty(card.FindAll("a[href='request/92']"));
        Assert.Empty(card.FindAll("a[href='record']"));
    }

    [Fact]
    public void AnUnplayedSoldOutFixtureSignedInOffersDetailsOnlyAndSaysWhy()
    {
        using var context = NewContext();

        var card = Card(context, isPlayed: false, slots: 0, canRequest: true);

        Assert.NotEmpty(card.FindAll("a[href='events/92']"));
        Assert.Empty(card.FindAll("a[href^='request/']"));

        // The absent control is explained rather than left to be noticed.
        Assert.Equal("No slots available", card.Find(".match-card__slots").TextContent.Trim());
    }

    // ---------------------------------------------------------- the capacity

    [Fact]
    public void CapacityIsStatedWhereItAppliesAndAbsentWhereItDoesNot()
    {
        using var context = NewContext();

        Assert.Equal("7 slots available",
            Card(context, isPlayed: false, slots: 7, canRequest: true).Find(".match-card__slots").TextContent.Trim());

        // A played fixture has no slots to have run out of.
        Assert.Empty(Card(context, isPlayed: true, slots: null, canRequest: true).FindAll(".match-card__slots"));

        // Nor does one the provider handed over without a value.
        Assert.Empty(Card(context, isPlayed: false, slots: null, canRequest: true).FindAll(".match-card__slots"));
    }

    // ------------------------------------------------------ the architecture

    [Fact]
    public void TheCardInjectsNoSessionProviderAndCannotAnswerTheQuestionItself()
    {
        // The rule this whole component exists under. If the card could work
        // out who is signed in, that would be a second place where "may this
        // person request access" is decided, and the two would eventually
        // disagree. It renders with no session registered at all — which is
        // only possible because it never asks.
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.WithLocale();

        Assert.Null(context.Services.GetService<SimulatedSessionProvider>());

        var card = Card(context, isPlayed: false, slots: 7, canRequest: true);

        Assert.NotEmpty(card.FindAll("a[href='request/92']"));

        var source = File.ReadAllText(Path.Combine(
            TestPaths.SourceRoot(), "Components", "MatchCard.razor"));

        Assert.DoesNotContain("@inject SimulatedSessionProvider", source);
        Assert.DoesNotContain("IAccessDataProvider", source);
    }

    [Fact]
    public void TheCardCarriesNoEditAffordanceAndNoUnsavedChangesLine()
    {
        // What EventCard brought with it that a fixture has no business
        // carrying: an inline edit toggle over data that is not the reader's to
        // change, and a line apologising that the changes are not saved.
        using var context = NewContext();

        var card = Card(context, isPlayed: true, slots: null, canRequest: true);

        Assert.Empty(card.FindAll("input"));
        Assert.DoesNotContain("Edit", card.Markup);
        Assert.DoesNotContain("aren't saved", card.Markup);
    }

    [Fact]
    public void EventCardSurvivesInTheRepositoryWithItsBindingDemonstrationIntact()
    {
        // Deliberate, and recorded so a future reader does not "clean up" an
        // unreferenced component: EventCard is the app's only demonstration of
        // the X/XChanged + @bind + EventCallback pairing. It is retired from
        // /matches, not deleted.
        var eventCard = Path.Combine(TestPaths.SourceRoot(), "Components", "EventCard.razor");
        Assert.True(File.Exists(eventCard), "EventCard.razor was deleted rather than retired from /matches");

        var source = File.ReadAllText(eventCard);
        Assert.Contains("EventCallback<string> EventNameChanged", source);

        // And the page no longer reaches for it.
        var list = File.ReadAllText(Path.Combine(TestPaths.SourceRoot(), "Pages", "EventList.razor"));
        Assert.DoesNotContain("<EventCard", list);
    }
}
