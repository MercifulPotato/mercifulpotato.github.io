using System.Net.Http.Json;

namespace ObserverMagazine.Web.Services;

public sealed class AnalyticsService(HttpClient http, ILogger<AnalyticsService> logger) : IAnalyticsService
{
    private const string BackendBaseUrl = "https://my-api.2w7sp317.workers.dev";
    private static readonly TimeSpan HealthCacheDuration = TimeSpan.FromMinutes(5);

    private Task? _initialHealthCheck;
    private bool _backendAvailable;
    private DateTime _lastHealthCheck = DateTime.MinValue;

    public bool IsBackendAvailable => _backendAvailable;

    public async Task CheckHealthAsync()
    {
        if (DateTime.UtcNow - _lastHealthCheck < HealthCacheDuration)
        {
            return;
        }

        try
        {
            using var response = await http.GetAsync($"{BackendBaseUrl}/api/health");
            _backendAvailable = response.IsSuccessStatusCode;

            if (_backendAvailable)
            {
                logger.LogInformation("Analytics backend is available at {BackendBaseUrl}", BackendBaseUrl);
            }
            else
            {
                logger.LogWarning(
                    "Analytics backend health check returned HTTP {StatusCode}; analytics, views and reactions are disabled",
                    (int)response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _backendAvailable = false;
            logger.LogWarning(ex,
                "Analytics backend at {BackendBaseUrl} is unreachable; analytics, views and reactions are disabled",
                BackendBaseUrl);
        }
        finally
        {
            _lastHealthCheck = DateTime.UtcNow;
        }
    }

    public Task TrackPageViewAsync(string pageName, string? detail = null)
    {
        var content = detail is not null
            ? $"[PageView] {pageName} — {detail}"
            : $"[PageView] {pageName}";

        return SendEventAsync($"PageView: {pageName}", content);
    }

    public Task TrackInteractionAsync(string action, string? detail = null)
    {
        var content = detail is not null
            ? $"[Interaction] {action} — {detail}"
            : $"[Interaction] {action}";

        return SendEventAsync($"Interaction: {action}", content);
    }

    public Task IncrementViewAsync(string slug) =>
        SendAsync("increment view count", slug,
            () => http.PostAsync($"{BackendBaseUrl}/api/views/{Uri.EscapeDataString(slug)}", null));

    public async Task<int?> GetViewCountAsync(string slug)
    {
        var result = await GetAsync<ViewCountResponse>("read view count", slug,
            $"{BackendBaseUrl}/api/views/{Uri.EscapeDataString(slug)}");
        return result?.Count;
    }

    public Task AddReactionAsync(string slug, string reactionType) =>
        SendAsync("add reaction", $"{slug}:{reactionType}",
            () => http.PostAsJsonAsync($"{BackendBaseUrl}/api/reactions/{Uri.EscapeDataString(slug)}", new { type = reactionType }));

    public Task<Dictionary<string, int>?> GetReactionsAsync(string slug) =>
        GetAsync<Dictionary<string, int>>("read reactions", slug,
            $"{BackendBaseUrl}/api/reactions/{Uri.EscapeDataString(slug)}");

    private Task SendEventAsync(string title, string content) =>
        SendAsync("send analytics event", title,
            () => http.PostAsJsonAsync($"{BackendBaseUrl}/api/notes", new { title, content }));

    private async Task<bool> EnsureBackendAvailableAsync()
    {
        await (_initialHealthCheck ??= CheckHealthAsync());
        return _backendAvailable;
    }

    private async Task SendAsync(string operation, string target, Func<Task<HttpResponseMessage>> send)
    {
        if (!await EnsureBackendAvailableAsync())
        {
            return;
        }

        try
        {
            using var response = await send();
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Analytics backend rejected request to {Operation} for {Target} with HTTP {StatusCode}",
                    operation, target, (int)response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to {Operation} for {Target}", operation, target);
        }
    }

    private async Task<T?> GetAsync<T>(string operation, string target, string url)
    {
        if (!await EnsureBackendAvailableAsync())
        {
            return default;
        }

        try
        {
            return await http.GetFromJsonAsync<T>(url);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to {Operation} for {Target}", operation, target);
            return default;
        }
    }

    private sealed record ViewCountResponse(int Count);
}
