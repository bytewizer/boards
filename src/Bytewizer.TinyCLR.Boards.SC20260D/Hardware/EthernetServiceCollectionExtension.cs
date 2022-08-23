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

            var networkSettings = new NetworkSettings()
            {
                EnablePin = enablePin,
                Controller = networkController,
                CommunicationSettings = interfaceSettings,
                InterfaceSettings = settings
            };

            services.Replace(
                new ServiceDescriptor(
                    typeof(IEthernetSettings),
                    networkSettings)
                );

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(EthernetService), 
                    typeof(EthernetService), 
                    ServiceLifetime.Singleton)
                );

            return services;
        }
    }

    public class EthernetService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly GpioPin _enablePin;
        private readonly IConfiguration _configuration;

        public NetworkController Controller { get; private set; }

        public bool EthernetLinked { get; private set; }

        public EthernetService(IConfiguration configuration, IEthernetSettings settings)
            : this(NullLoggerFactory.Instance, configuration, settings)
        { }

        public EthernetService(
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IEthernetSettings settings)
        {
            _logger = loggerFactory.CreateLogger(nameof(EthernetService));
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
                EthernetLinked = true;
                _logger.Log(LogLevel.Information, "Ethernet interface connected.");
            }
            else
            {
                EthernetLinked = false;
                _configuration[BoardSettings.EthernetConnected] = false;
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

            if (address[0] != 0)
            {
                _configuration[BoardSettings.EthernetConnected] = true;
            }
        }
    }
}
