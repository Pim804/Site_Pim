using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiStock.Domain.Entities;
using OptiStock.Infrastructure.Persistence;

namespace OptiStock.Web.ApiControllers;

[ApiController]
[Route("api/categorias")]
[Authorize]
public sealed class CategoriesApiController(OptiStockDbContext db) : ControllerBase
{
    public sealed record CategoryInput(string Name, string? Description);

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var item = await db.Categories.AsNoTracking().Where(x => x.Id == id && x.IsActive)
            .Select(x => new { x.Id, x.Name, x.Description }).SingleOrDefaultAsync(cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken) =>
        Ok(await db.Categories.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name, x.Description, PartCount = x.Parts.Count(p => p.IsActive) })
            .ToListAsync(cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(CategoryInput input, CancellationToken cancellationToken)
    {
        var name = input.Name.Trim();
        if (name.Length is < 2 or > 100) return BadRequest(new ProblemDetails { Status = 400, Title = "O nome deve ter de 2 a 100 caracteres." });
        if (await db.Categories.AnyAsync(x => x.Name.ToLower() == name.ToLower(), cancellationToken)) return Conflict(new ProblemDetails { Status = 409, Title = "Categoria já cadastrada." });
        var category = new Category { Name = name, Description = input.Description?.Trim() };
        db.Categories.Add(category); await db.SaveChangesAsync(cancellationToken);
        return Created($"/api/categorias/{category.Id}", new { category.Id, category.Name, category.Description });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, CategoryInput input, CancellationToken cancellationToken)
    {
        var category = await db.Categories.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (category is null) return NotFound();
        var name = input.Name.Trim();
        if (name.Length is < 2 or > 100) return BadRequest(new ProblemDetails { Status = 400, Title = "O nome deve ter de 2 a 100 caracteres." });
        if (await db.Categories.AnyAsync(x => x.Id != id && x.Name.ToLower() == name.ToLower(), cancellationToken))
            return Conflict(new ProblemDetails { Status = 409, Title = "Categoria já cadastrada." });
        category.Name = name; category.Description = input.Description?.Trim();
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var category = await db.Categories.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (category is null) return NotFound();
        if (await db.Parts.AnyAsync(x => x.CategoryId == id && x.IsActive, cancellationToken))
            return Conflict(new ProblemDetails { Status = 409, Title = "A categoria possui peças ativas e não pode ser desativada." });
        category.IsActive = false;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

