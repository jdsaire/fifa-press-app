// Language storage, and the one attribute the document owes an assistive
// technology.
//
// Loaded as a module by LocaleProvider.razor, the same way ThemeTrigger loads
// theme.js: one static file, imported on first render, no <script> tag.
//
// This file mirrors theme.ts in one respect and deliberately not in another.
// The division of labour is the same: something else owns the content — the
// per-locale JSON owns every translated string, as CSS owns every colour — and
// this module only remembers which language was chosen and tells the document
// which one it is in. No translated text appears here.
//
// UNLIKE THEME, THIS DOES NOT READ AN AMBIENT SYSTEM PREFERENCE. theme.ts's
// getSystemTheme reads prefers-color-scheme because a person who set a system
// dark-mode preference has already answered that question. Reading the
// browser's declared languages the same way was tried and deliberately
// reversed: EN is the fixed default on a first visit, full stop, and the only
// way a person reaches ES or PT is the language switch itself or a choice this
// module already remembered. If that default is ever revisited, it is a
// one-function addition here, not a redesign.
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
 * The stored choice, or null when none has been made and the app should open
 * in its fixed default (English). The mirror of getStoredTheme.
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
