namespace CosmeticStore.Core.Enums;


/// Enum cấp độ VIP của khách hàng - Dùng cho Strategy Pattern tính giá
/// Mỗi cấp độ có mức giảm giá khác nhau

public enum VipLevel
{
    
    /// Khách hàng thường - Không giảm giá
    
    None = 0,

    
    /// VIP Bronze - Giảm 5%
    /// Điều kiện: Tổng chi tiêu >= 1,000,000 VND
    
    Bronze = 1,

    
    /// VIP Silver - Giảm 10%
    /// Điều kiện: Tổng chi tiêu >= 5,000,000 VND
    
    Silver = 2,

    
    /// VIP Gold - Giảm 15%
    /// Điều kiện: Tổng chi tiêu >= 10,000,000 VND
    
    Gold = 3,

    
    /// VIP Platinum - Giảm 20%
    /// Điều kiện: Tổng chi tiêu >= 20,000,000 VND
    
    Platinum = 4
}

