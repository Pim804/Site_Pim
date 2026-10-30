using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiStock.Domain.Entities;
using OptiStock.Domain.Enums;
using OptiStock.Infrastructure.Persistence;

namespace OptiStock.Web.ApiControllers;

[ApiController]
[Route("api/pecas")]
[Authorize]
public sealed class PartsApiController(OptiStockDbContext db) : ControllerBase
{
    public sealed record PartInput(string Code, string Name, string? Description, int CategoryId, int? SupplierId,
        int InitialQuantity, int MinimumStock, decimal UnitPrice, string? WarehouseLocation);
    public sealed record UpdatePartInput(string Code, string Name, string? Description, int CategoryId, int? SupplierId,
        int MinimumStock, decimal UnitPrice, string? WarehouseLocation);

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? busca = null, [FromQuery] int? categoriaId = null,
        [FromQuery] string? endereco = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 100);
        var query = db.Parts.AsNoTracking().Include(x => x.Category).Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(busca)) query = query.Where(x => x.Name.Contains(busca) || x.Code.Contains(busca));
        if (categoriaId.HasValue) query = query.Where(x => x.CategoryId == categoriaId.Value);
        if (!string.IsNullOrWhiteSpace(endereco)) query = query.Where(x => x.WarehouseLocation != null && x.WarehouseLocation.Contains(endereco));
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.Id, x.Code, x.Name, x.Description, x.CategoryId, Category = x.Category.Name,
                x.SupplierId, x.StockQuantity, x.MinimumStock, x.UnitPrice, x.WarehouseLocation,
                Status = x.StockQuantity == 0 ? "Sem estoque" : x.StockQuantity <= x.MinimumStock ? "Estoque baixo" : "Regular" })
            .ToListAsync(cancellationToken);
        return Ok(new { items, page, pageSize, total });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var part = await db.Parts.AsNoTracking().Include(x => x.Category).Where(x => x.Id == id && x.IsActive)
            .Select(x => new { x.Id, x.Code, x.Name, x.Description, x.CategoryId, Category = x.Category.Name,
                x.SupplierId, x.StockQuantity, x.MinimumStock, x.UnitPrice, x.WarehouseLocation, x.CreatedAtUtc })
            .SingleOrDefaultAsync(cancellationToken);
        return part is null ? NotFound() : Ok(part);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(PartInput input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Code) || string.IsNullOrWhiteSpace(input.Name) || input.InitialQuantity < 0 || input.MinimumStock < 0 || input.UnitPrice < 0)
            return BadRequest(new ProblemDetails { Status = 400, Title = "Confira código, nome, quantidade, estoque mínimo e preço." });
        var code = input.Code.Trim().ToUpperInvariant();
        if (await db.Parts.AnyAsync(x => x.Code == code, cancellationToken))
            return Conflict(new ProblemDetails { Status = 409, Title = "Já existe uma peça com esse código." });
        if (!await db.Categories.AnyAsync(x => x.Id == input.CategoryId && x.IsActive, cancellationToken)) return BadRequest(new ProblemDetails { Status = 400, Title = "Categoria inválida." });
        if (input.SupplierId.HasValue && !await db.Suppliers.AnyAsync(x => x.Id == input.SupplierId && x.IsActive, cancellationToken)) return BadRequest(new ProblemDetails { Status = 400, Title = "Fornecedor inválido." });
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var part = new Part { Code = code, Name = input.Name.Trim(), Description = input.Description?.Trim(),
            CategoryId = input.CategoryId, SupplierId = input.SupplierId, StockQuantity = input.InitialQuantity,
            MinimumStock = input.MinimumStock, UnitPrice = input.UnitPrice, WarehouseLocation = input.WarehouseLocation?.Trim() };
        db.Parts.Add(part);
        await db.SaveChangesAsync(cancellationToken);
        if (input.InitialQuantity > 0)
            db.StockMovements.Add(new StockMovement { PartId = part.Id, Type = MovementType.Entry, Quantity = input.InitialQuantity,
                PreviousBalance = 0, NewBalance = input.InitialQuantity, Notes = "Saldo inicial do cadastro", UserId = userId });
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = part.Id }, new { part.Id, part.Code, part.Name, part.StockQuantity });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, UpdatePartInput input, CancellationToken cancellationToken)
    {
        var part = await db.Parts.SingleOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        if (part is null) return NotFound();
        var code = input.Code.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(input.Name) || input.MinimumStock < 0 || input.UnitPrice < 0)
            return BadRequest(new ProblemDetails { Status = 400, Title = "Confira código, nome, estoque mínimo e preço." });
        if (await db.Parts.AnyAsync(x => x.Id != id && x.Code == code, cancellationToken))
            return Conflict(new ProblemDetails { Status = 409, Title = "Já existe uma peça com esse código." });
        if (!await db.Categories.AnyAsync(x => x.Id == input.CategoryId && x.IsActive, cancellationToken))
            return BadRequest(new ProblemDetails { Status = 400, Title = "Categoria inválida." });
        if (input.SupplierId.HasValue && !await db.Suppliers.AnyAsync(x => x.Id == input.SupplierId && x.IsActive, cancellationToken))
            return BadRequest(new ProblemDetails { Status = 400, Title = "Fornecedor inválido." });
        part.Code = code; part.Name = input.Name.Trim(); part.Description = input.Description?.Trim();
        part.CategoryId = input.CategoryId; part.SupplierId = input.SupplierId; part.MinimumStock = input.MinimumStock;
        part.UnitPrice = input.UnitPrice; part.WarehouseLocation = input.WarehouseLocation?.Trim();
        part.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var part = await db.Parts.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (part is null) return NotFound();
        part.IsActive = false; part.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

