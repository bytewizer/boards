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
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            return services.AddTouchScreen(DisplayOrientation.Degrees0);
        }

        public static IServiceCollection AddTouchScreen(
            this IServiceCollection services, 
            DisplayController displayController, 
            FT5xx6Controller tocuhController,
            int backlightPin)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            if (displayController == null)
            {
                throw new ArgumentNullException();
            }

            if (tocuhController == null)
            {
                throw new ArgumentNullException();
            }

            var settings = new TouchScreenSettings()
            {
                DisplayController = displayController,
                TouchController = tocuhController,
                BacklightPin = backlightPin
            };

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(TouchScreenSettings),
                    settings
                ));

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(ITouchScreenService),
                    typeof(TouchScreenService),
                    ServiceLifetime.Singleton
                ));

            return services;
        }
    }

    public interface ITouchScreenService : IDisposable
    {
        FT5xx6Controller TouchController { get; }
        DisplayController DisplayController { get; }
        int Width { get; }
        int Height { get; }
        void Enable();
        void Disable();
    }

    public class TouchScreenService : ITouchScreenService
    {
        private readonly GpioPin _backlightPin;

        public FT5xx6Controller TouchController { get; private set; }
        public DisplayController DisplayController { get; private set; }
        public int Width => DisplayController.ActiveConfiguration.Width;
        public int Height => DisplayController.ActiveConfiguration.Height;

        public TouchScreenService(TouchScreenSettings settings)
        {
            _backlightPin = GpioController.GetDefault().OpenPin(settings.BacklightPin);
            _backlightPin.SetDriveMode(GpioPinDriveMode.Output);
            _backlightPin.Write(GpioPinValue.Low);

            DisplayController = settings.DisplayController;
            TouchController = settings.TouchController;
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