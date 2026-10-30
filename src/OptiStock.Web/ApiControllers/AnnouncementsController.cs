using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiStock.Domain.Entities;
using OptiStock.Infrastructure.Auth;
using OptiStock.Infrastructure.Persistence;

namespace OptiStock.Web.ApiControllers;

[ApiController]
[Route("api/comunicados")]
[Authorize]
public sealed class AnnouncementsApiController(OptiStockDbContext db) : ControllerBase
{
    public sealed record AnnouncementInput(string Title, string Content, bool IsPublished);

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var admin = User.IsInRole("Administrador");
        var now = DateTimeOffset.UtcNow;
        var item = await (from a in db.Announcements.AsNoTracking()
            join u in db.Users.AsNoTracking() on a.AuthorId equals u.Id
            where a.Id == id && !a.IsArchived && (admin || (a.IsPublished && a.PublishedAtUtc <= now))
            select new { a.Id, a.Title, a.Content, a.IsPublished, a.CreatedAtUtc, a.PublishedAtUtc, Author = u.DisplayName })
            .SingleOrDefaultAsync(ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 100);
        var admin = User.IsInRole("Administrador");
        var now = DateTimeOffset.UtcNow;
        var query = db.Announcements.AsNoTracking().Where(x => !x.IsArchived && (admin || (x.IsPublished && x.PublishedAtUtc <= now)));
        var total = await query.CountAsync(ct);
        var items = await (from a in query join u in db.Users.AsNoTracking() on a.AuthorId equals u.Id
            orderby a.PublishedAtUtc descending, a.CreatedAtUtc descending
            select new { a.Id, a.Title, a.Content, a.IsPublished, a.CreatedAtUtc, a.PublishedAtUtc, Author = u.DisplayName })
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Ok(new { items, page, pageSize, total });
    }

    [HttpPost, Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(AnnouncementInput input, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input.Title) || input.Title.Trim().Length > 180 || string.IsNullOrWhiteSpace(input.Content) || input.Content.Length > 10000)
            return BadRequest(new ProblemDetails { Status = 400, Title = "Informe título e conteúdo dentro dos limites permitidos." });
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authorId)) return Unauthorized();
        var announcement = new Announcement { Title = input.Title.Trim(), Content = input.Content.Trim(), AuthorId = authorId,
            IsPublished = input.IsPublished, PublishedAtUtc = input.IsPublished ? DateTimeOffset.UtcNow : null };
        db.Announcements.Add(announcement); await db.SaveChangesAsync(ct);
        return Created($"/api/comunicados/{announcement.Id}", new { announcement.Id, announcement.Title, announcement.IsPublished });
    }

    [HttpPut("{id:int}"), Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, AnnouncementInput input, CancellationToken ct)
    {
        var announcement = await db.Announcements.SingleOrDefaultAsync(x => x.Id == id && !x.IsArchived, ct);
        if (announcement is null) return NotFound();
        if (string.IsNullOrWhiteSpace(input.Title) || input.Title.Trim().Length > 180 || string.IsNullOrWhiteSpace(input.Content) || input.Content.Length > 10000)
            return BadRequest(new ProblemDetails { Status = 400, Title = "Informe título e conteúdo dentro dos limites permitidos." });
        announcement.Title = input.Title.Trim(); announcement.Content = input.Content.Trim();
        announcement.IsPublished = input.IsPublished;
        announcement.PublishedAtUtc = input.IsPublished ? announcement.PublishedAtUtc ?? DateTimeOffset.UtcNow : null;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}"), Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Archive(int id, CancellationToken ct)
    {
        var announcement = await db.Announcements.SingleOrDefaultAsync(x => x.Id == id && !x.IsArchived, ct);
        if (announcement is null) return NotFound();
        announcement.IsArchived = true; await db.SaveChangesAsync(ct); return NoContent();
    }
}

