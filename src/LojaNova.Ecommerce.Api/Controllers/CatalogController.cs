using LojaNova.Ecommerce.Api.Services;
using LojaNova.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LojaNova.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _catalogService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _catalogService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] Product product, IFormFile? imageFile)
        {
            Stream? imageStream = null;
            string? imageFileName = null;

            if (imageFile != null)
            {
                imageStream = imageFile.OpenReadStream();
                imageFileName = imageFile.FileName;
            }

            await _catalogService.AddProductAsync(product, imageStream, imageFileName);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromForm] Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            Stream? imageStream = null;
            string? imageFileName = null;

            if (imageFile != null)
            {
                imageStream = imageFile.OpenReadStream();
                imageFileName = imageFile.FileName;
            }
            else if (Request.Form.ContainsKey("removeImage") && Request.Form["removeImage"] == "true")
            {
                // Isso é um sinal para remover a imagem existente.
                // O serviço interpretará imageFile = null e imageFileName = "delete" como um comando para remover.
                imageFileName = "delete";
            }


            var updated = await _catalogService.UpdateProductAsync(product, imageStream, imageFileName);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _catalogService.DeleteProductAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
