using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiStock.Web.Services;

namespace OptiStock.Web.Controllers;

[Authorize(Roles = "Administrador")]
public sealed class UsersController(OptiStockApiClient api) : Controller
{
    public sealed record UserRow(Guid Id, string DisplayName, string? Email, bool IsActive, List<string> Roles);
    public sealed record UserPage(List<UserRow> Items, int Page, int PageSize, int Total);
    public sealed record CreateUserForm(string DisplayName, string Email, string Password, string Role);
    public sealed record EditUserForm(string DisplayName, string? Email, bool IsActive, string Role);
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        try { return View(await api.GetAsync<UserPage>("api/usuarios?pageSize=100", ct)); }
        catch (HttpRequestException) { ViewData["Error"] = "Não foi possível consultar usuários."; return View(null); }
    }
    public IActionResult Create() => View("Create", new CreateUserForm("", "", "", "Funcionário"));
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserForm form, CancellationToken ct)
    {
        using var response = await api.PostAsync("api/usuarios", new { form.DisplayName, form.Email, form.Password, form.Role }, ct);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError(string.Empty, "Confira nome, e-mail, perfil e os requisitos da senha.");
        return View(form);
    }
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        try
        {
            var user = await api.GetAsync<UserRow>($"api/usuarios/{id}", ct);
            if (user is null) return NotFound();
            ViewBag.Id = id;
            return View(new EditUserForm(user.DisplayName, user.Email, user.IsActive, user.Roles.FirstOrDefault() ?? "Funcionário"));
        }
        catch (HttpRequestException) { return NotFound(); }
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditUserForm form, CancellationToken ct)
    {
        using var response = await api.SendAsync(HttpMethod.Put, $"api/usuarios/{id}", new { form.DisplayName, form.Email, form.IsActive, form.Role }, ct);
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError(string.Empty, "Não foi possível atualizar o usuário."); ViewBag.Id = id; return View(form);
    }
}
