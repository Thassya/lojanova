using LojaNova.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace LojaNova.Ecommerce.Api.Data
{
    public class LojaNovaDbContext : DbContext
    {
        public LojaNovaDbContext(DbContextOptions<LojaNovaDbContext> options)
        : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações para OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany() // No caso de não ter uma propriedade de navegação em Product para OrderItem
                .HasForeignKey(oi => oi.ProductId);

            // Exemplo de seed de dados (opcional)
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "Alice Smith", Email = "alice@example.com", Address = "123 Main St", Phone = "555-1234" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop X", Description = "Powerful laptop for professionals", Price = 1200.00m, StockQuantity = 50, ImageUrl = "https://yourazurestorage.file.core.windows.net/product-images/laptop-x.jpg" },
                new Product { Id = 2, Name = "Smartphone Y", Description = "Latest smartphone model", Price = 800.00m, StockQuantity = 100, ImageUrl = "https://yourazurestorage.file.core.windows.net/product-images/smartphone-y.jpg" }
            );
        }
    }
}
