---
title: "OpenTelemetry vs. Vendor SDKs: The Azure Application Insights Story"
date: 2026-08-08
author: mercifulpotato-team
summary: "Microsoft's own telemetry SDK is now being retired in favor of OpenTelemetry. This post walks through exactly why, what a vendor 'distro' actually is, and makes the fair case for both sides of the choice."
tags:
  - opentelemetry
  - observability
  - plain-english
  - azure
  - dotnet
series: "OpenTelemetry from the Ground Up"
---

## Part 1: A Company Choosing to Retire Its Own Product

Most of the vendor-versus-standard comparisons an engineer encounters come framed as a marketing pitch: a vendor explaining why its own proprietary product is better than some open alternative. Today's post examines a genuinely more interesting and more informative case, because it runs in the opposite direction. Microsoft — a company that spent years building, selling, and supporting its own proprietary telemetry product, Azure Application Insights, complete with a dedicated software development kit that many thousands of organizations adopted directly because Microsoft told them to — has now formally announced that this same proprietary software development kit is deprecated, with a specific retirement date, and that new applications should not use it at all. Understanding exactly why a vendor would voluntarily retire its own successful product in favor of a neutral, open standard tells you something real about the strength of the underlying argument for that standard, precisely because Microsoft had every commercial incentive to keep customers inside its own proprietary tooling if that tooling still served Microsoft's own interests better than the alternative.

This post walks through that story in careful, specific, dated detail, and then does something this series has committed to throughout: makes the honest case for both sides, because the correct lesson here is not "proprietary SDKs are always inferior" but something more nuanced, which the rest of this post will earn properly rather than simply asserting.

## Part 2: What Application Insights Was, and Why It Was Genuinely Good

Application Insights, in its original, classic form, was Microsoft's Application Performance Monitoring product for applications running on or connected to Azure. Its core software development kit, versioned as what Microsoft's own documentation now calls "Classic API SDK 2.x," offered a central object called TelemetryClient, along with automatic collectors that, largely without any effort from the application developer, picked up incoming web requests, outgoing dependency calls, unhandled exceptions, and basic performance counters, and shipped all of it to Microsoft's own backend, where it appeared, generally within minutes, in polished, Azure-portal-integrated dashboards.

It would be unfair, and inconsistent with this series' stated commitment to evenhandedness, to describe this as merely an inferior stopgap that OpenTelemetry has now rendered obsolete. For a very large number of organizations, particularly those already committed to Azure as their cloud provider, Application Insights delivered genuine, substantial value with remarkably little setup effort. A development team could add one package, register one line of startup code, and have meaningful, request-level visibility into a production application within an afternoon, backed by a company with every commercial incentive to keep that pipeline reliable, since Application Insights was itself a product Microsoft sold. Teams that adopted it during the years it was Microsoft's flagship recommendation were, in the great majority of cases, well served.

## Part 3: The Lock-In Ledger, Applied to This Specific Case

The cost of that convenience, precisely as described in Part 2 of this series two days ago, showed up later, when circumstances changed. An application built directly against TelemetryClient, with proprietary Application Insights method calls scattered through its request-handling code, could not simply be pointed at a different backend without meaningful rework. If an organization later wanted to adopt a multi-cloud strategy, switch to a different observability vendor for cost or feature reasons, or simply reduce its dependence on any single vendor's proprietary interfaces as a matter of general engineering hygiene, the instrumentation itself — not merely the destination the data flowed to — had to be revisited.

This is precisely the switching-cost dynamic this series described in the abstract two days ago, now made concrete with a specific named product, a specific named company, and, as the next section shows, a specific, dated resolution.

## Part 4: Microsoft's Pivot, Precisely Dated

Microsoft's own current documentation states plainly: the Application Insights Classic API SDK 2.x for .NET is deprecated, and it retires on March 31, 2027. That documentation directs organizations still using it toward one of two paths. The first is Application Insights SDK 3.x, described by Microsoft as a nonbreaking upgrade path: it preserves most of the familiar TelemetryClient and TelemetryConfiguration application programming interfaces that existing code already calls, so that most classic Track-prefixed method calls continue to function largely unchanged after the upgrade — but internally, SDK 3.x routes that same data through an OpenTelemetry-based implementation underneath, using the Azure Monitor OpenTelemetry Exporter to actually send the resulting telemetry onward. The second, and the path Microsoft's own documentation states it prefers for genuinely new applications, or for any application willing to invest in a real migration rather than a compatibility bridge, is to adopt the Azure Monitor OpenTelemetry Distro directly, rewriting instrumentation to use OpenTelemetry's own APIs from the outset. Microsoft's documentation is explicit that these two paths should not be combined: an application should not run Application Insights SDK 3.x and the Azure Monitor OpenTelemetry Distro side by side.

It is worth pausing on what this sequence of facts actually demonstrates. Microsoft did not merely add OpenTelemetry as one option among several while continuing to promote its own proprietary alternative indefinitely. Microsoft set a firm, public retirement date for its own older proprietary SDK, and is actively directing its own customers toward the neutral, community-governed standard as the recommended long-term path, even for the many customers whose applications will only ever run on Azure and would face no realistic multi-cloud switching scenario. That is a meaningfully stronger signal about the strength of the underlying case for OpenTelemetry than a competitor's third-party endorsement would be, precisely because Microsoft's own short-term commercial interest, considered narrowly and in isolation, would arguably have favored keeping customers inside proprietary Application Insights tooling for as long as possible.

## Part 5: What a "Distro" Actually Is

The Azure Monitor OpenTelemetry Distro deserves a precise definition of its own, because the word "distro" — short for distribution — recurs throughout this series and gets used loosely elsewhere. A distribution, in this context, is the genuine, unmodified open-source OpenTelemetry SDK, combined with a specific vendor's chosen defaults, a curated bundle of instrumentation libraries that vendor considers most relevant, one or more exporters preconfigured to point at that vendor's own backend, and, typically, a formal support commitment from that vendor covering the whole bundle. Using a distribution does not mean using something other than real OpenTelemetry; the underlying API and SDK code is the same open-source project examined throughout this entire series. What changes is the packaging, the defaults, and who you can call for help when something breaks.

This is, honestly, a partial and reasonable return of a small amount of the very vendor gravity that neutral OpenTelemetry was designed to reduce — and there is nothing dishonest or contradictory about that fact. A distribution trades away a small amount of pure, backend-agnostic neutrality in exchange for a meaningfully lower setup burden and a genuine support relationship, and different organizations will reasonably weigh that trade differently depending on their own circumstances.

## Part 6: The Case for a Vendor Distro, Made Fairly

The Azure Monitor OpenTelemetry Distro, according to Microsoft's own current documentation, offers several concrete advantages over hand-assembling a purely neutral OpenTelemetry setup yourself. It bundles automatic telemetry collection — the instrumentation libraries for common frameworks and dependencies, already selected and configured, rather than left for the developer to research and assemble individually. It supports a feature Microsoft calls Live Metrics, letting an engineer watch a live, in-production application's telemetry stream in close to real time, which is a genuinely distinct capability from the batch-oriented telemetry pipeline OpenTelemetry's core design otherwise assumes. And it comes with an actual support relationship: when something in the pipeline breaks, an organization using the official distro has Microsoft's documented troubleshooting resources and support channels to fall back on, rather than needing to debug a self-assembled pipeline entirely alone.

There is a further, specific and honest limitation worth naming here, because it complicates any simple story of OpenTelemetry as a universal, complete replacement for every proprietary telemetry capability: browser and client-side telemetry. Microsoft's own current FAQ material states directly that the Application Insights JavaScript SDK, not OpenTelemetry, remains the recommended path for browser-based telemetry such as page views and user interactions, and describes OpenTelemetry's own browser story as still meaningfully immature — likely, in Microsoft's own stated assessment, several years away from a browser SDK capable of serving as a genuinely viable replacement for a mature, purpose-built JavaScript telemetry SDK. This is a useful, concrete illustration of a distinction this series will keep returning to: OpenTelemetry's maturity is genuinely uneven across different parts of its own scope, and a fair account of the project has to say so plainly rather than treating "OpenTelemetry" as a single monolithic thing that is either uniformly ready or uniformly not.

## Part 7: The Case for Neutral OpenTelemetry, Made Fairly

Set against the above, the case for adopting OpenTelemetry directly, without a vendor's distro layered on top, rests on exactly the portability argument this series has been building since Part 2. Instrumentation written against the neutral API and SDK can, in principle, be redirected to any OTLP-compatible destination — Azure Monitor, a self-hosted Jaeger and Prometheus stack, a different commercial vendor, or several destinations simultaneously through a Collector's fan-out capability, covered in Part 13 — without revisiting the instrumented application code itself. An organization genuinely committed to multi-cloud operation, or simply committed as a matter of long-term engineering policy to minimizing dependence on any single vendor's proprietary interfaces, gains real, durable value from this, value that compounds the longer the application is expected to live and the more its operating environment might plausibly change.

There is also a broader ecosystem argument. Because OpenTelemetry is a shared, widely adopted standard rather than one vendor's proprietary product, the community-maintained library of instrumentation packages covering popular frameworks, databases, and message queues is considerably larger and more actively maintained across the industry as a whole than any single vendor's own equivalent library is likely to be on its own, since that community effort is not confined to serving one company's customer base.

## Part 8: A Decision Framework, Not a Verdict

This series does not intend to hand down a single universal answer to "should I use a vendor distro or a purely neutral setup," because the honest answer genuinely depends on specifics, and pretending otherwise would be a disservice. A few concrete questions are worth asking directly. How long is this particular application actually expected to live, and how likely is its operating environment to change meaningfully during that lifetime? A short-lived internal tool has much less to gain from portability than a core platform expected to run for a decade. How realistic, honestly, is a multi-cloud or vendor-switching scenario for this specific organization, as opposed to a scenario that sounds prudent in the abstract but is unlikely to actually occur? How large is the team, and how much internal capacity genuinely exists to assemble and maintain a purely self-configured OpenTelemetry pipeline without vendor support to fall back on when something breaks at an inconvenient hour? None of these questions has a universally correct answer, and a reasonable, well-informed engineering team could, quite legitimately, land on either choice after weighing them honestly.

## Part 9: A Worked Comparison in Code

The following two code fragments show the actual difference in practice, for the exact same ASP.NET Core application, verified directly against Microsoft's current published documentation as of this writing. First, using the Azure Monitor OpenTelemetry Distro:

```csharp
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().UseAzureMonitor();

var app = builder.Build();
app.Run();
```

With the connection string supplied through an environment variable, kept out of source code entirely, exactly as Microsoft's own documentation recommends for production deployments:

```bash
APPLICATIONINSIGHTS_CONNECTION_STRING=YOUR_CONNECTION_STRING_HERE
```

Second, the same application using a purely neutral OpenTelemetry setup with an OTLP exporter, pointed at a destination that could be a self-hosted Collector, a different vendor, or, for local development, the free standalone Aspire Dashboard container covered in Part 12:

```csharp
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter());

var app = builder.Build();
app.Run();
```

Notice how structurally similar these two blocks are. Both call `AddOpenTelemetry()`. Both configure tracing and metrics. The difference that actually matters is small in code but large in consequence: `UseAzureMonitor()` bundles a curated set of defaults and locks the exporter destination to Azure Monitor specifically, while `AddOtlpExporter()` speaks the neutral protocol covered fully in Part 14, leaving the actual destination to be decided entirely by configuration outside this code, and changeable later without touching this file again.

The following table summarizes the comparison across the dimensions most likely to matter to a real decision.

| Dimension | Azure Monitor OpenTelemetry Distro | Neutral OpenTelemetry with OTLP |
|---|---|---|
| Setup effort | Lower — curated defaults, one method call | Higher — instrumentation and exporters chosen individually |
| Portability | Locked to Azure Monitor unless reconfigured | Portable to any OTLP-compatible destination |
| Live, real-time view | Yes, via Live Metrics | Not built in; depends on chosen backend |
| Vendor support relationship | Yes, Microsoft-provided | No single vendor; community and self-support |
| Browser/client-side telemetry | Application Insights JavaScript SDK (not OTel) | Immature; not yet a full replacement per Microsoft's own assessment |

## Part 10: How to Spot the Lie — Migration-Guide Archaeology

A vendor's own migration documentation is a genuinely useful, and genuinely underused, source for understanding where that vendor's real strategic interests lie, precisely because a migration guide has to be specific and technically honest in a way marketing copy does not — an organization following a migration guide's instructions will quickly discover, in practice, whether the guide was accurate, so vendors have a strong practical incentive to keep this specific category of documentation truthful even when other marketing material is more aspirational.

Read a migration guide, then, not only for its explicit instructions, but for what its structure reveals about the destination it is steering readers toward. In Microsoft's case, the fact that its own migration documentation exists at all, has a specific retirement date attached to the thing it migrates away from, and offers a clearly preferred path — the neutral distro, over the compatibility-bridge SDK 3.x — tells you plainly which direction Microsoft's own engineering investment is actually flowing, regardless of how any surrounding marketing material might be phrased elsewhere.

More generally, whenever you encounter a claim that a product "supports OpenTelemetry," apply the specific test introduced in yesterday's post: does this specific migration or integration path involve genuine, native OTLP ingestion, or does it route through a proprietary compatibility layer that merely accepts OpenTelemetry-shaped input at the door before converting it internally? Microsoft's own documentation is, again, commendably specific here — it explicitly distinguishes SDK 3.x, which converts and bridges, from the Azure Monitor OpenTelemetry Distro, which does not — and a vendor's willingness to draw that distinction explicitly, in its own words, is itself a small, positive signal about the trustworthiness of its documentation more broadly.

## Part 11: What Comes Tomorrow

We have now spent five days building the conceptual, historical, and architectural foundation this entire series depends on. Tomorrow, the series shifts from context into substance: we begin building OpenTelemetry's actual data model, one signal at a time, starting with traces — spans, attributes, events, links, and the complete, worked JSON example of a checkout journey that will recur, referenced and reused, for the remainder of this series.

## Sources and Further Reading

- Microsoft Learn, ["Monitor .NET and Node.js Applications with Application Insights (Classic API 2.x),"](https://learn.microsoft.com/en-us/azure/azure-monitor/app/classic-api) — the primary source for the March 31, 2027 retirement date of the Classic API SDK 2.x.
- Microsoft Learn, ["Migrate Application Insights Classic API SDKs to Azure Monitor OpenTelemetry,"](https://learn.microsoft.com/en-us/azure/azure-monitor/app/migrate-to-opentelemetry) — the detailed technical migration guidance referenced throughout Parts 4 and 10 of this post.
- Microsoft Learn, ["Enable OpenTelemetry in Application Insights,"](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable) — the source for the exact package names, registration code, and connection-string configuration guidance used in Part 9.
- Microsoft Learn, ["Application Insights FAQ,"](https://learn.microsoft.com/en-us/azure/azure-monitor/app/application-insights-faq) — the source for Microsoft's own stated assessment of OpenTelemetry's browser/client-side maturity referenced in Part 6.

*Tomorrow: traces from first principles — spans, attributes, events, and links, with a complete worked example we will reuse for the rest of this series.*
