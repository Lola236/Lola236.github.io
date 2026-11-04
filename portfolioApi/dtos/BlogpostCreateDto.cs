using System.ComponentModel.DataAnnotations;

namespace portfolioApi.DTOs;

public class BlogpostCreateDto
{
    //titel is verplicht bij het aanmaken van een blogpost
    [Required]
    public string Titel { get; set; } = string.Empty;

    [Required]
    public string Inhoud { get; set; } = string.Empty;
}
