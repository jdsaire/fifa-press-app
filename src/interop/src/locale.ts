// Language storage, and the one attribute the document owes an assistive
// technology.
//
// Loaded as a module by LocaleProvider.razor, the same way ThemeTrigger loads
// theme.js: one static file, imported on first render, no <script> tag.
//
// This file is the exact sibling of theme.ts and is deliberately shaped like it.
// The division of labour is the same: something else owns the content — the
// per-locale JSON owns every translated string, as CSS owns every colour — and
// this module only remembers which language was chosen and tells the document
// which one it is in. No translated text appears here.
//
// WHY THE lang ATTRIBUTE IS NOT OPTIONAL. A screen reader picks its
// pronunciation rules from document.documentElement.lang. A page of Spanish
// prose still marked lang="en" is read out by an English voice — technically
// present, practically unusable. The language switch changes what the text says;
// this is what changes what a screen reader does with it.

/** The three languages the app is built in, as the codes the app stores. */
export type LocaleCode = 'en' | 'es' | 'pt';

const STORAGE_KEY = 'fifa-press-app.locale';

function isLocaleCode(value: string | null): value is LocaleCode {
    return value === 'en' || value === 'es' || value === 'pt';
}

/**
 * The stored choice, or null when none has been made and the browser's own
 * preference should be left to decide. The mirror of getStoredTheme.
 */
export function getStoredLocale(): LocaleCode | null {
    try {
        const stored = localStorage.getItem(STORAGE_KEY);
        return isLocaleCode(stored) ? stored : null;
    } catch {
        // Private browsing and blocked storage both throw here. A language that
        // cannot be remembered is not a reason to fail to render one.
        return null;
    }
}

/**
 * The best of the browser's declared languages, falling back to English.
 *
 * <p>The mirror of getSystemTheme, and the same argument: a person who has told
 * their browser they read Spanish has already answered this question, and asking
 * them again on every first visit is asking them to configure something they
 * configured once already.</p>
 *
 * <p>Only the primary subtag is read, so es-419 and pt-BR resolve the same way
 * es-ES and pt-PT do. This app has one variant of each language and pretending
 * otherwise would be a promise it does not keep.</p>
 */
export function getBrowserLocale(): LocaleCode {
    const declared = navigator.languages?.length ? navigator.languages : [navigator.language];

    for (const tag of declared) {
        const primary = tag?.split('-')[0]?.toLowerCase() ?? '';
        if (isLocaleCode(primary)) {
            return primary;
        }
    }

    return 'en';
}

/**
 * Sets the document's language. Unlike theme.ts's applyTheme this never clears
 * the attribute: a document is always in some language, and an absent lang is a
 * worse answer than a wrong one because it leaves the screen reader guessing.
 *
 * The parameter is wider than LocaleCode because .NET calls this through
 * IJSObjectReference, which type-checks nothing in either direction.
 */
export function applyLocale(code: string | null): void {
    document.documentElement.setAttribute('lang', isLocaleCode(code) ? code : 'en');
}

export function storeLocale(code: string): void {
    try {
        localStorage.setItem(STORAGE_KEY, code);
    } catch {
        // Same as theme.ts: the choice still applies for this session, it just
        // will not survive a reload.
    }
}

export function clearStoredLocale(): void {
    try {
        localStorage.removeItem(STORAGE_KEY);
    } catch {
        // Nothing to do.
    }
}
