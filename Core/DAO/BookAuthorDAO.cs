using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

// Klasa za pristup vezama knjiga - autor.
// Veza nema svoj Id, odredjena je parom BookId i AuthorId.
public class BookAuthorDAO
{
    private readonly List<BookAuthor> _bookAuthors;
    private readonly Storage<BookAuthor> _storage;

    public BookAuthorDAO()
    {
        _storage = new Storage<BookAuthor>("book_authors.txt");
        _bookAuthors = _storage.Load();
    }

    public BookAuthor AddBookAuthor(BookAuthor bookAuthor)
    {
        _bookAuthors.Add(bookAuthor);
        _storage.Save(_bookAuthors);
        return bookAuthor;
    }

    public BookAuthor? RemoveBookAuthor(int bookId, int authorId)
    {
        BookAuthor? bookAuthor = GetBookAuthor(bookId, authorId);
        if (bookAuthor is null) return null;

        _bookAuthors.Remove(bookAuthor);
        _storage.Save(_bookAuthors);
        return bookAuthor;
    }

    public BookAuthor? GetBookAuthor(int bookId, int authorId)
    {
        return _bookAuthors.Find(ba => ba.BookId == bookId && ba.AuthorId == authorId);
    }

    public List<BookAuthor> GetAllBookAuthors()
    {
        return _bookAuthors;
    }

    // Veze jedne knjige (njeni autori)
    public List<BookAuthor> GetBookAuthorsByBookId(int bookId)
    {
        return _bookAuthors.FindAll(ba => ba.BookId == bookId);
    }

    // Veze jednog autora (njegove knjige)
    public List<BookAuthor> GetBookAuthorsByAuthorId(int authorId)
    {
        return _bookAuthors.FindAll(ba => ba.AuthorId == authorId);
    }
}