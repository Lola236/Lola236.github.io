
using Microsoft.EntityFrameworkCore;
using portfolioApi.Data;
using portfolioApi.DTOs;
using portfolioApi.Models;

namespace portfolioApi.Services;

//service die d acties voor projecten uitvoert
public class ProjectService
{
    //bewaart de toegang tot de db
    private readonly AppDbContext _context;

    //ontvangt de db context via dependency injection
    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    //GET: alle projecten ophalen
    //haalt alle projecten asynchroon op en geeft ze terug als een lijst met DTO's
    public async Task<List<ProjectReadDto>> GetProjects()
    {
        //haalt alle projecten op uit de db
        return await _context.Projects
            //zet elk project om naar een ProjectReadDto
            .Select(project => new ProjectReadDto
            {
                Id = project.Id,
                Titel = project.Titel,
                Beschrijving = project.Beschrijving,
                Categorie = project.Categorie,
                GitHubUrl = project.GitHubUrl,
                Datum = project.Datum
            })
            //zet de resultaten om naar een lijst
            .ToListAsync();
    }

    //GET: 1 project ophalen via id
  
    //zoekt één project op basis van het ID en geeft de projectgegevens terug
    public async Task<ProjectReadDto?> GetProjectById(int id)
    {
        return await _context.Projects
            //zoekt het project met het opgegeven id
            .Where(project => project.Id == id)
            //zet het project om naar een ProjectReadDto
            .Select(project => new ProjectReadDto
            {
                Id = project.Id,
                Titel = project.Titel,
                Beschrijving = project.Beschrijving,
                Categorie = project.Categorie,
                GitHubUrl = project.GitHubUrl,
                Datum = project.Datum
            })
            //geeft het eerste resultaat terug of null als er geen resultaat is
            .FirstOrDefaultAsync();
    }

    //POST: nieuw project toevoegen

    //maakt  nieuw project aan met de ontvangen gegevens en geeft het aangemaakte project terug
    public async Task<ProjectReadDto> CreateProject(ProjectCreateDto dto)
    {
        //maakt een nieuw project aan met de ontvangen gegevens 
        var project = new Project
        {
            Titel = dto.Titel,
            Beschrijving = dto.Beschrijving,
            Categorie = dto.Categorie,
            GitHubUrl = dto.GitHubUrl,
            Datum = dto.Datum
        };

        //voegt het nieuwe project toe aan de db context
        _context.Projects.Add(project);
        //slaat nieuew project toe in de db
        await _context.SaveChangesAsync();

        //geeft het aangemaakte project terug als een ProjectReadDto
        return new ProjectReadDto
        {
            Id = project.Id,
            Titel = project.Titel,
            Beschrijving = project.Beschrijving,
            Categorie = project.Categorie,
            GitHubUrl = project.GitHubUrl,
            Datum = project.Datum
        };
    }

    //PUT: Bestaand project wijzigen

    //past project aan op basis van ID zoekt project in db, met ProjectUpdateDto ontvang nieuwe gegevensen geeft true of false terug
    public async Task<bool> UpdateProject(int id, ProjectUpdateDto dto)
    {
        //zoekt project op in de db op id
        var project = await _context.Projects.FindAsync(id);
        //controleert of project bestaat
        if (project == null)
        {
            //geeft false terug als project niet bestaat
            return false;
        }
        //vervangt oude gegevens door nieuwe gegevens uit de DTO
        project.Titel = dto.Titel;
        project.Beschrijving = dto.Beschrijving;
        project.Categorie = dto.Categorie;
        project.GitHubUrl = dto.GitHubUrl;
        project.Datum = dto.Datum;

        //slaat de wijzigingen op in de db
        await _context.SaveChangesAsync();
        //geeft true terug als project is aangepast
        return true;
    }

    //DELETE: Project verwijderen
    //verwijdert project op basis van ID zoekt project in db, verwijdert project en geeft true of false terug
    public async Task<bool> DeleteProject(int id)
    {
        //zoekt project op in de db op id
        var project = await _context.Projects.FindAsync(id);

        //controleert of project bestaat
        if (project == null)
        {
            //geeft false terug als project niet bestaat
            return false;
        }

        //geeft aan dat project verwijderd wordt uit de db
        _context.Projects.Remove(project);
        //voert verwijdering uit in de db
        await _context.SaveChangesAsync();
        //geeft true terug als project is verwijderd
        return true;
    }
}
