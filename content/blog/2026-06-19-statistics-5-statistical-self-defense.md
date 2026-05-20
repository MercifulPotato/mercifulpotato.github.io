---
title: "Statistical Self-Defense: How to Identify, Dissect, and Destroy Bad-Faith Arguments Hiding Behind Numbers"
date: 2026-06-19
author: mercifulpotato-team
summary: "The fifth installment of our plain-English statistics series: a comprehensive catalog of the specific techniques used to mislead with statistics — cherry-picking, misleading graphs, correlation confusion, survivorship bias, Goodhart's Law, and more — with the exact questions you can ask to expose every one of them."
tags:
  - statistics
  - statistical-manipulation
  - critical-thinking
  - plain-english
  - data-literacy
  - misinformation
  - self-defense
series: "Probability and Statistics in Plain English"
---

## Part 1: The Arsenal of Deception

Numbers carry a special authority in public discourse. When someone makes a claim and supports it with data, the data tends to shut down argument. Saying "I feel like this is true" invites challenge. Saying "studies show that this is true" often does not. The statistical dressing of an argument — the charts, the percentages, the confidence intervals — functions as armor against scrutiny.

This armor is exploited constantly. Not always by malicious actors, though malice is common enough. Sometimes the exploitation is the result of genuine innumeracy — researchers or journalists or executives who do not understand what their own numbers say and therefore misrepresent them without realizing it. Sometimes the exploitation is the result of motivated reasoning — people who believe what they want to believe and unconsciously select evidence and framing to support it. And sometimes the exploitation is deliberate: researchers who massage their analysis until they get the answer their funder wants, politicians who present selectively framed data to support predetermined conclusions, companies that bury unfavorable findings in dense technical language while amplifying favorable ones in press releases.

Regardless of the source, the patterns are remarkably consistent. There is a finite catalog of techniques used to mislead with statistics, and once you know the catalog, you can recognize the moves almost instantly. This installment is that catalog.

For each technique, we will give: what it is, a concrete example of it in action, the specific questions that expose it, and a note on how to spot the lie when you cannot immediately run the numbers yourself.

---

## Part 2: Cherry-Picking — The Selective Harvest of Truth

Cherry-picking is the practice of selecting only the data, time periods, or studies that support your conclusion while ignoring all the data, time periods, or studies that contradict it. Every individual piece of selected data may be accurate. The deception is in the selection.

### Example One: Stock Market Returns

A financial advisor shows you a chart of their fund's performance over the last three years. The chart shows strong returns, outpacing the overall market index. What the chart does not show you is that the fund was chosen from a large family of funds after seeing which ones had the best three-year performance, that the fund's ten-year performance is mediocre, and that the overall category of similar funds (the appropriate comparison baseline) outperformed this specific fund in seven of those ten years.

The three years shown are cherry-picked. The comparison baseline is cherry-picked. Every number on the chart may be accurate; the chart as a whole is misleading.

### Example Two: Scientific Literature

A pharmaceutical company publishes a meta-analysis showing that five studies support the efficacy of their drug. What the analysis does not mention is that there are eleven other published studies that found no effect, and a further seventeen unpublished negative studies sitting in file drawers because the company chose not to publish them.

This is not hypothetical. A 2008 analysis of antidepressant studies submitted to the Food and Drug Administration found that thirty-seven of thirty-eight positive studies were published, while only fourteen of thirty-six negative studies were — and some of those were published in a way that made them appear positive. The published literature gave a radically more favorable picture of the drugs than the totality of the evidence warranted.

### How to Spot the Lie

Ask: "What data was excluded?" Ask: "Over what time period is this measured, and why was that period chosen?" Ask: "Is this a comprehensive review of all studies, or a selection?" Ask: "Who commissioned this analysis, and what is their financial interest in the conclusion?"

A well-designed analysis will explicitly discuss its inclusion criteria — what data it included, what it excluded, and why. An analysis that does not discuss what was left out is a red flag.

---

## Part 3: Misleading Graphs — The Visual Manipulation Toolkit

Visual representations of data can be manipulated in ways that are technically accurate but perceptually deceptive. The human brain processes visual information quickly and often makes incorrect inferences about proportions and trends from poorly scaled or deliberately distorted charts.

### The Truncated Y-Axis

The most common graphical manipulation is truncating the vertical axis of a bar chart or line chart so that it starts at a value significantly above zero. This exaggerates the visual size of any difference between the compared values.

Suppose two products have customer satisfaction scores of seventy-two percent and seventy-five percent. If you draw a bar chart with a y-axis that starts at seventy percent, the second bar appears roughly four times as tall as the first, implying a massive difference. If you draw the same chart with a y-axis that starts at zero, both bars are nearly the same height, accurately conveying that the difference is small.

Both charts accurately represent seventy-two versus seventy-five. The choice of y-axis origin determines whether the viewer perceives a large or small difference.

### How to Spot the Lie

Look at the y-axis of every chart before interpreting the chart. Does it start at zero? If not, ask whether the truncation is justified. A truncated axis can sometimes be legitimate — when comparing small differences in measurements that all happen to be near a high value (like body temperature, which varies only in a narrow range around 98.6 degrees Fahrenheit) — but it must be presented transparently, not as the default presentation of a political or commercial argument.

### The Missing Baseline

A chart showing "our sales have grown by thirty percent this year" with no comparison to industry-average growth, competitor growth, or the growth trend of prior years provides only half the context needed for evaluation. If industry-wide sales grew by forty percent, a thirty percent increase represents underperformance. If industry sales fell by ten percent, it represents strong outperformance.

### The Moving Endpoint

A study that tracks health outcomes across a period and reports results through whatever endpoint makes the data look best — stopping just before a reversal in trend, or extending just long enough to include an anomalously favorable period — is manipulating the time-series framing. This is particularly common in investment performance claims.

### The Two Axes Problem

A chart with two variables on different y-axes can be made to show almost any relationship between those variables by choosing different scales for the two axes. If you scale one axis to be compressed and the other to be stretched, you can make the two lines appear to move together or in opposition in whatever way serves your narrative. Always ask whether two-axis charts are using comparable scales.

---

## Part 4: Correlation Versus Causation — The Confusion That Kills

The phrase "correlation does not imply causation" is one of the most repeated sentences in all of statistics education, which has paradoxically made it a phrase that people recite without understanding it. Let us actually unpack what it means and why it matters.

Two variables are correlated if they tend to change together. As one goes up, the other tends to go up too (positive correlation). Or as one goes up, the other tends to go down (negative correlation). Correlation is a purely mathematical relationship between two measured quantities — it says nothing whatsoever about whether one causes the other.

The reason this matters is that correlation is everywhere, and causal claims are what most arguments are actually about. When a politician says "the crime rate fell after we passed stricter gun laws," they are making a causal claim based on correlational observation. When an advertisement says "people who drink our juice live longer," they are making a causal implication from correlation. When a study says "people who exercise more have better cognitive performance in old age," the correlation is well-established — but whether exercise causes better cognition, or whether the same underlying health and genetic factors cause both more exercise and better cognition, requires more careful analysis.

### The Three Explanations for Correlation

When two variables are correlated, there are only three categories of explanation:

**Direct causation:** A causes B. Exercise does improve cognitive function through specific biological mechanisms. This is one explanation for the correlation — and it may be correct — but it is not the only one.

**Reverse causation:** B causes A. Perhaps people who have better cognitive function tend to exercise more, because they are more motivated, healthier, and more planful by nature. The causal arrow runs in the opposite direction from the obvious interpretation.

**Confounding:** A third variable, C, causes both A and B independently. Perhaps overall health status causes both greater exercise and better cognitive outcomes. In this case, the correlation between exercise and cognition is real but not directly causal — it is an artifact of a common underlying cause.

A correlation, on its own, cannot distinguish among these three explanations. This is why randomized controlled trials — experiments where participants are randomly assigned to receive or not receive the factor being studied — are the gold standard for establishing causation. When participants are randomly assigned, confounding variables are approximately equalized between groups, and reverse causation is ruled out by design. Neither survey data nor observational studies, no matter how large, can achieve this without special analytical techniques.

### Famous Spurious Correlations

The writer Tyler Vigen maintains a database of absurd but statistically real correlations, which illustrates the point vividly. The divorce rate in Maine correlates remarkably well with the per capita consumption of margarine. Nicolas Cage films released per year correlates with drowning deaths in swimming pools. These correlations are genuine in the data but completely without causal meaning. They are accidental patterns that exist because, in any large dataset, some variables will happen to move together by chance.

### How to Spot the Lie

When someone says "X is associated with Y" or "X predicts Y," ask: "What is the proposed causal mechanism?" A causal claim without a mechanism is just a correlation claim dressed up in causal language. Ask: "Could a third variable explain both?" Ask: "Could the causal arrow run in the opposite direction?" Ask: "Was this an observational study or a randomized experiment?"

If the evidence comes from an observational study and no experimental evidence exists, treat the claim as a hypothesis, not a established fact, regardless of how large the study was.

---

## Part 5: Survivorship Bias — The Cemetery of Invisible Evidence

Survivorship bias is the error of analyzing only the entities or cases that "survived" a selection process, while ignoring the entities that did not survive and were therefore excluded from the data. The survivors are not representative of the original population — they are a special, selected subset.

### The Classic Example: World War II Planes

During the Second World War, the statistician Abraham Wald was asked by the U.S. military to analyze damage patterns on aircraft returning from combat missions. The military wanted to add armor to the planes, but weight constraints meant they could only reinforce certain areas. Looking at the returning planes, the damage was concentrated in the fuselage and wings. The military's initial instinct was to reinforce those heavily damaged areas.

Wald pointed out the error: they were looking only at planes that returned. The planes that were shot down and never returned were not in the data. The fact that returning planes showed lots of damage to the fuselage and wings meant those were the areas that could sustain heavy damage and the plane would still make it home. The areas with little damage on returning planes were areas that, when hit, meant the plane did not return. Those were the areas that needed reinforcement.

By looking only at survivors, the military had inverted the lesson of the data.

### Survivorship Bias in Business

The popular myth that small businesses fail at fantastically high rates — "ninety percent fail in the first year" is a commonly cited figure, and it is false — is related to survivorship bias. The successful businesses are visible and numerous. They survive long enough to be counted, to be written about in business publications, to be cited as examples. The businesses that fail quickly often leave little trace. We see many twenty-year-old restaurants and few one-month-old restaurants, not necessarily because restaurants rarely fail in their first month, but because the failures leave less evidence.

More pernicious is the genre of business success literature. Every "lessons learned from successful companies" book implicitly conditions its analysis on companies being successful. If you study what habits successful CEOs have in common, you will find that many successful CEOs wake up at five in the morning, exercise regularly, and maintain rigorous daily routines. But many failed CEOs had identical habits. The habits are not what distinguished the successful CEOs from the failures — survivorship bias is making the habits appear important by selecting only the winners.

```python
# Survivorship bias simulation: mutual fund performance
import random

def simulate_fund(years: int = 10) -> list[float]:
    """Simulate annual returns for a mutual fund."""
    return [random.gauss(0.07, 0.15) for _ in range(years)]

def fund_survived(annual_returns: list[float]) -> bool:
    """Fund is 'closed' if cumulative loss exceeds -50%."""
    cumulative = 1.0
    for r in annual_returns:
        cumulative *= (1 + r)
        if cumulative < 0.5:
            return False
    return True

# Simulate 1000 funds
n_funds = 1_000
all_funds = [simulate_fund() for _ in range(n_funds)]

surviving_funds = [f for f in all_funds if fund_survived(f)]
closed_funds = [f for f in all_funds if not fund_survived(f)]

# Average final value: survivor-only vs. all
def final_value(annual_returns):
    v = 1.0
    for r in annual_returns:
        v *= (1 + r)
    return v

avg_all = sum(final_value(f) for f in all_funds) / len(all_funds)
avg_surviving = sum(final_value(f) for f in surviving_funds) / len(surviving_funds) if surviving_funds else 0

print(f"Funds started: {n_funds}")
print(f"Funds that survived: {len(surviving_funds)}")
print(f"Average final value (all funds): {avg_all:.2f}x initial investment")
print(f"Average final value (survivors only): {avg_surviving:.2f}x initial investment")
print(f"Survivorship bias inflates apparent performance by: "
      f"{(avg_surviving / avg_all - 1):.0%}")
# The survivor-only average dramatically overstates typical fund performance
```

### How to Spot the Lie

Ask: "What happened to the cases that are not in this data?" When reading about success stories, ask: "How many unsuccessful attempts using the same approach are not mentioned?" When evaluating investment performance records, ask: "Are funds that were closed or merged included?" Any time you see analysis of entities that have been selected by survival, ask what the selection criteria were and how representative the survivors are.

---

## Part 6: Goodhart's Law — When the Measure Becomes the Target

Goodhart's Law, named for British economist Charles Goodhart, states: "When a measure becomes a target, it ceases to be a good measure." The moment you use a metric as the official target of an organizational incentive, behavior changes in response to the metric — often in ways that preserve the metric while destroying the underlying thing the metric was supposed to measure.

### The Test Score Example

A school district decides to hold teachers accountable for student test scores. Teachers whose students score poorly will be reprimanded; teachers whose students score well will receive bonuses.

Teachers now have powerful incentives to improve test scores. Some teachers respond by improving their teaching in genuine ways that produce both better test scores and better learning. Other teachers respond by teaching specifically to the test — drilling the exact formats of test questions at the expense of broader, deeper learning. Some teachers, under severe enough pressure, help students cheat.

The result: test scores may go up. Student learning may not. The metric (test scores) has decoupled from the underlying goal (education) because the metric became the target.

### The Medical Coding Example

A hospital is ranked and reimbursed partly based on mortality rates — the rate at which patients admitted to the hospital die. The hospital leadership, trying to improve this metric, notices that many patients arriving in very serious condition with low survival probabilities are dying in the hospital. They note that if these patients were transferred to hospice care rather than admitted, their deaths would not count against the hospital's mortality rate.

The result: the hospital changes its admissions and transfer policies, and its mortality rate improves. But actual patient outcomes have not improved — the patients who were transferred to hospice still died. The metric improved; the underlying reality it was meant to measure did not.

### How to Spot the Lie

When an organization shows improving metrics, ask: "What incentives existed to improve this metric, and could those incentives have been met by changing behavior in ways that improve the number without improving the underlying reality?" Ask: "What direct measure of the underlying goal would we use if measuring it were possible?" Ask: "Has the metric and the goal diverged?"

---

## Part 7: Relative Risk Without Absolute Risk — The Half-Truth in Medical Marketing

We introduced this in Part 1, but it is important enough to revisit with more depth here. The deliberate use of relative risk without absolute risk context is one of the most common and most consequential statistical manipulations in medical and pharmaceutical communication.

The structure of the manipulation is always the same: a treatment reduces risk by some impressive-sounding relative percentage, but the absolute baseline risk is small, so the absolute reduction is negligible.

### The Statin Example

Statins are a class of cholesterol-lowering drugs that have been extensively studied and are widely prescribed. One commonly cited statistic is that statins reduce the risk of a heart attack by about thirty-six percent in people with elevated cholesterol and no prior heart disease. Thirty-six percent sounds like a substantial benefit.

But in a population of generally healthy people with elevated cholesterol and no prior cardiac events, the baseline annual risk of a heart attack is roughly one to two percent over five years. A thirty-six percent relative reduction of a two percent baseline risk produces an absolute reduction of about zero point seven percent over five years. This translates to a number needed to treat of roughly one hundred and forty — meaning approximately one hundred and forty people must take statins for five years for one heart attack to be prevented.

Is this worthwhile? That depends on the cost of the drug, the side effect profile, and the individual patient's preferences and risk factors. There is a legitimate argument that it is worthwhile. But the patient who makes that decision based on "this drug reduces heart attack risk by thirty-six percent" is making it based on a framing that makes the benefit sound much larger than it is. The patient who is told "approximately one person in a hundred and forty who takes this drug for five years will avoid a heart attack that would otherwise have occurred" is making the same decision based on an honest representation of the same underlying data.

| How the benefit is stated | How it sounds | What it means |
|---|---|---|
| "Reduces risk by 36%" | Very significant | Relative risk reduction |
| "Absolute risk falls from 2% to 1.28%" | Modest | Absolute risk reduction |
| "1 in 140 people benefits over 5 years" | Very small | Number needed to treat |
| "For every 100 patients, 99 get no benefit" | Discouraging | Inverse of NNT |

All four rows describe exactly the same finding. The framing determines the emotional response.

### How to Spot the Lie

Always ask: what is the absolute risk reduction, not just the relative risk reduction? What is the number needed to treat? What is the baseline risk in the population this result applies to? A result that reports only relative risk reduction without absolute context is presenting an incomplete picture.

---

## Part 8: The Ecological Fallacy — Group Data Applied to Individuals

The ecological fallacy occurs when conclusions drawn from aggregate (group-level) data are applied to individuals. The patterns that exist at the group level often do not apply, and sometimes actively contradict, the patterns that exist at the individual level.

### Example: Education and Income

Suppose you observe across cities in a country that cities with higher average education levels have higher average incomes. You might conclude: education causes higher income. But consider: within each city, it is possible that the highest-earning individuals are high school dropouts who started successful businesses, while the lowest earners are recent college graduates paying off student loan debt and working in low-wage service jobs. The group-level correlation (average education correlates with average income across cities) might be driven entirely by other factors — like the concentration of high-skill industries in certain cities — that have nothing to do with any individual's education causing their income.

Using the group-level finding to advise an individual "you should get more education to earn more" is an ecological fallacy. The relationship at the population level does not necessarily translate to the individual level.

### How to Spot the Lie

When a statistical relationship is reported for groups (countries, cities, companies, demographic segments), ask: "Has this relationship been tested at the individual level?" Ask: "What is the unit of analysis, and does the conclusion apply at that unit?" Ask: "Could the group-level pattern emerge from individual-level patterns that are the opposite?"

---

## Part 9: The Prosecutor's Fallacy — Confusing the Direction of Conditional Probability

The prosecutor's fallacy is a specific form of conditional probability confusion that appears in courtrooms with frightening regularity and has contributed to unjust convictions.

The fallacy involves confusing the probability of evidence given innocence with the probability of innocence given evidence. These are different, and we saw this distinction clearly in our discussion of the base rate problem in medical testing in Part 4.

In a legal context: suppose DNA evidence from a crime scene matches the defendant. An expert testifies that "the probability of a random innocent person matching this DNA profile is one in a million." The prosecutor argues: "Therefore, the probability that this defendant is innocent is one in a million."

This is the fallacy. The one-in-a-million figure is the probability of a match given innocence — how likely is it that an innocent person happens to share this DNA profile? But the probability of innocence given a match is different, and it depends on the prior probability of guilt — how many people were reasonably considered as suspects before the DNA test?

If the police tested one million people, we would expect one innocent person among them to match purely by chance. If the defendant was specifically selected for testing because they were already the only suspect, the evidence is much stronger. The same match carries very different evidentiary weight depending on the context in which it was obtained.

This is not abstract. British courts have seen multiple miscarriages of justice partly attributable to this confusion. The Royal Statistical Society has published formal guidance for expert witnesses and judges on the correct interpretation of probabilistic evidence.

### How to Spot the Lie

Whenever conditional probabilities are presented as legal evidence, ask: "Is this P(evidence | innocence), or is this P(innocence | evidence)? What prior probability is being assumed?" Ask: "How many people were tested before this match was found?"

---

## Part 10: Anecdotes as Data — The Persuasive Story That Proves Nothing

Human brains are far more responsive to vivid individual stories than to cold aggregate data. Political speeches are full of individual people whose lives illustrate the speaker's policy point, precisely because that individual story is more emotionally compelling and more memorable than the statistical summary. Advertising is full of testimonials. Social media is full of personal narratives.

Anecdotes are not inherently dishonest. They can humanize statistical abstractions and help people understand what data means in practice. But anecdotes that are presented as representative of a general pattern — when they may be cherry-picked outliers — are a deceptive use of the form.

The classic version: a politician introduces a constituent who received excellent care under the current healthcare system, to argue that the system is working well. Or introduces a constituent who suffered a bureaucratic horror, to argue that it is not. Either individual story might be entirely accurate. Neither individual story tells you anything about what the distribution of experiences looks like across the millions of people using the system. For that, you need data. The story is not a substitute for data; it is bait designed to bypass the requirement for data.

### How to Spot the Lie

Ask: "Is this story representative, or is it an outlier?" Ask: "What do the aggregate statistics look like across all cases, not just this one?" Ask: "Why is this particular story being highlighted? Who chose it, and what is their interest in highlighting this example rather than a different one?"

---

## Part 11: The Comprehensive Checklist

Here, organized as a reference tool, is the complete checklist for evaluating any statistical claim:

```
STATISTICAL CLAIM EVALUATION CHECKLIST
=======================================

1. SOURCE AND FUNDING
   □ Who conducted this study or analysis?
   □ Who funded it? Do they have a financial interest in the conclusion?
   □ Has it been peer-reviewed and by whom?

2. SAMPLE AND SELECTION
   □ How large is the sample?
   □ How was the sample selected? Is it representative?
   □ Is survivorship bias present? What cases are excluded?
   □ Is there cherry-picking of time periods, studies, or data points?

3. COMPARISONS AND BASELINES
   □ Is relative risk reported without absolute risk?
   □ What is the actual baseline rate?
   □ What is the number needed to treat or number needed to harm?
   □ Is the comparison group appropriate?

4. CAUSATION
   □ Is a causal claim being made from correlational evidence?
   □ What is the proposed mechanism?
   □ Could a third variable explain both?
   □ Could the causal arrow run in reverse?
   □ Was this an experiment (random assignment) or observation?

5. SIGNIFICANCE AND SIZE
   □ Is statistical significance conflated with practical significance?
   □ What is the actual effect size?
   □ How large was the sample? (Large samples find tiny, irrelevant effects)
   □ Are confidence intervals reported?

6. GRAPHS AND PRESENTATION
   □ Does the y-axis start at zero? If not, is truncation justified?
   □ Are appropriate baselines included for comparison?
   □ Are scales comparable across variables on the same chart?

7. REPLICATION AND CONSISTENCY
   □ Has this finding been replicated independently?
   □ Are there contradictory studies? Were they mentioned?
   □ Was the analysis pre-registered?

8. GOODHART AND GAMING
   □ Is this metric subject to organizational incentives?
   □ Could the improvement in the metric reflect gaming rather than
     improvement in the underlying reality?
```

---

## Part 12: What Comes Next

We have now built the toolkit of statistical self-defense. In the sixth and final content installment of this series, we apply everything we have learned — distributions, hypothesis testing, Bayesian reasoning, and the manipulation catalog — to specific professional and civic domains. We will build tailored practical guides for informed voters, software developers, accountants and auditors, and executive leaders.

In the seventh through tenth installments, we build out the full expert-level synthesis: how to analyze a study from scratch, how to read a scientific paper's methods section, how numbers are used in law and policy, and how to bring probabilistic thinking into your professional life permanently.

The goal has always been not just to recognize bad arguments but to have the conceptual confidence to push back, to ask the right questions, and to hold number-bearers accountable. After five installments, you are nearly there.

---

## Sources and Further Reading

- Huff, Darrell. *How to Lie with Statistics*. W.W. Norton, 1954. The original and still essential guide to statistical deception; concise, witty, and comprehensive for a 1954 publication that anticipated every major modern manipulation technique.

- Spiegelhalter, David. *The Art of Statistics: How to Learn from Data*. Basic Books, 2019. Chapter 12 on "Communication and Visualization" and the sections on survival bias are particularly relevant to the material in this installment.

- Gelman, Andrew. "Statistical Modeling, Causal Inference, and Social Science" (blog). An indispensable ongoing commentary on statistical manipulation in social science and medical research; freely available online at statmodeling.stat.columbia.edu.

- Petticrew, Mark, and Roberts, Helen. *Systematic Reviews in the Social Sciences*. Wiley-Blackwell, 2006. The definitive guide to avoiding cherry-picking in literature reviews, with extensive discussion of publication bias and how to assess it.

- Turner, Erick H. et al. "Selective Publication of Antidepressant Trials and Its Influence on Apparent Efficacy." *New England Journal of Medicine*, 358(3), 2008. The antidepressant publication bias study referenced in Part 2.

- Wald, Abraham. "A Method of Estimating Plane Vulnerability Based on Damage of Survivors." Center for Naval Analyses, 1943 (reprinted). The classic survivorship bias analysis of World War II aircraft.

- Royal Statistical Society. "Fundamentals of Probability and Statistical Evidence in Criminal Proceedings." 2010. Guidance for expert witnesses, barristers, and judges on avoiding the prosecutor's fallacy and other statistical errors in legal proceedings. Available at rss.org.uk.
