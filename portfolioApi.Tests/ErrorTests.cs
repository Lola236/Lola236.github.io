using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using portfolioApi.Data;
using portfolioApi.Models;

namespace portfolioApi.Tests;

//bevat automatische testen voor foutscenario's
public class ErrorTests
{
    //GET onbekend ID

    //test of een onbekend ID een 404 teruggeeft
    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Get_OnbekendId_Returns404(string route)
    {
        await TestOnbekendId(route, "GET");
    }


    //PUT onbekend ID

    //test of een onbekend record niet gewijzigd kan worden
    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Put_OnbekendId_Returns404(string route)
    {
        await TestOnbekendId(route, "PUT");
    }


    //DELETE onbekend ID

    //test of een onbekend record niet verwijderd kan worden
    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Delete_OnbekendId_Returns404(string route)
    {
        await TestOnbekendId(route, "DELETE");
    }


    //POST ongeldige invoer

    //test of ongeldige gegevens niet opgeslagen worden
    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Post_OngeldigeInvoer_Returns400(string route)
    {
        await TestOngeldigeInvoer(route, "POST");
    }


    //PUT ongeldige invoer

    //test of een record niet gewijzigd wordt met ongeldige gegevens
    [Theory]
    [InlineData("projects")]
    [InlineData("blogposts")]
    public async Task Put_OngeldigeInvoer_Returns400(string route)
    {
        await TestOngeldigeInvoer(route, "PUT");
    }


    //Hulpmethode onbekend ID

    //voert de testen voor onbekende ID's uit
    private async Task TestOnbekendId(
        string route, string method)
    {
        //Arrange

        //maakt de testomgeving aan
        using var factory = new TestFactory();

        //maakt een HTTP-client aan
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

        //controleert of de testdatabase wordt gebruikt
        Assert.Equal(
            "PortfolioTestDb",
            context.Database.GetDbConnection().Database);

        //maakt unieke inloggegevens aan
        var email = $"editor{Guid.NewGuid():N}@test.nl";
        var password = $"Aa1!{Guid.NewGuid():N}";

        //gebruikt een onbekend ID
        int id = int.MaxValue;

        //controleert of het ID echt niet bestaat
        if (route == "projects")
        {
            Assert.False(await context.Projects
                .AnyAsync(p => p.Id == id));
        }
        else
        {
            Assert.False(await context.Blogposts
                .AnyAsync(b => b.Id == id));
        }

        //bereidt geldige projectgegevens voor
        var projectData = new
        {
            Titel = "Testproject",
            Beschrijving = "Testbeschrijving",
            Categorie = "School",
            GitHubUrl = "https://github.com/test",
            Datum = DateTime.Now
        };

        //bereidt geldige blogpostgegevens voor
        var blogpostData = new
        {
            Titel = "Testblogpost",
            Inhoud = "Testinhoud"
        };

        try
        {
            //logt in als Editor voor PUT en DELETE
            if (method != "GET")
            {
                await LoginAlsEditor(
                    client, userManager, email, password);
            }

            //maakt de URL met het onbekende ID
            var url = $"/api/{route}/{id}";

            HttpResponseMessage response;

            //Act

            //voert de juiste HTTP-methode uit
            switch (method)
            {
                case "GET":
                    response = await client.GetAsync(url);
                    break;

                case "PUT":
                    if (route == "projects")
                    {
                        response = await client.PutAsJsonAsync(
                            url, projectData);
                    }
                    else
                    {
                        response = await client.PutAsJsonAsync(
                            url, blogpostData);
                    }
                    break;

                case "DELETE":
                    response = await client.DeleteAsync(url);
                    break;

                default:
                    throw new Exception("Onbekende methode");
            }

            //Assert

            //controleert of de statuscode 404 is
            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);

            //controleert of het onbekende ID nog steeds niet bestaat
            if (route == "projects")
            {
                Assert.False(await context.Projects
                    .AnyAsync(p => p.Id == id));
            }
            else
            {
                Assert.False(await context.Blogposts
                    .AnyAsync(b => b.Id == id));
            }
        }
        finally
        {
            //verwijdert de tijdelijke gebruiker
            await VerwijderGebruiker(userManager, email);
        }
    }


    //Hulpmethode ongeldige invoer

    //voert de testen voor ongeldige gegevens uit
    private async Task TestOngeldigeInvoer(
        string route, string method)
    {
        //Arrange

        //maakt de testomgeving aan
        using var factory = new TestFactory();

        //maakt een HTTP-client aan
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

        //controleert of de testdatabase wordt gebruikt
        Assert.Equal(
            "PortfolioTestDb",
            context.Database.GetDbConnection().Database);

        //maakt unieke inloggegevens aan
        var email = $"editor{Guid.NewGuid():N}@test.nl";
        var password = $"Aa1!{Guid.NewGuid():N}";

        //maakt unieke testgegevens aan
        var oldTitle = $"Testtitel {Guid.NewGuid():N}";
        var marker = $"Testinhoud {Guid.NewGuid():N}";

        //bewaart het ID van het testrecord
        int id = 0;

        //maakt ongeldige projectgegevens met een lege titel
        var projectData = new
        {
            Titel = "",
            Beschrijving = marker,
            Categorie = "School",
            GitHubUrl = "https://github.com/test",
            Datum = DateTime.Now
        };

        //maakt ongeldige blogpostgegevens met een lege titel
        var blogpostData = new
        {
            Titel = "",
            Inhoud = marker
        };

        try
        {
            //logt in als Editor
            await LoginAlsEditor(
                client, userManager, email, password);

            //maakt een bestaand record aan voor PUT
            if (method == "PUT")
            {
                if (route == "projects")
                {
                    var project = new Project
                    {
                        Titel = oldTitle,
                        Beschrijving = "Oude beschrijving",
                        Categorie = "School",
                        GitHubUrl = "https://github.com/test",
                        Datum = DateTime.Now
                    };

                    context.Projects.Add(project);
                    await context.SaveChangesAsync();

                    id = project.Id;
                }
                else
                {
                    var blogpost = new Blogpost
                    {
                        Titel = oldTitle,
                        Inhoud = "Oude inhoud",
                        Publicatiedatum = DateTime.Now
                    };

                    context.Blogposts.Add(blogpost);
                    await context.SaveChangesAsync();

                    id = blogpost.Id;
                }
            }

            //maakt de URL van de route
            var url = $"/api/{route}";

            HttpResponseMessage response;

            //Act

            //probeert ongeldige gegevens op te slaan
            if (method == "POST")
            {
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
            }
            else
            {
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
            }

            //Assert

            //controleert of de statuscode 400 is
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            if (route == "projects")
            {
                //controleert of de ongeldige gegevens niet opgeslagen zijn
                Assert.False(await context.Projects
                    .AnyAsync(p => p.Beschrijving == marker));

                if (method == "PUT")
                {
                    //haalt het oorspronkelijke project op
                    var project = await context.Projects
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                    //controleert of het project ongewijzigd is
                    Assert.NotNull(project);
                    Assert.Equal(oldTitle, project.Titel);
                    Assert.Equal(
                        "Oude beschrijving",
                        project.Beschrijving);
                }
            }
            else
            {
                //controleert of de ongeldige gegevens niet opgeslagen zijn
                Assert.False(await context.Blogposts
                    .AnyAsync(b => b.Inhoud == marker));

                if (method == "PUT")
                {
                    //haalt de oorspronkelijke blogpost op
                    var blogpost = await context.Blogposts
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.Id == id);

                    //controleert of de blogpost ongewijzigd is
                    Assert.NotNull(blogpost);
                    Assert.Equal(oldTitle, blogpost.Titel);
                    Assert.Equal("Oude inhoud", blogpost.Inhoud);
                }
            }
        }
        finally
        {
            //verwijdert eerder gevolgde objecten uit de context
            context.ChangeTracker.Clear();

            //ruimt de tijdelijke projecten op
            if (route == "projects")
            {
                var projects = await context.Projects
                    .Where(p => p.Id == id ||
                                p.Titel == oldTitle ||
                                p.Beschrijving == marker)
                    .ToListAsync();

                context.Projects.RemoveRange(projects);
            }
            else
            {
                //ruimt de tijdelijke blogposts op
                var blogposts = await context.Blogposts
                    .Where(b => b.Id == id ||
                                b.Titel == oldTitle ||
                                b.Inhoud == marker)
                    .ToListAsync();

                context.Blogposts.RemoveRange(blogposts);
            }

            //slaat het verwijderen op
            await context.SaveChangesAsync();

            //verwijdert de tijdelijke gebruiker
            await VerwijderGebruiker(userManager, email);
        }
    }


    //Editor login

    //registreert een tijdelijke gebruiker en logt in als Editor
    private async Task LoginAlsEditor(
        HttpClient client,
        UserManager<User> userManager,
        string email,
        string password)
    {
        //bereidt de registratiegegevens voor
        var account = new
        {
            Email = email,
            Password = password
        };

        //registreert de gebruiker
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register", account);

        Assert.Equal(
            HttpStatusCode.OK,
            registerResponse.StatusCode);

        //zoekt de geregistreerde gebruiker op
        var user = await userManager.FindByEmailAsync(email);

        Assert.NotNull(user);

        //verwijdert de rol Gast
        var removeRole = await userManager.RemoveFromRoleAsync(
            user, "Gast");

        Assert.True(removeRole.Succeeded);

        //geeft de gebruiker de rol Editor
        var addRole = await userManager.AddToRoleAsync(
            user, "Editor");

        Assert.True(addRole.Succeeded);

        //logt in als Editor
        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login", account);

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);
    }


    //Gebruiker opruimen

    //verwijdert de tijdelijke gebruiker na het testen
    private async Task VerwijderGebruiker(
        UserManager<User> userManager, string email)
    {
        //zoekt de tijdelijke gebruiker op
        var user = await userManager.FindByEmailAsync(email);

        //controleert of de gebruiker bestaat
        if (user != null)
        {
            var result = await userManager.DeleteAsync(user);

            Assert.True(result.Succeeded);
        }
    }
}