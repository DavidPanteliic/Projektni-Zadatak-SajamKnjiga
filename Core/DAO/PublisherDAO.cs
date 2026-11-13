using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

// Source: OISISI, Vezbe 3, CRUDExample (DAO pattern)

// Klasa za pristup podacima o izdavacima.
// Cuva listu izdavaca i koristi Storage za ucitavanje i cuvanje u fajl.
public class PublisherDAO
{
    private readonly List<Publisher> _publishers;
    private readonly Storage<Publisher> _storage;
    private int _nextId;

    public PublisherDAO()
    {
        _storage = new Storage<Publisher>("publishers.txt");
        _publishers = _storage.Load();
        _nextId = _publishers.Count == 0 ? 0 : _publishers.Max(p => p.Id) + 1;
    }

    private int GenerateId()
    {
        return _nextId++;
    }

    public Publisher AddPublisher(Publisher publisher)
    {
        if (_publishers.Any(p => p.Code == publisher.Code))
            throw new InvalidOperationException("A publisher with this code already exists.");

        publisher.Id = GenerateId();
        _publishers.Add(publisher);
        _storage.Save(_publishers);
        return publisher;
    }

    public Publisher? UpdatePublisher(Publisher publisher)
    {
        Publisher? oldPublisher = GetPublisherById(publisher.Id);
        if (oldPublisher is null) return null;
        if (_publishers.Any(p => p.Id != publisher.Id && p.Code == publisher.Code))
            throw new InvalidOperationException("A publisher with this code already exists.");

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
