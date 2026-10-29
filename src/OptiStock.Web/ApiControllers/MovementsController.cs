using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiStock.Application.Stock;
using OptiStock.Domain.Enums;
using OptiStock.Infrastructure.Persistence;

namespace OptiStock.Web.ApiControllers;

[ApiController]
[Route("api/movimentacoes")]
[Authorize(Roles = "Administrador,Funcionário")]
public sealed class MovementsController(IStockMovementService movements, OptiStockDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<MovementResult>> Create(MovementRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        try
        {
            var result = await movements.RegisterAsync(request, userId, cancellationToken);
            return Created($"/api/movimentacoes/{result.Id}", result);
        }
        catch (ArgumentOutOfRangeException ex) { return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]> { ["quantidade"] = [ex.Message] })); }
        catch (KeyNotFoundException ex) { return NotFound(new ProblemDetails { Status = 404, Title = ex.Message }); }
        catch (UnauthorizedAccessException) { return Unauthorized(); }
        catch (InvalidOperationException ex) { return Conflict(new ProblemDetails { Status = 409, Title = ex.Message }); }
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 25,
        [FromQuery] string? busca = null, [FromQuery] MovementType? tipo = null,
        [FromQuery] DateTimeOffset? inicio = null, [FromQuery] DateTimeOffset? fim = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 100);
        var query = db.StockMovements.AsNoTracking().Include(x => x.Part).AsQueryable();
        if (!string.IsNullOrWhiteSpace(busca)) query = query.Where(x => x.Part.Name.Contains(busca) || x.Part.Code.Contains(busca) || (x.Notes != null && x.Notes.Contains(busca)));
        if (tipo.HasValue) query = query.Where(x => x.Type == tipo.Value);
        if (inicio.HasValue) query = query.Where(x => x.OccurredAtUtc >= inicio.Value);
        if (fim.HasValue) query = query.Where(x => x.OccurredAtUtc < fim.Value);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.OccurredAtUtc).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.Id, x.PartId, PartCode = x.Part.Code, PartName = x.Part.Name, x.Type, x.Quantity, x.PreviousBalance, x.NewBalance, x.Notes, x.OccurredAtUtc, x.UserId })
            .ToListAsync(cancellationToken);
        return Ok(new { items, page, pageSize, total });
    }
}
