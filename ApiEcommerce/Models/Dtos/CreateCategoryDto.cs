using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;
public class CreateCategoryDto
{
    [Required(ErrorMessage = "The name is required")]
    [MinLength(3, ErrorMessage="The name must be at least 3 characters long")]
    [MaxLength(50, ErrorMessage="The name must be at most 50 characters long")]
    public string Name { get; set; } = string.Empty;
}