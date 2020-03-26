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
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(requestBody))
                    throw new ArgumentException("Expecting JSON post body. Recieved empty string.");

                string response = await AddLearnerToChallengeAsync(requestBody, log);

                return new OkObjectResult(response);
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
            CloudSkillApi cscApi = await CloudSkillApi.CloudSkillsApiInstance;
            LearnerRequest request = JsonConvert.DeserializeObject<LearnerRequest>(learnerRequestJson);

            // Error checking for blank values
            if (request == null)
                throw new FormatException("Malformed learner JSON");
            else if (string.IsNullOrWhiteSpace(request.CustomerId))
                throw new ArgumentException(nameof(request.CustomerId));
            else if (string.IsNullOrWhiteSpace(request.LearnerId))
                throw new ArgumentException(nameof(request.LearnerId));

            string contestId = await GetContestId(request.CustomerId, request.CollectionName);
            string learnerReponse = await cscApi.AddLearnerAsync(contestId, request.LearnerId);

            log.LogInformation($"C# CreateLearner function processed: {learnerReponse}");

            return learnerReponse;
        }

        public static async Task<string> GetContestId(string customerId, string collectionName)
        {
            BlobApi blobApi = await BlobApi.BlobApiInstance;
            List<ContestResponse> contests = await blobApi.GetActiveContest(customerId);

            if (contests == null)
                throw new KeyNotFoundException($"CustomerId ({customerId}) not found");

            foreach(ContestResponse contest in contests)
            {
                if (contest.CollectionName as string == collectionName)
                    return contest.ContestId;
            }

            throw new DirectoryNotFoundException($"Collection name ({collectionName}) not found");
        }

        #endregion
    }
}
