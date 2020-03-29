using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CSCAutomateLib;

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
                string companyId = req.Query["companyId"];
                string learningPath = req.Query["learningPath"];

                BlobApi blobApi = await BlobApi.Instance;
                ContestResponse savedContest = await blobApi.GetContest(companyId, learningPath);

                CloudSkillApi cloudSkillApi = await CloudSkillApi.Instance;
                int progress = await cloudSkillApi.GetUserProgressAsync(savedContest.ContestId, userName);

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
