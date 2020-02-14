using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CSCAutomate
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Out.WriteLine("Error Arg Count not 3. Expecting [ConnectionString, ContainerName, CloudApiKey]");
                return;
            }

            var blobApi = new BlobApi(args[0], args[1]);
            var cscApi = new CloudSkillApi(args[2]);

            List<ContestResponse> contestReponseList = await cscApi.CreateCollectionChallengesAsync(ContestFactory.Collections, ContestFactory.Default);

            string json = JsonConvert.SerializeObject(contestReponseList);
            string fileName = $"{DateTime.Now.ToString("MMMM")}{Guid.NewGuid().ToString()}";

            await blobApi.UploadToBlobAsync(json, fileName);
        }
    }
}
