using LojaNova.Ecommerce.Api.Data;
using LojaNova.Ecommerce.Api.Repositories;
using LojaNova.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LojaNova.Ecommerce.Api.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        private LojaNovaDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LojaNovaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new LojaNovaDbContext(options);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsOrders()
        {
            using var context = GetInMemoryContext();
            await ConfigureContext(context);

            var repo = new OrderRepository(context);
            var result = await repo.GetAllOrdersAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOrder()
        {
            using var context = GetInMemoryContext();
            await ConfigureContext(context);
            int orderId = 1;

            var repo = new OrderRepository(context);

            var result = await repo.GetOrderByIdAsync(orderId);

            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
        }

        [Fact]
        public async Task AddOrderAsync_AddsAndSaves()
        {
            using var context = GetInMemoryContext();
            await ConfigureContext(context);

            var repo = new OrderRepository(context);

            var order = new Order { Id = 2, ClientId = 1 };
            var orderItem = new OrderItem
            {
                Id = 2,
                ProductId = 1,
                Quantity = 1
            };
            await repo.AddOrderAsync(order);

            Assert.Equal(2, context.Orders.Count());
            Assert.Equal(2, context.Orders.Last().Id);
        }

        private static async Task ConfigureContext(LojaNovaDbContext context)
        {
            var client = new Client { Id = 1, Name = "Test Client" };
            context.Clients.Add(client);
            context.SaveChanges();

            var product = new Product { Id = 1, Name = "Test Product", Price = 10.0m, StockQuantity = 100 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var orderItem = new OrderItem
            {
                Id = 1,
                ProductId = product.Id,
                Quantity = 2
            };

            var order = new Order { Id = 1, ClientId = client.Id };
            order.OrderItems.Add(orderItem);
            context.Orders.Add(order);
            await context.SaveChangesAsync();
        }
    }
}
