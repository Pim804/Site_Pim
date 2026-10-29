using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiStock.Web.Services;

namespace OptiStock.Web.Controllers;

[Authorize(Roles = "Administrador")]
public sealed class CategoriesController(OptiStockApiClient api) : Controller
{
    public sealed record CategoryRow(int Id, string Name, string? Description, int PartCount);
    public sealed record CategoryForm(string Name, string? Description);
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        try { return View(await api.GetAsync<List<CategoryRow>>("api/categorias", ct) ?? []); }
        catch (HttpRequestException) { ViewData["Error"] = "Não foi possível consultar categorias."; return View(new List<CategoryRow>()); }
    }
    public IActionResult Create() => View("Form", new CategoryForm("", ""));
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryForm form, CancellationToken ct) => await Save(form, null, ct);
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        try { var item = await api.GetAsync<CategoryFormResult>($"api/categorias/{id}", ct); if (item is null) return NotFound(); ViewBag.Id = id; return View("Form", new CategoryForm(item.Name, item.Description)); }
        catch (HttpRequestException) { return NotFound(); }
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryForm form, CancellationToken ct) => await Save(form, id, ct);
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
    {
        using var response = await api.SendAsync(HttpMethod.Delete, $"api/categorias/{id}", new { }, ct);
        if (!response.IsSuccessStatusCode) TempData["Error"] = "A categoria pode ter peças ativas vinculadas.";
        return RedirectToAction(nameof(Index));
    }
    private async Task<IActionResult> Save(CategoryForm form, int? id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(form.Name)) { ModelState.AddModelError(nameof(form.Name), "Informe um nome."); ViewBag.Id = id; return View("Form", form); }
        using var response = id is null ? await api.PostAsync("api/categorias", form, ct) : await api.SendAsync(HttpMethod.Put, $"api/categorias/{id}", form, ct);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError(string.Empty, "A API não aceitou os dados informados."); ViewBag.Id = id; return View("Form", form);
    }
    public sealed record CategoryFormResult(int Id, string Name, string? Description);
}
