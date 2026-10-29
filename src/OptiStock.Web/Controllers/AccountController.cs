using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiStock.Web.Services;

namespace OptiStock.Web.Controllers;

public sealed class AccountController(OptiStockApiClient api) : Controller
{
    public sealed record LoginForm(
        [param: Required(ErrorMessage = "Informe seu e-mail.")]
        [param: EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        string Email,
        [param: Required(ErrorMessage = "Informe sua senha.")]
        string Password);
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null) => View(new LoginForm(string.Empty, string.Empty));

    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginForm form, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(form);
        if (string.IsNullOrWhiteSpace(form.Email) || string.IsNullOrWhiteSpace(form.Password))
        { ModelState.AddModelError(string.Empty, "Informe e-mail e senha."); return View(form); }
        try
        {
            var result = await api.LoginAsync(form.Email.Trim(), form.Password, HttpContext.RequestAborted);
            if (result is null) { ModelState.AddModelError(string.Empty, "Credenciais inválidas."); return View(form); }
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, result.UserId.ToString()), new(ClaimTypes.Name, result.DisplayName), new("optistock_access_token", result.AccessToken) };
            claims.AddRange(result.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties { IsPersistent = false, ExpiresUtc = result.ExpiresAtUtc });
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl!) : RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException) { ModelState.AddModelError(string.Empty, "Não foi possível conectar à API do OptiStock."); return View(form); }
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try { using var response = await api.PostAsync("api/auth/logout", new { }, HttpContext.RequestAborted); }
        catch (HttpRequestException) { }
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
