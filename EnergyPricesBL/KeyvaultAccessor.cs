using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace EnergyPricesBL
{
    public class KeyvaultAccessor
    {
        private readonly string _connectionString;
        public KeyvaultAccessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> GetSecret(string value)
        {
            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                 }
            };
            var client = new SecretClient(new Uri(_connectionString), new DefaultAzureCredential(), options);
            var secret = await client.GetSecretAsync(value);
            return secret.Value.Value;
        }
    }
}
