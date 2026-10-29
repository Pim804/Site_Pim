using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiStock.Web.Services;

namespace OptiStock.Web.Controllers;

[Authorize]
public sealed class AnnouncementsController(OptiStockApiClient api) : Controller
{
    public sealed record AnnouncementRow(int Id, string Title, string Content, bool IsPublished, DateTimeOffset CreatedAtUtc, DateTimeOffset? PublishedAtUtc, string Author);
    public sealed record AnnouncementPage(List<AnnouncementRow> Items, int Page, int PageSize, int Total);
    public sealed record AnnouncementForm(string Title, string Content, bool IsPublished);

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        try { return View(await api.GetAsync<AnnouncementPage>("api/comunicados", ct)); }
        catch (HttpRequestException) { ViewData["ApiUnavailable"] = "Não foi possível carregar os comunicados."; return View(null); }
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult Create() => View("Form", new AnnouncementForm("", "", true));

    [Authorize(Roles = "Administrador"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AnnouncementForm form, CancellationToken ct) => await Save(form, null, ct);

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        try
        {
            var item = await api.GetAsync<AnnouncementRow>($"api/comunicados/{id}", ct);
            if (item is null) return NotFound();
            ViewBag.Id = id;
            return View("Form", new AnnouncementForm(item.Title, item.Content, item.IsPublished));
        }
        catch (HttpRequestException) { return NotFound(); }
    }

    [Authorize(Roles = "Administrador"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AnnouncementForm form, CancellationToken ct) => await Save(form, id, ct);

    [Authorize(Roles = "Administrador"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(int id, CancellationToken ct)
    {
        using var response = await api.SendAsync(HttpMethod.Delete, $"api/comunicados/{id}", new { }, ct);
        if (response.IsSuccessStatusCode) { TempData["ToastSuccess"] = "Comunicado arquivado."; return RedirectToAction(nameof(Index)); }
        TempData["Error"] = "Não foi possível arquivar o comunicado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> Save(AnnouncementForm form, int? id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(form.Title) || string.IsNullOrWhiteSpace(form.Content))
        { ModelState.AddModelError(string.Empty, "Título e conteúdo são obrigatórios."); ViewBag.Id = id; return View("Form", form); }
        using var response = id is null
            ? await api.PostAsync("api/comunicados", new { form.Title, form.Content, form.IsPublished }, ct)
            : await api.SendAsync(HttpMethod.Put, $"api/comunicados/{id}", new { form.Title, form.Content, form.IsPublished }, ct);
        if (response.IsSuccessStatusCode) { TempData["ToastSuccess"] = id is null ? "Comunicado publicado." : "Comunicado atualizado."; return RedirectToAction(nameof(Index)); }
        ModelState.AddModelError(string.Empty, "A API não aceitou o comunicado."); ViewBag.Id = id; return View("Form", form);
    }
}
