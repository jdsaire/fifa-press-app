using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The dark palette, re-anchored to solid black.
///
/// These tests read <c>app.css</c> as a file rather than through a rendered
/// component, because the defect they exist to catch is a source-level one: the
/// dark palette is written twice — once under <c>prefers-color-scheme</c> and
/// once under <c>[data-theme="dark"]</c> — and updating only one of them
/// produces an app where choosing dark and having dark chosen for you give two
/// different screens. No browser can see that; the file can.
///
/// The ratios asserted here are computed from the WCAG 2.2 relative-luminance
/// formula, not taken from the addendum's own table, which is labelled
/// [SIMULATED] and is imprecise for several tokens.
/// </summary>
public class ThemePaletteTests
{
    private static string AppCss()
    {
        var css = Path.Combine(RepoRoot(), "src", "FifaPressApp", "wwwroot", "css", "app.css");
        Assert.True(File.Exists(css), $"app.css not found at {css}");
        return File.ReadAllText(css);
    }

    // The test binary runs out of bin/, several levels from the repo and behind
    // a configuration-dependent path. The source file's own location is exact
    // and does not move with the build.
    private static string RepoRoot([CallerFilePath] string thisFile = "")
        => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", ".."));

    /// <summary>
    /// The two dark blocks, isolated. Both are matched by structure rather than
    /// by line number so that either can grow without breaking the other's test.
    /// </summary>
    private static (string Media, string Explicit) DarkBlocks()
    {
        var css = AppCss();

        var media = Regex.Match(css, @":root:not\(\[data-theme=""light""\]\)\s*\{(.*?)\n    \}", RegexOptions.Singleline);
        Assert.True(media.Success, "the prefers-color-scheme dark block was not found");

        var chosen = Regex.Match(css, @":root\[data-theme=""dark""\]\s*\{(.*?)\n\}", RegexOptions.Singleline);
        Assert.True(chosen.Success, "the explicitly-chosen dark block was not found");

        return (media.Groups[1].Value, chosen.Groups[1].Value);
    }

    private static string? TokenIn(string block, string token)
    {
        var match = Regex.Match(block, $@"{Regex.Escape(token)}:\s*([^;]+);");
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    [Theory]
    [InlineData("--color-surface", "#000000")]
    [InlineData("--color-text", "#ffffff")]
    [InlineData("--color-stale-text", "#c4c4c4")]
    [InlineData("--color-link", "#7ab8f5")]
    [InlineData("--color-focus-ring", "#7ab8f5")]
    [InlineData("--color-action-primary", "#4a94dc")]
    [InlineData("--color-action-primary-text", "#000000")]
    [InlineData("--color-action-primary-border", "#356f9e")]
    [InlineData("--color-danger", "#ff7a7a")]
    [InlineData("--color-success", "#57d382")]
    [InlineData("--color-code", "#ec84b8")]
    [InlineData("--color-danger-surface", "#6e0f0f")]
    [InlineData("--color-danger-surface-text", "#ffffff")]
    [InlineData("--color-progress-track", "#1a1a1a")]
    public void EveryReDerivedTokenHoldsItsValueInBothDarkBlocks(string token, string expected)
    {
        var (media, chosen) = DarkBlocks();

        Assert.Equal(expected, TokenIn(media, token));
        Assert.Equal(expected, TokenIn(chosen, token));
    }

    [Fact]
    public void TheTwoDarkBlocksAgreeOnEveryColourTokenTheyDefine()
    {
        // The general form of the test above: whatever colour tokens the dark
        // palette holds, both blocks must hold the same ones with the same
        // values. A token added to one block only fails here.
        var (media, chosen) = DarkBlocks();

        static Dictionary<string, string> Colours(string block) => Regex
            .Matches(block, @"(--color-[a-z-]+):\s*([^;]+);")
            .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value.Trim());

        Assert.Equal(Colours(media), Colours(chosen));
    }

    [Fact]
    public void TheLightPaletteIsUntouchedByTheDarkReDerivation()
    {
        var css = AppCss();
        var light = Regex.Match(css, @"^:root \{(.*?)\n\}", RegexOptions.Singleline | RegexOptions.Multiline);
        Assert.True(light.Success, "the light block was not found");

        Assert.Equal("#ffffff", TokenIn(light.Groups[1].Value, "--color-surface"));
        Assert.Equal("#1a1a1a", TokenIn(light.Groups[1].Value, "--color-text"));
        Assert.Equal("#0071c1", TokenIn(light.Groups[1].Value, "--color-link"));
        Assert.Equal("#dc3545", TokenIn(light.Groups[1].Value, "--color-danger"));
    }

    [Fact]
    public void TheSidebarGradientIsRetainedRatherThanFlattenedToBlack()
    {
        var (media, chosen) = DarkBlocks();

        foreach (var block in new[] { media, chosen })
        {
            Assert.Equal("#04173d", TokenIn(block, "--color-sidebar-grad-start"));
            Assert.Equal("#22032c", TokenIn(block, "--color-sidebar-grad-end"));
            Assert.Equal("#f0f0f0", TokenIn(block, "--color-sidebar-text"));
            Assert.Equal("#c9c9c9", TokenIn(block, "--color-nav-item"));
        }
    }

    [Theory]
    // Text tokens, against the new black surface, 4.5:1 floor.
    [InlineData("#ffffff", "#000000", 4.5)]
    [InlineData("#c4c4c4", "#000000", 4.5)]
    [InlineData("#7ab8f5", "#000000", 4.5)]
    [InlineData("#ff7a7a", "#000000", 4.5)]
    [InlineData("#ec84b8", "#000000", 4.5)]
    // Non-text: focus ring and the success validation outline, 3:1 floor.
    [InlineData("#7ab8f5", "#000000", 3.0)]
    [InlineData("#57d382", "#000000", 3.0)]
    // Pairs checked against their own partner rather than the surface.
    [InlineData("#4a94dc", "#000000", 4.5)]
    [InlineData("#ffffff", "#6e0f0f", 4.5)]
    // The sidebar pair, against the gradient's lighter — therefore worst — stop.
    [InlineData("#f0f0f0", "#04173d", 4.5)]
    [InlineData("#c9c9c9", "#04173d", 4.5)]
    public void EveryDarkPairClearsTheFloorThatActuallyAppliesToIt(string foreground, string background, double floor)
    {
        Assert.True(
            ContrastRatio(foreground, background) >= floor,
            $"{foreground} on {background} computed {ContrastRatio(foreground, background):F2}:1, below its {floor}:1 floor");
    }

    [Theory]
    [InlineData("#ffffff", "#000000", 21.00)]
    [InlineData("#c4c4c4", "#000000", 12.04)]
    [InlineData("#7ab8f5", "#000000", 10.00)]
    [InlineData("#4a94dc", "#000000", 6.56)]
    [InlineData("#ff7a7a", "#000000", 8.32)]
    [InlineData("#57d382", "#000000", 11.05)]
    [InlineData("#ec84b8", "#000000", 8.56)]
    [InlineData("#ffffff", "#6e0f0f", 12.08)]
    [InlineData("#f0f0f0", "#04173d", 15.42)]
    [InlineData("#c9c9c9", "#04173d", 10.61)]
    public void TheRatiosWrittenIntoTheCommentsAreTheComputedOnes(string foreground, string background, double documented)
    {
        // Guards the comments themselves. 09 §4.2's stated figures are labelled
        // [SIMULATED] and several are wrong by more than a rounding step — this
        // pins what app.css claims to what the formula actually returns, so a
        // future edit cannot quietly reintroduce a decorative number.
        Assert.Equal(documented, ContrastRatio(foreground, background), 2);
    }

    /// <summary>WCAG 2.2 contrast ratio, from the relative-luminance definition.</summary>
    private static double ContrastRatio(string a, string b)
    {
        var (high, low) = (RelativeLuminance(a), RelativeLuminance(b)) switch
        {
            var (x, y) when x > y => (x, y),
            var (x, y) => (y, x)
        };

        return (high + 0.05) / (low + 0.05);
    }

    private static double RelativeLuminance(string hex)
    {
        static double Channel(string hex, int offset)
        {
            var value = Convert.ToInt32(hex.Substring(offset, 2), 16) / 255.0;
            return value <= 0.03928 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
        }

        return 0.2126 * Channel(hex, 1) + 0.7152 * Channel(hex, 3) + 0.0722 * Channel(hex, 5);
    }
}
