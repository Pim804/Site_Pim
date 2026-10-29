using Microsoft.EntityFrameworkCore;
using OptiStock.Application.Stock;
using OptiStock.Domain.Entities;
using OptiStock.Domain.Enums;
using OptiStock.Domain.Stock;
using OptiStock.Infrastructure.Persistence;

namespace OptiStock.Infrastructure.Stock;

public sealed class StockMovementService(OptiStockDbContext db) : IStockMovementService
{
    public async Task<MovementResult> RegisterAsync(MovementRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0) throw new ArgumentOutOfRangeException(nameof(request.Quantity), "A quantidade deve ser maior que zero.");
        if (!Enum.IsDefined(request.Type)) throw new ArgumentOutOfRangeException(nameof(request.Type), "Tipo inválido.");
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var part = await db.Parts.SingleOrDefaultAsync(x => x.Id == request.PartId && x.IsActive, cancellationToken)
            ?? throw new KeyNotFoundException("Peça não encontrada ou inativa.");
        if (!await db.Users.AnyAsync(x => x.Id == userId && x.IsActive, cancellationToken))
            throw new UnauthorizedAccessException("Usuário inválido ou inativo.");
        var previous = part.StockQuantity;
        var updated = StockBalanceCalculator.Calculate(previous, request.Type, request.Quantity);
        part.StockQuantity = updated; part.Version++; part.UpdatedAtUtc = DateTimeOffset.UtcNow;
        var movement = new StockMovement { PartId = part.Id, Type = request.Type, Quantity = request.Quantity,
            PreviousBalance = previous, NewBalance = updated, Notes = request.Notes?.Trim(), UserId = userId };
        db.StockMovements.Add(movement);
        try { await db.SaveChangesAsync(cancellationToken); await transaction.CommitAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new InvalidOperationException("O saldo mudou durante a operação. Atualize e tente novamente.", ex);
        }
        return new MovementResult(movement.Id, part.Id, movement.Type, movement.Quantity, previous, updated, movement.OccurredAtUtc);
    }
}
