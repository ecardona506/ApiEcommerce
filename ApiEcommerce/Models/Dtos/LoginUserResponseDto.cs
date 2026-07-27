namespace ApiEcommerce.Models.Dtos;

public class LoginUserResponseDto
{
    public RegisterUserDto? User { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}