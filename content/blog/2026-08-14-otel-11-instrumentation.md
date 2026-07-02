---
title: "Instrumentation: Zero-Code, Libraries, and Doing It by Hand"
date: 2026-08-14
author: mercifulpotato-team
summary: "There are three genuinely different ways telemetry gets into running code: automatic agents, pre-built instrumentation libraries, and manual code you write yourself. Each sees a different slice of reality, and knowing which is which changes what you should expect from any of them."
tags:
  - opentelemetry
  - observability
  - plain-english
  - dotnet
series: "OpenTelemetry from the Ground Up"
---

## Part 1: Three Roads to the Same Destination

Every span, metric, and log record examined in this series so far had to come from somewhere: some piece of code, running at some specific moment, decided to call the API introduced in Part 10 and produce a piece of telemetry. Today's post is about the practical question of where that code actually comes from, because there are three genuinely different answers, each with real, distinct trade-offs, and understanding the difference between them will change what you reasonably expect any one of them to deliver.

The three roads are zero-code, or automatic, instrumentation, which requires touching no application source code at all; instrumentation libraries, pre-built packages that wrap a specific, popular framework or dependency; and manual instrumentation, code a developer writes by hand for their own application's own specific logic. None of the three is simply better than the others in every situation; they see different things, and a mature observability setup, as this post's final section argues, typically layers all three together deliberately.

## Part 2: Zero-Code Instrumentation, Honestly Described

Zero-code instrumentation works by injecting instrumentation into an already-compiled application from the outside, without the application's own source code needing any awareness that this is happening. In .NET specifically, this takes the form of the OpenTelemetry .NET Automatic Instrumentation, installed via a downloadable script that sets up the necessary machinery, after which an application is simply launched with a small set of environment variables set — `OTEL_SERVICE_NAME` to identify the service, `OTEL_RESOURCE_ATTRIBUTES` to attach further identifying details — and telemetry for supported libraries and frameworks begins flowing without a single line of the application's own code being modified. This works across the officially supported .NET runtimes and processor architectures, though it is worth noting, in the spirit of this series' care about precise maturity claims, that ARM64 processor support is currently marked experimental in the project's own documentation, a useful reminder that even within a single feature, different parts can sit at different points on the specified-implemented-stable-adopted ladder introduced back in Part 4.

The honest limitation of this approach, and it is a limitation inherent to the technique rather than a temporary rough edge, is what an agent working entirely from the outside can and cannot see. It can observe things that happen at well-known, well-understood boundaries: an incoming HTTP request arriving, an outgoing database call being made, a framework's own internal lifecycle events firing. It cannot know, and has no way of knowing, what a piece of business logic actually means. An automatic agent watching our checkout service can tell you an HTTP request came in and a database query ran; it cannot tell you that the query was specifically checking whether a particular promotional discount code was still valid, because that meaning exists only in the application's own source code, in the mind of whoever wrote it, not in any pattern a generic, external agent could recognize.

## Part 3: Instrumentation Libraries

Sitting between fully automatic and fully manual is the broad and genuinely important category of instrumentation libraries: purpose-built packages, typically maintained either by the OpenTelemetry project's own contributor community or by the maintainers of the framework being instrumented, that add telemetry-producing code to a specific, named dependency. Registering ASP.NET Core instrumentation and HttpClient instrumentation, exactly as shown in the `Program.cs` fragment back in Part 10, are both examples: rather than writing spans for incoming and outgoing HTTP calls by hand, a developer registers a small amount of configuration, and a well-tested, purpose-built library does the actual work.

This category deserves a specific, concrete warning that trips up newcomers constantly, and stating it plainly here will save real debugging time later: an `ActivitySource` or `Meter` that your own code creates, but that is never explicitly registered with the SDK, produces absolutely nothing — silently. There is no error, no warning, no exception. The calls simply do nothing, exactly as Part 10's discussion of the API's no-op minimal implementation would predict if the SDK genuinely were entirely absent, except here the SDK is present and working perfectly well for everything else; it simply was never told to listen to this particular named source. Here is that exact failure, shown side by side with the fix, using the wildcard registration pattern that is worth adopting as a standing habit specifically to avoid this trap:

```csharp
// SILENTLY PRODUCES NOTHING: "MercifulPotato.Checkout" was never registered
var activitySource = new ActivitySource("MercifulPotato.Checkout");

// THE FIX: register the source (or a wildcard covering it) with the SDK
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("MercifulPotato.*"));   // wildcard covers this and future sources
```

The wildcard pattern shown on the last line, registering `"MercifulPotato.*"` rather than each individual source name one at a time, is a small habit worth adopting deliberately: a new `ActivitySource` created next month, as the application grows, is automatically covered by the existing registration, rather than silently producing nothing until someone remembers to add it to a growing, easily forgotten list.

## Part 4: Manual Instrumentation as Storytelling

Manual instrumentation is code a developer deliberately writes, by hand, to describe something specific about their own application's own logic — precisely the kind of meaning Part 2 established that an automatic agent has no way of inferring on its own. Where an automatic agent or an instrumentation library can tell you, correctly but generically, that an HTTP call happened, only a developer with real knowledge of the business can add a span named "apply-promotional-discount," with an attribute recording which specific discount code was checked and whether it was found valid, because that meaning exists only in the developer's own understanding of what the code is actually for.

This is worth framing plainly as an act of storytelling, not merely instrumentation for its own sake: the goal of manual instrumentation is to answer, in advance, the specific questions a future engineer — quite possibly the very same developer, at three in the morning, several months from now — will actually need answered while investigating a real problem. "Was the discount code valid?" is a question with genuine diagnostic value; recording it as a span attribute today means that question already has an answer waiting, the next time this exact code path is involved in an investigation.

## Part 5: A Layering Strategy for a Small Team

Putting all three approaches together into one coherent plan, rather than treating them as mutually exclusive alternatives to choose between, is the practical advice this post has been building toward. A sensible, deliberately staged approach for a small team looks roughly like this: start with automatic, zero-code instrumentation to get a genuine baseline of visibility running on day one, with essentially no engineering effort spent and no source code touched. Add the specific instrumentation libraries relevant to the frameworks and dependencies actually in use — the database driver, the HTTP client, the message queue client — each contributing well-tested, purpose-built telemetry for exactly the well-known boundary it wraps. And finally, hand-write manual spans for the handful of business operations that genuinely matter most to the organization's own specific goals — not everything, since instrumenting every single line of business logic manually would be exhausting and would drown the genuinely important spans in noise, but the handful of operations a team would most want a clear, named, attribute-rich record of if something ever went wrong with them.

## Part 6: How to Spot the Lie — "Instant Observability, No Code Changes"

A specific marketing phrase deserves today's skeptical test, because it captures a genuine half-truth in a way that can mislead a buyer who has not read this post's Part 2 carefully: "instant observability, no code changes required." This is not exactly a lie, and this series does not intend to accuse it of being one; zero-code instrumentation genuinely exists, genuinely works, and genuinely requires no source code modification, exactly as Part 2 described. The half of the claim quietly left unstated is the limitation from that same section: what a zero-code agent delivers is visibility into well-known, generic boundaries — incoming requests, outgoing calls, framework internals — and nothing whatsoever about what your own application's business logic actually means.

The test this section offers is simple and directly actionable: when evaluating a claim of instant, code-free observability, pick one genuinely specific business question relevant to your own application — "was this specific discount code applied correctly," in our checkout example — and ask whether the zero-code solution being demonstrated can actually answer it. It cannot, and it was never going to be able to, because that answer requires the manual, storytelling instrumentation from Part 4, which no external agent can write on a developer's behalf. A vendor or a colleague who acknowledges this limitation plainly, rather than letting the word "instant" imply more than it can deliver, is giving you a more trustworthy account of what you are actually buying.

## Part 7: What Comes Tomorrow

Today's post has been deliberately general, covering instrumentation approaches across languages and frameworks broadly. Tomorrow's post narrows sharply and gets fully hands-on, in the language and framework this site itself runs on: a complete, working ASP.NET Core example built up piece by piece in full C# code, resolving the specific .NET-shaped preview from Part 6 of this series — the platform's own built-in `ActivitySource` and `Meter` facilities, wired together with `ILogger`, exported over OTLP, and viewed for free using the standalone Aspire Dashboard.

## Sources and Further Reading

- OpenTelemetry Project, ["Zero-code Instrumentation,"](https://opentelemetry.io/docs/concepts/instrumentation/zero-code/) opentelemetry.io — the project's own conceptual framing of automatic instrumentation and its scope.
- OpenTelemetry .NET Documentation, [".NET Zero-code Instrumentation,"](https://opentelemetry.io/docs/zero-code/dotnet/) opentelemetry.io — the primary source for the installer-script setup, environment-variable configuration, and current compatibility notes including the experimental ARM64 status referenced in Part 2.
- OpenTelemetry .NET Documentation, ["Using Instrumentation Libraries,"](https://opentelemetry.io/docs/languages/dotnet/libraries/) opentelemetry.io — the source for the `AddSource` wildcard registration pattern shown in Part 3.

*Tomorrow: hands-on .NET — a complete ASP.NET Core example in full C# code, using ActivitySource, Meter, and ILogger together, exported over OTLP and viewed for free.*
