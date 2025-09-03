using LojaNova.Ecommerce.Api.Data;
using LojaNova.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace LojaNova.Ecommerce.Api.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly LojaNovaDbContext _context;

        public ClientRepository(LojaNovaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task AddClientAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClientAsync(int id)
        {
            var clientToDelete = await _context.Clients.FindAsync(id);
            if (clientToDelete != null)
            {
                _context.Clients.Remove(clientToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
