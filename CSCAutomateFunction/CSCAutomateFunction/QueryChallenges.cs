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
                    throw new ArgumentNullException("Expecting customerId in query param. Recieved empty string.");

                BlobApi blobApi = await BlobApi.Instance;
                IList<ContestResponse> activeContests = await blobApi.GetContestResponseAsync(tpid);

                if (activeContests == null)
                    throw new KeyNotFoundException($"Customer Code ({tpid}) not found.");

                string response = activeContests.GetCollectionString();

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in CreateChallenges: {0}", ex.Message));
                return new BadRequestObjectResult(ex.Message);
            }
        }
        #endregion

        public static string GetCollectionString(this IList<ContestResponse> contestList)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ContestResponse contest in contestList)
            {
                builder.AppendLine($"  {contest.CollectionName}  ");
                builder.AppendLine($"  ({contest.CollectionUrl})  \n  ");
            }
            return builder.ToString();
        }
    }
}
