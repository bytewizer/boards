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

            var networkSettings = new WirelessSettings()
            {
                EnablePin = enablePin,
                Controller = networkController,
                CommunicationSettings = interfaceSettings,
                InterfaceSettings = settings
            };

            services.Replace(
                new ServiceDescriptor(
                    typeof(WirelessSettings),
                    networkSettings)
                );

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(IWirelessService),
                    typeof(WirelessService), 
                    ServiceLifetime.Singleton)
                );

            services.AddHostedService(typeof(WirelessWatchdog));
            
            return services;
        }
    }

    public class WirelessWatchdog : SchedulerService
    {
        private readonly IWirelessService _network;

        public WirelessWatchdog(IWirelessService network)       
            : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
        {
            _network = network;
        }

        protected override void ExecuteAsync()
        {
            if (_network.LinkConnected)
            {
                return;
            }

            _network.Disable();
            _network.Enable();     
        }
    }

    public class WirelessService : IWirelessService
    {
        private readonly ILogger _logger;
        private readonly GpioPin _enablePin;
        private readonly IConfiguration _configuration;

        public NetworkController Controller { get; private set; }

        public bool LinkConnected { get => Controller.GetLinkConnected(); }

        public WirelessService(ILoggerFactory loggerFactory, IConfiguration configuration, WirelessSettings settings)
        {
            _logger = loggerFactory.CreateLogger(nameof(WirelessService));
            _configuration = configuration;

            _enablePin = GpioController.GetDefault().OpenPin(settings.EnablePin);
            _enablePin.SetDriveMode(GpioPinDriveMode.Output);
            _enablePin.Write(GpioPinValue.Low);

            var interfaceSettings =  settings.InterfaceSettings as WiFiNetworkInterfaceSettings;

            if (interfaceSettings.DhcpEnable == true)
            {
                interfaceSettings.DhcpEnable = (bool)configuration.GetValueOrDefault(
                        WirelessSettings.DhcpEnable, true
                    );
            }

            if (interfaceSettings.DynamicDnsEnable == true)
            {
                interfaceSettings.DynamicDnsEnable = (bool)configuration.GetValueOrDefault(
                        WirelessSettings.DdnsEnable, true
                    );
            }

            if (interfaceSettings.MulticastDnsEnable == false)
            {
                interfaceSettings.MulticastDnsEnable = (bool)configuration.GetValueOrDefault(
                        WirelessSettings.DdnsEnable, false
                    );
            }

            if (interfaceSettings.Channel == 1)
            {
                interfaceSettings.Channel = (uint)configuration.GetValueOrDefault(
                        WirelessSettings.Channel, 1u
                    );
            }

            interfaceSettings.Ssid ??=
                configuration.GetValue(WirelessSettings.Ssid);
            interfaceSettings.Password ??=
                configuration.GetValue(WirelessSettings.Psk);
            interfaceSettings.Address ??=
                configuration.GetIpAddress(WirelessSettings.IpAddress);
            interfaceSettings.MacAddress ??=
                configuration.GetMacAddress(WirelessSettings.MacAddress);
            interfaceSettings.GatewayAddress ??=
                configuration.GetIpAddress(WirelessSettings.GatewayAddress);
            interfaceSettings.SubnetMask ??=
                configuration.GetIpAddress(WirelessSettings.SubnetMask);
            interfaceSettings.DnsAddresses ??=
                configuration.GetDnsAddresses(WirelessSettings.DnsAddresses);

            Controller = NetworkController.FromName(settings.Controller);
            Controller.SetCommunicationInterfaceSettings(settings.CommunicationSettings);
            Controller.SetInterfaceSettings(settings.InterfaceSettings);

            var defaultController = configuration[BoardSettings.ControllerDefault];
            if (defaultController == null)
            {
                Controller.SetAsDefaultController();
                configuration[BoardSettings.ControllerDefault] = Controller;
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
                _logger.Log(LogLevel.Information, "802.11 wireless interface connected.");
            }
            else
            {
                _configuration[BoardSettings.NetworkConnected] = false;
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
                _configuration[BoardSettings.NetworkConnected] = true;
            }
        }
    }
}
