using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EventSignup.Services
{
    public class AzureBlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("="))
            {
                _blobServiceClient = new BlobServiceClient(connectionString);
            }
            else
            {
                // Fallback or handle cases where connection string is missing or invalid for development
                _blobServiceClient = null!;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            if (_blobServiceClient == null) return string.Empty;

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType } });
            }

            return blobClient.Uri.ToString();
        }
    }
}
