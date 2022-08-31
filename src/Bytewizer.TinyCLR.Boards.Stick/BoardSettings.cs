namespace Bytewizer.TinyCLR.Hosting
{
    public static partial class BoardSettings
    {
        // internal use only
        public static readonly string BoardType = "_board:type_";

        // public configuration settings
        public static readonly string TimeZoneOffset = "timezone:offset";
        public static readonly string MinimumLoggingLevel = "logging:log-level";
    }
}