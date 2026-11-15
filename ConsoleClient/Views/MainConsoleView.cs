using BookFair.Core.DAO;
using BookFair.Core.Services;

namespace BookFair.ConsoleClient.Views;

public class MainConsoleView
{
    private readonly AddressConsoleView _addressView;
    private readonly AuthorConsoleView _authorView;
    private readonly BookConsoleView _bookView;
    private readonly VisitorConsoleView _visitorView;
    private readonly PurchaseConsoleView _purchaseView;
    private readonly PublisherConsoleView _publisherView;
    private readonly BookAuthorConsoleView _bookAuthorView;
    private readonly WishlistItemConsoleView _wishlistView;

    public MainConsoleView()
    {
        // Svaki DAO se pravi samo jednom i deli izmedju prikaza,
        // da bi svi prikazi videli iste podatke (npr. autora dodatog u ovoj sesiji)
        AddressDAO addressDao = new AddressDAO();
        AuthorDAO authorDao = new AuthorDAO();
        BookDAO bookDao = new BookDAO();
        VisitorDAO visitorDao = new VisitorDAO();
        PurchaseDAO purchaseDao = new PurchaseDAO();
        PublisherDAO publisherDao = new PublisherDAO();
        BookAuthorDAO bookAuthorDao = new BookAuthorDAO();
        WishlistItemDAO wishlistItemDao = new WishlistItemDAO();

        _addressView = new AddressConsoleView(addressDao, visitorDao, authorDao);
        _authorView = new AuthorConsoleView(authorDao, addressDao, bookAuthorDao, publisherDao);
        _bookView = new BookConsoleView(bookDao, publisherDao, purchaseDao, bookAuthorDao, wishlistItemDao);
        _visitorView = new VisitorConsoleView(visitorDao, addressDao, purchaseDao, wishlistItemDao);
        _purchaseView = new PurchaseConsoleView(purchaseDao, visitorDao, bookDao);
        _publisherView = new PublisherConsoleView(new PublisherService(publisherDao, authorDao, bookDao));
        _bookAuthorView = new BookAuthorConsoleView(bookAuthorDao, bookDao, authorDao);
        _wishlistView = new WishlistItemConsoleView(wishlistItemDao, visitorDao, bookDao);
    }

    public void RunMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== SAJAM KNJIGA ===");
            Console.WriteLine("1 - Adrese");
            Console.WriteLine("2 - Autori");
            Console.WriteLine("3 - Knjige");
            Console.WriteLine("4 - Posetioci");
            Console.WriteLine("5 - Kupovine");
            Console.WriteLine("6 - Izdavaci");
            Console.WriteLine("7 - Knjiga - autor");
            Console.WriteLine("8 - Lista zelja");
            Console.WriteLine("0 - Izlaz");
            Console.Write("Izbor: ");
            string input = Console.ReadLine() ?? "0";
            if (input == "0") return;
            HandleMenuInput(input);
        }
    }

    private void HandleMenuInput(string input)
    {
        switch (input)
        {
            case "1": _addressView.RunMenu(); break;
            case "2": _authorView.RunMenu(); break;
            case "3": _bookView.RunMenu(); break;
            case "4": _visitorView.RunMenu(); break;
            case "5": _purchaseView.RunMenu(); break;
            case "6": _publisherView.RunMenu(); break;
            case "7": _bookAuthorView.RunMenu(); break;
            case "8": _wishlistView.RunMenu(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }
}
