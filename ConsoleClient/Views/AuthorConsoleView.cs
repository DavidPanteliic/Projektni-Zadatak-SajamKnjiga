using BookFair.Core.DAO;
using BookFair.Core.Models;
using BookFair.Core.Services;

namespace BookFair.ConsoleClient.Views;

public class AuthorConsoleView
{
    private readonly AuthorDAO _authorDao;
    private readonly AddressDAO _addressDao;
    private readonly BookAuthorDAO _bookAuthorDao;
    private readonly PublisherDAO _publisherDao;
    private readonly BookDAO _bookDao;
    private readonly LinkService _linkService;

    public AuthorConsoleView(AuthorDAO authorDao, AddressDAO addressDao, BookAuthorDAO bookAuthorDao,
        PublisherDAO publisherDao, BookDAO bookDao, LinkService linkService)
    {
        _authorDao = authorDao;
        _addressDao = addressDao;
        _bookAuthorDao = bookAuthorDao;
        _publisherDao = publisherDao;
        _bookDao = bookDao;
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
        Console.WriteLine("=== AUTORI ===");
        Console.WriteLine("1 - Prikazi sve");
        Console.WriteLine("2 - Dodaj");
        Console.WriteLine("3 - Izmeni");
        Console.WriteLine("4 - Obrisi");
        Console.WriteLine("5 - Knjige autora");
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
            case "5": RunAuthorBooksMenu(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }

    // Knjige autora se vide i menjaju unutar autora (spisak knjiga je atribut autora)
    private void RunAuthorBooksMenu()
    {
        Console.Write("ID autora: ");
        int id = ConsoleViewUtils.SafeInputInt();
        Author? author = _authorDao.GetAuthorById(id);
        if (author is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"=== KNJIGE AUTORA: {author.FirstName} {author.LastName} ===");
            ShowAuthorBooks(author);
            Console.WriteLine("1 - Dodaj knjigu");
            Console.WriteLine("2 - Ukloni knjigu");
            Console.WriteLine("0 - Nazad");
            Console.Write("Izbor: ");
            string userInput = Console.ReadLine() ?? "0";
            if (userInput == "0") return;

            switch (userInput)
            {
                case "1": AddBookToAuthor(author); break;
                case "2": RemoveBookFromAuthor(author); break;
                default: Console.WriteLine("Nepoznata opcija."); break;
            }
        }
    }

    private void ShowAuthorBooks(Author author)
    {
        if (author.Books.Count == 0)
        {
            Console.WriteLine("Autor nema knjiga.");
            return;
        }

        foreach (Book book in author.Books)
        {
            Console.WriteLine($"ID: {book.Id}, ISBN: {book.Isbn}, NASLOV: {book.Title}, GODINA: {book.PublicationYear}");
        }
    }

    private void AddBookToAuthor(Author author)
    {
        if (_bookDao.GetAllBooks().Count == 0)
        {
            Console.WriteLine("Prvo dodaj bar jednu knjigu.");
            return;
        }

        Console.Write("ID knjige: ");
        int bookId = ConsoleViewUtils.SafeInputExistingId(id => _bookDao.GetBookById(id) is not null);
        try
        {
            _linkService.AddBookAuthor(bookId, author.Id);
            Console.WriteLine("Uspesno dodato.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void RemoveBookFromAuthor(Author author)
    {
        Console.Write("ID knjige: ");
        int bookId = ConsoleViewUtils.SafeInputInt();
        BookAuthor? removed = _linkService.RemoveBookAuthor(bookId, author.Id);
        Console.WriteLine(removed is null ? "Autor nema tu knjigu." : "Uspesno uklonjeno.");
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
            string address = ConsoleViewUtils.AddressToText(_addressDao.GetAddressById(author.AddressId));
            Console.WriteLine($"ID: {author.Id}, IME: {author.FirstName} {author.LastName}, DATUM: {author.DateOfBirth:dd.MM.yyyy}, ADRESA: {address}, TELEFON: {author.Phone}, EMAIL: {author.Email}, LK: {author.IdCardNumber}, ISKUSTVO: {author.YearsOfExperience}");
        }
    }

    private void AddAuthor()
    {
        if (!HasAddresses())
        {
            return;
        }

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

        if (!HasAddresses())
        {
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
        if (_authorDao.GetAuthorById(id) is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        if (IsAuthorInUse(id))
        {
            return;
        }

        _authorDao.RemoveAuthor(id);
        Console.WriteLine("Uspesno obrisano.");
    }

    // Autor mora imati adresu, pa bez ijedne adrese ne moze da se unese
    private bool HasAddresses()
    {
        if (_addressDao.GetAllAddresses().Count > 0)
        {
            return true;
        }

        Console.WriteLine("Prvo dodaj bar jednu adresu.");
        return false;
    }

    // Autor se ne brise dok ima knjige ili je sef nekog izdavaca,
    // da te veze ne bi pokazivale na autora koji ne postoji
    private bool IsAuthorInUse(int authorId)
    {
        int bookCount = _bookAuthorDao.GetBookAuthorsByAuthorId(authorId).Count;
        int publisherCount = _publisherDao.GetAllPublishers().FindAll(p => p.HeadAuthorId == authorId).Count;
        if (bookCount == 0 && publisherCount == 0)
        {
            return false;
        }

        Console.WriteLine($"Autor ne moze da se obrise jer je povezan sa drugim podacima (knjige: {bookCount}, sef izdavaca: {publisherCount}).");
        return true;
    }

    private Author InputAuthor()
    {
        Author author = new Author();
        Console.Write("Ime: ");
        author.FirstName = ConsoleViewUtils.SafeInputText();
        Console.Write("Prezime: ");
        author.LastName = ConsoleViewUtils.SafeInputText();
        Console.Write("Datum rodjenja (dd.MM.yyyy): ");
        author.DateOfBirth = ConsoleViewUtils.SafeInputDate();
        Console.Write("ID adrese: ");
        author.AddressId = ConsoleViewUtils.SafeInputExistingId(id => _addressDao.GetAddressById(id) is not null);
        Console.Write("Telefon: ");
        author.Phone = ConsoleViewUtils.SafeInputText();
        Console.Write("Email: ");
        author.Email = ConsoleViewUtils.SafeInputText();
        Console.Write("Broj licne karte: ");
        author.IdCardNumber = ConsoleViewUtils.SafeInputText();
        Console.Write("Godine iskustva: ");
        author.YearsOfExperience = ConsoleViewUtils.SafeInputInt();
        return author;
    }
}
