using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class LoginUserDto
{
    [Required(ErrorMessage = "The username field is required")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "The password field is required")]
    public string? Password { get; set; }
}