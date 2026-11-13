using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

// Klasa za pristup podacima o autorima.
// Cuva listu autora i koristi Storage za ucitavanje i cuvanje u fajl.
public class AuthorDAO
{
    private readonly List<Author> _authors;
    private readonly Storage<Author> _storage;

    public AuthorDAO()
    {
        _storage = new Storage<Author>("authors.txt");
        _authors = _storage.Load();
    }

    private int GenerateId()
    {
        if (_authors.Count == 0) return 0;
        return _authors[^1].Id + 1;
    }

    public Author AddAuthor(Author author)
    {
        author.Id = GenerateId();
        _authors.Add(author);
        _storage.Save(_authors);
        return author;
    }

    public Author? UpdateAuthor(Author author)
    {
        Author? oldAuthor = GetAuthorById(author.Id);
        if (oldAuthor is null) return null;

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