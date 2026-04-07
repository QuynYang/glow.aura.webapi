using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Handlers.Validations.Orders;

public class UserValidator : OrderValidationHandler
{
    private readonly IGenericRepository<User> _userRepository;

    public UserValidator(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    protected override async Task<OrderValidationResult> ValidateCurrentAsync(OrderValidationContext context, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(context.Command.UserId);
        if (user == null)
        {
            return OrderValidationResult.Failure("Không tìm thấy thông tin khách hàng", "USER_NOT_FOUND");
        }

        context.User = user;
        return OrderValidationResult.Success();
    }
}
