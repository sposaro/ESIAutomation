using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

namespace CSCAutomateLib
{
    public class VaultApi
    {
        #region "Local Variables"
        private readonly SecretClient secretClient;
        #endregion

        #region "Public Methods"
        public VaultApi(string keyVaultName)
        {
            string kvUri = string.Format("https://{0}.vault.azure.net", keyVaultName);

            // TODO: Should cache SecertClient
            secretClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        }

        public async Task<string> GetSecretAync(string secretName)
        {
            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);
            return secret.Value;
        }
        #endregion
    }
}
