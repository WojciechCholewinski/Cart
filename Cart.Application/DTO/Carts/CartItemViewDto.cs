namespace Cart.Application.DTO.Carts
{
    public sealed record CartItemViewDto(
    Guid ProductId,
    string Name,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);
}
