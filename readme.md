Voor deze opdracht heb ik een backend gemaakt voor mijn portfoliosite.
Met deze API kan ik projecten en blogposts toevoegen, bekijken, aanpassen en verwijderen(CRUD)

Gebruikte technologieën
- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger UI


API-stijl
Ik heb gekozen voor controllers. Controllers ontvangen verzoeken, zoals GET, POST, PUT en DELETE.
Ik vind controllers handig omdat ik mijn code hiermee overzichtelijk kan houden.

Codestructuur
Ik heb mijn code verdeeld over verschillende mappen
- Controllers: ontvange verzoeken + geven antwoorden terug
- Services: zorgen dat gegevens worden opgehaald, toegevoegd,aangepast of verwijderd
- Models: beschrijft hoe een project of blogpost eruitziet
- Dtos: bepaalt welke gegevens worden ontvangen en teruggestuurd
- Data: bevat AppDbContext die met db communiceert
- Migrations: zorgt ervoor dat db wordt aangemaakt of bijgewerkt

Dependency Injection
Ik gebruik Dependency Injection zodat mijn controllers de services kunnen gebruiken.
Ik heb dit gedaan in Program.cs ProjectService en BlogpostService met AddScoped().Hierdoor krijgt de controller automatisch service die hij nodig heeft.


Database
Ik gebruik SQLserver om projecten en blogposts op teslaan.
Met Entity Framework Core kan mijn API gegevens opslaan, ophalen, aanpassen en verwijderen.
Ik gebruik EF Core Migrations om de database aan te maken en bij te werken.
De verbinding met SQLserver staat in appsettings.json.

Database aanmaken
1. SQLserver moet gestart zijn.
2. Controleer de connection string in appsettings.json.
3. Open terminal in portfolioApi.
4. Voer dotnet ef database update uit
Hiermee worden de migrations uitgevoerd en wordt db aangemaakt of bijgewerkt

Applicatie starten
1. Open terminal in portfolioApi.
2. Voer dotnet run uit
3. Open Swagger UI in browser http://localhost:5119/swagger
Via Swagger kunnen alle CRUD-endpoints voor projects en blogposts worden getest

API-endpoints
Projects
- GET /api/projects
- GET /api/projects/{id}
- POST /api/projects
- PUT /api/projects/{id}
- DELETE /api/projects/{id}

Blogposts
- GET /api/blogposts
- GET /api/blogposts/{id}
- POST /api/blogposts
- PUT /api/blogposts/{id}
- DELETE /api/blogposts/{id}


Hoe werkt mijn API?
Als ik via swagger een verzoekstuur, komt het eerst bij de controller.
De controller gebruikt service om gegevens op tehalen, toe tevoegen, aan tepassen of te verwijderen.
De service gebruikt AppDbContext om gegevens uit sqlserver op tehalen of op teslaan.
Daarna stuurt de controller gegevens terug als json.
Ik gebruik dtos om tebepalen welke gegevens worden ontvangen en teruggestuurd

Voor opdracht 4 heb ik mijn bestaande api uitgebreid met authenticatie, autorisatie en automatische testen.

Toegevoegde technologieën
- ASP.NET Core Identity
- xUnit
- WebApplicationFactory
- Coverlet

Authenticatie
Ik gebruik ASP.NET Core Identity voor het registreren en inloggen van gebruikers. Met UserManager worden nieuwe gebruikers aangemaakt en krijgen zij automatisch de rol Gast. Wachtwoorden worden gehasht opgeslagen in de database. Met SignInManager worden de inloggegevens gecontroleerd. Bij een succesvolle login ontvangt de gebruiker een authenticatiecookie.

API-endpoints die ik heb toegevoegd
- POST /api/Auth/register
- POST /api/Auth/login

Autorisatie
Ik heb drie rollen toegevoegd:
- Gast: mag alleen projecten en blogposts bekijken.
- Editor: mag projecten en blogposts bekijken, toevoegen, aanpassen en verwijderen.
- Admin: mag projecten en blogposts bekijken, toevoegen, aanpassen en verwijderen.

De rollen worden automatisch aangemaakt door DbSeeder wanneer de api wordt gestart. In Program.cs heb ik de policy CanEditPortfolio toegevoegd. Hiermee mogen alleen gebruikers met de rol editor of admin post, put en delete verzoeken uitvoeren. Alle gets routes zijn openbaar.
Bij een schrijfverzoek zonder login geeft de api statuscode 401 terug. Wanneer een gebruiker is ingelogd maar niet de juiste rol heeft, geeft de api statuscode 403 terug.

Identity-migratie uitvoeren
1. Open de terminal.
2. Controleer de connection string in appsettings.json.
3. Voer dotnet ef database update uit.
Hiermee wordt ook de AddIdentity-migratie uitgevoerd en worden de Identity-tabellen toegevoegd aan de database.

Gebruikers aanmaken en rollen toekennen
Gast
1. Start de api en open swagger.
2. Registreer een gebruiker via POST /api/Auth/register.
3. De gebruiker krijgt automatisch de rol gast.

Editor
Registreer een nieuwe gebruiker via swagger. Open sql, selecteer PortfolioDb en voer de query uit.
Vervang het email door dat van de geregistreerde gebruiker.

UPDATE AspNetUserRoles
SET RoleId = (
    SELECT Id FROM AspNetRoles WHERE Name = 'Editor'
)
WHERE UserId = (
    SELECT Id FROM AspNetUsers WHERE Email = 'editor@test.nl'
)
AND RoleId = (
    SELECT Id FROM AspNetRoles WHERE Name = 'Gast'
);

Admin
Registreer een nieuwe gebruiker en voer de query uit met het juiste email.

UPDATE AspNetUserRoles
SET RoleId = (
    SELECT Id FROM AspNetRoles WHERE Name = 'Admin'
)
WHERE UserId = (
    SELECT Id FROM AspNetUsers WHERE Email = 'admin@test.nl'
)
AND RoleId = (
    SELECT Id FROM AspNetRoles WHERE Name = 'Gast'
);

Rollen controleren
SELECT u.Email, r.Name AS Rol
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id;

Na het aanpassen van een rol moet de gebruiker opnieuw inloggen via POST /api/Auth/login.

Automatische testen
Ik heb een afzonderlijk xUnit testproject aangemaakt genaamd. Met WebApplicationFactory kan ik mijn api via http testen zonder de api handmatig te starten.
Ik gebruik arrange, act en assert in mijn testen.
- Arrange: voorbereiden van testgegevens.
- Act: uitvoeren van functionaliteit.
- Assert: controleren of resultaat klopt.

Testdatabase
Ik gebruik een aparte testdatabase, zodat mijn gewone database niet wordt aangepast tijdens het testen.

Testdatabase voorbereiden
1. Ik maak in sql een database aan
2. ik controleer de connection string in portfolioApi.Tests/TestFactory.cs en pas als het nodig de servernaam aan.
3. ik open de terminal.
4. ik voer de commando uit.
dotnet ef database update --project portfolioApi --connection "Server=-;Database=PortfolioTestDb;Trusted_Connection=True;TrustServerCertificate=True;"

De tijdelijke testgegevens worden na het testen verwijderd.

Testen uitvoeren
1. Open de terminal
2. dotnet test uitvoeren.

Overzicht van de testen
- ProjectsTests.cs = 5 testen
- BlogpostsTests.cs = 5 testen
- AuthTests.cs = 6 testen
- AuthorizationTests.cs = 40 testen
- ErrorTests.cs = 10 testen
- DatabaseTests.cs = 1 testen
Totaal = 67, alle 67 testen zijn succesvol uitgevoerd.

Code coverage
Ik gebruik Coverlet om te meten hoeveel van mijn applicatiecode wordt uitgevoerd tijdens het testen. Om de code coverage te meten voer ik de commando uit:
dotnet test --collect:"XPlat Code Coverage" --settings portfolioApi/coverage.runsettings
Met coverage.runsettings sluit ik migraties en automatisch gegenereerde bestanden in de obj-map uit van de meting.

Mijn resultaten:
- Line coverage: 95,09%.
- Branch coverage: 80,56%.
- Geslaagde testen: 67 van 67.
