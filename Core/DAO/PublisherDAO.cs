using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

// Klasa za pristup podacima o izdavacima.
// Cuva listu izdavaca i koristi Storage za ucitavanje i cuvanje u fajl.
public class PublisherDAO
{
    private readonly List<Publisher> _publishers;
    private readonly Storage<Publisher> _storage;

    public PublisherDAO()
    {
        _storage = new Storage<Publisher>("publishers.txt");
        _publishers = _storage.Load();
    }

    private int GenerateId()
    {
        if (_publishers.Count == 0) return 0;
        return _publishers[^1].Id + 1;
    }

    public Publisher AddPublisher(Publisher publisher)
    {
        publisher.Id = GenerateId();
        _publishers.Add(publisher);
        _storage.Save(_publishers);
        return publisher;
    }

    public Publisher? UpdatePublisher(Publisher publisher)
    {
        Publisher? oldPublisher = GetPublisherById(publisher.Id);
        if (oldPublisher is null) return null;

        oldPublisher.Code = publisher.Code;
        oldPublisher.Name = publisher.Name;
        oldPublisher.HeadAuthorId = publisher.HeadAuthorId;

        _storage.Save(_publishers);
        return oldPublisher;
    }

    public Publisher? RemovePublisher(int id)
    {
        Publisher? publisher = GetPublisherById(id);
        if (publisher is null) return null;

        _publishers.Remove(publisher);
        _storage.Save(_publishers);
        return publisher;
    }

    public Publisher? GetPublisherById(int id)
    {
        return _publishers.Find(p => p.Id == id);
    }

    public List<Publisher> GetAllPublishers()
    {
        return _publishers;
    }
}