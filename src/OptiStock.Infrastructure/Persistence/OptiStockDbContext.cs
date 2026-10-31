using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OptiStock.Domain.Entities;
using OptiStock.Infrastructure.Auth;

namespace OptiStock.Infrastructure.Persistence;

public sealed class OptiStockDbContext(DbContextOptions<OptiStockDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<AppUser>(b => { b.Property(x => x.DisplayName).HasMaxLength(120).IsRequired(); b.HasIndex(x => x.IsActive); });
        builder.Entity<Category>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(100).IsRequired(); b.Property(x => x.Description).HasMaxLength(500);
            b.HasIndex(x => x.Name).IsUnique();
        });
        builder.Entity<Supplier>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(180).IsRequired(); b.Property(x => x.TaxId).HasMaxLength(18);
            b.Property(x => x.Email).HasMaxLength(254); b.Property(x => x.Phone).HasMaxLength(30);
            b.HasIndex(x => x.TaxId).IsUnique().HasFilter("\"documento_fiscal\" IS NOT NULL");
        });
        builder.Entity<Part>(b =>
        {
            b.Property(x => x.Code).HasMaxLength(50).IsRequired(); b.Property(x => x.Name).HasMaxLength(160).IsRequired();
            b.Property(x => x.Description).HasMaxLength(1000); b.Property(x => x.WarehouseLocation).HasMaxLength(100);
            b.Property(x => x.UnitPrice).HasPrecision(12, 2); b.Property(x => x.Version).IsConcurrencyToken();
            b.HasIndex(x => x.Code).IsUnique(); b.HasIndex(x => new { x.CategoryId, x.IsActive }); b.HasIndex(x => x.Name);
            b.ToTable(t => { t.HasCheckConstraint("ck_pecas_quantidade_nao_negativa", "\"quantidade_estoque\" >= 0"); t.HasCheckConstraint("ck_pecas_estoque_minimo_nao_negativo", "\"estoque_minimo\" >= 0"); t.HasCheckConstraint("ck_pecas_preco_nao_negativo", "\"preco_unitario\" >= 0"); });
            b.HasOne(x => x.Category).WithMany(x => x.Parts).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Supplier).WithMany(x => x.Parts).HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<StockMovement>(b =>
        {
            b.Property(x => x.Type).HasConversion<string>().HasMaxLength(10); b.Property(x => x.Notes).HasMaxLength(500);
            b.HasIndex(x => new { x.PartId, x.OccurredAtUtc }); b.HasIndex(x => x.UserId);
            b.ToTable(t => { t.HasCheckConstraint("ck_movimentacoes_quantidade_positiva", "\"quantidade\" > 0"); t.HasCheckConstraint("ck_movimentacoes_saldos_nao_negativos", "\"saldo_anterior\" >= 0 AND \"saldo_posterior\" >= 0"); });
            b.HasOne(x => x.Part).WithMany(x => x.Movements).HasForeignKey(x => x.PartId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<AppUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<Announcement>(b =>
        {
            b.Property(x => x.Title).HasMaxLength(180).IsRequired();
            b.Property(x => x.Content).HasMaxLength(10000).IsRequired();
            b.HasIndex(x => new { x.IsPublished, x.IsArchived, x.PublishedAtUtc });
            b.HasOne<AppUser>().WithMany().HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
        });
        NomesBancoPortugues.Aplicar(builder);
    }
}
