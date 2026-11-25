using Microsoft.AspNetCore.Identity;

namespace portfolioApi.Data;

//maakt standaard rollen aan
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        //maakt een tijdelijke scope aan
        using var scope = services.CreateScope();

        //haalt rolemanager op
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        //rollen die beschikbaar moeten zijn
        string[] roles = { "Gast", "Editor", "Admin" };

        //loopt door alle rollen
        foreach (var role in roles)
        {
            //controleert of de rol nog niet bestaat
            if (!await roleManager.RoleExistsAsync(role))
            {
                //maakt de rol aan
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                //controleert of het aanmaken is gelukt
                if (!result.Succeeded)
                {
                    throw new Exception($"Rol {role} aanmaken mislukt");
                }
            }
        }
    }
}