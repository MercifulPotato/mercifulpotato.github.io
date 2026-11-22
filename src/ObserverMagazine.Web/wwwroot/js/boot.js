(function () {
    'use strict';

    var ISSUES_URL = 'https://github.com/mercifulpotato/mercifulpotato.github.io/issues/new';
    var STALL_TIMEOUT_MS = 60000;
    var SLOW_HINT_MS = 12000;
    var MAX_LOG_ENTRIES = 60;
    var MAX_ISSUE_BODY = 6000;

    var WASM_SIMD_PROBE = [0, 97, 115, 109, 1, 0, 0, 0, 1, 5, 1, 96, 0, 1, 123, 3, 2, 1, 0, 10, 10, 1, 8, 0, 65, 0, 253, 15, 253, 98, 11];
    var WASM_EH_PROBE = [0, 97, 115, 109, 1, 0, 0, 0, 1, 4, 1, 96, 0, 0, 3, 2, 1, 0, 10, 8, 1, 6, 0, 6, 64, 25, 11, 11];

    var CAUSES = {
        noWebAssembly: {
            title: 'Your browser can’t run this site',
            summary: 'This site is a .NET application compiled to WebAssembly, and WebAssembly is not available in this browser.',
            explanation: 'Merciful Potato Magazine runs entirely inside your browser using Blazor WebAssembly on .NET 11. Your browser either does not support WebAssembly at all, or it has been switched off. Security features such as Apple’s Lockdown Mode or Microsoft Edge’s enhanced security mode, workplace browser policies, and some privacy extensions can disable it.',
            steps: [
                'Update your browser to its latest version, or open this page in a current version of Firefox, Chrome, Edge or Safari.',
                'If you use Lockdown Mode or Edge’s enhanced security mode, add an exception for this site or try a different browser.',
                'On a work or school device, ask your administrator whether WebAssembly is blocked by policy.'
            ],
            canTryAnyway: false
        },
        oldWebAssembly: {
            title: 'Your browser is too old for this site',
            summary: 'WebAssembly works here, but it is missing features that the .NET 11 runtime needs.',
            explanation: 'The .NET WebAssembly runtime relies on WebAssembly SIMD and WebAssembly exception handling. Mainstream browsers added both between 2021 and 2023. This browser reported that at least one of them is unavailable, so the runtime would fail to start.',
            steps: [
                'Update your browser to its latest version.',
                'If updates are not possible on this device (for example an older phone or tablet), try another browser or another device.',
                'If you believe your browser is up to date, choose “Try anyway” below and send us the technical details if it still fails.'
            ],
            canTryAnyway: true
        },
        fileProtocol: {
            title: 'This page was opened as a local file',
            summary: 'Browsers do not allow WebAssembly applications to load from file:// addresses.',
            explanation: 'The site has to be served over HTTP or HTTPS so the browser can download the .NET runtime and application files.',
            steps: [
                'Run the site with “dotnet run --project src/ObserverMagazine.Web”.',
                'Or serve the published wwwroot folder with any static web server.'
            ],
            canTryAnyway: false
        },
        bootScript: {
            title: 'The application loader did not download',
            summary: 'The browser could not load _framework/blazor.webassembly.js, the script that starts everything else.',
            explanation: 'This usually means the connection dropped, an ad or script blocker stopped the file, a firewall or proxy rejected it, or a new version of the site was being published at the moment you visited.',
            steps: [
                'Check your internet connection and reload the page.',
                'Temporarily disable ad, script or privacy blockers for this site.',
                'Wait a minute and try again in case a new version was being published.'
            ],
            canTryAnyway: false
        },
        integrity: {
            title: 'Downloaded files failed a security check',
            summary: 'One or more application files did not match the fingerprint recorded when the site was built, so the browser refused to run them.',
            explanation: 'Every file the app downloads is checked against a hash computed at build time. A mismatch happens when a cached copy from an older version is mixed with a newer one, when a proxy, antivirus or “speed-up” service rewrites JavaScript, or when files were modified after the site was published.',
            steps: [
                'Choose “Clear cached files and reload” below.',
                'Try a private or incognito window, or disable extensions that modify pages.',
                'Site maintainers: make sure nothing edits the published _framework folder after dotnet publish.'
            ],
            canTryAnyway: false
        },
        network: {
            title: 'Some application files could not be downloaded',
            summary: 'The .NET runtime or one of the application assemblies failed to download.',
            explanation: 'The app needs to download several megabytes of runtime and application files before it can start. One of those requests failed, which is usually caused by a lost connection, a blocker or firewall, or a deployment that was still in progress.',
            steps: [
                'Check your internet connection and reload the page.',
                'Temporarily disable ad, script or privacy blockers for this site.',
                'Choose “Clear cached files and reload” below.'
            ],
            canTryAnyway: false
        },
        startup: {
            title: 'The application crashed while starting',
            summary: 'The .NET runtime started, but the application threw an error before it could show the first page.',
            explanation: 'This is almost always a bug in the site rather than a problem with your browser. The technical details below contain the error the application reported.',
            steps: [
                'Reload the page; a temporary problem may have caused it.',
                'Choose “Clear cached files and reload” below.',
                'If it keeps happening, report the problem and include the technical details.'
            ],
            canTryAnyway: false
        },
        timeout: {
            title: 'The application stopped making progress',
            summary: 'Loading has not advanced for ' + Math.round(STALL_TIMEOUT_MS / 1000) + ' seconds.',
            explanation: 'The app may be stuck waiting for a file that will never arrive, or the device may be too slow or too low on memory to start the .NET runtime.',
            steps: [
                'Check your internet connection and reload the page.',
                'Close other tabs or apps to free memory, then reload.',
                'Choose “Clear cached files and reload” below, or try another browser.'
            ],
            canTryAnyway: false
        },
        unknown: {
            title: 'Something went wrong while starting',
            summary: 'The application could not start, and the reason did not match any known pattern.',
            explanation: 'The technical details below contain everything the browser reported while loading.',
            steps: [
                'Reload the page.',
                'Choose “Clear cached files and reload” below, or try another browser.',
                'If it keeps happening, report the problem and include the technical details.'
            ],
            canTryAnyway: false
        }
    };

    var state = {
        startedAt: Date.now(),
        ready: false,
        failed: false,
        cause: null,
        log: [],
        features: null,
        progress: '',
        progressChangedAt: Date.now(),
        watchdog: null
    };

    function elapsed() {
        return ((Date.now() - state.startedAt) / 1000).toFixed(1) + 's';
    }

    function describe(value) {
        if (value === null || value === undefined) {
            return String(value);
        }
        if (value instanceof Error) {
            return (value.name || 'Error') + ': ' + value.message + (value.stack ? '\n' + value.stack : '');
        }
        if (typeof value === 'object') {
            try {
                return JSON.stringify(value);
            } catch (e) {
                return Object.prototype.toString.call(value);
            }
        }
        return String(value);
    }

    function record(level, message) {
        state.log.push('[' + elapsed() + '] ' + level + ': ' + message);
        if (state.log.length > MAX_LOG_ENTRIES) {
            state.log.shift();
        }
    }

    function captureConsole(level) {
        var original = window.console && window.console[level];
        if (typeof original !== 'function') {
            return;
        }
        window.console[level] = function () {
            try {
                record(level, Array.prototype.map.call(arguments, describe).join(' '));
            } catch (e) {
                record(level, '(unserializable console message)');
            }
            return original.apply(window.console, arguments);
        };
    }

    captureConsole('error');
    captureConsole('warn');

    window.addEventListener('error', function (event) {
        var target = event.target;
        if (target && target !== window && (target.src || target.href)) {
            record('resource', 'Failed to load ' + (target.src || target.href));
            return;
        }
        var where = event.filename ? ' (' + event.filename + ':' + event.lineno + ':' + event.colno + ')' : '';
        var stack = event.error && event.error.stack ? '\n' + event.error.stack : '';
        record('error', (event.message || 'Script error') + where + stack);
    }, true);

    window.addEventListener('unhandledrejection', function (event) {
        record('rejection', describe(event.reason));
    });

    function probe(bytes) {
        try {
            return WebAssembly.validate(new Uint8Array(bytes));
        } catch (e) {
            return false;
        }
    }

    function detectFeatures() {
        var wasm = typeof WebAssembly === 'object' && typeof WebAssembly.validate === 'function';
        var storage = false;
        try {
            window.localStorage.setItem('observer-boot-probe', '1');
            window.localStorage.removeItem('observer-boot-probe');
            storage = true;
        } catch (e) {
            storage = false;
        }
        var importMaps = 'unknown';
        if (typeof HTMLScriptElement !== 'undefined' && typeof HTMLScriptElement.supports === 'function') {
            importMaps = HTMLScriptElement.supports('importmap');
        }
        return {
            webAssembly: wasm,
            webAssemblySimd: wasm && probe(WASM_SIMD_PROBE),
            webAssemblyExceptions: wasm && probe(WASM_EH_PROBE),
            promise: typeof Promise === 'function',
            fetch: typeof window.fetch === 'function',
            bigInt: typeof BigInt === 'function',
            importMaps: importMaps,
            localStorage: storage,
            cacheStorage: typeof window.caches === 'object',
            secureContext: window.isSecureContext === true,
            online: navigator.onLine !== false,
            cookiesEnabled: navigator.cookieEnabled !== false,
            deviceMemoryGb: navigator.deviceMemory || 'unknown',
            cpuCores: navigator.hardwareConcurrency || 'unknown',
            language: navigator.language || 'unknown',
            viewport: window.innerWidth + 'x' + window.innerHeight
        };
    }

    function missingRequirements(features) {
        var missing = [];
        if (!features.promise) {
            missing.push('JavaScript promises');
        }
        if (!features.fetch) {
            missing.push('the Fetch API');
        }
        if (!features.webAssemblySimd) {
            missing.push('WebAssembly SIMD');
        }
        if (!features.webAssemblyExceptions) {
            missing.push('WebAssembly exception handling');
        }
        return missing;
    }

    function classify() {
        var text = state.log.join('\n');
        if (/integrity|digest/i.test(text)) {
            return 'integrity';
        }
        if (navigator.onLine === false) {
            return 'network';
        }
        if (/Failed to fetch|NetworkError|Load failed|Failed to load|ERR_|status code 404|404 \(Not Found\)|Failed to download/i.test(text)) {
            return 'network';
        }
        if (/Failed to start platform|Unhandled exception|crit:|fail:|ManagedError|System\.\w+Exception/i.test(text)) {
            return 'startup';
        }
        return 'unknown';
    }

    function readProgress() {
        try {
            return window.getComputedStyle(document.documentElement).getPropertyValue('--blazor-load-percentage').trim();
        } catch (e) {
            return '';
        }
    }

    function report() {
        var cause = state.cause ? CAUSES[state.cause] : null;
        var lines = [
            'Merciful Potato Magazine startup report',
            'Status: ' + (state.ready ? 'ready' : state.failed ? 'failed' : 'loading'),
            'Cause: ' + (cause ? cause.title + ' (' + state.cause + ')' : 'none'),
            'Time: ' + new Date().toISOString(),
            'Elapsed: ' + elapsed(),
            'URL: ' + window.location.href,
            'User agent: ' + navigator.userAgent,
            'Load progress: ' + (state.progress || 'not reported'),
            '',
            'Browser capabilities:'
        ];
        var features = state.features || detectFeatures();
        for (var key in features) {
            if (Object.prototype.hasOwnProperty.call(features, key)) {
                lines.push('  ' + key + ': ' + features[key]);
            }
        }
        lines.push('', 'Messages captured while loading:');
        if (state.log.length === 0) {
            lines.push('  (none)');
        } else {
            for (var i = 0; i < state.log.length; i++) {
                lines.push('  ' + state.log[i]);
            }
        }
        return lines.join('\n');
    }

    function element(tag, className, text) {
        var node = document.createElement(tag);
        if (className) {
            node.className = className;
        }
        if (text !== undefined) {
            node.textContent = text;
        }
        return node;
    }

    function button(label, className, onClick) {
        var node = element('button', className, label);
        node.type = 'button';
        node.addEventListener('click', onClick);
        return node;
    }

    function clearCachesAndReload() {
        if (typeof Promise !== 'function') {
            window.location.reload();
            return;
        }
        var tasks = [];
        if (window.caches && typeof window.caches.keys === 'function') {
            tasks.push(window.caches.keys().then(function (keys) {
                return Promise.all(keys.map(function (key) { return window.caches.delete(key); }));
            }));
        }
        if (navigator.serviceWorker && typeof navigator.serviceWorker.getRegistrations === 'function') {
            tasks.push(navigator.serviceWorker.getRegistrations().then(function (registrations) {
                return Promise.all(registrations.map(function (registration) { return registration.unregister(); }));
            }));
        }
        Promise.all(tasks).then(null, function () { return null; }).then(function () {
            window.location.reload();
        });
    }

    function copyReport(trigger, textArea) {
        var text = report();
        var done = function () {
            trigger.textContent = 'Copied';
            window.setTimeout(function () { trigger.textContent = 'Copy details'; }, 2000);
        };
        var fallback = function () {
            textArea.value = text;
            textArea.focus();
            textArea.select();
            try {
                if (document.execCommand('copy')) {
                    done();
                    return;
                }
            } catch (e) {
                record('warn', 'Copy failed: ' + describe(e));
            }
            trigger.textContent = 'Select the text below and copy it';
        };
        if (navigator.clipboard && typeof navigator.clipboard.writeText === 'function') {
            navigator.clipboard.writeText(text).then(done, fallback);
        } else {
            fallback();
        }
    }

    function issueUrl(cause) {
        var body = report();
        if (body.length > MAX_ISSUE_BODY) {
            body = body.slice(0, MAX_ISSUE_BODY) + '\n… (truncated)';
        }
        return ISSUES_URL + '?title=' + encodeURIComponent('Site failed to start: ' + cause.title) +
            '&body=' + encodeURIComponent('```\n' + body + '\n```');
    }

    function removeErrorScreen() {
        var existing = document.getElementById('boot-error');
        if (existing && existing.parentNode) {
            existing.parentNode.removeChild(existing);
        }
    }

    function renderErrorScreen(causeKey, extraSummary, onTryAnyway) {
        removeErrorScreen();
        var cause = CAUSES[causeKey] || CAUSES.unknown;
        var root = element('div', 'boot-error');
        root.id = 'boot-error';
        root.setAttribute('role', 'alert');

        var card = element('div', 'boot-error-card');
        var head = element('div', 'boot-error-head');
        head.appendChild(element('div', 'boot-error-icon', '!'));
        head.appendChild(element('p', 'boot-error-brand', 'Merciful Potato Magazine'));
        head.appendChild(element('h1', 'boot-error-title', cause.title));
        head.appendChild(element('p', 'boot-error-summary', cause.summary + (extraSummary ? ' ' + extraSummary : '')));
        card.appendChild(head);

        var body = element('div', 'boot-error-body');
        body.appendChild(element('h2', null, 'What happened'));
        body.appendChild(element('p', null, cause.explanation));
        body.appendChild(element('h2', null, 'What you can try'));
        var list = element('ol', 'boot-error-steps');
        for (var i = 0; i < cause.steps.length; i++) {
            list.appendChild(element('li', null, cause.steps[i]));
        }
        body.appendChild(list);

        var actions = element('div', 'boot-error-actions');
        actions.appendChild(button('Reload', 'boot-btn boot-btn-primary', function () { window.location.reload(); }));
        actions.appendChild(button('Clear cached files and reload', 'boot-btn', clearCachesAndReload));
        if (cause.canTryAnyway && typeof onTryAnyway === 'function') {
            actions.appendChild(button('Try anyway', 'boot-btn', onTryAnyway));
        }
        body.appendChild(actions);

        var details = element('details', 'boot-error-details');
        details.appendChild(element('summary', null, 'Technical details'));
        var textArea = element('textarea', 'boot-error-report');
        textArea.readOnly = true;
        textArea.rows = 14;
        textArea.value = report();
        textArea.setAttribute('aria-label', 'Startup report');
        var detailActions = element('div', 'boot-error-actions');
        var copyButton = button('Copy details', 'boot-btn', function () { copyReport(copyButton, textArea); });
        detailActions.appendChild(copyButton);
        var reportLink = element('a', 'boot-btn', 'Report this problem');
        reportLink.href = issueUrl(cause);
        reportLink.target = '_blank';
        reportLink.rel = 'noopener noreferrer';
        detailActions.appendChild(reportLink);
        details.appendChild(textArea);
        details.appendChild(detailActions);
        body.appendChild(details);

        body.appendChild(element('p', 'boot-error-footnote', 'Your saved settings and data are kept when you reload or clear cached files.'));
        card.appendChild(body);
        root.appendChild(card);
        document.body.appendChild(root);
    }

    function stopWatchdog() {
        if (state.watchdog !== null) {
            window.clearInterval(state.watchdog);
            state.watchdog = null;
        }
    }

    function fail(causeKey, error, extraSummary, onTryAnyway) {
        if (state.ready || state.failed) {
            return;
        }
        if (error !== undefined) {
            record('fatal', describe(error));
        }
        state.failed = true;
        state.cause = causeKey || classify();
        stopWatchdog();
        renderErrorScreen(state.cause, extraSummary, onTryAnyway);
    }

    function markReady() {
        if (state.ready) {
            return;
        }
        state.ready = true;
        stopWatchdog();
        removeErrorScreen();
    }

    function watchForFirstRender() {
        var app = document.getElementById('app');
        if (!app || typeof MutationObserver !== 'function') {
            return;
        }
        var observer = new MutationObserver(function () {
            if (!document.getElementById('boot-loading')) {
                observer.disconnect();
                markReady();
            }
        });
        observer.observe(app, { childList: true, subtree: true });
    }

    function watchBlazorErrorUi() {
        var errorUi = document.getElementById('blazor-error-ui');
        if (!errorUi || typeof MutationObserver !== 'function') {
            return;
        }
        var observer = new MutationObserver(function () {
            if (state.ready) {
                observer.disconnect();
                return;
            }
            if (errorUi.style.display && errorUi.style.display !== 'none') {
                observer.disconnect();
                fail('startup');
            }
        });
        observer.observe(errorUi, { attributes: true, attributeFilter: ['style', 'class'] });
    }

    function startWatchdog() {
        var hint = document.getElementById('boot-hint');
        state.watchdog = window.setInterval(function () {
            var progress = readProgress();
            if (progress !== state.progress) {
                state.progress = progress;
                state.progressChangedAt = Date.now();
            }
            if (hint && hint.hidden && Date.now() - state.startedAt > SLOW_HINT_MS) {
                hint.hidden = false;
            }
            if (Date.now() - state.progressChangedAt > STALL_TIMEOUT_MS) {
                fail(navigator.onLine === false ? 'network' : (classify() === 'unknown' ? 'timeout' : null));
            }
        }, 1000);
    }

    function startBlazor() {
        if (typeof window.Blazor === 'undefined' || typeof window.Blazor.start !== 'function') {
            fail('bootScript');
            return;
        }
        try {
            var started = window.Blazor.start();
            if (started && typeof started.then === 'function') {
                started.then(null, function (error) { fail(null, error); });
            }
        } catch (error) {
            fail(null, error);
        }
    }

    function start() {
        watchForFirstRender();
        watchBlazorErrorUi();

        if (window.location.protocol === 'file:') {
            fail('fileProtocol');
            return;
        }

        state.features = detectFeatures();
        if (!state.features.webAssembly) {
            fail('noWebAssembly');
            return;
        }

        var missing = missingRequirements(state.features);
        if (missing.length > 0) {
            record('warn', 'Missing browser features: ' + missing.join(', '));
            fail('oldWebAssembly', undefined, 'Missing: ' + missing.join(', ') + '.', function () {
                removeErrorScreen();
                state.failed = false;
                state.cause = null;
                state.progressChangedAt = Date.now();
                startWatchdog();
                startBlazor();
            });
            return;
        }

        startWatchdog();
        startBlazor();
    }

    window.observerBoot = {
        report: report,
        status: function () {
            return state.ready ? 'ready' : state.failed ? 'failed' : 'loading';
        }
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', start);
    } else {
        start();
    }
}());
