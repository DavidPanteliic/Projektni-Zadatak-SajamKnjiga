using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class PurchaseConsoleView
{
    private readonly PurchaseDAO _purchaseDao;

    public PurchaseConsoleView(PurchaseDAO purchaseDao)
    {
        _purchaseDao = purchaseDao;
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
            Console.WriteLine($"ID: {purchase.Id}, POSETILAC ID: {purchase.VisitorId}, KNJIGA ID: {purchase.BookId}, DATUM: {purchase.PurchaseDate:dd.MM.yyyy}, OCENA: {purchase.Rating}, KOMENTAR: {purchase.Comment}");
        }
    }

    private void AddPurchase()
    {
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

    private Purchase InputPurchase()
    {
        Purchase purchase = new Purchase();
        Console.Write("ID posetioca: ");
        purchase.VisitorId = ConsoleViewUtils.SafeInputInt();
        Console.Write("ID knjige: ");
        purchase.BookId = ConsoleViewUtils.SafeInputInt();
        Console.Write("Datum kupovine (dd.MM.yyyy): ");
        purchase.PurchaseDate = ConsoleViewUtils.SafeInputDate();
        Console.Write("Ocena (1-5): ");
        purchase.Rating = ConsoleViewUtils.SafeInputRating();
        Console.Write("Komentar: ");
        purchase.Comment = Console.ReadLine() ?? string.Empty;
        return purchase;
    }
}
