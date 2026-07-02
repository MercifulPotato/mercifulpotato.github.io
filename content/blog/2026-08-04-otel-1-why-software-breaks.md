---
title: "Why Software Breaks and Nobody Knows Why: The Case for Observability"
date: 2026-08-04
author: mercifulpotato-team
summary: "Modern software fails in ways its own builders cannot see. This series opener explains, from absolute first principles, what observability means, why the industry invented it, and what it costs to have — or to lack."
tags:
  - opentelemetry
  - observability
  - plain-english
  - software-engineering
series: "OpenTelemetry from the Ground Up"
featured: true
---

## Part 1: A Monday That Would Not End

At 15:39 Coordinated Universal Time on October 4, 2021, a routine maintenance command was issued somewhere inside Facebook's network. Its purpose was mundane: to check how much spare capacity existed on the company's global backbone, the huge web of connections linking Facebook's data centers to one another and to the rest of the internet. The command was supposed to be a health check. Instead, an error in the tool meant to catch exactly this kind of mistake let the command through, and it severed the backbone connections it was only meant to inspect.

What happened next is a case study in how a single small event can cascade into something enormous, and in how a company staffed with some of the best network engineers on the planet can still spend hours groping in the dark before it understands what has gone wrong. Facebook's data centers, cut off from the rest of the network, lost the ability to route traffic anywhere, including to the public internet. Facebook's domain name system servers — the machines that translate a human-readable address like "facebook.com" into the numerical address a computer actually needs — were built with a safety mechanism: if they could not reach Facebook's own data centers, they would assume something was badly wrong with the network and would stop announcing their own existence to the rest of the internet. That safety mechanism worked exactly as designed. It just happened to be responding to the wrong emergency. Because the DNS servers announced their absence, the rest of the internet's routers concluded that the addresses for facebook.com, instagram.com, and whatsapp.com simply did not exist anywhere, and stopped trying to send traffic there at all.

By the outside world's clock, the effect was near-total silence. Cloudflare's engineers, watching global internet routing data as part of their own infrastructure business, noticed the withdrawal of Facebook's routes within a minute and initially worried it might be a fault in their own systems. It was not. Facebook, along with Instagram, WhatsApp, Messenger, and Oculus, had effectively vanished from the addressable internet. For close to six hours, by most independent measurements, and by Facebook's own account closer to six and a half, none of it could be reached by anyone, anywhere, through any normal means.

Here is the detail that matters most for a series about observability. Facebook did not lack monitoring. This was not a company with no dashboards, no alerts, and no on-call engineers. Facebook had, and has, one of the most sophisticated internal telemetry operations in the technology industry, built over more than a decade of hard-won experience running services at a scale few companies will ever approach. And yet the company's own subsequent account of the incident describes engineers unable to diagnose the problem quickly, in part because the very tools they normally used to investigate outages depended on the same internal network that had just been severed. The people whose job was to understand what was broken could not reach the systems that would have told them. When they eventually needed to physically enter a data center to intervene, they found that the badge readers and internal tools used to grant that access also ran on the same crippled network. Engineers who could ordinarily fix a problem like this from their laptops in minutes were, for a period, locked out of the buildings that held the hardware they needed to touch.

This is not a story about incompetence. It is a story about a boundary that every sufficiently complex system runs into eventually: the boundary between what a system's builders believe is true about how it behaves, and what is actually happening inside it at any given moment. Facebook's engineers understood their network in extraordinary detail under normal conditions. What they had not fully anticipated was a failure mode in which the tools used to see the network were themselves dependent on the part of the network that had failed. Observability, done well, is largely the discipline of closing that gap — of making sure that when something breaks, including in a way nobody predicted, the people responsible can still see what is happening well enough to act.

This twenty-part series is about a particular, and currently dominant, approach to closing that gap: an open standard called OpenTelemetry. Before we can explain what OpenTelemetry is, why it exists, or how to use it, we have to build up, from nothing, an understanding of the problem it exists to solve. That is the entire purpose of this first post. If you have never worked professionally with software, everything you need is here. If you have, we still recommend reading carefully, because the vocabulary introduced in this post is used precisely and consistently for the following nineteen days, and a shaky foundation here will make later posts harder than they need to be.

## Part 2: What Software Actually Is, Once It Is Running

Most people encounter software as something static: an app icon, a website, a program installed on a computer. That picture, while not wrong, hides almost everything relevant to this series. The interesting part of software, for our purposes, is not the code sitting on disk. It is what happens the moment that code starts running and begins doing work on someone's behalf.

Consider something as simple as loading a shopping website to buy a pair of shoes. From your perspective, you click a link, wait a moment, and see a page. From the perspective of the systems involved, a great deal happens in that moment. Your web browser sends a request — a formal message asking for something — out over the internet. That request typically does not travel to one single computer that does everything. Instead, in almost any modern web application of meaningful size, it lands first at something like a load balancer, a piece of infrastructure whose only job is to decide which of several available machines should handle this particular request. From there it might go to a web server that handles the basic mechanics of receiving your request and sending back a response. That web server, in turn, usually calls out to other pieces of the system to do the actual work: perhaps an inventory service to check whether the shoes are in stock, a pricing service to calculate the correct price including any discounts, a recommendation service to decide what other products to show you, and a database, which is a specialized system for reliably storing and retrieving information, to fetch the actual product details.

Each of those pieces — the load balancer, the web server, the inventory service, the pricing service, the recommendation service, the database — is typically what the industry calls a service: a distinct, independently running piece of software with a specific job, often built and maintained by a different team, sometimes running on entirely different physical or virtual machines, frequently written in a different programming language than its neighbors. A single request from you, the shopper, might touch five, ten, or in genuinely large systems, dozens of these services before a response makes its way back to your browser. This overall pattern, in which an application is built from many small, independently deployable services that communicate with each other over a network rather than as one single unified program, is called a distributed system, and it is now the default architecture for almost any web application, mobile app backend, or cloud service built at meaningful scale.

We are going to use a running analogy throughout this series to keep this concrete, because distributed systems are genuinely abstract and analogies help enormously. Picture a busy restaurant kitchen during dinner service. A customer's order — your request — arrives at the kitchen. It does not get cooked by one person from raw ingredients to finished plate. Instead, it passes through a sequence of stations: someone takes the order, someone at the grill station cooks the protein, someone at the salad station prepares the vegetables, someone plates the finished dish, and an expediter checks it before it goes out to the table. Each station is analogous to a service. The order itself, as it moves from station to station, is analogous to your request as it moves from service to service. And crucially, if the finished plate arrives at the table twenty minutes late, or wrong, or not at all, the customer does not know which station caused the problem. Neither, in a sufficiently busy and chaotic kitchen, does the head chef, unless someone made a point of tracking which station held the ticket for how long.

That last sentence is the entire motivating problem of this series, restated once more, plainly: when something goes wrong in a system built from many communicating pieces, understanding which piece is responsible, and why, is often extraordinarily difficult — unless the system was deliberately built to make that understanding possible.

## Part 3: Two Words That Sound Similar But Are Not: Monitoring and Observability

The two central terms in this field are monitoring and observability, and they are frequently used loosely, sometimes interchangeably, by people who should know better. For this series, we are going to be precise about them, because the distinction matters for everything that follows.

Monitoring is the practice of watching specific, predetermined signals and being alerted when they cross a threshold you decided on in advance. If you set up a system to alert you whenever a web server's error rate exceeds five percent, or whenever a database's disk space drops below ten percent free, you are monitoring. Monitoring answers questions you thought to ask before the problem happened. It is enormously valuable, and no serious operation runs without it, but it has an inherent limitation: it can only tell you about the things you anticipated might go wrong.

Observability is a broader and newer idea, and the term itself was not invented by the software industry. It was borrowed from a much older field: control theory, the branch of engineering concerned with how to steer and stabilize dynamic systems, from thermostats to spacecraft. In that field, a system is called observable if you can determine its complete internal state purely by examining its external outputs, without needing to see inside it directly. The software industry adopted the word because it captured something monitoring alone did not: the ability to ask a new question about a system's behavior, one you did not think of in advance, and get an answer using data the system already produces — without having to ship new code, deploy a new version, and wait for the problem to happen again just so you can watch for it properly this time.

Put more concretely: with good monitoring, you might know that checkout error rates spiked at 3:47 in the morning. With good observability, when that alert fires, you can immediately ask an unanticipated follow-up question — "was this spike concentrated among customers using a particular payment method, in a particular region, on a particular version of the mobile app?" — and get a real answer from data you already have, in minutes, without waiting for the problem to recur while you scramble to add new logging for exactly this scenario.

It is worth being honest here, in the spirit of skepticism this series intends to maintain throughout: "observability" has also become something of a marketing word in the last several years, applied loosely to almost any product that touches logs, metrics, or dashboards. Not every tool that calls itself an observability platform actually delivers the ability to ask arbitrary new questions of existing data; some simply monitor a fixed set of things more attractively than their predecessors did. Part of the value of this series, and specifically the recurring "how to spot the lie" sections we will build into every post, is giving you the tools to tell the difference between genuine observability and observability-flavored marketing. We take that seriously enough that post nineteen of this series is dedicated entirely to it.

## Part 4: The Three Classic Signals, Introduced Gently

The software industry has, over several decades, converged on three main categories of data that, together, tend to make a system observable in the sense just described. This series will spend an entire day on each of the three in Parts 6, 8, and 9 — this section is only a preview, enough to make the rest of this post make sense.

The first is the log. A log is a timestamped record of a discrete event: "at this moment, this thing happened." In our kitchen analogy, a log is the handwritten note a station cook might jot down: "11:47 PM, ran out of the salmon, told the expediter." Logs are excellent for capturing rich, specific, human-readable detail about a single occurrence, and they are the oldest form of this kind of data by a wide margin — programmers have been printing messages about what their code is doing since the earliest days of computing.

The second is the metric. A metric is a number that describes some aspect of a system's behavior, tracked over time, usually in aggregate. Not "the salmon ran out at 11:47," but "we have sold forty-two salmon dishes tonight," or "the grill station currently has six tickets waiting." Metrics are compact and cheap to store even at very high volumes, which makes them well suited to constant, ongoing measurement — you would not want a full written log entry every single time a single plate left the kitchen, but a running count is trivial to maintain.

The third, and the newest of the three in its modern form, is the trace, more specifically the distributed trace. A trace follows one specific request — one specific customer's order — as it moves through every station it touches, recording how long it spent at each one and in what order. This is precisely the kitchen ticket that follows the order from station to station, with each station's start and finish time stamped on it. A trace is what would have let the head chef in our restaurant look at a single late order and say, immediately, "this ticket sat at the grill station for eleven minutes because the grill was backed up," rather than guessing.

Here is a small, deliberately simplified illustration of what each of these three signal types might actually look like in raw form, for the very same underlying event — a slow checkout request. We will return to fuller, more realistic versions of these throughout the series; for now, the point is simply to see the shape of each one side by side.

A log entry for this event might look like this:

```json
{
  "timestamp": "2026-08-04T03:47:12.881Z",
  "severity": "ERROR",
  "message": "Checkout failed: payment gateway timeout after 8000ms",
  "service": "checkout-service",
  "order_id": "ORD-88213"
}
```

A metric data point describing the same general category of event, aggregated with many others, might look like this:

```json
{
  "name": "checkout.duration.milliseconds",
  "value": 8412,
  "timestamp": "2026-08-04T03:47:12.881Z",
  "attributes": {
    "service": "checkout-service",
    "outcome": "failure"
  }
}
```

And a fragment of a trace describing this one specific request's journey might look like this:

```json
{
  "trace_id": "4bf92f3577b34da6a3ce929d0e0e4736",
  "spans": [
    { "name": "http.request", "service": "web-frontend", "duration_ms": 8600 },
    { "name": "checkout.process", "service": "checkout-service", "duration_ms": 8480 },
    { "name": "payment.charge", "service": "payment-gateway-client", "duration_ms": 8412 }
  ]
}
```

Even without any further explanation, a pattern should already be visible. The log tells you something specific happened and roughly why. The metric tells you how often events like this are happening, in a form cheap enough to keep forever. The trace tells you precisely where, within this one journey, the time actually went — in this case, almost the entire eight and a half seconds of an unacceptably slow checkout was consumed inside the call to the payment gateway, not in the web frontend or the checkout service's own logic. That is exactly the kind of answer a good trace exists to provide, and exactly the kind of answer neither the log nor the metric could give you on their own with the same precision.

The following table summarizes the comparison we have just walked through, and is worth returning to as later posts get more technical.

| Signal | Answers the question | Kitchen equivalent | Typical volume | Cheapest to store at scale |
|---|---|---|---|---|
| Log | What exactly happened, and why? | The cook's handwritten note | High detail, moderate volume | No |
| Metric | How much, how often, and is it trending? | The running tally on the wall | Compact, very high volume tolerated | Yes |
| Trace | Where, specifically, in this one journey, did the time go? | The ticket that follows the order | Detailed per request, expensive at full volume | No |

## Part 5: What It Costs to Be Blind

It is tempting, in a series like this one, to lean on frightening statistics about the dollar cost of downtime, and the industry does produce a steady stream of such figures, usually from surveys commissioned by companies that sell products meant to reduce downtime. We are going to be deliberately cautious about those numbers here, and we will keep being cautious about vendor-supplied statistics throughout this series, because a company selling you observability tooling has an obvious incentive to make the cost of not having observability tooling sound as large as possible. That does not make every such figure false, but it does mean the reader should treat any specific dollar-per-minute claim with the same skepticism you would apply to a salesperson quoting the cost of the problem their product happens to solve.

What we can say with more confidence, because it follows directly from the Facebook example already discussed and from countless smaller, less famous incidents that happen inside ordinary companies every week, is this: the cost of an outage is not simply the revenue lost while the system is down. It includes the engineering time spent trying to figure out what is wrong in the first place, which in a genuinely opaque system can dwarf the time spent actually fixing the problem once it is understood. It includes what some engineering teams have started calling, half-jokingly, "mean time to innocence" — the time a team spends proving that the problem originated somewhere other than their own service, rather than time spent actually locating and fixing the root cause. In a large organization with dozens of teams each responsible for one small service in a much larger system, a meaningful fraction of incident response time can be consumed by teams pointing at each other's dashboards, each convinced the fault lies elsewhere, precisely because no shared, trustworthy view exists that could settle the question quickly.

It also includes a less visible but very real cost: the accumulated fatigue and eroded trust that comes from repeatedly failing to find the cause of a problem, or finding it only after an unacceptably long and stressful investigation. Teams that live through this repeatedly tend to develop a kind of institutional dread around their own systems — a sense that the software works most of the time for reasons nobody fully understands, and breaks occasionally for reasons nobody can reliably diagnose. That is a genuinely unpleasant way to build and operate software, and it is corrosive to an engineering organization over time in ways that are real even though they resist being reduced to a single dollar figure.

## Part 6: What It Costs to See

We would be doing this series a disservice, and doing you a disservice as a reader, if we presented observability as a free lunch. It is not, and the strong, sincere case against over-investing in it — including specific dollar and complexity costs — is the entire subject of Part 18 of this series, more than two weeks from now. We flag it here deliberately, early, so that nothing in the next seventeen posts is read as uncritical advocacy.

The short preview is this: every log line written, every metric point recorded, and every trace span captured has to be generated, which costs a small amount of computing time; transmitted, which costs network bandwidth; and typically stored somewhere for at least some period, which costs money, sometimes a great deal of money at genuine scale. Beyond the direct infrastructure cost, there is the human cost: someone has to build the pipeline that carries this data from where it is produced to where it can be examined, someone has to maintain that pipeline as the system it observes changes shape, and everyone has to learn how to actually use the resulting data well, which is its own nontrivial skill. A system that is drowning in telemetry nobody knows how to read is not meaningfully more observable than a system with none at all; it has simply traded one kind of blindness for a different kind of overwhelm.

This distinction — between collecting data and being able to use it to answer questions — is important enough that we are going to name it explicitly and return to it repeatedly across this series: inputs are not outcomes. Generating an enormous volume of telemetry is an input. Being able to quickly and correctly answer "why did this fail" during an actual incident is the outcome that telemetry is supposed to serve. A team can invest heavily in the first without meaningfully achieving the second, and a genuinely mature observability practice is judged by the second, not the first.

## Part 7: How to Spot the Lie — "We Have Full Visibility"

Each post in this series will end with a section like this one, applying a specific, repeatable test to a specific kind of claim you are likely to encounter, whether from a vendor's sales presentation, a colleague's status update, or your own organization's internal dashboards. This is the first of twenty, and it addresses perhaps the most common overclaim in the entire field: the assertion that a team or a product delivers "full visibility" or "complete observability" into a system.

Treat this phrase, on its own, as close to meaningless, for a simple reason: no system of any real complexity is ever completely visible, and anyone with hands-on experience running one knows this. There is always a boundary somewhere — a third-party service you depend on but cannot instrument, a piece of legacy infrastructure nobody has touched in years, a failure mode nobody anticipated and therefore nobody built a signal to detect, exactly as Facebook's own recovery tooling turned out to depend on the very network segment that had failed. "Full visibility" is not a factual claim so much as a confidence claim, and confidence is cheap to project in a sales deck or a status meeting.

A more useful question, and one you should get comfortable asking, is not "do we have full visibility" but "what specifically can we currently see, and what specifically can we not see?" A team or a vendor that can answer the second version of that question with precision — naming particular gaps, particular blind spots, particular categories of failure their current setup would not catch — is demonstrating real understanding of their own system. A team or a vendor that only offers the first, sweeping version, without being able to name a single concrete limitation, is very often telling you less than it sounds like. We will return to variations of this same test — replacing a vague, confident claim with a specific, falsifiable one — throughout this series, because it is close to the single most useful habit a skeptical reader of this material can develop.

## Part 8: Where This Series Is Going

This first post has deliberately stayed at the level of concepts and vocabulary, because everything that follows depends on the reader sharing a precise understanding of what "monitoring," "observability," "log," "metric," and "trace" actually mean, and of why the problem this series addresses is real and not manufactured by an industry looking to sell tooling.

Tomorrow's post goes backward in time, before OpenTelemetry existed at all, to trace the genuine history of how the software industry tried to solve this problem across four decades — from the earliest system logs in the 1980s, through the rise of dedicated metrics systems in the 2000s and 2010s, to the invention of distributed tracing at companies like Google and Twitter in the years after 2010. Understanding that history matters, because OpenTelemetry, which the rest of this series is about, was not invented in a vacuum. It was a direct response to a specific, painful problem that this earlier era created: a landscape of incompatible, proprietary tools that made switching from one vendor to another astonishingly expensive. Post 3, the day after, tells the exact story of how two competing open-source projects merged in 2019 to create the standard this series is named after, and names the specific people and companies whose incentives drove that merger.

From there, the series builds upward from data-model first principles — traces, then context propagation, then metrics, then logs, each in exhaustive detail with worked examples — before moving into the concrete architecture of OpenTelemetry itself, hands-on code in C# and ASP.NET Core, and finally the honest, unglamorous questions of cost, deception, governance, and what comes next. Twenty days, one careful step at a time, with no assumed prior knowledge and no unexamined vendor claims.

## Sources and Further Reading

- Meta Engineering, ["More details about the October 4 outage,"](https://engineering.fb.com/2021/10/05/networking-traffic/outage-details/) October 5, 2021 — Facebook's own engineering account of the root cause and the role of the DNS/BGP safety mechanism.
- Cloudflare Blog, ["Understanding How Facebook Disappeared from the Internet,"](https://blog.cloudflare.com/october-2021-facebook-outage/) October 4, 2021 — an outside, real-time technical analysis of the BGP route withdrawals as observed by a separate network operator.
- ThousandEyes, ["Facebook Outage Analysis,"](https://www.thousandeyes.com/blog/facebook-outage-analysis) October 2021 — independent third-party measurement of the outage's timeline and scope.
- OpenTelemetry Project, ["What is OpenTelemetry,"](https://opentelemetry.io/docs/what-is-opentelemetry/) opentelemetry.io — the project's own framing of the problem it addresses, useful to compare against the account given here.
- Charity Majors, Liz Fong-Jones, and George Miranda, *Observability Engineering* (O'Reilly Media) — a widely cited industry text on the distinction between monitoring and observability; readers should note the authors are also founders of an observability vendor, and read accordingly.

*If you are experiencing a live production incident right now and arrived at this article while searching for help, this series will not save you tonight — but Part 13, on the OpenTelemetry Collector, and Part 17, on backends, will be directly useful once the immediate fire is out.*
