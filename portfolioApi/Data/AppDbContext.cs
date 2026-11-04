using Microsoft.EntityFrameworkCore;
using portfolioApi.Models;

namespace portfolioApi.Data;

//regelt communicatie met db
public class AppDbContext : DbContext
{
    //ontvangt db instellingen via DI
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) //geeft instellingen door aan DbContext
    {
    }

    //maakt mogelijk om met projects en blogposts te werken in de db
    public DbSet<Project> Projects { get; set; }

    public DbSet<Blogpost> Blogposts { get; set; }
}