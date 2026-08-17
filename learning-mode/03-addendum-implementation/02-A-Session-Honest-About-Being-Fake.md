# 02 — A Session Honest About Being Fake

## The Problem With the Old Sign-In Form

Before this run, `/signin` was a form that validated what you typed and then told you, on screen, that sign-in wasn't actually implemented. Nothing behind it changed what you could see. That was an honest design for the app it described — but it also meant the app couldn't demonstrate one of its own central ideas: that *who* you are changes *what you see*, because two different accreditation holders genuinely see different things.

## A Session That Works, Documented as What It Isn't

`SimulatedSessionProvider` (in `Services/`) is a real, working session — signing in with one of the two published demo accounts genuinely changes which record renders, which nav rows appear, and which routes let you in. But it is not authentication in any real sense, and the class's own comments say so directly: there's no server, no token, nothing verifying anything. It's a single C# object, shared across the whole browser tab, holding "who is currently signed in" as a field — and it forgets that the moment you refresh the page, on purpose, because remembering it would make a demonstration look more like a real account system than it is.

The two demo passwords are printed in plain text right next to the sign-in form. That isn't an oversight — hiding a password that's compiled straight into the app and shipped to every visitor's browser wouldn't protect anything; it would just be theatre. The honest move is to say plainly what this is: a demonstration of what a signed-in holder would see, not a security boundary.

## Why Two Records, and What They're Meant to Prove

A single seeded holder can't demonstrate the difference a holder's *situation* makes to how urgently they're notified. So this run added a second one — a rights-holder broadcaster with a named contact at FIFA — specifically so an identical kind of change could be shown resolving two different ways: interrupting one holder immediately, and just quietly appearing in the log for the other, because that second holder has someone to ask if a condition ever needs explaining.

The two records are otherwise kept as similar as the underlying persona data allows. That's deliberate: if six things were different between them, you couldn't tell which one was doing the work. With only the "has a named contact" fact differing, the different outcome is unambiguously caused by that one fact — and nothing new had to be written to make it happen. The rule already existed in the codebase from an earlier run; this run just finally gave it two records to prove itself against.

## Gating Without Pretending to Be Secure

Once a session exists, it made sense to gate the one screen that's genuinely personal — a person's own accreditation record — behind it. But the *way* it's gated matters as much as the fact of it. A signed-out visitor who reaches `/record` is never shown anything that borrows the vocabulary of real access control — no "403," no "Access Denied." They're told plainly what the page needs and why, with a direct route to go get it. The distinction is the whole point: this demonstrates what a signed-in holder sees, and it never claims to be protecting anything.
