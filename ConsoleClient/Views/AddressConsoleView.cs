using BookFair.Core.DAO;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class AddressConsoleView
{
    private readonly AddressDAO _addressDao;

    public AddressConsoleView(AddressDAO addressDao)
    {
        _addressDao = addressDao;
    }

    public void RunMenu()
    {
        while (true)
        {
            ShowMenu();
            string userInput = Console.ReadLine() ?? "0";
            if (userInput == "0") return;
            HandleMenuInput(userInput);
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== ADRESE ===");
        Console.WriteLine("1 - Prikazi sve");
        Console.WriteLine("2 - Dodaj");
        Console.WriteLine("3 - Izmeni");
        Console.WriteLine("4 - Obrisi");
        Console.WriteLine("0 - Nazad");
        Console.Write("Izbor: ");
    }

    private void HandleMenuInput(string userInput)
    {
        switch (userInput)
        {
            case "1": ShowAllAddresses(); break;
            case "2": AddAddress(); break;
            case "3": UpdateAddress(); break;
            case "4": RemoveAddress(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }

    private void ShowAllAddresses()
    {
        List<Address> addresses = _addressDao.GetAllAddresses();
        if (addresses.Count == 0)
        {
            Console.WriteLine("Nema adresa.");
            return;
        }

        foreach (Address address in addresses)
        {
            Console.WriteLine($"ID: {address.Id}, ULICA: {address.Street}, BROJ: {address.Number}, GRAD: {address.City}, DRZAVA: {address.Country}");
        }
    }

    private void AddAddress()
    {
        Address address = InputAddress();
        _addressDao.AddAddress(address);
        Console.WriteLine("Uspesno dodato.");
    }

    private void UpdateAddress()
    {
        Console.Write("ID adrese: ");
        int id = ConsoleViewUtils.SafeInputInt();
        if (_addressDao.GetAddressById(id) is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        Address address = InputAddress();
        address.Id = id;
        Address? updated = _addressDao.UpdateAddress(address);
        Console.WriteLine(updated is null ? "Nije pronadjeno." : "Uspesno izmenjeno.");
    }

    private void RemoveAddress()
    {
        Console.Write("ID adrese: ");
        int id = ConsoleViewUtils.SafeInputInt();
        Address? removed = _addressDao.RemoveAddress(id);
        Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
    }

    private Address InputAddress()
    {
        Address address = new Address();
        Console.Write("Ulica: ");
        address.Street = Console.ReadLine() ?? string.Empty;
        Console.Write("Broj: ");
        address.Number = Console.ReadLine() ?? string.Empty;
        Console.Write("Grad: ");
        address.City = Console.ReadLine() ?? string.Empty;
        Console.Write("Drzava: ");
        address.Country = Console.ReadLine() ?? string.Empty;
        return address;
    }
}
