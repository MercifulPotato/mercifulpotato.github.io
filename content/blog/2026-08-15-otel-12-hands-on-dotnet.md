---
title: "Hands-On .NET: ActivitySource, Meter, and ILogger"
date: 2026-08-15
author: mercifulpotato-team
summary: "A complete, working ASP.NET Core service built up piece by piece: ActivitySource, Meter, and ILogger wired together, exported over OTLP, and viewed for free using the standalone Aspire Dashboard."
tags:
  - opentelemetry
  - observability
  - plain-english
  - dotnet
  - aspnet-core
series: "OpenTelemetry from the Ground Up"
---

## Part 1: .NET's Unusual Bargain

This series has previewed this moment twice already — once in Part 6, once again in Part 10 — and today is where the preview resolves into a complete, working example. Among the languages OpenTelemetry supports, .NET occupies a genuinely distinctive position: a meaningful share of what other languages call the "API," the neutral, no-op-by-default interface library authors call directly, is instead played by facilities that were already built directly into the .NET platform's own base class library, years before OpenTelemetry itself existed in anything like its current form. `System.Diagnostics.Activity` and its factory, `ActivitySource`, provide tracing. `System.Diagnostics.Metrics.Meter` provides metrics. `Microsoft.Extensions.Logging.ILogger` — the same interface .NET developers have already been using for years — provides logging, bridged into OpenTelemetry's own log data model exactly as Part 9 of this series described.

The practical consequence of this arrangement is genuinely pleasant: a .NET library author can add tracing and metrics to their own code using nothing but facilities already built into the platform itself, with zero OpenTelemetry-specific package references required, and any downstream application that later chooses to install the OpenTelemetry SDK will simply start receiving that telemetry, without the library author needing to have anticipated OpenTelemetry specifically at all. This is worth understanding precisely: an `Activity`, in this platform's own vocabulary, is a span wearing an older name — the `Activity` API predates OpenTelemetry, and was subsequently extended with status codes, events, and links to align with exactly the span model Part 6 of this series built from scratch.

## Part 2: Building a Tiny Observable Service, Step by Step

We are going to build a small, complete ASP.NET Core service, growing it one piece at a time, in the same spirit as this series' checkout example throughout. Everything shown below is verified, working code, following the exact pattern Microsoft's own official documentation demonstrates for OTLP-based observability.

First, the custom instrumentation: a `Meter` for our own metrics, and an `ActivitySource` for our own spans, defined once, near the top of the application:

```csharp
using System.Diagnostics;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Our own Meter and ActivitySource, defined once for this service
var checkoutMeter = new Meter("MercifulPotato.Checkout", "1.0.0");
var checkoutsCompleted = checkoutMeter.CreateCounter<int>(
    "checkout.completed", description: "Number of completed checkouts");

var checkoutActivitySource = new ActivitySource("MercifulPotato.Checkout");
```

Next, a minimal endpoint that actually uses both, plus `ILogger`, exercising all three signals at once in a single, small piece of business logic:

```csharp
async Task<string> ProcessCheckout(ILogger<Program> logger, string orderId)
{
    // A manual span, telling the story of this specific business operation
    using var activity = checkoutActivitySource.StartActivity("process-checkout");
    activity?.SetTag("order.id", orderId);

    logger.LogInformation("Processing checkout for order {OrderId}", orderId);

    // Simulate the work a real checkout would do
    await Task.Delay(50);

    checkoutsCompleted.Add(1, new KeyValuePair<string, object?>("payment.method", "credit-card"));

    activity?.SetStatus(ActivityStatusCode.Ok);
    logger.LogInformation("Checkout completed for order {OrderId}", orderId);

    return $"Order {orderId} processed";
}
```

Notice, in this one small function, all three signals from Parts 6 through 9 of this series working together: a span recording the operation's shape and outcome, a counter recording that a checkout completed, and structured log lines that, once the SDK is wired up in the next section, will automatically carry this span's trace and span identifiers — exactly the correlation payoff Part 9 built toward.

## Part 3: Wiring the SDK — the Complete Program.cs

Here is the complete startup wiring, following the same registration pattern introduced conceptually in Part 10, now filled in fully and correctly for all three signals:

```csharp
// Bridge ILogger into OpenTelemetry's log data model
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

var otel = builder.Services.AddOpenTelemetry();

otel.ConfigureResource(resource => resource
    .AddService(serviceName: "checkout-service", serviceVersion: "1.0.0"));

otel.WithMetrics(metrics =>
{
    metrics.AddAspNetCoreInstrumentation();      // built-in ASP.NET Core metrics
    metrics.AddMeter(checkoutMeter.Name);        // our own Meter
    metrics.AddMeter("Microsoft.AspNetCore.Hosting");
    metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
});

otel.WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddSource(checkoutActivitySource.Name);
});

// OTLP exporter, configured entirely through environment variables
var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
if (otlpEndpoint is not null)
{
    otel.UseOtlpExporter();
}

var app = builder.Build();
app.MapGet("/checkout/{orderId}", ProcessCheckout);
app.Run();
```

Every piece of this fragment maps directly onto vocabulary this series has already built. `ConfigureResource` attaches the identifying `service.name` and `service.version` — the resource attributes referenced throughout Parts 6 and 13 — to every piece of telemetry this service produces. `AddMeter("Microsoft.AspNetCore.Hosting")` and `AddMeter("Microsoft.AspNetCore.Server.Kestrel")` register two of the framework's own built-in meters, covering request-hosting and web-server-level metrics for free, without any manual instrumentation. `AddSource(checkoutActivitySource.Name)` is the exact registration step Part 11 warned is easy to forget — and forgetting it here would mean our `process-checkout` span, despite compiling and running with no error, would simply never appear anywhere, exactly as Part 11 described.

The NuGet packages this example depends on, verified against current Microsoft documentation, are `OpenTelemetry.Extensions.Hosting`, `OpenTelemetry.Instrumentation.AspNetCore`, `OpenTelemetry.Instrumentation.Http`, and `OpenTelemetry.Exporter.OpenTelemetryProtocol` — each a genuinely free, open-source package with no licensing cost, consistent with this publication's own free-of-cost principles.

## Part 4: Seeing It Locally, for Free

The `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable referenced in the code above is one of the standard, cross-vendor environment variables the OTLP specification defines, covered in full in Part 14 of this series. Set alongside `OTEL_SERVICE_NAME`, typically in `appsettings.Development.json` during local development:

```json
{
  "OTEL_EXPORTER_OTLP_ENDPOINT": "http://localhost:4317",
  "OTEL_SERVICE_NAME": "checkout-service"
}
```

For a completely free, zero-cost way to actually see the resulting telemetry during local development, Microsoft publishes a standalone Docker container running the Aspire Dashboard — a genuinely useful tool worth naming precisely here, because it is explicitly a development-time visualization tool, not a production monitoring backend, and this series will not pretend otherwise. It has no dependency on the broader Aspire framework; it simply exposes an OTLP endpoint and displays whatever telemetry arrives, from any application in any language that can speak OTLP. Here is the exact command, verified against current Microsoft documentation:

```bash
docker run --rm -it \
  -p 18888:18888 \
  -p 4317:18889 \
  --name aspire-dashboard \
  mcr.microsoft.com/dotnet/aspire-dashboard:latest
```

Running this container prints a one-time login token to the console; the dashboard requires that token by default, since the telemetry it displays can contain sensitive information. Once logged in, running our checkout service and requesting `/checkout/8821` populates the dashboard with exactly what this series has spent eleven days building toward: a structured log line reading "Processing checkout for order 8821," carrying this exact request's trace and span identifiers automatically; a `checkout.completed` counter incrementing by one; and a `process-checkout` span, tagged with the order ID, nested correctly beneath the incoming ASP.NET Core request span that triggered it.

## Part 5: The Classic Gotchas, Demonstrated

Three specific mistakes are worth demonstrating explicitly, because each one compiles cleanly, runs without any error, and produces confusing silence rather than a helpful failure — precisely the trap Part 11 warned about in the abstract, now made concrete.

The first, already shown in Part 3 above, is forgetting `AddSource` or `AddMeter` for a custom source: the calling code runs perfectly, the span or measurement object comes back non-null, and nothing ever appears at the destination. The second is forgetting `IncludeFormattedMessage = true` on the logging configuration: without it, the exported log record's body may contain the raw, unformatted message template rather than the fully rendered text a human would expect to read. The third, subtler than the first two, is disposing an `Activity` too early, or not at all: an `Activity` created with `StartActivity` should be held inside a `using` statement, exactly as shown in Part 2's `ProcessCheckout` example, so that its actual duration — and therefore its correctness as a signal answering "how long did this take," the entire point of Part 6's treatment of spans — is measured accurately rather than truncated or left dangling.

## Part 6: This Very Website as a Case

It is worth being honest and specific here, in exactly the spirit this series has maintained throughout, about how this principle applies to Merciful Potato Magazine's own site. This site's own `TelemetryService` is built on `ILogger`, following the same platform-native pattern described throughout this post, and is structured to be extendable toward a real OpenTelemetry Collector endpoint of the kind covered in Part 13. But this site is also a Blazor WebAssembly application, meaning a meaningful share of its own code runs inside a visitor's browser rather than on a server — and Part 5 of this series already flagged, honestly, that OpenTelemetry's own browser and client-side story remains considerably less mature than its server-side story, a limitation Microsoft's own documentation states plainly rather than glossing over. This series will not overclaim on this publication's own behalf: a small, free, static site is exactly the kind of workload where the honest, proportionate answer, foreshadowed back in Part 1's discussion of what it costs to see, is a comparatively modest telemetry investment matched to comparatively modest stakes — full server-side tracing and metrics of the kind this post demonstrates would be considerable overkill for a site with no backend beyond a free-tier analytics endpoint, precisely the theme Part 17 returns to explicitly.

## Part 7: How to Spot the Lie — The Suspiciously Clean Sample

Tutorial code — including, honestly, the code in this very post — has an occupational hazard worth naming directly: it tends to skip the parts that make real production code messier and more resilient, because showing every failure path would make the tutorial considerably longer and harder to follow. Our `ProcessCheckout` example above has no error handling; a real payment failure would need the span's status set to `ActivityStatusCode.Error`, an exception recorded on the span, and a log line at `Error` severity, none of which this teaching example bothered to show.

The test this section offers, then, applies as much to this series' own code as to any vendor's: when you are handed a clean, working sample — in a blog post, a conference talk, or a vendor's getting-started guide — ask specifically where its error path went. A sample that only ever shows the successful case is not lying to you, exactly, but it is quietly deferring the harder, more valuable half of the instrumentation work to you, the reader, and a genuinely trustworthy source of guidance will say so plainly rather than letting the omission pass unremarked. This series does so here: everything in this post works, and everything in this post is incomplete without the error-handling half a real production checkout would also need.

## Part 8: What Comes Tomorrow

Today's post completed the application side of this series' architecture: code that produces telemetry, correctly wired to an SDK, viewable for free during development. Tomorrow's post follows that same telemetry one step further, into the piece of infrastructure most real production deployments add between the application and its final destination: the OpenTelemetry Collector — receivers, processors, and exporters, and the concrete reasons a team chooses to add this extra piece rather than sending data straight from an application to a backend.

## Sources and Further Reading

- Microsoft Learn, ["Example: Use OpenTelemetry with OTLP and the Standalone Aspire Dashboard,"](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-otlp-example) — the primary source for the complete, verified code pattern, NuGet package list, environment-variable configuration, and Docker command used throughout this post.
- Microsoft Learn, [".NET Observability with OpenTelemetry,"](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel) — the source for the platform-native `Activity`/`Meter`/`ILogger` architecture described in Part 1.
- OpenTelemetry .NET Documentation, ["Getting Started,"](https://opentelemetry.io/docs/languages/dotnet/getting-started/) opentelemetry.io — cross-checked for the `AddSource` and `AddMeter` registration patterns referenced in Part 5.

*Tomorrow: the OpenTelemetry Collector — receivers, processors, exporters, and the concrete reasons a production deployment adds this extra piece of infrastructure.*
