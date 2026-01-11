using Cart.Application.Abstractions.Persistence;
using Cart.Domain.Orders;

namespace Cart.Infrastructure.Persistence.Repositories
{
    public sealed class OrderRepository(CartDbContext db) : IOrderRepository
    {
        public async Task AddAsync(Order order, CancellationToken ct)
        {
            await db.Orders.AddAsync(order, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct)
            => db.SaveChangesAsync(ct);
    }
}
