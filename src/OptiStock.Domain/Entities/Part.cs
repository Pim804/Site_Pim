namespace OptiStock.Domain.Entities;

public sealed class Part
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public int StockQuantity { get; set; }
    public int MinimumStock { get; set; }
    public decimal UnitPrice { get; set; }
    public string? WarehouseLocation { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAtUtc { get; set; }
    public long Version { get; set; }
    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();
}
