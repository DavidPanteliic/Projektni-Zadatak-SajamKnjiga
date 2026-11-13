using BookFair.Core.Serialization;

namespace BookFair.Core.Models;

public class Address : ISerializable
{
    public int Id { get; set; }
    public string Street { get; set; }
    public string Number { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public Address()
    {
        Street = string.Empty;
        Number = string.Empty;
        City = string.Empty;
        Country = string.Empty;
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            Id.ToString(),
            Street,
            Number,
            City,
            Country
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Street = values[1];
        Number = values[2];
        City = values[3];
        Country = values[4];
    }

    public override string ToString()
    {
        return $"{Street} {Number}, {City}, {Country}";
    }
}