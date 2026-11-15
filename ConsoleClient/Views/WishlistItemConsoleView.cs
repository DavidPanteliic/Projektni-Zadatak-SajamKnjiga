using BookFair.Core.DAO;
using BookFair.Core.Models;
using BookFair.Core.Services;

namespace BookFair.ConsoleClient.Views;

// Pregled svih lista zelja na jednom mestu.
// Dodavanje i brisanje ide preko LinkService, isto kao iz menija "Lista zelja" kod posetioca,
// pa se promena odmah vidi i u visitor.Wishlist i book.WishlistVisitors.
public class WishlistItemConsoleView
{
    private readonly WishlistItemDAO _wishlistItemDao;
    private readonly VisitorDAO _visitorDao;
    private readonly BookDAO _bookDao;
    private readonly LinkService _linkService;

    public WishlistItemConsoleView(WishlistItemDAO wishlistItemDao, VisitorDAO visitorDao, BookDAO bookDao, LinkService linkService)
    {
        _wishlistItemDao = wishlistItemDao;
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

        // ID je u zagradi jer je potreban za brisanje veze
        foreach (WishlistItem wishlistItem in wishlistItems)
        {
            Visitor? visitor = _visitorDao.GetVisitorById(wishlistItem.VisitorId);
            string visitorName = visitor is null ? "nepoznat" : $"{visitor.FirstName} {visitor.LastName}";
            string bookTitle = _bookDao.GetBookById(wishlistItem.BookId)?.Title ?? "nepoznata";
            Console.WriteLine($"POSETILAC: {visitorName} (ID {wishlistItem.VisitorId}), KNJIGA: {bookTitle} (ID {wishlistItem.BookId})");
        }
    }

    private void AddWishlistItem()
    {
        // Veza povezuje posetioca i knjigu, pa moraju postojati oba
        if (_visitorDao.GetAllVisitors().Count == 0)
        {
            Console.WriteLine("Prvo dodaj bar jednog posetioca.");
            return;
        }

        if (_bookDao.GetAllBooks().Count == 0)
        {
            Console.WriteLine("Prvo dodaj bar jednu knjigu.");
            return;
        }

        Console.Write("ID posetioca: ");
        int visitorId = ConsoleViewUtils.SafeInputExistingId(id => _visitorDao.GetVisitorById(id) is not null);
        Console.Write("ID knjige: ");
        int bookId = ConsoleViewUtils.SafeInputExistingId(id => _bookDao.GetBookById(id) is not null);

        try
        {
            _linkService.AddWishlistItem(visitorId, bookId);
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
        int visitorId = ConsoleViewUtils.SafeInputInt();
        Console.Write("ID knjige: ");
        int bookId = ConsoleViewUtils.SafeInputInt();
        WishlistItem? removed = _linkService.RemoveWishlistItem(visitorId, bookId);
        Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
    }
}
