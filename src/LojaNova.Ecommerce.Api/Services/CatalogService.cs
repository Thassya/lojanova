using LojaNova.Ecommerce.Api.Repositories;
using LojaNova.Shared.Models;

namespace LojaNova.Ecommerce.Api.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IProductRepository _productRepository;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConfiguration _configuration;
        private readonly string _productImagesShareName;

        public CatalogService(IProductRepository productRepository, IAzureStorageService azureStorageService, IConfiguration configuration)
        {
            _productRepository = productRepository;
            _azureStorageService = azureStorageService;
            _configuration = configuration;
            _productImagesShareName = _configuration["AzureStorageSettings:ProductImagesShareName"] ?? "product-images";
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task AddProductAsync(Product product, Stream? imageStream = null, string? imageFileName = null)
        {
            if (imageStream != null && !string.IsNullOrEmpty(imageFileName))
            {
                // Criar um nome de arquivo único para evitar colisões
                var uniqueFileName = $"{Guid.NewGuid()}_{imageFileName}";
                // O diretório dentro do file share pode ser vazio ou baseado em algo como o ID do produto, categoria, etc.
                var directoryName = "images"; // Exemplo: todos em um subdiretório 'images'
                var imageUrl = await _azureStorageService.UploadFileToShareAsync(_productImagesShareName, directoryName, uniqueFileName, imageStream);
                product.ImageUrl = imageUrl.ToString();
            }

            await _productRepository.AddProductAsync(product);
        }

        public async Task<bool> UpdateProductAsync(Product product, Stream? imageStream = null, string? imageFileName = null)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(product.Id);
            if (existingProduct == null)
            {
                return false;
            }

            // Se uma nova imagem for fornecida, apagar a antiga e carregar a nova
            if (imageStream != null && !string.IsNullOrEmpty(imageFileName))
            {
                // Apagar imagem antiga se existir
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    var uri = new Uri(existingProduct.ImageUrl);
                    var segments = uri.Segments.Select(s => s.Trim('/')).ToArray();
                    // Exemplo: https://account.file.core.windows.net/share/directory/filename.ext
                    // segments[0] é o share name, segments[1] é o directoryName, segments[2] é o filename
                    if (segments.Length >= 3) // Assume share/directory/filename structure
                    {
                        var shareName = segments[0]; // Isso pode ser hardcoded para _productImagesShareName
                        var directoryName = segments[1];
                        var oldFileName = segments[2];
                        await _azureStorageService.DeleteFileFromShareAsync(shareName, directoryName, oldFileName);
                    }
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{imageFileName}";
                var imagesDirectoryName = "images";
                var imageUrl = await _azureStorageService.UploadFileToShareAsync(_productImagesShareName, imagesDirectoryName, uniqueFileName, imageStream);
                product.ImageUrl = imageUrl.ToString();
            }
            else if (imageStream == null && imageFileName == "delete") // Sinal para remover a imagem
            {
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    var uri = new Uri(existingProduct.ImageUrl);
                    var segments = uri.Segments.Select(s => s.Trim('/')).ToArray();
                    if (segments.Length >= 3)
                    {
                        var shareName = segments[0];
                        var directoryName = segments[1];
                        var oldFileName = segments[2];
                        await _azureStorageService.DeleteFileFromShareAsync(shareName, directoryName, oldFileName);
                    }
                    product.ImageUrl = null;
                }
            }
            else // Se nenhuma nova imagem for fornecida, manter a URL existente
            {
                product.ImageUrl = existingProduct.ImageUrl;
            }


            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.ImageUrl = product.ImageUrl; // Atualiza a URL da imagem

            await _productRepository.UpdateProductAsync(existingProduct);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return false;
            }

            // Apagar imagem associada se existir
            if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
            {
                var uri = new Uri(existingProduct.ImageUrl);
                var segments = uri.Segments.Select(s => s.Trim('/')).ToArray();
                if (segments.Length >= 3)
                {
                    var shareName = segments[0];
                    var directoryName = segments[1];
                    var fileName = segments[2];
                    await _azureStorageService.DeleteFileFromShareAsync(shareName, directoryName, fileName);
                }
            }

            await _productRepository.DeleteProductAsync(id);
            return true;
        }
    }
}
