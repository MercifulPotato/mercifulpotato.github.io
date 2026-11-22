#!/usr/bin/env python3
import argparse
import functools
import http.server
import sys
import threading
from pathlib import Path

from playwright.sync_api import TimeoutError as PlaywrightTimeoutError
from playwright.sync_api import sync_playwright

READY_OR_FAILED = """() => {
    const failed = !!document.getElementById('boot-error');
    const loading = !!document.getElementById('boot-loading');
    const errorUi = document.getElementById('blazor-error-ui');
    const crashed = !!errorUi && errorUi.style.display !== '' && errorUi.style.display !== 'none';
    return failed || crashed || !loading;
}"""


class GitHubPagesLikeHandler(http.server.SimpleHTTPRequestHandler):
    extensions_map = {
        **http.server.SimpleHTTPRequestHandler.extensions_map,
        ".wasm": "application/wasm",
        ".js": "text/javascript",
        ".mjs": "text/javascript",
        ".json": "application/json",
        ".dat": "application/octet-stream",
        ".xml": "application/xml",
        ".mp3": "audio/mpeg",
    }

    def send_head(self):
        path = Path(self.translate_path(self.path))
        if path.exists():
            return super().send_head()
        root = Path(self.directory)
        fallback = root / "404.html"
        if not fallback.exists():
            fallback = root / "index.html"
        handle = fallback.open("rb")
        self.send_response(404)
        self.send_header("Content-Type", "text/html; charset=utf-8")
        self.send_header("Content-Length", str(fallback.stat().st_size))
        self.end_headers()
        return handle

    def log_message(self, format, *args):
        return


def check_route(browser, base_url, route, timeout_ms):
    page = browser.new_page()
    messages = []
    page.on("console", lambda message, log=messages: log.append(f"console.{message.type}: {message.text}"))
    page.on("pageerror", lambda error, log=messages: log.append(f"pageerror: {error}"))
    problem = None
    try:
        page.goto(base_url + route, wait_until="domcontentloaded")
        page.wait_for_function(READY_OR_FAILED, timeout=timeout_ms)
        if page.query_selector("#boot-error"):
            problem = "startup error screen was shown"
        elif page.evaluate("() => { const e = document.getElementById('blazor-error-ui'); return !!e && e.style.display !== '' && e.style.display !== 'none'; }"):
            problem = "Blazor error bar was shown"
        else:
            page.wait_for_selector("main h1", timeout=timeout_ms)
            page.wait_for_timeout(1000)
            if page.query_selector(".page-error"):
                problem = "page rendered the error boundary: " + (page.text_content(".page-error-details pre") or "").strip()
    except PlaywrightTimeoutError:
        problem = f"app did not finish loading within {timeout_ms // 1000}s"
    report = ""
    if problem:
        try:
            report = page.evaluate("() => window.observerBoot ? window.observerBoot.report() : 'observerBoot not available'")
        except Exception as error:
            report = f"could not collect boot report: {error}"
    page.close()
    return problem, report, messages


def main():
    parser = argparse.ArgumentParser(description="Serve a published Blazor WebAssembly site and verify it boots in headless Chromium.")
    parser.add_argument("--site-dir", required=True, type=Path)
    parser.add_argument("--route", action="append", dest="routes")
    parser.add_argument("--timeout-seconds", type=int, default=120)
    args = parser.parse_args()

    site_dir = args.site_dir.resolve()
    if not (site_dir / "index.html").exists():
        print(f"error: {site_dir / 'index.html'} does not exist", file=sys.stderr)
        return 2

    routes = args.routes or ["/", "/blog", "/about"]
    handler = functools.partial(GitHubPagesLikeHandler, directory=str(site_dir))
    server = http.server.ThreadingHTTPServer(("127.0.0.1", 0), handler)
    threading.Thread(target=server.serve_forever, daemon=True).start()
    base_url = f"http://127.0.0.1:{server.server_address[1]}"

    failures = 0
    try:
        with sync_playwright() as playwright:
            browser = playwright.chromium.launch()
            for route in routes:
                problem, report, messages = check_route(browser, base_url, route, args.timeout_seconds * 1000)
                if problem is None:
                    print(f"PASS {route}")
                    continue
                failures += 1
                print(f"FAIL {route}: {problem}")
                print(report)
                for message in messages:
                    print(f"  {message}")
            browser.close()
    finally:
        server.shutdown()

    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
