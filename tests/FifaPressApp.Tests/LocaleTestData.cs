using System.Net;
using System.Runtime.CompilerServices;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using FifaPressApp.Models;
using FifaPressApp.Services;

namespace FifaPressApp.Tests;

/// <summary>
/// A <see cref="LocaleService"/> over the app's real locale files.
///
/// <para>
/// The files are read from the source tree and served through a stub handler
/// rather than being copied or duplicated, for the same reason the schedule CSV
/// is: a test that asserts against its own copy of the resources is asserting
/// that it agrees with itself.
/// </para>
/// </summary>
internal static class LocaleTestData
{
    public static string ResourceDirectory([CallerFilePath] string thisFile = "") =>
        Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(thisFile)!, "..", "..", "src", "FifaPressApp", "wwwroot", "i18n"));

    public static LocaleService Loaded()
    {
        var service = new LocaleService(
            new HttpClient(new FileServingHandler(ResourceDirectory()))
            {
                BaseAddress = new Uri("http://localhost/"),
            });

        service.InitializeAsync().GetAwaiter().GetResult();
        return service;
    }

    /// <summary>
    /// Registers the locale service on a bUnit context.
    ///
    /// <para>
    /// Every component that renders text now depends on this, so every test that
    /// renders one has to supply it. That is what a shared, loaded-once helper
    /// is for — the alternative is each test file carrying its own copy of the
    /// same three lines.
    /// </para>
    /// </summary>
    public static BunitContext WithLocale(this BunitContext context)
    {
        context.Services.AddSingleton(Loaded());
        return context;
    }

    public static string RawJson(AppLocale locale) =>
        File.ReadAllText(Path.Combine(ResourceDirectory(), $"{LocaleService.CodeOf(locale)}.json"));

    /// <summary>Serves whichever file the request names, or 404 if it is absent.</summary>
    private sealed class FileServingHandler(string root) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var name = Path.GetFileName(request.RequestUri!.AbsolutePath);
            var path = Path.Combine(root, name);

            return Task.FromResult(File.Exists(path)
                ? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(File.ReadAllText(path)) }
                : new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
