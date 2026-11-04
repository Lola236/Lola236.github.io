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