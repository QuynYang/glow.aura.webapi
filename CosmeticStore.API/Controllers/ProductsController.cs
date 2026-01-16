using Microsoft.AspNetCore.Mvc;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.API.ViewModels;

namespace CosmeticStore.API.Controllers;

/// <summary>
/// Controller quản lý sản phẩm mỹ phẩm
/// 
/// Áp dụng:
/// - Dependency Injection (DI): Inject IRepository thay vì tạo trực tiếp
/// - TRỪU TƯỢNG: Controller chỉ biết đến Interface, không biết chi tiết implementation
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> _productRepository;
    private readonly IPricingStrategy _pricingStrategy;

    /// <summary>
    /// Constructor Injection - Nhận dependencies từ DI Container
    /// </summary>
    public ProductsController(
        IRepository<Product> productRepository,
        IPricingStrategy pricingStrategy)
    {
        _productRepository = productRepository;
        _pricingStrategy = pricingStrategy;
    }

    /// <summary>
    /// Lấy tất cả sản phẩm
    /// GET: api/products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        
        var response = products.Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            DiscountedPrice = _pricingStrategy.CalculatePrice(p.Price),
            Stock = p.Stock,
            Brand = p.Brand,
            Category = p.Category,
            ImageUrl = p.ImageUrl,
            CreatedAt = p.CreatedAt
        });

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

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DiscountedPrice = _pricingStrategy.CalculatePrice(product.Price),
            Stock = product.Stock,
            Brand = product.Brand,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            CreatedAt = product.CreatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Tạo sản phẩm mới
    /// POST: api/products
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest request)
    {
        // OOP: Tạo object bằng Constructor, đảm bảo tính toàn vẹn dữ liệu
        var product = new Product(request.Name, request.Price, request.Stock);
        
        // Cập nhật thông tin bổ sung
        product.UpdateInfo(
            description: request.Description,
            brand: request.Brand,
            category: request.Category,
            imageUrl: request.ImageUrl
        );

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Brand = product.Brand,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            CreatedAt = product.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
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
}

