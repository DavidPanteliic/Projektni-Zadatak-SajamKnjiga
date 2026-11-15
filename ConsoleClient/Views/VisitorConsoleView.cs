using BookFair.Core.DAO;
using BookFair.Core.Models;
using BookFair.Core.Services;

namespace BookFair.ConsoleClient.Views;

public class VisitorConsoleView
{
    private readonly VisitorDAO _visitorDao;
    private readonly AddressDAO _addressDao;
    private readonly PurchaseDAO _purchaseDao;
    private readonly WishlistItemDAO _wishlistItemDao;
    private readonly BookDAO _bookDao;
    private readonly LinkService _linkService;

    public VisitorConsoleView(VisitorDAO visitorDao, AddressDAO addressDao, PurchaseDAO purchaseDao,
        WishlistItemDAO wishlistItemDao, BookDAO bookDao, LinkService linkService)
    {
        _visitorDao = visitorDao;
        _addressDao = addressDao;
        _purchaseDao = purchaseDao;
        _wishlistItemDao = wishlistItemDao;
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
        Console.WriteLine("=== POSETIOCI ===");
        Console.WriteLine("1 - Prikazi sve");
        Console.WriteLine("2 - Dodaj");
        Console.WriteLine("3 - Izmeni");
        Console.WriteLine("4 - Obrisi");
        Console.WriteLine("5 - Kupljene knjige");
        Console.WriteLine("6 - Lista zelja");
        Console.WriteLine("0 - Nazad");
        Console.Write("Izbor: ");
    }

    private void HandleMenuInput(string userInput)
    {
        switch (userInput)
        {
            case "1": ShowAllVisitors(); break;
            case "2": AddVisitor(); break;
            case "3": UpdateVisitor(); break;
            case "4": RemoveVisitor(); break;
            case "5": ShowPurchasedBooks(); break;
            case "6": RunWishlistMenu(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }

    // Trazi ID posetioca i vraca posetioca, ili null ako ne postoji
    private Visitor? InputExistingVisitor()
    {
        Console.Write("ID posetioca: ");
        int id = ConsoleViewUtils.SafeInputInt();
        Visitor? visitor = _visitorDao.GetVisitorById(id);
        if (visitor is null)
        {
            Console.WriteLine("Nije pronadjeno.");
        }

        return visitor;
    }

    // Kupljene knjige posetioca, sa prosecnom ocenom i ukupnom potrosnjom
    private void ShowPurchasedBooks()
    {
        Visitor? visitor = InputExistingVisitor();
        if (visitor is null)
        {
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"=== KUPLJENE KNJIGE: {visitor.FirstName} {visitor.LastName} ===");
        if (visitor.Purchases.Count == 0)
        {
            Console.WriteLine("Posetilac nema kupovina.");
            return;
        }

        foreach (Purchase purchase in visitor.Purchases)
        {
            string bookTitle = purchase.Book?.Title ?? "nepoznata";
            decimal price = purchase.Book?.Price ?? 0;
            Console.WriteLine($"ID KUPOVINE: {purchase.Id}, KNJIGA: {bookTitle}, CENA: {price:F2}, DATUM: {purchase.PurchaseDate:dd.MM.yyyy}, OCENA: {purchase.Rating}");
        }

        decimal totalSpent = visitor.Purchases.Sum(purchase => purchase.Book?.Price ?? 0);
        Console.WriteLine($"PROSECNA OCENA: {visitor.AverageRating:F2}");
        Console.WriteLine($"UKUPNA POTROSNJA: {totalSpent:F2}");
    }

    // Lista zelja se vidi i menja unutar posetioca
    private void RunWishlistMenu()
    {
        Visitor? visitor = InputExistingVisitor();
        if (visitor is null)
        {
            return;
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"=== LISTA ZELJA: {visitor.FirstName} {visitor.LastName} ===");
            ShowWishlist(visitor);
            Console.WriteLine("1 - Dodaj knjigu");
            Console.WriteLine("2 - Ukloni knjigu");
            Console.WriteLine("0 - Nazad");
            Console.Write("Izbor: ");
            string userInput = Console.ReadLine() ?? "0";
            if (userInput == "0") return;

            switch (userInput)
            {
                case "1": AddBookToWishlist(visitor); break;
                case "2": RemoveBookFromWishlist(visitor); break;
                default: Console.WriteLine("Nepoznata opcija."); break;
            }
        }
    }

    private void ShowWishlist(Visitor visitor)
    {
        if (visitor.Wishlist.Count == 0)
        {
            Console.WriteLine("Lista zelja je prazna.");
            return;
        }

        foreach (Book book in visitor.Wishlist)
        {
            Console.WriteLine($"ID: {book.Id}, NASLOV: {book.Title}, CENA: {book.Price:F2}");
        }
    }

    private void AddBookToWishlist(Visitor visitor)
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
            _linkService.AddWishlistItem(visitor.Id, bookId);
            Console.WriteLine("Uspesno dodato.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void RemoveBookFromWishlist(Visitor visitor)
    {
        Console.Write("ID knjige: ");
        int bookId = ConsoleViewUtils.SafeInputInt();
        WishlistItem? removed = _linkService.RemoveWishlistItem(visitor.Id, bookId);
        Console.WriteLine(removed is null ? "Ta knjiga nije na listi zelja." : "Uspesno uklonjeno.");
    }

    private void ShowAllVisitors()
    {
        List<Visitor> visitors = _visitorDao.GetAllVisitors();
        if (visitors.Count == 0)
        {
            Console.WriteLine("Nema posetilaca.");
            return;
        }

        foreach (Visitor visitor in visitors)
        {
            string address = ConsoleViewUtils.AddressToText(_addressDao.GetAddressById(visitor.AddressId));
            Console.WriteLine($"ID: {visitor.Id}, KARTA: {visitor.CardNumber}, IME: {visitor.FirstName} {visitor.LastName}, DATUM: {visitor.DateOfBirth:dd.MM.yyyy}, ADRESA: {address}, TELEFON: {visitor.Phone}, EMAIL: {visitor.Email}, GODINA CLANSTVA: {visitor.MembershipYear}, STATUS: {visitor.Status}");
        }
    }

    private void AddVisitor()
    {
        if (!HasAddresses())
        {
            return;
        }

        Visitor visitor = InputVisitor();
        try
        {
            _visitorDao.AddVisitor(visitor);
            Console.WriteLine("Uspesno dodato.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void UpdateVisitor()
    {
        Console.Write("ID posetioca: ");
        int id = ConsoleViewUtils.SafeInputInt();
        if (_visitorDao.GetVisitorById(id) is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        if (!HasAddresses())
        {
            return;
        }

        Visitor visitor = InputVisitor();
        visitor.Id = id;
        try
        {
            Visitor? updated = _visitorDao.UpdateVisitor(visitor);
            Console.WriteLine(updated is null ? "Nije pronadjeno." : "Uspesno izmenjeno.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void RemoveVisitor()
    {
        Console.Write("ID posetioca: ");
        int id = ConsoleViewUtils.SafeInputInt();
        if (_visitorDao.GetVisitorById(id) is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        if (IsVisitorInUse(id))
        {
            return;
        }

        _visitorDao.RemoveVisitor(id);
        Console.WriteLine("Uspesno obrisano.");
    }

    // Posetilac mora imati adresu, pa bez ijedne adrese ne moze da se unese
    private bool HasAddresses()
    {
        if (_addressDao.GetAllAddresses().Count > 0)
        {
            return true;
        }

        Console.WriteLine("Prvo dodaj bar jednu adresu.");
        return false;
    }

    // Posetilac se ne brise dok ima kupovine ili knjige na listi zelja,
    // da te veze ne bi pokazivale na posetioca koji ne postoji
    private bool IsVisitorInUse(int visitorId)
    {
        int purchaseCount = _purchaseDao.GetPurchasesByVisitorId(visitorId).Count;
        int wishlistCount = _wishlistItemDao.GetWishlistItemsByVisitorId(visitorId).Count;
        if (purchaseCount == 0 && wishlistCount == 0)
        {
            return false;
        }

        Console.WriteLine($"Posetilac ne moze da se obrise jer je povezan sa drugim podacima (kupovine: {purchaseCount}, lista zelja: {wishlistCount}).");
        return true;
    }

    private Visitor InputVisitor()
    {
        Visitor visitor = new Visitor();
        Console.Write("Broj clanske karte: ");
        visitor.CardNumber = ConsoleViewUtils.SafeInputText();
        Console.Write("Ime: ");
        visitor.FirstName = ConsoleViewUtils.SafeInputText();
        Console.Write("Prezime: ");
        visitor.LastName = ConsoleViewUtils.SafeInputText();
        Console.Write("Datum rodjenja (dd.MM.yyyy): ");
        visitor.DateOfBirth = ConsoleViewUtils.SafeInputDate();
        Console.Write("ID adrese: ");
        visitor.AddressId = ConsoleViewUtils.SafeInputExistingId(id => _addressDao.GetAddressById(id) is not null);
        Console.Write("Telefon: ");
        visitor.Phone = ConsoleViewUtils.SafeInputText();
        Console.Write("Email: ");
        visitor.Email = ConsoleViewUtils.SafeInputText();
        Console.Write("Godina clanstva: ");
        visitor.MembershipYear = ConsoleViewUtils.SafeInputInt();
        Console.Write("Status (R/V): ");
        visitor.Status = ConsoleViewUtils.SafeInputStatus();
        return visitor;
    }
}
