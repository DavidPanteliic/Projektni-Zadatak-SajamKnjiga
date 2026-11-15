using BookFair.Core.Services;
using BookFair.Core.Models;

namespace BookFair.ConsoleClient.Views;

public class PublisherConsoleView
{
    private readonly PublisherService _publisherService;

    public PublisherConsoleView(PublisherService publisherService)
    {
        _publisherService = publisherService;
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
        Console.WriteLine("=== IZDAVACI ===");
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
            case "1": ShowAllPublishers(); break;
            case "2": AddPublisher(); break;
            case "3": UpdatePublisher(); break;
            case "4": RemovePublisher(); break;
            default: Console.WriteLine("Nepoznata opcija."); break;
        }
    }

    private void ShowAllPublishers()
    {
        List<Publisher> publishers = _publisherService.GetAllPublishers();
        if (publishers.Count == 0)
        {
            Console.WriteLine("Nema izdavaca.");
            return;
        }

        foreach (Publisher publisher in publishers)
        {
            Console.WriteLine($"ID: {publisher.Id}, SIFRA: {publisher.Code}, NAZIV: {publisher.Name}, SEF: {HeadAuthorToText(publisher)}");
        }
    }

    private void AddPublisher()
    {
        Publisher publisher = InputPublisher();
        try
        {
            _publisherService.AddPublisher(publisher);
            Console.WriteLine("Uspesno dodato.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void UpdatePublisher()
    {
        Console.Write("ID izdavaca: ");
        int id = ConsoleViewUtils.SafeInputInt();
        if (_publisherService.GetPublisherById(id) is null)
        {
            Console.WriteLine("Nije pronadjeno.");
            return;
        }

        Publisher publisher = InputPublisher();
        publisher.Id = id;
        try
        {
            Publisher? updated = _publisherService.UpdatePublisher(publisher);
            Console.WriteLine(updated is null ? "Nije pronadjeno." : "Uspesno izmenjeno.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void RemovePublisher()
    {
        Console.Write("ID izdavaca: ");
        int id = ConsoleViewUtils.SafeInputInt();
        try
        {
            Publisher? removed = _publisherService.RemovePublisher(id);
            Console.WriteLine(removed is null ? "Nije pronadjeno." : "Uspesno obrisano.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private string HeadAuthorToText(Publisher publisher)
    {
        if (publisher.HeadAuthorId is null)
        {
            return "nema";
        }

        Author? headAuthor = _publisherService.GetHeadAuthor(publisher);
        return headAuthor is null ? "nepoznat" : $"{headAuthor.FirstName} {headAuthor.LastName}";
    }

    private Publisher InputPublisher()
    {
        Publisher publisher = new Publisher();
        Console.Write("Sifra izdavaca: ");
        publisher.Code = ConsoleViewUtils.SafeInputText();
        Console.Write("Naziv: ");
        publisher.Name = ConsoleViewUtils.SafeInputText();
        Console.Write("ID sefa-autora (prazno ako nema): ");
        publisher.HeadAuthorId = ConsoleViewUtils.SafeInputOptionalInt();
        return publisher;
    }
}
