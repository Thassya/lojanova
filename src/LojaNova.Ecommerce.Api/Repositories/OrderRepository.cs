using LojaNova.Ecommerce.Api.Data;
using LojaNova.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace LojaNova.Ecommerce.Api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly LojaNovaDbContext _context;

        public OrderRepository(LojaNovaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                                 .Include(o => o.OrderItems)
                                 .ThenInclude(oi => oi.Product)
                                 .Include(o => o.Client)
                                 .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                                 .Include(o => o.OrderItems)
                                 .ThenInclude(oi => oi.Product)
                                 .Include(o => o.Client)
                                 .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
    }
}
