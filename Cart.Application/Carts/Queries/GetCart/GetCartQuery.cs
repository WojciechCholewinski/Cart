using Cart.Application.DTO.Carts;
using MediatR;

namespace Cart.Application.Carts.Queries.GetCart
{
    public sealed record GetCartQuery(Guid CartId) : IRequest<CartViewDto>;
}
