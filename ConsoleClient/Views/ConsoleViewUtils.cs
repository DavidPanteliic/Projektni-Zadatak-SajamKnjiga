using System.Globalization;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public static class ConsoleViewUtils
{
    private const char ForbiddenCharacter = '|';

    // Tekst ne sme da sadrzi '|' jer je to separator u fajlovima.
    // Obavezno polje ne sme biti prazno.
    public static string SafeInputText(bool isRequired = true)
    {
        while (true)
        {
            string? line = Console.ReadLine();
            if (line is null)
            {
                return string.Empty;
            }

            string input = line.Trim();
            if (input.Contains(ForbiddenCharacter))
            {
                Console.Write($"Tekst ne sme da sadrzi znak '{ForbiddenCharacter}'. Unesi ponovo: ");
                continue;
            }

            if (isRequired && input.Length == 0)
            {
                Console.Write("Polje je obavezno. Unesi ponovo: ");
                continue;
            }

            return input;
        }
    }

    public static int SafeInputInt()
    {
        while (true)
        {
            string input = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(input, out int number)) return number;
            Console.Write("Unesi ceo broj: ");
        }
    }

    // Trazi ID dok se ne unese ID zapisa koji postoji.
    // exists proverava da li zapis sa datim ID postoji (npr. id => dao.GetBookById(id) is not null).
    public static int SafeInputExistingId(Func<int, bool> exists)
    {
        while (true)
        {
            int id = SafeInputInt();
            if (exists(id))
            {
                return id;
            }

            Console.Write("Ne postoji zapis sa tim ID. Unesi ponovo: ");
        }
    }

    // Adresa za prikaz umesto ID-ja (koriste je posetioci i autori)
    public static string AddressToText(Address? address)
    {
        if (address is null)
        {
            return "nepoznata";
        }

        return $"{address.Street} {address.Number}, {address.City}";
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
