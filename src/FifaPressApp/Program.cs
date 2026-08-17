using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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

await builder.Build().RunAsync();
