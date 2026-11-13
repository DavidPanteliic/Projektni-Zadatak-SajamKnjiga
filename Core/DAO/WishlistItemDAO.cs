using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

// Source: OISISI, Vezbe 3, CRUDExample (DAO pattern)

// Klasa za pristup vezama posetilac - knjiga (lista zelja).
// Veza nema svoj Id, odredjena je parom VisitorId i BookId.
public class WishlistItemDAO
{
    private readonly List<WishlistItem> _wishlistItems;
    private readonly Storage<WishlistItem> _storage;

    public WishlistItemDAO()
    {
        _storage = new Storage<WishlistItem>("wishlist.txt");
        _wishlistItems = _storage.Load();
    }

    public WishlistItem AddWishlistItem(WishlistItem wishlistItem)
    {
        if (GetWishlistItem(wishlistItem.VisitorId, wishlistItem.BookId) is not null)
            throw new InvalidOperationException("This wishlist entry already exists.");

        _wishlistItems.Add(wishlistItem);
        _storage.Save(_wishlistItems);
        return wishlistItem;
    }

    public WishlistItem? RemoveWishlistItem(int visitorId, int bookId)
    {
        WishlistItem? wishlistItem = GetWishlistItem(visitorId, bookId);
        if (wishlistItem is null) return null;

        _wishlistItems.Remove(wishlistItem);
        _storage.Save(_wishlistItems);
        return wishlistItem;
    }

    public WishlistItem? GetWishlistItem(int visitorId, int bookId)
    {
        return _wishlistItems.Find(w => w.VisitorId == visitorId && w.BookId == bookId);
    }

    public List<WishlistItem> GetAllWishlistItems()
    {
        return _wishlistItems;
    }

    // Lista zelja jednog posetioca
    public List<WishlistItem> GetWishlistItemsByVisitorId(int visitorId)
    {
        return _wishlistItems.FindAll(w => w.VisitorId == visitorId);
    }

    // Posetioci koji imaju jednu knjigu na listi zelja
    public List<WishlistItem> GetWishlistItemsByBookId(int bookId)
    {
        return _wishlistItems.FindAll(w => w.BookId == bookId);
    }
}
