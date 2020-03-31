using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CSCAutomateLib;
using System.Collections.Generic;

namespace CSCAutomateFunction
{
    public static class GetLearnerProgress
    {
        [FunctionName("GetLearnerProgress")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string userName = req.Query["userName"];
                if (string.IsNullOrWhiteSpace(userName))
                    throw new ArgumentNullException($"{nameof(userName)} is blank.");

                string contestId = req.Query["contestId"];
                if (string.IsNullOrWhiteSpace(contestId))
                {
                    string companyId = req.Query["companyId"];
                    if (string.IsNullOrWhiteSpace(companyId))
                        throw new ArgumentNullException($"{nameof(companyId)} is blank.");

                    string learningPath = req.Query["learningPath"];
                    if (string.IsNullOrWhiteSpace(learningPath))
                        throw new ArgumentNullException($"{nameof(learningPath)} is blank.");

                    BlobApi blobApi = await BlobApi.Instance;
                    ContestResponse savedContest = await blobApi.GetContest(companyId, learningPath);
                    contestId = savedContest.ContestId;
                }

                CloudSkillApi cloudSkillApi = await CloudSkillApi.Instance;
                Learner learner = await cloudSkillApi.GetLearner(contestId, userName);

                if (learner == null)
                    throw new KeyNotFoundException($"Could not find the learner ({userName}) in this contest.");

                if (learner.ProgressPercentage == null)
                    throw new NullReferenceException($"The progress for this learner is null.");

                int progress = int.Parse(learner.ProgressPercentage);

                return new OkObjectResult(progress);
            }
            catch (Exception ex)
            {
                log.LogWarning($"GetLearnerProgress failed ({ex.Message})");
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
