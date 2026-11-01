using Bunit;
using Microsoft.Extensions.DependencyInjection;
using ObserverMagazine.Web.Components;
using ObserverMagazine.Web.Services;
using ObserverMagazine.Web.Tests.Services;
using Xunit;

namespace ObserverMagazine.Web.Tests.Components;

public class MasterDetailTests : IDisposable
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);

    private readonly BunitContext _ctx = new();

    private const string SampleProductsJson = """
        [
          { "name": "Widget A", "category": "Tools", "price": 9.99, "stock": 100, "rating": 4.5, "description": "A widget" },
          { "name": "Gadget B", "category": "Electronics", "price": 49.99, "stock": 25, "rating": 3.8, "description": "A gadget" }
        ]
        """;

    private IRenderedComponent<MasterDetail> RenderLoaded()
    {
        var httpClient = new HttpClient(new MasterDetailFakeHttpHandler(SampleProductsJson))
        {
            BaseAddress = new Uri("https://test.local/")
        };
        _ctx.Services.AddSingleton(httpClient);
        _ctx.Services.AddSingleton<IAnalyticsService>(new NoOpAnalyticsService());
        _ctx.JSInterop.SetupVoid("localStorage.setItem", _ => true);
        _ctx.JSInterop.SetupVoid("localStorage.removeItem", _ => true);
        _ctx.JSInterop.Setup<string?>("localStorage.getItem", _ => true).SetResult(null);

        var cut = _ctx.Render<MasterDetail>();
        cut.WaitForState(() => cut.FindAll(".md-list li").Count == 2, WaitTimeout);
        return cut;
    }

    [Fact]
    public void MasterDetail_RendersProductList()
    {
        var cut = RenderLoaded();

        Assert.Contains("Widget A", cut.Markup);
        Assert.Contains("Gadget B", cut.Markup);
    }

    [Fact]
    public void MasterDetail_ShowsEmptyDetailOnLoad()
    {
        var cut = RenderLoaded();

        Assert.Contains("Select an item", cut.Find(".md-empty").TextContent);
    }

    [Fact]
    public void MasterDetail_SelectProductShowsDetail()
    {
        var cut = RenderLoaded();

        cut.Find(".md-list li").Click();

        cut.WaitForAssertion(() =>
        {
            var detail = cut.Find(".md-detail").TextContent;
            Assert.Contains("Tools", detail);
            Assert.Contains("Edit", detail);
            Assert.Contains("Delete", detail);
        }, WaitTimeout);
    }

    [Fact]
    public void MasterDetail_AddButtonShowsForm()
    {
        var cut = RenderLoaded();

        cut.Find(".md-list-header button").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("New Product", cut.Find(".md-detail h3").TextContent);
            Assert.Contains("Save", cut.Find(".md-detail").TextContent);
            Assert.Contains("Cancel", cut.Find(".md-detail").TextContent);
        }, WaitTimeout);
    }

    [Fact]
    public void MasterDetail_HasResetButton()
    {
        var cut = RenderLoaded();

        Assert.Contains("Reset to Defaults", cut.Find(".md-list-footer").TextContent);
    }

    public void Dispose() => _ctx.Dispose();
}

internal sealed class MasterDetailFakeHttpHandler(string json) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        });
}
