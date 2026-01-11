using Cart.Application.Abstractions.Persistence;
using Cart.Application.Abstractions.Products;
using Cart.Infrastructure.Persistence;
using Cart.Infrastructure.Persistence.Repositories;
using Cart.Infrastructure.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// ---------- SQL Server ----------
builder.Services.AddDbContext<CartDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("CartDb")));

// ---------- Repositories ----------
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// ---------- ProductService ----------
builder.Services.Configure<ProductServiceOptions>(
    builder.Configuration.GetSection("ProductService"));

builder.Services.AddHttpClient<IProductClient, ProductClient>((sp, http) =>
{
    var opt = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<ProductServiceOptions>>().Value;

    http.BaseAddress = new Uri(opt.BaseUrl);
});

// ---------- MediatR ----------
builder.Services.AddMediatR(typeof(Cart.Application.Carts.Commands.CreateCart.CreateCartCommand).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
