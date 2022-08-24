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
            return AddWireless(services, ssid, psk, WiFiMode.Station);
        }

        public static IServiceCollection AddWireless(this IServiceCollection services, string ssid, string psk, WiFiMode mode)
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
                SC20100.NetworkController.ATWinc15x0,
                new WiFiNetworkInterfaceSettings()
                {
                    Ssid = ssid,
                    Password = psk,
                    Mode = mode
                },
                new SpiNetworkCommunicationInterfaceSettings()
                {
                    SpiApiName = FEZDuino.SpiBus.WiFi,
                    GpioApiName = SC20100.GpioPin.Id,
                    InterruptPin = GpioController.GetDefault().OpenPin(FEZDuino.GpioPin.WiFiInterrupt),
                    InterruptEdge = GpioPinEdge.FallingEdge,
                    InterruptDriveMode = GpioPinDriveMode.InputPullUp,
                    ResetPin = GpioController.GetDefault().OpenPin(FEZDuino.GpioPin.WiFiReset),
                    ResetActiveState = GpioPinValue.Low,
                    SpiSettings = new SpiConnectionSettings()
                    {
                        ChipSelectLine = GpioController.GetDefault().OpenPin(FEZDuino.GpioPin.WiFiChipselect),
                        ClockFrequency = 4000000,
                        Mode = SpiMode.Mode0,
                        ChipSelectType = SpiChipSelectType.Gpio,
                        ChipSelectHoldTime = TimeSpan.FromTicks(10),
                        ChipSelectSetupTime = TimeSpan.FromTicks(10)
                    }
                },
                FEZDuino.GpioPin.WiFiEnable
                );

            return services;
        }
    }
}
