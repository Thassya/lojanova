using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using System.Text;

namespace LojaNova.Ecommerce.Api.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly QueueClient _orderQueueClient;
        private readonly BlobServiceClient _blobServiceClient; // Não diretamente usado, mas útil para outros cenários ou se você decidir usar blobs para imagens
        private readonly ShareClient _shareClient; // Cliente para Azure File Shares

        public AzureStorageService(QueueClient orderQueueClient, BlobServiceClient blobServiceClient, ShareClient shareClient)
        {
            _orderQueueClient = orderQueueClient;
            _blobServiceClient = blobServiceClient;
            _shareClient = shareClient;
        }

        public async Task SendMessageToQueueAsync<T>(string queueName, T message)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(jsonMessage);
            await _orderQueueClient.SendMessageAsync(Convert.ToBase64String(bytes));
        }

        public async Task<Uri> UploadFileToShareAsync(string shareName, string directoryName, string fileName, Stream fileStream)
        {
            var directoryClient = _shareClient.GetDirectoryClient(directoryName);
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(fileName);
            await fileClient.CreateAsync(fileStream.Length); // Define o tamanho do arquivo
            await fileClient.UploadRangeAsync(new Azure.HttpRange(0, fileStream.Length),
                fileStream);

            // Retorna a URL do arquivo para armazenamento no banco de dados
            return fileClient.Uri;
        }

        public async Task DeleteFileFromShareAsync(string shareName, string directoryName, string fileName)
        {
            var directoryClient = _shareClient.GetDirectoryClient(directoryName);
            var fileClient = directoryClient.GetFileClient(fileName);
            await fileClient.DeleteIfExistsAsync();
        }
    }
}
