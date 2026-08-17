using FifaPressApp.Models;
using FifaPressApp.Services;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// Record content in three languages, and the validation that keeps it whole.
/// </summary>
public class LocalizedChangeTests
{
    private static LocalizedText Text(string prefix) =>
        new($"{prefix} en", $"{prefix} es", $"{prefix} pt");

    private static Change Build(
        LocalizedText? whatChanged = null,
        LocalizedText? reason = null,
        LocalizedText? nextStep = null,
        bool nextStepIsActionable = true,
        LocalizedText? decidedBy = null,
        int? dependsOnMatchNumber = null,
        LocalizedText? conditionText = null) =>
        new(
            changeId: "ch-test",
            credentialId: "MP-2026-04817",
            writtenUtc: new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            effectiveUtc: new DateTime(2026, 7, 2, 0, 0, 0, DateTimeKind.Utc),
            kind: ChangeKind.ZoneAccessWidened,
            track: new Track(TrackId.Freelance, HasNamedContact: false),
            whatChanged: whatChanged ?? Text("what"),
            reason: reason ?? Text("why"),
            nextStep: nextStep ?? Text("next"),
            nextStepIsActionable: nextStepIsActionable,
            decidedBy: decidedBy,
            dependsOnMatchNumber: dependsOnMatchNumber,
            conditionText: conditionText);

    [Theory]
    [InlineData(AppLocale.En, "what en")]
    [InlineData(AppLocale.Es, "what es")]
    [InlineData(AppLocale.Pt, "what pt")]
    public void EachFieldResolvesToTheAskedForLanguage(AppLocale locale, string expected)
    {
        Assert.Equal(expected, Build().WhatChangedText.For(locale));
    }

    [Fact]
    public void TheStringAccessorsAreTheEnglishCanonicalForm()
    {
        // Not a rendering path, and deliberately not an ambient read of some
        // "current locale". An ambient read compiles just as well and lets a
        // component render localized text without joining the render pass a
        // locale change triggers — a stale string nobody sees until they
        // switch language. It also leaked across parallel test classes when it
        // was tried.
        var change = Build();

        Assert.Equal("what en", change.WhatChanged);
        Assert.Equal("why en", change.Reason);
        Assert.Equal("next en", change.NextStep);
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public void ARequiredFieldMissingInAnyOneLanguageIsRefused(AppLocale missing)
    {
        // Per locale, not just in English. A half-translated change would
        // otherwise construct fine and render a blank line the moment somebody
        // switched language — a defect that only appears to the people it
        // affects.
        var half = new LocalizedText(
            En: missing == AppLocale.En ? "   " : "what en",
            Es: missing == AppLocale.Es ? "   " : "what es",
            Pt: missing == AppLocale.Pt ? "   " : "what pt");

        Assert.Throws<ArgumentException>(() => Build(whatChanged: half));
    }

    [Fact]
    public void AReasonThatRestatesTheOutcomeIsRefusedInEveryLanguage()
    {
        // The Spanish reason must not restate the Spanish outcome either — a
        // check applied to English alone would let two thirds of the record
        // through.
        var what = Text("the same thing");
        var reasonEchoingSpanishOnly = new LocalizedText(
            En: "a genuinely different explanation",
            Es: "the same thing es",
            Pt: "outra explicação diferente");

        Assert.Throws<ArgumentException>(() => Build(whatChanged: what, reason: reasonEchoingSpanishOnly));
    }

    [Fact]
    public void ADeadEndStillHasToNameWhoDecided_InEveryLanguage()
    {
        Assert.Throws<ArgumentException>(() => Build(nextStepIsActionable: false));

        Assert.Throws<ArgumentException>(() => Build(
            nextStepIsActionable: false,
            decidedBy: new LocalizedText("Someone", "", "Alguém")));

        // Complete in all three: fine.
        var change = Build(nextStepIsActionable: false, decidedBy: Text("decided by"));
        Assert.Equal("decided by es", change.DecidedByText!.For(AppLocale.Es));
    }

    [Fact]
    public void AConditionalChangeStillHasToStateItsCondition_InEveryLanguage()
    {
        Assert.Throws<ArgumentException>(() => Build(dependsOnMatchNumber: 93));

        Assert.Throws<ArgumentException>(() => Build(
            dependsOnMatchNumber: 93,
            conditionText: new LocalizedText("If x then y", "Si x entonces y", "  ")));
    }

    [Fact]
    public async Task DecidedByIsLocalizedEvenThoughR5DoesNotListIt()
    {
        // 11 §4.2's R5 names four fields. DecidedBy is a fifth and it is
        // user-visible — ChangeRow renders "Decided by …" — so leaving it in
        // English would fail the requirement that every user-visible string
        // render in all three locales.
        var provider = TestData.ProviderOverRealSchedule();

        var change = (await provider.GetChangesAsync(MockAccessDataProvider.AminaCredentialId)).Value
            .Single(c => c.ChangeId == "ch-003");

        Assert.NotNull(change.DecidedByText);
        Assert.Equal("Oficina de acreditaciones de la ciudad sede", change.DecidedByText.For(AppLocale.Es));
    }

    // ------------------------------------------------- the seeded record content

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public async Task EverySeededChangeIsCompleteInEveryLanguage(AppLocale locale)
    {
        var provider = TestData.ProviderOverRealSchedule();

        foreach (var credential in new[]
                 {
                     MockAccessDataProvider.AminaCredentialId,
                     MockAccessDataProvider.TomasCredentialId,
                 })
        {
            foreach (var change in (await provider.GetChangesAsync(credential)).Value)
            {
                Assert.False(string.IsNullOrWhiteSpace(change.WhatChangedText.For(locale)));
                Assert.False(string.IsNullOrWhiteSpace(change.ReasonText.For(locale)));
                Assert.False(string.IsNullOrWhiteSpace(change.NextStepText.For(locale)));

                if (change.ConditionTextLocalized is not null)
                {
                    Assert.False(string.IsNullOrWhiteSpace(change.ConditionTextLocalized.For(locale)));
                }
            }
        }
    }

    [Fact]
    public async Task NoSeededChangeCarriesTheSameSentenceInAllThreeLanguages()
    {
        var provider = TestData.ProviderOverRealSchedule();

        foreach (var change in (await provider.GetChangesAsync(MockAccessDataProvider.TomasCredentialId)).Value)
        {
            var text = change.WhatChangedText;
            Assert.False(
                text.En == text.Es && text.En == text.Pt,
                $"{change.ChangeId} reads identically in all three languages");
        }
    }

    [Theory]
    [InlineData(AppLocale.En)]
    [InlineData(AppLocale.Es)]
    [InlineData(AppLocale.Pt)]
    public async Task TheWithholdingRuleHoldsInEveryLanguage(AppLocale locale)
    {
        // A second language is a second chance to leak a name the English text
        // was careful not to say.
        var provider = TestData.ProviderOverRealSchedule();
        var teams = FixtureImporter.Parse(TestData.ScheduleCsv()).Matchups.Values
            .SelectMany(matchup => new[] { matchup.Home, matchup.Away })
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var credential in new[]
                 {
                     MockAccessDataProvider.AminaCredentialId,
                     MockAccessDataProvider.TomasCredentialId,
                 })
        {
            foreach (var change in (await provider.GetChangesAsync(credential)).Value)
            {
                var text = string.Join(" ",
                    change.WhatChangedText.For(locale),
                    change.ReasonText.For(locale),
                    change.NextStepText.For(locale),
                    change.ConditionTextLocalized?.For(locale) ?? string.Empty);

                foreach (var team in teams)
                {
                    Assert.DoesNotContain(team, text, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }

    [Fact]
    public async Task SwitchingLanguageChangesNoChangesIdentityOrderOrCount()
    {
        // 11 §4.4. A change written in three languages is still one change:
        // the log's identity, ordering and length are locale-independent, and
        // a switch must never appear to alter, duplicate or reorder it.
        var provider = TestData.ProviderOverRealSchedule();

        var changes = (await provider.GetChangesAsync(MockAccessDataProvider.AminaCredentialId)).Value;

        var identities = changes.Select(change => change.ChangeId).ToList();
        var urgencies = changes.Select(change => change.Urgency).ToList();

        // Nothing about reading the other two languages can move any of this,
        // because the language is a parameter to a lookup and not state.
        foreach (var locale in new[] { AppLocale.Es, AppLocale.Pt })
        {
            foreach (var change in changes)
            {
                _ = change.WhatChangedText.For(locale);
            }
        }

        var after = (await provider.GetChangesAsync(MockAccessDataProvider.AminaCredentialId)).Value;

        Assert.Equal(identities, after.Select(change => change.ChangeId));
        Assert.Equal(urgencies, after.Select(change => change.Urgency));
        Assert.Equal(changes.Count, after.Count);
    }

    // ------------------------------------------------------- the write path

    [Theory]
    [InlineData(AppLocale.En, "Access to match 42")]
    [InlineData(AppLocale.Es, "El acceso al partido 42")]
    [InlineData(AppLocale.Pt, "O acesso ao jogo 42")]
    public async Task AChangeWrittenAtRuntimeIsAuthoredInAllThreeLanguages(AppLocale locale, string expected)
    {
        // Written in all three at write time, not resolved at render time: a
        // request made in Spanish still reads correctly after a switch to
        // Portuguese.
        var provider = TestData.ProviderOverRealSchedule();

        var written = await provider.RequestMatchAccessAsync(
            MockAccessDataProvider.AminaCredentialId, 42);

        Assert.Contains(expected, written.WhatChangedText.For(locale));
    }

    [Fact]
    public async Task AWithdrawalIsAuthoredInAllThreeLanguagesToo()
    {
        var provider = TestData.ProviderOverRealSchedule();

        var request = await provider.RequestMatchAccessAsync(
            MockAccessDataProvider.AminaCredentialId, 42);
        var withdrawal = await provider.WithdrawRequestAsync(
            MockAccessDataProvider.AminaCredentialId, request.ChangeId);

        Assert.Contains("42", withdrawal.WhatChangedText.For(AppLocale.Es));
        Assert.Contains("42", withdrawal.WhatChangedText.For(AppLocale.Pt));
        Assert.NotEqual(
            withdrawal.WhatChangedText.For(AppLocale.Es),
            withdrawal.WhatChangedText.For(AppLocale.Pt));
    }
}
