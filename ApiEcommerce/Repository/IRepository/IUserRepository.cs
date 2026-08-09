using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<ApplicationUser> GetUsers();
    ApplicationUser? GetUser(string id);
    bool IsUserUnique(string username);
    Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto);
    Task<UserDataDto> Register(CreateUserDto createUserDto);
}
