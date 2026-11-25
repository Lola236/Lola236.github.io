using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using portfolioApi.Data;

namespace portfolioApi.Tests;

public class DatabaseTests
{
    [Fact]
    public void Database_GebruiktTestDatabase()
    {
        //Arrange
        using var factory = new TestFactory();
        using var scope = factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        //Act
        var databaseNaam = context.Database
            .GetDbConnection().Database;

        //Assert
        Assert.Equal("PortfolioTestDb", databaseNaam);
    }
}