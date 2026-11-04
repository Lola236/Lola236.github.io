using System.ComponentModel.DataAnnotations;

namespace portfolioApi.DTOs;

public class BlogpostUpdateDto
{
    [Required]
    public string Titel { get; set; } = string.Empty;

    [Required]
    public string Inhoud { get; set; } = string.Empty;
}
