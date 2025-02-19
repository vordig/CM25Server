﻿using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CM25Server.Infrastructure.Core.Options;
using CM25Server.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace CM25Server.WebApi.ApiVersioning;

internal static class OpenApiOptionsExtensions
{
    public static OpenApiOptions ApplyApiVersionInfo(this OpenApiOptions options, string title, string description,
        ICollection<ApiVersion> supportedApiVersions, ICollection<ApiVersion> deprecatedApiVersions)
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            var versionedDescriptionProvider = context.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            var apiDescription = versionedDescriptionProvider?.ApiVersionDescriptions
                .SingleOrDefault(description => description.GroupName == context.DocumentName);
            if (apiDescription is null)
            {
                return Task.CompletedTask;
            }

            document.Info.Version = apiDescription.ApiVersion.ToString();
            document.Info.Title = title;
            document.Info.Description =
                BuildDescription(apiDescription, description, supportedApiVersions, deprecatedApiVersions,
                    versionedDescriptionProvider);
            return Task.CompletedTask;
        });
        return options;
    }

    private static string BuildDescription(ApiVersionDescription api, string description,
        ICollection<ApiVersion> supportedApiVersions, ICollection<ApiVersion> deprecatedApiVersions,
        IApiVersionDescriptionProvider versionedDescriptionProvider)
    {
        var text = new StringBuilder(description);

        if (api.IsDeprecated)
        {
            if (text.Length > 0)
            {
                if (text[^1] != '.')
                {
                    text.Append('.');
                }

                text.Append(' ');
            }

            text.Append("This API version has been deprecated.");
        }

        if (api.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
            {
                if (text.Length > 0)
                {
                    if (text[^1] != '.')
                    {
                        text.Append('.');
                    }

                    text.Append(' ');
                }

                text.Append("The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                var rendered = false;

                foreach (var link in policy.Links.Where(l => l.Type == "text/html"))
                {
                    if (!rendered)
                    {
                        text.Append("<h4>Links</h4><ul>");
                        rendered = true;
                    }

                    text.Append("<li><a href=\"");
                    text.Append(link.LinkTarget.OriginalString);
                    text.Append("\">");
                    text.Append(
                        StringSegment.IsNullOrEmpty(link.Title)
                            ? link.LinkTarget.OriginalString
                            : link.Title.ToString());
                    text.Append("</a></li>");
                }

                if (rendered)
                {
                    text.Append("</ul>");
                }
            }
        }

        if (supportedApiVersions.Count != 0)
        {
            text.Append("<h4>Supported API versions</h4><ul>");

            foreach (var apiVersion in supportedApiVersions)
            {
                var formatedApiVersion = $"v{apiVersion.ToString()}";
                text.Append("<li><a href=\"");
                text.Append(formatedApiVersion);
                text.Append("\">");
                text.Append(formatedApiVersion);

                var apiDescription = versionedDescriptionProvider?.ApiVersionDescriptions
                    .SingleOrDefault(apiVersionDescription => apiVersionDescription.GroupName == formatedApiVersion);
                if (apiDescription?.SunsetPolicy is { Date: { } when })
                    text.Append(" - will be sunset on ")
                        .Append(when.Date.ToShortDateString());

                text.Append("</a></li>");
            }

            text.Append("</ul>");
        }

        if (deprecatedApiVersions.Count != 0)
        {
            text.Append("<h4>Deprecated API versions</h4><ul>");

            foreach (var apiVersion in deprecatedApiVersions.Select(x => $"v{x.ToString()}"))
            {
                text.Append("<li><a href=\"");
                text.Append(apiVersion);
                text.Append("\">");
                text.Append(apiVersion);
                text.Append("</a></li>");
            }

            text.Append("</ul>");
        }

        return text.ToString();
    }

    public static OpenApiOptions ApplySecuritySchemeDefinitions(this OpenApiOptions options)
    {
        options.AddDocumentTransformer<SecuritySchemeDefinitionsTransformer>();
        return options;
    }

    public static OpenApiOptions ApplyAuthorizationChecks(this OpenApiOptions options, string[] scopes)
    {
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            if (!metadata.OfType<IAuthorizeData>().Any())
            {
                return Task.CompletedTask;
            }

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            var oAuthScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [oAuthScheme] = scopes
                }
            };

            return Task.CompletedTask;
        });
        return options;
    }

    public static OpenApiOptions ApplyOperationDeprecatedStatus(this OpenApiOptions options)
    {
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            var apiDescription = context.Description;
            operation.Deprecated |= apiDescription.IsDeprecated();
            return Task.CompletedTask;
        });
        return options;
    }

    private static IOpenApiAny? CreateOpenApiAnyFromObject(object value)
    {
        return value switch
        {
            bool b => new OpenApiBoolean(b),
            int i => new OpenApiInteger(i),
            double d => new OpenApiDouble(d),
            string s => new OpenApiString(s),
            _ => null
        };
    }

    private class SecuritySchemeDefinitionsTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var authOptionsSection = configuration.GetSection(AuthOptions.SectionName);
            if (!authOptionsSection.Exists())
            {
                return Task.CompletedTask;
            }

            var securityScheme = new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes.Add("Bearer", securityScheme);
            return Task.CompletedTask;
        }
    }
}