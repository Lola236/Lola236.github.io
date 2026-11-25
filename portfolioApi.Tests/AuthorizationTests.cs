using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using portfolioApi.Data;
using portfolioApi.Models;

namespace portfolioApi.Tests;

//bevat automatische testen voor rollen en rechten
public class AuthorizationTests
{
    //GET alle projecten

    //test of iedereen alle projecten kan ophalen
    [Theory]
    [InlineData("Geen", 200)]
    [InlineData("Gast", 200)]
    [InlineData("Editor", 200)]
    [InlineData("Admin", 200)]
    public async Task GetProjects_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("projects", "GET", role, expectedStatus);
    }


    //GET 1 project

    //test of iedereen een bestaand project kan ophalen
    [Theory]
    [InlineData("Geen", 200)]
    [InlineData("Gast", 200)]
    [InlineData("Editor", 200)]
    [InlineData("Admin", 200)]
    public async Task GetProject_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("projects", "GETID", role, expectedStatus);
    }


    //POST project

    //test welke gebruikers een project mogen aanmaken
    [Theory]
    [InlineData("Geen", 401)]
    [InlineData("Gast", 403)]
    [InlineData("Editor", 201)]
    [InlineData("Admin", 201)]
    public async Task PostProject_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("projects", "POST", role, expectedStatus);
    }


    //PUT project

    //test welke gebruikers een project mogen wijzigen
    [Theory]
    [InlineData("Geen", 401)]
    [InlineData("Gast", 403)]
    [InlineData("Editor", 204)]
    [InlineData("Admin", 204)]
    public async Task PutProject_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("projects", "PUT", role, expectedStatus);
    }


    //DELETE project

    //test welke gebruikers een project mogen verwijderen
    [Theory]
    [InlineData("Geen", 401)]
    [InlineData("Gast", 403)]
    [InlineData("Editor", 204)]
    [InlineData("Admin", 204)]
    public async Task DeleteProject_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("projects", "DELETE", role, expectedStatus);
    }


    //GET alle blogposts

    //test of iedereen alle blogposts kan ophalen
    [Theory]
    [InlineData("Geen", 200)]
    [InlineData("Gast", 200)]
    [InlineData("Editor", 200)]
    [InlineData("Admin", 200)]
    public async Task GetBlogposts_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("blogposts", "GET", role, expectedStatus);
    }


    //GET 1 blogpost

    //test of iedereen een bestaande blogpost kan ophalen
    [Theory]
    [InlineData("Geen", 200)]
    [InlineData("Gast", 200)]
    [InlineData("Editor", 200)]
    [InlineData("Admin", 200)]
    public async Task GetBlogpost_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("blogposts", "GETID", role, expectedStatus);
    }


    //POST blogpost

    //test welke gebruikers een blogpost mogen aanmaken
    [Theory]
    [InlineData("Geen", 401)]
    [InlineData("Gast", 403)]
    [InlineData("Editor", 201)]
    [InlineData("Admin", 201)]
    public async Task PostBlogpost_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("blogposts", "POST", role, expectedStatus);
    }


    //PUT blogpost

    //test welke gebruikers een blogpost mogen wijzigen
    [Theory]
    [InlineData("Geen", 401)]
    [InlineData("Gast", 403)]
    [InlineData("Editor", 204)]
    [InlineData("Admin", 204)]
    public async Task PutBlogpost_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("blogposts", "PUT", role, expectedStatus);
    }


    //DELETE blogpost

    //test welke gebruikers een blogpost mogen verwijderen
    [Theory]
    [InlineData("Geen", 401)]
    [InlineData("Gast", 403)]
    [InlineData("Editor", 204)]
    [InlineData("Admin", 204)]
    public async Task DeleteBlogpost_AlleRollen_ReturnsStatus(
        string role, int expectedStatus)
    {
        await TestRoute("blogposts", "DELETE", role, expectedStatus);
    }


    //Hulpmethode voor alle autorisatietesten

    //voert de test uit met de opgegeven route, methode en rol
    private async Task TestRoute(
        string route,
        string method,
        string role,
        int expectedStatus)
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

        //controleert of de juiste testdatabase wordt gebruikt
        Assert.Equal(
            "PortfolioTestDb",
            context.Database.GetDbConnection().Database);

        //maakt unieke inloggegevens aan
        var account = new
        {
            Email = $"user{Guid.NewGuid():N}@test.nl",
            Password = $"Aa1!{Guid.NewGuid():N}"
        };

        //maakt unieke titels voor de testgegevens
        var oldTitle = $"Oude titel {Guid.NewGuid():N}";
        var newTitle = $"Nieuwe titel {Guid.NewGuid():N}";

        //bewaart het ID van het testrecord
        int id = 0;

        try
        {
            //registreert en logt in als een rol nodig is
            if (role != "Geen")
            {
                var registerResponse = await client.PostAsJsonAsync(
                    "/api/auth/register", account);

                //controleert of de registratie gelukt is
                Assert.Equal(
                    HttpStatusCode.OK,
                    registerResponse.StatusCode);

                //zoekt de geregistreerde gebruiker op
                var user = await userManager.FindByEmailAsync(
                    account.Email);

                Assert.NotNull(user);

                //wijzigt de rol als de gebruiker geen Gast moet zijn
                if (role != "Gast")
                {
                    //verwijdert de standaardrol Gast
                    var removeRole = await userManager
                        .RemoveFromRoleAsync(user, "Gast");

                    Assert.True(removeRole.Succeeded);

                    //voegt de gewenste rol toe
                    var addRole = await userManager
                        .AddToRoleAsync(user, role);

                    Assert.True(addRole.Succeeded);
                }

                //logt in met het tijdelijke account
                var loginResponse = await client.PostAsJsonAsync(
                    "/api/auth/login", account);

                //controleert of het inloggen gelukt is
                Assert.Equal(
                    HttpStatusCode.OK,
                    loginResponse.StatusCode);
            }

            //maakt een testrecord aan voor GET, PUT en DELETE
            if (method != "POST")
            {
                if (route == "projects")
                {
                    //maakt een testproject aan
                    var project = new Project
                    {
                        Titel = oldTitle,
                        Beschrijving = "Oude beschrijving",
                        Categorie = "School",
                        GitHubUrl = "https://github.com/test",
                        Datum = DateTime.Now
                    };

                    //slaat het project op
                    context.Projects.Add(project);
                    await context.SaveChangesAsync();

                    //bewaart het ID
                    id = project.Id;
                }
                else
                {
                    //maakt een testblogpost aan
                    var blogpost = new Blogpost
                    {
                        Titel = oldTitle,
                        Inhoud = "Oude inhoud",
                        Publicatiedatum = DateTime.Now
                    };

                    //slaat de blogpost op
                    context.Blogposts.Add(blogpost);
                    await context.SaveChangesAsync();

                    //bewaart het ID
                    id = blogpost.Id;
                }
            }

            //bereidt de projectgegevens voor
            var projectData = new
            {
                Titel = newTitle,
                Beschrijving = "Nieuwe beschrijving",
                Categorie = "School",
                GitHubUrl = "https://github.com/test",
                Datum = DateTime.Now
            };

            //bereidt de blogpostgegevens voor
            var blogpostData = new
            {
                Titel = newTitle,
                Inhoud = "Nieuwe inhoud"
            };

            //maakt de URL van de route
            var url = $"/api/{route}";

            //bewaart de response van de API
            HttpResponseMessage response;

            //Act

            //voert de juiste HTTP-methode uit
            switch (method)
            {
                case "GET":
                    response = await client.GetAsync(url);
                    break;

                case "GETID":
                    response = await client.GetAsync($"{url}/{id}");
                    break;

                case "POST":
                    if (route == "projects")
                    {
                        response = await client.PostAsJsonAsync(
                            url, projectData);
                    }
                    else
                    {
                        response = await client.PostAsJsonAsync(
                            url, blogpostData);
                    }
                    break;

                case "PUT":
                    if (route == "projects")
                    {
                        response = await client.PutAsJsonAsync(
                            $"{url}/{id}", projectData);
                    }
                    else
                    {
                        response = await client.PutAsJsonAsync(
                            $"{url}/{id}", blogpostData);
                    }
                    break;

                case "DELETE":
                    response = await client.DeleteAsync($"{url}/{id}");
                    break;

                default:
                    throw new Exception("Onbekende HTTP-methode");
            }

            //Assert

            //controleert of de statuscode overeenkomt
            Assert.Equal(expectedStatus, (int)response.StatusCode);

            //controleert of de gebruiker schrijfrechten heeft
            bool canEdit = role == "Editor" || role == "Admin";

            //controleert het resultaat in de database
            if (route == "projects")
            {
                if (method == "POST")
                {
                    //controleert of het project is aangemaakt
                    bool exists = await context.Projects
                        .AnyAsync(p => p.Titel == newTitle);

                    Assert.Equal(canEdit, exists);
                }
                else if (method == "PUT")
                {
                    //haalt de actuele projectgegevens op
                    var project = await context.Projects
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                    Assert.NotNull(project);

                    //controleert of de titel terecht is gewijzigd
                    Assert.Equal(
                        canEdit ? newTitle : oldTitle,
                        project.Titel);
                }
                else if (method == "DELETE")
                {
                    //controleert of het project nog bestaat
                    bool exists = await context.Projects
                        .AnyAsync(p => p.Id == id);

                    Assert.Equal(!canEdit, exists);
                }
            }
            else
            {
                if (method == "POST")
                {
                    //controleert of de blogpost is aangemaakt
                    bool exists = await context.Blogposts
                        .AnyAsync(b => b.Titel == newTitle);

                    Assert.Equal(canEdit, exists);
                }
                else if (method == "PUT")
                {
                    //haalt de actuele blogpostgegevens op
                    var blogpost = await context.Blogposts
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.Id == id);

                    Assert.NotNull(blogpost);

                    //controleert of de titel terecht is gewijzigd
                    Assert.Equal(
                        canEdit ? newTitle : oldTitle,
                        blogpost.Titel);
                }
                else if (method == "DELETE")
                {
                    //controleert of de blogpost nog bestaat
                    bool exists = await context.Blogposts
                        .AnyAsync(b => b.Id == id);

                    Assert.Equal(!canEdit, exists);
                }
            }
        }
        finally
        {
            //verwijdert de eerder gevolgde objecten uit de context
            context.ChangeTracker.Clear();

            //ruimt de tijdelijke projecten op
            if (route == "projects")
            {
                var projects = await context.Projects
                    .Where(p => p.Id == id ||
                                p.Titel == oldTitle ||
                                p.Titel == newTitle)
                    .ToListAsync();

                context.Projects.RemoveRange(projects);
            }
            else
            {
                //ruimt de tijdelijke blogposts op
                var blogposts = await context.Blogposts
                    .Where(b => b.Id == id ||
                                b.Titel == oldTitle ||
                                b.Titel == newTitle)
                    .ToListAsync();

                context.Blogposts.RemoveRange(blogposts);
            }

            //slaat het verwijderen op
            await context.SaveChangesAsync();

            //zoekt de tijdelijke gebruiker op
            var testUser = await userManager.FindByEmailAsync(
                account.Email);

            //verwijdert de tijdelijke gebruiker als deze bestaat
            if (testUser != null)
            {
                var result = await userManager.DeleteAsync(testUser);

                Assert.True(result.Succeeded);
            }
        }
    }
}