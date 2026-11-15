using BookFair.Core.DAO;
using BookFair.Core.Models;
using BookFair.Core.Services;

namespace BookFair.ConsoleClient.Views;

public class PurchaseConsoleView
{
    private readonly PurchaseDAO _purchaseDao;
    private readonly VisitorDAO _visitorDao;
    private readonly BookDAO _bookDao;
    private readonly LinkService _linkService;

    public PurchaseConsoleView(PurchaseDAO purchaseDao, VisitorDAO visitorDao, BookDAO bookDao, LinkService linkService)
    {
        _purchaseDao = purchaseDao;
        _visitorDao = visitorDao;
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
            // Nova ili obrisana kupovina menja visitor.Purchases i prosecnu ocenu
            _linkService.LinkAll();
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== KUPOVINE ===");
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
            case "1": ShowAllPurchases(); break;
            case "2": AddPurchase(); break;
            case "3": UpdatePurchase(); break;
            case "4": RemovePurchase(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }

    private void ShowAllPurchases()
    {
        List<Purchase> purchases = _purchaseDao.GetAllPurchases();
        if (purchases.Count == 0)
        {
            Console.WriteLine("Nema kupovina.");
            return;
        }

        foreach (Purchase purchase in purchases)
        {
            Visitor? visitor = _visitorDao.GetVisitorById(purchase.VisitorId);
            string visitorName = visitor is null ? "nepoznat" : $"{visitor.FirstName} {visitor.LastName}";
            string bookTitle = _bookDao.GetBookById(purchase.BookId)?.Title ?? "nepoznata";
            Console.WriteLine($"ID: {purchase.Id}, POSETILAC: {visitorName}, KNJIGA: {bookTitle}, DATUM: {purchase.PurchaseDate:dd.MM.yyyy}, OCENA: {purchase.Rating}, KOMENTAR: {purchase.Comment}");
        }
    }

    private void AddPurchase()
    {
        if (!HasVisitorsAndBooks())
        {
            return;
        }

        Purchase purchase = InputPurchase();
        _purchaseDao.AddPurchase(purchase);
        Console.WriteLine("Uspesno dodato.");
    }

    private void UpdatePurchase()
    {
        Console.Write("ID kupovine: ");
        int id = ConsoleViewUtils.SafeInputInt();
        if (_purchaseDao.GetPurchaseById(id) is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        if (!HasVisitorsAndBooks())
        {
            return;
        }

        Purchase purchase = InputPurchase();
        purchase.Id = id;
        Purchase? updated = _purchaseDao.UpdatePurchase(purchase);
        Console.WriteLine(updated is null ? "Nije pronadjeno." : "Uspesno izmenjeno.");
    }

    private void RemovePurchase()
    {
        Console.Write("ID kupovine: ");
        int id = ConsoleViewUtils.SafeInputInt();
        Purchase? removed = _purchaseDao.RemovePurchase(id);
        Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
    }

    // Kupovina povezuje posetioca i knjigu, pa moraju postojati oba
    private bool HasVisitorsAndBooks()
    {
        if (_visitorDao.GetAllVisitors().Count == 0)
        {
            Console.WriteLine("Prvo dodaj bar jednog posetioca.");
            return false;
        }

        if (_bookDao.GetAllBooks().Count == 0)
        {
            Console.WriteLine("Prvo dodaj bar jednu knjigu.");
            return false;
        }

        return true;
    }

    private Purchase InputPurchase()
    {
        Purchase purchase = new Purchase();
        Console.Write("ID posetioca: ");
        purchase.VisitorId = ConsoleViewUtils.SafeInputExistingId(id => _visitorDao.GetVisitorById(id) is not null);
        Console.Write("ID knjige: ");
        purchase.BookId = ConsoleViewUtils.SafeInputExistingId(id => _bookDao.GetBookById(id) is not null);
        Console.Write("Datum kupovine (dd.MM.yyyy): ");
        purchase.PurchaseDate = ConsoleViewUtils.SafeInputDate();
        Console.Write("Ocena (1-5): ");
        purchase.Rating = ConsoleViewUtils.SafeInputRating();
        Console.Write("Komentar: ");
        purchase.Comment = ConsoleViewUtils.SafeInputText(isRequired: false);
        return purchase;
    }
}
