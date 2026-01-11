namespace Cart.Domain.Orders
{
    public sealed class Order
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid CartId { get; private set; }
        public string UserId { get; private set; } = default!;
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items;

        private Order() { } // EF

        public Order(Guid cartId, string userId, IEnumerable<OrderItem> items)
        {
            if (cartId == Guid.Empty) throw new ArgumentException("CartId is required.", nameof(cartId));
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("UserId is required.", nameof(userId));

            CartId = cartId;
            UserId = userId;

            _items.AddRange(items ?? throw new ArgumentNullException(nameof(items)));
            if (_items.Count == 0) throw new ArgumentException("Order must have at least one item.", nameof(items));
        }
    }
}
