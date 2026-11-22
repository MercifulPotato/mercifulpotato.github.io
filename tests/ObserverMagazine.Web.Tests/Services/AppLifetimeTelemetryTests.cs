using Microsoft.Extensions.Logging.Abstractions;
using ObserverMagazine.Web.Services;
using Xunit;

namespace ObserverMagazine.Web.Tests.Services;

public class AppLifetimeTelemetryTests
{
    private static (AppLifetimeTelemetry Sut, TelemetryService Telemetry) Create()
    {
        var telemetry = new TelemetryService(NullLogger<TelemetryService>.Instance);
        return (new AppLifetimeTelemetry(telemetry, NullLogger<AppLifetimeTelemetry>.Instance), telemetry);
    }

    [Fact]
    public async Task StartAsync_IncrementsStartedCounter()
    {
        var (sut, telemetry) = Create();

        await sut.StartAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, telemetry.GetCounter(AppLifetimeTelemetry.StartedCounter));
        Assert.Equal(0, telemetry.GetCounter(AppLifetimeTelemetry.StoppedCounter));
    }

    [Fact]
    public async Task StopAsync_IncrementsStoppedCounter()
    {
        var (sut, telemetry) = Create();

        await sut.StartAsync(TestContext.Current.CancellationToken);
        await sut.StopAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, telemetry.GetCounter(AppLifetimeTelemetry.StoppedCounter));
    }
}
