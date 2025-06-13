using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Medicines.API.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Medicines.API.Swagger
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAllowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<IAllowAnonymous>()
                .Any();

            if (hasAllowAnonymous)
            {
                return;
            }

            var hasAuthorize = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<IAuthorizeData>()
                .Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = ApiKeyAuthOptions.SchemeName
                    }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [securityScheme] = new List<string>()
                    }
                };
            }
        }
    }
}
