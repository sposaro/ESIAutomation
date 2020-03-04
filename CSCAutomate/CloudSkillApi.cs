using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSCAutomate
{
    /// <summary>
    /// This class Wraps the Cloud Skills Challenge Apis (https://csc-praxeum-dev-apimgt.portal.azure-api.net/)
    /// </summary>
    public class CloudSkillApi
    {
        private readonly HttpClient httpClient;
        private readonly string END_POINT = "https://csc-praxeum-dev-apimgt.azure-api.net";
        
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="apiKey">Your assigned API Key</param>
        public CloudSkillApi(string apiKey)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
        }

        /// <summary>
        /// Makes get call to fetch contest.
        /// </summary>
        /// <param name="contestId">The id of the contest to fetch</param>
        /// <returns>The contest response if value. Throw exception otherwise</returns>
        public async Task<ContestResponse> GetContestStatus(string contestId)
        {
            string uri = END_POINT + $"/api/contests/{contestId}";

            var response = await httpClient.GetAsync(uri);

            string jsonResponse = await response.Content.ReadAsStringAsync();

            ContestResponse result = JsonConvert.DeserializeObject<ContestResponse>(jsonResponse);

            return result;
        }

        /// <summary>
        /// Creates a challenge for each collection based on template
        /// </summary>
        /// <param name="collections">The list of collections to be used in the template.</param>
        /// <param name="contestTemplate">The template to be used to created the contest.</param>
        /// <returns>The ContestResponse.</returns>
        public async Task<List<ContestResponse>> CreateCollectionChallengesAsync(List<string> collections, ContestRequest contestTemplate)
        {
            contestTemplate.Type = "Collection";
            var results = new List<ContestResponse>();

            foreach (string s in collections)
            {
                contestTemplate.CollectionUrl = s;
                contestTemplate.CollectionID = s.Substring(s.LastIndexOf('/') + 1);
                ContestResponse result = await CreateChallengeAsync(contestTemplate);
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// This method adds a learner to a contest.
        /// </summary>
        /// <param name="contestId">The ContestId to add to.</param>
        /// <param name="learnId">The MSLearn username to add to the contest</param>
        /// <returns>The result from the server.</returns>
        public async Task<string> AddLearner(string contestId, string learnId)
        {
            var uri = END_POINT + $"/api/contests/{contestId}/learners/{learnId}";

            var response = await httpClient.PostAsync(uri,
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            string result = await response.Content.ReadAsStringAsync();

            return result;
        }

        /// <summary>
        /// Creates a Cloud Skills Challenge based on the parameter.
        /// </summary>
        /// <param name="contest">The argurments to pass to the server.</param>
        /// <returns>The ContestResponse from the server.</returns>
        public async Task<ContestResponse> CreateChallengeAsync(ContestRequest contest)
        {
            string uri = END_POINT + $"/api/contests";

            string json = JsonConvert.SerializeObject(contest);

            HttpResponseMessage response = await httpClient.PostAsync(uri,
                new StringContent(json, Encoding.UTF8, "application/json"));

            string jsonResponse = await response.Content.ReadAsStringAsync();

            ContestResponse result = JsonConvert.DeserializeObject<ContestResponse>(jsonResponse);

            return result;
        }
    }
}
