using Microsoft.EntityFrameworkCore;
using portfolioApi.Data;
using portfolioApi.Services;

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

//services registreren, beschikbaar maken voor de controllers (Dependency Injection)
//addscoped:maakt per HTTP-request een eigen service aan
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogpostService>();

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

//zorgt ervoor dat de api alleen toegankelijk is voor geautoriseerde gebruikers
app.UseAuthorization();

//controllers koppelen aan de API
app.MapControllers();

//start api
app.Run();
