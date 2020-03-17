﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CSCAutomateLib
{
    public class BlobApi
    {
        #region "Local Variables"
        private readonly BlobServiceClient blobServiceClient;
        private readonly BlobContainerClient containerClient;
        private readonly string ConnectionString;
        private readonly string ContainerName;
        #endregion

        #region "Constructor"
        public BlobApi(Configuration config)
        {
            ConnectionString = config.StorageConn1;
            ContainerName = config.StorageContainer1;

            blobServiceClient = new BlobServiceClient(ConnectionString);
            containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        }
        #endregion

        #region "Public Methods"
        public async Task UploadToBlobAsync(string json, string blobName, string localFilePath = "./temp.json")
        {
            await containerClient.CreateIfNotExistsAsync();

            // Write text to the file
            File.WriteAllText(localFilePath, json);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient($"{blobName}.json");

            // Open the file and upload its data
            FileStream uploadFileStream = File.OpenRead(localFilePath);
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

        public List<BlobItem> GetBlobItems(string prefix = "")
        {
            var results = new List<BlobItem>();

            // List all blobs in the container
            foreach (BlobItem blobItem in containerClient.GetBlobs(prefix: prefix))
            {
                results.Add(blobItem);
            }

            return results;
        }
        #endregion

        #region "Private Methods"
        private T DeserializeFromStream<T>(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }
        #endregion
    }
}