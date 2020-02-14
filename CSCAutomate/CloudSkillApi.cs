using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSCAutomate
{
    public class CloudSkillApi
    {
        private readonly HttpClient httpClient;
        private readonly string END_POINT = "https://csc-praxeum-dev-apimgt.azure-api.net";
        
        public CloudSkillApi(string apiKey)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
        }

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

        public async Task<string> AddLearner(string contestId, string learnId)
        {
            var uri = END_POINT + $"/api/contests/{contestId}/learners/{learnId}";

            var response = await httpClient.PostAsync(uri,
                new StringContent(string.Empty, Encoding.UTF8, "application/json"));

            string result = await response.Content.ReadAsStringAsync();

            return result;
        }

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
