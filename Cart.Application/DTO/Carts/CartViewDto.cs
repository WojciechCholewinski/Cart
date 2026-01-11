namespace Cart.Application.DTO.Carts
{
    public sealed record CartViewDto(
      Guid CartId,
      string UserId,
      IReadOnlyList<CartItemViewDto> Items,
      decimal Total);
}
