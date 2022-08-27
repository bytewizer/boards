using Bytewizer.TinyCLR.Hosting;
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
            var spiController = SpiController.FromName(SC20100.SpiBus.Spi3);
            var gpioController = GpioController.GetDefault();

            var device = ST7735Controller.GetConnectionSettings(
                SpiChipSelectType.Gpio,
                gpioController.OpenPin(SC20100.GpioPin.PD10) 
            ); 

            var controller = new ST7735Controller(
                spiController.GetDevice(device),
                gpioController.OpenPin(SC20100.GpioPin.PC4), 
                gpioController.OpenPin(SC20100.GpioPin.PE15) 
            );

            controller.SetDataAccessControl(true, true, false, false); 
            
            var settings = new DisplaySettings()
            {
                Controller = controller,
                BacklightPin = SC20100.GpioPin.PE5 // this has a conflict with mikrobus slot1
            };

            services.Replace(
                new ServiceDescriptor(
                    typeof(DisplaySettings),
                    settings
                ));

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(IDisplayService),
                    typeof(DisplayService),
                    ServiceLifetime.Singleton
                ));

            return services;
        }
    }
}
