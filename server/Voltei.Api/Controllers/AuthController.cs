using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voltei.Api.Data;
using Voltei.Api.Dto;
using Voltei.Api.Models;
using Voltei.Api.Services;

namespace Voltei.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, TokenService tokenService) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> Signup(SignupRequest request)
    {
        if (await db.Users.AnyAsync(u => u.Email == request.Email))
            return Conflict(new { message = "Email já cadastrado." });

        var business = new Business
        {
            Id = Guid.NewGuid(),
            Nome = request.NomeNegocio,
            Email = request.Email,
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Nome = request.NomeNegocio,
            NegocioId = business.Id,
        };

        db.Businesses.Add(business);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = tokenService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            User = new AuthUserDto
            {
                Id = user.Id.ToString(),
                Nome = user.Nome,
                Email = user.Email,
                NegocioId = user.NegocioId.ToString(),
            }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.SenhaHash))
            return Unauthorized(new { message = "Email ou senha incorretos." });

        var token = tokenService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            User = new AuthUserDto
            {
                Id = user.Id.ToString(),
                Nome = user.Nome,
                Email = user.Email,
                NegocioId = user.NegocioId.ToString(),
            }
        });
    }
}
