using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiStock.Web.Models;
using OptiStock.Web.Services;

namespace OptiStock.Web.Controllers;

[Authorize]
public sealed class HomeController(OptiStockApiClient api) : Controller
{
    public sealed record DashboardTotals(int Parts, int Units, int Low, int Empty, decimal InventoryValue);
    public sealed record MovementPoint(DateTime Date, string Type, int Quantity, int Count);
    public sealed record MovementSeries(List<MovementPoint> Entries, List<MovementPoint> Exits);
    public sealed record RecentMovement(long Id, string PartCode, string PartName, string Type, int Quantity, DateTimeOffset OccurredAtUtc, string? Notes);
    public sealed record DashboardDto(DashboardTotals? Totals, DateTimeOffset PeriodStartUtc, DateTimeOffset PeriodEndUtc, MovementSeries? Movements, List<RecentMovement> RecentMovements);
    public sealed record DashboardPage(DashboardDto? Summary, List<PartsController.PartRow> Parts);

    public IActionResult Index()
    {
        ViewData["Title"] = "Home";
        return View();
    }

    public async Task<IActionResult> Dashboard(DateOnly? inicio, DateOnly? fim, CancellationToken ct)
    {
        try
        {
            if (inicio.HasValue && fim.HasValue && fim.Value < inicio.Value)
            {
                ViewData["DateFilterError"] = "A data final deve ser igual ou posterior à data inicial. Exibindo o período padrão.";
                inicio = null;
                fim = null;
            }
            var from = inicio?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var exclusiveTo = fim?.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var query = from is null && exclusiveTo is null ? "" : $"?inicio={Uri.EscapeDataString((from ?? DateTime.UtcNow.AddDays(-30)).ToString("O"))}&fim={Uri.EscapeDataString((exclusiveTo ?? DateTime.UtcNow).ToString("O"))}";
            ViewBag.Start = inicio?.ToString("yyyy-MM-dd");
            ViewBag.End = fim?.ToString("yyyy-MM-dd");
            var summary = await api.GetAsync<DashboardDto>($"api/dashboard{query}", ct);
            var parts = await api.GetAsync<PartsController.PartPage>("api/pecas?page=1&pageSize=8", ct);
            ViewData["Title"] = "Gerência";
            return View(new DashboardPage(summary, parts?.Items ?? []));
        }
        catch (HttpRequestException)
        {
            ViewData["Title"] = "Gerência";
            ViewData["ApiUnavailable"] = "Não foi possível carregar os indicadores. Confira a API e o PostgreSQL.";
            return View(new DashboardPage(null, []));
        }
    }

    public IActionResult Privacy() => View();

    [AllowAnonymous]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
}
