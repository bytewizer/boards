﻿using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Network;

namespace Bytewizer.TinyCLR.Boards
{
    public static class MikroBusWirelessServiceCollectionExtension
    {
        public static IServiceCollection AddWireless(this IServiceCollection services, string ssid, string psk)
        {
            return AddWireless(services, ssid, psk, WiFiMode.Station, MikroBus.One);
        }

        public static IServiceCollection AddWireless(this IServiceCollection services, string ssid, string psk, WiFiMode mode, MikroBus slot)
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
                interruptPin = SC20260.GpioPin.PG6;
                resetPin = SC20260.GpioPin.PI8;
                chipSelectLine = SC20260.GpioPin.PG12;
                enablePin = SC20260.GpioPin.PI0;
            }
            else
            {
                interruptPin = SC20260.GpioPin.PJ13;
                resetPin = SC20260.GpioPin.PI11;
                chipSelectLine = SC20260.GpioPin.PC13;
                enablePin = SC20260.GpioPin.PI5;
            }

            var gpioController = GpioController.GetDefault();

            services.AddWireless(
                SC20260.NetworkController.ATWinc15x0,
                new WiFiNetworkInterfaceSettings()
                {
                    Ssid = ssid,
                    Password = psk,
                    Mode = mode
                },
                new SpiNetworkCommunicationInterfaceSettings()
                {
                    SpiApiName = SC20260.SpiBus.Spi3,
                    GpioApiName = SC20260.GpioPin.Id,
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
