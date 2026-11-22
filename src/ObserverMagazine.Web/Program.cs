using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ObserverMagazine.Web;
using ObserverMagazine.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddSingleton<TelemetryService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddHostedService<AppLifetimeTelemetry>();

builder.Logging.SetMinimumLevel(LogLevel.Information);

var host = builder.Build();
GlobalExceptionLogging.Register(host.Services.GetRequiredService<ILoggerFactory>());

await host.RunAsync();
