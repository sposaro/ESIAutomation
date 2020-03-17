using CSCAutomateLib;
using System.Threading.Tasks;

namespace CSCAutomateUnitTest
{
    public static class TestHelper
    {
        #region "Public Test Methonds"
        public static string GetCSCAutomatekeyVaultName()
        {
            return "kv-cloudskillschallenge1";
        }
        public static string GetCSCAutomateEnvironmentType()
        {
            return "dev";
        }
        public static string GetCSCAutomateKeyVaultSecretName1()
        {
            return "cscApiKeyDev";
        }

        public static async Task<Configuration> GetConfigurationAsync()
        {
            Configuration config = await ConfigurationFactory.CreateConfigurationAsync(GetCSCAutomateEnvironmentType(), GetCSCAutomatekeyVaultName());
            return config;
        }
        #endregion
    }
}
