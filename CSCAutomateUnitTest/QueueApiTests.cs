using Azure.Storage.Queues.Models;
using CSCAutomateUnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        #endregion
    }
}