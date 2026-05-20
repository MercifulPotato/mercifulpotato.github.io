---
title: "Numbers in the Courtroom and the Capitol: How Statistics Are Used and Abused in Law and Policy"
date: 2026-06-22
author: mercifulpotato-team
summary: "The eighth installment of our plain-English statistics series: probabilistic evidence in criminal and civil trials, how policy evaluation works and what evidence is sufficient to change policy, the statistical basis for disparate-impact analysis in discrimination law, and the ways statistical arguments are weaponized to obstruct change."
tags:
  - statistics
  - law
  - policy
  - evidence
  - plain-english
  - data-literacy
  - discrimination
  - decision-making
series: "Probability and Statistics in Plain English"
---

## Part 1: Where Statistics Meet Power

Statistics in the courtroom and the legislative chamber are not the same beast as statistics in a research laboratory. The same numbers that a scientist presents as preliminary evidence in a professional journal become, in a legal or political context, instruments of argument, advocacy, and power. They are wielded by adversaries, interpreted by non-specialists, and used to justify decisions that affect people's lives, freedom, and livelihoods.

The statistical literacy required to navigate these contexts has a distinctive character. It is less about conducting analysis and more about evaluating arguments — understanding which claims the evidence supports, which it does not, and which interpretations are being advanced because they serve someone's interest rather than because they are the most honest reading of the data.

This installment covers four domains where statistics and power intersect with particular intensity: criminal evidence and probabilistic proof, civil liability and statistical causation, policy evaluation and evidence-based governance, and disparate-impact analysis in discrimination law.

---

## Part 2: Probabilistic Evidence in Criminal Trials

Criminal trials, in common-law systems, operate on the standard of "proof beyond a reasonable doubt." This phrase is deliberately vague — different jurisdictions have historically declined to quantify it precisely — but it is generally understood to mean a very high level of certainty: something like ninety-five to ninety-nine percent confidence in guilt.

The introduction of probabilistic evidence into this framework is fraught with difficulty, and the history of forensic statistics in criminal cases is marked by a succession of avoidable errors, unjust convictions, and exonerations.

### DNA Evidence: Powerful but Not Infallible

DNA profiling produces what appears to be highly precise probabilistic evidence: "the probability that a randomly selected person other than the defendant would match this DNA profile is one in ten billion." This sounds essentially conclusive. And in many cases, when properly understood, it is very strong evidence.

But several complications can erode its strength in specific cases.

**Laboratory error rates** are not included in the quoted probability. The one-in-ten-billion figure assumes the sample was correctly collected, correctly preserved, correctly analyzed, and correctly interpreted. Laboratory errors — contamination, mislabeling, analysis errors — occur at rates measurable in tens of cases per hundred thousand, which is small but far larger than one in ten billion. In any case where the match probability is very high, laboratory error is actually the more plausible explanation for a false positive than a chance population match.

**Database trawling** changes the statistical picture dramatically. If police collect DNA from a hundred thousand people in a population and test them all against a crime scene sample, they expect to find several people who match the profile by chance even if the crime scene profile is from an unknown person. The one-in-ten-billion statistic assumes the suspect was identified on independent grounds and then DNA-tested. When the DNA test was used to find the suspect in the first place — by running it against a database — the relevant probability is very different.

**Mixed samples** produce much weaker conclusions than single-contributor samples. A DNA sample from a surface touched by many people may be a mixture of multiple profiles, and interpretation of mixed profiles requires judgment calls that can be contested.

### The Prosecutor's Fallacy in Practice

We introduced the prosecutor's fallacy in Part 5, but it is worth revisiting in the legal context with the consequences in view.

In a real case in England in 1990, a man named Andrew Deen was convicted partly on the basis of a forensic scientist's testimony that the probability of a random match with the semen found on the victim was one in three million. The prosecution argued — incorrectly — that this meant there was only a one-in-three-million chance that the defendant was innocent.

This confuses the probability of the evidence given innocence with the probability of innocence given the evidence. What the one-in-three-million figure means is that if you tested three million random men, one would match by chance. In a country of sixty million men, roughly twenty men would match the profile. The evidence narrows suspicion from sixty million to roughly twenty — it is strong evidence, but it does not establish guilt to a one-in-three-million certainty. The prosecution's statement was wrong, and the Court of Appeal acknowledged the error when Deen's conviction was reviewed.

```
Correct interpretation of DNA match probability:
-------------------------------------------------
Random match probability: 1 in 3,000,000
UK male population: ~30,000,000 (1990s)
Expected random matches in population: ~10 men

The DNA evidence means: among all men who could have left this sample,
the suspect is one of roughly 10, not the only one.

Combined with other evidence (opportunity, motive, witness statements),
the posterior probability of guilt should be calculated using all evidence.
The DNA alone does not prove guilt to 1-in-3-million certainty.
```

### Eyewitness Testimony and Memory

While not statistical in the mathematical sense, the research on eyewitness reliability is statistical and directly relevant to legal evidence evaluation. Memory is not a recording device — it is a reconstructive process that is systematically influenced by post-event information, stress, cross-racial identifications, the confidence of the witness, and the conditions of identification procedures.

The research, summarized by psychologist Elizabeth Loftus over decades of work and confirmed by the Innocence Project's analysis of exoneration cases, shows that eyewitness misidentification is the single largest contributor to wrongful convictions, present in roughly seventy percent of cases where DNA later proved innocence.

A statistically literate juror, knowing this base rate, should treat eyewitness testimony with appropriate caution — not dismissal, but calibrated skepticism weighted by the conditions under which the identification was made, the opportunity the witness had to observe, and whether the identification procedure was appropriately conducted.

---

## Part 3: Statistical Evidence in Civil Litigation

Civil cases — particularly discrimination, antitrust, and product liability claims — rely heavily on statistical evidence to establish patterns that no single event can prove. This is where statistical proof of discrimination lives, and where the most sophisticated statistical arguments are contested.

### Disparate Impact in Employment Discrimination

Discrimination law in the United States (and many other jurisdictions) recognizes two theories of discrimination. Disparate treatment is intentional discrimination — treating someone differently because of their membership in a protected class. Disparate impact is unintentional discrimination — policies or practices that appear neutral but have a significantly different and adverse effect on members of a protected class.

Statistical evidence is central to disparate impact cases. A plaintiff who claims that a hiring test has disparate impact on racial minorities must typically show, through statistical analysis, that the test screens out minority applicants at a meaningfully higher rate than majority applicants.

The legal standard in the United States — the "eighty percent rule" or "four-fifths rule" — provides a simple bright line: if the selection rate for the protected group is less than eighty percent of the selection rate for the highest-selected group, there is a prima facie showing of adverse impact. This rule was formalized by the EEOC in 1978 and remains in wide use.

```python
# Disparate impact four-fifths rule calculation
def disparate_impact_analysis(
    majority_applicants: int,
    majority_selected: int,
    minority_applicants: int,
    minority_selected: int,
    group_labels: tuple[str, str] = ("Majority", "Minority")
) -> dict:
    """
    Calculate selection rates and the four-fifths (80%) rule.
    Returns whether adverse impact is indicated.
    """
    majority_rate = majority_selected / majority_applicants if majority_applicants > 0 else 0
    minority_rate = minority_selected / minority_applicants if minority_applicants > 0 else 0

    # Adverse impact ratio: minority rate / majority rate (if majority selects at higher rate)
    highest_rate = max(majority_rate, minority_rate)
    lower_rate = min(majority_rate, minority_rate)
    lower_group = group_labels[0] if majority_rate < minority_rate else group_labels[1]

    adverse_impact_ratio = lower_rate / highest_rate if highest_rate > 0 else 1.0
    adverse_impact_indicated = adverse_impact_ratio < 0.80

    return {
        f"{group_labels[0]} selection rate": f"{majority_rate:.1%}",
        f"{group_labels[1]} selection rate": f"{minority_rate:.1%}",
        "Adverse impact ratio": f"{adverse_impact_ratio:.2f}",
        "4/5 rule threshold": "0.80",
        "Adverse impact indicated": adverse_impact_indicated,
        "Lower-selected group": lower_group
    }

# Example: a written test for police officer positions
result = disparate_impact_analysis(
    majority_applicants=400,
    majority_selected=200,    # 50% selection rate
    minority_applicants=100,
    minority_selected=30,     # 30% selection rate
)
for k, v in result.items():
    print(f"{k}: {v}")
```

The four-fifths rule is a useful screening tool but has known limitations. It is sensitive to sample size — small samples can produce large rate differences by chance, and large samples can flag trivially small differences as adverse impact. Courts often require supplementary statistical analysis, including tests of statistical significance, alongside the ratio calculation.

Employers who have an adverse impact finding against them can justify a selection device by demonstrating its validity — showing through evidence that the test genuinely predicts job performance and that no less discriminatory alternative exists. This job-relatedness defense places the statistical burden on the employer to show that the test measures something real and necessary.

### Statistical Evidence in Product Liability

In mass tort litigation — lawsuits brought by thousands of plaintiffs against a manufacturer for alleged harm from a product — epidemiological evidence is often the primary basis for establishing general causation: the proposition that the product is capable of causing the claimed harm.

Establishing general causation requires showing, at minimum, that exposure to the product is associated with the harm at a level above what would be expected by chance, that the association is not explained by confounding or bias, and that the association is consistent with a plausible biological mechanism.

Courts have grappled extensively with how to evaluate this statistical evidence. The 1993 U.S. Supreme Court ruling in Daubert v. Merrell Dow Pharmaceuticals established a framework for evaluating scientific evidence in federal courts, requiring that expert testimony be based on methods that are testable, have been subjected to peer review and publication, have known error rates, and are generally accepted in the relevant scientific community.

The practical implication is that statistical evidence must meet scientific standards, not just legal advocacy standards. A hired expert who cherry-picks favorable studies, misapplies statistical tests, or reaches conclusions that the broader scientific community rejects will — if the opposing party is sophisticated — be subjected to vigorous challenge.

---

## Part 4: Policy Evaluation — What Evidence Should Change Policy

Democratic governments make policies based on assumptions about what those policies will accomplish. These assumptions range from evidence-based (well-supported by high-quality research) to ideological (assumed to work based on philosophical commitments) to politically motivated (designed to signal values regardless of effectiveness).

Statistical literacy is necessary for evaluating these claims — for understanding whether the evidence cited for a policy genuinely supports it, whether alternative evidence has been suppressed or ignored, and whether the claimed outcomes are measurable and are being measured honestly.

### The Counterfactual Problem in Policy

The fundamental challenge in policy evaluation is the counterfactual: what would have happened without the policy? This is logically impossible to observe directly — a country can either enact a policy or not, but it cannot do both simultaneously. This is why policy evaluation is so much harder than laboratory science.

Several methodological approaches attempt to address the counterfactual problem:

**Random policy assignment** — the direct equivalent of a randomized controlled trial in policy — is occasionally possible. Some countries have conducted explicit policy experiments, randomizing different regions or municipalities to receive different interventions. Some social programs have been deliberately randomized for evaluation purposes. These provide the strongest causal evidence, but they are rare and politically difficult.

**Regression discontinuity design** exploits sharp cutoff rules. If a program is available to all households below a certain income threshold and unavailable above it, households just below and just above the threshold are otherwise very similar — they differ primarily in whether they qualify for the program. Comparing outcomes just below the cutoff to outcomes just above it provides a quasi-experimental estimate of the program's effect.

**Difference-in-differences** compares outcome trends in places that adopted a policy versus places that did not, under the assumption that the trends would have been parallel absent the policy. The key assumption — parallel trends — is often untestable and sometimes wrong.

**Interrupted time series** looks for breaks in a time trend coinciding with a policy change. If a city enacted a distracted driving law and serious injury accidents then fell at a rate that broke from the prior trend, that break is evidence of effect — though other things that changed simultaneously may have contributed.

None of these methods is as clean as a true randomized experiment. All require assumptions that can be challenged. The statistically literate citizen should be skeptical of the single study cited as justification for a major policy change, and should look for convergent evidence from multiple designs.

### Evidence-Based Policy in Practice

The "evidence-based policy" movement, which emerged in the 1990s and accelerated in the 2000s, sought to apply the logic of evidence-based medicine to government programs: rigorously evaluate interventions with randomized designs when possible, synthesize evidence systematically, and base decisions on findings rather than on ideological preference or anecdote.

The What Works Clearinghouse (operated by the U.S. Department of Education) and the Campbell Collaboration (which conducts systematic reviews in social policy analogous to the Cochrane Collaboration in medicine) are institutional expressions of this approach. They establish explicit evidence standards, review available research systematically, and grade the strength of evidence behind specific interventions.

Even within the evidence-based policy framework, significant disagreements arise. How strong must the evidence be before adopting a policy? Who bears the burden of proof — advocates for change, or advocates for the status quo? How much weight should be given to the side effects and unintended consequences that randomized trials often do not measure? These are value questions that statistical methods can inform but cannot resolve.

---

## Part 5: How Statistics Are Weaponized to Obstruct Change

The same statistical sophistication that helps citizens evaluate evidence honestly has, throughout history, been deployed to manufacture false doubt about well-established scientific findings that threatened powerful economic interests.

This pattern — sometimes called the "tobacco playbook," after its most documented early application — follows a recognizable structure. A scientific consensus emerges that a product or practice is harmful. The industry with a financial stake in that product or practice commissions research designed to produce alternative findings, funds organizations that emphasize scientific uncertainty, lobbies to prevent regulation pending "more research," and attacks the methodology of studies that document harm.

The goal is not to prove the product is safe. The goal is to maintain the appearance of scientific controversy where none substantively exists, buying time and preventing regulatory action. Uncertainty is manufactured strategically.

Robert Proctor, a historian of science at Stanford, coined the term "agnotology" — the study of deliberate production of ignorance — to describe this phenomenon. His book *Golden Holocaust* documents how the tobacco industry deployed statistical arguments to delay regulation of cigarettes for decades after the scientific consensus on smoking and cancer was established.

The same pattern has been applied to leaded gasoline (funded by manufacturers of lead additives), asbestos, chlorofluorocarbons and the ozone layer, and climate science.

The statistical tools used in the obfuscation campaign are often technically sophisticated: emphasizing the range of confidence intervals rather than the point estimate, highlighting studies with null findings while dismissing studies that found effects, demanding impossible standards of proof ("we cannot say with certainty that..."), and conflating uncertainty in the magnitude of the effect with uncertainty about whether any effect exists.

### How to Recognize Manufactured Uncertainty

Ask: does the scientific debate occur primarily in peer-reviewed literature, or primarily in op-ed pages, industry-funded reports, and congressional testimony? Genuine scientific debates happen among scientists, using scientific methods, with findings subjected to peer review. Manufactured debates happen in public forums, using the rhetoric of science without its substance.

Ask: who is funding the dissenting research? A handful of scientists expressing doubt about a mainstream finding, when their research is funded by industries with financial stakes in the outcome, should be weighted very differently from independent researchers who reach the same conclusions.

Ask: is the uncertainty about whether the effect exists, or about its precise magnitude? Tobacco industry representatives argued for decades that science had "not proved" smoking causes cancer, exploiting the impossibility of absolute proof to suggest that no evidence existed at all. The relevant question was whether the evidence was strong enough to justify action — which it was.

---

## Part 6: Racial and Ethnic Disparities in the Criminal Justice System

Statistical analysis of disparities in criminal justice outcomes — arrest rates, prosecution rates, conviction rates, sentencing lengths — is both scientifically demanding and politically contentious. A complete treatment is beyond this installment's scope, but the statistical concepts we have developed apply directly.

Large and consistent disparities in outcomes by race have been documented across multiple stages of the criminal justice system across multiple countries. The policy-relevant question is not whether disparities exist — they do — but how much of each disparity is explained by differences in the underlying conduct being criminalized, how much by differences in policing patterns (which determine what conduct is observed), and how much by differential treatment at equivalent levels of conduct.

Answering these questions requires controlling for potential confounders — a methodologically challenging task, because the "underlying conduct" is itself often influenced by the factors being studied. Whether police presence is deployed more heavily in certain communities affects the observability of conduct, which affects the denominators of rate calculations, which affects how disparities are measured.

A statistically honest conversation about criminal justice disparities acknowledges all of these complexities without using them to dismiss findings of disparity. The existence of methodological complexity does not mean that no disparity exists or that the available evidence is uninformative. It means the evidence must be interpreted with appropriate nuance.

---

## Part 7: What Comes Next

We have now explored how statistical reasoning enters the domains of law and policy — where numbers serve as weapons in adversarial proceedings, where causal claims must meet evidentiary standards, and where strategic obfuscation can delay action on genuine harms.

In the ninth installment, we tackle the most technically forbidding concept in this series — standard deviation and variance — and render it completely accessible. These concepts lie beneath almost every statistical measure we have discussed, and understanding them in plain English will retroactively clarify much of what has come before.

---

## Sources and Further Reading

- Finkelstein, Michael O. and Levin, Bruce. *Statistics for Lawyers*. Third edition. Springer, 2015. The standard statistical reference for legal practitioners; highly rigorous treatment of the methods applicable to employment discrimination, antitrust, and criminal evidence.

- Loftus, Elizabeth. *Eyewitness Testimony*. Harvard University Press, 1979. The foundational scientific analysis of eyewitness memory unreliability; still the essential reference on this topic.

- Proctor, Robert N. *Golden Holocaust: Origins of the Cigarette Catastrophe and the Case for Abolition*. University of California Press, 2011. The comprehensive documentary history of the tobacco industry's statistical manipulation campaigns.

- Daubert v. Merrell Dow Pharmaceuticals, Inc., 509 U.S. 579 (1993). The Supreme Court ruling establishing the federal standards for expert scientific testimony; available at supremecourt.gov.

- Oreskes, Naomi and Conway, Erik M. *Merchants of Doubt*. Bloomsbury Press, 2010. Documents the manufactured uncertainty campaigns across tobacco, climate, and other industries; rigorous and highly readable.

- Equal Employment Opportunity Commission. *Uniform Guidelines on Employee Selection Procedures*, 29 C.F.R. Part 1607 (1978). The regulatory source of the four-fifths rule for adverse impact analysis; available at eeoc.gov.

- National Academy of Sciences, Engineering, and Medicine. *The Growth of Incarceration in the United States: Exploring Causes and Consequences*. National Academies Press, 2014. Rigorous systematic review of evidence on racial disparities in the U.S. criminal justice system.
