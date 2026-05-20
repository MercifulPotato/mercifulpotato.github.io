---
title: "The Shape of a Crowd: Distributions, Bell Curves, and Why Your Average Is Lying to You"
date: 2026-06-16
author: mercifulpotato-team
summary: "The second installment of our plain-English statistics series: what distributions are and why they matter, the normal distribution explained through height and test scores, skewed distributions and the gap between averages and medians, and the bimodal distribution as evidence of hidden groups within a population."
tags:
  - statistics
  - distributions
  - normal-distribution
  - plain-english
  - data-literacy
  - decision-making
series: "Probability and Statistics in Plain English"
---

## Part 1: What a Distribution Actually Is

Suppose you run a community center and you need to order chairs for a large event. You know that the chairs will hold people of many different heights. What height should you design around? You cannot order chairs that are perfect for one specific person if the people who will use them range from five feet to six feet four inches. You need to understand the spread of heights, not just the extremes and not just the average. You need to know the shape of the data.

This, in essence, is what a distribution tells you. A distribution is a picture — either literal or conceptual — of how a set of measurements is spread out. It answers not just "what is typical?" but "how typical is typical?", "how far do the unusual cases go?", and "is there a pattern to the spread itself?"

Every time someone collects data on a population — measuring heights, income levels, blood pressure readings, response times in a software system, error rates in a manufacturing process, scores on a standardized test — they are implicitly generating a distribution. The distribution is simply the full accounting of all the values that were measured, organized in a way that reveals their pattern.

The practical importance of distributions is immense. Summary statistics like "the average" or "the median" collapse a distribution down to a single number, which is convenient but which destroys information. That destruction of information is often harmless. Sometimes it is catastrophically misleading. The only way to know the difference is to understand what distributions look like and what they can hide.

We will spend this entire installment on three shapes that appear constantly in the real world and that every statistically literate person should be able to recognize and reason about: the normal distribution (the bell curve), the skewed distribution, and the bimodal distribution.

---

## Part 2: The Bell Curve — The Shape That Shows Up Everywhere

The bell curve is the single most famous shape in all of statistics, famous enough to have its name used as a synonym for normal distribution in ordinary conversation. You have seen pictures of it: a smooth, symmetrical, rounded shape that rises to a peak in the middle and tapers off gradually on both sides, like a cross-section of a very gently domed hill. It is called the normal distribution because it appears so frequently in natural measurements that early statisticians began to call it "normal" — the expected, natural, ordinary shape.

To understand why it appears so often, think about height in adult men. Imagine measuring the height of ten thousand adult men chosen at random from the United States. A few will be extremely short — under five feet. A few will be extremely tall — over six feet five. But the vast majority will cluster in a range from roughly five feet six to six feet one, and the most common heights will be somewhere around five feet nine or five feet ten. The distribution of heights will look, when drawn, like a bell: a high hump in the middle (most men are near the average) that slopes downward on both sides (fewer and fewer men as you move toward the extremes).

The bell shape emerges any time a measured characteristic is the result of many small, independent contributing factors, none of which dominates the others. Human height is influenced by dozens of genes, early nutrition, overall health during childhood, and various environmental factors. None of these alone determines your height; all of them nudge it slightly in one direction or another. The result of all those tiny nudges, none of them dominant, is a bell-shaped distribution.

### What the Bell Curve Tells You

The practical power of the bell curve comes from what it implies about where most measurements will fall. Because of the bell shape's specific mathematical properties — and again, we are not doing math here, we are reasoning about shapes — roughly two-thirds of all measurements in a normal distribution will fall within one "standard step" of the center in either direction. About ninety-five percent will fall within two standard steps. About ninety-nine percent will fall within three standard steps.

We will define "standard step" more carefully in a later installment when we discuss standard deviation. For now, just hold the shape in mind: the bell curve tells you that measurements cluster in the middle, with the density of clustering decreasing smoothly and symmetrically as you move outward in either direction. Extreme values become exponentially rarer. If the average height is five feet ten inches, a man of six feet four is already quite far out in the tail. A man of six feet ten is extremely rare. A man of seven feet two is so rare as to be almost one in a million.

### How to Spot a Bell Curve in the Wild

You do not need to draw a histogram or calculate anything to have a general intuition about whether data might be normally distributed. Ask: is there a natural "typical" value that most measurements cluster around? Are values above average roughly as common as values below average? Are extreme values equally rare on both sides?

Test scores on well-designed standardized tests are approximately normally distributed — the test is specifically designed to produce a spread of scores around a meaningful center. Blood pressure readings in a healthy adult population are approximately normally distributed. Measurement errors in precision manufacturing are normally distributed — small errors are common, large errors are rare, and errors are equally likely to go in either direction. Response times in a well-tuned web application cluster around a typical value with smaller and larger values on either side.

```python
# Conceptual simulation: what a normal distribution "looks like" in raw data
import random

def simulate_height_inches():
    """
    Simulate adult male height by summing many small genetic/environmental
    nudges. This mimics why heights are normally distributed.
    Each factor nudges height by a small random amount in either direction.
    """
    base_height = 70.0  # 5'10" in inches
    # 20 independent small factors, each nudging +/- 1.5 inches
    nudges = [random.uniform(-1.5, 1.5) for _ in range(20)]
    return base_height + sum(nudges) / len(nudges) * 3.0

heights = [simulate_height_inches() for _ in range(10_000)]

# Quick summary without any statistics library
heights.sort()
mean_height = sum(heights) / len(heights)
median_height = heights[len(heights) // 2]

print(f"Mean height:   {mean_height:.1f} inches ({mean_height/12:.1f} feet)")
print(f"Median height: {median_height:.1f} inches ({median_height/12:.1f} feet)")
# In a normal distribution, mean and median are very close together
# because the distribution is symmetric.
```

Notice in the code above that the mean and the median are very close to each other. This near-equality is a signature of a symmetric distribution. The mean is pulled toward wherever the data is concentrated; when the data is symmetrically concentrated around a center, the mean and median land in the same place. When the data is not symmetric — when it is skewed — the mean and median diverge. This divergence is one of the most useful diagnostic signals in all of practical statistics.

---

## Part 3: The Skewed Distribution — When the Average Lies

Suppose you are trying to understand the economic health of a neighborhood. You ask a researcher to tell you "the average household income" in that neighborhood. The researcher reports back: average household income is ninety thousand dollars per year.

But what if you walked through that neighborhood? What if you talked to the people there? You might find that most families are making thirty, forty, fifty thousand dollars — working-class to middle-class incomes. The neighborhood does not feel like a ninety-thousand-dollar neighborhood at all. What happened?

What happened is that the neighborhood is also home to several extremely wealthy families. One or two households with annual incomes of five or ten million dollars will dramatically inflate the average income for the entire neighborhood, even if every other household earns far less. The "average" of ninety thousand dollars is a mathematical truth, but it misrepresents the experience of almost everyone who lives there. This is a skewed distribution.

### Right Skew: The Long Tail to the Right

A right-skewed distribution — also called positively skewed — is one where most values cluster at the lower end and a smaller number of values stretch out to very high extremes. Draw it out: a tall hump on the left side, trailing off into a long, thin tail extending to the right.

Income is the paradigmatic example. In nearly every economy on earth, most people earn modest to moderate amounts, while a small number of people earn extraordinarily large amounts. The income distribution has a long tail extending to the right representing the extremely wealthy. When income distributions are described using means, the mean is pulled rightward by those extreme high values and overestimates what a typical person earns.

Home prices, asset values, the lifespans of software systems, the sizes of cities, the number of social media followers — all of these tend to be right-skewed. There are very many small values and very few extremely large ones, and the very large ones drag the mean far above what most individual values actually are.

### Left Skew: The Long Tail to the Left

Left-skewed distributions — negatively skewed — are less common in everyday experience but worth understanding. Here most values cluster at the high end and a smaller number trail off toward very low extremes. Draw it: a tall hump on the right, with a thin tail extending to the left.

The age at death in a modern high-income country is somewhat left-skewed. Most people live into old age, dying in their seventies, eighties, or nineties. But a smaller number die very young — in infancy, childhood, or early adulthood — pulling the tail to the left. The "most common" age at death (the mode) is somewhere in the seventies or eighties; the mean age at death is pulled leftward by the early deaths.

Exam scores in a course where the exam is very easy can be left-skewed: most students score near the top, and a smaller number of struggling students score much lower, stretching the tail toward the low end.

### The Mean, Median, and Mode — And Which to Use

A normal distribution has all three of these — mean, median, and mode — piled on top of each other in the center. The mean is the numerical average. The median is the middle value when you sort all measurements from lowest to highest. The mode is the most frequently occurring value.

In a skewed distribution, these three measures of "center" drift apart in a diagnostic way.

For a right-skewed distribution: the mean is pulled toward the high extreme and is therefore the largest of the three. The median is in the middle — it represents the point where half of all values are above and half are below, and it is pulled less severely by extreme values. The mode is the most common value and sits closest to the peak of the hump, which is toward the low end.

For a left-skewed distribution: these positions reverse. The mean is pulled toward the low extreme. The mode sits closest to the high-end hump.

This has immediate practical implications:

| Situation | Better Measure of Center | Why |
|---|---|---|
| Average home price in a neighborhood | Median | Extreme luxury homes distort the mean |
| Average salary in a company | Median | Executive salaries distort the mean |
| Average test score in an easy exam | Mean or median | Left skew is modest; both are reasonable |
| Average download speed in a region | Median | Occasional very slow connections distort the mean |
| Average human lifespan | Median | Early deaths distort the mean in developing countries |

When a politician, advertiser, or analyst uses "average" without specifying whether they mean mean or median, always ask which one they used, and then ask whether the data might be skewed in a direction that makes that choice misleading.

```python
# Demonstrating the mean vs. median gap in skewed data
incomes = [
    35_000, 40_000, 42_000, 45_000, 47_000, 48_000, 50_000,
    52_000, 55_000, 58_000, 60_000, 65_000, 70_000, 75_000,
    85_000, 90_000, 100_000, 120_000,
    # Two extremely wealthy residents:
    2_500_000, 8_000_000
]

incomes.sort()
n = len(incomes)
mean_income = sum(incomes) / n
median_income = (incomes[n // 2 - 1] + incomes[n // 2]) / 2

print(f"Number of households: {n}")
print(f"Mean income:          ${mean_income:,.0f}")
print(f"Median income:        ${median_income:,.0f}")

# Count how many households earn LESS than the mean:
below_mean = sum(1 for i in incomes if i < mean_income)
print(f"Households earning below the mean: {below_mean} out of {n}")
# Most households earn less than the "average" due to right skew
```

Running this code would reveal something striking: the mean income would be somewhere around six hundred thousand dollars, yet eighteen out of twenty households earn less than that. The "average" household in this scenario is not a useful description of any real household's experience. Reporting this neighborhood's income as "average household income: six hundred thousand dollars" would be technically accurate and practically dishonest.

---

## Part 4: The Bimodal Distribution — Hidden Worlds Within Your Data

A bimodal distribution is one with two peaks rather than one. Draw it: not a single bell but two bells, side by side, each with its own hump. When you see this shape — or have reason to suspect it — it is almost always telling you that your data contains two distinct groups that you have mixed together, and that those groups are fundamentally different from each other in the characteristic you are measuring.

### The Classic Example: Heights in a Mixed Population

If you measure the heights of adults in a room and plot them, you will probably see a roughly normal distribution. But if you then separate the plot by sex — plotting male heights and female heights separately — you will find that what looked like one distribution was actually two overlapping distributions. Adult women in the United States have an average height of roughly five feet four inches. Adult men have an average of roughly five feet nine to five feet ten. When you mix them together, you get a combined distribution that has two humps — one centered around female average height and one around male average height — which may look like a broad, slightly lumpy bell curve or, if the groups are more separated, like a clearly bimodal shape.

The practical lesson: if your distribution has two peaks, it is your data telling you "I contain two populations." Ignoring this and computing a single average for the combined population produces an average that may not represent either group well.

### Bimodal Distributions in Software Performance

A software engineer monitoring response times for a web service might collect data and find a bimodal distribution. Instead of one hump centered around, say, two hundred milliseconds, they find two humps: one around one hundred milliseconds and another around eight hundred milliseconds. This is not noise. This is signal. It is saying: there are two kinds of requests being served, and they perform very differently. Perhaps some requests are being served from a fast in-memory cache and others are hitting a slow database. Perhaps some users are on fast connections and others are on slow ones. Perhaps two code paths with radically different performance characteristics are both being counted in the same metric.

Finding this bimodal pattern — and then investigating why it exists — leads directly to the root cause of a performance problem. Computing the average response time and monitoring whether the average stays in range would completely miss this structure. The average might be pleasantly low (the fast requests drag it down) even while a substantial fraction of users experience very slow responses.

```python
# Simulating a bimodal response time distribution
import random

def simulate_response_times(n_requests=1_000):
    """
    60% of requests are served from cache (fast: ~100ms)
    40% are database queries (slow: ~800ms)
    Each has some natural variation.
    """
    times = []
    for _ in range(n_requests):
        if random.random() < 0.60:
            # Cache hit: normally distributed around 100ms
            t = max(10, random.gauss(100, 25))
        else:
            # DB query: normally distributed around 800ms
            t = max(100, random.gauss(800, 150))
        times.append(t)
    return times

times = simulate_response_times()

mean_time = sum(times) / len(times)
sorted_times = sorted(times)
median_time = sorted_times[len(sorted_times) // 2]
p95_time = sorted_times[int(0.95 * len(sorted_times))]  # 95th percentile
p99_time = sorted_times[int(0.99 * len(sorted_times))]  # 99th percentile

print(f"Mean response time:   {mean_time:.0f}ms")
print(f"Median response time: {median_time:.0f}ms")
print(f"95th percentile:      {p95_time:.0f}ms")
print(f"99th percentile:      {p99_time:.0f}ms")

# How many requests exceed 500ms?
slow_requests = sum(1 for t in times if t > 500)
print(f"Requests > 500ms:     {slow_requests} of {len(times)} ({100*slow_requests/len(times):.1f}%)")
```

The code above introduces two additional summary statistics that matter enormously in performance engineering: the ninety-fifth percentile and the ninety-ninth percentile. Rather than asking "what is the average?", percentile metrics ask "what does the slowest five percent of requests experience?" or "what does the slowest one percent experience?" These are called tail latencies, and they are often far more important to end-user experience than the average.

A web service with an average response time of three hundred milliseconds might sound acceptable. But if the ninety-ninth percentile is five seconds, then one in a hundred users is waiting five seconds for a page to load. If you have ten thousand users per minute, that is one hundred people per minute having a terrible experience. The average hides this. The percentile reveals it.

This is not an academic point. Amazon has published research showing that every one hundred milliseconds of additional page load time correlates with a one percent decrease in sales. Latency matters, and latency cannot be understood from averages alone.

---

## Part 5: The Uniform Distribution — When Everything Is Equally Likely

Before moving on, it is worth mentioning the uniform distribution because it is the conceptual opposite of the normal distribution and appears in contexts that matter. A uniform distribution is one where every value in a range is equally likely. There are no peaks, no tails — just a flat, level surface.

The result of a truly fair die roll is uniform: each of the six faces has exactly a one-in-six chance. The outcome of a fair lottery pick from a set of numbered balls is uniform. A random number generator that produces numbers from zero to one, if it is truly random, produces a uniform distribution.

Uniform distributions almost never occur naturally in measurements of real-world phenomena. When you see something that looks uniform where you expected a bell curve — or when you see a bell curve where you expected something uniform — it is worth investigating. Security researchers who encounter computer-generated data that looks suspiciously "too random" — too uniformly distributed — sometimes interpret that as a sign that the data was fabricated rather than actually collected.

---

## Part 6: Thinking About Tails — When Rare Events Are Not That Rare

One of the most practically important consequences of understanding distributions is the insight that even events in the far tails of a distribution — events labeled "extremely unlikely" — happen regularly when the underlying population is large enough.

The stock market crash of 2008 was described by some quants at the time as a "twenty-five sigma event" — meaning it fell twenty-five standard deviations from the normal, which, under a true normal distribution, would be so rare it should not occur once in the lifetime of the universe. But it happened. And the 1987 crash happened. And the dot-com collapse happened. And numerous smaller market disruptions happen regularly.

The reason is that financial market returns are not normally distributed. They have "fat tails" — extreme events are far more common than the normal distribution predicts. The tails of the real distribution are fatter than the tails of the assumed distribution. This mismatch between the assumed model and the actual data caused catastrophically overconfident risk calculations and contributed directly to the financial crisis.

This is called tail risk in finance, and the lesson generalizes: the shape of your distribution matters enormously for estimating the frequency of extreme events. Assuming normal distribution when the actual distribution has fatter tails means you are systematically underestimating how often bad things happen. If your safety margins are calculated on the assumption that catastrophic failures are one-in-a-billion events, but the actual distribution puts them at one-in-a-thousand events, you are off by a factor of a million in your risk assessment.

Insurance companies, nuclear plant designers, infrastructure engineers, and financial risk managers all have to grapple with tail risk — the probability of rare but catastrophic outcomes. The shape of the distribution is not just a technical detail in these contexts. It is life and death and economic survival.

---

## Part 7: Practical Applications for Specific Professions

### For Voters

When you read about "average household income" in your city or congressional district, ask whether the data is skewed. If it is right-skewed (which income almost always is), the median is more representative of typical households than the mean. A politician who says "the average income in this district is eighty thousand dollars" may be technically correct while describing an economic reality that applies to only a minority of constituents. Ask for the median.

When you read about "average life expectancy" in different countries or demographic groups, remember that life expectancy statistics are themselves complex summaries. A country where many children die young will have a low average life expectancy even if adults who survive childhood tend to live long lives. The shape of the mortality distribution matters.

### For Software Developers

Response time metrics should almost never be reported as averages alone. Always instrument your system to capture at minimum the fifty-fifth, ninety-fifth, and ninety-ninth percentiles. When you see a bimodal response time distribution, stop what you are doing and find out why. It always has a cause.

Likewise, error rate distributions across user sessions often hide multi-modal structure. If some users experience near-zero errors and another cluster experiences very high errors, the average error rate is a useless number. Segment your users, segment your errors, and look at the shapes.

### For Accountants and Auditors

Transaction size distributions in normal business operations are typically right-skewed: many small transactions and fewer very large ones. An audit technique called Benford's Law (which we will explore more in Part 5 of this series) exploits the specific expected shape of transaction digit distributions to detect fraud. When the actual distribution of first digits in transaction amounts does not match the expected distribution, it is a red flag for potential manipulation.

Expense distributions within categories should also follow predictable shapes. If employee meal expense claims follow a normal distribution clustering just below the reimbursement cap, that is an anomalous pattern suggesting strategic rounding to the cap rather than genuine expense reporting.

### For Executives and Decision-Makers

When reviewing performance data — sales figures, customer satisfaction scores, production output — always ask for distribution shapes, not just averages. A product line with an average satisfaction score of seven out of ten might be performing acceptably. But if the distribution is bimodal — with large clusters of both very satisfied customers (nines and tens) and very dissatisfied customers (ones and twos) — the average score is hiding a crisis in one segment of your customer base.

Revenue forecasts built on normal distribution assumptions about market conditions need to be stress-tested for fat-tail scenarios. What happens if outcomes fall in the far left tail — the catastrophically bad outcomes that normal distribution models say happen once per century but which empirically seem to happen once per decade?

---

## Part 8: How Bad Actors Use Distribution Ignorance Against You

Understanding distributions also helps you recognize when someone is using distribution manipulation to mislead you.

**Cherry-picking the summary statistic.** A company that claims "our average customer saves over three hundred dollars per year" might be using a right-skewed distribution where a small number of heavy users save thousands while the majority save very little. Asking for the median savings would tell a very different story.

**Hiding bimodal structure.** When a drug trial reports average improvement in symptoms, ask whether all patients improved equally or whether some improved dramatically and others showed no improvement at all. If the distribution is bimodal — with a cluster of responders and a cluster of non-responders — the average improvement conceals the critical clinical question: who responds to this treatment, and who does not? A "promising" average can hide a treatment that helps only a small subset of patients.

**Averaging across incompatible populations.** A school district that reports "average student performance" across wildly different schools — high-performing schools with well-resourced students and struggling schools serving high-poverty populations — is averaging across populations that are not comparable. The resulting average describes nobody's actual experience accurately.

---

## Part 9: What Comes Next

We have established the three most important distribution shapes and developed practical intuitions for what they tell us and how they can be misused. Normal distributions tell us about phenomena where many small factors combine. Skewed distributions warn us that means can be deceiving and medians are often more informative. Bimodal distributions reveal hidden structure — hidden populations, hidden failure modes, hidden segments — that averages completely obscure.

In the third installment, we enter the territory that most people find most confusing about statistics: the machinery of proof. What does it mean to "prove" something statistically? What is a null hypothesis, and why does every scientific test have one? What is a p-value, and why has it been at the center of a methodological crisis in science? What is statistical significance, and how is it different from practical significance? What is the significance level, and who gets to choose it?

These are the questions that separate confident statistical reasoning from vulnerable statistical ignorance. They are also the questions whose answers will most directly equip you to evaluate the scientific claims you encounter every day — in medical research, in policy analysis, in software performance benchmarks, and in every news story that begins with "a new study shows."

We will cover all of it, and as always, we will do it without a single Greek letter or algebraic formula, using only the language of reasoning, comparison, and intuition that you already possess.

---

## Sources and Further Reading

- Wheelan, Charles. *Naked Statistics: Stripping the Dread from the Data*. W.W. Norton, 2013. Particularly strong on the practical meaning of distributions and why averages can mislead; written for a general audience with great humor and clarity.

- Taleb, Nassim Nicholas. *The Black Swan: The Impact of the Highly Improbable*. Random House, 2007. The definitive popular treatment of fat tails, extreme events, and why normal distribution assumptions cause catastrophic risk mispricing. Dense but essential.

- Spiegelhalter, David. *The Art of Statistics: How to Learn from Data*. Basic Books, 2019. Chapter 3 provides an excellent treatment of distributions with visual examples and practical guidance on choosing summary statistics.

- Kahneman, Daniel. *Thinking, Fast and Slow*. Farrar, Straus and Giroux, 2011. Chapters on regression to the mean and the representativeness heuristic are directly relevant to why intuitions about distributions fail.

- Amazon Research. Multiple publications on latency measurement, percentile monitoring, and the inadequacy of average response times for production systems — search "Amazon builder library latency" for current articles in their engineering documentation.

- Hald, Anders. *A History of Mathematical Statistics from 1750 to 1930*. Wiley, 1998. For readers who want the intellectual history of how the normal distribution was discovered and why its prevalence was initially misunderstood even by its discoverers.
