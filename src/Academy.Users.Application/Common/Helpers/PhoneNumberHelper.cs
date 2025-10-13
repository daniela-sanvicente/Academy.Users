using System.Linq;

namespace Academy.Users.Application.Common.Helpers;

public static class PhoneNumberHelper
{
    public static bool IsValidMexicanPhoneNumber(string phoneNumber)
    {
        var normalizedDigits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        if (normalizedDigits.Length == 10)
        {
            return true;
        }

        if (normalizedDigits.Length == 12 && normalizedDigits.StartsWith("52"))
        {
            return true;
        }

        return false;
    }

    public static string NormalizePhoneNumber(string phoneNumber)
    {
        var normalizedDigits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        if (normalizedDigits.Length == 10)
        {
            return normalizedDigits;
        }

        if (normalizedDigits.Length == 12 && normalizedDigits.StartsWith("52"))
        {
            return $"+{normalizedDigits}";
        }

        return phoneNumber.Trim();
    }
}
