# Sajam knjiga – informacioni sistem uprave sajma

Projektni zadatak iz predmeta **Osnovi informacionih sistema i softverskog inženjerstva**, 2026/2027.

Desktop aplikacija sajma knjiga, pisana u C# (.NET 10).
U ovoj fazi (prva kontrolna tacka) aplikacija radi kroz konzolu.

## Tim

| Uloga | Ime i prezime | Broj indeksa |
|---|---|---|
| Student 1 | David Pantelić | RA179-2024 |
| Student 2 | Milovan Velemir | RA118-2024 |

## Podela posla

### Student 1 – David Pantelić
- Arhitektura rešenja (projekti `Core` i `ConsoleClient`) i modeli podataka
- Serijalizacija (`Serializer`, `Storage`, `ISerializable`) i putanje do fajlova (`DataPaths`)
- `#posetilac`: `VisitorDAO`, `VisitorConsoleView`
- `#kupovina`: `PurchaseDAO`, `PurchaseConsoleView`
- `#izdavac`: `PublisherDAO`, `PublisherService` (provera šefa izdavača), `PublisherConsoleView`
- Veze knjiga – autor i lista želja: `BookAuthorDAO`, `WishlistItemDAO`
- Glavni meni konzole (`MainConsoleView`)

### Student 2 – Milovan Velemir
- Arhitektura rešenja (projekti `Core` i `ConsoleClient`) i modeli podataka
- Serijalizacija (`Serializer`, `Storage`, `ISerializable`) i putanje do fajlova (`DataPaths`)
- `#autor`: `AuthorDAO`, `AuthorConsoleView`
- `#knjiga`: `BookDAO`, `BookConsoleView`
- `#adresa`: `AddressDAO`, `AddressConsoleView`
- Veze knjiga – autor i lista želja: `BookAuthorDAO`, `WishlistItemDAO`
- Glavni meni konzole (`MainConsoleView`)


Podaci se čuvaju u folderu `Data/` pored `BookFair.slnx`, svaki entitet u svom `.txt` fajlu.
Polja su razdvojena znakom `|`, pa taj znak nije dozvoljen u unosu kao i novi red.



