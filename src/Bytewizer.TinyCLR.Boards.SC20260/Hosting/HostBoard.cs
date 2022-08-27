﻿using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.Hosting.Configuration;
using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Logging.Debug;

using GHIElectronics.TinyCLR.Pins;

namespace Bytewizer.TinyCLR.Boards
{
    /// <summary>
    /// Provides convenience methods for creating instances of <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBoard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostBuilder"/>.
        /// </summary>
        /// <returns>The initialized <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder CreateDefaultBuilder()
        {
            var host = Host.CreateBuilder()
                .ConfigureServices((context, services) =>
                {
                    context.Configuration[BoardSettings.BoardType] = typeof(SC20260);
                    context.Configuration[BoardSettings.NetworkConnected] = false;

                    services.AddClock(
                        (int)context.Configuration.GetOrDefault("timezone:offset", 0)
                    );
                    services.AddLogging(builder =>
                    {
                        builder.AddDebug();
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