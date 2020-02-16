using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;

namespace CSCAutomate
{
    class Program
    {
        private static string ContestPrefix = "CurrentContest_";

        public static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Out.WriteLine("Error Arg Count not 3. Expecting [ConnectionString, ContainerName, CloudApiKey]");
                return;
            }

            var blobApi = new BlobApi(args[0], args[1]);
            var cscApi = new CloudSkillApi(args[2]);

            //await CreateChallengesAsyc(blobApi, cscApi);
            //await GetCurrentContestAsync(blobApi, cscApi);

            return;
        }

        public static async Task<List<ContestResponse>> GetCurrentContestAsync(BlobApi blobApi, CloudSkillApi cscApi)
        {
            List<BlobItem> blobs = await blobApi.GetBlobItems(ContestPrefix);

            if (blobs.Count == 0)
                return null;

            string blobName = blobs[0].Name;

            return await blobApi.GetBlobContentsAync<List<ContestResponse>>(blobName);
        }

        public static async Task CreateChallengesAsyc(BlobApi blobApi, CloudSkillApi cscApi)
        {
            List<ContestResponse> contestReponseList = await cscApi.CreateCollectionChallengesAsync(ContestFactory.Collections, ContestFactory.Default);

            string json = JsonConvert.SerializeObject(contestReponseList);
            string fileName = $"{ContestPrefix}-{DateTime.Now.ToString("MMMM")}{Guid.NewGuid().ToString()}";

            await blobApi.UploadToBlobAsync(json, fileName);
        }
    }
}
