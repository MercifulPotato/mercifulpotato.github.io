---
title: "The Machinery of Proof: Null Hypotheses, P-Values, and the Science of Not Being Wrong"
date: 2026-06-17
author: mercifulpotato-team
summary: "The third installment of our plain-English statistics series: the null hypothesis explained without jargon, what p-values actually mean and why they are so widely misunderstood, statistical significance versus practical significance, and how the replication crisis revealed that even peer-reviewed science can be systematically broken."
tags:
  - statistics
  - hypothesis-testing
  - p-values
  - statistical-significance
  - plain-english
  - data-literacy
  - scientific-method
series: "Probability and Statistics in Plain English"
---

## Part 1: Why Science Needs a Formal Way of Being Wrong

Before any scientific test, before any clinical trial, before any A/B experiment in a technology company, before any quality-control analysis in a factory, there is a choice being made that most people never consciously recognize. The choice is this: who bears the burden of proof?

In ordinary human decision-making, we often demand that advocates of a new idea prove that it works. We are skeptical of novelty by default and demand evidence before changing our minds. This is not irrational — it reflects accumulated experience that most new ideas, most proposed treatments, most claimed improvements are not what they are claimed to be.

Statistical hypothesis testing, in its formal structure, encodes exactly this disposition of skepticism into a mathematical procedure. It builds in a presumption of innocence for the status quo and demands that evidence for a change clear a specific threshold before we accept it.

Understanding this procedure deeply — not just the name "hypothesis testing" but the actual logic of what is happening — will make almost everything else in this series fall into place. The null hypothesis, the p-value, statistical significance, confidence intervals, and the replication crisis are all facets of this one central procedure. If you understand the logic, all the vocabulary follows naturally.

---

## Part 2: The Null Hypothesis — Assuming Nothing Happened

The null hypothesis is the starting position of skepticism. It is, in almost every scientific test, the hypothesis that says: "there is no real effect here." Or more precisely: "whatever pattern we observed in our data was produced by random chance, not by any genuine underlying cause."

The word "null" comes from Latin meaning "nothing" or "zero." The null hypothesis is the hypothesis of nothing — no real difference, no real effect, no real relationship. It is the bet that the pattern you think you see is actually just noise.

To make this concrete: imagine you are a researcher testing whether a new fertilizer makes tomato plants produce more fruit. You grow one hundred tomato plants, fifty with the new fertilizer and fifty with standard care. At harvest, the fertilized plants produce, on average, twenty-two pounds of tomatoes per plant. The unfertilized plants produce, on average, twenty pounds per plant.

The null hypothesis says: "the fertilizer has no real effect. The fact that the fertilized plants produced more is just random variation — if we ran this experiment again, the fertilized plants might produce less. The two-pound difference is noise, not signal."

This is not pessimism. This is the appropriate starting assumption for scientific inquiry. Before you accept that the fertilizer works, you need to rule out the explanation that the difference you observed was just luck.

Ruling out luck is exactly what statistical testing does. The test asks: if the null hypothesis is true — if the fertilizer really has no effect — how likely is it that we would observe a difference of two pounds or more just by random chance? If that likelihood is very small, the null hypothesis becomes hard to maintain. If that likelihood is substantial — if a two-pound random difference happens all the time even when there is no real effect — then the null hypothesis survives unchallenged, and we cannot conclude that the fertilizer does anything.

The output of this calculation is the p-value.

---

## Part 3: The P-Value — The Most Misunderstood Number in Science

The p-value is everywhere in scientific literature. It appears in almost every published study across medicine, psychology, economics, ecology, and engineering. It has been described as both the most important and the most misunderstood number in science. Two consecutive Nobel laureates in economics have publicly criticized how it is used. Multiple scientific journals have published editorials calling for its reform or abolition.

To understand why it provokes such strong reactions, you first need to understand what it actually says — because almost everyone who uses it casually, including many working scientists, has a subtly wrong mental model of it.

### What the P-Value Says

The p-value is the probability of observing data at least as extreme as what you actually observed, assuming that the null hypothesis is true.

Read that sentence again, carefully, because every word matters.

It is NOT the probability that the null hypothesis is true. It is NOT the probability that your result was due to chance. It is NOT the probability that your hypothesis is correct. It is NOT a measure of the importance or size of the effect.

It is the probability of seeing data this extreme (or more extreme) if the null hypothesis were true — if there were genuinely no effect at all, just random noise.

### An Analogy: Courtroom Logic

Think of a courtroom. The defendant is presumed innocent. Innocence is the null hypothesis. The prosecution presents evidence: fingerprints at the scene, cell phone records, a witness statement. The question the jury must answer is: how likely is this evidence if the defendant is innocent?

If the evidence would be very unlikely for an innocent person — if an innocent person would almost never have fingerprints at that scene, plus that phone location, plus a credible witness placing them there — then the innocent-person hypothesis becomes hard to maintain. The jury may convict.

But notice what the jury is not directly calculating. They are not calculating the probability that the defendant is actually innocent. They are calculating the probability of the evidence given the assumption of innocence. These are different. A very unlikely combination of coincidental evidence against an innocent person is still possible. If you run enough trials in a country of three hundred million people, some innocent people will face very damning-looking coincidental evidence.

This is precisely the situation with p-values. A p-value of 0.05 means: if the null hypothesis were true (no real effect), there is a five percent chance we would see data this extreme by random chance. It does not mean there is a five percent chance the null hypothesis is true. The probability that the null hypothesis is true is a different question entirely — one that requires additional information, specifically, what we thought the probability was before we ran the experiment. This is the realm of Bayesian reasoning, which gets its own full installment in Part 4.

---

## Part 4: The Significance Level — Who Decides What Counts as Proof?

If the p-value is a probability — the probability of data this extreme under the null hypothesis — then there must be a threshold below which we decide "this p-value is small enough that I reject the null hypothesis." That threshold is called the significance level, usually written as alpha in scientific papers.

The most common significance level in most fields is 0.05 — meaning a five percent threshold. If the p-value is below 0.05, the result is declared "statistically significant" and the null hypothesis is rejected. If the p-value is 0.05 or above, the null hypothesis is not rejected, and we conclude that the data does not provide sufficient evidence against it.

### Who chose five percent?

Ronald Aylmer Fisher, the British statistician who formalized much of this framework in the 1920s, suggested 0.05 as a "convenient" threshold in his 1925 book *Statistical Methods for Research Workers*. Fisher himself was ambivalent about using any fixed threshold — he thought researchers should use their judgment about different levels of evidence for different situations. The rigid 0.05 threshold became a convention largely because it provided a clear bright line that journals could use to decide what to publish.

This has turned out to be a problem of enormous proportions.

### The Perverse Incentives Created by the 0.05 Threshold

If "statistically significant" (p < 0.05) results get published and "statistically non-significant" (p ≥ 0.05) results do not, then the scientific literature systematically fills up with one kind of result and not the other. This is called publication bias, and it distorts the accumulated evidence on almost every topic that has been studied.

Imagine one hundred researchers testing whether a specific supplement has any effect on memory. In truth, the supplement does nothing — the null hypothesis is correct. But if each researcher runs a test at the 0.05 significance level, we would expect five of them (five percent of one hundred) to get a p-value below 0.05 purely by chance — five false positives out of one hundred correct null hypotheses.

If only those five studies get published (because journals prefer significant results), the scientific literature contains five papers showing that the supplement works and zero papers showing it does not. A meta-analysis that combines published results would conclude that the evidence strongly supports the supplement's effectiveness, even though the supplement does literally nothing.

This is not hypothetical. The history of nutritional research, social psychology, and many areas of medicine is populated by findings that turned out to be false positives produced by exactly this dynamic.

```python
# Simulating the publication bias problem
import random

def run_study(true_effect_exists: bool, significance_level: float = 0.05) -> dict:
    """
    Simulate a study. If true_effect_exists=False, the null is true.
    A p-value is simulated: under the null, p-values are uniformly distributed.
    (This is a key theoretical property: if H0 is true, p ~ Uniform(0,1))
    """
    if true_effect_exists:
        # With a real effect, p-values tend to be small
        p_value = random.betavariate(0.5, 5)  # Skewed low
    else:
        # Under the null, p-values are uniformly distributed
        p_value = random.uniform(0, 1)

    return {
        "p_value": p_value,
        "significant": p_value < significance_level
    }

# Scenario: supplement does NOTHING (null is true for all studies)
n_studies = 1000
results = [run_study(true_effect_exists=False) for _ in range(n_studies)]

significant_results = [r for r in results if r["significant"]]
published_rate = len(significant_results) / n_studies

print(f"Studies run: {n_studies}")
print(f"Studies finding 'significant' result: {len(significant_results)}")
print(f"'False positive' rate: {published_rate:.1%}")
print(f"If only 'significant' studies get published:")
print(f"  Published record shows: {len(significant_results)} positive studies")
print(f"  Published record shows: 0 negative studies (not published)")
print(f"  An observer would conclude: STRONG evidence the supplement works")
print(f"  The truth: the supplement does NOTHING")
```

The code above demonstrates that even in a world where nothing works — where the null hypothesis is true for every study — a five percent false positive rate means that out of a thousand studies, roughly fifty will appear to show an effect. If those fifty get published and the nine-hundred fifty "no effect" results stay in file drawers, the literature looks damning.

---

## Part 5: Statistical Significance Is Not Practical Significance

This distinction is one of the most important in all of applied statistics, and it is one of the most commonly conflated in public discourse.

**Statistical significance** is a statement about whether an observed effect is large enough to be unlikely to have occurred by chance, given the sample size used.

**Practical significance** is a statement about whether the effect is large enough to actually matter in the real world.

These are entirely different questions, and a result can be one without being the other.

### When Something Is Statistically Significant But Practically Worthless

Suppose a pharmaceutical company runs a clinical trial with fifty thousand patients and finds that their blood pressure medication reduces systolic blood pressure by an average of one millimeter of mercury. With fifty thousand patients, even a tiny effect can clear the 0.05 significance threshold — the enormous sample size gives extremely high statistical power, meaning the test can detect very small effects with great confidence.

But is a one-millimeter reduction in blood pressure clinically meaningful? Cardiologists generally consider reductions below five millimeters to be clinically negligible. The drug produces a real effect (the p-value is legitimate), but the effect is too small to affect health outcomes in any measurable way.

The study will truthfully report a "statistically significant" reduction in blood pressure. The drug will be marketed as "proven to reduce blood pressure." Both statements are accurate and together form a misleading picture. A patient who reads "statistically significant reduction in blood pressure" and concludes "this drug will help my heart health" has been led astray by words that sound like they mean something they do not actually mean in this context.

### When Something Is Practically Significant But Statistically "Non-Significant"

The opposite error is also common and is especially pernicious in safety research. Suppose a small study with thirty patients finds that a new diet intervention reduces the risk of a serious health event by forty percent, but the p-value is 0.12 — above the 0.05 threshold. The result is declared "statistically non-significant."

A forty percent reduction in serious health events would, if real, be a major clinical finding. But with only thirty patients, the study did not have enough statistical power to distinguish a genuine forty percent effect from random noise. The failure to reach significance does not mean the effect does not exist — it may mean the study was simply too small to detect it reliably. Declaring the result "non-significant" and concluding "the diet does not work" is an error.

This is called a Type II error or a false negative — the test missed a real effect because it lacked the power to find it.

```
Statistical Significance vs. Practical Significance Matrix:
-------------------------------------------------------------
|                     | Practically Significant | Not Practically Significant |
|---------------------|-------------------------|------------------------------|
| Statistically       | The ideal: real effect, | Danger: effect is real but   |
| Significant         | large enough to matter  | trivially small; huge N helps|
|---------------------|-------------------------|------------------------------|
| Not Statistically   | Danger: real effect,    | Acceptable: no effect, and   |
| Significant         | but study too small     | data confirms it             |
|                     | to detect it            |                              |
-------------------------------------------------------------

Key questions to always ask:
1. What is the actual size of the effect? (not just whether it exists)
2. How big was the study? (large studies detect tiny, irrelevant effects)
3. Is the effect size clinically/practically meaningful?
```

The concept of effect size — how large an effect actually is, independent of whether it is statistically significant — is one of the most important and underreported statistics in published research. When you read a study, always look for effect sizes alongside p-values. If the study does not report effect sizes, that is a red flag.

---

## Part 6: Confidence Intervals — A Better Way to Think About Evidence

Closely related to significance testing, and in many ways more informative, are confidence intervals. A confidence interval gives you a range of values that is consistent with the data you observed.

When a medical study reports "the drug reduces blood pressure by an average of 8 mm Hg with a 95 percent confidence interval of 4 to 12 mm Hg," it is saying: given the data we collected, values for the true effect anywhere between 4 mm Hg and 12 mm Hg are consistent with what we observed. If we ran this study many times, ninety-five percent of the time the interval we computed would capture the true value.

This is more informative than a p-value alone for several reasons.

First, it tells you the size of the effect, not just whether an effect was detected. An effect size of somewhere between 4 and 12 mm Hg is medically meaningful — those are values that would influence treatment decisions.

Second, it tells you the precision of your estimate. A narrow confidence interval (say, 7 to 9) tells you the study was large and well-designed; the data pin down the effect quite precisely. A wide confidence interval (say, minus 2 to plus 18) tells you the study was small or noisy; the effect could plausibly be anything from mildly beneficial to mildly harmful.

Third, whether or not the interval includes zero tells you about statistical significance in a natural way. If the entire interval is above zero, the effect is positive regardless of where in the range the truth lies — the data rules out no effect. If the interval straddles zero (say, minus 3 to plus 15), a zero effect is consistent with the data, and the study has not ruled out "no effect." This is exactly equivalent to p > 0.05 for two-sided tests.

```python
# Illustrating confidence intervals conceptually
# Suppose a study of 200 patients shows:
# - Mean blood pressure reduction: 8 mm Hg
# - Standard error of the mean: 2 mm Hg (reflects sample size and variability)

point_estimate = 8  # mm Hg
standard_error = 2  # mm Hg

# A 95% confidence interval spans roughly 1.96 standard errors in each direction
# (This comes from the normal distribution: 95% of values fall within ~2 standard deviations)
margin = 1.96 * standard_error

ci_lower = point_estimate - margin
ci_upper = point_estimate + margin

print(f"Estimated effect: {point_estimate} mm Hg")
print(f"95% CI: [{ci_lower:.1f}, {ci_upper:.1f}] mm Hg")
print(f"Includes zero? {ci_lower < 0 < ci_upper}")
# If CI does not include zero, the result is statistically significant at p < 0.05

# Now consider a SMALLER study with same effect but larger standard error:
small_study_se = 5  # Larger SE due to small sample
small_study_margin = 1.96 * small_study_se
small_ci_lower = point_estimate - small_study_margin
small_ci_upper = point_estimate + small_study_margin

print(f"\nSmall study - same effect:")
print(f"95% CI: [{small_ci_lower:.1f}, {small_ci_upper:.1f}] mm Hg")
print(f"Includes zero? {small_ci_lower < 0 < small_ci_upper}")
# Wide CI straddles zero; same true effect is "non-significant" with small n
```

---

## Part 7: The Replication Crisis — When Science Breaks Down

In 2011, a psychologist named Brian Nosek organized a project to replicate one hundred published psychology studies — to run the exact same experiments again with new participants and see if the results held up. The results of this Reproducibility Project were published in 2015 in the journal *Science*, and they sent shockwaves through the scientific community.

Only thirty-six of the one hundred original studies showed statistically significant results when replicated. Sixty-four did not replicate — the original findings either vanished entirely or shrank dramatically.

This was not a psychology-specific problem. Similar replication failures have been documented in cancer biology, medicine, economics, neuroscience, and nutrition research. In cancer biology, a 2012 study found that results from only eleven out of fifty-three published "landmark" papers could be reproduced internally at Amgen. In clinical medicine, a researcher named John Ioannidis published a famous 2005 paper titled "Why Most Published Research Findings Are False" — a paper that remains one of the most-downloaded scientific articles of all time.

### Why Did This Happen?

The replication crisis has multiple causes, all of which trace back to the structure of scientific incentives and the way hypothesis testing is used.

**Publication bias**, which we discussed in Part 4, means that positive findings (p < 0.05) get published and negative findings (p ≥ 0.05) do not. The literature fills up with one-time flukes.

**P-hacking**, also called data dredging, refers to the practice of running many different analyses on the same dataset until one produces p < 0.05, then reporting only that analysis as if it were the one planned from the start. If you measure fifty different outcomes and run fifty statistical tests, you expect roughly two to three of them to produce p < 0.05 purely by chance, even if nothing is real. If you report only the two or three "significant" ones and discard the rest, you are reporting noise as signal.

```python
# Simulating p-hacking: measure 50 outcomes, find "significant" one by chance
import random

def run_random_test():
    """Simulate a test of one outcome where null hypothesis is true.
    Returns a simulated p-value."""
    # Under the null, p-values are uniformly distributed between 0 and 1
    return random.uniform(0, 1)

outcomes_tested = 50
p_values = [run_random_test() for _ in range(outcomes_tested)]
significant = [(i+1, p) for i, p in enumerate(p_values) if p < 0.05]

print(f"Outcomes tested: {outcomes_tested}")
print(f"'Significant' outcomes (p < 0.05): {len(significant)}")
for outcome_num, p in significant:
    print(f"  Outcome {outcome_num}: p = {p:.4f}")
print(f"\nIf you report only these outcomes, your paper looks like:")
print(f"  'We found that Outcome {significant[0][0] if significant else 'N/A'} was")
print(f"   significantly associated with the treatment (p = {significant[0][1]:.4f})'")
print(f"  (The 49 other tested outcomes are not mentioned.)")
```

**HARKing** — Hypothesizing After Results are Known — is the practice of constructing a hypothesis after seeing the data, then presenting it as if it were an a priori prediction. A researcher who examines many possible relationships in a dataset, finds one that is statistically significant, and then writes their paper as if they had specifically predicted that relationship from the beginning is presenting a false impression of confirmatory evidence that is actually exploratory.

**Small sample sizes** are endemic in fields like psychology and nutrition. A study of twenty or thirty people has very low statistical power — it can only detect large effects reliably. Small-sample studies that do reach significance are more likely to have done so through a combination of genuine effect and lucky sampling. This means their effect size estimates are inflated, and they are less likely to replicate.

### What Can Be Done

The scientific community has responded to the replication crisis with a range of reforms, some more widely adopted than others.

Pre-registration requires researchers to publicly register their hypothesis, methodology, and analysis plan before collecting any data. This prevents p-hacking and HARKing because the plan is on record. Several journals now offer "registered reports" — a publication model where a study is accepted before results are known, based on the quality of the design.

Pre-registered replications, where independent labs commit in advance to replicating a specific study, are far more credible than replications with unknown prior probability of replication.

Raising the significance threshold — from 0.05 to 0.005 or lower — for fields where false positives have been particularly rampant has been proposed and partly adopted in some journals.

Publishing null results — studies that find no effect — reduces publication bias. Several journals now explicitly accept high-quality null-result papers.

---

## Part 8: How to Apply This Knowledge in Practice

### As a Reader of Medical Research

When you read about a study, ask: was the significance level declared in advance, or was the study designed to hunt for p < 0.05? Is the reported p-value accompanied by effect sizes and confidence intervals? Has the finding been replicated by independent researchers? Was the study pre-registered?

A single study with p = 0.04 that has not been replicated and was not pre-registered is very weak evidence. A finding that has been replicated in multiple independent studies across different populations and that was specified in a pre-registered design is strong evidence.

### As a Software Developer Running A/B Tests

A/B testing in software is hypothesis testing. The null hypothesis is that the two versions perform equally. The significance level is your false positive rate. Running many simultaneous A/B tests and acting on whichever ones hit your significance threshold first is p-hacking. Use Bonferroni correction or similar adjustments when running multiple simultaneous tests. Pre-commit to your sample size before running the test, rather than stopping when you see significance.

```python
# A/B test: correct practice vs. p-hacking
# Correct: pre-commit to sample size, run once, evaluate
# P-hacking: check for significance repeatedly, stop when p < 0.05

def ab_test_result(control_conversion_rate: float,
                   treatment_conversion_rate: float,
                   n_per_group: int) -> float:
    """
    Simulate an A/B test result as a simplified p-value proxy.
    Returns a float representing observed effect / noise ratio.
    Higher means more "significant."
    """
    import random
    control = [1 if random.random() < control_conversion_rate else 0
               for _ in range(n_per_group)]
    treatment = [1 if random.random() < treatment_conversion_rate else 0
                 for _ in range(n_per_group)]
    diff = sum(treatment)/n_per_group - sum(control)/n_per_group
    return diff

# Scenario: both versions have identical 10% conversion rate (null is true)
# If we peek 20 times as data accumulates, how often do we see "significant" result?
n_experiments = 1000
false_discoveries = 0
for _ in range(n_experiments):
    # Simulate sequential peeking at results
    found_significant = False
    for sample_size in range(50, 1000, 50):  # Peek every 50 samples
        effect = ab_test_result(0.10, 0.10, sample_size)
        # Rough threshold: |effect| > 0.03 as a stand-in for p < 0.05
        if abs(effect) > 0.03:
            found_significant = True
            break
    if found_significant:
        false_discoveries += 1

print(f"Sequential peeking false discovery rate: {false_discoveries/n_experiments:.1%}")
print(f"Expected if testing correctly once: ~5%")
# Sequential peeking dramatically inflates false positive rate
```

### As an Auditor or Analyst

When evaluating statistical claims in reports, ask whether the analysis was pre-specified or exploratory. Exploratory analyses generate hypotheses; they are not confirmation. Results from exploratory analyses require independent confirmation before being acted upon. Be especially skeptical of claims that emerge from large datasets where researchers had unlimited freedom to hunt for patterns — with enough fishing, something always bites.

### As a Voter or Citizen

When a politician or interest group cites scientific research to support a policy, ask whether the research has been replicated. Ask whether the finding is from a single study or a meta-analysis of many studies. Ask whether the research was funded by parties with a financial interest in the outcome. Ask whether the finding has held up under scrutiny or whether it is contested in the scientific community.

A "study shows" claim from a single, small, industry-funded study that has not been replicated is essentially meaningless as policy evidence. A finding that has been replicated across many independent studies conducted by researchers with no financial stake in the outcome is meaningful.

---

## Part 9: What Comes Next

We have now traveled through the full machinery of formal statistical proof: the null hypothesis as the presumption of no effect, the p-value as the probability of data this extreme under that presumption, the significance level as the threshold for rejecting the null, and the distinction between statistical and practical significance. We have also seen how this machinery breaks down — through publication bias, p-hacking, HARKing, and small sample sizes — and what reforms are beginning to address those failures.

In the fourth installment, we turn to one of the deepest and most productive arguments in all of statistics: the philosophical war between Bayesian and frequentist thinking. These are two fundamentally different answers to the question of what probability means and how it should be used. Understanding the divide, and understanding when each approach is more appropriate, is essential for applying statistical reasoning intelligently across the domains of medicine, law, artificial intelligence, and everyday decision-making.

As always: no Greek letters, no formulas, just clear thinking about deep ideas.

---

## Sources and Further Reading

- Fisher, R.A. *Statistical Methods for Research Workers*. Oliver and Boyd, 1925. The original source of the 0.05 significance threshold, now in its fourteenth edition; historically important for understanding where the convention came from.

- Open Science Collaboration. "Estimating the reproducibility of psychological science." *Science*, 349(6251), 2015. The landmark Reproducibility Project that triggered widespread recognition of the replication crisis. Available at science.sciencemag.org.

- Ioannidis, John P.A. "Why Most Published Research Findings Are False." *PLOS Medicine*, 2(8), 2005. The most-cited paper in the replication crisis literature; accessible, rigorous, and still highly relevant.

- Wasserstein, Ronald L. and Lazar, Nicole A. "The ASA Statement on p-Values: Context, Process, and Purpose." *The American Statistician*, 70(2), 2016. The American Statistical Association's formal guidance on the proper interpretation and use of p-values.

- Gelman, Andrew, and Loken, Eric. "The Statistical Crisis in Science." *American Scientist*, 102(6), 2014. An accessible explanation of how researcher degrees of freedom lead to false positive inflation.

- Simmons, Joseph P., Nelson, Leif D., and Simonsohn, Uri. "False-Positive Psychology: Undisclosed Flexibility in Data Collection and Analysis Allows Presenting Anything as Significant." *Psychological Science*, 22(11), 2011. The paper that demonstrated empirically how easy it is to find p < 0.05 with routine p-hacking practices.

- Nosek, Brian A. et al. "Promoting an open research culture." *Science*, 348(6242), 2015. Practical framework for open science reforms including pre-registration and data sharing.
