# 05 — Parsing a Real CSV

## The File

[`2026_World_Cup_Schedule.csv`](../../src/FifaPressApp/wwwroot/data/2026_World_Cup_Schedule.csv) is the published 2026 World Cup schedule: a header row and 104 fixtures, eight columns each.

```
Match,Date,Time (ET),Time (Local),Matchup,Group / Phase,Venue,City
1,11-Jun-26,15:00,13:00,Mexico v South Africa,Group A,Estadio Azteca,Mexico City
```

It's a small, clean, entirely ordinary spreadsheet. It still contains three things that break a naive parser, and all three are the kind you only find by looking.

[`FixtureImporter.cs`](../../src/FifaPressApp/Services/FixtureImporter.cs) does the reading. It's kept separate from the data provider on purpose: it understands a file format and knows nothing about access, entitlements, or which fixtures the app is allowed to look at.

## Breakage 1: A Date That Means Different Things on Different Machines

`11-Jun-26` is day, short month name, two-digit year. .NET will parse that if you tell it the format:

```csharp
DateTime.TryParseExact(value, "d-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
```

`CultureInfo.InvariantCulture` is the part that isn't decoration. Without it, .NET uses whatever culture the machine is set to — and `MMM` means "the abbreviated month name **in that culture**." On a machine set to English, `Jun` parses. On one set to French, it doesn't, because June abbreviates to `juin`. The same code, the same file, a different result depending on a setting that has nothing to do with either.

That's a particularly nasty class of bug: it works on the machine it was written on, and the failure appears somewhere else entirely, looking like corrupt data rather than a locale mismatch.

`d-MMM-yy` also matters as written. `d` accepts both `1-Jul-26` and `11-Jun-26`; `dd` would insist on two digits and reject every single-digit day in the file.

## Breakage 2: Two Values in One Column

The `Matchup` column holds both teams as one string: `Mexico v South Africa`. Splitting it is unavoidable, and the separator has to be chosen carefully.

Splitting on `"v"` is wrong immediately — it would cut into every team name containing the letter. Splitting on `" v "`, with spaces on both sides, works for all 104 rows here.

It's still a guess about the data, not a guarantee about the format. Nothing in a CSV declares that a column holds two things joined by a particular string; the file just happens to look that way. So the importer checks rather than assumes, and throws with the line number if a row doesn't match:

```
Line 42: 'Something Else' does not name two teams separated by 'v'.
```

The alternative — silently skipping unparseable rows — would leave the app running with a schedule quietly missing fixtures, and every screen above it would be confidently wrong. A parse failure that stops and names the line is much cheaper than one that keeps going.

## Breakage 3: A Clock That Says 24:00

Three rows record their Eastern kickoff as `24:00`:

```
6,13-Jun-26,24:00,21:00,Australia v Türkiye,Group D,BC Place,Vancouver
```

There is no such time. Clocks run 00:00 to 23:59, and any strict time parser rejects `24:00` outright.

What it means is clear enough — midnight ending that day, written as `24:00` so it stays visually attached to the date of the match rather than jumping to the next one. A West Coast evening kickoff is past midnight in Eastern time.

The importer handles it explicitly, because the only alternatives are rejecting three valid fixtures or mangling them:

```csharp
if (hour == 24 && minute == 0)
{
    return date.AddDays(1);
}
```

Midnight ending one day is the same instant as the start of the next, so adding a day and taking midnight is exactly right.

Worth noting: `24:00` appears only in the Eastern column, never in local time. The local column is the one the app compares against its simulated "now", so the field that actually matters for the withholding rule in [File 01](01-Putting-the-Data-Behind-a-Door.md) parses cleanly. That's luck, not design, and it would have been an unpleasant surprise the other way round.

## Things That Didn't Break, and Why That's Not Reassuring

Two hazards this file happens to dodge:

**Commas inside fields.** CSV separates on commas, so a field containing one has to be quoted — and then the parser needs to understand quoting. Splitting on `,` and counting eight columns works here because no field contains a comma. Add one venue named `Arena, North` and the naive split silently produces nine columns and shifts everything after it.

**Non-ASCII characters.** `Türkiye` and `Curaçao` are in the data and work correctly, because the file is UTF-8 and .NET reads it as UTF-8 by default. Get the encoding wrong and these don't crash — they turn into `TÃ¼rkiye` and keep going, which is worse than crashing because nothing reports an error.

The importer checks the column count on every row and throws if it isn't eight. That won't rescue a quoted comma, but it will notice that something changed rather than carrying on with misaligned data.

## What This Folder Covered

Five decisions, and what each one was actually for:

- Putting the data behind an interface, so swapping the implementation is one line rather than a search through every page — and so one rule about what the app is allowed to know lives in one place.
- Naming colours instead of spelling them out, and defining two themes together rather than inverting one.
- A record that only appends, so the past stays readable and a status can't disagree with its own log.
- Reading local data before the network, and what a loading state becomes once there's nothing to wait for.
- Reading a real file, where the awkward parts are a locale-sensitive date, two values in one column, and a clock reading 24:00.

For any term here that didn't land, [`Glossary.md`](../Glossary.md) has it in one paragraph with a pointer to where it shows up in the code.
