using System.Globalization;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public static class ConsoleViewUtils
{
    public static int SafeInputInt()
    {
        while (true)
        {
            string input = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(input, out int number)) return number;
            Console.Write("Unesi ceo broj: ");
        }
    }

    public static int? SafeInputOptionalInt()
    {
        while (true)
        {
            string input = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (int.TryParse(input, out int number)) return number;
            Console.Write("Unesi ceo broj ili ostavi prazno: ");
        }
    }

    public static decimal SafeInputDecimal()
    {
        while (true)
        {
            string input = (Console.ReadLine() ?? string.Empty).Replace(',', '.');
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number))
                return number;
            Console.Write("Unesi decimalni broj: ");
        }
    }

    public static DateOnly SafeInputDate()
    {
        while (true)
        {
            string input = Console.ReadLine() ?? string.Empty;
            if (DateOnly.TryParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateOnly date)) return date;
            Console.Write("Unesi datum u obliku dd.MM.yyyy: ");
        }
    }

    public static int SafeInputRating()
    {
        while (true)
        {
            int rating = SafeInputInt();
            if (rating >= 1 && rating <= 5) return rating;
            Console.Write("Ocena mora biti od 1 do 5: ");
        }
    }

    public static MembershipStatus SafeInputStatus()
    {
        while (true)
        {
            string input = (Console.ReadLine() ?? string.Empty).Trim();
            if (input.Equals("R", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("Regular", StringComparison.OrdinalIgnoreCase))
                return MembershipStatus.Regular;
            if (input.Equals("V", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("VIP", StringComparison.OrdinalIgnoreCase))
                return MembershipStatus.VIP;
            Console.Write("Unesi R ili V: ");
        }
    }
}
