using FifaPressApp.Api.Endpoints;
using FifaPressApp.Api.Models;

namespace FifaPressApp.Api.Validation;

/// <summary>
/// The collected reasons one request was rejected.
///
/// <para>
/// <b>Every problem at once, never the first one.</b> A validator that returns
/// as soon as it finds a fault makes the caller fix one field, resubmit, and
/// discover the next — which is the same "find out by being refused" pattern
/// this whole project exists to argue against, just pointed at a developer
/// instead of a journalist.
/// </para>
/// </summary>
public sealed class ValidationResult
{
    private readonly Dictionary<string, List<string>> problems = [];

    public bool IsValid => problems.Count == 0;

    /// <summary>Field name to everything wrong with it.</summary>
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Details =>
        problems.ToDictionary(entry => entry.Key, entry => (IReadOnlyList<string>)entry.Value);

    public void Add(string field, string message)
    {
        if (!problems.TryGetValue(field, out var list))
        {
            list = [];
            problems[field] = list;
        }

        list.Add(message);
    }
}

/// <summary>
/// Checks incoming data before anything is stored.
///
/// <para>
/// <b>Hand-written, and no validation library.</b> The rules here are few and
/// domain-specific, and every one of them is readable in place. Reaching for a
/// framework would add a dependency to express eight conditions that fit on one
/// screen.
/// </para>
///
/// <para>
/// <b>Why an enum is validated rather than parsed optimistically.</b> Parsing
/// an unknown track name throws, and an exception escaping to the error handler
/// would return a 500 — "we broke" — for what is plainly a bad request. The
/// caller's mistake must not be reported as the server's fault.
/// </para>
/// </summary>
public static class InputValidator
{
    public static ValidationResult Validate(AccreditationInput input, string credentialId)
    {
        var result = new ValidationResult();

        RequireText(result, nameof(input.HolderName), input.HolderName);
        RequireText(result, nameof(input.Outlet), input.Outlet);
        RequireEnum<TrackId>(result, nameof(input.TrackId), input.TrackId);
        RequireEnum<AccreditationStatus>(result, nameof(input.Status), input.Status);

        if (string.IsNullOrWhiteSpace(credentialId))
        {
            result.Add(nameof(input.CredentialId), "A credential id is required.");
        }

        // A credential that permits no zone permits nothing. An empty list here
        // would produce a record that renders as an accreditation while granting
        // its holder access to no part of any venue.
        if (input.ZoneAccess is null || input.ZoneAccess.Count == 0)
        {
            result.Add(nameof(input.ZoneAccess), "At least one zone is required.");
        }
        else if (input.ZoneAccess.Any(string.IsNullOrWhiteSpace))
        {
            result.Add(nameof(input.ZoneAccess), "Zone names cannot be blank.");
        }

        // Approved-until-when. An approved record with no expiry is the one
        // combination the frontend cannot render honestly, because every
        // validity line it prints is "approved until <date>".
        if (string.Equals(input.Status, nameof(AccreditationStatus.Approved), StringComparison.OrdinalIgnoreCase)
            && input.ValidUntil is null)
        {
            result.Add(nameof(input.ValidUntil), "An approved record must say what date it is valid until.");
        }

        return result;
    }

    public static ValidationResult Validate(ChangeInput input)
    {
        var result = new ValidationResult();

        RequireText(result, nameof(input.ChangeId), input.ChangeId);
        RequireEnum<ChangeKind>(result, nameof(input.Kind), input.Kind);

        // The three fields a change cannot exist without, checked in every
        // language. A change that cannot say what happened, why, and what comes
        // next is malformed — and checking per locale is what stops a
        // half-translated change existing at all, which would otherwise be a
        // blank line that only appears once somebody switches language.
        RequireAllLocales(result, nameof(input.WhatChanged), input.WhatChanged);
        RequireAllLocales(result, nameof(input.Reason), input.Reason);
        RequireAllLocales(result, nameof(input.NextStep), input.NextStep);

        // A reason that restates the outcome is not a reason. "Your access was
        // revoked" explains nothing that "what changed" did not already say.
        if (input.Reason is not null && input.WhatChanged is not null)
        {
            foreach (var (locale, reason, what) in Pairs(input.Reason, input.WhatChanged))
            {
                if (Normalize(reason) == Normalize(what) && Normalize(reason).Length > 0)
                {
                    result.Add(nameof(input.Reason),
                        $"The {locale} reason must explain why the change happened, not restate what changed.");
                }
            }
        }

        // A dead end still has to name who decided.
        if (input.NextStepIsActionable == false && IsMissing(input.DecidedBy))
        {
            result.Add(nameof(input.DecidedBy),
                "DecidedBy is required when the next step is not actionable.");
        }

        // A change hanging on an unplayed fixture has to say what the condition
        // is, or it reads as a decision already taken.
        if (input.DependsOnMatchNumber is not null && IsMissing(input.ConditionText))
        {
            result.Add(nameof(input.ConditionText),
                "ConditionText is required when a change depends on a fixture.");
        }

        return result;
    }

    private static void RequireText(ValidationResult result, string field, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result.Add(field, $"{field} is required and cannot be empty.");
        }
    }

    private static void RequireEnum<TEnum>(ValidationResult result, string field, string? value)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result.Add(field, $"{field} is required.");
            return;
        }

        if (!Enum.TryParse<TEnum>(value, ignoreCase: true, out _))
        {
            result.Add(field,
                $"'{value}' is not a recognised {field}. Expected one of: {string.Join(", ", Enum.GetNames<TEnum>())}.");
        }
    }

    private static void RequireAllLocales(ValidationResult result, string field, LocalizedText? value)
    {
        if (value is null)
        {
            result.Add(field, $"{field} is required in English, Spanish and Portuguese.");
            return;
        }

        foreach (var (locale, text) in Locales(value))
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                result.Add(field, $"{field} is required in {locale}.");
            }
        }
    }

    private static bool IsMissing(LocalizedText? value) =>
        value is null || Locales(value).Any(entry => string.IsNullOrWhiteSpace(entry.Text));

    private static (string Locale, string Text)[] Locales(LocalizedText value) =>
        [("English", value.En), ("Spanish", value.Es), ("Portuguese", value.Pt)];

    private static (string Locale, string Reason, string What)[] Pairs(LocalizedText reason, LocalizedText what) =>
    [
        ("English", reason.En, what.En),
        ("Spanish", reason.Es, what.Es),
        ("Portuguese", reason.Pt, what.Pt),
    ];

    private static string Normalize(string? value) =>
        new string([.. (value ?? string.Empty).Where(char.IsLetterOrDigit)]).ToLowerInvariant();
}
