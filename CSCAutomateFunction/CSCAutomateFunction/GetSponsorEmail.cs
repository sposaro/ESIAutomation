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
    public static class GetSponsorEmail
    {
        [FunctionName("GetSponsorEmail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string companyId = req.Query["companyId"];
                if (string.IsNullOrWhiteSpace(companyId))
                    throw new ArgumentNullException($"{nameof(companyId)} is blank.");

                string learningPath = req.Query["learningPath"];
                if (string.IsNullOrWhiteSpace(learningPath))
                    throw new ArgumentNullException($"{nameof(learningPath)} is blank.");

                BlobApi blobApi = await BlobApi.Instance;
                ContestResponse savedContest = await blobApi.GetContest(companyId, learningPath);
                string sponsorEmail = savedContest.MicrosoftAccountSponsor;

                return new OkObjectResult(sponsorEmail);
            }
            catch (Exception ex)
            {
                log.LogWarning($"Unable to get sponsor email. {ex}");
                return new BadRequestObjectResult($"{ex}");
            }
        }
    }
}
