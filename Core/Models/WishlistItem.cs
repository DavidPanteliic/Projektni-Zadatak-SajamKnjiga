using BookFair.Core.Serialization;

namespace BookFair.Core.Models;

// Veza posetilac - knjiga (lista zelja) je many to many.
// Serijalizujemo je pomocu ove klase, po uzoru na StudentSubject sa vezbi.
public class WishlistItem : ISerializable
{
    public int VisitorId { get; set; }
    public int BookId { get; set; }

    public WishlistItem()
    {
    }

    public WishlistItem(int visitorId, int bookId)
    {
        VisitorId = visitorId;
        BookId = bookId;
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            VisitorId.ToString(),
            BookId.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        VisitorId = int.Parse(values[0]);
        BookId = int.Parse(values[1]);
    }
}