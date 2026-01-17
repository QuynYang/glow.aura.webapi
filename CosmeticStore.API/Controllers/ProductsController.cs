using Microsoft.AspNetCore.Mvc;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.API.ViewModels;

namespace CosmeticStore.API.Controllers;

/// <summary>
/// Controller quản lý sản phẩm mỹ phẩm
/// 
/// Áp dụng:
/// - Dependency Injection (DI): Inject IProductRepository thay vì tạo trực tiếp
/// - TRỪU TƯỢNG: Controller chỉ biết đến Interface, không biết chi tiết implementation
/// - Repository Pattern: Sử dụng IProductRepository cho các query đặc thù
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IPricingStrategy _pricingStrategy;

    /// <summary>
    /// Constructor Injection - Nhận dependencies từ DI Container
    /// </summary>
    public ProductsController(
        IProductRepository productRepository,
        IPricingStrategy pricingStrategy)
    {
        _productRepository = productRepository;
        _pricingStrategy = pricingStrategy;
    }

    #region Basic CRUD Operations

    /// <summary>
    /// Lấy tất cả sản phẩm
    /// GET: api/products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Lấy sản phẩm theo Id
    /// GET: api/products/5
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound($"Không tìm thấy sản phẩm với Id: {id}");

        return Ok(MapToResponse(product));
    }

    /// <summary>
    /// Tạo sản phẩm mới
    /// POST: api/products
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest request)
    {
        // OOP: Tạo object bằng Constructor, đảm bảo tính toàn vẹn dữ liệu
        var product = new Product(request.Name, request.Price, request.Stock, request.SkinType, request.ExpiryDate);
        
        // Cập nhật thông tin bổ sung
        product.UpdateInfo(
            description: request.Description,
            brand: request.Brand,
            category: request.Category,
            imageUrl: request.ImageUrl
        );

        // Cập nhật thông tin mỹ phẩm đặc thù
        product.UpdateCosmeticInfo(
            ingredients: request.Ingredients,
            usageInstructions: request.UsageInstructions,
            volume: request.Volume
        );

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, MapToResponse(product));
    }

    /// <summary>
    /// Cập nhật thông tin sản phẩm
    /// PUT: api/products/5
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound($"Không tìm thấy sản phẩm với Id: {id}");

        // OOP: Gọi method để cập nhật, không set trực tiếp
        product.UpdateInfo(
            description: request.Description,
            brand: request.Brand,
            category: request.Category,
            imageUrl: request.ImageUrl
        );

        product.UpdateCosmeticInfo(
            skinType: request.SkinType,
            expiryDate: request.ExpiryDate,
            ingredients: request.Ingredients,
            usageInstructions: request.UsageInstructions,
            volume: request.Volume
        );

        if (request.NewPrice.HasValue)
            product.UpdatePrice(request.NewPrice.Value);

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Cập nhật số lượng tồn kho
    /// PATCH: api/products/5/stock
    /// </summary>
    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound($"Không tìm thấy sản phẩm với Id: {id}");

        try
        {
            // OOP: Logic được đóng gói trong Entity
            product.UpdateStock(quantity);
            
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return Ok(new { Message = $"Cập nhật tồn kho thành công. Số lượng hiện tại: {product.Stock}" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Xóa sản phẩm (Soft Delete)
    /// DELETE: api/products/5
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound($"Không tìm thấy sản phẩm với Id: {id}");

        // Soft Delete: Chỉ đánh dấu IsDeleted = true
        _productRepository.SoftDelete(product);
        await _productRepository.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region Skin Type Filtering (AI Skin Quiz Support)

    /// <summary>
    /// Lấy sản phẩm theo loại da - Hỗ trợ AI Skin Quiz
    /// GET: api/products/skin-type/oily
    /// </summary>
    [HttpGet("skin-type/{skinType}")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetBySkinType(SkinType skinType)
    {
        var products = await _productRepository.GetBySkinTypeAsync(skinType);
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Lấy sản phẩm theo loại da có phân trang
    /// GET: api/products/skin-type/oily/paged?pageNumber=1&pageSize=10
    /// </summary>
    [HttpGet("skin-type/{skinType}/paged")]
    public async Task<ActionResult<PaginatedResponse<ProductResponse>>> GetBySkinTypePaged(
        SkinType skinType, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var products = await _productRepository.GetBySkinTypeAsync(skinType, pageNumber, pageSize);
        var totalCount = await _productRepository.CountAsync(p => 
            skinType == SkinType.All || p.SkinType == skinType || p.SkinType == SkinType.All);

        return Ok(new PaginatedResponse<ProductResponse>
        {
            Items = products.Select(MapToResponse),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    #endregion

    #region Expiry Management

    /// <summary>
    /// Lấy sản phẩm sắp hết hạn - Quan trọng cho Expiry Management
    /// GET: api/products/expiring-soon?days=30
    /// </summary>
    [HttpGet("expiring-soon")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetExpiringSoon([FromQuery] int days = 30)
    {
        var products = await _productRepository.GetExpiringSoonAsync(days);
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Lấy sản phẩm đã hết hạn (Admin only)
    /// GET: api/products/expired
    /// </summary>
    [HttpGet("expired")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetExpired()
    {
        var products = await _productRepository.GetExpiredProductsAsync();
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    #endregion

    #region Flash Sale

    /// <summary>
    /// Lấy sản phẩm đang Flash Sale
    /// GET: api/products/flash-sale
    /// </summary>
    [HttpGet("flash-sale")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetFlashSale()
    {
        var products = await _productRepository.GetFlashSaleProductsAsync();
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Kích hoạt Flash Sale cho sản phẩm
    /// POST: api/products/5/flash-sale
    /// </summary>
    [HttpPost("{id}/flash-sale")]
    public async Task<IActionResult> ActivateFlashSale(int id, [FromBody] ActivateFlashSaleRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound($"Không tìm thấy sản phẩm với Id: {id}");

        try
        {
            product.ActivateFlashSale(request.DiscountPercent, request.EndTime);
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return Ok(new { Message = $"Đã kích hoạt Flash Sale cho sản phẩm '{product.Name}'" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Hủy Flash Sale cho sản phẩm
    /// DELETE: api/products/5/flash-sale
    /// </summary>
    [HttpDelete("{id}/flash-sale")]
    public async Task<IActionResult> DeactivateFlashSale(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return NotFound($"Không tìm thấy sản phẩm với Id: {id}");

        product.DeactivateFlashSale();
        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();

        return Ok(new { Message = $"Đã hủy Flash Sale cho sản phẩm '{product.Name}'" });
    }

    #endregion

    #region Brand & Category Filtering

    /// <summary>
    /// Lấy sản phẩm theo thương hiệu
    /// GET: api/products/brand/mac
    /// </summary>
    [HttpGet("brand/{brand}")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetByBrand(string brand)
    {
        var products = await _productRepository.GetByBrandAsync(brand);
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Lấy sản phẩm theo danh mục
    /// GET: api/products/category/son-moi
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetByCategory(string category)
    {
        var products = await _productRepository.GetByCategoryAsync(category);
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Lấy danh sách tất cả thương hiệu
    /// GET: api/products/brands
    /// </summary>
    [HttpGet("brands")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllBrands()
    {
        var brands = await _productRepository.GetAllBrandsAsync();
        return Ok(brands);
    }

    /// <summary>
    /// Lấy danh sách tất cả danh mục
    /// GET: api/products/categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
    {
        var categories = await _productRepository.GetAllCategoriesAsync();
        return Ok(categories);
    }

    #endregion

    #region Price & Stock Filtering

    /// <summary>
    /// Lấy sản phẩm trong khoảng giá
    /// GET: api/products/price-range?minPrice=100000&maxPrice=500000
    /// </summary>
    [HttpGet("price-range")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetByPriceRange(
        [FromQuery] decimal minPrice, 
        [FromQuery] decimal maxPrice)
    {
        var products = await _productRepository.GetByPriceRangeAsync(minPrice, maxPrice);
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Lấy sản phẩm sắp hết hàng (Admin)
    /// GET: api/products/low-stock?threshold=10
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetLowStock([FromQuery] int threshold = 10)
    {
        var products = await _productRepository.GetLowStockProductsAsync(threshold);
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Lấy sản phẩm đã hết hàng (Admin)
    /// GET: api/products/out-of-stock
    /// </summary>
    [HttpGet("out-of-stock")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetOutOfStock()
    {
        var products = await _productRepository.GetOutOfStockProductsAsync();
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    #endregion

    #region Search & Advanced Queries

    /// <summary>
    /// Tìm kiếm sản phẩm theo từ khóa
    /// GET: api/products/search?keyword=son
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> Search([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest("Vui lòng nhập từ khóa tìm kiếm");

        var products = await _productRepository.SearchAsync(keyword);
        var response = products.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Tìm kiếm nâng cao với nhiều điều kiện
    /// POST: api/products/advanced-search
    /// </summary>
    [HttpPost("advanced-search")]
    public async Task<ActionResult<PaginatedResponse<ProductResponse>>> AdvancedSearch(
        [FromBody] ProductSearchRequest request)
    {
        var products = await _productRepository.AdvancedSearchAsync(
            keyword: request.Keyword,
            skinType: request.SkinType,
            brand: request.Brand,
            category: request.Category,
            minPrice: request.MinPrice,
            maxPrice: request.MaxPrice,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize
        );

        var totalCount = await _productRepository.CountAdvancedSearchAsync(
            keyword: request.Keyword,
            skinType: request.SkinType,
            brand: request.Brand,
            category: request.Category,
            minPrice: request.MinPrice,
            maxPrice: request.MaxPrice
        );

        return Ok(new PaginatedResponse<ProductResponse>
        {
            Items = products.Select(MapToResponse),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }

    #endregion

    #region Dashboard Stats (Admin)

    /// <summary>
    /// Lấy thống kê Dashboard cho Admin
    /// GET: api/products/stats
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ProductDashboardStats>> GetDashboardStats()
    {
        var totalProducts = await _productRepository.CountAsync();
        var expiringSoonCount = await _productRepository.CountExpiringSoonAsync(30);
        var flashSaleCount = await _productRepository.CountFlashSaleProductsAsync();
        var lowStockProducts = await _productRepository.GetLowStockProductsAsync(10);
        var outOfStockProducts = await _productRepository.GetOutOfStockProductsAsync();

        return Ok(new ProductDashboardStats
        {
            TotalProducts = totalProducts,
            ExpiringSoonCount = expiringSoonCount,
            FlashSaleCount = flashSaleCount,
            LowStockCount = lowStockProducts.Count(),
            OutOfStockCount = outOfStockProducts.Count()
        });
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Map Product Entity sang ProductResponse ViewModel
    /// </summary>
    private ProductResponse MapToResponse(Product product)
    {
        var discountedPrice = _pricingStrategy.CalculatePrice(product.Price);
        
        // Nếu sản phẩm đang Flash Sale, áp dụng thêm giảm giá Flash Sale
        if (product.IsInActiveFlashSale())
        {
            discountedPrice = discountedPrice * (1 - product.FlashSaleDiscount / 100);
        }

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DiscountedPrice = discountedPrice,
            Stock = product.Stock,
            Brand = product.Brand,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            CreatedAt = product.CreatedAt,
            
            // Cosmetic-specific
            SkinType = product.SkinType.ToString(),
            ExpiryDate = product.ExpiryDate,
            DaysUntilExpiry = product.GetDaysUntilExpiry(),
            IsExpiringSoon = product.IsExpiringSoon(30),
            Ingredients = product.Ingredients,
            UsageInstructions = product.UsageInstructions,
            Volume = product.Volume,
            
            // Flash Sale
            IsFlashSale = product.IsInActiveFlashSale(),
            FlashSaleDiscount = product.IsInActiveFlashSale() ? product.FlashSaleDiscount : null,
            FlashSaleEndTime = product.IsInActiveFlashSale() ? product.FlashSaleEndTime : null
        };
    }

    #endregion
}
