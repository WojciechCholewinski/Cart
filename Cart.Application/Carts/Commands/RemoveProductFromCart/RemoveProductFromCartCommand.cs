using MediatR;

namespace Cart.Application.Carts.Commands.RemoveProductFromCart
{
    public sealed record RemoveProductFromCartCommand(
      Guid CartId,
      Guid ProductId,
      int Quantity) : IRequest<Unit>;
}
