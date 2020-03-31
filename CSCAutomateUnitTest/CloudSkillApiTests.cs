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
            CloudSkillApi cscApi = new CloudSkillApi(config);

            string json = ContestFactory.CreateChallengeRequestJson();
            ChallengeRequest request = ContestFactory.CreateChallengeRequest(json);
            List<ContestResponse> contestReponseList = await cscApi.CreateChallengesAsyc(request);

            Assert.IsTrue(contestReponseList.Count > 0);
        }

        [TestMethod()]
        public async Task AddLearnerAsyncTest()
        {
            Configuration config = await TestHelper.GetConfigurationAsync();
            CloudSkillApi cscApi = new CloudSkillApi(config);

            //string learnerRequestJson = ContestFactory.CreateLearnerRequestJson();
            Learner response = await cscApi.AddLearnerAsync("b95e57ea-61d1-4844-935e-7477e05d8f6a", "SriniAmbati-6212");

            Assert.IsNotNull(response);
        }
        #endregion
    }
}