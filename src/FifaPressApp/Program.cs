using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FifaPressApp;
using FifaPressApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Registering both trackers as singletons means one shared instance exists
// for the life of this browser tab. Any page that @injects SessionTracker or
// AttendanceTracker gets that same instance, not a fresh, disconnected copy —
// this is what lets state (like "you're registered") survive navigation.
builder.Services.AddSingleton<SessionTracker>();
builder.Services.AddSingleton<AttendanceTracker>();

// The simulated session, and the two published demo accounts behind it. Both
// are singletons for the same reason as the trackers above: the session has to
// survive navigation, or signing in would last exactly one page.
//
// Neither of these is an authentication system, and the classes say so in their
// own remarks rather than leaving it to this comment.
builder.Services.AddSingleton<DemoAccountStore>();
builder.Services.AddSingleton<SimulatedSessionProvider>();

// Which change the person just wrote, so the record can mark its arrival.
// A singleton for the same reason again: submitting a request navigates to a
// freshly mounted record screen, so the screen that needs to know is not the
// screen that knows.
builder.Services.AddSingleton<ChangeArrivalTracker>();

// The three locales, held in one place for the life of the tab. A singleton is
// what makes a language switch a dictionary lookup rather than a fetch: the
// dictionaries are already loaded, so changing language does no I/O and cannot
// leave half a screen in one language while the rest waits on a network.
//
// Its own HttpClient, for the same reason the provider below has one: a
// singleton holding the scoped client is a lifetime mismatch.
builder.Services.AddSingleton(_ =>
    new LocaleService(
        new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }));

// Registered against the interface, not the class. Every page and component
// asks for IAccessDataProvider and never names MockAccessDataProvider, so
// swapping in an implementation that talks to a real service is a change to
// this one line rather than a change to every caller.
//
// It gets its own HttpClient rather than the scoped one above: this provider is
// a singleton, and a singleton holding a scoped dependency is the kind of
// lifetime mismatch that works until it suddenly doesn't.
builder.Services.AddSingleton<IAccessDataProvider>(_ =>
    new MockAccessDataProvider(
        new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }));

var host = builder.Build();

// All three locale files, fetched once before the first render. Loading them
// here rather than lazily on first use is what lets a component render text on
// its very first pass — the same rule the access record already follows, where
// the headline paints with no spinner in front of it.
await host.Services.GetRequiredService<LocaleService>().InitializeAsync();

await host.RunAsync();
