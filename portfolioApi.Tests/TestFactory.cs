using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace portfolioApi.Tests;

//configureert de API voor automatische testen
public class TestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        //stelt de testomgeving in
        builder.UseEnvironment("Testing");

        //gebruikt de aparte testdatabase
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        @"Server=LENOOVOO\SQLEXPRESS;Database=PortfolioTestDb;Trusted_Connection=True;TrustServerCertificate=True;"
                });
        });
    }
}