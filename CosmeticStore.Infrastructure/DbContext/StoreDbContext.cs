using Microsoft.EntityFrameworkCore;
using CosmeticStore.Core.Entities;

namespace CosmeticStore.Infrastructure.DbContext;

/// <summary>
/// Database Context - Cầu nối giữa ứng dụng và SQL Server
/// Sử dụng Entity Framework Core để mapping Entity sang bảng trong database
/// </summary>
public class StoreDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Bảng Products trong database
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Cấu hình mapping Entity sang bảng
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình bảng Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Price)
                .HasPrecision(18, 2); // Độ chính xác cho tiền tệ
            
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            
            entity.Property(e => e.Brand)
                .HasMaxLength(100);
            
            entity.Property(e => e.Category)
                .HasMaxLength(100);
            
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            // Query Filter: Tự động lọc bỏ các record đã xóa mềm
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}

