using BookFair.Core.Serialization;

namespace BookFair.Core.Models;

// Veza knjiga - autor je many to many.
// Serijalizujemo je pomocu ove klase, po uzoru na StudentSubject sa vezbi.
public class BookAuthor : ISerializable
{
    public int BookId { get; set; }
    public int AuthorId { get; set; }

    public BookAuthor()
    {
    }

    public BookAuthor(int bookId, int authorId)
    {
        BookId = bookId;
        AuthorId = authorId;
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            BookId.ToString(),
            AuthorId.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        BookId = int.Parse(values[0]);
        AuthorId = int.Parse(values[1]);
    }
}