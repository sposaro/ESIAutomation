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
            List<ContestResponse> contestReponseList = await cscApi.CreateChallengesAsyc(blobApi, challengeRequestJson);

            Assert.IsTrue(contestReponseList.Count > 0);
        }        
        #endregion
    }
}