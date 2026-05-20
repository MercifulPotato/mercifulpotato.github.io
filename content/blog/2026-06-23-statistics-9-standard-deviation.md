---
title: "The Spread of Things: Standard Deviation, Variance, and Why Knowing the Average Is Never Enough"
date: 2026-06-23
author: mercifulpotato-team
summary: "The ninth installment of our plain-English statistics series: standard deviation and variance explained from first principles without any formulas — what spread means, why it matters more than the average in dozens of practical situations, and how to use the idea of standard deviation to reason about probability in everyday decisions."
tags:
  - statistics
  - standard-deviation
  - variance
  - plain-english
  - data-literacy
  - probability
  - distributions
series: "Probability and Statistics in Plain English"
---

## Part 1: The Question the Average Cannot Answer

In the spring of 2021, a widely circulated piece of satire observed that a statistician could drown in a river with an average depth of four feet. The joke is the premise of this installment: average depth does not tell you whether you will drown, because it says nothing about how that depth is distributed across the river.

If the river is four feet deep everywhere — a perfectly flat riverbed — then someone of average adult height can walk across it getting wet to their chest. If the river has shallow edges and a ten-foot channel in the middle, the average might still be four feet, but someone who steps into the channel will be swept under.

The average is the same. The experience is completely different. What distinguishes the two rivers is the spread of their depths — how far individual measurements deviate from the average, and whether they deviate consistently or wildly.

This is what standard deviation measures: the typical size of the deviation from the average. It is the single most important supplement to the average in describing a dataset, and it lies beneath almost every other statistical concept we have discussed in this series.

---

## Part 2: Building Intuition for Spread — The Test Score Example

Suppose two students both have an average test score of seventy-five across five tests. Student A's individual scores were: seventy-three, seventy-five, seventy-six, seventy-four, seventy-seven. Student B's individual scores were: fifty, sixty, seventy-five, ninety, eighty-five.

Both averages are seventy-five. But something is clearly different about these two students. Student A is consistent: you could predict the next test score with reasonable confidence. Student B is inconsistent: you know the average is seventy-five, but the next score could be anywhere from the forties to the nineties.

For a teacher deciding whether to intervene, the spread matters as much as the average. A student whose performance is consistently near passing might need different support than one whose performance is wildly variable — great on some topics, failing on others.

Standard deviation is simply a number that quantifies how far the typical individual value is from the average. A small standard deviation means the values cluster tightly around the average. A large standard deviation means they spread widely.

### Thinking About It in Steps

Without any formula, here is the logic of standard deviation as a sequence of intuitive steps:

Step one: find the average. For Student A: (73 + 75 + 76 + 74 + 77) divided by 5 equals 75.

Step two: for each individual value, ask "how far is this from the average?" Student A's distances from 75 are: 2, 0, 1, 1, 2. These are the individual deviations.

Step three: get a sense of the typical deviation. If you simply averaged the distances (treating them as positive regardless of direction), you would get (2 + 0 + 1 + 1 + 2) / 5 = 1.2. The typical score deviates from the average by about 1.2 points. This is a reasonable intuitive measure of spread, closely related to the mean absolute deviation.

Standard deviation uses a slightly different mathematical approach to step three — it squares the deviations before averaging and then takes a square root — but the conceptual purpose is identical: to quantify the typical size of how far individual values are from the average.

For Student B: the distances from 75 are: 25, 15, 0, 15, 10. The typical distance is about (25 + 15 + 0 + 15 + 10) / 5 = 13. Student B's scores deviate from the average by roughly 13 points on average. Compared to Student A's 1.2, Student B is about ten times more variable.

```python
# Computing spread metrics without invoking a statistics library
def spread_analysis(values: list[float], label: str = "") -> dict:
    n = len(values)
    mean = sum(values) / n

    # Mean absolute deviation (MAD): intuitive measure of spread
    absolute_deviations = [abs(v - mean) for v in values]
    mad = sum(absolute_deviations) / n

    # Variance: average of squared deviations
    squared_deviations = [(v - mean) ** 2 for v in values]
    variance = sum(squared_deviations) / (n - 1)  # n-1 for sample variance

    # Standard deviation: square root of variance
    std_dev = variance ** 0.5

    # Range: simple but sensitive to outliers
    data_range = max(values) - min(values)

    return {
        "label": label,
        "values": values,
        "mean": mean,
        "mean_absolute_deviation": mad,
        "standard_deviation": std_dev,
        "range": data_range,
    }

student_a = [73, 75, 76, 74, 77]
student_b = [50, 60, 75, 90, 85]

for data, label in [(student_a, "Student A"), (student_b, "Student B")]:
    result = spread_analysis(data, label)
    print(f"\n{result['label']}:")
    print(f"  Scores:     {result['values']}")
    print(f"  Mean:       {result['mean']:.1f}")
    print(f"  Std Dev:    {result['standard_deviation']:.1f}")
    print(f"  Range:      {result['range']}")
```

---

## Part 3: Standard Deviation and the Bell Curve — The 68-95-99.7 Rule

In the second installment of this series, we discussed the bell curve and noted that it has specific properties about how measurements cluster around the center. Now we can make that precise using standard deviation.

For data that is approximately normally distributed, the following rule holds with remarkable consistency across an enormous range of natural phenomena:

Roughly sixty-eight percent of all measurements fall within one standard deviation of the mean (above and below combined). About ninety-five percent fall within two standard deviations. About ninety-nine point seven percent fall within three standard deviations.

This is called the empirical rule or the 68-95-99.7 rule. It is not derived from arbitrary convention — it follows from the mathematical properties of the normal distribution. And it is practically useful because it transforms a single standard deviation number into a probabilistic statement about how likely individual measurements are.

### Applying This to Adult Height

Adult male heights in the United States are approximately normally distributed with a mean of approximately 69.5 inches (just under five feet ten) and a standard deviation of approximately 3 inches.

One standard deviation above the mean: 69.5 + 3 = 72.5 inches (six feet, half inch). One standard deviation below: 69.5 - 3 = 66.5 inches (five feet, six and a half inches). The rule tells us that about sixty-eight percent of men fall between five-six-and-a-half and six feet tall.

Two standard deviations above: 69.5 + 6 = 75.5 inches (six feet, three and a half inches). Two standard deviations below: 69.5 - 6 = 63.5 inches (five feet, three and a half inches). About ninety-five percent of men fall in this range.

Three standard deviations above: 69.5 + 9 = 78.5 inches (six feet, six and a half inches). This is already very tall. About 99.7% of men are shorter than this. The fraction taller than six feet six and a half is roughly 0.15% — about one in six hundred and sixty-seven.

```python
# Applying the 68-95-99.7 rule to a practical example
mean_height_inches = 69.5
std_dev_height = 3.0

print("Adult male heights (approximately normal distribution):")
print(f"Mean: {mean_height_inches} inches ({mean_height_inches/12:.1f}ft)")
print(f"Std dev: {std_dev_height} inches\n")

for n_std, pct in [(1, 68.3), (2, 95.4), (3, 99.7)]:
    lower = mean_height_inches - n_std * std_dev_height
    upper = mean_height_inches + n_std * std_dev_height
    print(f"Within {n_std} std devs ({lower:.1f}\"–{upper:.1f}\"): ~{pct:.1f}% of men")
    print(f"  ({lower/12:.2f}ft – {upper/12:.2f}ft)")

print("\nSpecific reference points:")
targets = [72, 78, 60]  # 6'0", 6'6", 5'0"
for target in targets:
    z = (target - mean_height_inches) / std_dev_height
    print(f"{target}\" ({target/12:.2f}ft): {z:.1f} standard deviations from mean")
```

---

## Part 4: The Z-Score — How Many Standard Deviations Away Are You?

The z-score (sometimes called the standard score) is simply a way of expressing any measurement in units of standard deviations from the mean. A z-score of positive two means the measurement is two standard deviations above the mean. A z-score of negative one means it is one standard deviation below.

The practical power of z-scores is that they allow comparison across different datasets that are measured in different units. You cannot directly compare a height in inches to a weight in pounds, but you can compare a height z-score of plus one point five to a weight z-score of minus zero point eight and say "this person is above average height and below average weight, with the height deviation being roughly twice as large as the weight deviation."

Z-scores also connect directly to probability statements through the empirical rule and the normal distribution. A z-score of plus two corresponds to being in approximately the top two point five percent of the distribution. A z-score of minus three corresponds to being in approximately the bottom zero point fifteen percent.

This is how standardized test scores like the SAT were historically designed: to put everyone on a common scale expressed in terms of standard deviations from the mean, making comparisons across different test cohorts and years more meaningful.

```python
# Z-score practical examples
def z_score(value: float, mean: float, std_dev: float) -> float:
    """How many standard deviations is this value from the mean?"""
    return (value - mean) / std_dev

def approximate_percentile(z: float) -> float:
    """
    Approximate the percentile from a z-score using a simple lookup.
    (In practice, you'd use a normal distribution CDF.)
    """
    # Key z-score to approximate percentile mapping
    z_to_pct = {
        -3.0: 0.1, -2.5: 0.6, -2.0: 2.3, -1.5: 6.7, -1.0: 15.9,
        -0.5: 30.9, 0.0: 50.0, 0.5: 69.1, 1.0: 84.1, 1.5: 93.3,
        2.0: 97.7, 2.5: 99.4, 3.0: 99.9
    }
    # Find closest key
    closest = min(z_to_pct.keys(), key=lambda k: abs(k - z))
    return z_to_pct[closest]

# Scenario: comparing performance across two metrics with different scales
employee_sales = {"mean": 500_000, "std": 80_000}  # Annual sales in dollars
employee_calls = {"mean": 120, "std": 25}            # Customer calls per month

alice = {"sales": 650_000, "calls": 115}
bob = {"sales": 520_000, "calls": 175}

for person, data in [("Alice", alice), ("Bob", bob)]:
    sales_z = z_score(data["sales"], employee_sales["mean"], employee_sales["std"])
    calls_z = z_score(data["calls"], employee_calls["mean"], employee_calls["std"])
    print(f"{person}:")
    print(f"  Sales:  ${data['sales']:,} → z = {sales_z:+.2f} (~{approximate_percentile(sales_z):.0f}th percentile)")
    print(f"  Calls:  {data['calls']}/month → z = {calls_z:+.2f} (~{approximate_percentile(calls_z):.0f}th percentile)")
    print()
```

---

## Part 5: Variance — Standard Deviation's Square Root

Variance is mathematically the square of standard deviation. If standard deviation is the typical distance of values from the mean (measured in the same units as the data), variance is that distance squared — which means its units are also squared. Heights measured in inches have a standard deviation in inches; the variance is in square-inches, which has no natural physical interpretation.

Why does variance exist if standard deviation is more intuitive? Because variance has mathematical properties that make it easier to work with in algebraic derivations. In particular, variances of independent quantities add together, while standard deviations do not. This addition property is the foundation for much of statistical theory.

For the statistically literate non-mathematician, the practical takeaway is simple: variance is standard deviation squared, and whenever you see a formula or calculation involving variance, you can translate it to "standard deviation squared" and think about what it would mean if you took the square root. The conceptual content is the same.

---

## Part 6: Standard Deviation in Quality Control

One of the most direct practical applications of standard deviation is in manufacturing quality control, where the concept is the foundation of a quality management philosophy called Six Sigma.

The premise is this: a manufacturing process has some target measurement — the intended width of a component, the intended volume of liquid in a bottle, the intended electrical resistance of a component. The actual output of the process varies around this target. That variation follows approximately a normal distribution with some mean and some standard deviation.

If the specification limits — the acceptable range within which measurements must fall — are set very wide relative to the process standard deviation, almost all output will be in spec and defect rates will be low. If the specification limits are narrow relative to the process standard deviation, many measurements will fall outside the limits and defect rates will be high.

Six Sigma refers to a situation where the specification limits are set six standard deviations on each side of the mean. In a normal distribution, this would imply that only about 0.002 parts per billion fall outside the specification — an extraordinarily low defect rate. (The actual Six Sigma target uses a slightly different calculation that accounts for process drift, yielding a defect rate of 3.4 per million opportunities.)

The practical management principle is: to reduce defects, you must either move the process mean closer to the target (centering) or reduce the process standard deviation (reducing variation). Both improve quality. But reducing variation — understanding and eliminating the sources of process spread — is often more powerful, because a narrower, more consistent process is more predictable and more controllable.

```python
# Quality control: defect rate as a function of sigma level
import math

def normal_cdf_approximation(z: float) -> float:
    """Approximate the CDF of the standard normal distribution."""
    # Abramowitz and Stegun approximation, accurate to 5 decimal places
    t = 1.0 / (1.0 + 0.2316419 * abs(z))
    poly = t * (0.319381530 +
                t * (-0.356563782 +
                t * (1.781477937 +
                t * (-1.821255978 +
                t * 1.330274429))))
    cdf = 1.0 - (1.0 / math.sqrt(2 * math.pi)) * math.exp(-0.5 * z * z) * poly
    return cdf if z >= 0 else 1.0 - cdf

def defect_rate_at_sigma_level(sigma: float) -> float:
    """
    Fraction of output outside ±sigma standard deviations from the mean.
    """
    above = 1 - normal_cdf_approximation(sigma)
    below = normal_cdf_approximation(-sigma)
    return above + below

print("Process quality at different sigma levels:")
print(f"{'Sigma':>8} {'Defect Rate':>15} {'Defects per million':>20}")
print("-" * 45)
for sigma in [1, 2, 3, 4, 5, 6]:
    rate = defect_rate_at_sigma_level(sigma)
    dpm = rate * 1_000_000
    print(f"{sigma:>8}  {rate:>14.6%}  {dpm:>18.1f}")
```

The table that code produces reveals the dramatic nonlinearity of the normal distribution's tails. Moving from three sigma (which catches 99.73% of output) to four sigma (which catches 99.9937%) more than halves the defect rate. Moving to six sigma reduces it by another factor of several thousand. Achieving very low defect rates requires extremely tight process control — and understanding process variation statistically is the only way to get there.

---

## Part 7: Standard Deviation in Investment and Risk

Risk in financial contexts is very often measured as the standard deviation of returns. An investment with high return volatility — large swings up and down — has a high standard deviation. An investment with consistently small returns in a narrow range has a low standard deviation.

This is why the Sharpe ratio — one of the most commonly used metrics for evaluating investment performance — divides the excess return of an investment (return above a risk-free rate) by the standard deviation of returns. It answers the question: "how much return are you getting per unit of risk taken?" A high Sharpe ratio means you are being well-compensated for the volatility you are accepting. A low Sharpe ratio means you are taking on a lot of volatility for relatively little extra return.

```python
# Sharpe ratio calculation
def sharpe_ratio(annual_returns: list[float], risk_free_rate: float = 0.04) -> float:
    """
    Compute annualized Sharpe ratio.
    annual_returns: list of annual returns as decimals (e.g. 0.12 for 12%)
    """
    n = len(annual_returns)
    mean_return = sum(annual_returns) / n
    excess_return = mean_return - risk_free_rate
    variance = sum((r - mean_return)**2 for r in annual_returns) / (n - 1)
    std_dev = variance ** 0.5

    if std_dev == 0:
        return float('inf')
    return excess_return / std_dev

# Portfolio A: moderate return, low volatility
portfolio_a_returns = [0.09, 0.11, 0.08, 0.10, 0.09, 0.10, 0.11, 0.09]

# Portfolio B: higher average return, much higher volatility
portfolio_b_returns = [0.25, -0.10, 0.30, -0.15, 0.20, 0.05, 0.35, -0.05]

a_mean = sum(portfolio_a_returns) / len(portfolio_a_returns)
b_mean = sum(portfolio_b_returns) / len(portfolio_b_returns)
a_sharpe = sharpe_ratio(portfolio_a_returns)
b_sharpe = sharpe_ratio(portfolio_b_returns)

print(f"Portfolio A: avg return = {a_mean:.1%}, Sharpe = {a_sharpe:.2f}")
print(f"Portfolio B: avg return = {b_mean:.1%}, Sharpe = {b_sharpe:.2f}")
print(f"\nPortfolio B has higher average return but lower Sharpe ratio.")
print(f"The extra return is not enough to compensate for the extra volatility.")
```

Understanding that the standard deviation of returns is a measure of risk — not just a statistical curiosity — changes how you think about investment performance claims. A fund that reports "twenty percent annual returns" is providing information that is almost meaningless without the standard deviation of those returns. If those twenty percent returns came with a standard deviation of thirty percent, there is a meaningful probability of losing twenty percent or more in any given year. If they came with a standard deviation of three percent, the returns are reliably near twenty percent.

---

## Part 8: When Standard Deviation Fails

Standard deviation is a powerful tool, but it has important limitations that are easy to overlook.

**It assumes a symmetric distribution.** Standard deviation is most interpretable when the data is approximately symmetric (like a normal distribution). For heavily right-skewed data — income distributions, asset values, the size of forest fires — the standard deviation can be very large relative to the mean, and the 68-95-99.7 rule does not apply. A single extreme outlier can dramatically inflate the standard deviation while leaving the median unchanged.

**It summarizes spread in a single number.** Just as the mean collapses the center of a distribution into one number, standard deviation collapses the spread into one number. This loses information about the shape. Two datasets with identical means and identical standard deviations can look very different — one might be unimodal and symmetric, another bimodal, another skewed. Summary statistics are always incomplete descriptions of data.

**It is sensitive to outliers.** Because standard deviation involves squaring deviations, extreme values (outliers) contribute disproportionately to the standard deviation. A single very large value can inflate the standard deviation dramatically, making the data appear much more variable than it is for the typical case.

For these reasons, it is often more appropriate to use the interquartile range — the range from the 25th to the 75th percentile — as a measure of spread for skewed or outlier-prone data. The interquartile range is robust to outliers (they are in the tails and do not affect the middle fifty percent of the distribution) and does not require any distributional assumption.

---

## Part 9: What Comes Next

We have now completed the conceptual scaffold. Standard deviation and variance are the measures of spread that underlie almost every other statistical concept in this series — the distribution shapes, the significance tests, the confidence intervals, the quality control frameworks, the risk measures, and the anomaly detection systems. With these tools in hand, everything we have discussed should now feel more integrated and more concrete.

In the tenth and final installment — our capstone — we bring the full series together. We will integrate every major concept, provide a master reference checklist for statistical reasoning in any context, catalog the complete list of manipulation techniques with their counters, and close with a reflection on what it means to be a statistically literate citizen in a world where numbers are used to shape opinion, policy, and power.

---

## Sources and Further Reading

- Freedman, David, Pisani, Robert, and Purves, Roger. *Statistics*. Fourth edition. W.W. Norton, 2007. The standard undergraduate text for non-mathematicians; the treatment of standard deviation in Part III is exceptionally clear and requires no mathematical background.

- Pande, Peter, Neuman, Robert, and Cavanagh, Roland. *The Six Sigma Way*. McGraw-Hill, 2000. The standard practitioner guide to Six Sigma quality methodology, with extensive discussion of process variation and standard deviation in manufacturing contexts.

- Sharpe, William F. "Mutual Fund Performance." *Journal of Business*, 39(1), 1966. The original paper introducing the Sharpe ratio; concise and accessible.

- Taleb, Nassim Nicholas. *The Black Swan*. Random House, 2007. Extensively discusses the limits of standard deviation as a risk measure when distributions have fat tails; essential context for why the 68-95-99.7 rule fails in financial markets.

- Tukey, John W. *Exploratory Data Analysis*. Addison-Wesley, 1977. The original presentation of robust statistics tools including the interquartile range and box-and-whisker plots as alternatives to mean and standard deviation for non-normal data.
