using System;

using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public static class EthernetServiceCollectionExtension
    {
        public static IServiceCollection AddEthernet(
            this IServiceCollection services,
            string networkController,
            EthernetNetworkInterfaceSettings settings,
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

            var networkSettings = new EthernetSettings()
            {
                EnablePin = enablePin,
                Controller = networkController,
                CommunicationSettings = interfaceSettings,
                InterfaceSettings = settings
            };

            services.Add(
                new ServiceDescriptor(
                    typeof(EthernetSettings),
                    networkSettings
                ));

            services.Add(
                new ServiceDescriptor(
                    typeof(INetworkService), 
                    typeof(EthernetService), 
                    ServiceLifetime.Singleton
                ));

            services.AddHostedService(typeof(NetworkTimeService));

            return services;
        }
    }

    public class EthernetService : INetworkService
    {
        private readonly ILogger _logger;
        private readonly GpioPin _enablePin;
        private readonly IConfiguration _configuration;

        public NetworkController Controller { get; private set; }

        public bool LinkConnected { get => Controller.GetLinkConnected(); }

        public EthernetService(
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            EthernetSettings settings)
        {
            _logger = loggerFactory.CreateLogger(nameof(EthernetService));
            _configuration = configuration;

            _enablePin = GpioController.GetDefault().OpenPin(settings.EnablePin);
            _enablePin.SetDriveMode(GpioPinDriveMode.Output);
            _enablePin.Write(GpioPinValue.Low);

            if (settings.InterfaceSettings.DhcpEnable == true)
            {
                settings.InterfaceSettings.DhcpEnable = (bool)configuration.GetValueOrDefault(
                        EthernetSettings.DhcpEnable, true
                    );
            }

            if (settings.InterfaceSettings.DynamicDnsEnable == true)
            {
                settings.InterfaceSettings.DynamicDnsEnable = (bool)configuration.GetValueOrDefault(
                        EthernetSettings.DdnsEnable, true
                    );
            }

            if (settings.InterfaceSettings.MulticastDnsEnable == false)
            {
                settings.InterfaceSettings.MulticastDnsEnable = (bool)configuration.GetValueOrDefault(
                        EthernetSettings.DdnsEnable, false
                    );
            }

            settings.InterfaceSettings.Address ??=
                configuration.GetIpAddress(EthernetSettings.IpAddress);
            settings.InterfaceSettings.MacAddress ??=
                configuration.GetMacAddress(EthernetSettings.MacAddress);
            settings.InterfaceSettings.GatewayAddress ??=
                configuration.GetIpAddress(EthernetSettings.GatewayAddress);
            settings.InterfaceSettings.SubnetMask ??=
                configuration.GetIpAddress(EthernetSettings.SubnetMask);
            settings.InterfaceSettings.DnsAddresses ??=
                configuration.GetDnsAddresses(EthernetSettings.DnsAddresses);

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
            try
            {
                _enablePin.Write(GpioPinValue.High);
                Controller.Enable();
            }
            catch
            {
                _logger.Log(LogLevel.Error, "Ethernet interface failed to connect.");
            }
        }

        public void Disable()
        {
            try
            {
                Controller.Disable();
                _enablePin.Write(GpioPinValue.Low);
            }
            catch
            {
                _logger.Log(LogLevel.Error, "Failed to disable ethernet.");
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
                _logger.Log(LogLevel.Information, "Ethernet interface connected.");
            }
            else
            {
                _logger.Log(LogLevel.Information, "Ethernet interface disconnected.");
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
        }
    }
}
