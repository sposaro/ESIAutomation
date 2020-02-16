using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public async Task<T> GetBlobContentsAync<T>(string blobname)
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobname);
            BlobDownloadInfo blobDownload = await blobClient.DownloadAsync();
            T result;

            using (var stream = new MemoryStream())
            {
                await blobDownload.Content.CopyToAsync(stream);
                stream.Position = 0;
                result = DeserializeFromStream<T>(stream);
            }
            return result;
        }

        private T DeserializeFromStream<T>(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        public async Task<List<BlobItem>> GetBlobItems(string prefix = "")   
        {
            var results = new List<BlobItem>();

            // List all blobs in the container
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: prefix))
            {
                results.Add(blobItem);
            }

            return results;
        }
    }
}
