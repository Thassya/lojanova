using LojaNova.Ecommerce.Api.Controllers;
using LojaNova.Ecommerce.Api.Services;
using LojaNova.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LojaNova.Ecommerce.Api.Tests.Controllers
{
    public class CatalogControllerTests
    {
        [Fact]
        public async Task GetProducts_ReturnsOkWithProducts()
        {
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(new List<Product>());

            var controller = new CatalogController(serviceMock.Object);
            var result = await controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        }

        [Fact]
        public async Task GetProduct_ReturnsOk_WhenFound()
        {
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(new Product { Id = 1 });

            var controller = new CatalogController(serviceMock.Object);
            var result = await controller.GetProduct(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<Product>(okResult.Value);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenNotFound()
        {
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync((Product?)null);

            var controller = new CatalogController(serviceMock.Object);
            var result = await controller.GetProduct(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostProduct_ReturnsCreatedAtAction()
        {
            var serviceMock = new Mock<ICatalogService>();
            var product = new Product { Id = 1 };
            var controller = new CatalogController(serviceMock.Object);

            var result = await controller.PostProduct(product, null);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(product, created.Value);
        }

        [Fact]
        public async Task PutProduct_ReturnsBadRequest_WhenIdMismatch()
        {
            var serviceMock = new Mock<ICatalogService>();
            var controller = new CatalogController(serviceMock.Object);

            var result = await controller.PutProduct(1, new Product { Id = 2 }, null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutProduct_ReturnsNotFound_WhenUpdateFails()
        {
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.UpdateProductAsync(It.IsAny<Product>(), null, null)).ReturnsAsync(false);

            var controller = new CatalogController(serviceMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.PutProduct(1, new Product { Id = 1 }, null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutProduct_ReturnsNoContent_WhenUpdateSucceeds()
        {
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.UpdateProductAsync(It.IsAny<Product>(), null, null)).ReturnsAsync(true);

            var controller = new CatalogController(serviceMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await controller.PutProduct(1, new Product { Id = 1 }, null);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenDeleteFails()
        {
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.DeleteProductAsync(1)).ReturnsAsync(false);

            var controller = new CatalogController(serviceMock.Object);
            var result = await controller.DeleteProduct(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenDeleteSucceeds()
        {
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.DeleteProductAsync(1)).ReturnsAsync(true);

            var controller = new CatalogController(serviceMock.Object);
            var result = await controller.DeleteProduct(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
