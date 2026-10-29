using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiStock.Web.Services;

namespace OptiStock.Web.Controllers;

[Authorize]
public sealed class PartsController(OptiStockApiClient api) : Controller
{
    public sealed record PartRow(int Id, string Code, string Name, string Category, int StockQuantity, int MinimumStock, decimal UnitPrice, string? WarehouseLocation, string Status);
    public sealed record PartPage(List<PartRow> Items, int Page, int PageSize, int Total);
    public sealed record CategoryOption(int Id, string Name);
    public sealed record SupplierOption(int Id, string Name);
    public sealed record PageModel(PartPage? Results, List<CategoryOption> Categories, string? Busca, string? Error = null);
    public sealed record PartForm(string Code, string Name, string? Description, int CategoryId, int? SupplierId, int InitialQuantity, int MinimumStock, decimal UnitPrice, string? WarehouseLocation);
    public sealed record EditPartForm(string Code, string Name, string? Description, int CategoryId, int? SupplierId, int MinimumStock, decimal UnitPrice, string? WarehouseLocation);
    public sealed record PartDetails(int Id, string Code, string Name, string? Description, int CategoryId, int? SupplierId, int StockQuantity, int MinimumStock, decimal UnitPrice, string? WarehouseLocation, DateTimeOffset CreatedAtUtc);

    public async Task<IActionResult> Index(string? busca, CancellationToken ct)
    {
        try
        {
            var query = string.IsNullOrWhiteSpace(busca) ? "" : $"?busca={Uri.EscapeDataString(busca)}";
            var results = await api.GetAsync<PartPage>($"api/pecas{query}", ct);
            return View(new PageModel(results, [], busca));
        }
        catch (HttpRequestException) { return View(new PageModel(null, [], busca, "Não foi possível consultar a API.")); }
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        try { await LoadOptions(ct); return View(new PartForm("", "", null, 0, null, 0, 0, 0, "")); }
        catch (HttpRequestException) { return View(new PartForm("", "", null, 0, null, 0, 0, 0, "")); }
    }

    [Authorize(Roles = "Administrador"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartForm form, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(form.Code) || string.IsNullOrWhiteSpace(form.Name) || form.CategoryId <= 0 || form.InitialQuantity < 0 || form.MinimumStock < 0 || form.UnitPrice < 0)
        {
            ModelState.AddModelError(string.Empty, "Confira os campos obrigatórios, quantidades e preço.");
            await LoadOptions(ct);
            return View(form);
        }
        using var response = await api.PostAsync("api/pecas", new { form.Code, form.Name, form.Description, form.CategoryId, form.SupplierId,
            form.InitialQuantity, form.MinimumStock, form.UnitPrice, form.WarehouseLocation }, ct);
        if (response.IsSuccessStatusCode) { TempData["ToastSuccess"] = "Peça cadastrada. O saldo inicial foi registrado no histórico."; return RedirectToAction(nameof(Index)); }
        ModelState.AddModelError(string.Empty, response.StatusCode == System.Net.HttpStatusCode.Conflict ? "O código já está cadastrado." : "Não foi possível cadastrar a peça.");
        await LoadOptions(ct);
        return View(form);
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        try
        {
            var part = await api.GetAsync<PartDetails>($"api/pecas/{id}", ct);
            if (part is null) return NotFound();
            await LoadOptions(ct);
            ViewBag.Id = id;
            return View(new EditPartForm(part.Code, part.Name, part.Description, part.CategoryId, part.SupplierId, part.MinimumStock, part.UnitPrice, part.WarehouseLocation));
        }
        catch (HttpRequestException) { return NotFound(); }
    }

    [Authorize(Roles = "Administrador"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditPartForm form, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(form.Code) || string.IsNullOrWhiteSpace(form.Name) || form.CategoryId <= 0 || form.MinimumStock < 0 || form.UnitPrice < 0)
        {
            ModelState.AddModelError(string.Empty, "Confira os campos da peça.");
            await LoadOptions(ct); ViewBag.Id = id; return View(form);
        }
        using var response = await api.SendAsync(HttpMethod.Put, $"api/pecas/{id}", form, ct);
        if (response.IsSuccessStatusCode) { TempData["ToastSuccess"] = "Peça atualizada."; return RedirectToAction(nameof(Index)); }
        ModelState.AddModelError(string.Empty, "Confira o código e os relacionamentos da peça.");
        await LoadOptions(ct); ViewBag.Id = id; return View(form);
    }

    [Authorize(Roles = "Administrador"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
    {
        using var response = await api.SendAsync(HttpMethod.Delete, $"api/pecas/{id}", new { }, ct);
        if (!response.IsSuccessStatusCode) TempData["Error"] = "Não foi possível desativar a peça.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadOptions(CancellationToken ct)
    {
        ViewBag.Categories = await api.GetAsync<List<CategoryOption>>("api/categorias", ct) ?? [];
        ViewBag.Suppliers = await api.GetAsync<List<SupplierOption>>("api/fornecedores", ct) ?? [];
    }
}
