using MediatR;

namespace Cart.Application.Carts.Commands.CheckoutCart
{
    public sealed record CheckoutCartCommand(
     Guid CartId,
     string UserId) : IRequest<Guid>;
}
