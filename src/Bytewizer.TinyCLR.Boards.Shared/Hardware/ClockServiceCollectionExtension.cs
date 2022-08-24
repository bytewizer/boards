using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.Devices.Rtc;

namespace Bytewizer.TinyCLR.Boards
{
    public static class ClockServiceCollectionExtension
    {
        public static IServiceCollection AddClock(this IServiceCollection services)
        {
            return AddClock(services, 0);
        }

        public static IServiceCollection AddClock(this IServiceCollection services, int timezoneOffset)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(ClockService),
                    new ClockService(timezoneOffset)
                ));

            return services;
        }
    }

    public class ClockService : IDisposable
    {
        private readonly int _timeZoneOffset;

        public RtcController Controller { get; private set; }
        
        public DateTime UtcNow { get => Controller.Now; }
        
        public DateTime Now { get => Controller.Now.AddSeconds(_timeZoneOffset); }

        public ClockService(int timezoneOffset)
        {
            _timeZoneOffset = timezoneOffset;

            Controller = RtcController.GetDefault();
            Controller.SetChargeMode(BatteryChargeMode.Fast);

            if (Controller.IsValid)
            {
                SystemTime.SetTime(Now);
            }
        }
        
        public void SetTime(DateTime value)
        {
            Controller.SetTime(RtcDateTime.FromDateTime(value));
            SystemTime.SetTime(value.AddSeconds(_timeZoneOffset));
        }

        public void Dispose()
        {
            Controller?.Dispose();
        }
    }
}