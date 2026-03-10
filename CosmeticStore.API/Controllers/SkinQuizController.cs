using Microsoft.AspNetCore.Mvc;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.SkinQuiz;

namespace CosmeticStore.API.Controllers;

/// <summary>
/// AI Skin Quiz Controller
/// 
/// GIAI ĐOẠN 5: AI & Nâng cao
/// 
/// Chức năng 9️⃣ - AI Skin Quiz:
/// - Trả lời 10 câu hỏi trắc nghiệm
/// - Hệ thống phân tích và xác định loại da bằng Google Gemini AI
/// - Lưu kết quả vào User profile
/// - User tự động hưởng SkinTypePricingStrategy (5% discount)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SkinQuizController : ControllerBase
{
    private readonly ISkinQuizService _skinQuizService;
    private readonly IProductRepository _productRepository;
    private readonly ISystemLogger _logger;

    public SkinQuizController(
        ISkinQuizService skinQuizService,
        IProductRepository productRepository,
        ISystemLogger logger)
    {
        _skinQuizService = skinQuizService;
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách câu hỏi trắc nghiệm
    /// </summary>
    [HttpGet("questions")]
    public ActionResult<List<SkinQuizQuestionResponse>> GetQuestions()
    {
        var questions = _skinQuizService.GetQuestions();
        
        var response = questions.Select(q => new SkinQuizQuestionResponse
        {
            Id = q.Id,
            Question = q.Question,
            Category = q.Category,
            Options = q.Options.Select(o => new OptionResponse
            {
                OptionId = o.OptionId,
                Text = o.Text
            }).ToList()
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Gửi câu trả lời và nhận kết quả phân tích loại da từ AI
    /// </summary>
    [HttpPost("analyze")]
    public async Task<ActionResult<SkinQuizResultResponse>> AnalyzeAndSave([FromBody] SkinQuizAnswerRequest request)
    {
        if (request.Answers == null || request.Answers.Count == 0)
        {
            return BadRequest(new { message = "Vui lòng trả lời ít nhất 1 câu hỏi" });
        }

        if (request.Answers.Count < 5)
        {
            return BadRequest(new { message = "Vui lòng trả lời ít nhất 5 câu hỏi để AI có kết quả chính xác" });
        }

        _logger.LogInfo("Skin quiz submitted", new 
        { 
            UserId = request.UserId, 
            AnswerCount = request.Answers.Count 
        });

        // Gọi sang GeminiSkinQuizService để phân tích
        var result = await _skinQuizService.AnalyzeSkinTypeAsync(request.Answers);

        // Lưu kết quả nếu user đã đăng nhập
        if (request.UserId.HasValue)
        {
            var saved = await _skinQuizService.SaveUserSkinTypeAsync(request.UserId.Value, result.DeterminedSkinType);
            if (saved)
            {
                _logger.LogInfo($"Skin type saved for user {request.UserId.Value}: {result.DeterminedSkinType}");
            }
        }

        // Map sang DTO trả về Frontend
        var response = new SkinQuizResultResponse
        {
            SkinType = result.DeterminedSkinType.ToString(),
            SkinTypeName = result.SkinTypeName,
            Description = result.Description, // Đoạn văn AI Summary
            AiSummary = result.Description,   // Thêm field rõ ràng cho Frontend dễ dùng
            Characteristics = result.Characteristics,
            RecommendedIngredients = result.RecommendedIngredients,
            IngredientsToAvoid = result.IngredientsToAvoid,
            SkincareTips = result.SkincareTips,
            ConfidencePercent = result.ConfidencePercent,
            MatchingProductCount = result.MatchingProductCount,
            HasSkinTypeDiscount = result.HasSkinTypeDiscount,
            SkinTypeDiscountPercent = result.SkinTypeDiscountPercent,
            DetailedScores = result.DetailedScores.ToDictionary(s => s.Key.ToString(), s => s.Value),
            
            // Giả lập điểm số để render UI đẹp (Bạn có thể map dữ liệu thật từ AI vào đây sau này)
            HydrationScore = 85,
            PigmentationScore = 30,
            OilinessScore = 60,
            SensitivityScore = 45,
            ElasticityScore = 70
        };

        return Ok(response);
    }

    /// <summary>
    /// Lấy thông tin chi tiết về một loại da
    /// </summary>
    [HttpGet("skin-types/{skinType}")]
    public ActionResult<SkinTypeDetailsResponse> GetSkinTypeDetails(string skinType)
    {
        if (!Enum.TryParse<SkinType>(skinType, true, out var parsedType))
        {
            return BadRequest(new { message = $"Loại da không hợp lệ: {skinType}" });
        }

        var details = _skinQuizService.GetSkinTypeDetails(parsedType);

        return Ok(new SkinTypeDetailsResponse
        {
            SkinType = parsedType.ToString(),
            Name = details.Name,
            Description = details.Description,
            Characteristics = details.Characteristics,
            RecommendedIngredients = details.RecommendedIngredients,
            IngredientsToAvoid = details.IngredientsToAvoid,
            SkincareTips = details.SkincareTips
        });
    }

    /// <summary>
    /// Lấy danh sách tất cả loại da
    /// </summary>
    [HttpGet("skin-types")]
    public ActionResult<List<SkinTypeSummary>> GetAllSkinTypes()
    {
        var skinTypes = new List<SkinTypeSummary>();
        
        foreach (SkinType type in Enum.GetValues<SkinType>())
        {
            if (type == SkinType.All) continue;

            var details = _skinQuizService.GetSkinTypeDetails(type);
            skinTypes.Add(new SkinTypeSummary
            {
                Code = type.ToString(),
                Name = details.Name,
                ShortDescription = details.Description.Length > 100 
                    ? details.Description[..100] + "..." 
                    : details.Description
            });
        }

        return Ok(skinTypes);
    }

    /// <summary>
    /// Lấy sản phẩm gợi ý theo loại da
    /// </summary>
    [HttpGet("recommendations/{skinType}")]
    public async Task<ActionResult<ProductRecommendationsResponse>> GetRecommendations(string skinType, [FromQuery] int take = 10)
    {
        if (!Enum.TryParse<SkinType>(skinType, true, out var parsedType))
        {
            return BadRequest(new { message = $"Loại da không hợp lệ: {skinType}" });
        }

        var products = await _productRepository.GetBySkinTypeAsync(parsedType);
        var productList = products.Take(take).Select(p => new RecommendedProduct
        {
            Id = p.Id,
            Name = p.Name,
            Brand = p.Brand,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            HasDiscount = true,
            DiscountPercent = 5,
            DiscountedPrice = p.Price * 0.95m
        }).ToList();

        return Ok(new ProductRecommendationsResponse
        {
            SkinType = parsedType.ToString(),
            TotalProducts = products.Count(),
            Products = productList
        });
    }

    /// <summary>
    /// Kiểm tra user đã làm quiz chưa
    /// </summary>
    [HttpGet("status/{userId}")]
    public async Task<ActionResult<QuizStatusResponse>> GetQuizStatus(int userId)
    {
        var hasCompleted = await _skinQuizService.HasCompletedQuizAsync(userId);
        
        return Ok(new QuizStatusResponse
        {
            UserId = userId,
            HasCompletedQuiz = hasCompleted
        });
    }

    #region Response DTOs

    public class SkinQuizQuestionResponse
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<OptionResponse> Options { get; set; } = new();
    }

    public class OptionResponse
    {
        public string OptionId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class SkinQuizResultResponse
    {
        public string SkinType { get; set; } = string.Empty;
        public string SkinTypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // CÁC TRƯỜNG MỚI ĐỂ RENDER UI BIỂU ĐỒ (0-100)
        public string AiSummary { get; set; } = string.Empty;
        public int HydrationScore { get; set; }
        public int PigmentationScore { get; set; }
        public int OilinessScore { get; set; }
        public int SensitivityScore { get; set; }
        public int ElasticityScore { get; set; }

        public List<string> Characteristics { get; set; } = new();
        public List<string> RecommendedIngredients { get; set; } = new();
        public List<string> IngredientsToAvoid { get; set; } = new();
        public List<string> SkincareTips { get; set; } = new();
        public int ConfidencePercent { get; set; }
        public int MatchingProductCount { get; set; }
        public bool HasSkinTypeDiscount { get; set; }
        public decimal SkinTypeDiscountPercent { get; set; }
        public Dictionary<string, int> DetailedScores { get; set; } = new();
    }

    public class SkinTypeDetailsResponse
    {
        public string SkinType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Characteristics { get; set; } = new();
        public List<string> RecommendedIngredients { get; set; } = new();
        public List<string> IngredientsToAvoid { get; set; } = new();
        public List<string> SkincareTips { get; set; } = new();
    }

    public class SkinTypeSummary
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
    }

    public class ProductRecommendationsResponse
    {
        public string SkinType { get; set; } = string.Empty;
        public int TotalProducts { get; set; }
        public List<RecommendedProduct> Products { get; set; } = new();
    }

    public class RecommendedProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool HasDiscount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountedPrice { get; set; }
    }

    public class QuizStatusResponse
    {
        public int UserId { get; set; }
        public bool HasCompletedQuiz { get; set; }
    }

    #endregion
}