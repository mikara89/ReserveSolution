
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Team.Api.Options 
{
    public class AuthenticationRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Security == null)
                operation.Security = new List<OpenApiSecurityRequirement>();

            operation.Responses.Add("201", new OpenApiResponse { Description = "Created" });
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            operation.Responses.Add("400", new OpenApiResponse { Description = "BadRequest" });
            operation.Responses.Add("500", new OpenApiResponse { Description = "Internal Server error" });

            ///OAUTH2
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme {Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"}
                        }
                    ] = new List<string>()
                }
            };
            ///API KEY
            var schemeAPIKey = new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = ApiKeyAuthenticationOptions.DefaultScheme 
                } 
            };
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [schemeAPIKey] = new List<string>()
            });

        }
    }
}
