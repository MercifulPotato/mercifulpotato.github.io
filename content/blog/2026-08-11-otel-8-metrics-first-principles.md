---
title: "Metrics from First Principles: Counters, Gauges, and Histograms"
date: 2026-08-11
author: mercifulpotato-team
summary: "Metrics turn millions of individual measurements into compact, cheap numbers you can keep forever. This post builds the entire model — counters, gauges, histograms, and the cardinality trap — in plain English with countable, worked examples."
tags:
  - opentelemetry
  - observability
  - plain-english
  - metrics
series: "OpenTelemetry from the Ground Up"
---

## Part 1: Why Not Just Trace Everything?

Part 6 of this series built a complete, worked trace: three spans, each with precise start and end times, telling us exactly where one customer's slow checkout spent its time. A reasonable question follows immediately: if traces are this detailed and this useful, why does OpenTelemetry bother with a second, entirely separate signal for measurements at all? Why not simply keep every trace, forever, and answer every question from that?

The honest answer, foreshadowed in Part 6's closing discussion of cost, is volume. A system handling even a modest amount of traffic produces an enormous number of individual events, and preserving full, detailed trace records of every single one, forever, is rarely realistic once you account for storage, network transfer, and processing cost at genuine scale — a problem Part 16 of this series addresses directly through sampling. Metrics exist to answer a different, complementary kind of question: not "what exactly happened during this one specific request," but "how much, how often, and is the overall picture getting better or worse over time." A metric is, by design, a deliberate summary — cheap enough to keep at very high volume and for a very long time, at the cost of no longer being about any one specific request.

This post's own brief, and this series' commitment to plain English throughout, comes under its sharpest test today. Metrics, more than any other signal in this series, tend to get explained using mathematical notation and statistical jargon. This post will build the entire model — the different kinds of instruments, and the genuinely tricky idea of a histogram — using nothing but plain language and countable, worked tables. If a concept in this post cannot be explained by counting things in a table, it will be left unexplained rather than reduced to a formula.

## Part 2: The Odometer, the Fuel Gauge, and the Queue

A useful mental picture, borrowed directly from how OpenTelemetry's own documentation introduces this exact idea, is the dashboard of a car. An odometer only ever goes up; it accumulates total distance traveled and never resets downward on its own. A fuel gauge, by contrast, tells you a current reading, taken at the moment you look at it, which can rise or fall as fuel is used or added. And a third familiar picture, closer to software: the number of people currently waiting in a line at a coffee shop, which rises as new customers arrive and falls as orders are completed — a quantity that goes both up and down over time, unlike the odometer.

These three everyday pictures map directly onto the three fundamental families of measurement OpenTelemetry defines, and understanding them through these pictures first, before the formal names, makes the formal names easy to remember rather than arbitrary.

## Part 3: The Instrument Families, One at a Time

OpenTelemetry calls the odometer picture a Counter: a value that only ever accumulates upward, never resets downward, over the life of the thing measuring it. A running total of completed checkouts, or total bytes sent over a network connection, is a natural Counter. In C#, creating one looks like this:

```csharp
var checkoutsCompleted = meter.CreateCounter<long>(
    "checkout.completed", description: "Total completed checkouts");

checkoutsCompleted.Add(1, new("payment.method", "credit-card"));
```

OpenTelemetry calls the coffee-shop-line picture an UpDownCounter: a value that accumulates over time but, unlike a Counter, can also decrease. A queue length is the textbook example — it grows as work arrives and shrinks as work is completed — and this series' own checkout example offers a natural one too: the number of in-progress checkout attempts at any given moment, rising as new customers begin checking out and falling as each one finishes, successfully or not.

OpenTelemetry calls the fuel-gauge picture a Gauge: a measurement of a current value, read at the moment it is captured, rather than something that accumulates. Current memory usage, current temperature of a server room, or the current number of open database connections are natural Gauges — each one is a snapshot, not a running total.

And OpenTelemetry defines one further, genuinely distinct family: a Histogram, described in the project's own documentation as a client-side aggregation of many individual values, most typically request durations — and understanding exactly what that means, and why it earns an entire family of its own rather than being just another kind of Gauge, is worth the dedicated, careful treatment the next several parts of this post give it.

A brief, honest technical note before moving on: each of the first three families — Counter, UpDownCounter, and Gauge — also has what OpenTelemetry calls an asynchronous variant, used specifically in situations where your own code cannot conveniently report every individual change as it happens, but can instead be asked, whenever an export is about to occur, "what is the current total right now?" A good example is a queue length tracked by a separate system your own code does not directly control incrementing and decrementing; rather than trying to intercept every single change, an asynchronous UpDownCounter simply reports the current total whenever OpenTelemetry asks for it. The choice between the ordinary and asynchronous variant of a given family is a matter of how your own code happens to have access to the underlying value, not a difference in what the measurement fundamentally represents.

The following table summarizes all seven official instrument kinds, alongside the everyday picture and a checkout-relevant example for each.

| Instrument | Everyday picture | Can it go down? | Checkout-relevant example |
|---|---|---|---|
| Counter | Odometer | No | Total checkouts completed |
| Asynchronous Counter | Odometer, read on demand | No | Total bytes read, reported by an external library |
| UpDownCounter | Coffee shop line | Yes | In-progress checkout attempts right now |
| Asynchronous UpDownCounter | Coffee shop line, read on demand | Yes | Current queue depth, reported by a queue system |
| Gauge | Fuel gauge | Yes (it's a snapshot) | Current memory usage |
| Asynchronous Gauge | Fuel gauge, read on demand | Yes (it's a snapshot) | Current CPU temperature, reported by hardware sensors |
| Histogram | A tally sheet of many timed events | N/A — see Part 4 | Checkout duration, across every checkout attempt |

## Part 4: Histograms, Explained Entirely by Counting

This is the part of the post where this series' "no formulas, no Greek letters" rule earns its keep. A histogram exists to answer a question none of the other six instruments can answer well on their own: not "what is the total," and not "what is the current value," but "across many individual measurements, what does the overall shape of those measurements actually look like?"

Here is the entirely plain-English way to understand a histogram: imagine you have just recorded the checkout duration, in seconds, for one thousand separate checkout attempts over the last hour. Instead of keeping all one thousand individual numbers forever, a histogram sorts them into a small number of labeled bins — buckets, in the terminology this field actually uses — each one covering a range of durations, and simply counts how many measurements landed in each bucket. Here is exactly that, worked out with a concrete, invented but realistic set of one thousand checkout durations:

| Duration bucket | Number of checkouts in this bucket |
|---|---|
| Under 1 second | 640 |
| 1 to 2 seconds | 210 |
| 2 to 4 seconds | 90 |
| 4 to 8 seconds | 40 |
| 8 seconds or more | 20 |
| **Total** | **1,000** |

This table, on its own, with no further mathematics, already tells a genuinely useful story: the large majority of checkouts — 640 out of 1,000 — complete in under a second, which is good. But 60 checkouts, six percent of the total, took four seconds or longer, and 20 of those took eight seconds or more — squarely in the range of yesterday's own slow checkout example from Part 6. A histogram is simply this table, kept efficiently and cheaply, refreshed continuously as new measurements arrive, without ever needing to store all one thousand — or, at real scale, millions — of the individual raw numbers that produced it.

## Part 5: Percentiles, Without a Formula

The term "p95," or "95th percentile," appears constantly in discussions of system performance, and it is worth defining it here using nothing but the counting table already built above, because the concept is genuinely simple once freed from its usual mathematical dressing. The 95th percentile of checkout duration is simply this: the specific duration value below which 95 out of every 100 checkouts fall.

Using our table of 1,000 checkouts: 640 finished under 1 second, and 640 plus 210 is 850, so 850 out of 1,000 — 85 percent — finished under 2 seconds. Adding the next bucket, 850 plus 90 is 940, so 94 percent finished under 4 seconds. We need slightly more than that to reach 95 percent, so the 95th percentile duration for this particular hour of checkouts falls somewhere just past the 4-second mark, inside the 4-to-8-second bucket. No formula was needed to arrive at that: only counting rows in a table, in order, until we passed the 95-out-of-100 mark. This is precisely why "p95" matters more than a simple average for understanding user experience: it tells you specifically about the slower end of the distribution — the requests where real customers actually had a noticeably bad experience — rather than blending everything into one single number the way an average does.

## Part 6: Why Averages Lie

Here is a worked, concrete illustration of exactly how an average can look reassuring while genuinely hiding real user pain. Imagine nineteen checkouts complete in a crisp half a second each, and one twentieth checkout takes nineteen and a half seconds due to a stuck payment gateway. The average across all twenty checkouts works out to exactly one full second — a number that, reported on its own on a dashboard, looks entirely healthy. But not one single customer, out of the twenty, actually experienced anything close to "one second." Nineteen of them had a fast, pleasant half-second checkout, and one of them sat through a nearly twenty-second ordeal that likely felt, to that one customer, like the checkout had simply broken. The average is mathematically correct and simultaneously almost useless for understanding what actually happened, precisely because it blends a large number of good experiences with a small number of bad ones into a single figure that resembles neither.

A histogram, and the percentile figures derived from it as shown in Part 5, avoid exactly this trap, because they preserve the shape of the distribution rather than collapsing it into one blended number. This is the single most important, concrete reason the discipline of metrics moved beyond simple averages and toward histograms as a first-class, dedicated instrument family in its own right.

## Part 7: Temporality — Two Honest Ways to Report the Same Counter

One further concept deserves a careful, plain-English explanation, because it recurs when metrics data actually gets exported and stored: temporality, meaning whether a reported number represents a total accumulated since some starting point, or only the amount that changed since the last time it was reported.

Picture a household water meter. One honest way to report water usage is to read the meter's running total directly: "the meter currently reads 4,812 gallons," which keeps climbing forever and never resets. OpenTelemetry calls this cumulative temporality. A different, equally honest way to report the exact same underlying reality is to report only the change since the last reading: "since the last time anyone checked, 12 more gallons were used." OpenTelemetry calls this delta temporality. Both descriptions are accurate; they simply answer slightly different practical questions, and different backends genuinely prefer one over the other for reasons connected to how they store and later query the data. The same underlying Counter can, depending on how the exporter is configured, be reported either way, and this is a configuration decision rather than a difference in what is actually being measured.

## Part 8: Dimensions and the Cardinality Bomb

Every metric instrument introduced so far can be enriched with attributes, in exactly the same key-value sense Part 6 introduced for spans — for instance, our `checkout.completed` counter carrying a `payment.method` attribute, letting you later ask "how many checkouts completed via credit card specifically" rather than only "how many checkouts completed in total."

This is genuinely valuable, and it is also the single most common way metrics quietly become expensive without anyone intending it to happen, so it deserves careful, worked attention here, foreshadowing the fuller treatment in Part 18. Each unique combination of attribute values that ever gets recorded against a given metric name creates what the field calls a separate time series — its own independently tracked, independently stored sequence of numbers over time. The number of distinct possible combinations is called cardinality, and it multiplies, rather than adds, across attributes. Here is that multiplication worked out concretely:

| Attribute added | Distinct values it can take | Running total of possible combinations |
|---|---|---|
| (none) | — | 1 |
| `payment.method` | 4 (credit-card, debit-card, wallet, gift-card) | 4 |
| `+ region` | 6 regions | 24 |
| `+ device.type` | 3 (desktop, mobile, tablet) | 72 |

Three attributes, each individually modest and reasonable-sounding, have combined to produce 72 separate time series for a single metric that started out as one simple running total. This is already a meaningful multiplier, and it is still a comparatively tame example: the genuinely dangerous version of this mistake occurs when a team, often without fully realizing the consequence, attaches an attribute with a very large number of distinct possible values — a customer ID, a specific order ID, a raw IP address — directly onto a metric. A single such attribute, applied to a system with even a modest number of distinct customers, can turn one metric into many thousands or millions of separate time series overnight, with real, direct consequences for storage cost and query performance at whatever backend receives the data. The rule this table exists to teach, stated plainly: attributes on metrics are for things with a small, bounded, genuinely enumerable set of possible values — categories, not identities.

## Part 9: How to Spot the Lie — Averages, Uptime Nines, and Vanity Metrics

This series' recurring statistics-adjacent skepticism arrives at metrics today in full force, because this signal is uniquely well suited to producing numbers that are technically accurate and practically misleading, exactly as Part 6 demonstrated.

The average-hides-the-tail trap from Part 6 is the first, most direct instance, and the fix is simple and now fully in hand: whenever someone reports a single average duration figure as evidence a system is healthy, ask specifically for the distribution, or at minimum the 95th or 99th percentile, before accepting the average's implied conclusion.

A second, closely related trap is uptime expressed as a percentage of nines, without the reader doing the arithmetic to see what that percentage actually means in lived time. Here is that arithmetic, worked plainly, assuming a standard thirty-day month:

| Claimed uptime | Allowed downtime per month |
|---|---|
| 99% | About 7 hours, 18 minutes |
| 99.9% | About 43 minutes |
| 99.99% | About 4 minutes, 20 seconds |
| 99.999% | About 26 seconds |

A jump from "99%" to "99.9%" sounds like a small, almost rounding-error improvement in the number itself, but the table shows it is actually the difference between over seven hours of downtime a month and under three quarters of an hour — a genuinely enormous practical difference hidden behind what looks like a tiny change in the percentage. Whenever an uptime figure is presented, converting it to this kind of concrete monthly-minutes table, rather than accepting the bare percentage at face value, is the single fastest way to understand what is actually being promised.

A third trap, worth naming explicitly and returned to more fully in Part 18: a raw count presented on its own, with no accompanying context about failure or quality, is a vanity metric. "We processed ten million requests today" sounds impressive and says nothing whatsoever about how many of those ten million actually succeeded, or how long they took. Any large raw count offered as evidence of health, on its own, should immediately prompt the question: compared to what denominator, and paired with what quality measure?

## Part 10: What Comes Tomorrow

Metrics answer how much and how often, in aggregate, cheaply, at whatever volume a system can produce. Tomorrow's post turns to the oldest of the three classic signals in its modern, structured form: logs — from the print-debugging habit introduced back in Part 2 of this series, through the anatomy of a proper structured log record, to the single most valuable payoff this series has been building toward since Part 1: using a trace ID to jump directly from one specific error log line to the exact span, and the exact neighboring spans, that produced it.

## Sources and Further Reading

- OpenTelemetry Project, ["Metrics,"](https://opentelemetry.io/docs/concepts/signals/metrics/) opentelemetry.io — the primary source for the seven official instrument kinds, their formal definitions, and the odometer and fuel-gauge analogies this post builds on directly.
- OpenTelemetry Project, ["Metrics Data Model,"](https://opentelemetry.io/docs/specs/otel/metrics/data-model/) opentelemetry.io — the formal specification underlying the temporality discussion in Part 7.
- OpenTelemetry .NET Documentation, ["Metric Instruments,"](https://opentelemetry.io/docs/languages/dotnet/metrics/instruments/) opentelemetry.io — the source for the C# `Meter.CreateCounter` example in Part 3, verified against current documentation.

*Tomorrow: logs from first principles — from print statements to structured records, and the payoff of correlating a single log line to the exact trace that produced it.*
