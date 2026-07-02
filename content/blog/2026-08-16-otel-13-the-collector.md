---
title: "The Collector: Receivers, Processors, Exporters, and Pipelines"
date: 2026-08-16
author: mercifulpotato-team
summary: "Between an application and its final backend usually sits a separate, independent piece of infrastructure: the OpenTelemetry Collector. This post explains why teams add it, what its three-part pipeline actually does, and how to read one real config file line by line."
tags:
  - opentelemetry
  - observability
  - plain-english
  - devops
series: "OpenTelemetry from the Ground Up"
---

## Part 1: Why Not Just Send It Straight There?

Part 12 of this series ended with our checkout service exporting telemetry directly to a local Aspire Dashboard using OTLP, and that arrangement genuinely works, and is genuinely fine for local development. A reasonable question follows: if an application can export straight to a destination, why does nearly every real production OpenTelemetry deployment add a separate, additional piece of infrastructure in between?

The answer is not that direct export is broken. It is that a small number of genuinely important capabilities become considerably easier to get right once they live in one shared, independent place rather than being reimplemented inside every single application. Retrying a failed network send, buffering data during a brief backend outage, keeping backend credentials out of every single application's own configuration, enriching data with information no individual application necessarily knows about itself, and sending the same data to more than one destination at once are all things an application could technically do on its own — but doing each of these well, correctly, and consistently across every service in an organization is a genuinely large amount of duplicated engineering effort. The OpenTelemetry Collector exists to do this work once, centrally, so that no individual application has to.

## Part 2: The Three-Part Pipeline

The Collector's internal architecture, once understood, explains most of what it can do. Data flows through three kinds of components, always in the same order: receivers, which are how data gets in; processors, which optionally transform data in flight; and exporters, which are how data gets out.

A receiver's job is to ingest telemetry from some source. The most relevant receiver for this series is the `otlp` receiver, which accepts data over the OpenTelemetry Protocol covered fully in Part 14 tomorrow — precisely what our checkout service from Part 12 would send if it were pointed at a Collector instead of directly at a backend. Other receivers exist for other sources entirely: reading log files directly off disk, scraping metrics from systems that use a pull-based model rather than pushing data out, or accepting data in older, non-OpenTelemetry formats from systems that have not yet adopted OTLP.

A processor sits in the middle and optionally transforms data as it passes through, before it reaches any exporter. A `memory_limiter` processor, which the Collector's own maintainers recommend including in nearly every real deployment, protects the Collector itself from running out of memory if telemetry arrives faster than it can be processed and sent onward, by rejecting excess data once a configured limit is approached rather than crashing outright. An `attributes` processor can add, rename, or remove specific fields — inserting a `deployment.environment` attribute onto every piece of data passing through, for instance, without requiring every single application to remember to set that attribute itself. A `probabilistic_sampler` processor can apply the sampling logic Part 16 of this series covers in full, centrally, rather than requiring every application to implement its own sampling decision independently.

An exporter's job is to send data onward to one or more final destinations, and, critically, a Collector can be configured with several exporters at once, sending the identical stream of incoming data to more than one place simultaneously — a capability with real, concrete uses this series returns to directly in Part 17.

## Part 3: A Real, Fully Annotated Configuration

Here is a genuine, working Collector configuration, verified against the project's own current documentation, receiving our checkout service's OTLP traffic, enriching it, protecting itself from memory exhaustion, and exporting to a local debug destination:

```yaml
receivers:
  otlp:
    protocols:
      grpc:
        endpoint: 0.0.0.0:4317
      http:
        endpoint: 0.0.0.0:4318

processors:
  memory_limiter:
    check_interval: 5s
    limit_mib: 4000
    spike_limit_mib: 500

  attributes:
    actions:
      - key: deployment.environment
        value: production
        action: insert

exporters:
  debug:
    verbosity: detailed

service:
  pipelines:
    traces:
      receivers: [otlp]
      processors: [memory_limiter, attributes]
      exporters: [debug]
    metrics:
      receivers: [otlp]
      processors: [memory_limiter]
      exporters: [debug]
    logs:
      receivers: [otlp]
      processors: [memory_limiter]
      exporters: [debug]
```

Reading this top to bottom, in prose, exactly as the Collector itself would apply it: the `otlp` receiver opens two network ports, one for gRPC on the standard port 4317 and one for HTTP on 4318 — the same standard ports our checkout service from Part 12 was already configured to send to, meaning no change is needed on the application side at all to route through this Collector instead of directly to a backend. Incoming traces pass first through `memory_limiter`, protecting the Collector itself, and then through `attributes`, where our checkout trace from Part 6 — recall its root span, named `checkout`, with its `order.id` attribute already attached — would gain a new `deployment.environment: production` attribute it never had when it left the application, because the application itself has no particular reason to know or care which environment it is running in beyond what it was told at startup. Notice, too, a detail worth internalizing directly: configuring a receiver, processor, or exporter in their respective top-level sections does not, by itself, enable anything; a component only takes effect once it is explicitly listed inside a pipeline in the `service` section, and each of the three pipeline types — `traces`, `metrics`, and `logs` — is configured independently, which is why this example lists `attributes` only under the `traces` pipeline, since that specific example only makes sense applied to spans.

## Part 4: Topologies — Agent, Gateway, and Both

The Collector can be deployed in more than one physical arrangement, and the choice matters for how a real production system actually behaves. In an agent pattern, a small Collector instance runs alongside each individual application — often literally on the same machine, or as a sidecar in the same container group — receiving that one application's telemetry with minimal network distance and minimal risk of losing data to a network problem between the application and its first point of contact. In a gateway pattern, a smaller number of larger, centrally managed Collector instances receive telemetry from many applications at once, providing a single, more easily managed place to apply organization-wide processing rules, sampling policy, and routing decisions. Many real deployments combine both: agents running close to each application, forwarding onward to a smaller number of central gateway instances, which apply the heavier, shared processing before finally exporting to backends — an arrangement the Collector's own documentation names directly as the agent-to-gateway pattern.

Each choice trades off differently along the dimension that matters most when something goes wrong: what happens if a given Collector instance becomes unavailable. An agent close to one application affects only that application if it fails; a shared gateway serving many applications at once is a more attractive target for careful redundancy planning precisely because its failure has a correspondingly wider blast radius.

## Part 5: Operating the Operator

It is worth stating plainly, in the spirit of this series' running honesty about cost, that the Collector is itself a piece of software an organization now has to run, monitor, and keep healthy — and a Collector that is silently failing, silently dropping data, or silently falling behind is a genuinely dangerous kind of failure, because everything upstream of it may continue reporting success while the actual telemetry a team is depending on quietly stops arriving. The Collector, sensibly, can be configured to emit its own internal telemetry about its own health — queue depths, dropped-data counts, processing latency — and a mature deployment treats watching the watcher as a real, non-optional part of the job, not an afterthought.

## Part 6: How to Spot the Lie — The Pipeline Diagram With No Failure Arrows

Architecture diagrams presenting a Collector-based pipeline, whether in a vendor's sales deck or an internal design document, near-universally show the happy path: application, arrow, Collector, arrow, backend, everything flowing smoothly left to right. The test this section offers is to look specifically for what such a diagram omits, because what happens at each of those arrows when the box on the receiving end is temporarily unavailable is precisely the information a clean, confident-looking diagram tends not to show.

Ask, specifically and directly: if the Collector itself is briefly unreachable, does the application's own SDK buffer data locally, retry, or simply drop it silently? If the backend the Collector exports to is briefly unavailable, does the Collector's own exporter queue and retry, and for how long, before it too begins dropping data? Back-pressure and data loss at each of these specific junctions are exactly the questions a genuinely informative architecture review should be able to answer plainly, and a diagram — or a person presenting one — unable or unwilling to answer them is not necessarily hiding something maliciously, but is very likely presenting a design that has not yet been pressure-tested against the failure modes that actually determine whether an observability pipeline can be trusted during the very incidents it exists to help diagnose.

## Part 7: What Comes Tomorrow

Today's post examined the Collector as an application-level concept — what it does and why teams add it. Tomorrow's post drops one level deeper, into the wire format that makes the whole arrangement possible in the first place: OTLP, the OpenTelemetry Protocol, dissected at the level of what actually crosses the network between an application, a Collector, and a backend, including gRPC versus HTTP, and a complete, annotated look at our checkout trace serialized exactly as it would really travel.

## Sources and Further Reading

- OpenTelemetry Project, ["Configuration,"](https://opentelemetry.io/docs/collector/configuration/) opentelemetry.io — the primary source for the receiver, processor, exporter, and service-pipeline syntax used throughout Part 3 of this post, including the `memory_limiter` and `attributes` processor examples.
- OpenTelemetry Project, ["Quick Start,"](https://opentelemetry.io/docs/collector/quick-start/) opentelemetry.io — the source for the standard OTLP ports (4317 gRPC, 4318 HTTP) and the current core-distribution Docker image reference.
- OpenTelemetry Project, ["Agent-to-Gateway Pattern,"](https://opentelemetry.io/docs/collector/deploy/other/agent-to-gateway/) opentelemetry.io — the source for the combined topology described in Part 4.

*Tomorrow: OTLP — the wire protocol that carries everything, dissected field by field, with our checkout trace serialized exactly as it would really travel across the network.*
