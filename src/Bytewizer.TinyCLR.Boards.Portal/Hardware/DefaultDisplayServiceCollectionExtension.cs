using System;
using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6;

using static GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6.FT5xx6Controller;

namespace Bytewizer.TinyCLR.Boards
{
    public static class DefaultDisplayServiceCollectionExtension
    {
        public static IServiceCollection AddTouchScreen(this IServiceCollection services, DisplayOrientation orientation)
        {
            TouchOrientation touchOrientation = TouchOrientation.Degrees0;

            switch (orientation)
            {
                case DisplayOrientation.Degrees90:
                    touchOrientation = TouchOrientation.Degrees90;
                    break;
                case DisplayOrientation.Degrees180:
                    touchOrientation = TouchOrientation.Degrees180;
                    break;
                case DisplayOrientation.Degrees270:
                    touchOrientation = TouchOrientation.Degrees270;
                    break;
            }

            var displayController = DisplayController.GetDefault();

            displayController.SetConfiguration(new ParallelDisplayControllerSettings
            {
                Width = 480,
                Height = 272,
                DataFormat = DisplayDataFormat.Rgb565,
                Orientation = orientation,
                PixelClockRate = 10000000,
                PixelPolarity = false,
                DataEnablePolarity = false,
                DataEnableIsFixed = false,
                HorizontalFrontPorch = 2,
                HorizontalBackPorch = 2,
                HorizontalSyncPulseWidth = 41,
                HorizontalSyncPolarity = false,
                VerticalFrontPorch = 2,
                VerticalBackPorch = 2,
                VerticalSyncPulseWidth = 10,
                VerticalSyncPolarity = false,
            });

            var i2cController = I2cController.FromName(FEZPortal.I2cBus.I2c1);

            var I2cSettings = new I2cConnectionSettings(0x38)
            {
                BusSpeed = 100000,
                AddressFormat = I2cAddressFormat.SevenBit,
            };

            var i2cDevice = i2cController.GetDevice(I2cSettings);

            var interrupt = GpioController.GetDefault().OpenPin(FEZPortal.GpioPin.TouchInterrupt);

            var touchController = new FT5xx6Controller(i2cDevice, interrupt)
            {
                Width = 480,
                Height = 272,
                Orientation = touchOrientation
            };

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(TouchScreenSettings),
                    new TouchScreenSettings(displayController, touchController, FEZPortal.GpioPin.Backlight)
                ));

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(TouchScreenService),
                    typeof(TouchScreenService),
                    ServiceLifetime.Singleton
                ));

           return services;
        } 
    }
}
