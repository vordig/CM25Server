﻿using System.Text.RegularExpressions;
using Asp.Versioning;

namespace CM25Server.WebApi.ApiVersioning;

public partial class ApiVersioningOptions
{
    [GeneratedRegex(@"(?<Major>\d+)(.(?<Minor>\d+))?(-(?<Status>\w+))?")]
    private static partial Regex ApiVersionTemplate();

    public const string ApiVersions = "ApiVersions";

    private ICollection<ApiVersion>? _supportedApiVersions;
    private ICollection<ApiVersion>? _deprecatedApiVersions;
    private IEnumerable<SunsetPolicy>? _sunsetPolicies;

    public IEnumerable<string> Supported { get; set; } = [];
    public IEnumerable<string> Deprecated { get; set; } = [];
    public IEnumerable<SunsetOptions> Sunset { get; set; } = [];

    public ICollection<ApiVersion> SupportedApiVersions => _supportedApiVersions ??= GetApiVersions(Supported);
    public ICollection<ApiVersion> DeprecatedApiVersions => _deprecatedApiVersions ??= GetApiVersions(Deprecated);

    public IEnumerable<SunsetPolicy> SunsetPolicies => _sunsetPolicies ??= GetSunsetPolicies();

    public string NewestActiveApiVersion => SupportedApiVersions.Last().ToString();

    public ICollection<string> AvailableApiVersions =>
    [
        .. SupportedApiVersions.Select(x => $"v{x.ToString()}"),
        .. DeprecatedApiVersions.Select(x => $"v{x.ToString()}")
    ];

    private static ICollection<ApiVersion> GetApiVersions(IEnumerable<string> apiVersions) =>
        apiVersions.Select(GetApiVersion).OrderDescending().ToList();

    private static ApiVersion GetApiVersion(string apiVersion)
    {
        var match = ApiVersionTemplate().Match(apiVersion);

        if (!match.Success) throw new ApplicationException($"Invalid ApiVersion {apiVersion}");

        var majorVersion = int.Parse(match.Groups["Major"].Value);
        int? minorVersion = int.TryParse(match.Groups["Minor"].Value, out var minor) ? minor : default;
        var versionStatus = match.Groups["Status"].Value;

        return new ApiVersion(majorVersion, minorVersion, versionStatus);
    }

    private IEnumerable<SunsetPolicy> GetSunsetPolicies() => Sunset.Select(sunsetOptions => new SunsetPolicy
    {
        ApiVersion = GetApiVersion(sunsetOptions.ApiVersion), 
        Effective = DateTime.SpecifyKind(sunsetOptions.Effective, DateTimeKind.Utc)
    }).ToList();

    public class SunsetOptions
    {
        public string ApiVersion { get; set; } = string.Empty;
        public DateTime Effective { get; set; }
    }

    public class SunsetPolicy
    {
        public required ApiVersion ApiVersion { get; init; }
        public required DateTime Effective { get; init; }
    }
}