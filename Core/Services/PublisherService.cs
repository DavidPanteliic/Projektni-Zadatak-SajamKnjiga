using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.Core.Services;

// Poslovna logika za izdavace.
// Sef izdavaca mora biti postojeci autor sa najmanje 5 godina iskustva,
// a izdavac koji ima knjige ne moze da se obrise
public class PublisherService
{
    public const int MinHeadAuthorExperience = 5;

    private readonly PublisherDAO _publisherDao;
    private readonly AuthorDAO _authorDao;
    private readonly BookDAO _bookDao;

    public PublisherService(PublisherDAO publisherDao, AuthorDAO authorDao, BookDAO bookDao)
    {
        _publisherDao = publisherDao;
        _authorDao = authorDao;
        _bookDao = bookDao;
    }

    public Publisher AddPublisher(Publisher publisher)
    {
        CheckHeadAuthor(publisher.HeadAuthorId);
        return _publisherDao.AddPublisher(publisher);
    }

    public Publisher? UpdatePublisher(Publisher publisher)
    {
        CheckHeadAuthor(publisher.HeadAuthorId);
        return _publisherDao.UpdatePublisher(publisher);
    }

    public Publisher? RemovePublisher(int id)
    {
        // Knjige ne bi smele da ostanu sa izdavacem koji ne postoji
        int bookCount = _bookDao.GetBooksByPublisherId(id).Count;
        if (bookCount > 0)
        {
            throw new InvalidOperationException($"Izdavac ne moze da se obrise jer ima knjige ({bookCount}).");
        }

        return _publisherDao.RemovePublisher(id);
    }

    // ispisujemo imena autora umesto samo njegovog ID-a
    public Author? GetHeadAuthor(Publisher publisher)
    {
        if (publisher.HeadAuthorId is null)
        {
            return null;
        }

        return _authorDao.GetAuthorById(publisher.HeadAuthorId.Value);
    }

    public Publisher? GetPublisherById(int id)
    {
        return _publisherDao.GetPublisherById(id);
    }

    public List<Publisher> GetAllPublishers()
    {
        return _publisherDao.GetAllPublishers();
    }

    private void CheckHeadAuthor(int? headAuthorId)
    {
        // Izdavac ne mora da ima sefa
        if (headAuthorId is null)
        {
            return;
        }

        Author? headAuthor = _authorDao.GetAuthorById(headAuthorId.Value);
        if (headAuthor is null)
        {
            throw new InvalidOperationException($"Autor sa ID {headAuthorId} ne postoji.");
        }

        if (headAuthor.YearsOfExperience < MinHeadAuthorExperience)
        {
            throw new InvalidOperationException(
                $"Sef izdavaca mora imati najmanje {MinHeadAuthorExperience} godina iskustva " +
                $"(autor {headAuthor.FirstName} {headAuthor.LastName} ima {headAuthor.YearsOfExperience}).");
        }
    }
}
