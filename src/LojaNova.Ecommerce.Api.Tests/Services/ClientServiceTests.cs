using LojaNova.Ecommerce.Api.Repositories;
using LojaNova.Ecommerce.Api.Services;
using LojaNova.Shared.Models;
using Moq;
using Xunit;

namespace LojaNova.Ecommerce.Api.Tests.Services
{
    public class ClientServiceTests
    {
        [Fact]
        public async Task GetAllClientsAsync_ReturnsClients()
        {
            var repoMock = new Mock<IClientRepository>();
            repoMock.Setup(r => r.GetAllClientsAsync()).ReturnsAsync(new List<Client>());

            var service = new ClientService(repoMock.Object);
            var result = await service.GetAllClientsAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetClientByIdAsync_ReturnsClient()
        {
            var repoMock = new Mock<IClientRepository>();
            repoMock.Setup(r => r.GetClientByIdAsync(1)).ReturnsAsync(new Client { Id = 1 });

            var service = new ClientService(repoMock.Object);
            var result = await service.GetClientByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task AddClientAsync_CallsRepository()
        {
            var repoMock = new Mock<IClientRepository>();
            var service = new ClientService(repoMock.Object);

            var client = new Client();
            await service.AddClientAsync(client);

            repoMock.Verify(r => r.AddClientAsync(client), Times.Once);
        }

        [Fact]
        public async Task UpdateClientAsync_ReturnsFalse_WhenNotFound()
        {
            var repoMock = new Mock<IClientRepository>();
            repoMock.Setup(r => r.GetClientByIdAsync(1)).ReturnsAsync((Client)null);

            var service = new ClientService(repoMock.Object);
            var result = await service.UpdateClientAsync(new Client { Id = 1 });

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateClientAsync_UpdatesAndReturnsTrue()
        {
            var repoMock = new Mock<IClientRepository>();
            var existing = new Client { Id = 1, Name = "Old" };
            repoMock.Setup(r => r.GetClientByIdAsync(1)).ReturnsAsync(existing);

            var service = new ClientService(repoMock.Object);
            var updated = new Client { Id = 1, Name = "New", Email = "a", Address = "b", Phone = "c" };
            var result = await service.UpdateClientAsync(updated);

            Assert.True(result);
            repoMock.Verify(r => r.UpdateClientAsync(existing), Times.Once);
            Assert.Equal("New", existing.Name);
        }

        [Fact]
        public async Task DeleteClientAsync_ReturnsFalse_WhenNotFound()
        {
            var repoMock = new Mock<IClientRepository>();
            repoMock.Setup(r => r.GetClientByIdAsync(1)).ReturnsAsync((Client)null);

            var service = new ClientService(repoMock.Object);
            var result = await service.DeleteClientAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteClientAsync_DeletesAndReturnsTrue()
        {
            var repoMock = new Mock<IClientRepository>();
            repoMock.Setup(r => r.GetClientByIdAsync(1)).ReturnsAsync(new Client { Id = 1 });

            var service = new ClientService(repoMock.Object);
            var result = await service.DeleteClientAsync(1);

            Assert.True(result);
            repoMock.Verify(r => r.DeleteClientAsync(1), Times.Once);
        }
    }
}
