using System;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CSCAutomate
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task Main(string[] args)
        {
            string response = await CreateChallengeAsync(ContestTests.TestContest);

            Console.Out.WriteLine(response);

            Console.In.Read();
        }

        public static async Task<string> CreateChallengeAsync(Contest contest)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            string API_KEY = "<Omitted>";
            string uri = "<Omitted>" + queryString;
            HttpResponseMessage response;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", API_KEY);

            // Request body
            string json = JsonConvert.SerializeObject(contest);

            // Make Call
            var responseData = await client.PostAsync(
                                        uri,
                                        new StringContent(json, Encoding.UTF8, "application/json"));

            return "called";
        }
    }
}
