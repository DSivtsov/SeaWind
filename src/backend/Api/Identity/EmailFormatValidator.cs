using Infrastructure.Postgres.Identity;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Api.Identity;

public class EmailFormatValidator : IUserValidator<AppUser>
{
    private static readonly Regex LabelRegex = new(@"^[A-Za-z0-9-]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly string TldPattern = @"^[A-Za-z0-9-]{2,63}$";
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(50);

    // IdnMapping methods are thread-safe if properties not modified concurrently
    private static readonly IdnMapping Idn = new() { UseStd3AsciiRules = true };

    public async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(user);

        var email = user.Email?.Trim();
        if (!IsLikelyEmail(email))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidEmail",
                Description = "Email имеет некорректный формат."
            });
        }

        // Опционально: проверка уникальности — предпочесть уникальный индекс в БД.
        if (manager.SupportsUserEmail)
        {
            var existing = await manager.FindByEmailAsync(email!).ConfigureAwait(false);
            if (existing != null && !string.Equals(existing.Id, user.Id, StringComparison.Ordinal))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Email уже используется."
                });
            }
        }

        return IdentityResult.Success;
    }

    private static bool IsLikelyEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        // MailAddress парсит локальную часть корректно (включая quoted local‑part)
        if (!MailAddress.TryCreate(email, out var addr)) return false;

        var localPart = addr.User;
        if (string.IsNullOrEmpty(localPart) || localPart.Length > 64) return false;

        // Нормализуем домен в ASCII (Punycode) и приводим к lower для проверок
        string hostAscii;
        try
        {
            hostAscii = Idn.GetAscii(addr.Host);
        }
        catch (ArgumentException)
        {
            return false;
        }

        // Общая длина адреса (в ASCII форме) не должна превышать 254
        var fullAscii = $"{localPart}@{hostAscii}";
        if (fullAscii.Length > 254) return false;

        if (!hostAscii.Contains('.')) return false;
        var parts = hostAscii.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) return false;

        var tld = parts[^1];
        // безопасный вызов Regex с таймаутом (защита от DoS через regex)
        try
        {
            if (!Regex.IsMatch(tld, TldPattern, RegexOptions.CultureInvariant, RegexTimeout))
                return false;
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }

        foreach (var label in parts)
        {
            if (label.Length is 0 or > 63) 
                return false;
            if (!char.IsLetterOrDigit(label[0]) || !char.IsLetterOrDigit(label[^1])) 
                return false;
            if (!LabelRegex.IsMatch(label)) 
                return false;
        }

        return true;
    }
}