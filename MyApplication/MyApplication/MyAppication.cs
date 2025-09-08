using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MyApplication
{
    /// <summary>
    /// Email validation utilities with RFC 5322–oriented syntax checks, IDN handling, and optional DNS existence check.
    /// </summary>
    public static partial class EmailValidator
    {
        private const int MaxEmailLength = 254;
        private const int MaxLocalPartLength = 64;

        // RFC 5322–inspired (practical subset). Supports quoted local parts.
        [GeneratedRegex(@"^(?:(?:[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*)|""(?:[\x01-\x08\x0B\x0C\x0E-\x1F\x21\x23-\x5B\x5D-\x7F]|\\[\x01-\x09\x0B\x0C\x0E-\x7F])*"")@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?)(?:\.(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?))*$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
        private static partial Regex CorePattern();

        public readonly record struct EmailValidationResult(bool IsValid, string? ErrorMessage)
        {
            public override string ToString() => IsValid ? "Valid" : $"Invalid: {ErrorMessage}";
        }

        /// <summary>
        /// Validates an email address with syntax, length, IDN normalization and optional DNS host existence.
        /// </summary>
        public static async Task<EmailValidationResult> ValidateAsync(
            string? email,
            bool performDnsLookup = false,
            CancellationToken cancellationToken = default)
        {
            var baseResult = ValidateBasic(email, out string? normalized, out string? domain);
            if (!baseResult.IsValid || !performDnsLookup || domain is null)
                return baseResult;

            try
            {
                // Lightweight existence check (A/AAAA). MX requires external DNS library.
                _ = await Dns.GetHostEntryAsync(domain);
                return baseResult;
            }
            catch (Exception ex) when (ex is SocketException or ArgumentException)
            {
                return new EmailValidationResult(false, "Domain does not resolve (A/AAAA lookup failed).");
            }
        }

        /// <summary>
        /// Synchronous validation without DNS lookups.
        /// </summary>
        public static EmailValidationResult Validate(string? email) =>
            ValidateBasic(email, out _, out _);

        private static EmailValidationResult ValidateBasic(
            string? input,
            out string? normalizedEmail,
            out string? asciiDomain)
        {
            normalizedEmail = null;
            asciiDomain = null;

            if (string.IsNullOrWhiteSpace(input))
                return Fail("Empty email.");

            string email = input.Trim();

            if (email.Length > MaxEmailLength)
                return Fail("Exceeds maximum length (254).");

            int atIndex = email.IndexOf('@');
            if (atIndex <= 0 || atIndex != email.LastIndexOf('@') || atIndex == email.Length - 1)
                return Fail("Must contain a single '@' with non-empty local and domain parts.");

            string local = email[..atIndex];
            string domain = email[(atIndex + 1)..];

            if (local.Length > MaxLocalPartLength)
                return Fail("Local part exceeds 64 characters.");

            if (local.StartsWith('.') || local.EndsWith('.') || local.Contains(".."))
                return Fail("Local part has invalid dot usage.");

            if (domain.StartsWith('-') || domain.EndsWith('-') || domain.Contains(".."))
                return Fail("Domain has invalid structure (hyphen/dot placement).");

            // IDN normalization
            try
            {
                var idn = new IdnMapping();
                string[] labels = domain.Split('.');
                for (int i = 0; i < labels.Length; i++)
                {
                    if (labels[i].Length == 0)
                        return Fail("Domain has empty label.");
                    labels[i] = idn.GetAscii(labels[i]);
                    if (labels[i].Length > 63)
                        return Fail("A domain label exceeds 63 characters.");
                }
                asciiDomain = string.Join('.', labels);
            }
            catch (ArgumentException)
            {
                return Fail("Domain contains invalid IDN characters.");
            }

            string candidate = $"{local}@{asciiDomain}";

            // Core regex (after IDN ASCII conversion of domain).
            if (!CorePattern().IsMatch(candidate))
                return Fail("Fails RFC 5322–compatible syntax pattern.");

            normalizedEmail = candidate;

            return new EmailValidationResult(true, null);

            static EmailValidationResult Fail(string reason) => new(false, reason);
        }
    }

    public class MyAppication
    {
        // Example usage:
        // var basic = EmailValidator.Validate("user@example.com");
        // var withDns = await EmailValidator.ValidateAsync("user@example.com", performDnsLookup: true);
    }
}
