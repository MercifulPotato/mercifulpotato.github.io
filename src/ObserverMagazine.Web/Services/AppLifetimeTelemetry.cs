using Microsoft.Extensions.Hosting;

namespace ObserverMagazine.Web.Services;

public sealed class AppLifetimeTelemetry(TelemetryService telemetry, ILogger<AppLifetimeTelemetry> logger) : IHostedService
{
    public const string StartedCounter = "app:started";
    public const string StoppedCounter = "app:stopped";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Merciful Potato Magazine started at {Time}", DateTime.UtcNow);
        telemetry.IncrementCounter(StartedCounter);
        telemetry.TrackEvent("AppStarted");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        telemetry.IncrementCounter(StoppedCounter);
        telemetry.TrackEvent("AppStopped");
        return Task.CompletedTask;
    }
}
