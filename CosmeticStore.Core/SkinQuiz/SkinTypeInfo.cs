using System.Collections.Generic;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.SkinQuiz;

public static class SkinTypeInfo
{
    public static readonly Dictionary<SkinType, SkinTypeDetails> Details = new()
    {
        {
            SkinType.Oily, new SkinTypeDetails
            {
                Name = "Da Dầu",
                Description = "Da dầu tiết nhiều bã nhờn, lỗ chân lông to và dễ bị mụn.",
                Characteristics = new List<string> { "Bóng nhờn vùng T", "Lỗ chân lông to", "Dễ nổi mụn" },
                RecommendedIngredients = new List<string> { "BHA", "Niacinamide", "Đất sét" },
                IngredientsToAvoid = new List<string> { "Dầu khoáng", "Dầu dừa" },
                SkincareTips = new List<string> { "Rửa mặt 2 lần/ngày", "Dùng dưỡng ẩm dạng gel" }
            }
        },
        {
            SkinType.Dry, new SkinTypeDetails
            {
                Name = "Da Khô",
                Description = "Da thiếu ẩm, cảm giác căng và dễ bong tróc.",
                Characteristics = new List<string> { "Căng sau rửa mặt", "Bề mặt thô ráp" },
                RecommendedIngredients = new List<string> { "HA", "Ceramides", "Glycerin" },
                IngredientsToAvoid = new List<string> { "Cồn khô", "Xà phòng mạnh" },
                SkincareTips = new List<string> { "Dùng kem dưỡng đặc", "Tránh nước nóng" }
            }
        },
        {
            SkinType.Sensitive, new SkinTypeDetails
            {
                Name = "Da Nhạy Cảm",
                Description = "Da mỏng, dễ đỏ và kích ứng với môi trường hoặc mỹ phẩm.",
                Characteristics = new List<string> { "Dễ mẩn đỏ", "Mỏng, lộ mạch máu" },
                RecommendedIngredients = new List<string> { "Rau má", "B5", "Hoa cúc" },
                IngredientsToAvoid = new List<string> { "Hương liệu", "Paraben", "Cồn" },
                SkincareTips = new List<string> { "Thử sản phẩm trước khi dùng", "Tối giản routine" }
            }
        },
        {
            SkinType.Normal, new SkinTypeDetails
            {
                Name = "Da Thường",
                Description = "Da cân bằng hoàn hảo giữa dầu và nước, mịn màng.",
                Characteristics = new List<string> { "Lỗ chân lông nhỏ", "Da đều màu" },
                RecommendedIngredients = new List<string> { "Vitamin C", "HA", "Peptides" },
                IngredientsToAvoid = new List<string> { "Hạn chế tẩy rửa quá mạnh" },
                SkincareTips = new List<string> { "Duy trì làm sạch và chống nắng" }
            }
        },
        {
            SkinType.Combination, new SkinTypeDetails
            {
                Name = "Da Hỗn Hợp",
                Description = "Vùng T (trán, mũi, cằm) dầu nhưng hai bên má lại khô.",
                Characteristics = new List<string> { "Dầu vùng T", "Má khô hoặc thường" },
                RecommendedIngredients = new List<string> { "Niacinamide", "HA" },
                IngredientsToAvoid = new List<string> { "Dùng kem quá đặc cho vùng T" },
                SkincareTips = new List<string> { "Chăm sóc riêng biệt từng vùng da" }
            }
        }
    };
}

public class SkinTypeDetails
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Characteristics { get; set; } = new();
    public List<string> RecommendedIngredients { get; set; } = new();
    public List<string> IngredientsToAvoid { get; set; } = new();
    public List<string> SkincareTips { get; set; } = new();
}