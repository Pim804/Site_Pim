using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using OptiStock.Infrastructure.Auth;

namespace OptiStock.Web.ApiControllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(UserManager<AppUser> users, SignInManager<AppUser> signIn, IConfiguration configuration) : ControllerBase
{
    public sealed record LoginRequest(string Email, string Password);
    public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAtUtc, Guid UserId, string DisplayName, IReadOnlyList<string> Roles);

    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await users.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive || !(await signIn.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true)).Succeeded)
            return Unauthorized(new ProblemDetails { Status = 401, Title = "Credenciais inválidas." });

        var roles = await users.GetRolesAsync(user);
        var expires = DateTimeOffset.UtcNow.AddMinutes(30);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new("security_stamp", await users.GetSecurityStampAsync(user)),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(configuration["Jwt:Issuer"] ?? "OptiStock",
            configuration["Jwt:Audience"] ?? "OptiStockClients", claims, DateTime.UtcNow, expires.UtcDateTime, credentials);
        return Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expires, user.Id, user.DisplayName, roles.ToArray()));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var user = await users.FindByIdAsync(userId.ToString());
        if (user is null) return Unauthorized();
        await users.UpdateSecurityStampAsync(user);
        return NoContent();
    }
}
