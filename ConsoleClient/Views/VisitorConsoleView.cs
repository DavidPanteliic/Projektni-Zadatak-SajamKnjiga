using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class VisitorConsoleView
{
    private readonly VisitorDAO _visitorDao;
    private readonly AddressDAO _addressDao;
    private readonly PurchaseDAO _purchaseDao;
    private readonly WishlistItemDAO _wishlistItemDao;

    public VisitorConsoleView(VisitorDAO visitorDao, AddressDAO addressDao, PurchaseDAO purchaseDao, WishlistItemDAO wishlistItemDao)
    {
        _visitorDao = visitorDao;
        _addressDao = addressDao;
        _purchaseDao = purchaseDao;
        _wishlistItemDao = wishlistItemDao;
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
        Console.WriteLine("=== POSETIOCI ===");
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
            case "1": ShowAllVisitors(); break;
            case "2": AddVisitor(); break;
            case "3": UpdateVisitor(); break;
            case "4": RemoveVisitor(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
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
