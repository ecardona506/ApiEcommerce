using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public class UserProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>().TwoWays();
        config.NewConfig<User, RegisterUserDto>().TwoWays();
        config.NewConfig<User, LoginUserDto>().TwoWays();
        config.NewConfig<User, LoginUserResponseDto>().TwoWays();
        config.NewConfig<ApplicationUser, UserDataDto>().TwoWays();
        config.NewConfig<ApplicationUser, UserDto>().TwoWays();
    }
}
