using BookFair.Core.Models;
using BookFair.Core.Storage;

namespace BookFair.Core.DAO;

// Source: OISISI, Vezbe 3, CRUDExample (DAO pattern)

// Klasa za pristup podacima o posetiocima.
// Cuva listu posetilaca i koristi Storage za ucitavanje i cuvanje u fajl.
public class VisitorDAO
{
    private readonly List<Visitor> _visitors;
    private readonly Storage<Visitor> _storage;
    private int _nextId;

    public VisitorDAO()
    {
        _storage = new Storage<Visitor>("visitors.txt");
        _visitors = _storage.Load();
        _nextId = _visitors.Count == 0 ? 0 : _visitors.Max(v => v.Id) + 1;
    }

    private int GenerateId()
    {
        return _nextId++;
    }

    public Visitor AddVisitor(Visitor visitor)
    {
        if (GetVisitorByCardNumber(visitor.CardNumber) is not null)
            throw new InvalidOperationException("A visitor with this card number already exists.");

        visitor.Id = GenerateId();
        _visitors.Add(visitor);
        _storage.Save(_visitors);
        return visitor;
    }

    public Visitor? UpdateVisitor(Visitor visitor)
    {
        Visitor? oldVisitor = GetVisitorById(visitor.Id);
        if (oldVisitor is null) return null;
        if (_visitors.Any(v => v.Id != visitor.Id && v.CardNumber == visitor.CardNumber))
            throw new InvalidOperationException("A visitor with this card number already exists.");

        oldVisitor.CardNumber = visitor.CardNumber;
        oldVisitor.FirstName = visitor.FirstName;
        oldVisitor.LastName = visitor.LastName;
        oldVisitor.DateOfBirth = visitor.DateOfBirth;
        oldVisitor.AddressId = visitor.AddressId;
        oldVisitor.Phone = visitor.Phone;
        oldVisitor.Email = visitor.Email;
        oldVisitor.MembershipYear = visitor.MembershipYear;
        oldVisitor.Status = visitor.Status;

        _storage.Save(_visitors);
        return oldVisitor;
    }

    public Visitor? RemoveVisitor(int id)
    {
        Visitor? visitor = GetVisitorById(id);
        if (visitor is null) return null;

        _visitors.Remove(visitor);
        _storage.Save(_visitors);
        return visitor;
    }

    public Visitor? GetVisitorById(int id)
    {
        return _visitors.Find(v => v.Id == id);
    }

    public Visitor? GetVisitorByCardNumber(string cardNumber)
    {
        return _visitors.Find(v => v.CardNumber == cardNumber);
    }

    public List<Visitor> GetAllVisitors()
    {
        return _visitors;
    }
}
