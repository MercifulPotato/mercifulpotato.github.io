---
title: "What OpenTelemetry Is — and What It Is Not"
date: 2026-08-07
author: mercifulpotato-team
summary: "OpenTelemetry is a specification and a set of tools for generating and moving telemetry — not a dashboard, not a database, not a monitoring product. This post draws the boundary precisely, piece by piece."
tags:
  - opentelemetry
  - observability
  - plain-english
  - software-engineering
series: "OpenTelemetry from the Ground Up"
---

## Part 1: The Single Most Common Misunderstanding

Ask someone who has only heard the term "OpenTelemetry" in passing to describe what it does, and a common wrong answer sounds something like this: "it's a monitoring tool" or "it's a dashboard for looking at your servers." This is understandable, because most of the tools an average engineer encounters in this space genuinely are complete, end-to-end products — you install something, and shortly afterward you are looking at a screen full of graphs. OpenTelemetry does not work that way, and understanding precisely why is the entire purpose of today's post.

Here is the single sentence this whole post exists to unpack: OpenTelemetry is a specification, together with a set of tools built to that specification, for generating, collecting, and exporting telemetry data — and it is deliberately, by design, not a place where that data ends up being stored, searched, or displayed. If you remember only one sentence from this entire series, this is a strong candidate, because a surprising fraction of confusion about OpenTelemetry in the wild traces back to skipping past this distinction.

## Part 2: The Pieces, Introduced One at a Time

OpenTelemetry, according to its own official documentation, is developed and organized around what it calls signals — tracing, metrics, logging, and, more recently and more tentatively, profiling — each of which we will cover in far greater technical depth in Parts 6, 8, 9, and 20 of this series respectively. What matters today is that each signal, according to the project's own published architecture, is built from the same four recurring pieces, and it is worth walking through each piece slowly, because these four words will recur, precisely and consistently, for the rest of this series.

The API, first, is the fixed set of function calls a programmer's own code, or a library the programmer depends on, actually invokes: "start a span," "record a measurement," "write a log entry." Crucially, and this is a design choice we examine in exhaustive detail in Part 10 tomorrow-plus-six, the API by itself does essentially nothing observable. If no further setup has been done, calling the API is close to a safe no-operation: your code runs exactly as it would have run anyway, just with these calls quietly present and quietly doing nothing.

The SDK, second, is the actual working implementation that makes those API calls meaningful. An application's own startup code chooses to install an SDK, configures it with decisions like "where should finished spans be sent" and "how often should metrics be flushed," and from that point onward, the same API calls that previously did nothing now produce real telemetry data, processed and forwarded according to however the SDK was configured.

The OpenTelemetry Protocol, universally abbreviated OTLP and covered in full technical detail in Part 14, is the specific, formally defined way telemetry data is encoded and sent over a network once it leaves an application. Think of OTLP as playing a role somewhat like a shipping container's standardized dimensions: it does not matter what is inside the container — traces, metrics, or logs — because any dock built to handle the standard container shape can receive it, regardless of who packed it or who built the dock.

The Collector, fourth and last, is a separate, standalone piece of software — not part of any one application, but its own independent program, typically run as infrastructure alongside the applications it serves — whose job is to receive telemetry, optionally transform or filter it in flight, and forward it onward to one or more final destinations. We devote the entirety of Part 13 to the Collector because its role in a realistic production setup is large enough to deserve that space; for today, the essential fact is simply that it exists as a distinct, optional-but-common intermediary layer, not a mandatory core requirement for OpenTelemetry to function at all.

Layered on top of these four core pieces, and touching every one of them, are semantic conventions: an agreed, shared dictionary specifying what a given piece of data should be named and how it should be structured, so that "the HTTP status code of this request" is represented identically whether the code that produced it was written in C#, Python, or Go. Part 15 of this series is devoted entirely to why this dictionary matters and why building it correctly has turned out to be genuinely difficult.

## Part 3: A Table, and Then the Journey Traced in Full

The following table restates the four core pieces alongside the current maturity of each signal, drawn directly from the project's own published specification status summary as of this writing — a page that, notably, the project itself explicitly recommends checking regularly, since it changes as work progresses.

| Signal | What it captures | API status | SDK status | Protocol status |
|---|---|---|---|---|
| Tracing | The path of one request through a system | Stable, long-term support | Stable, long-term support | Stable |
| Metrics | Aggregated numeric measurements over time | Stable | Stable | Stable |
| Logging | Discrete, timestamped event records | Stable | Stable | Stable |
| Profiles | Code-level resource consumption (CPU, memory) by function | Younger; still developing | Younger; still developing | Younger; still developing |

A note on that last row, in the spirit of the fact-checking rigor this series insists on throughout: profiles is the newest signal by a wide margin, and different sources describe its exact current maturity level slightly differently even at the same moment in time — some calling it alpha, others release-candidate-adjacent — which is itself a useful, concrete illustration of exactly the caution this series urges: for anything this new and fast-moving, treat the project's own primary specification pages, not secondary blog summaries, as the source of truth, and expect the precise status to have moved again by the time you read this. We return to profiles in careful, hedged detail in Part 20.

Now let us trace one complete journey through all four pieces at once, using the checkout example from Part 1 of this series. A programmer, writing checkout logic in C#, calls the tracing API to start a span named "process-payment." Because the application's startup code previously installed and configured an SDK, that call is not a no-operation; the SDK creates a real, timed span object, attaches whatever attributes the programmer specified, and, when the span ends, hands it to an exporter the SDK was configured with. That exporter serializes the span using OTLP and sends it, typically over the network, either directly to a backend or, more commonly in a production setup, to a Collector. The Collector, on receipt, might add a resource attribute identifying which specific server instance produced the data, might drop it entirely if it happens to match a filtering rule, or might simply forward it onward, again using OTLP, to one or more final destinations — a place like a trace-storage backend, covered in Part 17, where a human can finally search for and look at it.

Notice, across that entire journey, that nothing in the sentence just written described "OpenTelemetry" ever storing anything for longer than the brief instant it takes to process and forward a piece of data, nor did anything describe a screen, a graph, or a query language. That absence is not an oversight in the description. It is the accurate description of what the project actually does.

## Part 4: Announced, Implemented, Stable, Adopted — A Distinction Worth Repeating Precisely

This series introduced a phrase in Part 3 that deserves its full, careful application here, because today's material is exactly where it matters most: specified is not the same as implemented, implemented is not the same as stable, and stable is not the same as adopted.

A feature can appear in OpenTelemetry's written specification — meaning a committee of contributors has agreed, on paper, what it should do and how — well before any actual software exists that does it. Once software exists, it typically starts in what the project's own published lifecycle explicitly labels an "experimental" state: released, real, usable for testing, but still subject to breaking changes as the design gets refined based on real-world feedback. Only after that does a feature graduate to "stable," at which point the project makes a formal, meaningful promise: stable components are backward compatible and covered under long-term support, meaning future versions will not casually break code written against them. And even a feature that has reached stable status in the specification does not automatically mean every one of the roughly fifteen or so language implementations OpenTelemetry maintains has actually finished building it to that same standard; each language's own software-development kit progresses along this same draft-to-experimental-to-stable path somewhat independently, on its own schedule, and the project's own documentation explicitly directs readers to check each language's own repository for its current status rather than assuming uniformity.

This four-stage distinction is not pedantry for its own sake. It is the single most reliable tool for correctly interpreting almost any claim you will encounter about OpenTelemetry capability, and we will apply it again explicitly in Parts 10, 14, 18, and 20.

## Part 5: What Flows Where — The Escape Hatch, Honestly Priced

Return, for a moment, to yesterday's discussion of vendor lock-in and switching costs. OpenTelemetry's structural answer to that problem is now visible in the architecture just described: because application code calls a neutral API, and because the choice of where data ultimately ends up lives entirely in configuration — which exporter the SDK uses, which destinations the Collector forwards to — switching from one backend to another can, in the best case, become a matter of changing a configuration file rather than rewriting instrumented code scattered throughout an application.

It would be dishonest, however, to present this as a complete, cost-free escape from lock-in, and this series does not intend to be dishonest in either direction — neither dismissive of OpenTelemetry's genuine achievement here, nor uncritically promotional about it. Switching the destination your telemetry data flows to is not the same thing as switching everything built on top of that data once it arrives. Dashboards, saved queries, alert rules, and the specific query language a team has spent months learning to use fluently on one particular backend generally do not transfer automatically to a different backend, even when both speak OTLP fluently and receive functionally identical data. A team that switches backends using OpenTelemetry's portability will still, in almost every realistic case, need to rebuild its dashboards and alerts by hand on the new system. What OpenTelemetry genuinely removes is the re-instrumentation cost — the part of yesterday's fragmentation story that lived inside application source code — not the entire cost of a migration in every dimension. This honest, partial framing of the value proposition will resurface directly in Part 5 tomorrow, when we examine a specific, real-world instance of exactly this trade-off: Microsoft's own guidance for organizations moving away from its older, proprietary Application Insights software development kit.

## Part 6: How to Spot the Lie — "Powered by OpenTelemetry"

You will encounter this phrase, or close variants of it, on the marketing pages of a great many observability products, and it is worth having a precise, three-way test ready, because the phrase can honestly describe at least three structurally different arrangements, and a buyer who does not distinguish between them can end up with meaningfully less portability than the phrase implied.

The first, strongest arrangement is native OTLP ingestion: the backend genuinely accepts the standard OTLP protocol directly, at its front door, with no proprietary conversion step required, meaning any OpenTelemetry-instrumented application, using any Collector configuration, can send data there simply by pointing an exporter at the right address. The second, weaker arrangement is conversion at the door: the vendor accepts OTLP as a courtesy on the way in, but immediately transforms the data into its own internal proprietary format for actual storage and processing, which may mean certain OpenTelemetry-specific concepts, attributes, or structures get quietly lossy-compressed or dropped in translation, since the vendor's internal model was not originally designed around OpenTelemetry's data model at all. The third, weakest arrangement is exporter-only support: the vendor ships a plugin that lets its own separate, proprietary agent optionally also emit some OpenTelemetry-shaped data on the way out, as an interoperability courtesy to other tools, while the vendor's own primary data collection continues to run through its own proprietary instrumentation entirely, meaning "powered by OpenTelemetry" describes a secondary export path rather than the core plumbing.

The practical version of this test, which you can genuinely apply in a sales conversation or a technical evaluation, is simply to ask, directly and specifically: does your product accept OTLP natively, at full fidelity, for all three of tracing, metrics, and logs, or does OpenTelemetry compatibility describe something narrower than that? A vendor confident in a strong, native answer will typically be glad to say so plainly and specifically, because it is a genuine selling point; a vague or evasive answer to a specific, technical question of this kind is itself useful information. We return to a fuller, backend-focused version of this exact test in Part 17, once the necessary background on OTLP itself from Part 14 is in place.

## Part 7: What Comes Tomorrow

Today's post drew the map: what OpenTelemetry consists of, and the sharp boundary around what it deliberately excludes. Tomorrow's post takes the single specific comparison this series' brief asked for by name and works through it in full, honest detail on both sides: OpenTelemetry against a genuine, real, currently evolving proprietary alternative — Microsoft's Azure Application Insights — including the exact, dated retirement notice Microsoft has issued for its own older software development kit, and a fair accounting of what a dedicated vendor distribution can still offer that a purely neutral setup, by design, does not.

## Sources and Further Reading

- OpenTelemetry Project, ["Specification Status Summary,"](https://opentelemetry.io/docs/specs/status/) opentelemetry.io — the project's own current, authoritative account of signal-by-signal maturity; check this page directly rather than relying on any secondary summary, including this one, for anything time-sensitive.
- OpenTelemetry Project, ["What is OpenTelemetry?"](https://opentelemetry.io/docs/what-is-opentelemetry/) and ["Overview"](https://opentelemetry.io/docs/specs/otel/overview/) — the project's own framing of its architecture and core terminology.
- OpenTelemetry Project, ["Versioning and Stability for OpenTelemetry Clients,"](https://opentelemetry.io/docs/specs/otel/versioning-and-stability/) opentelemetry.io — the formal definitions of the draft, experimental, stable, deprecated, and removed lifecycle stages referenced throughout this post.
- OpenTelemetry Blog, stabilization and release-practices announcement, opentelemetry.io/blog — the project's own account, referenced again in Part 18, of the tension between rapid feature growth and production-grade reliability.

*Tomorrow: OpenTelemetry against Azure Application Insights, argued fairly on both sides — including Microsoft's own dated deprecation notice for its older proprietary SDK.*
