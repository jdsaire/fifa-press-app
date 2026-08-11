# GLOSSARY

**Repo path:** `/Design Research/GLOSSARY.md`
**Status:** Not a gate deliverable. Added after the eight-gate deploy, per `BACKLOG.md` item 1.

Every term in this repo that a reader outside UX research, software, or football administration might reasonably not know. Written for someone encountering all three worlds for the first time. If a definition here needs another definition to make sense, that's a defect — tell me and I'll fix it.

---

## 1. How this repo labels things

These tags appear on almost every claim in the repo. They exist because the study behind it used invented data, and a reader needs to know at a glance which sentences rest on facts and which don't.

| Tag | In plain words |
|---|---|
| **`[SOURCED]`** | This traces back to a real, published document — usually something FIFA itself put out. Trustworthy. |
| **`[SIMULATED]`** | This comes only from the made-up data in this study. It is not evidence of anything about real people. |
| **`[ASSUMPTION]`** | Neither of the above. Someone reasoned their way to it. Treat with suspicion. |
| **`[VERIFIED]` / `[INFERRED]` / `[REPORTED]`** | Used inside the Run 1 research file for its sources. *Verified* = confirmed in an official document. *Inferred* = reasonably deduced but not stated anywhere. *Reported* = a news outlet or trade publication said it, but no official source confirms it. |
| **`SIMULATED — NOT EVIDENCE`** | A header stamped on every file containing invented data, so nobody can quote a number from it without seeing the warning first. |
| **Run 1 / Run 2** | Two stages of work. Run 1 gathered real published facts. Run 2 (everything else here) invented a study on top of them. |
| **Gate** | One step of Run 2, numbered 0 to 7. Each produced one file and required approval before the next began. |
| **P01–P05** | Code names for the five invented interview participants. Used instead of names because the people aren't real and shouldn't be mistaken for real. |
| **R01–R30** | Code names for the thirty invented survey respondents. |

---

## 2. Research terms

### The basics

| Term | In plain words |
|---|---|
| **UX research** | Studying the people who will use a product, so it gets built around what they actually need rather than what the builders assume. |
| **Primary research** | Going and asking people directly — interviews, surveys, watching them work. |
| **Secondary research** | Reading what already exists — documents, reports, news. Run 1 was entirely this. |
| **Guerrilla research** | Research done fast and cheap, by one or two people with no budget. The honest opposite of a funded, formal study. |
| **Qualitative** | Words. What people say, and what it means. Small numbers of people, studied closely. |
| **Quantitative** | Numbers. How many, how often, how much. Larger numbers of people, studied shallowly. |
| **Simulated (or synthetic) data** | Data somebody wrote by hand to look like real answers. Used here because no real people were available. It can demonstrate that an analysis works; it can never prove anything about the world. |

### Planning a study

| Term | In plain words |
|---|---|
| **SMART goal** | A research aim written so precisely that you can tell whether you achieved it — as opposed to something vague like "understand our users." |
| **Hypothesis** | A prediction stated before you look at the data, so the data has a chance to prove you wrong. |
| **Falsifiable** | A prediction specific enough that some result *could* disprove it. A prediction nothing could disprove is worthless. |
| **Disconfirming observation** | The result that would prove your prediction wrong, written down in advance so you can't quietly move the goalposts later. |
| **Screener** | A few questions at the start that decide whether someone is the right person for the study. |
| **Disqualification logic** | The rules for who the screener turns away, and why. |
| **Sample** | The group of people studied, standing in for the much larger group you actually care about. |
| **Stratified sample** | A sample deliberately built to include set numbers from each type of person, rather than whoever turns up. |
| **Quota** *(in research)* | How many people of each type the sample is supposed to include. *(Note: "quota" means something completely different in the FIFA sections — see §3.)* |
| **n** | Simply the number of people. "n = 30" means thirty people. |
| **Statistical power** | Whether you have enough people for the numbers to mean anything. Thirty people does not. |
| **Saturation** | The point in interviewing where new participants stop telling you anything new. This study never claims to have reached it. |
| **Consent notice** | What you tell participants before they take part — what the study is, what happens to their words, and that they can stop. |

### Measuring

| Term | In plain words |
|---|---|
| **UMUX-Lite** | A standard two-question test of how usable a system is. Both questions are always the same — roughly *"it does what I need"* and *"it's easy to use"* — answered on a 1-to-7 agreement scale. It's popular because it's short, and because using the same wording everywhere lets different products be compared. In this repo the wording was slightly changed for translation reasons, which weakens that comparability, and the answers were invented anyway — so the scores here mean nothing outside this file. |
| **Agreement scale** | Answer options running from "strongly disagree" to "strongly agree." |
| **Frequency scale** | Answer options running from "never" to "always." |
| **Forced choice** | A question offering only two options, designed so the answer reveals which of two things someone values more. |
| **Baseline** | A measurement of how things work *now*, taken so you can tell later whether you improved anything. This project has none, which is stated wherever a metric appears. |

### Analysing

| Term | In plain words |
|---|---|
| **Descriptives** | Plain summary numbers — averages, counts, how answers were spread. No claims, just description. |
| **Inferential test** | A calculation that asks whether a pattern in your data is likely to be real or likely to be coincidence. |
| **p-value** | Roughly: how likely you'd see this pattern by pure chance. Small numbers (under 0.05) conventionally count as "probably not chance." **Important:** on invented data a small p-value proves only that the calculation ran correctly. |
| **Effect size** | How *big* a difference is, as opposed to how confident you are that it exists. A tiny difference can be statistically certain and practically irrelevant. Reported here as *r* or *Cohen's h*. |
| **Mann-Whitney U** | A test comparing two groups when the data is lopsided or the groups are small. Used here to compare years of experience between people who understood the access rules and people who didn't. |
| **Binomial test** | A test asking whether a two-way split (24 chose A, 6 chose B) is meaningfully different from a coin flip. |
| **Coding** | Reading interview transcripts and tagging recurring ideas with short labels, so patterns become visible. |
| **Codebook** | The list of those labels with their definitions and an example of each. |
| **Emergent code** | A label that came out of the interviews themselves rather than one the researcher expected to find. Marked separately because unexpected findings are worth more than confirmed ones. |
| **Theme** | A bigger pattern assembled from several codes. The main output of qualitative analysis. |
| **Triangulation** | Checking whether two different sources — say, the survey and the interviews — point the same way. When they disagree, that disagreement is itself a finding. |
| **Inter-rater reliability** | Whether two independent people reading the same transcripts would tag them the same way. Impossible here: one person wrote the data, the labels, and the conclusions. Stated openly in Gate 4. |

### Design artifacts

| Term | In plain words |
|---|---|
| **Persona** | A single fictional person written to represent a whole group of users, so the team can ask "would this work for her?" instead of arguing about an abstraction. |
| **Empathy map** | A one-page summary of what a persona says, thinks, does and feels, plus what frustrates them and what would help. |
| **Scenario** | A short story of someone using the product. *Typical* = the ordinary day. *Critical* = something goes wrong but is survivable. *Tragic* = it fails badly. |
| **Service blueprint** | A map of a whole service laid out in layers: what the customer does, what they can see, and — crucially — everything happening out of sight that determines whether it works. Used here because this service mostly fails out of sight. |
| **Frontstage** | The parts the user can see. A theatre metaphor: what happens on stage. |
| **Backstage** | The parts they can't see but that determine their outcome. Where this particular service breaks. |
| **Support processes** | The systems and organisations behind the scenes that make each step possible. |
| **Evidence** *(blueprint column)* | The tangible thing the user ends up holding — an email, a card, a receipt — that proves a step happened. |
| **Failure point** | A step where things are known to go wrong. |
| **Wait state** | Time spent waiting with nothing to do and often nothing to see. In this service, one wait lasts weeks and shows the applicant nothing at all. |
| **Journey map** | A simpler artifact showing a user's steps and feelings over time. Deliberately not produced here, because the blueprint already carries the emotional row. |
| **Zero visibility (ZV)** | A step where FIFA itself cannot see what's happening — because a federation, a government, or a consulate is doing it. Marked throughout the blueprint. |
| **How Might We (HMW)** | A design question phrased to invite solutions without naming one. Narrow enough to act on, open enough not to pre-decide the answer. |
| **Information architecture (IA)** | How information is organised, named, and structured — what things are called and where they live. |
| **Design principle** | A rule the team agrees to before designing, each paired here with what it forbids, so it constrains real decisions instead of decorating a slide. |

### Findings named in this repo

| Term | In plain words |
|---|---|
| **Discovery-by-failure** | Learning that something about your situation changed only by trying to do something and being blocked. Nobody told you; the system let you find out the hard way. |
| **Gate-desync** | The staff checking credentials at a stadium entrance are working from older information than the person presenting the credential. Both are following the rules; the rules disagree. |
| **Two-token** | Shorthand for the fact that entry requires *two* separate permissions, not one — see §3. |

---

## 3. Football and accreditation terms

| Term | In plain words |
|---|---|
| **Accreditation** | Official permission to work at an event as a member of the press. In football it is not a ticket — see the next two entries. |
| **The two-token problem** | The central confusion in this project. A World Cup **accreditation** gets a journalist into the stadium's media centre and nothing more. Getting to the pitch, the players, or the press conference requires a **separate match ticket**, requested per match, limited to one match per day. Many people assume the first covers the second and find out otherwise at a barrier. |
| **Media ticket** | That second permission — the per-match one. |
| **Member Association (MA)** | A national football federation (the English FA, the Brazilian CBF, and so on). FIFA's members are these organisations, not individuals. |
| **Control key** | A unique code a national federation gives a journalist. Without it, the application form cannot even be opened. This makes the federation, not FIFA, the gatekeeper — and FIFA cannot see how fairly or quickly keys are handed out. |
| **Quota** *(in football)* | The number of press places FIFA allocates to each national federation, which then decides who gets them. *(Different from the research meaning in §2.)* |
| **Quota reallocation** | Those numbers changing mid-tournament — typically shrinking once a country's team is knocked out and its journalists are presumed to be going home. |
| **Confederation** | One of six regional bodies grouping national federations: **UEFA** (Europe), **CONMEBOL** (South America), **CAF** (Africa), **AFC** (Asia), **CONCACAF** (North/Central America and Caribbean), **OFC** (Oceania). |
| **Rights-holder** | A broadcaster that has paid for the right to televise matches. Handled by a separate, better-resourced FIFA process with a named contact. |
| **Non-rights broadcaster (NRH)** | A broadcaster without those rights, allowed only limited news access. |
| **Wire agency / photo agency** | Organisations like Reuters or AFP that supply news and images to many outlets at once. A small number apply to FIFA directly, bypassing the federation route. |
| **Mixed zone** | A corridor players walk through after a match where journalists can stop them for comment. Requires the match ticket, not just accreditation. |
| **Photo position** | An assigned spot at pitchside for a photographer. Limited, allocated per match, and heavily contested for big games. |
| **Media centre** | The working room inside a stadium with desks, wifi and screens. What accreditation alone gets you. |
| **IBC (International Broadcast Centre)** | The central hub where broadcasters run their operations for the whole tournament. |
| **FIFA Media Hub** | FIFA's website for accreditation applications and match ticket requests. |
| **Quota Management System (QMS)** | FIFA's internal system for tracking federation allocations and issuing control keys. |
| **Security vetting** | A background check run by the host country's national authorities, not FIFA. FIFA cannot see the reasoning or overturn the outcome. |
| **Single-entry visa** | A visa allowing one entry into a country. If a journalist leaves to follow their team to a neighbouring host country, they cannot come back — a documented and serious problem in 2026, when the tournament spanned three countries. |
| **Biometric access** | Entry controlled by a physical characteristic such as a face scan, rather than by inspecting a card. |
| **AIPS** | The international association of sports journalists. It formally protested to FIFA in June 2026 about accredited journalists being refused visas. |
| **ATA carnet** | A customs document letting professional equipment cross borders temporarily without import duties. Relevant because broadcast gear crossed three countries in 2026. |
| **Host city** | A city staging matches. Each runs its own local press arrangements alongside FIFA's. |

---

## 4. A note on two words that mean two things

**"Quota"** means a research sample target in Gates 1 and 3, and a federation's allocation of press places everywhere else. Context makes it clear, but the collision is worth knowing about.

**"Gate"** means a numbered step of this study (Gate 0 through Gate 7), and also a physical stadium entrance. Where a stadium entrance is meant, the repo says *stadium gate*, *barrier*, or *turnstile*.

---

*Added post-deploy. Closes `BACKLOG.md` item 1.*
