using LojaNova.Function.ProcessarPedido.Data;
using LojaNova.Shared.Models;

namespace LojaNova.Function.ProcessarPedido.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly LojaNovaDbContext _context;

        public OrderRepository(LojaNovaDbContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order)
        {
            // Certifique-se de que os objetos aninhados (OrderItems) também sejam adicionados
            // Se os IDs de Product e Client já existirem, o EF Core tentará adicioná-los novamente
            // e causará um erro. É importante que o Order object recebido da fila não inclua
            // objetos Client ou Product completos para serem inseridos, apenas seus IDs.
            // Se vierem completos, você precisará anexá-los ou carregá-los do DB.

            // Para evitar problemas com rastreamento de entidades ou IDs gerados:
            // Se OrderItem.Product/Client forem apenas para navegação e não para inserção/atualização junto com o pedido
            // Certifique-se de que eles não estão sendo rastreados pelo DbContext antes de adicionar o Order.

            // Uma forma segura é carregar os produtos do DB se você precisar validar ou recalcular valores
            // E garantir que OrderItems não tente inserir novos produtos, apenas referenciá-los.
            foreach (var item in order.OrderItems)
            {
                // Se o ProductId for válido, e você não quiser inserir um novo Product
                // defina o Product como null para que o EF Core não tente adicioná-lo
                item.Product = null;
            }
            order.Client = null; // Assume que o cliente já existe no banco de dados e não será inserido junto com o pedido

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
    }
}
