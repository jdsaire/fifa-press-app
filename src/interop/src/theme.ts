// Theme storage and application.
//
// Loaded as a module by ThemeTrigger.razor, so it needs no <script> tag in
// index.html and adds no dependency — it is one static file the component
// imports when it first renders.
//
// The division of labour matters: CSS owns which palette applies (:root for
// light, a prefers-color-scheme block for the system default, and a
// [data-theme] attribute for an explicit choice). This file only ever sets or
// clears that one attribute and remembers what was chosen. No colour value
// appears here.
//
// CONVERTED FROM JAVASCRIPT, MECHANISM UNCHANGED. Every function keeps its
// name, its signature, its behaviour and its reasoning. What the conversion
// adds is the Theme union below: 'light' | 'dark' is a type now rather than a
// convention, so a fourth theme string cannot reach applyTheme by accident, and
// the storage read that has always narrowed to those two values now says so in
// a way the compiler enforces.

/**
 * The only two themes there are. A union rather than a string, because every
 * function here already treated it as one and the checks below were the proof.
 */
export type Theme = 'light' | 'dark';

const STORAGE_KEY = 'fifa-press-app.theme';

function isTheme(value: string | null): value is Theme {
    return value === 'light' || value === 'dark';
}

/**
 * The stored choice, or null when none has been made and the system preference
 * should be left to decide.
 */
export function getStoredTheme(): Theme | null {
    try {
        const stored = localStorage.getItem(STORAGE_KEY);
        return isTheme(stored) ? stored : null;
    } catch {
        // Private browsing and blocked storage both throw here. A theme that
        // cannot be remembered is not a reason to fail to render one.
        return null;
    }
}

export function getSystemTheme(): Theme {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
        ? 'dark'
        : 'light';
}

/**
 * Setting the attribute is what makes an explicit choice stick: the
 * stylesheet's system-preference block is written to stand down whenever it is
 * present, so the operating system flipping to dark at sunset cannot quietly
 * overturn someone who picked light on purpose.
 *
 * The parameter is deliberately wider than Theme. .NET calls this through
 * IJSObjectReference, which type-checks nothing, so the guard has to stay a
 * runtime guard — the union documents the intent and the check enforces it.
 */
export function applyTheme(theme: string | null): void {
    if (isTheme(theme)) {
        document.documentElement.setAttribute('data-theme', theme);
    } else {
        document.documentElement.removeAttribute('data-theme');
    }
}

export function storeTheme(theme: string): void {
    try {
        localStorage.setItem(STORAGE_KEY, theme);
    } catch {
        // Same as above: the choice still applies for this session, it just
        // will not survive a reload.
    }
}

export function clearStoredTheme(): void {
    try {
        localStorage.removeItem(STORAGE_KEY);
    } catch {
        // Nothing to do.
    }
}
