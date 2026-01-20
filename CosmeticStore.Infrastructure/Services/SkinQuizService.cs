using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Events;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.SkinQuiz;
using CosmeticStore.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// AI Skin Quiz Service
/// 
/// STRATEGY PATTERN Context:
/// - Phân tích câu trả lời → Xác định loại da
/// - Cập nhật User.SkinType → User tự động hưởng SkinTypePricingStrategy
/// 
/// AI Logic:
/// - Mỗi câu trả lời có điểm số cho từng loại da
/// - Tổng điểm cao nhất = Loại da chính
/// - Confidence = (Điểm cao nhất / Tổng điểm) × 100%
/// </summary>
public class SkinQuizService : ISkinQuizService
{
    private readonly StoreDbContext _dbContext;
    private readonly IProductRepository _productRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ISystemLogger _logger;

    // Cấu hình
    private const decimal SKIN_TYPE_DISCOUNT_PERCENT = 5m; // 5% discount cho sản phẩm phù hợp

    public SkinQuizService(
        StoreDbContext dbContext,
        IProductRepository productRepository,
        IDomainEventDispatcher eventDispatcher,
        ISystemLogger logger)
    {
        _dbContext = dbContext;
        _productRepository = productRepository;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách câu hỏi
    /// </summary>
    public List<SkinQuizQuestion> GetQuestions()
    {
        return SkinQuizQuestions.Questions;
    }

    /// <summary>
    /// Phân tích câu trả lời và xác định loại da
    /// 
    /// AI LOGIC:
    /// 1. Duyệt qua từng câu trả lời
    /// 2. Lấy điểm số từ option được chọn
    /// 3. Cộng dồn điểm cho mỗi loại da
    /// 4. Loại da có tổng điểm cao nhất = Kết quả
    /// </summary>
    public async Task<SkinQuizResult> AnalyzeSkinTypeAsync(List<QuizAnswer> answers)
    {
        _logger.LogInfo("Starting skin type analysis", new { AnswerCount = answers.Count });

        // Khởi tạo bảng điểm
        var scores = new Dictionary<SkinType, int>
        {
            { SkinType.Oily, 0 },
            { SkinType.Dry, 0 },
            { SkinType.Sensitive, 0 },
            { SkinType.Normal, 0 },
            { SkinType.Combination, 0 }
        };

        // Tính điểm cho mỗi câu trả lời
        foreach (var answer in answers)
        {
            var option = SkinQuizQuestions.GetOption(answer.QuestionId, answer.SelectedOptionId);
            if (option != null)
            {
                foreach (var score in option.SkinTypeScores)
                {
                    scores[score.Key] += score.Value;
                }
            }
        }

        // Tìm loại da có điểm cao nhất
        var determinedSkinType = scores.OrderByDescending(s => s.Value).First().Key;
        var maxScore = scores[determinedSkinType];
        var totalScore = scores.Values.Sum();

        // Tính độ tin cậy
        var confidence = totalScore > 0 
            ? (int)Math.Round((double)maxScore / totalScore * 100) 
            : 0;

        // Lấy thông tin chi tiết
        var details = GetSkinTypeDetails(determinedSkinType);

        // Đếm sản phẩm phù hợp
        var matchingProducts = await CountMatchingProductsAsync(determinedSkinType);

        var result = new SkinQuizResult
        {
            DeterminedSkinType = determinedSkinType,
            SkinTypeName = details.Name,
            Description = details.Description,
            Characteristics = details.Characteristics,
            RecommendedIngredients = details.RecommendedIngredients,
            IngredientsToAvoid = details.IngredientsToAvoid,
            SkincareTips = details.SkincareTips,
            DetailedScores = scores,
            ConfidencePercent = confidence,
            MatchingProductCount = matchingProducts,
            HasSkinTypeDiscount = true,
            SkinTypeDiscountPercent = SKIN_TYPE_DISCOUNT_PERCENT
        };

        _logger.LogInfo($"Skin type determined: {determinedSkinType}", new 
        { 
            Confidence = confidence,
            MatchingProducts = matchingProducts,
            DetailedScores = scores
        });

        return result;
    }

    /// <summary>
    /// Lưu kết quả quiz cho user
    /// 
    /// STRATEGY PATTERN:
    /// - Sau khi lưu SkinType, user tự động được áp dụng SkinTypePricingStrategy
    /// - Khi mua sản phẩm phù hợp loại da → Giảm 5%
    /// </summary>
    public async Task<bool> SaveUserSkinTypeAsync(int userId, SkinType skinType)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User not found for skin quiz: {userId}");
                return false;
            }

            // Gọi method của User Entity (Encapsulation)
            user.CompleteSkinQuiz(skinType);

            await _dbContext.SaveChangesAsync();

            _logger.LogInfo($"User {userId} completed skin quiz", new 
            { 
                SkinType = skinType.ToString(),
                CompletedAt = DateTime.UtcNow
            });

            // Publish event để các handler khác xử lý (Observer Pattern)
            // Ví dụ: Gửi email chào mừng, push notification gợi ý sản phẩm...
            var matchingProducts = await _productRepository.GetBySkinTypeAsync(skinType);
            var productIds = matchingProducts.Take(5).Select(p => p.Id).ToList();

            // TODO: Publish SkinQuizCompletedEvent nếu cần

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save skin quiz result for user {userId}", ex);
            return false;
        }
    }

    /// <summary>
    /// Lấy thông tin chi tiết về loại da
    /// </summary>
    public SkinTypeDetails GetSkinTypeDetails(SkinType skinType)
    {
        if (SkinTypeInfo.Details.TryGetValue(skinType, out var details))
        {
            return details;
        }

        // Fallback cho Unknown
        return new SkinTypeDetails
        {
            Name = "Chưa xác định",
            Description = "Vui lòng hoàn thành bài trắc nghiệm để xác định loại da.",
            Characteristics = new List<string>(),
            RecommendedIngredients = new List<string>(),
            IngredientsToAvoid = new List<string>(),
            SkincareTips = new List<string> { "Hoàn thành Skin Quiz để nhận tư vấn phù hợp!" }
        };
    }

    /// <summary>
    /// Đếm số sản phẩm phù hợp với loại da
    /// </summary>
    public async Task<int> CountMatchingProductsAsync(SkinType skinType)
    {
        return await _dbContext.Products
            .Where(p => p.SkinType == skinType && !p.IsDeleted)
            .CountAsync();
    }

    /// <summary>
    /// Kiểm tra user đã làm quiz chưa
    /// </summary>
    public async Task<bool> HasCompletedQuizAsync(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        return user?.HasCompletedSkinQuiz ?? false;
    }
}

