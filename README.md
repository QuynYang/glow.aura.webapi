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
 ┃ ┃ ┣ 📄 AuthController.cs           # Đăng ký, Đăng nhập, JWT Token
 ┃ ┃ ┣ 📄 UserController.cs           # Quản lý User, Profile, Admin
 ┃ ┃ ┣ 📄 OrderController.cs          # CRUD Order với Command Pattern
 ┃ ┃ ┣ 📄 ProductsController.cs       # Controller quản lý sản phẩm (30+ endpoints)
 ┃ ┃ ┗ 📄 SkinQuizController.cs       # AI Skin Quiz endpoints
 ┃ ┣ 📂 ViewModels/
 ┃ ┃ ┣ 📄 AuthViewModels.cs           # Register, Login, Token DTOs
 ┃ ┃ ┣ 📄 OrderViewModels.cs          # Order Request/Response DTOs
 ┃ ┃ ┗ 📄 ProductViewModels.cs        # Product Request/Response models
 ┃ ┣ 📄 Program.cs                    # Entry point, cấu hình DI, JWT
 ┃ ┣ 📄 appsettings.json              # Cấu hình ứng dụng, JWT settings
 ┃ ┗ 📄 CosmeticStore.API.csproj
 ┃
 ┣ 📂 CosmeticStore.Core/             # Tầng Core (Domain Layer)
 ┃ ┣ 📂 Entities/
 ┃ ┃ ┣ 📄 BaseEntity.cs               # Class cha - Inheritance
 ┃ ┃ ┣ 📄 Product.cs                  # Entity sản phẩm - Encapsulation
 ┃ ┃ ┣ 📄 User.cs                     # Entity người dùng - VIP & SkinType
 ┃ ┃ ┣ 📄 Order.cs                    # Entity đơn hàng - Aggregate Root
 ┃ ┃ ┣ 📄 OrderItem.cs                # Entity chi tiết đơn hàng
 ┃ ┃ ┗ 📄 SystemLog.cs                # Entity log hệ thống - Singleton Logger
 ┃ ┣ 📂 Enums/
 ┃ ┃ ┣ 📄 SkinType.cs                 # Enum loại da (Oily, Dry, Sensitive...)
 ┃ ┃ ┣ 📄 VipLevel.cs                 # Enum cấp VIP (Bronze, Silver, Gold, Platinum)
 ┃ ┃ ┣ 📄 UserRole.cs                 # Enum vai trò (User, Staff, Admin)
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
 ┃ ┣ 📂 Builders/                     # Builder Pattern
 ┃ ┃ ┗ 📄 IOrderBuilder.cs            # Interface Builder + DTOs (CartItem, OrderBuildResult)
 ┃ ┣ 📂 SkinAnalysis/                 # Adapter Pattern - Skin Analysis
 ┃ ┃ ┗ 📄 SkinAnalysisResult.cs       # Value Object (Brightness, AcneCount, Recommendations...)
 ┃ ┣ 📂 Interfaces/
 ┃ ┃ ┣ 📄 IGenericRepository.cs       # Interface CRUD cơ bản
 ┃ ┃ ┣ 📄 IProductRepository.cs       # Interface đặc thù cho Product
 ┃ ┃ ┣ 📄 IOrderRepository.cs         # Interface đặc thù cho Order
 ┃ ┃ ┣ 📄 IPricingStrategy.cs         # Interface Strategy Pattern
 ┃ ┃ ┣ 📄 IPriceDecorator.cs          # Abstract class Decorator Pattern
 ┃ ┃ ┣ 📄 IPricingService.cs          # Interface Pricing Orchestrator
 ┃ ┃ ┣ 📄 IPaymentService.cs          # Interface Payment Services
 ┃ ┃ ┣ 📄 IPaymentGateway.cs          # Interface cổng thanh toán (Factory)
 ┃ ┃ ┣ 📄 IAppLogger.cs               # Interface Logger (Singleton)
 ┃ ┃ ┣ 📄 ISystemLogger.cs            # Interface Logger nâng cao (5 levels)
 ┃ ┃ ┣ 📄 IAuthService.cs             # Interface Authentication (JWT)
 ┃ ┃ ┗ 📂 Notifications/              # Abstract Factory Pattern
 ┃ ┃   ┣ 📄 IEmailService.cs          # Abstract Product (Email)
 ┃ ┃   ┣ 📄 ISmsService.cs            # Abstract Product (SMS)
 ┃ ┃   ┣ 📄 INotificationFactory.cs   # Abstract Factory
 ┃ ┃   ┗ 📄 INotificationFactoryProvider.cs # Factory Selector
 ┃ ┣ 📂 Events/                       # Observer Pattern - Domain Events
 ┃ ┃ ┣ 📄 IDomainEvent.cs             # Interface + Base class
 ┃ ┃ ┣ 📄 IDomainEventHandler.cs      # Interface Handler + INotificationService
 ┃ ┃ ┣ 📄 OrderEvents.cs              # Order-related events
 ┃ ┃ ┣ 📄 ProductEvents.cs            # Product-related events
 ┃ ┃ ┗ 📄 ReviewEvents.cs             # Review-related events
 ┃ ┣ 📂 SkinQuiz/                     # AI Skin Quiz (Giai đoạn 5)
 ┃ ┃ ┣ 📄 SkinQuizModels.cs           # DTOs + SkinTypeInfo chi tiết
 ┃ ┃ ┗ 📄 SkinQuizQuestions.cs        # 10 câu hỏi với điểm số
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
 ┃ ┃ ┣ 📄 CreateOrderWithBuilderHandler.cs # Tạo đơn hàng (Builder Pattern)
 ┃ ┃ ┣ 📄 CancelOrderCommandHandler.cs  # Hủy đơn hàng
 ┃ ┃ ┣ 📄 ConfirmOrderCommandHandler.cs # Xác nhận đơn hàng
 ┃ ┃ ┗ 📄 PayOrderCommandHandler.cs     # Thanh toán (dùng Factory)
 ┃ ┣ 📂 Builders/                      # Builder Pattern
 ┃ ┃ ┗ 📄 OrderBuilder.cs              # Concrete Builder (Fluent Interface)
 ┃ ┣ 📂 Gateways/                     # Factory Pattern - Payment Gateways
 ┃ ┃ ┣ 📄 PaymentGatewayFactory.cs    # Factory tạo Gateway từ string
 ┃ ┃ ┣ 📄 MomoGateway.cs              # Cổng Momo (QR, DeepLink)
 ┃ ┃ ┣ 📄 ZaloPayGateway.cs           # Cổng ZaloPay
 ┃ ┃ ┣ 📄 VNPayGateway.cs             # Cổng VNPay
 ┃ ┃ ┗ 📄 CODGateway.cs               # Thanh toán khi nhận hàng
 ┃ ┣ 📂 Events/                       # Observer Pattern
 ┃ ┃ ┗ 📄 DomainEventDispatcher.cs    # Trung tâm phân phối events
 ┃ ┣ 📂 Services/
 ┃ ┃ ┣ 📄 PricingService.cs           # Orchestrator Strategy + Decorator
 ┃ ┃ ┣ 📄 AppLogger.cs                # Logger (Singleton qua DI)
 ┃ ┃ ┣ 📄 SystemLogger.cs             # Logger nâng cao (File + DB, Batch Write)
 ┃ ┃ ┣ 📄 NotificationService.cs      # Gửi Email/SMS/Push/Admin Alert
 ┃ ┃ ┣ 📄 PaymentFactory.cs           # Factory tạo Payment Service (Legacy)
 ┃ ┃ ┣ 📄 MomoPaymentService.cs       # Thanh toán Momo
 ┃ ┃ ┣ 📄 CodPaymentService.cs        # Thanh toán COD
 ┃ ┃ ┣ 📄 VnPayPaymentService.cs      # Thanh toán VNPay
 ┃ ┃ ┣ 📄 ZaloPayPaymentService.cs    # Thanh toán ZaloPay
 ┃ ┃ ┣ 📄 SkinQuizService.cs          # AI phân tích loại da
 ┃ ┃ ┗ 📄 AuthService.cs              # JWT Token + Password Hash (PBKDF2)
 ┃ ┣ 📂 Handlers/Notifications/       # Observer Pattern - Handlers
 ┃ ┃ ┣ 📄 EmailNotificationHandler.cs # Handler gửi Email
 ┃ ┃ ┣ 📄 SmsNotificationHandler.cs   # Handler gửi SMS
 ┃ ┃ ┣ 📄 AdminAlertHandler.cs        # Handler thông báo Admin
 ┃ ┃ ┗ 📄 VipAwareNotificationHandler.cs # Abstract Factory handlers
 ┃ ┣ 📂 Services/Notifications/      # Abstract Factory Pattern
 ┃ ┃ ┣ 📄 LuxuryEmailService.cs       # Concrete Product (VIP Email)
 ┃ ┃ ┣ 📄 LuxurySmsService.cs         # Concrete Product (VIP SMS)
 ┃ ┃ ┣ 📄 StandardEmailService.cs     # Concrete Product (Normal Email)
 ┃ ┃ ┣ 📄 StandardSmsService.cs       # Concrete Product (Normal SMS)
 ┃ ┃ ┣ 📄 LuxuryNotificationFactory.cs    # Concrete Factory (VIP)
 ┃ ┃ ┣ 📄 StandardNotificationFactory.cs  # Concrete Factory (Standard)
 ┃ ┃ ┗ 📄 NotificationFactoryProvider.cs  # Factory Selector
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

### ✅ Giai đoạn 4: Hệ thống phản hồi (Singleton + Observer Pattern)

**Mục tiêu:** Hoàn thành chức năng 6️⃣ (Thông báo), 8️⃣ (Log), 1️⃣2️⃣ (Review).

#### Bước 4.1: System Logger (Singleton Pattern) ✅

| File | Mô tả | Tính năng |
|------|-------|-----------|
| `ISystemLogger.cs` | Interface Logger mở rộng | 5 Log Levels, Business Logging |
| `SystemLog.cs` | Entity lưu log trong DB | Factory Methods, Encapsulation |
| `SystemLogger.cs` | Singleton implementation | File + DB Logging, Batch Write |

**Singleton Pattern - Workflow:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         SINGLETON PATTERN                               │
│              Toàn hệ thống chỉ có 1 SystemLogger instance               │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│   ┌─────────────┐     ┌─────────────┐     ┌─────────────┐               │
│   │ Controller  │     │   Handler   │     │   Service   │               │
│   └──────┬──────┘     └──────┬──────┘     └──────┬──────┘               │
│          │                   │                   │                       │
│          └───────────────────┴───────────────────┘                       │
│                              │                                           │
│                              ▼                                           │
│                     ┌─────────────────┐                                  │
│                     │  ISystemLogger  │ ◄── DI Container (Singleton)    │
│                     └────────┬────────┘                                  │
│                              │                                           │
│                              ▼                                           │
│                     ┌─────────────────┐                                  │
│                     │  SystemLogger   │ (1 instance duy nhất)            │
│                     └────────┬────────┘                                  │
│                              │                                           │
│              ┌───────────────┼───────────────┐                           │
│              ▼               ▼               ▼                           │
│     ┌────────────────┐ ┌────────────┐ ┌──────────────┐                  │
│     │   File Log     │ │  Database  │ │   Console    │                  │
│     │ system-*.log   │ │ SystemLogs │ │  (Dev only)  │                  │
│     └────────────────┘ └────────────┘ └──────────────┘                  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**Log Levels:**

| Level | Method | Mô tả | Khi nào dùng |
|-------|--------|-------|--------------|
| Debug | `LogDebug()` | Chi tiết phát triển | Development only |
| Info | `LogInfo()` | Thông tin thường | Hoạt động bình thường |
| Warning | `LogWarning()` | Cảnh báo | Hành vi không mong muốn |
| Error | `LogError()` | Lỗi | Exception xảy ra |
| Critical | `LogCritical()` | Nghiêm trọng | Hệ thống gặp sự cố |

**Business Activity Logging:**

```csharp
// Đăng ký Singleton trong Program.cs
builder.Services.AddSingleton<ISystemLogger, SystemLogger>();

// Inject và sử dụng
public class OrderHandler
{
    private readonly ISystemLogger _logger;

    public OrderHandler(ISystemLogger logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(CreateOrderCommand command)
    {
        // Log hoạt động đơn hàng
        _logger.LogOrderActivity(
            orderId: order.Id,
            activityType: OrderActivityType.Created,
            details: $"Đơn hàng {order.OrderNumber} được tạo",
            userId: command.UserId
        );

        // Log thanh toán
        _logger.LogPaymentActivity(
            orderId: order.Id,
            paymentMethod: "MOMO",
            status: PaymentActivityStatus.Success,
            amount: 500000,
            transactionId: "MOMO123456"
        );

        // Log sản phẩm
        _logger.LogProductActivity(
            productId: product.Id,
            activityType: ProductActivityType.StockUpdated,
            details: "Trừ 5 sản phẩm",
            userId: userId
        );
    }
}
```

**Tính năng nổi bật:**

| Tính năng | Mô tả |
|-----------|-------|
| **File Logging** | Ghi vào `logs/system-yyyy-MM-dd.log` |
| **DB Logging** | Lưu vào bảng `SystemLogs` để query |
| **Batch Writing** | Queue 50 logs rồi ghi 1 lần |
| **Thread-safe** | ConcurrentQueue cho multi-thread |
| **Auto Flush** | Tự động ghi mỗi 5 giây |
| **Query Support** | GetLogsByDate, SearchLogs... |

---

#### Bước 4.2: Observer Pattern (Domain Events) ✅

**OBSERVER PATTERN** - Cơ chế lắng nghe và phản hồi sự kiện trong hệ thống.

| File | Mô tả | Pattern |
|------|-------|---------|
| `IDomainEvent.cs` | Interface Event + Base class | Observer |
| `OrderEvents.cs` | Events: Created, Confirmed, Cancelled, Payment... | Observer |
| `ProductEvents.cs` | Events: Expiring, LowStock, FlashSale... | Observer |
| `ReviewEvents.cs` | Events: Created, Reported, Approved... | Observer |
| `IDomainEventHandler.cs` | Interface Handler + INotificationService | Observer |
| `DomainEventDispatcher.cs` | Trung tâm phân phối events | Observer |
| `NotificationService.cs` | Gửi Email/SMS/Push/Admin Alert | Observer |
| `EmailNotificationHandler.cs` | Handler gửi Email | Observer |
| `SmsNotificationHandler.cs` | Handler gửi SMS | Observer |
| `AdminAlertHandler.cs` | Handler thông báo Admin | Observer |

**Observer Pattern - Workflow:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         OBSERVER PATTERN                                │
│                       (Domain Events Flow)                              │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│   ┌─────────────────────┐                                               │
│   │ CreateOrderHandler  │ ── raise ──► OrderCreatedEvent                │
│   └─────────────────────┘                    │                          │
│                                              ▼                          │
│                                 ┌────────────────────────┐              │
│                                 │ DomainEventDispatcher  │              │
│                                 │   (Subject/Publisher)  │              │
│                                 └───────────┬────────────┘              │
│                                             │                           │
│              ┌──────────────────────────────┼───────────────────┐       │
│              │                              │                   │       │
│              ▼                              ▼                   ▼       │
│   ┌──────────────────┐        ┌──────────────────┐  ┌─────────────────┐ │
│   │ EmailHandler     │        │  SmsHandler      │  │ AdminHandler    │ │
│   │ (Observer 1)     │        │  (Observer 2)    │  │ (Observer 3)    │ │
│   └────────┬─────────┘        └────────┬─────────┘  └────────┬────────┘ │
│            │                           │                     │          │
│            ▼                           ▼                     ▼          │
│      📧 Send Email              📱 Send SMS           🚨 Alert Admin   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**Các Events được định nghĩa:**

| Event | Mô tả | Handlers |
|-------|-------|----------|
| `OrderCreatedEvent` | Đơn hàng được tạo | Email, SMS |
| `OrderConfirmedEvent` | Đơn hàng xác nhận | SMS |
| `OrderCancelledEvent` | Đơn hàng bị hủy | Email |
| `OrderDeliveredEvent` | Giao hàng thành công | SMS |
| `PaymentSuccessEvent` | Thanh toán thành công | Email |
| `PaymentFailedEvent` | Thanh toán thất bại | SMS, Admin |
| `ProductExpiringSoonEvent` | Sản phẩm sắp hết hạn | Admin |
| `ProductLowStockEvent` | Sản phẩm sắp hết hàng | Admin |
| `FlashSaleActivatedEvent` | Kích hoạt Flash Sale | Admin |
| `ReviewCreatedEvent` | Review mới | Admin |
| `ReviewReportedEvent` | Review bị báo cáo | Admin |

**Ví dụ sử dụng:**

```csharp
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IDomainEventDispatcher _eventDispatcher;

    public async Task<CommandResult<CreateOrderResult>> HandleAsync(CreateOrderCommand command)
    {
        // 1. Tạo đơn hàng...
        var order = new Order(...);

        // 2. Raise event - Tất cả handlers tự động được gọi
        await _eventDispatcher.PublishAsync(new OrderCreatedEvent(
            orderId: order.Id,
            orderNumber: order.OrderNumber,
            userId: user.Id,
            userEmail: user.Email,
            userPhone: user.PhoneNumber,
            userName: user.FullName,
            totalAmount: order.TotalAmount,
            itemCount: order.ItemCount,
            shippingAddress: order.ShippingAddress,
            paymentMethod: order.PaymentMethod
        ));

        // → EmailNotificationHandler nhận event → Gửi email
        // → SmsNotificationHandler nhận event → Gửi SMS
        // → Không cần biết có bao nhiêu handlers

        return CommandResult<CreateOrderResult>.Success(result);
    }
}
```

**Lợi ích Observer Pattern:**

| Lợi ích | Mô tả |
|---------|-------|
| **Loose Coupling** | Handler không biết Entity, Entity không biết Handler |
| **Single Responsibility** | Mỗi handler chỉ làm 1 việc (SRP) |
| **Open/Closed** | Thêm handler mới không sửa code cũ (OCP) |
| **Extensibility** | Dễ dàng thêm notification channels mới |
| **Testability** | Test từng handler độc lập |

---

### ✅ Giai đoạn 5: AI & Tính năng nâng cao

**Mục tiêu:** Hoàn thành chức năng 9️⃣ (Skin Quiz), 🔟 (Try-on), 1️⃣1️⃣ (Expiry Automation).

#### Bước 5.1: AI Skin Quiz (Strategy Context) ✅

| File | Mô tả | Layer |
|------|-------|-------|
| `SkinQuiz/SkinQuizModels.cs` | DTOs + SkinTypeInfo chi tiết | Core |
| `SkinQuiz/SkinQuizQuestions.cs` | 10 câu hỏi với điểm số | Core |
| `ISkinQuizService.cs` | Interface Skin Quiz Service | Core |
| `SkinQuizService.cs` | Logic phân tích loại da | Infrastructure |
| `SkinQuizController.cs` | 6 API endpoints | API |

**AI Skin Quiz - Workflow:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        AI SKIN QUIZ SYSTEM                              │
│                   (Strategy Pattern Integration)                        │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  1️⃣ User trả lời 10 câu hỏi                                            │
│     ┌─────────────────────────────────────────┐                         │
│     │ Q1: Sau rửa mặt 30p, da thế nào?        │                         │
│     │ Q2: Lỗ chân lông trông ra sao?          │                         │
│     │ Q3: Có hay bị mụn không?                │                         │
│     │ ...                                     │                         │
│     │ Q10: Vấn đề lo lắng nhất?               │                         │
│     └─────────────────────────────────────────┘                         │
│                          │                                              │
│                          ▼                                              │
│  2️⃣ Hệ thống tính điểm cho mỗi loại da                                 │
│     ┌─────────────────────────────────────────┐                         │
│     │ Oily: 15 | Dry: 8 | Sensitive: 5        │                         │
│     │ Normal: 3 | Combination: 12             │                         │
│     │                                         │                         │
│     │ → Kết quả: DA DẦU (Oily) - 42% tin cậy  │                         │
│     └─────────────────────────────────────────┘                         │
│                          │                                              │
│                          ▼                                              │
│  3️⃣ Cập nhật User.SkinType = Oily                                      │
│                          │                                              │
│                          ▼                                              │
│  4️⃣ STRATEGY PATTERN tự động áp dụng                                   │
│     ┌─────────────────────────────────────────┐                         │
│     │ PricingService.CalculateFinalPrice()    │                         │
│     │                                         │                         │
│     │ if (user.SkinType == product.SkinType)  │                         │
│     │   → SkinTypePricingStrategy (5% OFF)    │                         │
│     └─────────────────────────────────────────┘                         │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**API Endpoints:**

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/api/skinquiz/questions` | Lấy 10 câu hỏi |
| `POST` | `/api/skinquiz/analyze` | Gửi trả lời, nhận kết quả |
| `GET` | `/api/skinquiz/skin-types` | Lấy tất cả loại da |
| `GET` | `/api/skinquiz/skin-types/{type}` | Chi tiết loại da |
| `GET` | `/api/skinquiz/recommendations/{type}` | Sản phẩm gợi ý |
| `GET` | `/api/skinquiz/status/{userId}` | Kiểm tra đã làm quiz |

**Kết quả phân tích bao gồm:**

| Field | Mô tả |
|-------|-------|
| `SkinTypeName` | Tên loại da (Da Dầu, Da Khô...) |
| `Description` | Mô tả chi tiết về loại da |
| `Characteristics` | Đặc điểm nhận dạng |
| `RecommendedIngredients` | Thành phần nên dùng |
| `IngredientsToAvoid` | Thành phần nên tránh |
| `SkincareTips` | Lời khuyên chăm sóc da |
| `ConfidencePercent` | Độ tin cậy kết quả |
| `MatchingProductCount` | Số sản phẩm phù hợp |
| `SkinTypeDiscountPercent` | 5% discount cho sản phẩm phù hợp |

**Ví dụ sử dụng:**

```http
POST /api/skinquiz/analyze
Content-Type: application/json

{
  "userId": 1,
  "answers": [
    { "questionId": 1, "selectedOptionId": "1a" },
    { "questionId": 2, "selectedOptionId": "2a" },
    { "questionId": 3, "selectedOptionId": "3a" },
    { "questionId": 4, "selectedOptionId": "4e" },
    { "questionId": 5, "selectedOptionId": "5a" },
    { "questionId": 6, "selectedOptionId": "6a" },
    { "questionId": 7, "selectedOptionId": "7a" },
    { "questionId": 8, "selectedOptionId": "8a" },
    { "questionId": 9, "selectedOptionId": "9a" },
    { "questionId": 10, "selectedOptionId": "10a" }
  ]
}
```

```json
{
  "skinType": "Oily",
  "skinTypeName": "Da Dầu",
  "description": "Da dầu tiết nhiều bã nhờn, đặc biệt ở vùng chữ T...",
  "characteristics": [
    "Tiết nhiều dầu, đặc biệt vùng chữ T",
    "Lỗ chân lông to, dễ thấy",
    "Dễ bị mụn đầu đen, mụn ẩn"
  ],
  "recommendedIngredients": [
    "Salicylic Acid (BHA)",
    "Niacinamide",
    "Tea Tree Oil"
  ],
  "skincareTips": [
    "Rửa mặt 2 lần/ngày với sữa rửa mặt dạng gel",
    "Đắp mặt nạ đất sét 1-2 lần/tuần"
  ],
  "confidencePercent": 42,
  "matchingProductCount": 15,
  "hasSkinTypeDiscount": true,
  "skinTypeDiscountPercent": 5
}
```

---

### ✅ Giai đoạn 6: Authentication & Authorization (JWT)

**Mục tiêu:** Hoàn thành chức năng Đăng ký, Đăng nhập, Phân quyền.

#### Bước 6.1: User Role & Entity ✅

| File | Mô tả |
|------|-------|
| `UserRole.cs` | Enum vai trò: User, Staff, Admin |
| `User.cs` | Thêm: Role, IsActive, LastLoginAt, RefreshToken |

**User Entity - Các property mới:**

```csharp
public class User : BaseEntity
{
    // ... existing properties ...
    
    // Authentication
    public UserRole Role { get; private set; } = UserRole.User;
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    
    // Methods
    public void RecordLogin() { ... }
    public void SetRefreshToken(string token, DateTime expiry) { ... }
    public void RevokeRefreshToken() { ... }
    public bool IsRefreshTokenValid(string token) { ... }
    public void ChangeRole(UserRole newRole) { ... }
    public bool IsAdmin => Role == UserRole.Admin;
    public bool IsStaffOrAdmin => Role >= UserRole.Staff;
}
```

#### Bước 6.2: Authentication Service ✅

| File | Mô tả |
|------|-------|
| `IAuthService.cs` | Interface Register, Login, JWT |
| `AuthService.cs` | Implementation với PBKDF2 + JWT |

**JWT Token Flow:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         JWT AUTHENTICATION FLOW                          │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  1️⃣ Login Request                                                       │
│     POST /api/auth/login { email, password }                            │
│                          │                                              │
│                          ▼                                              │
│  2️⃣ AuthService.LoginAsync()                                           │
│     ├── Validate email exists                                           │
│     ├── Verify password (PBKDF2)                                        │
│     ├── Generate Access Token (JWT, 1h)                                 │
│     ├── Generate Refresh Token (random, 7d)                             │
│     └── Save RefreshToken to User                                       │
│                          │                                              │
│                          ▼                                              │
│  3️⃣ Response                                                            │
│     { accessToken, refreshToken, expiresAt, user }                      │
│                          │                                              │
│                          ▼                                              │
│  4️⃣ Client lưu tokens, gửi kèm mỗi request                              │
│     Authorization: Bearer <accessToken>                                 │
│                          │                                              │
│                          ▼                                              │
│  5️⃣ JWT Middleware validate token                                       │
│     ├── Check signature                                                 │
│     ├── Check expiry                                                    │
│     ├── Extract claims (UserId, Role, VipLevel, SkinType)               │
│     └── Populate User.Identity                                          │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**JWT Token chứa các Claims:**

| Claim | Mô tả |
|-------|-------|
| `NameIdentifier` | User ID |
| `Email` | Email người dùng |
| `Name` | Họ tên |
| `Role` | Vai trò (User/Staff/Admin) |
| `VipLevel` | Cấp VIP |
| `SkinType` | Loại da |

#### Bước 6.3: Controllers với Authorization ✅

| Controller | Mô tả | Authorization |
|------------|-------|---------------|
| `AuthController` | Register, Login, RefreshToken, Logout | Public / [Authorize] |
| `UserController` | Profile, Admin quản lý users | [Authorize], [Authorize(Roles = "Admin")] |
| `OrderController` | CRUD Order + Command Pattern | [Authorize], [Authorize(Roles = "Admin,Staff")] |

**Ví dụ Authorization:**

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Tất cả endpoints cần đăng nhập
public class UserController : ControllerBase
{
    [HttpGet("me")]  // User tự xem profile
    public async Task<ActionResult<UserResponse>> GetCurrentUser() { ... }
    
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]  // Chỉ Admin/Staff
    public async Task<ActionResult> GetAllUsers() { ... }
    
    [HttpPatch("{id}/role")]
    [Authorize(Roles = "Admin")]  // Chỉ Admin
    public async Task<ActionResult> ChangeUserRole(int id) { ... }
}
```

---

### ✅ Giai đoạn 7: Abstract Factory Pattern (Notification System)

**Mục tiêu:** Hệ thống Notification Email/SMS theo "Family" (Họ sản phẩm/Khách hàng).

#### Vấn đề

Không chỉ gửi Email/SMS đơn thuần:
- **Khách VIP** (Gold/Platinum): Cần Email giao diện sang trọng (Gold template), SMS kiểu "Trợ lý cá nhân"
- **Khách thường** (None/Bronze/Silver): Email giao diện chuẩn, SMS tự động ngắn gọn

#### Giải pháp: Abstract Factory Pattern

Abstract Factory tạo ra một **họ các đối tượng** (Email + SMS) liên quan mà không cần chỉ định class cụ thể.

#### Bước 7.1: Abstract Products (Sản phẩm trừu tượng) ✅

| File | Mô tả | Layer |
|------|-------|-------|
| `IEmailService.cs` | Interface gửi Email với các method: SendEmailAsync, SendOrderConfirmationAsync... | Core |
| `ISmsService.cs` | Interface gửi SMS với các method: SendSmsAsync, SendOrderConfirmationSmsAsync... | Core |

```csharp
// Abstract Product - Email
public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task<bool> SendOrderConfirmationAsync(string to, string customerName, string orderNumber, decimal totalAmount);
    Task<bool> SendWelcomeEmailAsync(string to, string customerName);
    string TemplateName { get; }  // "Luxury Gold Template" hoặc "Standard Template"
}

// Abstract Product - SMS
public interface ISmsService
{
    Task<bool> SendSmsAsync(string phoneNumber, string message);
    Task<bool> SendOrderConfirmationSmsAsync(string phoneNumber, string customerName, string orderNumber, decimal totalAmount);
    string MessageStyle { get; }  // "Personal Assistant Style" hoặc "Standard Auto Style"
}
```

#### Bước 7.2: Abstract Factory (Nhà máy trừu tượng) ✅

| File | Mô tả | Layer |
|------|-------|-------|
| `INotificationFactory.cs` | Interface Factory tạo ra "họ" Email + SMS | Core |
| `INotificationFactoryProvider.cs` | Provider chọn Factory phù hợp theo VIP Level | Core |

```csharp
// Abstract Factory
public interface INotificationFactory
{
    IEmailService CreateEmailService();   // Tạo Email Service phù hợp
    ISmsService CreateSmsService();       // Tạo SMS Service phù hợp
    string FactoryName { get; }
}

// Factory Selector
public interface INotificationFactoryProvider
{
    INotificationFactory GetFactory(VipLevel vipLevel);
    INotificationFactory GetDefaultFactory();
    INotificationFactory GetLuxuryFactory();
}
```

#### Bước 7.3: Concrete Products (Sản phẩm cụ thể) ✅

| File | Mô tả | Template Style |
|------|-------|----------------|
| `LuxuryEmailService.cs` | Email template vàng sang trọng | Gold gradient, VIP badge |
| `LuxurySmsService.cs` | SMS phong cách trợ lý cá nhân | Kính gửi Quý khách... |
| `StandardEmailService.cs` | Email template chuẩn | Pink gradient, chuyên nghiệp |
| `StandardSmsService.cs` | SMS ngắn gọn tự động | Tiếng Việt không dấu |

**Ví dụ LuxuryEmailService (Gold Template):**

```html
<!-- Email VIP với Gold Template -->
<div class="header" style="background: linear-gradient(135deg, #D4AF37 0%, #F5E6A3 100%);">
    <span class="vip-badge">👑 VIP MEMBER</span>
    <h1>GlowAura Luxury</h1>
</div>
<div class="content">
    <p>Kính gửi Quý khách <strong style="color: #D4AF37;">Nguyễn Văn A</strong>,</p>
    <p>Chúng tôi vô cùng vinh hạnh được phục vụ Quý khách!</p>
    💎 Đội ngũ chăm sóc khách hàng VIP sẽ liên hệ trong vòng 30 phút
</div>
```

**Ví dụ StandardEmailService (Simple Template):**

```html
<!-- Email thường với Template chuẩn -->
<div class="header" style="background: #FF6B9D;">
    <h1>GlowAura</h1>
</div>
<div class="content">
    <p>Xin chào <strong>Nguyễn Văn A</strong>,</p>
    <p>Cảm ơn bạn đã đặt hàng tại GlowAura!</p>
</div>
```

#### Bước 7.4: Concrete Factories (Nhà máy cụ thể) ✅

| File | Mô tả | Tạo ra |
|------|-------|--------|
| `LuxuryNotificationFactory.cs` | Factory cho VIP | LuxuryEmailService + LuxurySmsService |
| `StandardNotificationFactory.cs` | Factory cho khách thường | StandardEmailService + StandardSmsService |
| `NotificationFactoryProvider.cs` | Chọn Factory theo VipLevel | Luxury/Standard Factory |

```csharp
// Concrete Factory - Luxury
public class LuxuryNotificationFactory : INotificationFactory
{
    public string FactoryName => "Luxury Notification Factory (VIP)";
    
    public IEmailService CreateEmailService()
        => new LuxuryEmailService(_logger);  // Gold Template
    
    public ISmsService CreateSmsService()
        => new LuxurySmsService(_logger);    // Personal Assistant Style
}

// Factory Provider - Chọn Factory theo VIP Level
public class NotificationFactoryProvider : INotificationFactoryProvider
{
    public INotificationFactory GetFactory(VipLevel vipLevel)
    {
        return vipLevel switch
        {
            VipLevel.Gold => _luxuryFactory,
            VipLevel.Platinum => _luxuryFactory,
            _ => _standardFactory
        };
    }
}
```

#### Bước 7.5: Tích hợp vào Event Handlers ✅

| File | Mô tả | Events Handled |
|------|-------|----------------|
| `VipAwareNotificationHandler.cs` | Handler sử dụng Abstract Factory | OrderCreated, UserRegistered, VipUpgraded, Promotion |

**Abstract Factory Pattern - Workflow:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                     ABSTRACT FACTORY PATTERN                             │
│                (Notification System by VIP Level)                        │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│   1️⃣ Event xảy ra: OrderCreatedEvent                                    │
│                          │                                              │
│                          ▼                                              │
│   2️⃣ VipAwareOrderCreatedHandler                                        │
│      - Lấy VipLevel từ event                                            │
│      - Gọi FactoryProvider.GetFactory(vipLevel)                         │
│                          │                                              │
│              ┌───────────┴───────────┐                                  │
│              ▼                       ▼                                  │
│   ┌─────────────────────┐  ┌─────────────────────┐                      │
│   │ Gold / Platinum     │  │ None / Bronze / Silver                     │
│   │        ↓            │  │        ↓            │                      │
│   │ LuxuryNotification  │  │ StandardNotification│                      │
│   │     Factory         │  │     Factory         │                      │
│   └─────────┬───────────┘  └─────────┬───────────┘                      │
│             │                        │                                  │
│   ┌─────────┴─────────┐    ┌─────────┴─────────┐                        │
│   ▼                   ▼    ▼                   ▼                        │
│ LuxuryEmail    LuxurySms  StandardEmail  StandardSms                    │
│ (Gold Template) (Personal) (Simple)      (Auto)                         │
│                                                                         │
│   3️⃣ Gửi notification với template phù hợp                              │
│      - VIP nhận email sang trọng + SMS cá nhân hóa                      │
│      - Khách thường nhận email chuẩn + SMS ngắn gọn                     │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**Ví dụ sử dụng trong Handler:**

```csharp
public class VipAwareOrderCreatedHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly INotificationFactoryProvider _factoryProvider;

    public async Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken ct)
    {
        // 1. Lấy VIP Level từ event
        var vipLevel = domainEvent.UserVipLevel;

        // 2. Abstract Factory: Chọn factory phù hợp
        var factory = _factoryProvider.GetFactory(vipLevel);
        // → Gold/Platinum: LuxuryNotificationFactory
        // → None/Bronze/Silver: StandardNotificationFactory

        // 3. Factory tạo Email Service (không biết Luxury hay Standard)
        var emailService = factory.CreateEmailService();
        
        // 4. Gửi email (template tự động theo factory)
        await emailService.SendOrderConfirmationAsync(
            domainEvent.UserEmail,
            domainEvent.UserName,
            domainEvent.OrderNumber,
            domainEvent.TotalAmount
        );
        // → VIP: Gold template với "Kính gửi Quý khách..."
        // → Normal: Simple template với "Xin chào..."

        // 5. Tương tự với SMS
        if (!string.IsNullOrEmpty(domainEvent.UserPhone))
        {
            var smsService = factory.CreateSmsService();
            await smsService.SendOrderConfirmationSmsAsync(...);
        }
    }
}
```

**Lợi ích Abstract Factory Pattern:**

| Lợi ích | Mô tả |
|---------|-------|
| **Family Products** | Email + SMS luôn đồng bộ theo cùng style (Luxury hoặc Standard) |
| **Open/Closed** | Thêm factory mới (PremiumNotificationFactory) không sửa code cũ |
| **Loose Coupling** | Handler không biết dùng Luxury hay Standard, chỉ biết interface |
| **Single Responsibility** | Mỗi factory chỉ tạo 1 family sản phẩm |
| **Consistency** | Đảm bảo VIP luôn nhận email + SMS VIP style |

**Cấu trúc thư mục Abstract Factory:**

```
📂 CosmeticStore.Core/Interfaces/Notifications/
├── 📄 IEmailService.cs           ← Abstract Product (Email)
├── 📄 ISmsService.cs             ← Abstract Product (SMS)
├── 📄 INotificationFactory.cs    ← Abstract Factory
└── 📄 INotificationFactoryProvider.cs ← Factory Selector

📂 CosmeticStore.Infrastructure/Services/Notifications/
├── 📄 LuxuryEmailService.cs      ← Concrete Product (VIP Email)
├── 📄 LuxurySmsService.cs        ← Concrete Product (VIP SMS)
├── 📄 StandardEmailService.cs    ← Concrete Product (Normal Email)
├── 📄 StandardSmsService.cs      ← Concrete Product (Normal SMS)
├── 📄 LuxuryNotificationFactory.cs    ← Concrete Factory (VIP)
├── 📄 StandardNotificationFactory.cs  ← Concrete Factory (Standard)
└── 📄 NotificationFactoryProvider.cs  ← Factory Selector Logic

📂 CosmeticStore.Infrastructure/Handlers/Notifications/
└── 📄 VipAwareNotificationHandler.cs  ← Handlers dùng Abstract Factory
```

---

### ✅ Giai đoạn 8: Builder Pattern (Order Construction)

**Mục tiêu:** Xây dựng đối tượng Order phức tạp từng bước.

#### Vấn đề

Class Order ngày càng phình to. Để tạo một Order hoàn chỉnh, cần:
- Set User (VIP Level, SkinType)
- Add danh sách Items (với giá đã tính qua Strategy + Decorator)
- Set địa chỉ giao hàng
- Chọn phương thức thanh toán
- Áp dụng Voucher (optional)
- Thêm ghi chú (optional)
- Gói quà (optional)
- Giao hàng nhanh (optional)

```csharp
// ❌ Constructor dài và dễ sai sót
var order = new Order(
    userId, address, phone, name, paymentMethod, 
    notes, couponCode, giftMessage, isExpress, shippingFee...
);
```

#### Giải pháp: Builder Pattern với Fluent Interface

```csharp
// ✅ Builder Pattern - Xây dựng từng bước, dễ đọc
var order = _orderBuilder
    .WithUser(currentUser)                                  // Step 1
    .WithItems(cartItems)                                   // Step 2 (tính giá)
    .WithShippingAddress(address, phone, name)              // Step 3
    .WithPaymentMethod(PaymentMethod.Momo)                  // Step 4
    .WithVoucher("SALE20")                                  // Optional
    .WithNotes("Giao giờ hành chính")                       // Optional
    .WithGiftWrap("Chúc mừng sinh nhật!", 25000)            // Optional
    .WithExpressDelivery()                                  // Optional
    .Build();                                               // Validate & Build
```

#### Bước 8.1: IOrderBuilder Interface (Core) ✅

| File | Mô tả | Layer |
|------|-------|-------|
| `IOrderBuilder.cs` | Interface với Fluent Interface | Core |
| `CartItem` | DTO cho item trong giỏ hàng | Core |
| `OrderBuildResult` | Kết quả build chi tiết | Core |
| `DiscountDetail` | Chi tiết một khoản giảm giá | Core |

**Interface IOrderBuilder:**

```csharp
public interface IOrderBuilder
{
    // Required steps
    IOrderBuilder WithUser(User user);
    IOrderBuilder WithUserId(int userId);
    IOrderBuilder WithItems(IEnumerable<CartItem> cartItems);
    IOrderBuilder WithShippingAddress(string address, string phone, string receiverName);
    IOrderBuilder WithPaymentMethod(PaymentMethod method);
    
    // Optional steps
    IOrderBuilder WithVoucher(string? voucherCode);
    IOrderBuilder WithNotes(string? notes);
    IOrderBuilder WithGiftWrap(string? giftMessage, decimal giftWrapFee = 0);
    IOrderBuilder WithShippingFee(decimal shippingFee);
    IOrderBuilder WithExpressDelivery(bool isExpress = true);
    
    // Build
    Order Build();
    bool CanBuild();
    IReadOnlyList<string> GetValidationErrors();
    IOrderBuilder Reset();
}
```

#### Bước 8.2: OrderBuilder Implementation (Infrastructure) ✅

| File | Mô tả | Kết hợp Pattern |
|------|-------|-----------------|
| `OrderBuilder.cs` | Concrete Builder implementation | Builder + Strategy + Decorator |

**Kết hợp các Pattern trong OrderBuilder:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                            OrderBuilder                                  │
│           (Builder Pattern + Strategy + Decorator + Encapsulation)       │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  WithUser(user)                                                          │
│  ├── Lưu User object để tính VIP discount                                │
│  └── Xác định VIP Level cho miễn phí ship                                │
│                                                                          │
│  WithItems(cartItems)                                                    │
│  ├── Load Product từ Repository                                          │
│  ├── Validate stock, expiry                                              │
│  ├── Tính giá qua PricingService (Strategy + Decorator)                  │
│  │   ├── VipPricingStrategy (5-20%)                                      │
│  │   ├── SkinTypePricingStrategy (5%)                                    │
│  │   ├── ExpiryDiscountDecorator (15-40%)                                │
│  │   ├── FlashSaleDecorator                                              │
│  │   └── CouponDecorator                                                 │
│  └── Tạo OrderItem với giá đã tính                                       │
│                                                                          │
│  WithShippingAddress(address, phone, name)                               │
│  └── Validate và lưu địa chỉ giao hàng                                   │
│                                                                          │
│  WithPaymentMethod(method)                                               │
│  └── Lưu phương thức thanh toán                                          │
│                                                                          │
│  WithVoucher(code) [Optional]                                            │
│  └── Áp dụng vào PricingService                                          │
│                                                                          │
│  Build()                                                                 │
│  ├── Validate required fields                                            │
│  ├── Validate cart items                                                 │
│  ├── Tính phí ship (miễn phí >= 500k hoặc VIP Gold+)                     │
│  ├── Tạo Order entity                                                    │
│  ├── Add OrderItems                                                      │
│  ├── Apply discount                                                      │
│  └── Return Order                                                        │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

#### Bước 8.3: Tích hợp vào CommandHandler ✅

| File | Mô tả | Cách dùng |
|------|-------|-----------|
| `CreateOrderWithBuilderHandler.cs` | Handler mới dùng Builder | Thay thế code dài dòng |

**So sánh trước và sau Builder:**

```csharp
// ❌ TRƯỚC: CreateOrderCommandHandler (80+ dòng logic)
public async Task<CommandResult<CreateOrderResult>> HandleAsync(CreateOrderCommand command)
{
    // 1. Validate User (5 dòng)
    // 2. Validate Items (10 dòng)
    // 3. Validate Products & Stock (30 dòng loop)
    // 4. Tính giá (10 dòng)
    // 5. Tạo Order (5 dòng)
    // 6. Thêm items (5 dòng loop)
    // 7. Tính ship (3 dòng)
    // 8. Lưu DB (5 dòng)
    // 9. Log (5 dòng)
    // ...
}

// ✅ SAU: CreateOrderWithBuilderHandler (~30 dòng)
public async Task<CommandResult<CreateOrderResult>> HandleAsync(CreateOrderCommand command)
{
    var user = await _userRepository.GetByIdAsync(command.UserId);
    var cartItems = await LoadCartItemsAsync(command.Items);
    
    // Builder Pattern: Xây dựng từng bước
    var order = _orderBuilder
        .Reset()
        .WithUser(user)
        .WithItems(cartItems)
        .WithShippingAddress(command.ShippingAddress, command.ShippingPhone, command.ReceiverName)
        .WithPaymentMethod(command.PaymentMethod)
        .WithVoucher(command.CouponCode)
        .WithNotes(command.Notes)
        .Build();
    
    // Lưu và return
    await _orderRepository.AddAsync(order);
    return CommandResult<CreateOrderResult>.Success(...);
}
```

**Dependency Injection:**

```csharp
// Program.cs
builder.Services.AddScoped<IOrderBuilder, OrderBuilder>();
builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, CreateOrderResult>, CreateOrderWithBuilderHandler>();
```

**Lợi ích Builder Pattern:**

| Lợi ích | Mô tả |
|---------|-------|
| **Fluent Interface** | Code dễ đọc như văn xuôi |
| **Step-by-step** | Xây dựng từng bước, dễ hiểu |
| **Validation** | Validate tự động khi Build() |
| **Flexible** | Optional steps không cần thiết |
| **Reusable** | Builder có thể Reset() và tái sử dụng |
| **Testable** | Dễ mock từng step |
| **SRP** | Logic xây dựng tách khỏi Handler |

**Cấu trúc thư mục Builder Pattern:**

```
📂 CosmeticStore.Core/Builders/
└── 📄 IOrderBuilder.cs           ← Interface Builder + DTOs

📂 CosmeticStore.Infrastructure/Builders/
└── 📄 OrderBuilder.cs            ← Concrete Builder Implementation

📂 CosmeticStore.Infrastructure/Handlers/
├── 📄 CreateOrderCommandHandler.cs       ← Handler cũ (không dùng Builder)
└── 📄 CreateOrderWithBuilderHandler.cs   ← Handler mới (dùng Builder)
```

---

### 🔄 Giai đoạn 9: Skin Analysis Camera (Adapter Pattern) - Đang phát triển

**Mục tiêu:** Xây dựng tính năng theo dõi tình trạng da mặt theo thời gian bằng camera.

#### Chức năng chính

| Chức năng | Mô tả |
|-----------|-------|
| Chụp ảnh khuôn mặt | Hướng dẫn căn chỉnh, kiểm tra có khuôn mặt |
| Phát hiện khuôn mặt | Cắt vùng khuôn mặt từ ảnh |
| Phân tích da | Độ sáng, đều màu, mụn, đốm nâu, lỗ chân lông |
| Lưu lịch sử | Lưu ảnh và kết quả theo ngày/giờ |
| So sánh xu hướng | Nhận biết cải thiện hay xấu đi |
| Đưa lời khuyên | Gợi ý chăm sóc da phù hợp |

#### Bước 9.1: Core & Abstraction (Adapter Pattern) ✅

| File | Mô tả | Layer |
|------|-------|-------|
| `SkinAnalysis/SkinAnalysisResult.cs` | Value Object chứa kết quả phân tích (Brightness, AcneCount...) | Core |
| `Interfaces/ISkinAnalysisService.cs` | Interface Adapter cho việc phân tích da | Core |
| `Entities/SkinAnalysisHistory.cs` | Entity lưu lịch sử phân tích | Core |
| `Interfaces/ISkinAnalysisHistoryRepository.cs` | Repository Interface cho lịch sử | Core |

**Adapter Pattern - Abstraction:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                       ISkinAnalysisService                               │
│                    (Interface trong Core)                                │
├─────────────────────────────────────────────────────────────────────────┤
│  AnalyzeAsync(Stream image) → SkinAnalysisResult                         │
│  ContainsFaceAsync(Stream image) → bool                                  │
│  DetectAndCropFaceAsync(Stream image) → FaceDetectionResult             │
│  ValidateImageQualityAsync(Stream image) → ImageQualityResult           │
│  GetFaceAlignmentGuidanceAsync(Stream image) → FaceAlignmentGuidance    │
│  CompareSkinAnalysis(current, previous) → SkinTrendAnalysis             │
│  AnalyzeTrends(historicalResults) → SkinTrendReport                     │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    │ Implement
                   ┌────────────────┼────────────────┐
                   ▼                ▼                ▼
        ┌──────────────────┐ ┌──────────────────┐ ┌──────────────────┐
        │ OpenCvSharp      │ │ Python AI Server │ │ Azure Computer   │
        │ SkinAnalysis     │ │ SkinAnalysis     │ │ Vision           │
        │ Service          │ │ Service          │ │ Service          │
        └──────────────────┘ └──────────────────┘ └──────────────────┘
         (Local Processing)  (Remote AI Model)   (Cloud API)
```

**SkinAnalysisResult - Value Object:**

```csharp
public class SkinAnalysisResult
{
    // Phát hiện khuôn mặt
    public bool FaceDetected { get; }
    public decimal FaceConfidence { get; }
    
    // Chỉ số da (0-100)
    public decimal Brightness { get; }      // Độ sáng
    public decimal Evenness { get; }        // Đều màu
    public decimal Smoothness { get; }      // Độ mịn
    public decimal Hydration { get; }       // Độ ẩm
    public decimal Oiliness { get; }        // Dầu nhờn
    
    // Vấn đề da
    public int AcneCount { get; }           // Số mụn
    public int DarkSpotCount { get; }       // Số đốm nâu
    public decimal WrinkleLevel { get; }    // Nếp nhăn
    public decimal PoreSize { get; }        // Lỗ chân lông
    public decimal Redness { get; }         // Đỏ da
    
    // Đánh giá tổng quan
    public decimal OverallScore { get; }    // Điểm sức khỏe da (0-100)
    public SkinCondition Condition { get; } // Excellent/Good/Normal/NeedsAttention/Poor
    public DetectedSkinType DetectedSkinType { get; } // Oily/Dry/Combination/Normal/Sensitive
    
    // Lời khuyên
    public IReadOnlyList<SkinConcern> DetectedConcerns { get; }
    public IReadOnlyList<SkinAdvice> Recommendations { get; }
}
```

**Lợi ích Adapter Pattern:**

| Lợi ích | Mô tả |
|---------|-------|
| **Flexibility** | Dễ dàng thay đổi từ OpenCvSharp sang Python AI hoặc Cloud Vision |
| **Abstraction** | Core không biết implementation cụ thể |
| **Testable** | Có thể mock interface để test |
| **Open/Closed** | Thêm implementation mới không sửa code cũ |

#### Các bước tiếp theo (Pending)

| Bước | Mô tả | Pattern |
|------|-------|---------|
| **9.2** | Implement OpenCvSharpSkinAnalysisService | Adapter |
| **9.3** | Tạo SkinAnalysisController (API endpoints) | - |
| **9.4** | Tạo SkinAnalysisHistoryRepository | Repository |
| **9.5** | Frontend Camera Integration | - |

---

### ⏳ Giai đoạn tiếp theo (Đang phát triển)

| Giai đoạn | Mô tả | Pattern |
|-----------|-------|---------|
| **Bước 9.2** | Implement SkinAnalysisService với OpenCvSharp | Adapter |
| **Bước 5.2** | Virtual Try-on | Module Integration |
| **Bước 5.3** | Expiry Automation | Background Service |
| **Bước 4.3** | Review System | Repository + Observer |

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
| `Entities/SystemLog.cs` | Entity log hệ thống (DB Logging) | **Singleton + Factory Methods** |
| `Enums/SkinType.cs` | Enum loại da (Oily, Dry, Sensitive, Normal, Combination) | - |
| `Enums/VipLevel.cs` | Enum cấp VIP (None, Bronze, Silver, Gold, Platinum) | - |
| `Enums/OrderStatus.cs` | Enum trạng thái đơn hàng (Pending → Completed) | - |
| `Enums/PaymentMethod.cs` | Enum phương thức thanh toán (COD, Momo, VNPay...) | - |
| `Enums/UserRole.cs` | Enum vai trò người dùng (User, Staff, Admin) | - |
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
| `Interfaces/ISystemLogger.cs` | Interface Logger nâng cao (5 levels, Business Logging) | **Singleton** |
| `Interfaces/IAuthService.cs` | Interface Authentication (JWT, Password Hash) | **Trừu tượng** |
| `Events/IDomainEvent.cs` | Interface Domain Event + Base class | **Observer** |
| `Events/IDomainEventHandler.cs` | Interface Handler + INotificationService | **Observer** |
| `Events/OrderEvents.cs` | Events: Created, Confirmed, Cancelled, Payment... | **Observer** |
| `Events/ProductEvents.cs` | Events: Expiring, LowStock, FlashSale... | **Observer** |
| `Events/ReviewEvents.cs` | Events: Created, Reported, Approved... | **Observer** |
| `SkinQuiz/SkinQuizModels.cs` | DTOs + SkinTypeInfo chi tiết (5 loại da) | **AI Quiz** |
| `SkinQuiz/SkinQuizQuestions.cs` | 10 câu hỏi với điểm số | **AI Quiz** |
| `Interfaces/ISkinQuizService.cs` | Interface AI Skin Quiz | **Strategy Context** |
| `Builders/IOrderBuilder.cs` | Interface Builder + DTOs (CartItem, DiscountDetail, OrderBuildResult) | **Builder** |
| `SkinAnalysis/SkinAnalysisResult.cs` | Value Object kết quả phân tích da (Brightness, AcneCount...) | **Adapter** |
| `Entities/SkinAnalysisHistory.cs` | Entity lưu lịch sử phân tích da | **Encapsulation** |
| `Interfaces/ISkinAnalysisService.cs` | Interface Adapter cho phân tích da | **Adapter** |
| `Interfaces/ISkinAnalysisHistoryRepository.cs` | Repository Interface cho lịch sử phân tích | **Repository** |

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
| `Services/SystemLogger.cs` | Logger nâng cao (File + DB, Batch Write) | **Singleton** |
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
| `Events/DomainEventDispatcher.cs` | Trung tâm phân phối Domain Events | **Observer** |
| `Services/NotificationService.cs` | Gửi Email/SMS/Push/Admin Alert | **Observer** |
| `Handlers/Notifications/EmailNotificationHandler.cs` | Handler gửi Email thông báo | **Observer** |
| `Handlers/Notifications/SmsNotificationHandler.cs` | Handler gửi SMS thông báo | **Observer** |
| `Handlers/Notifications/AdminAlertHandler.cs` | Handler thông báo Admin | **Observer** |
| `Services/SkinQuizService.cs` | AI phân tích loại da (Strategy Context) | **AI Quiz** |
| `Services/AuthService.cs` | JWT Token + Password Hash (PBKDF2) | **Authentication** |
| `Builders/OrderBuilder.cs` | Concrete Builder cho Order (Fluent Interface) | **Builder** |
| `Handlers/CreateOrderWithBuilderHandler.cs` | Handler dùng Builder Pattern | **Builder + Command** |

### 📂 CosmeticStore.API (Tầng Presentation)

| File | Mô tả | OOP/Pattern |
|------|-------|-------------|
| `Program.cs` | Entry point, cấu hình DI, JWT Auth | **DI Container** |
| `Controllers/AuthController.cs` | Register, Login, RefreshToken, Logout | **Authentication** |
| `Controllers/UserController.cs` | Profile, Admin quản lý users | **Authorization** |
| `Controllers/OrderController.cs` | CRUD Order với Command Pattern | **Command Pattern** |
| `Controllers/ProductsController.cs` | 30+ API endpoints | **Constructor Injection** |
| `Controllers/SkinQuizController.cs` | 6 API endpoints cho AI Skin Quiz | **AI Quiz** |
| `ViewModels/AuthViewModels.cs` | Register, Login, Token DTOs | - |
| `ViewModels/OrderViewModels.cs` | Order Request/Response DTOs | - |
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

### 🔐 Authentication (AuthController)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `POST` | `/api/auth/register` | Đăng ký tài khoản mới | ❌ |
| `POST` | `/api/auth/login` | Đăng nhập, nhận JWT Token | ❌ |
| `POST` | `/api/auth/refresh-token` | Làm mới Access Token | ❌ |
| `POST` | `/api/auth/logout` | Đăng xuất (revoke token) | ✅ |
| `POST` | `/api/auth/change-password` | Đổi mật khẩu | ✅ |

**Ví dụ đăng ký:**

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123",
  "confirmPassword": "Password123",
  "fullName": "Nguyễn Văn A",
  "phoneNumber": "0901234567"
}
```

**Response:**

```json
{
  "isSuccess": true,
  "message": "Đăng ký thành công",
  "token": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
    "accessTokenExpiry": "2026-01-23T15:00:00Z",
    "refreshTokenExpiry": "2026-01-30T14:00:00Z"
  },
  "user": {
    "id": 1,
    "email": "user@example.com",
    "fullName": "Nguyễn Văn A",
    "role": "User",
    "vipLevel": "None"
  }
}
```

---

### 👤 User Management (UserController)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/api/user/me` | Lấy profile của tôi | ✅ User |
| `PUT` | `/api/user/me` | Cập nhật profile | ✅ User |
| `GET` | `/api/user/me/loyalty` | Xem VIP & điểm thưởng | ✅ User |
| `GET` | `/api/user` | Danh sách users | ✅ Admin/Staff |
| `GET` | `/api/user/{id}` | Chi tiết user | ✅ Admin/Staff |
| `POST` | `/api/user` | Tạo user (chỉ định role) | ✅ Admin |
| `PATCH` | `/api/user/{id}/role` | Đổi role | ✅ Admin |
| `PATCH` | `/api/user/{id}/status` | Khóa/mở tài khoản | ✅ Admin |
| `DELETE` | `/api/user/{id}` | Xóa user | ✅ Admin |
| `GET` | `/api/user/stats` | Thống kê users | ✅ Admin |

---

### 🛒 Order Management (OrderController)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `POST` | `/api/order` | Tạo đơn hàng | ✅ User |
| `GET` | `/api/order/my-orders` | Đơn hàng của tôi | ✅ User |
| `GET` | `/api/order/{id}` | Chi tiết đơn hàng | ✅ User/Staff |
| `POST` | `/api/order/{id}/cancel` | Hủy đơn | ✅ User |
| `POST` | `/api/order/{id}/pay` | Thanh toán | ✅ User |
| `GET` | `/api/order` | Tất cả đơn hàng | ✅ Admin/Staff |
| `POST` | `/api/order/{id}/confirm` | Xác nhận đơn | ✅ Admin/Staff |
| `PATCH` | `/api/order/{id}/status` | Cập nhật trạng thái | ✅ Admin/Staff |
| `GET` | `/api/order/stats` | Thống kê đơn hàng | ✅ Admin |
| `GET` | `/api/order/pending` | Đơn chờ xử lý | ✅ Admin/Staff |
| `GET` | `/api/order/by-number/{orderNumber}` | Tìm theo mã đơn | ✅ Admin/Staff |

**Ví dụ tạo đơn hàng:**

```http
POST /api/order
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 3, "quantity": 1 }
  ],
  "shippingAddress": "123 Nguyễn Văn Linh, Q.7, TP.HCM",
  "shippingPhone": "0901234567",
  "receiverName": "Nguyễn Văn A",
  "paymentMethod": 1,
  "notes": "Giao giờ hành chính",
  "couponCode": "SALE10"
}
```

---

### 📦 Product Management (CRUD Cơ bản)

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

