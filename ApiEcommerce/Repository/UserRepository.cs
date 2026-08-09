using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    public readonly ApplicationDbContext _db;
    private string? secretKey;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext db,
                        IConfiguration configuration,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole> roleManager,
                        IMapper mapper)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }
    public User? GetUser(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<User> GetUsers()
    {
        return _db.Users.OrderBy(u => u.UserName).ToList();
    }

    public bool IsUserUnique(string username)
    {
        return !_db.Users.Any(u => u.UserName.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<LoginUserResponseDto> Login(LoginUserDto loginUserDto)
    {
        if (string.IsNullOrEmpty(loginUserDto.UserName))
        {
            return new LoginUserResponseDto()
            {
                Token = "",
                User = null,
                Message = "Username is required"
            };
        }
        var user = await _db.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == loginUserDto.UserName.ToLower().Trim());
        if (user == null)
        {
            return new LoginUserResponseDto()
            {
                Token = "",
                User = null,
                Message = "User not found"
            };
        }
        if (loginUserDto.Password == null)
        {
            return new LoginUserResponseDto()
            {
                Token = "",
                User = null,
                Message = "Password required"
            };
        }
        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
        if (!isPasswordValid)
        {
            return new LoginUserResponseDto()
            {
                Token = "",
                User = null,
                Message = "Invalid credentials"
            };
        }
        var handlerToken = new JwtSecurityTokenHandler();
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("SecretKey is not set");
        }
        var roles = await _userManager.GetRolesAsync(user);
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(new[]
          {
              new Claim("id", user.Id.ToString()),
              new Claim("username", user.UserName ?? string.Empty),
              new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty)
          }
          ),
          Expires = DateTime.UtcNow.AddHours(2),
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = handlerToken.CreateToken(tokenDescriptor);
        return new LoginUserResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = _mapper.Map<UserDataDto>(user),
            Message = "User logged succesfully"
        };
    }   

    public async Task<User> Register(CreateUserDto createUserDto)
    {
        var encriptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = new User()
        {
            UserName = createUserDto.UserName,
            Name = createUserDto.Name,
            Password = encriptedPassword,
            Role = createUserDto.Role
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }
}
