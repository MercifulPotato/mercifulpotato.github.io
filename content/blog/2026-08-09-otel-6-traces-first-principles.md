---
title: "Traces from First Principles: Spans, Attributes, Events, and Links"
date: 2026-08-09
author: mercifulpotato-team
summary: "A distributed trace is the biography of a single request. This post builds the idea of a span from nothing, then assembles spans into traces, with a complete worked JSON example you can read line by line."
tags:
  - opentelemetry
  - observability
  - plain-english
  - distributed-tracing
series: "OpenTelemetry from the Ground Up"
---

## Part 1: From History to Anatomy

The first five posts in this series established why observability matters, where its tools came from, who built the standard this series is named after, and precisely what that standard consists of. Today, the series shifts from history and architecture into substance. We begin building OpenTelemetry's actual data model, one signal at a time, starting with tracing — the signal Part 4 of this series noted has reached the most mature status of any part of the OpenTelemetry specification, fully stable and covered by long-term support.

This post will construct a checkout trace, piece by piece, that we will reuse — propagated across a network in Part 7, exported to a console in Part 12, transformed by a Collector in Part 13, serialized over the wire in Part 14, and sampled in Part 16. Read this post carefully; the vocabulary introduced here is load-bearing for the remainder of the series.

## Part 2: The Span, Built From Nothing

Return, one more time, to the restaurant kitchen analogy established in Part 1. A single order moves through several stations: it is taken, cooked, plated, and expedited. Imagine that each station, as it begins and finishes its part of the work, writes down four things on a shared ticket: what it was doing, exactly when it started, exactly when it finished, and any facts worth noting along the way. That single station's written record — one bounded, timed unit of work — is, in essence, what OpenTelemetry calls a span.

More precisely, and now moving into the actual terminology this series will use consistently from here forward: a span represents one named operation, with a definite start time and end time, performed by one specific piece of code, in one specific service. "Validate the customer's shipping address" is a span. "Query the inventory database for stock levels" is a span. "Charge the customer's card through the payment gateway" is a span. Each is bounded — it has a clear beginning and a clear end — and each has a name that a human reading it later should be able to understand without additional context.

## Part 3: Assembling Spans Into a Trace

A single span, on its own, tells you about one operation. The genuinely valuable idea, the one that makes distributed tracing worth an entire signal of its own, is what happens when many spans, produced by many different pieces of a system, get tied together into a single structure describing one customer's entire journey. That structure is called a trace.

The mechanism that ties spans together is a shared identifier: every span belonging to the same overall journey carries the same trace identifier, commonly called a trace ID. In addition, each individual span carries its own unique span identifier, and — with one specific exception we come to in a moment — a reference to the identifier of whichever span called it into existence, called its parent span ID. This parent-child relationship is precisely what lets a trace be reconstructed, after the fact, as a tree: one root operation at the top, with all the work it triggered branching out beneath it, and all of that work's own further triggered work branching out beneath that, and so on, however deep the actual call chain happened to run.

The one span in any given trace that has no parent — the operation that started the whole journey, typically the very first request a customer's action generated — is called the root span, and it is identifiable specifically by the absence of a parent span ID field.

## Part 4: A Complete Worked Trace, Read Line by Line

Here is a complete, worked example: three spans belonging to a single trace, formatted in a simplified, human-readable JSON style that mirrors the format OpenTelemetry's own official documentation uses to introduce this exact concept — a deliberately simplified illustration, not the literal wire format a real system would send over a network, which we will see precisely in Part 14 when we cover the OpenTelemetry Protocol directly.

```json
{
  "name": "checkout",
  "context": {
    "trace_id": "4bf92f3577b34da6a3ce929d0e0e4736",
    "span_id": "00f067aa0ba902b7"
  },
  "parent_id": null,
  "start_time": "2026-08-09T03:47:04.881000Z",
  "end_time": "2026-08-09T03:47:12.881000Z",
  "attributes": {
    "http.route": "/checkout",
    "order.id": "ORD-88213"
  },
  "events": []
}
```

```json
{
  "name": "validate-inventory",
  "context": {
    "trace_id": "4bf92f3577b34da6a3ce929d0e0e4736",
    "span_id": "b7ad6b7169203331"
  },
  "parent_id": "00f067aa0ba902b7",
  "start_time": "2026-08-09T03:47:04.912000Z",
  "end_time": "2026-08-09T03:47:05.100000Z",
  "attributes": {
    "db.system": "postgresql",
    "order.id": "ORD-88213"
  },
  "events": []
}
```

```json
{
  "name": "charge-payment",
  "context": {
    "trace_id": "4bf92f3577b34da6a3ce929d0e0e4736",
    "span_id": "9c2748f5f0c8a3d1"
  },
  "parent_id": "00f067aa0ba902b7",
  "start_time": "2026-08-09T03:47:05.104000Z",
  "end_time": "2026-08-09T03:47:12.870000Z",
  "attributes": {
    "payment.gateway": "external-processor",
    "order.id": "ORD-88213"
  },
  "events": [
    {
      "name": "gateway.timeout.retry",
      "timestamp": "2026-08-09T03:47:09.500000Z",
      "attributes": {
        "retry.attempt": 1
      }
    }
  ]
}
```

Now read the story these three JSON blocks tell, line by line, exactly the way a human debugging this in production would. All three spans share the same `trace_id`, so we know they belong to the same journey — the checkout for order ORD-88213. The first span, named `checkout`, has a `parent_id` of `null`, so it is the root: this is where the whole journey began, and it ran from 03:47:04.881 to 03:47:12.881, a total of eight full seconds. The second span, `validate-inventory`, has a `parent_id` matching the root span's `span_id`, so we know the checkout operation called this one; it took a brisk 188 milliseconds. The third span, `charge-payment`, is also a direct child of the root, and it ran from 03:47:05.104 to 03:47:12.870 — nearly seven and three-quarter seconds, which is almost the entirety of the parent's eight-second total duration. We have just located, with certainty and without guessing, exactly where this slow checkout's time actually went: not in inventory validation, but almost entirely inside the payment charge.

Notice, too, the one event attached to the `charge-payment` span: a timestamped note, at 03:47:09.500, recording that a retry occurred after a gateway timeout. That single retry attempt accounts for a meaningful chunk of the delay, and we would never have known it happened at all from the span's start and end times alone. This is exactly the distinction the next section makes precise.

## Part 5: Attributes Versus Events — a Distinction Worth Getting Right

A span's attributes, as seen throughout the example above, are key-value facts that describe the operation as a whole, true for its entire duration: which database system was used, which order this concerns, which payment gateway was involved. Attributes answer "what was this operation, in general?"

A span's events, by contrast, are timestamped notes recording something that happened at one specific moment during the operation's lifetime, not something true of the whole span. The retry in our example is a textbook case: it did not describe the entire `charge-payment` operation from start to finish; it described one specific thing that occurred partway through. A useful, memorable rule for choosing between the two: if a fact is true from the operation's start to its end, it belongs as an attribute; if a fact describes something that happened at one particular instant within the operation, it belongs as an event.

## Part 6: Span Status and Span Kind

Two further fields, not shown in the simplified example above but present in any real OpenTelemetry span, deserve introduction here because they recur throughout the rest of this series. Span status records whether the operation the span represents succeeded or failed — a simple but essential piece of information for quickly separating the traces worth investigating from the ones that completed normally. Span kind records the operation's role in a larger request: whether this span represents a server handling an incoming request, a client making an outgoing call, an internal operation with no direct network communication, or one of two roles specific to message-queue-style systems, a producer sending a message or a consumer receiving one. Span kind matters because a tracing backend, covered fully in Part 17, uses it to correctly draw the relationships between services — recognizing, for instance, that a client span in one service and a server span in another, both carrying matching identifiers, represent the two sides of the same network call.

## Part 7: Span Links — When Parenthood Does Not Fit

The parent-child model covered so far assumes a clean, tree-shaped structure: one operation calls another, which calls another. Real systems occasionally produce a shape that does not fit neatly into a single tree, and OpenTelemetry has a specific mechanism, called a span link, for exactly this situation.

Consider a batch job that processes a hundred previously queued orders in one pass. That batch operation cannot honestly claim to be the sole parent of each of those hundred orders' own original checkout traces — each order's checkout happened independently, at its own earlier time, as part of its own separate trace. What the batch job can honestly say is that it is related to each of those hundred earlier traces, without claiming to be their parent. A span link expresses precisely that: a reference from one span to another span, potentially in an entirely different trace, establishing a relationship without asserting a parent-child hierarchy that does not actually exist. The same need arises in the "scatter-gather" pattern, where one operation fans out work to several others and then combines their results — the combining operation is related to each of the several it gathered from, but calling itself their single parent would misrepresent what actually happened.

## Part 8: The Waterfall — How a Human Actually Reads a Trace

The raw JSON in Part 4, while precise, is not how a person investigating a real incident typically looks at a trace. Nearly every tracing backend renders a trace as what is commonly called a waterfall: a horizontal bar for each span, positioned according to when it started, sized according to how long it ran, and indented according to its depth in the parent-child tree. Here is our checkout trace rendered as a simple text waterfall, which, while cruder than a real backend's polished visual rendering, conveys exactly the same underlying information:

```
checkout                    [========================================] 8000ms
  validate-inventory        [==]                                          188ms
  charge-payment              [======================================]  7766ms
    (retry event at +4396ms)         ^
```

The value of this rendering should be immediately visible: the eye goes straight to the longest bar, `charge-payment`, and immediately understands that this is where the investigation should focus, without needing to read a single number. This is precisely the payoff distributed tracing exists to deliver, and it is worth being explicit that this payoff depends entirely on the parent-child structure and precise timestamps this entire post has been building up to.

## Part 9: What Traces Cost

It would be inconsistent with this series' stated commitments to leave today's post without a brief, honest acknowledgment of cost, even though the full treatment waits until Part 18. Every span shown in this post had to be created, timestamped, populated with attributes, and eventually transmitted somewhere. In a system handling a modest number of requests per second, with each request touching a handful of services, the resulting volume of spans can already run into the millions per day; in a genuinely large system, into the millions per minute. Generating, transmitting, and storing every single span, for every single request, forever, is rarely realistic at scale — which is precisely the problem Part 16's treatment of sampling exists to address, deliberately keeping only a representative fraction of all traces while still preserving enough signal to catch the problems that matter.

## Part 10: How to Spot the Lie — The Demo Trace

Vendor demonstrations and conference talks in this field share a recurring, almost universal pattern: a clean, five-or-six-span trace, every span present and accounted for, every parent-child relationship intact, rendered as a beautiful waterfall with the slow span helpfully highlighted in red. This is a genuinely useful teaching tool — this very post has just used one — and it is also, reliably, not what a trace looks like on a bad night in a real production system.

Real production traces, at genuine scale, routinely have gaps: a service somewhere in the chain was never instrumented, so its portion of the journey simply does not appear, leaving a mysterious span of unaccounted-for time between a parent span's start and the next child span's appearance. Real traces sometimes have broken parent links, where a header got stripped somewhere along the way — exactly the failure mode Part 7 of this series, tomorrow, examines in full technical detail. Real traces can suffer from clock skew, where two different servers' system clocks disagree by a few milliseconds, making a child span appear to have started before its own parent, which is logically impossible but mechanically common.

The test this section offers, then, is simple and directly actionable: when evaluating a tracing product, whether in a sales demonstration or your own team's proof of concept, ask specifically to see a trace with a gap in it, or a trace where something clearly went wrong in collection rather than only in the application being observed. A vendor or a colleague who can readily produce one, and explain calmly what happened and why, is demonstrating real, lived experience with the tool in production conditions. A demonstration that only ever shows the clean, complete, five-span version is not necessarily dishonest, but it is not yet showing you the part of the picture that will matter most the first time you actually need this tool at three in the morning.

## Part 11: What Comes Tomorrow

Today's post assumed, without fully explaining, that a trace ID and a parent span ID can somehow survive the trip from one service to another, across a real network, so that a span created in a second service can correctly claim the first service's span as its parent. Tomorrow's post explains exactly how that survival happens: context propagation, the W3C Trace Context standard, the `traceparent` header dissected byte by byte, and the specific, common ways this mechanism breaks in practice — including message queues, the exact scenario referenced in Part 7's discussion of span links today.

## Sources and Further Reading

- OpenTelemetry Project, ["Traces,"](https://opentelemetry.io/docs/concepts/signals/traces/) opentelemetry.io — the primary source for the span JSON structure, field names, and the "hello / hello-greetings / hello-salutations" illustrative example this post's own worked example is directly modeled on.
- OpenTelemetry Project, ["Trace,"](https://opentelemetry.io/docs/specs/otel/trace/) opentelemetry.io — the formal specification for span attributes, events, links, status, and kind referenced throughout this post.
- OpenTelemetry Project, ["Sampling,"](https://opentelemetry.io/docs/concepts/sampling/) opentelemetry.io — background for the cost discussion in Part 9, covered fully in Part 16 of this series.

*Tomorrow: context propagation — how a trace ID and parent span ID actually survive the trip across a real network, and the W3C standard that makes it possible.*
