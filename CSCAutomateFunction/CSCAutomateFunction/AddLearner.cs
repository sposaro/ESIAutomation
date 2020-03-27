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
    public static class AddLearner
    {
        #region "Public Methods"
        [FunctionName("AddLearnerByChallengeName")]
        public static async Task<IActionResult> RunHttp(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                string collectionUrl = await AddLearnerToChallengeAsync(requestBody, log);

                return new OkObjectResult(collectionUrl);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in AddLearnerToChallengeFromHttp: {0}", ex.Message));
                return new BadRequestObjectResult(ex.Message);
            }
        }
        #endregion

        #region "Private Methods"

        private static async Task<string> AddLearnerToChallengeAsync(string learnerRequestJson, ILogger log)
        {
            log.LogInformation($"C# CreateLearner function processing async: {learnerRequestJson}");
            
            if (string.IsNullOrWhiteSpace(learnerRequestJson))
                throw new ArgumentException("Expecting JSON post body. Recieved empty string.");

            CloudSkillApi cscApi = await CloudSkillApi.CloudSkillsApiInstance;
            BlobApi blobApi = await BlobApi.BlobApiInstance;
            LearnerRequest request = JsonConvert.DeserializeObject<LearnerRequest>(learnerRequestJson);

            // Error checking for blank values
            if (request == null)
                throw new FormatException("Malformed learner JSON");
            else if (string.IsNullOrWhiteSpace(request.CustomerId))
                throw new ArgumentException($"{nameof(request.CustomerId)} is blank");
            else if (string.IsNullOrWhiteSpace(request.LearnerId))
                throw new ArgumentException($"{nameof(request.LearnerId)} is blank");

            ContestResponse contest = await blobApi.GetContest(request.CustomerId, request.CollectionName);
            Learner learnerReponse = await cscApi.AddLearnerAsync(contest.ContestId, request.LearnerId);

            log.LogInformation($"C# CreateLearner function processed: {learnerReponse}");

            return contest.CollectionUrl;
        }

        #endregion
    }
}
