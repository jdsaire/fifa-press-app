using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace FifaPressApp.Tests;

/// <summary>
/// The interop layer: its TypeScript sources, and the compiled JavaScript the
/// app actually loads.
///
/// <para>
/// <b>What these tests are for.</b> The compiled output is committed rather than
/// built in CI, which keeps Node out of the deployment workflow and out of a
/// local <c>dotnet run</c> — at the cost that the <c>.ts</c> and the <c>.js</c>
/// can drift apart if somebody edits the output directly. These tests are that
/// cost's mitigation: they assert that every function the TypeScript exports
/// exists in the compiled JavaScript, and that the two storage keys agree.
/// </para>
///
/// <para>
/// They deliberately do not execute the JavaScript. There is no JavaScript
/// engine in this test project, and adding one to assert that a six-line
/// localStorage wrapper works would be a large dependency bought for a small
/// claim. What can go wrong here without anybody noticing is drift, and drift
/// is what is checked.
/// </para>
/// </summary>
public class InteropTests
{
    private static string RepoRoot([CallerFilePath] string thisFile = "")
        => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", ".."));

    private static string Source(string name) =>
        File.ReadAllText(Path.Combine(RepoRoot(), "src", "interop", "src", name));

    private static string Compiled(string name) =>
        File.ReadAllText(Path.Combine(RepoRoot(), "src", "FifaPressApp", "wwwroot", "js", name));

    private static IReadOnlyList<string> ExportedFunctions(string code) =>
        Regex.Matches(code, @"export function (\w+)")
            .Select(match => match.Groups[1].Value)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

    [Theory]
    [InlineData("theme.ts", "theme.js")]
    [InlineData("locale.ts", "locale.js")]
    public void EveryFunctionTheTypeScriptExportsIsInTheCompiledJavaScript(string source, string compiled)
    {
        var expected = ExportedFunctions(Source(source));
        var actual = ExportedFunctions(Compiled(compiled));

        Assert.NotEmpty(expected);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TheCompiledOutputExistsForEverySource()
    {
        var sources = Directory
            .EnumerateFiles(Path.Combine(RepoRoot(), "src", "interop", "src"), "*.ts")
            .Select(Path.GetFileNameWithoutExtension)
            .Order(StringComparer.Ordinal)
            .ToList();

        foreach (var name in sources)
        {
            var path = Path.Combine(RepoRoot(), "src", "FifaPressApp", "wwwroot", "js", $"{name}.js");
            Assert.True(File.Exists(path), $"{name}.ts has no committed {name}.js beside it");
        }

        // And nothing in wwwroot/js is orphaned — a .js with no .ts would be a
        // file nobody can safely regenerate.
        foreach (var emitted in Directory.EnumerateFiles(
                     Path.Combine(RepoRoot(), "src", "FifaPressApp", "wwwroot", "js"), "*.js"))
        {
            Assert.Contains(Path.GetFileNameWithoutExtension(emitted), sources);
        }
    }

    // ---------------------------------------------- the conversion, unchanged

    [Theory]
    [InlineData("getStoredTheme")]
    [InlineData("getSystemTheme")]
    [InlineData("applyTheme")]
    [InlineData("storeTheme")]
    [InlineData("clearStoredTheme")]
    public void TheThemeModuleStillExportsEveryFunctionTheComponentCalls(string name)
    {
        // 09 §5.3 holds theme.js's mechanism unchanged, and ThemeTrigger's @code
        // block was not touched. A renamed export would break the component at
        // runtime with nothing at compile time to catch it.
        Assert.Contains($"export function {name}", Compiled("theme.js"));
    }

    [Fact]
    public void TheThemeStorageKeyIsUnchangedByTheConversion()
    {
        // A different key would silently forget every existing visitor's choice.
        Assert.Contains("'fifa-press-app.theme'", Compiled("theme.js"));
    }

    [Fact]
    public void TheThemeModuleStillOnlyTouchesTheOneAttributeTheStylesheetWatches()
    {
        var compiled = Compiled("theme.js");

        Assert.Contains("setAttribute('data-theme'", compiled);
        Assert.Contains("removeAttribute('data-theme')", compiled);
    }

    [Fact]
    public void NoColourValueAppearsInTheInteropLayer()
    {
        // CSS owns the palette. A hex value here would be a second place a
        // colour is decided, and the one that wins would depend on load order.
        foreach (var file in Directory.EnumerateFiles(
                     Path.Combine(RepoRoot(), "src", "interop", "src"), "*.ts"))
        {
            var code = File.ReadAllText(file);
            Assert.DoesNotMatch(new Regex(@"#[0-9a-fA-F]{3,8}\b"), code);
            Assert.DoesNotContain("rgb(", code);
        }
    }

    [Fact]
    public void NoTranslatedStringAppearsInTheInteropLayer()
    {
        // The per-locale JSON owns the text. These modules own one storage key
        // and one attribute each, and a sentence in here would be a string no
        // translator would ever find.
        foreach (var file in Directory.EnumerateFiles(
                     Path.Combine(RepoRoot(), "src", "interop", "src"), "*.ts"))
        {
            var code = Regex.Replace(File.ReadAllText(file), @"//.*|/\*.*?\*/", "", RegexOptions.Singleline);

            foreach (Match literal in Regex.Matches(code, @"'([^']{25,})'"))
            {
                Assert.Fail($"{Path.GetFileName(file)} carries a long string literal: {literal.Groups[1].Value}");
            }
        }
    }

    // ------------------------------------------------------- the toolchain

    // -------------------------------------------------- the locale module

    [Theory]
    [InlineData("getStoredLocale")]
    [InlineData("getBrowserLocale")]
    [InlineData("applyLocale")]
    [InlineData("storeLocale")]
    [InlineData("clearStoredLocale")]
    public void TheLocaleModuleExportsEveryFunctionTheProviderCalls(string name)
    {
        Assert.Contains($"export function {name}", Compiled("locale.js"));
    }

    [Fact]
    public void TheTwoModulesUseDifferentStorageKeys()
    {
        // One key for two settings would make choosing a language forget the
        // theme. They are siblings, not the same thing.
        Assert.Contains("'fifa-press-app.theme'", Compiled("theme.js"));
        Assert.Contains("'fifa-press-app.locale'", Compiled("locale.js"));
    }

    [Fact]
    public void TheLocaleModuleSetsTheLangAttributeAndNeverClearsIt()
    {
        // A document is always in some language, and an absent lang leaves a
        // screen reader guessing — a worse answer than a wrong one.
        var compiled = Compiled("locale.js");

        Assert.Contains("setAttribute('lang'", compiled);
        Assert.DoesNotContain("removeAttribute('lang')", compiled);
    }

    [Fact]
    public void TheLocaleModuleIsShapedLikeItsSibling()
    {
        // The two modules are deliberately the same shape: a stored read, an
        // ambient-preference read, an apply, a store, a clear. A reader who has
        // understood one has understood both. Both halves of each name are
        // normalised — the subject (Theme/Locale) and the source of the ambient
        // preference, which is the operating system for one and the browser's
        // declared languages for the other.
        static IReadOnlyList<string> Shape(string code) =>
            Regex.Matches(code, @"export function (\w+)")
                .Select(match => Regex.Replace(match.Groups[1].Value, "Theme|Locale|System|Browser", "X"))
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();

        Assert.Equal(Shape(Compiled("theme.js")), Shape(Compiled("locale.js")));
    }

    [Fact]
    public void TheStoredLocaleReadNarrowsToTheThreeLanguagesTheAppHas()
    {
        // A stored value from a future version of the app, or a hand-edited
        // one, must not become an active locale the resources have no entry
        // for.
        Assert.Contains("value === 'en' || value === 'es' || value === 'pt'", Compiled("locale.js"));
    }

    [Fact]
    public void TheToolchainIsDevelopmentOnlyAndOutsideTheAppProject()
    {
        var csproj = File.ReadAllText(
            Path.Combine(RepoRoot(), "src", "FifaPressApp", "FifaPressApp.csproj"));

        // The app project does not know this folder exists.
        Assert.DoesNotContain("interop", csproj, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tsc", csproj, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("TypeScript", csproj, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TypeScriptIsADevDependencyAndTheOnlyOne()
    {
        using var document = JsonDocument.Parse(
            File.ReadAllText(Path.Combine(RepoRoot(), "src", "interop", "package.json")));

        var root = document.RootElement;

        Assert.True(root.GetProperty("private").GetBoolean());
        Assert.False(root.TryGetProperty("dependencies", out _),
            "the interop layer has no runtime dependencies and must not acquire any");

        var dev = root.GetProperty("devDependencies");
        Assert.Single(dev.EnumerateObject());
        Assert.True(dev.TryGetProperty("typescript", out _));
    }

    [Fact]
    public void TheCompilerIsHeldToStrictSettings()
    {
        // These flags are the reason to author this layer in TypeScript at all.
        // Without them the conversion buys a file extension.
        var tsconfig = File.ReadAllText(Path.Combine(RepoRoot(), "src", "interop", "tsconfig.json"));

        foreach (var flag in new[]
                 {
                     "\"strict\": true",
                     "\"noUncheckedIndexedAccess\": true",
                     "\"noImplicitReturns\": true",
                     "\"exactOptionalPropertyTypes\": true",
                     "\"noUnusedLocals\": true",
                 })
        {
            Assert.Contains(flag, tsconfig);
        }
    }

    [Fact]
    public void TheEmittedOutputGoesWhereTheComponentsImportFrom()
    {
        var tsconfig = File.ReadAllText(Path.Combine(RepoRoot(), "src", "interop", "tsconfig.json"));

        Assert.Contains("\"outDir\": \"../FifaPressApp/wwwroot/js\"", tsconfig);
    }

    [Fact]
    public void TheDeploymentWorkflowNeedsNoNode()
    {
        // 09 §5.3 offered a workflow lift for a compile step. Committing the
        // output is what makes it unnecessary, and this asserts it stayed
        // unnecessary.
        var workflow = File.ReadAllText(
            Path.Combine(RepoRoot(), ".github", "workflows", "deploy-pages.yml"));

        foreach (var absent in new[] { "setup-node", "npm ", "tsc", "node_modules" })
        {
            Assert.DoesNotContain(absent, workflow, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void TheToolchainsOwnArtefactsAreNotTracked()
    {
        var ignore = File.ReadAllText(Path.Combine(RepoRoot(), ".gitignore"));

        Assert.Contains("node_modules/", ignore);
    }

    [Fact]
    public void NothingInTheRepoClaimsTheAppIsBuiltInTypeScript()
    {
        // The app is C# and Razor with a two-file interop layer. "Built in
        // TypeScript" would be a claim about the whole thing that is not true.
        var roots = new[]
        {
            Path.Combine(RepoRoot(), "src"),
            Path.Combine(RepoRoot(), "tests"),
        };

        foreach (var root in roots)
        {
            foreach (var file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
            {
                if (file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                    || file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                    || file.Contains($"{Path.DirectorySeparatorChar}node_modules{Path.DirectorySeparatorChar}"))
                {
                    continue;
                }

                if (Path.GetExtension(file) is not (".cs" or ".razor" or ".md" or ".ts" or ".json"))
                {
                    continue;
                }

                // This file carries the phrase as the thing it searches for, so
                // it cannot audit itself.
                if (Path.GetFileName(file) == "InteropTests.cs")
                {
                    continue;
                }

                // The phrase may appear only as something the repo denies —
                // this file and the interop README both say the app is NOT
                // built in TypeScript, and forbidding the words outright would
                // forbid saying so.
                var text = File.ReadAllText(file);
                foreach (Match occurrence in Regex.Matches(text, @"built in TypeScript", RegexOptions.IgnoreCase))
                {
                    var lead = text[Math.Max(0, occurrence.Index - 60)..occurrence.Index];
                    Assert.True(
                        lead.Contains("not", StringComparison.OrdinalIgnoreCase),
                        $"{Path.GetFileName(file)} claims the app is built in TypeScript");
                }
            }
        }
    }
}
