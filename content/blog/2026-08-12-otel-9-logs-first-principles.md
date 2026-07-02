---
title: "Logs from First Principles: From Print Statements to Structured Records"
date: 2026-08-12
author: mercifulpotato-team
summary: "Logs are the oldest signal in this series, and OpenTelemetry treats them with unusual humility: rather than replacing what already exists, it bridges to it. This post explains why, and shows the payoff — jumping from one log line to the exact trace that produced it."
tags:
  - opentelemetry
  - observability
  - plain-english
  - logging
series: "OpenTelemetry from the Ground Up"
---

## Part 1: The Oldest Habit in Programming

Every programmer, in every language, in every era of computing, has done some version of the same thing: inserted a line into their own code that prints a short message describing what the code is currently doing, then run the program and watched the messages scroll by. This habit, often called print debugging, predates distributed systems, predates the internet, and predates nearly every other idea in this series by decades. Part 2 of this series traced its earliest formalized version — Eric Allman's syslog, built in the early 1980s — but the underlying habit itself is older still, essentially as old as programming.

There is nothing to be embarrassed about in this habit, and this post will not pretend otherwise. A single programmer, working alone on code running on one machine, gets genuinely excellent value from a handful of well-placed print statements. The habit only stops working well the moment a single logical task starts crossing the boundaries this entire series has been describing since Part 1: when the "cook" and the "waiter" are different processes, on different machines, and a note scribbled in one process's own console output is invisible to everyone else.

## Part 2: What a Log Actually Is

OpenTelemetry's own documentation defines a log, cleanly and without unnecessary elaboration, as a timestamped text record, either structured or unstructured, with optional metadata. A log answers a narrower and more specific question than either of the two signals this series has already covered in Parts 6 through 8: not "how did this request travel through the system" and not "how many of these have happened in total," but simply "at this specific moment, this specific thing happened, and here is a description of it."

It is worth noting a small but genuinely useful piece of vocabulary from OpenTelemetry's own specification here: the project defines an event as a special, more tightly structured kind of log, used specifically to record something with a well-defined name and structured data rather than free-form text. The relationship between the two is one-directional and worth remembering precisely: not all logs are events, but all events are logs.

## Part 3: The Anatomy of a Modern Log Record

A log record, in OpenTelemetry's formal data model, carries considerably more than the bare timestamp-plus-message pairing that older, simpler logging habits produced. A complete record includes a precise timestamp; a severity level, indicating how serious or routine the event is; a body, which is the actual message content, whether free text or structured data; a set of attributes, exactly the same key-value concept introduced for spans back in Part 6; and a resource, identifying which specific service, instance, and environment produced this particular record.

Crucially, and this is the detail that turns an isolated line into something considerably more powerful, a log record can also carry a trace ID and a span ID — the very same identifiers this series built up carefully across Parts 6 and 7. When those identifiers are present, a single log line stops being an isolated fact and becomes one specific, dateable, locatable chapter of one specific customer's journey, connected directly to the exact span, and the exact neighboring spans, that were active at the moment it was written.

## Part 4: Structured, Unstructured, and the Trap in Between

OpenTelemetry's own documentation draws a careful three-way distinction here that is worth preserving precisely, because a common and easy mistake is to think that simply formatting a log message as JSON automatically makes it "structured" in the sense that actually matters.

An unstructured log is exactly what decades of print debugging have always produced: free-form prose, meant for a human eye, with no fixed, predictable layout a machine could reliably parse — "checkout failed for order 8821, payment gateway timed out after 8 seconds." A structured log, by contrast, has a consistent, predictable schema: the same fields, in the same shape, every single time an event of this kind occurs, so that software, not just a human reader, can reliably extract and query specific fields across millions of records. And in between sits something OpenTelemetry's own documentation is careful to name explicitly as its own category, semistructured: a log that happens to be encoded as JSON, and therefore looks organized at a glance, but whose actual fields, field names, or nesting vary unpredictably from one occurrence to the next, meaning a program trying to reliably extract "the order ID" cannot actually count on it appearing in the same place every time. Encoding as JSON is not, by itself, a guarantee of the schema stability that the word "structured" is actually meant to promise.

Here is the same underlying event, shown in these three forms, to make the distinction concrete rather than abstract:

```
Unstructured: Checkout failed for order 8821, payment gateway timed out after 8s

Semistructured JSON (fields vary between occurrences):
{"msg": "checkout failed", "order": "8821", "gateway_timeout_seconds": 8}

Structured (consistent schema every time):
{
  "timestamp": "2026-08-12T03:47:12.881Z",
  "severity": "ERROR",
  "body": "Checkout failed: payment gateway timeout",
  "attributes": {
    "order.id": "ORD-88213",
    "payment.gateway": "external-processor",
    "timeout.seconds": 8
  },
  "trace_id": "4bf92f3577b34da6a3ce929d0e0e4736",
  "span_id": "9c2748f5f0c8a3d1"
}
```

Only the third version, with a genuinely fixed, predictable set of field names used consistently every time this kind of event occurs, delivers what a modern observability practice actually needs: the ability to reliably query, filter, and aggregate across millions of log records by field, rather than falling back on searching free text and hoping the wording happened to be consistent.

## Part 5: Severity Levels, and the Inflation Problem

Every serious logging system, going back directly to syslog's original eight-level severity scale introduced in Part 2, uses some version of a severity level: a way of marking how serious or routine a given event is, so that a reader — or an automated alerting system — can filter for what actually matters without wading through everything. A typical modern scale runs from Trace and Debug, at the routine end, through Info, Warning, Error, and up to Fatal or Critical at the most serious end.

The genuine, common problem this series would be remiss not to name plainly: severity levels are only useful if they are applied with real discipline, and in practice, discipline erodes over time in almost every codebase that is not actively maintained against it. A developer under deadline pressure, uncertain whether a given condition truly warrants urgent attention, will very often mark it Error simply to be safe, "just in case." Multiply this across enough developers and enough years, and a codebase can end up with a severity scale that has quietly inflated into meaninglessness — where Error means anything from "the entire system is down" to "a single optional feature returned a slightly unexpected value," and a genuinely serious Error is now indistinguishable, at a glance, from routine background noise. A log severity scale is only as trustworthy as the discipline behind it, and that discipline is worth actively protecting rather than assuming it will simply take care of itself.

## Part 6: OpenTelemetry's Deliberate Humility

Here is where logs, as a signal, diverge sharply in approach from traces and metrics, and the divergence is deliberate rather than accidental. When OpenTelemetry defined its tracing and metrics data models, it was largely building something new, asking application authors to adopt new concepts — spans, instruments — more or less from scratch. Logs are different: as OpenTelemetry's own documentation states plainly, of all telemetry signals, logs have the biggest legacy, and most programming languages already have deeply entrenched, well-established logging libraries that predate OpenTelemetry by many years and that huge amounts of existing code already depend on directly.

Rather than asking the entire software industry to rip out and replace decades of existing logging code, OpenTelemetry instead built what it calls a Log Appender, or Bridge: a piece of integration code that listens to whatever logging library or built-in facility a given language already uses, and channels that existing output into OpenTelemetry's own log data model, without requiring the application's own logging calls to change at all. In .NET, this bridge connects directly to Microsoft.Extensions.Logging's ILogger — the same interface .NET developers have already been using for years, entirely independent of OpenTelemetry's existence. In Java, it connects to established libraries like Log4j and SLF4J. In Python, to the standard library's own built-in logging module.

This adoption strategy trades away a small amount of architectural elegance for something considerably more valuable in practice: a vastly lower barrier to adoption. A team with years of existing ILogger-based logging code scattered throughout a large application does not need to touch a single one of those existing calls to begin benefiting from OpenTelemetry's log correlation — they need only ensure the bridge itself is correctly wired up in their application's startup configuration, exactly as later posts in this series, particularly Part 12, will show concretely.

It is worth stating honestly, in keeping with this series' commitment to the "specified is not the same as implemented" distinction from Part 4, that this bridging strategy has produced genuinely uneven results across languages, and the unevenness is worth naming rather than glossing over. While the logs signal itself is formally stable in OpenTelemetry's specification, and while C#/.NET, C++, Java, and PHP have each reached a stable implementation of the Logs API and SDK, other languages remain considerably further behind: Go and Rust sit at a beta level, and Erlang/Elixir, JavaScript, Python, Ruby, and Swift each remain, as of this writing, at what the project's own documentation labels a development level rather than stable. This is precisely the four-stage distinction from Part 4 — specified, implemented, stable, adopted — playing out visibly and unevenly, signal by signal and language by language, and it is exactly the kind of claim this series insists on verifying against the project's own current documentation rather than assuming uniform maturity across every language a reader might happen to use.

## Part 7: Correlation — the Payoff This Series Has Been Building Toward

Here, finally, is the concrete moment this entire series has been quietly working toward since its opening post. Recall the Facebook incident from Part 1: engineers who could not quickly diagnose what had gone wrong, in part because the tools meant to show them what was happening depended on the very thing that had broken. Recall the slow checkout trace from Part 6, which told us precisely that a payment charge, not inventory validation, consumed almost the entirety of an unacceptably slow request. Now put the two together.

Imagine an on-call engineer, at three in the morning, staring at a single error log line: "Checkout failed: payment gateway timeout." On its own, in the old, unstructured, uncorrelated world this series described back in Part 2, this line raises far more questions than it answers. Which customer? Which specific request, out of the millions that ran that day? Was this an isolated blip, or part of a wider pattern? What was happening in the rest of the system at that exact moment?

With OpenTelemetry's automatic correlation in place — precisely the mechanism described in the project's own documentation, where an active SDK automatically wraps the log body with the trace and span identifiers active at that moment — that single log line now carries a trace ID directly on it. The engineer can take that one trace ID and, using precisely the mechanism from Part 6, pull up the complete trace: the full checkout journey for this exact order, with every span, every duration, and every neighboring event visible at once. The isolated, context-free line has become the entry point into the complete, connected story of exactly what happened, to exactly which customer, at exactly which point in the system's larger behavior that night. This loop — error log to trace ID to full trace to root cause — is the single most concrete, practical payoff this whole series exists to explain, and it depends entirely on the correlation mechanism this post has just described.

## Part 8: Logs and Money

It would be inconsistent with this series' running commitment to honesty about cost to leave this post without acknowledging plainly what logs, of the three classic signals, tend to cost the most. Logs are, in most real systems, the highest-volume signal by a considerable margin, and correspondingly they are very often the largest single line item on an observability bill. Every request can plausibly generate several log lines across every service it touches, and at genuine scale this adds up to a volume that dwarfs the number of metric data points or even the number of spans covering the same traffic.

What teams actually do about this, in practice, is worth stating plainly rather than left implied: retain different severities for different lengths of time, keeping routine Debug-level output only briefly, or sampling it down to a small fraction, while retaining Error-level output, which is comparatively rare and disproportionately valuable during an investigation, for much longer. This is a direct preview of the sampling discipline Part 16 of this series covers in full for traces, applied here to a different signal with its own distinct cost profile.

## Part 9: How to Spot the Lie — Log-Volume Theater

A specific, recurring boast deserves its own skeptical test today: a team, or a vendor, proudly announcing the sheer volume of logs it ingests — "we process fifteen terabytes of logs a day" — offered as if the number itself were evidence of thoroughness or observability maturity.

Treat a bare volume figure of this kind with exactly the same skepticism Part 8's discussion of vanity metrics urged for any large raw count offered without context. A genuinely enormous daily log volume is not, on its own, evidence that anyone can actually find the one line that matters during an actual incident; if anything, an unmanaged flood of routine, undisciplined logging makes that one important line measurably harder to locate, buried under a vastly larger volume of noise that nobody has bothered to curate, structure, or assign meaningful severity to. A large ingest figure recited as a boast is, more often than its speaker perhaps realizes, closer to an inadvertent confession: proof of volume, not proof of usefulness.

The more useful question to ask, in place of "how much do you log," is "when the payment gateway timed out at three in the morning, how many steps, and how much searching, did it take to go from the alert to the specific trace that explained it." A team that can answer that second question crisply, using the correlation mechanism described in Part 7, is demonstrating a genuinely mature logging practice. A team that can only answer the first question, with an impressive-sounding number and nothing more, has not yet demonstrated anything about whether their logging is actually useful when it counts.

## Part 10: What Comes Tomorrow

The last four days have built OpenTelemetry's three classic signals, one at a time, from first principles: traces in Parts 6 and 7, metrics in Part 8, and logs today. Tomorrow's post steps back from any single signal and returns to an architectural question this series flagged but deliberately deferred back in Part 10 — wait, we have not reached Part 10 yet, tomorrow is Part 10 — the API and SDK split introduced conceptually in Part 4: why OpenTelemetry deliberately separated the interfaces application code calls from the actual working implementation behind them, and why this single architectural decision is a large part of what makes everything covered so far in this series actually deployable in the real world, one library at a time, without forcing every dependency in an application to agree on a single telemetry vendor all at once.

## Sources and Further Reading

- OpenTelemetry Project, ["Logs,"](https://opentelemetry.io/docs/concepts/signals/logs/) opentelemetry.io — the primary source for this post's definitions of log records, the structured/semistructured/unstructured distinction, the Log Appender/Bridge architecture, and the current per-language stability table referenced in Part 6.
- OpenTelemetry Project, ["Logs Data Model,"](https://opentelemetry.io/docs/specs/otel/logs/data-model/) opentelemetry.io — the formal specification for log record fields including severity, body, attributes, and trace/span correlation.
- OpenTelemetry .NET Documentation, ["Log Correlation,"](https://opentelemetry.io/docs/languages/dotnet/logs/correlation/) opentelemetry.io — the source for the automatic trace-and-span-ID correlation behavior described in Part 7, specific to the .NET ILogger bridge.
- Internet Engineering Task Force, RFC 5424, "The Syslog Protocol" — referenced in Part 5 for the lineage of the severity-level concept, previously covered in full in Part 2 of this series.

*Tomorrow: the API and SDK split — why OpenTelemetry deliberately divided itself in two, and why that division is what makes the rest of this series actually work in practice.*
