﻿using System;
using System.Threading;

using Bytewizer.TinyCLR.DependencyInjection;

using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Hosting.Configuration;

namespace Bytewizer.TinyCLR.Boards
{
    public static class NetworkTimeServiceCollectionExtension
    {
        public static IServiceCollection AddNetworkTime(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            services.AddHostedService(typeof(NetworkTimeService));

            return services;
        }
    }

    public class NetworkTimeService : SchedulerService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ClockService _clock;

        public NetworkTimeService(IConfiguration configuration, ClockService clock)
             : this(NullLoggerFactory.Instance, configuration, clock)
        { }

        public NetworkTimeService(ILoggerFactory loggerFactory, IConfiguration configuration, ClockService clock)
            : base(TimeSpan.FromSeconds(20), TimeSpan.FromDays(1))
        {
            _logger = loggerFactory.CreateLogger(nameof(NetworkTimeService));
            _configuration = configuration;
            _clock = clock;
        }

        protected override void ExecuteAsync()
        {
            var wifiConnected = (bool)_configuration.GetOrDefault(BoardSettings.WirelessConnected, false);
            var ethernetConnected = (bool)_configuration.GetOrDefault(BoardSettings.EthernetConnected, false);

            if (wifiConnected || ethernetConnected)
            {
                try
                {
                    var accurateTime = GetNetworkTime();

                    _clock.SetTime(accurateTime);
                    _logger.Log(LogLevel.Information, $"Realtime clock set to {_clock.Now:MM/dd/yyyy hh:mm:ss tt} UTC from network time.");
                }
                catch
                {
                    _logger.Log(LogLevel.Information, "Failed getting network time.");
                }
            }
        }

        public static DateTime GetNetworkTime()
        {
            const string ntpServer = "pool.ntp.org";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;

            var addresses = System.Net.Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new System.Net.IPEndPoint(addresses[0], 123);
            var socket = new System.Net.Sockets.Socket(
                System.Net.Sockets.AddressFamily.InterNetwork,
                System.Net.Sockets.SocketType.Dgram,
                System.Net.Sockets.ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            Thread.Sleep(1);

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 |
                (ulong)ntpData[42] << 8 | (ulong)ntpData[43];

            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 |
                (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            var networkDateTime = (new System.DateTime(1900, 1, 1)).
                AddMilliseconds((long)milliseconds);

            return networkDateTime;
        }
    }
}