---
title: "How to Spot the Lie: The Complete Toolkit"
date: 2026-08-22
author: mercifulpotato-team
summary: "Eighteen days, eighteen tests. This post gathers every one of them into a single standalone toolkit, adds three new tests this series hasn't yet covered, and ends with the one test that has to survive being turned on itself."
tags:
  - opentelemetry
  - observability
  - plain-english
  - critical-thinking
series: "OpenTelemetry from the Ground Up"
---

## Part 1: Why a Toolkit, Not a Listicle

Every post in this series, since Part 1, has closed with a section applying one specific, repeatable test to one specific kind of misleading claim. Scattered across eighteen days, each test made sense in the context of whatever that day's material happened to be. Gathered into one place, a genuine pattern becomes visible — and that pattern, not any single test in isolation, is the real payoff of today's post.

The pattern is this: nearly every test this series has built reduces to the same underlying move, applied to a different surface. Replace a vague, confident, unfalsifiable claim with a specific, checkable one, and see whether it survives the replacement. A claim that survives being made specific was probably honest to begin with. A claim that only sounded convincing while it stayed vague was not offering you information at all — it was offering you a feeling, dressed as information. Today's post makes that underlying move explicit, gathers every surface this series applied it to, and adds three genuinely new surfaces this series has not yet had a dedicated day to cover.

## Part 2: The Toolkit, Consolidated

What follows is every test from Parts 1 through 18, restated compactly, organized by what kind of claim each one targets.

**Claims about coverage and completeness.** "We have full visibility" (Part 1): ask what specifically can be seen and what specifically cannot, rather than accepting a sweeping, unfalsifiable assurance. "End-to-end tracing" (Part 7): pick one real request, follow its actual trace ID by hand through every service it should touch, and verify the chain genuinely holds rather than silently splitting somewhere.

**Claims about standards and compatibility.** "Industry standard" (Part 2): check for a public specification independent of any one vendor, multiple genuinely independent implementations, and governance that no single company controls. "Powered by OpenTelemetry" and "OTLP-compatible" (Parts 4 and 14): ask specifically whether ingestion is native, converted at the door, or merely an exporter-side afterthought, which signals and transports are actually covered, and what, concretely, fails to survive the trip intact.

**Claims about vendor and institutional communication.** Reading a foundation press release (Part 3): separate what a milestone actually, specifically certifies from what the surrounding celebratory tone merely implies. Migration-guide archaeology (Part 5): a vendor's own technical migration documentation reveals where its real engineering investment is flowing more honestly than its marketing does. The structural "no lock-in" test (Part 10): ask which specific layer — instrumentation, or the analysis and visualization sitting on top of it — a portability claim actually covers.

**Claims built on selective demonstration.** The suspiciously clean sample and the demo trace (Parts 6 and 12): ask to see the failure path, the gap, the thing that went wrong in collection itself, not only the polished happy case. "Instant observability, no code changes" (Part 11): pick one genuinely specific business question and ask whether the zero-code solution being demonstrated can actually answer it. The pipeline diagram with no failure arrows (Part 13): ask what happens at each arrow when the box on the receiving end is briefly unreachable.

**Claims built on numbers.** Averages, uptime nines, and vanity metrics (Part 8): convert a percentage to concrete minutes per month; ask for the distribution, not the average; ask what denominator a large raw count is being measured against. Log-volume theater (Part 9): a large ingest figure is not evidence anyone can find the one line that matters; ask how many steps separate an alert from the trace that explains it. The cross-service comparison chart (Part 15): ask what mapping reconciled genuinely different underlying data, and what quietly failed to survive it. Sampled data presented as a census (Part 16): ask whether a number was adjusted for the sampling rate that produced it, and in which direction — toward understatement or toward artificial alarm — that rate would bias it.

**Claims about cost and value.** The pricing page (Part 17): do the arithmetic yourself, in the open, using your own honestly estimated traffic, before any sales conversation. ROI theater (Part 18): scrutinize the specific dollar figures and counterfactual assumptions fed into any calculation claiming observability investment simply pays for itself.

Eighteen tests, one underlying move, applied to eighteen different surfaces.

## Part 3: New Surface — Dashboard Theater

Here is a surface this series has referenced but never named directly: a dashboard, full of green indicators, that creates a powerful, comforting impression of health without any of those indicators actually being wired to a threshold that would meaningfully change color if something were genuinely wrong. This is dashboard theater — visual reassurance manufactured by the simple, human tendency to trust a clean, well-designed screen more than the screen's own content actually warrants.

The test: for any dashboard you are shown, pick one specific green indicator and ask, directly, "what specific value would need to occur for this to turn red, and when, historically, did it last actually do so?" A dashboard whose owner can answer this precisely, for any indicator you pick, is demonstrating a real, functioning alerting relationship between the number and the color. A dashboard whose owner cannot answer it — where the threshold turns out to be an arbitrary default nobody has revisited since the panel was created, or where the panel has simply never turned red regardless of what happened underneath — is decoration, not observability, regardless of how professional it looks.

## Part 4: New Surface — SLO Gaming

A service-level objective, typically something like "99.9% of requests complete successfully within 200 milliseconds," is a genuinely valuable tool when set honestly and monitored faithfully. It becomes a different thing entirely once meeting the number, rather than the underlying reality the number was meant to represent, becomes the actual goal an engineer or a team is being measured against — a distinction that echoes, in a new form, the exact "inputs versus outcomes" theme this series introduced in Part 1 and returned to explicitly in Part 18.

Concrete, common ways this happens, worth naming plainly: narrowing the definition of what counts as a "request" for measurement purposes until only the easy, reliably fast cases remain inside the denominator; quietly excluding a chronically troublesome but genuinely important endpoint from SLO measurement entirely, on the grounds that it is "not representative"; or setting the threshold itself so generously in the first place that the objective was never at meaningful risk of being missed regardless of how the underlying service actually performed. Each of these produces a technically true, unbroken SLO chart while doing nothing whatsoever to make the actual user experience better — and in the narrowing and exclusion cases, potentially concealing real, ongoing user pain sitting just outside whatever boundary was quietly redrawn to keep the number looking good.

The test: ask, for any SLO presented as evidence of health, exactly what is excluded from its denominator, and exactly who made that exclusion decision and why. An SLO whose scope has been narrowed by the same team being measured against it, without independent review, deserves real scrutiny before being trusted as evidence of anything.

## Part 5: New Surface — Goodhart's Law, Named

The pattern underlying SLO gaming, and honestly a fair amount of the vanity-metric material from Part 8, has a name, and giving it that name explicitly is worth doing before this series closes: Goodhart's Law, most commonly stated in the form popularized by anthropologist Marilyn Strathern in 1997, building on a phrasing by Keith Hoskin the year before — "when a measure becomes a target, it ceases to be a good measure." It traces back to economist Charles Goodhart's own considerably more technical 1975 observation about monetary policy, that any observed statistical regularity tends to collapse once real pressure is placed on it for control purposes.

Applied directly to this series' own subject matter: the moment "reduce the number of Error-severity log lines" becomes a target an engineer is evaluated against, the easiest, least effortful way to satisfy that target is very often to reclassify genuine errors as Warnings, rather than to actually fix the underlying problems generating them — satisfying the letter of the measurement while making the real, underlying system being measured no better, and the telemetry describing it actively less trustworthy than before the target existed. Every metric this series has built across the last nineteen days — span counts, error rates, sampling percentages, SLO compliance — is vulnerable to exactly this dynamic the moment it stops being a tool for understanding and starts being a tool for judging the people who produce it. This is not a flaw specific to OpenTelemetry, or to observability tooling generally; it is a structural feature of measurement itself, wherever it is applied, and naming it plainly is the best available defense against it.

## Part 6: The Meta-Test — Spotting Confident, Fluent Technical Prose

This series has been, cover to cover, an extended piece of confident, fluent, well-organized technical writing — and it would be a strange omission to close this particular post without acknowledging that the same skepticism it has spent eighteen days building applies to writing exactly like this, including this series itself.

Fluent, well-structured prose is not, on its own, evidence of accuracy. It is evidence of fluency. The genuine test of any confident technical claim, wherever it originates — a human blog post, a vendor's documentation, or for that matter a long-form series like this one — is not how smoothly it reads, but whether its specific, checkable claims survive being checked against a primary source. This series has tried to make that checking easy throughout, by naming primary sources plainly in a dedicated section at the end of every single post, by distinguishing verified facts from reasonable inference, and by flagging explicitly, repeatedly, the places where a claim was new enough or fast-moving enough to warrant a fresh check by the time you are actually reading it. A reader who takes this series at its word, rather than spot-checking a claim or two against the linked sources, has not been deceived by it — but has also not fully exercised the very habit this entire post exists to recommend.

## Part 7: How to Spot the Lie — The Checklist That Never Invalidates Itself

Every prior test in this series pointed outward, at some other claim. This final one, fittingly, points at itself, because a toolkit for detecting overclaiming that is not itself subject to the same scrutiny would be a strange and self-undermining place to end.

So: this toolkit does not certify that a claim satisfying all of its tests is therefore true. It certifies something narrower, and more honest: that the claim has been stated specifically enough to be checked at all. A claim can pass every single test in this post — a precisely stated scope, a genuine primary source, an honestly built SLO with a clearly documented denominator — and still turn out, on the actual evidence, to be wrong. What this toolkit buys you is not certainty. It is the ability to locate, precisely, where a specific disagreement actually lives, rather than being left to argue about a vague, unfalsifiable impression that never had a precise location to begin with. That, in the end, is the most honest thing nineteen days of testing claims against evidence can actually promise you, and it is enough.

## Part 8: What Comes Tomorrow

Today's post consolidated. Tomorrow's post, the twentieth and final installment of this series, looks outward one last time: OpenTelemetry's governance in its own current, still-developing form, the road ahead as the project's own Governance Committee has publicly described it, and a closing, honest reckoning with everything this series has built across the preceding nineteen days.

## Sources and Further Reading

- Marilyn Strathern, "'Improving Ratings': Audit in the British University System," *European Review* 5, no. 3 (1997): 305–321 — the primary academic source for the popularized phrasing of Goodhart's Law quoted in Part 5, building on Keith Hoskin's 1996 formulation.
- Charles Goodhart, *Monetary Theory and Practice* — the original, more technical 1975 formulation underlying the law named after him, referenced in Part 5.
- Every prior post in this series, Parts 1 through 18, each linked from its own dated entry in the archive — the primary sources for the eighteen individual tests consolidated in Part 2 of this post.

*Tomorrow, the finale: governance, the road ahead, and a closing reckoning with everything this series has built.*
