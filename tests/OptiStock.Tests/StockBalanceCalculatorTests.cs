using OptiStock.Domain.Enums;
using OptiStock.Domain.Stock;

namespace OptiStock.Tests;

public sealed class StockBalanceCalculatorTests
{
    [Fact]
    public void Entry_IncreasesBalance() => Assert.Equal(15, StockBalanceCalculator.Calculate(10, MovementType.Entry, 5));

    [Fact]
    public void Exit_DecreasesBalance() => Assert.Equal(5, StockBalanceCalculator.Calculate(10, MovementType.Exit, 5));

    [Fact]
    public void Exit_AboveAvailableBalance_IsRejected() =>
        Assert.Throws<InvalidOperationException>(() => StockBalanceCalculator.Calculate(3, MovementType.Exit, 4));

    [Fact]
    public void ZeroQuantity_IsRejected() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => StockBalanceCalculator.Calculate(3, MovementType.Entry, 0));
}
