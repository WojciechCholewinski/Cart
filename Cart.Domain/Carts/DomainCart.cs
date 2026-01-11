namespace Cart.Domain.Carts
{
    public sealed class DomainCart
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string UserId { get; private set; } = default!;
        public CartStatus Status { get; private set; } = CartStatus.Active;

        public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

        private readonly List<CartItem> _items = new();
        public IReadOnlyCollection<CartItem> Items => _items;
        private DomainCart() { } // EF Core

        public DomainCart(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.", nameof(userId));

            UserId = userId;
            Touch();
        }

        public void AddItem(Guid productId, int quantity)
        {
            EnsureActive();
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            var existing = _items.SingleOrDefault(x => x.ProductId == productId);
            if (existing is null)
            {
                _items.Add(new CartItem(productId, quantity));
            }
            else
            {
                existing.Increase(quantity);
            }

            Touch();
        }

        /// <summary>
        /// Zmniejsza ilość o quantity. Jeśli Quantity spadnie do 0 lub mniej – usuwa pozycję.
        /// </summary>
        public void RemoveItem(Guid productId, int quantity)
        {
            EnsureActive();
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            var existing = _items.SingleOrDefault(x => x.ProductId == productId);
            if (existing is null)
                return; // na 3.0 może być "idempotentnie"

            existing.Decrease(quantity);

            if (existing.Quantity <= 0)
                _items.Remove(existing);

            Touch();
        }

        public void Checkout()
        {
            EnsureActive();

            if (_items.Count == 0)
                throw new InvalidOperationException("Cannot checkout an empty cart.");

            Status = CartStatus.CheckedOut;
            Touch();
        }

        private void EnsureActive()
        {
            if (Status != CartStatus.Active)
                throw new InvalidOperationException("Cart is not active.");
        }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}

