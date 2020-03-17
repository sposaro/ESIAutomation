using CSCAutomateLib;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;

namespace CSCAutomateFunction
{
    public static class CreateChallengesFunction
    {
        #region "Public Methods"
        [FunctionName("CreateChallengesFunction")]
        public static async void Run([QueueTrigger("stq-cloudskillschallenge1")]string myQueueItem, ILogger log)
        {
            try
            {
                log.LogInformation($"C# Queue trigger function processing async: {myQueueItem}");

                string environmentType = Environment.GetEnvironmentVariable("CSCAutomateEnvironment", EnvironmentVariableTarget.Process);
                string keyVaultName = Environment.GetEnvironmentVariable("CSCApiKeyVaultName", EnvironmentVariableTarget.Process);

                Configuration config = await ConfigurationFactory.CreateConfigurationAsync(environmentType, keyVaultName);
                BlobApi blobApi = new BlobApi(config);
                CloudSkillApi cscApi = new CloudSkillApi(config);

                await cscApi.CreateChallengesAsyc(blobApi, myQueueItem);

                log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            }
            catch (Exception ex)
            {
                log.LogInformation(string.Format("Error in CreateChallengesFunction: {0}", ex.Message));
                throw;
            }
        }
        #endregion
    }
}
