using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
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
        private const string Https = "https";
        private const string ApiKeyName = "Ocp-Apim-Subscription-Key";
        private const string ContestNameTag = "| Exam";
        #endregion

        #region "Constructor"
        public static AsyncLazy<CloudSkillApi> CloudSkillsApiInstance = new AsyncLazy<CloudSkillApi>(async () =>
            new CloudSkillApi(await ConfigurationFactory.CreateDevConfigurationAysnc()));

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
        /// CreateChallengesAsync
        /// </summary>
        /// <param name="challengeRequestJson"></param>
        /// <returns></returns>
        public async Task<List<ContestResponse>> CreateChallengesAsyc(string challengeRequestJson)
        {
            ChallengeRequest challengeRequest = ContestFactory.CreateChallengeRequest(challengeRequestJson);
            List<ContestResponse> contestReponseList = await CreateCollectionChallengesAsync(
                challengeRequest.LearningPaths,
                challengeRequest.BaseInputs);

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
        /// <param name="request">Request Body</param>
        /// <returns>The result from the server.</returns>
        public async Task<Learner> AddLearnerAsync(string contestId, string learnerId)
        {
            string uri = string.Concat(apiRoot, ApiPathContests, contestId, ApiMethodLearners, learnerId);
            var response = await httpClient.PostAsync(uri,
                new StringContent(string.Empty, Encoding.UTF8, MediaTypeJson));

            string resultMessage = await response.Content.ReadAsStringAsync();
            
            // Not a valid MSLearn ID
            // Server send a valid message
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new KeyNotFoundException(resultMessage);

            // The is likley a duplicate request
            if (response.StatusCode == HttpStatusCode.InternalServerError)
                throw new DuplicateNameException($"The MSLearn Username '{learnerId}' is likely already registerd for this path. {resultMessage}");

            // Unknown issue occured
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"An unknown server error as occured. {resultMessage}");

            return JsonConvert.DeserializeObject<Learner>(resultMessage);
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
            request.Name = $"{request.Name} {ContestNameTag}";

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

