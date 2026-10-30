using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using OptiStock.Domain.Entities;
using OptiStock.Infrastructure.Persistence;

namespace OptiStock.Web.ApiControllers;

[ApiController]
[Route("api/fornecedores")]
[Authorize(Roles = "Administrador")]
public sealed class SuppliersApiController(OptiStockDbContext db) : ControllerBase
{
    public sealed record SupplierInput(string Name, string? TaxId, string? Phone, string? Email, string? Address);

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var item = await db.Suppliers.AsNoTracking().Where(x => x.Id == id && x.IsActive)
            .Select(x => new { x.Id, x.Name, x.TaxId, x.Phone, x.Email, x.Address }).SingleOrDefaultAsync(cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? busca, CancellationToken cancellationToken)
    {
        var query = db.Suppliers.AsNoTracking().Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(busca)) query = query.Where(x => x.Name.Contains(busca) || (x.TaxId != null && x.TaxId.Contains(busca)));
        return Ok(await query.OrderBy(x => x.Name).Select(x => new { x.Id, x.Name, x.TaxId, x.Phone, x.Email, x.Address }).ToListAsync(cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(SupplierInput input, CancellationToken cancellationToken)
    {
        var validation = Validate(input);
        if (validation is not null) return BadRequest(new ProblemDetails { Status = 400, Title = validation });
        var supplier = new Supplier { Name = input.Name.Trim(), TaxId = CleanTaxId(input.TaxId), Phone = input.Phone?.Trim(), Email = input.Email?.Trim(), Address = input.Address?.Trim() };
        if (supplier.TaxId is not null && await db.Suppliers.AnyAsync(x => x.TaxId == supplier.TaxId, cancellationToken))
            return Conflict(new ProblemDetails { Status = 409, Title = "CNPJ já cadastrado." });
        db.Suppliers.Add(supplier); await db.SaveChangesAsync(cancellationToken);
        return Created($"/api/fornecedores/{supplier.Id}", new { supplier.Id, supplier.Name, supplier.TaxId });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, SupplierInput input, CancellationToken cancellationToken)
    {
        var supplier = await db.Suppliers.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (supplier is null) return NotFound();
        var validation = Validate(input);
        if (validation is not null) return BadRequest(new ProblemDetails { Status = 400, Title = validation });
        var taxId = CleanTaxId(input.TaxId);
        if (taxId is not null && await db.Suppliers.AnyAsync(x => x.Id != id && x.TaxId == taxId, cancellationToken))
            return Conflict(new ProblemDetails { Status = 409, Title = "CNPJ já cadastrado." });
        supplier.Name = input.Name.Trim(); supplier.TaxId = taxId; supplier.Phone = input.Phone?.Trim();
        supplier.Email = input.Email?.Trim(); supplier.Address = input.Address?.Trim();
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var supplier = await db.Suppliers.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (supplier is null) return NotFound();
        if (await db.Parts.AnyAsync(x => x.SupplierId == id && x.IsActive, cancellationToken))
            return Conflict(new ProblemDetails { Status = 409, Title = "Há peças ativas vinculadas ao fornecedor." });
        supplier.IsActive = false; await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static string? Validate(SupplierInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length > 180) return "Informe o nome/razão social (até 180 caracteres).";
        if (input.Email is { Length: > 254 }) return "E-mail muito longo.";
        if (input.TaxId is { Length: > 18 }) return "CNPJ deve ter até 18 caracteres.";
        if (!string.IsNullOrWhiteSpace(input.Email) && !MailAddress.TryCreate(input.Email.Trim(), out _)) return "E-mail inválido.";
        if (!string.IsNullOrWhiteSpace(input.TaxId) && CleanTaxId(input.TaxId)!.Length != 14) return "Informe um CNPJ com 14 dígitos.";
        return null;
    }

    private static string? CleanTaxId(string? taxId) => string.IsNullOrWhiteSpace(taxId) ? null : new string(taxId.Where(char.IsDigit).ToArray());
}

