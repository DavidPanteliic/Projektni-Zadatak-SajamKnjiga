using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class VisitorConsoleView
{
    private readonly VisitorDAO _visitorDao;

    public VisitorConsoleView(VisitorDAO visitorDao)
    {
        _visitorDao = visitorDao;
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
            Console.WriteLine($"ID: {visitor.Id}, KARTA: {visitor.CardNumber}, IME: {visitor.FirstName} {visitor.LastName}, DATUM: {visitor.DateOfBirth:dd.MM.yyyy}, ADRESA ID: {visitor.AddressId}, TELEFON: {visitor.Phone}, EMAIL: {visitor.Email}, GODINA CLANSTVA: {visitor.MembershipYear}, STATUS: {visitor.Status}");
        }
    }

    private void AddVisitor()
    {
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
        Visitor? removed = _visitorDao.RemoveVisitor(id);
        Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
    }

    private Visitor InputVisitor()
    {
        Visitor visitor = new Visitor();
        Console.Write("Broj clanske karte: ");
        visitor.CardNumber = Console.ReadLine() ?? string.Empty;
        Console.Write("Ime: ");
        visitor.FirstName = Console.ReadLine() ?? string.Empty;
        Console.Write("Prezime: ");
        visitor.LastName = Console.ReadLine() ?? string.Empty;
        Console.Write("Datum rodjenja (dd.MM.yyyy): ");
        visitor.DateOfBirth = ConsoleViewUtils.SafeInputDate();
        Console.Write("ID adrese: ");
        visitor.AddressId = ConsoleViewUtils.SafeInputInt();
        Console.Write("Telefon: ");
        visitor.Phone = Console.ReadLine() ?? string.Empty;
        Console.Write("Email: ");
        visitor.Email = Console.ReadLine() ?? string.Empty;
        Console.Write("Godina clanstva: ");
        visitor.MembershipYear = ConsoleViewUtils.SafeInputInt();
        Console.Write("Status (R/V): ");
        visitor.Status = ConsoleViewUtils.SafeInputStatus();
        return visitor;
    }
}
