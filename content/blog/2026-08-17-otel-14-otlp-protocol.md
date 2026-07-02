---
title: "OTLP: The Wire Protocol That Carries Everything"
date: 2026-08-17
author: mercifulpotato-team
summary: "Beneath every API call and every Collector pipeline sits one final layer: the actual bytes that cross the network. This post dissects OTLP — the protocol that makes any OpenTelemetry sender able to talk to any receiver."
tags:
  - opentelemetry
  - observability
  - plain-english
  - software-engineering
series: "OpenTelemetry from the Ground Up"
---

## Part 1: Why a Protocol Is the Real Standard

This series has spent thirteen days building up layer after layer of OpenTelemetry's architecture: spans and traces in Parts 6 and 7, metrics in Part 8, logs in Part 9, the API and SDK split in Part 10, instrumentation approaches in Part 11, a working application in Part 12, and the Collector in Part 13. Every one of those layers, at some point, needed to actually get data from one place to another over a real network connection. Today's post is about the specific, formally defined answer to that final, unglamorous, and absolutely essential question: what, precisely, crosses the wire?

This matters more than it might first appear, and it is worth stating plainly why. Part 3 of this series established a three-part test for a genuine standard: a public specification, multiple independent implementations, and neutral governance. APIs and SDKs, covered in Part 10, standardize how application code speaks. But it is the wire protocol — the actual bytes sent over the network — that determines whether a sender built by one team, in one language, using one particular library, can actually be understood by a receiver built by an entirely different team, in an entirely different language, with no coordination between them whatsoever. This is the layer where the neutrality this series has praised repeatedly actually gets cashed out into something concrete and checkable.

## Part 2: What OTLP Is, Precisely

The OpenTelemetry Protocol, universally abbreviated OTLP, is defined by its own formal specification as describing the encoding, transport, and delivery mechanism of telemetry data between telemetry sources, intermediate nodes such as Collectors, and telemetry backends. As of this writing, OTLP's own specification status, verified directly against the project's current documentation, is stable for the trace, metric, and log signals — the three classic signals this entire series has built from first principles — and still in development for the newer profiles signal introduced in Part 4, a useful, concrete confirmation of exactly the uneven, signal-by-signal maturity this series has tracked carefully throughout.

OTLP is, at its core, a request-and-response protocol: a client — typically an application's SDK, or a Collector forwarding data onward — sends a request containing telemetry, and a server — typically a Collector, or a backend — replies with a response acknowledging it. The specification defines this pattern once, generically, as an operation called `Export`, applied separately to each signal: an `ExportTraceServiceRequest` for traces, an `ExportMetricsServiceRequest` for metrics, and an `ExportLogsServiceRequest` for logs.

Underlying all of this is a specific, formally defined encoding: Protocol Buffers, a compact, schema-defined binary format for structuring data, originally developed at Google and now used across a huge share of the software industry for exactly this kind of efficient, structured data exchange between programs. Think of a schema, in this sense, as a strict, agreed-upon form: it specifies exactly which fields exist, in what order, and of what type, so that a program reading the data knows precisely what to expect without needing to guess or parse loosely structured text.

## Part 3: Two Transports, One Underlying Model

OTLP's specification defines exactly two ways this Protocol Buffers-encoded data can actually travel across a network: gRPC and HTTP.

gRPC, a remote-procedure-call framework itself also originally developed at Google, is defined by OTLP's specification as the primary transport, and it is what our checkout service from Part 12 used by default when connecting to the local Aspire Dashboard, and what the Collector configuration from Part 13 opened on its default port. HTTP, defined as OTLP's second, alternative transport, exists specifically to serve environments where gRPC is inconvenient or unsupported — certain browser environments, corporate network configurations with proxies that handle ordinary HTTP more gracefully than gRPC's particular underlying mechanics, and clients whose language or platform simply has more mature HTTP tooling available than gRPC tooling.

Both transports carry the same underlying Protocol Buffers-encoded data; the specification also defines an alternative JSON encoding of that same Protobuf schema specifically to let a human, or a simple tool without a full Protobuf toolchain, read and inspect OTLP data directly, at the cost of being considerably more verbose on the wire than the binary form actually used for real production traffic. Verified directly against the current specification, the conventional default ports are 4317 for OTLP over gRPC and 4318 for OTLP over HTTP — precisely the ports this series' own Collector configuration in Part 13 opened, and precisely the port our application in Part 12 pointed its exporter at.

The following table summarizes the trade-off in plain terms.

| Transport | Typical use | Underlying encoding |
|---|---|---|
| gRPC | Application-to-Collector, Collector-to-Collector; the specification's primary transport | Protocol Buffers, binary |
| HTTP | Browser environments, restrictive networks, simpler client tooling | Protocol Buffers, binary (or JSON, for human inspection) |

## Part 4: Reading an OTLP Payload

Recall the three-span checkout trace built carefully in Part 6, and its `traceparent` header from Part 7. Here is a small, deliberately simplified illustration of that same root `checkout` span, shaped according to the actual fields the OTLP wire format defines — a resource, describing which service produced the data, and a list of spans, each carrying the exact identifiers this series has used consistently since Part 6:

```json
{
  "resourceSpans": [
    {
      "resource": {
        "attributes": [
          { "key": "service.name", "value": { "stringValue": "checkout-service" } }
        ]
      },
      "scopeSpans": [
        {
          "spans": [
            {
              "traceId": "4bf92f3577b34da6a3ce929d0e0e4736",
              "spanId": "00f067aa0ba902b7",
              "name": "checkout",
              "attributes": [
                { "key": "order.id", "value": { "stringValue": "ORD-88213" } }
              ]
            }
          ]
        }
      ]
    }
  ]
}
```

Reading this structure top to bottom: `resourceSpans` is the outermost container, grouping everything by which resource — which service, in our case `checkout-service` — actually produced it, precisely the `ConfigureResource` call from Part 12's `Program.cs`. Nested inside, `scopeSpans` groups spans by which specific instrumentation library or `ActivitySource` created them, before finally arriving at the individual `spans` themselves, each carrying the same `traceId` and `spanId` fields this series dissected byte by byte all the way back in Part 6, now shown in the actual shape the wire format defines them, rather than the deliberately simplified illustration Part 6 used to introduce the underlying concept for the first time. It is worth being precise here, in the same spirit as the caveat Part 6 itself carried: this is a simplified, human-readable illustration of OTLP's JSON encoding, not the literal binary Protocol Buffers bytes a real sender and receiver actually exchange — but it is an accurate representation of the fields and structure that binary form ultimately encodes.

## Part 5: One Protocol, Every Signal

A detail worth stating plainly, because it is easy to lose sight of amid all the signal-specific detail this series has built up across Parts 6 through 9: OTLP is not three separate protocols wearing a shared name. It is one protocol, with one shared underlying request-response model and one shared encoding, applied consistently across traces, metrics, and logs, with the newer profiles signal following the identical pattern as it continues to mature. This uniformity is precisely why a single Collector configuration, of the kind built in Part 13, can receive all three signals through the very same `otlp` receiver, on the very same two ports, and route each one into its own appropriately named pipeline — `traces`, `metrics`, `logs` — without needing three entirely different ingestion mechanisms bolted together.

## Part 6: How to Spot the Lie — "OTLP-Compatible"

This series introduced, in Part 4, a three-way test for the phrase "powered by OpenTelemetry": native ingestion, conversion at the door, or exporter-only support as an afterthought. Today's post is where that test can be sharpened further, with the specific technical vocabulary this post has just built.

When a product claims "OTLP-compatible" or "OTLP support," the sharpened version of Part 4's test asks four specific, checkable questions. Which signals, specifically — traces only, or traces and metrics and logs alike? Which transport — gRPC, HTTP, or both, since a product supporting only one may quietly exclude clients or environments that default to the other, exactly as Part 3 described? Which version of the OTLP specification, since the protocol itself continues to evolve and a sufficiently old implementation may lack fields or behaviors a current sender expects? And, most concretely of all, does anything get dropped or silently reinterpreted in the process — does a specific attribute type, a particular signal-specific field, or a piece of resource information survive the trip intact, or does it quietly vanish because the receiving system's own internal model has no equivalent place to put it?

A vendor or a colleague who can answer all four of these specifically and confidently is describing something genuinely verifiable, in the same sense Part 3's original standard-verification test was verifiable: you can, in principle, capture the actual traffic, compare it against the specification this post has described, and check for yourself. A vague, confident-sounding "yes, we support OTLP" that cannot be broken down along these four lines is not necessarily false, but it has not yet told you enough to actually trust it.

## Part 7: What Comes Tomorrow

Today's post covered the wire format that lets any two OTLP-speaking parties exchange bytes and, mechanically, understand the shape of what they are receiving. But shape alone does not guarantee the two parties agree on what the shapes mean — whether a field named `http.method` on one system's spans refers to the same concept, formatted the same way, as a field with a similar name on another. Tomorrow's post covers exactly that: semantic conventions, the shared dictionary that OpenTelemetry has spent years building and refining, and the genuinely difficult, still-ongoing work of getting an entire industry to agree on what to call things.

## Sources and Further Reading

- OpenTelemetry Project, ["OTLP Specification 1.10.0,"](https://opentelemetry.io/docs/specs/otlp/) opentelemetry.io — the primary source for the request/response `Export` model, the current stability status per signal, the gRPC and HTTP transports, and the default port numbers used throughout this post.
- OpenTelemetry Project, ["Design Goals for OpenTelemetry Wire Protocol,"](https://opentelemetry.io/docs/specs/otel/protocol/design-goals/) opentelemetry.io — background on why OTLP was designed the way it was.
- Google, ["Protocol Buffers Overview,"](https://developers.google.com/protocol-buffers/docs/overview) — background on the schema-defined binary encoding underlying OTLP, referenced in Part 2.

*Tomorrow: semantic conventions — the shared dictionary that decides whether "http.method" means the same thing on two different systems, and why building that dictionary has turned out to be genuinely hard.*
