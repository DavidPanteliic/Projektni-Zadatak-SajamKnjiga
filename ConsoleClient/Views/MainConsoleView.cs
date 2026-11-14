using BookFair.Core.DAO;

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
        _addressView = new AddressConsoleView(new AddressDAO());
        _authorView = new AuthorConsoleView(new AuthorDAO());
        _bookView = new BookConsoleView(new BookDAO());
        _visitorView = new VisitorConsoleView(new VisitorDAO());
        _purchaseView = new PurchaseConsoleView(new PurchaseDAO());
        _publisherView = new PublisherConsoleView(new PublisherDAO());
        _bookAuthorView = new BookAuthorConsoleView(new BookAuthorDAO());
        _wishlistView = new WishlistItemConsoleView(new WishlistItemDAO());
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
