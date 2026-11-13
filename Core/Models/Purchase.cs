using System.Globalization;
using BookFair.Core.Serialization;

namespace BookFair.Core.Models;

public class Purchase : ISerializable
{
    private const string DateFormat = "yyyy-MM-dd";

    public int Id { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }

    // Id posetioca i Id knjige se serijalizuju
    public int VisitorId { get; set; }
    public int BookId { get; set; }

    // Posetilac i knjiga se ne serijalizuju, popunjavaju se nakon ucitavanja
    public Visitor? Visitor { get; set; }
    public Book? Book { get; set; }

    public Purchase()
    {
        Comment = string.Empty;
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            VisitorId.ToString(),
            BookId.ToString(),
            PurchaseDate.ToString(DateFormat, CultureInfo.InvariantCulture),
            Rating.ToString(),
            Comment
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        VisitorId = int.Parse(values[1]);
        BookId = int.Parse(values[2]);
        PurchaseDate = DateOnly.ParseExact(values[3], DateFormat, CultureInfo.InvariantCulture);
        Rating = int.Parse(values[4]);
        Comment = values[5];
    }

    public override string ToString()
    {
        return $"ID: {Id}, " +
               $"POSETILAC: {Visitor?.FirstName} {Visitor?.LastName}, " +
               $"KNJIGA: {Book?.Title}, " +
               $"DATUM: {PurchaseDate:dd.MM.yyyy}, " +
               $"OCENA: {Rating}, " +
               $"KOMENTAR: {Comment}";
    }
}