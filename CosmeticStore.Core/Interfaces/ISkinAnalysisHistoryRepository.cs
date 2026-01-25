using CosmeticStore.Core.Entities;
using CosmeticStore.Core.SkinAnalysis;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Repository Interface cho SkinAnalysisHistory
/// 
/// REPOSITORY PATTERN:
/// - Kế thừa IGenericRepository cho CRUD cơ bản
/// - Thêm các method đặc thù cho việc theo dõi da
/// </summary>
public interface ISkinAnalysisHistoryRepository : IGenericRepository<SkinAnalysisHistory>
{
    #region Query Methods

    /// <summary>
    /// Lấy lịch sử phân tích của một user
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="limit">Số lượng tối đa</param>
    /// <returns>Danh sách lịch sử (mới nhất đầu tiên)</returns>
    Task<IEnumerable<SkinAnalysisHistory>> GetByUserIdAsync(int userId, int limit = 50);

    /// <summary>
    /// Lấy lịch sử trong khoảng thời gian
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <returns>Danh sách lịch sử trong khoảng thời gian</returns>
    Task<IEnumerable<SkinAnalysisHistory>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Lấy lần phân tích gần nhất của user
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <returns>Lần phân tích gần nhất hoặc null</returns>
    Task<SkinAnalysisHistory?> GetLatestByUserIdAsync(int userId);

    /// <summary>
    /// Lấy lần phân tích theo mã
    /// </summary>
    /// <param name="analysisCode">Mã phân tích</param>
    /// <returns>Lịch sử phân tích hoặc null</returns>
    Task<SkinAnalysisHistory?> GetByAnalysisCodeAsync(string analysisCode);

    /// <summary>
    /// Lấy N lần phân tích gần nhất để so sánh xu hướng
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="count">Số lượng lần phân tích</param>
    /// <returns>Danh sách lịch sử</returns>
    Task<IEnumerable<SkinAnalysisHistory>> GetRecentForTrendAsync(int userId, int count = 10);

    #endregion

    #region Statistics Methods

    /// <summary>
    /// Lấy điểm trung bình trong khoảng thời gian
    /// </summary>
    Task<decimal> GetAverageScoreAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Lấy điểm cao nhất đạt được
    /// </summary>
    Task<decimal> GetHighestScoreAsync(int userId);

    /// <summary>
    /// Lấy số lần phân tích
    /// </summary>
    Task<int> GetAnalysisCountAsync(int userId);

    /// <summary>
    /// Kiểm tra user đã phân tích trong ngày hôm nay chưa
    /// </summary>
    Task<bool> HasAnalyzedTodayAsync(int userId);

    #endregion

    #region Comparison Methods

    /// <summary>
    /// Lấy 2 lần phân tích liên tiếp để so sánh
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="analysisId">ID lần phân tích hiện tại</param>
    /// <returns>Tuple (current, previous)</returns>
    Task<(SkinAnalysisHistory? Current, SkinAnalysisHistory? Previous)> GetForComparisonAsync(int userId, int analysisId);

    /// <summary>
    /// So sánh với lần đầu tiên phân tích
    /// </summary>
    Task<SkinAnalysisHistory?> GetFirstAnalysisAsync(int userId);

    #endregion
}

