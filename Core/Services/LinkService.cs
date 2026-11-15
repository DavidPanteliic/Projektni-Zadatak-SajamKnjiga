using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.Core.Services;

// Povezuje objekte u memoriji (author.Books, book.Authors, visitor.Purchases, ...).
// U fajlovima se cuvaju samo ID-jevi, a ova klasa iz njih puni liste u modelima.
// LinkAll se poziva pri pokretanju i posle svake izmene podataka,
// pa liste uvek odgovaraju onome sto je u fajlovima.
public class LinkService
{
    private readonly AddressDAO _addressDao;
    private readonly AuthorDAO _authorDao;
    private readonly BookDAO _bookDao;
    private readonly VisitorDAO _visitorDao;
    private readonly PurchaseDAO _purchaseDao;
    private readonly PublisherDAO _publisherDao;
    private readonly BookAuthorDAO _bookAuthorDao;
    private readonly WishlistItemDAO _wishlistItemDao;

    public LinkService(AddressDAO addressDao, AuthorDAO authorDao, BookDAO bookDao, VisitorDAO visitorDao,
        PurchaseDAO purchaseDao, PublisherDAO publisherDao, BookAuthorDAO bookAuthorDao, WishlistItemDAO wishlistItemDao)
    {
        _addressDao = addressDao;
        _authorDao = authorDao;
        _bookDao = bookDao;
        _visitorDao = visitorDao;
        _purchaseDao = purchaseDao;
        _publisherDao = publisherDao;
        _bookAuthorDao = bookAuthorDao;
        _wishlistItemDao = wishlistItemDao;
    }

    public void LinkAll()
    {
        ClearLinks();
        LinkAddresses();
        LinkPublishers();
        LinkBookAuthors();
        LinkPurchases();
        LinkWishlist();
        LinkPublisherAuthors();
    }

    // Veza knjiga - autor (koriste je meni "Knjiga - autor" i "Knjige autora")
    public void AddBookAuthor(int bookId, int authorId)
    {
        _bookAuthorDao.AddBookAuthor(new BookAuthor(bookId, authorId));
        LinkAll();
    }

    public BookAuthor? RemoveBookAuthor(int bookId, int authorId)
    {
        BookAuthor? removed = _bookAuthorDao.RemoveBookAuthor(bookId, authorId);
        LinkAll();
        return removed;
    }

    // Lista zelja (koriste je meni "Lista zelja" i "Lista zelja" kod posetioca)
    public void AddWishlistItem(int visitorId, int bookId)
    {
        _wishlistItemDao.AddWishlistItem(new WishlistItem(visitorId, bookId));
        LinkAll();
    }

    public WishlistItem? RemoveWishlistItem(int visitorId, int bookId)
    {
        WishlistItem? removed = _wishlistItemDao.RemoveWishlistItem(visitorId, bookId);
        LinkAll();
        return removed;
    }

    // Prvo se sve isprazni, da u listama ne ostanu obrisani ili stari objekti
    private void ClearLinks()
    {
        foreach (Author author in _authorDao.GetAllAuthors())
        {
            author.Address = null;
            author.Books.Clear();
        }

        foreach (Book book in _bookDao.GetAllBooks())
        {
            book.Publisher = null;
            book.Authors.Clear();
            book.Buyers.Clear();
            book.WishlistVisitors.Clear();
        }

        foreach (Visitor visitor in _visitorDao.GetAllVisitors())
        {
            visitor.Address = null;
            visitor.Purchases.Clear();
            visitor.Wishlist.Clear();
        }

        foreach (Publisher publisher in _publisherDao.GetAllPublishers())
        {
            publisher.HeadAuthor = null;
            publisher.Books.Clear();
            publisher.Authors.Clear();
        }

        foreach (Purchase purchase in _purchaseDao.GetAllPurchases())
        {
            purchase.Visitor = null;
            purchase.Book = null;
        }
    }

    private void LinkAddresses()
    {
        foreach (Author author in _authorDao.GetAllAuthors())
        {
            author.Address = _addressDao.GetAddressById(author.AddressId);
        }

        foreach (Visitor visitor in _visitorDao.GetAllVisitors())
        {
            visitor.Address = _addressDao.GetAddressById(visitor.AddressId);
        }
    }

    private void LinkPublishers()
    {
        foreach (Publisher publisher in _publisherDao.GetAllPublishers())
        {
            if (publisher.HeadAuthorId is not null)
            {
                publisher.HeadAuthor = _authorDao.GetAuthorById(publisher.HeadAuthorId.Value);
            }
        }

        foreach (Book book in _bookDao.GetAllBooks())
        {
            Publisher? publisher = _publisherDao.GetPublisherById(book.PublisherId);
            book.Publisher = publisher;
            publisher?.Books.Add(book);
        }
    }

    private void LinkBookAuthors()
    {
        foreach (BookAuthor bookAuthor in _bookAuthorDao.GetAllBookAuthors())
        {
            Book? book = _bookDao.GetBookById(bookAuthor.BookId);
            Author? author = _authorDao.GetAuthorById(bookAuthor.AuthorId);
            if (book is null || author is null)
            {
                continue;
            }

            book.Authors.Add(author);
            author.Books.Add(book);
        }
    }

    private void LinkPurchases()
    {
        foreach (Purchase purchase in _purchaseDao.GetAllPurchases())
        {
            Visitor? visitor = _visitorDao.GetVisitorById(purchase.VisitorId);
            Book? book = _bookDao.GetBookById(purchase.BookId);
            purchase.Visitor = visitor;
            purchase.Book = book;

            visitor?.Purchases.Add(purchase);
            if (visitor is not null && book is not null && !book.Buyers.Contains(visitor))
            {
                book.Buyers.Add(visitor);
            }
        }
    }

    private void LinkWishlist()
    {
        foreach (WishlistItem wishlistItem in _wishlistItemDao.GetAllWishlistItems())
        {
            Visitor? visitor = _visitorDao.GetVisitorById(wishlistItem.VisitorId);
            Book? book = _bookDao.GetBookById(wishlistItem.BookId);
            if (visitor is null || book is null)
            {
                continue;
            }

            visitor.Wishlist.Add(book);
            book.WishlistVisitors.Add(visitor);
        }
    }

    // Autori izdavaca su autori njegovih knjiga, svaki samo jednom
    private void LinkPublisherAuthors()
    {
        foreach (Publisher publisher in _publisherDao.GetAllPublishers())
        {
            foreach (Book book in publisher.Books)
            {
                foreach (Author author in book.Authors)
                {
                    if (!publisher.Authors.Contains(author))
                    {
                        publisher.Authors.Add(author);
                    }
                }
            }
        }
    }
}
