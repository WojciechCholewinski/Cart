namespace Cart.Domain.Orders
{
    public sealed class OrderItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid OrderId { get; private set; } // EF
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        private OrderItem() { } // EF

        public OrderItem(Guid productId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            ProductId = productId;
            Quantity = quantity;
        }
    }
}
