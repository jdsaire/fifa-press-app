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

const STORAGE_KEY = 'fifa-press-app.theme';

// Returns 'light', 'dark', or null when no explicit choice has been made and
// the system preference should be left to decide.
export function getStoredTheme() {
    try {
        const stored = localStorage.getItem(STORAGE_KEY);
        return stored === 'light' || stored === 'dark' ? stored : null;
    } catch {
        // Private browsing and blocked storage both throw here. A theme that
        // cannot be remembered is not a reason to fail to render one.
        return null;
    }
}

export function getSystemTheme() {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
        ? 'dark'
        : 'light';
}

// Setting the attribute is what makes an explicit choice stick: the stylesheet's
// system-preference block is written to stand down whenever it is present, so
// the operating system flipping to dark at sunset cannot quietly overturn
// someone who picked light on purpose.
export function applyTheme(theme) {
    if (theme === 'light' || theme === 'dark') {
        document.documentElement.setAttribute('data-theme', theme);
    } else {
        document.documentElement.removeAttribute('data-theme');
    }
}

export function storeTheme(theme) {
    try {
        localStorage.setItem(STORAGE_KEY, theme);
    } catch {
        // Same as above: the choice still applies for this session, it just
        // will not survive a reload.
    }
}

export function clearStoredTheme() {
    try {
        localStorage.removeItem(STORAGE_KEY);
    } catch {
        // Nothing to do.
    }
}
