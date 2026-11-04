
using Microsoft.EntityFrameworkCore;
using portfolioApi.Data;
using portfolioApi.DTOs;
using portfolioApi.Models;

namespace portfolioApi.Services;

//service die acties voor blogposts uitvoert
public class BlogpostService
{
    //bewaart de toegang tot de db
    private readonly AppDbContext _context;
    //ontvangt de db context via dependency injection
    public BlogpostService(AppDbContext context)
    {
        _context = context;
    }

    //GET, alle blogposts ophalen
    //haalt alle blogposts asynchroon op en geeft ze terug als een lijst met DTO's
    public async Task<List<BlogpostReadDto>> GetBlogposts()
    {
        //haalt alle blogposts op uit de db
        return await _context.Blogposts
            //zet elk blogpost om naar een BlogpostReadDto
            .Select(blogpost => new BlogpostReadDto
            {
                Id = blogpost.Id,
                Titel = blogpost.Titel,
                Inhoud = blogpost.Inhoud,
                Publicatiedatum = blogpost.Publicatiedatum
            })
            //zet de resultaten om naar een lijst
            .ToListAsync();
    }

    //GET, 1 blogpost ophalen
    //zoekt een blogpost op ID en geeft een ReadDto of null terug
    public async Task<BlogpostReadDto?> GetBlogpostById(int id)
    {
        return await _context.Blogposts
            //zoekt de blogpost met het opgegeven id
            .Where(blogpost => blogpost.Id == id)
            //zet de gevonden blogpost om naar een BlogpostReadDto
            .Select(blogpost => new BlogpostReadDto
            {
                Id = blogpost.Id,
                Titel = blogpost.Titel,
                Inhoud = blogpost.Inhoud,
                Publicatiedatum = blogpost.Publicatiedatum
            })
            //geeft eeerste blogpost terug of null als er geen blogpost is gevonden
            .FirstOrDefaultAsync();
    }

    //POST, Nieuwe blogpost toevoegen
    //maakt een nieuwe blogpost aan en geeft de gegevens terug als ReadDto
    public async Task<BlogpostReadDto> CreateBlogpost(BlogpostCreateDto dto)
    {
        //maakt nieuwe blogpost aan
        var blogpost = new Blogpost
        {
            //neemt gegegevns over uit db
            Titel = dto.Titel,
            Inhoud = dto.Inhoud,
            Publicatiedatum = DateTime.UtcNow
        };
        //voegt nieuwe blogpost toe aan db context
        _context.Blogposts.Add(blogpost);
        //slaat de nieuwe blogpost op in de db
        await _context.SaveChangesAsync();
        //geeft aangemaakte blogpost terug als BlogpostReadDto
        return new BlogpostReadDto
        {
            Id = blogpost.Id,
            Titel = blogpost.Titel,
            Inhoud = blogpost.Inhoud,
            Publicatiedatum = blogpost.Publicatiedatum
        };
    }

    //PUT, bestaande blogpost wijzigen
    //past een blogpost aan op basis van ID en geeft true of false terug
    public async Task<bool> UpdateBlogpost(int id, BlogpostUpdateDto dto)
    {
        //zoekt blogpost op in de db op basis van id
        var blogpost = await _context.Blogposts.FindAsync(id);
        //controleert of blogpost bestaat
        if (blogpost == null)
        {
            //geeft false terug als blogpost niet bestaat
            return false;
        }
        //vervangt oude gegevens door nieuwe gegevens 
        blogpost.Titel = dto.Titel;
        blogpost.Inhoud = dto.Inhoud;

        //slaat de wijzigingen op in de db
        await _context.SaveChangesAsync();
        //geeft true terug als blogpost is aangepast
        return true;
    }

    //DELETE, Blogpost verwijderen
    //verwijdert een blogpost op basis van ID en geeft true of false terug
    public async Task<bool> DeleteBlogpost(int id)
    {
        //zoekt blogpost op in de db op id
        var blogpost = await _context.Blogposts.FindAsync(id);
        //controleert of blogpost bestaat
        if (blogpost == null)
        {
            //geeft false terug als blogpost niet bestaat
            return false;
        }
        //geeft aan dat blogpost verwijderd moet worden uit de db
        _context.Blogposts.Remove(blogpost);
        //voert de verwijdering uit in de db
        await _context.SaveChangesAsync();
        //geeft true terug als blogpost is verwijderd
        return true;
    }
}
