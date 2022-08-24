using System;

using Bytewizer.TinyCLR.Hosting;
using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Devices.Display;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.FocalTech.FT5xx6;

namespace Bytewizer.TinyCLR.Boards
{
    public static class TouchDisplayServiceCollectionExtension
    {
        public static IServiceCollection AddTouchScreen(this IServiceCollection services)
        {
            return services.AddTouchScreen(DisplayOrientation.Degrees0);
        }
    }

    public class TouchScreenService : IDisposable
    {
        private readonly GpioPin _backlightPin;

        public FT5xx6Controller TouchController { get; private set; }
        public DisplayController DisplayController { get; private set; }
        public int Width => DisplayController.ActiveConfiguration.Width;
        public int Height => DisplayController.ActiveConfiguration.Height;

        public TouchScreenService(TouchScreenSettings settings)
        {
            DisplayController = settings.DisplayController;
            TouchController = settings.TouchController;

            _backlightPin = GpioController.GetDefault().OpenPin(settings.BacklighPin);
            _backlightPin.SetDriveMode(GpioPinDriveMode.Output);
            _backlightPin.Write(GpioPinValue.Low);

            Enable();
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
            TouchController?.Dispose();

            _backlightPin?.Dispose();
        }
    }
}