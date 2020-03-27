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

                BlobApi blobApi = await BlobApi.BlobApiInstance;
                List<ContestResponse> activeContests = await blobApi.GetAllContestByCustomerId(tpid);

                if (activeContests == null)
                    throw new KeyNotFoundException($"Customer Code ({tpid}) not found.");

                string response = GetCollectionString(activeContests);

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(string.Format("Error in CreateChallenges: {0}", ex.Message));
                return new BadRequestObjectResult(ex.Message);
            }
        }
        #endregion

        #region Private Methods
        private static string GetCollectionString(List<ContestResponse> contestList)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ContestResponse contest in contestList)
            {
                builder.Append($"{contest.CollectionName}, ");
            }
            builder.Remove(builder.Length - 2, 2);

            return builder.ToString();
        }
        #endregion
    }
}
