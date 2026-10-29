using OptiStock.Domain.Enums;

namespace OptiStock.Domain.Entities;

public sealed class StockMovement
{
    public long Id { get; set; }
    public int PartId { get; set; }
    public Part Part { get; set; } = null!;
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public int PreviousBalance { get; set; }
    public int NewBalance { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public Guid UserId { get; set; }
}
