using OptiStock.Domain.Enums;

namespace OptiStock.Domain.Stock;

public static class StockBalanceCalculator
{
    public static int Calculate(int currentBalance, MovementType type, int quantity)
    {
        if (currentBalance < 0) throw new ArgumentOutOfRangeException(nameof(currentBalance));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "A quantidade deve ser positiva.");
        return type switch
        {
            MovementType.Entry => checked(currentBalance + quantity),
            MovementType.Exit when quantity <= currentBalance => currentBalance - quantity,
            MovementType.Exit => throw new InvalidOperationException("A saída excede o saldo disponível."),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Tipo inválido.")
        };
    }
}
