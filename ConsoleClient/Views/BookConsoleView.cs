using BookFair.Core.DAO;
using BookFair.Core.Models;
using BookFair.Core.Services;

namespace BookFair.ConsoleClient.Views;

public class BookConsoleView
{
    private readonly BookDAO _bookDao;
    private readonly PublisherDAO _publisherDao;
    private readonly PurchaseDAO _purchaseDao;
    private readonly BookAuthorDAO _bookAuthorDao;
    private readonly WishlistItemDAO _wishlistItemDao;
    private readonly LinkService _linkService;

    public BookConsoleView(BookDAO bookDao, PublisherDAO publisherDao, PurchaseDAO purchaseDao,
        BookAuthorDAO bookAuthorDao, WishlistItemDAO wishlistItemDao, LinkService linkService)
    {
        _bookDao = bookDao;
        _publisherDao = publisherDao;
        _purchaseDao = purchaseDao;
        _bookAuthorDao = bookAuthorDao;
        _wishlistItemDao = wishlistItemDao;
        _linkService = linkService;
    }

    public void RunMenu()
    {
        while (true)
        {
            ShowMenu();
            string userInput = Console.ReadLine() ?? "0";
            if (userInput == "0") return;
            HandleMenuInput(userInput);
            // Posle svake izmene liste u modelima moraju da prate podatke
            _linkService.LinkAll();
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== KNJIGE ===");
        Console.WriteLine("1 - Prikazi sve");
        Console.WriteLine("2 - Dodaj");
        Console.WriteLine("3 - Izmeni");
        Console.WriteLine("4 - Obrisi");
        Console.WriteLine("0 - Nazad");
        Console.Write("Izbor: ");
    }

    private void HandleMenuInput(string userInput)
    {
        switch (userInput)
        {
            case "1": ShowAllBooks(); break;
            case "2": AddBook(); break;
            case "3": UpdateBook(); break;
            case "4": RemoveBook(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }

    private void ShowAllBooks()
    {
        List<Book> books = _bookDao.GetAllBooks();
        if (books.Count == 0)
        {
            Console.WriteLine("Nema knjiga.");
            return;
        }

        foreach (Book book in books)
        {
            string publisher = _publisherDao.GetPublisherById(book.PublisherId)?.Name ?? "nepoznat";
            string authors = book.Authors.Count == 0
                ? "nema"
                : string.Join(", ", book.Authors.Select(author => $"{author.FirstName} {author.LastName}"));
            Console.WriteLine($"ID: {book.Id}, ISBN: {book.Isbn}, NASLOV: {book.Title}, ZANR: {book.Genre}, GODINA: {book.PublicationYear}, CENA: {book.Price:F2}, STRANA: {book.PageCount}, IZDAVAC: {publisher}, AUTORI: {authors}");
        }
    }

    private void AddBook()
    {
        if (!HasPublishers())
        {
            return;
        }

        Book book = InputBook();
        try
        {
            _bookDao.AddBook(book);
            Console.WriteLine("Uspesno dodato.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void UpdateBook()
    {
        Console.Write("ID knjige: ");
        int id = ConsoleViewUtils.SafeInputInt();
        if (_bookDao.GetBookById(id) is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        if (!HasPublishers())
        {
            return;
        }

        Book book = InputBook();
        book.Id = id;
        try
        {
            Book? updated = _bookDao.UpdateBook(book);
            Console.WriteLine(updated is null ? "Nije pronadjeno." : "Uspesno izmenjeno.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void RemoveBook()
    {
        Console.Write("ID knjige: ");
        int id = ConsoleViewUtils.SafeInputInt();
        if (_bookDao.GetBookById(id) is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        if (IsBookInUse(id))
        {
            return;
        }

        _bookDao.RemoveBook(id);
        Console.WriteLine("Uspesno obrisano.");
    }

    // Knjiga mora imati izdavaca, pa bez ijednog izdavaca ne moze da se unese
    private bool HasPublishers()
    {
        if (_publisherDao.GetAllPublishers().Count > 0)
        {
            return true;
        }

        Console.WriteLine("Prvo dodaj bar jednog izdavaca.");
        return false;
    }

    // Knjiga se ne brise dok je neko kupio, ima autore ili je na listi zelja,
    // da te veze ne bi pokazivale na knjigu koja ne postoji
    private bool IsBookInUse(int bookId)
    {
        int purchaseCount = _purchaseDao.GetPurchasesByBookId(bookId).Count;
        int authorCount = _bookAuthorDao.GetBookAuthorsByBookId(bookId).Count;
        int wishlistCount = _wishlistItemDao.GetWishlistItemsByBookId(bookId).Count;
        if (purchaseCount == 0 && authorCount == 0 && wishlistCount == 0)
        {
            return false;
        }

        Console.WriteLine($"Knjiga ne moze da se obrise jer je povezana sa drugim podacima (kupovine: {purchaseCount}, autori: {authorCount}, liste zelja: {wishlistCount}).");
        return true;
    }

    private Book InputBook()
    {
        Book book = new Book();
        Console.Write("ISBN: ");
        book.Isbn = ConsoleViewUtils.SafeInputText();
        Console.Write("Naslov: ");
        book.Title = ConsoleViewUtils.SafeInputText();
        Console.Write("Zanr: ");
        book.Genre = ConsoleViewUtils.SafeInputText();
        Console.Write("Godina izdanja: ");
        book.PublicationYear = ConsoleViewUtils.SafeInputInt();
        Console.Write("Cena: ");
        book.Price = ConsoleViewUtils.SafeInputDecimal();
        Console.Write("Broj strana: ");
        book.PageCount = ConsoleViewUtils.SafeInputInt();
        Console.Write("ID izdavaca: ");
        book.PublisherId = ConsoleViewUtils.SafeInputExistingId(id => _publisherDao.GetPublisherById(id) is not null);
        return book;
    }
}
