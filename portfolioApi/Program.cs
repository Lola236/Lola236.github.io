using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using portfolioApi.Data;
using portfolioApi.Services;
using portfolioApi.Models;

//maakt basis van api aan
var builder = WebApplication.CreateBuilder(args);

//controllers toevoegen zodat api die kan gebruiken
builder.Services.AddControllers();

//swagger toevoegen om api te testen
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//db verbinden met sqlserver
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

//services registreren (Dependency Injection)
//addscoped: maakt per http request een eigen service aan
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogpostService>();

//identity toevoegen en koppelen aan db entity
builder.Services.AddIdentityApiEndpoints<User>()
    .AddRoles<IdentityRole>() //rollen toevoegen
    .AddEntityFrameworkStores<AppDbContext>();

//authenticatie toevoegen
builder.Services.AddAuthentication();

//alleen editor en admin mogen schrijven
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanEditPortfolio", policy =>
        policy.RequireRole("Editor", "Admin"));

//bouwt api
var app = builder.Build();

//zet swagger aan als api in development draait
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//stuurt http verzoek naar https
app.UseHttpsRedirection();

//controleert zelf wie de gebruiker is
app.UseAuthentication();

//controleert zelf wat de gebruiker mag
app.UseAuthorization();

//controllers koppelen aan de API
app.MapControllers();

//maakt standaard rollen aan in de db
await DbSeeder.SeedAsync(app.Services);

//start api
app.Run();

//maakt Program toegankelijk voor het testproject
public partial class Program { }