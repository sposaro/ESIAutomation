using System;
using System.Threading.Tasks;

namespace CSCAutomateLib
{
    public static class ConfigurationFactory
    {
        #region "Constants"
        private const string EnvironmentTypeProd = "prod";
        private const string ApiKeyDev = "cscApiKeyDev";
        private const string ApiKeyProd = "cscApiKeyProd";
        private const string ApiRootDev = "cscApiRootDev";
        private const string ApiRootProd = "cscApiRootProd";
        private const string AutomateContainer1 = "cscAutomateContainer1";
        private const string AutomateStorageConn = "cscAutomateStorageConn";
        private const string AutomateQueue1 = "cscAutomateQueue1";
        #endregion

        #region "Public Methods"
        public static async Task<Configuration> CreateDevConfigurationAysnc()
        {
            string environmentType = Environment.GetEnvironmentVariable("CSCAutomateEnvironment", EnvironmentVariableTarget.Process);
            string keyVaultName = Environment.GetEnvironmentVariable("CSCApiKeyVaultName", EnvironmentVariableTarget.Process);

            return await ConfigurationFactory.CreateConfigurationAsync(environmentType, keyVaultName);
        }

        public static async Task<Configuration> CreateConfigurationAsync(string environmentType, string keyVaultName)
        {

            Configuration result = new Configuration
            {
                EnvironmentType = environmentType,
                KeyVaultName = keyVaultName
            };

            VaultApi vaultApi = new VaultApi(result.KeyVaultName);
            if (result.EnvironmentType != EnvironmentTypeProd)
            {
                result.ApiEndpoint = await vaultApi.GetSecretAync(ApiRootDev);
                result.ApiKey = await vaultApi.GetSecretAync(ApiKeyDev);
            }
            else
            {
                result.ApiEndpoint = await vaultApi.GetSecretAync(ApiRootProd);
                result.ApiKey = await vaultApi.GetSecretAync(ApiKeyProd);
            }

            result.QueueName1 = await vaultApi.GetSecretAync(AutomateQueue1);
            result.StorageConn1 = await vaultApi.GetSecretAync(AutomateStorageConn);
            result.StorageContainer1 = await vaultApi.GetSecretAync(AutomateContainer1);

            return result;
        }
        #endregion
    }
}
