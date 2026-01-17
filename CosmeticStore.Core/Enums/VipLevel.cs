namespace CosmeticStore.Core.Enums;

/// <summary>
/// Enum cấp độ VIP của khách hàng - Dùng cho Strategy Pattern tính giá
/// Mỗi cấp độ có mức giảm giá khác nhau
/// </summary>
public enum VipLevel
{
    /// <summary>
    /// Khách hàng thường - Không giảm giá
    /// </summary>
    None = 0,

    /// <summary>
    /// VIP Bronze - Giảm 5%
    /// Điều kiện: Tổng chi tiêu >= 1,000,000 VND
    /// </summary>
    Bronze = 1,

    /// <summary>
    /// VIP Silver - Giảm 10%
    /// Điều kiện: Tổng chi tiêu >= 5,000,000 VND
    /// </summary>
    Silver = 2,

    /// <summary>
    /// VIP Gold - Giảm 15%
    /// Điều kiện: Tổng chi tiêu >= 10,000,000 VND
    /// </summary>
    Gold = 3,

    /// <summary>
    /// VIP Platinum - Giảm 20%
    /// Điều kiện: Tổng chi tiêu >= 20,000,000 VND
    /// </summary>
    Platinum = 4
}

