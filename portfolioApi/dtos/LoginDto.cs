using System.ComponentModel.DataAnnotations;

namespace portfolioApi.DTOs;

//gegevens voor het inloggen
public class LoginDto
{
    //verplicht en geldig e-mailadres
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    //verplicht wachtwoord
    [Required]
    public string Password { get; set; } = string.Empty;
}