using System;
using System.Drawing;
using Bytewizer.TinyCLR.DependencyInjection;
using Bytewizer.TinyCLR.Hosting;

using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;

namespace Bytewizer.TinyCLR.Boards
{
    public static class DisplayServiceCollectionExtension
    {
        public static IServiceCollection AddDisplay(this IServiceCollection services, ST7735Controller controller, int backlightPin)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            if (controller == null)
            {
                throw new ArgumentNullException();
            }

            var settings = new DisplaySettings()
            {
                Controller = controller,
                BacklightPin = backlightPin // this has a conflict with mikrobus slot1
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

    public interface IDisplayService : IDisposable
    {
        ST7735Controller DisplayController { get; }
        int Width { get; }
        int Height { get; }
        void Enable();
        void Disable();
    }

    public class DisplayService : IDisplayService
    {
        private readonly GpioPin _backlightPin;

        public ST7735Controller DisplayController { get; private set; }
        public int Width { get => DisplayController.Width; }
        public int Height { get => DisplayController.Height; }

        public DisplayService(DisplaySettings settings)
        {
            _backlightPin = GpioController.GetDefault().OpenPin(settings.BacklightPin);
            _backlightPin.SetDriveMode(GpioPinDriveMode.Output);
            _backlightPin.Write(GpioPinValue.Low);

            DisplayController = settings.Controller;
            DisplayController.SetDrawWindow(0, 0, Width, Height);

            Graphics.OnFlushEvent += (sender, data, x, y, width, heigh, originalWidth) =>
            {
                if (DisplayController != null)
                {
                    DisplayController.DrawBuffer(data);
                }
            };
        }

        public void Enable()
        {
            _backlightPin.Write(GpioPinValue.High);
            DisplayController.Enable();
        }

        public void Disable()
        {
            _backlightPin.Write(GpioPinValue.Low);
            DisplayController.Disable();
        }

        public void Dispose()
        {
            Disable();
            DisplayController?.Dispose();

            _backlightPin?.Dispose();
        }
    }
}