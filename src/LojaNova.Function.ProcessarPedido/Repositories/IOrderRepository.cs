using LojaNova.Shared.Models;

namespace LojaNova.Function.ProcessarPedido.Repositories
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
    }
}
