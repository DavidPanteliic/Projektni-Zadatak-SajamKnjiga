using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class AuthorConsoleView
{
    private readonly AuthorDAO _authorDao;

    public AuthorConsoleView(AuthorDAO authorDao)
    {
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
        Console.WriteLine("=== AUTORI ===");
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
            case "1": ShowAllAuthors(); break;
            case "2": AddAuthor(); break;
            case "3": UpdateAuthor(); break;
            case "4": RemoveAuthor(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }

    private void ShowAllAuthors()
    {
        List<Author> authors = _authorDao.GetAllAuthors();
        if (authors.Count == 0)
        {
            Console.WriteLine("Nema autora.");
            return;
        }

        foreach (Author author in authors)
        {
            Console.WriteLine($"ID: {author.Id}, IME: {author.FirstName} {author.LastName}, DATUM: {author.DateOfBirth:dd.MM.yyyy}, ADRESA ID: {author.AddressId}, TELEFON: {author.Phone}, EMAIL: {author.Email}, LK: {author.IdCardNumber}, ISKUSTVO: {author.YearsOfExperience}");
        }
    }

    private void AddAuthor()
    {
        Author author = InputAuthor();
        try
        {
            _authorDao.AddAuthor(author);
            Console.WriteLine("Uspesno dodato.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void UpdateAuthor()
    {
        Console.Write("ID autora: ");
        int id = ConsoleViewUtils.SafeInputInt();
        if (_authorDao.GetAuthorById(id) is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        Author author = InputAuthor();
        author.Id = id;
        try
        {
            Author? updated = _authorDao.UpdateAuthor(author);
            Console.WriteLine(updated is null ? "Nije pronadjeno." : "Uspesno izmenjeno.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void RemoveAuthor()
    {
        Console.Write("ID autora: ");
        int id = ConsoleViewUtils.SafeInputInt();
        Author? removed = _authorDao.RemoveAuthor(id);
        Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
    }

    private Author InputAuthor()
    {
        Author author = new Author();
        Console.Write("Ime: ");
        author.FirstName = Console.ReadLine() ?? string.Empty;
        Console.Write("Prezime: ");
        author.LastName = Console.ReadLine() ?? string.Empty;
        Console.Write("Datum rodjenja (dd.MM.yyyy): ");
        author.DateOfBirth = ConsoleViewUtils.SafeInputDate();
        Console.Write("ID adrese: ");
        author.AddressId = ConsoleViewUtils.SafeInputInt();
        Console.Write("Telefon: ");
        author.Phone = Console.ReadLine() ?? string.Empty;
        Console.Write("Email: ");
        author.Email = Console.ReadLine() ?? string.Empty;
        Console.Write("Broj licne karte: ");
        author.IdCardNumber = Console.ReadLine() ?? string.Empty;
        Console.Write("Godine iskustva: ");
        author.YearsOfExperience = ConsoleViewUtils.SafeInputInt();
        return author;
    }
}
