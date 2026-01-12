namespace Cart.Domain.Carts
{
    public sealed class CartItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid CartId { get; private set; } // FK dla EF Core
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        private CartItem() { } // EF Core
        public CartItem(Guid productId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            ProductId = productId;
            Quantity = quantity;
        }

        // DomainCart ustawi to przy dodaniu
        internal void AttachToCart(Guid cartId)
        {
            if (cartId == Guid.Empty) throw new ArgumentException("CartId is required.", nameof(cartId));
            CartId = cartId;
        }

        public void Increase(int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            Quantity += quantity;
        }

        public void Decrease(int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            Quantity -= quantity;
        }
    }
}
