using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using portfolioApi.Data;
using portfolioApi.Models;

namespace portfolioApi.Tests;

//bevat automatische testen voor projects
public class ProjectsTests
{
    //GET alle projecten

    //test of iedereen zonder login projecten kan ophalen
    [Fact]
    public async Task GetProjects_ZonderLogin_Returns200()
    {
        //Arrange

        //maakt de testomgeving aan
        using var factory = new TestFactory();

        //maakt een HTTP-client aan
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                //stelt het basisadres van de test-API in
                BaseAddress = new Uri("https://localhost"),

                //voorkomt automatisch doorsturen
                AllowAutoRedirect = false
            });

        //maakt een tijdelijke scope aan
        using var scope = factory.Services.CreateScope();

        //haalt de databasecontext op
        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        //controleert of de testdatabase wordt gebruikt
        Assert.Equal("PortfolioTestDb",
            context.Database.GetDbConnection().Database);

        //maakt een testproject aan
        var project = new Project
        {
            Titel = $"Testproject {Guid.NewGuid():N}",
            Beschrijving = "Beschrijving van testproject",
            Categorie = "School",
            GitHubUrl = "https://github.com/test",
            Datum = DateTime.Now
        };

        //slaat het testproject op in de testdatabase
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        try
        {
            //Act

            //haalt alle projecten op
            var response = await client.GetAsync("/api/projects");

            //Assert

            //controleert of de statuscode 200 is!!!
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //zet de JSON-response om naar een lijst met projecten
            //rresponse gegevens ophalen
            var result = await response.Content
                .ReadFromJsonAsync<List<Project>>();

            //controleert of de lijst bestaat
            Assert.NotNull(result);

            //controleert of het testproject in de lijst staat
            // controlleert responsegegevens
            Assert.Contains(result, p =>
                p.Id == project.Id &&
                p.Titel == project.Titel);
        }
        finally
        {
            //verwijdert het tijdelijke testproject
            context.Projects.Remove(project);
            await context.SaveChangesAsync();
        }
    }


    //GET 1 project

    //test of een bestaand project opgehaald kan worden
    [Fact]
    public async Task GetProject_BestaandId_Returns200()
    {
        //Arrange

        //maakt de testomgeving aan
        using var factory = new TestFactory();

        //maakt een HTTP-client aan
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });

        //maakt een tijdelijke scope aan
        using var scope = factory.Services.CreateScope();

        //haalt de databasecontext op
        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        //maakt een testproject aan
        var project = new Project
        {
            Titel = "Testproject",
            Beschrijving = "Beschrijving van testproject",
            Categorie = "School",
            GitHubUrl = "https://github.com/test",
            Datum = DateTime.Now
        };

        //slaat het testproject op in de testdatabase
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        try
        {
            //Act

            //haalt het project op via het ID
            var response = await client.GetAsync(
                $"/api/projects/{project.Id}");

            //Assert

            //controleert of de statuscode 200 is
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //zet de JSON-response om naar een project
            var result = await response.Content
                .ReadFromJsonAsync<Project>();

            //controleert of het project bestaat
            Assert.NotNull(result);

            //controleert of het juiste ID is teruggegeven
            Assert.Equal(project.Id, result.Id);

            //controleert of de titel overeenkomt
            Assert.Equal(project.Titel, result.Titel);
        }
        finally
        {
            //verwijdert het testproject na afloop
            context.Projects.Remove(project);
            await context.SaveChangesAsync();
        }
    }


    //POST project

    //test of een editor een project kan aanmaken
    [Fact]
    public async Task PostProject_AlsEditor_Returns201()
    {
        //Arrange

        //maakt de testomgeving aan
        using var factory = new TestFactory();

        //maakt een HTTP-client aan die cookies bewaart
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false,
                HandleCookies = true
            });

        //maakt een tijdelijke scope aan
        using var scope = factory.Services.CreateScope();

        //haalt de databasecontext op
        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        //haalt de UserManager op
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        //maakt unieke inloggegevens aan
        var email = $"editor{Guid.NewGuid():N}@test.nl";
        var password = $"Aa1!{Guid.NewGuid():N}";

        //bereidt de registratiegegevens voor
        var account = new
        {
            Email = email,
            Password = password
        };

        //bereidt de projectgegevens voor
        var project = new
        {
            Titel = $"Testproject {Guid.NewGuid():N}",
            Beschrijving = "Automatische POST-test",
            Categorie = "School",
            GitHubUrl = "https://github.com/test",
            Datum = DateTime.Now
        };

        try
        {
            //registreert een tijdelijke gebruiker
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register", account);

            //controleert of de registratie gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                registerResponse.StatusCode);

            //zoekt de geregistreerde gebruiker op
            var user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);

            //verwijdert de rol Gast
            var removeRole = await userManager.RemoveFromRoleAsync(
                user, "Gast");

            //controleert of de rol verwijderd is
            Assert.True(removeRole.Succeeded);

            //geeft de gebruiker de rol Editor
            var addRole = await userManager.AddToRoleAsync(
                user, "Editor");

            //controleert of de rol toegevoegd is
            Assert.True(addRole.Succeeded);

            //logt in als Editor
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login", account);

            //controleert of het inloggen gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            //Act

            //maakt een project aan via de API
            var response = await client.PostAsJsonAsync(
                "/api/projects", project);

            //Assert

            //controleert of de statuscode 201 is
            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            //leest het aangemaakte project uit de response
            var result = await response.Content
                .ReadFromJsonAsync<Project>();

            //controleert of het project bestaat
            Assert.NotNull(result);

            //controleert of de titel overeenkomt
            Assert.Equal(project.Titel, result.Titel);

            //zoekt het aangemaakte project op in de database
            var savedProject = await context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == result.Id);

            //controleert of het project echt is opgeslagen
            Assert.NotNull(savedProject);
            Assert.Equal(project.Titel, savedProject.Titel);
        }
        finally
        {
            //zoekt het tijdelijke testproject op
            var testProject = await context.Projects
                .FirstOrDefaultAsync(p => p.Titel == project.Titel);

            //verwijdert het testproject als het bestaat
            if (testProject != null)
            {
                context.Projects.Remove(testProject);
                await context.SaveChangesAsync();
            }

            //zoekt de tijdelijke gebruiker op
            var testUser = await userManager.FindByEmailAsync(email);

            //verwijdert de tijdelijke gebruiker
            if (testUser != null)
            {
                var deleteResult = await userManager.DeleteAsync(testUser);

                //controleert of de gebruiker verwijderd is
                Assert.True(deleteResult.Succeeded);
            }
        }
    }


    //PUT project

    //test of een editor een bestaand project kan wijzigen
    [Fact]
    public async Task PutProject_AlsEditor_Returns204()
    {
        //Arrange

        //maakt de testomgeving aan
        using var factory = new TestFactory();

        //maakt een HTTP-client aan die cookies bewaart
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false,
                HandleCookies = true
            });

        //maakt een tijdelijke scope aan
        using var scope = factory.Services.CreateScope();

        //haalt de databasecontext op
        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        //haalt de UserManager op
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        //maakt unieke inloggegevens aan
        var email = $"editor{Guid.NewGuid():N}@test.nl";
        var password = $"Aa1!{Guid.NewGuid():N}";

        //bereidt de registratiegegevens voor
        var account = new
        {
            Email = email,
            Password = password
        };

        //maakt een bestaand testproject aan
        var project = new Project
        {
            Titel = "Oude titel",
            Beschrijving = "Oude beschrijving",
            Categorie = "School",
            GitHubUrl = "https://github.com/test",
            Datum = DateTime.Now
        };

        //slaat het testproject op in de database
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        //bereidt de gewijzigde projectgegevens voor
        var updatedProject = new
        {
            Titel = "Nieuwe titel",
            Beschrijving = "Nieuwe beschrijving",
            Categorie = "School",
            GitHubUrl = "https://github.com/test",
            Datum = DateTime.Now
        };

        try
        {
            //registreert een tijdelijke gebruiker
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register", account);

            //controleert of de registratie gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                registerResponse.StatusCode);

            //zoekt de geregistreerde gebruiker op
            var user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);

            //verwijdert de rol Gast
            var removeRole = await userManager.RemoveFromRoleAsync(
                user, "Gast");

            //controleert of de rol verwijderd is
            Assert.True(removeRole.Succeeded);

            //geeft de gebruiker de rol Editor
            var addRole = await userManager.AddToRoleAsync(
                user, "Editor");

            //controleert of de rol toegevoegd is
            Assert.True(addRole.Succeeded);

            //logt in als Editor
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login", account);

            //controleert of het inloggen gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            //Act

            //wijzigt het bestaande project via de API
            var response = await client.PutAsJsonAsync(
                $"/api/projects/{project.Id}", updatedProject);

            //Assert

            //controleert of de statuscode 204 is
            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);

            //haalt de nieuwste projectgegevens op
            var savedProject = await context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            //controleert of het project nog bestaat
            Assert.NotNull(savedProject);

            //controleert of de titel gewijzigd is
            Assert.Equal(updatedProject.Titel, savedProject.Titel);

            //controleert of de beschrijving gewijzigd is
            Assert.Equal(
                updatedProject.Beschrijving,
                savedProject.Beschrijving);
        }
        finally
        {
            //verwijdert het tijdelijke testproject
            context.Projects.Remove(project);
            await context.SaveChangesAsync();

            //zoekt de tijdelijke gebruiker op
            var testUser = await userManager.FindByEmailAsync(email);

            //verwijdert de tijdelijke gebruiker
            if (testUser != null)
            {
                var deleteResult = await userManager.DeleteAsync(testUser);

                //controleert of de gebruiker verwijderd is
                Assert.True(deleteResult.Succeeded);
            }
        }
    }


    //DELETE project

    //test of een editor een bestaand project kan verwijderen
    [Fact]
    public async Task DeleteProject_AlsEditor_Returns204()
    {
        //Arrange

        //maakt de testomgeving aan
        using var factory = new TestFactory();

        //maakt een HTTP-client aan die cookies bewaart
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false,
                HandleCookies = true
            });

        //maakt een tijdelijke scope aan
        using var scope = factory.Services.CreateScope();

        //haalt de databasecontext op
        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        //haalt de UserManager op
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        //maakt unieke inloggegevens aan
        var email = $"editor{Guid.NewGuid():N}@test.nl";
        var password = $"Aa1!{Guid.NewGuid():N}";

        //bereidt de registratiegegevens voor
        var account = new
        {
            Email = email,
            Password = password
        };

        //maakt een testproject aan
        var project = new Project
        {
            Titel = "Project om te verwijderen",
            Beschrijving = "Automatische DELETE-test",
            Categorie = "School",
            GitHubUrl = "https://github.com/test",
            Datum = DateTime.Now
        };

        //slaat het testproject op in de database
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        try
        {
            //registreert een tijdelijke gebruiker
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register", account);

            //controleert of de registratie gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                registerResponse.StatusCode);

            //zoekt de geregistreerde gebruiker op
            var user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);

            //verwijdert de rol Gast
            var removeRole = await userManager.RemoveFromRoleAsync(
                user, "Gast");

            //controleert of de rol verwijderd is
            Assert.True(removeRole.Succeeded);

            //geeft de gebruiker de rol Editor
            var addRole = await userManager.AddToRoleAsync(
                user, "Editor");

            //controleert of de rol toegevoegd is
            Assert.True(addRole.Succeeded);

            //logt in als Editor
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login", account);

            //controleert of het inloggen gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            //Act

            //verwijdert het project via de API
            var response = await client.DeleteAsync(
                $"/api/projects/{project.Id}");

            //Assert

            //controleert of de statuscode 204 is
            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);

            //zoekt het verwijderde project op in de database
            var deletedProject = await context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            //controleert of het project niet meer bestaat
            Assert.Null(deletedProject);
        }
        finally
        {
            //controleert of het testproject nog bestaat
            var testProject = await context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            //ruimt het testproject op als de DELETE-test mislukt
            if (testProject != null)
            {
                context.Projects.Remove(project);
                await context.SaveChangesAsync();
            }

            //zoekt de tijdelijke gebruiker op
            var testUser = await userManager.FindByEmailAsync(email);

            //verwijdert de tijdelijke gebruiker
            if (testUser != null)
            {
                var deleteResult = await userManager.DeleteAsync(testUser);

                //controleert of de gebruiker verwijderd is
                Assert.True(deleteResult.Succeeded);
            }
        }
    }
}