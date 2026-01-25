using CosmeticStore.Core.SkinAnalysis;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface Skin Analysis Service - ADAPTER PATTERN
/// 
/// ADAPTER PATTERN:
/// - Định nghĩa "khuôn mẫu" cho việc phân tích da
/// - Không quan tâm bên dưới dùng thuật toán gì
/// - Dễ dàng thay đổi implementation:
///   + OpenCvSharp (local processing)
///   + Python AI Server (remote processing)
///   + Azure Computer Vision (cloud processing)
///   + Google Cloud Vision (cloud processing)
///   + Custom ML Model (local/remote)
/// 
/// ABSTRACTION:
/// - Core layer chỉ biết interface này
/// - Infrastructure layer implement với thuật toán cụ thể
/// - Tuân thủ Dependency Inversion Principle
/// 
/// SỬ DỤNG:
/// 1. Chụp ảnh khuôn mặt từ camera
/// 2. Gọi AnalyzeAsync() với stream ảnh
/// 3. Nhận kết quả SkinAnalysisResult
/// 4. Lưu vào lịch sử và hiển thị cho user
/// </summary>
public interface ISkinAnalysisService
{
    #region Core Analysis Methods

    /// <summary>
    /// Phân tích da từ stream ảnh
    /// </summary>
    /// <param name="imageStream">Stream chứa dữ liệu ảnh (JPEG, PNG)</param>
    /// <param name="userId">ID người dùng (để lưu lịch sử)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả phân tích da chi tiết</returns>
    Task<SkinAnalysisResult> AnalyzeAsync(
        Stream imageStream, 
        int? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Phân tích da từ đường dẫn file ảnh
    /// </summary>
    /// <param name="imagePath">Đường dẫn đến file ảnh</param>
    /// <param name="userId">ID người dùng</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả phân tích da chi tiết</returns>
    Task<SkinAnalysisResult> AnalyzeFromFileAsync(
        string imagePath, 
        int? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Phân tích da từ Base64 string (phù hợp với Web/Mobile upload)
    /// </summary>
    /// <param name="base64Image">Ảnh dưới dạng Base64</param>
    /// <param name="userId">ID người dùng</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả phân tích da chi tiết</returns>
    Task<SkinAnalysisResult> AnalyzeFromBase64Async(
        string base64Image, 
        int? userId = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Face Detection

    /// <summary>
    /// Kiểm tra ảnh có chứa khuôn mặt hay không
    /// </summary>
    /// <param name="imageStream">Stream ảnh</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>True nếu phát hiện khuôn mặt</returns>
    Task<bool> ContainsFaceAsync(
        Stream imageStream, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Phát hiện và cắt vùng khuôn mặt từ ảnh
    /// </summary>
    /// <param name="imageStream">Stream ảnh gốc</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Stream ảnh khuôn mặt đã cắt, null nếu không phát hiện</returns>
    Task<FaceDetectionResult> DetectAndCropFaceAsync(
        Stream imageStream, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Validation

    /// <summary>
    /// Kiểm tra chất lượng ảnh có đủ tốt để phân tích không
    /// </summary>
    /// <param name="imageStream">Stream ảnh</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả validation với các cảnh báo</returns>
    Task<ImageQualityResult> ValidateImageQualityAsync(
        Stream imageStream, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Guidance

    /// <summary>
    /// Lấy hướng dẫn căn chỉnh khuôn mặt real-time
    /// </summary>
    /// <param name="imageStream">Stream ảnh từ camera</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Hướng dẫn cho người dùng</returns>
    Task<FaceAlignmentGuidance> GetFaceAlignmentGuidanceAsync(
        Stream imageStream, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Comparison

    /// <summary>
    /// So sánh 2 kết quả phân tích và đưa ra xu hướng
    /// </summary>
    /// <param name="currentResult">Kết quả hiện tại</param>
    /// <param name="previousResult">Kết quả trước đó</param>
    /// <returns>Phân tích xu hướng</returns>
    SkinTrendAnalysis CompareSkinAnalysis(
        SkinAnalysisResult currentResult, 
        SkinAnalysisResult previousResult);

    /// <summary>
    /// Phân tích xu hướng từ nhiều kết quả lịch sử
    /// </summary>
    /// <param name="historicalResults">Danh sách kết quả theo thời gian (mới nhất đầu tiên)</param>
    /// <returns>Báo cáo xu hướng tổng hợp</returns>
    SkinTrendReport AnalyzeTrends(IEnumerable<SkinAnalysisResult> historicalResults);

    #endregion
}

#region Supporting DTOs

/// <summary>
/// Kết quả phát hiện khuôn mặt
/// </summary>
public class FaceDetectionResult
{
    /// <summary>Có phát hiện khuôn mặt không</summary>
    public bool FaceDetected { get; set; }
    
    /// <summary>Độ tin cậy (0-100%)</summary>
    public decimal Confidence { get; set; }
    
    /// <summary>Vùng khuôn mặt</summary>
    public FaceRegion? Region { get; set; }
    
    /// <summary>Stream ảnh khuôn mặt đã cắt (null nếu không phát hiện)</summary>
    public Stream? CroppedFaceStream { get; set; }
    
    /// <summary>Kích thước ảnh gốc</summary>
    public ImageSize? OriginalSize { get; set; }
    
    /// <summary>Thông báo lỗi (nếu có)</summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Kích thước ảnh
/// </summary>
public class ImageSize
{
    public int Width { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// Kết quả kiểm tra chất lượng ảnh
/// </summary>
public class ImageQualityResult
{
    /// <summary>Ảnh đủ chất lượng để phân tích</summary>
    public bool IsAcceptable { get; set; }
    
    /// <summary>Điểm chất lượng tổng thể (0-100)</summary>
    public decimal QualityScore { get; set; }
    
    /// <summary>Độ sáng đủ không</summary>
    public bool HasAdequateLighting { get; set; }
    
    /// <summary>Độ nét đủ không</summary>
    public bool HasAdequateSharpness { get; set; }
    
    /// <summary>Kích thước ảnh đủ không</summary>
    public bool HasAdequateResolution { get; set; }
    
    /// <summary>Có khuôn mặt rõ ràng không</summary>
    public bool HasClearFace { get; set; }
    
    /// <summary>Danh sách cảnh báo</summary>
    public List<string> Warnings { get; set; } = new();
    
    /// <summary>Gợi ý cải thiện</summary>
    public List<string> Suggestions { get; set; } = new();
}

/// <summary>
/// Hướng dẫn căn chỉnh khuôn mặt
/// </summary>
public class FaceAlignmentGuidance
{
    /// <summary>Khuôn mặt đã được căn chỉnh đúng chưa</summary>
    public bool IsAligned { get; set; }
    
    /// <summary>Độ tin cậy căn chỉnh (0-100%)</summary>
    public decimal AlignmentScore { get; set; }
    
    /// <summary>Cần di chuyển theo hướng nào</summary>
    public MovementDirection? SuggestedMovement { get; set; }
    
    /// <summary>Cần xoay khuôn mặt không</summary>
    public RotationDirection? SuggestedRotation { get; set; }
    
    /// <summary>Cần di chuyển xa/gần camera</summary>
    public DistanceAdjustment? SuggestedDistance { get; set; }
    
    /// <summary>Hướng dẫn bằng text</summary>
    public string GuidanceText { get; set; } = string.Empty;
    
    /// <summary>Có thể chụp ảnh được chưa</summary>
    public bool CanCapture { get; set; }
}

/// <summary>
/// Hướng di chuyển
/// </summary>
public enum MovementDirection
{
    None,
    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown,
    Center
}

/// <summary>
/// Hướng xoay
/// </summary>
public enum RotationDirection
{
    None,
    TiltLeft,
    TiltRight,
    LookStraight
}

/// <summary>
/// Điều chỉnh khoảng cách
/// </summary>
public enum DistanceAdjustment
{
    None,
    MoveCloser,
    MoveFarther
}

/// <summary>
/// Báo cáo xu hướng da theo thời gian
/// </summary>
public class SkinTrendReport
{
    /// <summary>Thời điểm bắt đầu theo dõi</summary>
    public DateTime TrackingStartDate { get; set; }
    
    /// <summary>Số lần phân tích</summary>
    public int TotalAnalysisCount { get; set; }
    
    /// <summary>Điểm sức khỏe da trung bình</summary>
    public decimal AverageScore { get; set; }
    
    /// <summary>Điểm cao nhất đạt được</summary>
    public decimal HighestScore { get; set; }
    
    /// <summary>Điểm thấp nhất</summary>
    public decimal LowestScore { get; set; }
    
    /// <summary>Xu hướng tổng thể trong khoảng thời gian</summary>
    public TrendDirection OverallTrend { get; set; }
    
    /// <summary>Tốc độ thay đổi trung bình (%/tuần)</summary>
    public decimal AverageChangeRatePerWeek { get; set; }
    
    /// <summary>Các vấn đề da thường gặp nhất</summary>
    public List<SkinConcernFrequency> MostCommonConcerns { get; set; } = new();
    
    /// <summary>Các chỉ số cải thiện nhiều nhất</summary>
    public List<MetricImprovement> TopImprovements { get; set; } = new();
    
    /// <summary>Các chỉ số cần chú ý</summary>
    public List<MetricImprovement> AreasNeedingAttention { get; set; } = new();
    
    /// <summary>Tóm tắt báo cáo</summary>
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>Lời khuyên dựa trên xu hướng</summary>
    public List<SkinAdvice> TrendBasedAdvice { get; set; } = new();
}

/// <summary>
/// Tần suất xuất hiện vấn đề da
/// </summary>
public class SkinConcernFrequency
{
    /// <summary>Loại vấn đề</summary>
    public SkinConcernType ConcernType { get; set; }
    
    /// <summary>Số lần xuất hiện</summary>
    public int OccurrenceCount { get; set; }
    
    /// <summary>Mức độ nghiêm trọng trung bình</summary>
    public decimal AverageSeverity { get; set; }
    
    /// <summary>Đang cải thiện hay xấu đi</summary>
    public TrendDirection Trend { get; set; }
}

/// <summary>
/// Cải thiện chỉ số
/// </summary>
public class MetricImprovement
{
    /// <summary>Tên chỉ số</summary>
    public string MetricName { get; set; } = string.Empty;
    
    /// <summary>Giá trị đầu</summary>
    public decimal StartValue { get; set; }
    
    /// <summary>Giá trị cuối</summary>
    public decimal EndValue { get; set; }
    
    /// <summary>Thay đổi tuyệt đối</summary>
    public decimal Change { get; set; }
    
    /// <summary>Thay đổi phần trăm</summary>
    public decimal ChangePercent { get; set; }
}

#endregion

