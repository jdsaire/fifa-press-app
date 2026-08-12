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

await builder.Build().RunAsync();
