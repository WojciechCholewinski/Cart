using Cart.Application.Abstractions.Persistence;
using Cart.Application.Abstractions.Products;
using Cart.Application.DTO.Carts;
using MediatR;

namespace Cart.Application.Carts.Queries.GetCart;

public sealed class GetCartQueryHandler(
    ICartRepository carts,
    IProductClient products)
    : IRequestHandler<GetCartQuery, CartViewDto>
{
    public async Task<CartViewDto> Handle(GetCartQuery request, CancellationToken ct)
    {
        var cart = await carts.GetByIdAsync(request.CartId, ct);
        if (cart is null)
            throw new KeyNotFoundException($"Cart '{request.CartId}' not found.");

        var items = new List<CartItemViewDto>(cart.Items.Count);

        foreach (var item in cart.Items)
        {
            var product = await products.GetByIdAsync(item.ProductId, ct);
            if (product is null)
            {
                // Na 3.0 najczyściej: fail fast, bo koszyk ma referencję do nieistniejącego produktu.
                throw new InvalidOperationException($"Product '{item.ProductId}' no longer exists in ProductService.");
            }

            var lineTotal = product.Price * item.Quantity;

            items.Add(new CartItemViewDto(
                ProductId: item.ProductId,
                Name: product.Name,
                Quantity: item.Quantity,
                UnitPrice: product.Price,
                LineTotal: lineTotal));
        }

        var total = items.Sum(x => x.LineTotal);

        return new CartViewDto(
            CartId: cart.Id,
            UserId: cart.UserId,
            Items: items,
            Total: total);
    }
}
