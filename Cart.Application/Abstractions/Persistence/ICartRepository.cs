using Cart.Domain.Carts;

namespace Cart.Application.Abstractions.Persistence
{
    public interface ICartRepository
    {
        Task<DomainCart?> GetByIdAsync(Guid cartId, CancellationToken ct);
        Task AddAsync(DomainCart cart, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
