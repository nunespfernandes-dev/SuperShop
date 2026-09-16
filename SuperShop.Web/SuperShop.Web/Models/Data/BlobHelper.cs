using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SuperShop.Web.Data
{
    public class BlobHelper : IBlobHelper
    {
        // Connection string do Storage Account do Azure. Vem do
        // appsettings.json (secção "Blob"): em desenvolvimento, mete lá a
        // connection string que o Azure dá em "Chaves de acesso", do
        // Storage Account criado na aula 14. Nunca a guardes a sério no
        // Git — usa User Secrets ou uma variável de ambiente.
        private readonly string _connectionString;

        public BlobHelper(IConfiguration configuration)
        {
            _connectionString = configuration["Blob:ConnectionString"] ?? string.Empty;
        }

        public async Task<Guid> UploadBlobAsync(IFormFile imageFile, string containerName)
        {
            using var stream = imageFile.OpenReadStream();
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(byte[] imageFile, string containerName)
        {
            using var stream = new MemoryStream(imageFile);
            return await UploadStreamAsync(stream, containerName);
        }

        private async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
        {
            var guid = Guid.NewGuid();

            var containerClient = new BlobContainerClient(_connectionString, containerName);

            // Cria o contentor caso ainda não exista, com acesso de leitura
            // anónimo ao nível do blob — é o que permite às views mostrarem
            // a imagem diretamente pelo endereço do Storage, sem chave.
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(guid.ToString());
            await blobClient.UploadAsync(stream, overwrite: true);

            return guid;
        }
    }
}
