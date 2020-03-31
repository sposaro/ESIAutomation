using CSCAutomateLib;
using CSCAutomateLib.Models;
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
        private static async Task<IList<ContestResponse>> CreateAndSaveChallengesAsync(string json, ILogger logger)
        {
            logger.LogInformation($"C# CreateChallenges function processing async: {json}");

            ChallengeRequest request = ContestFactory.CreateChallengeRequest(json);

            BlobApi blobApi = await BlobApi.Instance;
            CloudSkillApi cscApi = await CloudSkillApi.Instance;

            Tuple<IList<ContestResponse>,string> tuple = await blobApi.GetAllContestTupleAsync(request.BaseInputs.Mstpid);
            if (tuple?.Item1 != null)
            {
                request.LearningPaths = RemoveDuplicateLearningPaths(request, tuple.Item1);
                await blobApi.DeleteBlobAsync(tuple.Item2);
            }

            List<ContestResponse> response = await cscApi.CreateChallengesAsyc(request);
            logger.LogInformation($"Created the Challenges. Saving response to blob");

            if (tuple?.Item1 != null)
            {
                foreach (ContestResponse contest in tuple.Item1)
                {
                    response.Add(contest);
                }
            }

            await WriteToBlobAsync(request.BaseInputs, response);

            logger.LogInformation($"C# CreateChallenges function processed");

            return response;
        }

        private static async Task WriteToBlobAsync(ContestRequest request, IList<ContestResponse> contests)
        {
            BlobApi blobApi = await BlobApi.Instance;

            // Writes the blob to storage
            BlobContents contents = new BlobContents
            {
                BaseInputs = request,
                Contests = contests
            };
            string jsonString = JsonConvert.SerializeObject(contents);
            string fileName = $"{request.Mstpid}-{DateTime.Now:yyyy-MM}-{Guid.NewGuid()}";
            await blobApi.UploadToBlobAsync(jsonString, fileName);
        }

        private static IList<LearningPath> RemoveDuplicateLearningPaths(ChallengeRequest request, IList<ContestResponse> currentContests)
        {
            List<LearningPath> filteredPaths = new List<LearningPath>();
            HashSet<string> collectionHash = GetCollectionNames(currentContests);

            foreach(LearningPath path in request.LearningPaths)
            {
                if (!collectionHash.Contains(path.CollectionName))
                {
                    filteredPaths.Add(path);
                }
            }

            return filteredPaths;
        }

        private static HashSet<string> GetCollectionNames (IList<ContestResponse> contests)
        {
            HashSet<string> hash = new HashSet<string>();
            foreach (ContestResponse contest in contests)
            {
                hash.Add(contest.CollectionName);
            }
            return hash;
        }

        #endregion
    }
}
