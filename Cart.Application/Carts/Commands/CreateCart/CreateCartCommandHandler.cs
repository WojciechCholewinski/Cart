using Cart.Application.Abstractions.Persistence;
using Cart.Domain.Carts;
using MediatR;

namespace Cart.Application.Carts.Commands.CreateCart
{
    public sealed class CreateCartCommandHandler(ICartRepository carts)
     : IRequestHandler<CreateCartCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCartCommand request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException("UserId is required.", nameof(request.UserId));

            var cart = new DomainCart(request.UserId);
            await carts.AddAsync(cart, ct);
            await carts.SaveChangesAsync(ct);

            return cart.Id;
        }
    }
}
