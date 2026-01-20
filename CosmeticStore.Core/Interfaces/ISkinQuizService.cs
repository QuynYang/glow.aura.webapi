using CosmeticStore.Core.Enums;
using CosmeticStore.Core.SkinQuiz;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface cho AI Skin Quiz Service
/// 
/// STRATEGY PATTERN Context:
/// - Sau khi xác định loại da, User được chuyển sang nhóm hưởng SkinTypePricingStrategy
/// - Strategy Pattern đã implement ở Phase 2
/// 
/// Workflow:
/// 1. User trả lời 10 câu hỏi
/// 2. Service tính điểm cho từng loại da
/// 3. Loại da có điểm cao nhất = kết quả
/// 4. Cập nhật User.SkinType
/// 5. User tự động hưởng SkinTypePricingStrategy (5% discount cho sản phẩm phù hợp)
/// </summary>
public interface ISkinQuizService
{
    /// <summary>
    /// Lấy danh sách câu hỏi
    /// </summary>
    List<SkinQuizQuestion> GetQuestions();

    /// <summary>
    /// Phân tích câu trả lời và xác định loại da
    /// </summary>
    /// <param name="answers">Danh sách câu trả lời</param>
    /// <returns>Kết quả phân tích</returns>
    Task<SkinQuizResult> AnalyzeSkinTypeAsync(List<QuizAnswer> answers);

    /// <summary>
    /// Lưu kết quả quiz cho user
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="skinType">Loại da được xác định</param>
    /// <returns>True nếu thành công</returns>
    Task<bool> SaveUserSkinTypeAsync(int userId, SkinType skinType);

    /// <summary>
    /// Lấy thông tin chi tiết về loại da
    /// </summary>
    /// <param name="skinType">Loại da</param>
    /// <returns>Chi tiết loại da</returns>
    SkinTypeDetails GetSkinTypeDetails(SkinType skinType);

    /// <summary>
    /// Đếm số sản phẩm phù hợp với loại da
    /// </summary>
    /// <param name="skinType">Loại da</param>
    /// <returns>Số sản phẩm</returns>
    Task<int> CountMatchingProductsAsync(SkinType skinType);

    /// <summary>
    /// Kiểm tra user đã làm quiz chưa
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <returns>True nếu đã làm</returns>
    Task<bool> HasCompletedQuizAsync(int userId);
}

