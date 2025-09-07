using Azure.Storage.Queues;
using LojaNova.Ecommerce.Api.Repositories;
using LojaNova.Ecommerce.Api.Services;
using LojaNova.Shared.Models;
using Newtonsoft.Json;


public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAzureStorageService _azureStorageService;
    private readonly IConfiguration _configuration;
    private readonly string _orderQueueName;

    public OrderService(IOrderRepository orderRepository, IClientRepository clientRepository, IProductRepository productRepository, IAzureStorageService azureStorageService, IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _clientRepository = clientRepository;
        _productRepository = productRepository;
        _azureStorageService = azureStorageService;
        _configuration = configuration;
        _orderQueueName = _configuration["AzureStorageSettings:OrderQueueName"] ?? "orders";
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllOrdersAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _orderRepository.GetOrderByIdAsync(id);
    }

    public async Task SendOrderToQueueAsync(Order order)
    {
        // Validar se o cliente e os produtos existem e se o estoque é suficiente
        var client = await _clientRepository.GetClientByIdAsync(order.ClientId);
        if (client == null)
        {
            throw new ArgumentException("Client not found.");
        }

        order.TotalAmount = 0;
        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetProductByIdAsync(item.ProductId);
            if (product == null || product.StockQuantity < item.Quantity)
            {
                throw new ArgumentException($"Product {item.ProductId} not found or insufficient stock.");
            }
            item.UnitPrice = product.Price; // Garante que o preço usado é o atual do produto
            order.TotalAmount += item.Quantity * item.UnitPrice;
        }

        // A inserção real no DB será feita pela Azure Function.
        // Aqui, apenas enviamos o objeto Order para a fila.
        await _azureStorageService.SendMessageToQueueAsync(_orderQueueName, order);
    }
}