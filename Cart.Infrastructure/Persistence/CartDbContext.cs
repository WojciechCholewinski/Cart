using Cart.Domain.Carts;
using Cart.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Persistence;

public sealed class CartDbContext : DbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options)
        : base(options) { }

    public DbSet<DomainCart> Carts => Set<DomainCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------- Cart ----------
        modelBuilder.Entity<DomainCart>(b =>
        {
            b.ToTable("Carts");
            b.HasKey(x => x.Id);

            // niech EF nie próbuje generować Id po stronie DB
            b.Property(x => x.Id).ValueGeneratedNever();

            b.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.Status)
                .IsRequired();

            b.Property(x => x.UpdatedAtUtc)
                .IsRequired();

            // !!! najważniejsze: backing field dla read-only kolekcji !!!
            b.Metadata.FindNavigation(nameof(DomainCart.Items))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- CartItem ----------
        modelBuilder.Entity<CartItem>(b =>
        {
            b.ToTable("CartItems");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).ValueGeneratedNever();

            b.Property(x => x.CartId).IsRequired();

            b.Property(x => x.ProductId)
                .IsRequired();

            b.Property(x => x.Quantity)
                .IsRequired();
        });

        // ---------- Order ----------
        modelBuilder.Entity<Order>(b =>
        {
            b.ToTable("Orders");
            b.HasKey(x => x.Id);

            b.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.CreatedAtUtc)
                .IsRequired();

            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- OrderItem ----------
        modelBuilder.Entity<OrderItem>(b =>
        {
            b.ToTable("OrderItems");
            b.HasKey(x => x.Id);

            b.Property(x => x.ProductId)
                .IsRequired();

            b.Property(x => x.Quantity)
                .IsRequired();
        });
    }
}
