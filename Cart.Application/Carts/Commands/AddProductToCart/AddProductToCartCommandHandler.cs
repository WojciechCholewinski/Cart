using Cart.Application.Abstractions.Persistence;
using Cart.Application.Abstractions.Products;
using MediatR;

namespace Cart.Application.Carts.Commands.AddProductToCart;

public sealed class AddProductToCartCommandHandler(
    ICartRepository carts,
    IProductClient products)
    : IRequestHandler<AddProductToCartCommand, Unit>
{
    public async Task<Unit> Handle(AddProductToCartCommand request, CancellationToken ct)
    {
        if (request.Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.Quantity), "Quantity must be > 0.");

        var cart = await carts.GetByIdAsync(request.CartId, ct);
        if (cart is null)
            throw new KeyNotFoundException($"Cart '{request.CartId}' not found.");

        // Na 3.0 polecam sprawdzić, czy produkt istnieje (lepsze demo, mniej edge-case’ów).
        var product = await products.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            throw new KeyNotFoundException($"Product '{request.ProductId}' not found in ProductService.");

        cart.AddItem(request.ProductId, request.Quantity);

        await carts.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
