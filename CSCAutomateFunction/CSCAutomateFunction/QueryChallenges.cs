using CSCAutomateLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSCAutomateFunction
{
    public static class QueryChallenges
    {
        #region "Public Methods"
        [FunctionName("QueryChallengesByName")]
        public static async Task<IActionResult> RunQuery(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string tpid = req.Query["customerId"];

                if (string.IsNullOrWhiteSpace(tpid))
                {
                    string warningMessage = "Expecting customer number in post body. Recieved empty string.";
                    log.LogWarning(warningMessage);
                    return new BadRequestObjectResult(warningMessage);
                }

                BlobApi blobApi = await BlobApi.BlobApiInstance;
                List<ContestResponse> activeContests = await blobApi.GetActiveContest(tpid);

                if (activeContests == null)
                {
                    new BadRequestObjectResult($"Customer id ({tpid}) not found.");
                }

                StringBuilder builder = new StringBuilder();
                foreach (ContestResponse contest in activeContests)
                {
                    builder.Append($"{contest.CollectionName}, ");
                }
                builder.Remove(builder.Length - 2, 2);

                return new OkObjectResult(builder.ToString());
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in CreateChallenges: {0}", ex.Message));
                throw;
            }
        }
        #endregion
    }
}
