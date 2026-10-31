using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiStock.Infrastructure.Auth;
using OptiStock.Infrastructure.Persistence;

namespace OptiStock.Web.ApiControllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Administrador")]
public sealed class UsersApiController(UserManager<AppUser> users, OptiStockDbContext db) : ControllerBase
{
    public sealed record CreateUserRequest(string DisplayName, string Email, string Password, string Role);
    public sealed record UpdateUserRequest(string DisplayName, string? Email, bool IsActive, string Role);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await users.FindByIdAsync(id.ToString());
        return user is null ? NotFound() : Ok(new { user.Id, user.DisplayName, user.Email, user.IsActive, Roles = await users.GetRolesAsync(user) });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? busca, [FromQuery] int page = 1, [FromQuery] int pageSize = 25)
    {
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 100);
        var query = users.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(busca)) query = query.Where(x => x.DisplayName.Contains(busca) || x.Email!.Contains(busca));
        var total = await query.CountAsync();
        var rows = await query.OrderBy(x => x.DisplayName).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var items = new List<object>();
        foreach (var user in rows) items.Add(new { user.Id, user.DisplayName, user.Email, user.IsActive, Roles = await users.GetRolesAsync(user) });
        return Ok(new { items, page, pageSize, total });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var role = NormalizeRole(request.Role);
        if (role is null) return BadRequest(new ProblemDetails { Status = 400, Title = "Perfil deve ser Administrador ou Funcionário." });
        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Trim().Length > 120 || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new ProblemDetails { Status = 400, Title = "Nome e e-mail são obrigatórios; o nome deve ter até 120 caracteres." });
        var user = new AppUser { UserName = request.Email.Trim(), Email = request.Email.Trim(), DisplayName = request.DisplayName.Trim(), EmailConfirmed = true };
        var result = await users.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(new ValidationProblemDetails(result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())));
        var roleResult = await users.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            await users.DeleteAsync(user);
            return Problem(statusCode: 500, title: "Não foi possível atribuir o perfil ao usuário.");
        }
        return Created($"/api/usuarios/{user.Id}", new { user.Id, user.DisplayName, user.Email, user.IsActive, Role = role });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest request)
    {
        var user = await users.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();
        var role = NormalizeRole(request.Role);
        if (role is null) return BadRequest(new ProblemDetails { Status = 400, Title = "Perfil inválido." });
        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Trim().Length > 120)
            return BadRequest(new ProblemDetails { Status = 400, Title = "Nome é obrigatório e deve ter até 120 caracteres." });
        if (id.ToString() == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value &&
            (!request.IsActive || role != "Administrador"))
            return Conflict(new ProblemDetails { Status = 409, Title = "O administrador autenticado não pode desativar ou rebaixar a própria conta." });
        user.DisplayName = request.DisplayName.Trim(); user.IsActive = request.IsActive;
        if (request.Email is not null) { user.Email = request.Email.Trim(); user.UserName = request.Email.Trim(); }
        var result = await users.UpdateAsync(user);
        if (!result.Succeeded) return BadRequest(new ValidationProblemDetails(result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())));
        var existing = await users.GetRolesAsync(user);
        if (existing.Count > 0) await users.RemoveFromRolesAsync(user, existing);
        await users.AddToRoleAsync(user, role);
        await users.UpdateSecurityStampAsync(user);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var user = await users.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();
        if (id.ToString() == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            return Conflict(new ProblemDetails { Status = 409, Title = "Você não pode excluir a própria conta administrativa." });
        var hasMovements = await db.StockMovements.AnyAsync(x => x.UserId == id, cancellationToken);
        var hasAnnouncements = await db.Announcements.AnyAsync(x => x.AuthorId == id, cancellationToken);
        if (hasMovements || hasAnnouncements)
            return Conflict(new ProblemDetails { Status = 409, Title = "Esta conta possui registros históricos vinculados e não pode ser excluída. Desative-a para bloquear o acesso e preservar a auditoria." });
        var result = await users.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(new ValidationProblemDetails(result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())));
        return NoContent();
    }
    private static string? NormalizeRole(string role) => role?.Trim() switch
    {
        "Administrador" => "Administrador",
        "Funcionário" => "Funcionário",
        _ => null
    };
}

