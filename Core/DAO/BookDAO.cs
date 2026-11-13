using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

// Klasa za pristup podacima o knjigama.
// Cuva listu knjiga i koristi Storage za ucitavanje i cuvanje u fajl.
public class BookDAO
{
    private readonly List<Book> _books;
    private readonly Storage<Book> _storage;

    public BookDAO()
    {
        _storage = new Storage<Book>("books.txt");
        _books = _storage.Load();
    }

    private int GenerateId()
    {
        if (_books.Count == 0) return 0;
        return _books[^1].Id + 1;
    }

    public Book AddBook(Book book)
    {
        book.Id = GenerateId();
        _books.Add(book);
        _storage.Save(_books);
        return book;
    }

    public Book? UpdateBook(Book book)
    {
        Book? oldBook = GetBookById(book.Id);
        if (oldBook is null) return null;

        oldBook.Isbn = book.Isbn;
        oldBook.Title = book.Title;
        oldBook.Genre = book.Genre;
        oldBook.PublicationYear = book.PublicationYear;
        oldBook.Price = book.Price;
        oldBook.PageCount = book.PageCount;
        oldBook.PublisherId = book.PublisherId;

        _storage.Save(_books);
        return oldBook;
    }

    public Book? RemoveBook(int id)
    {
        Book? book = GetBookById(id);
        if (book is null) return null;

        _books.Remove(book);
        _storage.Save(_books);
        return book;
    }

    public Book? GetBookById(int id)
    {
        return _books.Find(b => b.Id == id);
    }

    public Book? GetBookByIsbn(string isbn)
    {
        return _books.Find(b => b.Isbn == isbn);
    }

    public List<Book> GetAllBooks()
    {
        return _books;
    }

    // Knjige jednog izdavaca
    public List<Book> GetBooksByPublisherId(int publisherId)
    {
        return _books.FindAll(b => b.PublisherId == publisherId);
    }
}