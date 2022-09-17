using System;
using System.Threading;

using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.Logging;

using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public class NetworkTimeService : NetworkLinkService, IDisposable
    {
        private Timer _executeTimer;

        private readonly ILogger _logger;
        private readonly IClockService _clock;
        private readonly IConfiguration _configuration;

        public TimeSpan Time { get; set; }
        public TimeSpan Interval { get; set; }

        public override void Start()
        {
            var enabled = (bool)_configuration.GetValueOrDefault(BoardSettings.NetworkTimeEnabled, true);

            if (enabled)
            {
                base.Start();
            }
        }

        public NetworkTimeService(IServiceProvider services, IClockService clock, IConfiguration configuration, ILoggerFactory loggerFactory)
            : base(services)
        {
            _clock = clock;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger(nameof(NetworkTimeService));

            Time = TimeSpan.Zero;
            Interval = TimeSpan.FromDays(1);
        }

        protected override void LinkConnected(NetworkController sender, NetworkAddressChangedEventArgs args)
        {
            _executeTimer = new Timer(state =>
            {
                SetTime();
            }, null, Time, Interval);
        }

        protected override void LinkDisconnected(NetworkController sender, NetworkAddressChangedEventArgs args)
        {
            if (_executeTimer == null)
            {
                return;
            }

            try
            {
                _executeTimer.Change(Timeout.Infinite, 0);
            }
            finally
            {
                _executeTimer = null;
            }
        }

        public virtual void Dispose()
        {
            _executeTimer?.Dispose();
        }

        private void SetTime()
        {
            var ntpServer = (string)_configuration.GetValueOrDefault(BoardSettings.NetworkTimeServer, "pool.ntp.org");

            try
            {
                var accurateTime = GetNetworkTime(ntpServer);

                _clock.SetTime(accurateTime);
                _logger.Log(LogLevel.Information, $"Realtime clock set to {_clock.Now:MM/dd/yyyy hh:mm:ss tt} UTC from '{ntpServer}'.");
            }
            catch
            {
                _logger.Log(LogLevel.Error, $"Failed getting network time from '{ntpServer}'.");
            }
        }

        private static DateTime GetNetworkTime(string ntpServer)
        {
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

            var networkDateTime = (new DateTime(1900, 1, 1)).
                AddMilliseconds((long)milliseconds);

            return networkDateTime;
        }
    }
}