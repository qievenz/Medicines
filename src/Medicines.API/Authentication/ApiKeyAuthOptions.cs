using Microsoft.AspNetCore.Authentication;

namespace Medicines.API.Authentication
{
    public class ApiKeyAuthOptions : AuthenticationSchemeOptions
    {
        public const string SchemeName = "ApiKey";

        public string SecretToken { get; set; } = string.Empty;
    }
}
