using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public static class MikroBusEthernetServiceCollectionExtension
    {
        public static IServiceCollection AddEthernet(this IServiceCollection services)
        {
            return AddEthernet(services, MikroBus.One);
        }

        public static IServiceCollection AddEthernet(this IServiceCollection services, MikroBus slot)
        {
            var mac = new byte[6] { 0x00, 0x8D, 0xB4, 0x49, 0xAD, 0xBD };

            return AddEthernet(services, mac, slot);
        }

        public static IServiceCollection AddEthernet(this IServiceCollection services, byte[] macAddress, MikroBus slot)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            int interruptPin;
            int resetPin;
            int chipSelectLine;
            int enablePin;

            if (slot == MikroBus.One)
            {
                interruptPin = SC20100.GpioPin.PC5;
                resetPin = SC20100.GpioPin.PD4;
                chipSelectLine = SC20100.GpioPin.PD3;
                enablePin = SC20100.GpioPin.PE5;
            }
            else
            {
                interruptPin = SC20100.GpioPin.PA8;
                resetPin = SC20100.GpioPin.PD15;
                chipSelectLine = SC20100.GpioPin.PD14;
                enablePin = SC20100.GpioPin.PA3;
            }

            services.AddEthernet(
                SC20100.NetworkController.Enc28j60,
                new EthernetNetworkInterfaceSettings()
                {
                    MacAddress = macAddress
                },
                new SpiNetworkCommunicationInterfaceSettings()
                {
                    SpiApiName = SC20100.SpiBus.Spi3,
                    GpioApiName = SC20100.GpioPin.Id,
                    InterruptPin = GpioController.GetDefault().OpenPin(interruptPin),
                    InterruptEdge = GpioPinEdge.FallingEdge,
                    InterruptDriveMode = GpioPinDriveMode.InputPullUp,
                    ResetPin = GpioController.GetDefault().OpenPin(resetPin),
                    ResetActiveState = GpioPinValue.Low,
                    SpiSettings = new SpiConnectionSettings()
                    {
                        ChipSelectLine = GpioController.GetDefault().OpenPin(chipSelectLine),
                        ClockFrequency = 4000000,
                        Mode = SpiMode.Mode0,
                        ChipSelectType = SpiChipSelectType.Gpio,
                        ChipSelectHoldTime = TimeSpan.FromTicks(10),
                        ChipSelectSetupTime = TimeSpan.FromTicks(10)
                    }
                },
                enablePin
            );

            return services;
        }
    }
}
