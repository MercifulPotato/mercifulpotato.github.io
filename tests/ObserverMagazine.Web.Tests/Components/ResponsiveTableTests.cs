using Bunit;
using Microsoft.Extensions.DependencyInjection;
using ObserverMagazine.Web.Components;
using ObserverMagazine.Web.Services;
using ObserverMagazine.Web.Tests.Services;
using Xunit;

namespace ObserverMagazine.Web.Tests.Components;

public class ResponsiveTableTests : IDisposable
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);

    private readonly BunitContext _ctx = new();

    private const string SampleProductsJson = """
        [
          { "name": "Widget A", "category": "Tools", "price": 9.99, "stock": 100, "rating": 4.5, "description": "A widget" },
          { "name": "Gadget B", "category": "Electronics", "price": 49.99, "stock": 25, "rating": 3.8, "description": "A gadget" }
        ]
        """;

    private IRenderedComponent<ResponsiveTable> RenderLoaded()
    {
        var httpClient = new HttpClient(new ComponentFakeHttpHandler(SampleProductsJson))
        {
            BaseAddress = new Uri("https://test.local/")
        };
        _ctx.Services.AddSingleton(httpClient);
        _ctx.Services.AddSingleton<IAnalyticsService, NoOpAnalyticsService>();

        var cut = _ctx.Render<ResponsiveTable>();
        cut.WaitForState(() => cut.FindAll(".rt-data-table tbody tr").Count == 2, WaitTimeout);
        return cut;
    }

    private static string FirstRowName(IRenderedComponent<ResponsiveTable> cut) =>
        cut.Find(".rt-data-table tbody tr td").TextContent.Trim();

    [Fact]
    public void RendersTable_WithData()
    {
        var cut = RenderLoaded();

        Assert.Contains("Widget A", cut.Markup);
        Assert.Contains("Gadget B", cut.Markup);
    }

    [Fact]
    public void FilterInput_FiltersRows()
    {
        var cut = RenderLoaded();

        cut.Find(".rt-filter-input").Input("Widget");

        cut.WaitForAssertion(() =>
        {
            var rows = cut.FindAll(".rt-data-table tbody tr");
            Assert.Single(rows);
            Assert.Contains("Widget A", rows[0].TextContent);
        }, WaitTimeout);
    }

    [Fact]
    public void ClickColumnHeader_SortsData()
    {
        var cut = RenderLoaded();
        Assert.Equal("Gadget B", FirstRowName(cut));

        cut.FindAll("th.rt-sortable")[2].Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("Widget A", FirstRowName(cut));
            Assert.Contains("▲", cut.FindAll("th.rt-sortable")[2].TextContent);
        }, WaitTimeout);

        cut.FindAll("th.rt-sortable")[2].Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("Gadget B", FirstRowName(cut));
            Assert.Contains("▼", cut.FindAll("th.rt-sortable")[2].TextContent);
        }, WaitTimeout);
    }

    public void Dispose() => _ctx.Dispose();
}

internal sealed class ComponentFakeHttpHandler(string json) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        });
}
