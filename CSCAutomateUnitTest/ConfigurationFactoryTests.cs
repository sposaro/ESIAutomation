using CSCAutomateUnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;


namespace CSCAutomateLib.Tests
{
    [TestClass()]
    public class ConfigurationFactoryTests
    {
        #region "Public Test Methonds"
        [TestMethod()]
        public async Task CreateConfigurationAsyncTestAsync()
        {
            Configuration config = await ConfigurationFactory.CreateConfigurationAsync(
                TestHelper.GetCSCAutomateEnvironmentType(), 
                TestHelper.GetCSCAutomatekeyVaultName());

            Assert.IsNotNull(config);
        }
        #endregion
    }
}