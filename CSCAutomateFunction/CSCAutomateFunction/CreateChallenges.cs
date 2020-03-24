using CSCAutomateLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CSCAutomateFunction
{
    public static class CreateChallenges
    {
        #region "Public Methods"
        [FunctionName("CreateChallengesFromQueue")]
        public static async Task RunQueue(
            [QueueTrigger("stq-cloudskillschallenge1")]string myQueueItem, 
            ILogger log)
        {
            try
            {
                await CreateChallengesAsync(myQueueItem, log);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in CreateChallenges: {0}", ex.Message));
                throw;
            }
        }

        [FunctionName("CreateChallengesFromHttp")]
        public static async Task<IActionResult> RunHttp(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                await CreateChallengesAsync(requestBody, log);
                string responseMessage = "This HTTP triggered function executed successfully.";
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in CreateChallenges: {0}", ex.Message));
                throw;
            }
        }
        #endregion

        #region "Private Methods"

        static async Task CreateChallengesAsync([QueueTrigger("stq-cloudskillschallenge1")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# CreateChallenges function processing async: {myQueueItem}");
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
