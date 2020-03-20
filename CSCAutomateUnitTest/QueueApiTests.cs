using CSCAutomateLib;
using Azure.Storage.Queues.Models;
using CSCAutomateUnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSCAutomateLib.Tests
{
    [TestClass()]
    public class QueueApiTests
    {
        #region "Public Test Methonds"
        [TestMethod()]
        public async Task SendQueueMessageAyncTestAsync()
        {
            Configuration config = await TestHelper.GetConfigurationAsync();
            QueueApi queueApi = new QueueApi(config);

            SendReceipt sr = await queueApi.SendQueueMessageAync(ContestFactory.CreateChallengeRequestJson());

            Assert.IsNotNull(sr);
        }

        [TestMethod()]
        public void TestMessageAyncTest()
        {
            string doubleQuote = "\"";
            string rawJsonMessage = "\\\"[{\\\"additionalInputs\\\":[{\\\"autoRecurring\\\":true,\\\"requestedVouchers\\\":true}],\\\"baseInputs\\\":[{\\\"EndDateStr\\\":\\\"03-31-2020 23:59:59\\\",\\\"StartDateStr\\\":\\\"03-18-2020 00:00:00\\\",\\\"accountType\\\":\\\"Customer\\\",\\\"allowTeams\\\":\\\"false\\\",\\\"challengeDescription\\\":\\\"Cloud Skills Challenge\\\",\\\"collectionUrl\\\":\\\"null\\\",\\\"country\\\":\\\"United States\\\",\\\"createdby\\\":\\\"datoront\\\",\\\"customCSS\\\":\\\"null\\\",\\\"eou\\\":\\\"null\\\",\\\"hasPrizes\\\":\\\"false\\\",\\\"microsoftAccountSponsor\\\":\\\"datoront\\\",\\\"mpnid\\\":\\\"null\\\",\\\"mstpid\\\":\\\"123456\\\",\\\"name\\\":\\\"Test\\\",\\\"participantType\\\":\\\"Customer\\\",\\\"selfRegistrationEnabled\\\":\\\"true\\\",\\\"teams\\\":\\\"null\\\",\\\"templateSelection\\\":\\\"theme0\\\",\\\"timeZone\\\":\\\"null\\\",\\\"type\\\":\\\"Growth\\\"}],\\\"learningPaths\\\":[{\\\"collectionName\\\":\\\"AZ-120\\\",\\\"collectionUrl\\\":\\\"Needs Collection URL\\\"},{\\\"collectionName\\\":\\\"AZ-203\\\",\\\"collectionUrl\\\":\\\"https://docs.microsoft.com/en-us/users/drfrank/collections/053c14o2gjx0j\\\"},{\\\"collectionName\\\":\\\"AZ-220\\\",\\\"collectionUrl\\\":\\\"Needs Collection URL\\\"},{\\\"collectionName\\\":\\\"AZ-300/301\\\",\\\"collectionUrl\\\":\\\"https://docs.microsoft.com/en-us/users/drfrank/collections/704b82r4o66g1\\\"}]}]\\\"";

            string jsonMessage = Regex.Unescape(rawJsonMessage);
            jsonMessage = jsonMessage.Remove(jsonMessage.IndexOf(doubleQuote), doubleQuote.Length);
            jsonMessage = jsonMessage.Remove(jsonMessage.LastIndexOf(doubleQuote));


            Assert.IsTrue(jsonMessage.Length != 0);
        }

        [TestMethod()]
        public async Task ReceiveQueueMessageAyncTestAsync()
        {
            Configuration config = await TestHelper.GetConfigurationAsync();
            QueueApi queueApi = new QueueApi(config);

            await queueApi.SendQueueMessageAync(ContestFactory.CreateChallengeRequestJson());

            Azure.Response<QueueMessage[]> response = await queueApi.ReceiveQueueMessageAync();

            Assert.IsNotNull(response);
        }
        #endregion


    }
}