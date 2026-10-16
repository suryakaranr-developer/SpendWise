using System.Reflection;

namespace SpendWise.App.Services;

/// <summary>Which flavour this binary was built as (QA, STAGING or RELEASE), baked in by Build/Environments.props.</summary>
public static class BuildInfo
{
    public static string Environment { get; } =
        typeof(BuildInfo).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "AppEnvironment")?.Value ?? "RELEASE";

    public static bool IsProduction => Environment == "RELEASE";

    public static string Label => $"{Environment} · v{AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";
}
