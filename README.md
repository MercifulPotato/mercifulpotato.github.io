# Merciful Potato Magazine

A free, open-source Blazor WebAssembly application deployed to GitHub Pages.

Built with **.NET 11** (STS) and **C# 15**.

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

## SDK selection

`global.json` pins the minimum SDK to `11.0.100-rc.3.26522.148` with `allowPrerelease: true` and
`rollForward: latestFeature`:

- Locally, a machine that only has the RC3 SDK (for example Fedora's `dotnet-sdk-11.0` package) uses RC3.
- Any machine with a GA `11.0.1xx` SDK installed uses the GA SDK, because it is newer than RC3.
- CI installs the latest GA SDK with `actions/setup-dotnet` (`dotnet-version: 11.0.x`, `dotnet-quality: ga`).

Packages reference the GA `11.0.0` ASP.NET Core and Microsoft.Extensions builds in both cases. Once the
GA SDK is available locally, change `sdk.version` to `11.0.100` and remove `allowPrerelease`.

## .NET 11 features used

| Feature | Where |
|---------|-------|
| C# 15 closed hierarchy | `Models/BlogPostView.cs`, consumed by `Pages/BlogPost.razor` |
| C# 15 collection expression arguments (`[with(capacity: n)]`) | `ProductDataGenerator`, ContentProcessor |
| `IHostedService` in Blazor WebAssembly | `Services/AppLifetimeTelemetry.cs` |
| `System.Threading.Lock` | `Services/TelemetryService.cs` |
| Fingerprinted Blazor boot script (`OverrideHtmlAssetPlaceholders`) | `wwwroot/index.html` |
| `dotnet test --timeout` run-level limit | CI workflows |

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
dotnet test --timeout 10m
dotnet run --project tests/ObserverMagazine.Integration.Tests
```

Run `dotnet test -?` from the repo root for the full option list. `DOTNET_TEST_RUNNER=VSTest`
overrides `global.json` for one session.

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
| Framework | .NET 11 / Blazor WASM 11.0.0 | MIT |
| Markdown | Markdig 1.6.0 | BSD-2-Clause |
| YAML | YamlDotNet 18.1.0 | MIT |
| Test framework | xUnit.net v3 4.0.3 | Apache-2.0 |
| Component testing | bUnit 2.15.2 | MIT |
| Test runner | Microsoft.Testing.Platform v2 | MIT |
| Text-to-speech | KittenTTS 0.8.1 | Apache-2.0 |

All dependencies are free for any use — no payment required, ever.

## License

AGPLv3
