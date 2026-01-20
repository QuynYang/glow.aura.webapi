using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.SkinQuiz;

/// <summary>
/// Câu hỏi trắc nghiệm loại da
/// </summary>
public class SkinQuizQuestion
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Oiliness, Sensitivity, Hydration, Aging
    public List<SkinQuizOption> Options { get; set; } = new();
}

/// <summary>
/// Lựa chọn trả lời
/// </summary>
public class SkinQuizOption
{
    public string OptionId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// Điểm số cho từng loại da (dùng để tính toán)
    /// </summary>
    public Dictionary<SkinType, int> SkinTypeScores { get; set; } = new();
}

/// <summary>
/// Request trả lời quiz
/// </summary>
public class SkinQuizAnswerRequest
{
    /// <summary>
    /// User ID (nếu đã đăng nhập)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Danh sách câu trả lời
    /// </summary>
    public List<QuizAnswer> Answers { get; set; } = new();
}

/// <summary>
/// Một câu trả lời
/// </summary>
public class QuizAnswer
{
    public int QuestionId { get; set; }
    public string SelectedOptionId { get; set; } = string.Empty;
}

/// <summary>
/// Kết quả phân tích loại da
/// </summary>
public class SkinQuizResult
{
    /// <summary>
    /// Loại da được xác định
    /// </summary>
    public SkinType DeterminedSkinType { get; set; }

    /// <summary>
    /// Tên loại da (tiếng Việt)
    /// </summary>
    public string SkinTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả chi tiết loại da
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Đặc điểm của loại da
    /// </summary>
    public List<string> Characteristics { get; set; } = new();

    /// <summary>
    /// Thành phần nên tìm kiếm
    /// </summary>
    public List<string> RecommendedIngredients { get; set; } = new();

    /// <summary>
    /// Thành phần nên tránh
    /// </summary>
    public List<string> IngredientsToAvoid { get; set; } = new();

    /// <summary>
    /// Lời khuyên chăm sóc da
    /// </summary>
    public List<string> SkincareTips { get; set; } = new();

    /// <summary>
    /// Điểm số chi tiết (cho debug/analysis)
    /// </summary>
    public Dictionary<SkinType, int> DetailedScores { get; set; } = new();

    /// <summary>
    /// Độ tin cậy của kết quả (%)
    /// </summary>
    public int ConfidencePercent { get; set; }

    /// <summary>
    /// Số sản phẩm phù hợp trong cửa hàng
    /// </summary>
    public int MatchingProductCount { get; set; }

    /// <summary>
    /// Có được hưởng ưu đãi riêng cho loại da không
    /// </summary>
    public bool HasSkinTypeDiscount { get; set; }

    /// <summary>
    /// Phần trăm giảm giá cho loại da
    /// </summary>
    public decimal SkinTypeDiscountPercent { get; set; }
}

/// <summary>
/// Thông tin chi tiết về loại da
/// </summary>
public static class SkinTypeInfo
{
    public static readonly Dictionary<SkinType, SkinTypeDetails> Details = new()
    {
        {
            SkinType.Oily, new SkinTypeDetails
            {
                Name = "Da Dầu",
                Description = "Da dầu tiết nhiều bã nhờn, đặc biệt ở vùng chữ T. Da thường bóng nhờn và dễ bị mụn.",
                Characteristics = new List<string>
                {
                    "Tiết nhiều dầu, đặc biệt vùng chữ T",
                    "Lỗ chân lông to, dễ thấy",
                    "Dễ bị mụn đầu đen, mụn ẩn",
                    "Makeup dễ bị trôi",
                    "Da bóng sau vài giờ rửa mặt"
                },
                RecommendedIngredients = new List<string>
                {
                    "Salicylic Acid (BHA)",
                    "Niacinamide",
                    "Clay (Đất sét)",
                    "Tea Tree Oil",
                    "Hyaluronic Acid (không dầu)"
                },
                IngredientsToAvoid = new List<string>
                {
                    "Dầu khoáng (Mineral Oil)",
                    "Coconut Oil",
                    "Silicone nặng",
                    "Lanolin"
                },
                SkincareTips = new List<string>
                {
                    "Rửa mặt 2 lần/ngày với sữa rửa mặt dạng gel",
                    "Sử dụng toner không cồn",
                    "Dùng kem dưỡng dạng gel, oil-free",
                    "Đắp mặt nạ đất sét 1-2 lần/tuần",
                    "Luôn dùng kem chống nắng không dầu"
                }
            }
        },
        {
            SkinType.Dry, new SkinTypeDetails
            {
                Name = "Da Khô",
                Description = "Da khô thiếu độ ẩm và dầu tự nhiên, thường bị căng, bong tróc và có thể xuất hiện nếp nhăn sớm.",
                Characteristics = new List<string>
                {
                    "Da căng, khô sau khi rửa mặt",
                    "Dễ bong tróc, có vảy",
                    "Lỗ chân lông nhỏ, khó thấy",
                    "Da xỉn màu, thiếu sức sống",
                    "Dễ xuất hiện nếp nhăn"
                },
                RecommendedIngredients = new List<string>
                {
                    "Hyaluronic Acid",
                    "Ceramides",
                    "Squalane",
                    "Shea Butter",
                    "Glycerin",
                    "Vitamin E"
                },
                IngredientsToAvoid = new List<string>
                {
                    "Cồn (Alcohol Denat.)",
                    "Sodium Lauryl Sulfate",
                    "Retinol nồng độ cao (ban đầu)",
                    "AHA/BHA nồng độ cao"
                },
                SkincareTips = new List<string>
                {
                    "Rửa mặt với sữa rửa mặt dạng cream, ẩm",
                    "Dùng toner dưỡng ẩm",
                    "Layer nhiều lớp serum + kem dưỡng",
                    "Đắp mặt nạ dưỡng ẩm 2-3 lần/tuần",
                    "Uống đủ nước mỗi ngày"
                }
            }
        },
        {
            SkinType.Sensitive, new SkinTypeDetails
            {
                Name = "Da Nhạy Cảm",
                Description = "Da nhạy cảm dễ bị kích ứng, đỏ, ngứa khi tiếp xúc với một số thành phần hoặc môi trường.",
                Characteristics = new List<string>
                {
                    "Dễ bị đỏ, ngứa, châm chích",
                    "Phản ứng với nhiều sản phẩm",
                    "Da mỏng, dễ thấy mạch máu",
                    "Dễ bị kích ứng bởi thời tiết",
                    "Có thể có triệu chứng như eczema, rosacea"
                },
                RecommendedIngredients = new List<string>
                {
                    "Centella Asiatica (Rau má)",
                    "Aloe Vera",
                    "Chamomile (Hoa cúc)",
                    "Allantoin",
                    "Panthenol (Vitamin B5)",
                    "Oat Extract"
                },
                IngredientsToAvoid = new List<string>
                {
                    "Hương liệu (Fragrance)",
                    "Cồn",
                    "Essential Oils mạnh",
                    "AHA/BHA nồng độ cao",
                    "Retinol",
                    "Màu nhân tạo"
                },
                SkincareTips = new List<string>
                {
                    "Patch test trước khi dùng sản phẩm mới",
                    "Chọn sản phẩm hypoallergenic, fragrance-free",
                    "Tránh nước quá nóng hoặc quá lạnh",
                    "Dùng kem chống nắng vật lý (mineral)",
                    "Đơn giản hóa routine, ít bước"
                }
            }
        },
        {
            SkinType.Normal, new SkinTypeDetails
            {
                Name = "Da Thường",
                Description = "Da thường là loại da cân bằng, không quá dầu cũng không quá khô, ít gặp vấn đề về da.",
                Characteristics = new List<string>
                {
                    "Da cân bằng, không quá dầu/khô",
                    "Lỗ chân lông vừa phải",
                    "Ít mụn, ít vấn đề về da",
                    "Da căng mịn, khỏe mạnh",
                    "Makeup bám tốt cả ngày"
                },
                RecommendedIngredients = new List<string>
                {
                    "Vitamin C",
                    "Niacinamide",
                    "Hyaluronic Acid",
                    "Peptides",
                    "Antioxidants"
                },
                IngredientsToAvoid = new List<string>
                {
                    "Không có thành phần cần tránh đặc biệt",
                    "Tránh sản phẩm quá mạnh không cần thiết"
                },
                SkincareTips = new List<string>
                {
                    "Duy trì routine cơ bản: cleanse, tone, moisturize, SPF",
                    "Có thể thử nghiệm các active ingredients",
                    "Tập trung vào phòng ngừa lão hóa",
                    "Dùng serum Vitamin C buổi sáng",
                    "Dùng retinol nhẹ buổi tối (nếu muốn)"
                }
            }
        },
        {
            SkinType.Combination, new SkinTypeDetails
            {
                Name = "Da Hỗn Hợp",
                Description = "Da hỗn hợp có vùng chữ T (trán, mũi, cằm) dầu nhưng hai bên má khô hoặc thường.",
                Characteristics = new List<string>
                {
                    "Vùng chữ T dầu, bóng",
                    "Hai bên má khô hoặc thường",
                    "Lỗ chân lông to ở vùng T, nhỏ ở má",
                    "Mụn thường xuất hiện ở vùng T",
                    "Cần chăm sóc khác nhau cho từng vùng"
                },
                RecommendedIngredients = new List<string>
                {
                    "Niacinamide (cân bằng)",
                    "Hyaluronic Acid",
                    "Green Tea Extract",
                    "Salicylic Acid (chỉ vùng T)",
                    "Lightweight moisturizers"
                },
                IngredientsToAvoid = new List<string>
                {
                    "Sản phẩm quá dầu cho toàn mặt",
                    "Sản phẩm quá khô cho toàn mặt"
                },
                SkincareTips = new List<string>
                {
                    "Multi-masking: mặt nạ đất sét vùng T, mặt nạ dưỡng ẩm vùng má",
                    "Dùng kem dưỡng nhẹ, không dầu",
                    "Có thể dùng 2 loại kem dưỡng cho 2 vùng",
                    "Blot vùng T trong ngày nếu cần",
                    "Tẩy tế bào chết đều đặn nhưng nhẹ nhàng"
                }
            }
        }
    };
}

/// <summary>
/// Chi tiết về loại da
/// </summary>
public class SkinTypeDetails
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Characteristics { get; set; } = new();
    public List<string> RecommendedIngredients { get; set; } = new();
    public List<string> IngredientsToAvoid { get; set; } = new();
    public List<string> SkincareTips { get; set; } = new();
}

