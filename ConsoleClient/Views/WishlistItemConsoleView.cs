using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class WishlistItemConsoleView
{
    private readonly WishlistItemDAO _wishlistItemDao;

    public WishlistItemConsoleView(WishlistItemDAO wishlistItemDao)
    {
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
        Console.WriteLine("=== LISTA ZELJA ===");
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
            case "1": ShowAllWishlistItems(); break;
            case "2": AddWishlistItem(); break;
            case "3": RemoveWishlistItem(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }

    private void ShowAllWishlistItems()
    {
        List<WishlistItem> wishlistItems = _wishlistItemDao.GetAllWishlistItems();
        if (wishlistItems.Count == 0)
        {
            Console.WriteLine("Nema veza.");
            return;
        }

        foreach (WishlistItem wishlistItem in wishlistItems)
        {
            Console.WriteLine($"Veza: VisitorId = {wishlistItem.VisitorId}, BookId = {wishlistItem.BookId}");
        }
    }

    private void AddWishlistItem()
    {
        Console.Write("ID posetioca: ");
        int firstId = ConsoleViewUtils.SafeInputInt();
        Console.Write("ID knjige: ");
        int secondId = ConsoleViewUtils.SafeInputInt();
        WishlistItem wishlistItem = new WishlistItem(firstId, secondId);

        try
        {
            _wishlistItemDao.AddWishlistItem(wishlistItem);
            Console.WriteLine("Uspesno dodato.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void RemoveWishlistItem()
    {
        Console.Write("ID posetioca: ");
        int firstId = ConsoleViewUtils.SafeInputInt();
        Console.Write("ID knjige: ");
        int secondId = ConsoleViewUtils.SafeInputInt();
        WishlistItem? removed = _wishlistItemDao.RemoveWishlistItem(firstId, secondId);
        Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
    }
}
