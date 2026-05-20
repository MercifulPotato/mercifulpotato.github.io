Manage project memory
Claude regenerates project memory every evening from your past chats in this project. Only you can see this memory, and it is not shared with other project users.

Purpose & context

Kushal runs Merciful Potato Magazine (mercifulpotato.github.io), a static blog built on Blazor WebAssembly (.NET 10). The site is used to publish long-form, deeply researched article series on a range of topics. Kushal uses Claude within a project environment to plan and execute full multi-part blog series, from series architecture through complete article generation, with minimal ongoing direction — typically just enough to initiate or resume work.

Current state

Two major series have been completed recently:

"The Foundation Beneath Everything" (10 parts, dated 2026-05-26 through 2026-06-04) — a deep-dive series on foundation construction types, covering slab/crawl space/basement, deep foundations, frost and waterproofing, regional case studies (Newport News VA, Dallas-Fort Worth, Kathmandu), and foundation repair.
"You Own a Home Now: The Complete First-Time Homeowner Survival Guide" (10 parts, dated 2026-06-05 through 2026-06-14) — a comprehensive survival guide for first-time homeowners covering maintenance, HVAC, appliances, landscaping, plumbing/electrical, finances, renovations, and a capstone with master checklist.
Both series immediately followed a completed 10-part Tolkien Middle-earth mythology series ("The Complete Chronological History of Middle-earth"), which ran to approximately 15,000–20,000 words per article with in-universe historical and scholarly analytical voices.

On the horizon

No next series has been explicitly scoped yet. The active output window ends at 2026-06-14, leaving the publication calendar open for whatever Kushal plans next.

Key learnings & principles

Kushal trusts Claude to maintain full series consistency — voice, structure, front matter, depth — without iterative direction. Minimal prompting ("day two," "generate them all") is the norm.
Articles should be long-form, deeply detailed, and authoritative. Superficial treatment is explicitly avoided; the style does not defer to professionals without explanation.
Body text uses flowing prose (no bullet points in body paragraphs) for series that specify this convention.
Sources and citations are included in a closing "Sources and Further Reading" section where appropriate.
Post 1 and the final capstone post of a series receive featured: true in the front matter; mid-series posts do not.
Approach & patterns

Series-first structure: Kushal establishes a full series plan before generation begins, then requests all remaining posts in sequence.
Resumption pattern: When generation stops mid-series (due to output limits), Kushal resumes with a minimal continuation prompt; Claude picks up from the correct post without re-briefing.
Project file as source of truth: Before writing, Claude reads dump.txt from the project filesystem to infer content conventions, repository structure, and existing series context.
Output separation: Generated files are written to /mnt/user-data/outputs/ (not directly into content/blog/), then surfaced via present_files after each batch.
Naming convention: YYYY-MM-DD-[series-slug]-N-[post-slug].md
Tools & resources

Platform: Blazor WebAssembly / .NET 10, hosted on GitHub Pages
Claude tools in use: create_file, view (with view_range for large files), present_files, read, bash
Project file: dump.txt (~19,800 lines) read in segmented overlapping ranges via view_range
Front matter rules:
Author ID: mercifulpotato-team (hyphenated)
Series key on all posts in a series
No draft: true
Tags: lowercase-hyphenated
Summary strings containing colons wrapped in double quotes
featured: true only on series-opener and capstone posts
