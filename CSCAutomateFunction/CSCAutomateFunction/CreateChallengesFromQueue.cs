using CSCAutomateLib;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSCAutomateFunction
{
    public static class CreateChallengesFromQueue
    {
        #region "Public Methods"
        [FunctionName("CreateChallengesFromQueue")]
        public static async Task Run([QueueTrigger("stq-cloudskillschallenge1")]string myQueueItem, ILogger log)
        {
            try
            {
                await CreateChallenge(myQueueItem, log);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in CreateChallengesFunction: {0}", ex.Message));
                throw;
            }
        }
        #endregion

        #region "Private Methods"

        static async Task CreateChallenge([QueueTrigger("stq-cloudskillschallenge1")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processing async: {myQueueItem}");
            string jsonMessage = Regex.Unescape(myQueueItem);
            string doubleQuote = "\"";
            jsonMessage = jsonMessage.Remove(jsonMessage.IndexOf(doubleQuote), doubleQuote.Length);
            jsonMessage = jsonMessage.Remove(jsonMessage.LastIndexOf(doubleQuote));

            string environmentType = Environment.GetEnvironmentVariable("CSCAutomateEnvironment", EnvironmentVariableTarget.Process);
            string keyVaultName = Environment.GetEnvironmentVariable("CSCApiKeyVaultName", EnvironmentVariableTarget.Process);

            Configuration config = await ConfigurationFactory.CreateConfigurationAsync(environmentType, keyVaultName);
            BlobApi blobApi = new BlobApi(config);
            CloudSkillApi cscApi = new CloudSkillApi(config);

            await cscApi.CreateChallengesAsyc(blobApi, jsonMessage);

            //string jsonConfig = JsonConvert.SerializeObject(config);
            //log.LogInformation($"Config: {jsonConfig}");
            log.LogInformation($"C# Queue trigger function processed: {jsonMessage}");
        }

        #endregion
    }
}
