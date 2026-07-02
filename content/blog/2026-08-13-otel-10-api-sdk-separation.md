---
title: "The API and the SDK: Why OpenTelemetry Split Itself in Two"
date: 2026-08-13
author: mercifulpotato-team
summary: "OpenTelemetry deliberately split itself into two separate pieces: an API that does almost nothing by itself, and an SDK the application owner chooses. This single design decision is what makes everything in this series actually deployable."
tags:
  - opentelemetry
  - observability
  - plain-english
  - software-engineering
series: "OpenTelemetry from the Ground Up"
---

## Part 1: A Problem About Libraries, Not Applications

Every post so far in this series has looked at OpenTelemetry from the point of view of a single application: our checkout service, deciding to create spans, record metrics, and write logs. Today's post looks at the same architecture from a different, more revealing angle: the point of view of the person who writes a library — a database driver, an HTTP client, a message-queue package — that thousands of other people's applications will depend on.

Put yourself in that library author's position for a moment. You maintain a popular open-source database driver used across the industry. You would genuinely like your driver to emit a span every time it executes a query, because that single addition would make life easier for every one of your thousands of downstream users, in every application that depends on your driver, without any of them needing to do anything themselves. But here is the immediate problem: if adding that span requires your driver to take a hard dependency on a specific, complete tracing product — a specific vendor's software development kit, with all of its own dependencies, its own configuration requirements, its own opinions about where data should be sent — then every single application that uses your driver is now forced to carry that vendor's entire tracing stack too, whether or not they want it, whether or not they use that vendor at all, and whether or not two of your downstream dependencies each try to force a different, incompatible vendor onto the same application at once. This is a genuine, concrete version of what engineers sometimes call dependency hell, and it would make instrumenting a widely used library an act of considerable hostility toward that library's own users.

OpenTelemetry's answer to this exact problem is the single architectural decision this entire post is about, and it turns out to be one of the more consequential design choices anywhere in this series.

## Part 2: The Split, Stated Precisely

OpenTelemetry's own formal client design principles state the requirement in language worth quoting closely for its precision: the API and SDK libraries must be provided as independent artifacts, specifically so that third-party libraries and frameworks can depend only on the API, without their authors needing to know or care what specific implementation of OpenTelemetry, if any, the final application eventually chooses to install.

The API, in practice, is a set of interfaces — fixed shapes of function calls like "start a span," "record this measurement," "write this log entry" — that a library author can call directly, with no further dependency baggage attached. The SDK is the separate, considerably heavier package that actually does something with those calls: batching data, applying sampling decisions, formatting it, and sending it onward to wherever it is configured to go. Critically, and this is the detail that makes the whole arrangement work, an application is never forced to install the SDK at all. A library instrumented purely against the API will build, run, and behave completely normally, producing no errors and requiring no special handling, whether or not any SDK is present in the final application.

## Part 3: The No-Op Default, and Why It Matters More Than It Sounds

The specification is explicit and unusually careful about one particular consequence of this design: when no SDK is installed, calls to the API must not fail, must not throw errors, and must return valid, usable objects rather than forcing the calling code to add extra defensive checks. Calling `StartSpan()` with no SDK present must still hand back something the caller can safely use exactly as if a real span had been created; it simply does nothing further with it. The specification calls this behavior the API's minimal implementation, and states directly that it must impose as close to zero performance penalty as possible.

Why does this small technical detail deserve its own careful treatment in this series? Because it is the mechanism that makes the library author's dilemma from Part 1 disappear entirely. A database driver can call the tracing API on every single query, unconditionally, for every one of its users, forever — including users who have never heard of OpenTelemetry and have no intention of ever installing an SDK — and those users will simply never notice, because the calls quietly do nothing and cost next to nothing. Only the smaller subset of applications that do choose to install and configure an SDK actually see any telemetry emerge from that same, unmodified library code. The library author gets to instrument once, for everyone, with total confidence that doing so imposes no meaningful burden on anyone who does not want the result.

## Part 4: Providers, Exporters, and Processors — the Wiring, Named

Turning on real telemetry, from the application owner's side, means installing the SDK and wiring together a small number of specific, named pieces, and understanding these names precisely will make the hands-on code in Part 12 considerably easier to follow.

A Provider — TracerProvider for tracing, MeterProvider for metrics, LoggerProvider for logs — is the top-level object the application creates once, at startup, and configures with everything else: which exporters to use, what resource information identifies this particular service, and so on. Everything else hangs off the relevant provider.

A Processor sits between where telemetry is generated and where it eventually leaves the application, and its job is to decide when and how finished data actually gets handed off for export. The most common processor for production use is a batching processor, which deliberately waits and accumulates a number of finished spans, or a short span of time, before sending them onward together in one batch — trading a small amount of delay for a considerable reduction in the number of separate network calls an application has to make, which matters enormously at real traffic volumes. A simpler alternative, common in local development, sends each piece of telemetry immediately as it is produced, favoring instant visibility over network efficiency.

An Exporter is the final, outward-facing piece: the specific code responsible for serializing telemetry into some wire format and actually sending it somewhere, whether that is OTLP to a Collector, as covered in Parts 13 and 14, or a simpler destination like a local console for debugging.

Here is a small, annotated `Program.cs` fragment showing these pieces named explicitly, which we will build on directly in Part 12:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "checkout-service"))
    .WithTracing(tracing => tracing
        .AddSource("MercifulPotato.Checkout")   // register our own ActivitySource
        .AddOtlpExporter());                     // the exporter: where spans go

var app = builder.Build();
app.Run();
```

Every line in this fragment corresponds to a named concept just introduced: `ConfigureResource` sets up the identifying information attached to every piece of telemetry this service produces; `WithTracing` configures the TracerProvider; `AddSource` tells the SDK which of the application's own span-producing code to actually listen to — a registration step we will see cause real, silent confusion in Part 11 when it is forgotten; and `AddOtlpExporter` wires in the actual outward-facing exporter. The batching-versus-immediate processor choice sits, by default, quietly configured behind this convenience method, with sensible batching defaults already applied unless explicitly overridden.

## Part 5: Why API Stability Matters More Than SDK Stability

Part 4 of this series introduced OpenTelemetry's formal stability lifecycle: draft, experimental, stable, deprecated. Today's architectural split explains directly why the specification treats API stability as the more urgent, more heavily protected commitment of the two.

A library author who instruments their database driver against the OpenTelemetry API today is making a choice that will live inside that driver's own published code for years, across every future version, consumed by countless downstream applications the author will never personally interact with. If the API itself were to change in a breaking way, every one of those already-published library versions would potentially stop compiling, or worse, silently misbehave, the next time someone tried to use them. The SDK, by contrast, is chosen and configured by each individual application at the point of deployment, not baked permanently into a widely distributed library; an application can upgrade its own SDK dependency on its own schedule, entirely independently of when the libraries it depends on were originally published. This is precisely why OpenTelemetry's own versioning rules require the API and SDK to carry independent version numbers, and why the specification places particular weight on the API's long-term backward compatibility specifically — breaking the API breaks code the project does not control and cannot ask to be immediately republished, while breaking the SDK breaks only what an application team can, at their own discretion, choose to upgrade.

## Part 6: .NET's Unusual Position, Previewed

There is a detail about how this split plays out differently in .NET specifically that deserves a full, dedicated treatment rather than a rushed mention here, and Part 12 of this series is that dedicated treatment. The short preview, worth flagging now so it does not come as a surprise: in most languages, the "API" role described throughout this post is played by OpenTelemetry's own published API package, written and maintained by the OpenTelemetry project itself. In .NET, a meaningful part of that same role is instead played by facilities that were already built directly into the .NET platform's own base class library — `System.Diagnostics.Activity` for tracing, `System.Diagnostics.Metrics.Meter` for metrics — years before OpenTelemetry itself adopted and extended them. This is a genuinely distinctive arrangement among the languages OpenTelemetry supports, and understanding it properly is worth an entire post of its own.

## Part 7: How to Spot the Lie — "No Vendor Lock-In" Claims, Examined Structurally

This series has referenced the "no vendor lock-in" claim several times already, most directly in Parts 4 and 5, and today's post is where that claim can finally be examined structurally rather than merely rhetorically, because the API and SDK split described above is precisely, mechanically, why the claim can be true at all.

Because a library only ever depends on the API, and because the API is neutral with respect to any specific backend, switching an application's chosen SDK configuration — which exporter it uses, which backend it points at — genuinely does not require touching a single line of that library's own already-published, already-instrumented code. This is a real, structural, verifiable property of the architecture, not merely a marketing promise, and you now understand exactly which part of the design delivers it: the independent-artifacts requirement from Part 2, combined with the no-op minimal implementation from Part 3.

It remains equally important, in the same honest spirit as Part 4's discussion, to name what this structural guarantee does not cover, because an incomplete understanding here is precisely how "no vendor lock-in" claims get overextended in practice. The architecture frees you from re-instrumenting application and library code when you change backends. It does not, on its own, free you from rebuilding dashboards, alert rules, and saved queries that were built against one particular backend's own query language and user interface, nor does it prevent a specific vendor's distribution, of the kind examined in Part 5, from adding its own genuinely useful but backend-specific extra features on top of the neutral core, which a switch away from that vendor would still mean giving up. When you next encounter a "no lock-in" claim, the test this post offers is simple: ask specifically which layer the claim actually covers — the instrumentation layer, which this architecture genuinely does neutralize, or the analysis-and-visualization layer sitting on top of it, which it structurally does not.

## Part 8: What Comes Tomorrow

Today's post explained the architectural line between API and SDK. Tomorrow's post turns to the practical question sitting immediately on top of that line: given that an application needs telemetry-producing code somewhere, where does that code actually come from in practice? We will build out the full taxonomy — zero-code automatic instrumentation that requires touching no application code at all, instrumentation libraries that wrap specific popular frameworks and dependencies, and manual instrumentation that a developer writes by hand for their own business logic — and be honest about what each approach can and cannot see.

## Sources and Further Reading

- OpenTelemetry Project, ["OpenTelemetry Client Design Principles,"](https://opentelemetry.io/docs/specs/otel/library-guidelines/) opentelemetry.io — the formal specification for the API/SDK independent-artifacts requirement, the no-op minimal implementation behavior, and the independent versioning rules covered throughout this post.
- OpenTelemetry Project, ["Versioning and Stability for OpenTelemetry Clients,"](https://opentelemetry.io/docs/specs/otel/versioning-and-stability/) opentelemetry.io — referenced in Part 5 for the stability-lifecycle context connecting back to Part 4 of this series.
- OpenTelemetry .NET Documentation, ["Getting Started,"](https://opentelemetry.io/docs/languages/dotnet/getting-started/) opentelemetry.io — the source for the `Program.cs` registration pattern shown in Part 4, verified against current documentation.

*Tomorrow: instrumentation in practice — zero-code, libraries, and doing it by hand, and what each one can and cannot see.*
