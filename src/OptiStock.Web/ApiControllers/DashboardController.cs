using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiStock.Domain.Enums;
using OptiStock.Infrastructure.Persistence;

namespace OptiStock.Web.ApiControllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Administrador")]
public sealed class DashboardController(OptiStockDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTimeOffset? inicio = null, [FromQuery] DateTimeOffset? fim = null,
        CancellationToken cancellationToken = default)
    {
        var from = inicio ?? DateTimeOffset.UtcNow.AddDays(-30);
        var to = fim ?? DateTimeOffset.UtcNow;
        if (to <= from) return BadRequest(new ProblemDetails { Status = 400, Title = "O fim do período deve ser posterior ao início." });
        var activeParts = db.Parts.AsNoTracking().Where(x => x.IsActive);
        var totals = await activeParts.GroupBy(_ => 1).Select(g => new
        {
            Parts = g.Count(), Units = g.Sum(x => x.StockQuantity), Low = g.Count(x => x.StockQuantity > 0 && x.StockQuantity <= x.MinimumStock),
            Empty = g.Count(x => x.StockQuantity == 0), InventoryValue = g.Sum(x => x.UnitPrice * x.StockQuantity)
        }).SingleOrDefaultAsync(cancellationToken);
        var period = db.StockMovements.AsNoTracking().Where(x => x.OccurredAtUtc >= from && x.OccurredAtUtc < to);
        var movementRows = await period.Select(x => new { x.OccurredAtUtc, x.Type, x.Quantity }).ToListAsync(cancellationToken);
        var movements = movementRows.GroupBy(x => new { Date = x.OccurredAtUtc.UtcDateTime.Date, x.Type })
            .Select(g => new { g.Key.Date, g.Key.Type, Quantity = g.Sum(x => x.Quantity), Count = g.Count() })
            .OrderBy(x => x.Date).ToList();
        var recent = await db.StockMovements.AsNoTracking().Include(x => x.Part).OrderByDescending(x => x.OccurredAtUtc).Take(10)
            .Select(x => new { x.Id, PartCode = x.Part.Code, PartName = x.Part.Name, x.Type, x.Quantity, x.OccurredAtUtc, x.Notes })
            .ToListAsync(cancellationToken);
        return Ok(new
        {
            PeriodStartUtc = from, PeriodEndUtc = to,
            Totals = totals ?? new { Parts = 0, Units = 0, Low = 0, Empty = 0, InventoryValue = 0m },
            Movements = new { Entries = movements.Where(x => x.Type == MovementType.Entry), Exits = movements.Where(x => x.Type == MovementType.Exit) },
            RecentMovements = recent
        });
    }
}
