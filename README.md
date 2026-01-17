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
 ┃ ┃ ┗ 📄 Product.cs                  # Entity sản phẩm - Encapsulation
 ┃ ┣ 📂 Enums/
 ┃ ┃ ┗ 📄 SkinType.cs                 # Enum loại da (Oily, Dry, Sensitive...)
 ┃ ┣ 📂 Interfaces/
 ┃ ┃ ┣ 📄 IGenericRepository.cs       # Interface CRUD cơ bản
 ┃ ┃ ┣ 📄 IProductRepository.cs       # Interface đặc thù cho Product
 ┃ ┃ ┣ 📄 IPricingStrategy.cs         # Interface Strategy Pattern
 ┃ ┃ ┗ 📄 IPaymentService.cs          # Interface Payment Services
 ┃ ┗ 📄 CosmeticStore.Core.csproj
 ┃
 ┣ 📂 CosmeticStore.Infrastructure/   # Tầng Infrastructure
 ┃ ┣ 📂 DbContext/
 ┃ ┃ ┗ 📄 StoreDbContext.cs           # EF Core DbContext
 ┃ ┣ 📂 Repositories/
 ┃ ┃ ┣ 📄 GenericRepository.cs        # Generic Repository - CRUD cơ bản
 ┃ ┃ ┗ 📄 ProductRepository.cs        # Product Repository - Query đặc thù
 ┃ ┣ 📂 Strategies/
 ┃ ┃ ┣ 📄 VipPricingStrategy.cs       # Chiến lược giá VIP
 ┃ ┃ ┣ 📄 StandardPricingStrategy.cs  # Chiến lược giá thường
 ┃ ┃ ┗ 📄 SalePricingStrategy.cs      # Chiến lược khuyến mãi
 ┃ ┣ 📂 Services/
 ┃ ┃ ┣ 📄 PaymentFactory.cs           # Factory tạo Payment Service
 ┃ ┃ ┣ 📄 MomoPaymentService.cs       # Thanh toán Momo
 ┃ ┃ ┗ 📄 CodPaymentService.cs        # Thanh toán COD
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

### ⏳ Giai đoạn tiếp theo (Đang phát triển)

| Giai đoạn | Mô tả | Pattern |
|-----------|-------|---------|
| **Giai đoạn 2** | Giỏ hàng & Tính giá | Strategy + Decorator |
| **Giai đoạn 3** | Đặt hàng | Command Pattern |
| **Giai đoạn 4** | Thanh toán | Factory Pattern |
| **Giai đoạn 5** | Thông báo | Observer Pattern |
| **Giai đoạn 6** | Ghi log | Singleton Pattern |

---

## 📋 Giải thích chi tiết các file

### 📂 CosmeticStore.Core (Tầng Domain)

| File | Mô tả | OOP/Pattern |
|------|-------|-------------|
| `Entities/BaseEntity.cs` | Class cha chứa Id, CreatedAt, IsDeleted | **Kế thừa** |
| `Entities/Product.cs` | Entity với logic UpdateStock, ActivateFlashSale | **Đóng gói** |
| `Enums/SkinType.cs` | Enum loại da (Oily, Dry, Sensitive, Normal, Combination) | - |
| `Interfaces/IGenericRepository.cs` | Interface CRUD cơ bản | **Trừu tượng** |
| `Interfaces/IProductRepository.cs` | Interface đặc thù cho Product | **Kế thừa** |
| `Interfaces/IPricingStrategy.cs` | Interface Strategy Pattern | **Đa hình** |
| `Interfaces/IPaymentService.cs` | Interface Payment Services | **Trừu tượng** |

### 📂 CosmeticStore.Infrastructure (Tầng Hạ tầng)

| File | Mô tả | OOP/Pattern |
|------|-------|-------------|
| `DbContext/StoreDbContext.cs` | EF Core DbContext với Query Filter | - |
| `Repositories/GenericRepository.cs` | Implement IGenericRepository | **Repository** |
| `Repositories/ProductRepository.cs` | Implement IProductRepository | **Repository + Kế thừa** |
| `Strategies/StandardPricingStrategy.cs` | Chiến lược giá thường | **Strategy** |
| `Strategies/VipPricingStrategy.cs` | Chiến lược giá VIP (giảm 10%) | **Strategy** |
| `Strategies/SalePricingStrategy.cs` | Chiến lược khuyến mãi | **Strategy** |
| `Services/PaymentFactory.cs` | Factory tạo Payment Service | **Factory** |
| `Services/MomoPaymentService.cs` | Xử lý thanh toán Momo | **Đa hình** |
| `Services/CodPaymentService.cs` | Xử lý thanh toán COD | **Đa hình** |

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
git clone https://github.com/<your-username>/CosmeticStore.git
cd CosmeticStore
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

