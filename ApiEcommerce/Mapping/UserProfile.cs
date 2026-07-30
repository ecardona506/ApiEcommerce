using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, RegisterUserDto>().ReverseMap();
        CreateMap<User, LoginUserDto>().ReverseMap();
        CreateMap<User, LoginUserResponseDto>().ReverseMap();
    }
}
