using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;


namespace Bytewizer.TinyCLR.Boards
{
    public static class IntegratedDisplayServiceCollectionExtension 
    {
        public static IServiceCollection AddDisplay(this IServiceCollection services)
        {
            return AddDisplay(services, true, true, false, false);
        }

        public static IServiceCollection AddDisplay(this IServiceCollection services, bool swapRowColumn, bool invertRow, bool invertColumn, bool useBgrPanel)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            var spiController = SpiController.FromName(FEZBit.SpiBus.Display);
            var gpioController = GpioController.GetDefault();

            var device = ST7735Controller.GetConnectionSettings(
                SpiChipSelectType.Gpio,
                gpioController.OpenPin(FEZBit.GpioPin.DisplayChipselect)
            ); 

            var controller = new ST7735Controller(
                spiController.GetDevice(device),
                gpioController.OpenPin(FEZBit.GpioPin.DisplayRs),
                gpioController.OpenPin(FEZBit.GpioPin.DisplayReset)
            );

            controller.SetDataAccessControl(swapRowColumn, invertRow, invertColumn, useBgrPanel);

            return services.AddDisplay(controller, FEZBit.GpioPin.Backlight);
        }
    }
}
