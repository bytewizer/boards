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
            var spiController = SpiController.FromName(FEZBit.SpiBus.Display);
            var gpioController = GpioController.GetDefault();

            var device = ST7735Controller.GetConnectionSettings(
                SpiChipSelectType.Gpio,
                gpioController.OpenPin(FEZBit.GpioPin.DisplayChipselect) // ChipSelect 
            ); 

            var controller = new ST7735Controller(
                spiController.GetDevice(device),
                gpioController.OpenPin(FEZBit.GpioPin.DisplayRs), // Pin RS
                gpioController.OpenPin(FEZBit.GpioPin.DisplayReset) // Pin RESET
            );

            controller.SetDataAccessControl(true, true, false, false); //Rotate the screen.

            return services.AddDisplay(controller, FEZBit.GpioPin.Backlight);
        }
    }
}
