using Cart.Application.Abstractions.Products;
using Cart.Application.Dto.Products;
using System.Net.Http.Json;

namespace Cart.Infrastructure.Products
{
    public sealed class ProductClient(HttpClient http) : IProductClient
    {
        public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            // ProductsController: /api/Products/{id}
            using var response = await http.GetAsync($"/api/Products/{id}", ct);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            // zakładam, że ProductDto w ProductService ma więcej pól,
            // więc tu możesz zrobić osobny record pasujący do response i zmapować.
            var product = await response.Content.ReadFromJsonAsync<ProductServiceProductDto>(cancellationToken: ct);
            if (product is null) return null;

            return new ProductDto(product.Id, product.Name, product.Price);
        }

        private sealed record ProductServiceProductDto(Guid Id, string Name, decimal Price);
    }
}
