using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public static class IntegratedWirelessServiceCollectionExtension 
    {
        public static IServiceCollection AddWireless(this IServiceCollection services, string ssid, string psk)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrEmpty(ssid))
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrEmpty(psk))
            {
                throw new ArgumentNullException();
            }

            services.AddWireless(
                SC20260.NetworkController.ATWinc15x0,
                new WiFiNetworkInterfaceSettings()
                {
                    Ssid = ssid,
                    Password = psk
                },
                new SpiNetworkCommunicationInterfaceSettings()
                {
                    SpiApiName = FEZPortal.SpiBus.Spi3,
                    GpioApiName = SC20260.GpioPin.Id,
                    InterruptPin = GpioController.GetDefault().OpenPin(FEZPortal.GpioPin.WiFiInterrupt),
                    InterruptEdge = GpioPinEdge.FallingEdge,
                    InterruptDriveMode = GpioPinDriveMode.InputPullUp,
                    ResetPin = GpioController.GetDefault().OpenPin(FEZPortal.GpioPin.WiFiReset),
                    ResetActiveState = GpioPinValue.Low,
                    SpiSettings = new SpiConnectionSettings()
                    {
                        ChipSelectLine = GpioController.GetDefault().OpenPin(FEZPortal.GpioPin.WiFiChipSelect),
                        ClockFrequency = 4000000,
                        Mode = SpiMode.Mode0,
                        ChipSelectType = SpiChipSelectType.Gpio,
                        ChipSelectHoldTime = TimeSpan.FromTicks(10),
                        ChipSelectSetupTime = TimeSpan.FromTicks(10)
                    }
                },
                FEZPortal.GpioPin.WiFiEnable
                );
           
            return services;
        }
    }
}
