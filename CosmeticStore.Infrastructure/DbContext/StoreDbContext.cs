using Microsoft.EntityFrameworkCore;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;

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
    /// Bảng Orders trong database
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Bảng OrderItems trong database
    /// </summary>
    public DbSet<OrderItem> OrderItems { get; set; }

    /// <summary>
    /// Bảng SystemLogs trong database - Singleton Logger ghi dữ liệu vào đây
    /// </summary>
    public DbSet<SystemLog> SystemLogs { get; set; }

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

            // Authentication Properties
            entity.Property(e => e.Role)
                .HasConversion<int>()
                .HasDefaultValue(0);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.LastLoginAt);

            entity.Property(e => e.RefreshToken)
                .HasMaxLength(500);

            entity.Property(e => e.RefreshTokenExpiryTime);

            // Indexes
            entity.HasIndex(e => e.VipLevel);
            entity.HasIndex(e => e.SkinType);
            entity.HasIndex(e => e.Role);
            entity.HasIndex(e => e.IsActive);

            // Query Filter: Soft Delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ==========================================
        // CẤU HÌNH BẢNG ORDER
        // ==========================================
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Order Number - Unique
            entity.Property(e => e.OrderNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(e => e.OrderNumber)
                .IsUnique();

            // Price Properties
            entity.Property(e => e.SubTotal)
                .HasPrecision(18, 2);

            entity.Property(e => e.ShippingFee)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            entity.Property(e => e.TotalDiscount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            // Status & Payment
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(0);

            entity.Property(e => e.PaymentMethod)
                .HasConversion<int>();

            entity.Property(e => e.PaymentTransactionId)
                .HasMaxLength(100);

            entity.Property(e => e.CouponCode)
                .HasMaxLength(50);

            // Shipping Info
            entity.Property(e => e.ShippingAddress)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.ShippingPhone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.ReceiverName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);

            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500);

            // Relationships
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.OrderItems)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.PaymentMethod);

            // Query Filter: Soft Delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ==========================================
        // CẤU HÌNH BẢNG ORDER ITEM
        // ==========================================
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Product Snapshot
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2);

            entity.Property(e => e.DiscountedPrice)
                .HasPrecision(18, 2);

            entity.Property(e => e.DiscountDescription)
                .HasMaxLength(500);

            // Relationships
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);

            // Query Filter: Soft Delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ==========================================
        // CẤU HÌNH BẢNG SYSTEM LOG (Singleton Logger)
        // ==========================================
        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Timestamp
            entity.Property(e => e.Timestamp)
                .IsRequired();

            // Log Level
            entity.Property(e => e.Level)
                .HasConversion<int>();

            // Category
            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50);

            // Message
            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(4000);

            // Data (JSON)
            entity.Property(e => e.Data)
                .HasMaxLength(8000);

            // Exception Details
            entity.Property(e => e.ExceptionDetails)
                .HasMaxLength(4000);

            // Stack Trace
            entity.Property(e => e.StackTrace)
                .HasMaxLength(8000);

            // Request Info
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50);

            entity.Property(e => e.RequestPath)
                .HasMaxLength(500);

            entity.Property(e => e.HttpMethod)
                .HasMaxLength(10);

            entity.Property(e => e.RelatedEntityType)
                .HasMaxLength(50);

            entity.Property(e => e.MachineName)
                .HasMaxLength(100);

            // Indexes for performance
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => new { e.RelatedEntityType, e.RelatedEntityId });
            entity.HasIndex(e => e.UserId);

            // Không áp dụng Soft Delete cho logs
            // Logs cần được giữ nguyên để audit
        });
    }
}
