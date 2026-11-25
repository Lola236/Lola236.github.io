using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using portfolioApi.Models;

namespace portfolioApi.Tests;

//bevat automatische testen voor registratie en login
public class AuthTests
{
    //Geldige registratie

    //test of een gebruiker zich succesvol kan registreren
    [Fact]
    public async Task Register_GeldigeGegevens_Returns200()
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

        //haalt de UserManager op
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        //maakt unieke registratiegegevens aan
        var account = new
        {
            Email = $"user{Guid.NewGuid():N}@test.nl",
            Password = $"Aa1!{Guid.NewGuid():N}"
        };

        try
        {
            //Act

            //registreert de gebruiker via de API
            var response = await client.PostAsJsonAsync(
                "/api/auth/register", account);

            //Assert

            //controleert of de statuscode 200 is
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //zoekt de gebruiker op in de database
            var user = await userManager.FindByEmailAsync(
                account.Email);

            //controleert of de gebruiker echt is opgeslagen
            Assert.NotNull(user);
            Assert.Equal(account.Email, user.Email);
        }
        finally
        {
            //verwijdert de tijdelijke gebruiker
            await VerwijderGebruiker(userManager, account.Email);
        }
    }


    //Ongeldige registratie

    //test of registratie met een ongeldig wachtwoord wordt geweigerd
    [Fact]
    public async Task Register_OngeldigWachtwoord_Returns400()
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

        //haalt de UserManager op
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        //maakt registratiegegevens met een ongeldig wachtwoord
        var account = new
        {
            Email = $"user{Guid.NewGuid():N}@test.nl",
            Password = "abc"
        };

        try
        {
            //Act

            //probeert de gebruiker te registreren
            var response = await client.PostAsJsonAsync(
                "/api/auth/register", account);

            //Assert

            //controleert of de statuscode 400 is
            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            //zoekt de gebruiker op in de database
            var user = await userManager.FindByEmailAsync(
                account.Email);

            //controleert of het account niet is aangemaakt
            Assert.Null(user);
        }
        finally
        {
            //ruimt het account op als het toch is aangemaakt
            await VerwijderGebruiker(userManager, account.Email);
        }
    }


    //Gast-rol controleren

    //test of een nieuwe gebruiker automatisch de rol Gast krijgt
    [Fact]
    public async Task Register_NieuweGebruiker_KrijgtGastRol()
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

        //haalt de UserManager op
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        //maakt unieke registratiegegevens aan
        var account = new
        {
            Email = $"user{Guid.NewGuid():N}@test.nl",
            Password = $"Aa1!{Guid.NewGuid():N}"
        };

        try
        {
            //Act

            //registreert een nieuwe gebruiker
            var response = await client.PostAsJsonAsync(
                "/api/auth/register", account);

            //Assert

            //controleert of de registratie gelukt is
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //zoekt de geregistreerde gebruiker op
            var user = await userManager.FindByEmailAsync(
                account.Email);

            Assert.NotNull(user);

            //haalt de rollen van de gebruiker op
            var roles = await userManager.GetRolesAsync(user);

            //controleert of de gebruiker precies één rol heeft
            Assert.Single(roles);

            //controleert of de gebruiker de rol Gast heeft
            Assert.Contains("Gast", roles);
        }
        finally
        {
            //verwijdert de tijdelijke gebruiker
            await VerwijderGebruiker(userManager, account.Email);
        }
    }


    //Geldige login

    //test of een gebruiker met geldige gegevens kan inloggen
    [Fact]
    public async Task Login_GeldigeGegevens_Returns200()
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

        //haalt de UserManager op
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        //maakt unieke inloggegevens aan
        var account = new
        {
            Email = $"user{Guid.NewGuid():N}@test.nl",
            Password = $"Aa1!{Guid.NewGuid():N}"
        };

        try
        {
            //registreert eerst een tijdelijke gebruiker
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register", account);

            //controleert of de registratie gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                registerResponse.StatusCode);

            //Act

            //logt in met de juiste gegevens
            var response = await client.PostAsJsonAsync(
                "/api/auth/login", account);

            //Assert

            //controleert of de statuscode 200 is
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            //verwijdert de tijdelijke gebruiker
            await VerwijderGebruiker(userManager, account.Email);
        }
    }


    //Authenticatiecookie controleren

    //test of een gebruiker na het inloggen een authenticatiecookie krijgt
    [Fact]
    public async Task Login_GeldigeGegevens_OntvangtCookie()
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

        //haalt de UserManager op
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        //maakt unieke inloggegevens aan
        var account = new
        {
            Email = $"user{Guid.NewGuid():N}@test.nl",
            Password = $"Aa1!{Guid.NewGuid():N}"
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

            //Act

            //logt in met de juiste gegevens
            var response = await client.PostAsJsonAsync(
                "/api/auth/login", account);

            //Assert

            //controleert of het inloggen gelukt is
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //controleert of de response een Set-Cookie-header heeft
            Assert.True(response.Headers.Contains("Set-Cookie"));

            //haalt de cookies uit de response op
            var cookies = response.Headers.GetValues("Set-Cookie");

            //controleert of de Identity-cookie aanwezig is
            Assert.Contains(cookies, cookie =>
                cookie.StartsWith(".AspNetCore.Identity.Application="));
        }
        finally
        {
            //verwijdert de tijdelijke gebruiker
            await VerwijderGebruiker(userManager, account.Email);
        }
    }


    //Ongeldige login

    //test of inloggen met een verkeerd wachtwoord wordt geweigerd
    [Fact]
    public async Task Login_VerkeerdWachtwoord_Returns401()
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

        //haalt de UserManager op
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

        //maakt unieke registratiegegevens aan
        var account = new
        {
            Email = $"user{Guid.NewGuid():N}@test.nl",
            Password = $"Aa1!{Guid.NewGuid():N}"
        };

        //maakt inloggegevens met een verkeerd wachtwoord
        var wrongAccount = new
        {
            Email = account.Email,
            Password = $"Wrong1!{Guid.NewGuid():N}"
        };

        try
        {
            //registreert eerst een tijdelijke gebruiker
            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register", account);

            //controleert of de registratie gelukt is
            Assert.Equal(
                HttpStatusCode.OK,
                registerResponse.StatusCode);

            //Act

            //probeert in te loggen met het verkeerde wachtwoord
            var response = await client.PostAsJsonAsync(
                "/api/auth/login", wrongAccount);

            //Assert

            //controleert of de statuscode 401 is
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);

            //controleert of er geen authenticatiecookie is afgegeven
            if (response.Headers.TryGetValues(
                "Set-Cookie", out var cookies))
            {
                Assert.DoesNotContain(cookies, cookie =>
                    cookie.StartsWith(
                        ".AspNetCore.Identity.Application="));
            }
        }
        finally
        {
            //verwijdert de tijdelijke gebruiker
            await VerwijderGebruiker(userManager, account.Email);
        }
    }


    //Gebruiker opruimen

    //verwijdert tijdelijke gebruikers na het testen
    private async Task VerwijderGebruiker(
        UserManager<User> userManager, string email)
    {
        //zoekt de tijdelijke gebruiker op
        var user = await userManager.FindByEmailAsync(email);

        //controleert of de gebruiker bestaat
        if (user != null)
        {
            //verwijdert de gebruiker
            var result = await userManager.DeleteAsync(user);

            //controleert of het verwijderen gelukt is
            Assert.True(result.Succeeded);
        }
    }
}