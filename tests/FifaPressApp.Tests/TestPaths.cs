using System.Runtime.CompilerServices;

namespace FifaPressApp.Tests;

/// <summary>
/// Where the app's source lives, relative to this test project.
///
/// <para>
/// Several test files resolve this for themselves with a private copy of the
/// same <c>[CallerFilePath]</c> walk. This is that walk, written once, for the
/// files added from this run onward — the existing copies are left alone rather
/// than swept into a refactor that no gate asked for.
/// </para>
/// </summary>
internal static class TestPaths
{
    public static string SourceRoot([CallerFilePath] string thisFile = "")
        => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "..", "src", "FifaPressApp"));
}
