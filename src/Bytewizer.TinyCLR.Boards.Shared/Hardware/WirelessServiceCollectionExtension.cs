using System;

using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public static class WirelessServiceCollectionExtension
    {
        public static IServiceCollection AddWireless(
            this IServiceCollection services,
            string networkController,
            WiFiNetworkInterfaceSettings settings,
            NetworkCommunicationInterfaceSettings interfaceSettings,
            int enablePin)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            if (settings == null)
            {
                throw new ArgumentNullException();
            }

            if (interfaceSettings == null)
            {
                throw new ArgumentNullException();
            }

            var networkSettings = new NetworkSettings()
            {
                EnablePin = enablePin,
                Controller = networkController,
                CommunicationSettings = interfaceSettings,
                InterfaceSettings = settings
            };

            services.Replace(
                new ServiceDescriptor(
                    typeof(IWirelessSettings),
                    networkSettings)
                );

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(WirelessService),
                    typeof(WirelessService), 
                    ServiceLifetime.Singleton)
                );

            services.AddHostedService(typeof(WirelessWatchdog));
            
            return services;
        }
    }

    public class WirelessWatchdog : SchedulerService
    {
        private readonly WirelessService _network;

        public WirelessWatchdog(WirelessService network)
            
            : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
        {
            _network = network;
        }

        protected override void ExecuteAsync()
        {
            if (_network.WirelessLinked)
            {
                return;
            }

            _network.Disable();
            _network.Enable();     
        }
    }

    public class WirelessService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly GpioPin _enablePin;
        private readonly IConfiguration _configuration;

        public NetworkController Controller { get; private set; }

        public bool WirelessLinked { get; private set; }

        public WirelessService(ILoggerFactory loggerFactory, IConfiguration configuration, IWirelessSettings settings)
        {
            _logger = loggerFactory.CreateLogger(nameof(WirelessService));
            _configuration = configuration;

            _enablePin = GpioController.GetDefault().OpenPin(settings.EnablePin);
            _enablePin.SetDriveMode(GpioPinDriveMode.Output);
            _enablePin.Write(GpioPinValue.Low);

            Controller = NetworkController.FromName(settings.Controller);
            Controller.SetCommunicationInterfaceSettings(settings.CommunicationSettings);
            Controller.SetInterfaceSettings(settings.InterfaceSettings);

            var defaultController = configuration[BoardSettings.DefaultController];
            if (defaultController == null)
            {
                Controller.SetAsDefaultController();
                configuration[BoardSettings.DefaultController] = Controller;
            }

            Controller.NetworkLinkConnectedChanged += NetworkLinkConnectedChanged;
            Controller.NetworkAddressChanged += NetworkAddressChanged;

            Enable();
        }

        public void Enable()
        {
            _enablePin.Write(GpioPinValue.High);

            try
            {
                Controller.Enable();
            }
            catch
            {
                _logger.Log(LogLevel.Error, "802.11 wireless failed to connect verify ssid and password.");
            }
        }

        public void Disable()
        {
            try
            {
                Controller.Disable();
            }
            catch
            {
                _logger.Log(LogLevel.Error, "802.11 wireless failed to disable.");
            }
            finally
            {
                _enablePin.Write(GpioPinValue.Low);
            }
        }

        public void Dispose()
        {
            Disable();
            Controller?.Dispose();

            _enablePin?.Dispose();
        }

        private void NetworkLinkConnectedChanged(NetworkController sender, NetworkLinkConnectedChangedEventArgs args)
        {
            if (args.Connected)
            {
                WirelessLinked = true;
                _logger.Log(LogLevel.Information, "802.11 wireless interface connected.");
            }
            else
            {
                WirelessLinked = false;
                _configuration[BoardSettings.WirelessConnected] = false;
                _logger.Log(LogLevel.Information, "802.11 wireless interface disconnected.");
            }
        }

        private void NetworkAddressChanged(NetworkController sender, NetworkAddressChangedEventArgs args)
        {
            var ipProperties = sender.GetIPProperties();
            var address = ipProperties.Address.GetAddressBytes();

            if (address != null && address[0] != 0 && address.Length > 0)
            {
                var info = NetworkHelper.GetNetworkInfo(sender);
                _logger.Log(LogLevel.Information, info);

                var settings = NetworkHelper.GetNetworkSettings(sender);
                _logger.Log(LogLevel.Information, settings);
            }

            if (address[0] != 0)
            {
                _configuration[BoardSettings.WirelessConnected] = true;
            }
        }
    }
}
