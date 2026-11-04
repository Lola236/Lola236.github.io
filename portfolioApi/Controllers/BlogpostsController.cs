using Microsoft.AspNetCore.Mvc;
using portfolioApi.DTOs;
using portfolioApi.Services;

namespace portfolioApi.Controllers;

[ApiController]
//alle routes van deze controller beginnen met api/blogposts
[Route("api/blogposts")]

//maakt blogpostcontroller aan
//controllerbase: geeft notfound, ok, created, no content terug
public class BlogpostsController : ControllerBase
{
    //bewaart de service die de controller gebruikt
    //private readonly: na aanmaken kan niet zmr veranderen
    private readonly BlogpostService _blogpostService;

    //krijgt de blogpostservice automatisch via dependency injection
    public BlogpostsController(BlogpostService blogpostService)
    {
        //bewaart de service die de controller gebruikt
        _blogpostService = blogpostService;
    }

    //GET Alle blogposts ophalen
    [HttpGet]
    //haalt alle blogposts op via service en stuurt lijst met blogpostreaddto terug met statuscode 200 (OK)
    public async Task<ActionResult<List<BlogpostReadDto>>> GetBlogposts()
    {
        //vraagt alle blogposts op via de service
        var blogposts = await _blogpostService.GetBlogposts();
        //stuurt lijst met blogposts terug met statuscode 200 (OK)
        return Ok(blogposts);
    }

    //GET 1 blogpost ophalen via ID
    [HttpGet("{id}")]

    //ontvangt id en haalt één blogpost op via service en stuurt blogpostreaddto terug met statuscode 200 (OK)
    public async Task<ActionResult<BlogpostReadDto>> GetBlogpostById(int id)
    {
        //zoekt blogpost op via service
        var blogpost = await _blogpostService.GetBlogpostById(id);
        //controleert of blogpost bestaat
        if (blogpost == null)
        {
            //geeft 404 terug als blogpost niet bestaat
            return NotFound();
        }
        //geeft 200 terug met gevonden blogpost
        return Ok(blogpost);
    }

    //POST Nieuwe blogpost toevoegen
    [HttpPost]
    //ontvangt blogpostgegevens en voegt nieuwe blogpost toe via service en stuurt blogpostreaddto terug met statuscode 201 (Created)
    //[FromBody]: leest gegevens uit http request body en zet die om naar blogpostcreatedto
    public async Task<ActionResult<BlogpostReadDto>> CreateBlogpost(
        [FromBody] BlogpostCreateDto dto)
    {
        //vraagt service om nieuwe blogpost aan te maken/opslaan
        var blogpost = await _blogpostService.CreateBlogpost(dto);
        //stuurt 201 terug met de locatie van de nieuwe blogpost en de blogpostgegevens
        return Created($"/api/blogposts/{blogpost.Id}", blogpost);
    }

    //PUT Blogpost wijzigen
    [HttpPut("{id}")]
    //ontvangt id en blogpostgegevens en wijzigt bestaande blogpost via service en stuurt statuscode 204 (No Content) terug
    public async Task<IActionResult> UpdateBlogpost(
        int id, [FromBody] BlogpostUpdateDto dto)
    {
        //vraagt service om blogpost te wijzigen
        //updateblogpost geeft true terug als blogpost is aangepast, false als blogpost niet bestaat
        var updated = await _blogpostService.UpdateBlogpost(id, dto);
        //controleert of aanpassen niet gelukt is
        if (!updated)
        { 
            //404 teruggeven als blogpost niet bestaat
            return NotFound();
        }
        //204 teruggeven als blogpost is aangepast
        return NoContent();
    }

    //DELETE Blogpost verwijderen
    [HttpDelete("{id}")]
    //ontvangt id en verwijdert blogpost via service en stuurt statuscode 204 (No Content) terug
    public async Task<IActionResult> DeleteBlogpost(int id)
    {
        //vraagt service om blogpost te verwijderen
        var deleted = await _blogpostService.DeleteBlogpost(id);
        //controleert of het blogpost niet bestaat
        if (!deleted)
        {
            //404 teruggeven als blogpost niet bestaat
            return NotFound();
        }
        //204 teruggeven als blogpost is verwijderd
        return NoContent();
    }
}
