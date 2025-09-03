using LojaNova.Shared.Models;

namespace LojaNova.Ecommerce.Api.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task SendOrderToQueueAsync(Order order);
    }
}
