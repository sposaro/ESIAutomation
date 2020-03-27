using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public static AsyncLazy<BlobApi> BlobApiInstance = new AsyncLazy<BlobApi>(async () =>
            new BlobApi(await ConfigurationFactory.CreateDevConfigurationAysnc()));

        public BlobApi(Configuration config)
        {
            ConnectionString = config.StorageConn1;
            ContainerName = config.StorageContainer1;

            blobServiceClient = new BlobServiceClient(ConnectionString);
            containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        }
        #endregion

        #region "Public Methods"
        public async Task<ContestResponse> GetContest(string customerId, string collectionName)
        {
            List<ContestResponse> contests = await GetAllContestByCustomerId(customerId);

            if (contests == null)
                throw new KeyNotFoundException($"CustomerId ({customerId}) not found");

            foreach (ContestResponse contest in contests)
            {
                if (contest.CollectionName == collectionName)
                    return contest;
            }

            throw new DirectoryNotFoundException($"Collection name ({collectionName}) not found");
        }

        /// <summary>
        /// Gets current contests from Blob Storage
        /// </summary>
        /// <param name="blobApi"></param>
        /// <returns></returns>
        public async Task<List<ContestResponse>> GetAllContestByCustomerId(string tpid)
        {
            List<BlobItem> blobs = await GetBlobItemsAsync(tpid);

            if (blobs.Count == 0)
                return null;

            //TODO: Falsely assuming one active blob per TPID
            string blobName = blobs[0].Name;

            return await GetBlobContentsAync<List<ContestResponse>>(blobName);
        }

        public async Task UploadToBlobAsync(string json, string blobName)
        {
            await containerClient.CreateIfNotExistsAsync();

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient($"{blobName}.json");

            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            MemoryStream stream = new MemoryStream(byteArray);

            await blobClient.UploadAsync(stream);
        }
        #endregion

        #region "Private Methods"
        public async Task<T> GetBlobContentsAync<T>(string blobname)
        {
            await containerClient.CreateIfNotExistsAsync();
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

        /// <summary>
        /// Returns the raw blobs from storage
        /// </summary>
        /// <param name="prefix">The filename prefix. Default retuns all</param>
        /// <returns>Returns all the blob that filename matches the <paramref name="prefix"/></returns>
        public async Task<List<BlobItem>> GetBlobItemsAsync(string prefix = "")
        {
            await containerClient.CreateIfNotExistsAsync();

            var results = new List<BlobItem>();

            // List all blobs in the container
            foreach (BlobItem blobItem in containerClient.GetBlobs(prefix: prefix))
            {
                results.Add(blobItem);
            }

            return results;
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
        #endregion
    }
}
