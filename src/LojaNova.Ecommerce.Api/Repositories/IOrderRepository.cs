using LojaNova.Shared.Models;

namespace LojaNova.Ecommerce.Api.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task AddOrderAsync(Order order); // Esta será usada pela Azure Function, não diretamente pela API
        // Task UpdateOrderAsync(Order order); // Se a API precisar atualizar status, etc.
    }
}
