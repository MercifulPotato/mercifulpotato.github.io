using System.Net;
using Microsoft.Extensions.Logging;
using ObserverMagazine.Web.Services;
using Xunit;

namespace ObserverMagazine.Web.Tests.Services;

public class AnalyticsServiceTests
{
    private static (AnalyticsService Sut, ListLogger<AnalyticsService> Logger, RoutingHttpHandler Handler) Create(
        Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        var handler = new RoutingHttpHandler(respond);
        var logger = new ListLogger<AnalyticsService>();
        return (new AnalyticsService(new HttpClient(handler), logger), logger, handler);
    }

    private static bool IsHealthCheck(HttpRequestMessage request) =>
        request.RequestUri!.AbsolutePath == "/api/health";

    private static HttpResponseMessage Json(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
    };

    [Fact]
    public async Task TrackPageView_WhenBackendRejectsEvent_LogsWarningWithStatusCode()
    {
        var (sut, logger, _) = Create(r => new HttpResponseMessage(
            IsHealthCheck(r) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError));

        await sut.TrackPageViewAsync("Home");

        var warning = Assert.Single(logger.At(LogLevel.Warning));
        Assert.Contains("500", warning.Message);
    }

    [Fact]
    public async Task TrackInteraction_WhenRequestThrows_LogsWarningWithException()
    {
        var (sut, logger, _) = Create(r => IsHealthCheck(r)
            ? new HttpResponseMessage(HttpStatusCode.OK)
            : throw new HttpRequestException("connection reset"));

        await sut.TrackInteractionAsync("Click", "button");

        var warning = Assert.Single(logger.At(LogLevel.Warning));
        Assert.IsType<HttpRequestException>(warning.Exception);
    }

    [Fact]
    public async Task WhenBackendIsUnreachable_ChecksHealthOnceAndSkipsEvents()
    {
        var (sut, logger, handler) = Create(_ => throw new HttpRequestException("offline"));

        await sut.TrackPageViewAsync("Home");
        await sut.TrackInteractionAsync("Click");
        await sut.IncrementViewAsync("first-post");

        Assert.False(sut.IsBackendAvailable);
        Assert.Equal(1, handler.RequestCount);
        Assert.Single(logger.At(LogLevel.Warning));
    }

    [Fact]
    public async Task GetViewCount_WhenResponseIsNotJson_ReturnsNullAndLogsWarning()
    {
        var (sut, logger, _) = Create(r => IsHealthCheck(r)
            ? new HttpResponseMessage(HttpStatusCode.OK)
            : Json("not json"));

        var count = await sut.GetViewCountAsync("first-post");

        Assert.Null(count);
        Assert.Single(logger.At(LogLevel.Warning));
    }

    [Fact]
    public async Task GetReactions_WhenBackendHealthy_ReturnsCountsWithoutWarnings()
    {
        var (sut, logger, _) = Create(r => IsHealthCheck(r)
            ? new HttpResponseMessage(HttpStatusCode.OK)
            : Json("""{ "like": 3, "love": 1 }"""));

        var reactions = await sut.GetReactionsAsync("first-post");

        Assert.NotNull(reactions);
        Assert.Equal(3, reactions["like"]);
        Assert.Empty(logger.At(LogLevel.Warning));
        Assert.True(sut.IsBackendAvailable);
    }
}

internal sealed class RoutingHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
{
    public int RequestCount { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestCount++;
        return Task.FromResult(respond(request));
    }
}
