using LojaNova.Ecommerce.Api.Repositories;
using LojaNova.Shared.Models;

namespace LojaNova.Ecommerce.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllClientsAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetClientByIdAsync(id);
        }

        public async Task AddClientAsync(Client client)
        {
            await _clientRepository.AddClientAsync(client);
        }

        public async Task<bool> UpdateClientAsync(Client client)
        {
            var existingClient = await _clientRepository.GetClientByIdAsync(client.Id);
            if (existingClient == null)
            {
                return false;
            }

            // Atualizar propriedades
            existingClient.Name = client.Name;
            existingClient.Email = client.Email;
            existingClient.Address = client.Address;
            existingClient.Phone = client.Phone;

            await _clientRepository.UpdateClientAsync(existingClient);
            return true;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var existingClient = await _clientRepository.GetClientByIdAsync(id);
            if (existingClient == null)
            {
                return false;
            }
            await _clientRepository.DeleteClientAsync(id);
            return true;
        }
    }
}
