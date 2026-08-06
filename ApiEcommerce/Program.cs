using Microsoft.EntityFrameworkCore;
using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Repository;
using ApiEcommerce.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("Secret Key is not set");
}
// Add services to the container.

builder.Services.AddControllers(option =>
{
    option.CacheProfiles.Add("Default10", new CacheProfile()
    {
        Duration = 10
    });
    option.CacheProfiles.Add("Default20", new CacheProfile()
    {
        Duration = 20
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024;
    options.UseCaseSensitivePaths = true;
});
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));

builder.Services.AddAuthentication(options =>
{ 
   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
      ValidateIssuer = false,
      ValidateAudience = true
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
   options.AddPolicy(CorsPolicyNames.AllowSpecificOrigin, builder =>
   {
       builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
   }); 
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicyNames.AllowSpecificOrigin);

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
