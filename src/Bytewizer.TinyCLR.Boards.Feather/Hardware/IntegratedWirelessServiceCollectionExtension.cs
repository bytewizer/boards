using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards.SC20260D
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

            var gpioController = GpioController.GetDefault();

            services.AddWireless(
                SC20100.NetworkController.ATWinc15x0,
                new WiFiNetworkInterfaceSettings()
                {
                    Ssid = ssid,
                    Password = psk
                },
                new SpiNetworkCommunicationInterfaceSettings()
                {
                    SpiApiName = FEZFeather.SpiBus.WiFi,
                    GpioApiName = SC20100.GpioPin.Id,
                    InterruptPin = gpioController.OpenPin(FEZFeather.GpioPin.WiFiInterrupt),
                    InterruptEdge = GpioPinEdge.FallingEdge,
                    InterruptDriveMode = GpioPinDriveMode.InputPullUp,
                    ResetPin = gpioController.OpenPin(FEZFeather.GpioPin.WiFiReset),
                    ResetActiveState = GpioPinValue.Low,
                    SpiSettings = new SpiConnectionSettings()
                    {
                        ChipSelectLine = gpioController.OpenPin(FEZFeather.GpioPin.WiFiChipselect),
                        ClockFrequency = 4000000,
                        Mode = SpiMode.Mode0,
                        ChipSelectType = SpiChipSelectType.Gpio,
                        ChipSelectHoldTime = TimeSpan.FromTicks(10),
                        ChipSelectSetupTime = TimeSpan.FromTicks(10)
                    }
                },
                FEZFeather.GpioPin.WiFiEnable
                );
           
            return services;
        }
    }
}
