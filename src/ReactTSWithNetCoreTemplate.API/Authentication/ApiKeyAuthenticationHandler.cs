using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Serilog;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ReactTSWithNetCoreTemplate.API.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthOptions>
    {
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder)
            : base(options, loggerFactory, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (string.IsNullOrEmpty(OptionsMonitor.CurrentValue.SecretToken))
            {
                Log.Error("API Key is not configured. Authentication cannot proceed.");
                return AuthenticateResult.Fail("API Key is not configured.");
            }

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                Log.Debug("Authorization header not found.");
                return AuthenticateResult.NoResult();
            }

            string authorizationHeader = Request.Headers["Authorization"];

            if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                Log.Warning("Authorization header does not start with 'Bearer '.");
                return AuthenticateResult.Fail("Invalid Authorization header scheme.");
            }

            string receivedToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (receivedToken == OptionsMonitor.CurrentValue.SecretToken)
            {
                Log.Information("API Key authentication successful.");

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "ApiClient"),
                    new Claim(ClaimTypes.Name, "ApiClient"),
                    new Claim(ClaimTypes.Role, "ApiConsumer")
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            else
            {
                Log.Warning("API Key authentication failed: Invalid token received.");
                return AuthenticateResult.Fail("Invalid API Key.");
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"{ApiKeyAuthOptions.SchemeName} realm=\"Your Realm\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
