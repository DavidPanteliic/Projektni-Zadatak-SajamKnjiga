using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

// Klasa za pristup podacima o kupovinama.
// Cuva listu kupovina i koristi Storage za ucitavanje i cuvanje u fajl.
public class PurchaseDAO
{
    private readonly List<Purchase> _purchases;
    private readonly Storage<Purchase> _storage;

    public PurchaseDAO()
    {
        _storage = new Storage<Purchase>("purchases.txt");
        _purchases = _storage.Load();
    }

    private int GenerateId()
    {
        if (_purchases.Count == 0) return 0;
        return _purchases[^1].Id + 1;
    }

    public Purchase AddPurchase(Purchase purchase)
    {
        purchase.Id = GenerateId();
        _purchases.Add(purchase);
        _storage.Save(_purchases);
        return purchase;
    }

    public Purchase? UpdatePurchase(Purchase purchase)
    {
        Purchase? oldPurchase = GetPurchaseById(purchase.Id);
        if (oldPurchase is null) return null;

        oldPurchase.VisitorId = purchase.VisitorId;
        oldPurchase.BookId = purchase.BookId;
        oldPurchase.PurchaseDate = purchase.PurchaseDate;
        oldPurchase.Rating = purchase.Rating;
        oldPurchase.Comment = purchase.Comment;

        _storage.Save(_purchases);
        return oldPurchase;
    }

    public Purchase? RemovePurchase(int id)
    {
        Purchase? purchase = GetPurchaseById(id);
        if (purchase is null) return null;

        _purchases.Remove(purchase);
        _storage.Save(_purchases);
        return purchase;
    }

    public Purchase? GetPurchaseById(int id)
    {
        return _purchases.Find(p => p.Id == id);
    }

    public List<Purchase> GetAllPurchases()
    {
        return _purchases;
    }

    // Kupovine jednog posetioca
    public List<Purchase> GetPurchasesByVisitorId(int visitorId)
    {
        return _purchases.FindAll(p => p.VisitorId == visitorId);
    }

    // Kupovine jedne knjige
    public List<Purchase> GetPurchasesByBookId(int bookId)
    {
        return _purchases.FindAll(p => p.BookId == bookId);
    }
}