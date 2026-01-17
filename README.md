# 🌸 CosmeticStore - Web Bán Mỹ Phẩm

> Đồ án ASP.NET Core Web API áp dụng các nguyên tắc **OOP** và **Design Patterns**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)
[![Entity Framework](https://img.shields.io/badge/EF%20Core-10.0-purple?style=flat-square)](https://docs.microsoft.com/ef/)

## 📑 Mục lục

- [Giới thiệu](#-giới-thiệu)
- [Kiến trúc dự án](#-kiến-trúc-dự-án)
- [Cấu trúc thư mục](#-cấu-trúc-thư-mục)
- [4 tính chất OOP](#-4-tính-chất-oop-được-áp-dụng)
- [Design Patterns](#-design-patterns-được-áp-dụng)
- [Giai đoạn phát triển](#-giai-đoạn-phát-triển)
- [Giải thích chi tiết các file](#-giải-thích-chi-tiết-các-file)
- [Hướng dẫn cài đặt](#-hướng-dẫn-cài-đặt)
- [API Endpoints](#-api-endpoints)

---

## 🎯 Giới thiệu

**CosmeticStore** là một dự án Web API bán mỹ phẩm được xây dựng theo kiến trúc **Clean Architecture** với ASP.NET Core. Dự án tập trung vào việc áp dụng đúng đắn các nguyên tắc **Lập trình Hướng đối tượng (OOP)** và các **Design Patterns** phổ biến.

### Công nghệ sử dụng

| Công nghệ | Phiên bản | Mục đích |
|-----------|-----------|----------|
| .NET | 10.0 | Runtime & SDK |
| ASP.NET Core | 10.0 | Web API Framework |
| Entity Framework Core | 10.0 | ORM (Object-Relational Mapping) |
| SQL Server | LocalDB | Database |
| Swagger/OpenAPI | 7.x | API Documentation |

---

## 🏗️ Kiến trúc dự án

Dự án được tổ chức theo mô hình **Clean Architecture** (Kiến trúc sạch), chia thành 3 tầng:

```
┌─────────────────────────────────────────────────────────────┐
│                    CosmeticStore.API                        │
│              (Controllers, ViewModels, DI)                  │
│                    ↓ phụ thuộc vào ↓                        │
├─────────────────────────────────────────────────────────────┤
│               CosmeticStore.Infrastructure                  │
│         (DbContext, Repositories, Services, Strategies)     │
│                    ↓ phụ thuộc vào ↓                        │
├─────────────────────────────────────────────────────────────┤
│                   CosmeticStore.Core                        │
│            (Entities, Interfaces, Enums)                    │
│               ✨ KHÔNG PHỤ THUỘC GÌ ✨                       │
└─────────────────────────────────────────────────────────────┘
```

### Nguyên tắc phụ thuộc (Dependency Rule)

- **Core**: Tầng lõi, chứa logic nghiệp vụ, KHÔNG phụ thuộc bất kỳ tầng nào
- **Infrastructure**: Triển khai chi tiết (Database, Services), phụ thuộc vào Core
- **API**: Tầng giao diện, phụ thuộc vào cả Core và Infrastructure

---

## 📂 Cấu trúc thư mục

```
📦 CosmeticStore/
 ┣ 📂 CosmeticStore.API/              # Tầng API (Presentation Layer)
 ┃ ┣ 📂 Controllers/
 ┃ ┃ ┗ 📄 ProductsController.cs       # Controller quản lý sản phẩm (30+ endpoints)
 ┃ ┣ 📂 ViewModels/
 ┃ ┃ ┗ 📄 ProductViewModels.cs        # Request/Response models
 ┃ ┣ 📄 Program.cs                    # Entry point, cấu hình DI
 ┃ ┣ 📄 appsettings.json              # Cấu hình ứng dụng
 ┃ ┗ 📄 CosmeticStore.API.csproj
 ┃
 ┣ 📂 CosmeticStore.Core/             # Tầng Core (Domain Layer)
 ┃ ┣ 📂 Entities/
 ┃ ┃ ┣ 📄 BaseEntity.cs               # Class cha - Inheritance
 ┃ ┃ ┣ 📄 Product.cs                  # Entity sản phẩm - Encapsulation
 ┃ ┃ ┣ 📄 User.cs                     # Entity người dùng - VIP & SkinType
 ┃ ┃ ┣ 📄 Order.cs                    # Entity đơn hàng - Aggregate Root
 ┃ ┃ ┗ 📄 OrderItem.cs                # Entity chi tiết đơn hàng
 ┃ ┣ 📂 Enums/
 ┃ ┃ ┣ 📄 SkinType.cs                 # Enum loại da (Oily, Dry, Sensitive...)
 ┃ ┃ ┣ 📄 VipLevel.cs                 # Enum cấp VIP (Bronze, Silver, Gold, Platinum)
 ┃ ┃ ┣ 📄 OrderStatus.cs              # Enum trạng thái đơn hàng
 ┃ ┃ ┗ 📄 PaymentMethod.cs            # Enum phương thức thanh toán
 ┃ ┣ 📂 Commands/                     # Command Pattern
 ┃ ┃ ┣ 📄 ICommand.cs                 # Interface & Base class Command
 ┃ ┃ ┣ 📄 ICommandHandler.cs          # Interface Handler & Result
 ┃ ┃ ┗ 📂 Orders/                     # Order-related Commands
 ┃ ┃   ┣ 📄 CreateOrderCommand.cs     # Tạo đơn hàng
 ┃ ┃   ┣ 📄 CancelOrderCommand.cs     # Hủy đơn hàng
 ┃ ┃   ┣ 📄 ConfirmOrderCommand.cs    # Xác nhận đơn hàng
 ┃ ┃   ┗ 📄 PayOrderCommand.cs        # Thanh toán
 ┃ ┣ 📂 Interfaces/
 ┃ ┃ ┣ 📄 IGenericRepository.cs       # Interface CRUD cơ bản
 ┃ ┃ ┣ 📄 IProductRepository.cs       # Interface đặc thù cho Product
 ┃ ┃ ┣ 📄 IOrderRepository.cs         # Interface đặc thù cho Order
 ┃ ┃ ┣ 📄 IPricingStrategy.cs         # Interface Strategy Pattern
 ┃ ┃ ┣ 📄 IPriceDecorator.cs          # Abstract class Decorator Pattern
 ┃ ┃ ┣ 📄 IPricingService.cs          # Interface Pricing Orchestrator
 ┃ ┃ ┣ 📄 IPaymentService.cs          # Interface Payment Services
 ┃ ┃ ┗ 📄 IAppLogger.cs               # Interface Logger (Singleton)
 ┃ ┗ 📄 CosmeticStore.Core.csproj
 ┃
 ┣ 📂 CosmeticStore.Infrastructure/   # Tầng Infrastructure
 ┃ ┣ 📂 DbContext/
 ┃ ┃ ┗ 📄 StoreDbContext.cs           # EF Core DbContext (Products, Users, Orders)
 ┃ ┣ 📂 Repositories/
 ┃ ┃ ┣ 📄 GenericRepository.cs        # Generic Repository - CRUD cơ bản
 ┃ ┃ ┣ 📄 ProductRepository.cs        # Product Repository - Query đặc thù
 ┃ ┃ ┗ 📄 OrderRepository.cs          # Order Repository - Query đặc thù
 ┃ ┣ 📂 Strategies/                   # Strategy Pattern implementations
 ┃ ┃ ┣ 📄 StandardPricingStrategy.cs  # Giá thường (0%)
 ┃ ┃ ┣ 📄 VipPricingStrategy.cs       # Giá VIP (5%-20%)
 ┃ ┃ ┣ 📄 SkinTypePricingStrategy.cs  # Giá theo loại da (5%)
 ┃ ┃ ┗ 📄 SalePricingStrategy.cs      # Giá khuyến mãi
 ┃ ┣ 📂 Decorators/                   # Decorator Pattern implementations
 ┃ ┃ ┣ 📄 ExpiryDiscountDecorator.cs  # Giảm giá cận hạn (15%-40%)
 ┃ ┃ ┣ 📄 FlashSaleDecorator.cs       # Giảm giá Flash Sale
 ┃ ┃ ┗ 📄 CouponDecorator.cs          # Giảm giá mã coupon
 ┃ ┣ 📂 Handlers/                     # Command Handlers (Single Responsibility)
 ┃ ┃ ┣ 📄 CreateOrderCommandHandler.cs  # Tạo đơn hàng
 ┃ ┃ ┣ 📄 CancelOrderCommandHandler.cs  # Hủy đơn hàng
 ┃ ┃ ┣ 📄 ConfirmOrderCommandHandler.cs # Xác nhận đơn hàng
 ┃ ┃ ┗ 📄 PayOrderCommandHandler.cs     # Thanh toán (dùng Factory)
 ┃ ┣ 📂 Gateways/                     # Factory Pattern - Payment Gateways
 ┃ ┃ ┣ 📄 PaymentGatewayFactory.cs    # Factory tạo Gateway từ string
 ┃ ┃ ┣ 📄 MomoGateway.cs              # Cổng Momo (QR, DeepLink)
 ┃ ┃ ┣ 📄 ZaloPayGateway.cs           # Cổng ZaloPay
 ┃ ┃ ┣ 📄 VNPayGateway.cs             # Cổng VNPay
 ┃ ┃ ┗ 📄 CODGateway.cs               # Thanh toán khi nhận hàng
 ┃ ┣ 📂 Services/
 ┃ ┃ ┣ 📄 PricingService.cs           # Orchestrator Strategy + Decorator
 ┃ ┃ ┣ 📄 AppLogger.cs                # Logger (Singleton qua DI)
 ┃ ┃ ┣ 📄 PaymentFactory.cs           # Factory tạo Payment Service (Legacy)
 ┃ ┃ ┣ 📄 MomoPaymentService.cs       # Thanh toán Momo
 ┃ ┃ ┣ 📄 CodPaymentService.cs        # Thanh toán COD
 ┃ ┃ ┣ 📄 VnPayPaymentService.cs      # Thanh toán VNPay
 ┃ ┃ ┗ 📄 ZaloPayPaymentService.cs    # Thanh toán ZaloPay
 ┃ ┗ 📄 CosmeticStore.Infrastructure.csproj
 ┃
 ┣ 📄 CosmeticStore.sln               # Solution file
 ┣ 📄 .gitignore                      # Git ignore rules
 ┗ 📄 README.md                       # Tài liệu này
```

---

## 🎓 4 Tính chất OOP được áp dụng

### 1. 🧬 Tính Kế thừa (Inheritance)

> **Mục đích**: Giảm code lặp lại, tái sử dụng code

**File**: `CosmeticStore.Core/Entities/BaseEntity.cs`

```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
```

**Áp dụng trong Repository Pattern**: `IProductRepository` kế thừa từ `IGenericRepository<Product>`

```csharp
// IGenericRepository là interface CHA
public interface IGenericRepository<T> where T : BaseEntity { ... }

// IProductRepository KẾ THỪA và mở rộng
public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetBySkinTypeAsync(SkinType skinType);
    Task<IEnumerable<Product>> GetExpiringSoonAsync(int days);
    Task<IEnumerable<Product>> GetFlashSaleProductsAsync();
}
```

---

### 2. 🔒 Tính Đóng gói (Encapsulation)

> **Mục đích**: Bảo vệ dữ liệu, logic nghiệp vụ nằm trong Entity

**File**: `CosmeticStore.Core/Entities/Product.cs`

```csharp
public class Product : BaseEntity
{
    // Private set: Không thể sửa tùy tiện từ bên ngoài
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public SkinType SkinType { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public bool IsFlashSale { get; private set; }

    // Logic nghiệp vụ được đóng gói trong method
    public void UpdateStock(int quantity)
    {
        if (Stock + quantity < 0)
            throw new InvalidOperationException("Không đủ hàng tồn kho");
        Stock += quantity;
    }

    public void ActivateFlashSale(decimal discountPercent, DateTime endTime)
    {
        if (discountPercent <= 0 || discountPercent > 100)
            throw new ArgumentException("Phần trăm giảm giá phải từ 1-100");
        IsFlashSale = true;
        FlashSaleDiscount = discountPercent;
        FlashSaleEndTime = endTime;
    }

    public bool IsExpiringSoon(int days)
    {
        if (!ExpiryDate.HasValue) return false;
        return ExpiryDate.Value <= DateTime.UtcNow.AddDays(days);
    }
}
```

**So sánh**:

| ❌ TRƯỚC (Anemic Model) | ✅ SAU (Rich Domain Model) |
|------------------------|---------------------------|
| `product.Stock = product.Stock - 5;` | `product.DecreaseStock(5);` |
| `product.IsFlashSale = true;` | `product.ActivateFlashSale(20, endTime);` |
| Logic rải rác ở Controller | Logic tập trung trong Entity |

---

### 3. 🎭 Tính Đa hình (Polymorphism)

> **Mục đích**: Cùng interface, nhiều cách thực hiện khác nhau

**File**: `CosmeticStore.Core/Interfaces/IPricingStrategy.cs`

```csharp
public interface IPricingStrategy
{
    decimal CalculatePrice(decimal originalPrice);
    string StrategyName { get; }
}
```

**Các implementation khác nhau**:

| Strategy | File | Cách tính |
|----------|------|-----------|
| `StandardPricingStrategy` | `Strategies/StandardPricingStrategy.cs` | Giữ nguyên giá |
| `VipPricingStrategy` | `Strategies/VipPricingStrategy.cs` | Giảm 10% |
| `SalePricingStrategy` | `Strategies/SalePricingStrategy.cs` | Giảm theo % tùy chỉnh |

```csharp
// Cùng gọi CalculatePrice() nhưng kết quả khác nhau
IPricingStrategy strategy = new VipPricingStrategy();
decimal price = strategy.CalculatePrice(100000);  // → 90,000 VND

strategy = new StandardPricingStrategy();
price = strategy.CalculatePrice(100000);  // → 100,000 VND
```

---

### 4. 🎨 Tính Trừu tượng (Abstraction)

> **Mục đích**: Ẩn chi tiết implementation, chỉ expose interface

**File**: `CosmeticStore.Core/Interfaces/IGenericRepository.cs`

```csharp
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void SoftDelete(T entity);
    Task<int> SaveChangesAsync();
}
```

**Lợi ích**:

- **Controller** chỉ biết đến `IProductRepository`, không biết dùng EF Core hay Dapper
- **ProductRepository** che giấu sự phức tạp của SQL/LINQ
- Dễ dàng mock trong Unit Testing

```csharp
// Controller chỉ inject Interface
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;  // ← Interface
    
    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
}
```

---

## 🧩 Design Patterns được áp dụng

### 1. Repository Pattern ⭐

> **Mục đích**: Tách biệt logic truy cập dữ liệu khỏi business logic

#### Cấu trúc Repository Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                    IGenericRepository<T>                        │
│  ├── GetByIdAsync(id)                                           │
│  ├── GetAllAsync()                                              │
│  ├── FindAsync(predicate)                                       │
│  ├── FirstOrDefaultAsync(predicate)                             │
│  ├── AnyAsync(predicate)                                        │
│  ├── CountAsync(predicate)                                      │
│  ├── AddAsync(entity)                                           │
│  ├── AddRangeAsync(entities)                                    │
│  ├── Update(entity)                                             │
│  ├── SoftDelete(entity)                                         │
│  ├── HardDelete(entity)                                         │
│  └── SaveChangesAsync()                                         │
└──────────────────────────┬──────────────────────────────────────┘
                           │ Kế thừa (Inheritance)
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│                    IProductRepository                           │
│  ├── GetBySkinTypeAsync(skinType)      ← AI Skin Quiz           │
│  ├── GetExpiringSoonAsync(days)        ← Expiry Management      │
│  ├── GetFlashSaleProductsAsync()       ← Flash Sale             │
│  ├── GetByBrandAsync(brand)            ← Filter by Brand        │
│  ├── GetByCategoryAsync(category)      ← Filter by Category     │
│  ├── GetByPriceRangeAsync(min, max)    ← Price Filter           │
│  ├── GetLowStockProductsAsync(threshold) ← Stock Alert          │
│  ├── SearchAsync(keyword)              ← Basic Search           │
│  └── AdvancedSearchAsync(...)          ← Advanced Search        │
└─────────────────────────────────────────────────────────────────┘
```

#### Implementation

| File | Vai trò | OOP |
|------|---------|-----|
| `IGenericRepository.cs` | Interface CRUD cơ bản | **Abstraction** |
| `IProductRepository.cs` | Interface đặc thù, kế thừa Generic | **Inheritance** |
| `GenericRepository.cs` | Implement CRUD với EF Core | **Abstraction** |
| `ProductRepository.cs` | Implement query đặc thù | **Inheritance** |

```csharp
// GenericRepository - Class cha
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly StoreDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
}

// ProductRepository - KẾ THỪA từ GenericRepository
public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(StoreDbContext context) : base(context) { }

    // Method đặc thù - che giấu sự phức tạp của LINQ
    public async Task<IEnumerable<Product>> GetBySkinTypeAsync(SkinType skinType)
    {
        return await _dbSet
            .Where(p => p.SkinType == skinType || p.SkinType == SkinType.All)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetExpiringSoonAsync(int days)
    {
        var warningDate = DateTime.UtcNow.AddDays(days);
        return await _dbSet
            .Where(p => p.ExpiryDate.HasValue 
                        && p.ExpiryDate.Value <= warningDate 
                        && p.ExpiryDate.Value > DateTime.UtcNow)
            .OrderBy(p => p.ExpiryDate)
            .ToListAsync();
    }
}
```

---

### 2. Strategy Pattern

> **Mục đích**: Cho phép thay đổi thuật toán (chiến lược) trong runtime

| File | Chiến lược |
|------|------------|
| `IPricingStrategy.cs` | Interface chung |
| `StandardPricingStrategy.cs` | Giữ nguyên giá |
| `VipPricingStrategy.cs` | Giảm 10% cho VIP |
| `SalePricingStrategy.cs` | Giảm giá theo chương trình |

```csharp
// Thay đổi Strategy trong DI Container
builder.Services.AddScoped<IPricingStrategy, VipPricingStrategy>();  // Cho VIP
// hoặc
builder.Services.AddScoped<IPricingStrategy, SalePricingStrategy>(); // Khi sale
```

---

### 3. Factory Pattern

> **Mục đích**: Tạo object mà không cần biết class cụ thể

**File**: `CosmeticStore.Infrastructure/Services/PaymentFactory.cs`

```csharp
public class PaymentFactory
{
    public IPaymentService GetPaymentService(string paymentMethod)
    {
        return paymentMethod.ToUpper() switch
        {
            "MOMO" => new MomoPaymentService(),
            "COD" => new CodPaymentService(),
            // Dễ dàng thêm: "VNPAY" => new VnPayPaymentService(),
            _ => throw new ArgumentException("Phương thức không hỗ trợ")
        };
    }
}
```

---

## 📈 Giai đoạn phát triển

### ✅ Giai đoạn 1: Tầng Dữ Liệu & Truy Vấn (Repository Pattern)

> **Mục tiêu**: Hoàn thành chức năng Quản lý sản phẩm & Truy vấn dữ liệu

#### Bước 1.1: Định nghĩa Interface trong Core ✅

| File | Mô tả |
|------|-------|
| `IGenericRepository<T>` | Interface CRUD cơ bản cho mọi Entity |
| `IProductRepository` | Kế thừa Generic, thêm method đặc thù |
| `SkinType.cs` | Enum loại da (Oily, Dry, Sensitive...) |

#### Bước 1.2: Triển khai trong Infrastructure ✅

| File | Mô tả | Logic OOP |
|------|-------|-----------|
| `GenericRepository.cs` | Implement CRUD với EF Core LINQ | Class cha, tái sử dụng |
| `ProductRepository.cs` | Implement query đặc thù | Kế thừa, che giấu SQL |

**Các method đặc thù trong ProductRepository**:

```csharp
// Lọc sản phẩm theo loại da - Hỗ trợ AI Skin Quiz
Task<IEnumerable<Product>> GetBySkinTypeAsync(SkinType skinType);

// Lọc sản phẩm cận hạn - Expiry Management
Task<IEnumerable<Product>> GetExpiringSoonAsync(int days);

// Lấy sản phẩm Flash Sale
Task<IEnumerable<Product>> GetFlashSaleProductsAsync();

// Tìm kiếm nâng cao với nhiều điều kiện
Task<IEnumerable<Product>> AdvancedSearchAsync(
    string? keyword, SkinType? skinType, string? brand,
    string? category, decimal? minPrice, decimal? maxPrice,
    int pageNumber, int pageSize);
```

#### Bước 1.3: Cấu hình DI trong Program.cs ✅

```csharp
// Đăng ký Generic Repository cho các Entity chung
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Đăng ký Product Repository với các method đặc thù
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

---

### ✅ Giai đoạn 2: Tính giá & Khuyến mãi (Strategy + Decorator Pattern)

> **Mục tiêu**: Hoàn thành chức năng Tính giá, Khuyến mãi, Quản lý hạn sử dụng

#### Bước 2.1: Strategy Pattern - Chiến lược giá gốc ✅

| File | Mô tả | OOP |
|------|-------|-----|
| `VipLevel.cs` | Enum cấp độ VIP (None, Bronze, Silver, Gold, Platinum) | - |
| `User.cs` | Entity người dùng với VipLevel, SkinType | **Encapsulation** |
| `IPricingStrategy.cs` | Interface với `CalculatePrice(Product, User)` | **Abstraction** |
| `StandardPricingStrategy.cs` | Giữ nguyên giá gốc | **Polymorphism** |
| `VipPricingStrategy.cs` | Giảm theo VipLevel (5%-20%) | **Polymorphism** |
| `SkinTypePricingStrategy.cs` | Giảm 5% khi loại da phù hợp | **Polymorphism** |

**Bảng giảm giá VIP:**

| VipLevel | Chi tiêu tích lũy | Giảm giá |
|----------|-------------------|----------|
| None | < 1,000,000 VND | 0% |
| Bronze | ≥ 1,000,000 VND | 5% |
| Silver | ≥ 5,000,000 VND | 10% |
| Gold | ≥ 10,000,000 VND | 15% |
| Platinum | ≥ 20,000,000 VND | 20% |

#### Bước 2.2: Decorator Pattern - Cộng dồn khuyến mãi ✅

| File | Mô tả | Giảm giá |
|------|-------|----------|
| `PriceDecorator.cs` | Abstract class chứa IPricingStrategy bên trong | **Decorator Base** |
| `ExpiryDiscountDecorator.cs` | Giảm giá sản phẩm cận hạn | ≤7d: 40%, ≤14d: 25%, ≤30d: 15% |
| `FlashSaleDecorator.cs` | Giảm giá Flash Sale | Theo Product.FlashSaleDiscount |
| `CouponDecorator.cs` | Giảm giá theo mã | % hoặc số tiền cố định |

**Ví dụ cộng dồn giảm giá:**

```
Giá gốc: 100,000 VND
├── VipPricingStrategy (Gold -15%): 85,000 VND
├── ExpiryDiscountDecorator (≤14d -25%): 63,750 VND
├── FlashSaleDecorator (-20%): 51,000 VND
└── CouponDecorator (-10%): 45,900 VND

→ Giá cuối: 45,900 VND (Giảm 54.1%)
```

#### Bước 2.3: Pricing Service - Orchestrator ✅

| File | Mô tả |
|------|-------|
| `IPricingService.cs` | Interface với `CalculateFinalPrice()`, `BuildPricingChain()` |
| `PricingService.cs` | Tự động chọn Strategy và wrap Decorator phù hợp |

**Luồng xử lý của PricingService:**

```csharp
// Input: Product + User + CouponCode
var result = pricingService.CalculateFinalPrice(product, user, "SALE20");

// Output: PricingResult
// - OriginalPrice: 100,000
// - FinalPrice: 45,900
// - TotalDiscountPercent: 54.1%
// - AppliedDiscounts: [VIP, Expiry, FlashSale, Coupon]
// - Warnings: ["Sản phẩm sắp hết hạn trong 10 ngày"]
```

---

### ✅ Giai đoạn 3: Xử lý Đơn hàng (Command Pattern)

> **Mục tiêu**: Hoàn thành chức năng Đặt hàng, Thanh toán

#### Bước 3.1: Tách biệt Request và Handler ✅

| File | Mô tả | Chức năng |
|------|-------|-----------|
| `OrderStatus.cs` | Enum trạng thái đơn hàng | Pending → Confirmed → Paid → Shipping → Completed |
| `PaymentMethod.cs` | Enum phương thức thanh toán | COD, Momo, VNPay, ZaloPay |
| `Order.cs` | Entity đơn hàng - Aggregate Root | **Encapsulation**: Logic nghiệp vụ trong class |
| `OrderItem.cs` | Entity chi tiết đơn hàng | Snapshot giá, số lượng |

**Command Pattern - Các Command đã tạo:**

| Command | Input | Output | Mô tả |
|---------|-------|--------|-------|
| `CreateOrderCommand` | UserId, Items, Address, PaymentMethod | OrderId, OrderNumber | Tạo đơn hàng mới |
| `CancelOrderCommand` | OrderId, Reason | RefundAmount | Hủy đơn hàng |
| `ConfirmOrderCommand` | OrderId, ShippingFee | TotalAmount | Xác nhận đơn hàng |
| `PayOrderCommand` | OrderId, PaymentMethod | TransactionId, PaymentUrl | Thanh toán |

**Cấu trúc Command Pattern:**

```
┌─────────────────────────────────────────────────────────────────┐
│                         ICommand<T>                             │
│                    (Interface chung)                            │
└──────────────────────────┬──────────────────────────────────────┘
                           │ Implement
           ┌───────────────┼───────────────┬───────────────────────┐
           ▼               ▼               ▼                       ▼
┌──────────────────┐ ┌──────────────────┐ ┌──────────────────┐ ┌──────────────────┐
│ CreateOrder      │ │   CancelOrder    │ │  ConfirmOrder    │ │    PayOrder      │
│   Command        │ │   Command        │ │    Command       │ │    Command       │
├──────────────────┤ ├──────────────────┤ ├──────────────────┤ ├──────────────────┤
│ - UserId         │ │ - OrderId        │ │ - OrderId        │ │ - OrderId        │
│ - Items[]       │ │ - Reason         │ │ - ShippingFee    │ │ - PaymentMethod  │
│ - Address        │ │ - CancelledBy    │ │ - AdminNotes     │ │ - ReturnUrl      │
│ - PaymentMethod  │ └──────────────────┘ └──────────────────┘ └──────────────────┘
└──────────────────┘

                    ┌─────────────────────────────────────────┐
                    │      ICommandHandler<TCommand, TResult> │
                    │              (Xử lý logic)              │
                    └─────────────────────────────────────────┘
```

**Ví dụ sử dụng Command:**

```csharp
// Tạo Command với dữ liệu
var command = new CreateOrderCommand(
    userId: 1,
    items: new[] { new OrderItemInput { ProductId = 5, Quantity = 2 } },
    shippingAddress: "123 Nguyễn Văn A, Q.1, TP.HCM",
    shippingPhone: "0901234567",
    receiverName: "Nguyễn Văn A",
    paymentMethod: PaymentMethod.Momo,
    couponCode: "SALE20"
);

// Gửi đến Handler xử lý
var result = await _handler.HandleAsync(command);

// Kết quả
if (result.IsSuccess)
{
    Console.WriteLine($"Đơn hàng {result.Data.OrderNumber} đã tạo thành công!");
    Console.WriteLine($"Tổng tiền: {result.Data.TotalAmount:N0} VND");
}
```

---

#### Bước 3.2: Command Handlers (Single Responsibility) ✅

| Handler | Input Command | Workflow | Output |
|---------|---------------|----------|--------|
| `CreateOrderCommandHandler` | `CreateOrderCommand` | Validate User → Validate Stock → Tính giá (PricingService) → Trừ kho → Lưu DB → Log | `CreateOrderResult` |
| `CancelOrderCommandHandler` | `CancelOrderCommand` | Validate Order → Check status → Hoàn kho → Update status → Log | `CancelOrderResult` |
| `ConfirmOrderCommandHandler` | `ConfirmOrderCommand` | Validate Order → Set shipping → Confirm → Log | `ConfirmOrderResult` |
| `PayOrderCommandHandler` | `PayOrderCommand` | Validate → Factory tạo Payment Service → Process → Update → Log | `PayOrderResult` |

**Single Responsibility Principle:**

```
┌─────────────────────────────────────────────────────────────────────┐
│                   CreateOrderCommandHandler                         │
│                 (Chỉ làm 1 việc: Tạo đơn hàng)                      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  1. Validate User         → IGenericRepository<User>                │
│  2. Validate Products     → IProductRepository                      │
│  3. Tính giá             → IPricingService (Strategy + Decorator)  │
│  4. Trừ tồn kho          → Product.UpdateStock() (Encapsulation)   │
│  5. Tạo Order            → Order Entity (Domain Logic)              │
│  6. Lưu Database         → IOrderRepository                         │
│  7. Ghi Log              → IAppLogger (Singleton)                   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

**Kết hợp các Pattern:**

```csharp
// Handler tạo đơn hàng - Kết hợp tất cả Pattern
public class CreateOrderCommandHandler
{
    // Repository Pattern
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    
    // Strategy + Decorator Pattern (Phase 2)
    private readonly IPricingService _pricingService;
    
    // Singleton Pattern
    private readonly IAppLogger _logger;
    
    public async Task<CommandResult<CreateOrderResult>> HandleAsync(CreateOrderCommand command)
    {
        // Tính giá (Strategy + Decorator)
        var pricingResult = _pricingService.CalculateFinalPrice(product, user, couponCode);
        
        // Encapsulation - Trừ kho qua method trong Entity
        product.UpdateStock(-quantity);
        
        // Domain Logic trong Entity
        var order = new Order(userId, address, phone, name, paymentMethod);
        order.AddItem(orderItem);
        
        // Ghi Log (Singleton)
        _logger.LogOrderActivity(order.Id, "CREATE", details);
        
        return CommandResult<CreateOrderResult>.Success(result);
    }
}
```

---

#### Bước 3.3: Thanh toán đa kênh (Factory Pattern) ✅

| File | Mô tả | Gateway |
|------|-------|---------|
| `IPaymentGateway.cs` | Interface cổng thanh toán | Base Interface |
| `MomoGateway.cs` | Cổng Momo | QR, Deep Link |
| `ZaloPayGateway.cs` | Cổng ZaloPay | QR, Deep Link |
| `VNPayGateway.cs` | Cổng VNPay | Redirect URL |
| `CODGateway.cs` | Thanh toán khi nhận hàng | Không online |
| `PaymentGatewayFactory.cs` | Factory tạo Gateway | Factory Pattern |

**Factory Pattern - Workflow:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        PaymentGatewayFactory                            │
│                     (Tạo đúng Gateway từ string)                        │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  CreateGateway("MOMO")     ────────────► new MomoGateway()              │
│  CreateGateway("ZALOPAY")  ────────────► new ZaloPayGateway()           │
│  CreateGateway("VNPAY")    ────────────► new VNPayGateway()             │
│  CreateGateway("COD")      ────────────► new CODGateway()               │
│                                                                         │
│  Tất cả đều trả về IPaymentGateway (Polymorphism)                       │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**Ví dụ sử dụng:**

```csharp
// Controller hoặc Handler
var factory = new PaymentGatewayFactory();

// Tạo gateway từ string (từ request của user)
IPaymentGateway gateway = factory.CreateGateway("MOMO");

// Gọi ProcessPaymentAsync - Polymorphism
// Client không biết đây là MomoGateway
var result = await gateway.ProcessPaymentAsync(new PaymentRequest
{
    OrderId = "123",
    OrderNumber = "ORD20260117001",
    Amount = 500000,
    Description = "Thanh toán đơn hàng mỹ phẩm",
    ReturnUrl = "https://mysite.com/payment/callback"
});

// Xử lý kết quả
if (result.IsSuccess)
{
    // Redirect đến cổng thanh toán
    return Redirect(result.PaymentUrl);
}
```

**Các cổng thanh toán được hỗ trợ:**

| Gateway | Mã | Online | QR Code | Deep Link |
|---------|-----|--------|---------|-----------|
| Momo | `MOMO` | ✅ | ✅ | ✅ |
| ZaloPay | `ZALOPAY` | ✅ | ✅ | ✅ |
| VNPay | `VNPAY` | ✅ | ❌ | ❌ |
| COD | `COD` | ❌ | ❌ | ❌ |

---

### ⏳ Giai đoạn tiếp theo (Đang phát triển)

| Giai đoạn | Mô tả | Pattern |
|-----------|-------|---------|
| **Giai đoạn 4** | Thông báo | Observer Pattern |
| **Giai đoạn 5** | Ghi log nâng cao | Singleton Pattern |

---

## 📋 Giải thích chi tiết các file

### 📂 CosmeticStore.Core (Tầng Domain)

| File | Mô tả | OOP/Pattern |
|------|-------|-------------|
| `Entities/BaseEntity.cs` | Class cha chứa Id, CreatedAt, IsDeleted | **Kế thừa** |
| `Entities/Product.cs` | Entity với logic UpdateStock, ActivateFlashSale | **Đóng gói** |
| `Entities/User.cs` | Entity người dùng với VipLevel, SkinType | **Đóng gói** |
| `Entities/Order.cs` | Entity đơn hàng - Aggregate Root | **Đóng gói + Command** |
| `Entities/OrderItem.cs` | Entity chi tiết đơn hàng | **Đóng gói** |
| `Enums/SkinType.cs` | Enum loại da (Oily, Dry, Sensitive, Normal, Combination) | - |
| `Enums/VipLevel.cs` | Enum cấp VIP (None, Bronze, Silver, Gold, Platinum) | - |
| `Enums/OrderStatus.cs` | Enum trạng thái đơn hàng (Pending → Completed) | - |
| `Enums/PaymentMethod.cs` | Enum phương thức thanh toán (COD, Momo, VNPay...) | - |
| `Commands/ICommand.cs` | Interface và Base class cho Command | **Command** |
| `Commands/ICommandHandler.cs` | Interface Handler và CommandResult | **Command** |
| `Commands/Orders/CreateOrderCommand.cs` | Command tạo đơn hàng | **Command** |
| `Commands/Orders/CancelOrderCommand.cs` | Command hủy đơn hàng | **Command** |
| `Commands/Orders/ConfirmOrderCommand.cs` | Command xác nhận đơn hàng | **Command** |
| `Commands/Orders/PayOrderCommand.cs` | Command thanh toán | **Command** |
| `Interfaces/IGenericRepository.cs` | Interface CRUD cơ bản | **Trừu tượng** |
| `Interfaces/IProductRepository.cs` | Interface đặc thù cho Product | **Kế thừa** |
| `Interfaces/IOrderRepository.cs` | Interface đặc thù cho Order | **Kế thừa** |
| `Interfaces/IPricingStrategy.cs` | Interface Strategy Pattern với Product, User | **Đa hình** |
| `Interfaces/IPriceDecorator.cs` | Abstract class cho Decorator Pattern | **Decorator** |
| `Interfaces/IPricingService.cs` | Interface Pricing Orchestrator | **Trừu tượng** |
| `Interfaces/IPaymentService.cs` | Interface Payment Services | **Trừu tượng** |
| `Interfaces/IPaymentGateway.cs` | Interface cổng thanh toán | **Factory** |
| `Interfaces/IAppLogger.cs` | Interface Logger (Singleton) | **Singleton** |

### 📂 CosmeticStore.Infrastructure (Tầng Hạ tầng)

| File | Mô tả | OOP/Pattern |
|------|-------|-------------|
| `DbContext/StoreDbContext.cs` | EF Core DbContext với Query Filter | - |
| `Repositories/GenericRepository.cs` | Implement IGenericRepository | **Repository** |
| `Repositories/ProductRepository.cs` | Implement IProductRepository | **Repository + Kế thừa** |
| `Repositories/OrderRepository.cs` | Implement IOrderRepository | **Repository + Kế thừa** |
| `Strategies/StandardPricingStrategy.cs` | Chiến lược giá thường (0%) | **Strategy** |
| `Strategies/VipPricingStrategy.cs` | Chiến lược VIP (5%-20%) | **Strategy** |
| `Strategies/SkinTypePricingStrategy.cs` | Chiến lược loại da (5%) | **Strategy** |
| `Strategies/SalePricingStrategy.cs` | Chiến lược khuyến mãi | **Strategy** |
| `Decorators/ExpiryDiscountDecorator.cs` | Giảm giá cận hạn (15%-40%) | **Decorator** |
| `Decorators/FlashSaleDecorator.cs` | Giảm giá Flash Sale | **Decorator** |
| `Decorators/CouponDecorator.cs` | Giảm giá mã coupon | **Decorator** |
| `Handlers/CreateOrderCommandHandler.cs` | Handler tạo đơn hàng | **Command + SRP** |
| `Handlers/CancelOrderCommandHandler.cs` | Handler hủy đơn hàng | **Command + SRP** |
| `Handlers/ConfirmOrderCommandHandler.cs` | Handler xác nhận đơn hàng | **Command + SRP** |
| `Handlers/PayOrderCommandHandler.cs` | Handler thanh toán | **Command + Factory** |
| `Services/PricingService.cs` | Orchestrator Strategy + Decorator | **Service** |
| `Services/AppLogger.cs` | Logger (Singleton qua DI) | **Singleton** |
| `Services/PaymentFactory.cs` | Factory tạo Payment Service | **Factory** |
| `Services/MomoPaymentService.cs` | Xử lý thanh toán Momo | **Đa hình** |
| `Services/CodPaymentService.cs` | Xử lý thanh toán COD | **Đa hình** |
| `Services/VnPayPaymentService.cs` | Xử lý thanh toán VNPay | **Đa hình** |
| `Services/ZaloPayPaymentService.cs` | Xử lý thanh toán ZaloPay | **Đa hình** |
| `Gateways/PaymentGatewayFactory.cs` | Factory tạo Payment Gateway | **Factory** |
| `Gateways/MomoGateway.cs` | Cổng thanh toán Momo (QR, DeepLink) | **Factory** |
| `Gateways/ZaloPayGateway.cs` | Cổng thanh toán ZaloPay | **Factory** |
| `Gateways/VNPayGateway.cs` | Cổng thanh toán VNPay | **Factory** |
| `Gateways/CODGateway.cs` | Thanh toán khi nhận hàng | **Factory** |

### 📂 CosmeticStore.API (Tầng Presentation)

| File | Mô tả | OOP/Pattern |
|------|-------|-------------|
| `Program.cs` | Entry point, cấu hình DI | **DI Container** |
| `Controllers/ProductsController.cs` | 30+ API endpoints | **Constructor Injection** |
| `ViewModels/ProductViewModels.cs` | DTOs, PaginatedResponse | - |

---

## 🚀 Hướng dẫn cài đặt

### Yêu cầu

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) hoặc LocalDB
- [Visual Studio Code](https://code.visualstudio.com/) + C# Dev Kit Extension

### Bước 1: Clone repository

```bash
git clone https://github.com/QuynYang/glow.aura.webapi.git
cd glow.aura.webapi
```

### Bước 2: Cấu hình Database

Mở file `CosmeticStore.API/appsettings.json` và cập nhật connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CosmeticStoreDb;Trusted_Connection=True;"
  }
}
```

### Bước 3: Restore packages & Migration

```bash
# Restore NuGet packages
dotnet restore

# Tạo Migration
dotnet ef migrations add InitialCreate --project CosmeticStore.Infrastructure --startup-project CosmeticStore.API

# Cập nhật Database
dotnet ef database update --project CosmeticStore.Infrastructure --startup-project CosmeticStore.API
```

### Bước 4: Chạy ứng dụng

```bash
dotnet run --project CosmeticStore.API
```

Truy cập Swagger UI: `http://localhost:5xxx/swagger`

---

## 📡 API Endpoints

### CRUD Cơ bản

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/api/products` | Lấy tất cả sản phẩm |
| `GET` | `/api/products/{id}` | Lấy sản phẩm theo ID |
| `POST` | `/api/products` | Tạo sản phẩm mới |
| `PUT` | `/api/products/{id}` | Cập nhật sản phẩm |
| `PATCH` | `/api/products/{id}/stock` | Cập nhật số lượng tồn kho |
| `DELETE` | `/api/products/{id}` | Xóa sản phẩm (soft delete) |

### Lọc theo Loại da (AI Skin Quiz)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/api/products/skin-type/{skinType}` | Lọc theo loại da |
| `GET` | `/api/products/skin-type/{skinType}/paged` | Lọc có phân trang |

### Quản lý Hạn sử dụng (Expiry Management)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/api/products/expiring-soon?days=30` | Sản phẩm sắp hết hạn |
| `GET` | `/api/products/expired` | Sản phẩm đã hết hạn |

### Flash Sale

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/api/products/flash-sale` | Lấy sản phẩm Flash Sale |
| `POST` | `/api/products/{id}/flash-sale` | Kích hoạt Flash Sale |
| `DELETE` | `/api/products/{id}/flash-sale` | Hủy Flash Sale |

### Lọc & Tìm kiếm

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/api/products/brand/{brand}` | Lọc theo thương hiệu |
| `GET` | `/api/products/category/{category}` | Lọc theo danh mục |
| `GET` | `/api/products/brands` | Danh sách thương hiệu |
| `GET` | `/api/products/categories` | Danh sách danh mục |
| `GET` | `/api/products/price-range` | Lọc theo khoảng giá |
| `GET` | `/api/products/search?keyword=` | Tìm kiếm |
| `POST` | `/api/products/advanced-search` | Tìm kiếm nâng cao |

### Quản lý Kho & Dashboard

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/api/products/low-stock?threshold=10` | Sản phẩm sắp hết hàng |
| `GET` | `/api/products/out-of-stock` | Sản phẩm hết hàng |
| `GET` | `/api/products/stats` | Thống kê Dashboard |

### Ví dụ Request

**Tạo sản phẩm mới:**

```http
POST /api/products
Content-Type: application/json

{
  "name": "Son môi MAC Ruby Woo",
  "description": "Son lì màu đỏ ruby kinh điển",
  "price": 450000,
  "stock": 100,
  "brand": "MAC",
  "category": "Son môi",
  "skinType": 0,
  "expiryDate": "2026-12-31",
  "ingredients": "Castor Oil, Beeswax, Carnauba Wax",
  "volume": "3g"
}
```

**Tìm kiếm nâng cao:**

```http
POST /api/products/advanced-search
Content-Type: application/json

{
  "keyword": "son",
  "skinType": 1,
  "brand": "MAC",
  "minPrice": 100000,
  "maxPrice": 500000,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

## 👨‍💻 Tác giả

- **Họ tên**: Vũ Ngọc Quỳnh Giang
- **MSSV**: 22DH114506
- **Môn học**: Mẫu Thiết Kế Phần Mềm

---

