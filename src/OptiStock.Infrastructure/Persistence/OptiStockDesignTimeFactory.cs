using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OptiStock.Infrastructure.Persistence;

public sealed class OptiStockDesignTimeFactory : IDesignTimeDbContextFactory<OptiStockDbContext>
{
    public OptiStockDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__OptiStock")
            ?? "Host=localhost;Database=optistock;Username=optistock_user";
        return new OptiStockDbContext(new DbContextOptionsBuilder<OptiStockDbContext>()
            .UseNpgsql(connection).Options);
    }
}
