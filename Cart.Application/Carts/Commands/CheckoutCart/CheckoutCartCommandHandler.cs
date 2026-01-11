using Cart.Application.Abstractions.Persistence;
using Cart.Domain.Orders;
using MediatR;

namespace Cart.Application.Carts.Commands.CheckoutCart;

public sealed class CheckoutCartCommandHandler(
    ICartRepository carts,
    IOrderRepository orders)
    : IRequestHandler<CheckoutCartCommand, Guid>
{
    public async Task<Guid> Handle(CheckoutCartCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("UserId is required.", nameof(request.UserId));

        var cart = await carts.GetByIdAsync(request.CartId, ct);
        if (cart is null)
            throw new KeyNotFoundException($"Cart '{request.CartId}' not found.");

        // Proste zabezpieczenie spójności: checkout robi właściciel koszyka.
        if (!string.Equals(cart.UserId, request.UserId, StringComparison.Ordinal))
            throw new InvalidOperationException("UserId does not match cart owner.");

        cart.Checkout();

        var orderItems = cart.Items.Select(i => new OrderItem(i.ProductId, i.Quantity)).ToList();
        var order = new Order(cart.Id, cart.UserId, orderItems);

        await orders.AddAsync(order, ct);

        // Ważne: w Infrastructure zrobimy tak, że oba repo używają tego samego DbContext.
        // Wtedy jedno SaveChanges zapisze i Cart (status) i Order.
        await carts.SaveChangesAsync(ct);

        return order.Id;
    }
}
