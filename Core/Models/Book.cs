using System.Globalization;
using System.Text;
using BookFair.Core.Serialization;

namespace BookFair.Core.Models;

public class Book : ISerializable
{
    public int Id { get; set; }
    public string Isbn { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public int PublicationYear { get; set; }
    public decimal Price { get; set; }
    public int PageCount { get; set; }

    // Id izdavaca se serijalizuje
    public int PublisherId { get; set; }

    // Izdavac se ne serijalizuje, popunjava se nakon ucitavanja
    public Publisher? Publisher { get; set; }

    // Liste se ne serijalizuju, popunjavaju se nakon ucitavanja
    public List<Author> Authors { get; set; }          // preko BookAuthor
    public List<Visitor> Buyers { get; set; }          // preko Purchase
    public List<Visitor> WishlistVisitors { get; set; } // preko WishlistItem

    public Book()
    {
        Isbn = string.Empty;
        Title = string.Empty;
        Genre = string.Empty;
        Authors = new List<Author>();
        Buyers = new List<Visitor>();
        WishlistVisitors = new List<Visitor>();
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            Isbn,
            Title,
            Genre,
            PublicationYear.ToString(),
            Price.ToString(CultureInfo.InvariantCulture),
            PageCount.ToString(),
            PublisherId.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Isbn = values[1];
        Title = values[2];
        Genre = values[3];
        PublicationYear = int.Parse(values[4]);
        Price = decimal.Parse(values[5], CultureInfo.InvariantCulture);
        PageCount = int.Parse(values[6]);
        PublisherId = int.Parse(values[7]);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"ID: {Id}, ");
        sb.Append($"ISBN: {Isbn}, ");
        sb.Append($"NAZIV: {Title}, ");
        sb.Append($"ŽANR: {Genre}, ");
        sb.Append($"GODINA: {PublicationYear}, ");
        sb.Append($"CENA: {Price:F2}, ");
        sb.Append($"STRANA: {PageCount}, ");
        sb.Append($"IZDAVAČ: {Publisher?.Name}, ");
        sb.Append("AUTORI: ");
        sb.AppendJoin(", ", Authors.Select(author => $"{author.FirstName} {author.LastName}"));
        return sb.ToString();
    }
}