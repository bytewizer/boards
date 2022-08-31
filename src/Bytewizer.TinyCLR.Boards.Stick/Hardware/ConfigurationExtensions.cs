using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.Logging;

internal static class ConfigurationExtensions
{
    internal static LogLevel GetLogLevel(this IConfiguration configuration, string key)
    {
        if (configuration == null)
        {
            return LogLevel.Information;
        }

        string value = configuration[key] as string;

        if (value == null)
        {
            return LogLevel.Information;
        }

        switch (value.ToLower().Trim())
        {
            case "trace":
                return LogLevel.Trace;
            case "debug":
                return LogLevel.Debug;
            case "information":
                return LogLevel.Information;
            case "warning":
                return LogLevel.Warning;
            case "error":
                return LogLevel.Error;
            case "critical":
                return LogLevel.Critical;
            case "none":
                return LogLevel.None;
            default:
                return LogLevel.Information;
        }
    }
}