using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CSCAutomateLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static AsyncLazy<BlobApi> Instance = new AsyncLazy<BlobApi>(async () =>
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
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentNullException($"{nameof(customerId)} is blank");
            else if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException($"{nameof(collectionName)} is blank");

            IList<ContestResponse> contests = await GetContestResponseAsync(customerId);

            if (contests == null)
                throw new KeyNotFoundException($"CustomerId ({customerId}) not found");

            foreach (ContestResponse contest in contests)
            {
                if (contest.CollectionName.ToLower() == collectionName.ToLower())
                    return contest;
            }

            throw new DirectoryNotFoundException($"Learning Path ({collectionName}) not found");
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            await containerClient.DeleteBlobIfExistsAsync(blobName);
        }

        public async Task<IList<ContestResponse>> GetContestResponseAsync(string tpid)
            => (await GetAllContestTupleAsync(tpid)).Item1;

        /// <summary>
        /// Gets all the contest.
        /// </summary>
        /// <param name="tpid">If blank, return all contests</param>
        /// <returns></returns>
        public async Task<Tuple<IList<ContestResponse>,string>> GetAllContestTupleAsync(string tpid)
        {
            IList<BlobItem> blobs = await GetBlobItemsAsync(tpid);

            if (blobs.Count == 0)
                throw new KeyNotFoundException($"Customer Code {tpid} was not found.");

            string blobName = blobs[0].Name;
            BlobContents contents = await GetBlobContentsAync<BlobContents>(blobName);

            return new Tuple<IList<ContestResponse>, string>(contents.Contests, blobName);
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
        private async Task<T> GetBlobContentsAync<T>(string blobname)
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
        private async Task<IList<BlobItem>> GetBlobItemsAsync(string prefix = "")
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
