#!/usr/bin/env python3

import argparse
import hashlib
import json
import os
import re
import subprocess
import sys
import tempfile
import time

PIPELINE_VERSION = 2
MANIFEST_FILENAME = "audio-manifest.json"
SAMPLE_RATE = 24000
DEFAULT_MAX_CHUNK_CHARS = 250
MIN_SPLIT_CHARS = 20
MAX_SPLIT_DEPTH = 5
CHUNK_GAP_SECONDS = 0.2
MIN_TEXT_CHARS = 10
FRONT_MATTER = re.compile(r"\A---\s*\n.*?\n---\s*(?:\n|\Z)", re.DOTALL)


def log(message: str) -> None:
    print(message, flush=True)


def warn(message: str) -> None:
    prefix = "::warning::" if os.environ.get("GITHUB_ACTIONS") == "true" else "WARNING: "
    print(f"{prefix}{message}", flush=True)


def write_github_output(name: str, value: str) -> None:
    path = os.environ.get("GITHUB_OUTPUT")
    if path:
        with open(path, "a", encoding="utf-8") as f:
            f.write(f"{name}={value}\n")


def write_step_summary(text: str) -> None:
    path = os.environ.get("GITHUB_STEP_SUMMARY")
    if path:
        with open(path, "a", encoding="utf-8") as f:
            f.write(text + "\n")


def mark_changed(marker: str | None) -> None:
    if marker:
        os.makedirs(os.path.dirname(os.path.abspath(marker)), exist_ok=True)
        with open(marker, "w", encoding="utf-8") as f:
            f.write("changed\n")


def load_manifest(output_dir: str) -> dict:
    path = os.path.join(output_dir, MANIFEST_FILENAME)
    if not os.path.exists(path):
        return {}
    try:
        with open(path, encoding="utf-8") as f:
            data = json.load(f)
        return data if isinstance(data, dict) else {}
    except (json.JSONDecodeError, OSError) as e:
        warn(f"Could not read manifest {path}: {e}")
        return {}


def save_manifest(output_dir: str, manifest: dict) -> None:
    path = os.path.join(output_dir, MANIFEST_FILENAME)
    tmp = path + ".tmp"
    with open(tmp, "w", encoding="utf-8") as f:
        json.dump(manifest, f, indent=2, sort_keys=True)
    os.replace(tmp, path)


def derive_slug(filename: str) -> str:
    name = os.path.splitext(filename)[0]
    if len(name) > 11 and name[4] == "-" and name[7] == "-" and name[10] == "-" and name[:4].isdigit():
        return name[11:]
    return name


def shard_of(slug: str, shards: int) -> int:
    return int(hashlib.sha256(slug.encode("utf-8")).hexdigest(), 16) % shards


def strip_front_matter(content: str) -> str:
    content = content.replace("\r\n", "\n")
    return FRONT_MATTER.sub("", content, count=1).strip()


def content_hash(body: str, voice: str, model: str, speed: float) -> str:
    h = hashlib.sha256()
    h.update(f"v{PIPELINE_VERSION}|{model}|{voice}|{speed:.3f}\n".encode("utf-8"))
    h.update(body.encode("utf-8"))
    return h.hexdigest()


def end_sentence(match: re.Match) -> str:
    item = match.group(1)
    return item if item[-1] in ".!?:;" else item + "."


def strip_markdown(text: str) -> str:
    text = re.sub(r"```[\s\S]*?```", " ", text)
    text = re.sub(r"~~~[\s\S]*?~~~", " ", text)
    text = re.sub(r"`[^`\n]+`", " ", text)
    text = re.sub(r"!\[[^\]]*\]\([^)]*\)", " ", text)
    text = re.sub(r"\[([^\]]+)\]\([^)]+\)", r"\1", text)
    text = re.sub(r"\[\^[^\]]+\]", "", text)
    text = re.sub(r"<[^>]+>", " ", text)
    text = re.sub(r"^\s*\|?\s*:?-{3,}:?\s*(\|\s*:?-{3,}:?\s*)*\|?\s*$", " ", text, flags=re.MULTILINE)
    text = re.sub(r"^\s*\|(.*)\|\s*$", lambda m: m.group(1).replace("|", ", ") + ".", text, flags=re.MULTILINE)
    text = re.sub(r"^#{1,6}\s+", "", text, flags=re.MULTILINE)
    text = re.sub(r"\*{1,3}([^*\n]+)\*{1,3}", r"\1", text)
    text = re.sub(r"(?<!\w)_{1,3}([^_\n]+?)_{1,3}(?!\w)", r"\1", text)
    text = re.sub(r"^\s*[-*_]{3,}\s*$", " ", text, flags=re.MULTILINE)
    text = re.sub(r"^\s*>\s?", "", text, flags=re.MULTILINE)
    text = re.sub(r"^\s*(?:[-*+]|\d+[.)])\s+(.+?)\s*$", end_sentence, text, flags=re.MULTILINE)
    text = re.sub(r"\n{2,}", ". ", text)
    text = re.sub(r"\n", " ", text)
    text = re.sub(r"\.(\s*\.)+", ".", text)
    text = re.sub(r"([!?:;])\s*\.", r"\1", text)
    text = re.sub(r"\s+([,.;:!?])(?=\s|$)", r"\1", text)
    text = re.sub(r"\s{2,}", " ", text)
    return text.strip()


PROGRAMMING_TERMS = [
    (r"\.NET\s+10", "dot net ten"),
    (r"\.NET\s+9", "dot net nine"),
    (r"\.NET\s+8", "dot net eight"),
    (r"\.NET\s+7", "dot net seven"),
    (r"\.NET\s+6", "dot net six"),
    (r"\.NET\s+Core\s+3\.0", "dot net core three point oh"),
    (r"\.NET\s+Core", "dot net core"),
    (r"\.NET\s+Framework\s+4\.8", "dot net framework four point eight"),
    (r"\.NET\s+Framework", "dot net framework"),
    (r"\bASP\.NET", "asp dot net"),
    (r"\.NET", "dot net"),
    (r"\bC#", "C sharp"),
    (r"\bF#", "F sharp"),
    (r"\bC\+\+", "C plus plus"),
    (r"\.csproj\b", " C sharp project file"),
    (r"\.cshtml\b", " C S H T M L"),
    (r"\.ascx\b", " A S C X"),
    (r"\.aspx\b", " A S P X"),
    (r"\.slnx\b", " solution X"),
    (r"\.sln\b", " solution"),
    (r"\.json\b", " JSON"),
    (r"\.yml\b", " YAML"),
    (r"\.xml\b", " X M L"),
    (r"\.md\b", " markdown"),
    (r"\.css\b", " C S S"),
    (r"\.js\b", " JavaScript"),
    (r"\.wasm\b", " web assembly"),
    (r"\bWASM\b", "web assembly"),
    (r"\bIL\b", "intermediate language"),
    (r"\bCLR\b", "common language runtime"),
    (r"\bJIT\b", "just in time"),
    (r"\bAOT\b", "ahead of time"),
    (r"\bNGen\b", "N gen"),
    (r"\bR2R\b", "ready to run"),
    (r"\bDI\b", "dependency injection"),
    (r"\bOWIN\b", "oh win"),
    (r"\bCORS\b", "cross origin resource sharing"),
    (r"\bRSS\b", "R S S"),
    (r"\bAPIs\b", "A P Is"),
    (r"\bAPI\b", "A P I"),
    (r"\bUIs\b", "U Is"),
    (r"\bUI\b", "U I"),
    (r"\bURL\b", "U R L"),
    (r"\bHTTPS\b", "H T T P S"),
    (r"\bHTTP\b", "H T T P"),
    (r"\bHTML\b", "H T M L"),
    (r"\bCSS\b", "C S S"),
    (r"\bSQL\b", "S Q L"),
    (r"\bSSDs\b", "S S Ds"),
    (r"\bSSD\b", "S S D"),
    (r"\bI/O\b", "I O"),
    (r"\bIIS\b", "I I S"),
    (r"\bMVC\b", "M V C"),
    (r"\bLTS\b", "long term support"),
    (r"\bSDK\b", "S D K"),
    (r"\bNuGet\b", "new get"),
    (r"\bxUnit\b", "x unit"),
    (r"\bbUnit\b", "b unit"),
    (r"=>", " arrow "),
    (r"!=", " not equal "),
    (r"==", " equals "),
    (r">=", " greater than or equal "),
    (r"<=", " less than or equal "),
    (r"\bv(\d+)\b", r"version \1"),
]

SYMBOL_TERMS = [
    (r"\$\s?(\d[\d,]*(?:\.\d+)?)\s*(thousand|million|billion|trillion)\b", r"\1 \2 dollars"),
    (r"\bRs\.\s*", "rupees "),
    (r"\bNPR\s+", "N P R "),
    (r"\bUSD\s+", "U S D "),
    (r"(?<=\d)\s*×\s*(?=\d)", " times "),
    (r"\s×\s", " times "),
    (r"~\s*(?=\d)", "about "),
    (r"\s&\s", " and "),
    (r"\s=\s", " equals "),
    (r"(?<=\d)/(?=[A-Za-z])", " per "),
    (r"(?<=\d)\s*–\s*(?=\d)", " to "),
    (r"\s*[—–]\s*", ", "),
]


def apply_rules(text: str, rules: list) -> str:
    for pattern, replacement in rules:
        text = re.sub(pattern, replacement, text)
    return text


_preprocessor = None


def normalize_for_speech(text: str) -> str:
    global _preprocessor
    text = apply_rules(text, PROGRAMMING_TERMS)
    text = apply_rules(text, SYMBOL_TERMS)
    if _preprocessor is None:
        from kittentts.preprocess import TextPreprocessor
        _preprocessor = TextPreprocessor(lowercase=False, remove_punctuation=False)
    text = _preprocessor(text)
    return re.sub(r"\s{2,}", " ", text).strip()


def split_long(sentence: str, max_chars: int) -> list[str]:
    if len(sentence) <= max_chars:
        return [sentence]
    parts: list[str] = []
    for clause in re.split(r"(?<=[,;:])\s+", sentence):
        if len(clause) <= max_chars:
            parts.append(clause)
            continue
        current = ""
        for word in clause.split():
            if current and len(current) + len(word) + 1 > max_chars:
                parts.append(current)
                current = word
            else:
                current = f"{current} {word}" if current else word
        if current:
            parts.append(current)
    return parts


def chunk_text(text: str, max_chars: int) -> list[str]:
    chunks: list[str] = []
    current = ""
    for sentence in re.split(r"(?<=[.!?])\s+", text):
        sentence = sentence.strip()
        if not sentence:
            continue
        for piece in split_long(sentence, max_chars):
            if current and len(current) + len(piece) + 1 > max_chars:
                chunks.append(current)
                current = piece
            else:
                current = f"{current} {piece}" if current else piece
    if current:
        chunks.append(current)
    return chunks


def split_in_half(text: str) -> tuple[str, str]:
    middle = len(text) // 2
    left = text.rfind(" ", 0, middle)
    right = text.find(" ", middle)
    candidates = [i for i in (left, right) if i > 0]
    cut = min(candidates, key=lambda i: abs(i - middle)) if candidates else middle
    return text[:cut].strip(), text[cut:].strip()


class Synthesizer:
    def __init__(self, model_name: str, voice: str, speed: float, max_chars: int):
        from kittentts import KittenTTS
        import numpy as np

        self.np = np
        started = time.monotonic()
        tts = KittenTTS(model_name)
        inner = getattr(tts, "model", None)
        self.engine = inner if inner is not None and hasattr(inner, "generate") else tts
        self.voice = voice
        self.speed = speed
        self.max_chars = max_chars
        self.gap = np.zeros(int(SAMPLE_RATE * CHUNK_GAP_SECONDS), dtype=np.float32)
        log(f"  Model {model_name} loaded in {time.monotonic() - started:.1f}s")

    def _piece(self, text: str, depth: int) -> tuple[list, int]:
        try:
            audio = self.engine.generate(text, voice=self.voice, speed=self.speed, clean_text=False)
            return [self.np.asarray(audio, dtype=self.np.float32).reshape(-1)], 0
        except Exception as e:
            if len(text) < MIN_SPLIT_CHARS or depth >= MAX_SPLIT_DEPTH:
                warn(f"Dropped {len(text)} chars after {depth} split(s): {type(e).__name__}: {str(e)[:160]}")
                return [], len(text)
            left, right = split_in_half(text)
            if not left or not right:
                warn(f"Dropped {len(text)} chars (unsplittable): {type(e).__name__}")
                return [], len(text)
            a, lost_a = self._piece(left, depth + 1)
            b, lost_b = self._piece(right, depth + 1)
            return a + b, lost_a + lost_b

    def synthesize(self, text: str) -> tuple[object | None, int, int]:
        chunks = chunk_text(text, self.max_chars)
        segments = []
        lost = 0
        for chunk in chunks:
            pieces, dropped = self._piece(chunk, 0)
            lost += dropped
            for piece in pieces:
                segments.append(piece)
                segments.append(self.gap)
        if not segments:
            return None, len(chunks), lost
        return self.np.concatenate(segments), len(chunks), lost


def encode_mp3(audio, output_mp3: str) -> float:
    import soundfile as sf

    os.makedirs(os.path.dirname(os.path.abspath(output_mp3)), exist_ok=True)
    fd, tmp_wav = tempfile.mkstemp(suffix=".wav")
    os.close(fd)
    tmp_mp3 = output_mp3 + ".partial.mp3"
    try:
        sf.write(tmp_wav, audio, SAMPLE_RATE)
        result = subprocess.run(
            ["ffmpeg", "-y", "-hide_banner", "-loglevel", "error", "-i", tmp_wav,
             "-codec:a", "libmp3lame", "-b:a", "24k", "-ac", "1", "-ar", str(SAMPLE_RATE), tmp_mp3],
            capture_output=True, text=True, timeout=900,
        )
        if result.returncode != 0:
            raise RuntimeError(f"ffmpeg failed: {result.stderr.strip()[:500]}")
        os.replace(tmp_mp3, output_mp3)
        return os.path.getsize(output_mp3) / (1024 * 1024)
    finally:
        for path in (tmp_wav, tmp_mp3):
            if os.path.exists(path):
                os.unlink(path)


def collect_posts(content_dir: str, shard: int, shards: int, voice: str, model: str, speed: float) -> list[dict]:
    posts = []
    for md_file in sorted(f for f in os.listdir(content_dir) if f.endswith(".md")):
        slug = derive_slug(md_file)
        if shard_of(slug, shards) != shard:
            continue
        with open(os.path.join(content_dir, md_file), encoding="utf-8") as f:
            body = strip_front_matter(f.read())
        posts.append({
            "file": md_file,
            "slug": slug,
            "body": body,
            "hash": content_hash(body, voice, model, speed),
        })
    return posts


def prune_orphans(output_dir: str, manifest: dict, slugs: set[str]) -> int:
    removed = 0
    for name in os.listdir(output_dir):
        if name.endswith(".partial.mp3"):
            os.unlink(os.path.join(output_dir, name))
            removed += 1
        elif name.endswith(".mp3") and name[:-4] not in slugs:
            os.unlink(os.path.join(output_dir, name))
            removed += 1
    for slug in [s for s in manifest if s not in slugs]:
        del manifest[slug]
        removed += 1
    return removed


def main() -> int:
    parser = argparse.ArgumentParser(description="Generate TTS audio for blog posts")
    parser.add_argument("--content-dir", default="content/blog")
    parser.add_argument("--output-dir", default="src/ObserverMagazine.Web/wwwroot/blog-data")
    parser.add_argument("--voice", default="Bella", help="Bella, Jasper, Luna, Bruno, Rosie, Hugo, Kiki, Leo")
    parser.add_argument("--model", default="KittenML/kitten-tts-nano-0.8")
    parser.add_argument("--speed", type=float, default=1.15)
    parser.add_argument("--max-chunk-chars", type=int, default=DEFAULT_MAX_CHUNK_CHARS)
    parser.add_argument("--shard", type=int, default=0)
    parser.add_argument("--shards", type=int, default=1)
    parser.add_argument("--time-budget-minutes", type=float, default=0, help="0 means unlimited")
    parser.add_argument("--changed-marker", default=None, help="File created when the output directory changes")
    parser.add_argument("--check", action="store_true", help="Only report pending work; needs no TTS dependencies")
    parser.add_argument("--force", action="store_true")
    args = parser.parse_args()

    if args.shards < 1 or not 0 <= args.shard < args.shards:
        log(f"Invalid shard {args.shard} of {args.shards}")
        return 2
    if not os.path.isdir(args.content_dir):
        log(f"Content directory not found: {args.content_dir}")
        return 1

    started = time.monotonic()
    budget_seconds = args.time_budget_minutes * 60
    os.makedirs(args.output_dir, exist_ok=True)

    manifest = load_manifest(args.output_dir)
    posts = collect_posts(args.content_dir, args.shard, args.shards, args.voice, args.model, args.speed)
    slugs = {p["slug"] for p in posts}

    pruned = prune_orphans(args.output_dir, manifest, slugs)
    if pruned:
        save_manifest(args.output_dir, manifest)
        mark_changed(args.changed_marker)
        log(f"Pruned {pruned} orphaned file(s)/entries")

    pending = []
    for post in posts:
        mp3_path = os.path.join(args.output_dir, f"{post['slug']}.mp3")
        current = manifest.get(post["slug"], {}).get("hash")
        if args.force or current != post["hash"] or not os.path.exists(mp3_path):
            pending.append(post)

    log(f"Shard {args.shard + 1}/{args.shards}: {len(posts)} post(s), {len(posts) - len(pending)} up to date, {len(pending)} pending")
    write_github_output("pending", str(len(pending)))

    if args.check or not pending:
        return 0

    synthesizer = Synthesizer(args.model, args.voice, args.speed, args.max_chunk_chars)
    generated, failed, deferred = [], [], []

    for index, post in enumerate(pending, 1):
        elapsed = time.monotonic() - started
        if budget_seconds and elapsed > budget_seconds:
            deferred = [p["slug"] for p in pending[index - 1:]]
            warn(f"Time budget reached after {elapsed / 60:.0f} min; {len(deferred)} post(s) deferred to the next run")
            break

        slug = post["slug"]
        log(f"[{index}/{len(pending)}] {post['file']} -> {slug}.mp3")
        post_started = time.monotonic()
        try:
            text = normalize_for_speech(strip_markdown(post["body"]))
            if len(text) < MIN_TEXT_CHARS:
                log("  Skipped: too little text")
                continue
            audio, chunk_count, lost = synthesizer.synthesize(text)
            if audio is None:
                raise RuntimeError("no audio produced")
            size_mb = encode_mp3(audio, os.path.join(args.output_dir, f"{slug}.mp3"))
        except Exception as e:
            failed.append(slug)
            warn(f"{slug}: {type(e).__name__}: {e}")
            continue

        manifest[slug] = {"hash": post["hash"]}
        save_manifest(args.output_dir, manifest)
        mark_changed(args.changed_marker)
        generated.append(slug)
        duration = len(audio) / SAMPLE_RATE
        log(f"  {len(text)} chars, {chunk_count} chunks, {duration / 60:.1f} min audio, {size_mb:.2f} MB, "
            f"{time.monotonic() - post_started:.0f}s" + (f", {lost} chars dropped" if lost else ""))
        if lost:
            warn(f"{slug}: {lost} chars could not be synthesized")

    summary = (f"Shard {args.shard + 1}/{args.shards}: {len(generated)} generated, {len(failed)} failed, "
               f"{len(deferred)} deferred, {len(posts) - len(pending)} up to date "
               f"in {(time.monotonic() - started) / 60:.1f} min")
    log(summary)
    write_step_summary(f"- {summary}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
