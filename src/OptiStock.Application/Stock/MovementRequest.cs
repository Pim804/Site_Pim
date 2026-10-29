using OptiStock.Domain.Enums;

namespace OptiStock.Application.Stock;

public sealed record MovementRequest(int PartId, MovementType Type, int Quantity, string? Notes);
public sealed record MovementResult(long Id, int PartId, MovementType Type, int Quantity, int PreviousBalance, int NewBalance, DateTimeOffset OccurredAtUtc);
public interface IStockMovementService
{
    Task<MovementResult> RegisterAsync(MovementRequest request, Guid userId, CancellationToken cancellationToken = default);
}
