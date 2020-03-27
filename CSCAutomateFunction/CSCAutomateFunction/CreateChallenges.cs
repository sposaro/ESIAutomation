using CSCAutomateLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace CSCAutomateFunction
{
    public static class CreateChallenges
    {
        #region "Public Methods"
        [FunctionName("CreateChallengesFromHttp")]
        public static async Task<IActionResult> RunHttp(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
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

                await CreateAndSaveChallengesAsync(requestBody, log);

                return new OkObjectResult(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in CreateChallengesFromHttp: {0}", ex.Message));
                return new BadRequestObjectResult($"Error creating challenge. {ex.Message}");
            }
        }
        #endregion

        #region "Private Methods"
        private static async Task CreateAndSaveChallengesAsync(string json, ILogger logger)
        {
            logger.LogInformation($"C# CreateChallenges function processing async: {json}");

            ChallengeRequest challengeRequest = ContestFactory.CreateChallengeRequest(json);

            if (challengeRequest == null)
                throw new ArgumentException("Error parsing challenge request JSON");

            BlobApi blobApi = await BlobApi.BlobApiInstance;
            CloudSkillApi cscApi = await CloudSkillApi.CloudSkillsApiInstance;

            // Creates the challenges
            List<ContestResponse> response = await cscApi.CreateChallengesAsyc(challengeRequest);

            logger.LogInformation($"Created the Challenges. Saving response to blob");

            // Uses the tpid as the prefix for the blobs filename
            string blobPrefix = response.First<ContestResponse>().Mstpid;

            // Writes the blob to storage
            string jsonString = JsonConvert.SerializeObject(response);
            string fileName = $"{blobPrefix}-{DateTime.Now:yyyy-MM}-{Guid.NewGuid()}";
            await blobApi.UploadToBlobAsync(jsonString, fileName);

            logger.LogInformation($"C# CreateChallenges function processed");
        }

        #endregion
    }
}
