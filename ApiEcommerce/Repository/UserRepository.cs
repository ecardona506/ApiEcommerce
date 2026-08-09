using System;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
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
    public ApplicationUser? GetUser(string id)
    {
        return _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<ApplicationUser> GetUsers()
    {
        return _db.ApplicationUsers.OrderBy(u => u.UserName).ToList();
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

    public async Task<UserDataDto> Register(CreateUserDto createUserDto)
    {
        if (string.IsNullOrEmpty(createUserDto.UserName))
        {
            throw new ArgumentNullException("Username is required");
        }
        if (createUserDto.Password == null)
        {
            throw new ArgumentNullException("Password is required");
        }
        var user = new ApplicationUser()
        {
            UserName = createUserDto.UserName,
            Email = createUserDto.UserName,
            NormalizedEmail = createUserDto.UserName.Trim().ToLower(),
            Name = createUserDto.Name
        };
        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (result.Succeeded)
        {
            var userRole = createUserDto.Role ?? "User";
            var roleExists = await _roleManager.RoleExistsAsync(userRole);
            if (!roleExists)
            {
                var identityRole = new IdentityRole(userRole);
                await _roleManager.CreateAsync(identityRole);
            }
            await _userManager.AddToRoleAsync(user, userRole);
            var createdUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == createUserDto.UserName);
            return _mapper.Map<UserDataDto>(createdUser);
        }
        throw new ApplicationException("User couldn't be registered");
    }
}
