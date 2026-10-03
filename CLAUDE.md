# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ASP.NET Core 8 Web API (single project `ApiEcommerce/`, solution `ApiEcommerce.slnx`) for managing categories, products and users. SQL Server via EF Core, ASP.NET Identity + JWT auth, AutoMapper, Asp.Versioning, Swagger. There is no test project.

## Commands

Run from the repo root unless noted.

```bash
# Start SQL Server 2022 (reads DATABASE_PASSWORD from .env at repo root)
docker compose up -d

dotnet build
dotnet run --project ApiEcommerce            # http://localhost:5097, Swagger at /swagger
dotnet run --project ApiEcommerce --launch-profile https   # https://localhost:7297

# EF Core migrations (run inside ApiEcommerce/ or pass --project ApiEcommerce)
dotnet ef migrations add <Name> --project ApiEcommerce
dotnet ef database update --project ApiEcommerce
```

`ApiEcommerce/ApiEcommerce.http` holds sample requests.

## Configuration

- `ConnectionStrings:DefaultConnection` is **not** in appsettings — it comes from .NET user secrets (`UserSecretsId` in the csproj). Set it with `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<conn>" --project ApiEcommerce`.
- `ApiSettings:SecretKey` (JWT signing key) is read in `Program.cs` and `UserRepository`; startup throws if missing.

## Architecture

Request flow: **Controller → Repository (interface in `Repository/IRepository/`) → `ApplicationDbContext`**, with AutoMapper profiles in `Mapping/` converting entities ↔ DTOs (`Models/Dtos/`). Repositories are registered as scoped in `Program.cs`; AutoMapper picks up all `Profile` classes in the assembly automatically. Repositories return entities and `bool` success flags (`Save()` pattern); controllers do validation, populate `ModelState` with `"CustomError"` keys, and map to DTOs.

- **Data / seeding**: `ApplicationDbContext` extends `IdentityDbContext<ApplicationUser>`. Seed data (roles `Admin`/`User`, users `admin@admin.com`/`Admin123!` and `user@user.com`/`User123!`, categories, products) lives in `Data/DataSeeder.cs` and runs through EF Core's `UseSeeding` hook configured in `Program.cs` (fires on `database update` / `EnsureCreated`), not via `HasData` in migrations.
- **Users**: Identity's `ApplicationUser` is the real user model (login/register in `UserRepository` via `UserManager`/`RoleManager`, JWT with a single `Role` claim, 2h expiry). `Models/User.cs` / `DbSet<User> Users` is a legacy pre-Identity table — note `IsUserUnique` still queries it.
- **Auth**: Controllers are `[Authorize(Roles = "Admin")]` at class level; public endpoints opt out with `[AllowAnonymous]`.
- **Versioning**: Routes are `api/v{version:apiVersion}/[controller]`, default version 1.0. `CategoryController` exists in `Controllers/V1` and `Controllers/V2` (same class name, different namespaces, `[ApiVersion("1.0")]`/`[ApiVersion("2.0")]`); `ProductController` and `UserController` are `[ApiVersionNeutral]`. Adding a new version requires a matching `SwaggerDoc`/`SwaggerEndpoint` in `Program.cs`.
- **Caching**: Response caching profiles are named constants in `Constants/CacheProfiles.cs` and used via `[ResponseCache(CacheProfileName = ...)]`. CORS policy name is in `Constants/CorsPolicyNames.cs`.
- **Pagination**: `GET api/v1/Product/Paginated?pageNumber=&pageSize=` returns `PaginationResponse<T>` (`Models/Dtos/Responses/`); repository exposes `GetPaginatedProducts` + `GetTotalProducts`.
- **Product images**: Create/Update accept `[FromForm]` DTOs with an optional `Image` file, saved to `<cwd>/Static/ProductsImages/` with `ImageUrl` (public URL) and `ImageUrlLocal` (disk path) stored on the product.
