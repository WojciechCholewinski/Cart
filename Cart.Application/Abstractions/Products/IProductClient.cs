using Cart.Application.Dto.Products;

namespace Cart.Application.Abstractions.Products
{
    public interface IProductClient
    {
        Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
