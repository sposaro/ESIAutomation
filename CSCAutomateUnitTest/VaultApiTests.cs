using CSCAutomateUnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace CSCAutomateLib.Tests
{
    [TestClass()]
    public class VaultApiTests
    {
        #region "Public Test Methonds"
        [TestMethod()]
        public async Task GetSecretAyncTest()
        {
            string keyVaultName = TestHelper.GetCSCAutomatekeyVaultName();
            string secretName1 = TestHelper.GetCSCAutomateKeyVaultSecretName1();

            VaultApi vaultApi = new VaultApi(keyVaultName);
            string result = await vaultApi.GetSecretAync(secretName1);

            Assert.IsTrue(result.Trim().Length > 0);
        }
        #endregion
    }
}