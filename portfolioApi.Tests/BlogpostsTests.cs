using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using portfolioApi.Data;
using portfolioApi.Models;

namespace portfolioApi.Tests;

//bevat automatische testen voor blogposts
public class BlogpostsTests
{
    //GET alle blogposts

    //test of iedereen zonder login blogposts kan ophalen
    [Fact]
    public async Task GetBlogposts_ZonderLogin_Returns200()
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

        //maakt een testblogpost aan
        var blogpost = new Blogpost
        {
            Titel = "Testblogpost",
            Inhoud = "Inhoud van de testblogpost",
            Publicatiedatum = DateTime.Now
        };

        //slaat de testblogpost op
        context.Blogposts.Add(blogpost);
        await context.SaveChangesAsync();

        try
        {
            //Act

            //haalt alle blogposts op
            var response = await client.GetAsync("/api/blogposts");

            //Assert

            //controleert of de statuscode 200 is
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //zet de JSON-response om naar een lijst
            var result = await response.Content
                .ReadFromJsonAsync<List<Blogpost>>();

            //controleert of de lijst bestaat
            Assert.NotNull(result);

            //controleert of de testblogpost in de lijst staat
            Assert.Contains(result, b =>
                b.Id == blogpost.Id &&
                b.Titel == blogpost.Titel);
        }
        finally
        {
            //verwijdert de tijdelijke blogpost
            context.Blogposts.Remove(blogpost);
            await context.SaveChangesAsync();
        }
    }


    //GET 1 blogpost

    //test of een bestaande blogpost opgehaald kan worden
    [Fact]
    public async Task GetBlogpost_BestaandId_Returns200()
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

        //maakt een testblogpost aan
        var blogpost = new Blogpost
        {
            Titel = "Testblogpost",
            Inhoud = "Inhoud van de testblogpost",
            Publicatiedatum = DateTime.Now
        };

        //slaat de testblogpost op
        context.Blogposts.Add(blogpost);
        await context.SaveChangesAsync();

        try
        {
            //Act

            //haalt de blogpost op via het ID
            var response = await client.GetAsync(
                $"/api/blogposts/{blogpost.Id}");

            //Assert

            //controleert of de statuscode 200 is
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //zet de JSON-response om naar een blogpost
            var result = await response.Content
                .ReadFromJsonAsync<Blogpost>();

            //controleert of de blogpost bestaat
            Assert.NotNull(result);

            //controleert de teruggegeven gegevens
            Assert.Equal(blogpost.Id, result.Id);
            Assert.Equal(blogpost.Titel, result.Titel);
            Assert.Equal(blogpost.Inhoud, result.Inhoud);
        }
        finally
        {
            //verwijdert de tijdelijke blogpost
            context.Blogposts.Remove(blogpost);
            await context.SaveChangesAsync();
        }
    }


    //POST blogpost

    //test of een editor een blogpost kan aanmaken
    [Fact]
    public async Task PostBlogpost_AlsEditor_Returns201()
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

        //bereidt de blogpostgegevens voor
        var blogpost = new
        {
            Titel = $"Testblogpost {Guid.NewGuid():N}",
            Inhoud = "Automatische POST-test"
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

            Assert.True(removeRole.Succeeded);

            //geeft de gebruiker de rol Editor
            var addRole = await userManager.AddToRoleAsync(
                user, "Editor");

            Assert.True(addRole.Succeeded);

            //logt in als Editor
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login", account);

            //controleert of het inloggen gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            //Act

            //maakt een blogpost aan via de API
            var response = await client.PostAsJsonAsync(
                "/api/blogposts", blogpost);

            //Assert

            //controleert of de statuscode 201 is
            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            //leest de aangemaakte blogpost uit de response
            var result = await response.Content
                .ReadFromJsonAsync<Blogpost>();

            //controleert de teruggegeven gegevens
            Assert.NotNull(result);
            Assert.Equal(blogpost.Titel, result.Titel);
            Assert.Equal(blogpost.Inhoud, result.Inhoud);

            //zoekt de blogpost op in de database
            var savedBlogpost = await context.Blogposts
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == result.Id);

            //controleert of de blogpost echt is opgeslagen
            Assert.NotNull(savedBlogpost);
            Assert.Equal(blogpost.Titel, savedBlogpost.Titel);
            Assert.Equal(blogpost.Inhoud, savedBlogpost.Inhoud);
        }
        finally
        {
            //zoekt de tijdelijke blogpost op
            var testBlogpost = await context.Blogposts
                .FirstOrDefaultAsync(b => b.Titel == blogpost.Titel);

            //verwijdert de blogpost als deze bestaat
            if (testBlogpost != null)
            {
                context.Blogposts.Remove(testBlogpost);
                await context.SaveChangesAsync();
            }

            //zoekt de tijdelijke gebruiker op
            var testUser = await userManager.FindByEmailAsync(email);

            //verwijdert de tijdelijke gebruiker
            if (testUser != null)
            {
                var deleteResult = await userManager.DeleteAsync(testUser);

                Assert.True(deleteResult.Succeeded);
            }
        }
    }


    //PUT blogpost

    //test of een editor een bestaande blogpost kan wijzigen
    [Fact]
    public async Task PutBlogpost_AlsEditor_Returns204()
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

        //maakt een bestaande testblogpost aan
        var blogpost = new Blogpost
        {
            Titel = "Oude titel",
            Inhoud = "Oude inhoud",
            Publicatiedatum = DateTime.Now
        };

        //slaat de testblogpost op
        context.Blogposts.Add(blogpost);
        await context.SaveChangesAsync();

        //bereidt de gewijzigde gegevens voor
        var updatedBlogpost = new
        {
            Titel = "Nieuwe titel",
            Inhoud = "Nieuwe inhoud"
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

            Assert.True(removeRole.Succeeded);

            //geeft de gebruiker de rol Editor
            var addRole = await userManager.AddToRoleAsync(
                user, "Editor");

            Assert.True(addRole.Succeeded);

            //logt in als Editor
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login", account);

            //controleert of het inloggen gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            //Act

            //wijzigt de bestaande blogpost via de API
            var response = await client.PutAsJsonAsync(
                $"/api/blogposts/{blogpost.Id}", updatedBlogpost);

            //Assert

            //controleert of de statuscode 204 is
            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);

            //haalt de nieuwste blogpostgegevens op
            var savedBlogpost = await context.Blogposts
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == blogpost.Id);

            //controleert of de blogpost nog bestaat
            Assert.NotNull(savedBlogpost);

            //controleert of de titel gewijzigd is
            Assert.Equal(updatedBlogpost.Titel, savedBlogpost.Titel);

            //controleert of de inhoud gewijzigd is
            Assert.Equal(updatedBlogpost.Inhoud, savedBlogpost.Inhoud);
        }
        finally
        {
            //verwijdert de tijdelijke blogpost
            context.Blogposts.Remove(blogpost);
            await context.SaveChangesAsync();

            //zoekt de tijdelijke gebruiker op
            var testUser = await userManager.FindByEmailAsync(email);

            //verwijdert de tijdelijke gebruiker
            if (testUser != null)
            {
                var deleteResult = await userManager.DeleteAsync(testUser);

                Assert.True(deleteResult.Succeeded);
            }
        }
    }


    //DELETE blogpost

    //test of een editor een bestaande blogpost kan verwijderen
    [Fact]
    public async Task DeleteBlogpost_AlsEditor_Returns204()
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

        //maakt een testblogpost aan
        var blogpost = new Blogpost
        {
            Titel = "Blogpost om te verwijderen",
            Inhoud = "Automatische DELETE-test",
            Publicatiedatum = DateTime.Now
        };

        //slaat de testblogpost op
        context.Blogposts.Add(blogpost);
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

            Assert.True(removeRole.Succeeded);

            //geeft de gebruiker de rol Editor
            var addRole = await userManager.AddToRoleAsync(
                user, "Editor");

            Assert.True(addRole.Succeeded);

            //logt in als Editor
            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login", account);

            //controleert of het inloggen gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            //Act

            //verwijdert de blogpost via de API
            var response = await client.DeleteAsync(
                $"/api/blogposts/{blogpost.Id}");

            //Assert

            //controleert of de statuscode 204 is
            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);

            //zoekt de verwijderde blogpost op
            var deletedBlogpost = await context.Blogposts
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == blogpost.Id);

            //controleert of de blogpost niet meer bestaat
            Assert.Null(deletedBlogpost);
        }
        finally
        {
            //controleert of de testblogpost nog bestaat
            var testBlogpost = await context.Blogposts
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == blogpost.Id);

            //ruimt de blogpost op als de DELETE-test mislukt
            if (testBlogpost != null)
            {
                context.Blogposts.Remove(blogpost);
                await context.SaveChangesAsync();
            }

            //zoekt de tijdelijke gebruiker op
            var testUser = await userManager.FindByEmailAsync(email);

            //verwijdert de tijdelijke gebruiker
            if (testUser != null)
            {
                var deleteResult = await userManager.DeleteAsync(testUser);

                Assert.True(deleteResult.Succeeded);
            }
        }
    }
}