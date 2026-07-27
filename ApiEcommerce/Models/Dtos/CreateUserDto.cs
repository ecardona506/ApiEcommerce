using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class CreateUserDto
{
    [Required(ErrorMessage = "The username field is required")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "The name field is required")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "The password field is required")]
    public string? Password { get; set; }
    [Required(ErrorMessage = "The role field is required")]
    public string? Role { get; set; }
}