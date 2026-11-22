using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using ObserverMagazine.Web.Layout;
using Xunit;

namespace ObserverMagazine.Web.Tests.Components;

public class MainLayoutTests : IDisposable
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);

    private readonly BunitContext _ctx = new();
    private readonly RecordingErrorBoundaryLogger _errorLogger = new();

    public MainLayoutTests()
    {
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        _ctx.Services.AddSingleton<IErrorBoundaryLogger>(_errorLogger);
    }

    [Fact]
    public void RendersPageBody()
    {
        RenderFragment body = b => b.AddMarkupContent(0, "<h1>Hello</h1>");

        var cut = _ctx.Render<MainLayout>(p => p.Add(l => l.Body, body));

        Assert.Equal("Hello", cut.Find("main h1").TextContent);
        Assert.Empty(cut.FindAll(".page-error"));
    }

    [Fact]
    public void PageException_IsContainedAndLogged()
    {
        RenderFragment body = b =>
        {
            b.OpenComponent<ThrowingPage>(0);
            b.CloseComponent();
        };

        var cut = _ctx.Render<MainLayout>(p => p.Add(l => l.Body, body));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("This page ran into a problem", cut.Find(".page-error").TextContent);
            Assert.Contains("boom", cut.Find(".page-error-details").TextContent);
        }, WaitTimeout);

        Assert.NotNull(cut.Find(".top-bar"));
        var logged = Assert.Single(_errorLogger.Errors);
        Assert.IsType<InvalidOperationException>(logged);
    }

    public void Dispose() => _ctx.Dispose();
}

internal sealed class ThrowingPage : ComponentBase
{
    protected override void OnInitialized() => throw new InvalidOperationException("boom");
}

internal sealed class RecordingErrorBoundaryLogger : IErrorBoundaryLogger
{
    public List<Exception> Errors { get; } = [];

    public ValueTask LogErrorAsync(Exception exception)
    {
        Errors.Add(exception);
        return ValueTask.CompletedTask;
    }
}
