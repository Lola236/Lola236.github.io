using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using portfolioApi.Models;
using portfolioApi.DTOs;

namespace portfolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    //ontvangt identity managers via DI
    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    //registreert een nieuwe gebruiker
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        //maakt een nieuwe gebruiker aan!
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        //slaat de gebruiker op in de db
        var result = await _userManager.CreateAsync(
            user, dto.Password);

        //controleert of registratie is gelukt
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        //geeft de gebruiker de rol gast!!
        var roleResult = await _userManager.AddToRoleAsync(
            user, "Gast");

        //controleert of roltoekenning is gelukt
        if (!roleResult.Succeeded)
        {
            //probeert het aangemaakte account te verwijderen
            var deleteResult = await _userManager.DeleteAsync(user);

            //controleert of het verwijderen is gelukt
            if (!deleteResult.Succeeded)
            {
                return StatusCode(500, "Account verwijderen mislukt");
            }

            return StatusCode(500, "Roltoekenning mislukt");
        }

        //registratie succesvol
        return Ok("Registratie gelukt");
    }

    //laat een gebruiker inloggen
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        //gebruikt cookies voor het inloggen
        _signInManager.AuthenticationScheme =
            IdentityConstants.ApplicationScheme;

        //controleert email en wachtwoord
        var result = await _signInManager.PasswordSignInAsync(
            dto.Email,
            dto.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        //controleert of het inloggen is gelukt
        if (!result.Succeeded)
        {
            return Unauthorized("Ongeldige inloggegevens");
        }

        //login succesvol
        return Ok("Login gelukt");
    }
}