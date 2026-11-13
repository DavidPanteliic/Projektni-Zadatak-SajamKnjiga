using System.Globalization;
using System.Text;
using BookFair.Core.Serialization;

namespace BookFair.Core.Models;

public class Author : ISerializable
{
    private const string DateFormat = "yyyy-MM-dd";

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string IdCardNumber { get; set; }
    public int YearsOfExperience { get; set; }

    // Id adrese se serijalizuje
    public int AddressId { get; set; }

    // Adresa se ne serijalizuje, popunjava se nakon ucitavanja
    public Address? Address { get; set; }

    // Knjige se ne serijalizuju, popunjavaju se nakon ucitavanja (preko BookAuthor)
    public List<Book> Books { get; set; }

    public Author()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
        IdCardNumber = string.Empty;
        Books = new List<Book>();
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            FirstName,
            LastName,
            DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture),
            AddressId.ToString(),
            Phone,
            Email,
            IdCardNumber,
            YearsOfExperience.ToString()
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        FirstName = values[1];
        LastName = values[2];
        DateOfBirth = DateOnly.ParseExact(values[3], DateFormat, CultureInfo.InvariantCulture);
        AddressId = int.Parse(values[4]);
        Phone = values[5];
        Email = values[6];
        IdCardNumber = values[7];
        YearsOfExperience = int.Parse(values[8]);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"ID: {Id}, ");
        sb.Append($"IME: {FirstName} {LastName}, ");
        sb.Append($"DATUM ROĐENJA: {DateOfBirth:dd.MM.yyyy}, ");
        sb.Append($"ADRESA: {Address}, ");
        sb.Append($"TELEFON: {Phone}, ");
        sb.Append($"EMAIL: {Email}, ");
        sb.Append($"LK: {IdCardNumber}, ");
        sb.Append($"ISKUSTVO: {YearsOfExperience} god., ");
        sb.Append("KNJIGE: ");
        sb.AppendJoin(", ", Books.Select(book => book.Title));
        return sb.ToString();
    }
}