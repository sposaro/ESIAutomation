using CSCAutomateLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
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
                LearnerRequest request = JsonConvert.DeserializeObject<LearnerRequest>(requestBody);

                BlobApi blobApi = await BlobApi.Instance;
                ContestResponse contest = await blobApi.GetContest(request.CustomerId, request.CollectionName);

                CloudSkillApi cloudSkillApi = await CloudSkillApi.Instance;
                Learner learnerReponse = await cloudSkillApi.AddLearnerAsync(contest.ContestId, request.LearnerId);

                return new OkObjectResult(contest.CollectionUrl);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in AddLearnerToChallengeFromHttp: {0}", ex.Message));
                return new BadRequestObjectResult(ex.Message);
            }
        }
        #endregion
    }
}
