using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class BookConsoleView
{
    private readonly BookDAO _bookDao;

    public BookConsoleView(BookDAO bookDao)
    {
        _bookDao = bookDao;
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
            Console.WriteLine($"ID: {book.Id}, ISBN: {book.Isbn}, NASLOV: {book.Title}, ZANR: {book.Genre}, GODINA: {book.PublicationYear}, CENA: {book.Price:F2}, STRANA: {book.PageCount}, IZDAVAC ID: {book.PublisherId}");
        }
    }

    private void AddBook()
    {
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
        Book? removed = _bookDao.RemoveBook(id);
        Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
    }

    private Book InputBook()
    {
        Book book = new Book();
        Console.Write("ISBN: ");
        book.Isbn = Console.ReadLine() ?? string.Empty;
        Console.Write("Naslov: ");
        book.Title = Console.ReadLine() ?? string.Empty;
        Console.Write("Zanr: ");
        book.Genre = Console.ReadLine() ?? string.Empty;
        Console.Write("Godina izdanja: ");
        book.PublicationYear = ConsoleViewUtils.SafeInputInt();
        Console.Write("Cena: ");
        book.Price = ConsoleViewUtils.SafeInputDecimal();
        Console.Write("Broj strana: ");
        book.PageCount = ConsoleViewUtils.SafeInputInt();
        Console.Write("ID izdavaca: ");
        book.PublisherId = ConsoleViewUtils.SafeInputInt();
        return book;
    }
}
