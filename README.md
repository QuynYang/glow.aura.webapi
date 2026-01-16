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
 ┣ 📂 CosmeticStore.API/           # Tầng API (Presentation Layer)
 ┃ ┣ 📂 Controllers/               # Các API Controllers
 ┃ ┃ ┗ 📄 ProductsController.cs    # Controller quản lý sản phẩm
 ┃ ┣ 📂 ViewModels/                # Data Transfer Objects (DTOs)
 ┃ ┃ ┗ 📄 ProductViewModels.cs     # Request/Response models
 ┃ ┣ 📄 Program.cs                 # Entry point, cấu hình DI
 ┃ ┣ 📄 appsettings.json           # Cấu hình ứng dụng
 ┃ ┗ 📄 CosmeticStore.API.csproj   # Project file
 ┃
 ┣ 📂 CosmeticStore.Core/          # Tầng Core (Domain Layer)
 ┃ ┣ 📂 Entities/                  # Domain Entities
 ┃ ┃ ┣ 📄 BaseEntity.cs            # Class cha cho tất cả Entity
 ┃ ┃ ┗ 📄 Product.cs               # Entity sản phẩm
 ┃ ┣ 📂 Interfaces/                # Abstractions
 ┃ ┃ ┣ 📄 IRepository.cs           # Interface Repository Pattern
 ┃ ┃ ┣ 📄 IPricingStrategy.cs      # Interface Strategy Pattern
 ┃ ┃ ┗ 📄 IPaymentService.cs       # Interface Payment Services
 ┃ ┣ 📂 Enums/                     # Enumerations
 ┃ ┗ 📄 CosmeticStore.Core.csproj  # Project file
 ┃
 ┣ 📂 CosmeticStore.Infrastructure/  # Tầng Infrastructure
 ┃ ┣ 📂 DbContext/                   # Database Context
 ┃ ┃ ┗ 📄 StoreDbContext.cs          # EF Core DbContext
 ┃ ┣ 📂 Repositories/                # Repository implementations
 ┃ ┃ ┗ 📄 GenericRepository.cs       # Generic Repository
 ┃ ┣ 📂 Strategies/                  # Strategy implementations
 ┃ ┃ ┣ 📄 VipPricingStrategy.cs      # Chiến lược giá VIP
 ┃ ┃ ┣ 📄 StandardPricingStrategy.cs # Chiến lược giá thường
 ┃ ┃ ┗ 📄 SalePricingStrategy.cs     # Chiến lược khuyến mãi
 ┃ ┣ 📂 Services/                    # Service implementations
 ┃ ┃ ┣ 📄 PaymentFactory.cs          # Factory tạo Payment Service
 ┃ ┃ ┣ 📄 MomoPaymentService.cs      # Thanh toán Momo
 ┃ ┃ ┗ 📄 CodPaymentService.cs       # Thanh toán COD
 ┃ ┗ 📄 CosmeticStore.Infrastructure.csproj
 ┃
 ┣ 📄 CosmeticStore.sln              # Solution file
 ┣ 📄 .gitignore                     # Git ignore rules
 ┗ 📄 README.md                      # Tài liệu này
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

**Áp dụng**: Tất cả Entity (Product, Order, User...) đều kế thừa từ `BaseEntity`, không cần khai báo lại các thuộc tính chung.

```csharp
public class Product : BaseEntity  // ← Kế thừa
{
    public string Name { get; private set; }
    // ... Product tự động có Id, CreatedAt, UpdatedAt, IsDeleted
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

    // Logic nghiệp vụ được đóng gói trong method
    public void UpdateStock(int quantity)
    {
        if (Stock + quantity < 0)
            throw new InvalidOperationException("Không đủ hàng tồn kho");
        
        Stock += quantity;  // ← Chỉ thay đổi qua method
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**So sánh**:

| ❌ TRƯỚC (Anemic Model) | ✅ SAU (Rich Domain Model) |
|------------------------|---------------------------|
| `product.Stock = product.Stock - 5;` | `product.DecreaseStock(5);` |
| Logic rải rác ở Controller | Logic tập trung trong Entity |
| Dễ bị sửa sai dữ liệu | Có validation trong method |

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

**File**: `CosmeticStore.Core/Interfaces/IRepository.cs`

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void SoftDelete(T entity);
    Task<int> SaveChangesAsync();
}
```

**Lợi ích**:

- **Controller** chỉ biết đến `IRepository<Product>`, không biết dùng EF Core hay Dapper
- Dễ dàng mock trong Unit Testing
- Có thể thay đổi implementation mà không sửa Controller

```csharp
// Controller chỉ inject Interface, không biết GenericRepository
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> _repo;  // ← Interface
    
    public ProductsController(IRepository<Product> repo)
    {
        _repo = repo;
    }
}
```

---

## 🧩 Design Patterns được áp dụng

### 1. Repository Pattern

> **Mục đích**: Tách biệt logic truy cập dữ liệu khỏi business logic

| File | Vai trò |
|------|---------|
| `IRepository.cs` | Interface định nghĩa các thao tác CRUD |
| `GenericRepository.cs` | Implementation sử dụng EF Core |

```csharp
// Interface (Core)
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    // ...
}

// Implementation (Infrastructure)
public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly StoreDbContext _context;
    
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
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

## 📋 Giải thích chi tiết các file

### 📂 CosmeticStore.Core (Tầng Domain)

| File | Mô tả | OOP/Pattern |
|------|-------|-------------|
| `Entities/BaseEntity.cs` | Class cha chứa các thuộc tính chung (Id, CreatedAt, IsDeleted) | **Kế thừa** |
| `Entities/Product.cs` | Entity sản phẩm với logic nghiệp vụ (UpdateStock, UpdatePrice) | **Đóng gói** |
| `Interfaces/IRepository.cs` | Interface cho Repository Pattern, định nghĩa các thao tác CRUD | **Trừu tượng** |
| `Interfaces/IPricingStrategy.cs` | Interface cho Strategy Pattern tính giá | **Đa hình** |
| `Interfaces/IPaymentService.cs` | Interface cho các dịch vụ thanh toán | **Trừu tượng** |

### 📂 CosmeticStore.Infrastructure (Tầng Hạ tầng)

| File | Mô tả | OOP/Pattern |
|------|-------|-------------|
| `DbContext/StoreDbContext.cs` | EF Core DbContext, mapping Entity sang SQL | - |
| `Repositories/GenericRepository.cs` | Implementation của IRepository | **Repository Pattern** |
| `Strategies/StandardPricingStrategy.cs` | Chiến lược giá thường | **Strategy Pattern** |
| `Strategies/VipPricingStrategy.cs` | Chiến lược giá VIP (giảm 10%) | **Strategy Pattern** |
| `Strategies/SalePricingStrategy.cs` | Chiến lược khuyến mãi | **Strategy Pattern** |
| `Services/PaymentFactory.cs` | Factory tạo Payment Service | **Factory Pattern** |
| `Services/MomoPaymentService.cs` | Xử lý thanh toán Momo | **Đa hình** |
| `Services/CodPaymentService.cs` | Xử lý thanh toán COD | **Đa hình** |

### 📂 CosmeticStore.API (Tầng Presentation)

| File | Mô tả | OOP/Pattern |
|------|-------|-------------|
| `Program.cs` | Entry point, cấu hình Dependency Injection | **DI Container** |
| `Controllers/ProductsController.cs` | API endpoints cho sản phẩm | **Constructor Injection** |
| `ViewModels/ProductViewModels.cs` | DTOs cho request/response | - |
| `appsettings.json` | Cấu hình ứng dụng, connection string | - |

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

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/api/products` | Lấy tất cả sản phẩm |
| `GET` | `/api/products/{id}` | Lấy sản phẩm theo ID |
| `POST` | `/api/products` | Tạo sản phẩm mới |
| `PUT` | `/api/products/{id}` | Cập nhật sản phẩm |
| `PATCH` | `/api/products/{id}/stock` | Cập nhật số lượng tồn kho |
| `DELETE` | `/api/products/{id}` | Xóa sản phẩm (soft delete) |

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
  "imageUrl": "https://example.com/mac-ruby-woo.jpg"
}
```

---

## 👨‍💻 Tác giả

- **Họ tên**: Vũ Ngọc Quỳnh Giang
- **MSSV**: 22DH114506
- **Môn học**: Mẫu Thiết Kế Phần Mềm

---

## 📄 License

Dự án này được phát hành dưới giấy phép [MIT License](LICENSE).

