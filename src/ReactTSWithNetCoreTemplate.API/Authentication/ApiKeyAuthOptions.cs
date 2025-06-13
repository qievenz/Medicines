using Microsoft.AspNetCore.Authentication;

namespace ReactTSWithNetCoreTemplate.API.Authentication
{
    public class ApiKeyAuthOptions : AuthenticationSchemeOptions
    {
        public const string SchemeName = "ApiKey";

        public string SecretToken { get; set; } = string.Empty;
    }
}
