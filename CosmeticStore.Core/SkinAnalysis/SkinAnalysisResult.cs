namespace CosmeticStore.Core.SkinAnalysis;

/// <summary>
/// Value Object - Kết quả phân tích da mặt
/// 
/// ADAPTER PATTERN:
/// - Đây là "cầu nối" giữa Core và Infrastructure
/// - Dù bên dưới dùng OpenCvSharp, Python AI, hay Cloud Vision API
/// - Kết quả trả về đều cùng định dạng này
/// 
/// VALUE OBJECT:
/// - Immutable (không thay đổi sau khi tạo)
/// - Equality by value (so sánh theo giá trị, không theo reference)
/// </summary>
public class SkinAnalysisResult
{
    #region Thông tin phân tích

    /// <summary>
    /// ID định danh kết quả phân tích
    /// </summary>
    public string AnalysisId { get; }

    /// <summary>
    /// Thời điểm phân tích
    /// </summary>
    public DateTime AnalyzedAt { get; }

    /// <summary>
    /// Có phát hiện khuôn mặt không
    /// </summary>
    public bool FaceDetected { get; }

    /// <summary>
    /// Độ tin cậy của việc phát hiện khuôn mặt (0-100%)
    /// </summary>
    public decimal FaceConfidence { get; }

    #endregion

    #region Chỉ số da

    /// <summary>
    /// Độ sáng da (0-100)
    /// 0 = Rất tối, 100 = Rất sáng
    /// </summary>
    public decimal Brightness { get; }

    /// <summary>
    /// Mức độ đều màu da (0-100)
    /// 0 = Không đều màu, 100 = Rất đều màu
    /// </summary>
    public decimal Evenness { get; }

    /// <summary>
    /// Mức độ mịn màng (0-100)
    /// 0 = Rất thô ráp, 100 = Rất mịn màng
    /// </summary>
    public decimal Smoothness { get; }

    /// <summary>
    /// Mức độ ẩm da (0-100)
    /// Ước tính dựa trên độ bóng/khô của da
    /// </summary>
    public decimal Hydration { get; }

    /// <summary>
    /// Mức độ dầu nhờn (0-100)
    /// 0 = Khô, 100 = Rất dầu
    /// </summary>
    public decimal Oiliness { get; }

    #endregion

    #region Phát hiện vấn đề da

    /// <summary>
    /// Số lượng mụn phát hiện được
    /// </summary>
    public int AcneCount { get; }

    /// <summary>
    /// Mức độ nghiêm trọng của mụn (0-100)
    /// </summary>
    public decimal AcneSeverity { get; }

    /// <summary>
    /// Số lượng đốm nâu/tàn nhang phát hiện được
    /// </summary>
    public int DarkSpotCount { get; }

    /// <summary>
    /// Mức độ nghiêm trọng của đốm nâu (0-100)
    /// </summary>
    public decimal DarkSpotSeverity { get; }

    /// <summary>
    /// Mức độ nếp nhăn (0-100)
    /// 0 = Không có, 100 = Nhiều nếp nhăn
    /// </summary>
    public decimal WrinkleLevel { get; }

    /// <summary>
    /// Mức độ lỗ chân lông to (0-100)
    /// 0 = Lỗ chân lông nhỏ, 100 = Rất to
    /// </summary>
    public decimal PoreSize { get; }

    /// <summary>
    /// Mức độ đỏ da/kích ứng (0-100)
    /// </summary>
    public decimal Redness { get; }

    #endregion

    #region Đánh giá tổng quan

    /// <summary>
    /// Điểm sức khỏe da tổng thể (0-100)
    /// Tính toán từ các chỉ số trên
    /// </summary>
    public decimal OverallScore { get; }

    /// <summary>
    /// Phân loại tình trạng da
    /// </summary>
    public SkinCondition Condition { get; }

    /// <summary>
    /// Loại da phát hiện được (dựa trên phân tích)
    /// </summary>
    public DetectedSkinType DetectedSkinType { get; }

    /// <summary>
    /// Danh sách vấn đề da được phát hiện
    /// </summary>
    public IReadOnlyList<SkinConcern> DetectedConcerns { get; }

    /// <summary>
    /// Lời khuyên chăm sóc da
    /// </summary>
    public IReadOnlyList<SkinAdvice> Recommendations { get; }

    #endregion

    #region Thông tin ảnh

    /// <summary>
    /// Đường dẫn/URL ảnh gốc
    /// </summary>
    public string? OriginalImagePath { get; }

    /// <summary>
    /// Đường dẫn/URL ảnh khuôn mặt đã cắt
    /// </summary>
    public string? CroppedFacePath { get; }

    /// <summary>
    /// Tọa độ khuôn mặt trong ảnh (x, y, width, height)
    /// </summary>
    public FaceRegion? FaceRegion { get; }

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor đầy đủ
    /// </summary>
    public SkinAnalysisResult(
        string analysisId,
        DateTime analyzedAt,
        bool faceDetected,
        decimal faceConfidence,
        decimal brightness,
        decimal evenness,
        decimal smoothness,
        decimal hydration,
        decimal oiliness,
        int acneCount,
        decimal acneSeverity,
        int darkSpotCount,
        decimal darkSpotSeverity,
        decimal wrinkleLevel,
        decimal poreSize,
        decimal redness,
        decimal overallScore,
        SkinCondition condition,
        DetectedSkinType detectedSkinType,
        IReadOnlyList<SkinConcern> detectedConcerns,
        IReadOnlyList<SkinAdvice> recommendations,
        string? originalImagePath = null,
        string? croppedFacePath = null,
        FaceRegion? faceRegion = null)
    {
        AnalysisId = analysisId;
        AnalyzedAt = analyzedAt;
        FaceDetected = faceDetected;
        FaceConfidence = faceConfidence;
        Brightness = brightness;
        Evenness = evenness;
        Smoothness = smoothness;
        Hydration = hydration;
        Oiliness = oiliness;
        AcneCount = acneCount;
        AcneSeverity = acneSeverity;
        DarkSpotCount = darkSpotCount;
        DarkSpotSeverity = darkSpotSeverity;
        WrinkleLevel = wrinkleLevel;
        PoreSize = poreSize;
        Redness = redness;
        OverallScore = overallScore;
        Condition = condition;
        DetectedSkinType = detectedSkinType;
        DetectedConcerns = detectedConcerns;
        Recommendations = recommendations;
        OriginalImagePath = originalImagePath;
        CroppedFacePath = croppedFacePath;
        FaceRegion = faceRegion;
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Tạo kết quả khi không phát hiện được khuôn mặt
    /// </summary>
    public static SkinAnalysisResult NoFaceDetected(string? imagePath = null)
    {
        return new SkinAnalysisResult(
            analysisId: Guid.NewGuid().ToString(),
            analyzedAt: DateTime.UtcNow,
            faceDetected: false,
            faceConfidence: 0,
            brightness: 0,
            evenness: 0,
            smoothness: 0,
            hydration: 0,
            oiliness: 0,
            acneCount: 0,
            acneSeverity: 0,
            darkSpotCount: 0,
            darkSpotSeverity: 0,
            wrinkleLevel: 0,
            poreSize: 0,
            redness: 0,
            overallScore: 0,
            condition: SkinCondition.Unknown,
            detectedSkinType: DetectedSkinType.Unknown,
            detectedConcerns: Array.Empty<SkinConcern>(),
            recommendations: new List<SkinAdvice>
            {
                new SkinAdvice(
                    priority: 1,
                    category: "Hướng dẫn",
                    title: "Không phát hiện khuôn mặt",
                    description: "Vui lòng chụp lại ảnh với khuôn mặt rõ ràng, đủ ánh sáng và nhìn thẳng vào camera.",
                    actionType: AdviceActionType.Instruction
                )
            },
            originalImagePath: imagePath
        );
    }

    /// <summary>
    /// Tạo kết quả khi có lỗi phân tích
    /// </summary>
    public static SkinAnalysisResult Error(string errorMessage)
    {
        return new SkinAnalysisResult(
            analysisId: Guid.NewGuid().ToString(),
            analyzedAt: DateTime.UtcNow,
            faceDetected: false,
            faceConfidence: 0,
            brightness: 0,
            evenness: 0,
            smoothness: 0,
            hydration: 0,
            oiliness: 0,
            acneCount: 0,
            acneSeverity: 0,
            darkSpotCount: 0,
            darkSpotSeverity: 0,
            wrinkleLevel: 0,
            poreSize: 0,
            redness: 0,
            overallScore: 0,
            condition: SkinCondition.Unknown,
            detectedSkinType: DetectedSkinType.Unknown,
            detectedConcerns: Array.Empty<SkinConcern>(),
            recommendations: new List<SkinAdvice>
            {
                new SkinAdvice(
                    priority: 1,
                    category: "Lỗi",
                    title: "Không thể phân tích",
                    description: errorMessage,
                    actionType: AdviceActionType.Error
                )
            }
        );
    }

    #endregion

    #region Comparison Methods

    /// <summary>
    /// So sánh với kết quả phân tích trước đó
    /// </summary>
    public SkinTrendAnalysis CompareTo(SkinAnalysisResult previousResult)
    {
        return new SkinTrendAnalysis(
            currentResult: this,
            previousResult: previousResult
        );
    }

    #endregion
}

#region Supporting Types

/// <summary>
/// Tình trạng da tổng quan
/// </summary>
public enum SkinCondition
{
    /// <summary>Không xác định</summary>
    Unknown = 0,
    
    /// <summary>Da rất tốt (85-100)</summary>
    Excellent = 1,
    
    /// <summary>Da tốt (70-84)</summary>
    Good = 2,
    
    /// <summary>Da bình thường (55-69)</summary>
    Normal = 3,
    
    /// <summary>Da cần chú ý (40-54)</summary>
    NeedsAttention = 4,
    
    /// <summary>Da có vấn đề (0-39)</summary>
    Poor = 5
}

/// <summary>
/// Loại da phát hiện được từ phân tích
/// </summary>
public enum DetectedSkinType
{
    /// <summary>Không xác định</summary>
    Unknown = 0,
    
    /// <summary>Da dầu</summary>
    Oily = 1,
    
    /// <summary>Da khô</summary>
    Dry = 2,
    
    /// <summary>Da hỗn hợp</summary>
    Combination = 3,
    
    /// <summary>Da thường</summary>
    Normal = 4,
    
    /// <summary>Da nhạy cảm</summary>
    Sensitive = 5
}

/// <summary>
/// Vấn đề da được phát hiện
/// </summary>
public class SkinConcern
{
    /// <summary>Loại vấn đề</summary>
    public SkinConcernType Type { get; }
    
    /// <summary>Mức độ nghiêm trọng (0-100)</summary>
    public decimal Severity { get; }
    
    /// <summary>Vị trí trên khuôn mặt (nếu có)</summary>
    public string? Location { get; }
    
    /// <summary>Mô tả chi tiết</summary>
    public string Description { get; }

    public SkinConcern(SkinConcernType type, decimal severity, string description, string? location = null)
    {
        Type = type;
        Severity = severity;
        Description = description;
        Location = location;
    }
}

/// <summary>
/// Loại vấn đề da
/// </summary>
public enum SkinConcernType
{
    Acne,           // Mụn
    DarkSpots,      // Đốm nâu/tàn nhang
    Wrinkles,       // Nếp nhăn
    LargePores,     // Lỗ chân lông to
    Redness,        // Da đỏ/kích ứng
    Dryness,        // Da khô
    Oiliness,       // Da dầu
    UnevenTone,     // Không đều màu
    Dullness,       // Da xỉn màu
    Dehydration     // Thiếu nước
}

/// <summary>
/// Lời khuyên chăm sóc da
/// </summary>
public class SkinAdvice
{
    /// <summary>Độ ưu tiên (1 = cao nhất)</summary>
    public int Priority { get; }
    
    /// <summary>Danh mục lời khuyên</summary>
    public string Category { get; }
    
    /// <summary>Tiêu đề ngắn</summary>
    public string Title { get; }
    
    /// <summary>Mô tả chi tiết</summary>
    public string Description { get; }
    
    /// <summary>Loại hành động</summary>
    public AdviceActionType ActionType { get; }
    
    /// <summary>Sản phẩm gợi ý (nếu có)</summary>
    public IReadOnlyList<int>? RecommendedProductIds { get; }
    
    /// <summary>Link tham khảo (nếu có)</summary>
    public string? ReferenceUrl { get; }

    public SkinAdvice(
        int priority,
        string category,
        string title,
        string description,
        AdviceActionType actionType,
        IReadOnlyList<int>? recommendedProductIds = null,
        string? referenceUrl = null)
    {
        Priority = priority;
        Category = category;
        Title = title;
        Description = description;
        ActionType = actionType;
        RecommendedProductIds = recommendedProductIds;
        ReferenceUrl = referenceUrl;
    }
}

/// <summary>
/// Loại hành động lời khuyên
/// </summary>
public enum AdviceActionType
{
    Instruction,        // Hướng dẫn
    ProductSuggestion,  // Gợi ý sản phẩm
    LifestyleChange,    // Thay đổi lối sống
    DoctorConsultation, // Tham vấn bác sĩ
    Warning,            // Cảnh báo
    Error               // Lỗi
}

/// <summary>
/// Vùng khuôn mặt trong ảnh
/// </summary>
public class FaceRegion
{
    /// <summary>Tọa độ X (góc trái trên)</summary>
    public int X { get; }
    
    /// <summary>Tọa độ Y (góc trái trên)</summary>
    public int Y { get; }
    
    /// <summary>Chiều rộng</summary>
    public int Width { get; }
    
    /// <summary>Chiều cao</summary>
    public int Height { get; }

    public FaceRegion(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}

/// <summary>
/// Phân tích xu hướng da theo thời gian
/// </summary>
public class SkinTrendAnalysis
{
    /// <summary>Kết quả hiện tại</summary>
    public SkinAnalysisResult CurrentResult { get; }
    
    /// <summary>Kết quả trước đó</summary>
    public SkinAnalysisResult PreviousResult { get; }
    
    /// <summary>Khoảng thời gian giữa 2 lần phân tích</summary>
    public TimeSpan TimeDifference { get; }
    
    /// <summary>Xu hướng tổng thể</summary>
    public TrendDirection OverallTrend { get; }
    
    /// <summary>Thay đổi điểm sức khỏe da</summary>
    public decimal ScoreChange { get; }
    
    /// <summary>Phần trăm thay đổi</summary>
    public decimal ChangePercent { get; }
    
    /// <summary>Các chỉ số cải thiện</summary>
    public IReadOnlyList<string> Improvements { get; }
    
    /// <summary>Các chỉ số xấu đi</summary>
    public IReadOnlyList<string> Deteriorations { get; }
    
    /// <summary>Nhận xét xu hướng</summary>
    public string Summary { get; }

    public SkinTrendAnalysis(SkinAnalysisResult currentResult, SkinAnalysisResult previousResult)
    {
        CurrentResult = currentResult;
        PreviousResult = previousResult;
        TimeDifference = currentResult.AnalyzedAt - previousResult.AnalyzedAt;
        
        ScoreChange = currentResult.OverallScore - previousResult.OverallScore;
        ChangePercent = previousResult.OverallScore > 0 
            ? (ScoreChange / previousResult.OverallScore) * 100 
            : 0;

        var improvements = new List<string>();
        var deteriorations = new List<string>();

        // So sánh các chỉ số
        CompareMetric("Độ sáng", currentResult.Brightness, previousResult.Brightness, improvements, deteriorations);
        CompareMetric("Độ đều màu", currentResult.Evenness, previousResult.Evenness, improvements, deteriorations);
        CompareMetric("Độ mịn", currentResult.Smoothness, previousResult.Smoothness, improvements, deteriorations);
        CompareMetric("Độ ẩm", currentResult.Hydration, previousResult.Hydration, improvements, deteriorations);
        
        // Các chỉ số thấp hơn là tốt hơn
        CompareMetricReverse("Số mụn", currentResult.AcneCount, previousResult.AcneCount, improvements, deteriorations);
        CompareMetricReverse("Đốm nâu", currentResult.DarkSpotCount, previousResult.DarkSpotCount, improvements, deteriorations);
        CompareMetricReverse("Lỗ chân lông", currentResult.PoreSize, previousResult.PoreSize, improvements, deteriorations);
        CompareMetricReverse("Đỏ da", currentResult.Redness, previousResult.Redness, improvements, deteriorations);

        Improvements = improvements;
        Deteriorations = deteriorations;

        // Xác định xu hướng
        if (ScoreChange > 5)
            OverallTrend = TrendDirection.Improving;
        else if (ScoreChange < -5)
            OverallTrend = TrendDirection.Worsening;
        else
            OverallTrend = TrendDirection.Stable;

        // Tạo summary
        Summary = GenerateSummary();
    }

    private void CompareMetric(string name, decimal current, decimal previous, List<string> improvements, List<string> deteriorations)
    {
        var diff = current - previous;
        if (diff > 3) improvements.Add($"{name} (+{diff:N1})");
        else if (diff < -3) deteriorations.Add($"{name} ({diff:N1})");
    }

    private void CompareMetricReverse(string name, decimal current, decimal previous, List<string> improvements, List<string> deteriorations)
    {
        var diff = current - previous;
        if (diff < -1) improvements.Add($"{name} ({diff:N0})");
        else if (diff > 1) deteriorations.Add($"{name} (+{diff:N0})");
    }

    private string GenerateSummary()
    {
        return OverallTrend switch
        {
            TrendDirection.Improving => $"Da của bạn đang cải thiện! Điểm sức khỏe da tăng {ScoreChange:N1} điểm ({ChangePercent:N1}%) trong {TimeDifference.Days} ngày.",
            TrendDirection.Worsening => $"Tình trạng da có dấu hiệu xấu đi. Điểm sức khỏe da giảm {Math.Abs(ScoreChange):N1} điểm trong {TimeDifference.Days} ngày. Cần chú ý chăm sóc.",
            TrendDirection.Stable => $"Tình trạng da ổn định trong {TimeDifference.Days} ngày qua. Tiếp tục duy trì routine chăm sóc da hiện tại.",
            _ => "Không đủ dữ liệu để đánh giá xu hướng."
        };
    }
}

/// <summary>
/// Hướng xu hướng
/// </summary>
public enum TrendDirection
{
    /// <summary>Không xác định</summary>
    Unknown = 0,
    
    /// <summary>Đang cải thiện</summary>
    Improving = 1,
    
    /// <summary>Ổn định</summary>
    Stable = 2,
    
    /// <summary>Đang xấu đi</summary>
    Worsening = 3
}

#endregion

