using System;
using System.Net;

using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.Logging;

internal static class ConfigurationExtensions
{
    internal static IPAddress GetIpAddress(this IConfiguration configuration, string key)
    {
        if (configuration == null)
        {
            return null;
        }

        string value = configuration[key] as string;

        if (value == null)
        {
            return null;
        }

        return IPAddress.Parse(value);
    }

    internal static IPAddress[] GetDnsAddresses(this IConfiguration configuration, string key)
    {
        if (configuration == null)
        {
            return null;
        }

        string value = configuration[key] as string;

        if (value == null)
        {
            return null;
        }

        string[] array = value.Split(',');
        IPAddress[] addresses = new IPAddress[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            addresses[i] = IPAddress.Parse(array[i].Trim());
        }

        return addresses;
    }

    internal static byte[] GetMacAddress(this IConfiguration configuration, string key)
    {
        if (configuration == null)
        {
            return null;
        }

        string value = configuration[key] as string;
        
        if (value == null)
        {
            return null;
        }

        string[] array = value.Split('-');
        byte[] bytes = new byte[6];

        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)Convert.ToInt32(array[i].Trim(), 16);
        }

        return bytes;
    }

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