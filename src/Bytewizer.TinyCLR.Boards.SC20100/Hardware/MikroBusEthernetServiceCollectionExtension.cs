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
        public static IServiceCollection AddEthernet(this IServiceCollection services, byte[] macAddress)
        {
            return AddEthernet(services, macAddress, MikroBus.One);
        }

        public static IServiceCollection AddEthernet(this IServiceCollection services, byte[] macAddress, MikroBus slot)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            if (macAddress.Length != 6)
            {
                throw new ArgumentException();
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
                enablePin = SC20100.GpioPin.PE5;  // this has a conflict
            }
            else
            {
                interruptPin = SC20100.GpioPin.PA8;
                resetPin = SC20100.GpioPin.PD15;
                chipSelectLine = SC20100.GpioPin.PD14;
                enablePin = SC20100.GpioPin.PA3;
            }
            var gpioController = GpioController.GetDefault();

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
                    InterruptPin = gpioController.OpenPin(interruptPin),
                    InterruptEdge = GpioPinEdge.FallingEdge,
                    InterruptDriveMode = GpioPinDriveMode.InputPullUp,
                    ResetPin = gpioController.OpenPin(resetPin),
                    ResetActiveState = GpioPinValue.Low,
                    SpiSettings = new SpiConnectionSettings()
                    {
                        ChipSelectLine = gpioController.OpenPin(chipSelectLine),
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
