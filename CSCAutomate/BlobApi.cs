using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

namespace CSCAutomate
{
    public class BlobApi
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly BlobContainerClient containerClient;
        private readonly string ConnectionString;
        private readonly string ContainerName;

        public BlobApi(string connectionString, string containerName)
        {
            ConnectionString = connectionString;
            ContainerName = containerName;

            blobServiceClient = new BlobServiceClient(ConnectionString);
            containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        }

        public async Task UploadToBlobAsync(string json, string blobName, string localFilePath = "./temp.json")
        {
            await containerClient.CreateIfNotExistsAsync();

            // Write text to the file
            await File.WriteAllTextAsync(localFilePath, json);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient($"{blobName}.json");

            // Open the file and upload its data
            using FileStream uploadFileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(uploadFileStream);
            uploadFileStream.Close();
        }
    }
}
