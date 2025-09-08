using Azure;
using Azure.Data.Tables;
using LojaNova.Shared.Models;

namespace LojaNova.Function.ProcessarPedido.Repositories;

public class OrderEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "Order";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // Example properties (add more as needed)
    public int OrderId { get; set; }
    public int ClientId { get; set; }
    public decimal TotalAmount { get; set; }
}

public class OrderTableRepository : IOrderRepository
{
    private readonly TableClient _tableClient;

    public OrderTableRepository(string storageConnectionString, string tableName)
    {
        _tableClient = new TableClient(storageConnectionString, tableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task AddOrderAsync(Order order)
    {
        var entity = MapToEntity(order);
        await _tableClient.AddEntityAsync(entity);
    }

    // Helper methods for mapping
    private static OrderEntity MapToEntity(Order order)
    {
        return new OrderEntity
        {
            PartitionKey = "Order",
            RowKey = order.Id.ToString(),
            OrderId = order.Id,
            ClientId = order.ClientId,
            TotalAmount = order.TotalAmount
            // Map other properties as needed
        };
    }

    private static Order MapToOrder(OrderEntity entity)
    {
        return new Order
        {
            Id = entity.OrderId,
            ClientId = entity.ClientId,
            TotalAmount = entity.TotalAmount
            // Map other properties as needed
        };
    }
}