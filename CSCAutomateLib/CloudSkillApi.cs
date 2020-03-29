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
        private const string ApiKeyName = "Ocp-Apim-Subscription-Key";
        private const string ContestNameTag = "LearningPath";
        private const string ContestDateFormat = "MMMM";
        private const string MsLearnUriPrefix = "https://docs.microsoft.com/";
        #endregion

        #region "Constructor"
        public static AsyncLazy<CloudSkillApi> Instance = new AsyncLazy<CloudSkillApi>(async () =>
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
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<ContestResponse>> CreateChallengesAsyc(ChallengeRequest request)
        {
            if (request == null || request.BaseInputs == null || request.LearningPaths == null)
                throw new ArgumentNullException("Challenge request does not contain required fields");

            List<ContestResponse> contestReponseList = await CreateCollectionChallengesAsync(
                request.LearningPaths,
                request.BaseInputs);

            return contestReponseList;
        }

        /// <summary>
        /// Return the progress precentage of the user
        /// </summary>
        /// <param name="contestId">The contestId</param>
        /// <param name="userName">The mslearnid</param>
        /// <returns>T
        ///     The progress percentage of the learning path the user completed.
        ///     -1 if the user was not found in the learning path.
        /// </returns>
        public async Task<int> GetUserProgressAsync(string contestId, string userName)
        {
            ContestResponse currentContest = await GetContestAsync(contestId);

            foreach (Learner learner in currentContest.Learners)
            {
                if (learner.UserName.ToLower() == userName.ToLower())
                {
                    return int.Parse(learner.ProgressPercentage);
                }
            }

            return -1;
        }

        /// <summary>
        /// This method adds a learner to a contest.
        /// </summary>
        /// <param name="request">Request Body</param>
        /// <returns>The result from the server.</returns>
        public async Task<Learner> AddLearnerAsync(string contestId, string learnerId)
        {
            if (string.IsNullOrWhiteSpace(contestId))
                throw new ArgumentNullException($"{nameof(contestId)} is blank.");
            else if (string.IsNullOrWhiteSpace(learnerId))
                throw new ArgumentNullException($"{nameof(learnerId)} is blank.");

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
        #endregion

        #region "Private Methods"
        /// <summary>
        /// Makes get call to fetch contest.
        /// </summary>
        /// <param name="contestId">The id of the contest to fetch</param>
        /// <returns>The contest response if value. Throw exception otherwise</returns>
        private async Task<ContestResponse> GetContestAsync(string contestId)
        {
            if (string.IsNullOrWhiteSpace(contestId))
                throw new ArgumentNullException($"{nameof(contestId)} is blank");

            string uri = string.Concat(apiRoot, ApiPathContests, contestId);
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Failed http request");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            ContestResponse result = JsonConvert.DeserializeObject<ContestResponse>(jsonResponse);
            return result;
        }

        /// <summary>
        /// Creates a challenge for each collection based on template
        /// </summary>
        /// <param name="learningPaths">The list of collections to be used in the template.</param>
        /// <param name="request">The template to be used to created the contest.</param>
        /// <returns>The ContestResponse.</returns>
        private async Task<List<ContestResponse>> CreateCollectionChallengesAsync(IList<LearningPath> learningPaths, ContestRequest request)
        {
            if (learningPaths == null)
                throw new ArgumentNullException($"{nameof(learningPaths)} is null");

            var results = new List<ContestResponse>();
            request.Name = $"{request.Name} | {DateTime.Now.ToString(ContestDateFormat)} {ContestNameTag}";

            foreach (LearningPath lp in learningPaths)
            {
                request.CollectionUrl = lp.CollectionUrl;
                request.CollectionName = lp.CollectionName;
                request.CollectionID = lp.GetCollectionId();

                if (!string.IsNullOrWhiteSpace(request.CollectionName) &&
                    request.CollectionUrl.StartsWith(MsLearnUriPrefix) &&
                    !string.IsNullOrWhiteSpace(request.CollectionName))
                {
                    ContestResponse result = await CreateChallengeAsync(request);
                    results.Add(result);
                }
            }

            return results;
        }

        /// <summary>
        /// Creates a Cloud Skills Challenge based on the parameter.
        /// </summary>
        /// <param name="contest">The argurments to pass to the server.</param>
        /// <returns>The ContestResponse from the server.</returns>
        private async Task<ContestResponse> CreateChallengeAsync(ContestRequest contest)
        {
            Uri uri = new Uri ($"{apiRoot}{ApiPathContests}");
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

