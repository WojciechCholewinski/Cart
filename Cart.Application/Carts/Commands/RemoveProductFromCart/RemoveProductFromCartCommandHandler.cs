using Cart.Application.Abstractions.Persistence;
using MediatR;

namespace Cart.Application.Carts.Commands.RemoveProductFromCart;

public sealed class RemoveProductFromCartCommandHandler(ICartRepository carts)
    : IRequestHandler<RemoveProductFromCartCommand>
{
    public async Task Handle(RemoveProductFromCartCommand request, CancellationToken ct)
    {
        if (request.Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.Quantity), "Quantity must be > 0.");

        var cart = await carts.GetByIdAsync(request.CartId, ct);
        if (cart is null)
            throw new KeyNotFoundException($"Cart '{request.CartId}' not found.");

        cart.RemoveItem(request.ProductId, request.Quantity);

        await carts.SaveChangesAsync(ct);
    }

    Task<Unit> IRequestHandler<RemoveProductFromCartCommand, Unit>.Handle(RemoveProductFromCartCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
