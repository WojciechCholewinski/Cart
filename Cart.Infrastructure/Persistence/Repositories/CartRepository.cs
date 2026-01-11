using Cart.Application.Abstractions.Persistence;
using Cart.Domain.Carts;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Persistence.Repositories
{
    public sealed class CartRepository(CartDbContext db) : ICartRepository
    {
        public async Task<DomainCart?> GetByIdAsync(Guid cartId, CancellationToken ct)
        {
            return await db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId, ct);
        }

        public async Task AddAsync(DomainCart cart, CancellationToken ct)
        {
            await db.Carts.AddAsync(cart, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct)
            => db.SaveChangesAsync(ct);
    }
}
