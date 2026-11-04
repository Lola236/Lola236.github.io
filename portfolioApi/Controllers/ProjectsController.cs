using Microsoft.AspNetCore.Mvc;
using portfolioApi.DTOs;
using portfolioApi.Services;

namespace portfolioApi.Controllers;

[ApiController]
//alle routes van deze controller beginnen met api/projects
[Route("api/projects")]

//maakt projectcontroller aan
//controllerbase: geeft notfound, ok, created, no content terug
public class ProjectsController : ControllerBase
{
    //bewaart de service die de controller gebruikt
    //private readonly: na aanmaken kan niet zmr veranderen
    private readonly ProjectService _projectService;

//krijgt de projectservice automatisch via dependency injection
    public ProjectsController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    //GET: Alle projecten ophalen
    [HttpGet]

    //haalt alle projecten op via service en stuurt lijst met projectreaddto terug met statuscode 200 (OK)
    public async Task<ActionResult<List<ProjectReadDto>>> GetProjects()
    {
        //vraagt alle projecten op via de service
        var projecten = await _projectService.GetProjects();

        return Ok(projecten);
    }

    //GET: 1 project ophalen via ID
    [HttpGet("{id}")]

    //ontvangt id en haalt één project op via service en stuurt projectreaddto terug met statuscode 200 (OK)
    public async Task<ActionResult<ProjectReadDto>> GetProjectById(int id)
    {
        //zoekt project op via service
        var project = await _projectService.GetProjectById(id);

        //controleert of project bestaat
        if (project == null)
        {
            //geeft 404 terug
            return NotFound();
        }
        //geeft 200 terug met gevonden project
        return Ok(project);
    }

    //POST:nieuw project toevoegen
    [HttpPost]

    //ontvangt projectgegevens en voegt nieuw project toe via service en stuurt projectreaddto terug met statuscode 201 (Created)
    //[FromBody]: leest gegevens uit http request body en zet die om naar projectcreatedto
    //projectcreatedto: bevat de gegevens van het nieuwe project
    public async Task<ActionResult<ProjectReadDto>> CreateProject(
        [FromBody] ProjectCreateDto dto)
    {
        //stuurt gegevens naar service om project op te slaan
        var project = await _projectService.CreateProject(dto);

        //stuurt 201 terug met locatie van nieuw project en projectgegevens
        return Created($"/api/projects/{project.Id}", project);
    }

    //PUT:bestaand project wijzigen
    [HttpPut("{id}")]

    //ontvang id en nieuwe projectgegevens en wijzigt project via service en stuurt statuscode 204 (No Content) terug
    public async Task<IActionResult> UpdateProject(
        int id, [FromBody] ProjectUpdateDto dto)
    {
        //vraagt service om project te wijzigen
        //updateproject geeft true terug als project is gewijzigd, false als project niet bestaat
        var updated = await _projectService.UpdateProject(id, dto);

        //controleert of aanpassen niet gelukt is
        if (!updated)
        {
            //404 teruggeven als project niet bestaat
            return NotFound();
        }
        //204 teruggeven als project is aangepast
        return NoContent();
    }

    //DELETE: Project verwijderen
    [HttpDelete("{id}")]

    //ontvangt id en verwijdert project via service en stuurt statuscode 204 (No Content) terug
    public async Task<IActionResult> DeleteProject(int id)
    {
        //vraagt service om project te verwijderen
        //deleteproject geeft true terug als project is verwijderd, false als project niet bestaat
        var deleted = await _projectService.DeleteProject(id);

        //controleert of het project niet gevonden is
        if (!deleted)
        {
            //404 teruggeven als project niet bestaat
            return NotFound();
        }
        //204 teruggeven als project is verwijderd
        return NoContent();
    }
}
