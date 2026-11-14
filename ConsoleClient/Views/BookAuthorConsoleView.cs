using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class BookAuthorConsoleView
{
    private readonly BookAuthorDAO _bookAuthorDao;

    public BookAuthorConsoleView(BookAuthorDAO bookAuthorDao)
    {
        _bookAuthorDao = bookAuthorDao;
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

        foreach (BookAuthor bookAuthor in bookAuthors)
        {
            Console.WriteLine($"Veza: BookId = {bookAuthor.BookId}, AuthorId = {bookAuthor.AuthorId}");
        }
    }

    private void AddBookAuthor()
    {
        Console.Write("ID knjige: ");
        int firstId = ConsoleViewUtils.SafeInputInt();
        Console.Write("ID autora: ");
        int secondId = ConsoleViewUtils.SafeInputInt();
        BookAuthor bookAuthor = new BookAuthor(firstId, secondId);

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
        int firstId = ConsoleViewUtils.SafeInputInt();
        Console.Write("ID autora: ");
        int secondId = ConsoleViewUtils.SafeInputInt();
        BookAuthor? removed = _bookAuthorDao.RemoveBookAuthor(firstId, secondId);
        Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
    }
}
