using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.Hosting.Configuration;
using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Logging.Debug;

using GHIElectronics.TinyCLR.Pins;

namespace Bytewizer.TinyCLR.Boards
{
    public static class HostBoard
    {
        public static IHostBuilder CreateDefaultBuilder()
        {
            var host = Host.CreateBuilder()
                .ConfigureServices((context, services) =>
                {
                    context.Configuration[BoardSettings.BoardType] = typeof(FEZPortal);

                    services.AddClock(
                        (int)context.Configuration.GetValueOrDefault(BoardSettings.TimeZoneOffset, 0)
                    );
                    services.AddLogging(builder =>
                    {
                        builder.AddDebug();
                        builder.SetMinimumLevel(
                                context.Configuration.GetLogLevel(BoardSettings.MinimumLogLevel)
                            );
                    });
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddInMemoryCollection();
                });

            return host;
        }
    }
}