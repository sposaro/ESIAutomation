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
        private static async Task<List<ContestResponse>> CreateAndSaveChallengesAsync(string json, ILogger logger)
        {
            logger.LogInformation($"C# CreateChallenges function processing async: {json}");

            ChallengeRequest request = ContestFactory.CreateChallengeRequest(json);

            if (request == null)
                throw new ArgumentException("Error parsing challenge request JSON");

            if (string.IsNullOrWhiteSpace(request.BaseInputs.Mstpid))
                throw new ArgumentNullException($"{nameof(request.BaseInputs.Mstpid)} must be valid");

            BlobApi blobApi = await BlobApi.BlobApiInstance;
            CloudSkillApi cscApi = await CloudSkillApi.CloudSkillsApiInstance;

            // Set the end date for +1 month
            DateTime startDate = DateTime.Parse(request.BaseInputs.StartDateStr);
            request.BaseInputs.EndDateStr = startDate.AddMonths(1).ToString("MM-dd-yyyy HH:mm:ss");

            List<ContestResponse> response = await cscApi.CreateChallengesAsyc(request);

            logger.LogInformation($"Created the Challenges. Saving response to blob");

            // Writes the blob to storage
            string jsonString = JsonConvert.SerializeObject(response);
            string fileName = $"{request.BaseInputs.Mstpid}-{DateTime.Now:yyyy-MM}-{Guid.NewGuid()}";
            await blobApi.UploadToBlobAsync(jsonString, fileName);

            logger.LogInformation($"C# CreateChallenges function processed");

            return response;
        }

        #endregion
    }
}
