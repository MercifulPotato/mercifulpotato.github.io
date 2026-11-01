# Merciful Potato Magazine

A free, open-source Blazor WebAssembly application deployed to GitHub Pages.

Built with **.NET 10** (LTS) and **C# 14**.

## Features

- **Blog** — Write posts in Markdown with YAML front matter; compiled to static JSON at build time
- **RSS Feed** — Auto-generated `feed.xml`
- **Audio narration** — Every post is narrated with KittenTTS at build time and served as MP3
- **Responsive Tables** — Sortable, filterable data tables that work on mobile
- **Master-Detail Flow** — Click a list item to view full details
- **OpenTelemetry-ready Logging** — Structured logging via `ILogger`
- **Full Test Suite** — bUnit component tests + xUnit.net v3 integration tests, running on Microsoft.Testing.Platform

## Quick Start
```bash
dotnet run --project tools/ObserverMagazine.ContentProcessor
dotnet run --project src/ObserverMagazine.Web
dotnet test
```

## Testing

Tests run on **Microsoft.Testing.Platform (MTP)**, not VSTest. The opt-in lives in
`global.json`:

```json
{ "test": { "runner": "Microsoft.Testing.Platform" } }
```

Test projects are ordinary executables, so each one can also be run on its own:

```bash
dotnet test --solution ObserverMagazine.slnx
dotnet test --project tests/ObserverMagazine.Web.Tests
dotnet test --filter-class ObserverMagazine.Web.Tests.Services.BlogServiceTests
dotnet test --filter-trait Category=Integration
dotnet run --project tests/ObserverMagazine.Integration.Tests
```

Run `dotnet test -?` from the repo root for the full option list.

xUnit configuration for MTP lives in `testconfig.json` next to a test project
(not `xunit.runner.json`, which is the VSTest-era file).

bUnit tests that wait for asynchronous rendering pass an explicit timeout
(bUnit's default is one second, which is too tight on a busy CI runner).

## Audio narration

`tools/generate_audio.py` turns each post into `blog-data/<slug>.mp3`. Posts without
an MP3 simply hide the audio player.

The work is incremental. Each finished post is recorded in `audio-manifest.json`
using a hash of the post body (front matter excluded), voice, model, speed and
`PIPELINE_VERSION`. Bump `PIPELINE_VERSION` to force regeneration after changing
text preprocessing.

In CI (`deploy.yml`):

- Posts are split across 4 parallel runners by a stable hash of the slug.
- Each runner restores its own cache and generates only what is missing or changed.
- Each runner stops starting new posts after 270 minutes.
- The cache is saved even when the job fails, so the next run resumes where this one stopped.
- The site deploys with whatever audio exists.
- When nothing is pending, the Python toolchain is never installed.

Locally:

```bash
pip install -r tools/requirements-audio.txt
sudo apt-get install -y espeak-ng ffmpeg
python tools/generate_audio.py --check
python tools/generate_audio.py
python tools/generate_audio.py --shard 0 --shards 4 --output-dir tts-cache/shard-0 --time-budget-minutes 60
```

Published GitHub Pages sites may be no larger than 1 GB. The deploy job prints the
site size and warns above 900 MB.

## Technology

| Component | Technology | License |
|-----------|-----------|---------|
| Framework | .NET 10 / Blazor WASM 10.0.14 | MIT |
| Markdown | Markdig 1.5.1 | BSD-2-Clause |
| YAML | YamlDotNet 18.1.0 | MIT |
| Test framework | xUnit.net v3 4.0.2 | Apache-2.0 |
| Component testing | bUnit 2.13.3 | MIT |
| Test runner | Microsoft.Testing.Platform v2 | MIT |
| Text-to-speech | KittenTTS 0.8.1 | Apache-2.0 |

All dependencies are free for any use — no payment required, ever.

## License

AGPLv3
