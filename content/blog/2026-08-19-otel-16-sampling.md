---
title: "Sampling: Keeping Some Data and Throwing the Rest Away"
date: 2026-08-19
author: mercifulpotato-team
summary: "Storing every single trace forever is rarely realistic. This post builds sampling from first principles using nothing but a table of one hundred requests — no formulas — and corrects a piece of terminology the industry itself frequently gets backward."
tags:
  - opentelemetry
  - observability
  - plain-english
  - sampling
series: "OpenTelemetry from the Ground Up"
---

## Part 1: The Firehose Problem, Quantified

Part 6 of this series closed with a brief acknowledgment of cost: a system handling even modest traffic can produce millions of spans a day, and storing every single one, forever, is rarely realistic once network transfer, processing, and storage costs are accounted for at genuine scale. Today's post gives that acknowledgment the full, dedicated treatment it deserves, building the entire idea of sampling from nothing, using only tables and counting — no formulas, exactly as this series' own writing rules require.

Here is the volume problem, worked out concretely rather than left abstract. Imagine a modest service handling one thousand requests per second — a real, unremarkable figure for a moderately busy production system — and imagine each request touches five services, each producing one span. That is five thousand spans every second, three hundred thousand every minute, eighteen million every hour, and well over four hundred million in a single day. Multiply that by even a modest average span size, and the resulting daily storage and transfer volume becomes a genuinely significant, ongoing cost — precisely the cost this series flagged in Part 1's discussion of what it costs to see, and precisely the cost sampling exists to control.

## Part 2: A Piece of Terminology Worth Getting Exactly Right

Before building the mechanics, this series is going to correct a piece of vocabulary that the industry itself, by OpenTelemetry's own admission, frequently gets backward — and getting it right from the outset will save real confusion in everything that follows.

OpenTelemetry's own documentation defines the terms precisely: a trace or span that is kept, processed, and exported is called sampled. A trace or span that is discarded, not processed, and not exported is called not sampled. This may sound almost like a trivial point of phrasing, but the project's own documentation calls out explicitly that people commonly say the opposite — describing discarded data as having been "sampled out," as though "sampled" meant "removed." It does not. Sampled means kept. This series will use the term exactly as the project defines it for the remainder of this post.

## Part 3: Representativeness — the Idea That Makes This Honest

Sampling is not simply "throwing away most of your data and hoping for the best." It rests on a specific, defensible idea OpenTelemetry's own documentation calls representativeness: the principle that a smaller group, chosen correctly, can accurately stand in for a much larger group. This is not a new idea invented for observability; it is the same underlying principle behind political polling, quality-control sampling in manufacturing, and most of applied statistics — a small, correctly chosen sample can tell you, with real confidence, what the much larger population looks like, without needing to examine every single member of that population individually.

A genuinely useful, concrete fact worth stating plainly: the more traffic a system generates, the smaller a percentage you typically need to keep to still get an accurate picture. For a very high-volume system, a sampling rate of one percent or even lower can very accurately represent the full population, precisely because a large enough number of samples, even a small percentage of an enormous total, is still a large absolute number.

## Part 4: Head Sampling — Deciding at Birth

Head sampling is a sampling technique that makes its decision as early as possible, at or near the moment a trace begins, without inspecting anything about how the trace eventually turns out. The most common form, formally called Consistent Probability Sampling, works from the trace ID itself, introduced back in Part 6, combined with a desired keep rate: a mathematical property of the trace ID lets every service touched by that same trace ID arrive at the identical sampling decision, independently, without needing to communicate with each other, ensuring a trace is either kept whole or dropped whole — never partially kept with some of its spans missing.

Here is that decision, worked out with a table of one hundred requests, no formulas involved: imagine a keep rate of ten percent, applied to one hundred incoming requests. Exactly ten of them are marked to keep; the other ninety are marked to drop. Which specific ten are chosen is effectively a lottery, seeded by each request's own trace ID, but the mechanism guarantees that roughly one in ten, consistently, gets kept across a large enough number of requests.

| Outcome | Count out of 100 |
|---|---|
| Kept (sampled) | 10 |
| Dropped (not sampled) | 90 |

Head sampling's real strength, and the reason it remains genuinely popular despite the limitation covered next, is that it is easy to understand, easy to configure, computationally efficient, and can be applied at any point in the pipeline — inside the application's own SDK, or later, inside a Collector of the kind covered in Part 13. Its real, unavoidable weakness is built into its own name: because the decision happens at the head, at birth, before anything about the request's eventual outcome is known, it has no way of preferentially keeping the traces that later turn out to matter most — the ones that failed, or the ones that ran unusually slowly.

## Part 5: Tail Sampling — Deciding After the Story Is Known

Tail sampling makes its decision after the fact, once all or most of a trace's spans have already completed, letting the decision be based on the trace's actual, observed outcome rather than a blind lottery at birth. This genuinely valuable capability comes at a real architectural cost worth stating plainly: because the decision requires seeing the whole trace, something in the pipeline — typically a Collector configured with a tail-sampling processor, of the kind introduced conceptually in Part 13 — must buffer every span belonging to a trace, in memory, until that trace is judged complete and a decision can finally be made.

A realistic, common tail-sampling policy might read: always keep any trace that contains an error, always keep any trace whose overall duration exceeds some threshold considered unacceptably slow, and keep only a small, uniform baseline percentage of everything else — the ordinary, healthy, fast traces that make up the overwhelming bulk of real traffic. This exact policy is precisely why our own slow, failing checkout trace from Part 6 — the one that spent nearly its entire eight seconds inside a single payment-gateway call — would be a strong, near-certain candidate for being kept under a tail-sampling policy built around exactly this pattern, specifically because it both ran unacceptably slowly and involved a retry event worth investigating, even if it had been drawn into the ninety percent a pure head-sampling lottery would otherwise have discarded.

## Part 6: What Sampling Does to Your Numbers

Sampling has a genuine, honest consequence for how the kept data should be interpreted, and this series will not gloss over it. If a system keeps one trace in every ten, then each kept trace, in a meaningful sense, stands in for roughly ten traces' worth of the underlying population — a fact a properly built backend accounts for by adjusting displayed counts upward to compensate, rather than simply reporting the raw count of what happened to survive sampling.

Here is that adjustment worked out with counting, not formulas. Imagine, out of our earlier hundred-request example, that two of the ten kept traces happened to represent failed checkouts. A backend that simply reports "two failures" is badly understating reality, because those two survivors are standing in for the ninety that were never inspected at all. A backend that correctly adjusts for the one-in-ten keep rate would instead report an estimated twenty failures across the full, unsampled population — multiplying the raw kept-and-observed count by the same factor the keep rate implies. This single adjustment is precisely why the honest question to ask about any number derived from sampled data is not merely "what does the dashboard show," but "was this number adjusted for the sampling rate that produced it, and if so, how."

## Part 7: Signals Differ

It is worth being precise, in closing out the mechanics of this post, about a distinction this series has quietly assumed throughout but never stated outright: sampling, in the specific sense built up across this entire post, applies principally to traces. Metrics, covered in Part 8, are typically pre-aggregated at the point of collection — a counter or a histogram bucket already represents a summary across many individual events by the time it is ever recorded, which is a fundamentally different kind of data reduction than selectively discarding whole traces, and metrics are not normally described as being "sampled" in the sense this post has built. Logs, covered in Part 9, sit somewhere in between: rather than the whole-trace-or-nothing lottery this post describes for traces, log volume is more commonly managed through severity-based retention of exactly the kind Part 9 described — keeping Error-level output for a long time while discarding routine Debug-level output quickly or not at all.

## Part 8: Choosing a Policy, and Its Honest Bill

For a small team building a sensible sampling policy from scratch, the shape this post's own material suggests is genuinely simple to state, even if implementing it well takes real care: apply tail sampling to always keep traces containing errors and traces that ran unacceptably slowly, apply a low, uniform head-sampling baseline — perhaps one or two percent — to everything else, and, per OpenTelemetry's own documentation, seriously consider routing the deliberately dropped, "just in case" volume to genuinely cheap, low-cost storage rather than discarding it outright, preserving at least the option of digging deeper later if an unanticipated need arises.

It is worth being honest about the bill this arrangement carries, exactly in the spirit of the project's own documentation, which names the real costs plainly rather than presenting sampling as a free win: the direct compute cost of running a tail-sampling proxy that must buffer and evaluate every trace before deciding; the ongoing engineering cost of maintaining sensible, correctly tuned sampling rules as an application's own traffic patterns and failure modes inevitably change over time; and the real opportunity cost of an imperfectly designed policy quietly discarding exactly the rare, unusual trace that would have explained tomorrow's hardest incident. Sampling is a genuinely effective way to control cost. It is not, in itself, free.

## Part 9: How to Spot the Lie — Sampled Data Presented as a Census

A specific, easy-to-miss deception deserves this post's own skeptical test: any number derived from sampled data, presented without any acknowledgment that it was sampled at all, quietly inviting the reader to treat it as a complete, exhaustive count of everything that happened — a census — rather than what it actually is, an estimate built from a smaller representative group.

"Only 0.01% of requests failed last month" is a genuinely different claim depending on how it was actually measured. Measured against every single request, it is a real, verifiable fact. Measured against a one-percent head-sampled subset, with no adjustment applied for that rate, it could be dramatically understating the true failure rate — precisely because, as Part 4 explained, a blind, birth-time lottery has no particular reason to have happened to catch the failures at all. There is a second, related and genuinely important distortion worth naming here, cutting in the opposite direction: a tail-sampling policy of exactly the kind Part 5 recommended, one that deliberately keeps every failing trace, will make a trace-viewing tool's own display look considerably worse — proportionally full of failures and slow requests — than the system's true, overall health actually is, precisely because the policy was designed to preferentially retain exactly the interesting, unhealthy cases rather than a neutral cross-section of everything. Neither of these is a lie in the sense of a deliberate falsehood, but both are genuinely easy to misread if the sampling policy behind a number is left unstated. The two questions worth asking of any figure derived from sampled telemetry, every time, are simply these: what was the sampling decision, and when, precisely, was it made — at the trace's birth, or after its outcome was already known?

## Part 10: What Comes Tomorrow

Today's post covered the deliberate discipline of keeping less. Tomorrow's post turns to the question of where whatever a team does keep actually goes: backends — the open-source, self-hosted options like Jaeger and Prometheus, the commercial vendor landscape, and this publication's own honest, proportionate answer for a small, free, static site with no backend beyond a free-tier analytics endpoint.

## Sources and Further Reading

- OpenTelemetry Project, ["Sampling,"](https://opentelemetry.io/docs/concepts/sampling/) opentelemetry.io — the primary source for this post's terminology corrections (sampled versus not sampled), the representativeness principle, and the when-to-sample and when-not-to-sample guidance in Parts 2, 3, and 8.
- OpenTelemetry Project, ["TraceState: Probability Sampling,"](https://opentelemetry.io/docs/specs/otel/trace/tracestate-probability-sampling/) opentelemetry.io — the formal specification for Consistent Probability Sampling referenced in Part 4.
- OpenTelemetry Collector Documentation, tail-sampling processor reference — background for the buffering and policy mechanics described in Part 5.

*Tomorrow: backends — where telemetry goes to actually be useful, argued fairly across open-source and commercial options alike.*
