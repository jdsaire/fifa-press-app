# The hosting decision

**Status:** decided by this run. This document is the formal record.

## Why this document exists at all

Run 4C — the backend layer — was **blocked**, on paper, before it started.

The scope patch that approved SignalR for this run
(`P-PROTOTYPE_FIFA_Run4-Scope-PATCH_v1.md`, §5.1) pointed out a problem with its
own approval: a persistent server connection is structural to this layer, and
GitHub Pages, where this project's frontend is deployed, serves static files
only. It cannot run a server. The patch declared the hosting question converted
"from parked to blocking" and stopped there, deliberately, so the question would
be answered before a run started rather than discovered in the middle of one.

This document answers it.

## The decision

**The API is intended to run on Azure App Service, free tier (F1), with the
deployed GitHub Pages frontend calling it directly across origins.**

The binding constraint was **no cost**. Not "low cost" — none. This is a
portfolio project, it will sit online indefinitely, and a hosting bill that
arrives every month for a demonstration nobody is using is a bad trade at any
price.

### Why this option

| Option | Why not |
|---|---|
| **Azure App Service, free tier** | **Chosen.** Free indefinitely, not a trial. Runs ASP.NET Core natively with no packaging step. Supports WebSockets, which SignalR needs. Publishing is one command. |
| Azure Container Apps | Has a free grant, but requires containerising the API. Docker is explicitly outside this run's technical ceiling, and adding it to solve a hosting problem would be scope creep with a hosting excuse. |
| Azure SignalR Service | A managed service purpose-built for this. It has a free tier, but it is a second resource to provision and explain for an app with two demo users and one hub. The self-hosted hub inside the API is sufficient here. |
| Anything paid | Ruled out by the constraint. |
| Not deploying at all | Considered seriously, and it is the fallback. The frontend does not need the API — see below. |

### What this run did and did not do

This run **provisions nothing**. It has no Azure credentials, creates no
resource, and depends on no external service beyond GitHub. What it produced is
an API that runs correctly on a free-tier host and step-by-step instructions for
the principal to deploy it, in
[`05_RUNNING-AND-DEPLOYING.md`](05_RUNNING-AND-DEPLOYING.md).

**Until somebody follows those steps, no API exists.** The deployed site is
unaffected by that, on purpose: the frontend ships with no API URL configured
and runs on its in-memory mock exactly as it did before this layer was written.
Configuring a URL is what opts in, and nothing else does.

## What the free tier costs you, in behaviour

Two consequences an inspector should know about before concluding something is
broken.

**Cold starts.** The free tier stops your application when nothing has called
it for a while — around twenty minutes of idleness. The next request has to wait
for it to start again, which takes several seconds and can look like a hang or a
failure. This is normal and there is no way to prevent it on a free plan without
paying for an "always on" setting.

The frontend is built to survive this rather than to hide it: a read that times
out falls back to the last record it successfully fetched, clearly labelled with
when that was, and the app carries on. That behaviour was not added for Azure —
the staleness indicator has been there since v9, because the concept was always
that a record should say how old it is. The free tier just makes it earn its
keep.

**No managed SignalR resource.** The hub runs inside the API process, on the
same free instance. There is no Azure SignalR Service, which means no
guaranteed connection capacity and no scale-out beyond that one instance. For
two demo holders this is not a limitation anyone will meet. For anything real it
would be the first thing to change.

## The frozen brief, and why this decision is recorded here

[`ux-ui/03-ui-prototyping/07_BUILD-BRIEF.md`](../ux-ui/03-ui-prototyping/07_BUILD-BRIEF.md)
is the build brief for run 4B — the frontend vertical slice — and its §5
anti-scope-creep list opens with "Add a backend, API, or database." That was
correct when written: 4B was a frontend run, and a backend appearing inside it
would have been exactly the scope creep the list existed to prevent.

That brief is a **frozen document**. This project's standing rule is that a
completed dossier is never edited afterwards — a record that gets quietly
rewritten to agree with later decisions stops being a record. So the brief still
says what it said, the prohibition it states was true of the run it governed,
and this document is where the later decision lives instead.

**Nothing in `07_BUILD-BRIEF.md` has been edited by this run.** Not one
character. If you diff it against any earlier version you will find it
unchanged, including the line that appears to forbid what this folder documents.

## Simulated authentication

Stated here because it belongs in any document about where this thing runs: the
API's token check is a simulated string comparison, not authentication. The
token is published in this repository. Deploying this API to a public URL does
not put anything behind a lock, and it should not be described as though it
does. See [`03_MIDDLEWARE-PIPELINE.md`](03_MIDDLEWARE-PIPELINE.md).
