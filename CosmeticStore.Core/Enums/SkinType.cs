namespace CosmeticStore.Core.Enums;

/// <summary>
/// Enum phân loại loại da - Dùng để lọc sản phẩm phù hợp
/// Áp dụng trong AI Skin Quiz và Product Filtering
/// </summary>
public enum SkinType
{
    /// <summary>
    /// Tất cả loại da (sản phẩm phổ thông)
    /// </summary>
    All = 0,

    /// <summary>
    /// Da dầu - Oily Skin
    /// Đặc điểm: Bóng nhờn, lỗ chân lông to, dễ nổi mụn
    /// </summary>
    Oily = 1,

    /// <summary>
    /// Da khô - Dry Skin
    /// Đặc điểm: Da căng, bong tróc, thiếu độ ẩm
    /// </summary>
    Dry = 2,

    /// <summary>
    /// Da hỗn hợp - Combination Skin
    /// Đặc điểm: Vùng T dầu, vùng má khô
    /// </summary>
    Combination = 3,

    /// <summary>
    /// Da nhạy cảm - Sensitive Skin
    /// Đặc điểm: Dễ kích ứng, đỏ, ngứa
    /// </summary>
    Sensitive = 4,

    /// <summary>
    /// Da thường - Normal Skin
    /// Đặc điểm: Cân bằng, ít vấn đề
    /// </summary>
    Normal = 5
}

