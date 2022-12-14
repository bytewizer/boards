using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public static class IntegratedEthernetServiceCollectionExtension
    {
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
