# Series Plan — “Nepal’s Budget 2026-27 in Plain English”

**A 20-part, first-principles, fact-checked explainer of Nepal’s federal budget for fiscal year 2083/84 (2026/27 AD).**

- **Author identity:** `mercifulpotato-team`
- **Series field (identical on all 20 posts):** `Nepal's Budget 2026-27 in Plain English`
- **Publishing window:** **2026-07-15 → 2026-08-03** — 20 consecutive daily posts
- **Status of this document:** planning / execution outline. It is **not** a blog post and is **not** committed to `content/blog/`.

---

## 0. How to use this document

A future session executes this one post at a time. In the established workflow a terse prompt — “day one”, “please continue from post 3 onward” — means *write that full post in full*, carrying all of the context below without re-briefing. Everything needed to keep 20 posts consistent (schedule, exact front matter, the verified number base, the recurring devices, the source map) lives here so no session re-derives conventions or invents figures.

**Nothing in the application needs to change to host this series.** The codebase already supports `series`, `featured`, lowercase-hyphenated `tags`, future-dated scheduling, and RSS. This series is *content only* — markdown files dropped into `content/blog/`.

---

## 1. Editorial premise & the non-negotiable lens

The goal is **not** to reprint the budget speech. It is to teach a complete novice — someone who may hold confident misconceptions about everything — how to understand a national budget *and* how to judge this one on the merits. Every post must:

1. **Build from first principles.** Assume the reader knows nothing and assume prior *mis*-knowledge. Define everything the first time it appears: what Nepal is, what money is, what a budget is, what Parliament is, what “recurrent expenditure”, a “tariff”, or a “fiscal deficit” means.
2. **Be exhaustive and patient.** Ultra-long-form prose (target 10,000–20,000+ words per post). No “and so on”, no “etc.” Expand rather than compress. Body paragraphs are prose, not bullet lists.
3. **Be balanced and skeptical — never promotional.** Do not assume the budget is good, that the new government is competent, that past governments were better, or that the opposition would do better. Give upsides *and* downsides of doing (or not doing) each thing. Fact-check every claim, including claims by people in authority. Be especially skeptical of round, flattering, or politically convenient numbers.
4. **Use both currencies, always.** Every monetary amount appears in **NPR and USD**. Close every post with the assumed exchange rate and any assumptions made (§2, §5).
5. **Arm the reader against spin.** Every post includes a **“How budgets deceive”** breakdown — the specific accounting or rhetorical trick relevant to that topic, and how a citizen, journalist, auditor, or developer can catch it.
6. **Ground every concept in a concrete story or worked example**, plus at least one **data artifact** for technical readers (a clean Markdown table, a JSON representation of figures, or runnable pseudo-code an auditor/developer could point at published data).
7. **Cite primary sources.** Prefer the Ministry of Finance (MoF) budget speech and Red Book, the Finance Bill 2083, the Economic Survey 2082/83, and Nepal Rastra Bank (NRB) over newspaper summaries. Close every post with a Sources / Further Reading section.
8. **English only.** No Devanagari in the body. Nepali terms (“Jestha”, “arba”, “khali”, “Shrawan”) are introduced once, transliterated, explained in plain English, then used in English.

> Structure mirrors the proven house style (e.g. *Probability and Statistics in Plain English*, *The Alternative Development Finance Act: Nepal’s Future in Plain English*): numbered `##` parts, `###` subsections, mandatory worked examples, concrete case studies, a recurring “how to spot the lie” device, and a synthesis capstone.

---

## 2. Assumptions & editorial decisions (state in Part 1; recap in every post)

**Exchange rate (series-wide assumption):** `1 USD = NPR 150`.

- A deliberately round figure close to the NRB reference rate in mid-June 2026, when the open-market USD/NPR rate sat around **NPR 150.8–153.2**. The NPR does not float against the dollar: it is **pegged to the Indian rupee at a fixed `1 INR = 1.60 NPR`**, so USD/NPR only moves when USD/INR moves. Because the rate drifts daily, **all USD figures are approximations for intuition, not accounting.** Say so every time.
- Convention: divide NPR by 150 for USD. Show **NPR first** (the budget’s native unit), USD second in parentheses. Round USD sensibly.

**Corrections to the brief (applied with best judgment; logged for transparency).** The brief carried leftover values from an earlier template:

- **Posts: 20**, not “ten.” The topic line says “twenty posts … over twenty days”, twice.
- **Start date: 2026-07-15**, not “2026-06-05” and not the example’s “2026-06-25.” The stated *Publish date: 2026-07-15* is used and is chronologically correct: the prior series ran through 2026-07-14, and FY 2083/84 begins 16 July 2026 — so the series opens exactly as the fiscal year starts.
- **“No Greek letters / math formulas”** is honored in spirit: percentages, ratios, growth rates explained in plain English with analogies, never as formulas.
- **“How to spot the lie”** is adapted to budgets: “How budgets deceive” / “How to read this like an auditor.”

**Framing assumptions:**
- Figures are **nominal NPR** unless explicitly inflation-adjusted.
- “The budget” means a *proposal* until the **Finance Bill 2083** and **Appropriation Bill** pass Parliament and receive Presidential authentication. Keep **“announced ≠ enacted ≠ implemented”** visible throughout.
- Sectoral/ministry figures below come from multiple secondary sources (newspapers + the ICAN/PKF highlights) and **must be cross-checked against the official MoF Red Book and Finance Bill 2083 during drafting.** Where a figure is uncertain, say so rather than assert false precision.

---

## 3. The verified fact base (single source of truth for all 20 posts)

USD computed at `1 USD = NPR 150`. **Cross-check every figure against the official MoF documents in the project before publishing the post that uses it.** Items marked ⚠ have a known discrepancy or nuance to resolve while drafting.

### 3.1 Headline aggregates (FY 2083/84 / 2026-27)

| Item | NPR | USD (≈, ÷150) | Notes |
|---|---|---|---|
| **Total budget outlay** | 2,124.34 billion | ≈ 14.16 billion | “Largest in Nepal’s history” (nominal) |
| Recurrent (current) expenditure | 1,270.58 billion | ≈ 8.47 billion | 59.8% of total |
| Capital expenditure | 431.10 billion | ≈ 2.87 billion | 20.3% of total |
| Financing / financial management | 422.64 billion ⚠ | ≈ 2.82 billion | 19.9%; one outlet reported 422.24 — verify |
| Revenue (tax + non-tax) | 1,405.31 billion | ≈ 9.37 billion | the collection *target* |
| Foreign grants | 61.74 billion | ≈ 0.41 billion | |
| Foreign loans | 247.28 billion | ≈ 1.65 billion | |
| Domestic borrowing (gross) | 410.00 billion | ≈ 2.73 billion | |
| Domestic principal repaid (same year) | 245.89 billion | ≈ 1.64 billion | |
| **Net new domestic borrowing** | 164.11 billion | ≈ 1.09 billion | 410.00 − 245.89 |
| **Deficit / financing gap** | 657.29 billion | ≈ 4.38 billion | expenditure − revenue − grants |

### 3.2 The “25.2% increase” — the most abused statistic in the budget

- New budget NPR 2,124.34B is **+25.2% over the *revised* estimate** of FY 2082/83.
- FY 2082/83’s **original** budget was **NPR 1.964 trillion**. A 25.2% rise over the revised figure implies the revised figure was **≈ NPR 1.70 trillion** ⚠ (verify exact number) — i.e. last year’s budget was cut mid-year as spending and revenue fell short.
- So the same budget is **only ≈ +8.2% over last year’s *original* budget.** This base-effect gap recurs in Parts 5, 6, 19.

### 3.3 Macro targets vs reality

| Metric | Target / figure | Reality check |
|---|---|---|
| Real GDP growth target | 7.0% | CBS estimates FY 2082/83 growth at **3.85%** |
| Inflation ceiling | 6.0% | a *target*, not a guarantee |
| FY 2083/84 dates | Shrawan 1, 2083 → end Ashadh 2084 | **16 July 2026 → ~16 July 2027** |
| Presented | Jestha 15, 2083 | **29 May 2026**, joint session of Parliament |

### 3.4 Ministry-wise allocations (reported; verify against Red Book) ⚠

| Ministry | NPR (billion) | USD (≈) |
|---|---|---|
| Physical Infrastructure & Transport | 302.83 | ≈ 2.02 billion |
| Education & Sports | 218.30 | ≈ 1.46 billion |
| Women, Children & Social Welfare | 122.61 | ≈ 0.82 billion |
| Energy, Water Resources & Irrigation | 114.02 | ≈ 0.76 billion |
| Home Affairs | 108.32 | ≈ 0.72 billion |
| Health & Population (ministry) | 96.43 | ≈ 0.64 billion |
| Finance | 84.73 | ≈ 0.56 billion |
| Agriculture, Forestry & Environment | 73.12 | ≈ 0.49 billion |
| Defence | 64.96 | ≈ 0.43 billion |
| Land Management, Cooperatives & Poverty Alleviation | 14.94 | ≈ 100 million |
| Culture, Tourism & Civil Aviation | 10.53 | ≈ 70 million |
| Industry, Commerce & Supplies | 9.34 | ≈ 62 million |
| Foreign Affairs | 8.73 | ≈ 58 million |
| Information & Communications | 5.93 | ≈ 40 million |
| Science, Technology & Innovation | 4.00 | ≈ 27 million |
| Youth, Labour & Employment | 3.62 | ≈ 24 million |
| Law, Justice & Parliamentary Affairs | 0.58 | ≈ 3.9 million |
| **“Miscellaneous” / undisclosed (Finance)** ⚠ | 90.42 | ≈ 0.60 billion |

> ⚠ The **health “sector” total (≈ NPR 101.95B, ≈ USD 0.68B)** differs from the **Health & Population *ministry* figure (NPR 96.43B)**. Keep “sector” vs “ministry” distinct (Part 15). The same care applies to other sectors that span multiple ministries.

### 3.5 Fiscal-federalism transfers

| Transfer | Provinces (NPR) | Local governments (NPR) |
|---|---|---|
| Fiscal equalization grants | 61.50 billion (≈ USD 0.41B) | 90.20 billion (≈ USD 0.60B) |
| Conditional grants | 39.72 billion (≈ USD 0.26B) | 206.08 billion (≈ USD 1.37B) |
| Total transfers (all types) | **> 600 billion combined (≈ USD 4.0B)** | |

### 3.6 Finance Bill 2083 — the statutory tax changes

- **Personal income tax:** exemption threshold **doubled, NPR 500,000 → NPR 1,000,000** (≈ USD 3,333 → ≈ USD 6,667). Top marginal rate **cut 10 points, 39% → 29%.** Intermediate slab structure is being published via Inland Revenue Department (IRD) circulars and the Finance Act 2083 schedule ⚠ (do not invent the band table; cite circulars).
- **Capital gains tax** on the sale of **listed** securities **made final** (no separate settlement).
- **Corporate tax:** standard **25%**; **30%** for banks, insurance, telecom, tobacco, alcohol (unchanged).
- **Customs:** tariff structure **collapsed from 11 tiers to 7**; duty **reduced on 273 categories of industrial raw materials** so raw-material duty stays at least one band below finished goods.
- **Excise:** **abolished on 360 goods.**
- **VAT (standard 13%):** **10% instant VAT refund for digital payments**; **5% VAT on electricity consumption above 50 units**; **5% VAT on ride-hailing services**; a **universal VAT-bill lottery** (a formalization nudge).
- **Green Tax:** a new levy **consolidating the former infrastructure development tax and the road maintenance fee** collected at customs.
- **Export & tech incentives:** **50% income-tax exemption on export income**; **50% exemption for IT-export income**; an **IT “sweat-equity” exemption**.

### 3.7 Spending, pay, and structural reform highlights

- **Civil service pay:** base scale **+10%**, plus a **monthly incentive allowance of 10%** of the new scale → **≈ +21% net**, effective **Shrawan 1, 2083 (16 July 2026)**; minimum remuneration set around **NPR 40,000** (≈ USD 267).
- **Right-sizing the state:** federal ministries **22 → 18**; **31 agencies dissolved, 6 merged, 6 transferred, 18 restructured**; estimated savings **≈ NPR 20 billion** (≈ USD 133 million) ⚠ (the savings number is an estimate — treat skeptically).
- **Signature programs (verify scope/cost during drafting):** the country’s first **sovereign AI compute centre**; **NPR 4B** for science/technology/innovation; **NPR 500M** Nepal Enterprise Facility (≈ USD 3.3M); **90% health-insurance coverage within 3 years**; **336 basic hospitals**; **free childhood-cancer treatment** in government hospitals; **Dalit/child nutrition allowance doubled to NPR 1,000/month** (≈ USD 6.67); **foreign citizens permitted to buy apartments** (conditional); **non-resident Nepalis** allowed into the **secondary securities market**; **Visit Nepal Year 2085** and **Nepal Health Year 2087**; **agriculture grants up to 40% of initial capital** for investments **≥ NPR 2 crore** (≈ USD 133,000); **Nepal Telecom share sale**; a **National Asset Management Company** (a bad-debt vehicle); an **“Investment Express” one-stop system**; **East–West Highway four-lane upgrade** and **Pushpalal Mid-Hill Highway** completion targets.

### 3.8 The political context (verified)

- **September 2025:** “Gen Z” protests → **PM K. P. Sharma Oli resigns**; the President (on government recommendation) announces early elections.
- **5 March 2026:** early general election for all **275 House of Representatives** seats.
- **Result:** the **Rastriya Swatantra Party (RSP)** wins a landslide — **182 seats**, ~**47.8%** proportional vote (the highest PR share since the 2008 system began), the **first single-party majority since 1999.** **Balen (Balendra) Shah** — former Kathmandu mayor, who joined RSP as a senior leader on 28 December 2025 — is the party’s PM candidate and becomes Prime Minister; he defeated Oli in Jhapa-5. **Rabi Lamichhane** leads the party. **Nepali Congress** suffered its worst-ever defeat (38 seats); **CPN-UML** its worst (25 seats, Oli losing his seat); the **Nepali Communist Party** (Dahal) won 17.
- **Finance Minister:** economist **Dr. Swarnim Wagle.**
- **Why this matters editorially:** this is the **first budget of a brand-new, untested government** elected on a youth-driven anti-establishment mandate. That cuts both ways — fresh political will and a clean slate, *but* no governing track record, sky-high expectations, and strong incentives to over-promise. This is the central tension the series interrogates (especially Parts 3 and 19).

---

## 4. Front matter & file conventions (must match the platform exactly)

The content processor parses YAML front matter with YamlDotNet (camelCase keys, unmatched keys ignored). Recognized keys: `title, date, author, summary, tags, series, featured` (plus optional `updated, image, draft`). The slug is the filename **with the `YYYY-MM-DD-` prefix stripped**.

**Template for a non-featured post (Parts 2–19):**

```yaml
---
title: "Recurrent vs Capital: The Most Important Distinction in the Whole Budget"
date: 2026-07-20
author: mercifulpotato-team
summary: "Part six of our plain-English series: what recurrent and capital spending actually are, why Nepal spends 60 percent on running the state and only 20 percent on building it, and how to tell consumption dressed up as investment."
tags:
  - nepal
  - nepal-budget
  - fiscal-year-2083-84
  - public-finance
  - capital-expenditure
  - plain-english
series: "Nepal's Budget 2026-27 in Plain English"
---
```

**Template for a featured post (Part 1 and Part 20 ONLY):** identical, but add a single line `featured: true`.

**Hard rules (house conventions, confirmed in the codebase):**
- `author` is the hyphenated id **`mercifulpotato-team`** — never the display name.
- **Only Part 1 and Part 20** carry `featured: true`. All mid-series posts **omit the `featured` line entirely** — never write `featured: false`.
- **Never** add `draft: true` (it would hide the post).
- Tags are **lowercase and hyphenated**. Use the anchor tags `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `plain-english` on most posts, plus 2–4 topic tags.
- Any string field containing a colon-space (`: `) — almost every `title` and `summary` here — **must be wrapped in double quotes**.
- `date` is the publish date (see schedule). Future-dated posts stay hidden until that date arrives (the daily 6 AM UTC rebuild publishes them).
- **Filename pattern:** `content/blog/YYYY-MM-DD-<slug>.md`. The `<slug>` is the per-part slug listed in §6/§7.

---

## 5. Recurring elements every post must contain

Every one of the 20 posts ends with these two blocks, in this order:

**(a) Sources / Further Reading** — a `## Sources and Further Reading` section. Cite primary documents first (MoF budget speech FY 2083/84, Finance Bill 2083, Economic Survey 2082/83, Red Book, NRB monetary policy and forex), then reputable secondary coverage. Do not make links clickable inside code blocks (keep them plain so copy-paste is clean).

**(b) Assumptions & exchange rate** — a short `## A Note on Numbers and Assumptions` block that always states:
- `1 USD = NPR 150` (mid-June 2026 NRB-area rate; NPR is pegged to INR at 1.60; the rate drifts daily, so USD figures are approximations).
- Figures are nominal NPR from the FY 2083/84 budget speech and Finance Bill 2083 unless stated otherwise.
- “Announced” is not “enacted” is not “implemented.”
- Any post-specific assumption or unresolved figure (e.g. an ⚠ item still being verified).

In addition, **inside the body** each post must include:
- **At least one “How budgets deceive” box** (the post’s spot-the-spin device).
- **At least one data artifact** (Markdown table, JSON block, or pseudo-code).
- **At least one concrete case study / worked scenario** grounded in Nepal.
- **NPR and USD** on every monetary figure.

---

## 6. The 20-day publishing schedule

| # | Date | Title | Slug | Featured |
|---|---|---|---|---|
| 1 | 2026-07-15 | What Even Is a National Budget? Nepal, Money, and the Machinery of the State | `nepal-budget-2026-1-what-is-a-budget` | ✅ |
| 2 | 2026-07-16 | The Fiscal Year, the Constitution, and the Calendar | `nepal-budget-2026-2-fiscal-year-and-calendar` | — |
| 3 | 2026-07-17 | Who Is in the Room? Parliament, Government, and the People Who Wrote This Budget | `nepal-budget-2026-3-who-wrote-this-budget` | — |
| 4 | 2026-07-18 | How a Budget Becomes Law: From Speech to Finance Act | `nepal-budget-2026-4-how-a-budget-becomes-law` | — |
| 5 | 2026-07-19 | The Big Number, Dissected: NPR 2.12 Trillion | `nepal-budget-2026-5-the-big-number` | — |
| 6 | 2026-07-20 | Recurrent vs Capital: The Most Important Distinction | `nepal-budget-2026-6-recurrent-vs-capital` | — |
| 7 | 2026-07-21 | Where the Money Comes From, Part 1: Revenue & Remittances | `nepal-budget-2026-7-revenue-and-remittances` | — |
| 8 | 2026-07-22 | Where the Money Comes From, Part 2: Grants, Loans & the Deficit | `nepal-budget-2026-8-deficit-and-debt` | — |
| 9 | 2026-07-23 | Three Governments, One Country: Fiscal Federalism | `nepal-budget-2026-9-fiscal-federalism` | — |
| 10 | 2026-07-24 | Income Tax for Humans: The Biggest Tax Cut in a Decade | `nepal-budget-2026-10-income-tax` | — |
| 11 | 2026-07-25 | VAT, Customs, and Excise: The Taxes You Pay Without Noticing | `nepal-budget-2026-11-vat-customs-excise` | — |
| 12 | 2026-07-26 | Open for Business? Investment, Company Law & the Reform Promises | `nepal-budget-2026-12-business-and-investment` | — |
| 13 | 2026-07-27 | Power and the Planet: Energy, Hydropower & the New Green Tax | `nepal-budget-2026-13-energy-and-green-tax` | — |
| 14 | 2026-07-28 | Building the Country: Infrastructure & the Capital-Spending Problem | `nepal-budget-2026-14-infrastructure` | — |
| 15 | 2026-07-29 | Health and Education: Investing in People (or Promising To) | `nepal-budget-2026-15-health-and-education` | — |
| 16 | 2026-07-30 | The Safety Net: Social Protection, Pensions & Who Gets Left Out | `nepal-budget-2026-16-social-protection` | — |
| 17 | 2026-07-31 | Agriculture, Tourism, and the Productivity Question | `nepal-budget-2026-17-agriculture-and-tourism` | — |
| 18 | 2026-08-01 | The Future Pitch: AI, Startups & the “Digital Nepal” Story | `nepal-budget-2026-18-ai-and-startups` | — |
| 19 | 2026-08-02 | Can They Actually Do It? Credibility, Execution & Reading Like an Auditor | `nepal-budget-2026-19-credibility-and-execution` | — |
| 20 | 2026-08-03 | The Citizen’s Field Guide to Nepal’s Budget 2026-27 | `nepal-budget-2026-20-citizens-field-guide` | ✅ |

*Logical arc:* Foundations (1–4) → The numbers (5–9) → The tax system (10–13) → The sectors (14–18) → Critical synthesis (19) → Capstone field guide (20).

---

## 7. The 20 parts in detail

Each block below is the writing brief for one post. `Filename` = `content/blog/<date>-<slug>.md`.

---

### Part 1 — “What Even Is a National Budget? Nepal, Money, and the Machinery of the State”
- **Date / slug / featured:** 2026-07-15 / `nepal-budget-2026-1-what-is-a-budget` / **featured: true**
- **Filename:** `content/blog/2026-07-15-nepal-budget-2026-1-what-is-a-budget.md`
- **summary:** `"The opening of a twenty-part series that explains Nepal's 2026-27 federal budget from absolute first principles: what Nepal is, what money and a government actually are, what a national budget is for, and why a single document worth NPR 2.12 trillion (about USD 14.2 billion) shapes the life of every person in the country."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `public-finance`, `plain-english`, `series-introduction`
- **Sections (`###`):** What this series is and is not · What is Nepal? (geography, ~30 million people, landlocked between India and China, low-income, federal republic since 2015) · What is money, and what is the Nepalese rupee? (the INR peg at 1.60) · What is a government, and what does it do with money? · What is a budget? (a plan, a law, and a political statement all at once) · Why a budget every single year · The 2026-27 budget at a glance · How to read this series (dual currency, skepticism, first principles)
- **First-principles concepts:** state, public goods, taxation, expenditure, currency peg, nominal vs real, “trillion”/“arba”/“khali” translated.
- **Key figures:** total NPR 2,124.34B (≈ USD 14.16B); per-capita budget ≈ NPR 70,000 (≈ USD 470) — compute and explain.
- **Balanced/critical angle:** a big number is not the same as a good plan; size ≠ delivery.
- **How budgets deceive:** “the headline total” — why the biggest-ever framing is nearly meaningless without inflation and execution context (preview of Part 5).
- **Case study:** trace one rupee of tax from a shopkeeper in Pokhara to a teacher’s salary, to show the budget touching real lives.
- **Data artifact:** a JSON object of the top-line budget (`total`, `recurrent`, `capital`, `financing`, each with `npr_billion` and `usd_billion`).
- **Primary sources:** MoF budget speech FY 2083/84; NRB forex; Economic Survey 2082/83.

---

### Part 2 — “The Fiscal Year, the Constitution, and the Calendar: Why Nepal’s Budget Arrives on Jestha 15”
- **Date / slug:** 2026-07-16 / `nepal-budget-2026-2-fiscal-year-and-calendar`
- **Filename:** `content/blog/2026-07-16-nepal-budget-2026-2-fiscal-year-and-calendar.md`
- **summary:** `"Part two: why Nepal's budget year runs from mid-July to mid-July, how the Bikram Sambat calendar maps onto 2026-27, why the Constitution forces the budget out on Jestha 15 (29 May), and what an Economic Survey and a 'revised estimate' actually are."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `constitution`, `public-finance`, `plain-english`
- **Sections:** Two calendars (Bikram Sambat vs Gregorian) and how to convert · What a fiscal year is and why it isn’t the calendar year · FY 2083/84 = 16 July 2026 → ~16 July 2027 · The constitutional deadline (Article 119) and why a fixed date exists · The Economic Survey (28 May 2026) as the budget’s factual prologue · “Original” vs “revised” estimates vs “actuals” · Why timing changes everything (procurement, monsoon, the spending year)
- **First-principles concepts:** fiscal year, constitutional mandate, estimate vs actual, the budget cycle.
- **Key figures:** dates; the revised-estimate nuance from §3.2.
- **Balanced/critical angle:** a fixed presentation date is good discipline; but on-time presentation says nothing about on-time *spending*.
- **How budgets deceive:** “the base year” — comparing against a quietly shrunken revised estimate to inflate the growth headline.
- **Case study:** why a road project funded on 16 July often cannot break ground until after the monsoon, compressing the real spending window.
- **Data artifact:** a small table mapping BS ↔ AD dates for the budget cycle; pseudo-code for a BS→AD fiscal-year converter.
- **Primary sources:** Constitution of Nepal (Art. 119); Economic Survey 2082/83; MoF.

---

### Part 3 — “Who Is in the Room? Parliament, Government, and the People Who Wrote This Budget”
- **Date / slug:** 2026-07-17 / `nepal-budget-2026-3-who-wrote-this-budget`
- **Filename:** `content/blog/2026-07-17-nepal-budget-2026-3-who-wrote-this-budget.md`
- **summary:** `"Part three: a plain-English tour of how Nepal is governed, then the human story behind this budget: the Gen Z protests of 2025, the fall of the old order, the March 2026 landslide that made the Rastriya Swatantra Party and Balen Shah the government, and why the people in the room shape what is in the document."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `nepal-politics`, `governance`, `plain-english`
- **Sections:** What a parliament is · Nepal’s bicameral Federal Parliament (House of Representatives, 275; National Assembly, 59) · How a government forms; the Council of Ministers; the President’s ceremonial role · What the Finance Minister and the Ministry of Finance do · The actors and their motives: Gen Z protests → Oli’s resignation → 5 March 2026 election → RSP’s 182-seat landslide → Balen Shah as PM, Rabi Lamichhane as party leader, Dr. Swarnim Wagle as Finance Minister · The decimated opposition (NC 38, UML 25, NCP 17) and why a weak opposition is itself a fiscal risk · Whose interests a budget serves
- **First-principles concepts:** legislature vs executive, coalition vs majority, mandate, separation of powers.
- **Key figures:** seat counts; vote shares (§3.8).
- **Balanced/critical angle:** present every actor skeptically — the new government’s mandate is real but untested; a supermajority reduces scrutiny; do not lionize the protests or demonize the old guard.
- **How budgets deceive:** “the mandate halo” — treating an electoral win as proof that the numbers add up.
- **Case study:** contrast the political incentives of a first-budget government with strong expectations vs an outgoing one.
- **Data artifact:** a table of parties → seats → PR vote share.
- **Primary sources:** Election Commission of Nepal; the 2026 general-election record; MoF.

---

### Part 4 — “How a Budget Becomes Law: From Speech to Finance Act”
- **Date / slug:** 2026-07-18 / `nepal-budget-2026-4-how-a-budget-becomes-law`
- **Filename:** `content/blog/2026-07-18-nepal-budget-2026-4-how-a-budget-becomes-law.md`
- **summary:** `"Part four: the process almost nobody explains. The difference between the budget speech, the Appropriation Bill, and the Finance Bill 2083; how a bill becomes the Finance Act 2083; and why 'announced' is a world away from 'law' and even further from 'implemented'."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `finance-bill-2083`, `public-finance`, `plain-english`
- **Sections:** Three documents, not one (speech vs Appropriation Bill vs Finance Bill) · What the Finance Bill 2083 contains and how it becomes the Finance Act 2083 · Parliamentary stages: debate, amendments, votes, Presidential authentication · IRD circulars and why the real tax tables appear after the speech · The public-finance scaffolding (Appropriation Act, National Debt Act, Loan and Guarantee Act) · The watchdogs: Auditor General, Financial Comptroller General Office (FCGO), Public Accounts Committee · Where a citizen finds the real documents
- **First-principles concepts:** appropriation, statute vs speech, authentication, oversight institutions.
- **Key figures:** none headline; reference the verified totals being *appropriated*.
- **Balanced/critical angle:** strong process on paper; weak enforcement and chronic audit backlogs in practice.
- **How budgets deceive:** “announcement as achievement” — counting a promised program as if it already exists.
- **Case study:** a popular announcement that lapses because the enabling law or circular never lands within the fiscal year.
- **Data artifact:** a pseudo-code state machine `Announced → Tabled → Amended → Passed → Authenticated → Implemented`.
- **Primary sources:** Finance Bill 2083; Appropriation Bill; Office of the Auditor General; FCGO.

---

### Part 5 — “The Big Number, Dissected: NPR 2.12 Trillion and What ‘Largest in History’ Really Means”
- **Date / slug:** 2026-07-19 / `nepal-budget-2026-5-the-big-number`
- **Filename:** `content/blog/2026-07-19-nepal-budget-2026-5-the-big-number.md`
- **summary:** `"Part five: taking apart the headline. What NPR 2,124.34 billion (about USD 14.2 billion) means in real terms, why 'largest in history' is almost always true and almost always misleading, the difference between nominal and real growth, and why what governments budget is not what they spend."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `public-finance`, `data-literacy`, `plain-english`
- **Sections:** Reading a “trillion” (and arba/khali) · Nominal vs real: stripping out inflation · “Largest in history” as a near-meaningless claim · Budget vs actual: Nepal’s chronic under-spending · The +25.2% over revised vs +8.2% over original gap · Per-capita and share-of-GDP framings · A skeptic’s checklist for any headline budget number
- **First-principles concepts:** nominal vs real, base effects, execution/absorption rate, per-capita normalization.
- **Key figures:** NPR 2,124.34B (≈ USD 14.16B); revised vs original (§3.2); per-capita ≈ NPR 70,000 (≈ USD 470).
- **Balanced/critical angle:** ambition is legitimate; but the credibility of a record budget rests on a spending record that has historically disappointed.
- **How budgets deceive:** the full anatomy of the “record budget” claim.
- **Case study:** compare last year’s announced vs revised vs likely-actual to show the shrinkage.
- **Data artifact:** a table: original / revised / new, with NPR, USD, and the two percentage framings.
- **Primary sources:** MoF; Economic Survey; NRB.

---

### Part 6 — “Recurrent vs Capital: The Most Important Distinction in the Whole Budget”
- **Date / slug:** 2026-07-20 / `nepal-budget-2026-6-recurrent-vs-capital`
- **Filename:** `content/blog/2026-07-20-nepal-budget-2026-6-recurrent-vs-capital.md`
- **summary:** `"Part six: what recurrent and capital spending actually are, why Nepal spends about 60 percent running the state and only 20 percent building it, what the 'financing' third really covers, and how to spot consumption dressed up as investment."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `public-finance`, `capital-expenditure`, `plain-english`
- **Sections:** Recurrent (salaries, operations, interest) · Capital (roads, schools, durable assets) · Financing/financial management (debt service, principal repayment, on-lending) · The 59.8 / 20.3 / 19.9 split and why it worries economists · The civil-service pay rise (+21% net) and the recurrent ratchet · The ministry-merger “≈ NPR 20B savings” claim, examined · Why capital budgets routinely go unspent
- **First-principles concepts:** consumption vs investment, fixed capital formation, debt service, structural rigidity.
- **Key figures:** recurrent 1,270.58B (≈ 8.47B USD); capital 431.10B (≈ 2.87B USD); financing 422.64B ⚠; pay +21%; savings ≈ NPR 20B ⚠.
- **Balanced/critical angle:** salaries and services matter and aren’t “waste”; but a low and under-executed capital share limits future growth.
- **How budgets deceive:** “investment” labels on recurrent items; counting transfers as capital.
- **Case study:** a school “built” on paper where most of the line item was salaries and meetings.
- **Data artifact:** stacked breakdown table + JSON; a pseudo-code “classify line item: recurrent or capital?”
- **Primary sources:** Red Book; MoF; FCGO execution data.

---

### Part 7 — “Where the Money Comes From, Part 1: Revenue, Taxes, and the Remittance Economy”
- **Date / slug:** 2026-07-21 / `nepal-budget-2026-7-revenue-and-remittances`
- **Filename:** `content/blog/2026-07-21-nepal-budget-2026-7-revenue-and-remittances.md`
- **summary:** `"Part seven: how Nepal actually raises NPR 1.4 trillion (about USD 9.4 billion). Tax versus non-tax revenue, why so much of it rides on imports and therefore on remittances, and why cutting tax rates while raising the revenue target is a bet that needs scrutiny."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `taxation`, `remittances`, `plain-english`
- **Sections:** What government revenue is · Tax vs non-tax revenue · Nepal’s heavy reliance on customs and import-linked VAT · The remittance engine (over a quarter of GDP) and how it funds the treasury indirectly · Why a consumption/import-based revenue base is fragile · The tension: lower rates + higher target · A realism check on the NPR 1,405.31B goal
- **First-principles concepts:** revenue base, elasticity, import dependence, remittances → consumption → tax.
- **Key figures:** revenue NPR 1,405.31B (≈ USD 9.37B); remittances ~USD 9–11B/yr and >25% of GDP (verify latest NRB number).
- **Balanced/critical angle:** broadening the base via formalization is sound; relying on optimistic collection is risky.
- **How budgets deceive:** “revenue optimism” — targets set above trend to make the deficit look smaller.
- **Case study:** how a slowdown in remittance-fed imports can blow a hole in customs/VAT receipts.
- **Data artifact:** a revenue-composition table; pseudo-code projecting revenue from an import-growth assumption.
- **Primary sources:** MoF revenue tables; NRB remittance/BoP data; IRD; Customs Department.

---

### Part 8 — “Where the Money Comes From, Part 2: Grants, Loans, and the Deficit”
- **Date / slug:** 2026-07-22 / `nepal-budget-2026-8-deficit-and-debt`
- **Filename:** `content/blog/2026-07-22-nepal-budget-2026-8-deficit-and-debt.md`
- **summary:** `"Part eight: the gap between what Nepal plans to spend and what it expects to collect, about NPR 657 billion (USD 4.4 billion), and how it is filled with foreign grants, foreign loans, and domestic borrowing. What public debt is, who Nepal owes, and when borrowing becomes dangerous."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `public-debt`, `fiscal-deficit`, `plain-english`
- **Sections:** What a deficit is (in a household and in a state) · Grants vs loans · Foreign vs domestic borrowing · Net new domestic borrowing after repayment · What public debt is; debt-to-GDP; concessional vs commercial · Who Nepal owes (World Bank/IDA, ADB, bilateral) · Crowding out and interest as a recurrent cost · When debt is sustainable and when it isn’t
- **First-principles concepts:** deficit, debt stock vs flow, concessionality, debt service, crowding out, sustainability.
- **Key figures:** deficit ≈ NPR 657.29B (≈ USD 4.38B); grants 61.74B; foreign loans 247.28B; domestic 410B gross / 164.11B net.
- **Balanced/critical angle:** borrowing to build assets can be wise; borrowing to fund salaries is not; the mix matters.
- **How budgets deceive:** “the hidden deficit” — labels and off-budget items that understate true borrowing.
- **Case study:** how interest on past loans quietly eats the recurrent budget.
- **Data artifact:** a financing-waterfall table (revenue → grants → loans → borrowing = total); JSON of the financing plan.
- **Primary sources:** MoF financing tables; Public Debt Management Office; NRB; IMF/World Bank debt assessments.

---

### Part 9 — “Three Governments, One Country: Fiscal Federalism and the Money That Flows Down”
- **Date / slug:** 2026-07-23 / `nepal-budget-2026-9-fiscal-federalism`
- **Filename:** `content/blog/2026-07-23-nepal-budget-2026-9-fiscal-federalism.md`
- **summary:** `"Part nine: Nepal has not one government but three tiers, and over NPR 600 billion (about USD 4 billion) flows from the centre to provinces and 753 local governments. What fiscal federalism is, how the grants work, and why dividing money across three levels is both empowering and expensive."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `federalism`, `local-government`, `plain-english`
- **Sections:** What federalism is · Nepal’s three tiers (federal, 7 provinces, 753 local units) since 2015 · The four transfer types (equalization, conditional, special, supplementary) · Revenue sharing · Double-taxation disputes and overlapping mandates · The Governance Innovation Challenge Fund · Why federalism costs money and how it can still be worth it
- **First-principles concepts:** unitary vs federal, vertical/horizontal fiscal imbalance, conditional vs unconditional grants, subsidiarity.
- **Key figures:** transfers > NPR 600B (≈ USD 4.0B); equalization 61.5B/90.2B; conditional 39.72B/206.08B (§3.5).
- **Balanced/critical angle:** decentralization brings services closer to people but multiplies overheads and disputes.
- **How budgets deceive:** “decentralization theatre” — announcing transfers while centralizing real control through conditions.
- **Case study:** a local government that receives a conditional grant it cannot spend because the conditions don’t fit local needs.
- **Data artifact:** a transfers matrix (tier × grant type); pseudo-code splitting a notional pool across tiers.
- **Primary sources:** National Natural Resources and Fiscal Commission; MoF intergovernmental fiscal transfer tables.

---

### Part 10 — “Income Tax for Humans: The Biggest Tax Cut in a Decade”
- **Date / slug:** 2026-07-24 / `nepal-budget-2026-10-income-tax`
- **Filename:** `content/blog/2026-07-24-nepal-budget-2026-10-income-tax.md`
- **summary:** `"Part ten: income tax explained without a single formula. Why the doubled exemption to NPR 1 million (about USD 6,667) and the top-rate cut from 39 to 29 percent is the largest personal-tax reset in over a decade, who really benefits, and what it costs the treasury."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `income-tax`, `finance-bill-2083`, `plain-english`
- **Sections:** What income tax is · Slabs/brackets and the difference between marginal and effective rates (no formulas) · The doubled exemption and the 39→29 top-rate cut · Who gains most (and why those earning under NPR 10 lakh already paid little) · Capital gains on listed shares made final · The revenue cost vs the stimulus argument · Equity: is this progressive or regressive?
- **First-principles concepts:** marginal vs effective rate, exemption threshold, progressivity, tax incidence, capital gains.
- **Key figures:** exemption 500k→1,000,000 (≈ USD 3,333→6,667); top rate 39%→29%; CGT-final on listed shares.
- **Balanced/critical angle:** real relief for the salaried middle class and a formalization incentive; but the largest absolute gains accrue to higher earners, and it costs revenue during a deficit.
- **How budgets deceive:** “relief for everyone” framing that hides who gets the biggest cheque.
- **Case study:** three taxpayers (NPR 8 lakh, 20 lakh, 60 lakh incomes) — who saves how much, in NPR and USD.
- **Data artifact:** a before/after table by income band; pseudo-code computing tax under old vs new thresholds (with a note that the intermediate slab table must come from IRD circulars).
- **Primary sources:** Finance Bill 2083; IRD circulars; Income Tax Act 2058.

---

### Part 11 — “VAT, Customs, and Excise: The Taxes You Pay Without Noticing”
- **Date / slug:** 2026-07-25 / `nepal-budget-2026-11-vat-customs-excise`
- **Filename:** `content/blog/2026-07-25-nepal-budget-2026-11-vat-customs-excise.md`
- **summary:** `"Part eleven: the invisible taxes that fund most of the state. How VAT, customs duties, and excise work; why collapsing customs from 11 tiers to 7 and abolishing excise on 360 goods matters; and the new digital-payment VAT refund, electricity VAT, ride-hailing VAT, and Green Tax."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `vat`, `customs-and-excise`, `plain-english`
- **Sections:** VAT (13%) from first principles — a tax on consumption, not income · Customs/tariffs and the 11→7 tier simplification; the 273 raw-material cuts · Excise and the abolition on 360 goods · New VAT moves: 10% instant refund on digital payments; 5% on electricity above 50 units; 5% on ride-hailing; the VAT-bill lottery · The Green Tax (infrastructure development tax + road maintenance fee consolidated) · Why consumption taxes are regressive
- **First-principles concepts:** consumption tax, tariff/protection, cascading, regressivity, formalization nudges.
- **Key figures:** VAT 13%; customs 11→7; 273 raw materials; excise on 360 goods; the four VAT measures; Green Tax (§3.6).
- **Balanced/critical angle:** simplification and digital nudges are genuinely good; but new VAT on electricity contradicts electrification goals and consumption taxes hit the poor hardest.
- **How budgets deceive:** “simplification” that quietly raises the effective burden; “green” labels on revenue grabs.
- **Case study:** a household’s monthly bill — phone, electricity, a ride, groceries — recomputed under the new rules.
- **Data artifact:** a table of each indirect-tax change with direction of effect; pseudo-code applying VAT/customs to a sample invoice.
- **Primary sources:** Finance Bill 2083; Customs Tariff 2083/84; VAT Act 2052; Excise Act 2058.

---

### Part 12 — “Open for Business? Investment, Company Law, and the Reform Promises”
- **Date / slug:** 2026-07-26 / `nepal-budget-2026-12-business-and-investment`
- **Filename:** `content/blog/2026-07-26-nepal-budget-2026-12-business-and-investment.md`
- **summary:** `"Part twelve: the budget's business-climate package. Corporate tax rates, the 50 percent export exemption, company-law and insolvency reforms, a new limited-liability-partnership law for venture capital, the Investment Express one-stop system, and why reform-by-announcement so often stalls in Nepal."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `investment`, `business-reform`, `plain-english`
- **Sections:** Corporate tax (25% standard; 30% for banks/insurance/telecom/tobacco/alcohol) · The 50% export-income exemption · Company Law amendments (conflict of interest, easier dissolution) · A Limited Liability Partnership law for angel/VC/PE · Insolvency Act amendments for consumers and MSMEs · Bilateral Investment Protection Agreements and Double Taxation Avoidance Agreements · Investment Express one-stop system; foreign apartment purchase; NRN secondary-market access · The implementation gap
- **First-principles concepts:** corporate vs personal tax, limited liability, insolvency/bankruptcy, FDI, ease of doing business.
- **Key figures:** corporate 25%/30%; export exemption 50%; foreign apartment purchase (conditional) (§3.6/3.7).
- **Balanced/critical angle:** a coherent pro-investment agenda; but Nepal’s binding constraints (electricity, logistics, policy stability, enforcement) outlast any single budget.
- **How budgets deceive:** “reform-by-announcement” — laws promised but not drafted, agencies promised but not staffed.
- **Case study:** a would-be foreign investor walking through the one-stop promise vs the real approval chain.
- **Data artifact:** a table of each reform with status (announced / bill needed / circular needed); pseudo-code of an “approval workflow”.
- **Primary sources:** Finance Bill 2083; Companies Act; Insolvency Act; Investment Board Nepal; FITTA.

---

### Part 13 — “Power and the Planet: Energy, Hydropower, and the New Green Tax”
- **Date / slug:** 2026-07-27 / `nepal-budget-2026-13-energy-and-green-tax`
- **Filename:** `content/blog/2026-07-27-nepal-budget-2026-13-energy-and-green-tax.md`
- **summary:** `"Part thirteen: Nepal's biggest economic bet, electricity. The NPR 114 billion (about USD 760 million) energy allocation, hydropower expansion and exports, and the contradiction at the heart of taxing electricity above 50 units while branding a new Green Tax as climate policy."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `hydropower`, `green-tax`, `plain-english`
- **Sections:** Why electricity is Nepal’s comparative advantage (rivers, hydropower) · The Energy/Water/Irrigation allocation · Generation, transmission, and electricity exports to India (and the Bangladesh route) · The 5% VAT on electricity above 50 units — and why it cuts against electrification · The Green Tax: what it consolidates and whether it is climate policy or revenue · Climate vulnerability (glaciers, floods, GLOFs) vs climate finance
- **First-principles concepts:** comparative advantage, base-load vs peak, transmission constraints, externalities, Pigouvian taxes.
- **Key figures:** Energy ministry NPR 114.02B (≈ USD 0.76B); 5% electricity VAT >50 units; Green Tax (§3.6).
- **Balanced/critical angle:** hydropower ambition is real and exportable; but taxing consumption while urging electrification, plus a “Green Tax” that is mostly a renamed road levy, invites skepticism.
- **How budgets deceive:** “green-washing” a revenue measure; counting MW announced vs MW commissioned.
- **Case study:** a household crossing the 50-unit threshold; a stalled transmission line bottlenecking generation.
- **Data artifact:** a table of energy line items; pseudo-code computing the electricity-VAT step at 50 units.
- **Primary sources:** MoF energy allocation; Nepal Electricity Authority; Department of Electricity Development; NRB.

---

### Part 14 — “Building the Country: Infrastructure, Roads, and the Capital-Spending Problem”
- **Date / slug:** 2026-07-28 / `nepal-budget-2026-14-infrastructure`
- **Filename:** `content/blog/2026-07-28-nepal-budget-2026-14-infrastructure.md`
- **summary:** `"Part fourteen: the single largest ministry allocation, NPR 302.83 billion (about USD 2.02 billion) for physical infrastructure. The flagship highways and rail dreams, and Nepal's chronic inability to spend its capital budget on time, on cost, and on quality."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `infrastructure`, `capital-expenditure`, `plain-english`
- **Sections:** Why infrastructure is the biggest ministry · Flagships: East–West Highway four-laning, Pushpalal Mid-Hill Highway, rail/metro studies, airports · The capital-absorption problem (allocations unspent, year-end rushes, cost overruns) · Why projects stall: readiness, procurement, land acquisition, contractor capacity, monsoon · Quality vs quantity · Donor-funded vs domestically funded works
- **First-principles concepts:** capital project lifecycle, absorption capacity, procurement, cost overrun, asset quality.
- **Key figures:** Infrastructure NPR 302.83B (≈ USD 2.02B); capital total 431.10B (≈ USD 2.87B).
- **Balanced/critical angle:** the need is enormous and the allocation rational; but allocation has never been Nepal’s problem — *execution* is.
- **How budgets deceive:** “ribbon-cutting accounting” — re-announcing the same project across multiple budgets.
- **Case study:** a highway that appears in successive budgets with shifting completion dates.
- **Data artifact:** a table of flagship projects (allocation, target date, status); pseudo-code of a year-end “spend-down” detector.
- **Primary sources:** MoF; Department of Roads; National Planning Commission; FCGO execution reports.

---

### Part 15 — “Health and Education: Investing in People (or Promising To)”
- **Date / slug:** 2026-07-29 / `nepal-budget-2026-15-health-and-education`
- **Filename:** `content/blog/2026-07-29-nepal-budget-2026-15-health-and-education.md`
- **summary:** `"Part fifteen: the human-capital budget. About NPR 102 billion (USD 0.68 billion) for health and NPR 218 billion (USD 1.46 billion) for education, the promise of 90 percent health-insurance coverage and 336 hospitals, free childhood-cancer care, and why counting rupees is not the same as counting outcomes."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `health`, `education`, `plain-english`
- **Sections:** Health sector (≈ NPR 101.95B) vs the Health ministry (NPR 96.43B) — why the distinction matters · Health insurance to 90% in 3 years; NPR 15B insurance; 336 basic hospitals; free childhood-cancer treatment; safe motherhood · Education (NPR 218.30B); scholarships (NPR 8.60B); foreign universities in Nepal; Open University; residential schools; Geta MBBS · Inputs vs outcomes (coverage, learning, health results) · Recurrent capture (most of these budgets are salaries)
- **First-principles concepts:** human capital, inputs vs outcomes, universal coverage, unit cost, public vs private provision.
- **Key figures:** health ≈ NPR 101.95B (≈ USD 0.68B) / ministry 96.43B; education 218.30B (≈ USD 1.46B); insurance NPR 15B; scholarships NPR 8.60B.
- **Balanced/critical angle:** ambitious and humane targets; but coverage promises have slipped before, and money ≠ quality.
- **How budgets deceive:** “input-counting” — celebrating rupees and buildings rather than results.
- **Case study:** an insurance scheme with high enrolment but low actual utilization or claim payment.
- **Data artifact:** a table of health/education line items; pseudo-code converting an allocation into a per-capita figure.
- **Primary sources:** MoF; Ministry of Health & Population; Ministry of Education; Health Insurance Board.

---

### Part 16 — “The Safety Net: Social Protection, Pensions, and Who Gets Left Out”
- **Date / slug:** 2026-07-30 / `nepal-budget-2026-16-social-protection`
- **Filename:** `content/blog/2026-07-30-nepal-budget-2026-16-social-protection.md`
- **summary:** `"Part sixteen: the budget's promises to the vulnerable. Social-security allowances, the doubled Dalit and child nutrition allowance to NPR 1,000 a month (about USD 6.67), labour reforms and a migrant returnee program, and the hard trade-off between universal allowances and a sustainable budget."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `social-protection`, `labour`, `plain-english`
- **Sections:** What social protection is · Existing allowances (elderly, single women, disability) and their fiscal weight · The doubled Dalit/child nutrition allowance (NPR 1,000/month); women/children/gender minorities (NPR 2.27B); disability rehab; autism schools; street-children and senior support · Labour: registry, minimum wage, insurance, workplace safety, bank-paid wages; returnee-migrant program; women health-volunteer travel +50% · Targeting vs universality · Who falls through the cracks
- **First-principles concepts:** social protection vs charity, universal vs targeted transfers, leakage, fiscal entitlements.
- **Key figures:** nutrition allowance NPR 1,000/mo (≈ USD 6.67); WCSW ministry NPR 122.61B; women/children NPR 2.27B; labour ≈ NPR 3.62–3.63B ⚠.
- **Balanced/critical angle:** allowances are a genuine lifeline; but universal, indexed entitlements harden the recurrent budget for decades.
- **How budgets deceive:** “announce-a-program” without an eligibility list, delivery channel, or sunset.
- **Case study:** an allowance that doubles on paper but doesn’t reach remote claimants without IDs or bank access.
- **Data artifact:** a table of allowances and beneficiaries; pseudo-code estimating annual cost from a per-beneficiary figure.
- **Primary sources:** MoF; Ministry of Women, Children & Senior Citizens; Social Security Fund; Ministry of Labour.

---

### Part 17 — “Agriculture, Tourism, and the Productivity Question”
- **Date / slug:** 2026-07-31 / `nepal-budget-2026-17-agriculture-and-tourism`
- **Filename:** `content/blog/2026-07-31-nepal-budget-2026-17-agriculture-and-tourism.md`
- **summary:** `"Part seventeen: the real economy most Nepalis live in. The NPR 73 billion (about USD 490 million) agriculture allocation, a grant covering up to 40 percent of capital for larger farm investments, the tourism push toward Visit Nepal Year 2085, and why subsidies so often reward the well-connected."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `agriculture`, `tourism`, `plain-english`
- **Sections:** Why agriculture dominates livelihoods but not output · The Agriculture/Forestry/Environment allocation · The up-to-40% capital grant for investments ≥ NPR 2 crore — who can actually qualify · Food imports and the productivity gap · Tourism: Visit Nepal Year 2085; the Culture/Tourism/Civil Aviation allocation · Subsidy design and elite capture
- **First-principles concepts:** productivity, subsidy incidence, elite capture, value chains, seasonality.
- **Key figures:** Agriculture ministry NPR 73.12B (≈ USD 0.49B); grant up to 40% for ≥ NPR 2 crore (≈ USD 133,000); Tourism ministry NPR 10.53B (≈ USD 70M).
- **Balanced/critical angle:** supporting commercial farming can raise output; but a high investment floor (NPR 2 crore) risks channeling public money to those who least need it.
- **How budgets deceive:** “pro-farmer” subsidies whose thresholds exclude most farmers.
- **Case study:** a smallholder vs an agribusiness applying for the same grant.
- **Data artifact:** a table of agri/tourism line items; pseudo-code checking grant eligibility against the NPR 2 crore floor.
- **Primary sources:** MoF; Ministry of Agriculture & Livestock Development; Nepal Tourism Board.

---

### Part 18 — “The Future Pitch: AI, Startups, Technology, and the ‘Digital Nepal’ Story”
- **Date / slug:** 2026-08-01 / `nepal-budget-2026-18-ai-and-startups`
- **Filename:** `content/blog/2026-08-01-nepal-budget-2026-18-ai-and-startups.md`
- **summary:** `"Part eighteen: the headline-grabbing future bets. A sovereign AI compute centre, NPR 4 billion (about USD 27 million) for science and innovation, a startup financing facility and IT-export incentives, plus the privatization moves, and a hard look at whether buzzwords can survive contact with Nepal's constraints."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `technology`, `startups`, `plain-english`
- **Sections:** The sovereign AI compute centre — what it would even require (power, chips, talent, capital) · NPR 4B for science/technology/innovation; NPR 500M Nepal Enterprise Facility · 50% IT-export exemption and the IT sweat-equity exemption; defining “startup” · Privatization and restructuring: Nepal Telecom share sale; Civil Aviation Authority split; National Asset Management Company (a bad-debt vehicle) · Buzzword budgeting vs deliverable policy
- **First-principles concepts:** compute/electricity/skills constraints, sweat equity, privatization, bad-bank/AMC, signalling.
- **Key figures:** STI NPR 4B (≈ USD 27M); Nepal Enterprise Facility NPR 500M (≈ USD 3.3M); IT-export exemption 50% (§3.6/3.7).
- **Balanced/critical angle:** the tech-and-startup framing is forward-looking and popular with the RSP’s base; but a “sovereign AI compute centre” is a tall order for a power- and capital-constrained economy, and risks being political marketing.
- **How budgets deceive:** “shiny-object” line items sized for headlines, not impact.
- **Case study:** what NPR 4B can and cannot buy in compute and talent terms (sanity-check the ambition).
- **Data artifact:** a table of tech/innovation/privatization items; pseudo-code sketching an AMC’s bad-loan intake.
- **Primary sources:** MoF; Ministry of Communication & IT; Ministry of Science, Technology & Innovation; Nepal Telecom.

---

### Part 19 — “Can They Actually Do It? Credibility, Execution, and How to Read a Budget Like an Auditor”
- **Date / slug:** 2026-08-02 / `nepal-budget-2026-19-credibility-and-execution`
- **Filename:** `content/blog/2026-08-02-nepal-budget-2026-19-credibility-and-execution.md`
- **summary:** `"Part nineteen: the reckoning. Nepal's chronic gap between budgeted and spent, the opaque NPR 90 billion (about USD 600 million) miscellaneous heading, a brand-new government with no track record, a 7 percent growth target against 3.85 percent reality, and the complete auditor's toolkit for reading any budget."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `accountability`, `data-literacy`, `plain-english`
- **Sections:** The execution gap (capital spend often well below allocation; revenue often below target) · The “miscellaneous/undisclosed” NPR 90.42B and why opacity matters · A new government: clean slate vs zero track record · The 7% vs 3.85% credibility gap · Mid-year revisions and how budgets quietly shrink · **The full “How to read a budget like an auditor” toolkit:** record-size framing, base effects, execution/absorption rate, off-budget items, revenue optimism, consumption-as-investment, reform-by-announcement, allocated ≠ spent ≠ delivered, per-capita and share-of-GDP normalization
- **First-principles concepts:** credibility, transparency, absorption, variance analysis, accountability institutions.
- **Key figures:** miscellaneous NPR 90.42B (≈ USD 0.60B) ⚠; growth 7% vs 3.85%; the §3.2 base-effect.
- **Balanced/critical angle:** the fairest verdict is conditional — good intentions, real reforms, but a delivery system that has repeatedly underperformed; judge by execution data, not speeches.
- **How budgets deceive:** the consolidated catalogue of every trick from Parts 1–18, in one reference.
- **Case study:** walk the reader through evaluating one flagship promise end-to-end using public data.
- **Data artifact:** an “auditor’s checklist” table; pseudo-code computing an execution rate (`actual / allocated`) and flagging year-end spikes.
- **Primary sources:** Office of the Auditor General; FCGO; National Planning Commission; NRB; MoF mid-term review.

---

### Part 20 — “The Citizen’s Field Guide to Nepal’s Budget 2026-27: Everything You Now Know”
- **Date / slug / featured:** 2026-08-03 / `nepal-budget-2026-20-citizens-field-guide` / **featured: true**
- **Filename:** `content/blog/2026-08-03-nepal-budget-2026-20-citizens-field-guide.md`
- **summary:** `"The capstone: a complete, standalone field guide to Nepal's 2026-27 budget. A master glossary of every term, a one-page numbers reference in NPR and USD, a citizen's checklist for tracking whether promises are kept, a twelve-month watch calendar, and a balanced final verdict on the good, the risky, and the doubtful."`
- **tags:** `nepal`, `nepal-budget`, `fiscal-year-2083-84`, `public-finance`, `capstone`, `plain-english`
- **Sections:** How to use this guide · **Master glossary** (recurrent, capital, financing, deficit, debt, VAT, excise, customs tiers, marginal vs effective rate, equalization/conditional grant, absorption rate, nominal vs real, concessional loan, and more) · **The numbers, on one page** (every key figure from §3 in NPR and USD) · **The citizen’s checklist** (where to find execution data, AG reports, FCGO, NRB, MoF; how to track a specific promise) · **The 12-month watch calendar** (Nepal Telecom share sale ~Jan 2027; the mid-term review; IRD circulars; AG report timing) · **Balanced verdict:** the genuinely good ideas, the genuinely risky bets, and the things to stay skeptical about
- **First-principles concepts:** consolidation of every concept; this post must be readable entirely on its own.
- **Key figures:** the full §3 reference set, restated in NPR and USD.
- **Balanced/critical angle:** a fair, non-partisan closing assessment that neither cheerleads nor dismisses.
- **How budgets deceive:** a compact one-screen “spot-the-spin” cheat sheet pulling together all prior devices.
- **Case study:** a worked “track this promise yourself” walkthrough the reader can repeat for any line item.
- **Data artifact:** the master glossary table + a complete JSON snapshot of the budget for developers.
- **Primary sources:** the consolidated source list from the whole series.

---

## 8. Source-document map (project PDFs → topics)

Pull verified figures from these primary documents during drafting; cross-check newspapers against them, never the reverse. Read large PDFs incrementally (ranged reads).

| Topic / Part | Primary documents in the project |
|---|---|
| Budget speech, themes, all aggregates (Parts 1, 5, 6, all) | `Budget_Speech_2026_27_English_Translation_ysmw9xe.pdf`; `budgetprastab208384.pdf` |
| Highlights & ministry/function tables (Parts 5–18) | `Federal_Budget_2083_84_Highlights_V1.pdf`; `1780322646_PKF__TRU_Nepal_Budget_Statement_Highlights_202627.pdf`; `RCAW6379_18BudgetHighlights.pdf` |
| Tax law changes — income tax, VAT, customs, excise, Green Tax (Parts 10–13) | `FinanceBill2083KeyTaxChangesandLegalImplicationsforBusinessesinNepal2026infinitynp_com.pdf`; `AmendmentinTaxLawsbyBudget208283.pdf` (prior-year baseline); `1750310698_Tax_Fact_20252026.pdf` (prior-year baseline) |
| Independent analysis & critique (Parts 5, 19) | `NepalBudget2083_84Analysis.pdf`; `ssrn6857318.pdf`; `289202506011026915.pdf` |
| Monetary policy & macro context — inflation, rates, forex, remittances (Parts 7, 8, 13) | `MonetarypolicyinEnglish2025_26.pdf` (FY 2082/83 MP); `Monetary_Policy_2082083_English.pdf`; `MonetaryPolicyinEnglishfor202425.pdf` (older baselines) |
| Prior-year budget baselines for base-effect comparisons (Parts 2, 5, 6) | `budget_overview_8283.pdf`; `BKAG_Concise_Budget_Highlights_FY_208283.pdf`; `budgetupdate2082withtaxamendment20250616160234.pdf`; `2083_Baisakh_GDS_i116eqf.pdf`; `20260530120701trn30may1.pdf`; `6720cef296e9d935e76ada04resources1749799827777.pdf`; `1nplea2026002.pdf`; `publicationdocument1657798279.pdf` |

> Several PDFs are FY 2082/83 (2025/26) documents — use them only as the **prior-year baseline** for comparisons, not as the 2026/27 budget itself. Always note which fiscal year a figure belongs to.

**Web sources to corroborate (cite primary first):** Ministry of Finance (mof.gov.np) budget statement FY 2083/84; Nepal Rastra Bank (nrb.org.np) forex and monetary policy; Office of the Auditor General; FCGO; Election Commission of Nepal; reputable outlets (The Himalayan Times, The Kathmandu Post, Nepalnews, Radio Nepal). Treat any single secondary figure as provisional until checked against MoF.

---

## 9. Automation & build / deploy notes

The platform already does everything required; the series rides the existing pipeline.

- **Write** each post as `content/blog/YYYY-MM-DD-<slug>.md` with the front matter in §4.
- **Future-dating is the publishing mechanism.** The content processor skips posts dated after “now (UTC)”, and the deploy workflow runs daily at **06:00 UTC** to publish anything whose date has arrived. So committing all 20 future-dated files now causes them to appear automatically, one per day, 2026-07-15 → 2026-08-03.
- **Process content locally** to verify before commit:
  `dotnet run --project tools/ObserverMagazine.ContentProcessor -- --content-dir content/blog --output-dir src/ObserverMagazine.Web/wwwroot --authors-dir content/authors`
- **RSS** is generated automatically into `feed.xml` (summary → `<description>`, full HTML → `content:encoded`, tags → `<category>`). No action needed.
- **Markdown features available** (Markdig `UseAdvancedExtensions`): pipe tables (auto-wrapped for horizontal scroll on mobile), fenced code blocks with language hints (use ```text, ```json, ```python, ```csharp), footnotes, blockquotes, definition lists. Keep links **outside** code fences so copy-paste stays clean.
- **Recommended verification sequence before committing a batch** (mirrors the project’s norm):
  `dotnet format` → `dotnet restore` → run the content processor → `dotnet test` → `dotnet list package` → `bash export.sh`.
- **Author** `mercifulpotato-team` already exists in `content/authors/`. No new author file is needed.
- **No code or workflow change is required for this series.**

---

## 10. Reusable per-post generation prompt

Paste this (filling the bracketed fields from §6/§7) at the start of each “day N” session:

```text
Write Part N of the "Nepal's Budget 2026-27 in Plain English" series for Merciful Potato Magazine.

Part N: [title]
File: content/blog/[date]-[slug].md
Featured: [true only for Part 1 and Part 20; otherwise omit the featured line]

Rules (non-negotiable):
- Front matter exactly per the plan: author mercifulpotato-team; series "Nepal's Budget 2026-27 in Plain English";
  lowercase-hyphenated tags; quote any title/summary containing a colon-space; never featured:false; never draft:true.
- First principles, assume zero prior knowledge and likely misconceptions; English only.
- Ultra-long-form prose (10,000-20,000+ words), no bullet lists in body paragraphs, no "etc."
- Balanced and skeptical; cover upsides and downsides; fact-check every claim against the MoF budget speech,
  Finance Bill 2083, Economic Survey 2082/83, and NRB before asserting a figure.
- Every monetary amount in NPR and USD at 1 USD = NPR 150.
- Include: at least one "How budgets deceive" box, one data artifact (table/JSON/pseudo-code),
  and one concrete Nepal case study.
- End with: "## Sources and Further Reading" then "## A Note on Numbers and Assumptions"
  (restating the exchange rate, nominal-NPR caveat, announced != enacted != implemented, and any unresolved figures).
- Use only the verified figures in the plan's fact base; flag anything not yet cross-checked rather than inventing it.

Read the relevant primary PDFs (see the source map) with ranged reads before writing, and read the full dump.txt
for current conventions. Then output the complete markdown file.
```

---

## 11. Open questions / things to verify during drafting

Resolve each against the official MoF documents in the project (do not guess in the published posts):

1. **Financing/financial-management total:** NPR **422.64B** vs the **422.24B** seen in one outlet — confirm from the Red Book.
2. **Revised FY 2082/83 estimate:** confirm the exact figure behind the “+25.2% over revised” claim (the ≈ NPR 1.70T implied in §3.2).
3. **Income-tax slab table:** the doubled exemption and 39→29 top rate are confirmed; the **intermediate band structure** must come from IRD circulars / the Finance Act 2083 schedule — do not invent it.
4. **Health “sector” (≈ 101.95B) vs Health ministry (96.43B):** keep separate; confirm both.
5. **Ministry allocations** in §3.4: cross-check every line against the Red Book; some outlets round differently.
6. **“≈ NPR 20B savings” from right-sizing** and the **NPR 90.42B miscellaneous** heading: treat both as government estimates/claims and verify scope.
7. **Remittance share of GDP** (Part 7) and **current forex reserves / import cover** (Parts 7–8): use the latest NRB figure, dated.
8. **Monetary policy for FY 2083/84:** NRB typically issues it after the budget (often mid-to-late July); if it is out by a given post’s publish date, incorporate it — otherwise cite the FY 2082/83 monetary policy and say so.
9. **Seat count nuance:** use the final tally (RSP 182 of 275); distinguish from live-coverage partials.
10. **Exchange-rate drift:** if NRB’s USD/NPR has moved materially from ~150 by publication, keep 150 as the stated assumption but note the then-current rate.

---

*End of plan. Next step: write Part 1 (`2026-07-15-nepal-budget-2026-1-what-is-a-budget.md`).*
