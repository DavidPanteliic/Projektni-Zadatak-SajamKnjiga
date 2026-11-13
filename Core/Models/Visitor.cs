using System.Globalization;
using System.Text;
using BookFair.Core.Serialization;

namespace BookFair.Core.Models;

public enum MembershipStatus
{
    Regular,
    VIP
}

public class Visitor : ISerializable
{
    private const string DateFormat = "yyyy-MM-dd";

    public int Id { get; set; }
    public string CardNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public int MembershipYear { get; set; }
    public MembershipStatus Status { get; set; }

    // Id adrese se serijalizuje
    public int AddressId { get; set; }

    // Adresa se ne serijalizuje, popunjava se nakon ucitavanja
    public Address? Address { get; set; }

    // Liste se ne serijalizuju, popunjavaju se nakon ucitavanja
    public List<Purchase> Purchases { get; set; } // preko Purchase.VisitorId
    public List<Book> Wishlist { get; set; }      // preko WishlistItem

    // Racuna se iz kupovina, ne serijalizuje se
    public double AverageRating
    {
        get
        {
            if (Purchases.Count == 0)
            {
                return 0;
            }

            return Purchases.Average(purchase => purchase.Rating);
        }
    }

    public Visitor()
    {
        CardNumber = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
        Status = MembershipStatus.Regular;
        Purchases = new List<Purchase>();
        Wishlist = new List<Book>();
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            CardNumber,
            FirstName,
            LastName,
            DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture),
            AddressId.ToString(),
            Phone,
            Email,
            MembershipYear.ToString(),
            Status.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        CardNumber = values[1];
        FirstName = values[2];
        LastName = values[3];
        DateOfBirth = DateOnly.ParseExact(values[4], DateFormat, CultureInfo.InvariantCulture);
        AddressId = int.Parse(values[5]);
        Phone = values[6];
        Email = values[7];
        MembershipYear = int.Parse(values[8]);
        Status = Enum.Parse<MembershipStatus>(values[9]);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"ID: {Id}, ");
        sb.Append($"KARTA: {CardNumber}, ");
        sb.Append($"IME: {FirstName} {LastName}, ");
        sb.Append($"DATUM ROĐENJA: {DateOfBirth:dd.MM.yyyy}, ");
        sb.Append($"ADRESA: {Address}, ");
        sb.Append($"TELEFON: {Phone}, ");
        sb.Append($"EMAIL: {Email}, ");
        sb.Append($"GODINA ČLANSTVA: {MembershipYear}, ");
        sb.Append($"STATUS: {Status}, ");
        sb.Append($"PROSEČNA OCENA: {AverageRating:F2}, ");
        sb.Append("KUPLJENE: ");
        sb.AppendJoin(", ", Purchases.Select(purchase => purchase.Book?.Title));
        sb.Append(", ŽELJE: ");
        sb.AppendJoin(", ", Wishlist.Select(book => book.Title));
        return sb.ToString();
    }
}