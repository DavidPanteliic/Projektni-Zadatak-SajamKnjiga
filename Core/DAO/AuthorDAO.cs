using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

// Source: OISISI, Vezbe 3, CRUDExample (DAO pattern)

// Klasa za pristup podacima o autorima.
// Cuva listu autora i koristi Storage za ucitavanje i cuvanje u fajl.
public class AuthorDAO
{
    private readonly List<Author> _authors;
    private readonly Storage<Author> _storage;
    private int _nextId;

    public AuthorDAO()
    {
        _storage = new Storage<Author>("authors.txt");
        _authors = _storage.Load();
        _nextId = _authors.Count == 0 ? 0 : _authors.Max(a => a.Id) + 1;
    }

    private int GenerateId()
    {
        return _nextId++;
    }

    public Author AddAuthor(Author author)
    {
        if (GetAuthorByIdCardNumber(author.IdCardNumber) is not null)
            throw new InvalidOperationException("An author with this ID card number already exists.");

        author.Id = GenerateId();
        _authors.Add(author);
        _storage.Save(_authors);
        return author;
    }

    public Author? UpdateAuthor(Author author)
    {
        Author? oldAuthor = GetAuthorById(author.Id);
        if (oldAuthor is null) return null;
        if (_authors.Any(a => a.Id != author.Id && a.IdCardNumber == author.IdCardNumber))
            throw new InvalidOperationException("An author with this ID card number already exists.");

        oldAuthor.FirstName = author.FirstName;
        oldAuthor.LastName = author.LastName;
        oldAuthor.DateOfBirth = author.DateOfBirth;
        oldAuthor.AddressId = author.AddressId;
        oldAuthor.Phone = author.Phone;
        oldAuthor.Email = author.Email;
        oldAuthor.IdCardNumber = author.IdCardNumber;
        oldAuthor.YearsOfExperience = author.YearsOfExperience;

        _storage.Save(_authors);
        return oldAuthor;
    }

    public Author? RemoveAuthor(int id)
    {
        Author? author = GetAuthorById(id);
        if (author is null) return null;

        _authors.Remove(author);
        _storage.Save(_authors);
        return author;
    }

    public Author? GetAuthorById(int id)
    {
        return _authors.Find(a => a.Id == id);
    }

    public Author? GetAuthorByIdCardNumber(string idCardNumber)
    {
        return _authors.Find(a => a.IdCardNumber == idCardNumber);
    }

    public List<Author> GetAllAuthors()
    {
        return _authors;
    }
}
