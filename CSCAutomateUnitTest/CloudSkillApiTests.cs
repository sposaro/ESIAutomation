using CSCAutomateLib;
using CSCAutomateUnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSCAutomateLib.Tests
{
    [TestClass()]
    public class CloudSkillApiTests
    {
        #region "Public Test Methods"
        [TestMethod()]
        public async Task CreateChallengesAsycTestAsync()
        {
            Configuration config = await TestHelper.GetConfigurationAsync();
            BlobApi blobApi = new BlobApi(config);
            CloudSkillApi cscApi = new CloudSkillApi(config);

            string challengeRequestJson = ContestFactory.CreateChallengeRequestJson();
            List<ContestResponse> contestReponseList = await cscApi.CreateChallengesAsync(blobApi, challengeRequestJson);

            Assert.IsTrue(contestReponseList.Count > 0);
        }

        [TestMethod()]
        public async Task AddLearnerAsyncTest()
        {
            Configuration config = await TestHelper.GetConfigurationAsync();
            CloudSkillApi cscApi = new CloudSkillApi(config);

            string learnerRequestJson = ContestFactory.CreateLearnerRequestJson();
            string learnerReponse = await cscApi.AddLearnerAsync(learnerRequestJson);

            Assert.IsTrue(learnerReponse.Length > 0);
        }
        #endregion
    }
}