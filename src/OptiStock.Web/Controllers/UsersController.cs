using System.Net;
using System.Text.Json;
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
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        { return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index)) }); }
        catch (HttpRequestException) { ViewData["Error"] = "Não foi possível consultar usuários. Confira a conexão com a API."; return View(null); }
    }

    public IActionResult Create() => View("Create", new CreateUserForm("", "", "", "Funcionário"));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserForm form, CancellationToken ct)
    {
        using var response = await api.PostAsync("api/usuarios", new { form.DisplayName, form.Email, form.Password, form.Role }, ct);
        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Usuário criado. Compartilhe a senha inicial com ele por um canal seguro.";
            return RedirectToAction(nameof(Index));
        }
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Create)) });
        ModelState.AddModelError(string.Empty, await ApiErrorAsync(response, "Não foi possível criar o usuário.", ct));
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
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        { return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Edit), new { id }) }); }
        catch (HttpRequestException) { return NotFound(); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditUserForm form, CancellationToken ct)
    {
        using var response = await api.SendAsync(HttpMethod.Put, $"api/usuarios/{id}", new { form.DisplayName, form.Email, form.IsActive, form.Role }, ct);
        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Dados do usuário atualizados.";
            return RedirectToAction(nameof(Index));
        }
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Edit), new { id }) });
        ModelState.AddModelError(string.Empty, await ApiErrorAsync(response, "Não foi possível atualizar o usuário.", ct));
        ViewBag.Id = id;
        return View(form);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        using var response = await api.SendAsync(HttpMethod.Delete, $"api/usuarios/{id}", new { }, ct);
        if (response.IsSuccessStatusCode)
            TempData["Success"] = "Conta excluída permanentemente.";
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index)) });
        else
            TempData["Error"] = await ApiErrorAsync(response, "Não foi possível excluir a conta.", ct);
        return RedirectToAction(nameof(Index));
    }

    private static async Task<string> ApiErrorAsync(HttpResponseMessage response, string fallback, CancellationToken ct)
    {
        try
        {
            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            var root = json.RootElement;
            var messages = new List<string>();
            if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(title.GetString()))
                messages.Add(title.GetString()!);
            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                foreach (var field in errors.EnumerateObject())
                    if (field.Value.ValueKind == JsonValueKind.Array)
                        messages.AddRange(field.Value.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()!).Where(x => !string.IsNullOrWhiteSpace(x)));
            if (messages.Count > 0) return string.Join(" ", messages.Distinct());
        }
        catch (JsonException) { }
        return fallback;
    }
}