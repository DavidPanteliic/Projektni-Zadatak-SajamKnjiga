using System.Text;
using BookFair.Core.Serialization;

namespace BookFair.Core.Models;

public class Publisher : ISerializable
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }

    // Id sefa se serijalizuje (null ako izdavac jos nema sefa)
    public int? HeadAuthorId { get; set; }

    // Sef se ne serijalizuje, popunjava se nakon ucitavanja
    public Author? HeadAuthor { get; set; }

    // Liste se ne serijalizuju, popunjavaju se nakon ucitavanja
    public List<Book> Books { get; set; }     // preko Book.PublisherId
    public List<Author> Authors { get; set; } // autori knjiga ovog izdavaca

    public Publisher()
    {
        Code = string.Empty;
        Name = string.Empty;
        Books = new List<Book>();
        Authors = new List<Author>();
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            Code,
            Name,
            HeadAuthorId?.ToString() ?? string.Empty
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Code = values[1];
        Name = values[2];
        HeadAuthorId = values[3] == string.Empty ? null : int.Parse(values[3]);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"ID: {Id}, ");
        sb.Append($"ŠIFRA: {Code}, ");
        sb.Append($"NAZIV: {Name}, ");
        sb.Append($"ŠEF: {HeadAuthor?.FirstName} {HeadAuthor?.LastName}, ");
        sb.Append("KNJIGE: ");
        sb.AppendJoin(", ", Books.Select(book => book.Title));
        sb.Append(", AUTORI: ");
        sb.AppendJoin(", ", Authors.Select(author => $"{author.FirstName} {author.LastName}"));
        return sb.ToString();
    }
}