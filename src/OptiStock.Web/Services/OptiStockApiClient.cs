using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace OptiStock.Web.Services;

public sealed class OptiStockApiClient(HttpClient http, IHttpContextAccessor context)
{
    public sealed record LoginResult(string AccessToken, DateTimeOffset ExpiresAtUtc, Guid UserId, string DisplayName, string[] Roles);
    public async Task<LoginResult?> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync("api/auth/login", new { email, password }, ct);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<LoginResult>(cancellationToken: ct) : null;
    }
    public async Task<T?> GetAsync<T>(string path, CancellationToken ct = default)
    {
        using var request = AuthorizedRequest(HttpMethod.Get, path);
        using var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && context.HttpContext is { } httpContext)
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }
    public Task<HttpResponseMessage> PostAsync<T>(string path, T body, CancellationToken ct = default) => SendAsync(HttpMethod.Post, path, body, ct);
    public async Task<HttpResponseMessage> SendAsync<T>(HttpMethod method, string path, T body, CancellationToken ct = default)
    {
        var request = AuthorizedRequest(method, path);
        request.Content = JsonContent.Create(body);
        var response = await http.SendAsync(request, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && context.HttpContext is { } httpContext)
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return response;
    }
    private HttpRequestMessage AuthorizedRequest(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, path);
        var token = context.HttpContext?.User.FindFirstValue("optistock_access_token");
        if (!string.IsNullOrWhiteSpace(token)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}
