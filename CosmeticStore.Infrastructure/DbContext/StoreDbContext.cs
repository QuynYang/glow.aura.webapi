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
    /// Bảng Users trong database
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Cấu hình mapping Entity sang bảng
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==========================================
        // CẤU HÌNH BẢNG PRODUCT
        // ==========================================
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Basic Properties
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Price)
                .HasPrecision(18, 2);
            
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            
            entity.Property(e => e.Brand)
                .HasMaxLength(100);
            
            entity.Property(e => e.Category)
                .HasMaxLength(100);
            
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            // Cosmetic-Specific Properties
            entity.Property(e => e.SkinType)
                .HasConversion<int>();

            entity.Property(e => e.ExpiryDate);

            entity.Property(e => e.IsFlashSale)
                .HasDefaultValue(false);

            entity.Property(e => e.FlashSaleDiscount)
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            entity.Property(e => e.FlashSaleEndTime);

            entity.Property(e => e.Ingredients)
                .HasMaxLength(2000);

            entity.Property(e => e.UsageInstructions)
                .HasMaxLength(1000);

            entity.Property(e => e.Volume)
                .HasMaxLength(50);

            // Indexes
            entity.HasIndex(e => e.SkinType);
            entity.HasIndex(e => e.ExpiryDate);
            entity.HasIndex(e => e.IsFlashSale);
            entity.HasIndex(e => e.Brand);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Price);

            // Query Filter: Soft Delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ==========================================
        // CẤU HÌNH BẢNG USER
        // ==========================================
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Basic Properties
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(e => e.Address)
                .HasMaxLength(500);

            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500);

            // VIP & Loyalty Properties
            entity.Property(e => e.VipLevel)
                .HasConversion<int>()
                .HasDefaultValue(0);

            entity.Property(e => e.TotalSpent)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            entity.Property(e => e.LoyaltyPoints)
                .HasDefaultValue(0);

            // Skin Type Properties
            entity.Property(e => e.SkinType)
                .HasConversion<int>()
                .HasDefaultValue(0);

            entity.Property(e => e.HasCompletedSkinQuiz)
                .HasDefaultValue(false);

            entity.Property(e => e.SkinQuizCompletedAt);

            // Indexes
            entity.HasIndex(e => e.VipLevel);
            entity.HasIndex(e => e.SkinType);

            // Query Filter: Soft Delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
