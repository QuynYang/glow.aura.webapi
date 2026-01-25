using CosmeticStore.Core.SkinAnalysis;

namespace CosmeticStore.Core.Entities;

/// <summary>
/// Entity lưu lịch sử phân tích da
/// 
/// ENCAPSULATION:
/// - Private setters để bảo vệ dữ liệu
/// - Logic nghiệp vụ nằm trong class
/// 
/// Chức năng:
/// - Lưu kết quả phân tích theo thời gian
/// - Cho phép xem lại lịch sử
/// - So sánh kết quả giữa các lần
/// - Nhận biết xu hướng cải thiện/xấu đi
/// </summary>
public class SkinAnalysisHistory : BaseEntity
{
    #region Properties

    /// <summary>
    /// ID người dùng (Foreign Key)
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Mã phân tích (unique)
    /// </summary>
    public string AnalysisCode { get; private set; } = string.Empty;

    /// <summary>
    /// Thời điểm chụp ảnh
    /// </summary>
    public DateTime CapturedAt { get; private set; }

    /// <summary>
    /// Đường dẫn ảnh gốc
    /// </summary>
    public string? OriginalImagePath { get; private set; }

    /// <summary>
    /// Đường dẫn ảnh khuôn mặt đã cắt
    /// </summary>
    public string? CroppedFacePath { get; private set; }

    #endregion

    #region Kết quả phân tích

    /// <summary>
    /// Có phát hiện khuôn mặt không
    /// </summary>
    public bool FaceDetected { get; private set; }

    /// <summary>
    /// Độ tin cậy phát hiện khuôn mặt (0-100)
    /// </summary>
    public decimal FaceConfidence { get; private set; }

    /// <summary>
    /// Độ sáng da (0-100)
    /// </summary>
    public decimal Brightness { get; private set; }

    /// <summary>
    /// Độ đều màu da (0-100)
    /// </summary>
    public decimal Evenness { get; private set; }

    /// <summary>
    /// Độ mịn màng (0-100)
    /// </summary>
    public decimal Smoothness { get; private set; }

    /// <summary>
    /// Mức độ ẩm da (0-100)
    /// </summary>
    public decimal Hydration { get; private set; }

    /// <summary>
    /// Mức độ dầu nhờn (0-100)
    /// </summary>
    public decimal Oiliness { get; private set; }

    /// <summary>
    /// Số lượng mụn phát hiện được
    /// </summary>
    public int AcneCount { get; private set; }

    /// <summary>
    /// Mức độ nghiêm trọng của mụn (0-100)
    /// </summary>
    public decimal AcneSeverity { get; private set; }

    /// <summary>
    /// Số lượng đốm nâu
    /// </summary>
    public int DarkSpotCount { get; private set; }

    /// <summary>
    /// Mức độ nghiêm trọng của đốm nâu (0-100)
    /// </summary>
    public decimal DarkSpotSeverity { get; private set; }

    /// <summary>
    /// Mức độ nếp nhăn (0-100)
    /// </summary>
    public decimal WrinkleLevel { get; private set; }

    /// <summary>
    /// Mức độ lỗ chân lông (0-100)
    /// </summary>
    public decimal PoreSize { get; private set; }

    /// <summary>
    /// Mức độ đỏ da (0-100)
    /// </summary>
    public decimal Redness { get; private set; }

    /// <summary>
    /// Điểm sức khỏe da tổng thể (0-100)
    /// </summary>
    public decimal OverallScore { get; private set; }

    /// <summary>
    /// Tình trạng da (Excellent, Good, Normal, NeedsAttention, Poor)
    /// </summary>
    public SkinCondition Condition { get; private set; }

    /// <summary>
    /// Loại da phát hiện được
    /// </summary>
    public DetectedSkinType DetectedSkinType { get; private set; }

    /// <summary>
    /// Danh sách vấn đề da (JSON serialized)
    /// </summary>
    public string? ConcernsJson { get; private set; }

    /// <summary>
    /// Danh sách lời khuyên (JSON serialized)
    /// </summary>
    public string? RecommendationsJson { get; private set; }

    #endregion

    #region Comparison with Previous

    /// <summary>
    /// ID lần phân tích trước đó (để so sánh)
    /// </summary>
    public int? PreviousAnalysisId { get; private set; }

    /// <summary>
    /// Thay đổi điểm so với lần trước
    /// </summary>
    public decimal? ScoreChange { get; private set; }

    /// <summary>
    /// Xu hướng so với lần trước
    /// </summary>
    public TrendDirection? TrendFromPrevious { get; private set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// User sở hữu lịch sử này
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Lần phân tích trước đó
    /// </summary>
    public SkinAnalysisHistory? PreviousAnalysis { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor mặc định cho EF Core
    /// </summary>
    protected SkinAnalysisHistory() { }

    /// <summary>
    /// Constructor từ SkinAnalysisResult
    /// </summary>
    public SkinAnalysisHistory(int userId, SkinAnalysisResult result)
    {
        UserId = userId;
        AnalysisCode = result.AnalysisId ?? GenerateAnalysisCode();
        CapturedAt = result.AnalyzedAt;
        OriginalImagePath = result.OriginalImagePath;
        CroppedFacePath = result.CroppedFacePath;

        FaceDetected = result.FaceDetected;
        FaceConfidence = result.FaceConfidence;
        Brightness = result.Brightness;
        Evenness = result.Evenness;
        Smoothness = result.Smoothness;
        Hydration = result.Hydration;
        Oiliness = result.Oiliness;
        AcneCount = result.AcneCount;
        AcneSeverity = result.AcneSeverity;
        DarkSpotCount = result.DarkSpotCount;
        DarkSpotSeverity = result.DarkSpotSeverity;
        WrinkleLevel = result.WrinkleLevel;
        PoreSize = result.PoreSize;
        Redness = result.Redness;
        OverallScore = result.OverallScore;
        Condition = result.Condition;
        DetectedSkinType = result.DetectedSkinType;

        // Serialize concerns and recommendations to JSON
        if (result.DetectedConcerns?.Any() == true)
        {
            ConcernsJson = System.Text.Json.JsonSerializer.Serialize(
                result.DetectedConcerns.Select(c => new
                {
                    c.Type,
                    c.Severity,
                    c.Description,
                    c.Location
                }));
        }

        if (result.Recommendations?.Any() == true)
        {
            RecommendationsJson = System.Text.Json.JsonSerializer.Serialize(
                result.Recommendations.Select(r => new
                {
                    r.Priority,
                    r.Category,
                    r.Title,
                    r.Description,
                    r.ActionType
                }));
        }
    }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Liên kết với lần phân tích trước và tính toán xu hướng
    /// </summary>
    public void LinkToPreviousAnalysis(SkinAnalysisHistory previous)
    {
        PreviousAnalysisId = previous.Id;
        ScoreChange = OverallScore - previous.OverallScore;

        if (ScoreChange > 5)
            TrendFromPrevious = TrendDirection.Improving;
        else if (ScoreChange < -5)
            TrendFromPrevious = TrendDirection.Worsening;
        else
            TrendFromPrevious = TrendDirection.Stable;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cập nhật đường dẫn ảnh
    /// </summary>
    public void UpdateImagePaths(string? originalPath, string? croppedPath)
    {
        OriginalImagePath = originalPath;
        CroppedFacePath = croppedPath;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Chuyển đổi thành SkinAnalysisResult
    /// </summary>
    public SkinAnalysisResult ToAnalysisResult()
    {
        var concerns = DeserializeConcerns();
        var recommendations = DeserializeRecommendations();

        return new SkinAnalysisResult(
            analysisId: AnalysisCode,
            analyzedAt: CapturedAt,
            faceDetected: FaceDetected,
            faceConfidence: FaceConfidence,
            brightness: Brightness,
            evenness: Evenness,
            smoothness: Smoothness,
            hydration: Hydration,
            oiliness: Oiliness,
            acneCount: AcneCount,
            acneSeverity: AcneSeverity,
            darkSpotCount: DarkSpotCount,
            darkSpotSeverity: DarkSpotSeverity,
            wrinkleLevel: WrinkleLevel,
            poreSize: PoreSize,
            redness: Redness,
            overallScore: OverallScore,
            condition: Condition,
            detectedSkinType: DetectedSkinType,
            detectedConcerns: concerns,
            recommendations: recommendations,
            originalImagePath: OriginalImagePath,
            croppedFacePath: CroppedFacePath
        );
    }

    /// <summary>
    /// Lấy mô tả tình trạng da
    /// </summary>
    public string GetConditionDescription()
    {
        return Condition switch
        {
            SkinCondition.Excellent => "Da rất tốt! Tiếp tục duy trì chế độ chăm sóc hiện tại.",
            SkinCondition.Good => "Da khỏe mạnh. Một số điểm nhỏ cần lưu ý.",
            SkinCondition.Normal => "Da bình thường. Có thể cải thiện thêm với routine phù hợp.",
            SkinCondition.NeedsAttention => "Da cần được chú ý chăm sóc đúng cách.",
            SkinCondition.Poor => "Da có nhiều vấn đề. Nên tham khảo ý kiến chuyên gia.",
            _ => "Không xác định được tình trạng da."
        };
    }

    /// <summary>
    /// Lấy mô tả xu hướng
    /// </summary>
    public string GetTrendDescription()
    {
        if (!TrendFromPrevious.HasValue)
            return "Chưa có dữ liệu so sánh.";

        return TrendFromPrevious.Value switch
        {
            TrendDirection.Improving => $"Da đang cải thiện! (+{ScoreChange:N1} điểm)",
            TrendDirection.Stable => "Tình trạng da ổn định.",
            TrendDirection.Worsening => $"Da có dấu hiệu xấu đi. ({ScoreChange:N1} điểm)",
            _ => "Không xác định xu hướng."
        };
    }

    #endregion

    #region Private Methods

    private static string GenerateAnalysisCode()
    {
        return $"SKA{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }

    private IReadOnlyList<SkinConcern> DeserializeConcerns()
    {
        if (string.IsNullOrEmpty(ConcernsJson))
            return Array.Empty<SkinConcern>();

        try
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<List<ConcernDto>>(ConcernsJson);
            return data?.Select(d => new SkinConcern(d.Type, d.Severity, d.Description, d.Location)).ToList()
                   ?? new List<SkinConcern>();
        }
        catch
        {
            return Array.Empty<SkinConcern>();
        }
    }

    private IReadOnlyList<SkinAdvice> DeserializeRecommendations()
    {
        if (string.IsNullOrEmpty(RecommendationsJson))
            return Array.Empty<SkinAdvice>();

        try
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<List<AdviceDto>>(RecommendationsJson);
            return data?.Select(d => new SkinAdvice(d.Priority, d.Category, d.Title, d.Description, d.ActionType)).ToList()
                   ?? new List<SkinAdvice>();
        }
        catch
        {
            return Array.Empty<SkinAdvice>();
        }
    }

    // DTO for JSON deserialization
    private class ConcernDto
    {
        public SkinConcernType Type { get; set; }
        public decimal Severity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
    }

    private class AdviceDto
    {
        public int Priority { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AdviceActionType ActionType { get; set; }
    }

    #endregion
}

