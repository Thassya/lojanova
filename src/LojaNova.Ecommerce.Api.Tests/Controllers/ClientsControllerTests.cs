using LojaNova.Ecommerce.Api.Controllers;
using LojaNova.Ecommerce.Api.Services;
using LojaNova.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LojaNova.Ecommerce.Api.Tests.Controllers
{
    public class ClientsControllerTests
    {
        [Fact]
        public async Task GetClients_ReturnsOkWithClients()
        {
            var serviceMock = new Mock<IClientService>();
            serviceMock.Setup(s => s.GetAllClientsAsync()).ReturnsAsync(new List<Client>());

            var controller = new ClientsController(serviceMock.Object);
            var result = await controller.GetClients();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<Client>>(okResult.Value);
        }

        [Fact]
        public async Task GetClient_ReturnsOk_WhenFound()
        {
            var serviceMock = new Mock<IClientService>();
            serviceMock.Setup(s => s.GetClientByIdAsync(1)).ReturnsAsync(new Client { Id = 1 });

            var controller = new ClientsController(serviceMock.Object);
            var result = await controller.GetClient(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<Client>(okResult.Value);
        }

        [Fact]
        public async Task GetClient_ReturnsNotFound_WhenNotFound()
        {
            var serviceMock = new Mock<IClientService>();
            serviceMock.Setup(s => s.GetClientByIdAsync(1)).ReturnsAsync((Client)null);

            var controller = new ClientsController(serviceMock.Object);
            var result = await controller.GetClient(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostClient_ReturnsCreatedAtAction()
        {
            var serviceMock = new Mock<IClientService>();
            var client = new Client { Id = 1 };
            var controller = new ClientsController(serviceMock.Object);

            var result = await controller.PostClient(client);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(client, created.Value);
        }

        [Fact]
        public async Task PutClient_ReturnsBadRequest_WhenIdMismatch()
        {
            var serviceMock = new Mock<IClientService>();
            var controller = new ClientsController(serviceMock.Object);

            var result = await controller.PutClient(1, new Client { Id = 2 });

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutClient_ReturnsNotFound_WhenUpdateFails()
        {
            var serviceMock = new Mock<IClientService>();
            serviceMock.Setup(s => s.UpdateClientAsync(It.IsAny<Client>())).ReturnsAsync(false);

            var controller = new ClientsController(serviceMock.Object);
            var result = await controller.PutClient(1, new Client { Id = 1 });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutClient_ReturnsNoContent_WhenUpdateSucceeds()
        {
            var serviceMock = new Mock<IClientService>();
            serviceMock.Setup(s => s.UpdateClientAsync(It.IsAny<Client>())).ReturnsAsync(true);

            var controller = new ClientsController(serviceMock.Object);
            var result = await controller.PutClient(1, new Client { Id = 1 });

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteClient_ReturnsNotFound_WhenDeleteFails()
        {
            var serviceMock = new Mock<IClientService>();
            serviceMock.Setup(s => s.DeleteClientAsync(1)).ReturnsAsync(false);

            var controller = new ClientsController(serviceMock.Object);
            var result = await controller.DeleteClient(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteClient_ReturnsNoContent_WhenDeleteSucceeds()
        {
            var serviceMock = new Mock<IClientService>();
            serviceMock.Setup(s => s.DeleteClientAsync(1)).ReturnsAsync(true);

            var controller = new ClientsController(serviceMock.Object);
            var result = await controller.DeleteClient(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
