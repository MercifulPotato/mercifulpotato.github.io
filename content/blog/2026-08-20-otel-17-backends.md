---
title: "Backends: Where Telemetry Goes to Be Useful"
date: 2026-08-20
author: mercifulpotato-team
summary: "OpenTelemetry deliberately stops short of storing or displaying anything. This post surveys where the data actually ends up — open-source, commercial, and this publication's own honest, proportionate answer — with no rankings and no recommendations."
tags:
  - opentelemetry
  - observability
  - plain-english
  - devops
series: "OpenTelemetry from the Ground Up"
---

## Part 1: What a Backend Actually Does

Part 4 of this series drew a sharp, deliberate line around what OpenTelemetry is not: a place data gets stored, searched, or displayed. Every post since has built the machinery that gets telemetry generated, correctly shaped, correctly sampled, and correctly sent onward — but every single one of those posts has stopped at the door of wherever that telemetry finally lands. Today's post walks through that door.

A backend, in the sense this post uses the word, does five distinct jobs: it stores telemetry, typically for some bounded retention period; it indexes that telemetry so specific pieces can actually be found again later; it lets a human or a program query it, using whatever language that particular backend has chosen; it visualizes the results, typically as dashboards, graphs, and trace waterfalls of exactly the kind Part 6 introduced; and it can alert, triggering a notification when some condition it is watching for becomes true. OpenTelemetry, deliberately, does none of these five things itself — which is precisely why this post exists as its own dedicated day in this series, filling in the one piece of the picture every prior post has assumed but never actually named.

## Part 2: The Open-Source, Self-Hosted Road

Several backends predate OpenTelemetry itself and have, over time, added varying degrees of native OTLP support, exactly the four-question test Part 14 built for evaluating any "OTLP-compatible" claim.

Jaeger, introduced back in Part 2's history of pre-OpenTelemetry tracing tools, remains a CNCF-graduated project focused exclusively on distributed tracing, and it accepts OTLP directly. A detail worth stating plainly because it illustrates this series' own recurring theme of how standards eventually supersede the tools that came before them: Jaeger's own original client libraries have themselves been deprecated in favor of instrumenting with OpenTelemetry SDKs directly, meaning OTel is now the primary, recommended way to get data into Jaeger at all — a genuinely striking illustration of a pre-OpenTelemetry tool's own maintainers concluding that the neutral standard had simply become the better path, even into their own product.

Prometheus, whose origin story Part 2 also told in full, has moved from metrics-only, pull-based scraping toward native OTLP ingestion as a first-class, stable capability: an OTLP receiver reached stable status in Prometheus's own 3.0 release, after an earlier period as an experimental, opt-in feature. This is a genuinely useful, concrete example of the specified-implemented-stable-adopted ladder from Part 4 playing out over real, dated releases, rather than as an abstract description.

The Grafana ecosystem takes a different architectural shape entirely: rather than one unified backend, it offers a family of purpose-built, signal-specific stores — Tempo for traces, Mimir for metrics, Loki for logs — each accepting OTLP with its own degree of nativeness, unified under Grafana itself as a shared visualization layer sitting on top of all three. Mimir, specifically, documents native OTLP ingestion over HTTP directly, rather than requiring a translation step. The honest trade-off this architecture carries, worth stating plainly rather than glossing over: three specialized stores mean three separate query languages to learn — Tempo's TraceQL, Mimir's PromQL, and Loki's LogQL — and three separate sets of operational and scaling concerns, in exchange for the genuine benefit of each individual store being purpose-built and well-optimized for its own specific signal.

## Part 3: The Commercial Road, Described Structurally

The commercial vendor category — familiar names like Datadog, New Relic, Dynatrace, Splunk, and the cloud-native offering examined at length back in Part 5, Azure Monitor — is worth describing structurally rather than comparatively, in keeping with this series' stated commitment to giving readers the tools to evaluate options themselves rather than ranking them.

Pricing models across this category vary in ways that genuinely shape behavior, and it is worth naming the incentive each one creates rather than treating pricing as a mere administrative detail. A per-host or per-instance pricing model incentivizes consolidating workloads onto fewer, larger machines. A per-gigabyte-ingested model incentivizes exactly the sampling discipline Part 16 covered in detail, since every byte sent has a direct, visible cost. A per-user, seat-based model incentivizes limiting how many engineers actually get direct access to the tooling. None of these models is inherently dishonest, but each one nudges an organization's actual behavior in a specific, predictable direction, and understanding which model a given vendor uses tells you something real about what that vendor's pricing structure will quietly encourage you to do over time.

## Part 4: Degrees of OTel-Nativeness, Applied

Part 4 and Part 14 of this series each built a version of the same underlying test: does a system genuinely, natively speak OTLP, or does it convert at the door, and if the latter, what quietly fails to survive that conversion intact? This distinction maps onto the backend landscape with unusual clarity. A small number of backends were built from the ground up around OTLP as their native ingestion format, with no internal proprietary format standing between the wire protocol and storage. A much larger number — including, honestly, parts of the very open-source stacks praised in Part 2, plus the entire commercial vendor category — predate OpenTelemetry, and accept OTLP as an ingestion option that gets converted, at the door, into a proprietary or pre-existing internal format for actual storage and querying.

This is not, in itself, a mark against the second category; genuine ecosystem maturity, years of production hardening, and deep, polished dashboard functionality are real and valuable, and this series does not intend to suggest otherwise. But the honest, practical consequence, worth checking directly rather than assuming, is data fidelity: whether custom attributes, specific resource labels, or less commonly used semantic-convention fields survive the conversion step intact, or whether some of them are silently truncated, dropped, or reinterpreted along the way. The test from Part 14 applies here without modification: ask specifically, and verify concretely by inspecting a real, ingested trace against what was actually sent.

## Part 5: This Site's Own Honest Scale

It would be inconsistent with everything this series has argued to leave this post without turning the same lens on this publication's own infrastructure. Merciful Potato Magazine is a static Blazor WebAssembly site, hosted on GitHub Pages, with no backend server of its own beyond a free-tier Cloudflare Workers analytics endpoint. Deploying a full Collector-and-backend stack of the kind described across Parts 13 through this post — a Jaeger instance, a Prometheus server, a Grafana dashboard, ongoing operational upkeep for all three — would be a considerable, genuinely disproportionate overreach for a site of this size and these stakes, echoing directly back to Part 1's very first framing of this series: the cost of seeing has to be weighed honestly against what there actually is to see, and against what would actually go wrong if you couldn't.

This is not a failure to adopt best practice. It is best practice, correctly scaled: the appropriate observability investment for a small, free, static publication is a small, free, appropriately scoped one, and matching the two honestly is itself the discipline this entire series has been building toward, not a compromise against it.

## Part 6: Mixing and Migrating

One further, genuinely practical payoff of everything built since Part 13 deserves naming directly here: the Collector's fan-out capability — a single pipeline configured with more than one exporter, sending the identical stream of incoming telemetry to multiple destinations simultaneously — makes it straightforward to trial a new backend alongside an existing one, in production, on real traffic, without any risk to the incumbent system, simply by adding a second exporter to an existing pipeline until the new option has been evaluated on its own merits and a final decision made.

## Part 7: How to Spot the Lie — The Pricing Page

A specific, concrete exercise deserves this post's own closing test: before any sales conversation with a commercial observability vendor, do the arithmetic yourself, in the open, for a hypothetical, honestly sized version of your own actual service. Using the volume figures Part 16 already worked out — a modest one-thousand-requests-per-second service producing tens of millions of spans an hour before any sampling is applied — take a vendor's own published per-gigabyte or per-host rate and compute, concretely, what a realistic month would actually cost, both before and after a sensible sampling policy of the kind Part 16 described.

Watch specifically for the mechanics a pricing page tends to leave for the fine print rather than the headline: what a generous-sounding "free tier" actually covers once real production traffic exceeds it; what egress charges apply if you ever want your own data back out; and what the vendor's own default retention period is, since a lower headline price attached to a shorter retention window is not the same offer as a higher price with data kept for a year. A vendor confident in its own honest value will not object to you doing this arithmetic yourself, in advance, and a genuinely well-matched choice is one where the number you compute yourself, independently, roughly matches what you are eventually quoted.

## Part 8: What Comes Tomorrow

This post surveyed where telemetry goes once it leaves the pipeline this series spent thirteen days building. Tomorrow's post turns the series' skepticism fully inward, onto OpenTelemetry itself, for the single most important post of the closing stretch: the full, honest accounting of cost and downside — overhead, complexity, uneven maturity, and the sincere, direct question of when the right answer is simply not to adopt OpenTelemetry at all.

## Sources and Further Reading

- OpenObserve, ["What Backends Support OpenTelemetry (OTLP)?"](https://openobserve.ai/blog/opentelemetry-backends-otlp-support/) — a current, comparative survey of OTel-native versus OTel-compatible backends, cross-referenced against vendor-specific documentation for the claims used in Parts 2 and 4 of this post.
- Grafana Labs, ["OpenTelemetry at Grafana Labs"](https://grafana.com/docs/opentelemetry/) and ["Grafana Tempo Documentation"](https://grafana.com/docs/tempo/latest/) — the primary sources for the Tempo, Mimir, and Loki OTLP-ingestion claims in Part 2.
- Jaeger Project Documentation and CNCF project pages — the source for Jaeger's graduated status and its own client-library deprecation in favor of OpenTelemetry SDKs, referenced in Part 2.
- Prometheus Project Documentation and release notes — the source for the native OTLP receiver's path from experimental to stable status across Prometheus releases, referenced in Part 2.

*Tomorrow: the costs and downsides of OpenTelemetry itself, argued as sincerely and thoroughly as this series has argued for its benefits — including a direct, honest answer to when the right choice is not to adopt it at all.*
