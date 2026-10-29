using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiStock.Web.Services;

namespace OptiStock.Web.Controllers;

[Authorize]
public sealed class MovementsController(OptiStockApiClient api) : Controller
{
    public sealed record MovementRow(long Id, int PartId, string PartCode, string PartName, string Type, int Quantity, int PreviousBalance, int NewBalance, string? Notes, DateTimeOffset OccurredAtUtc, Guid UserId);
    public sealed record MovementPage(List<MovementRow> Items, int Page, int PageSize, int Total);
    public sealed record PartOption(int Id, string Code, string Name, int StockQuantity);
    public sealed record MovementForm(int PartId, string Type, int Quantity, string? Notes);

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        try { return View(await api.GetAsync<MovementPage>("api/movimentacoes?pageSize=50", ct)); }
        catch (HttpRequestException) { ViewData["ApiUnavailable"] = "Não foi possível consultar o histórico."; return View(null); }
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewBag.Parts = await api.GetAsync<PartPage>("api/pecas?pageSize=100", ct);
        return View(new MovementForm(0, "entry", 1, ""));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovementForm form, CancellationToken ct)
    {
        if (form.PartId <= 0 || form.Quantity <= 0 || form.Type is not ("entry" or "exit"))
        {
            ModelState.AddModelError(string.Empty, "Selecione a peça e informe tipo e quantidade positiva.");
            ViewBag.Parts = await api.GetAsync<PartPage>("api/pecas?pageSize=100", ct);
            return View(form);
        }
        using var response = await api.PostAsync("api/movimentacoes", new { form.PartId, type = form.Type, form.Quantity, form.Notes }, ct);
        if (response.IsSuccessStatusCode) { TempData["ToastSuccess"] = "Movimentação registrada e saldo atualizado."; return RedirectToAction(nameof(Index)); }
        ModelState.AddModelError(string.Empty, response.StatusCode == System.Net.HttpStatusCode.Conflict ? "Saída maior que o saldo ou o estoque foi alterado por outra operação." : "Não foi possível registrar a movimentação.");
        ViewBag.Parts = await api.GetAsync<PartPage>("api/pecas?pageSize=100", ct);
        return View(form);
    }

    public sealed record PartPage(List<PartOption> Items, int Page, int PageSize, int Total);
}
