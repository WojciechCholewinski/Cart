using MediatR;

namespace Cart.Application.Carts.Commands.CreateCart
{
    public sealed record CreateCartCommand(string UserId) : IRequest<Guid>;

}
