using Azure.Storage.Queues.Models;
using LojaNova.Function.ProcessarPedido.Repositories;
using LojaNova.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace LojaNova.Function.ProcessarPedido
{
    public class OrderProcessorFunction
    {
        private readonly ILogger<OrderProcessorFunction> _logger;
        private readonly IOrderRepository _orderRepository;

        public OrderProcessorFunction(ILogger<OrderProcessorFunction> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        [Function(nameof(OrderProcessorFunction))]
        public async Task Run([QueueTrigger("order-queue", Connection = "LojaNovaStorageAccount")] QueueMessage myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed a new message: {myQueueItem}");

            try
            {
                var order = JsonConvert.DeserializeObject<Order>(myQueueItem.Body.ToString());

                if (order == null)
                {
                    _logger.LogError("Failed to deserialize order message. Message was null.");
                    return;
                }

                // Você pode adicionar validações adicionais aqui, se necessário.
                if (order.OrderItems == null || !order.OrderItems.Any())
                {
                    _logger.LogWarning($"Order {order.Id} has no items. Skipping database insertion.");
                    return;
                }

                // Exemplo de lógica adicional: atualizar estoque
                // Isso exigiria um ProductRepository e transações se fosse crítico
                // foreach (var item in order.OrderItems)
                // {
                //    var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                //    if (product != null)
                //    {
                //        product.StockQuantity -= item.Quantity;
                //        await _productRepository.UpdateProductAsync(product);
                //    }
                // }

                order.Status = "Approved";

                // Insere o pedido na base de dados SQL
                await _orderRepository.AddOrderAsync(order);

                _logger.LogInformation($"Order {order.Id} processed and saved to database successfully.");
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError($"Error deserializing message from queue: {ex.Message}. Message content: {myQueueItem}");
                // Considerar mover para uma fila de dead-letter
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unhandled error occurred while processing queue message: {ex.Message}. Stack Trace: {ex.StackTrace}");
                // Erros que não são de serialização podem indicar problemas no DB ou lógica.
                // A mensagem será automaticamente reenfileirada (com atraso) até o MaxDequeueCount
                // após o qual será movida para uma fila de dead-letter (order-queue-poison).
            }
        }
    }
}
