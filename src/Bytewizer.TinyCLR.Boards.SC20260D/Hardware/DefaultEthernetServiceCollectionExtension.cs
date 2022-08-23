using System;
using System.Collections;
using System.Text;
using System.Threading;

using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.DependencyInjection;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Pins;

namespace Bytewizer.TinyCLR.Boards
{
    public static class DefaultEthernetServiceCollectionExtension 
    {
        public static IServiceCollection AddEthernet(this IServiceCollection services)
        {
            var mac = new byte[6] { 0x00, 0x8D, 0xB4, 0x49, 0xAD, 0xBD };

            return AddEthernet(services, mac);
        }
        public static IServiceCollection AddEthernet(this IServiceCollection services, byte[] macAddress)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            services.AddEthernet(
                    SC20260.NetworkController.EthernetEmac,
                    new EthernetNetworkInterfaceSettings()
                    {
                        MacAddress = macAddress
                    },
                    new BuiltInNetworkCommunicationInterfaceSettings(),
                    SC20260.GpioPin.PG3
                );

            return services;
        }
    }
}
