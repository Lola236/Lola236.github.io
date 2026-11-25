using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using portfolioApi.Models;

namespace portfolioApi.Data;

//regelt communicatie met db en identity
public class AppDbContext : IdentityDbContext<User>
{
    //ontvangt db instellingen via DI
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) //geeft instellingen door aan dbcontext
    {
    }

    //maakt mogelijk om met projects en blogposts te werken in de db
    public DbSet<Project> Projects { get; set; }

    public DbSet<Blogpost> Blogposts { get; set; }
}