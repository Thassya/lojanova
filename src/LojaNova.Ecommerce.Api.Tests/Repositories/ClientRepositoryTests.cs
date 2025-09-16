using LojaNova.Ecommerce.Api.Data;
using LojaNova.Ecommerce.Api.Repositories;
using LojaNova.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LojaNova.Ecommerce.Api.Tests.Repositories
{
    public class ClientRepositoryTests
    {
        private LojaNovaDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LojaNovaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new LojaNovaDbContext(options);
        }

        [Fact]
        public async Task GetAllClientsAsync_ReturnsClients()
        {
            using var context = GetInMemoryContext();
            context.Clients.Add(new Client { Id = 1, Name = "Test" });
            context.SaveChanges();

            var repo = new ClientRepository(context);
            var result = await repo.GetAllClientsAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetClientByIdAsync_ReturnsClient()
        {
            using var context = GetInMemoryContext();
            var client = new Client { Id = 1, Name = "Test" };
            context.Clients.Add(client);
            context.SaveChanges();

            var repo = new ClientRepository(context);
            var result = await repo.GetClientByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(client.Id, result.Id);
        }

        [Fact]
        public async Task AddClientAsync_AddsAndSaves()
        {
            using var context = GetInMemoryContext();
            var repo = new ClientRepository(context);

            var client = new Client { Id = 2, Name = "Test2" };
            await repo.AddClientAsync(client);

            Assert.Single(context.Clients);
            Assert.Equal(2, context.Clients.First().Id);
        }

        [Fact]
        public async Task UpdateClientAsync_UpdatesAndSaves()
        {
            using var context = GetInMemoryContext();
            var client = new Client { Id = 1, Name = "Old" };
            context.Clients.Add(client);
            context.SaveChanges();

            var repo = new ClientRepository(context);
            client.Name = "Updated";
            await repo.UpdateClientAsync(client);

            Assert.Equal("Updated", context.Clients.First().Name);
        }

        [Fact]
        public async Task DeleteClientAsync_RemovesAndSaves_WhenFound()
        {
            using var context = GetInMemoryContext();
            var client = new Client { Id = 1, Name = "ToDelete" };
            context.Clients.Add(client);
            context.SaveChanges();

            var repo = new ClientRepository(context);
            await repo.DeleteClientAsync(1);

            Assert.Empty(context.Clients);
        }
    }
}
