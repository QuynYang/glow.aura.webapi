using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Events;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.Events;
using CosmeticStore.Infrastructure.Gateways;

namespace CosmeticStore.Infrastructure.Services;


/// FACADE PATTERN - Đơn giản hóa quy trình Checkout
/// ┌─────────────────────────────────────────────────┐
/// │              CheckoutFacade                      │
/// │  (Client chỉ cần gọi 1 method duy nhất)        │
/// │                                                  │
/// │  ProcessCheckoutAsync() gọi lần lượt:           │
/// │    ├── IGenericRepository  → Lấy User           │
/// │    ├── IProductRepository  → Lấy sản phẩm       │
/// │    ├── IPricingService     → Tính giá            │
/// │    ├── IOrderRepository    → Lưu đơn hàng       │
/// │    ├── PaymentGateway      → Xử lý thanh toán   │
/// │    └── EventDispatcher     → Gửi thông báo      │
/// └─────────────────────────────────────────────────┘

public class CheckoutFacade : ICheckoutFacade
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPricingService _pricingService;
    private readonly IOrderRepository _orderRepository;
    private readonly PaymentGatewayFactory _paymentGatewayFactory;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IAppLogger _logger;


    public CheckoutFacade(
        IGenericRepository<User> userRepository,
        IProductRepository productRepository,
        IPricingService pricingService,
        IOrderRepository orderRepository,
        PaymentGatewayFactory paymentGatewayFactory,
        IDomainEventDispatcher eventDispatcher,
        IAppLogger logger)
    {
        _userRepository = userRepository;
        _productRepository = productRepository;
        _pricingService = pricingService;
        _orderRepository = orderRepository;
        _paymentGatewayFactory = paymentGatewayFactory;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    
    /// Luồng xử lý:
    /// 1. Validate dữ liệu đầu vào
    /// 2. Lấy thông tin User từ DB
    /// 3. Lấy thông tin sản phẩm + kiểm tra tồn kho
    /// 4. Tính giá (Strategy + Decorator Pattern)
    /// 5. Tạo đơn hàng + lưu DB
    /// 6. Xử lý thanh toán (Factory Pattern)
    /// 7. Gửi thông báo (Observer Pattern)
    /// 8. Trả kết quả
    
    public async Task<CheckoutResult> ProcessCheckoutAsync(CheckoutRequest request)
    {
        try
        {
            
            // BƯỚC 1: Validate dữ liệu đầu vào
            
            if (request.Items == null || !request.Items.Any())
            {
                return new CheckoutResult
                {
                    IsSuccess = false,
                    Message = "Giỏ hàng trống, vui lòng thêm sản phẩm"
                };
            }

            
            // BƯỚC 2: Lấy thông tin User
            
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return new CheckoutResult
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy người dùng"
                };
            }

            
            // BƯỚC 3: Lấy sản phẩm + Kiểm tra tồn kho
            
            var itemDetails = new List<CheckoutItemDetail>();
            var orderItems = new List<OrderItem>();
            decimal subTotal = 0;
            decimal totalDiscount = 0;

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return new CheckoutResult
                    {
                        IsSuccess = false,
                        Message = $"Không tìm thấy sản phẩm với Id: {item.ProductId}"
                    };
                }

                if (product.Stock < item.Quantity)
                {
                    return new CheckoutResult
                    {
                        IsSuccess = false,
                        Message = $"Sản phẩm '{product.Name}' không đủ tồn kho. Còn lại: {product.Stock}"
                    };
                }

                
                // BƯỚC 4: Tính giá (sử dụng PricingService)
                // PricingService nội bộ dùng Strategy + Decorator Pattern
                
                var pricingResult = _pricingService.CalculateFinalPrice(product, user, request.CouponCode);

                var detail = new CheckoutItemDetail
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    OriginalPrice = product.Price,
                    FinalPrice = pricingResult.FinalPrice,
                    TotalPrice = pricingResult.FinalPrice * item.Quantity,
                    AppliedDiscounts = pricingResult.AppliedDiscounts
                        .Select(d => d.Description)
                        .ToList()
                };
                itemDetails.Add(detail);

                // Tạo OrderItem
                var orderItem = new OrderItem(
                    product.Id,
                    product.Name,
                    product.Price,
                    pricingResult.FinalPrice,
                    item.Quantity
                );
                orderItems.Add(orderItem);

                subTotal += detail.TotalPrice;
                totalDiscount += (product.Price - pricingResult.FinalPrice) * item.Quantity;

                // Giảm tồn kho
                product.DecreaseStock(item.Quantity);
                _productRepository.Update(product);
            }

            
            // BƯỚC 5: Tạo đơn hàng + Lưu DB
            
            var paymentMethod = ParsePaymentMethod(request.PaymentMethod);
            var order = new Order(
                request.UserId,
                request.ShippingAddress,
                request.ShippingPhone,
                request.ReceiverName,
                paymentMethod,
                request.Notes,
                request.CouponCode
            );

            // Thêm sản phẩm vào đơn hàng
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync(); // Lưu trước để có Order.Id

            foreach (var item in orderItems)
            {
                order.AddItem(item);
            }

            if (totalDiscount > 0)
            {
                order.ApplyDiscount(totalDiscount);
            }

            // Tính phí ship (miễn phí cho đơn > 500k)
            var shippingFee = subTotal >= 500000 ? 0 : 30000;
            order.SetShippingFee(shippingFee);

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            
            // BƯỚC 6: Xử lý thanh toán (Factory Pattern)
            
            string? paymentUrl = null;
            string? transactionId = null;

            var gatewayCode = request.PaymentMethod.ToUpper();
            if (_paymentGatewayFactory.IsSupported(gatewayCode))
            {
                var gateway = _paymentGatewayFactory.CreateGateway(gatewayCode);
                var paymentRequest = new PaymentRequest
                {
                    OrderId = order.Id.ToString(),
                    OrderNumber = order.OrderNumber,
                    Amount = order.TotalAmount,
                    Description = $"Thanh toán đơn hàng {order.OrderNumber}"
                };

                var paymentResponse = await gateway.ProcessPaymentAsync(paymentRequest);
                paymentUrl = paymentResponse.PaymentUrl;
                transactionId = paymentResponse.TransactionId;

                if (paymentResponse.IsSuccess && gatewayCode == "COD")
                {
                    // COD: Đánh dấu chờ giao hàng mới thu tiền
                    transactionId = paymentResponse.TransactionId;
                }
            }

            
            // BƯỚC 7: Gửi thông báo (Observer Pattern)
            
            var orderCreatedEvent = new OrderCreatedEvent(
                orderId: order.Id,
                orderNumber: order.OrderNumber,
                userId: user.Id,
                userEmail: user.Email,
                userPhone: user.PhoneNumber ?? "",
                userName: user.FullName,
                totalAmount: order.TotalAmount,
                itemCount: orderItems.Count,
                shippingAddress: request.ShippingAddress,
                paymentMethod: paymentMethod,
                userVipLevel: user.VipLevel
            );
            await _eventDispatcher.PublishAsync(orderCreatedEvent);

            _logger.LogInfo($"[FACADE] Checkout thành công - Đơn hàng: {order.OrderNumber}, " +
                           $"Tổng tiền: {order.TotalAmount:N0} VND, User: {user.Email}");

            
            // BƯỚC 8: Trả kết quả
            
            return new CheckoutResult
            {
                IsSuccess = true,
                Message = $"Đặt hàng thành công! Mã đơn: {order.OrderNumber}",
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                PaymentUrl = paymentUrl,
                TransactionId = transactionId,
                ItemDetails = itemDetails
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"[FACADE] Checkout thất bại: {ex.Message}");
            return new CheckoutResult
            {
                IsSuccess = false,
                Message = $"Checkout thất bại: {ex.Message}"
            };
        }
    }

    
    /// Xem trước đơn hàng - Chỉ tính giá, KHÔNG tạo đơn thật
    /// Dùng để hiển thị tổng tiền cho khách trước khi xác nhận
    
    public async Task<CheckoutPreview> PreviewCheckoutAsync(CheckoutRequest request)
    {
        try
        {
            if (request.Items == null || !request.Items.Any())
            {
                return new CheckoutPreview
                {
                    IsValid = false,
                    ErrorMessage = "Giỏ hàng trống"
                };
            }

            var user = await _userRepository.GetByIdAsync(request.UserId);
            var itemDetails = new List<CheckoutItemDetail>();
            var warnings = new List<string>();
            decimal subTotal = 0;
            decimal totalDiscount = 0;

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return new CheckoutPreview
                    {
                        IsValid = false,
                        ErrorMessage = $"Không tìm thấy sản phẩm với Id: {item.ProductId}"
                    };
                }

                if (product.Stock < item.Quantity)
                {
                    warnings.Add($"Sản phẩm '{product.Name}' chỉ còn {product.Stock} sản phẩm");
                }

                // Tính giá (không tạo đơn)
                var pricingResult = _pricingService.CalculateFinalPrice(product, user, request.CouponCode);

                var detail = new CheckoutItemDetail
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    OriginalPrice = product.Price,
                    FinalPrice = pricingResult.FinalPrice,
                    TotalPrice = pricingResult.FinalPrice * item.Quantity,
                    AppliedDiscounts = pricingResult.AppliedDiscounts
                        .Select(d => d.Description)
                        .ToList()
                };
                itemDetails.Add(detail);

                subTotal += detail.TotalPrice;
                totalDiscount += (product.Price - pricingResult.FinalPrice) * item.Quantity;

                // Thêm cảnh báo từ PricingService
                warnings.AddRange(pricingResult.Warnings);
            }

            var shippingFee = subTotal >= 500000 ? 0 : 30000m;

            return new CheckoutPreview
            {
                IsValid = true,
                SubTotal = subTotal,
                ShippingFee = shippingFee,
                TotalDiscount = totalDiscount,
                TotalAmount = subTotal + shippingFee,
                ItemDetails = itemDetails,
                Warnings = warnings
            };
        }
        catch (Exception ex)
        {
            return new CheckoutPreview
            {
                IsValid = false,
                ErrorMessage = $"Lỗi khi xem trước: {ex.Message}"
            };
        }
    }

    #region Private Helper Methods

    
    /// Chuyển đổi string → PaymentMethod enum
    
    private static PaymentMethod ParsePaymentMethod(string method)
    {
        return method.ToUpper() switch
        {
            "MOMO" => PaymentMethod.Momo,
            "VNPAY" => PaymentMethod.VNPay,
            "ZALOPAY" => PaymentMethod.ZaloPay,
            "BANK" => PaymentMethod.BankTransfer,
            _ => PaymentMethod.COD // Mặc định là COD
        };
    }

    #endregion
}
