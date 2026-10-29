using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptiStock.Web.Services;

namespace OptiStock.Web.Controllers;

[Authorize]
public sealed class PortalController(OptiStockApiClient api) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.Today = DateTime.Now.ToString("D");
        try
        {
            var page = await api.GetAsync<AnnouncementsController.AnnouncementPage>("api/comunicados?pageSize=5", ct);
            return View(page?.Items ?? []);
        }
        catch (HttpRequestException)
        {
            ViewData["Unavailable"] = "Comunicados indisponíveis no momento.";
            return View(new List<AnnouncementsController.AnnouncementRow>());
        }
    }
}
