using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public static class BuiltInEthernetServiceCollectionExtension 
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
