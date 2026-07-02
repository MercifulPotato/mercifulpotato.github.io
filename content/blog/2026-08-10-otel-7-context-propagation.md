---
title: "Context Propagation: How a Request Keeps Its Identity"
date: 2026-08-10
author: mercifulpotato-team
summary: "A trace ID has to survive the trip across a real network for distributed tracing to work at all. This post dissects the W3C traceparent header byte by byte and explains exactly where propagation breaks in practice."
tags:
  - opentelemetry
  - observability
  - plain-english
  - distributed-tracing
series: "OpenTelemetry from the Ground Up"
---

## Part 1: The Passport Problem

Yesterday's post built a complete, three-span trace and read its story with confidence, because every span in that example already carried the correct shared trace identifier and the correct parent-child relationships. This post asks the question yesterday's post quietly skipped over: how does a span created inside one service, running on one machine, know to claim a span created inside an entirely different service, running on an entirely different machine, as its parent? The two services do not share memory. They do not share a process. The only way information passes between them at all is whatever gets sent over the network — typically an HTTP request, though the same problem exists for other kinds of network calls too.

Without a deliberate, agreed mechanism for carrying trace identity across that network boundary, every single service in a distributed system would have no choice but to start a brand new, orphaned trace every time it received a request, with no way of knowing it was actually one small part of a larger journey that began somewhere else. This would make yesterday's entire post's payoff — the ability to see one customer's request travel end to end through many services — simply impossible. Today's post is about the specific, standardized mechanism that prevents exactly that failure: context propagation.

## Part 2: Two Kinds of Context, Carefully Distinguished

It is worth separating two related but distinct ideas before going further, because conflating them causes real confusion. The first is in-process context: how "which span is currently active" follows a single program's own execution as it moves through function calls, asynchronous operations, and callbacks, all within one running process, on one machine. This is largely a mechanical concern handled by each programming language's own OpenTelemetry implementation, using whatever context-carrying facility that language provides — in .NET, for instance, this rides on the same underlying mechanism the platform's own Activity API already uses internally.

The second, and the one this post focuses on, is cross-process propagation: how that same identity survives the actual trip across a network, from one service to a completely separate one. In-process context is largely invisible plumbing a programmer rarely has to think about directly. Cross-process propagation is where the interesting, standardized, occasionally fragile mechanics live, and it is the subject of the rest of this post.

## Part 3: W3C Trace Context, Introduced

The specific standard OpenTelemetry uses by default for cross-process propagation is called W3C Trace Context, published by the World Wide Web Consortium — the same standards body responsible for foundational web technologies like HTML. This detail matters for a reason connected directly to Part 6 of this series two days ago: W3C Trace Context is not an OpenTelemetry-specific invention. It is a genuinely neutral, independently governed web standard, meaning it satisfies the same three-part test this series introduced for verifying a genuine standard — a public specification, multiple independent implementations, and neutral governance — and it predates and exists independently of any single tracing vendor or tool. This is precisely why trace context can, in principle, survive a hop through infrastructure that has nothing to do with OpenTelemetry at all: a browser, a load balancer, or a proxy server that has never heard of OpenTelemetry can still correctly recognize and forward a standard W3C Trace Context header, simply because enough of the industry, including major browser vendors, agreed to support the same neutral specification.

## Part 4: The traceparent Header, Dissected Byte by Byte

W3C Trace Context defines a specific HTTP header, named `traceparent`, and here is one real, well-formed example, directly connected to yesterday's checkout trace:

```
traceparent: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01
```

This single line of text is the entire mechanism. It consists of four fields, separated by hyphens, and we will walk through each one in order.

The first field, `00`, is the version. It is two hexadecimal characters — hexadecimal simply meaning a counting system using sixteen symbols, zero through nine and then the letters a through f, chosen because two hexadecimal characters can compactly represent any value a single byte of computer memory can hold. The specification currently defines only version zero; a value of `ff` is explicitly forbidden and reserved.

The second field, `4bf92f3577b34da6a3ce929d0e0e4736`, is the trace ID — exactly the same trace ID from yesterday's checkout example. It is thirty-two hexadecimal characters, representing sixteen bytes of underlying data, and per the specification, this exact value must remain unchanged across every single span belonging to the same trace, no matter how many services that trace eventually passes through. A trace ID of all zeros is explicitly forbidden, since real tracing tools use that specific value as a signal meaning "no valid trace present."

The third field, `00f067aa0ba902b7`, is the parent ID — sixteen hexadecimal characters representing eight bytes, and it plays a specific, precise role that is easy to state but easy to get subtly wrong in casual description: it identifies the span of the service that is about to make the outgoing call, so that whichever service receives this header knows which specific span, in the calling service, to record as its own new span's parent. Critically, and this is the detail that makes the whole mechanism actually work correctly hop after hop: every time a service is about to make an outgoing call, it must generate a fresh parent ID representing its own current span — the trace ID stays fixed for the entire journey, but the parent ID changes at every single hop, precisely because each hop needs the next service to correctly identify which specific span called it, not merely which overall trace it belongs to.

The fourth and final field, `01`, is trace flags — a single byte, two hexadecimal characters, of which the specification currently defines meaning for only the least significant bit: the sampled flag. A value of `01` signals that the calling service made the decision to record this particular trace in detail; a value of `00` signals that it did not. This flag exists specifically to let a sampling decision, once made — a topic covered fully in Part 16 — travel consistently through every subsequent service in the same journey, so that either the whole trace gets recorded end to end, or a downstream service at least knows the upstream decision and can choose to honor it, rather than each service in the chain independently and inconsistently deciding for itself whether this particular request is worth recording.

## Part 5: The Companion Header — tracestate

W3C Trace Context defines a second, related header, `tracestate`, whose purpose is different and worth distinguishing clearly. Where `traceparent` carries the mandatory, universally meaningful core identity — version, trace ID, parent ID, sampling flag — `tracestate` exists as a sidecar for additional, vendor-specific or system-specific information that does not need to be universally understood by every participant in the chain to still be useful to the specific systems that do understand it. A tool built by one particular vendor might use `tracestate` to carry an extra piece of internal bookkeeping it needs, without requiring every other tool in the chain, which has no reason to care about that vendor's internal bookkeeping, to understand or preserve it perfectly. The specification's own processing rule for this header is notably permissive by design: a participant that has no reason to modify it should simply pass it through unchanged, which is precisely the behavior expected of a pass-through component like a simple proxy that is not itself participating meaningfully in the trace.

## Part 6: Baggage — A Different Kind of Cargo

OpenTelemetry defines a separate mechanism, distinct from both `traceparent` and `tracestate`, called baggage, worth introducing here because it travels alongside trace context using a very similar cross-process propagation mechanism, but serves a genuinely different purpose. Baggage lets an application attach its own arbitrary key-value pairs — a tenant identifier, an experiment flag, a customer tier — that ride along with a request across every service boundary in the same journey, becoming available to any downstream service that wants to read them, entirely independent of tracing itself.

Baggage has two sharp edges worth naming plainly, precisely because they are easy to overlook until they cause a real problem. First, baggage travels as visible data inside request headers, meaning it adds real bytes to every single network hop in a journey, and a team that starts attaching increasingly large or numerous baggage values can measurably affect network overhead across an entire system without immediately realizing why. Second, and this trips up newcomers specifically, baggage is not automatically copied onto span attributes; a value present in baggage does not, by itself, show up as a searchable field on any given span unless a piece of code explicitly reads that baggage value and adds it as an attribute. Baggage and span attributes are two related but genuinely separate mechanisms, and conflating them is a common, avoidable source of confusion.

## Part 7: Injection and Extraction, Concretely

The actual mechanics of using this system in real code reduce to two operations, performed at every service boundary. When a service is about to make an outgoing call, it injects the current trace context into the outgoing request's headers — reading its own current span's identity, formatting it as a `traceparent` string exactly as described above, and attaching it to the request about to be sent. When a service receives an incoming call, it extracts trace context from the incoming request's headers — reading the `traceparent` string, if one is present, parsing its four fields, and using the trace ID and parent ID found there as the basis for the new span this service is about to create for its own portion of the work.

Here is a small, deliberately simplified illustration of the extraction and injection pattern, in pseudo-C#, at the boundary of a service in the middle of our checkout journey — receiving a call from the web frontend and, in turn, calling onward to the payment gateway:

```csharp
// Extraction: reading trace context from the incoming request
var incomingTraceparent = incomingRequest.Headers["traceparent"];
var parentContext = ParseTraceparent(incomingTraceparent);

// Start a new span, correctly nested under the caller's span
using var activity = activitySource.StartActivity(
    "charge-payment", ActivityKind.Server, parentContext);

// Injection: attaching updated trace context to the outgoing request
var outgoingTraceparent = BuildTraceparent(
    traceId: activity.TraceId,
    parentId: activity.SpanId,
    sampled: activity.Recorded);
outgoingRequest.Headers["traceparent"] = outgoingTraceparent;
```

In practice, for HTTP calls made through standard library facilities in a language with built-in OpenTelemetry instrumentation, this injection and extraction happens automatically once the relevant instrumentation library is registered, exactly as Part 11 of this series will show for ASP.NET Core and HttpClient specifically. The code above is shown explicitly, not because a working developer typically writes it by hand, but because seeing it written out plainly is the clearest way to understand what the automatic instrumentation is actually doing on your behalf, quietly, every time a request crosses a boundary.

## Part 8: Where Propagation Breaks

Context propagation depends, at every single hop, on the receiving component correctly reading, and the sending component correctly forwarding, the relevant headers. This dependency is precisely where real systems most often lose trace continuity, and it is worth naming the specific, common failure points plainly.

Message queues are the single most common source of broken propagation in practice, and the reason is structural rather than incidental: an HTTP request and its response happen within a single, continuous network exchange, but a message placed onto a queue may sit there, unread, for seconds, minutes, or considerably longer before some entirely separate consumer process picks it up — and unless that message's own headers or metadata were deliberately populated with trace context at the moment the message was published, and deliberately read back out by the consumer at the moment it is received, the connection between "the request that published this message" and "the process that eventually handled it" is simply lost, because no automatic, built-in mechanism bridges that gap the way ordinary HTTP instrumentation does. This requires the same explicit injection and extraction pattern shown in Part 7 above, just applied to a message's own headers rather than an HTTP request's, and it is exactly the scenario that produced the batch-job example in yesterday's discussion of span links, where a later consuming operation is genuinely related to, but cannot honestly claim to be the direct parent of, work that happened much earlier.

Batch jobs and scheduled work present a related problem: a nightly job that processes a thousand previously queued orders has no single incoming request to extract context from at all, since it was not triggered by any one customer's action in the first place; whatever trace it produces necessarily begins fresh, and any relationship it has to each individual order's own earlier checkout trace has to be expressed deliberately, typically using the span-link mechanism from yesterday's post, rather than through ordinary parent-child propagation.

And legacy services or intermediate infrastructure that predates modern tracing practice — an older internal proxy, a network appliance, a piece of middleware nobody has touched in years — can simply strip unrecognized headers as a matter of old, unrelated policy, with no awareness that doing so silently breaks trace continuity for everything passing through it. This failure mode is particularly insidious precisely because nothing about it produces an error or a warning; the request continues to work perfectly well from the application's own point of view, and the only symptom is a trace that mysteriously and silently splits into two disconnected pieces at exactly the point where the stripping occurred.

## Part 9: How to Spot the Lie — "End-to-End Tracing"

Vendor and product marketing in this space uses the phrase "end-to-end tracing" freely, and it is worth having a precise, skeptical test ready for exactly the reason Part 8 above should have made clear: if even one hop, anywhere in a request's actual journey, drops or fails to forward the `traceparent` header, "end-to-end" quietly and silently degrades into something considerably weaker — two, or more, separately disconnected traces that a dashboard may still display side by side, inviting a viewer to assume they are connected when the underlying data no longer actually says so.

The test this section offers is simple, concrete, and something any team can genuinely do for themselves rather than merely trusting a vendor's claim: pick one specific, real request, follow its actual trace ID by hand through every service it is supposed to have touched, and verify, concretely, that the trace ID you expect to see is the trace ID that actually shows up at each hop, all the way through. If it holds together cleanly, the "end-to-end" claim, at least for this specific path through the system, is genuinely earned. If it silently splits somewhere, you have just located precisely the boundary — an unmigrated legacy component, an unconfigured message queue, a misbehaving proxy — that most urgently needs the deliberate injection-and-extraction work described in Part 7, rather than assuming the tracing tool itself is broken or lying to you.

## Part 10: What Comes Tomorrow

Traces answer the question of where, specifically, a request's time went. Tomorrow's post turns to the second of OpenTelemetry's three original classic signals: metrics — the discipline of turning many individual measurements into compact, cheap-to-store, aggregated numbers over time, built from first principles with no formulas and no Greek letters, using worked, countable examples throughout, exactly as this series' own writing rules demand.

## Sources and Further Reading

- World Wide Web Consortium, ["Trace Context,"](https://www.w3.org/TR/trace-context/) W3C Recommendation — the primary specification for the `traceparent` and `tracestate` headers dissected in this post.
- OpenTelemetry Project, ["Context Propagation,"](https://opentelemetry.io/docs/concepts/context-propagation/) opentelemetry.io — the project's own conceptual overview of in-process context versus cross-process propagation.
- OpenTelemetry Project, ["Baggage,"](https://opentelemetry.io/docs/specs/otel/baggage/) opentelemetry.io — the formal specification for the baggage mechanism covered in Part 6 of this post.
- Uptrace, ["OpenTelemetry Context Propagation,"](https://uptrace.dev/get/opentelemetry-go/propagation) — a practical, code-level illustration of injection and extraction, cross-checked against the W3C specification's own processing rules for accuracy.

*Tomorrow: metrics from first principles — counters, gauges, and histograms, built entirely in plain English with countable, worked examples.*
