using CSCAutomateLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;


namespace CSCAutomateFunction
{
    public static class AddLearner
    {
        #region "Public Methods"
        [FunctionName("AddLearnerToChallengeFromHttp")]
        public static async Task<IActionResult> RunHttp(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    string warningMessage = "Expecting JSON post body. Recieved empty string.";
                    log.LogWarning(warningMessage);
                    return new BadRequestObjectResult(warningMessage);
                }

                await AddLearnerToChallengeAsync(requestBody, log);

                return new OkObjectResult(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in AddLearnerToChallengeFromHttp: {0}", ex.Message));
                throw;
            }
        }
        #endregion

        #region "Private Methods"

        private static async Task AddLearnerToChallengeAsync(string learnerRequestJson, ILogger log)
        {
            log.LogInformation($"C# CreateLearner function processing async: {learnerRequestJson}");
            string environmentType = Environment.GetEnvironmentVariable("CSCAutomateEnvironment", EnvironmentVariableTarget.Process);
            string keyVaultName = Environment.GetEnvironmentVariable("CSCApiKeyVaultName", EnvironmentVariableTarget.Process);

            Configuration config = await ConfigurationFactory.CreateConfigurationAsync(environmentType, keyVaultName);
            CloudSkillApi cscApi = new CloudSkillApi(config);

            string learnerReponse = await cscApi.AddLearnerAsync(learnerRequestJson);

            log.LogInformation($"C# CreateLearner function processed: {learnerReponse}");
        }

        #endregion
    }
}
