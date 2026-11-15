using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class BookAuthorConsoleView
{
    private readonly BookAuthorDAO _bookAuthorDao;
    private readonly BookDAO _bookDao;
    private readonly AuthorDAO _authorDao;

    public BookAuthorConsoleView(BookAuthorDAO bookAuthorDao, BookDAO bookDao, AuthorDAO authorDao)
    {
        _bookAuthorDao = bookAuthorDao;
        _bookDao = bookDao;
        _authorDao = authorDao;
    }

    public void RunMenu()
    {
        while (true)
        {
            ShowMenu();
            string userInput = Console.ReadLine() ?? "0";
            if (userInput == "0") return;
            HandleMenuInput(userInput);
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== KNJIGA - AUTOR ===");
        Console.WriteLine("1 - Prikazi sve veze");
        Console.WriteLine("2 - Dodaj vezu");
        Console.WriteLine("3 - Obrisi vezu");
        Console.WriteLine("0 - Nazad");
        Console.Write("Izbor: ");
    }

    private void HandleMenuInput(string userInput)
    {
        switch (userInput)
        {
            case "1": ShowAllBookAuthors(); break;
            case "2": AddBookAuthor(); break;
            case "3": RemoveBookAuthor(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }

    private void ShowAllBookAuthors()
    {
        List<BookAuthor> bookAuthors = _bookAuthorDao.GetAllBookAuthors();
        if (bookAuthors.Count == 0)
        {
            Console.WriteLine("Nema veza.");
            return;
        }

        // ID je u zagradi jer je potreban za brisanje veze
        foreach (BookAuthor bookAuthor in bookAuthors)
        {
            string bookTitle = _bookDao.GetBookById(bookAuthor.BookId)?.Title ?? "nepoznata";
            Author? author = _authorDao.GetAuthorById(bookAuthor.AuthorId);
            string authorName = author is null ? "nepoznat" : $"{author.FirstName} {author.LastName}";
            Console.WriteLine($"KNJIGA: {bookTitle} (ID {bookAuthor.BookId}), AUTOR: {authorName} (ID {bookAuthor.AuthorId})");
        }
    }

    private void AddBookAuthor()
    {
        // Veza povezuje knjigu i autora, pa moraju postojati oba
        if (_bookDao.GetAllBooks().Count == 0)
        {
            Console.WriteLine("Prvo dodaj bar jednu knjigu.");
            return;
        }

        if (_authorDao.GetAllAuthors().Count == 0)
        {
            Console.WriteLine("Prvo dodaj bar jednog autora.");
            return;
        }

        Console.Write("ID knjige: ");
        int bookId = ConsoleViewUtils.SafeInputExistingId(id => _bookDao.GetBookById(id) is not null);
        Console.Write("ID autora: ");
        int authorId = ConsoleViewUtils.SafeInputExistingId(id => _authorDao.GetAuthorById(id) is not null);
        BookAuthor bookAuthor = new BookAuthor(bookId, authorId);

        try
        {
            _bookAuthorDao.AddBookAuthor(bookAuthor);
            Console.WriteLine("Uspesno dodato.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void RemoveBookAuthor()
    {
        Console.Write("ID knjige: ");
        int bookId = ConsoleViewUtils.SafeInputInt();
        Console.Write("ID autora: ");
        int authorId = ConsoleViewUtils.SafeInputInt();
        BookAuthor? removed = _bookAuthorDao.RemoveBookAuthor(bookId, authorId);
        Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
    }
}
