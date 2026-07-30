using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<User> GetUsers();
    User? GetUser(int id);
    bool IsUserUnique(string username);
    Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto);
    Task<User> Register(CreateUserDto createUserDto);
}
