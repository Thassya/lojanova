namespace LojaNova.Ecommerce.Api.Services
{
    public interface IAzureStorageService
    {
        Task SendMessageToQueueAsync<T>(string queueName, T message);
        Task<Uri> UploadFileToShareAsync(string shareName, string directoryName, string fileName, Stream fileStream);
        Task DeleteFileFromShareAsync(string shareName, string directoryName, string fileName);
        // Adicionar métodos para download, listagem se necessário
    }
}
