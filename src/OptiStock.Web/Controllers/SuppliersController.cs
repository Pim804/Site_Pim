using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiStock.Web.Services;

namespace OptiStock.Web.Controllers;

[Authorize(Roles = "Administrador")]
public sealed class SuppliersController(OptiStockApiClient api) : Controller
{
    public sealed record SupplierRow(int Id, string Name, string? TaxId, string? Phone, string? Email, string? Address);
    public sealed record SupplierForm(string Name, string? TaxId, string? Phone, string? Email, string? Address);
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        try { return View(await api.GetAsync<List<SupplierRow>>("api/fornecedores", ct) ?? []); }
        catch (HttpRequestException) { ViewData["Error"] = "Não foi possível consultar fornecedores."; return View(new List<SupplierRow>()); }
    }
    public IActionResult Create() => View("Form", new SupplierForm("", "", "", "", ""));
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierForm form, CancellationToken ct) => await Save(form, null, ct);
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        try { var item = await api.GetAsync<SupplierRow>($"api/fornecedores/{id}", ct); if (item is null) return NotFound(); ViewBag.Id = id; return View("Form", new SupplierForm(item.Name, item.TaxId, item.Phone, item.Email, item.Address)); }
        catch (HttpRequestException) { return NotFound(); }
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierForm form, CancellationToken ct) => await Save(form, id, ct);
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
    {
        using var response = await api.SendAsync(HttpMethod.Delete, $"api/fornecedores/{id}", new { }, ct);
        if (!response.IsSuccessStatusCode) TempData["Error"] = "Há peças ativas vinculadas a este fornecedor.";
        return RedirectToAction(nameof(Index));
    }
    private async Task<IActionResult> Save(SupplierForm form, int? id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(form.Name)) { ModelState.AddModelError(nameof(form.Name), "Informe o nome/razão social."); ViewBag.Id = id; return View("Form", form); }
        using var response = id is null ? await api.PostAsync("api/fornecedores", form, ct) : await api.SendAsync(HttpMethod.Put, $"api/fornecedores/{id}", form, ct);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError(string.Empty, "Confira os dados e tente novamente."); ViewBag.Id = id; return View("Form", form);
    }
}
