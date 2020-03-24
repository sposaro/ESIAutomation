using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSCAutomateLib
{
    /// <summary>
    /// This class Wraps the Cloud Skills Challenge Api
    /// </summary>
    public class CloudSkillApi
    {
        #region "Local Variables"
        private readonly HttpClient httpClient;
        private string apiRoot = string.Empty;
        #endregion

        #region "Constants"

        private const string ApiPathContests = "/api/contests/";
        private const string ApiMethodLearners = "/learners/";
        private const string MediaTypeJson = "application/json";
        private const string ContestPrefix = "CurrentContest_";
        private const string Https = "https";
        private const string DateFormat = "MMMDDYYYY";
        private const string ApiKeyName = "Ocp-Apim-Subscription-Key";
        #endregion

        #region "Constructor"
        public CloudSkillApi(Configuration config)
        {
            if (string.IsNullOrEmpty(config.ApiKey.Trim()))
            {
                throw new ArgumentException("apiKey can't be null or empty");
            }
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(ApiKeyName, config.ApiKey);
            apiRoot = config.ApiEndpoint;
        }
        #endregion


        #region "Public Methonds"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobApi"></param>
        /// <param name="challengeRequestJson"></param>
        /// <returns></returns>
        public async Task<List<ContestResponse>> CreateChallengesAsyc(BlobApi blobApi, string challengeRequestJson)
        {
            ChallengeRequest challengeRequest = ContestFactory.CreateChallengeRequest(challengeRequestJson);
            List<ContestResponse> contestReponseList = await CreateCollectionChallengesAsync(
                challengeRequest.LearningPaths,
                challengeRequest.BaseInputs[0]);

            string json = JsonConvert.SerializeObject(contestReponseList);
            string fileName = $"{ContestPrefix}{DateTime.Now.ToString(DateFormat)}_{Guid.NewGuid().ToString()}";
            await blobApi?.UploadToBlobAsync(json, fileName);

            return contestReponseList;
        }

        /// <summary>
        /// Makes get call to fetch contest.
        /// </summary>
        /// <param name="contestId">The id of the contest to fetch</param>
        /// <returns>The contest response if value. Throw exception otherwise</returns>
        public async Task<ContestResponse> GetContestStatus(string contestId)
        {
            string uri = string.Concat(apiRoot, ApiPathContests, contestId);
            var response = await httpClient.GetAsync(uri);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            ContestResponse result = JsonConvert.DeserializeObject<ContestResponse>(jsonResponse);
            return result;
        }

        /// <summary>
        /// This method adds a learner to a contest.
        /// </summary>
        /// <param name="contestId">The ContestId to add to.</param>
        /// <param name="learnId">The MSLearn username to add to the contest</param>
        /// <returns>The result from the server.</returns>
        public async Task<string> AddLearner(string contestId, string learnId)
        {
            string uri = string.Concat(apiRoot, ApiPathContests, contestId, ApiMethodLearners, learnId);
            var response = await httpClient.PostAsync(uri,
                new StringContent(string.Empty, Encoding.UTF8, MediaTypeJson));
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }

        /// <summary>
        /// Gets current contests from Blob Storage
        /// </summary>
        /// <param name="blobApi"></param>
        /// <returns></returns>
        public static async Task<List<ContestResponse>> GetCurrentContestAsync(BlobApi blobApi)
        {
            List<BlobItem> blobs = blobApi.GetBlobItems(ContestPrefix);

            if (blobs.Count == 0)
                return null;

            string blobName = blobs[0].Name;

            return await blobApi.GetBlobContentsAync<List<ContestResponse>>(blobName);
        }

        /// <summary>
        /// Creates a challenge for each collection based on template
        /// </summary>
        /// <param name="learningPaths">The list of collections to be used in the template.</param>
        /// <param name="request">The template to be used to created the contest.</param>
        /// <returns>The ContestResponse.</returns>
        public async Task<List<ContestResponse>> CreateCollectionChallengesAsync(IList<LearningPath> learningPaths, ContestRequest request)
        {
            var results = new List<ContestResponse>();

            foreach (LearningPath lp in learningPaths)
            {
                request.CollectionUrl = lp.CollectionUrl;
                request.CollectionName = lp.CollectionName;
                request.CollectionID = lp.GetCollectionId();
                if (request.CollectionUrl.ToLower().StartsWith(Https))
                {
                    ContestResponse result = await CreateChallengeAsync(request);
                    results.Add(result);
                }
            }

            return results;
        }
        #endregion

        #region "Private Methods"        
        /// <summary>
        /// Creates a Cloud Skills Challenge based on the parameter.
        /// </summary>
        /// <param name="contest">The argurments to pass to the server.</param>
        /// <returns>The ContestResponse from the server.</returns>
        private async Task<ContestResponse> CreateChallengeAsync(ContestRequest contest)
        {
            string uri = string.Concat(apiRoot, ApiPathContests);
            string json = JsonConvert.SerializeObject(contest);
            try
            { 
                HttpResponseMessage response = await httpClient.PostAsync(uri,
                    new StringContent(json, Encoding.UTF8, MediaTypeJson));
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ContestResponse result = JsonConvert.DeserializeObject<ContestResponse>(jsonResponse);
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Error: {0}; Request details: {1}", ex.Message, json));
                throw;
            }
        }
        #endregion
    }
}

